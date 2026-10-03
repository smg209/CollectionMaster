"""Daily TCGplayer price refresh from TCGCSV (https://tcgcsv.com - free, no key, updated once a day ~20:00 UTC).

For every Pokemon group (set) on TCGplayer it reads the product list and the price list, then:
  - updates the TCGplayer price of every version (CollectibleVariant) that carries that TCGplayer product id,
    matching TCGCSV's subTypeName ("Holofoil", "Reverse Holofoil", "1st Edition Holofoil", ...) to the version;
  - for a card product whose id the catalog does not know yet, finds the card by set + card number + name,
    records the product id on the right version (creating the version when TCGplayer sells a printing the
    catalog source did not list) and prices it;
  - adds sealed products (booster boxes, tins, ...) as Collectible rows with Category = 'Sealed' in the set the
    group's cards belong to, with their picture and price;
  - (English category only) gives a TCGplayer group that is no catalog set a set of its own, in the series
    "Other products": a group the catalog source does not have at all gets its cards and sealed products from
    TCGplayer's own data; a mixed group ("Miscellaneous Cards & Products", "World Championship Decks" - its
    cards belong to many sets) gets its sealed products only.

Two kinds of TCGplayer price, kept as two data sources so they are never confused:
  TCGplayer            the MARKET price - what copies recently sold for. The headline figure.
  TCGplayer listings   only when there is NO market price (too few sales): Price is the LOWEST asking price,
                       LowPrice/HighPrice the range of asking prices. An asking price is not a value.

Price history: a CollectiblePriceHistory row is added only when the price changed; otherwise the newest row's
LastConfirmed moves forward. Run once a day and the history builds itself.

TCGCSV rules followed: custom User-Agent, 100 ms between requests, well under 10,000 requests a day (about 450).

Usage:  python refresh_tcgplayer.py --cache <folder> [--no-sealed]
"""
import sys, json, re, time, argparse, unicodedata, urllib.request, urllib.error
from datetime import datetime
from pathlib import Path
from collections import Counter, defaultdict
import pyodbc

ap = argparse.ArgumentParser()
ap.add_argument("--cache", required=True)
ap.add_argument("--no-sealed", action="store_true")
ap.add_argument("--fix-mid", action="store_true", help="one-off: remove TCGplayer 'market' rows that an earlier version filled from the mid asking price")
ap.add_argument("--show-misses", action="store_true")
ap.add_argument("--category", type=int, default=3, help="TCGplayer category: 3 = Pokemon, 85 = Pokemon Japan")
ap.add_argument("--only", default="", help="only groups whose name contains this text")
ARGS = ap.parse_args()

CONN = r"Driver={ODBC Driver 18 for SQL Server};Server=localhost\SQL2025;Database=CollectionMaster;Trusted_Connection=yes;TrustServerCertificate=yes;"
UA = "CollectionMaster-dev/0.1 (personal collection tracker)"
CATEGORY = ARGS.category             # 3 = Pokemon (English and other western releases), 85 = Pokemon Japan
LANGUAGE_CODE = {3: "en", 85: "ja"}.get(CATEGORY, "en")
CACHE = Path(ARGS.cache)
CACHE.mkdir(parents=True, exist_ok=True)
stats = Counter()


def get(url):
    for attempt in range(3):
        try:
            req = urllib.request.Request(url, headers={"User-Agent": UA, "Accept": "application/json"})
            with urllib.request.urlopen(req, timeout=60) as r:
                return r.status, r.read().decode("utf-8", "replace")
        except urllib.error.HTTPError as e:
            if e.code in (403, 404):
                return e.code, e.read().decode("utf-8", "replace")[:500]
            time.sleep(2 * (attempt + 1))
        except Exception:
            time.sleep(2 * (attempt + 1))
    return None, ""


def get_json(url, cache_name):
    p = CACHE / cache_name
    if p.exists():
        return json.loads(p.read_text(encoding="utf-8"))
    time.sleep(0.12)
    st, body = get(url)
    if st != 200:
        return None
    d = json.loads(body).get("results", [])
    p.write_text(json.dumps(d, ensure_ascii=False), encoding="utf-8")
    return d


def money(v):
    try:
        v = round(float(v), 2)
        return v if v > 0 else None
    except Exception:
        return None


def split_subtype(name):
    """TCGCSV subTypeName -> (printing name, is 1st edition)."""
    n = name or "Normal"
    first = n.startswith("1st Edition")
    for prefix in ("1st Edition", "Unlimited"):
        if n.startswith(prefix):
            n = n[len(prefix):].strip() or "Normal"
    return n, first


def norm(s):
    """Lower-case letters and digits only, accents removed: 'Pok\u00e9mon Breeder' and 'Pokemon Breeder' compare equal."""
    s = unicodedata.normalize("NFKD", s or "").encode("ascii", "ignore").decode()
    return re.sub(r"[^a-z0-9]", "", s.lower())


def numkey(s):
    """Card number as a comparison key: '004/102' -> '4', 'RC01/RC32' -> 'RC1', 'XY01' -> 'XY1', '055a' -> '55A'.
    Sources disagree on leading zeros, in front of the number and after a letter prefix."""
    n = str(s or "").split("/")[0].strip().upper()
    m = re.match(r"^([A-Z]*?)0*(\d.*)$", n)
    return (m.group(1) + m.group(2)) if m else (n or "0")


def clean_product_name(name):
    """'Charizard ex - 199/165' / 'Pikachu (Cosmos Holo)' -> ('Charizard ex', None) / ('Pikachu', 'Cosmos Holo')."""
    n = re.sub(r"\s+-\s+[A-Za-z0-9]+/[A-Za-z0-9]+\s*$", "", name or "")
    extra = re.findall(r"\(([^)]*)\)", n)
    n = re.sub(r"\s*\([^)]*\)", "", n).strip()
    return n, (", ".join(extra)[:100] if extra else None)


GROUP_SUFFIXES = (" Trainer Gallery", " Galarian Gallery", " Classic Collection", " Base Set")
# TCGplayer group name -> the catalog's set name, where no rule gets there
GROUP_ALIASES = {
    "SM Base Set": "Sun & Moon",
    "XY Promos": "XY Black Star Promos",
    "Black and White Promos": "BW Black Star Promos",
    "Diamond and Pearl Promos": "DP Black Star Promos",
    "ME: Mega Evolution Promo": "MEP Black Star Promos",
}


def group_name_candidates(name):
    """TCGplayer group names carry a series prefix ('SV01: Scarlet & Violet Base Set', 'SM - Lost Thunder') and
    sometimes name a subset of a set ('Hidden Fates: Shiny Vault'). Most specific candidate first."""
    out = [GROUP_ALIASES[name]] if name in GROUP_ALIASES else []
    out.append(name)
    for sep in (": ", " - "):
        if sep in name:
            head, tail = name.split(sep, 1)
            out += [tail, head]
    for c in list(out):
        for suffix in GROUP_SUFFIXES:
            if c.endswith(suffix) and len(c) > len(suffix):
                out.append(c[: -len(suffix)])
    return out


# ----------------------------------------------------------------------------- source
st, stamp = get("https://tcgcsv.com/last-updated.txt")
try:
    AS_OF = datetime.fromisoformat(stamp.strip().replace("+0000", "+00:00")).astimezone().replace(tzinfo=None, microsecond=0)
except Exception:
    AS_OF = datetime.now().replace(microsecond=0)
day = AS_OF.strftime("%Y%m%d")
print("TCGCSV last updated (local time):", AS_OF)
groups = get_json(f"https://tcgcsv.com/tcgplayer/{CATEGORY}/groups", f"groups_{CATEGORY}_{day}.json") or []
if ARGS.only:
    groups = [g for g in groups if ARGS.only.lower() in g["name"].lower()]
print("groups:", len(groups))

# ----------------------------------------------------------------------------- database state
cn = pyodbc.connect(CONN, autocommit=False)
cur = cn.cursor()


def scalar(sql, *a):
    r = cur.execute(sql, *a).fetchone()
    return r[0] if r else None


def insert(sql, *a):
    cur.execute("SET NOCOUNT ON; " + sql + "; SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", *a)
    return int(cur.fetchone()[0])


SRC = {r.Name: r.Oid for r in cur.execute("SELECT Oid, Name FROM DataSource")}
if "TCGCSV" not in SRC:
    SRC["TCGCSV"] = insert("INSERT INTO DataSource (Name, Url) VALUES (?, ?)", "TCGCSV", "https://tcgcsv.com")
if "TCGplayer listings" not in SRC:
    SRC["TCGplayer listings"] = insert("INSERT INTO DataSource (Name, Url) VALUES (?, ?)", "TCGplayer listings", "https://www.tcgplayer.com")
PRINT = {r.Name: r.Oid for r in cur.execute("SELECT Oid, Name FROM Printing")}
EN = scalar("SELECT Oid FROM [Language] WHERE Code = ?", LANGUAGE_CODE)     # the language of versions and sealed products this run creates
ENGLISH = scalar("SELECT Oid FROM [Language] WHERE Code = 'en'")            # TCGplayer's product and group names are always English
FOREIGN = LANGUAGE_CODE != "en"

# TCGplayer product id -> the versions that carry it
variants = defaultdict(list)
for r in cur.execute("""SELECT e.ExternalId, v.Oid, pr.Name, v.Stamp, v.Subtype, v.CollectibleOid, c.CardSetOid, v.LanguageOid
                        FROM CollectibleVariantExternalId e
                        JOIN CollectibleVariant v ON v.Oid = e.CollectibleVariantOid
                        JOIN Printing pr ON pr.Oid = v.PrintingOid
                        JOIN Collectible c ON c.Oid = v.CollectibleOid
                        WHERE e.DataSourceOid = ?""", SRC["TCGplayer"]):
    variants[r[0]].append({"oid": r[1], "printing": r[2], "stamp": r[3], "subtype": r[4], "card": r[5], "set": r[6], "lang": r[7]})
# current TCGplayer price row per version
price_rows = {(r[0], r[3]): (r[1], float(r[2])) for r in cur.execute(
    "SELECT CollectibleVariantOid, Oid, Price, DataSourceOid FROM CollectiblePrice WHERE DataSourceOid IN (?, ?) AND IsGraded = 0 AND ConditionOid IS NULL",
    SRC["TCGplayer"], SRC["TCGplayer listings"])}
print("known TCGplayer product ids:", len(variants), "| existing TCGplayer prices:", len(price_rows))

# sets by name and code, for groups whose products the catalog does not know
set_by_name, set_by_code = {}, {}
for r in cur.execute("""SELECT s.Oid, s.Name, s.Code, (SELECT TOP 1 n.Name FROM CardSetName n JOIN [Language] nl ON nl.Oid = n.LanguageOid WHERE n.CardSetOid = s.Oid AND nl.Code = 'en')
                        FROM CardSet s JOIN [Language] l ON l.Oid = s.LanguageOid_Primary
                        WHERE l.Code = ? AND ISNULL(s.Series, '') <> 'Other products'""", LANGUAGE_CODE):   # real catalog sets only
    set_by_name.setdefault(norm(r[1]), r[0])
    if r[3]:
        set_by_name.setdefault(norm(r[3]), r[0])
    if r[2]:
        set_by_code.setdefault(r[2].upper(), []).append(r[0])
# cards of each set by card number, and every card's versions
cards_by_number = defaultdict(lambda: defaultdict(list))        # set oid -> number key -> [(card oid, normalised name)]
for r in cur.execute("SELECT Oid, CardSetOid, CardNumber, Name FROM Collectible WHERE ISNULL(Category, '') <> 'Sealed'"):
    cards_by_number[r[1]][numkey(r[2])].append((r[0], norm(r[3])))
# cards that have no picture from any source get TCGplayer's product picture
has_image = {r[0] for r in cur.execute("SELECT DISTINCT CollectibleOid FROM CollectibleImage")}


def picture(card_oid, pr):
    if card_oid in has_image or not pr.get("imageUrl"):
        return
    cur.execute("INSERT INTO CollectibleImage (CollectibleOid, LanguageOid, DataSourceOid, ImageUrl, ThumbnailUrl) VALUES (?, ?, ?, ?, ?)",
                card_oid, EN, SRC["TCGplayer"], pr["imageUrl"].replace("_200w.", "_in_1000x1000."), pr["imageUrl"])
    has_image.add(card_oid)
    stats["pictures added from TCGplayer (card had none)"] += 1


card_variants = defaultdict(list)
for r in cur.execute("""SELECT v.Oid, pr.Name, v.Stamp, v.Subtype, v.CollectibleOid, c.CardSetOid, v.LanguageOid
                        FROM CollectibleVariant v JOIN Printing pr ON pr.Oid = v.PrintingOid JOIN Collectible c ON c.Oid = v.CollectibleOid"""):
    card_variants[r[4]].append({"oid": r[0], "printing": r[1], "stamp": r[2], "subtype": r[3], "card": r[4], "set": r[5], "lang": r[6]})


def find_set(group):
    for cand in group_name_candidates(group["name"]):
        oid = set_by_name.get(norm(cand))
        if oid is not None:
            return oid
    # 'SV6a: Night Wanderer' carries the set code in front; TCGplayer also has an abbreviation of its own
    codes = [(group.get("abbreviation") or "").upper()]
    if ": " in group["name"]:
        codes.insert(0, group["name"].split(": ", 1)[0].strip().upper())
    for code in codes:
        hits = set_by_code.get(code, [])
        if len(hits) == 1:
            return hits[0]
    return None


def english_name(table, fk, oid, name):
    """Records TCGplayer's English name for a set or card of another language, once."""
    if name and scalar(f"SELECT Oid FROM {table} WHERE {fk} = ? AND LanguageOid = ?", oid, ENGLISH) is None:
        cur.execute(f"INSERT INTO {table} ({fk}, LanguageOid, Name) VALUES (?, ?, ?)", oid, ENGLISH, name[:200])
        stats[f"English names added ({table})"] += 1


GAME = scalar("SELECT Oid FROM Game WHERE Name = 'Pokemon'")


def group_set(group):
    """The catalog set that stands for a TCGplayer group which is no set of the catalog source. Created on first use."""
    gid = str(group["groupId"])
    mapped = scalar("SELECT CardSetOid FROM CardSetExternalId WHERE DataSourceOid = ? AND ExternalId = ?", SRC["TCGplayer"], gid)
    if mapped is not None and scalar("SELECT COUNT(*) FROM CardSetExternalId WHERE CardSetOid = ? AND DataSourceOid = ?", mapped, SRC["TCGdex"]) == 0:
        return mapped
    name = group["name"][:190]
    if scalar("SELECT Oid FROM CardSet WHERE GameOid = ? AND Name = ?", GAME, name) is not None:
        name += " (TCGplayer)"
    oid = insert("INSERT INTO CardSet (GameOid, Name, Code, Series, LanguageOid_Primary, ReleasedOn) VALUES (?, ?, ?, 'Other products', ?, ?)",
                 GAME, name, (group.get("abbreviation") or None), EN, (group.get("publishedOn") or "")[:10] or None)
    cur.execute("INSERT INTO CardSetName (CardSetOid, LanguageOid, Name) VALUES (?, ?, ?)", oid, ENGLISH, name)
    if mapped is None:
        cur.execute("INSERT INTO CardSetExternalId (CardSetOid, DataSourceOid, ExternalId) VALUES (?, ?, ?)", oid, SRC["TCGplayer"], gid)
    else:
        cur.execute("UPDATE CardSetExternalId SET CardSetOid = ? WHERE DataSourceOid = ? AND ExternalId = ?", oid, SRC["TCGplayer"], gid)
    stats["sets created for TCGplayer groups the catalog source does not have"] += 1
    return oid


def create_card(set_oid, pr, ext, pid):
    """A card the catalog source does not list, taken from TCGplayer's product data."""
    name = re.sub(r"\s+-\s+[A-Za-z0-9]+/[A-Za-z0-9]+\s*$", "", pr.get("name") or "?")[:300]
    number = str(ext.get("Number") or "").split("/")[0].strip()[:50] or None
    card = insert("INSERT INTO Collectible (CardSetOid, Name, CardNumber, Rarity) VALUES (?, ?, ?, ?)", set_oid, name, number, (ext.get("Rarity") or None))
    cur.execute("INSERT INTO CollectibleName (CollectibleOid, LanguageOid, Name) VALUES (?, ?, ?)", card, ENGLISH, name)
    cur.execute("INSERT INTO CollectibleExternalId (CollectibleOid, DataSourceOid, ExternalId) VALUES (?, ?, ?)", card, SRC["TCGplayer"], pid)
    if pr.get("imageUrl"):
        cur.execute("INSERT INTO CollectibleImage (CollectibleOid, LanguageOid, DataSourceOid, ImageUrl, ThumbnailUrl) VALUES (?, ?, ?, ?, ?)",
                    card, EN, SRC["TCGplayer"], pr["imageUrl"].replace("_200w.", "_in_1000x1000."), pr["imageUrl"])
    has_image.add(card)
    stats["cards created from TCGplayer data"] += 1
    return card


def variant_for(card_oid, set_oid, printing, stamp, product_id):
    """The version of a card for one TCGplayer printing; created when the catalog does not have it."""
    pool = [v for v in card_variants[card_oid] if v["printing"] == printing and v["lang"] == EN]
    if stamp is None:
        plain = [v for v in pool if v["stamp"] is None]
        plain.sort(key=lambda v: v["subtype"] not in (None, "unlimited"))
        hit = plain[0] if plain else None
    else:
        hit = next((v for v in pool if (v["stamp"] or "").lower() == stamp.lower()), None)
    if hit is None:
        voz = insert("INSERT INTO CollectibleVariant (CollectibleOid, PrintingOid, LanguageOid, Subtype, Stamp) VALUES (?, ?, ?, NULL, ?)",
                     card_oid, printing_oid(printing), EN, stamp)
        hit = {"oid": voz, "printing": printing, "stamp": stamp, "subtype": None, "card": card_oid, "set": set_oid, "lang": EN}
        card_variants[card_oid].append(hit)
        stats["versions only TCGplayer lists (created)"] += 1
    if not any(v["oid"] == hit["oid"] for v in variants[product_id]):
        cur.execute("INSERT INTO CollectibleVariantExternalId (CollectibleVariantOid, DataSourceOid, ExternalId) VALUES (?, ?, ?)", hit["oid"], SRC["TCGplayer"], product_id)
        variants[product_id].append(hit)
        stats["TCGplayer product ids recorded on versions"] += 1
    return hit


def printing_oid(name):
    if name not in PRINT:
        PRINT[name] = insert("INSERT INTO Printing (Name, SortOrder) VALUES (?, ?)", name, 100 + 10 * len(PRINT))
        stats["Printing added: " + name] += 1
    return PRINT[name]


priced_this_run = set()


def set_price(variant_oid, p, product_id):
    market = money(p.get("marketPrice"))
    low, high = money(p.get("lowPrice")), money(p.get("highPrice"))
    if market is not None:
        source, price = SRC["TCGplayer"], market
    else:
        if ARGS.fix_mid and (variant_oid, SRC["TCGplayer"]) in price_rows:
            cur.execute("DELETE FROM CollectiblePrice WHERE Oid = ?", price_rows.pop((variant_oid, SRC["TCGplayer"]))[0])   # history cascades
            stats["market rows removed (they held an asking price)"] += 1
        if low is None:
            stats["price rows with neither a market nor a low price"] += 1
            return
        source, price = SRC["TCGplayer listings"], low
    # Two TCGplayer rows can lead to the same version ("Holofoil" and "Unlimited Holofoil" of one product). The
    # first one wins: letting both write would flip the price back and forth and fill the history with noise.
    if (variant_oid, source) in priced_this_run:
        stats["second price for the same version in one run (ignored)"] += 1
        return
    priced_this_run.add((variant_oid, source))
    url = f"https://www.tcgplayer.com/product/{product_id}"
    row = price_rows.get((variant_oid, source))
    label = "market" if source == SRC["TCGplayer"] else "listing"
    if row is None:
        oid = insert("""INSERT INTO CollectiblePrice (CollectibleVariantOid, DataSourceOid, DataSourceOid_RetrievedFrom, IsGraded, Price, LowPrice, HighPrice, CurrencyCode, LastConfirmed, SourceUrl)
                        VALUES (?, ?, ?, 0, ?, ?, ?, 'USD', ?, ?)""", variant_oid, source, SRC["TCGCSV"], price, low, high, AS_OF, url)
        cur.execute("INSERT INTO CollectiblePriceHistory (CollectiblePriceOid, Price, LowPrice, HighPrice, CurrencyCode, FirstConfirmed, LastConfirmed) VALUES (?, ?, ?, ?, 'USD', ?, ?)",
                    oid, price, low, high, AS_OF, AS_OF)
        price_rows[(variant_oid, source)] = (oid, price)
        stats[f"{label} prices added"] += 1
        return
    oid, old = row
    cur.execute("""UPDATE CollectiblePrice SET Price = ?, LowPrice = ?, HighPrice = ?, LastConfirmed = ?, SourceUrl = ?, DataSourceOid_RetrievedFrom = ?, ModifiedOn = GETDATE()
                   WHERE Oid = ? AND LastConfirmed <= ?""", price, low, high, AS_OF, url, SRC["TCGCSV"], oid, AS_OF)
    if cur.rowcount == 0:
        stats["prices already newer than TCGCSV (left alone)"] += 1
        return
    if old == price:
        cur.execute("""UPDATE CollectiblePriceHistory SET LastConfirmed = ? WHERE Oid = (SELECT TOP 1 Oid FROM CollectiblePriceHistory
                       WHERE CollectiblePriceOid = ? ORDER BY LastConfirmed DESC, Oid DESC) AND LastConfirmed < ?""", AS_OF, oid, AS_OF)
        stats[f"{label} prices unchanged"] += 1
    else:
        cur.execute("INSERT INTO CollectiblePriceHistory (CollectiblePriceOid, Price, LowPrice, HighPrice, CurrencyCode, FirstConfirmed, LastConfirmed) VALUES (?, ?, ?, ?, 'USD', ?, ?)",
                    oid, price, low, high, AS_OF, AS_OF)
        price_rows[(variant_oid, source)] = (oid, price)
        stats[f"{label} prices changed (history row added)"] += 1


def pick_variant(candidates, printing, first):
    same = [v for v in candidates if v["printing"] == printing]
    if not same:
        return None
    want = [v for v in same if (v["stamp"] is not None and "1st-edition" in v["stamp"]) == first]
    pool = want or ([] if first else same)
    if not pool:
        return None
    # plain versions first: no stamp other than the edition, unlimited before the special print runs
    pool.sort(key=lambda v: (v["stamp"] not in (None, "1st-edition"), v["subtype"] not in (None, "unlimited", "shadowless") if first else v["subtype"] not in (None, "unlimited")))
    return pool[0]


# ----------------------------------------------------------------------------- per group
unmapped = []
misses = defaultdict(list)
for gi, g in enumerate(groups):
    gid = g["groupId"]
    prods = get_json(f"https://tcgcsv.com/tcgplayer/{CATEGORY}/{gid}/products", f"products_{gid}_{day}.json")
    prices = get_json(f"https://tcgcsv.com/tcgplayer/{CATEGORY}/{gid}/prices", f"prices_{gid}_{day}.json")
    if prods is None or prices is None:
        stats["groups that could not be read"] += 1
        continue
    by_product = defaultdict(list)
    for p in prices:
        by_product[str(p["productId"])].append(p)

    # which of our sets is this group? the set most of its known products belong to
    # only card products vote: a sealed product sits wherever an earlier run put it
    votes = Counter(v["set"] for pr in prods if any(e["name"] == "Number" for e in pr.get("extendedData", []))
                    for v in variants.get(str(pr["productId"]), [])[:1])
    set_oid = votes.most_common(1)[0][0] if votes else find_set(g)
    # A group whose cards come from many sets ("Miscellaneous Cards & Products", "League & Championship Cards")
    # has no set of its own: its sealed products must not be filed under whichever set happens to lead.
    mixed = bool(votes) and votes.most_common(1)[0][1] < 0.6 * sum(votes.values())
    if votes:
        stats["groups matched to a set by known product ids"] += 1
    elif set_oid is not None:
        stats["groups matched to a set by name or code"] += 1

    # products that share a card number inside this group: the extra ones are special versions (stamped, patterned, ...)
    same_number = Counter(numkey(e["value"]) for pr in prods for e in pr.get("extendedData", []) if e["name"] == "Number")
    # A group that is no catalog set gets a set of its own (English category only - see the header).
    # Not for trainer kits: TCGplayer sells two half-decks as one group, the catalog has them as two sets, and
    # nothing in the product data says which half a card belongs to - creating them again would duplicate them.
    own_set = group_set(g) if (set_oid is None or mixed) and prods and not FOREIGN and not ARGS.no_sealed and "Trainer Kit" not in g["name"] else None
    if set_oid is None:
        unmapped.append(f"{g['name']} ({len(prods)} products)")
    elif not mixed:
        mapped = scalar("SELECT CardSetOid FROM CardSetExternalId WHERE DataSourceOid = ? AND ExternalId = ?", SRC["TCGplayer"], str(gid))
        if mapped is None:
            cur.execute("INSERT INTO CardSetExternalId (CardSetOid, DataSourceOid, ExternalId) VALUES (?, ?, ?)", set_oid, SRC["TCGplayer"], str(gid))
        elif mapped != set_oid:
            cur.execute("UPDATE CardSetExternalId SET CardSetOid = ? WHERE DataSourceOid = ? AND ExternalId = ?", set_oid, SRC["TCGplayer"], str(gid))
            stats["groups re-mapped to a better set"] += 1
        if FOREIGN:
            english_name("CardSetName", "CardSetOid", set_oid, g["name"].split(": ", 1)[-1])

    for pr in prods:
        pid = str(pr["productId"])
        known = variants.get(pid)
        ext = {e["name"]: e["value"] for e in pr.get("extendedData", [])}
        if known and "Number" not in ext and mixed:
            # a sealed product of a mixed group: it belongs in the group's own set, not in whichever set leads the vote
            if own_set is not None and known[0]["set"] != own_set:
                cur.execute("UPDATE Collectible SET CardSetOid = ?, ModifiedOn = GETDATE() WHERE Oid = ? AND Category = 'Sealed'", own_set, known[0]["card"])
                known[0]["set"] = own_set
                stats["sealed products moved to the right set"] += cur.rowcount
            for p in by_product.get(pid, [])[:1]:
                set_price(known[0]["oid"], p, pid)
            continue
        if known and "Number" not in ext and set_oid is not None and known[0]["set"] != set_oid:
            # a sealed product an earlier run filed under the wrong set
            cur.execute("UPDATE Collectible SET CardSetOid = ?, ModifiedOn = GETDATE() WHERE Oid = ? AND Category = 'Sealed'", set_oid, known[0]["card"])
            known[0]["set"] = set_oid
            stats["sealed products moved to the right set"] += cur.rowcount
        if known:
            picture(known[0]["card"], pr)
            if FOREIGN and "Number" in ext:
                english_name("CollectibleName", "CollectibleOid", known[0]["card"], clean_product_name(pr.get("name"))[0])
            for p in by_product.get(pid, []):
                printing, first = split_subtype(p.get("subTypeName"))
                v = pick_variant(known, printing, first)
                if v is None:
                    # TCGplayer sells a printing the catalog source did not list: add it as a version of the same card
                    stamp = "1st-edition" if first else None
                    lang = known[0]["lang"]
                    voz = scalar("""SELECT Oid FROM CollectibleVariant WHERE CollectibleOid = ? AND PrintingOid = ? AND LanguageOid = ?
                                    AND Subtype IS NULL AND ISNULL(Stamp, '') = ISNULL(?, '')""", known[0]["card"], printing_oid(printing), lang, stamp)
                    if voz is None:
                        voz = insert("INSERT INTO CollectibleVariant (CollectibleOid, PrintingOid, LanguageOid, Subtype, Stamp) VALUES (?, ?, ?, NULL, ?)",
                                     known[0]["card"], printing_oid(printing), lang, stamp)
                        stats["versions only TCGplayer lists (created)"] += 1
                    cur.execute("INSERT INTO CollectibleVariantExternalId (CollectibleVariantOid, DataSourceOid, ExternalId) VALUES (?, ?, ?)", voz, SRC["TCGplayer"], pid)
                    v = {"oid": voz, "printing": printing, "stamp": stamp, "subtype": None, "card": known[0]["card"], "set": known[0]["set"], "lang": lang}
                    known.append(v)
                set_price(v["oid"], p, pid)
            continue
        if "Number" in ext:
            if set_oid is None and own_set is not None:
                card_oid = create_card(own_set, pr, ext, pid)
                rows = by_product.get(pid, [])
                for p in rows:
                    printing, first = split_subtype(p.get("subTypeName"))
                    set_price(variant_for(card_oid, own_set, printing, "1st-edition" if first else None, pid)["oid"], p, pid)
                if not rows:
                    variant_for(card_oid, own_set, "Normal", None, pid)       # every card has at least one version
                continue
            if set_oid is None:
                stats["card products skipped: group has no matching set"] += 1
                continue
            key = numkey(ext["Number"])
            base_name, extra = clean_product_name(pr.get("name"))
            wanted = norm(base_name)
            cands = cards_by_number[set_oid].get(key, [])
            if FOREIGN:
                # the catalog name is in another script, so it cannot be compared with TCGplayer's English name:
                # accept the card only when the number is unambiguous in the set
                cands = cands if len(cands) == 1 else []
            else:
                cands = [c for c in cands if c[1] and (c[1] == wanted or c[1] in wanted or wanted in c[1])]
            if not cands:
                stats["card products skipped: no card with that number and name in the set"] += 1
                misses[g["name"]].append(f"{ext['Number']} {pr.get('name')} | catalog has: {[c[1] for c in cards_by_number[set_oid].get(key, [])][:2]}")
                continue
            card_oid = cands[0][0]
            picture(card_oid, pr)
            if FOREIGN:
                english_name("CollectibleName", "CollectibleOid", card_oid, base_name)
            special = extra if same_number[key] > 1 else None
            for p in by_product.get(pid, []):
                printing, first = split_subtype(p.get("subTypeName"))
                stamp = "1st-edition" if first else special
                v = variant_for(card_oid, set_oid, printing, stamp, pid)
                set_price(v["oid"], p, pid)
            stats["card products matched by set + number + name"] += 1
            continue
        target = own_set if (set_oid is None or mixed) else set_oid
        if ARGS.no_sealed or target is None or not by_product.get(pid):
            stats["sealed products skipped (no set or no price)"] += 1
            continue
        # a sealed product: one Collectible with one version
        name = (pr.get("name") or "?")[:300]
        card = insert("INSERT INTO Collectible (CardSetOid, Name, CardNumber, Rarity, Category) VALUES (?, ?, NULL, NULL, 'Sealed')", target, name)
        cur.execute("INSERT INTO CollectibleName (CollectibleOid, LanguageOid, Name) VALUES (?, ?, ?)", card, ENGLISH, name)
        cur.execute("INSERT INTO CollectibleExternalId (CollectibleOid, DataSourceOid, ExternalId) VALUES (?, ?, ?)", card, SRC["TCGplayer"], pid)
        if pr.get("imageUrl"):
            cur.execute("INSERT INTO CollectibleImage (CollectibleOid, LanguageOid, DataSourceOid, ImageUrl, ThumbnailUrl) VALUES (?, ?, ?, ?, ?)",
                        card, EN, SRC["TCGplayer"], pr["imageUrl"].replace("_200w.", "_in_1000x1000."), pr["imageUrl"])
        voz = insert("INSERT INTO CollectibleVariant (CollectibleOid, PrintingOid, LanguageOid) VALUES (?, ?, ?)", card, printing_oid("Normal"), EN)
        cur.execute("INSERT INTO CollectibleVariantExternalId (CollectibleVariantOid, DataSourceOid, ExternalId) VALUES (?, ?, ?)", voz, SRC["TCGplayer"], pid)
        variants[pid].append({"oid": voz, "printing": "Normal", "stamp": None, "subtype": None, "card": card, "set": target, "lang": EN})
        stats["sealed products added"] += 1
        set_price(voz, by_product[pid][0], pid)
    cn.commit()
    if gi % 40 == 0:
        print(f"  group {gi + 1}/{len(groups)} {g['name']}", flush=True)

# a group set that ended up with nothing in it (its products had no price) is not worth a tile in the catalog
empty = [r[0] for r in cur.execute("""SELECT s.Oid FROM CardSet s WHERE s.Series = 'Other products'
                                      AND NOT EXISTS (SELECT 1 FROM Collectible c WHERE c.CardSetOid = s.Oid)""").fetchall()]
for oid in empty:
    cur.execute("DELETE FROM CardSetExternalId WHERE CardSetOid = ?", oid)
    cur.execute("DELETE FROM CardSetName WHERE CardSetOid = ?", oid)
    cur.execute("DELETE FROM CardSet WHERE Oid = ?", oid)
cn.commit()
stats["empty group sets removed"] += len(empty)

# yesterday's cached files are of no further use
for old_file in CACHE.glob("*.json"):
    if not old_file.stem.endswith("_" + day):
        old_file.unlink()

print("=== result")
for k in sorted(stats):
    print(f"  {k}: {stats[k]}")
print(f"  groups with no matching set ({len(unmapped)}):", unmapped[:60])
if ARGS.show_misses:
    print("=== card products with no catalog card, worst groups first")
    for name, rows in sorted(misses.items(), key=lambda kv: -len(kv[1]))[:22]:
        print(f"  {name}: {len(rows)} | {rows[:3]}")
print("  totals: prices", scalar("SELECT COUNT(*) FROM CollectiblePrice"), "| history", scalar("SELECT COUNT(*) FROM CollectiblePriceHistory"),
      "| sealed", scalar("SELECT COUNT(*) FROM Collectible WHERE Category = 'Sealed'"))
cn.close()

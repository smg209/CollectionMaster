"""Catalog importer: loads Pokemon sets into the CollectionMaster database.

  TCGdex   (no key)  catalog, names by language, images, versions, TCGplayer + Cardmarket prices
  JustTCG  (key)     prices by condition and by grade - ONLY from cached responses unless --justtcg N allows N requests
  TCGCSV   (no key)  see refresh_tcgplayer.py (daily TCGplayer prices, sealed products)

Safe to run again: everything is found by its source id first and inserted only when missing; a price that
already exists is updated in place and a history row is added only when the price changed.

Usage:
  python import_catalog.py --cache <folder> [--older-cache <folder> ...] [--sets base1,sv06.5 | --all-en | --all-ja]
                           [--refresh] [--justtcg N]
  --refresh   ignore cached card JSON and fetch again (prices move daily)
"""
import os, sys, json, time, re, argparse, urllib.request, urllib.error, urllib.parse
from concurrent.futures import ThreadPoolExecutor
from datetime import datetime
from pathlib import Path
from collections import Counter, defaultdict
import pyodbc

ap = argparse.ArgumentParser()
ap.add_argument("--cache", required=True)
ap.add_argument("--older-cache", action="append", default=[])
ap.add_argument("--sets", default="")
ap.add_argument("--all-en", action="store_true")
ap.add_argument("--all-ja", action="store_true")
ap.add_argument("--refresh", action="store_true")
ap.add_argument("--justtcg", type=int, default=0)
ARGS = ap.parse_args()

CONN = r"Driver={ODBC Driver 18 for SQL Server};Server=localhost\SQL2025;Database=CollectionMaster;Trusted_Connection=yes;TrustServerCertificate=yes;"
CACHE = [Path(ARGS.cache)] + [Path(p) for p in ARGS.older_cache]
CACHE[0].mkdir(parents=True, exist_ok=True)
KEY = (os.environ.get("JUSTTCG_API_KEY") or "").strip()
UA = "CollectionMaster-dev/0.1 (personal collection tracker)"
jt_calls = 0
jt_last = 0.0

WESTERN = ["fr", "de", "es", "it", "pt"]
SKIP_SERIES = {"tcgp"}            # Pokemon TCG Pocket: digital-only cards, nothing to collect or price
# TCGdex set id -> (JustTCG game, JustTCG set id or prefix). Only these sets get JustTCG prices.
# A value ending in "-" is an id prefix; "name:..." is looked up by set name in the cached JustTCG set list.
JUSTTCG_SETS = {
    "base1": ("pokemon", "base-set-pokemon"),
    "sv06.5": ("pokemon", "sv-shrouded-fable-pokemon"),
    "SV6a": ("pokemon-japan", "sv6a-"),
    "sv08.5": ("pokemon", "name:Prismatic Evolutions"),
    "swsh7": ("pokemon", "name:Evolving Skies"),
    "sv03.5": ("pokemon", "name:151"),
    "sv08": ("pokemon", "name:Surging Sparks"),
}
PRINTING_BY_TYPE = {"normal": "Normal", "holo": "Holofoil", "reverse": "Reverse Holofoil"}
TCGPLAYER_KEY_BY_TYPE = {"normal": "normal", "holo": "holofoil", "reverse": "reverse-holofoil"}


# ----------------------------------------------------------------------------- http + cache
def http_get(url, headers=None, tries=3):
    for attempt in range(tries):
        req = urllib.request.Request(url, headers={"User-Agent": UA, "Accept": "application/json", **(headers or {})})
        try:
            with urllib.request.urlopen(req, timeout=40) as r:
                return r.status, r.read().decode("utf-8", "replace")
        except urllib.error.HTTPError as e:
            if e.code in (404, 400, 401, 403):
                return e.code, ""
            time.sleep(1.5 * (attempt + 1))
        except Exception:
            time.sleep(1.5 * (attempt + 1))
    return None, ""


def safe(name):
    return re.sub(r"[^A-Za-z0-9._-]", "_", name)


def cached(name):
    name = safe(name)
    for folder in CACHE:
        p = folder / name
        if p.exists():
            try:
                return json.loads(p.read_text(encoding="utf-8"))
            except Exception:
                pass
    return None


def store(name, obj):
    (CACHE[0] / safe(name)).write_text(json.dumps(obj, ensure_ascii=False), encoding="utf-8")


def tcgdex(path, cache_name, refresh=False):
    d = None if refresh else cached(cache_name)
    if d is not None:
        return d
    st, body = http_get("https://api.tcgdex.net/v2/" + path)
    if st != 200:
        return None
    d = json.loads(body)
    store(cache_name, d)
    return d


def tcgdex_cards(lang, set_id, briefs):
    name = f"tcgdex_cards_{lang}_{set_id}.json"
    d = None if ARGS.refresh else cached(name)
    have = {}
    if d is not None:
        have = {c.get("id"): c for c in d}
        if all(b["id"] in have for b in briefs):
            return d
        briefs_wanted = [b for b in briefs if b["id"] not in have]      # a previous run could not fetch these
    else:
        briefs_wanted = briefs

    def one_card(b):
        st, body = http_get(f"https://api.tcgdex.net/v2/{lang}/cards/{urllib.parse.quote(str(b['id']), safe='')}")
        try:
            return json.loads(body) if st == 200 else None
        except Exception:
            return None

    with ThreadPoolExecutor(max_workers=8) as pool:
        cards = list(have.values()) + [c for c in pool.map(one_card, briefs_wanted) if c]
    if len(cards) < len(briefs):
        got = {c.get("id") for c in cards}
        print(f"  {set_id}: {len(briefs) - len(cards)} card(s) could not be fetched: {[b['id'] for b in briefs if b['id'] not in got][:10]}")
    store(name, cards)
    return cards


def justtcg(path, params):
    global jt_calls, jt_last
    if not KEY or jt_calls >= ARGS.justtcg:
        return None
    wait = 6.6 - (time.time() - jt_last)
    if wait > 0:
        time.sleep(wait)
    jt_calls += 1
    jt_last = time.time()
    st, body = http_get("https://api.justtcg.com/v1" + path + "?" + urllib.parse.urlencode(params), {"x-api-key": KEY}, tries=1)
    if st != 200:
        print("  JustTCG", path, "status", st)
        return None
    return json.loads(body.replace(KEY, "***"))


def justtcg_cards(game, set_id):
    name = f"justtcg_cards_{set_id}.json"
    d = cached(name)
    if d is not None:
        return d
    cards, off = [], 0
    while True:
        r = justtcg("/cards", {"game": game, "set": set_id, "limit": 20, "offset": off})
        if not r or not isinstance(r.get("data"), list):
            return None          # incomplete: do not cache a partial set
        cards += r["data"]
        off += len(r["data"])
        if not r["data"] or not (r.get("meta") or {}).get("hasMore"):
            break
    store(name, cards)
    return cards


# ----------------------------------------------------------------------------- helpers
def norm(s):
    return re.sub(r"[^a-z0-9]", "", (s or "").lower())


def numkey(s):
    return (str(s or "").split("/")[0].strip().lstrip("0").upper()) or "0"


def from_unix(t):
    return datetime.fromtimestamp(int(t)).replace(microsecond=0)      # local time, like GETDATE()


def from_iso(s):
    try:
        return datetime.fromisoformat(s.replace("Z", "+00:00")).astimezone().replace(tzinfo=None, microsecond=0)
    except Exception:
        return datetime.now().replace(microsecond=0)


def money(v):
    try:
        v = round(float(v), 2)
        return v if v > 0 else None
    except Exception:
        return None


def spans(points, current_price, current_time):
    """[(time, price)] -> [[price, first, last]]: one row per run of the same price, ending with the current price."""
    out = []
    for t, p in sorted((x for x in points if x[1] is not None), key=lambda x: x[0]):
        if out and out[-1][0] == p:
            out[-1][2] = t
        else:
            out.append([p, t, t])
    if out and out[-1][0] == current_price:
        out[-1][2] = max(out[-1][2], current_time)
    else:
        out.append([current_price, current_time, current_time])
    return out


def split_printing(name, default_lang):
    """JustTCG names Japanese printings 'Holofoil - Japanese': the printing is Holofoil, the language is Japanese."""
    name = name or "Normal"
    if " - " in name:
        base, tail = name.rsplit(" - ", 1)
        if tail in LANG_BY_NAME:
            return base, LANG_BY_NAME[tail]
    return name, default_lang


# ----------------------------------------------------------------------------- database
cn = pyodbc.connect(CONN, autocommit=False)
cur = cn.cursor()
stats = Counter()


def one(sql, *a):
    r = cur.execute(sql, *a).fetchone()
    return r[0] if r else None


def insert(sql, *a):
    cur.execute("SET NOCOUNT ON; " + sql + "; SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", *a)
    return int(cur.fetchone()[0])


LANG = {r.Code: r.Oid for r in cur.execute("SELECT Oid, Code FROM [Language]")}
LANG_BY_NAME = {r.Name: r.Oid for r in cur.execute("SELECT Oid, Name FROM [Language]")}
SRC = {r.Name: r.Oid for r in cur.execute("SELECT Oid, Name FROM DataSource")}
COND = {r.Name: r.Oid for r in cur.execute("SELECT Oid, Name FROM Condition")}
PRINT = {r.Name: r.Oid for r in cur.execute("SELECT Oid, Name FROM Printing")}
COMPANY = {r.Abbreviation: r.Oid for r in cur.execute("SELECT Oid, Abbreviation FROM GradingCompany")}
GRADE = {(r.GradingCompanyOid, r.Code): r.Oid for r in cur.execute("SELECT Oid, GradingCompanyOid, Code FROM Grade")}


def printing_oid(name):
    if name not in PRINT:
        PRINT[name] = insert("INSERT INTO Printing (Name, SortOrder) VALUES (?, ?)", name, 100 + 10 * len(PRINT))
        stats["Printing added: " + name] += 1
    return PRINT[name]


def grade_oid(company, grade, label, canonical):
    co = COMPANY.get(company)
    if co is None:
        return None
    g = float(grade)
    code = (str(int(g)) if g == int(g) else str(g)) + ((" " + label) if label else "")
    if (co, code) not in GRADE:
        GRADE[(co, code)] = insert("INSERT INTO Grade (GradingCompanyOid, Code, Name, GradeValue, SortOrder) VALUES (?, ?, ?, ?, ?)",
                                   co, code, canonical or f"{company} {code}", g, int(200 - g * 10))
        stats[f"Grade added: {company} {code}"] += 1
    return GRADE[(co, code)]


def get_game():
    oid = one("SELECT Oid FROM Game WHERE Name = ?", "Pokemon")
    if oid is None:
        ct = one("SELECT Oid FROM CollectibleType WHERE Name = ?", "Trading Card Game")
        oid = insert("INSERT INTO Game (CollectibleTypeOid, Name) VALUES (?, ?)", ct, "Pokemon")
    return oid


def upsert_name(table, fk, oid, lang_oid, name, fresh=False):
    if not name:
        return
    if fresh or one(f"SELECT Oid FROM {table} WHERE {fk} = ? AND LanguageOid = ?", oid, lang_oid) is None:
        cur.execute(f"INSERT INTO {table} ({fk}, LanguageOid, Name) VALUES (?, ?, ?)", oid, lang_oid, name[:300 if table == "CollectibleName" else 200])
        stats[table] += 1


def ext_id(table, fk, oid, source, external, fresh=False):
    if external is None or external == "":
        return
    external = str(external)
    if fresh or one(f"SELECT Oid FROM {table} WHERE {fk} = ? AND DataSourceOid = ? AND ExternalId = ?", oid, SRC[source], external) is None:
        cur.execute(f"INSERT INTO {table} ({fk}, DataSourceOid, ExternalId) VALUES (?, ?, ?)", oid, SRC[source], external)
        stats[table] += 1


def upsert_price(variant, source, via, graded, grade, cond, qualifier, price, low, high, currency, confirmed, url, history, fresh=False):
    """history = [(time, price)] from the source, or [] when the source gives only today's price.
    With source history the stored history is rebuilt from it. Without, a row is appended only when the price changed."""
    price = money(price)
    if price is None:
        return
    low, high = money(low), money(high)
    row = None if fresh else cur.execute("""SELECT Oid, Price, LastConfirmed FROM CollectiblePrice WHERE CollectibleVariantOid = ? AND DataSourceOid = ?
                 AND ISNULL(GradeOid, 0) = ISNULL(?, 0) AND ISNULL(ConditionOid, 0) = ISNULL(?, 0) AND ISNULL(Qualifier, '') = ISNULL(?, '')""",
                                         variant, SRC[source], grade, cond, qualifier).fetchone()
    if row is None:
        oid = insert("""INSERT INTO CollectiblePrice (CollectibleVariantOid, DataSourceOid, DataSourceOid_RetrievedFrom, IsGraded, GradeOid, ConditionOid, Qualifier,
                        Price, LowPrice, HighPrice, CurrencyCode, LastConfirmed, SourceUrl) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)""",
                     variant, SRC[source], SRC[via] if via else None, 1 if graded else 0, grade, cond, qualifier, price, low, high, currency, confirmed, url)
        stats[f"CollectiblePrice ({source}{' graded' if graded else ''})"] += 1
        rows = spans([(t, money(p)) for t, p in history], price, confirmed)
    else:
        oid = row[0]
        if row[2] is not None and confirmed < row[2] and not history:
            stats["prices already newer in the database (left alone)"] += 1
            return                      # another source refreshed this price more recently; never move it back in time
        cur.execute("UPDATE CollectiblePrice SET Price = ?, LowPrice = ?, HighPrice = ?, CurrencyCode = ?, LastConfirmed = ?, SourceUrl = ?, ModifiedOn = GETDATE() WHERE Oid = ?",
                    price, low, high, currency, confirmed, url, oid)
        stats["CollectiblePrice updated"] += 1
        if history:
            cur.execute("DELETE FROM CollectiblePriceHistory WHERE CollectiblePriceOid = ?", oid)
            rows = spans([(t, money(p)) for t, p in history], price, confirmed)
        else:
            last = cur.execute("SELECT TOP 1 Oid, Price FROM CollectiblePriceHistory WHERE CollectiblePriceOid = ? ORDER BY LastConfirmed DESC, Oid DESC", oid).fetchone()
            if last is not None and float(last[1]) == price:
                cur.execute("UPDATE CollectiblePriceHistory SET LastConfirmed = ? WHERE Oid = ? AND LastConfirmed < ?", confirmed, last[0], confirmed)
                return
            rows = [[price, confirmed, confirmed]]
    for p, first, last in rows:
        cur.execute("INSERT INTO CollectiblePriceHistory (CollectiblePriceOid, Price, LowPrice, HighPrice, CurrencyCode, FirstConfirmed, LastConfirmed) VALUES (?, ?, ?, ?, ?, ?, ?)",
                    oid, p, low if p == price else None, high if p == price else None, currency, first, last)
    stats["CollectiblePriceHistory"] += len(rows)


def get_variant(collectible, printing_name, lang_oid, subtype, stamp, fresh=False):
    """-> (oid, is_new)"""
    p = printing_oid(printing_name)
    oid = None if fresh else one("""SELECT Oid FROM CollectibleVariant WHERE CollectibleOid = ? AND PrintingOid = ? AND LanguageOid = ?
                 AND ISNULL(Subtype, '') = ISNULL(?, '') AND ISNULL(Stamp, '') = ISNULL(?, '')""", collectible, p, lang_oid, subtype, stamp)
    if oid is not None:
        return oid, False
    oid = insert("INSERT INTO CollectibleVariant (CollectibleOid, PrintingOid, LanguageOid, Subtype, Stamp) VALUES (?, ?, ?, ?, ?)", collectible, p, lang_oid, subtype, stamp)
    stats["CollectibleVariant"] += 1
    return oid, True


# ----------------------------------------------------------------------------- import one set
def import_set(game_oid, set_id, lang, other_langs):
    s = tcgdex(f"{lang}/sets/{urllib.parse.quote(set_id, safe='')}", f"tcgdex_set_{lang}_{set_id}.json")
    if not s:
        print(f"=== {set_id} [{lang}]: not found on TCGdex - skipped")
        return
    if (s.get("serie") or {}).get("id") in SKIP_SERIES:
        stats["sets skipped (digital-only series)"] += 1
        return
    cards = tcgdex_cards(lang, set_id, s.get("cards", []))
    lang_oid = LANG[lang]
    # TCGdex reuses ids across languages in different case (English "sm1", Japanese "SM1") and the database
    # compares text without regard to case, so every non-English id is stored with its language in front.
    prefix = "" if lang == "en" else lang + ":"
    abbr = (s.get("abbreviation") or {}).get("official") or s.get("tcgOnline") or set_id

    set_oid = one("SELECT CardSetOid FROM CardSetExternalId WHERE DataSourceOid = ? AND ExternalId = ?", SRC["TCGdex"], prefix + set_id)
    set_fresh = set_oid is None
    if set_fresh:
        cc = s.get("cardCount") or {}
        name = s["name"]
        if one("SELECT Oid FROM CardSet WHERE GameOid = ? AND Name = ?", game_oid, name) is not None:
            name = f"{name} ({set_id})"          # two different sets share a display name
        set_oid = insert("""INSERT INTO CardSet (GameOid, Name, Code, Series, LanguageOid_Primary, ReleasedOn, CardCountOfficial, CardCountTotal, LogoUrl, SymbolUrl)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)""",
                         game_oid, name, abbr, (s.get("serie") or {}).get("name"), lang_oid, s.get("releaseDate"), cc.get("official"), cc.get("total"),
                         (s["logo"] + ".png") if s.get("logo") else None, (s["symbol"] + ".png") if s.get("symbol") else None)
        stats["CardSet"] += 1
    ext_id("CardSetExternalId", "CardSetOid", set_oid, "TCGdex", prefix + set_id, set_fresh)
    upsert_name("CardSetName", "CardSetOid", set_oid, lang_oid, s["name"], set_fresh)

    localized = {}
    for lg in other_langs:
        ls = tcgdex(f"{lg}/sets/{urllib.parse.quote(set_id, safe='')}", f"tcgdex_set_{lg}_{set_id}.json")
        if not ls:
            continue
        upsert_name("CardSetName", "CardSetOid", set_oid, LANG[lg], ls.get("name"))
        localized[lg] = {b["id"]: b for b in ls.get("cards", [])}

    by_tcgplayer = defaultdict(list)     # tcgplayer product id -> [(variant oid, printing name, subtype, stamp)]
    by_number = {}
    for c in cards:
        cid = c["id"]
        oid = None if set_fresh else one("SELECT CollectibleOid FROM CollectibleExternalId WHERE DataSourceOid = ? AND ExternalId = ?", SRC["TCGdex"], prefix + cid)
        fresh = oid is None
        if fresh:
            oid = insert("INSERT INTO Collectible (CardSetOid, Name, CardNumber, Rarity, Category, Illustrator) VALUES (?, ?, ?, ?, ?, ?)",
                         set_oid, (c.get("name") or "?")[:300], str(c.get("localId")), c.get("rarity"), c.get("category"), c.get("illustrator"))
            stats["Collectible"] += 1
        by_number[numkey(c.get("localId"))] = oid
        ext_id("CollectibleExternalId", "CollectibleOid", oid, "TCGdex", prefix + cid, fresh)
        upsert_name("CollectibleName", "CollectibleOid", oid, lang_oid, c.get("name"), fresh)
        images = [(lang_oid, c.get("image"))]
        for lg, briefs in localized.items():
            b = briefs.get(cid)
            if b:
                upsert_name("CollectibleName", "CollectibleOid", oid, LANG[lg], b.get("name"), fresh)
                images.append((LANG[lg], b.get("image")))
        for lo, img in images:
            if img and (fresh or one("SELECT Oid FROM CollectibleImage WHERE CollectibleOid = ? AND LanguageOid = ? AND DataSourceOid = ?", oid, lo, SRC["TCGdex"]) is None):
                cur.execute("INSERT INTO CollectibleImage (CollectibleOid, LanguageOid, DataSourceOid, ImageUrl, ThumbnailUrl) VALUES (?, ?, ?, ?, ?)",
                            oid, lo, SRC["TCGdex"], img + "/high.webp", img + "/low.webp")
                stats["CollectibleImage"] += 1

        v = c.get("variants") or {}
        detailed = c.get("variants_detailed") or [{"type": t} for t in ("normal", "holo", "reverse") if v.get(t)] or [{"type": "normal"}]
        seen = set()
        for v in detailed:
            vtype = v.get("type") or "normal"
            pname = PRINTING_BY_TYPE.get(vtype, vtype.replace("-", " ").title())
            subtype = v.get("subtype")
            if v.get("size") and v.get("size") != "standard":
                subtype = ((subtype + " ") if subtype else "") + v["size"]
            stamp = ",".join(sorted(v.get("stamp") or [])) or None
            if (pname, subtype, stamp) in seen:
                stats["TCGdex duplicate variant rows merged"] += 1
                continue
            seen.add((pname, subtype, stamp))
            voz, vfresh = get_variant(oid, pname, lang_oid, (subtype or None) and subtype[:50], (stamp or None) and stamp[:100], fresh)
            tp = (v.get("thirdParty") or {})
            ext_id("CollectibleVariantExternalId", "CollectibleVariantOid", voz, "TCGplayer", tp.get("tcgplayer"), vfresh)
            ext_id("CollectibleVariantExternalId", "CollectibleVariantOid", voz, "Cardmarket", tp.get("cardmarket"), vfresh)
            if tp.get("tcgplayer"):
                by_tcgplayer[str(tp["tcgplayer"])].append((voz, pname, subtype, stamp))
            pr = v.get("pricing") or {}
            t = pr.get("tcgplayer") or {}
            first = "1st-edition-" if stamp and "1st-edition" in stamp else ""
            base = TCGPLAYER_KEY_BY_TYPE.get(vtype, vtype)
            for k in ([first + base] if first else [base, "unlimited-" + base]):
                e = t.get(k)
                if isinstance(e, dict):
                    # market price only: with no recent sales TCGdex still relays asking prices, and an asking
                    # price is not a value (refresh_tcgplayer.py stores those as "TCGplayer listings")
                    upsert_price(voz, "TCGplayer", "TCGdex", False, None, None, None, e.get("marketPrice"), e.get("lowPrice"), e.get("highPrice"),
                                 t.get("unit") or "USD", from_iso(t.get("updated") or ""),
                                 f"https://www.tcgplayer.com/product/{e.get('productId')}" if e.get("productId") else None, [], vfresh)
                    break
            m = pr.get("cardmarket") or {}
            if m:
                sfx = "-holo" if vtype == "reverse" else ""
                upsert_price(voz, "Cardmarket", "TCGdex", False, None, None, None, m.get("trend" + sfx) or m.get("avg" + sfx), m.get("low" + sfx), None,
                             m.get("unit") or "EUR", from_iso(m.get("updated") or ""),
                             f"https://www.cardmarket.com/en/Pokemon/Products?idProduct={m.get('idProduct')}" if m.get("idProduct") else None, [], vfresh)
    print(f"=== {set_id} [{lang}] {s['name']}: {len(cards)} cards{' (new)' if set_fresh else ''}", flush=True)

    if set_id in JUSTTCG_SETS:
        # TCGplayer product ids recorded by other runs too (refresh_tcgplayer.py adds the special versions TCGdex
        # does not list - stamped, patterned - each with its own product id)
        for r in cur.execute("""SELECT e.ExternalId, v.Oid, pr.Name, v.Subtype, v.Stamp
                                FROM CollectibleVariantExternalId e
                                JOIN CollectibleVariant v ON v.Oid = e.CollectibleVariantOid
                                JOIN Printing pr ON pr.Oid = v.PrintingOid
                                JOIN Collectible c ON c.Oid = v.CollectibleOid
                                WHERE c.CardSetOid = ? AND e.DataSourceOid = ?""", set_oid, SRC["TCGplayer"]).fetchall():
            if not any(o[0] == r[1] for o in by_tcgplayer[r[0]]):
                by_tcgplayer[r[0]].append((r[1], r[2], r[3], r[4]))
        import_justtcg(set_id, set_oid, lang, lang_oid, by_tcgplayer, by_number)


def import_justtcg(set_id, set_oid, lang, lang_oid, by_tcgplayer, by_number):
    jt_game, jt_set = JUSTTCG_SETS[set_id]
    if jt_set.endswith("-") or jt_set.startswith("name:"):
        jsets = cached(f"justtcg_sets_{jt_game}.json") or {}
        if jt_set.startswith("name:"):
            want = norm(jt_set[5:])
            hit = [x for x in (jsets.get("data") or []) if norm(x.get("name", "").split(":")[-1]) == want or norm(x.get("name", "")).endswith(want)]
        else:
            hit = [x for x in (jsets.get("data") or []) if x.get("id", "").startswith(jt_set)]
        if not hit:
            print("  JustTCG: set not found in the cached set list:", jt_set)
            return
        # a set costs one request per 20 entries; never start one the remaining budget cannot finish
        pages = -(-int(hit[0].get("count") or hit[0].get("cards_count") or 0) // 20) + 1
        if cached(f"justtcg_cards_{hit[0]['id']}.json") is None and pages > ARGS.justtcg - jt_calls:
            print(f"  JustTCG: {hit[0]['name']} needs about {pages} requests, only {ARGS.justtcg - jt_calls} left in this run's budget - skipped")
            return
        jt_set, jt_name = hit[0]["id"], hit[0]["name"]
        if lang != "en":
            upsert_name("CardSetName", "CardSetOid", set_oid, LANG["en"], jt_name.split(":", 1)[1].strip() if ":" in jt_name else jt_name)
    ext_id("CardSetExternalId", "CardSetOid", set_oid, "JustTCG", jt_set)
    jcards = justtcg_cards(jt_game, jt_set)
    if not jcards:
        print("  JustTCG: no data (not cached and no request budget)")
        return
    matched = Counter()
    for jc in jcards:
        jvars = [v for v in (jc.get("variants") or []) if v.get("condition") != "Sealed"]
        if not jvars:
            matched["sealed (skipped)"] += 1
            continue
        ours = by_tcgplayer.get(str(jc.get("tcgplayerId") or ""))
        coll = None
        if ours:
            coll = one("SELECT CollectibleOid FROM CollectibleVariant WHERE Oid = ?", ours[0][0])
            matched["by TCGplayer id"] += 1
        else:
            # by card number only for a plain entry: "Pikachu (Poke Ball Pattern)" is a special version with its own
            # product id, and pricing it onto the plain card would be wrong
            plain = "(" not in (jc.get("name") or "")
            cand = by_number.get(numkey(jc.get("number"))) if plain and jc.get("number") and str(jc.get("number")).upper() != "N/A" else None
            if cand is not None:
                en = one("SELECT Name FROM Collectible WHERE Oid = ?", cand)
                base = norm(re.sub(r"\s*-\s*\d+/\d+\s*$", "", jc.get("name") or ""))
                if lang != "en" or norm(en) in base or base in norm(en):
                    coll = cand
                    matched["by card number"] += 1
        if coll is None:
            matched["unmatched"] += 1
            continue
        # the readable id where it fits the column (100 characters), else JustTCG's uuid for the same entry
        jt_id = jc.get("id") if len(jc.get("id") or "") <= 100 else jc.get("uuid")
        ext_id("CollectibleExternalId", "CollectibleOid", coll, "JustTCG", jt_id)
        if lang != "en":
            upsert_name("CollectibleName", "CollectibleOid", coll, LANG["en"], re.sub(r"\s*-\s*\d+/\d+\s*$", "", jc.get("name") or ""))
        for jv in jvars:
            pname, v_lang = split_printing(jv.get("printing"), LANG_BY_NAME.get(jv.get("language"), lang_oid))
            # A product id names specific versions. Where the printing names agree use that one; where JustTCG calls
            # the printing something else (a patterned holo listed as "Reverse Holofoil"), the product's own version
            # still wins - never fall through to some other version of the card.
            pick = [o for o in (ours or []) if o[1] == pname] or (ours or [])
            if pick:
                voz = sorted(pick, key=lambda o: (o[3] is not None, o[2] not in (None, "unlimited")))[0][0]
            else:
                voz = one("""SELECT TOP 1 v.Oid FROM CollectibleVariant v WHERE v.CollectibleOid = ? AND v.PrintingOid = ? AND v.LanguageOid = ?
                             ORDER BY CASE WHEN v.Stamp IS NULL THEN 0 ELSE 1 END, CASE WHEN ISNULL(v.Subtype, 'unlimited') = 'unlimited' THEN 0 ELSE 1 END""",
                          coll, printing_oid(pname), v_lang)
                if voz is None:
                    voz, _ = get_variant(coll, pname, v_lang, None, None)
                    stats["variants only JustTCG reports (created)"] += 1
                ext_id("CollectibleVariantExternalId", "CollectibleVariantOid", voz, "TCGplayer", jc.get("tcgplayerId"))
            cond = COND.get(jv.get("condition"))
            if cond is None:
                continue
            when = from_unix(jv["lastUpdated"]) if jv.get("lastUpdated") else datetime.now().replace(microsecond=0)
            hist = [(from_unix(h["t"]), h.get("p")) for h in (jv.get("priceHistory") or []) if h.get("t")]
            upsert_price(voz, "JustTCG", None, False, None, cond, None, jv.get("price"), None, None, "USD", when, None, hist)
    print(f"  JustTCG: {len(jcards)} entries {dict(matched)}")

    g = cached("justtcg_v2_graded_base.json") if set_id == "base1" else None
    for gc in ((g or {}).get("data") or []):
        ours = by_tcgplayer.get(str((gc.get("external_ids") or {}).get("tcgplayer") or ""))
        if not ours:
            continue
        for gv in gc.get("variants") or []:
            gr = gv.get("grading") or {}
            if gv.get("type") != "graded" or gr.get("grade") is None:
                continue
            pick = [o for o in ours if o[1] == split_printing(gv.get("printing"), None)[0]] or ours
            voz = sorted(pick, key=lambda o: (o[3] is not None, o[2] not in (None, "unlimited")))[0][0]
            goid = grade_oid(gr.get("company"), gr.get("grade"), gr.get("grade_label"), gr.get("canonical"))
            if goid is None:
                continue
            for m in gv.get("markets") or []:
                if m.get("region") != "US":
                    continue
                when = from_unix(m["updated_at"]) if m.get("updated_at") else datetime.now().replace(microsecond=0)
                hist = [(from_unix(h["t"]), h.get("p")) for h in (m.get("price_history") or []) if h.get("t")]
                upsert_price(voz, "JustTCG", None, True, goid, None, gr.get("qualifier"), m.get("price"), None, None, m.get("currency") or "USD", when, None, hist)


# ----------------------------------------------------------------------------- main
todo = []
if ARGS.sets:
    for sid in ARGS.sets.split(","):
        sid = sid.strip()
        todo.append((sid, "ja" if sid in ("SV6a",) or sid[:2].isupper() else "en"))
if ARGS.all_en:
    todo += [(x["id"], "en") for x in (tcgdex("en/sets", "tcgdex_sets_en.json", refresh=True) or [])]
if ARGS.all_ja:
    todo += [(x["id"], "ja") for x in (tcgdex("ja/sets", "tcgdex_sets_ja.json", refresh=True) or [])]

t0 = time.time()
game = get_game()
failed = []
for sid, lang in todo:
    try:
        import_set(game, sid, lang, WESTERN if lang == "en" else [])
        cn.commit()
    except Exception as e:
        cn.rollback()
        failed.append((sid, f"{type(e).__name__}: {str(e)[:300]}"))
        print(f"=== {sid}: FAILED and rolled back - {type(e).__name__}: {str(e)[:300]}", flush=True)

print(f"\n=== done in {int(time.time() - t0)}s | sets requested {len(todo)} | failed {len(failed)}")
for f in failed:
    print("  FAILED", f)
print("=== inserted / updated this run")
for k in sorted(stats):
    print(f"  {k}: {stats[k]}")
print("  JustTCG requests used this run:", jt_calls)
print("=== table totals")
for t in ("CardSet", "CardSetName", "Collectible", "CollectibleName", "CollectibleImage", "CollectibleVariant",
          "CollectibleExternalId", "CollectibleVariantExternalId", "CollectiblePrice", "CollectiblePriceHistory", "Printing", "Grade"):
    print(f"  {t}: {one(f'SELECT COUNT(*) FROM {t}')}")
cn.close()

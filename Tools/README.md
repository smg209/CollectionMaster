# Tools

Python scripts that fill and refresh the CollectionMaster database. They are development tools: they connect to
`localhost\SQL2025` with Windows authentication and need Python 3 with `pyodbc`.

Every script is safe to run again. Things are found by their source id first and inserted only when missing.

## refresh_daily.cmd - run once a day

Runs `refresh_tcgplayer.py`. This is what builds price history: each run overwrites current prices and adds a
history row wherever a price changed. The charts and the market movers only know what these runs stored.

## refresh_tcgplayer.py

TCGplayer prices and sealed products from TCGCSV (free, no key, updated once a day).

    python refresh_tcgplayer.py --cache <folder>
    python refresh_tcgplayer.py --cache <folder> --category 85 --only SV6a --no-sealed     (one Japanese set)

- `TCGplayer` prices are market prices (based on sales).
- `TCGplayer listings` prices exist only where there is no market price: the lowest asking price.
- Card products are matched to catalog cards by TCGplayer product id, or else by set + card number + name.
- `--show-misses` lists products that matched no card.

TCGCSV asks for a custom User-Agent, 100 ms between requests and no repeated downloads of the same file in a day.
The script does all three (the cache folder is what prevents repeats).

## import_catalog.py

Catalog, names in each language, pictures, versions, and TCGplayer + Cardmarket prices from TCGdex (free, no key).
Optionally prices by condition and by grade from JustTCG.

    python import_catalog.py --cache <folder> --all-en                    every English set (about 20 minutes the first time)
    python import_catalog.py --cache <folder> --all-en --refresh          fetch every card again (new Cardmarket prices, new cards)
    python import_catalog.py --cache <folder> --sets base1,sv08           named TCGdex set ids
    python import_catalog.py --cache <folder> --sets sv08 --justtcg 20    allow up to 20 JustTCG requests

- JustTCG needs the `JUSTTCG_API_KEY` environment variable. The free tier allows 100 requests a day and is for
  personal, non-commercial use. Without `--justtcg N` only cached JustTCG responses are used.
- Which sets get JustTCG prices is the `JUSTTCG_SETS` table at the top of the script.
- Pokemon TCG Pocket sets (digital only) are skipped.

## Sources and their terms

| Source | Used for | Key | Notes |
|---|---|---|---|
| TCGdex | catalog, names, pictures, relayed prices | none | pictures are linked, not copied; licence terms not yet confirmed |
| TCGCSV | TCGplayer prices, sealed products | none | its price archive is withdrawn, so history cannot be backfilled |
| JustTCG | prices by condition and grade | `JUSTTCG_API_KEY` | free tier non-commercial; paid plans allow storing prices |

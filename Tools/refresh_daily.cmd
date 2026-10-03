@echo off
rem Daily price refresh for CollectionMaster.
rem Reads today's TCGplayer prices for every Pokemon set from TCGCSV (about 450 small requests) and updates
rem CollectionMaster: current prices are overwritten, and a price-history row is added wherever a price changed.
rem Run it once a day (any time after TCGCSV's update at about 20:00 UTC) - the price charts and the market
rem movers are built from what these runs store.
rem
rem To run it automatically: Windows Task Scheduler, a daily task that starts this file.

setlocal
set CACHE=%TEMP%\CollectionMaster\tcgcsv
python -X utf8 "%~dp0refresh_tcgplayer.py" --cache "%CACHE%"
if errorlevel 1 echo The refresh failed - see the messages above.
endlocal

-- =============================================================================
-- CollectionMaster - Alter_002  (data only - no schema change, nothing to regenerate)
--
-- Condition.ValueFactor: replaces the rule-of-thumb values from Alter_001 with ratios
-- measured from real prices. For every version JustTCG prices in several conditions, each
-- condition's price was divided by the Near Mint price of the same version (Near Mint >= $1),
-- and the median taken. Measured 2026-10-03 on Base Set, Shrouded Fable and Night Wanderer:
--
--     condition   pairs   median   middle half of the cards
--     LP           139    0.7446   0.53 - 0.96
--     MP           127    0.4417   0.32 - 0.72
--     HP           108    0.2939   0.22 - 0.46
--     DMG          124    0.2457   0.17 - 0.51
--
-- The spread is wide, so a value built from these stays an ESTIMATE (the app labels it so),
-- and three sets is a small sample - measure again when more sets carry per-condition prices.
--
-- STATUS:
--   [x] Applied 2026-10-03 - localhost\SQL2025 (steven-evoc, dev)
-- =============================================================================

USE CollectionMaster;
GO

SET NOCOUNT ON;
GO

UPDATE Condition SET ValueFactor = 1.0000 WHERE Abbreviation = N'NM';
UPDATE Condition SET ValueFactor = 0.7500 WHERE Abbreviation = N'LP';
UPDATE Condition SET ValueFactor = 0.4500 WHERE Abbreviation = N'MP';
UPDATE Condition SET ValueFactor = 0.3000 WHERE Abbreviation = N'HP';
UPDATE Condition SET ValueFactor = 0.2500 WHERE Abbreviation = N'DMG';
GO

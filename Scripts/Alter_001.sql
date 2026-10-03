-- =============================================================================
-- CollectionMaster - Alter_001
--
-- 1. Condition.ValueFactor
--    How much of the Near Mint price a raw card in this condition is worth, as a fraction
--    (Near Mint = 1.0000). Used ONLY to estimate the value of an owned card when no data
--    source gives a price for that exact condition - a source's own per-condition price
--    always wins. The starting values are rules of thumb, not market data: change the rows
--    as real per-condition prices show what the ratios really are.
--
-- After running: regenerate GeneratedClasses_CM.cs in DBQ (Reload -> Generate) so the
-- Condition class picks up the new property. The app reads the column through SQL and
-- works without the regeneration.
--
-- STATUS:
--   [x] Applied 2026-10-03 - localhost\SQL2025 (steven-evoc, dev)
-- =============================================================================

USE CollectionMaster;
GO

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

IF COL_LENGTH('Condition', 'ValueFactor') IS NOT NULL
BEGIN
    RAISERROR('Alter_001.sql has already been applied (Condition.ValueFactor exists).', 16, 1);
    SET NOEXEC ON;
END
GO

ALTER TABLE Condition ADD ValueFactor DECIMAL(5,4) NULL;
GO

UPDATE Condition SET ValueFactor = 1.0000 WHERE Abbreviation = N'NM';
UPDATE Condition SET ValueFactor = 0.8000 WHERE Abbreviation = N'LP';
UPDATE Condition SET ValueFactor = 0.6500 WHERE Abbreviation = N'MP';
UPDATE Condition SET ValueFactor = 0.4500 WHERE Abbreviation = N'HP';
UPDATE Condition SET ValueFactor = 0.3000 WHERE Abbreviation = N'DMG';
GO

SET NOEXEC OFF;
GO

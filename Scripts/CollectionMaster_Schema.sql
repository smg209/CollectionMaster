-- =============================================================================
-- CollectionMaster - initial schema (creation script)
--
-- FROZEN. This script has been run (see STATUS). Every later change goes into its own
-- numbered script: Alter_001.sql, Alter_002.sql, ...
--
-- Tables:
--   Tenant and user
--     1. Collector                    The tenant. One row per account owner; every owned-data
--                                     table carries CollectorOid. Linked to Authentication.Tenant.
--     2. UserPreferences              One row per signed-in user (theme etc.), same shape as ETS/SMG.
--   Reference data (global)
--     3. DataSource                   An outside system we take data from (prices, catalog, certs).
--     4. GradingCompany               PSA, BGS, CGC, ...
--     5. Grade                        Each grading company's own scale, one row per grade.
--     6. Condition                    Condition of a raw (ungraded) item.
--     7. Printing                     Normal, Holofoil, Reverse Holofoil, ...
--     8. Language                     Language a card is printed in.
--   Catalog (global)
--     9. CollectibleType              Top of the catalog: "Trading Card Game" today.
--    10. Game                         Pokemon, Magic: The Gathering, ...
--    11. CardSet                      A set within a game.
--    12. CardSetName                  The set's name in each language it was released in.
--    13. CardSetExternalId            A data source's own id for a CardSet.
--    14. Collectible                  THE CARD: one row per card in a set.
--    15. CollectibleName              The card's name in each language it was printed in.
--    16. CollectibleImage             A picture of the card, per language and source.
--    17. CollectibleVariant           A VERSION of the card: one printing in one language.
--                                     This is the thing a person owns, a grader certifies and
--                                     a price is quoted for.
--    18. CollectibleExternalId        A data source's own id for a Collectible.
--    19. CollectibleVariantExternalId A data source's own id for a CollectibleVariant.
--    20. Cert                         One graded, slabbed item as its grader certified it.
--    21. CollectiblePrice             The CURRENT price of a variant from one source, for one
--                                     grade (graded) or one condition (raw).
--    22. CollectiblePriceHistory      Every price that source has shown for that row, over time.
--   Owned data (tenant-scoped)
--    23. CollectionItem               One physical item a Collector owns.
--
-- Only Collector, UserPreferences and CollectionItem are per-collector. Everything else is
-- shared: a card's identity, grade, population and market price are the same facts for
-- every collector. Only ownership is tenant data.
--
-- Scope: trading card games, Pokemon first. The one piece of scaffolding for anything else
-- is CollectibleType - a different kind of collectible would be a new row there with its own
-- Game / CardSet rows beneath it, not a new set of tables.
--
-- Languages (from the TCGdex / JustTCG probe, 2026-10-02):
--   - The western languages share one set and one card list (Base Set is the same 102 cards
--     in English, French, German, ...). That is ONE CardSet and ONE Collectible per card,
--     with a CardSetName / CollectibleName row per language and a CollectibleVariant per
--     printing + language.
--   - Japanese (and the other Asian releases) are different sets with different card lists.
--     Those are their own CardSet and Collectible rows.
--
-- Not in this script yet (each needs a decision or a real API payload first):
--   sealed products, population-by-grade snapshots, API call log.
--
-- Cross-database references (TenantOid, UserAuthOid*) are plain BIGINT with no FK
-- constraint - SQL Server cannot enforce a foreign key into the Authentication database.
--
-- Every foreign key is ON DELETE NO ACTION except CollectiblePriceHistory -> CollectiblePrice.
--
-- STATUS:
--   [x] Applied 2026-10-02 - localhost\SQL2025 (steven-evoc, dev)
-- =============================================================================

USE CollectionMaster;
GO

SET NOCOUNT ON;
-- Required by the filtered indexes below. SSMS turns these on by default; sqlcmd does not.
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- This script creates everything from nothing. If any of its tables already exist it has
-- been run before (or the database is not empty): stop and compile the rest without executing.
IF EXISTS (SELECT 1 FROM sys.tables WHERE name IN (
    'Collector', 'UserPreferences', 'DataSource', 'GradingCompany', 'Grade', 'Condition', 'Printing', 'Language',
    'CollectibleType', 'Game', 'CardSet', 'CardSetName', 'CardSetExternalId', 'Collectible', 'CollectibleName',
    'CollectibleImage', 'CollectibleVariant', 'CollectibleExternalId',
    'CollectibleVariantExternalId', 'Cert', 'CollectiblePrice', 'CollectiblePriceHistory', 'CollectionItem'))
BEGIN
    RAISERROR('CollectionMaster_Schema.sql: one or more of its tables already exist. This script runs once; use an Alter script for changes.', 16, 1);
    SET NOEXEC ON;
END
GO

-- =============================================================================
-- TENANT AND USER
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 1. Collector - the tenant
-- -----------------------------------------------------------------------------
CREATE TABLE Collector (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    TenantOid               BIGINT          NOT NULL,   -- Authentication.Tenant.Oid (cross-database, no FK)
    DisplayName             NVARCHAR(100)   NOT NULL CONSTRAINT DF_Collector_DisplayName DEFAULT (''),
    IsActive                BIT             NOT NULL CONSTRAINT DF_Collector_IsActive DEFAULT (1),
    CreatedOn               DATETIME        NOT NULL CONSTRAINT DF_Collector_CreatedOn DEFAULT (GETDATE()),
    ModifiedOn              DATETIME        NULL,
    UserAuthOid_CreatedBy   BIGINT          NULL,       -- Authentication.UserAuth.Oid (cross-database, no FK)
    UserAuthOid_ModifiedBy  BIGINT          NULL,       -- Authentication.UserAuth.Oid (cross-database, no FK)
    CONSTRAINT PK_Collector PRIMARY KEY CLUSTERED (Oid)
);
GO

-- One Collector per Authentication tenant. Also the index the sign-in lookup uses.
CREATE UNIQUE INDEX UQ_Collector_TenantOid ON Collector (TenantOid);
GO

-- -----------------------------------------------------------------------------
-- 2. UserPreferences - per-user settings blob
-- -----------------------------------------------------------------------------
CREATE TABLE UserPreferences (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    UserAuthOid             BIGINT          NOT NULL,   -- Authentication.UserAuth.Oid (cross-database, no FK)
    PreferencesJSON         NVARCHAR(MAX)   NULL,
    CONSTRAINT PK_UserPreferences PRIMARY KEY CLUSTERED (Oid)
);
GO

-- One row per user: a second row for the same user could never be found again.
CREATE UNIQUE INDEX UQ_UserPreferences_UserAuthOid ON UserPreferences (UserAuthOid);
GO

-- =============================================================================
-- REFERENCE DATA (global)
-- Tables rather than code enums, because each of these lists is expected to grow and to
-- pick up its own attributes over time.
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 3. DataSource - an outside system we take data from
--    One row per system, whatever it supplies: prices, catalog data, cert lookups, ids.
-- -----------------------------------------------------------------------------
CREATE TABLE DataSource (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    Name                    NVARCHAR(100)   NOT NULL,
    Url                     NVARCHAR(300)   NULL,       -- the source's site, for attribution / link-back
    IsActive                BIT             NOT NULL CONSTRAINT DF_DataSource_IsActive DEFAULT (1),
    CreatedOn               DATETIME        NOT NULL CONSTRAINT DF_DataSource_CreatedOn DEFAULT (GETDATE()),
    ModifiedOn              DATETIME        NULL,
    CONSTRAINT PK_DataSource PRIMARY KEY CLUSTERED (Oid)
);
GO

CREATE UNIQUE INDEX UQ_DataSource_Name ON DataSource (Name);
GO

-- -----------------------------------------------------------------------------
-- 4. GradingCompany
-- -----------------------------------------------------------------------------
CREATE TABLE GradingCompany (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    Name                    NVARCHAR(100)   NOT NULL,
    Abbreviation            NVARCHAR(20)    NOT NULL,   -- "PSA"
    Url                     NVARCHAR(300)   NULL,
    IsActive                BIT             NOT NULL CONSTRAINT DF_GradingCompany_IsActive DEFAULT (1),
    CONSTRAINT PK_GradingCompany PRIMARY KEY CLUSTERED (Oid)
);
GO

CREATE UNIQUE INDEX UQ_GradingCompany_Abbreviation ON GradingCompany (Abbreviation);
GO

-- -----------------------------------------------------------------------------
-- 5. Grade - one row per grade on one company's scale
--    Every company defines its own scale, so a grade always belongs to a company.
--    Code is the grade as printed on the label ("10", "8.5", "A"). A special label that
--    prices differently is its own row (for example a BGS 10 "Black Label").
--    GradeValue is the same grade as a number, for sorting and for comparing across
--    companies; NULL when the grade is not numeric (for example Authentic).
-- -----------------------------------------------------------------------------
CREATE TABLE Grade (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    GradingCompanyOid       BIGINT          NOT NULL,
    Code                    NVARCHAR(20)    NOT NULL,
    Name                    NVARCHAR(100)   NOT NULL,   -- "GEM-MT", "NM-MT+", "Authentic"
    GradeValue              DECIMAL(4,1)    NULL,
    SortOrder               INT             NOT NULL CONSTRAINT DF_Grade_SortOrder DEFAULT (0),
    IsActive                BIT             NOT NULL CONSTRAINT DF_Grade_IsActive DEFAULT (1),
    CONSTRAINT PK_Grade PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_Grade_GradingCompany FOREIGN KEY (GradingCompanyOid) REFERENCES GradingCompany (Oid) ON DELETE NO ACTION
);
GO

-- A code appears once per company. Leads with GradingCompanyOid, so it is also that FK's index.
CREATE UNIQUE INDEX UQ_Grade_GradingCompanyOid_Code ON Grade (GradingCompanyOid, Code);
GO

-- -----------------------------------------------------------------------------
-- 6. Condition - condition of a raw (ungraded) item
-- -----------------------------------------------------------------------------
CREATE TABLE Condition (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    Name                    NVARCHAR(100)   NOT NULL,   -- "Near Mint"
    Abbreviation            NVARCHAR(20)    NULL,       -- "NM"
    SortOrder               INT             NOT NULL CONSTRAINT DF_Condition_SortOrder DEFAULT (0),
    IsActive                BIT             NOT NULL CONSTRAINT DF_Condition_IsActive DEFAULT (1),
    CONSTRAINT PK_Condition PRIMARY KEY CLUSTERED (Oid)
);
GO

CREATE UNIQUE INDEX UQ_Condition_Name ON Condition (Name);
GO

-- -----------------------------------------------------------------------------
-- 7. Printing - how a card was printed
--    The list differs by game (Holofoil, Reverse Holofoil, 1st Edition, ...); rows are
--    added as data sources report them.
-- -----------------------------------------------------------------------------
CREATE TABLE Printing (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    Name                    NVARCHAR(100)   NOT NULL,
    SortOrder               INT             NOT NULL CONSTRAINT DF_Printing_SortOrder DEFAULT (0),
    IsActive                BIT             NOT NULL CONSTRAINT DF_Printing_IsActive DEFAULT (1),
    CONSTRAINT PK_Printing PRIMARY KEY CLUSTERED (Oid)
);
GO

CREATE UNIQUE INDEX UQ_Printing_Name ON Printing (Name);
GO

-- -----------------------------------------------------------------------------
-- 8. Language - language a card is printed in
-- -----------------------------------------------------------------------------
CREATE TABLE [Language] (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    Name                    NVARCHAR(100)   NOT NULL,   -- "English"
    Code                    NVARCHAR(10)    NULL,       -- "en"
    IsActive                BIT             NOT NULL CONSTRAINT DF_Language_IsActive DEFAULT (1),
    CONSTRAINT PK_Language PRIMARY KEY CLUSTERED (Oid)
);
GO

CREATE UNIQUE INDEX UQ_Language_Name ON [Language] (Name);
GO

-- =============================================================================
-- CATALOG (global)
-- CollectibleType > Game > CardSet > Collectible > CollectibleVariant
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 9. CollectibleType - the kind of collectible a game belongs to
--    The only scaffolding for growth beyond trading card games. Nothing reads it yet.
-- -----------------------------------------------------------------------------
CREATE TABLE CollectibleType (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    Name                    NVARCHAR(100)   NOT NULL,
    IsActive                BIT             NOT NULL CONSTRAINT DF_CollectibleType_IsActive DEFAULT (1),
    CONSTRAINT PK_CollectibleType PRIMARY KEY CLUSTERED (Oid)
);
GO

CREATE UNIQUE INDEX UQ_CollectibleType_Name ON CollectibleType (Name);
GO

-- -----------------------------------------------------------------------------
-- 10. Game
-- -----------------------------------------------------------------------------
CREATE TABLE Game (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    CollectibleTypeOid      BIGINT          NOT NULL,
    Name                    NVARCHAR(150)   NOT NULL,
    IsActive                BIT             NOT NULL CONSTRAINT DF_Game_IsActive DEFAULT (1),
    CONSTRAINT PK_Game PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_Game_CollectibleType FOREIGN KEY (CollectibleTypeOid) REFERENCES CollectibleType (Oid) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX UQ_Game_Name ON Game (Name);
GO

CREATE INDEX IX_Game_CollectibleTypeOid ON Game (CollectibleTypeOid);
GO

-- -----------------------------------------------------------------------------
-- 11. CardSet - a set within a game
--    Name is the set's name in its primary language (LanguageOid_Primary): English for a
--    set released internationally, Japanese for a Japan-only set. The names in every
--    language are in CardSetName.
-- -----------------------------------------------------------------------------
CREATE TABLE CardSet (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    GameOid                 BIGINT          NOT NULL,
    Name                    NVARCHAR(200)   NOT NULL,
    Code                    NVARCHAR(50)    NULL,       -- the set's short code, where the game uses one ("BS", "SFA")
    Series                  NVARCHAR(200)   NULL,       -- the era the set belongs to ("Base", "Scarlet & Violet"), as the source names it
    LanguageOid_Primary     BIGINT          NOT NULL,   -- the language Name is written in
    ReleasedOn              DATETIME        NULL,
    CardCountOfficial       INT             NULL,       -- the number printed on the cards ("/102")
    CardCountTotal          INT             NULL,       -- including secret rares beyond the official count
    LogoUrl                 NVARCHAR(500)   NULL,
    SymbolUrl               NVARCHAR(500)   NULL,
    CreatedOn               DATETIME        NOT NULL CONSTRAINT DF_CardSet_CreatedOn DEFAULT (GETDATE()),
    ModifiedOn              DATETIME        NULL,
    CONSTRAINT PK_CardSet PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_CardSet_Game             FOREIGN KEY (GameOid)             REFERENCES Game (Oid)       ON DELETE NO ACTION,
    CONSTRAINT FK_CardSet_Language_Primary FOREIGN KEY (LanguageOid_Primary) REFERENCES [Language] (Oid) ON DELETE NO ACTION
);
GO

-- A set name appears once per game. Leads with GameOid, so it is also that FK's index.
CREATE UNIQUE INDEX UQ_CardSet_GameOid_Name ON CardSet (GameOid, Name);
GO

CREATE INDEX IX_CardSet_LanguageOid_Primary ON CardSet (LanguageOid_Primary);
GO

-- -----------------------------------------------------------------------------
-- 12. CardSetName - the set's name in one language
--    One row per language the set was released in, INCLUDING the primary language. The
--    rows also answer "which languages does this set exist in".
-- -----------------------------------------------------------------------------
CREATE TABLE CardSetName (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    CardSetOid              BIGINT          NOT NULL,
    LanguageOid             BIGINT          NOT NULL,
    Name                    NVARCHAR(200)   NOT NULL,
    CONSTRAINT PK_CardSetName PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_CardSetName_CardSet  FOREIGN KEY (CardSetOid)  REFERENCES CardSet (Oid)    ON DELETE NO ACTION,
    CONSTRAINT FK_CardSetName_Language FOREIGN KEY (LanguageOid) REFERENCES [Language] (Oid) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX UQ_CardSetName_CardSetOid_LanguageOid ON CardSetName (CardSetOid, LanguageOid);
GO

CREATE INDEX IX_CardSetName_LanguageOid ON CardSetName (LanguageOid);
GO

-- -----------------------------------------------------------------------------
-- 13. CardSetExternalId - a data source's own id for a set
-- -----------------------------------------------------------------------------
CREATE TABLE CardSetExternalId (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    CardSetOid              BIGINT          NOT NULL,
    DataSourceOid           BIGINT          NOT NULL,
    ExternalId              NVARCHAR(100)   NOT NULL,
    CONSTRAINT PK_CardSetExternalId PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_CardSetExternalId_CardSet    FOREIGN KEY (CardSetOid)    REFERENCES CardSet (Oid)    ON DELETE NO ACTION,
    CONSTRAINT FK_CardSetExternalId_DataSource FOREIGN KEY (DataSourceOid) REFERENCES DataSource (Oid) ON DELETE NO ACTION
);
GO

-- A source's id points at exactly one set. Leads with DataSourceOid, so it is also that FK's index.
CREATE UNIQUE INDEX UQ_CardSetExternalId_DataSource_ExternalId ON CardSetExternalId (DataSourceOid, ExternalId);
GO

CREATE INDEX IX_CardSetExternalId_CardSetOid ON CardSetExternalId (CardSetOid);
GO

-- -----------------------------------------------------------------------------
-- 14. Collectible - the card
--    One row per card in a set, regardless of how many versions of it were printed.
--    Name is the card's name in its set's primary language; every language is in CollectibleName.
--    Rarity and Category are kept as the source's own text: the vocabulary differs for every game.
-- -----------------------------------------------------------------------------
CREATE TABLE Collectible (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    CardSetOid              BIGINT          NOT NULL,
    Name                    NVARCHAR(300)   NOT NULL,
    CardNumber              NVARCHAR(50)    NULL,
    Rarity                  NVARCHAR(100)   NULL,
    Category                NVARCHAR(100)   NULL,       -- "Pokemon", "Trainer", "Energy"
    Illustrator             NVARCHAR(200)   NULL,
    CreatedOn               DATETIME        NOT NULL CONSTRAINT DF_Collectible_CreatedOn DEFAULT (GETDATE()),
    ModifiedOn              DATETIME        NULL,
    CONSTRAINT PK_Collectible PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_Collectible_CardSet FOREIGN KEY (CardSetOid) REFERENCES CardSet (Oid) ON DELETE NO ACTION
);
GO

-- Cards of a set, in card-number order. Not unique: some sets reuse a number.
CREATE INDEX IX_Collectible_CardSetOid_CardNumber ON Collectible (CardSetOid, CardNumber);
GO

-- Catalog search by name.
CREATE INDEX IX_Collectible_Name ON Collectible (Name);
GO

-- -----------------------------------------------------------------------------
-- 15. CollectibleName - the card's name in one language
--    One row per language the card was printed in, INCLUDING the primary language.
-- -----------------------------------------------------------------------------
CREATE TABLE CollectibleName (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    CollectibleOid          BIGINT          NOT NULL,
    LanguageOid             BIGINT          NOT NULL,
    Name                    NVARCHAR(300)   NOT NULL,
    CONSTRAINT PK_CollectibleName PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_CollectibleName_Collectible FOREIGN KEY (CollectibleOid) REFERENCES Collectible (Oid) ON DELETE NO ACTION,
    CONSTRAINT FK_CollectibleName_Language    FOREIGN KEY (LanguageOid)    REFERENCES [Language] (Oid)  ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX UQ_CollectibleName_CollectibleOid_LanguageOid ON CollectibleName (CollectibleOid, LanguageOid);
GO

CREATE INDEX IX_CollectibleName_LanguageOid ON CollectibleName (LanguageOid);
GO

-- Catalog search by name in any language.
CREATE INDEX IX_CollectibleName_Name ON CollectibleName (Name);
GO

-- -----------------------------------------------------------------------------
-- 16. CollectibleImage - a picture of the card
--    Per card, per language (the printed text differs) and per source. The URLs point at
--    the source's own image host; nothing is copied here.
-- -----------------------------------------------------------------------------
CREATE TABLE CollectibleImage (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    CollectibleOid          BIGINT          NOT NULL,
    LanguageOid             BIGINT          NOT NULL,
    DataSourceOid           BIGINT          NOT NULL,
    ImageUrl                NVARCHAR(500)   NOT NULL,   -- full size
    ThumbnailUrl            NVARCHAR(500)   NULL,       -- small, for grids
    CreatedOn               DATETIME        NOT NULL CONSTRAINT DF_CollectibleImage_CreatedOn DEFAULT (GETDATE()),
    CONSTRAINT PK_CollectibleImage PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_CollectibleImage_Collectible FOREIGN KEY (CollectibleOid) REFERENCES Collectible (Oid) ON DELETE NO ACTION,
    CONSTRAINT FK_CollectibleImage_Language    FOREIGN KEY (LanguageOid)    REFERENCES [Language] (Oid)  ON DELETE NO ACTION,
    CONSTRAINT FK_CollectibleImage_DataSource  FOREIGN KEY (DataSourceOid)  REFERENCES DataSource (Oid)  ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX UQ_CollectibleImage_Collectible_Language_DataSource ON CollectibleImage (CollectibleOid, LanguageOid, DataSourceOid);
GO

CREATE INDEX IX_CollectibleImage_LanguageOid ON CollectibleImage (LanguageOid);
GO

CREATE INDEX IX_CollectibleImage_DataSourceOid ON CollectibleImage (DataSourceOid);
GO

-- -----------------------------------------------------------------------------
-- 17. CollectibleVariant - one version of the card
--    The same card in a given printing and language. Subtype and Stamp split a printing
--    further where the print run matters - Base Set Charizard exists as Holofoil unlimited,
--    Holofoil shadowless, and Holofoil shadowless with a 1st Edition stamp. Both are kept as
--    the source's own text until the vocabulary is known. Condition and grade are NOT part of
--    the variant: they describe one physical copy (CollectionItem, Cert) or one price
--    (CollectiblePrice), not the version.
-- -----------------------------------------------------------------------------
CREATE TABLE CollectibleVariant (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    CollectibleOid          BIGINT          NOT NULL,
    PrintingOid             BIGINT          NOT NULL,
    LanguageOid             BIGINT          NOT NULL,
    Subtype                 NVARCHAR(50)    NULL,       -- "unlimited", "shadowless", ...
    Stamp                   NVARCHAR(100)   NULL,       -- "1st-edition", "pre-release", ... (comma separated when several)
    CreatedOn               DATETIME        NOT NULL CONSTRAINT DF_CollectibleVariant_CreatedOn DEFAULT (GETDATE()),
    ModifiedOn              DATETIME        NULL,
    CONSTRAINT PK_CollectibleVariant PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_CollectibleVariant_Collectible FOREIGN KEY (CollectibleOid) REFERENCES Collectible (Oid) ON DELETE NO ACTION,
    CONSTRAINT FK_CollectibleVariant_Printing    FOREIGN KEY (PrintingOid)    REFERENCES Printing (Oid)    ON DELETE NO ACTION,
    CONSTRAINT FK_CollectibleVariant_Language    FOREIGN KEY (LanguageOid)    REFERENCES [Language] (Oid)  ON DELETE NO ACTION
);
GO

-- One row per card + printing + language + subtype + stamp (NULLs count as equal). Leads
-- with CollectibleOid, so it is also the index for "all versions of this card".
CREATE UNIQUE INDEX UQ_CollectibleVariant_Collectible_Printing_Language_Subtype_Stamp ON CollectibleVariant (CollectibleOid, PrintingOid, LanguageOid, Subtype, Stamp);
GO

CREATE INDEX IX_CollectibleVariant_PrintingOid ON CollectibleVariant (PrintingOid);
GO

CREATE INDEX IX_CollectibleVariant_LanguageOid ON CollectibleVariant (LanguageOid);
GO

-- -----------------------------------------------------------------------------
-- 18. CollectibleExternalId - a data source's own id for a card
--    For ids a source assigns at card level (a product id, a card uuid). One card can
--    carry an id from every source; this is how a later refresh finds the same card again.
-- -----------------------------------------------------------------------------
CREATE TABLE CollectibleExternalId (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    CollectibleOid          BIGINT          NOT NULL,
    DataSourceOid           BIGINT          NOT NULL,
    ExternalId              NVARCHAR(100)   NOT NULL,
    CONSTRAINT PK_CollectibleExternalId PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_CollectibleExternalId_Collectible FOREIGN KEY (CollectibleOid) REFERENCES Collectible (Oid) ON DELETE NO ACTION,
    CONSTRAINT FK_CollectibleExternalId_DataSource  FOREIGN KEY (DataSourceOid)  REFERENCES DataSource (Oid)  ON DELETE NO ACTION
);
GO

-- A source's id points at exactly one card. Leads with DataSourceOid, so it is also that FK's index.
CREATE UNIQUE INDEX UQ_CollectibleExternalId_DataSource_ExternalId ON CollectibleExternalId (DataSourceOid, ExternalId);
GO

CREATE INDEX IX_CollectibleExternalId_CollectibleOid ON CollectibleExternalId (CollectibleOid);
GO

-- -----------------------------------------------------------------------------
-- 19. CollectibleVariantExternalId - a data source's own id for a version
--    For ids a source assigns at version level. PSA's SpecID is one: once a spec has been
--    matched to a variant here, every later cert with that spec matches automatically.
--    Not unique per source id: a TCGplayer product id covers every printing of a card, so
--    the Normal and Reverse Holofoil variants carry the same id.
-- -----------------------------------------------------------------------------
CREATE TABLE CollectibleVariantExternalId (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    CollectibleVariantOid   BIGINT          NOT NULL,
    DataSourceOid           BIGINT          NOT NULL,
    ExternalId              NVARCHAR(100)   NOT NULL,
    CONSTRAINT PK_CollectibleVariantExternalId PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_CollectibleVariantExternalId_CollectibleVariant FOREIGN KEY (CollectibleVariantOid) REFERENCES CollectibleVariant (Oid) ON DELETE NO ACTION,
    CONSTRAINT FK_CollectibleVariantExternalId_DataSource         FOREIGN KEY (DataSourceOid)         REFERENCES DataSource (Oid)         ON DELETE NO ACTION
);
GO

-- Leads with DataSourceOid, so it is also that FK's index and the lookup "which variants carry this id".
CREATE UNIQUE INDEX UQ_CollectibleVariantExternalId_DataSource_ExternalId_Variant ON CollectibleVariantExternalId (DataSourceOid, ExternalId, CollectibleVariantOid);
GO

CREATE INDEX IX_CollectibleVariantExternalId_CollectibleVariantOid ON CollectibleVariantExternalId (CollectibleVariantOid);
GO

-- -----------------------------------------------------------------------------
-- 20. Cert - one graded item, as certified
--    A cert number is only unique within its grading company.
--
--    The grader describes the card in its own words, and nothing in that description is a
--    key into our catalog. So the description is stored here exactly as returned (SpecID
--    through Variety, CardGrade, GradeDescription), and the two links into our own data are
--    filled in when a match is made:
--      CollectibleVariantOid  NULL until the card has been matched to the catalog
--      GradeOid               NULL until the grade has been matched to the company's scale
--
--    Qualifier is the grader's defect marker, when there is one ("OC", "MK", ...).
--    The population columns are a snapshot taken at LastVerifiedOn - they change over time.
--    LastVerifiedOn is what lets the app reuse a stored cert instead of spending an API call.
-- -----------------------------------------------------------------------------
CREATE TABLE Cert (
    Oid                             BIGINT IDENTITY(1,1) NOT NULL,
    GradingCompanyOid               BIGINT          NOT NULL,
    CertNumber                      NVARCHAR(30)    NOT NULL,
    CollectibleVariantOid           BIGINT          NULL,
    GradeOid                        BIGINT          NULL,
    Qualifier                       NVARCHAR(20)    NULL,
    SpecID                          INT             NULL,
    SpecNumber                      NVARCHAR(50)    NULL,
    [Year]                          NVARCHAR(20)    NULL,
    Brand                           NVARCHAR(200)   NULL,
    Category                        NVARCHAR(100)   NULL,
    CardNumber                      NVARCHAR(50)    NULL,
    Subject                         NVARCHAR(300)   NULL,
    Variety                         NVARCHAR(300)   NULL,
    CardGrade                       NVARCHAR(50)    NULL,
    GradeDescription                NVARCHAR(100)   NULL,
    AutographGrade                  NVARCHAR(50)    NULL,
    LabelType                       NVARCHAR(50)    NULL,
    IsReverseBarCode                BIT             NULL,
    IsPSADNA                        BIT             NULL,
    IsDualCert                      BIT             NULL,
    TotalPopulation                 INT             NULL,
    TotalPopulationWithQualifier    INT             NULL,
    PopulationHigher                INT             NULL,
    ItemStatus                      NVARCHAR(50)    NULL,
    LastVerifiedOn                  DATETIME        NULL,
    CreatedOn                       DATETIME        NOT NULL CONSTRAINT DF_Cert_CreatedOn DEFAULT (GETDATE()),
    ModifiedOn                      DATETIME        NULL,
    CONSTRAINT PK_Cert PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_Cert_GradingCompany     FOREIGN KEY (GradingCompanyOid)     REFERENCES GradingCompany (Oid)     ON DELETE NO ACTION,
    CONSTRAINT FK_Cert_CollectibleVariant FOREIGN KEY (CollectibleVariantOid) REFERENCES CollectibleVariant (Oid) ON DELETE NO ACTION,
    CONSTRAINT FK_Cert_Grade              FOREIGN KEY (GradeOid)              REFERENCES Grade (Oid)              ON DELETE NO ACTION
);
GO

-- Leads with GradingCompanyOid, so it is also that FK's index.
CREATE UNIQUE INDEX UQ_Cert_GradingCompanyOid_CertNumber ON Cert (GradingCompanyOid, CertNumber);
GO

CREATE INDEX IX_Cert_CollectibleVariantOid ON Cert (CollectibleVariantOid) WHERE CollectibleVariantOid IS NOT NULL;
GO

CREATE INDEX IX_Cert_GradeOid ON Cert (GradeOid) WHERE GradeOid IS NOT NULL;
GO

-- Finds the certs still waiting to be matched once a spec has been matched to a variant.
CREATE INDEX IX_Cert_SpecID ON Cert (SpecID) WHERE SpecID IS NOT NULL;
GO

-- -----------------------------------------------------------------------------
-- 21. CollectiblePrice - the CURRENT price of a variant
--    One row per variant + source + (grade when graded | condition when raw) + qualifier.
--    So one variant can hold a graded price from source A, a raw price from source B and
--    another graded price from source C at the same time - but never two current rows for
--    the same combination. Reading the current price is a plain lookup of this row.
--
--    A graded price must say which grade (and through Grade, which company): "graded"
--    alone cannot be compared with a slab someone owns. A raw price may carry a condition.
--
--    DataSourceOid is WHOSE price it is (JustTCG, TCGplayer, Cardmarket).
--    DataSourceOid_RetrievedFrom is the system it was fetched through when that is a
--    different one (TCGdex relays TCGplayer and Cardmarket prices); NULL when fetched direct.
--
--    LastConfirmed is the last time the source was checked and still showed this price.
--    When a refresh finds a different price, this row is updated in place and the change
--    is recorded in CollectiblePriceHistory.
-- -----------------------------------------------------------------------------
CREATE TABLE CollectiblePrice (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    CollectibleVariantOid   BIGINT          NOT NULL,
    DataSourceOid           BIGINT          NOT NULL,
    DataSourceOid_RetrievedFrom BIGINT      NULL,
    IsGraded                BIT             NOT NULL,
    GradeOid                BIGINT          NULL,       -- required when IsGraded = 1
    ConditionOid            BIGINT          NULL,       -- only when IsGraded = 0
    Qualifier               NVARCHAR(20)    NULL,       -- only when IsGraded = 1
    Price                   DECIMAL(12,2)   NOT NULL,   -- the headline figure (market price where the source gives one)
    LowPrice                DECIMAL(12,2)   NULL,
    HighPrice               DECIMAL(12,2)   NULL,
    CurrencyCode            NVARCHAR(3)     NOT NULL CONSTRAINT DF_CollectiblePrice_CurrencyCode DEFAULT ('USD'),
    LastConfirmed           DATETIME        NOT NULL,
    SourceUrl               NVARCHAR(500)   NULL,       -- the exact page or listing, when there is one
    CreatedOn               DATETIME        NOT NULL CONSTRAINT DF_CollectiblePrice_CreatedOn DEFAULT (GETDATE()),
    ModifiedOn              DATETIME        NULL,
    CONSTRAINT PK_CollectiblePrice PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_CollectiblePrice_CollectibleVariant FOREIGN KEY (CollectibleVariantOid) REFERENCES CollectibleVariant (Oid) ON DELETE NO ACTION,
    CONSTRAINT FK_CollectiblePrice_DataSource         FOREIGN KEY (DataSourceOid)         REFERENCES DataSource (Oid)         ON DELETE NO ACTION,
    CONSTRAINT FK_CollectiblePrice_DataSource_RetrievedFrom FOREIGN KEY (DataSourceOid_RetrievedFrom) REFERENCES DataSource (Oid) ON DELETE NO ACTION,
    CONSTRAINT FK_CollectiblePrice_Grade              FOREIGN KEY (GradeOid)              REFERENCES Grade (Oid)              ON DELETE NO ACTION,
    CONSTRAINT FK_CollectiblePrice_Condition          FOREIGN KEY (ConditionOid)          REFERENCES Condition (Oid)          ON DELETE NO ACTION,
    CONSTRAINT CK_CollectiblePrice_GradedOrRaw CHECK (
        (IsGraded = 1 AND GradeOid IS NOT NULL AND ConditionOid IS NULL)
     OR (IsGraded = 0 AND GradeOid IS NULL AND Qualifier IS NULL))
);
GO

-- The "one current row" rule. SQL Server treats NULLs as equal in a unique index, so this
-- also allows exactly one raw row with no condition. Leads with CollectibleVariantOid, so
-- it is also the index for "all current prices of this version".
CREATE UNIQUE INDEX UQ_CollectiblePrice_Variant_Source_Grade_Condition_Qualifier
    ON CollectiblePrice (CollectibleVariantOid, DataSourceOid, GradeOid, ConditionOid, Qualifier);
GO

CREATE INDEX IX_CollectiblePrice_DataSourceOid ON CollectiblePrice (DataSourceOid);
GO

CREATE INDEX IX_CollectiblePrice_DataSourceOid_RetrievedFrom ON CollectiblePrice (DataSourceOid_RetrievedFrom) WHERE DataSourceOid_RetrievedFrom IS NOT NULL;
GO

CREATE INDEX IX_CollectiblePrice_GradeOid ON CollectiblePrice (GradeOid) WHERE GradeOid IS NOT NULL;
GO

CREATE INDEX IX_CollectiblePrice_ConditionOid ON CollectiblePrice (ConditionOid) WHERE ConditionOid IS NOT NULL;
GO

-- -----------------------------------------------------------------------------
-- 22. CollectiblePriceHistory - how one CollectiblePrice has moved over time
--    One row per price the source has shown for that CollectiblePrice, INCLUDING the
--    current one. FirstConfirmed..LastConfirmed is the span over which the source kept
--    showing that price:
--      - a refresh that finds the same price moves LastConfirmed forward on the newest row;
--      - a refresh that finds a new price adds a row.
--    History is only read when someone asks for it; the current price never needs it.
--    Deleting a CollectiblePrice deletes its history (the one cascade in this schema).
-- -----------------------------------------------------------------------------
CREATE TABLE CollectiblePriceHistory (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    CollectiblePriceOid     BIGINT          NOT NULL,
    Price                   DECIMAL(12,2)   NOT NULL,
    LowPrice                DECIMAL(12,2)   NULL,
    HighPrice               DECIMAL(12,2)   NULL,
    CurrencyCode            NVARCHAR(3)     NOT NULL CONSTRAINT DF_CollectiblePriceHistory_CurrencyCode DEFAULT ('USD'),
    FirstConfirmed          DATETIME        NOT NULL,
    LastConfirmed           DATETIME        NOT NULL,
    CONSTRAINT PK_CollectiblePriceHistory PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_CollectiblePriceHistory_CollectiblePrice FOREIGN KEY (CollectiblePriceOid) REFERENCES CollectiblePrice (Oid) ON DELETE CASCADE
);
GO

-- The timeline for one price, in order.
CREATE INDEX IX_CollectiblePriceHistory_CollectiblePriceOid_FirstConfirmed ON CollectiblePriceHistory (CollectiblePriceOid, FirstConfirmed);
GO

-- =============================================================================
-- OWNED DATA (tenant-scoped)
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 23. CollectionItem - what a Collector owns
--    A raw item:    CollectibleVariantOid set, CertOid NULL, ConditionOid says what shape it is in.
--    A graded item: CertOid set. CollectibleVariantOid is set too once the cert's card has
--                   been matched to the catalog; a slab can be added before that happens.
--    Every item must point at one or the other.
-- -----------------------------------------------------------------------------
CREATE TABLE CollectionItem (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    CollectorOid            BIGINT          NOT NULL,
    CollectibleVariantOid   BIGINT          NULL,
    CertOid                 BIGINT          NULL,
    ConditionOid            BIGINT          NULL,
    AcquiredOn              DATETIME        NULL,
    PurchasePrice           DECIMAL(12,2)   NULL,
    Notes                   NVARCHAR(MAX)   NULL,
    CreatedOn               DATETIME        NOT NULL CONSTRAINT DF_CollectionItem_CreatedOn DEFAULT (GETDATE()),
    ModifiedOn              DATETIME        NULL,
    UserAuthOid_CreatedBy   BIGINT          NULL,       -- Authentication.UserAuth.Oid (cross-database, no FK)
    UserAuthOid_ModifiedBy  BIGINT          NULL,       -- Authentication.UserAuth.Oid (cross-database, no FK)
    CONSTRAINT PK_CollectionItem PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_CollectionItem_Collector          FOREIGN KEY (CollectorOid)          REFERENCES Collector (Oid)          ON DELETE NO ACTION,
    CONSTRAINT FK_CollectionItem_CollectibleVariant FOREIGN KEY (CollectibleVariantOid) REFERENCES CollectibleVariant (Oid) ON DELETE NO ACTION,
    CONSTRAINT FK_CollectionItem_Cert               FOREIGN KEY (CertOid)               REFERENCES Cert (Oid)               ON DELETE NO ACTION,
    CONSTRAINT FK_CollectionItem_Condition          FOREIGN KEY (ConditionOid)          REFERENCES Condition (Oid)          ON DELETE NO ACTION,
    CONSTRAINT CK_CollectionItem_VariantOrCert CHECK (CollectibleVariantOid IS NOT NULL OR CertOid IS NOT NULL)
);
GO

-- Every query on this table filters by CollectorOid.
CREATE INDEX IX_CollectionItem_CollectorOid ON CollectionItem (CollectorOid);
GO

CREATE INDEX IX_CollectionItem_CollectibleVariantOid ON CollectionItem (CollectibleVariantOid) WHERE CollectibleVariantOid IS NOT NULL;
GO

-- A collector cannot add the same slab twice.
CREATE UNIQUE INDEX UQ_CollectionItem_CollectorOid_CertOid ON CollectionItem (CollectorOid, CertOid) WHERE CertOid IS NOT NULL;
GO

-- Lookups by cert alone (who holds this slab; the FK check when a Cert is deleted).
CREATE INDEX IX_CollectionItem_CertOid ON CollectionItem (CertOid) WHERE CertOid IS NOT NULL;
GO

CREATE INDEX IX_CollectionItem_ConditionOid ON CollectionItem (ConditionOid) WHERE ConditionOid IS NOT NULL;
GO

-- =============================================================================
-- SEED: reference data the app cannot work without
-- Games, sets and cards are NOT seeded: they arrive from the data sources.
-- =============================================================================
INSERT INTO CollectibleType (Name) VALUES (N'Trading Card Game');
GO

-- Every language TCGdex publishes Pokemon sets in (2026-10-02).
INSERT INTO [Language] (Name, Code) VALUES
    (N'English', N'en'), (N'French', N'fr'), (N'German', N'de'), (N'Spanish', N'es'), (N'Italian', N'it'),
    (N'Portuguese', N'pt'), (N'Japanese', N'ja'), (N'Korean', N'ko'), (N'Chinese (Traditional)', N'zh-tw'),
    (N'Chinese (Simplified)', N'zh-cn'), (N'Indonesian', N'id'), (N'Thai', N'th'), (N'Dutch', N'nl'),
    (N'Polish', N'pl'), (N'Russian', N'ru');
GO

-- The printing names JustTCG and TCGplayer use for Pokemon. More are added as sources report them.
INSERT INTO Printing (Name, SortOrder) VALUES (N'Normal', 10), (N'Holofoil', 20), (N'Reverse Holofoil', 30);
GO

-- The five-step raw condition scale the TCG marketplaces share.
INSERT INTO Condition (Name, Abbreviation, SortOrder) VALUES
    (N'Near Mint',         N'NM',  10),
    (N'Lightly Played',    N'LP',  20),
    (N'Moderately Played', N'MP',  30),
    (N'Heavily Played',    N'HP',  40),
    (N'Damaged',           N'DMG', 50);
GO

-- PSA:        cert lookups, and the SpecID stored in CollectibleVariantExternalId.
-- TCGdex:     catalog, names by language, images; relays TCGplayer and Cardmarket prices.
-- JustTCG:    prices by condition and by grade.
-- TCGplayer / Cardmarket: marketplaces whose prices and product ids arrive through the others.
INSERT INTO DataSource (Name, Url) VALUES
    (N'PSA',        N'https://www.psacard.com'),
    (N'TCGdex',     N'https://tcgdex.dev'),
    (N'JustTCG',    N'https://justtcg.com'),
    (N'TCGplayer',  N'https://www.tcgplayer.com'),
    (N'Cardmarket', N'https://www.cardmarket.com');
GO

INSERT INTO GradingCompany (Name, Abbreviation, Url) VALUES
    (N'Professional Sports Authenticator', N'PSA',  N'https://www.psacard.com'),
    (N'Beckett Grading Services',          N'BGS',  N'https://www.beckett.com/grading'),
    (N'Beckett Vintage Grading',           N'BVG',  N'https://www.beckett.com/grading'),
    (N'Beckett Collectors Club Grading',   N'BCCG', N'https://www.beckett.com/grading'),
    (N'Certified Guaranty Company',        N'CGC',  N'https://www.cgccards.com'),
    (N'Sportscard Guaranty Corporation',   N'SGC',  N'https://www.gosgc.com');
GO

-- PSA's 1-10 card scale (half grades from 1.5 to 8.5, plus Authentic). The other companies'
-- grades are added as rows by the import when a graded price first reports them.
INSERT INTO Grade (GradingCompanyOid, Code, Name, GradeValue, SortOrder)
SELECT gc.Oid, v.Code, v.Name, v.GradeValue, v.SortOrder
FROM GradingCompany gc
CROSS JOIN (VALUES
    (N'10',  N'GEM-MT',    10.0, 10),
    (N'9',   N'MINT',       9.0, 20),
    (N'8.5', N'NM-MT+',     8.5, 30),
    (N'8',   N'NM-MT',      8.0, 40),
    (N'7.5', N'NM+',        7.5, 50),
    (N'7',   N'NM',         7.0, 60),
    (N'6.5', N'EX-MT+',     6.5, 70),
    (N'6',   N'EX-MT',      6.0, 80),
    (N'5.5', N'EX+',        5.5, 90),
    (N'5',   N'EX',         5.0, 100),
    (N'4.5', N'VG-EX+',     4.5, 110),
    (N'4',   N'VG-EX',      4.0, 120),
    (N'3.5', N'VG+',        3.5, 130),
    (N'3',   N'VG',         3.0, 140),
    (N'2.5', N'GOOD+',      2.5, 150),
    (N'2',   N'GOOD',       2.0, 160),
    (N'1.5', N'FR',         1.5, 170),
    (N'1',   N'PR',         1.0, 180),
    (N'A',   N'Authentic',  NULL, 190)
) AS v (Code, Name, GradeValue, SortOrder)
WHERE gc.Abbreviation = N'PSA';
GO

SET NOEXEC OFF;
GO

-- =============================================================================
-- CollectionMaster - Alter_003
--
-- Written 2026-10-03 as a draft, reviewed by Steven and applied the same day ("assume it is
-- correct for now; we will alter later if we find gaps"). Schema for sold tracking, recorded
-- market sales and the watchlist.
--
-- 1. Marketplace                     Where things are bought and sold (TCGplayer, eBay, a card
--                                    show, ...). Not the same as DataSource: a DataSource is a
--                                    system we take data from; a marketplace is where a sale
--                                    happened. TCGplayer is both.
-- 2. CollectionItem (new columns)    Where a copy was bought, and - once it is sold - when,
--                                    for how much, with what fees, and where. A sold item
--                                    stays in the table: that is what realised gain/loss and
--                                    "sold" history are built from. SoldOn IS NULL = still owned.
-- 3. CollectibleSale                 One real sale of a version on a marketplace (a sold
--                                    listing), from a data source that reports sales. This is
--                                    the "what did it actually sell for" record - CollectiblePrice
--                                    holds price guides and asking prices, never single sales.
-- 4. WatchlistItem                   A version a collector is watching, with optional alert
--                                    prices. Tenant data.
--
-- After running: regenerate GeneratedClasses_CM.cs in DBQ (Reload -> Generate).
--
-- STATUS:
--   [x] Applied 2026-10-03 to localhost\SQL2025 CollectionMaster (runner job_0075)
-- =============================================================================

USE CollectionMaster;
GO

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

IF EXISTS (SELECT 1 FROM sys.tables WHERE name IN ('Marketplace', 'CollectibleSale', 'WatchlistItem'))
BEGIN
    RAISERROR('Alter_003.sql has already been applied (one or more of its tables exist).', 16, 1);
    SET NOEXEC ON;
END
GO

-- -----------------------------------------------------------------------------
-- 1. Marketplace
-- -----------------------------------------------------------------------------
CREATE TABLE Marketplace (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    Name                    NVARCHAR(100)   NOT NULL,
    Url                     NVARCHAR(300)   NULL,
    IsOnline                BIT             NOT NULL CONSTRAINT DF_Marketplace_IsOnline DEFAULT (1),
    SortOrder               INT             NOT NULL CONSTRAINT DF_Marketplace_SortOrder DEFAULT (0),
    IsActive                BIT             NOT NULL CONSTRAINT DF_Marketplace_IsActive DEFAULT (1),
    CONSTRAINT PK_Marketplace PRIMARY KEY CLUSTERED (Oid)
);
GO

CREATE UNIQUE INDEX UQ_Marketplace_Name ON Marketplace (Name);
GO

INSERT INTO Marketplace (Name, Url, IsOnline, SortOrder) VALUES
    (N'TCGplayer',            N'https://www.tcgplayer.com',  1, 10),
    (N'eBay',                 N'https://www.ebay.com',       1, 20),
    (N'Cardmarket',           N'https://www.cardmarket.com', 1, 30),
    (N'Whatnot',              N'https://www.whatnot.com',    1, 40),
    (N'Facebook Marketplace', N'https://www.facebook.com/marketplace', 1, 50),
    (N'Local card shop',      NULL, 0, 60),
    (N'Card show',            NULL, 0, 70),
    (N'Private sale',         NULL, 0, 80),
    (N'Other',                NULL, 0, 90);
GO

-- -----------------------------------------------------------------------------
-- 2. CollectionItem - where it was bought, and the sale once it is sold
--    SalePrice is what the buyer paid for the item; SaleFees is everything that came off
--    it (marketplace fees, shipping paid by the seller). Realised gain = SalePrice - SaleFees
--    - PurchasePrice.
-- -----------------------------------------------------------------------------
ALTER TABLE CollectionItem ADD
    MarketplaceOid_Acquired BIGINT          NULL,
    SoldOn                  DATETIME        NULL,
    SalePrice               DECIMAL(12,2)   NULL,
    SaleFees                DECIMAL(12,2)   NULL,
    MarketplaceOid_Sold     BIGINT          NULL,
    CONSTRAINT FK_CollectionItem_Marketplace_Acquired FOREIGN KEY (MarketplaceOid_Acquired) REFERENCES Marketplace (Oid) ON DELETE NO ACTION,
    CONSTRAINT FK_CollectionItem_Marketplace_Sold     FOREIGN KEY (MarketplaceOid_Sold)     REFERENCES Marketplace (Oid) ON DELETE NO ACTION,
    CONSTRAINT CK_CollectionItem_Sale CHECK (SoldOn IS NOT NULL OR (SalePrice IS NULL AND SaleFees IS NULL AND MarketplaceOid_Sold IS NULL));
GO

CREATE INDEX IX_CollectionItem_MarketplaceOid_Acquired ON CollectionItem (MarketplaceOid_Acquired) WHERE MarketplaceOid_Acquired IS NOT NULL;
GO

CREATE INDEX IX_CollectionItem_MarketplaceOid_Sold ON CollectionItem (MarketplaceOid_Sold) WHERE MarketplaceOid_Sold IS NOT NULL;
GO

-- -----------------------------------------------------------------------------
-- 3. CollectibleSale - one real sale of a version
--    ExternalId is the marketplace's own id for the listing; with DataSourceOid it is what
--    stops the same sale being stored twice. A sale is graded (GradeOid) or raw (ConditionOid
--    when the listing states one) - the same rule as CollectiblePrice.
-- -----------------------------------------------------------------------------
CREATE TABLE CollectibleSale (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    CollectibleVariantOid   BIGINT          NOT NULL,
    DataSourceOid           BIGINT          NOT NULL,   -- who reported the sale
    MarketplaceOid          BIGINT          NOT NULL,   -- where it sold
    ExternalId              NVARCHAR(100)   NULL,
    SoldOn                  DATETIME        NOT NULL,
    Price                   DECIMAL(12,2)   NOT NULL,   -- the item price, without shipping
    ShippingPrice           DECIMAL(12,2)   NULL,
    CurrencyCode            NVARCHAR(3)     NOT NULL CONSTRAINT DF_CollectibleSale_CurrencyCode DEFAULT ('USD'),
    IsGraded                BIT             NOT NULL,
    GradeOid                BIGINT          NULL,       -- required when IsGraded = 1
    ConditionOid            BIGINT          NULL,       -- only when IsGraded = 0
    Qualifier               NVARCHAR(20)    NULL,
    SaleType                NVARCHAR(30)    NULL,       -- "auction", "fixed price", "best offer" - the source's own text
    Title                   NVARCHAR(300)   NULL,       -- the listing title, as sold
    ListingUrl              NVARCHAR(500)   NULL,
    CreatedOn               DATETIME        NOT NULL CONSTRAINT DF_CollectibleSale_CreatedOn DEFAULT (GETDATE()),
    CONSTRAINT PK_CollectibleSale PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_CollectibleSale_CollectibleVariant FOREIGN KEY (CollectibleVariantOid) REFERENCES CollectibleVariant (Oid) ON DELETE NO ACTION,
    CONSTRAINT FK_CollectibleSale_DataSource         FOREIGN KEY (DataSourceOid)         REFERENCES DataSource (Oid)         ON DELETE NO ACTION,
    CONSTRAINT FK_CollectibleSale_Marketplace        FOREIGN KEY (MarketplaceOid)        REFERENCES Marketplace (Oid)        ON DELETE NO ACTION,
    CONSTRAINT FK_CollectibleSale_Grade              FOREIGN KEY (GradeOid)              REFERENCES Grade (Oid)              ON DELETE NO ACTION,
    CONSTRAINT FK_CollectibleSale_Condition          FOREIGN KEY (ConditionOid)          REFERENCES Condition (Oid)          ON DELETE NO ACTION,
    CONSTRAINT CK_CollectibleSale_GradedOrRaw CHECK (
        (IsGraded = 1 AND GradeOid IS NOT NULL AND ConditionOid IS NULL)
     OR (IsGraded = 0 AND GradeOid IS NULL AND Qualifier IS NULL))
);
GO

-- The sales of one version, newest first - the query every sales list and chart makes.
CREATE INDEX IX_CollectibleSale_CollectibleVariantOid_SoldOn ON CollectibleSale (CollectibleVariantOid, SoldOn DESC);
GO

-- The same listing is never stored twice. Leads with DataSourceOid, so it is also that FK's index.
CREATE UNIQUE INDEX UQ_CollectibleSale_DataSource_ExternalId ON CollectibleSale (DataSourceOid, ExternalId) WHERE ExternalId IS NOT NULL;
GO

CREATE INDEX IX_CollectibleSale_DataSourceOid ON CollectibleSale (DataSourceOid);
GO

CREATE INDEX IX_CollectibleSale_MarketplaceOid ON CollectibleSale (MarketplaceOid);
GO

CREATE INDEX IX_CollectibleSale_GradeOid ON CollectibleSale (GradeOid) WHERE GradeOid IS NOT NULL;
GO

CREATE INDEX IX_CollectibleSale_ConditionOid ON CollectibleSale (ConditionOid) WHERE ConditionOid IS NOT NULL;
GO

-- -----------------------------------------------------------------------------
-- 4. WatchlistItem - a version a collector is watching
--    AlertBelow / AlertAbove are optional: "tell me when the market price drops to X" (a
--    buy target) and "... rises to Y" (a sell target). Prices are in USD, like the market price.
-- -----------------------------------------------------------------------------
CREATE TABLE WatchlistItem (
    Oid                     BIGINT IDENTITY(1,1) NOT NULL,
    CollectorOid            BIGINT          NOT NULL,
    CollectibleVariantOid   BIGINT          NOT NULL,
    AlertBelow              DECIMAL(12,2)   NULL,
    AlertAbove              DECIMAL(12,2)   NULL,
    Notes                   NVARCHAR(MAX)   NULL,
    CreatedOn               DATETIME        NOT NULL CONSTRAINT DF_WatchlistItem_CreatedOn DEFAULT (GETDATE()),
    ModifiedOn              DATETIME        NULL,
    UserAuthOid_CreatedBy   BIGINT          NULL,       -- Authentication.UserAuth.Oid (cross-database, no FK)
    UserAuthOid_ModifiedBy  BIGINT          NULL,       -- Authentication.UserAuth.Oid (cross-database, no FK)
    CONSTRAINT PK_WatchlistItem PRIMARY KEY CLUSTERED (Oid),
    CONSTRAINT FK_WatchlistItem_Collector          FOREIGN KEY (CollectorOid)          REFERENCES Collector (Oid)          ON DELETE NO ACTION,
    CONSTRAINT FK_WatchlistItem_CollectibleVariant FOREIGN KEY (CollectibleVariantOid) REFERENCES CollectibleVariant (Oid) ON DELETE NO ACTION
);
GO

-- A version is on a collector's watchlist once. Leads with CollectorOid, which every query filters by.
CREATE UNIQUE INDEX UQ_WatchlistItem_CollectorOid_CollectibleVariantOid ON WatchlistItem (CollectorOid, CollectibleVariantOid);
GO

CREATE INDEX IX_WatchlistItem_CollectibleVariantOid ON WatchlistItem (CollectibleVariantOid);
GO

SET NOEXEC OFF;
GO

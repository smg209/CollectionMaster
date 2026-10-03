// ╔══════════════════════════════════════════════════════════════════════╗
// ║  GENERATOR WARNINGS — Relationships skipped due to missing names     ║
// ║  Open PagSchema, select the table, click the relationship, and set   ║
// ║  the Accessor Property Name.  Then regenerate.                        ║
// ╠══════════════════════════════════════════════════════════════════════╣
// ║  Skipped duplicate property 'DataSource' for relationship 'FK_CollectiblePrice_DataSource_RetrievedFrom' (column 'CollectiblePrice.DataSourceOid_RetrievedFrom') — name already claimed by relationship 'FK_CollectiblePrice_DataSource' (column 'CollectiblePrice.DataSourceOid').║
// ║  Skipped duplicate property 'CollectiblePrices' for relationship 'FK_CollectiblePrice_DataSource_RetrievedFrom' (column 'CollectiblePrice.DataSourceOid_RetrievedFrom') — name already claimed by relationship 'FK_CollectiblePrice_DataSource' (column 'CollectiblePrice.DataSourceOid').║
// ╚══════════════════════════════════════════════════════════════════════╝

using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using FSDataUtil.Core;
using FSCommon.Util;

namespace CM.Shared
{

    //***************************************************
    //*  Class: CardSet
    //***************************************************
    #region CardSet
    public partial class CardSet : Record<CardSet>
    {

        #region Fields
        private long _oid;
        private long _gameOid;
        private string _name;
        private string _code;
        private string _series;
        private long _languageOid_Primary;
        private DateTime? _releasedOn;
        private int? _cardCountOfficial;
        private int? _cardCountTotal;
        private string _logoUrl;
        private string _symbolUrl;
        private DateTime _createdOn;
        private DateTime? _modifiedOn;
        private ManagedList<CardSetExternalId>? _cardSetExternalIds = null;
        private ManagedList<CardSetName>? _cardSetNames = null;
        private ManagedList<Collectible>? _collectibles = null;
        private Game? _game = null;
        private Language? _language = null;
        #endregion (Fields)

        #region Constructor
        public CardSet() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "CardSet";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("GameOid")]
        public long GameOid { get { return _gameOid; } set { if (_gameOid != value) { _gameOid = value; IsChanged = true; } } }

        [Column("Name")]
        public string Name { get { return _name; } set { if (_name != value) { _name = value; IsChanged = true; } } }

        [Column("Code")]
        public string Code { get { return _code; } set { if (_code != value) { _code = value; IsChanged = true; } } }

        [Column("Series")]
        public string Series { get { return _series; } set { if (_series != value) { _series = value; IsChanged = true; } } }

        [Column("LanguageOid_Primary")]
        public long LanguageOid_Primary { get { return _languageOid_Primary; } set { if (_languageOid_Primary != value) { _languageOid_Primary = value; IsChanged = true; } } }

        [Column("ReleasedOn")]
        public DateTime? ReleasedOn { get { return _releasedOn; } set { if (_releasedOn != value) { _releasedOn = value; IsChanged = true; } } }

        [Column("CardCountOfficial")]
        public int? CardCountOfficial { get { return _cardCountOfficial; } set { if (_cardCountOfficial != value) { _cardCountOfficial = value; IsChanged = true; } } }

        [Column("CardCountTotal")]
        public int? CardCountTotal { get { return _cardCountTotal; } set { if (_cardCountTotal != value) { _cardCountTotal = value; IsChanged = true; } } }

        [Column("LogoUrl")]
        public string LogoUrl { get { return _logoUrl; } set { if (_logoUrl != value) { _logoUrl = value; IsChanged = true; } } }

        [Column("SymbolUrl")]
        public string SymbolUrl { get { return _symbolUrl; } set { if (_symbolUrl != value) { _symbolUrl = value; IsChanged = true; } } }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get { return _createdOn; } set { if (_createdOn != value) { _createdOn = value; IsChanged = true; } } }

        [Column("ModifiedOn")]
        public DateTime? ModifiedOn { get { return _modifiedOn; } set { if (_modifiedOn != value) { _modifiedOn = value; IsChanged = true; } } }

        [Ignore]
        public ManagedList<CardSetExternalId>? CardSetExternalIds {
            get { if (_cardSetExternalIds == null) { LoadCardSetExternalIds(); } return _cardSetExternalIds; }
            set {
                if (_cardSetExternalIds != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CardSetExternalIds", "FK_CardSetExternalId_CardSet");
                    }
                    _cardSetExternalIds = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CardSetName>? CardSetNames {
            get { if (_cardSetNames == null) { LoadCardSetNames(); } return _cardSetNames; }
            set {
                if (_cardSetNames != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CardSetNames", "FK_CardSetName_CardSet");
                    }
                    _cardSetNames = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<Collectible>? Collectibles {
            get { if (_collectibles == null) { LoadCollectibles(); } return _collectibles; }
            set {
                if (_collectibles != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "Collectibles", "FK_Collectible_CardSet");
                    }
                    _collectibles = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public Game? Game {
            get { if (_game == null) { LoadGame(); } return _game; }
            set { _game = value; IsChanged = true; }
        }

        [Ignore]
        public Language? Language {
            get { if (_language == null) { LoadLanguage(); } return _language; }
            set { _language = value; IsChanged = true; }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCardSetExternalIds() {
            _cardSetExternalIds = CardSetExternalId.Fetch("WHERE CardSetOid = @0", Oid);
            _cardSetExternalIds.BindToParent(this, "CardSetExternalIds", "FK_CardSetExternalId_CardSet");
        }

        public async Task LoadCardSetExternalIdsAsync() {
            _cardSetExternalIds = await CardSetExternalId.FetchAsync("WHERE CardSetOid = @0", Oid).ConfigureAwait(false);
            _cardSetExternalIds.BindToParent(this, "CardSetExternalIds", "FK_CardSetExternalId_CardSet");
        }

        private void LoadCardSetNames() {
            _cardSetNames = CardSetName.Fetch("WHERE CardSetOid = @0", Oid);
            _cardSetNames.BindToParent(this, "CardSetNames", "FK_CardSetName_CardSet");
        }

        public async Task LoadCardSetNamesAsync() {
            _cardSetNames = await CardSetName.FetchAsync("WHERE CardSetOid = @0", Oid).ConfigureAwait(false);
            _cardSetNames.BindToParent(this, "CardSetNames", "FK_CardSetName_CardSet");
        }

        private void LoadCollectibles() {
            _collectibles = Collectible.Fetch("WHERE CardSetOid = @0", Oid);
            _collectibles.BindToParent(this, "Collectibles", "FK_Collectible_CardSet");
        }

        public async Task LoadCollectiblesAsync() {
            _collectibles = await Collectible.FetchAsync("WHERE CardSetOid = @0", Oid).ConfigureAwait(false);
            _collectibles.BindToParent(this, "Collectibles", "FK_Collectible_CardSet");
        }

        private void LoadGame() {
            if (GameOid != null)
                _game = Game.First("WHERE Oid = @0", GameOid);
        }

        public async Task LoadGameAsync() {
            if (GameOid != null)
                _game = await Game.FirstAsync("WHERE Oid = @0", GameOid).ConfigureAwait(false);
        }

        private void LoadLanguage() {
            if (LanguageOid_Primary != null)
                _language = Language.First("WHERE Oid = @0", LanguageOid_Primary);
        }

        public async Task LoadLanguageAsync() {
            if (LanguageOid_Primary != null)
                _language = await Language.FirstAsync("WHERE Oid = @0", LanguageOid_Primary).ConfigureAwait(false);
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "GAMEOID": oReturn = this.GameOid; break;
                    case "NAME": oReturn = this.Name; break;
                    case "CODE": oReturn = this.Code; break;
                    case "SERIES": oReturn = this.Series; break;
                    case "LANGUAGEOID_PRIMARY": oReturn = this.LanguageOid_Primary; break;
                    case "RELEASEDON": oReturn = this.ReleasedOn; break;
                    case "CARDCOUNTOFFICIAL": oReturn = this.CardCountOfficial; break;
                    case "CARDCOUNTTOTAL": oReturn = this.CardCountTotal; break;
                    case "LOGOURL": oReturn = this.LogoUrl; break;
                    case "SYMBOLURL": oReturn = this.SymbolUrl; break;
                    case "CREATEDON": oReturn = this.CreatedOn; break;
                    case "MODIFIEDON": oReturn = this.ModifiedOn; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "GAMEOID": this.GameOid = (long)value; break;
                    case "NAME": this.Name = (string)value; break;
                    case "CODE": this.Code = (string)value; break;
                    case "SERIES": this.Series = (string)value; break;
                    case "LANGUAGEOID_PRIMARY": this.LanguageOid_Primary = (long)value; break;
                    case "RELEASEDON": this.ReleasedOn = (DateTime?)value; break;
                    case "CARDCOUNTOFFICIAL": this.CardCountOfficial = (int?)value; break;
                    case "CARDCOUNTTOTAL": this.CardCountTotal = (int?)value; break;
                    case "LOGOURL": this.LogoUrl = (string)value; break;
                    case "SYMBOLURL": this.SymbolUrl = (string)value; break;
                    case "CREATEDON": this.CreatedOn = (DateTime)value; break;
                    case "MODIFIEDON": this.ModifiedOn = (DateTime?)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("GameOid", typeof(long));
            oReturn.Columns.Add("Name", typeof(string));
            oReturn.Columns.Add("Code", typeof(string));
            oReturn.Columns.Add("Series", typeof(string));
            oReturn.Columns.Add("LanguageOid_Primary", typeof(long));
            oReturn.Columns.Add("ReleasedOn", typeof(DateTime?));
            oReturn.Columns.Add("CardCountOfficial", typeof(int?));
            oReturn.Columns.Add("CardCountTotal", typeof(int?));
            oReturn.Columns.Add("LogoUrl", typeof(string));
            oReturn.Columns.Add("SymbolUrl", typeof(string));
            oReturn.Columns.Add("CreatedOn", typeof(DateTime));
            oReturn.Columns.Add("ModifiedOn", typeof(DateTime?));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, GameOid, Name, Code, Series, LanguageOid_Primary, ReleasedOn, CardCountOfficial, CardCountTotal, LogoUrl, SymbolUrl, CreatedOn, ModifiedOn);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CardSet().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CardSet().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
            foreach (CardSetExternalId oChild in CardSetExternalIds.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CardSetName oChild in CardSetNames.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (Collectible oChild in Collectibles.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
            foreach (CardSetExternalId oChild in CardSetExternalIds.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CardSetName oChild in CardSetNames.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (Collectible oChild in Collectibles.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (CardSet)

    //***************************************************
    //*  Class: CardSetExternalId
    //***************************************************
    #region CardSetExternalId
    public partial class CardSetExternalId : Record<CardSetExternalId>
    {

        #region Fields
        private long _oid;
        private long _cardSetOid;
        private long _dataSourceOid;
        private string _externalId;
        private CardSet? _cardSet = null;
        private DataSource? _dataSource = null;
        #endregion (Fields)

        #region Constructor
        public CardSetExternalId() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "CardSetExternalId";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("CardSetOid")]
        public long CardSetOid { get { return _cardSetOid; } set { if (_cardSetOid != value) { _cardSetOid = value; IsChanged = true; } } }

        [Column("DataSourceOid")]
        public long DataSourceOid { get { return _dataSourceOid; } set { if (_dataSourceOid != value) { _dataSourceOid = value; IsChanged = true; } } }

        [Column("ExternalId")]
        public string ExternalId { get { return _externalId; } set { if (_externalId != value) { _externalId = value; IsChanged = true; } } }

        [Ignore]
        public CardSet? CardSet {
            get { if (_cardSet == null) { LoadCardSet(); } return _cardSet; }
            set { _cardSet = value; IsChanged = true; }
        }

        [Ignore]
        public DataSource? DataSource {
            get { if (_dataSource == null) { LoadDataSource(); } return _dataSource; }
            set { _dataSource = value; IsChanged = true; }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCardSet() {
            if (CardSetOid != null)
                _cardSet = CardSet.First("WHERE Oid = @0", CardSetOid);
        }

        public async Task LoadCardSetAsync() {
            if (CardSetOid != null)
                _cardSet = await CardSet.FirstAsync("WHERE Oid = @0", CardSetOid).ConfigureAwait(false);
        }

        private void LoadDataSource() {
            if (DataSourceOid != null)
                _dataSource = DataSource.First("WHERE Oid = @0", DataSourceOid);
        }

        public async Task LoadDataSourceAsync() {
            if (DataSourceOid != null)
                _dataSource = await DataSource.FirstAsync("WHERE Oid = @0", DataSourceOid).ConfigureAwait(false);
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "CARDSETOID": oReturn = this.CardSetOid; break;
                    case "DATASOURCEOID": oReturn = this.DataSourceOid; break;
                    case "EXTERNALID": oReturn = this.ExternalId; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "CARDSETOID": this.CardSetOid = (long)value; break;
                    case "DATASOURCEOID": this.DataSourceOid = (long)value; break;
                    case "EXTERNALID": this.ExternalId = (string)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("CardSetOid", typeof(long));
            oReturn.Columns.Add("DataSourceOid", typeof(long));
            oReturn.Columns.Add("ExternalId", typeof(string));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, CardSetOid, DataSourceOid, ExternalId);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CardSetExternalId().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CardSetExternalId().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (CardSetExternalId)

    //***************************************************
    //*  Class: CardSetName
    //***************************************************
    #region CardSetName
    public partial class CardSetName : Record<CardSetName>
    {

        #region Fields
        private long _oid;
        private long _cardSetOid;
        private long _languageOid;
        private string _name;
        private CardSet? _cardSet = null;
        private Language? _language = null;
        #endregion (Fields)

        #region Constructor
        public CardSetName() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "CardSetName";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("CardSetOid")]
        public long CardSetOid { get { return _cardSetOid; } set { if (_cardSetOid != value) { _cardSetOid = value; IsChanged = true; } } }

        [Column("LanguageOid")]
        public long LanguageOid { get { return _languageOid; } set { if (_languageOid != value) { _languageOid = value; IsChanged = true; } } }

        [Column("Name")]
        public string Name { get { return _name; } set { if (_name != value) { _name = value; IsChanged = true; } } }

        [Ignore]
        public CardSet? CardSet {
            get { if (_cardSet == null) { LoadCardSet(); } return _cardSet; }
            set { _cardSet = value; IsChanged = true; }
        }

        [Ignore]
        public Language? Language {
            get { if (_language == null) { LoadLanguage(); } return _language; }
            set { _language = value; IsChanged = true; }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCardSet() {
            if (CardSetOid != null)
                _cardSet = CardSet.First("WHERE Oid = @0", CardSetOid);
        }

        public async Task LoadCardSetAsync() {
            if (CardSetOid != null)
                _cardSet = await CardSet.FirstAsync("WHERE Oid = @0", CardSetOid).ConfigureAwait(false);
        }

        private void LoadLanguage() {
            if (LanguageOid != null)
                _language = Language.First("WHERE Oid = @0", LanguageOid);
        }

        public async Task LoadLanguageAsync() {
            if (LanguageOid != null)
                _language = await Language.FirstAsync("WHERE Oid = @0", LanguageOid).ConfigureAwait(false);
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "CARDSETOID": oReturn = this.CardSetOid; break;
                    case "LANGUAGEOID": oReturn = this.LanguageOid; break;
                    case "NAME": oReturn = this.Name; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "CARDSETOID": this.CardSetOid = (long)value; break;
                    case "LANGUAGEOID": this.LanguageOid = (long)value; break;
                    case "NAME": this.Name = (string)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("CardSetOid", typeof(long));
            oReturn.Columns.Add("LanguageOid", typeof(long));
            oReturn.Columns.Add("Name", typeof(string));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, CardSetOid, LanguageOid, Name);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CardSetName().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CardSetName().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (CardSetName)

    //***************************************************
    //*  Class: Cert
    //***************************************************
    #region Cert
    public partial class Cert : Record<Cert>
    {

        #region Fields
        private long _oid;
        private long _gradingCompanyOid;
        private string _certNumber;
        private long? _collectibleVariantOid;
        private long? _gradeOid;
        private string _qualifier;
        private int? _specID;
        private string _specNumber;
        private string _year;
        private string _brand;
        private string _category;
        private string _cardNumber;
        private string _subject;
        private string _variety;
        private string _cardGrade;
        private string _gradeDescription;
        private string _autographGrade;
        private string _labelType;
        private bool? _isReverseBarCode;
        private bool? _isPSADNA;
        private bool? _isDualCert;
        private int? _totalPopulation;
        private int? _totalPopulationWithQualifier;
        private int? _populationHigher;
        private string _itemStatus;
        private DateTime? _lastVerifiedOn;
        private DateTime _createdOn;
        private DateTime? _modifiedOn;
        private ManagedList<CollectionItem>? _collectionItems = null;
        private CollectibleVariant? _collectibleVariant = null;
        private Grade? _grade = null;
        private GradingCompany? _gradingCompany = null;
        #endregion (Fields)

        #region Constructor
        public Cert() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "Cert";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("GradingCompanyOid")]
        public long GradingCompanyOid { get { return _gradingCompanyOid; } set { if (_gradingCompanyOid != value) { _gradingCompanyOid = value; IsChanged = true; } } }

        [Column("CertNumber")]
        public string CertNumber { get { return _certNumber; } set { if (_certNumber != value) { _certNumber = value; IsChanged = true; } } }

        [Column("CollectibleVariantOid")]
        public long? CollectibleVariantOid { get { return _collectibleVariantOid; } set { if (_collectibleVariantOid != value) { _collectibleVariantOid = value; IsChanged = true; } } }

        [Column("GradeOid")]
        public long? GradeOid { get { return _gradeOid; } set { if (_gradeOid != value) { _gradeOid = value; IsChanged = true; } } }

        [Column("Qualifier")]
        public string Qualifier { get { return _qualifier; } set { if (_qualifier != value) { _qualifier = value; IsChanged = true; } } }

        [Column("SpecID")]
        public int? SpecID { get { return _specID; } set { if (_specID != value) { _specID = value; IsChanged = true; } } }

        [Column("SpecNumber")]
        public string SpecNumber { get { return _specNumber; } set { if (_specNumber != value) { _specNumber = value; IsChanged = true; } } }

        [Column("Year")]
        public string Year { get { return _year; } set { if (_year != value) { _year = value; IsChanged = true; } } }

        [Column("Brand")]
        public string Brand { get { return _brand; } set { if (_brand != value) { _brand = value; IsChanged = true; } } }

        [Column("Category")]
        public string Category { get { return _category; } set { if (_category != value) { _category = value; IsChanged = true; } } }

        [Column("CardNumber")]
        public string CardNumber { get { return _cardNumber; } set { if (_cardNumber != value) { _cardNumber = value; IsChanged = true; } } }

        [Column("Subject")]
        public string Subject { get { return _subject; } set { if (_subject != value) { _subject = value; IsChanged = true; } } }

        [Column("Variety")]
        public string Variety { get { return _variety; } set { if (_variety != value) { _variety = value; IsChanged = true; } } }

        [Column("CardGrade")]
        public string CardGrade { get { return _cardGrade; } set { if (_cardGrade != value) { _cardGrade = value; IsChanged = true; } } }

        [Column("GradeDescription")]
        public string GradeDescription { get { return _gradeDescription; } set { if (_gradeDescription != value) { _gradeDescription = value; IsChanged = true; } } }

        [Column("AutographGrade")]
        public string AutographGrade { get { return _autographGrade; } set { if (_autographGrade != value) { _autographGrade = value; IsChanged = true; } } }

        [Column("LabelType")]
        public string LabelType { get { return _labelType; } set { if (_labelType != value) { _labelType = value; IsChanged = true; } } }

        [Column("IsReverseBarCode")]
        public bool? IsReverseBarCode { get { return _isReverseBarCode; } set { if (_isReverseBarCode != value) { _isReverseBarCode = value; IsChanged = true; } } }

        [Column("IsPSADNA")]
        public bool? IsPSADNA { get { return _isPSADNA; } set { if (_isPSADNA != value) { _isPSADNA = value; IsChanged = true; } } }

        [Column("IsDualCert")]
        public bool? IsDualCert { get { return _isDualCert; } set { if (_isDualCert != value) { _isDualCert = value; IsChanged = true; } } }

        [Column("TotalPopulation")]
        public int? TotalPopulation { get { return _totalPopulation; } set { if (_totalPopulation != value) { _totalPopulation = value; IsChanged = true; } } }

        [Column("TotalPopulationWithQualifier")]
        public int? TotalPopulationWithQualifier { get { return _totalPopulationWithQualifier; } set { if (_totalPopulationWithQualifier != value) { _totalPopulationWithQualifier = value; IsChanged = true; } } }

        [Column("PopulationHigher")]
        public int? PopulationHigher { get { return _populationHigher; } set { if (_populationHigher != value) { _populationHigher = value; IsChanged = true; } } }

        [Column("ItemStatus")]
        public string ItemStatus { get { return _itemStatus; } set { if (_itemStatus != value) { _itemStatus = value; IsChanged = true; } } }

        [Column("LastVerifiedOn")]
        public DateTime? LastVerifiedOn { get { return _lastVerifiedOn; } set { if (_lastVerifiedOn != value) { _lastVerifiedOn = value; IsChanged = true; } } }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get { return _createdOn; } set { if (_createdOn != value) { _createdOn = value; IsChanged = true; } } }

        [Column("ModifiedOn")]
        public DateTime? ModifiedOn { get { return _modifiedOn; } set { if (_modifiedOn != value) { _modifiedOn = value; IsChanged = true; } } }

        [Ignore]
        public ManagedList<CollectionItem>? CollectionItems {
            get { if (_collectionItems == null) { LoadCollectionItems(); } return _collectionItems; }
            set {
                if (_collectionItems != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectionItems", "FK_CollectionItem_Cert");
                    }
                    _collectionItems = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public CollectibleVariant? CollectibleVariant {
            get { if (_collectibleVariant == null) { LoadCollectibleVariant(); } return _collectibleVariant; }
            set { _collectibleVariant = value; IsChanged = true; }
        }

        [Ignore]
        public Grade? Grade {
            get { if (_grade == null) { LoadGrade(); } return _grade; }
            set { _grade = value; IsChanged = true; }
        }

        [Ignore]
        public GradingCompany? GradingCompany {
            get { if (_gradingCompany == null) { LoadGradingCompany(); } return _gradingCompany; }
            set { _gradingCompany = value; IsChanged = true; }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCollectionItems() {
            _collectionItems = CollectionItem.Fetch("WHERE CertOid = @0", Oid);
            _collectionItems.BindToParent(this, "CollectionItems", "FK_CollectionItem_Cert");
        }

        public async Task LoadCollectionItemsAsync() {
            _collectionItems = await CollectionItem.FetchAsync("WHERE CertOid = @0", Oid).ConfigureAwait(false);
            _collectionItems.BindToParent(this, "CollectionItems", "FK_CollectionItem_Cert");
        }

        private void LoadCollectibleVariant() {
            if (CollectibleVariantOid != null)
                _collectibleVariant = CollectibleVariant.First("WHERE Oid = @0", CollectibleVariantOid);
        }

        public async Task LoadCollectibleVariantAsync() {
            if (CollectibleVariantOid != null)
                _collectibleVariant = await CollectibleVariant.FirstAsync("WHERE Oid = @0", CollectibleVariantOid).ConfigureAwait(false);
        }

        private void LoadGrade() {
            if (GradeOid != null)
                _grade = Grade.First("WHERE Oid = @0", GradeOid);
        }

        public async Task LoadGradeAsync() {
            if (GradeOid != null)
                _grade = await Grade.FirstAsync("WHERE Oid = @0", GradeOid).ConfigureAwait(false);
        }

        private void LoadGradingCompany() {
            if (GradingCompanyOid != null)
                _gradingCompany = GradingCompany.First("WHERE Oid = @0", GradingCompanyOid);
        }

        public async Task LoadGradingCompanyAsync() {
            if (GradingCompanyOid != null)
                _gradingCompany = await GradingCompany.FirstAsync("WHERE Oid = @0", GradingCompanyOid).ConfigureAwait(false);
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "GRADINGCOMPANYOID": oReturn = this.GradingCompanyOid; break;
                    case "CERTNUMBER": oReturn = this.CertNumber; break;
                    case "COLLECTIBLEVARIANTOID": oReturn = this.CollectibleVariantOid; break;
                    case "GRADEOID": oReturn = this.GradeOid; break;
                    case "QUALIFIER": oReturn = this.Qualifier; break;
                    case "SPECID": oReturn = this.SpecID; break;
                    case "SPECNUMBER": oReturn = this.SpecNumber; break;
                    case "YEAR": oReturn = this.Year; break;
                    case "BRAND": oReturn = this.Brand; break;
                    case "CATEGORY": oReturn = this.Category; break;
                    case "CARDNUMBER": oReturn = this.CardNumber; break;
                    case "SUBJECT": oReturn = this.Subject; break;
                    case "VARIETY": oReturn = this.Variety; break;
                    case "CARDGRADE": oReturn = this.CardGrade; break;
                    case "GRADEDESCRIPTION": oReturn = this.GradeDescription; break;
                    case "AUTOGRAPHGRADE": oReturn = this.AutographGrade; break;
                    case "LABELTYPE": oReturn = this.LabelType; break;
                    case "ISREVERSEBARCODE": oReturn = this.IsReverseBarCode; break;
                    case "ISPSADNA": oReturn = this.IsPSADNA; break;
                    case "ISDUALCERT": oReturn = this.IsDualCert; break;
                    case "TOTALPOPULATION": oReturn = this.TotalPopulation; break;
                    case "TOTALPOPULATIONWITHQUALIFIER": oReturn = this.TotalPopulationWithQualifier; break;
                    case "POPULATIONHIGHER": oReturn = this.PopulationHigher; break;
                    case "ITEMSTATUS": oReturn = this.ItemStatus; break;
                    case "LASTVERIFIEDON": oReturn = this.LastVerifiedOn; break;
                    case "CREATEDON": oReturn = this.CreatedOn; break;
                    case "MODIFIEDON": oReturn = this.ModifiedOn; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "GRADINGCOMPANYOID": this.GradingCompanyOid = (long)value; break;
                    case "CERTNUMBER": this.CertNumber = (string)value; break;
                    case "COLLECTIBLEVARIANTOID": this.CollectibleVariantOid = (long?)value; break;
                    case "GRADEOID": this.GradeOid = (long?)value; break;
                    case "QUALIFIER": this.Qualifier = (string)value; break;
                    case "SPECID": this.SpecID = (int?)value; break;
                    case "SPECNUMBER": this.SpecNumber = (string)value; break;
                    case "YEAR": this.Year = (string)value; break;
                    case "BRAND": this.Brand = (string)value; break;
                    case "CATEGORY": this.Category = (string)value; break;
                    case "CARDNUMBER": this.CardNumber = (string)value; break;
                    case "SUBJECT": this.Subject = (string)value; break;
                    case "VARIETY": this.Variety = (string)value; break;
                    case "CARDGRADE": this.CardGrade = (string)value; break;
                    case "GRADEDESCRIPTION": this.GradeDescription = (string)value; break;
                    case "AUTOGRAPHGRADE": this.AutographGrade = (string)value; break;
                    case "LABELTYPE": this.LabelType = (string)value; break;
                    case "ISREVERSEBARCODE": this.IsReverseBarCode = (bool?)value; break;
                    case "ISPSADNA": this.IsPSADNA = (bool?)value; break;
                    case "ISDUALCERT": this.IsDualCert = (bool?)value; break;
                    case "TOTALPOPULATION": this.TotalPopulation = (int?)value; break;
                    case "TOTALPOPULATIONWITHQUALIFIER": this.TotalPopulationWithQualifier = (int?)value; break;
                    case "POPULATIONHIGHER": this.PopulationHigher = (int?)value; break;
                    case "ITEMSTATUS": this.ItemStatus = (string)value; break;
                    case "LASTVERIFIEDON": this.LastVerifiedOn = (DateTime?)value; break;
                    case "CREATEDON": this.CreatedOn = (DateTime)value; break;
                    case "MODIFIEDON": this.ModifiedOn = (DateTime?)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("GradingCompanyOid", typeof(long));
            oReturn.Columns.Add("CertNumber", typeof(string));
            oReturn.Columns.Add("CollectibleVariantOid", typeof(long?));
            oReturn.Columns.Add("GradeOid", typeof(long?));
            oReturn.Columns.Add("Qualifier", typeof(string));
            oReturn.Columns.Add("SpecID", typeof(int?));
            oReturn.Columns.Add("SpecNumber", typeof(string));
            oReturn.Columns.Add("Year", typeof(string));
            oReturn.Columns.Add("Brand", typeof(string));
            oReturn.Columns.Add("Category", typeof(string));
            oReturn.Columns.Add("CardNumber", typeof(string));
            oReturn.Columns.Add("Subject", typeof(string));
            oReturn.Columns.Add("Variety", typeof(string));
            oReturn.Columns.Add("CardGrade", typeof(string));
            oReturn.Columns.Add("GradeDescription", typeof(string));
            oReturn.Columns.Add("AutographGrade", typeof(string));
            oReturn.Columns.Add("LabelType", typeof(string));
            oReturn.Columns.Add("IsReverseBarCode", typeof(bool?));
            oReturn.Columns.Add("IsPSADNA", typeof(bool?));
            oReturn.Columns.Add("IsDualCert", typeof(bool?));
            oReturn.Columns.Add("TotalPopulation", typeof(int?));
            oReturn.Columns.Add("TotalPopulationWithQualifier", typeof(int?));
            oReturn.Columns.Add("PopulationHigher", typeof(int?));
            oReturn.Columns.Add("ItemStatus", typeof(string));
            oReturn.Columns.Add("LastVerifiedOn", typeof(DateTime?));
            oReturn.Columns.Add("CreatedOn", typeof(DateTime));
            oReturn.Columns.Add("ModifiedOn", typeof(DateTime?));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, GradingCompanyOid, CertNumber, CollectibleVariantOid, GradeOid, Qualifier, SpecID, SpecNumber, Year, Brand, Category, CardNumber, Subject, Variety, CardGrade, GradeDescription, AutographGrade, LabelType, IsReverseBarCode, IsPSADNA, IsDualCert, TotalPopulation, TotalPopulationWithQualifier, PopulationHigher, ItemStatus, LastVerifiedOn, CreatedOn, ModifiedOn);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Cert().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Cert().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
            foreach (CollectionItem oChild in CollectionItems.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
            foreach (CollectionItem oChild in CollectionItems.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (Cert)

    //***************************************************
    //*  Class: Collectible
    //***************************************************
    #region Collectible
    public partial class Collectible : Record<Collectible>
    {

        #region Fields
        private long _oid;
        private long _cardSetOid;
        private string _name;
        private string _cardNumber;
        private string _rarity;
        private string _category;
        private string _illustrator;
        private DateTime _createdOn;
        private DateTime? _modifiedOn;
        private CardSet? _cardSet = null;
        private ManagedList<CollectibleExternalId>? _collectibleExternalIds = null;
        private ManagedList<CollectibleImage>? _collectibleImages = null;
        private ManagedList<CollectibleName>? _collectibleNames = null;
        private ManagedList<CollectibleVariant>? _collectibleVariants = null;
        #endregion (Fields)

        #region Constructor
        public Collectible() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "Collectible";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("CardSetOid")]
        public long CardSetOid { get { return _cardSetOid; } set { if (_cardSetOid != value) { _cardSetOid = value; IsChanged = true; } } }

        [Column("Name")]
        public string Name { get { return _name; } set { if (_name != value) { _name = value; IsChanged = true; } } }

        [Column("CardNumber")]
        public string CardNumber { get { return _cardNumber; } set { if (_cardNumber != value) { _cardNumber = value; IsChanged = true; } } }

        [Column("Rarity")]
        public string Rarity { get { return _rarity; } set { if (_rarity != value) { _rarity = value; IsChanged = true; } } }

        [Column("Category")]
        public string Category { get { return _category; } set { if (_category != value) { _category = value; IsChanged = true; } } }

        [Column("Illustrator")]
        public string Illustrator { get { return _illustrator; } set { if (_illustrator != value) { _illustrator = value; IsChanged = true; } } }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get { return _createdOn; } set { if (_createdOn != value) { _createdOn = value; IsChanged = true; } } }

        [Column("ModifiedOn")]
        public DateTime? ModifiedOn { get { return _modifiedOn; } set { if (_modifiedOn != value) { _modifiedOn = value; IsChanged = true; } } }

        [Ignore]
        public CardSet? CardSet {
            get { if (_cardSet == null) { LoadCardSet(); } return _cardSet; }
            set { _cardSet = value; IsChanged = true; }
        }

        [Ignore]
        public ManagedList<CollectibleExternalId>? CollectibleExternalIds {
            get { if (_collectibleExternalIds == null) { LoadCollectibleExternalIds(); } return _collectibleExternalIds; }
            set {
                if (_collectibleExternalIds != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectibleExternalIds", "FK_CollectibleExternalId_Collectible");
                    }
                    _collectibleExternalIds = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CollectibleImage>? CollectibleImages {
            get { if (_collectibleImages == null) { LoadCollectibleImages(); } return _collectibleImages; }
            set {
                if (_collectibleImages != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectibleImages", "FK_CollectibleImage_Collectible");
                    }
                    _collectibleImages = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CollectibleName>? CollectibleNames {
            get { if (_collectibleNames == null) { LoadCollectibleNames(); } return _collectibleNames; }
            set {
                if (_collectibleNames != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectibleNames", "FK_CollectibleName_Collectible");
                    }
                    _collectibleNames = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CollectibleVariant>? CollectibleVariants {
            get { if (_collectibleVariants == null) { LoadCollectibleVariants(); } return _collectibleVariants; }
            set {
                if (_collectibleVariants != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectibleVariants", "FK_CollectibleVariant_Collectible");
                    }
                    _collectibleVariants = value;
                    IsChanged = true;
                }
            }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCardSet() {
            if (CardSetOid != null)
                _cardSet = CardSet.First("WHERE Oid = @0", CardSetOid);
        }

        public async Task LoadCardSetAsync() {
            if (CardSetOid != null)
                _cardSet = await CardSet.FirstAsync("WHERE Oid = @0", CardSetOid).ConfigureAwait(false);
        }

        private void LoadCollectibleExternalIds() {
            _collectibleExternalIds = CollectibleExternalId.Fetch("WHERE CollectibleOid = @0", Oid);
            _collectibleExternalIds.BindToParent(this, "CollectibleExternalIds", "FK_CollectibleExternalId_Collectible");
        }

        public async Task LoadCollectibleExternalIdsAsync() {
            _collectibleExternalIds = await CollectibleExternalId.FetchAsync("WHERE CollectibleOid = @0", Oid).ConfigureAwait(false);
            _collectibleExternalIds.BindToParent(this, "CollectibleExternalIds", "FK_CollectibleExternalId_Collectible");
        }

        private void LoadCollectibleImages() {
            _collectibleImages = CollectibleImage.Fetch("WHERE CollectibleOid = @0", Oid);
            _collectibleImages.BindToParent(this, "CollectibleImages", "FK_CollectibleImage_Collectible");
        }

        public async Task LoadCollectibleImagesAsync() {
            _collectibleImages = await CollectibleImage.FetchAsync("WHERE CollectibleOid = @0", Oid).ConfigureAwait(false);
            _collectibleImages.BindToParent(this, "CollectibleImages", "FK_CollectibleImage_Collectible");
        }

        private void LoadCollectibleNames() {
            _collectibleNames = CollectibleName.Fetch("WHERE CollectibleOid = @0", Oid);
            _collectibleNames.BindToParent(this, "CollectibleNames", "FK_CollectibleName_Collectible");
        }

        public async Task LoadCollectibleNamesAsync() {
            _collectibleNames = await CollectibleName.FetchAsync("WHERE CollectibleOid = @0", Oid).ConfigureAwait(false);
            _collectibleNames.BindToParent(this, "CollectibleNames", "FK_CollectibleName_Collectible");
        }

        private void LoadCollectibleVariants() {
            _collectibleVariants = CollectibleVariant.Fetch("WHERE CollectibleOid = @0", Oid);
            _collectibleVariants.BindToParent(this, "CollectibleVariants", "FK_CollectibleVariant_Collectible");
        }

        public async Task LoadCollectibleVariantsAsync() {
            _collectibleVariants = await CollectibleVariant.FetchAsync("WHERE CollectibleOid = @0", Oid).ConfigureAwait(false);
            _collectibleVariants.BindToParent(this, "CollectibleVariants", "FK_CollectibleVariant_Collectible");
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "CARDSETOID": oReturn = this.CardSetOid; break;
                    case "NAME": oReturn = this.Name; break;
                    case "CARDNUMBER": oReturn = this.CardNumber; break;
                    case "RARITY": oReturn = this.Rarity; break;
                    case "CATEGORY": oReturn = this.Category; break;
                    case "ILLUSTRATOR": oReturn = this.Illustrator; break;
                    case "CREATEDON": oReturn = this.CreatedOn; break;
                    case "MODIFIEDON": oReturn = this.ModifiedOn; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "CARDSETOID": this.CardSetOid = (long)value; break;
                    case "NAME": this.Name = (string)value; break;
                    case "CARDNUMBER": this.CardNumber = (string)value; break;
                    case "RARITY": this.Rarity = (string)value; break;
                    case "CATEGORY": this.Category = (string)value; break;
                    case "ILLUSTRATOR": this.Illustrator = (string)value; break;
                    case "CREATEDON": this.CreatedOn = (DateTime)value; break;
                    case "MODIFIEDON": this.ModifiedOn = (DateTime?)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("CardSetOid", typeof(long));
            oReturn.Columns.Add("Name", typeof(string));
            oReturn.Columns.Add("CardNumber", typeof(string));
            oReturn.Columns.Add("Rarity", typeof(string));
            oReturn.Columns.Add("Category", typeof(string));
            oReturn.Columns.Add("Illustrator", typeof(string));
            oReturn.Columns.Add("CreatedOn", typeof(DateTime));
            oReturn.Columns.Add("ModifiedOn", typeof(DateTime?));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, CardSetOid, Name, CardNumber, Rarity, Category, Illustrator, CreatedOn, ModifiedOn);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Collectible().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Collectible().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
            foreach (CollectibleExternalId oChild in CollectibleExternalIds.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectibleImage oChild in CollectibleImages.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectibleName oChild in CollectibleNames.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectibleVariant oChild in CollectibleVariants.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
            foreach (CollectibleExternalId oChild in CollectibleExternalIds.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectibleImage oChild in CollectibleImages.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectibleName oChild in CollectibleNames.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectibleVariant oChild in CollectibleVariants.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (Collectible)

    //***************************************************
    //*  Class: CollectibleExternalId
    //***************************************************
    #region CollectibleExternalId
    public partial class CollectibleExternalId : Record<CollectibleExternalId>
    {

        #region Fields
        private long _oid;
        private long _collectibleOid;
        private long _dataSourceOid;
        private string _externalId;
        private Collectible? _collectible = null;
        private DataSource? _dataSource = null;
        #endregion (Fields)

        #region Constructor
        public CollectibleExternalId() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "CollectibleExternalId";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("CollectibleOid")]
        public long CollectibleOid { get { return _collectibleOid; } set { if (_collectibleOid != value) { _collectibleOid = value; IsChanged = true; } } }

        [Column("DataSourceOid")]
        public long DataSourceOid { get { return _dataSourceOid; } set { if (_dataSourceOid != value) { _dataSourceOid = value; IsChanged = true; } } }

        [Column("ExternalId")]
        public string ExternalId { get { return _externalId; } set { if (_externalId != value) { _externalId = value; IsChanged = true; } } }

        [Ignore]
        public Collectible? Collectible {
            get { if (_collectible == null) { LoadCollectible(); } return _collectible; }
            set { _collectible = value; IsChanged = true; }
        }

        [Ignore]
        public DataSource? DataSource {
            get { if (_dataSource == null) { LoadDataSource(); } return _dataSource; }
            set { _dataSource = value; IsChanged = true; }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCollectible() {
            if (CollectibleOid != null)
                _collectible = Collectible.First("WHERE Oid = @0", CollectibleOid);
        }

        public async Task LoadCollectibleAsync() {
            if (CollectibleOid != null)
                _collectible = await Collectible.FirstAsync("WHERE Oid = @0", CollectibleOid).ConfigureAwait(false);
        }

        private void LoadDataSource() {
            if (DataSourceOid != null)
                _dataSource = DataSource.First("WHERE Oid = @0", DataSourceOid);
        }

        public async Task LoadDataSourceAsync() {
            if (DataSourceOid != null)
                _dataSource = await DataSource.FirstAsync("WHERE Oid = @0", DataSourceOid).ConfigureAwait(false);
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "COLLECTIBLEOID": oReturn = this.CollectibleOid; break;
                    case "DATASOURCEOID": oReturn = this.DataSourceOid; break;
                    case "EXTERNALID": oReturn = this.ExternalId; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "COLLECTIBLEOID": this.CollectibleOid = (long)value; break;
                    case "DATASOURCEOID": this.DataSourceOid = (long)value; break;
                    case "EXTERNALID": this.ExternalId = (string)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("CollectibleOid", typeof(long));
            oReturn.Columns.Add("DataSourceOid", typeof(long));
            oReturn.Columns.Add("ExternalId", typeof(string));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, CollectibleOid, DataSourceOid, ExternalId);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectibleExternalId().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectibleExternalId().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (CollectibleExternalId)

    //***************************************************
    //*  Class: CollectibleImage
    //***************************************************
    #region CollectibleImage
    public partial class CollectibleImage : Record<CollectibleImage>
    {

        #region Fields
        private long _oid;
        private long _collectibleOid;
        private long _languageOid;
        private long _dataSourceOid;
        private string _imageUrl;
        private string _thumbnailUrl;
        private DateTime _createdOn;
        private Collectible? _collectible = null;
        private DataSource? _dataSource = null;
        private Language? _language = null;
        #endregion (Fields)

        #region Constructor
        public CollectibleImage() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "CollectibleImage";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("CollectibleOid")]
        public long CollectibleOid { get { return _collectibleOid; } set { if (_collectibleOid != value) { _collectibleOid = value; IsChanged = true; } } }

        [Column("LanguageOid")]
        public long LanguageOid { get { return _languageOid; } set { if (_languageOid != value) { _languageOid = value; IsChanged = true; } } }

        [Column("DataSourceOid")]
        public long DataSourceOid { get { return _dataSourceOid; } set { if (_dataSourceOid != value) { _dataSourceOid = value; IsChanged = true; } } }

        [Column("ImageUrl")]
        public string ImageUrl { get { return _imageUrl; } set { if (_imageUrl != value) { _imageUrl = value; IsChanged = true; } } }

        [Column("ThumbnailUrl")]
        public string ThumbnailUrl { get { return _thumbnailUrl; } set { if (_thumbnailUrl != value) { _thumbnailUrl = value; IsChanged = true; } } }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get { return _createdOn; } set { if (_createdOn != value) { _createdOn = value; IsChanged = true; } } }

        [Ignore]
        public Collectible? Collectible {
            get { if (_collectible == null) { LoadCollectible(); } return _collectible; }
            set { _collectible = value; IsChanged = true; }
        }

        [Ignore]
        public DataSource? DataSource {
            get { if (_dataSource == null) { LoadDataSource(); } return _dataSource; }
            set { _dataSource = value; IsChanged = true; }
        }

        [Ignore]
        public Language? Language {
            get { if (_language == null) { LoadLanguage(); } return _language; }
            set { _language = value; IsChanged = true; }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCollectible() {
            if (CollectibleOid != null)
                _collectible = Collectible.First("WHERE Oid = @0", CollectibleOid);
        }

        public async Task LoadCollectibleAsync() {
            if (CollectibleOid != null)
                _collectible = await Collectible.FirstAsync("WHERE Oid = @0", CollectibleOid).ConfigureAwait(false);
        }

        private void LoadDataSource() {
            if (DataSourceOid != null)
                _dataSource = DataSource.First("WHERE Oid = @0", DataSourceOid);
        }

        public async Task LoadDataSourceAsync() {
            if (DataSourceOid != null)
                _dataSource = await DataSource.FirstAsync("WHERE Oid = @0", DataSourceOid).ConfigureAwait(false);
        }

        private void LoadLanguage() {
            if (LanguageOid != null)
                _language = Language.First("WHERE Oid = @0", LanguageOid);
        }

        public async Task LoadLanguageAsync() {
            if (LanguageOid != null)
                _language = await Language.FirstAsync("WHERE Oid = @0", LanguageOid).ConfigureAwait(false);
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "COLLECTIBLEOID": oReturn = this.CollectibleOid; break;
                    case "LANGUAGEOID": oReturn = this.LanguageOid; break;
                    case "DATASOURCEOID": oReturn = this.DataSourceOid; break;
                    case "IMAGEURL": oReturn = this.ImageUrl; break;
                    case "THUMBNAILURL": oReturn = this.ThumbnailUrl; break;
                    case "CREATEDON": oReturn = this.CreatedOn; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "COLLECTIBLEOID": this.CollectibleOid = (long)value; break;
                    case "LANGUAGEOID": this.LanguageOid = (long)value; break;
                    case "DATASOURCEOID": this.DataSourceOid = (long)value; break;
                    case "IMAGEURL": this.ImageUrl = (string)value; break;
                    case "THUMBNAILURL": this.ThumbnailUrl = (string)value; break;
                    case "CREATEDON": this.CreatedOn = (DateTime)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("CollectibleOid", typeof(long));
            oReturn.Columns.Add("LanguageOid", typeof(long));
            oReturn.Columns.Add("DataSourceOid", typeof(long));
            oReturn.Columns.Add("ImageUrl", typeof(string));
            oReturn.Columns.Add("ThumbnailUrl", typeof(string));
            oReturn.Columns.Add("CreatedOn", typeof(DateTime));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, CollectibleOid, LanguageOid, DataSourceOid, ImageUrl, ThumbnailUrl, CreatedOn);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectibleImage().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectibleImage().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (CollectibleImage)

    //***************************************************
    //*  Class: CollectibleName
    //***************************************************
    #region CollectibleName
    public partial class CollectibleName : Record<CollectibleName>
    {

        #region Fields
        private long _oid;
        private long _collectibleOid;
        private long _languageOid;
        private string _name;
        private Collectible? _collectible = null;
        private Language? _language = null;
        #endregion (Fields)

        #region Constructor
        public CollectibleName() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "CollectibleName";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("CollectibleOid")]
        public long CollectibleOid { get { return _collectibleOid; } set { if (_collectibleOid != value) { _collectibleOid = value; IsChanged = true; } } }

        [Column("LanguageOid")]
        public long LanguageOid { get { return _languageOid; } set { if (_languageOid != value) { _languageOid = value; IsChanged = true; } } }

        [Column("Name")]
        public string Name { get { return _name; } set { if (_name != value) { _name = value; IsChanged = true; } } }

        [Ignore]
        public Collectible? Collectible {
            get { if (_collectible == null) { LoadCollectible(); } return _collectible; }
            set { _collectible = value; IsChanged = true; }
        }

        [Ignore]
        public Language? Language {
            get { if (_language == null) { LoadLanguage(); } return _language; }
            set { _language = value; IsChanged = true; }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCollectible() {
            if (CollectibleOid != null)
                _collectible = Collectible.First("WHERE Oid = @0", CollectibleOid);
        }

        public async Task LoadCollectibleAsync() {
            if (CollectibleOid != null)
                _collectible = await Collectible.FirstAsync("WHERE Oid = @0", CollectibleOid).ConfigureAwait(false);
        }

        private void LoadLanguage() {
            if (LanguageOid != null)
                _language = Language.First("WHERE Oid = @0", LanguageOid);
        }

        public async Task LoadLanguageAsync() {
            if (LanguageOid != null)
                _language = await Language.FirstAsync("WHERE Oid = @0", LanguageOid).ConfigureAwait(false);
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "COLLECTIBLEOID": oReturn = this.CollectibleOid; break;
                    case "LANGUAGEOID": oReturn = this.LanguageOid; break;
                    case "NAME": oReturn = this.Name; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "COLLECTIBLEOID": this.CollectibleOid = (long)value; break;
                    case "LANGUAGEOID": this.LanguageOid = (long)value; break;
                    case "NAME": this.Name = (string)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("CollectibleOid", typeof(long));
            oReturn.Columns.Add("LanguageOid", typeof(long));
            oReturn.Columns.Add("Name", typeof(string));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, CollectibleOid, LanguageOid, Name);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectibleName().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectibleName().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (CollectibleName)

    //***************************************************
    //*  Class: CollectiblePrice
    //***************************************************
    #region CollectiblePrice
    public partial class CollectiblePrice : Record<CollectiblePrice>
    {

        #region Fields
        private long _oid;
        private long _collectibleVariantOid;
        private long _dataSourceOid;
        private long? _dataSourceOid_RetrievedFrom;
        private bool _isGraded;
        private long? _gradeOid;
        private long? _conditionOid;
        private string _qualifier;
        private decimal _price;
        private decimal? _lowPrice;
        private decimal? _highPrice;
        private string _currencyCode;
        private DateTime _lastConfirmed;
        private string _sourceUrl;
        private DateTime _createdOn;
        private DateTime? _modifiedOn;
        private ManagedList<CollectiblePriceHistory>? _collectiblePriceHistories = null;
        private CollectibleVariant? _collectibleVariant = null;
        private Condition? _condition = null;
        private DataSource? _dataSource = null;
        private Grade? _grade = null;
        #endregion (Fields)

        #region Constructor
        public CollectiblePrice() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "CollectiblePrice";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("CollectibleVariantOid")]
        public long CollectibleVariantOid { get { return _collectibleVariantOid; } set { if (_collectibleVariantOid != value) { _collectibleVariantOid = value; IsChanged = true; } } }

        [Column("DataSourceOid")]
        public long DataSourceOid { get { return _dataSourceOid; } set { if (_dataSourceOid != value) { _dataSourceOid = value; IsChanged = true; } } }

        [Column("DataSourceOid_RetrievedFrom")]
        public long? DataSourceOid_RetrievedFrom { get { return _dataSourceOid_RetrievedFrom; } set { if (_dataSourceOid_RetrievedFrom != value) { _dataSourceOid_RetrievedFrom = value; IsChanged = true; } } }

        [Column("IsGraded")]
        public bool IsGraded { get { return _isGraded; } set { if (_isGraded != value) { _isGraded = value; IsChanged = true; } } }

        [Column("GradeOid")]
        public long? GradeOid { get { return _gradeOid; } set { if (_gradeOid != value) { _gradeOid = value; IsChanged = true; } } }

        [Column("ConditionOid")]
        public long? ConditionOid { get { return _conditionOid; } set { if (_conditionOid != value) { _conditionOid = value; IsChanged = true; } } }

        [Column("Qualifier")]
        public string Qualifier { get { return _qualifier; } set { if (_qualifier != value) { _qualifier = value; IsChanged = true; } } }

        [Column("Price")]
        public decimal Price { get { return _price; } set { if (_price != value) { _price = value; IsChanged = true; } } }

        [Column("LowPrice")]
        public decimal? LowPrice { get { return _lowPrice; } set { if (_lowPrice != value) { _lowPrice = value; IsChanged = true; } } }

        [Column("HighPrice")]
        public decimal? HighPrice { get { return _highPrice; } set { if (_highPrice != value) { _highPrice = value; IsChanged = true; } } }

        [Column("CurrencyCode")]
        public string CurrencyCode { get { return _currencyCode; } set { if (_currencyCode != value) { _currencyCode = value; IsChanged = true; } } }

        [Column("LastConfirmed")]
        public DateTime LastConfirmed { get { return _lastConfirmed; } set { if (_lastConfirmed != value) { _lastConfirmed = value; IsChanged = true; } } }

        [Column("SourceUrl")]
        public string SourceUrl { get { return _sourceUrl; } set { if (_sourceUrl != value) { _sourceUrl = value; IsChanged = true; } } }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get { return _createdOn; } set { if (_createdOn != value) { _createdOn = value; IsChanged = true; } } }

        [Column("ModifiedOn")]
        public DateTime? ModifiedOn { get { return _modifiedOn; } set { if (_modifiedOn != value) { _modifiedOn = value; IsChanged = true; } } }

        [Ignore]
        public ManagedList<CollectiblePriceHistory>? CollectiblePriceHistories {
            get { if (_collectiblePriceHistories == null) { LoadCollectiblePriceHistories(); } return _collectiblePriceHistories; }
            set {
                if (_collectiblePriceHistories != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectiblePriceHistories", "FK_CollectiblePriceHistory_CollectiblePrice");
                    }
                    _collectiblePriceHistories = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public CollectibleVariant? CollectibleVariant {
            get { if (_collectibleVariant == null) { LoadCollectibleVariant(); } return _collectibleVariant; }
            set { _collectibleVariant = value; IsChanged = true; }
        }

        [Ignore]
        public Condition? Condition {
            get { if (_condition == null) { LoadCondition(); } return _condition; }
            set { _condition = value; IsChanged = true; }
        }

        [Ignore]
        public DataSource? DataSource {
            get { if (_dataSource == null) { LoadDataSource(); } return _dataSource; }
            set { _dataSource = value; IsChanged = true; }
        }

        [Ignore]
        public Grade? Grade {
            get { if (_grade == null) { LoadGrade(); } return _grade; }
            set { _grade = value; IsChanged = true; }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCollectiblePriceHistories() {
            _collectiblePriceHistories = CollectiblePriceHistory.Fetch("WHERE CollectiblePriceOid = @0", Oid);
            _collectiblePriceHistories.BindToParent(this, "CollectiblePriceHistories", "FK_CollectiblePriceHistory_CollectiblePrice");
        }

        public async Task LoadCollectiblePriceHistoriesAsync() {
            _collectiblePriceHistories = await CollectiblePriceHistory.FetchAsync("WHERE CollectiblePriceOid = @0", Oid).ConfigureAwait(false);
            _collectiblePriceHistories.BindToParent(this, "CollectiblePriceHistories", "FK_CollectiblePriceHistory_CollectiblePrice");
        }

        private void LoadCollectibleVariant() {
            if (CollectibleVariantOid != null)
                _collectibleVariant = CollectibleVariant.First("WHERE Oid = @0", CollectibleVariantOid);
        }

        public async Task LoadCollectibleVariantAsync() {
            if (CollectibleVariantOid != null)
                _collectibleVariant = await CollectibleVariant.FirstAsync("WHERE Oid = @0", CollectibleVariantOid).ConfigureAwait(false);
        }

        private void LoadCondition() {
            if (ConditionOid != null)
                _condition = Condition.First("WHERE Oid = @0", ConditionOid);
        }

        public async Task LoadConditionAsync() {
            if (ConditionOid != null)
                _condition = await Condition.FirstAsync("WHERE Oid = @0", ConditionOid).ConfigureAwait(false);
        }

        private void LoadDataSource() {
            if (DataSourceOid != null)
                _dataSource = DataSource.First("WHERE Oid = @0", DataSourceOid);
        }

        public async Task LoadDataSourceAsync() {
            if (DataSourceOid != null)
                _dataSource = await DataSource.FirstAsync("WHERE Oid = @0", DataSourceOid).ConfigureAwait(false);
        }

        private void LoadGrade() {
            if (GradeOid != null)
                _grade = Grade.First("WHERE Oid = @0", GradeOid);
        }

        public async Task LoadGradeAsync() {
            if (GradeOid != null)
                _grade = await Grade.FirstAsync("WHERE Oid = @0", GradeOid).ConfigureAwait(false);
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "COLLECTIBLEVARIANTOID": oReturn = this.CollectibleVariantOid; break;
                    case "DATASOURCEOID": oReturn = this.DataSourceOid; break;
                    case "DATASOURCEOID_RETRIEVEDFROM": oReturn = this.DataSourceOid_RetrievedFrom; break;
                    case "ISGRADED": oReturn = this.IsGraded; break;
                    case "GRADEOID": oReturn = this.GradeOid; break;
                    case "CONDITIONOID": oReturn = this.ConditionOid; break;
                    case "QUALIFIER": oReturn = this.Qualifier; break;
                    case "PRICE": oReturn = this.Price; break;
                    case "LOWPRICE": oReturn = this.LowPrice; break;
                    case "HIGHPRICE": oReturn = this.HighPrice; break;
                    case "CURRENCYCODE": oReturn = this.CurrencyCode; break;
                    case "LASTCONFIRMED": oReturn = this.LastConfirmed; break;
                    case "SOURCEURL": oReturn = this.SourceUrl; break;
                    case "CREATEDON": oReturn = this.CreatedOn; break;
                    case "MODIFIEDON": oReturn = this.ModifiedOn; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "COLLECTIBLEVARIANTOID": this.CollectibleVariantOid = (long)value; break;
                    case "DATASOURCEOID": this.DataSourceOid = (long)value; break;
                    case "DATASOURCEOID_RETRIEVEDFROM": this.DataSourceOid_RetrievedFrom = (long?)value; break;
                    case "ISGRADED": this.IsGraded = (bool)value; break;
                    case "GRADEOID": this.GradeOid = (long?)value; break;
                    case "CONDITIONOID": this.ConditionOid = (long?)value; break;
                    case "QUALIFIER": this.Qualifier = (string)value; break;
                    case "PRICE": this.Price = (decimal)value; break;
                    case "LOWPRICE": this.LowPrice = (decimal?)value; break;
                    case "HIGHPRICE": this.HighPrice = (decimal?)value; break;
                    case "CURRENCYCODE": this.CurrencyCode = (string)value; break;
                    case "LASTCONFIRMED": this.LastConfirmed = (DateTime)value; break;
                    case "SOURCEURL": this.SourceUrl = (string)value; break;
                    case "CREATEDON": this.CreatedOn = (DateTime)value; break;
                    case "MODIFIEDON": this.ModifiedOn = (DateTime?)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("CollectibleVariantOid", typeof(long));
            oReturn.Columns.Add("DataSourceOid", typeof(long));
            oReturn.Columns.Add("DataSourceOid_RetrievedFrom", typeof(long?));
            oReturn.Columns.Add("IsGraded", typeof(bool));
            oReturn.Columns.Add("GradeOid", typeof(long?));
            oReturn.Columns.Add("ConditionOid", typeof(long?));
            oReturn.Columns.Add("Qualifier", typeof(string));
            oReturn.Columns.Add("Price", typeof(decimal));
            oReturn.Columns.Add("LowPrice", typeof(decimal?));
            oReturn.Columns.Add("HighPrice", typeof(decimal?));
            oReturn.Columns.Add("CurrencyCode", typeof(string));
            oReturn.Columns.Add("LastConfirmed", typeof(DateTime));
            oReturn.Columns.Add("SourceUrl", typeof(string));
            oReturn.Columns.Add("CreatedOn", typeof(DateTime));
            oReturn.Columns.Add("ModifiedOn", typeof(DateTime?));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, CollectibleVariantOid, DataSourceOid, DataSourceOid_RetrievedFrom, IsGraded, GradeOid, ConditionOid, Qualifier, Price, LowPrice, HighPrice, CurrencyCode, LastConfirmed, SourceUrl, CreatedOn, ModifiedOn);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectiblePrice().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectiblePrice().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
            foreach (CollectiblePriceHistory oChild in CollectiblePriceHistories.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
            foreach (CollectiblePriceHistory oChild in CollectiblePriceHistories.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
        }

        public override void GeneratedCascadingDeleteInternal() {
            foreach (CollectiblePriceHistory oChild in CollectiblePriceHistories.ToList()) {
                oChild.GeneratedCascadingDeleteInternal();
            }
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            foreach (CollectiblePriceHistory oChild in CollectiblePriceHistories.ToList()) {
                await oChild.GeneratedCascadingDeleteInternalAsync(toDb).ConfigureAwait(false);
            }
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (CollectiblePrice)

    //***************************************************
    //*  Class: CollectiblePriceHistory
    //***************************************************
    #region CollectiblePriceHistory
    public partial class CollectiblePriceHistory : Record<CollectiblePriceHistory>
    {

        #region Fields
        private long _oid;
        private long _collectiblePriceOid;
        private decimal _price;
        private decimal? _lowPrice;
        private decimal? _highPrice;
        private string _currencyCode;
        private DateTime _firstConfirmed;
        private DateTime _lastConfirmed;
        private CollectiblePrice? _collectiblePrice = null;
        #endregion (Fields)

        #region Constructor
        public CollectiblePriceHistory() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "CollectiblePriceHistory";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("CollectiblePriceOid")]
        public long CollectiblePriceOid { get { return _collectiblePriceOid; } set { if (_collectiblePriceOid != value) { _collectiblePriceOid = value; IsChanged = true; } } }

        [Column("Price")]
        public decimal Price { get { return _price; } set { if (_price != value) { _price = value; IsChanged = true; } } }

        [Column("LowPrice")]
        public decimal? LowPrice { get { return _lowPrice; } set { if (_lowPrice != value) { _lowPrice = value; IsChanged = true; } } }

        [Column("HighPrice")]
        public decimal? HighPrice { get { return _highPrice; } set { if (_highPrice != value) { _highPrice = value; IsChanged = true; } } }

        [Column("CurrencyCode")]
        public string CurrencyCode { get { return _currencyCode; } set { if (_currencyCode != value) { _currencyCode = value; IsChanged = true; } } }

        [Column("FirstConfirmed")]
        public DateTime FirstConfirmed { get { return _firstConfirmed; } set { if (_firstConfirmed != value) { _firstConfirmed = value; IsChanged = true; } } }

        [Column("LastConfirmed")]
        public DateTime LastConfirmed { get { return _lastConfirmed; } set { if (_lastConfirmed != value) { _lastConfirmed = value; IsChanged = true; } } }

        [Ignore]
        public CollectiblePrice? CollectiblePrice {
            get { if (_collectiblePrice == null) { LoadCollectiblePrice(); } return _collectiblePrice; }
            set { _collectiblePrice = value; IsChanged = true; }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCollectiblePrice() {
            if (CollectiblePriceOid != null)
                _collectiblePrice = CollectiblePrice.First("WHERE Oid = @0", CollectiblePriceOid);
        }

        public async Task LoadCollectiblePriceAsync() {
            if (CollectiblePriceOid != null)
                _collectiblePrice = await CollectiblePrice.FirstAsync("WHERE Oid = @0", CollectiblePriceOid).ConfigureAwait(false);
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "COLLECTIBLEPRICEOID": oReturn = this.CollectiblePriceOid; break;
                    case "PRICE": oReturn = this.Price; break;
                    case "LOWPRICE": oReturn = this.LowPrice; break;
                    case "HIGHPRICE": oReturn = this.HighPrice; break;
                    case "CURRENCYCODE": oReturn = this.CurrencyCode; break;
                    case "FIRSTCONFIRMED": oReturn = this.FirstConfirmed; break;
                    case "LASTCONFIRMED": oReturn = this.LastConfirmed; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "COLLECTIBLEPRICEOID": this.CollectiblePriceOid = (long)value; break;
                    case "PRICE": this.Price = (decimal)value; break;
                    case "LOWPRICE": this.LowPrice = (decimal?)value; break;
                    case "HIGHPRICE": this.HighPrice = (decimal?)value; break;
                    case "CURRENCYCODE": this.CurrencyCode = (string)value; break;
                    case "FIRSTCONFIRMED": this.FirstConfirmed = (DateTime)value; break;
                    case "LASTCONFIRMED": this.LastConfirmed = (DateTime)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("CollectiblePriceOid", typeof(long));
            oReturn.Columns.Add("Price", typeof(decimal));
            oReturn.Columns.Add("LowPrice", typeof(decimal?));
            oReturn.Columns.Add("HighPrice", typeof(decimal?));
            oReturn.Columns.Add("CurrencyCode", typeof(string));
            oReturn.Columns.Add("FirstConfirmed", typeof(DateTime));
            oReturn.Columns.Add("LastConfirmed", typeof(DateTime));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, CollectiblePriceOid, Price, LowPrice, HighPrice, CurrencyCode, FirstConfirmed, LastConfirmed);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectiblePriceHistory().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectiblePriceHistory().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (CollectiblePriceHistory)

    //***************************************************
    //*  Class: CollectibleType
    //***************************************************
    #region CollectibleType
    public partial class CollectibleType : Record<CollectibleType>
    {

        #region Fields
        private long _oid;
        private string _name;
        private bool _isActive;
        private ManagedList<Game>? _games = null;
        #endregion (Fields)

        #region Constructor
        public CollectibleType() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "CollectibleType";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("Name")]
        public string Name { get { return _name; } set { if (_name != value) { _name = value; IsChanged = true; } } }

        [Column("IsActive")]
        public bool IsActive { get { return _isActive; } set { if (_isActive != value) { _isActive = value; IsChanged = true; } } }

        [Ignore]
        public ManagedList<Game>? Games {
            get { if (_games == null) { LoadGames(); } return _games; }
            set {
                if (_games != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "Games", "FK_Game_CollectibleType");
                    }
                    _games = value;
                    IsChanged = true;
                }
            }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadGames() {
            _games = Game.Fetch("WHERE CollectibleTypeOid = @0", Oid);
            _games.BindToParent(this, "Games", "FK_Game_CollectibleType");
        }

        public async Task LoadGamesAsync() {
            _games = await Game.FetchAsync("WHERE CollectibleTypeOid = @0", Oid).ConfigureAwait(false);
            _games.BindToParent(this, "Games", "FK_Game_CollectibleType");
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "NAME": oReturn = this.Name; break;
                    case "ISACTIVE": oReturn = this.IsActive; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "NAME": this.Name = (string)value; break;
                    case "ISACTIVE": this.IsActive = (bool)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("Name", typeof(string));
            oReturn.Columns.Add("IsActive", typeof(bool));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, Name, IsActive);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectibleType().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectibleType().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
            foreach (Game oChild in Games.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
            foreach (Game oChild in Games.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (CollectibleType)

    //***************************************************
    //*  Class: CollectibleVariant
    //***************************************************
    #region CollectibleVariant
    public partial class CollectibleVariant : Record<CollectibleVariant>
    {

        #region Fields
        private long _oid;
        private long _collectibleOid;
        private long _printingOid;
        private long _languageOid;
        private string _subtype;
        private string _stamp;
        private DateTime _createdOn;
        private DateTime? _modifiedOn;
        private Collectible? _collectible = null;
        private ManagedList<Cert>? _certs = null;
        private ManagedList<CollectiblePrice>? _collectiblePrices = null;
        private ManagedList<CollectibleVariantExternalId>? _collectibleVariantExternalIds = null;
        private ManagedList<CollectionItem>? _collectionItems = null;
        private Language? _language = null;
        private Printing? _printing = null;
        #endregion (Fields)

        #region Constructor
        public CollectibleVariant() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "CollectibleVariant";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("CollectibleOid")]
        public long CollectibleOid { get { return _collectibleOid; } set { if (_collectibleOid != value) { _collectibleOid = value; IsChanged = true; } } }

        [Column("PrintingOid")]
        public long PrintingOid { get { return _printingOid; } set { if (_printingOid != value) { _printingOid = value; IsChanged = true; } } }

        [Column("LanguageOid")]
        public long LanguageOid { get { return _languageOid; } set { if (_languageOid != value) { _languageOid = value; IsChanged = true; } } }

        [Column("Subtype")]
        public string Subtype { get { return _subtype; } set { if (_subtype != value) { _subtype = value; IsChanged = true; } } }

        [Column("Stamp")]
        public string Stamp { get { return _stamp; } set { if (_stamp != value) { _stamp = value; IsChanged = true; } } }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get { return _createdOn; } set { if (_createdOn != value) { _createdOn = value; IsChanged = true; } } }

        [Column("ModifiedOn")]
        public DateTime? ModifiedOn { get { return _modifiedOn; } set { if (_modifiedOn != value) { _modifiedOn = value; IsChanged = true; } } }

        [Ignore]
        public Collectible? Collectible {
            get { if (_collectible == null) { LoadCollectible(); } return _collectible; }
            set { _collectible = value; IsChanged = true; }
        }

        [Ignore]
        public ManagedList<Cert>? Certs {
            get { if (_certs == null) { LoadCerts(); } return _certs; }
            set {
                if (_certs != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "Certs", "FK_Cert_CollectibleVariant");
                    }
                    _certs = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CollectiblePrice>? CollectiblePrices {
            get { if (_collectiblePrices == null) { LoadCollectiblePrices(); } return _collectiblePrices; }
            set {
                if (_collectiblePrices != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectiblePrices", "FK_CollectiblePrice_CollectibleVariant");
                    }
                    _collectiblePrices = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CollectibleVariantExternalId>? CollectibleVariantExternalIds {
            get { if (_collectibleVariantExternalIds == null) { LoadCollectibleVariantExternalIds(); } return _collectibleVariantExternalIds; }
            set {
                if (_collectibleVariantExternalIds != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectibleVariantExternalIds", "FK_CollectibleVariantExternalId_CollectibleVariant");
                    }
                    _collectibleVariantExternalIds = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CollectionItem>? CollectionItems {
            get { if (_collectionItems == null) { LoadCollectionItems(); } return _collectionItems; }
            set {
                if (_collectionItems != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectionItems", "FK_CollectionItem_CollectibleVariant");
                    }
                    _collectionItems = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public Language? Language {
            get { if (_language == null) { LoadLanguage(); } return _language; }
            set { _language = value; IsChanged = true; }
        }

        [Ignore]
        public Printing? Printing {
            get { if (_printing == null) { LoadPrinting(); } return _printing; }
            set { _printing = value; IsChanged = true; }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCollectible() {
            if (CollectibleOid != null)
                _collectible = Collectible.First("WHERE Oid = @0", CollectibleOid);
        }

        public async Task LoadCollectibleAsync() {
            if (CollectibleOid != null)
                _collectible = await Collectible.FirstAsync("WHERE Oid = @0", CollectibleOid).ConfigureAwait(false);
        }

        private void LoadCerts() {
            _certs = Cert.Fetch("WHERE CollectibleVariantOid = @0", Oid);
            _certs.BindToParent(this, "Certs", "FK_Cert_CollectibleVariant");
        }

        public async Task LoadCertsAsync() {
            _certs = await Cert.FetchAsync("WHERE CollectibleVariantOid = @0", Oid).ConfigureAwait(false);
            _certs.BindToParent(this, "Certs", "FK_Cert_CollectibleVariant");
        }

        private void LoadCollectiblePrices() {
            _collectiblePrices = CollectiblePrice.Fetch("WHERE CollectibleVariantOid = @0", Oid);
            _collectiblePrices.BindToParent(this, "CollectiblePrices", "FK_CollectiblePrice_CollectibleVariant");
        }

        public async Task LoadCollectiblePricesAsync() {
            _collectiblePrices = await CollectiblePrice.FetchAsync("WHERE CollectibleVariantOid = @0", Oid).ConfigureAwait(false);
            _collectiblePrices.BindToParent(this, "CollectiblePrices", "FK_CollectiblePrice_CollectibleVariant");
        }

        private void LoadCollectibleVariantExternalIds() {
            _collectibleVariantExternalIds = CollectibleVariantExternalId.Fetch("WHERE CollectibleVariantOid = @0", Oid);
            _collectibleVariantExternalIds.BindToParent(this, "CollectibleVariantExternalIds", "FK_CollectibleVariantExternalId_CollectibleVariant");
        }

        public async Task LoadCollectibleVariantExternalIdsAsync() {
            _collectibleVariantExternalIds = await CollectibleVariantExternalId.FetchAsync("WHERE CollectibleVariantOid = @0", Oid).ConfigureAwait(false);
            _collectibleVariantExternalIds.BindToParent(this, "CollectibleVariantExternalIds", "FK_CollectibleVariantExternalId_CollectibleVariant");
        }

        private void LoadCollectionItems() {
            _collectionItems = CollectionItem.Fetch("WHERE CollectibleVariantOid = @0", Oid);
            _collectionItems.BindToParent(this, "CollectionItems", "FK_CollectionItem_CollectibleVariant");
        }

        public async Task LoadCollectionItemsAsync() {
            _collectionItems = await CollectionItem.FetchAsync("WHERE CollectibleVariantOid = @0", Oid).ConfigureAwait(false);
            _collectionItems.BindToParent(this, "CollectionItems", "FK_CollectionItem_CollectibleVariant");
        }

        private void LoadLanguage() {
            if (LanguageOid != null)
                _language = Language.First("WHERE Oid = @0", LanguageOid);
        }

        public async Task LoadLanguageAsync() {
            if (LanguageOid != null)
                _language = await Language.FirstAsync("WHERE Oid = @0", LanguageOid).ConfigureAwait(false);
        }

        private void LoadPrinting() {
            if (PrintingOid != null)
                _printing = Printing.First("WHERE Oid = @0", PrintingOid);
        }

        public async Task LoadPrintingAsync() {
            if (PrintingOid != null)
                _printing = await Printing.FirstAsync("WHERE Oid = @0", PrintingOid).ConfigureAwait(false);
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "COLLECTIBLEOID": oReturn = this.CollectibleOid; break;
                    case "PRINTINGOID": oReturn = this.PrintingOid; break;
                    case "LANGUAGEOID": oReturn = this.LanguageOid; break;
                    case "SUBTYPE": oReturn = this.Subtype; break;
                    case "STAMP": oReturn = this.Stamp; break;
                    case "CREATEDON": oReturn = this.CreatedOn; break;
                    case "MODIFIEDON": oReturn = this.ModifiedOn; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "COLLECTIBLEOID": this.CollectibleOid = (long)value; break;
                    case "PRINTINGOID": this.PrintingOid = (long)value; break;
                    case "LANGUAGEOID": this.LanguageOid = (long)value; break;
                    case "SUBTYPE": this.Subtype = (string)value; break;
                    case "STAMP": this.Stamp = (string)value; break;
                    case "CREATEDON": this.CreatedOn = (DateTime)value; break;
                    case "MODIFIEDON": this.ModifiedOn = (DateTime?)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("CollectibleOid", typeof(long));
            oReturn.Columns.Add("PrintingOid", typeof(long));
            oReturn.Columns.Add("LanguageOid", typeof(long));
            oReturn.Columns.Add("Subtype", typeof(string));
            oReturn.Columns.Add("Stamp", typeof(string));
            oReturn.Columns.Add("CreatedOn", typeof(DateTime));
            oReturn.Columns.Add("ModifiedOn", typeof(DateTime?));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, CollectibleOid, PrintingOid, LanguageOid, Subtype, Stamp, CreatedOn, ModifiedOn);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectibleVariant().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectibleVariant().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
            foreach (Cert oChild in Certs.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectiblePrice oChild in CollectiblePrices.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectibleVariantExternalId oChild in CollectibleVariantExternalIds.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectionItem oChild in CollectionItems.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
            foreach (Cert oChild in Certs.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectiblePrice oChild in CollectiblePrices.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectibleVariantExternalId oChild in CollectibleVariantExternalIds.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectionItem oChild in CollectionItems.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (CollectibleVariant)

    //***************************************************
    //*  Class: CollectibleVariantExternalId
    //***************************************************
    #region CollectibleVariantExternalId
    public partial class CollectibleVariantExternalId : Record<CollectibleVariantExternalId>
    {

        #region Fields
        private long _oid;
        private long _collectibleVariantOid;
        private long _dataSourceOid;
        private string _externalId;
        private CollectibleVariant? _collectibleVariant = null;
        private DataSource? _dataSource = null;
        #endregion (Fields)

        #region Constructor
        public CollectibleVariantExternalId() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "CollectibleVariantExternalId";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("CollectibleVariantOid")]
        public long CollectibleVariantOid { get { return _collectibleVariantOid; } set { if (_collectibleVariantOid != value) { _collectibleVariantOid = value; IsChanged = true; } } }

        [Column("DataSourceOid")]
        public long DataSourceOid { get { return _dataSourceOid; } set { if (_dataSourceOid != value) { _dataSourceOid = value; IsChanged = true; } } }

        [Column("ExternalId")]
        public string ExternalId { get { return _externalId; } set { if (_externalId != value) { _externalId = value; IsChanged = true; } } }

        [Ignore]
        public CollectibleVariant? CollectibleVariant {
            get { if (_collectibleVariant == null) { LoadCollectibleVariant(); } return _collectibleVariant; }
            set { _collectibleVariant = value; IsChanged = true; }
        }

        [Ignore]
        public DataSource? DataSource {
            get { if (_dataSource == null) { LoadDataSource(); } return _dataSource; }
            set { _dataSource = value; IsChanged = true; }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCollectibleVariant() {
            if (CollectibleVariantOid != null)
                _collectibleVariant = CollectibleVariant.First("WHERE Oid = @0", CollectibleVariantOid);
        }

        public async Task LoadCollectibleVariantAsync() {
            if (CollectibleVariantOid != null)
                _collectibleVariant = await CollectibleVariant.FirstAsync("WHERE Oid = @0", CollectibleVariantOid).ConfigureAwait(false);
        }

        private void LoadDataSource() {
            if (DataSourceOid != null)
                _dataSource = DataSource.First("WHERE Oid = @0", DataSourceOid);
        }

        public async Task LoadDataSourceAsync() {
            if (DataSourceOid != null)
                _dataSource = await DataSource.FirstAsync("WHERE Oid = @0", DataSourceOid).ConfigureAwait(false);
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "COLLECTIBLEVARIANTOID": oReturn = this.CollectibleVariantOid; break;
                    case "DATASOURCEOID": oReturn = this.DataSourceOid; break;
                    case "EXTERNALID": oReturn = this.ExternalId; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "COLLECTIBLEVARIANTOID": this.CollectibleVariantOid = (long)value; break;
                    case "DATASOURCEOID": this.DataSourceOid = (long)value; break;
                    case "EXTERNALID": this.ExternalId = (string)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("CollectibleVariantOid", typeof(long));
            oReturn.Columns.Add("DataSourceOid", typeof(long));
            oReturn.Columns.Add("ExternalId", typeof(string));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, CollectibleVariantOid, DataSourceOid, ExternalId);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectibleVariantExternalId().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectibleVariantExternalId().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (CollectibleVariantExternalId)

    //***************************************************
    //*  Class: CollectionItem
    //***************************************************
    #region CollectionItem
    public partial class CollectionItem : Record<CollectionItem>
    {

        #region Fields
        private long _oid;
        private long _collectorOid;
        private long? _collectibleVariantOid;
        private long? _certOid;
        private long? _conditionOid;
        private DateTime? _acquiredOn;
        private decimal? _purchasePrice;
        private string _notes;
        private DateTime _createdOn;
        private DateTime? _modifiedOn;
        private long? _userAuthOid_CreatedBy;
        private long? _userAuthOid_ModifiedBy;
        private Cert? _cert = null;
        private CollectibleVariant? _collectibleVariant = null;
        private Collector? _collector = null;
        private Condition? _condition = null;
        #endregion (Fields)

        #region Constructor
        public CollectionItem() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "CollectionItem";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("CollectorOid")]
        public long CollectorOid { get { return _collectorOid; } set { if (_collectorOid != value) { _collectorOid = value; IsChanged = true; } } }

        [Column("CollectibleVariantOid")]
        public long? CollectibleVariantOid { get { return _collectibleVariantOid; } set { if (_collectibleVariantOid != value) { _collectibleVariantOid = value; IsChanged = true; } } }

        [Column("CertOid")]
        public long? CertOid { get { return _certOid; } set { if (_certOid != value) { _certOid = value; IsChanged = true; } } }

        [Column("ConditionOid")]
        public long? ConditionOid { get { return _conditionOid; } set { if (_conditionOid != value) { _conditionOid = value; IsChanged = true; } } }

        [Column("AcquiredOn")]
        public DateTime? AcquiredOn { get { return _acquiredOn; } set { if (_acquiredOn != value) { _acquiredOn = value; IsChanged = true; } } }

        [Column("PurchasePrice")]
        public decimal? PurchasePrice { get { return _purchasePrice; } set { if (_purchasePrice != value) { _purchasePrice = value; IsChanged = true; } } }

        [Column("Notes")]
        public string Notes { get { return _notes; } set { if (_notes != value) { _notes = value; IsChanged = true; } } }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get { return _createdOn; } set { if (_createdOn != value) { _createdOn = value; IsChanged = true; } } }

        [Column("ModifiedOn")]
        public DateTime? ModifiedOn { get { return _modifiedOn; } set { if (_modifiedOn != value) { _modifiedOn = value; IsChanged = true; } } }

        [Column("UserAuthOid_CreatedBy")]
        public long? UserAuthOid_CreatedBy { get { return _userAuthOid_CreatedBy; } set { if (_userAuthOid_CreatedBy != value) { _userAuthOid_CreatedBy = value; IsChanged = true; } } }

        [Column("UserAuthOid_ModifiedBy")]
        public long? UserAuthOid_ModifiedBy { get { return _userAuthOid_ModifiedBy; } set { if (_userAuthOid_ModifiedBy != value) { _userAuthOid_ModifiedBy = value; IsChanged = true; } } }

        [Ignore]
        public Cert? Cert {
            get { if (_cert == null) { LoadCert(); } return _cert; }
            set { _cert = value; IsChanged = true; }
        }

        [Ignore]
        public CollectibleVariant? CollectibleVariant {
            get { if (_collectibleVariant == null) { LoadCollectibleVariant(); } return _collectibleVariant; }
            set { _collectibleVariant = value; IsChanged = true; }
        }

        [Ignore]
        public Collector? Collector {
            get { if (_collector == null) { LoadCollector(); } return _collector; }
            set { _collector = value; IsChanged = true; }
        }

        [Ignore]
        public Condition? Condition {
            get { if (_condition == null) { LoadCondition(); } return _condition; }
            set { _condition = value; IsChanged = true; }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCert() {
            if (CertOid != null)
                _cert = Cert.First("WHERE Oid = @0", CertOid);
        }

        public async Task LoadCertAsync() {
            if (CertOid != null)
                _cert = await Cert.FirstAsync("WHERE Oid = @0", CertOid).ConfigureAwait(false);
        }

        private void LoadCollectibleVariant() {
            if (CollectibleVariantOid != null)
                _collectibleVariant = CollectibleVariant.First("WHERE Oid = @0", CollectibleVariantOid);
        }

        public async Task LoadCollectibleVariantAsync() {
            if (CollectibleVariantOid != null)
                _collectibleVariant = await CollectibleVariant.FirstAsync("WHERE Oid = @0", CollectibleVariantOid).ConfigureAwait(false);
        }

        private void LoadCollector() {
            if (CollectorOid != null)
                _collector = Collector.First("WHERE Oid = @0", CollectorOid);
        }

        public async Task LoadCollectorAsync() {
            if (CollectorOid != null)
                _collector = await Collector.FirstAsync("WHERE Oid = @0", CollectorOid).ConfigureAwait(false);
        }

        private void LoadCondition() {
            if (ConditionOid != null)
                _condition = Condition.First("WHERE Oid = @0", ConditionOid);
        }

        public async Task LoadConditionAsync() {
            if (ConditionOid != null)
                _condition = await Condition.FirstAsync("WHERE Oid = @0", ConditionOid).ConfigureAwait(false);
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "COLLECTOROID": oReturn = this.CollectorOid; break;
                    case "COLLECTIBLEVARIANTOID": oReturn = this.CollectibleVariantOid; break;
                    case "CERTOID": oReturn = this.CertOid; break;
                    case "CONDITIONOID": oReturn = this.ConditionOid; break;
                    case "ACQUIREDON": oReturn = this.AcquiredOn; break;
                    case "PURCHASEPRICE": oReturn = this.PurchasePrice; break;
                    case "NOTES": oReturn = this.Notes; break;
                    case "CREATEDON": oReturn = this.CreatedOn; break;
                    case "MODIFIEDON": oReturn = this.ModifiedOn; break;
                    case "USERAUTHOID_CREATEDBY": oReturn = this.UserAuthOid_CreatedBy; break;
                    case "USERAUTHOID_MODIFIEDBY": oReturn = this.UserAuthOid_ModifiedBy; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "COLLECTOROID": this.CollectorOid = (long)value; break;
                    case "COLLECTIBLEVARIANTOID": this.CollectibleVariantOid = (long?)value; break;
                    case "CERTOID": this.CertOid = (long?)value; break;
                    case "CONDITIONOID": this.ConditionOid = (long?)value; break;
                    case "ACQUIREDON": this.AcquiredOn = (DateTime?)value; break;
                    case "PURCHASEPRICE": this.PurchasePrice = (decimal?)value; break;
                    case "NOTES": this.Notes = (string)value; break;
                    case "CREATEDON": this.CreatedOn = (DateTime)value; break;
                    case "MODIFIEDON": this.ModifiedOn = (DateTime?)value; break;
                    case "USERAUTHOID_CREATEDBY": this.UserAuthOid_CreatedBy = (long?)value; break;
                    case "USERAUTHOID_MODIFIEDBY": this.UserAuthOid_ModifiedBy = (long?)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("CollectorOid", typeof(long));
            oReturn.Columns.Add("CollectibleVariantOid", typeof(long?));
            oReturn.Columns.Add("CertOid", typeof(long?));
            oReturn.Columns.Add("ConditionOid", typeof(long?));
            oReturn.Columns.Add("AcquiredOn", typeof(DateTime?));
            oReturn.Columns.Add("PurchasePrice", typeof(decimal?));
            oReturn.Columns.Add("Notes", typeof(string));
            oReturn.Columns.Add("CreatedOn", typeof(DateTime));
            oReturn.Columns.Add("ModifiedOn", typeof(DateTime?));
            oReturn.Columns.Add("UserAuthOid_CreatedBy", typeof(long?));
            oReturn.Columns.Add("UserAuthOid_ModifiedBy", typeof(long?));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, CollectorOid, CollectibleVariantOid, CertOid, ConditionOid, AcquiredOn, PurchasePrice, Notes, CreatedOn, ModifiedOn, UserAuthOid_CreatedBy, UserAuthOid_ModifiedBy);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectionItem().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new CollectionItem().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (CollectionItem)

    //***************************************************
    //*  Class: Collector
    //***************************************************
    #region Collector
    public partial class Collector : Record<Collector>
    {

        #region Fields
        private long _oid;
        private long _tenantOid;
        private string _displayName;
        private bool _isActive;
        private DateTime _createdOn;
        private DateTime? _modifiedOn;
        private long? _userAuthOid_CreatedBy;
        private long? _userAuthOid_ModifiedBy;
        private ManagedList<CollectionItem>? _collectionItems = null;
        #endregion (Fields)

        #region Constructor
        public Collector() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "Collector";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("TenantOid")]
        public long TenantOid { get { return _tenantOid; } set { if (_tenantOid != value) { _tenantOid = value; IsChanged = true; } } }

        [Column("DisplayName")]
        public string DisplayName { get { return _displayName; } set { if (_displayName != value) { _displayName = value; IsChanged = true; } } }

        [Column("IsActive")]
        public bool IsActive { get { return _isActive; } set { if (_isActive != value) { _isActive = value; IsChanged = true; } } }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get { return _createdOn; } set { if (_createdOn != value) { _createdOn = value; IsChanged = true; } } }

        [Column("ModifiedOn")]
        public DateTime? ModifiedOn { get { return _modifiedOn; } set { if (_modifiedOn != value) { _modifiedOn = value; IsChanged = true; } } }

        [Column("UserAuthOid_CreatedBy")]
        public long? UserAuthOid_CreatedBy { get { return _userAuthOid_CreatedBy; } set { if (_userAuthOid_CreatedBy != value) { _userAuthOid_CreatedBy = value; IsChanged = true; } } }

        [Column("UserAuthOid_ModifiedBy")]
        public long? UserAuthOid_ModifiedBy { get { return _userAuthOid_ModifiedBy; } set { if (_userAuthOid_ModifiedBy != value) { _userAuthOid_ModifiedBy = value; IsChanged = true; } } }

        [Ignore]
        public ManagedList<CollectionItem>? CollectionItems {
            get { if (_collectionItems == null) { LoadCollectionItems(); } return _collectionItems; }
            set {
                if (_collectionItems != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectionItems", "FK_CollectionItem_Collector");
                    }
                    _collectionItems = value;
                    IsChanged = true;
                }
            }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCollectionItems() {
            _collectionItems = CollectionItem.Fetch("WHERE CollectorOid = @0", Oid);
            _collectionItems.BindToParent(this, "CollectionItems", "FK_CollectionItem_Collector");
        }

        public async Task LoadCollectionItemsAsync() {
            _collectionItems = await CollectionItem.FetchAsync("WHERE CollectorOid = @0", Oid).ConfigureAwait(false);
            _collectionItems.BindToParent(this, "CollectionItems", "FK_CollectionItem_Collector");
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "TENANTOID": oReturn = this.TenantOid; break;
                    case "DISPLAYNAME": oReturn = this.DisplayName; break;
                    case "ISACTIVE": oReturn = this.IsActive; break;
                    case "CREATEDON": oReturn = this.CreatedOn; break;
                    case "MODIFIEDON": oReturn = this.ModifiedOn; break;
                    case "USERAUTHOID_CREATEDBY": oReturn = this.UserAuthOid_CreatedBy; break;
                    case "USERAUTHOID_MODIFIEDBY": oReturn = this.UserAuthOid_ModifiedBy; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "TENANTOID": this.TenantOid = (long)value; break;
                    case "DISPLAYNAME": this.DisplayName = (string)value; break;
                    case "ISACTIVE": this.IsActive = (bool)value; break;
                    case "CREATEDON": this.CreatedOn = (DateTime)value; break;
                    case "MODIFIEDON": this.ModifiedOn = (DateTime?)value; break;
                    case "USERAUTHOID_CREATEDBY": this.UserAuthOid_CreatedBy = (long?)value; break;
                    case "USERAUTHOID_MODIFIEDBY": this.UserAuthOid_ModifiedBy = (long?)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("TenantOid", typeof(long));
            oReturn.Columns.Add("DisplayName", typeof(string));
            oReturn.Columns.Add("IsActive", typeof(bool));
            oReturn.Columns.Add("CreatedOn", typeof(DateTime));
            oReturn.Columns.Add("ModifiedOn", typeof(DateTime?));
            oReturn.Columns.Add("UserAuthOid_CreatedBy", typeof(long?));
            oReturn.Columns.Add("UserAuthOid_ModifiedBy", typeof(long?));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, TenantOid, DisplayName, IsActive, CreatedOn, ModifiedOn, UserAuthOid_CreatedBy, UserAuthOid_ModifiedBy);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Collector().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Collector().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
            foreach (CollectionItem oChild in CollectionItems.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
            foreach (CollectionItem oChild in CollectionItems.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (Collector)

    //***************************************************
    //*  Class: Condition
    //***************************************************
    #region Condition
    public partial class Condition : Record<Condition>
    {

        #region Fields
        private long _oid;
        private string _name;
        private string _abbreviation;
        private int _sortOrder;
        private bool _isActive;
        private ManagedList<CollectiblePrice>? _collectiblePrices = null;
        private ManagedList<CollectionItem>? _collectionItems = null;
        #endregion (Fields)

        #region Constructor
        public Condition() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "Condition";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("Name")]
        public string Name { get { return _name; } set { if (_name != value) { _name = value; IsChanged = true; } } }

        [Column("Abbreviation")]
        public string Abbreviation { get { return _abbreviation; } set { if (_abbreviation != value) { _abbreviation = value; IsChanged = true; } } }

        [Column("SortOrder")]
        public int SortOrder { get { return _sortOrder; } set { if (_sortOrder != value) { _sortOrder = value; IsChanged = true; } } }

        [Column("IsActive")]
        public bool IsActive { get { return _isActive; } set { if (_isActive != value) { _isActive = value; IsChanged = true; } } }

        [Ignore]
        public ManagedList<CollectiblePrice>? CollectiblePrices {
            get { if (_collectiblePrices == null) { LoadCollectiblePrices(); } return _collectiblePrices; }
            set {
                if (_collectiblePrices != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectiblePrices", "FK_CollectiblePrice_Condition");
                    }
                    _collectiblePrices = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CollectionItem>? CollectionItems {
            get { if (_collectionItems == null) { LoadCollectionItems(); } return _collectionItems; }
            set {
                if (_collectionItems != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectionItems", "FK_CollectionItem_Condition");
                    }
                    _collectionItems = value;
                    IsChanged = true;
                }
            }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCollectiblePrices() {
            _collectiblePrices = CollectiblePrice.Fetch("WHERE ConditionOid = @0", Oid);
            _collectiblePrices.BindToParent(this, "CollectiblePrices", "FK_CollectiblePrice_Condition");
        }

        public async Task LoadCollectiblePricesAsync() {
            _collectiblePrices = await CollectiblePrice.FetchAsync("WHERE ConditionOid = @0", Oid).ConfigureAwait(false);
            _collectiblePrices.BindToParent(this, "CollectiblePrices", "FK_CollectiblePrice_Condition");
        }

        private void LoadCollectionItems() {
            _collectionItems = CollectionItem.Fetch("WHERE ConditionOid = @0", Oid);
            _collectionItems.BindToParent(this, "CollectionItems", "FK_CollectionItem_Condition");
        }

        public async Task LoadCollectionItemsAsync() {
            _collectionItems = await CollectionItem.FetchAsync("WHERE ConditionOid = @0", Oid).ConfigureAwait(false);
            _collectionItems.BindToParent(this, "CollectionItems", "FK_CollectionItem_Condition");
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "NAME": oReturn = this.Name; break;
                    case "ABBREVIATION": oReturn = this.Abbreviation; break;
                    case "SORTORDER": oReturn = this.SortOrder; break;
                    case "ISACTIVE": oReturn = this.IsActive; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "NAME": this.Name = (string)value; break;
                    case "ABBREVIATION": this.Abbreviation = (string)value; break;
                    case "SORTORDER": this.SortOrder = (int)value; break;
                    case "ISACTIVE": this.IsActive = (bool)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("Name", typeof(string));
            oReturn.Columns.Add("Abbreviation", typeof(string));
            oReturn.Columns.Add("SortOrder", typeof(int));
            oReturn.Columns.Add("IsActive", typeof(bool));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, Name, Abbreviation, SortOrder, IsActive);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Condition().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Condition().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
            foreach (CollectiblePrice oChild in CollectiblePrices.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectionItem oChild in CollectionItems.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
            foreach (CollectiblePrice oChild in CollectiblePrices.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectionItem oChild in CollectionItems.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (Condition)

    //***************************************************
    //*  Class: DataSource
    //***************************************************
    #region DataSource
    public partial class DataSource : Record<DataSource>
    {

        #region Fields
        private long _oid;
        private string _name;
        private string _url;
        private bool _isActive;
        private DateTime _createdOn;
        private DateTime? _modifiedOn;
        private ManagedList<CardSetExternalId>? _cardSetExternalIds = null;
        private ManagedList<CollectibleExternalId>? _collectibleExternalIds = null;
        private ManagedList<CollectibleImage>? _collectibleImages = null;
        private ManagedList<CollectiblePrice>? _collectiblePrices = null;
        private ManagedList<CollectibleVariantExternalId>? _collectibleVariantExternalIds = null;
        #endregion (Fields)

        #region Constructor
        public DataSource() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "DataSource";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("Name")]
        public string Name { get { return _name; } set { if (_name != value) { _name = value; IsChanged = true; } } }

        [Column("Url")]
        public string Url { get { return _url; } set { if (_url != value) { _url = value; IsChanged = true; } } }

        [Column("IsActive")]
        public bool IsActive { get { return _isActive; } set { if (_isActive != value) { _isActive = value; IsChanged = true; } } }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get { return _createdOn; } set { if (_createdOn != value) { _createdOn = value; IsChanged = true; } } }

        [Column("ModifiedOn")]
        public DateTime? ModifiedOn { get { return _modifiedOn; } set { if (_modifiedOn != value) { _modifiedOn = value; IsChanged = true; } } }

        [Ignore]
        public ManagedList<CardSetExternalId>? CardSetExternalIds {
            get { if (_cardSetExternalIds == null) { LoadCardSetExternalIds(); } return _cardSetExternalIds; }
            set {
                if (_cardSetExternalIds != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CardSetExternalIds", "FK_CardSetExternalId_DataSource");
                    }
                    _cardSetExternalIds = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CollectibleExternalId>? CollectibleExternalIds {
            get { if (_collectibleExternalIds == null) { LoadCollectibleExternalIds(); } return _collectibleExternalIds; }
            set {
                if (_collectibleExternalIds != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectibleExternalIds", "FK_CollectibleExternalId_DataSource");
                    }
                    _collectibleExternalIds = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CollectibleImage>? CollectibleImages {
            get { if (_collectibleImages == null) { LoadCollectibleImages(); } return _collectibleImages; }
            set {
                if (_collectibleImages != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectibleImages", "FK_CollectibleImage_DataSource");
                    }
                    _collectibleImages = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CollectiblePrice>? CollectiblePrices {
            get { if (_collectiblePrices == null) { LoadCollectiblePrices(); } return _collectiblePrices; }
            set {
                if (_collectiblePrices != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectiblePrices", "FK_CollectiblePrice_DataSource");
                    }
                    _collectiblePrices = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CollectibleVariantExternalId>? CollectibleVariantExternalIds {
            get { if (_collectibleVariantExternalIds == null) { LoadCollectibleVariantExternalIds(); } return _collectibleVariantExternalIds; }
            set {
                if (_collectibleVariantExternalIds != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectibleVariantExternalIds", "FK_CollectibleVariantExternalId_DataSource");
                    }
                    _collectibleVariantExternalIds = value;
                    IsChanged = true;
                }
            }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCardSetExternalIds() {
            _cardSetExternalIds = CardSetExternalId.Fetch("WHERE DataSourceOid = @0", Oid);
            _cardSetExternalIds.BindToParent(this, "CardSetExternalIds", "FK_CardSetExternalId_DataSource");
        }

        public async Task LoadCardSetExternalIdsAsync() {
            _cardSetExternalIds = await CardSetExternalId.FetchAsync("WHERE DataSourceOid = @0", Oid).ConfigureAwait(false);
            _cardSetExternalIds.BindToParent(this, "CardSetExternalIds", "FK_CardSetExternalId_DataSource");
        }

        private void LoadCollectibleExternalIds() {
            _collectibleExternalIds = CollectibleExternalId.Fetch("WHERE DataSourceOid = @0", Oid);
            _collectibleExternalIds.BindToParent(this, "CollectibleExternalIds", "FK_CollectibleExternalId_DataSource");
        }

        public async Task LoadCollectibleExternalIdsAsync() {
            _collectibleExternalIds = await CollectibleExternalId.FetchAsync("WHERE DataSourceOid = @0", Oid).ConfigureAwait(false);
            _collectibleExternalIds.BindToParent(this, "CollectibleExternalIds", "FK_CollectibleExternalId_DataSource");
        }

        private void LoadCollectibleImages() {
            _collectibleImages = CollectibleImage.Fetch("WHERE DataSourceOid = @0", Oid);
            _collectibleImages.BindToParent(this, "CollectibleImages", "FK_CollectibleImage_DataSource");
        }

        public async Task LoadCollectibleImagesAsync() {
            _collectibleImages = await CollectibleImage.FetchAsync("WHERE DataSourceOid = @0", Oid).ConfigureAwait(false);
            _collectibleImages.BindToParent(this, "CollectibleImages", "FK_CollectibleImage_DataSource");
        }

        private void LoadCollectiblePrices() {
            _collectiblePrices = CollectiblePrice.Fetch("WHERE DataSourceOid = @0", Oid);
            _collectiblePrices.BindToParent(this, "CollectiblePrices", "FK_CollectiblePrice_DataSource");
        }

        public async Task LoadCollectiblePricesAsync() {
            _collectiblePrices = await CollectiblePrice.FetchAsync("WHERE DataSourceOid = @0", Oid).ConfigureAwait(false);
            _collectiblePrices.BindToParent(this, "CollectiblePrices", "FK_CollectiblePrice_DataSource");
        }

        private void LoadCollectibleVariantExternalIds() {
            _collectibleVariantExternalIds = CollectibleVariantExternalId.Fetch("WHERE DataSourceOid = @0", Oid);
            _collectibleVariantExternalIds.BindToParent(this, "CollectibleVariantExternalIds", "FK_CollectibleVariantExternalId_DataSource");
        }

        public async Task LoadCollectibleVariantExternalIdsAsync() {
            _collectibleVariantExternalIds = await CollectibleVariantExternalId.FetchAsync("WHERE DataSourceOid = @0", Oid).ConfigureAwait(false);
            _collectibleVariantExternalIds.BindToParent(this, "CollectibleVariantExternalIds", "FK_CollectibleVariantExternalId_DataSource");
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "NAME": oReturn = this.Name; break;
                    case "URL": oReturn = this.Url; break;
                    case "ISACTIVE": oReturn = this.IsActive; break;
                    case "CREATEDON": oReturn = this.CreatedOn; break;
                    case "MODIFIEDON": oReturn = this.ModifiedOn; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "NAME": this.Name = (string)value; break;
                    case "URL": this.Url = (string)value; break;
                    case "ISACTIVE": this.IsActive = (bool)value; break;
                    case "CREATEDON": this.CreatedOn = (DateTime)value; break;
                    case "MODIFIEDON": this.ModifiedOn = (DateTime?)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("Name", typeof(string));
            oReturn.Columns.Add("Url", typeof(string));
            oReturn.Columns.Add("IsActive", typeof(bool));
            oReturn.Columns.Add("CreatedOn", typeof(DateTime));
            oReturn.Columns.Add("ModifiedOn", typeof(DateTime?));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, Name, Url, IsActive, CreatedOn, ModifiedOn);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new DataSource().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new DataSource().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
            foreach (CardSetExternalId oChild in CardSetExternalIds.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectibleExternalId oChild in CollectibleExternalIds.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectibleImage oChild in CollectibleImages.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectiblePrice oChild in CollectiblePrices.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectiblePrice oChild in CollectiblePrices.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectibleVariantExternalId oChild in CollectibleVariantExternalIds.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
            foreach (CardSetExternalId oChild in CardSetExternalIds.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectibleExternalId oChild in CollectibleExternalIds.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectibleImage oChild in CollectibleImages.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectiblePrice oChild in CollectiblePrices.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectiblePrice oChild in CollectiblePrices.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectibleVariantExternalId oChild in CollectibleVariantExternalIds.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (DataSource)

    //***************************************************
    //*  Class: Game
    //***************************************************
    #region Game
    public partial class Game : Record<Game>
    {

        #region Fields
        private long _oid;
        private long _collectibleTypeOid;
        private string _name;
        private bool _isActive;
        private CollectibleType? _collectibleType = null;
        private ManagedList<CardSet>? _cardSets = null;
        #endregion (Fields)

        #region Constructor
        public Game() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "Game";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("CollectibleTypeOid")]
        public long CollectibleTypeOid { get { return _collectibleTypeOid; } set { if (_collectibleTypeOid != value) { _collectibleTypeOid = value; IsChanged = true; } } }

        [Column("Name")]
        public string Name { get { return _name; } set { if (_name != value) { _name = value; IsChanged = true; } } }

        [Column("IsActive")]
        public bool IsActive { get { return _isActive; } set { if (_isActive != value) { _isActive = value; IsChanged = true; } } }

        [Ignore]
        public CollectibleType? CollectibleType {
            get { if (_collectibleType == null) { LoadCollectibleType(); } return _collectibleType; }
            set { _collectibleType = value; IsChanged = true; }
        }

        [Ignore]
        public ManagedList<CardSet>? CardSets {
            get { if (_cardSets == null) { LoadCardSets(); } return _cardSets; }
            set {
                if (_cardSets != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CardSets", "FK_CardSet_Game");
                    }
                    _cardSets = value;
                    IsChanged = true;
                }
            }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCollectibleType() {
            if (CollectibleTypeOid != null)
                _collectibleType = CollectibleType.First("WHERE Oid = @0", CollectibleTypeOid);
        }

        public async Task LoadCollectibleTypeAsync() {
            if (CollectibleTypeOid != null)
                _collectibleType = await CollectibleType.FirstAsync("WHERE Oid = @0", CollectibleTypeOid).ConfigureAwait(false);
        }

        private void LoadCardSets() {
            _cardSets = CardSet.Fetch("WHERE GameOid = @0", Oid);
            _cardSets.BindToParent(this, "CardSets", "FK_CardSet_Game");
        }

        public async Task LoadCardSetsAsync() {
            _cardSets = await CardSet.FetchAsync("WHERE GameOid = @0", Oid).ConfigureAwait(false);
            _cardSets.BindToParent(this, "CardSets", "FK_CardSet_Game");
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "COLLECTIBLETYPEOID": oReturn = this.CollectibleTypeOid; break;
                    case "NAME": oReturn = this.Name; break;
                    case "ISACTIVE": oReturn = this.IsActive; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "COLLECTIBLETYPEOID": this.CollectibleTypeOid = (long)value; break;
                    case "NAME": this.Name = (string)value; break;
                    case "ISACTIVE": this.IsActive = (bool)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("CollectibleTypeOid", typeof(long));
            oReturn.Columns.Add("Name", typeof(string));
            oReturn.Columns.Add("IsActive", typeof(bool));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, CollectibleTypeOid, Name, IsActive);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Game().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Game().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
            foreach (CardSet oChild in CardSets.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
            foreach (CardSet oChild in CardSets.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (Game)

    //***************************************************
    //*  Class: Grade
    //***************************************************
    #region Grade
    public partial class Grade : Record<Grade>
    {

        #region Fields
        private long _oid;
        private long _gradingCompanyOid;
        private string _code;
        private string _name;
        private decimal? _gradeValue;
        private int _sortOrder;
        private bool _isActive;
        private ManagedList<Cert>? _certs = null;
        private ManagedList<CollectiblePrice>? _collectiblePrices = null;
        private GradingCompany? _gradingCompany = null;
        #endregion (Fields)

        #region Constructor
        public Grade() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "Grade";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("GradingCompanyOid")]
        public long GradingCompanyOid { get { return _gradingCompanyOid; } set { if (_gradingCompanyOid != value) { _gradingCompanyOid = value; IsChanged = true; } } }

        [Column("Code")]
        public string Code { get { return _code; } set { if (_code != value) { _code = value; IsChanged = true; } } }

        [Column("Name")]
        public string Name { get { return _name; } set { if (_name != value) { _name = value; IsChanged = true; } } }

        [Column("GradeValue")]
        public decimal? GradeValue { get { return _gradeValue; } set { if (_gradeValue != value) { _gradeValue = value; IsChanged = true; } } }

        [Column("SortOrder")]
        public int SortOrder { get { return _sortOrder; } set { if (_sortOrder != value) { _sortOrder = value; IsChanged = true; } } }

        [Column("IsActive")]
        public bool IsActive { get { return _isActive; } set { if (_isActive != value) { _isActive = value; IsChanged = true; } } }

        [Ignore]
        public ManagedList<Cert>? Certs {
            get { if (_certs == null) { LoadCerts(); } return _certs; }
            set {
                if (_certs != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "Certs", "FK_Cert_Grade");
                    }
                    _certs = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CollectiblePrice>? CollectiblePrices {
            get { if (_collectiblePrices == null) { LoadCollectiblePrices(); } return _collectiblePrices; }
            set {
                if (_collectiblePrices != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectiblePrices", "FK_CollectiblePrice_Grade");
                    }
                    _collectiblePrices = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public GradingCompany? GradingCompany {
            get { if (_gradingCompany == null) { LoadGradingCompany(); } return _gradingCompany; }
            set { _gradingCompany = value; IsChanged = true; }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCerts() {
            _certs = Cert.Fetch("WHERE GradeOid = @0", Oid);
            _certs.BindToParent(this, "Certs", "FK_Cert_Grade");
        }

        public async Task LoadCertsAsync() {
            _certs = await Cert.FetchAsync("WHERE GradeOid = @0", Oid).ConfigureAwait(false);
            _certs.BindToParent(this, "Certs", "FK_Cert_Grade");
        }

        private void LoadCollectiblePrices() {
            _collectiblePrices = CollectiblePrice.Fetch("WHERE GradeOid = @0", Oid);
            _collectiblePrices.BindToParent(this, "CollectiblePrices", "FK_CollectiblePrice_Grade");
        }

        public async Task LoadCollectiblePricesAsync() {
            _collectiblePrices = await CollectiblePrice.FetchAsync("WHERE GradeOid = @0", Oid).ConfigureAwait(false);
            _collectiblePrices.BindToParent(this, "CollectiblePrices", "FK_CollectiblePrice_Grade");
        }

        private void LoadGradingCompany() {
            if (GradingCompanyOid != null)
                _gradingCompany = GradingCompany.First("WHERE Oid = @0", GradingCompanyOid);
        }

        public async Task LoadGradingCompanyAsync() {
            if (GradingCompanyOid != null)
                _gradingCompany = await GradingCompany.FirstAsync("WHERE Oid = @0", GradingCompanyOid).ConfigureAwait(false);
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "GRADINGCOMPANYOID": oReturn = this.GradingCompanyOid; break;
                    case "CODE": oReturn = this.Code; break;
                    case "NAME": oReturn = this.Name; break;
                    case "GRADEVALUE": oReturn = this.GradeValue; break;
                    case "SORTORDER": oReturn = this.SortOrder; break;
                    case "ISACTIVE": oReturn = this.IsActive; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "GRADINGCOMPANYOID": this.GradingCompanyOid = (long)value; break;
                    case "CODE": this.Code = (string)value; break;
                    case "NAME": this.Name = (string)value; break;
                    case "GRADEVALUE": this.GradeValue = (decimal?)value; break;
                    case "SORTORDER": this.SortOrder = (int)value; break;
                    case "ISACTIVE": this.IsActive = (bool)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("GradingCompanyOid", typeof(long));
            oReturn.Columns.Add("Code", typeof(string));
            oReturn.Columns.Add("Name", typeof(string));
            oReturn.Columns.Add("GradeValue", typeof(decimal?));
            oReturn.Columns.Add("SortOrder", typeof(int));
            oReturn.Columns.Add("IsActive", typeof(bool));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, GradingCompanyOid, Code, Name, GradeValue, SortOrder, IsActive);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Grade().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Grade().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
            foreach (Cert oChild in Certs.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectiblePrice oChild in CollectiblePrices.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
            foreach (Cert oChild in Certs.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectiblePrice oChild in CollectiblePrices.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (Grade)

    //***************************************************
    //*  Class: GradingCompany
    //***************************************************
    #region GradingCompany
    public partial class GradingCompany : Record<GradingCompany>
    {

        #region Fields
        private long _oid;
        private string _name;
        private string _abbreviation;
        private string _url;
        private bool _isActive;
        private ManagedList<Cert>? _certs = null;
        private ManagedList<Grade>? _grades = null;
        #endregion (Fields)

        #region Constructor
        public GradingCompany() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "GradingCompany";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("Name")]
        public string Name { get { return _name; } set { if (_name != value) { _name = value; IsChanged = true; } } }

        [Column("Abbreviation")]
        public string Abbreviation { get { return _abbreviation; } set { if (_abbreviation != value) { _abbreviation = value; IsChanged = true; } } }

        [Column("Url")]
        public string Url { get { return _url; } set { if (_url != value) { _url = value; IsChanged = true; } } }

        [Column("IsActive")]
        public bool IsActive { get { return _isActive; } set { if (_isActive != value) { _isActive = value; IsChanged = true; } } }

        [Ignore]
        public ManagedList<Cert>? Certs {
            get { if (_certs == null) { LoadCerts(); } return _certs; }
            set {
                if (_certs != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "Certs", "FK_Cert_GradingCompany");
                    }
                    _certs = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<Grade>? Grades {
            get { if (_grades == null) { LoadGrades(); } return _grades; }
            set {
                if (_grades != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "Grades", "FK_Grade_GradingCompany");
                    }
                    _grades = value;
                    IsChanged = true;
                }
            }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCerts() {
            _certs = Cert.Fetch("WHERE GradingCompanyOid = @0", Oid);
            _certs.BindToParent(this, "Certs", "FK_Cert_GradingCompany");
        }

        public async Task LoadCertsAsync() {
            _certs = await Cert.FetchAsync("WHERE GradingCompanyOid = @0", Oid).ConfigureAwait(false);
            _certs.BindToParent(this, "Certs", "FK_Cert_GradingCompany");
        }

        private void LoadGrades() {
            _grades = Grade.Fetch("WHERE GradingCompanyOid = @0", Oid);
            _grades.BindToParent(this, "Grades", "FK_Grade_GradingCompany");
        }

        public async Task LoadGradesAsync() {
            _grades = await Grade.FetchAsync("WHERE GradingCompanyOid = @0", Oid).ConfigureAwait(false);
            _grades.BindToParent(this, "Grades", "FK_Grade_GradingCompany");
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "NAME": oReturn = this.Name; break;
                    case "ABBREVIATION": oReturn = this.Abbreviation; break;
                    case "URL": oReturn = this.Url; break;
                    case "ISACTIVE": oReturn = this.IsActive; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "NAME": this.Name = (string)value; break;
                    case "ABBREVIATION": this.Abbreviation = (string)value; break;
                    case "URL": this.Url = (string)value; break;
                    case "ISACTIVE": this.IsActive = (bool)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("Name", typeof(string));
            oReturn.Columns.Add("Abbreviation", typeof(string));
            oReturn.Columns.Add("Url", typeof(string));
            oReturn.Columns.Add("IsActive", typeof(bool));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, Name, Abbreviation, Url, IsActive);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new GradingCompany().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new GradingCompany().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
            foreach (Cert oChild in Certs.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (Grade oChild in Grades.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
            foreach (Cert oChild in Certs.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (Grade oChild in Grades.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (GradingCompany)

    //***************************************************
    //*  Class: Language
    //***************************************************
    #region Language
    public partial class Language : Record<Language>
    {

        #region Fields
        private long _oid;
        private string _name;
        private string _code;
        private bool _isActive;
        private ManagedList<CardSet>? _cardSets = null;
        private ManagedList<CardSetName>? _cardSetNames = null;
        private ManagedList<CollectibleImage>? _collectibleImages = null;
        private ManagedList<CollectibleName>? _collectibleNames = null;
        private ManagedList<CollectibleVariant>? _collectibleVariants = null;
        #endregion (Fields)

        #region Constructor
        public Language() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "Language";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("Name")]
        public string Name { get { return _name; } set { if (_name != value) { _name = value; IsChanged = true; } } }

        [Column("Code")]
        public string Code { get { return _code; } set { if (_code != value) { _code = value; IsChanged = true; } } }

        [Column("IsActive")]
        public bool IsActive { get { return _isActive; } set { if (_isActive != value) { _isActive = value; IsChanged = true; } } }

        [Ignore]
        public ManagedList<CardSet>? CardSets {
            get { if (_cardSets == null) { LoadCardSets(); } return _cardSets; }
            set {
                if (_cardSets != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CardSets", "FK_CardSet_Language_Primary");
                    }
                    _cardSets = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CardSetName>? CardSetNames {
            get { if (_cardSetNames == null) { LoadCardSetNames(); } return _cardSetNames; }
            set {
                if (_cardSetNames != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CardSetNames", "FK_CardSetName_Language");
                    }
                    _cardSetNames = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CollectibleImage>? CollectibleImages {
            get { if (_collectibleImages == null) { LoadCollectibleImages(); } return _collectibleImages; }
            set {
                if (_collectibleImages != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectibleImages", "FK_CollectibleImage_Language");
                    }
                    _collectibleImages = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CollectibleName>? CollectibleNames {
            get { if (_collectibleNames == null) { LoadCollectibleNames(); } return _collectibleNames; }
            set {
                if (_collectibleNames != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectibleNames", "FK_CollectibleName_Language");
                    }
                    _collectibleNames = value;
                    IsChanged = true;
                }
            }
        }

        [Ignore]
        public ManagedList<CollectibleVariant>? CollectibleVariants {
            get { if (_collectibleVariants == null) { LoadCollectibleVariants(); } return _collectibleVariants; }
            set {
                if (_collectibleVariants != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectibleVariants", "FK_CollectibleVariant_Language");
                    }
                    _collectibleVariants = value;
                    IsChanged = true;
                }
            }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCardSets() {
            _cardSets = CardSet.Fetch("WHERE LanguageOid_Primary = @0", Oid);
            _cardSets.BindToParent(this, "CardSets", "FK_CardSet_Language_Primary");
        }

        public async Task LoadCardSetsAsync() {
            _cardSets = await CardSet.FetchAsync("WHERE LanguageOid_Primary = @0", Oid).ConfigureAwait(false);
            _cardSets.BindToParent(this, "CardSets", "FK_CardSet_Language_Primary");
        }

        private void LoadCardSetNames() {
            _cardSetNames = CardSetName.Fetch("WHERE LanguageOid = @0", Oid);
            _cardSetNames.BindToParent(this, "CardSetNames", "FK_CardSetName_Language");
        }

        public async Task LoadCardSetNamesAsync() {
            _cardSetNames = await CardSetName.FetchAsync("WHERE LanguageOid = @0", Oid).ConfigureAwait(false);
            _cardSetNames.BindToParent(this, "CardSetNames", "FK_CardSetName_Language");
        }

        private void LoadCollectibleImages() {
            _collectibleImages = CollectibleImage.Fetch("WHERE LanguageOid = @0", Oid);
            _collectibleImages.BindToParent(this, "CollectibleImages", "FK_CollectibleImage_Language");
        }

        public async Task LoadCollectibleImagesAsync() {
            _collectibleImages = await CollectibleImage.FetchAsync("WHERE LanguageOid = @0", Oid).ConfigureAwait(false);
            _collectibleImages.BindToParent(this, "CollectibleImages", "FK_CollectibleImage_Language");
        }

        private void LoadCollectibleNames() {
            _collectibleNames = CollectibleName.Fetch("WHERE LanguageOid = @0", Oid);
            _collectibleNames.BindToParent(this, "CollectibleNames", "FK_CollectibleName_Language");
        }

        public async Task LoadCollectibleNamesAsync() {
            _collectibleNames = await CollectibleName.FetchAsync("WHERE LanguageOid = @0", Oid).ConfigureAwait(false);
            _collectibleNames.BindToParent(this, "CollectibleNames", "FK_CollectibleName_Language");
        }

        private void LoadCollectibleVariants() {
            _collectibleVariants = CollectibleVariant.Fetch("WHERE LanguageOid = @0", Oid);
            _collectibleVariants.BindToParent(this, "CollectibleVariants", "FK_CollectibleVariant_Language");
        }

        public async Task LoadCollectibleVariantsAsync() {
            _collectibleVariants = await CollectibleVariant.FetchAsync("WHERE LanguageOid = @0", Oid).ConfigureAwait(false);
            _collectibleVariants.BindToParent(this, "CollectibleVariants", "FK_CollectibleVariant_Language");
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "NAME": oReturn = this.Name; break;
                    case "CODE": oReturn = this.Code; break;
                    case "ISACTIVE": oReturn = this.IsActive; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "NAME": this.Name = (string)value; break;
                    case "CODE": this.Code = (string)value; break;
                    case "ISACTIVE": this.IsActive = (bool)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("Name", typeof(string));
            oReturn.Columns.Add("Code", typeof(string));
            oReturn.Columns.Add("IsActive", typeof(bool));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, Name, Code, IsActive);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Language().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Language().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
            foreach (CardSet oChild in CardSets.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CardSetName oChild in CardSetNames.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectibleImage oChild in CollectibleImages.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectibleName oChild in CollectibleNames.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
            foreach (CollectibleVariant oChild in CollectibleVariants.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
            foreach (CardSet oChild in CardSets.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CardSetName oChild in CardSetNames.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectibleImage oChild in CollectibleImages.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectibleName oChild in CollectibleNames.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
            foreach (CollectibleVariant oChild in CollectibleVariants.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (Language)

    //***************************************************
    //*  Class: Printing
    //***************************************************
    #region Printing
    public partial class Printing : Record<Printing>
    {

        #region Fields
        private long _oid;
        private string _name;
        private int _sortOrder;
        private bool _isActive;
        private ManagedList<CollectibleVariant>? _collectibleVariants = null;
        #endregion (Fields)

        #region Constructor
        public Printing() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "Printing";
            PrimaryKey = "Oid";
            _cascadingSaveAction = GeneratedCascadingSave;
            _cascadingDeleteAction = GeneratedCascadingDelete;
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("Name")]
        public string Name { get { return _name; } set { if (_name != value) { _name = value; IsChanged = true; } } }

        [Column("SortOrder")]
        public int SortOrder { get { return _sortOrder; } set { if (_sortOrder != value) { _sortOrder = value; IsChanged = true; } } }

        [Column("IsActive")]
        public bool IsActive { get { return _isActive; } set { if (_isActive != value) { _isActive = value; IsChanged = true; } } }

        [Ignore]
        public ManagedList<CollectibleVariant>? CollectibleVariants {
            get { if (_collectibleVariants == null) { LoadCollectibleVariants(); } return _collectibleVariants; }
            set {
                if (_collectibleVariants != value) {
                    if (value != null && !value.IsBound) {
                        value.BindToParent(this, "CollectibleVariants", "FK_CollectibleVariant_Printing");
                    }
                    _collectibleVariants = value;
                    IsChanged = true;
                }
            }
        }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Relationship Load Methods
        private void LoadCollectibleVariants() {
            _collectibleVariants = CollectibleVariant.Fetch("WHERE PrintingOid = @0", Oid);
            _collectibleVariants.BindToParent(this, "CollectibleVariants", "FK_CollectibleVariant_Printing");
        }

        public async Task LoadCollectibleVariantsAsync() {
            _collectibleVariants = await CollectibleVariant.FetchAsync("WHERE PrintingOid = @0", Oid).ConfigureAwait(false);
            _collectibleVariants.BindToParent(this, "CollectibleVariants", "FK_CollectibleVariant_Printing");
        }

        #endregion (Relationship Load Methods)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "NAME": oReturn = this.Name; break;
                    case "SORTORDER": oReturn = this.SortOrder; break;
                    case "ISACTIVE": oReturn = this.IsActive; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "NAME": this.Name = (string)value; break;
                    case "SORTORDER": this.SortOrder = (int)value; break;
                    case "ISACTIVE": this.IsActive = (bool)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("Name", typeof(string));
            oReturn.Columns.Add("SortOrder", typeof(int));
            oReturn.Columns.Add("IsActive", typeof(bool));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, Name, SortOrder, IsActive);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Printing().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new Printing().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
            foreach (CollectibleVariant oChild in CollectibleVariants.ToList()) {
                oChild.GeneratedCascadingSaveInternal();
            }
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
            foreach (CollectibleVariant oChild in CollectibleVariants.ToList()) {
                await oChild.GeneratedCascadingSaveInternalAsync(toDb).ConfigureAwait(false);
            }
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (Printing)

    //***************************************************
    //*  Class: UserPreferences
    //***************************************************
    #region UserPreferences
    public partial class UserPreferences : Record<UserPreferences>
    {

        #region Fields
        private long _oid;
        private long _userAuthOid;
        private string _preferencesJSON;
        #endregion (Fields)

        #region Constructor
        public UserPreferences() : base() {
            DatabaseName = "CollectionMaster";
            ClassName = "UserPreferences";
            PrimaryKey = "Oid";
            PostConstructorInitialization();
        }
        #endregion (Constructor)

        #region Properties
        [AutoIncrement]
        [PrimaryKey("Oid")]
        [Column("Oid")]
        public long Oid { get { return _oid; } set { if (_oid != value) { _oid = value; IsChanged = true; } } }

        [Column("UserAuthOid")]
        public long UserAuthOid { get { return _userAuthOid; } set { if (_userAuthOid != value) { _userAuthOid = value; IsChanged = true; } } }

        [Column("PreferencesJSON")]
        public string PreferencesJSON { get { return _preferencesJSON; } set { if (_preferencesJSON != value) { _preferencesJSON = value; IsChanged = true; } } }

        // ── DTO Generation Support ────────────────────────────────────────
        /// <summary>Authoritative SQL column name from DbColumn.Name.
        /// Always use this for SQL emission — DbDto_Column.Name can go stale.</summary>
        [Ignore]
        public string? ResolvedColumnName { get; set; }

        /// <summary>SQL data type from DbColumn e.g. "bigint", "nvarchar".
        /// Populated by DtoGenerator.LoadContextAsync() via JOIN to DbColumn.</summary>
        [Ignore]
        public string? ResolvedDataType { get; set; }

        /// <summary>Whether the source DbColumn allows nulls.
        /// Used with ResolvedDataType to emit nullable types where appropriate.</summary>
        [Ignore]
        public bool ResolvedIsNullable { get; set; }

        /// <summary>C# enum type name e.g. "eDbType" when the column is stored
        /// as int in SQL but typed as an enum in C#. Empty for non-enum columns.</summary>
        [Ignore]
        public string? ResolvedEnumType { get; set; }

        #endregion (Properties)

        #region Indexer
        [Ignore]
        public override object? this[string tsPropertyName] {
            get {
                object? oReturn = null;
                switch (tsPropertyName.ToUpper()) {
                    case "OID": oReturn = this.Oid; break;
                    case "USERAUTHOID": oReturn = this.UserAuthOid; break;
                    case "PREFERENCESJSON": oReturn = this.PreferencesJSON; break;
                }
                return oReturn;
            }
            set {
                tsPropertyName = tsPropertyName.ToUpper();
                switch (tsPropertyName) {
                    case "OID": this.Oid = (long)value; break;
                    case "USERAUTHOID": this.UserAuthOid = (long)value; break;
                    case "PREFERENCESJSON": this.PreferencesJSON = (string)value; break;
                }
            }
        }
        #endregion (Indexer)

        #region Mass Update
        public override DataTable CreateDataTable() {
            DataTable oReturn = new DataTable();
            oReturn.Columns.Add("Oid", typeof(long));
            oReturn.Columns.Add("UserAuthOid", typeof(long));
            oReturn.Columns.Add("PreferencesJSON", typeof(string));
            return oReturn;
        }

        public override void LoadDataTable(DataTable toDataTable) {
            toDataTable.Rows.Add(Oid, UserAuthOid, PreferencesJSON);
        }
        #endregion (Mass Update)

        #region Cascading Save & Delete

        /// <summary>
        /// Async cascading save — opens one scope and one transaction for the
        /// entire graph. If any save fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingSaveAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new UserPreferences().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingSaveInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Async cascading delete — opens one scope and one transaction for the
        /// entire graph. If any delete fails the whole operation rolls back.
        /// </summary>
        public override async Task GeneratedCascadingDeleteAsync() {
            using var oScope = DbPoolAccessor.Instance!.BeginScope();
            var oDb = DbPoolAccessor.Instance.GetDatabaseByName(new UserPreferences().DatabaseName);
            await oDb.BeginTransactionAsync().ConfigureAwait(false);
            try {
                await GeneratedCascadingDeleteInternalAsync(oDb).ConfigureAwait(false);
                await oDb.CommitAndEndTransactionAsync().ConfigureAwait(false);
            } catch {
                await oDb.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public override void GeneratedCascadingSaveInternal() {
            this.Save();
        }

        public async Task GeneratedCascadingSaveInternalAsync(IDatabase toDb) {
            await toDb.SaveAsync(this).ConfigureAwait(false);
        }

        public override void GeneratedCascadingDeleteInternal() {
            this.Delete();
        }

        public async Task GeneratedCascadingDeleteInternalAsync(IDatabase toDb) {
            await toDb.DeleteAsync(this).ConfigureAwait(false);
        }

        #endregion (Cascading Save & Delete)

    }
    #endregion (UserPreferences)

}

namespace SunamoSqlServer.Layer;

/// <summary>
/// Must be named ColumnNames, not Columns, due to bad replacing in CopyInWeb
/// </summary>
    public class ColumnNames
    {
    public const string IDApp = "IDApp";
    public const  string Caches = "Caches";
        public const string SessionID = "SessionID";
        public const string Text = "Text";
        public const  string Description = "Description";
    public const string Changelog = "Changelog";
    public const string IDUser = "IDUser";
    public const string IDModule = "IDModule";
    public const string IDProject = "IDProject";
    public const string IDSolution = "IDSolution";
    public const string IDPageLyrSong = "IDPageLyrSong";
    public const string IDPage = "IDPage";
    public const string ID = "ID";
    public const string HtmlSummary = "HtmlSummary";
    public const string IDArtist = "IDArtist";
    public const string Czk = "Czk";
    public const  string IDTable = "IDTable";
    public const  string IDItem = "IDItem";
    public const  string Day = "Day";
    public const  string IsShowing = "IsShowing";
    public const string Tag = "Tag";
    public const string Uri = "Uri";
    public const string Lon = "Lon";
    public const string Lat = "Lat";
    public const string IDKaraoke = "IDKaraoke";
    /// <summary>
    /// Wont use it! In admin section cant be searched with it.
    /// Only really unique which I can use is orderNumber which is unique
    /// </summary>
    public static string PaymentSessionId = "PaymentSessionId";
    public static string OrderId = "OrderId";
    public static string ValidTo = "ValidTo";
    public static string DatePayed = "DatePayed";
    public static string ActualVersion = "ActualVersion";
    public static string SessionState = "SessionState";
    public static string Month = "Month";
    public static string Login = "Login";
    public static string IsFast = "IsFast";
    public static string Count = "Count";
    public static string IDIPAddress = "IDIPAddress";
    public static string LastUpdated = "LastUpdated";
    public static string IDOrder = "IDOrder";
    public static string CgId = "CgId";
    public static string DateTime = "DateTime";
    public static string Site = "Site";
    public static object AppCheckingRandomly = "AppCheckingRandomly";
    public static object Value;
    public static string Email = "Email";
    public static string ViewLastWeek = "ViewLastWeek";
    public static string Views = "Views";
    public static string IsTranslate = "IsTranslate";
    public static string IDSong = "IDSong";
    public const string ViewCount = "ViewCount";
    public const string OverallViews = "OverallViews";
    public const string IDWebs = "IDWebs";
    public const string IDUserAgent = "IDUserAgent";
    public const string Year = "Year";
    public const string Lang = "Lang";
    public const string ShortDescription = "ShortDescription";
    public const string Name = "Name";
    public const string DT = "DT";
}

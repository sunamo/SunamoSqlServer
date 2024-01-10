namespace SunamoSqlServer.MSSQL;

public partial class MSStoredProceduresI : MSStoredProceduresIBase
{
    public static bool forceIsVps = false;
    static MSStoredProceduresIBase _ci = new MSStoredProceduresIBase();
    public static MSStoredProceduresIBase ci
    {
        get
        {
            return _ci;
        }
        private set
        {
            _ci = value;
        }
    }


    public static string ConvertToVarChar(string item)
    {
        return SqlServerHelper.ConvertToVarChar(item);
    }
}

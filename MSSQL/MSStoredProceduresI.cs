namespace SunamoSqlServer.MSSQL;

/// <summary>
/// Třída musí být public, protože jinak se mi ji nepodaří zkompilovat
/// </summary>
public partial class MSStoredProceduresI : MSStoredProceduresIBase // : IStoredProceduresI<SqlConnection, SqlCommand>
{
    static Type type = typeof(MSStoredProceduresI);

    public static void SetVariable(SqlConnection ci, string databaseName)
    {
        throw new Exception( "Commented due to new approach - create new db conn with every request");
        //_ci.conn = ci;
        //MSDatabaseLayer._conn = ci;
        //_databaseName = databaseName;
    }

    static string _databaseName = null;
    /// <summary>
    /// Název databáze z výčtu Databases
    /// </summary>
    public static string databaseName
    {
        get
        {
            return _databaseName;
        }
    }

    

    
}

namespace SunamoSqlServer.SqlResults;

public partial class CastSqlResult
{
    public static SqlResult<object[]> FirstRowToArrayObject(SqlResult<DataTable> dt)
    {
        var result = new SqlResult<object[]>();
        result.result = dt.result.Rows[0].ItemArray;
        return result;
    }
}

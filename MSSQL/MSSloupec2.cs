
namespace SunamoSqlServer.MSSQL;
using SunamoBts;
using SunamoString;


public class MSSloupec2
{
    ///string table = "";


    /// <param name="table"></param>
    /// <param name="uniqueColumns"></param>
    /// <param name="primaryKey"></param>
    public static MSColumnsDB GetColumns(string table, List<string> uniqueColumns, List<string> primaryKey)
    {
        MSColumnsDB ms = new MSColumnsDB();
        DataTable dt = MSStoredProceduresI.ci.SelectDataTableSelective("INFORMATION_SCHEMA.COLUMNS", "COLUMN_NAME,IS_NULLABLE,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH", "TABLE_NAME", table, "ORDINAL_POSITION", System.Data.SqlClient.SortOrder.Ascending);
        foreach (DataRow item in dt.Rows)
        {
            object[] o = item.ItemArray;
            string COLUMN_NAME = MSTableRowParse.GetString(o, 0);
            bool is_nullable = BTS.StringToBool(SH.FirstCharUpper(MSTableRowParse.GetString(o, 1).ToLower()));
            string DATA_TYPE = MSTableRowParse.GetString(o, 2);
            string zav = "";
            if (table == "ASPStateTempSessions" && COLUMN_NAME == "SessionItemLong")
            {
                //zav = "(MAX)";
            }
            else
                if (!SqlServerHelper.IsNull(o[3]))
            {
                int CHARACTER_MAXIMUM_LENGTH = MSTableRowParse.GetInt(o, 3);
                if (CHARACTER_MAXIMUM_LENGTH != -1)
                {
                    zav = AllStrings.lb + CHARACTER_MAXIMUM_LENGTH + AllStrings.rb;
                }
                else
                {
                    zav = "(MAX)";
                }
            }

            MSSloupecDB s = null;
            if (primaryKey.Contains(COLUMN_NAME))
            {
                s = MSSloupecDB.CI((SqlDbType2)Enum.Parse(typeof(SqlDbType2), DATA_TYPE, true), COLUMN_NAME + zav, true);
            }
            else
            {
                s = MSSloupecDB.CI((SqlDbType2)Enum.Parse(typeof(SqlDbType2), DATA_TYPE, true), COLUMN_NAME + zav, is_nullable, uniqueColumns.Contains(COLUMN_NAME));
            }
            ms.Add(s);
        }
        return ms;
    }
}

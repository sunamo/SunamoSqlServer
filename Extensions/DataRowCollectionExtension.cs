namespace SunamoSqlServer.Extensions;
public static class DataRowCollectionExtension
{
    public static List<object[]> ToItemArrayList(this DataTable dt)
    {
        List<object[]> result = new List<object[]>(dt.Rows.Count);
        foreach (DataRow item in dt.Rows)
        {
            result.Add(item.ItemArray);
        }

        return result;
    }
}

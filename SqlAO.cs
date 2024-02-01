namespace SunamoSqlServer;
using SunamoUnderscore;
/// <summary>
/// Sql Array Object
/// </summary>
public class SqlAO
{
    public static object[] FillWithDefaultOrMaxValues(string tn, List<string> columns2, object[] o, Dictionary<string, MSColumnsDB> layer)
    {
        var dict = layer[tn].dict;

        var c = _.allColumns[tn];
        object[] result = new object[c.Count];

        List<int> l = new List<int>(columns2.Count);


        foreach (var item in columns2)
        {
            l.Add(c.IndexOf(item));
        }

        l.Sort();

        int dxOriginalArray = 0;

        for (int i = 0; i < c.Count; i++)
        {
            if (!l.Contains(i))
            {
                var cName = c[i];
                var column = dict[cName];
                SqlDbType2 type = column.Type;

                // teď potřebuji přendat SqlDbType2 na dotnet / rovnou získat jeho default value
                // nemůžu předat null protože v MSTableRowParse s null nepočítají. Nebudu si kazit výkon

                result[i] = GeneratorMsSql.MaxValueForSqlDbType2(type);
            }
            else
            {
                result[i] = o[dxOriginalArray++];
            }
        }

        return result;
    }
}

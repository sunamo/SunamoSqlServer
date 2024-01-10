namespace SunamoSqlServer.MSSQL;

public class MSTableRowIDNameShort : MSTableRowIDNameT<short>
{
    public MSTableRowIDNameShort(string tableName, string name) : base(tableName, name)
    {
    }

    public MSTableRowIDNameShort(string tableName, MSColumnsDB columns, string name) : base(tableName, columns, name)
    {
    }

    protected override short GetT(object[] o, int i)
    {
        return MSTableRowParse.GetShort(o, i);
    }

    protected override void ParseRow(object[] o)
    {
        ID = MSTableRowParse.GetShort(o, 0);
        Name = MSTableRowParse.GetString(o, 1);
    }
}

namespace SunamoSqlServer.MSSQL;

public class MSFactoryColumnDB : IFactoryColumnDB<MSSloupecDB, SqlDbType2>
{
    public static IFactoryColumnDB<MSSloupecDB, SqlDbType2> Instance = new MSFactoryColumnDB();

    private MSFactoryColumnDB() {
    }

    public MSSloupecDB CreateInstance(SqlDbType2 typ, string nazev, Signed signed, bool canBeNull, bool mustBeUnique, string referencesTable, string referencesColumn, bool primaryKey)
    {
        return CreateInstance(typ, new MSSloupecDBArgs { nazev = nazev, canBeNull = canBeNull, mustBeUnique = mustBeUnique, referencesTable = referencesTable, referencesColumn = referencesColumn, primaryKey = primaryKey, signed = signed });
    }

    public MSSloupecDB CreateInstance(SqlDbType2 typ, MSSloupecDBArgs a)
    {
        string nazev;
        bool canBeNull; bool mustBeUnique; string referencesTable; string referencesColumn; bool primaryKey;
        Signed signed = a.signed;

        nazev = a.nazev;
        canBeNull = a.canBeNull;
        mustBeUnique = a.mustBeUnique;
        referencesTable = a.referencesTable;
        referencesColumn = a.referencesColumn;
        primaryKey = a.primaryKey;

        MSSloupecDB column = new MSSloupecDB();
        column.typ = typ; //ConvertSqlDbType.ToSqlDbType(typ, out isNewId);
        
        if (column.Type == SqlDbType2.NChar || column.Type == SqlDbType2.NVarChar)
        {
            column.IsUnicode = true;
        }
        else if (column.Type == SqlDbType2.Char || column.Type == SqlDbType2.VarChar)
        {
            column.IsUnicode = false;
        }

        column.isNewId = false;
        column.Name = nazev;
        column._signed = signed;
        column.canBeNull = canBeNull;
        column.mustBeUnique = mustBeUnique;
        column.referencesTable = referencesTable;
        column.referencesColumn = referencesColumn;
        
        column.identityIncrementBy1 = a.identityIncrementBy1;
        if (a.identityIncrementBy1)
        {
            column.primaryKey = true;
        }
        else
        {
            column.primaryKey = primaryKey;
        }

        return column;
    }

}

namespace SunamoSqlServer.MSSQL;

public class MSSloupecDB : SloupecDBBase< MSSloupecDB, SqlDbType2>
{
    public MSSloupecDB() : base()
    {
        databaseLayer = MSDatabaseLayer.ci;
    }

    static MSSloupecDB()
    {
        SloupecDBBase< MSSloupecDB, SqlDbType2>.databaseLayer = MSDatabaseLayer.ci;
        // could set up here also factory column DB
    }

}

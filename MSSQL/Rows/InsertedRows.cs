namespace SunamoSqlServer.MSSQL.Rows;

    public class InsertedRows
    {
        public int quantity = 0;
        public DataTable insertedRows = null;
        public string error = null;

        public InsertedRows()
        {

        }

        public InsertedRows(string Chyba)
        {
            this.error = Chyba;
        }
    }

namespace SunamoSqlServer.MSSQL;

public partial class MSTSP // : IStoredProceduresI<SqlConnection, SqlCommand>
{
static Type type = typeof(MSTSP);
    public object[] SelectDataTableOneRow(SqlTransaction tran, string TableName, string nazevSloupce, object hodnotaSloupce)
    {
        return SelectOneRow(tran, TableName, nazevSloupce, hodnotaSloupce);
    }
    // Duplikátní metoda
    /// <summary>
    /// G null pokud nenalezne
    /// </summary>
    /// <param name="table"></param>
    /// <param name="sloupecID"></param>
    /// <param name="id"></param>
    /// <param name="hledanySloupec"></param>
    public string SelectCellDataTableString(SqlTransaction tran, string table, string sloupecID, object id, string hledanySloupec)
    {
        DataTable dt = SelectDataTableSelective(tran, table, sloupecID, id, hledanySloupec);
        return GetCellDataTableString(dt, 0, 0);
    }
    /// <summary>
    /// Tato hodnota byla založena aby používal všude v DB konzistentní datovou hodnotu, klidně může mít i hodnotu DT.MaxValue když to tak má být
    /// </summary>
    public static readonly DateTime DateTimeMinVal = new DateTime(1900, 1, 1);
    public static readonly DateTime DateTimeMaxVal = new DateTime(2079, 6, 6);
    public static MSTSP ci = new MSTSP();
    public object ExecuteScalar(SqlTransaction tran, SqlCommand comm)
    {
        //SqlDbType.SmallDateTime;
        comm.Connection = conn;
        comm.Transaction = tran;
        return comm.ExecuteScalar();
    }
    public int ExecuteNonQuery(SqlTransaction tran, string commText, params object[] para)
    {
        SqlCommand comm = new SqlCommand(commText);
        comm.Connection = conn;
        comm.Transaction = tran;
        for (int i = 0; i < para.Length; i++)
        {
            AddCommandParameter(comm, i, para[i]);
        }
        return comm.ExecuteNonQuery();
    }
    public int ExecuteNonQuery(SqlTransaction tran, SqlCommand comm, params object[] para)
    {
        comm.Connection = conn;
        comm.Transaction = tran;
        for (int i = 0; i < para.Length; i++)
        {
            AddCommandParameter(comm, i, para[i]);
        }
        return comm.ExecuteNonQuery();
    }
    public int ExecuteNonQuery(SqlTransaction tran, SqlCommand comm)
    {
        comm.Connection = conn;
        comm.Transaction = tran;
        return comm.ExecuteNonQuery();
    }
    /// <summary>
    /// Vrátí null, když získaná value bude DBNull.Value
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="radek"></param>
    /// <param name="sloupec"></param>
    private string GetCellDataTableString(DataTable dataTable, int radek, int sloupec)
    {
        if (dataTable.Rows.Count > radek)
        {
            object[] o = dataTable.Rows[radek].ItemArray; ;
            if (o.Length >= sloupec)
            {
                if ( o[sloupec] == DBNull.Value)
                {
                    return null;
                }
                else
                {
                    return o[sloupec].ToString();
                }
            }
            return null;
        }
        return null;
        //throw new Exception("Zadaná buňka nebyla nalezena");
    }
    private bool GetCellDataTableBool(DataTable dataTable, int radek, int sloupec)
    {
        if (dataTable.Rows.Count > radek)
        {
            object[] o = dataTable.Rows[radek].ItemArray; ;
            if (o.Length >= sloupec)
            {
                if (o[sloupec] == DBNull.Value)
                {
                    return false;
                }
                else
                {
                    return bool.Parse(o[sloupec].ToString());
                }
            }
            return false;
        }
        return false;
    }
    private int GetCellDataTableInt(DataTable dataTable, int radek, int sloupec)
    {
        if (dataTable.Rows.Count > radek)
        {
            object[] o = dataTable.Rows[radek].ItemArray; ;
            if (o.Length >= sloupec)
            {
                if (o[sloupec] == DBNull.Value)
                {
                    return -1;
                }
                else
                {
                    return int.Parse(o[sloupec].ToString());
                }
            }
            return -1;
        }
        return -1;
    }
    public DataTable SelectDataTableAllRows(SqlTransaction tran, string table)
    {
        return SelectDataTable(tran, "SELECT * FROM" + " " + table);
    }
    public DataTable SelectDataTable(SqlTransaction tran, string tableName, int limit)
    {
        SqlCommand comm = new SqlCommand("SELECT TOP(" + limit.ToString() + " " + " *" + " " + "FROM" + " " + tableName);
        //AddCommandParameter(comm, 0, hodnotaWhere);
        return SelectDataTable(tran, comm);
    }
    public DataTable SelectDataTable(SqlTransaction tran, string tableName, int limit, string sloupecWhere, object hodnotaWhere)
    {
        SqlCommand comm = new SqlCommand("SELECT TOP(" + limit.ToString() + " " + " *" + " " + "FROM" + " " + tableName + GeneratorMsSql.SimpleWhere(sloupecWhere));
        AddCommandParameter(comm, 0, hodnotaWhere);
        return SelectDataTable(tran, comm);
    }
    /// <summary>
    /// 2
    /// </summary>
    public DataTable SelectAllRowsOfColumns(SqlTransaction tran, string p, params string[] selectSloupce)
    {
        return SelectDataTable(tran, string.Format("SELECT {0} FROM {1}", string.Join(AllChars.comma, selectSloupce), p));
    }
    public DataTable SelectAllRowsOfColumns(SqlTransaction tran, string p, List<string> ziskaneSloupce, string idColumnName, int idColumnValue)
    {
        string nazvy = string.Join(AllChars.comma, ziskaneSloupce.ToArray());
        SqlCommand comm = new SqlCommand(string.Format("SELECT {0} FROM {1} ", nazvy, p) + GeneratorMsSql.SimpleWhere(idColumnName));
        AddCommandParameter(comm, 0, idColumnValue);
        return SelectDataTable(tran, comm);
    }
    /// <summary>
    /// Vrátí mi všechny položky ze sloupce
    /// </summary>
    public DataTable SelectGreaterThan(SqlTransaction tran, string tableName, string tableColumn, object hodnotaOd)
    {
        SqlCommand comm = new SqlCommand(string.Format("SELECT * FROM {0} WHERE {1} > @p0", tableName, tableColumn), conn);
        AddCommandParameter(comm, 0, hodnotaOd);
        return SelectDataTable(tran, comm);
    }
    public string SelectCellDataTableStringOneRow(SqlTransaction tran, string tabulka, string hledanySloupec, params AB[] aB)
    {
        string sql = string.Format("SELECT TOP(1) {0} FROM {1} ", hledanySloupec, tabulka) + GeneratorMsSql.CombinedWhere(aB);
        SqlCommand comm = new SqlCommand(sql);
        for (int i = 0; i < aB.Length; i++)
        {
            AddCommandParameter(comm, i, aB[i].B);
        }
        DataTable dt = SelectDataTable(tran, comm);
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        object vr = dt.Rows[0].ItemArray[0];
        if (vr == DBNull.Value)
        {
            return null;
        }
        return vr.ToString();
    }
    public string SelectCellDataTableStringOneLastRow(SqlTransaction tran, string table, string idColumnName, object idColumnValue, string vracenySloupec, string unique_column)
    {
        //SELECT TOP 1 * FROM table_Name ORDER BY unique_column DESC
        string sql = GeneratorMsSql.SimpleWhereOneRow(vracenySloupec, table, idColumnName);
        SqlCommand comm = new SqlCommand(sql + string.Format(" ORDER BY {0} DESC", unique_column));
        AddCommandParameter(comm, 0, idColumnValue);
        DataTable dt = SelectDataTable(tran, comm);
        if (dt.Rows.Count == 0)
        {
            return "";
        }
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return dt.Rows[0].ItemArray[0].ToString();
    }
    public string SelectCellDataTableStringOneRow(SqlTransaction tran, string table, string idColumnName, object idColumnValue, string vracenySloupec)
    {
        string sql = GeneratorMsSql.SimpleWhereOneRow(vracenySloupec, table, idColumnName);
        SqlCommand comm = new SqlCommand(sql);
        AddCommandParameter(comm, 0, idColumnValue);
        DataTable dt = SelectDataTable(tran, comm);
        if (dt.Rows.Count == 0)
        {
            return "";
        }
        return dt.Rows[0].ItemArray[0].ToString();
    }
    public object[] SelectOneRow(SqlTransaction tran, string TableName, string nazevSloupce, object hodnotaSloupce)
    {
        // Index nemůže být ani pole bajtů ani null takže to je v pohodě
        DataTable dt = SelectDataTable(tran, "SELECT TOP(1) * FROM" + " " + TableName + " " + "WHERE" + " " + nazevSloupce + " = @p0", hodnotaSloupce);
        if (dt.Rows.Count == 0)
        {
            return null; // CA.CreateEmptyArray(pocetSloupcu);
        }
        return dt.Rows[0].ItemArray;
    }
    public List<string> SelectValuesOfIDs(SqlTransaction tran, string tabulka, List<int> idFces)
    {
        List<string> vr = new List<string>();
        foreach (int var in idFces)
        {
            vr.Add(SelectValueOfID(tran, tabulka, var));
        }
        return vr;
    }
    /// <summary>
    /// Jakékoliv změny zde musíš provést i v metodě SelectValuesOfColumnAllRowsStringTrim
    /// </summary>
    public List<string> SelectValuesOfColumnAllRowsString(SqlTransaction tran, string tabulka, string sloupec)
    {
        List<string> vr = new List<string>();
        SqlCommand comm = new SqlCommand(string.Format("SELECT {0} FROM {1}", sloupec, tabulka));
        DataTable dt = SelectDataTable(tran, comm);
        foreach (DataRow var in dt.Rows)
        {
            object o = var.ItemArray[0];
            if (o != DBNull.Value)
            {
                vr.Add(o.ToString());
            }
            else
            {
                vr.Add(null);
            }
        }
        return vr;
    }
    /// <summary>
    /// Jakékoliv změny zde musíš provést i v metodě SelectValuesOfColumnAllRowsString
    /// </summary>
    /// <param name="tabulka"></param>
    /// <param name="sloupec"></param>
    public List<string> SelectValuesOfColumnAllRowsStringTrim(SqlTransaction tran, string tabulka, string sloupec)
    {
        List<string> vr = new List<string>();
        SqlCommand comm = new SqlCommand(string.Format("SELECT {0} FROM {1}", sloupec, tabulka));
        DataTable dt = SelectDataTable(tran, comm);
        foreach (DataRow var in dt.Rows)
        {
            object o = var.ItemArray[0];
            if (o != DBNull.Value)
            {
                vr.Add(o.ToString().Trim());
            }
            else
            {
                vr.Add(null);
            }
        }
        return vr;
    }
    public bool SelectExistsCombination(SqlTransaction tran, string p, params AB[] aB)
    {
        string sql = string.Format("SELECT {0} FROM {1} {2}", aB[0].A, p, GeneratorMsSql.CombinedWhere(aB));
        ABC abc = new ABC(aB);
        DataTable result = SelectDataTable(tran, sql, abc.OnlyBs());
        return result.Rows.Count != 0;
    }
    public bool SelectExistsCombination(SqlTransaction tran, string p, out DataTable result, params AB[] aB)
    {
        string sql = string.Format("SELECT {0} FROM {1} {2}", aB[0].A, p, GeneratorMsSql.CombinedWhere(aB));
        SqlCommand comm = new SqlCommand(sql);
        AddCommandParameterFromAbc(comm, aB);
        result = SelectDataTable(tran, comm);
        return result.Rows.Count != 0;
    }
    public bool SelectExistsCombination(SqlTransaction tran, string p, out DataTable result, string sloupceKVraceni, params AB[] aB)
    {
        string sql = string.Format("SELECT {0} FROM {1} {2}", sloupceKVraceni, p, GeneratorMsSql.CombinedWhere(aB));
        SqlCommand comm = new SqlCommand(sql);
        AddCommandParameterFromAbc(comm, aB);
        result = SelectDataTable(tran, comm);
        return result.Rows.Count != 0;
    }
    public bool SelectExists(SqlTransaction tran, string tabulka, string sloupec, object hodnota)
    {
        string sql = string.Format("SELECT {0} FROM {1} {2}", sloupec, tabulka, GeneratorMsSql.SimpleWhere(sloupec));
        DataTable result = SelectDataTable(tran, sql, hodnota);
        return result.Rows.Count != 0;
    }
    public int SelectID(SqlTransaction tran, string tabulka, string nazevSloupce, object hodnotaSloupce)
    {
        SqlCommand c = new SqlCommand(string.Format("SELECT (ID) FROM {0} WHERE {1} = @p0", tabulka, nazevSloupce), conn, tran);
        AddCommandParameter(c, 0, hodnotaSloupce);
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter(c);
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            int vr = 0;
            if (int.TryParse(dt.Rows[0].ItemArray[0].ToString(), out vr))
            {
                return vr;
            }
            //return int.Parse();
        }
        else
        {
            return int.MinValue;
        }
        return int.MinValue;
    }
    public int SelectIDOrNextIndex(SqlTransaction tran, string tabulka, string nazevSloupce, object hodnotaSloupce)
    {
        int dex = SelectID(tran, tabulka, nazevSloupce, hodnotaSloupce);
        if (dex == int.MinValue)
        {
            // Metoda již řádek vkládá a SQLiteCommand vrací jen tak..
            InsertRowTypeEnumIfNotExists(tran, tabulka, hodnotaSloupce.ToString());
            return SelectFindOutNumberOfRows(tran, tabulka);
        }
        return dex;
    }
    public void InsertRowTypeEnumIfNotExists(SqlTransaction tran, string tabulka, string nazev)
    {
        if (SelectID(tran, tabulka, "Name", nazev) == int.MinValue)
        {
            SqlCommand c = new SqlCommand(string.Format("INSERT INTO {0} (ID, Name) VALUES (@p0, @p1)", tabulka), conn, tran);
            AddCommandParameter(c, 0, SelectFindOutNumberOfRows(tran, tabulka) + 1);
            AddCommandParameter(c, 1, nazev);
            c.ExecuteNonQuery();
        }
    }
    public string SelectValueOfID(SqlTransaction tran, string tabulka, int id)
    {
        return GetCellDataTableString(SelectDataTableSelective(tran, tabulka, "ID", id), 0, 1);
    }
    /// <summary>
    /// Vrátí mi 2. sloupec v celém získáném řádku.
    /// Musí to být sloupec Name
    /// </summary>
    /// <param name="tabulka"></param>
    /// <param name="idValue"></param>
    /// <param name="idColumn"></param>
    public string SelectValueOfID(SqlTransaction tran, string tabulka, int idValue, string idColumn)
    {
        return GetCellDataTableString(SelectDataTableSelective(tran, tabulka, idColumn, idValue), 0, 1);
    }
    public string SelectValueOfIDOrSE(SqlTransaction tran, string tabulka, int id)
    {
        DataTable dt = SelectDataTableSelective(tran, tabulka, "ID", id);
        if (dt.Rows.Count == 0)
        {
            return "";
        }
        //SQLiteCommand comm = new SQLiteCommand(string.Format("SELECT Name FROM {0} WHERE ID = {1}", tabulka, id));
        return GetCellDataTableString(dt, 0, 1);
    }
    public string SelectValueOfIDOrSE(SqlTransaction tran, string tabulka, int id, string idColumnName)
    {
        DataTable dt = SelectDataTableSelective(tran, tabulka, idColumnName, id);
        if (dt.Rows.Count == 0)
        {
            return "";
        }
        //SQLiteCommand comm = new SQLiteCommand(string.Format("SELECT Name FROM {0} WHERE ID = {1}", tabulka, id));
        return GetCellDataTableString(dt, 0, 1);
    }
    /// <summary>
    /// Přičte ke stávající hodnotě novou a vypočte nový průměr
    /// </summary>
    /// <summary>
    /// Vrátí nohou hodnotu v DB
    /// </summary>
    /// <param name="table"></param>
    /// <param name="sloupecID"></param>
    /// <param name="id"></param>
    /// <param name="sloupecKUpdate"></param>
    /// <param name="pridej"></param>
    public int UpdateSumIntValue(SqlTransaction tran, string table, string sloupecKUpdate, int pridej, string sloupecID, int id)
    {
        int d = int.Parse(SelectCellDataTableStringOneRow(tran, table, sloupecID, id, sloupecKUpdate).ToString());
        int n = pridej;
        if (d != 0)
        {
            n = (d + pridej) / 2;
        }
        Update(tran, table, sloupecKUpdate, n, sloupecID, id);
        return n;
    }
    public int UpdateAppendStringValue(SqlTransaction tran, string tableName, string sloupecAppend, string hodnotaAppend, string sloupecID, object hodnotaID)
    {
        string aktual = SelectCellDataTableStringOneRow(tran, tableName, sloupecID, hodnotaID, sloupecAppend).ToString();
        aktual = aktual.Trim();
        aktual += hodnotaAppend + AllStrings.comma;
        return Update(tran, tableName, sloupecAppend, aktual, sloupecID, hodnotaID);
    }
    /// <summary>
    /// Pouze když hodnota nebude existovat, přidá ji znovu
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="sloupecID"></param>
    /// <param name="hodnotaID"></param>
    /// <param name="sloupecAppend"></param>
    /// <param name="hodnotaAppend"></param>
    public int UpdateAppendStringValueCheckExists(SqlTransaction tran, string tableName, string sloupecAppend, string hodnotaAppend, string sloupecID, object hodnotaID)
    {
        string aktual = SelectCellDataTableStringOneRow(tran, tableName, sloupecID, hodnotaID, sloupecAppend).ToString();
        aktual = aktual.Trim();
        List<string> d = new List<string>(SHSplit.Split(aktual, AllStrings.comma));
        if (!d.Contains(hodnotaAppend))
        {
            aktual += hodnotaAppend + AllStrings.comma;
            string save = string.Join(AllChars.comma, d.ToArray());
            return Update(tran, tableName, sloupecAppend, aktual, sloupecID, hodnotaID);
        }
        return 0;
    }
    public int UpdateCutStringValue(SqlTransaction tran, string tableName, string sloupecID, object hodnotaID, string sloupecCut, string hodnotaCut)
    {
        string aktual = SelectCellDataTableStringOneRow(tran, tableName, sloupecID, hodnotaID, sloupecCut).ToString();
        aktual = aktual.Trim();
        List<string> d = new List<string>(SHSplit.Split(aktual, AllStrings.comma));
        d.Remove(hodnotaCut);
        string save = string.Join(AllChars.comma, d.ToArray());
        return Update(tran, tableName, sloupecCut, save, sloupecID, hodnotaID);
    }
    /// <summary>
    /// Conn nastaví automaticky
    /// </summary>
    public int Update(SqlTransaction tran, string table, string sloupecKUpdate, object n, string sloupecID, object id)
    {
        string sql = string.Format("UPDATE {0} SET {1}=@p0 WHERE {2} = @p1", table, sloupecKUpdate, sloupecID);
        SqlCommand comm = new SqlCommand(sql, conn, tran);
        AddCommandParameter(comm, 0, n);
        AddCommandParameter(comm, 1, id);
        //SqlException: String or binary data would be truncated.
        return comm.ExecuteNonQuery();
    }
    public void UpdateValuesCombination(SqlTransaction tran, string TableName, string nameOfColumn, object valueOfColumn, params object[] setsNameValue)
    {
        ABC abc = new ABC(setsNameValue);
        UpdateValuesCombination(tran, TableName, nameOfColumn, valueOfColumn, abc.ToArray());
    }
    /// <summary>
    /// Conn nastaví automaticky
    /// </summary>
    public void UpdateValuesCombination(SqlTransaction tran, string TableName, string nameOfColumn, object valueOfColumn, params AB[] sets)
    {
        string setString = GeneratorMsSql.CombinedSet(sets);
        //int pocetParametruSets = sets.Length;
        int indexParametrWhere = sets.Length;
        SqlCommand comm = new SqlCommand(string.Format("UPDATE {0} {1} WHERE {2}={3}", TableName, setString, nameOfColumn, "@p" + (indexParametrWhere).ToString()));
        for (int i = 0; i < indexParametrWhere; i++)
        {
            AddCommandParameter(comm, i, sets[i].B);
            //}
        }
        AddCommandParameter(comm, indexParametrWhere, valueOfColumn);
        // NT-Při úpravách uprav i UpdateValuesCombinationCombinedWhere
        ExecuteNonQuery(tran, comm);
    }
    /// <summary>
    /// Conn nastaví automaticky
    /// Vrátí zda byl vymazán alespoň jeden řádek
    /// </summary>
    /// <param name="TableName"></param>
    /// <param name="where"></param>
    public bool Delete(SqlTransaction tran, string TableName, params AB[] where)
    {
        string whereS = GeneratorMsSql.CombinedWhere(where);
        SqlCommand comm = new SqlCommand("DELETE FROM" + " " + TableName + whereS);
        AddCommandParameterFromAbc(comm, where);
        int f = ExecuteNonQuery(tran, comm);
        return f > 0;
    }
    public int Delete(SqlTransaction tran, string table, string sloupec, object id)
    {
        return ExecuteNonQuery(tran, new SqlCommand(string.Format("DELETE FROM {0} WHERE {1} = @p0", table, sloupec)), id);
    }
    /// <summary>
    /// Conn nastaví automaticky
    /// Raději používej tuto metodu s 4/2 parametrem sloupecID, pokud používáš tabulky ve kterých není první sloupec ID
    /// </summary>
    /// <param name="tabulka"></param>
    /// <param name="nazvySloupcu"></param>
    /// <param name="sloupce"></param>
    public int InsertToTable(SqlTransaction tran, string tabulka, string nazvySloupcu, params object[] sloupce)
    {
        return InsertToTable2(tran, tabulka, "ID", nazvySloupcu, sloupce);
    }
    /// <summary>
    /// Conn nastaví automaticky
    /// IB - podle W3S ale dobře
    /// A2 je zde proto aby se po vložení zjistilo jaké ID je poslední v tabulce.
    /// </summary>
    public int InsertToTable2(SqlTransaction tran, string tabulka, string sloupecID, string nazvySloupcu, params object[] sloupce)
    {
        string hodnoty = MSDatabaseLayer.GetValues(sloupce);
        SqlCommand comm = new SqlCommand(string.Format("INSERT INTO {0} {1} VALUES {2}", tabulka, nazvySloupcu, hodnoty), conn, tran);
        for (int i = 0; i < sloupce.Length; i++)
        {
            AddCommandParameter(comm, i, sloupce[i]);
        }
        comm.ExecuteNonQuery();
        return SelectLastIDFromTable(tran, tabulka, sloupecID);
    }
    /// <summary>
    /// Raději používej metodu s 3/2A sloupecID, pokud používáš v tabulce sloupce ID, které se nejmenují ID
    /// Sloupec u kterého se bude určovat poslední index a ten inkrementovat a na ten vkládat je ID
    /// Používej tehdy když ID sloupec má nějaký standardní název, Tedy ID, ne IDUsers atd.
    /// </summary>
    /// <param name="tabulka"></param>
    /// <param name="sloupce"></param>
    public int InsertToRow(SqlTransaction tran, string tabulka, params object[] sloupce)
    {
        return InsertToRow2(tran, tabulka, "ID", sloupce);
    }
    /// <summary>
    /// Do této metody se vkládají hodnoty bez ID
    /// ID se počítá jako v Sqlite - tedy od 1
    /// A2 je zde proto aby se mohlo určit poslední index a ten inkrementovat a na ten vložit. Název/hodnota/whatever tohoto sloupce musí být 1. v A3.
    /// Používej tehdy když ID sloupec má nějaký speciální název, např. IDUsers
    /// </summary>
    /// <param name="tabulka"></param>
    /// <param name="sloupce"></param>
    public int InsertToRow2(SqlTransaction tran, string tabulka, string sloupecID, params object[] sloupce)
    {
        // Nyní je to +1 protože při vkládání do Ggd_Songs mi to názelo chyby nesprávný počet parametrů. Pokud ti to bude házet zase u jiných webů než Ggdag, uprav to na bez "+ 1"
        string hodnoty = MSDatabaseLayer.GetValuesDirect(sloupce.Length + 1);
        int d = SelectLastIDFromTable(tran, tabulka, sloupecID);
        SqlCommand comm = new SqlCommand(string.Format("INSERT INTO {0} VALUES {1}", tabulka, hodnoty), conn, tran);
        comm.Parameters.AddWithValue("@p0", ++d);
        int to = sloupce.Length + 1;
        for (int i = 1; i < to; i++)
        {
            object o = sloupce[i - 1];
            AddCommandParameter(comm, i, o);
            //DateTime.Now.Month;
        }
        comm.ExecuteNonQuery();
        return d;
    }
    /// <summary>
    /// Vrací A2
    /// A2 je ID řádku na který se bude vkládat. Název/hodnota/whatever tohoto sloupce musí být 1. v A3.
    /// Používej tehdy když chceš určit index na který vkládat.
    /// </summary>
    /// <param name="tabulka"></param>
    /// <param name="IDUsers"></param>
    /// <param name="sloupce"></param>
    public void Insert3(SqlTransaction tran, string tabulka, int IDUsers, params object[] sloupce)
    {
        string hodnoty = MSDatabaseLayer.GetValues(sloupce);
        SqlCommand comm = new SqlCommand(string.Format("INSERT INTO {0} VALUES {1}", tabulka, hodnoty), conn, tran);
        comm.Parameters.AddWithValue("@p0", IDUsers);
        int to = sloupce.Length + 1;
        for (int i = 1; i < to; i++)
        {
            object o = sloupce[i - 1];
            AddCommandParameter(comm, i, o);
            //DateTime.Now.Month;
        }
        comm.ExecuteNonQuery();
    }
    /// <summary>
    /// Vrací A2
    /// A2 je ID řádku na který se bude vkládat. Název/hodnota/whatever tohoto sloupce musí být 1. v A3.
    /// Používej tehdy když chceš určit index na který vkládat.
    /// </summary>
    /// <param name="tabulka"></param>
    /// <param name="IDUsers"></param>
    /// <param name="sloupce"></param>
    public void Insert3(SqlTransaction tran, string tabulka, long IDUsers, params object[] sloupce)
    {
        string hodnoty = MSDatabaseLayer.GetValues(sloupce);
        SqlCommand comm = new SqlCommand(string.Format("INSERT INTO {0} VALUES {1}", tabulka, hodnoty), conn, tran);
        comm.Parameters.AddWithValue("@p0", IDUsers);
        int to = sloupce.Length + 1;
        for (int i = 1; i < to; i++)
        {
            object o = sloupce[i - 1];
            AddCommandParameter(comm, i, o);
            //DateTime.Now.Month;
        }
        comm.ExecuteNonQuery();
    }
    /// <summary>
    /// Vrací skutečně nejvyšší ID, proto když chceš pomocí ní ukládat do DB, musíš si to číslo inkrementovat
    /// </summary>
    /// <param name="p"></param>
    /// <param name="sloupecID"></param>
    public int SelectLastIDFromTable(SqlTransaction tran, string p, string sloupecID)
    {
        string dd = ExecuteScalar(tran, new SqlCommand("SELECT MAX(" + sloupecID + " " + " " + "FROM" + " " + p)).ToString();
        if (dd == "")
        {
            return 0;
        }
        return int.Parse(dd);
    }
    /// <summary>
    /// Může se používat pouze když je sloupec ID = ID
    /// Vrací skutečně nejvyšší ID, proto když chceš pomocí ní ukládat do DB, musíš si to číslo inkrementovat
    /// </summary>
    public int SelectLastIDFromTable(SqlTransaction tran, string p)
    {
        return SelectLastIDFromTable(tran, p, "ID");
    }
    string cs = null;
    /// <summary>
    /// Tuto metodu nepoužívej například po vkládání, když chceš zjistit ID posledního řádku, protože když tam bude něco smazaného , tak to budeš mít o to posunuté !!
    ///
    /// </summary>
    public int SelectFindOutNumberOfRows(SqlTransaction tran, string tabulka)
    {
        using (var conn = new SqlConnection(cs))
        {
            conn.Open();
            SqlCommand comm = new SqlCommand("SELECT Count(*) FROM" + " " + tabulka, conn, tran);
            //comm.Transaction = tran;
            string s = comm.ExecuteScalar().ToString();
            conn.Close();
            return int.Parse(s);
        }
    }
    public List<string> SelectGetAllTablesInDB(SqlTransaction tran)
    {
        List<string> vr = new List<string>();
        DataTable dt = SelectDataTableWithWhere(tran, "INFORMATION_SCHEMA.TABLES", AllStrings.asterisk, "TABLE_TYPE", "BASE TABLE");
        foreach (DataRow item in dt.Rows)
        {
            vr.Add(item.ItemArray[2].ToString());
        }
        return vr;
    }
    /// <summary>
    /// Conn nastaví automaticky
    /// </summary>
    /// <param name="tabulka"></param>
    /// <param name="sloupce"></param>
    /// <param name="whereName"></param>
    /// <param name="whereValue"></param>
    private DataTable SelectDataTableWithWhere(SqlTransaction tran, string tabulka, string sloupce, string whereName, object whereValue)
    {
        return SelectDataTable(tran, string.Format("SELECT {0} FROM {1} WHERE {2} = @p0", sloupce, tabulka, whereName), whereValue);
        //return null;
    }
    public List<string> SelectColumnsNamesOfTable(SqlTransaction tran, string p)
    {
        List<string> vr = new List<string>();
        //SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TableNameGoesHere'
        DataTable dt = SelectDataTableWithWhere(tran, "INFORMATION_SCHEMA.COLUMNS", "COLUMN_NAME", "TABLE_NAME", p);
        foreach (DataRow item in dt.Rows)
        {
            vr.Add(item.ItemArray[0].ToString());
        }
        return vr;
    }
    public bool SelectExistsTable(SqlTransaction tran, string p)
    {
        DataTable dt = SelectDataTable(tran, string.Format("SELECT * FROM sysobjects WHERE id = object_id(N'{0}') AND OBJECTPROPERTY(id, N'IsUserTable') = 1", p));
        return dt.Rows.Count != 0;
    }
    public int DropTableIfExists(SqlTransaction tran, string table)
    {
        if (SelectExistsTable(tran, table))
        {
            return ExecuteNonQuery(tran, new SqlCommand("DROP TABLE" + " " + table));
        }
        return 0;
    }
    public void DropAllTables(SqlTransaction tran)
    {
        List<string> dd = SelectGetAllTablesInDB(tran);
        foreach (string item in dd)
        {
            ExecuteNonQuery(tran, new SqlCommand("DROP TABLE" + " " + item));
        }
    }
    public void UpdateValuesCombinationCombinedWhere(SqlTransaction tran, string TableName, ABC sets, ABC where)
    {
        string setString = GeneratorMsSql.CombinedSet(sets);
        string whereString = GeneratorMsSql.CombinedWhere(where);
        int indexParametrWhere = sets.Length + 1;
        SqlCommand comm = new SqlCommand(string.Format("UPDATE {0} {1} WHERE {2}", TableName, setString, whereString));
        //bool first = true;
        for (int i = 0; i < indexParametrWhere; i++)
        {
            AddCommandParameter(comm, i, sets[i].B);
            //}
        }
        indexParametrWhere--;
        for (int i = 0; i < where.Length; i++)
        {
            AddCommandParameter(comm, indexParametrWhere++, where[i].B);
        }
        // NT-Při úpravách uprav i UpdateValuesCombination
        ExecuteNonQuery(tran, comm);
    }
    public object SelectValueOfIDOneRow(SqlTransaction tran, string tabulka, string idColumnName, int idColumnValue, string vracenySloupec)
    {
        SqlCommand comm = new SqlCommand("SELECT TOP(1)" + " " + vracenySloupec + " " + "FROM" + " " + tabulka + " " + "WHERE" + " " + idColumnName + " = @p0");
        AddCommandParameter(comm, 0, idColumnValue);
        //comm.Connection = conn;
        DataTable dt = SelectDataTable(tran, comm);
        if (dt.Rows.Count != 0)
        {
            return dt.Rows[0][0];
        }
        return null;
    }
    /// <param name="table"></param>
    /// <param name="sloupecID"></param>
    /// <param name="id"></param>
    /// <param name="sloupecKUpdate"></param>
    /// <param name="pridej"></param>
    public float UpdatePlusRealValue(SqlTransaction tran, string table, string sloupecKUpdate, float pridej, string sloupecID, int id)
    {
        float d = float.Parse(SelectCellDataTableStringOneRow(tran, table, sloupecID, id, sloupecKUpdate).ToString());
        if (d != 0)
        {
            pridej = (d + pridej) / 2;
        }
        Update(tran, table, sloupecKUpdate, pridej, sloupecID, id);
        return pridej;
    }
    public int SelectCellDataTableIntOneRow(SqlTransaction tran, string table, string idColumnName, object idColumnValue, string vracenySloupec)
    {
        string sql = GeneratorMsSql.SimpleWhereOneRow(vracenySloupec, table, idColumnName);
        SqlCommand comm = new SqlCommand(sql);
        AddCommandParameter(comm, 0, idColumnValue);
        DataTable dt = SelectDataTable(tran, comm);
        if (dt.Rows.Count == 0)
        {
            return -1;
        }
        return int.Parse(dt.Rows[0].ItemArray[0].ToString());
    }
    public object SelectCellDataTableObjectOneRow(SqlTransaction tran, string table, string vracenySloupec, string idColumnName, object idColumnValue)
    {
        string sql = GeneratorMsSql.SimpleWhereOneRow(vracenySloupec, table, idColumnName);
        SqlCommand comm = new SqlCommand(sql);
        AddCommandParameter(comm, 0, idColumnValue);
        DataTable dt = SelectDataTable(tran, comm);
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return dt.Rows[0].ItemArray[0];
    }
    public void InsertToTable3(SqlTransaction tran, string table, string sloupce, string valuesParams, object[] values)
    {
        SqlCommand comm = new SqlCommand("INSERT INTO" + " " + table + AllStrings.space + sloupce + " " + "VALUES" + " " + valuesParams);
        for (int i = 0; i < values.Length; i++)
        {
            AddCommandParameter(comm, i, values[i]);
        }
        ExecuteNonQuery(tran, comm);
        //InsertToTable2;
    }
}

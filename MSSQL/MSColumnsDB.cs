
namespace SunamoSqlServer.MSSQL;
using SunamoConverters.Converts;
using SunamoDevCode;
using SunamoDevCode.Enums;
using SunamoHtml.Generators;
using SunamoSqlServer._sunamo;
using SunamoStringTrim;


//using System.Activities;
public partial class MSColumnsDB : List<MSSloupecDB>
{
    /// <summary>
    /// A2 is name of site - part before Layer class
    /// A3 is Lyr_ etc.
    /// A4 is parameter which is inserted into GetSqlCreateTable.
    /// dynamicTables - whether is not desirable to create references to other tables. Good while test tables and apps, when I will it delete later.
    /// </summary>
    public static string GetSqlForClearAndCreateTables(Dictionary<string, MSColumnsDB> sl, string Mja, string tablePrefix, bool dynamicTables)
    {
        StringBuilder dropes = new StringBuilder();
        StringBuilder createTable = new StringBuilder();

        foreach (KeyValuePair<string, MSColumnsDB> item in sl)
        {
            if (!IsDynamic(item.Key, tablePrefix))
            {
                dropes.AppendLine("MSStoredProceduresI.ci.DropTableIfExists(\"" + item.Key + "\");");
            }
        }
        foreach (KeyValuePair<string, MSColumnsDB> item in sl)
        {
            if (!IsDynamic(item.Key, tablePrefix))
            {
                createTable.AppendLine(Mja + "Layer.s[\"" + item.Key + "\"].GetSqlCreateTable(\"" + item.Key + "\", " + dynamicTables.ToString().ToLower() + ").ExecuteNonQuery();");
            }
        }

        return dropes.ToString() + Environment.NewLine + createTable.ToString();
    }

    /// <summary>
    /// True if name of table after prefix contains another _
    /// </summary>
    /// <param name="p"></param>
    /// <param name="tablePrefix"></param>
    private static bool IsDynamic(string p, string tablePrefix)
    {
        //List<int> nt = SH.ReturnOccurencesOfString(p, "_");
        string d = null;
        if (p.StartsWith(tablePrefix) && tablePrefix != "")
        {
            d = p.Substring(tablePrefix.Length);
        }
        else
        {
            d = p;
        }

        return d.Contains("_");
    }


    public string GetTROfColumns()
    {
        StringBuilder sb = new StringBuilder();
        if (this.Count == 0)
        {
            throw new Exception("Nebyly nalezeny žádné sloupce");
        }
        sb.Append("(");
        foreach (MSSloupecDB item in this)
        {
            sb.Append(item.Name + ",");
        }
        return sb.ToString().TrimEnd(',') + ")";
    }


    /// <summary>
    /// Vyplň A2 na SE pokud chceš všechny sloupce
    /// </summary>
    /// <param name="tableNameWithPrefix"></param>
    /// <param name="columns"></param>
    public string GetCsSunamoGridView(string tableNameWithPrefix, string columns)
    {
        CSharpGenerator csg = new CSharpGenerator();
        csg.AppendLine(2, "DataTable dt = MSStoredProceduresI.ci.SelectDataTableSelective(\"" + tableNameWithPrefix + "\");");
        csg.AppendLine(2, "int radku = dt.Rows.Count;");
        List<string> s = new List<string>();
        List<string> ss = new List<string>();
        foreach (MSSloupecDB item in this)
        {
            string a = item.Name + "s";
            s.Add(item.Name);
            ss.Add(a);
            csg.AppendLine(2, "List<string> {0} = new string[radku];", a);
        }
        csg.AppendLine(2, "");
        csg.AppendLine(2, "");
        csg.AppendLine(2, "int i = 0;");
        csg.AppendLine(2, "foreach (DataRow item in dt.Rows)");
        csg.StartBrace(2);
        csg.AppendLine(3, "object[] o = item.ItemArray;");
        for (int i = 0; i < this.Count; i++)
        {
            csg.AppendLine(3, ss[i] + "[i] = o[" + i.ToString() + "].ToString();");
        }
        csg.AppendLine(3, "i++;");
        csg.EndBrace(2);
        csg.AppendLine(2, "SunamoGridView sgv = new SunamoGridView({0});", string.Join(',', ss.ToArray()));
        csg.AppendLine(2, "");
        csg.AppendLine(2, "");
        for (int i = 0; i < ss.Count; i++)
        {
            csg.AppendLine(2, "sgv.AddSpanColumn(\"" + s[i] + "\", " + i + ");");
        }
        csg.AppendLine(2, "");
        csg.AppendLine(2, "");
        csg.AppendLine(2, "");
        return csg.ToString();
    }

    /// <summary>
    /// A1
    /// A2 = mss_, může být vloženo i Nope_, on si jej automaticky nahradí za SE
    /// A3 = folder to write in scz
    /// A4 = folder to write in sczAdmin, can be null but not SE
    /// dříve A5 = bool generate4 ale NSN
    /// </summary>
    /// <param name="nazevTabulky"></param>
    /// <param name="dbPrefix"></param>
    /// <param name="folderSaveToDirectoryName"></param>
    /// <param name="generate4"></param>
    public string SaveCsTableRow(string nazevTabulky, string dbPrefix, string folderSaveToDirectoryName, string folderSunamoCzAdminSaveToDirectoryName)
    {
        if (nazevTabulky.Contains("DayView") || nazevTabulky.EndsWith("2"))
        {
            return "";
        }

        // 1) create full = PathSczAdmin
        string nameCs = null;
        string cs = GetCsTableRow(signed, nazevTabulky, dbPrefix, out nameCs); // cs
        string Path2 = Path.Combine(folderSaveToDirectoryName, "DontCopy2", nameCs + ".cs");
        string PathSczAdmin = null;
        if (folderSunamoCzAdminSaveToDirectoryName != null)
        {
            PathSczAdmin = Path.Combine(folderSunamoCzAdminSaveToDirectoryName, "DontCopy2", nameCs + ".cs");
        }


        // 2) create base = PathSczBaseAdmin
        string csBase = GetCsTableRowBase(nazevTabulky, dbPrefix); // cs

        string PathSczBaseAdmin = null;
        if (folderSunamoCzAdminSaveToDirectoryName != null)
        {
            PathSczBaseAdmin = Path.Combine(folderSunamoCzAdminSaveToDirectoryName, "DontCopyBase", nameCs + "Base.cs");
        }

        // 3) PathBase

        string PathBase = Path.Combine(folderSaveToDirectoryName, "DontCopyBase", nameCs + "Base.cs");



        FS.CreateUpfoldersPsysicallyUnlessThere(Path2);
        //FS.CreateUpfoldersPsysicallyUnlessThere(Path4);
        FS.CreateUpfoldersPsysicallyUnlessThere(PathBase);



        if (PathSczAdmin != null)
        {
            FS.CreateUpfoldersPsysicallyUnlessThere(PathSczAdmin);
            FS.CreateUpfoldersPsysicallyUnlessThere(PathSczBaseAdmin);
        }

        // 1) PathSczAdmin

        if (replaceMSinMSStoredProceduresI != null)
        {
            string cs2 = cs.Replace("MSStoredProceduresI", replaceMSinMSStoredProceduresI + "StoredProceduresI");
            File.WriteAllTextAsync(Path2, cs2);
            if (PathSczAdmin != null)
            {
                File.WriteAllTextAsync(PathSczAdmin, cs2);
            }

        }
        else
        {
            File.WriteAllTextAsync(Path2, cs);
            if (PathSczAdmin != null)
            {
                File.WriteAllTextAsync(PathSczAdmin, cs);
            }
        }

        // 2) PathSczBaseAdmin

        if (PathSczBaseAdmin != null)
        {
            File.WriteAllTextAsync(PathSczBaseAdmin, csBase);
        }

        // 3) PathBase

        File.WriteAllTextAsync(PathBase, csBase);

        return "Uloženo do souboru " + Path2;
    }

    /// <summary>
    /// Do A2 může být vloženo i Nope_, on si jej automaticky nahradí za SE
    /// </summary>
    /// <param name="nazevTabulky"></param>
    /// <param name="dbPrefix"></param>
    public string GetCsTableRow4(string nazevTabulky, string dbPrefix)
    {
        string nazevCs;
        return GetCsTableRow4(nazevTabulky, dbPrefix, out nazevCs);
    }

    /// <summary>
    /// Generate base (TableRowAlbumBase)
    /// Do A2 může být vloženo i Nope_, on si jej automaticky nahradí za SE
    /// </summary>
    /// <param name="nazevTabulky"></param>
    /// <param name="dbPrefix"></param>
    public string GetCsTableRowBase(string nazevTabulky, string dbPrefix)
    {
        string nazevCs;
        return GetCsTableRowBase(nazevTabulky, dbPrefix, out nazevCs);
    }

    /// <summary>
    /// Do A2 může být vloženo i Nope_, on si jej automaticky nahradí za SE
    /// </summary>
    /// <param name="nazevTabulky"></param>
    /// <param name="dbPrefix"></param>
    /// <param name="tableName"></param>
    public string GetCsTableRowBase(string nazevTabulky, string dbPrefix, out string tableName)
    {
        string dbPrefix2 = dbPrefix;
        if (dbPrefix2 == "Nope_")
        {
            dbPrefix2 = "";
        }
        //string nazevTabulkyCopy = (nazevTabulky);

        CSharpGenerator csg = new CSharpGenerator();
        if (nazevTabulky.StartsWith(dbPrefix))
        {
            nazevTabulky = nazevTabulky.Substring(dbPrefix.Length);
        }

        bool isDynamicTable = false;
        if (nazevTabulky.Contains("_"))
        {
            isDynamicTable = true;
            nazevTabulky = Case.NET.CaseConverter.PascalCase.ConvertCase(nazevTabulky); //ConvertPascalConvention.ToConvention(nazevTabulky);
        }

        tableName = "TableRow" + nazevTabulky + "Base";
        csg.Using(usings);
        csg.StartClass(0, AccessModifiers.Public, false, tableName);

        if (isDynamicTable)
        {
            csg.Field(1, AccessModifiers.Private, false, VariableModifiers.None, "string", _tableNameField, true);
        }

        // 1) generate params for ctor, all fields
        List<string> paramsForCtor = new List<string>(this.Count * 2);
        foreach (MSSloupecDB item in this)
        {
            string typ = MSDatabaseLayer.ConvertSqlDbTypeToDotNetType(item.Type2);
            if (typ == "string" && item.Delka.ToUpper() != "(MAX)")
            {
                string name = item.Name;
                if (item.Delka != string.Empty)
                {
                    name = name.Replace(item.Delka, string.Empty);
                }
                string nameLower = SH.FirstCharLower(name);
                // Public kvůli používání ve SunamoCzAdmin.Cmd a SunamoCzAdmin.Wpf
                csg.Field(1, AccessModifiers.Private, false, VariableModifiers.None, typ, nameLower, true);
                csg.Property(1, AccessModifiers.Public, false, typ, name, "return SHSubstring.Substring(" + nameLower + ",0," + item.LengthWithoutBraces + ", true);", true, nameLower);
                paramsForCtor.Add(typ);
                paramsForCtor.Add(name);
            }
            else
            {
                string name = item.Name;
                // Public kvůli používání ve SunamoCzAdmin.Cmd a SunamoCzAdmin.Wpf
                csg.Field(1, AccessModifiers.Public, false, VariableModifiers.None, typ, name, true);
                paramsForCtor.Add(typ);
                paramsForCtor.Add(name);
            }
        }

        // 2) ctors
        csg.Append(1, GenerateCtors(tableName, isDynamicTable, paramsForCtor, true, dbPrefix));

        // 3) TableName property
        if (isDynamicTable)
        {
            csg.Property(1, AccessModifiers.Protected, false, "string", "TableName", true, false, _tableNameField);
        }
        else
        {
            csg.Property(1, AccessModifiers.Protected, false, "string", "TableName", true, false, "Tables." + dbPrefix2 + nazevTabulky);
        }

        // 4) Parse row

        CSharpGenerator innerParseRow = new CSharpGenerator();

        innerParseRow.AppendLine(2, "if (o != null)");
        innerParseRow.AppendLine(2, "{");
        for (int i = 0; i < this.Count; i++)
        {
            MSSloupecDB item = this[i];
            innerParseRow.AppendLine(3, Copy(item.Name) + " = MSTableRowParse." + ConvertSqlDbTypeToGetMethod(item.Type2, item.canBeNull) + "(o," + i.ToString() + ");");
        }

        // Na závěr každé metody nesmí být AppendLine
        innerParseRow.Append(2, "}");
        // Musí být public, protože když získám DataTable, jak ji mám pak co nejrychleji napasovat na nějaký objekt?
        csg.Method(2, "protected void ParseRow(object[] o)", innerParseRow.ToString());
        csg.EndBrace(0);

        return csg.ToString();
    }

    public static string Copy(string p)
    {
        return p;
    }

    public string GetCsTableRow(bool signed, string nazevTabulky, string dbPrefix)
    {
        string nazevCs;
        return GetCsTableRow(signed, nazevTabulky, dbPrefix, out nazevCs);
    }
    public bool IsOtherColumnID
    {
        get
        {
            return this[0].Name != "ID";
        }
    }
    /// <summary>
    /// Do A1 se dává prostě klíč ze slovníku, do A2 mss.ToString() + "_"
    /// </summary>
    /// <param name="nazevTabulky"></param>
    /// <param name="dbPrefix"></param>
    public string GetNameTableRow(string nazevTabulky, string dbPrefix, out bool isDynamicTable, out string nazevTabulkyJC, out string dbPrefix2)
    {
        isDynamicTable = false;
        dbPrefix2 = dbPrefix;
        if (dbPrefix2 == "Nope_")
        {
            dbPrefix2 = "";
        }
        // OBSAHUJE I PREFIX, TAKŽE TŘEBA Koc_
        string nazevTabulkyCopy = (nazevTabulky);
        string niMethod = CSharpGenerator.AddTab(2, "throw new Exception();");

        // ZBAVÍM TABULKU nazevTabulky PREFIXU, ČILI NEOBSAHUJE NAPŘ. Koc_
        if (nazevTabulky.StartsWith(dbPrefix))
        {
            nazevTabulky = nazevTabulky.Substring(dbPrefix.Length);
        }
        if (nazevTabulky.Contains("_"))
        {
            isDynamicTable = true;
            nazevTabulky = ConvertPascalConvention.ToConvention(nazevTabulky);
        }
        nazevTabulkyJC = SH.ConvertPluralToSingleEn(nazevTabulky);
        string tableName = "TableRow" + nazevTabulky;
        return tableName;
    }
    public string GetNameTableRow(string nazevTabulky, string dbPrefix)
    {
        bool isDynamicTable = false;
        string dbPrefix2 = "";
        string nazevTabulkyJC = "";
        return GetNameTableRow(nazevTabulky, dbPrefix, out isDynamicTable, out nazevTabulkyJC, out dbPrefix2);
    }
    /// <summary>
    /// Generate only derived(TableRowAlbum)
    /// Do A2 může být vloženo i Nope_, on si jej automaticky nahradí za SE
    /// </summary>
    /// <param name="nazevTabulky"></param>
    /// <param name="dbPrefix"></param>
    /// <param name="tableName"></param>
    public string GetCsTableRow(bool signed, string nazevTabulky, string dbPrefix, out string tableName)
    {
        CSharpGenerator csg = new CSharpGenerator();

        // Zda první sloupec má jiný název než null
        bool isOtherColumnID = IsOtherColumnID;
        bool isDynamicTable = false;
        string dbPrefix2 = "";
        string nazevTabulkyJC = "";
        tableName = GetNameTableRow(nazevTabulky, dbPrefix, out isDynamicTable, out nazevTabulkyJC, out dbPrefix2);
        csg.Using(usings);
        //, "ITableRow<" + MSDatabaseLayer.ConvertSqlDbTypeToDotNetType(this[0].Type) + ">"
        string implements = tableName + "Base";
        string am = "public ";
        if (derived != null)
        {

            implements += "," + derived;
        }
        csg.StartClass(0, AccessModifiers.Public, false, tableName, implements);
        string seznamNameValue = "";
        List<string> nameFieldsFirstCharLower = new List<string>();
        List<string> allColumnsWithFirst = new List<string>();
        bool first = true;
        string sloupecID = null;
        string sloupecIDTyp = null;
        // Bude null, pokud sloupec nebude číselný typ
        Type typSloupecID = null;
        // Name of columns
        List<string> nameFields = new List<string>();
        // this is List<MSSloupecDB>
        foreach (MSSloupecDB item in this)
        {
            string typ = MSDatabaseLayer.ConvertSqlDbTypeToDotNetType(item.Type2);
            string name = item.Name;
            if (first)
            {
                first = false;
                if (name.StartsWith("ID") || name.StartsWith("Serie"))
                {
                    sloupecID = Copy(name);
                    sloupecIDTyp = typ;
                    typSloupecID = ConvertTypeNameTypeNumbers.ToType(typ);

                }
                else
                {
                    // Je to například IDMisters
                    throw new Exception("V prvním sloupci není řádek ID nebo ID*");
                }
            }
            else
            {
                // Používá se při insert,
                nameFields.Add(name);
                //nameFieldsFirstCharLower.Add(Copy(name));
            }
            allColumnsWithFirst.Add(name);
        }
        seznamNameValue = string.Join(',', allColumnsWithFirst.ToArray());
        string seznamNameValueBezPrvniho = string.Join(',', nameFields.ToArray());
        string nazvySloupcuBezPrvnihoVZavorkach = "(" + string.Join(',', nameFields.ToArray()) + ")";
        /*
         * TableName je již s TableRow, který slouží k vytváření K
         *
         * private static string NewMethod(string tableName,
         * bool isDynamicTable, string nazevTabulkyCopy, string, List<string> paramsForCtor)
         */
        List<string> paramsForCtor = new List<string>(this.Count * 2);
        foreach (MSSloupecDB item in this)
        {
            string typ = MSDatabaseLayer.ConvertSqlDbTypeToDotNetType(item.Type2);
            string name = item.Name;
            paramsForCtor.Add(typ);
            paramsForCtor.Add(name);
        }
        csg.Append(2, GenerateCtors(tableName, isDynamicTable, paramsForCtor, false, dbPrefix));

        #region Bez transakce

        CSharpGenerator innerSelectInTable = new CSharpGenerator();
        innerSelectInTable.AppendLine(2, "object[] o = MSStoredProceduresI.ci.SelectOneRow(TableName, \"" + sloupecID + "\", " + Copy(sloupecID) + ");" + @"
ParseRow(o);");
        csg.Method(1, "public void SelectInTable()", innerSelectInTable.ToString());
        string pridatDoNazvuMetody = "";
        if (sloupecIDTyp == "Guid")
        {
            pridatDoNazvuMetody = "Guid";
        }
        bool signed2 = false;
        if (signed)
        {
            if (typSloupecID != null)
            {
                signed2 = true;
            }
        }
        if (nameFields.Count == 0)
        {
            throw new Exception("Tabulka nemůže mít jen 1 sloupec.");
        }
        else
        {
            string pretypovaniInsert = "";
            if (typSloupecID != typeof(long))
            {
                pretypovaniInsert = "(" + sloupecIDTyp + ")";
            }
            CreateMethodInsert1(csg, am, sloupecID, typSloupecID, seznamNameValueBezPrvniho, signed2);
            string innerInsertToTable2 = CSharpGenerator.AddTab(2, sloupecID + "=(" + sloupecIDTyp + ")MSStoredProceduresI.ci.Insert2" + pridatDoNazvuMetody + "(TableName,\"" + sloupecID + "\",typeof(" + sloupecIDTyp + ")," + seznamNameValueBezPrvniho + ");");
            innerInsertToTable2 += CSharpGenerator.AddTab(2, "return " + sloupecID + ";");
            csg.Method(1, am + sloupecIDTyp + " InsertToTable2()", innerInsertToTable2);
            string innerInsertToTable3 = CSharpGenerator.AddTab(2, "MSStoredProceduresI.ci.Insert" + pridatDoNazvuMetody + "4(TableName, " + seznamNameValue + ");");
            csg.Method(1, am + "void InsertToTable3(" + sloupecIDTyp + " " + sloupecID + ")", innerInsertToTable3);
        }
        #region Metody které jsem odstranil aby dll nebyla tak velká
        if (derived == "ITableRowWordLong")
        {
            string nameVariable = "ID";
            csg.Property(1, AccessModifiers.Public, false, "long", SH.FirstCharLower(nameVariable), true, true, nameVariable);
            nameVariable = "Word";
            csg.Property(1, AccessModifiers.Public, false, "string", SH.FirstCharLower(nameVariable), true, true, nameVariable);
        }
        else if (derived == "ITableRowWordInt")
        {
            string nameVariable = "ID";
            csg.Property(1, AccessModifiers.Public, false, "int", SH.FirstCharLower(nameVariable), true, true, nameVariable);
            nameVariable = "Word";
            csg.Property(1, AccessModifiers.Public, false, "string", SH.FirstCharLower(nameVariable), true, true, nameVariable);
        }
        else if (derived == "ITableRowSearchIndexLong")
        {
            string nameVariable = "IDWord";
            csg.Property(1, AccessModifiers.Public, false, "long", SH.FirstCharLower(nameVariable), true, true, nameVariable);
            nameVariable = "TableChar";
            csg.Property(1, AccessModifiers.Public, false, "string", SH.FirstCharLower(nameVariable), true, true, nameVariable);
            nameVariable = "EntityID";
            csg.Property(1, AccessModifiers.Public, false, "int", SH.FirstCharLower(nameVariable), true, true, nameVariable);
        }
        else if (derived == "ITableRowSearchIndexInt")
        {
            string nameVariable = "IDWord";
            csg.Property(1, AccessModifiers.Public, false, "int", SH.FirstCharLower(nameVariable), true, true, nameVariable);
            nameVariable = "TableChar";
            csg.Property(1, AccessModifiers.Public, false, "string", SH.FirstCharLower(nameVariable), true, true, nameVariable);
            nameVariable = "EntityID";
            csg.Property(1, AccessModifiers.Public, false, "int", SH.FirstCharLower(nameVariable), true, true, nameVariable);
        }
        #endregion

        #endregion
        if (this.Count > 1)
        {
            MSSloupecDB sloupec = this[1];
            //&& !nazevTabulky.Contains("_")
            if (sloupec.Name == "Name" && !isDynamicTable)
            {
                if (this[0].Name == "ID")
                {
                    csg.Method(1, "public static string Get" + nazevTabulkyJC + "Name(" + sloupecIDTyp + " id)", CSharpGenerator.AddTab(2, "return MSStoredProceduresI.ci.SelectNameOfID(Tables." + nazevTabulky + ", id);"));
                }
                else
                {
                    //csg.Method("public static string Get" + nazevTabulkyJC + "Name(int id)", "return MSStoredProceduresI.ci.SelectValueOfIDOrSE(\"" + nazevTabulkyCopy + "\", id, \"" + this[0].Name + "\");");
                    csg.Method(1, "public static string Get" + nazevTabulkyJC + "Name(" + sloupecIDTyp + " id)", CSharpGenerator.AddTab(2, "return MSStoredProceduresI.ci.SelectNameOfID(Tables." + nazevTabulky + ", id,\"" + this[0].Name + "\");"));
                }
            }
        }
        csg.EndBrace(0);
        return csg.ToString();

    }

    const string TableRow = "TableRow";

    /// <param name="tableName"></param>
    /// <param name="isDynamicTable"></param>
    /// <param name="_tableNameField"></param>
    /// <param name="paramsForCtor"></param>
    private static string GenerateCtors(string tableName, bool isDynamicTable, List<string> paramsForCtor2, bool isBase, string dbPrefix)
    {
        dbPrefix = dbPrefix.TrimEnd('_');
        var tableNameWithoutTableRowAndBase = SHTrim.TrimEnd(tableName.Substring(TableRow.Length), "Base");

        List<string> paramsForCtor = new List<string>(paramsForCtor2);
        CSharpGenerator csg2 = new CSharpGenerator();
        CSharpGenerator ctor1Inner = new CSharpGenerator();
        ctor1Inner.AppendLine(3, "ParseRow(o);");
        if (isBase && isDynamicTable)
        {
            csg2.Ctor(1, ModifiersConstructor.Public, tableName, "");
        }

        // 1) všechny properties s object[]
        csg2.Ctor(1, ModifiersConstructor.Public, tableName, ctor1Inner.ToString(), "object[]", "o");
        List<string> paramsForCtorWithoutID = new List<string>();
        if (paramsForCtor.Count != 0)
        {
            paramsForCtorWithoutID = new List<string>(paramsForCtor);
            paramsForCtorWithoutID.RemoveAt(0);
            paramsForCtorWithoutID.RemoveAt(0);
        }

        for (int i = 0; i < paramsForCtorWithoutID.Count; i++)
        {
            if (i % 2 == 1)
            {
                paramsForCtorWithoutID[i] = Copy(paramsForCtorWithoutID[i]);
            }
        }

        // 2) bezparametrický ctor
        if (isDynamicTable)
        {
            csg2.Ctor(1, ModifiersConstructor.Public, tableName, true, isBase, "string", _tableNameField);
        }
        else
        {
            csg2.Ctor(1, ModifiersConstructor.Public, tableName, false, isBase);
        }

        // 3) všechny proměnné samostatně
        if (paramsForCtorWithoutID.Count != 0)
        {
            if (isDynamicTable)
            {
                paramsForCtorWithoutID.Insert(0, _tableNameField);
                paramsForCtorWithoutID.Insert(0, "string");
                csg2.Ctor(1, ModifiersConstructor.Public, tableName, true, isBase, paramsForCtorWithoutID.ToArray());
            }
            else
            {
                csg2.Ctor(1, ModifiersConstructor.Public, tableName, true, isBase, paramsForCtorWithoutID.ToArray());
            }
        }

        // 4) převod vybraných proměnných
        csg2.Ctor(1, ModifiersConstructor.Public, tableName, "//" + dbPrefix + "AO." + tableNameWithoutTableRowAndBase + "(columns, o)" + Environment.NewLine, new List<string>(["string", "columns", "object[]", "o"]).ToArray());

        return csg2.ToString();
    }
    /// <summary>
    /// Under canBeNull could be Nullable method
    /// Unfortunately actually I don't have time edit too
    /// </summary>
    /// <param name="p"></param>
    /// <param name="canBeNull"></param>
    /// <returns></returns>
    public static string ConvertSqlDbTypeToGetMethod(SqlDbType p, bool canBeNull)
    {
        switch (p)
        {
            case SqlDbType.Text:
            case SqlDbType.Char:
            case SqlDbType.NText:
            case SqlDbType.NChar:
            case SqlDbType.NVarChar:
            case SqlDbType.VarChar:
                return "GetString";

            case SqlDbType.Int:
                if (canBeNull)
                {
                    return "GetInt";
                }
                else
                {
                    return "GetInt";
                }

            case SqlDbType.Real:
                if (canBeNull)
                {
                    return "GetFloat";
                }
                else
                {
                    return "GetFloat";
                }

            case SqlDbType.BigInt:
                if (canBeNull)
                {
                    return "GetLong";
                }
                else
                {
                    return "GetLong";
                }

            case SqlDbType.Bit:
                if (canBeNull)
                {
                    return "GetBool";
                }
                else
                {
                    return "GetBool";
                }

            case SqlDbType.Date:
            case SqlDbType.DateTime:
            case SqlDbType.DateTime2:
            case SqlDbType.Time:
            case SqlDbType.DateTimeOffset:
            case SqlDbType.SmallDateTime:
                if (canBeNull)
                {
                    return "GetDateTime";
                }
                else
                {
                    return "GetDateTime";
                }
            // Bude to až po všech běžně používaných datových typech, protože bych se měl vyvarovat ukládat do malé DB takové množství dat
            case SqlDbType.Timestamp:
            case SqlDbType.Binary:
            case SqlDbType.VarBinary:
            case SqlDbType.Image:

                return "GetImage";


            case SqlDbType.SmallMoney:
            case SqlDbType.Money:
            case SqlDbType.Decimal:
            case SqlDbType.Float:
                if (canBeNull)
                {
                    return "GetDecimal";
                }
                else
                {
                    return "GetDecimal";
                }

            case SqlDbType.SmallInt:
                if (canBeNull)
                {
                    return "GetShort";
                }
                else
                {
                    return "GetShort";
                }

            case SqlDbType.TinyInt:
                if (canBeNull)
                {
                    return "GetByte";
                }
                else
                {
                    return "GetByte";
                }

            case SqlDbType.Structured:
            case SqlDbType.Udt:
            case SqlDbType.Xml:
                throw new Exception("Snažíte se převést na int strukturovaný(složitý) datový typ");
                return null;
            case SqlDbType.UniqueIdentifier:
                if (canBeNull)
                {
                    return "GetGuid";
                }
                else
                {
                    return "GetGuid";
                }
            case SqlDbType.Variant:
                return "GetObject";
            default:
                throw new Exception("Snažíte se převést datový typ, pro který není implementována větev");
                return null;

        }
    }

    public string GetCsEntityView(string table, string dbPrefix, string nameOfVariable)
    {
        CSharpGenerator csg = new CSharpGenerator();
        CSharpGenerator csgDisplayInfo = new CSharpGenerator();
        string nvc = (nameOfVariable);
        string nultyParametr = "";
        bool numero = false;
        foreach (MSSloupecDB item in this)
        {
            numero = false;
            nultyParametr = "";
            nameOfVariable = (nvc);
            switch (item.Type2)
            {
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.UniqueIdentifier:
                case SqlDbType.VarChar:
                    // Nechat tak, už by měl být string, akorát u UniqueIdentifier si toho nejsem úplně jistý ale to by šlo vylepšit.
                    nameOfVariable += "." + item.Name;
                    break;
                case SqlDbType.SmallInt:
                case SqlDbType.SmallMoney:
                case SqlDbType.Int:
                case SqlDbType.Money:
                case SqlDbType.Decimal:
                case SqlDbType.TinyInt:
                case SqlDbType.BigInt:
                    numero = true;
                    nameOfVariable = nameOfVariable + "." + item.Name;// + ".ToString()";
                    break;
                case SqlDbType.Char:
                    // Nechat tak, když se bude jednat o ID(ve většině případů) tak se to bude muset ještě projet nějakou metodou, protože Kompilátor to podtrhne a vyhodí chybu
                    nameOfVariable = nameOfVariable + "." + item.Name;// + ".ToString()";
                    break;
                case SqlDbType.Bit:
                    nameOfVariable += "." + item.Name;
                    // Nechat tak, kompilátor vyhodí chybu, uživatel musí nastavit vlastní metodu
                    break;
                case SqlDbType.Date:
                    nultyParametr = nameOfVariable + "." + item.Name + ",";
                    nameOfVariable = nameOfVariable + "." + item.Name + ".ToShortDateString()";
                    break;
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.SmallDateTime:
                    nultyParametr = nameOfVariable + "." + item.Name + ",";
                    nameOfVariable = nameOfVariable + "." + item.Name + ".ToString()";
                    break;
                case SqlDbType.Time:
                    nultyParametr = nameOfVariable + "." + item.Name + ",";
                    nameOfVariable = nameOfVariable + "." + item.Name + ".ToShortTimeString()";
                    break;
                case SqlDbType.DateTimeOffset:
                case SqlDbType.Timestamp:
                    throw new Exception("Datový typ DateTimeOffset a Timestamp není podporován.");
                    return null;
                case SqlDbType.Real:
                case SqlDbType.Float:
                    nameOfVariable = nameOfVariable + "." + item.Name + ".ToString()";
                    break;
                case SqlDbType.Image:
                case SqlDbType.Binary:
                case SqlDbType.VarBinary:
                    throw new Exception("Not supported convert binary data to string");
                    return null;
                case SqlDbType.Structured:
                    throw new Exception("Strukturované datové typy nejsou podporovány.");
                    return null;
                case SqlDbType.Udt:
                    throw new Exception("Univerzální datové typy nejsou podporovány.");
                    return null;
                case SqlDbType.Variant:
                    throw new Exception("Variantní datové typy nejsou podporovány.");
                    return null;
                case SqlDbType.Xml:
                    throw new Exception("Xml datový typ není podporován");
                    return null;
                default:
                    break;
            }
            if (nultyParametr == "")
            {
                if (numero)
                {
                    csgDisplayInfo.If(3, nameOfVariable + " != -1");
                }
                csgDisplayInfo.AppendLine(0, CSharpGenerator.AddTab(4, "SetP(" + nameOfVariable + ", lbl" + item.Name + ", p" + item.Name + ");"));
                if (numero)
                {
                    csgDisplayInfo.EndBrace(3);
                    csgDisplayInfo.Else(3);
                    csgDisplayInfo.AppendLine(0, CSharpGenerator.AddTab(4, "p" + item.Name + ".Visible = false;"));
                    csgDisplayInfo.EndBrace(3);
                }
            }
            else
            {
                csgDisplayInfo.AppendLine(3, "SetPDateTime(" + nultyParametr + nameOfVariable + ", lbl" + item.Name + ", p" + item.Name + ");");
            }
        }
        csg.Method(2, "public void DisplayInfo()", csgDisplayInfo.ToString());
        csg.Method(2, "private void SetP(string p, HtmlGenericControl lblName, HtmlGenericControl pName)",
CSharpGenerator.AddTab(3, @"string t = p.Trim();
        if (t != " + "\"\"" + @")
        {
            lblName.InnerHtml = t;
            pName.Visible = true;
        }
        else
        {
            pName.Visible = false;
        }"));
        csg.Method(2, "protected void SetPDateTime(DateTime dt, string p, HtmlGenericControl lblName, HtmlGenericControl pName)",
CSharpGenerator.AddTab(3, @"if ((dt.Day == 31 && dt.Month == 12 && dt.Year == 9999) || (dt.Hour == 23 || dt.Minute == 59))
{
            pName.Visible = false;
            return;
        }
        string t = p.Trim();
        if (t != " + "\"\"" + @")
        {
            lblName.InnerHtml = t;
            pName.Visible = true;
        }
        else
        {
            pName.Visible = false;
        }"));
        csg.Method(2, "private void SetVisible(bool b)", CSharpGenerator.AddTab(3, @"divButtons.Visible = b;
        h1.Visible = b;
        entityInfo.Visible = b;"));
        return csg.ToString();
    }
    public string GetHtmlEntityView(string table, string dbPrefix)
    {
        HtmlGenerator hg = new HtmlGenerator();
        hg.WriteTagWithAttrs("h1", "id", "h1", "runat", "server");
        hg.TerminateTag("h1");
        hg.WriteTagWithAttrs("div", "class", "tl");
        hg.WriteTagWithAttrs("div", "runat", "server", "id", "divButtons");
        hg.StartComment();
        hg.WriteNonPairTagWithAttrs("asp:Button", "runat", "server", "Text", "", "CssClass", "button", "ID", "btnChs");
        hg.EndComment();
        hg.TerminateTag("div");
        hg.WriteTagWithAttrs("div", "runat", "server", "id", "entityInfo");
        foreach (MSSloupecDB item in this)
        {
            string n = item.Name;
            hg.WriteTagWithAttrs("p", "id", "p" + n, "runat", "server");
            hg.WriteElement("b", "abc");
            hg.WriteBr();
            hg.WriteTagWithAttrs("span", "runat", "server", "id", "lbl" + n);
            hg.TerminateTag("span");
            hg.TerminateTag("p");
        }
        hg.TerminateTag("div");
        hg.TerminateTag("div");
        return hg.ToString();
    }
    /// <summary>
    /// Must call Close after return SqlCommand make work
    /// </summary>
    /// <param name="nazevTabulky"></param>
    /// <param name="cs"></param>
    public SqlCommand GetSqlCreateTable(string nazevTabulky, string cs)
    {
        return GetSqlCreateTable(nazevTabulky, false, cs);
    }
    public static MSColumnsDB IDName(int p)
    {
        return IDNameWorker(p, SqlDbType2.NVarChar);
    }

    public static MSColumnsDB IDNameVarChar(int p)
    {
        return IDNameWorker(p, SqlDbType2.VarChar);
    }

    public static MSColumnsDB IDNameChar(int p)
    {
        return IDNameWorker(p, SqlDbType2.Char);
    }

    public static MSColumnsDB IDNameNChar(int p)
    {
        return IDNameWorker(p, SqlDbType2.NChar);
    }

    public static MSColumnsDB IDNameWorker(int p, SqlDbType2 tName)
    {
        return IDNameWorker(p, tName, SqlDbType2.Int);
    }

    public static MSColumnsDB IDNameWorker(int p, SqlDbType2 tName, SqlDbType2 tID)
    {
        return new MSColumnsDB(
            MSSloupecDB.CI(tID, "ID", true),
            MSSloupecDB.CI(tName, "Name(" + p.ToString() + ")", false, true)
            );
    }

    public static MSColumnsDB IDNameShort(int p)
    {
        return new MSColumnsDB(
            MSSloupecDB.CI(SqlDbType2.SmallInt, "ID", true),
            MSSloupecDB.CI(SqlDbType2.NVarChar, "Name(" + p.ToString() + ")", false, true)
            );
    }
    public static MSColumnsDB IDNameTinyInt(int p)
    {
        return new MSColumnsDB(
            MSSloupecDB.CI(SqlDbType2.TinyInt, "ID", true),
            MSSloupecDB.CI(SqlDbType2.NVarChar, "Name(" + p.ToString() + ")", false, true)
            );
    }
    public static MSColumnsDB IntInt(string c1, string c2)
    {
        return new MSColumnsDB(
            MSSloupecDB.CI(SqlDbType2.Int, c1),
            MSSloupecDB.CI(SqlDbType2.Int, c2)
        );
    }
}

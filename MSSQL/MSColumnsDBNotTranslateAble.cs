
namespace SunamoSqlServer.MSSQL;
using Case.NET;
using SunamoDevCode;
using SunamoDevCode.Enums;


public partial class MSColumnsDB : List<MSSloupecDB>
{

    static string usings = @"using System;
";
    private static void CreateMethodInsert1(CSharpGenerator csg, string am, string sloupecID, Type typSloupecID, string seznamNameValueBezPrvniho, bool signed2)
    {
        string typSloupecIDS = MSDatabaseLayer.ConvertObjectToDotNetType(typSloupecID);
        string innerInsertToTable = "";
        if (!signed2)
        {
            innerInsertToTable = CSharpGenerator.AddTab(2, sloupecID + " = (" + typSloupecIDS + ")MSStoredProceduresI.ci.Insert(TableName, typeof(" + typSloupecIDS + "),\"" + sloupecID + "\"," + string.Join(',', seznamNameValueBezPrvniho) + @");
            return " + sloupecID + ";");
        }
        else
        {
            innerInsertToTable = CSharpGenerator.AddTab(2, sloupecID + " = (" + typSloupecIDS + ")MSStoredProceduresI.ci.InsertSigned(TableName, typeof(" + typSloupecIDS + "),\"" + sloupecID + "\"," + seznamNameValueBezPrvniho + @");
            return " + sloupecID + ";");
        }
        csg.Method(1, am + typSloupecIDS + " InsertToTable()", innerInsertToTable);
    }

    /// <summary>
    /// Do A2 může být vloženo i Nope_, on si jej automaticky nahradí za SE
    /// </summary>
    /// <param name="nazevTabulky"></param>
    /// <param name="dbPrefix"></param>
    /// <param name="tableName"></param>
    public string GetCsTableRow4(string nazevTabulky, string dbPrefix, out string tableName)
    {
        string dbPrefix2 = dbPrefix;
        if (dbPrefix2 == "Nope_")
        {
            dbPrefix2 = "";
        }
        if (nazevTabulky.StartsWith(dbPrefix))
        {
            nazevTabulky = nazevTabulky.Substring(dbPrefix.Length);
        }
        bool isDynamicTable = false;
        if (nazevTabulky.Contains("_"))
        {
            isDynamicTable = true;
            nazevTabulky = CaseConverter.PascalCase.ConvertCase(nazevTabulky); // ConvertPascalConvention.ToConvention(nazevTabulky);
        }
        tableName = "TableRow" + nazevTabulky + "4";
        List<string> paramsForCtor = new List<string>(this.Count * 2);
        foreach (MSSloupecDB item in this)
        {
            string typ = MSDatabaseLayer.ConvertSqlDbTypeToDotNetType(item.Type2);
            string name = item.Name;
            paramsForCtor.Add(typ);
            paramsForCtor.Add(name);
        }
        List<string> nameFields = new List<string>();
        List<string> temp = new List<string>();
        bool first = true;
        string sloupecID = null;
        string sloupecIDTyp = null;
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
                }
                else
                {
                    // Je to například IDMisters
                    throw new Exception("V prvním sloupci není řádek ID nebo ID*");
                }
            }
            else
            {
                nameFields.Add(Copy(name));
                // Používá se při update
                temp.Add("\"" + name + "\"");
                temp.Add(name);
            }
        }
        string seznamNameValue = string.Join(',', temp.ToArray());
        bool isOtherColumnID = IsOtherColumnID;
        CSharpGenerator csg = new CSharpGenerator();
        csg.Using(usings);
        string tableName2 = "TableRow" + nazevTabulky;
        csg.StartClass(0, AccessModifiers.Public, false, tableName, tableName2 + "Base");
        csg.Append(2, GenerateCtors(tableName, isDynamicTable, paramsForCtor, false, dbPrefix));
        CSharpGenerator innerSelectInTable = new CSharpGenerator();
        innerSelectInTable.AppendLine(2, "o = MSStoredProceduresI.ci.SelectOneRow(TableName, \"" + sloupecID + "\", " + Copy(sloupecID) + ");" + @");
ParseRow(o);");
        csg.Method(1, "public void SelectInTable()", innerSelectInTable.ToString());
        if (sloupecIDTyp == "int")
        {
            if (isOtherColumnID)
            {
                string innerInsertToTable = CSharpGenerator.AddTab(2, sloupecID + " = MSTSP.ci.InsertToRow2(trans,TableName,\"" + sloupecID + "\"," + string.Join(',', nameFields.ToArray()) + @");
            return " + sloupecID + ";");
                csg.Method(1, "public " + sloupecIDTyp + " InsertToTable(SqlTransaction trans)", innerInsertToTable);
            }
            else
            {
                string innerInsertToTable = CSharpGenerator.AddTab(2, sloupecID + @" = MSTSP.ci.InsertToRow(trans,TableName," + string.Join(',', nameFields.ToArray()) + @");
            return " + sloupecID + ";");
                csg.Method(1, "public " + sloupecIDTyp + " InsertToTable(SqlTransaction trans)", innerInsertToTable);
            }
        }
        else
        {
            if (isOtherColumnID)
            {
                string innerInsertToTable = CSharpGenerator.AddTab(2, sloupecID + " = (" + sloupecIDTyp + ")MSTSP.ci.InsertToRow2(trans,TableName,\"" + sloupecID + "\"," + string.Join(',', nameFields.ToArray()) + @");
            return " + sloupecID + ";");
                csg.Method(1, "public " + sloupecIDTyp + " InsertToTable(SqlTransaction trans)", innerInsertToTable);
            }
            else
            {
                string innerInsertToTable = CSharpGenerator.AddTab(2, sloupecID + @" = (" + sloupecIDTyp + ")MSTSP.ci.InsertToRow(trans,TableName," + string.Join(',', nameFields.ToArray()) + @");
            return " + sloupecID + ";");
                csg.Method(1, "public " + sloupecIDTyp + " InsertToTable(SqlTransaction trans)", innerInsertToTable);
            }
        }
        string innerInsertToTable3 = CSharpGenerator.AddTab(2, "MSTSP.ci.Insert3(trans,TableName, " + sloupecID + "," + string.Join(',', nameFields.ToArray()) + ");");
        csg.Method(1, "public void InsertToTable3(SqlTransaction trans)", innerInsertToTable3);
        csg.EndBrace(0);
        return csg.ToString();
    }
}


namespace SunamoSqlServer.MSSQL;
using SunamoData.Data;
using SunamoEnums.Enums;
using SunamoExceptions.OnlyInSE;
using SunamoShared.Helpers.Text;
using SunamoTextBuilder;
using SunamoValues;


public partial class GeneratorMsSql
{
    public static string CombinedWhere(ABC where, ABC isNotWhere, ABC greaterThanWhere, ABC lowerThanWhere, ABC whereOr = null)
    {
        return GeneratorMsSqlWorker.CombinedWhere(true, where, isNotWhere, greaterThanWhere, lowerThanWhere, whereOr);
    }

    public static SqlCommand CombinedWhereCommand(string commandBeforeWhere, ABC where, ABC isNotWhere, ABC greaterThanWhere, ABC lowerThanWhere, string orderBy)
    {
        return GeneratorMsSqlWorker.CombinedWhereCommand(true, commandBeforeWhere, where, isNotWhere, greaterThanWhere, lowerThanWhere, orderBy);
    }

    public static object MaxValueForSqlDbType2(SqlDbType2 t)
    {
        switch (t)
        {
            // prvně číselné typy
            case SqlDbType2.Int:
                return int.MaxValue;
            case SqlDbType2.BigInt:
                return long.MaxValue;
            case SqlDbType2.TinyInt:
                return byte.MaxValue;
            case SqlDbType2.Real:
                return float.MaxValue;
            case SqlDbType2.SmallInt:
                return short.MaxValue;
            case SqlDbType2.Decimal:
                return decimal.MaxValue;

            // date
            case SqlDbType2.DateTime2:
                return DateTime.MaxValue;
            case SqlDbType2.Date:
                return DateTime.MaxValue;
            case SqlDbType2.SmallDateTime:
                return Consts.DateTimeMaxVal;


            case SqlDbType2.Bit:
                return false;


            // typy u kterých si nejsem jistý jestli vrátit null nebo nějakou non null hondotu
            // zatím budu vracet null protože to užívám v FillWithDefaultOrMaxValues
            // a když bude null, vyhodí mi to chybu, pokusím li se s tím manipulovat
            case SqlDbType2.NVarChar:
                return null;
            case SqlDbType2.NChar:
                return null;
            case SqlDbType2.Binary:
                return null;
            case SqlDbType2.VarChar:
                return null;
            case SqlDbType2.Char:
                return char.MaxValue;


            // na závěr ty které nemějí max value
            case SqlDbType2.UniqueIdentifier:
            case SqlDbType2.UniqueIdentifierAutoNewId:
                return Guid.Empty;

            default:
                ThrowEx.NotImplementedCase(t);
                return null;
        }
    }

    /// <summary>
    /// otázka je jestli chci užívat MinValue
    /// mohl bych užívat i default který pro typ vrací c#
    /// vždycky jsem si zakládal na co nejšetrnějším používaní dat, tedy začínalo se na MinValue
    /// Proto se mi default nehodí, protože z toho nepoznám jestli to je reálná hodnota nebo vyjadřuje že nebyla nastavená
    /// 
    /// To že bych měl pro každou hodnotu bool zda byla nastavená se mi zdá že mnoho kódu pro nic
    /// 
    /// 
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static object MinValueForSqlDbType2(SqlDbType2 t)
    {
        switch (t)
        {
            case SqlDbType2.Int:
                return int.MinValue;
            case SqlDbType2.BigInt:
                return long.MinValue;
            case SqlDbType2.DateTime2:

            case SqlDbType2.UniqueIdentifier:

            case SqlDbType2.UniqueIdentifierAutoNewId:

            case SqlDbType2.Date:

            case SqlDbType2.SmallDateTime:

            case SqlDbType2.Real:


            case SqlDbType2.NVarChar:

            case SqlDbType2.NChar:

            case SqlDbType2.Bit:

            case SqlDbType2.TinyInt:

            case SqlDbType2.SmallInt:

            case SqlDbType2.Binary:


            case SqlDbType2.Decimal:

            case SqlDbType2.VarChar:

            case SqlDbType2.Char:

            default:
                ThrowEx.NotImplementedCase(t);
                break;
        }

        return null;
    }

    /// <summary>
    /// A3 pokud nechci aby se mi vytvářeli reference na ostatní tabulky. Vhodné při testování tabulek a programů, kdy je pak ještě budu mazat a znovu plnit.
    /// </summary>
    /// <param name="var"></param>
    /// <param name="inTable"></param>
    /// <param name="dynamicTables"></param>
    private static string Column(MSSloupecDB var, string inTable, bool dynamicTables)
    {
        InstantSB sb = new InstantSB(AllStrings.space);

        sb.AddItem(var.Name);
        sb.AddItem((var.Type + var.Delka));
        var t = var.Type;
        /*Tyto typy které používám nemůžou obsahovat text a collace je u nich zakázána
         * t == System.Data.SqlDbType.BigInt || 
         * t == System.Data.SqlDbType.Int ||
         * t == System.Data.SqlDbType.Bit || 
         * t == System.Data.SqlDbType.TinyInt || 
         * t == System.Data.SqlDbType.SmallInt || 
         * t == System.Data.SqlDbType.UniqueIdentifier || 
         * t == System.Data.SqlDbType.Date || 
         * t == System.Data.SqlDbType.SmallDateTime || 
         * t == System.Data.SqlDbType.Real ||  
         * t == System.Data.SqlDbType.Binary ||
         * t == System.Data.SqlDbType.Decimal ||
         */
        if (
            t == SqlDbType2.VarChar ||
            t == SqlDbType2.Char ||
            t == SqlDbType2.NVarChar ||
            t == SqlDbType2.NChar
            )
        {
            // Musí to být AI, protože když bych měl slovo è, SQL Server by mi vrátil že neexistuje ale když bych ho chtěl vložit, udělal by z něho "e" a vrátil by chybu
            sb.AddItem("COLLATE Czech_CS_AS_KS_WS");
        }

        if (!var.CanBeNull)
        {
            sb.AddItem("NOT NULL");
        }
        if (var.PrimaryKey || var.MustBeUnique)
        {
            if (var.identityIncrementBy1)
            {
                sb.AddItem("IDENTITY (" + (var.IsSigned == Signed.Signed ? MinValueForSqlDbType2(var.typ) : 0) + ",1)");
            }

            if (var.PrimaryKey)
            {
                sb.AddItem("PRIMARY KEY");
            }
            else
            {
                sb.AddItem("UNIQUE");
            }
        }
        if (var.IsNewId)
        {
            sb.AddItem("DEFAULT(newid())");
            //sb.AddItem("DEFAULT newsequentialid()");
        }
        if (!dynamicTables)
        {
            if (var.referencesTable != null)
            {
                sb.AddItem("CONSTRAINT");
                sb.AddItem(("fk_" + var.Name + AllStrings.lowbar + inTable + AllStrings.lowbar + var.referencesTable + AllStrings.lowbar + var.referencesColumn));
                sb.AddItem("FOREIGN KEY REFERENCES");
                sb.AddItem((var.referencesTable + AllStrings.lb + var.referencesColumn + AllStrings.rb));
            }
        }


        return sb.ToString();
    }



    /// <summary>
    /// A3 - whether is not desirable to create references to other tables. Good while test tables and apps, when I will it delete later.
    /// </summary>
    /// <param name="table"></param>
    /// <param name="sloupce"></param>
    /// <param name="dynamicTables"></param>
    /// <returns></returns>
    public static string SqlForCreateTable(string table, MSColumnsDB sloupce, bool dynamicTables)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("CREATE TABLE {0}(", table);
        foreach (MSSloupecDB var in sloupce)
        {
            sb.Append(GeneratorMsSql.Column(var, table, dynamicTables) + AllStrings.comma);
        }
        string dd = sb.ToString();
        dd = dd.TrimEnd(AllChars.comma);
        string vr = dd + AllStrings.rb;
        //vr);
        return vr;
    }

    /// <summary>
    /// Může vrátit null když tabulka bude existovat
    /// Výchozí pro A3 je true
    /// A3 - whether is not desirable to create references to other tables. Good while test tables and apps, when I will it delete later.
    /// </summary>
    /// <param name="table"></param>
    /// <param name="sloupce"></param>
    /// <param name="p"></param>
    public static string CreateTable(string table, MSColumnsDB sloupce, bool dynamicTables, SqlConnection conn)
    {
        //var sloupce = (MSColumnsDB)sloupce2;
        bool exists = MSStoredProceduresI.ci.SelectExistsTable(table, conn);
        if (!exists)
        {
            return SqlForCreateTable(table, sloupce, dynamicTables);
        }
        return null;
    }

    /// <summary>
    /// Na začátek přidá where pokud obsahuje A1 obsahoval nějaké prvky
    /// </summary>
    /// <param name="where"></param>
    public static string CombinedWhere(ABC where)
    {
        int pridavatOd = 0;
        return CombinedWhere(where, ref pridavatOd);
    }

    /// <summary>
    /// Anything of args can be null
    /// Na začátek přidá where pokud obsahuje A1 obsahoval nějaké prvky
    /// </summary>
    /// <param name="where"></param>
    /// <param name="pridavatOd"></param>
    public static string CombinedWhere(ABC where, ref int pridavatOd)
    {
        StringBuilder sb = new StringBuilder();
        if (where != null)
        {
            if (where.Length > 0)
            {
                sb.Append(" " + "WHERE" + " ");
            }

            bool první = true;


            foreach (AB var in where)
            {
                if (první)
                {
                    první = false;
                }
                else
                {
                    sb.Append(" AND ");
                }
                sb.Append(string.Format(" {0} = {1} ", var.A, "@p" + pridavatOd));
                pridavatOd++;
            }

        }
        return sb.ToString();
    }
}

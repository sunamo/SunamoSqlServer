

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunamoSqlServer._sunamo;

internal class SH
{
    internal static string ConvertPluralToSingleEn(string nazevTabulky)
    {
        if (nazevTabulky[nazevTabulky.Length - 1] == 's')
        {
            if (nazevTabulky[nazevTabulky.Length - 2] == 'e')
            {
                if (nazevTabulky[nazevTabulky.Length - 3] == 'i')
                {
                    return nazevTabulky.Substring(0, nazevTabulky.Length - 3) + "y";
                }
            }
            return nazevTabulky.Substring(0, nazevTabulky.Length - 1);
        }

        return nazevTabulky;
    }
    internal static string FirstCharLower(string nazevPP)
    {
        if (nazevPP.Length < 2) return nazevPP;

        var sb = nazevPP.Substring(1);
        return nazevPP[0].ToString().ToLower() + sb;
    }

    internal static string GetFirstWord(string p, bool returnEmptyWhenDontHaveLenght = true)
    {
        p = p.Trim();
        int dex = p.IndexOf(AllCharsSE.space);
        if (dex != -1)
        {
            return p.Substring(0, dex);
        }

        if (returnEmptyWhenDontHaveLenght)
        {
            return string.Empty;
        }
        return p;
    }
}

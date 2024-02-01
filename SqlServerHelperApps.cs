
namespace SunamoSqlServer;
using SunamoSqlServer._sunamo;


/// <summary>
/// For sharing with Apps
/// </summary>
public partial class SqlServerHelper
{
    /// <summary>
    /// Tato hodnota byla založena aby používal všude v DB konzistentní datovou hodnotu, klidně může mít i hodnotu DT.MaxValue když to tak má být
    /// </summary>
    public static readonly DateTime DateTimeMinVal = new DateTime(1900, 1, 1);
    public static readonly DateTime DateTimeMaxVal = new DateTime(2079, 6, 6);
    //public static List<char> s_availableCharsInVarCharWithoutDiacriticLetters = null;




    public static string ConvertToVarCharPathOrFileName(string maybeUnicode)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in maybeUnicode)
        {
            {
                sb.Append(item);
            }
        }

        string vr = SHReplace.ReplaceAll(sb.ToString(), AllStrings.space, AllStrings.doubleSpace).Trim();
        vr = SHReplace.ReplaceAll(vr, AllStrings.bs, " \\");
        vr = SHReplace.ReplaceAll(vr, AllStrings.bs, "\\ ");
        vr = SHReplace.ReplaceAll(vr, AllStrings.slash, "/ ");
        vr = SHReplace.ReplaceAll(vr, AllStrings.slash, " /");
        return vr;
    }
}


namespace SunamoSqlServer;
using SunamoExceptions.OnlyInSE;


public partial class SqlServerHelper
{


    public static T EmptyNonSigned<T>() where T : struct
    {
        var t = typeof(T);

        if (t == Types.tShort)
        {
            short v = -1;
            return (T)(dynamic)v;
        }
        if (t == Types.tInt)
        {
            short v = -1;
            return (T)(dynamic)v;
        }
        ThrowEx.NotImplementedCase(t);
        return default(T);
    }












}

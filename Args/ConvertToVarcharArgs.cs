
namespace SunamoSqlServer.Args;
using SunamoCollectionWithoutDuplicates;


public class ConvertToVarcharArgs
{
    public CollectionWithoutDuplicates<char> notSupportedChars = new CollectionWithoutDuplicates<char>();
    public Dictionary<string, List<string>> changed = new Dictionary<string, List<string>>();
}

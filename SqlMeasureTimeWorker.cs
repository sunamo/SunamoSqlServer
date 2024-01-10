namespace SunamoSqlServer;

public class SqlMeasureTimeWorker
{
    public static event Action NeedNewFile;
    public static int writtenLines = 0;
    public static StreamWriter swSqlLog;

    public static void IncrementWrittenLines()
    {
        writtenLines++;
        if (writtenLines > 5000)
        {
            if (MSStoredProceduresI.measureTime)
            {
                NeedNewFile();
            }
        }
    }

    public static SqlMeasureTimeHelper mt = new SqlMeasureTimeHelper();

    public static void Init()
    {
        #region Must be after set up ThisApp.Name
        if (MSStoredProceduresI.measureTime)
        {
            mt.NewSw();
            NeedNewFile += ThisApp_NeedNewFile;
        }
        #endregion
    }

    private static void ThisApp_NeedNewFile()
    {
        mt.NewSw();
    }
}

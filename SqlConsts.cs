namespace SunamoSqlServer;

public class SqlConsts
{
    public const int timeout = 950;
    public const string dbo = "dbo";
    public const string delete = "delete";
    public const string update = "update";
    public const string insert = "insert";
    public const string select = "select";
    public const string from = "from";
    public const string top1 = "top(1)";

}

public class SqlMeasureTimeHelper
{
    bool mt = false;
    int waitMs = 0;
    bool forceIsVps = false;
    static string fn = null;

    public void Before(bool mt, int waitMs, bool forceIsVps)
    {
        mt = MSStoredProceduresI.measureTime;
        waitMs = MSStoredProceduresI.waitMs;
        forceIsVps = MSStoredProceduresI.forceIsVps;

        MSStoredProceduresI.measureTime = true;
        MSStoredProceduresI.waitMs = 0; //StopwatchStaticSql.maxMs + 100;
        MSStoredProceduresI.forceIsVps = true;

        NewSw();
    }

    public void Before(string fn2 = "Sql.txt")
    {
        fn = fn2;

        Before(mt, waitMs, forceIsVps);
    }

    static Type type = typeof(Type);

    public void NewSw()
    {
        if (!MSStoredProceduresI.measureTime)
        {
            throw new Exception("MSStoredProceduresI.measureTime must be true to run NewSw");
        }

        if (/*VpsHelperSunamo.IsVps ||*/ MSStoredProceduresI.forceIsVps)
        {
            var bufferInKb = 65536 / 8; //8192
            string pathSqlLog = PathSqlLog();
            StreamWriter sw = null;
            while (true)
            {
                try
                {
                    sw = new StreamWriter(pathSqlLog, true, Encoding.Default, bufferInKb);

                    break;
                }
                catch (Exception ex)
                {
                    pathSqlLog = PathSqlLog();
                }
            }
            SqlMeasureTimeWorker.swSqlLog = sw;
            SqlMeasureTimeWorker.swSqlLog.AutoFlush = true;
            SqlMeasureTimeWorker.writtenLines = 0;
        }
    }

    private string PathSqlLog()
    {
        throw new NotImplementedException();
        //var dt = DateTime.Now;
        //string r = Directory.GetFiles(AppData.ci.GetFolder(AppFolders.Logs), fn + $"{dt.Year}-{dt.Month}-{dt.Day}", ".txt");
        //return r;
    }



    public void After()
    {
        MSStoredProceduresI.waitMs = waitMs;
        MSStoredProceduresI.measureTime = mt;
        MSStoredProceduresI.forceIsVps = forceIsVps;
    }
}

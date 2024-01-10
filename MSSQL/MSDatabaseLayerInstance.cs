namespace SunamoSqlServer.MSSQL;

public class MSDatabaseLayerInstance : IDatabaseLayer<SqlDbType2>
{
    public Dictionary<SqlDbType2, string> usedTa { get => MSDatabaseLayer.usedTa; set => MSDatabaseLayer.usedTa = value; }
    public Dictionary<SqlDbType2, string> hiddenTa { get => MSDatabaseLayer.hiddenTa; set => MSDatabaseLayer.hiddenTa = value; }
}

namespace BAL.Utils
{
    public interface IUtils
    {
        List<string> GetTableNamesFromConnectionString();
        List<object> GetAllTablesDataAndStoreToList();
    }
}
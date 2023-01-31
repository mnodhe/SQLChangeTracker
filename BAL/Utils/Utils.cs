using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace BAL.Utils
{
    public class Utils : IUtils
    {
        private readonly IConfiguration _configuration;

        public Utils(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<string> GetTableNamesFromConnectionString()
        {
            SqlConnection con = new SqlConnection(_configuration["ConnectionString"].ToString());
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", con);
            SqlDataReader dr = cmd.ExecuteReader();
            List<string> data = new List<string>();
            if (dr.HasRows == true)
            {
                while (dr.Read())
                {
                    data.Add(dr[0].ToString());
                }
            }
            if (!dr.IsClosed)
            {
                dr.Close();
            }
            if (con != null)
            {
                con.Close();
            }
            return data;
        }

        public List<object> GetAllTablesDataAndStoreToList()
        {
            List<object> list = new List<object>();
            List<string> tableNameList = GetTableNamesFromConnectionString();
            foreach (var tableName in tableNameList)
            {
                var x = GetAllDataFromTableName(tableName);
                list.Add(x);
            }
            return list;
        }

        private Dictionary<string, object> GetAllDataFromTableName(string TableName)
        {
            SqlConnection con = new SqlConnection(_configuration["ConnectionString"].ToString());
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM " + TableName, con);
            SqlDataReader dr = cmd.ExecuteReader();
            //List<object> data = new List<object>();
            var tableData = new Dictionary<string, object>();
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            if (dr.HasRows == true)
            {
                while (dr.Read())
                {
                    // Dictionary to store data for each column
                    var row = new Dictionary<string, object>();

                    // Retrieve number of columns
                    int columnCount = dr.FieldCount;

                    // Loop through each column
                    for (int i = 0; i < columnCount; i++)
                    {
                        // Retrieve column name and value
                        string columnName = dr.GetName(i);
                        object columnValue = dr.GetValue(i);

                        // Add column name and value to dictionary
                        row.Add(columnName, columnValue);
                    }

                    // Add row data to list
                    data.Add(row);
                }
                // Dictionary to store table name and data

                // Add table name and data to dictionary
                tableData.Add("TableName", TableName);
                tableData.Add("Data", data);

            }
            if (!dr.IsClosed)
            {
                dr.Close();
            }
            if (con != null)
            {
                con.Close();
            }
            return tableData;
        }

    }
}

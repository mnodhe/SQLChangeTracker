using BAL.Utils;
using JsonDiffer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SQLChangeTracker.Controllers
{
    [Route("/api/v1")]
    [ApiController]
    public class TrackingController : Controller
    {
        private readonly IUtils _utils;
        public TrackingController(IUtils utils, IConfiguration configuration)
        {
            _utils = utils;
        }


        /// <summary>
        /// This Api Is For Checking if the System Is Connected to Database or Not
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllTableFromConnectionString")]
        public ActionResult GetAllTableFromConnectionString()
        {
            var x = _utils.GetTableNamesFromConnectionString();
            return Ok(x);
        }


        /// <summary>
        /// this APi created to Store The Database latest change
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllTablesDataAndStoreToList")]
        public ActionResult GetAllTablesDataAndStoreToFile()
        {
            //var last = [\r\n  {\r\n    \"TableName\": \"__EFMigrationsHistory\",\r\n    \"Data\": [\r\n      {\r\n        \"MigrationId\": \"20220501032454_init\",\r\n        \"ProductVersion\": \"6.0.4\"\r\n      },\r\n      {\r\n        \"MigrationId\": \"20220501033037_adduserdevicestbl\",\r\n        \"ProductVersion\": \"6.0.4\"\r\n      },\r\n      {\r\n        \"MigrationId\": \"20220501033155_changetblname\",\r\n        \"ProductVersion\": \"6.0.4\"\r\n      }\r\n    ]\r\n  },\r\n  {},\r\n  {\r\n    \"TableName\": \"Applications\",\r\n    \"Data\": [\r\n      {\r\n        \"Id\": 2,\r\n        \"Name\": \"app1\",\r\n        \"URL\": \"http://app1.com\",\r\n        \"PrivateInternalToken\": \"47as5d4as5d46as\"\r\n      },\r\n      {\r\n        \"Id\": 5,\r\n        \"Name\": \"app2\",\r\n        \"URL\": \"http://app2.com\",\r\n        \"PrivateInternalToken\": \"56a1sd654as65das6\"\r\n      }\r\n    ]\r\n  },\r\n  {},\r\n  {},\r\n  {},\r\n  {}\r\n];
            var TableNamesAndDataList = _utils.GetAllTablesDataAndStoreToList();
            var json = JsonConvert.SerializeObject(TableNamesAndDataList, Formatting.Indented);
            System.IO.File.WriteAllText("list.txt", json);

            return Ok(TableNamesAndDataList);
        }

        /// <summary>
        /// this API Show What Is Changed in DB
        /// </summary>
        /// <returns></returns>
        [HttpGet("WhatIsChangedInDB")]
        public ActionResult WhatIsChangedInDB()
        {
            // get old data
            string json = System.IO.File.ReadAllText("list.txt");
            List<object> lastdbmodel = JsonConvert.DeserializeObject<List<object>>(json);
            //get latest data
            var TableNamesAndDataList = JsonConvert.SerializeObject( _utils.GetAllTablesDataAndStoreToList());

            // find diffrences.
            var difference = TableNamesAndDataList.Where(x =>
            !lastdbmodel.Any(y => 
            JsonConvert.SerializeObject(y) == JsonConvert.SerializeObject(x)
            )).ToList();

            List<Dictionary<string, object>> diff= new List<Dictionary<string, object>>();


            var j1 = JToken.Parse(json);
            var j2 = JToken.Parse(TableNamesAndDataList);

            var diffs = JsonDifferentiator.Differentiate(j1, j2);
            var x = 1;
            //diff.Add("TableName", TableName);
            //diff.Add("Data", data);

            //update text file with new data model
            GetAllTablesDataAndStoreToFile();
            if(diffs is null)
            {
                return NotFound("No Diffrence Found");
            }
            return Ok(diffs.ToString());
        }
    }

}


using CatMash.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using CatMash.Domain.Enums;
using Dapper;
using System.Reflection;
using FastMember;

namespace CatMash.ConsoleTool
{
    class Program
    {
        private static readonly string connectionString =
            "Server=tcp:catmash-server.database.windows.net,1433;Initial Catalog=CatMash;Persist Security Info=False;User ID=dbadmin;Password=Azerty123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        static void Main(string[] args)
        {
            var cats = DesirializeCats();
            var furs = new List<string>();

            foreach (var value in Enum.GetValues(typeof(FurTypesEnum)))
            {
                furs.Add(value.ToString());
            }

            foreach (var cat in cats)
            {
                string fur = "";
                foreach (var item in cat.FurTypes)
                {
                    fur += $"{item};";
                }

                Console.WriteLine(cat.CatUrl);
                Console.WriteLine(fur);
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                connection.Query("CreateTables", commandType: CommandType.StoredProcedure);

                using (var bulkCopy = new SqlBulkCopy(connection))
                using (var reader = ObjectReader.Create(cats, "CatUrl", "IsAStar", "IsTopOne", "IsAlone", "Rating", "ViewsNumber", "ProbabilityWeight"))
                {
                    var catUrlMap = new SqlBulkCopyColumnMapping("CatUrl", "CatUrl");
                    bulkCopy.ColumnMappings.Add(catUrlMap);
                    var isAStarMap = new SqlBulkCopyColumnMapping("IsAStar", "IsAStar");
                    bulkCopy.ColumnMappings.Add(isAStarMap);
                    var isTopOneMap = new SqlBulkCopyColumnMapping("IsTopOne", "IsTopOne");
                    bulkCopy.ColumnMappings.Add(isTopOneMap);
                    var isAloneMap = new SqlBulkCopyColumnMapping("IsAlone", "IsAlone");
                    bulkCopy.ColumnMappings.Add(isAloneMap);
                    var ratingMap = new SqlBulkCopyColumnMapping("Rating", "Rating");
                    bulkCopy.ColumnMappings.Add(ratingMap);
                    var viewsNumberMap = new SqlBulkCopyColumnMapping("ViewsNumber", "ViewsNumber");
                    bulkCopy.ColumnMappings.Add(viewsNumberMap);
                    var probabilityWeightMap = new SqlBulkCopyColumnMapping("ProbabilityWeight", "ProbabilityWeight");
                    bulkCopy.ColumnMappings.Add(probabilityWeightMap);

                    bulkCopy.BatchSize = cats.Count();
                    bulkCopy.DestinationTableName = "Cats";
                    bulkCopy.WriteToServer(reader);
                }

                using (var bulkCopy = new SqlBulkCopy(connection))
                using (var reader = ObjectReader.Create(furs.Select(x => new { FurType = x }), "FurType"))
                {
                    var furTypeMap = new SqlBulkCopyColumnMapping("FurType", "FurType");
                    bulkCopy.ColumnMappings.Add(furTypeMap);

                    bulkCopy.BatchSize = furs.Count();
                    bulkCopy.DestinationTableName = "FurTypes";
                    bulkCopy.WriteToServer(reader);
                }

                var catsWithId = connection.Query<Cat>(@"SELECT Id, CatUrl, IsAStar, IsTopOne, IsAlone, Rating FROM Cats");

                cats.ToList().ForEach(x => x.Id = catsWithId.Where(y => y.CatUrl == x.CatUrl).Select(z => z.Id).FirstOrDefault());

                var association = new List<Tuple<int, int>>();
                foreach (var cat in cats)
                {
                    foreach (var type in cat.FurTypes)
                    {
                        association.Add(new Tuple<int, int>(cat.Id, (int)type));
                    }
                }

                using (var bulkCopy = new SqlBulkCopy(connection))
                using (var reader = ObjectReader.Create(association.Select(x => new { CatId = x.Item1, FurTypeId = x.Item2}), "CatId", "FurTypeId"))
                {
                    var catIdMap = new SqlBulkCopyColumnMapping("CatId", "CatId");
                    bulkCopy.ColumnMappings.Add(catIdMap);
                    var furTypeIdMap = new SqlBulkCopyColumnMapping("FurTypeId", "FurTypeId");
                    bulkCopy.ColumnMappings.Add(furTypeIdMap);

                    bulkCopy.BatchSize = association.Count();
                    bulkCopy.DestinationTableName = "CatsFurs";
                    bulkCopy.WriteToServer(reader);
                }
            }
        }

    private static IEnumerable<Cat> DesirializeCats()
    {
        var cats = new List<Cat>();
        using (StreamReader reader = new StreamReader("Cats.json"))
        {
            string json = reader.ReadToEnd();
            var parsedObject = JObject.Parse(json);
            var jtokens = parsedObject["Cats"].Children().ToList();

            foreach (var jtoken in jtokens)
            {
                var furs = new List<FurTypesEnum>();
                FurTypesEnum furEnum = FurTypesEnum.BiColor;
                string furList = jtoken["fur"].ToString();
                string[] items = furList.Split(';');
                foreach (var fur in items)
                {
                    Enum.TryParse(fur, out furEnum);
                    furs.Add(furEnum);
                }
                cats.Add(new Cat
                {
                    CatUrl = jtoken["url"].ToString(),
                    FurTypes = furs,
                    IsAlone = (items.Count() <= 1),
                    ProbabilityWeight = 0.99
                });
            }
        }
        return cats;
    }
}
}

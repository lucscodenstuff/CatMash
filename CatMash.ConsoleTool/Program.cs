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
            var furEnums = new List<FurTypesEnum>(); 

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

                connection.Execute(@"INSERT Cats(CatUrl,IsAStar,IsTopOne,IsAlone,Rating,ViewsNumber,ProbabilityWeight) VALUES (@catUrl,@isAStar,@isTopOne,@isAlone,@rating,@viewsNumber,@probabilityWeight)",
                    cats.Select(x => new
                    {
                        catUrl = x.CatUrl,
                        isAStar = x.IsAStar,
                        isTopOne = x.IsTopOne,
                        isAlone = x.IsAlone,
                        rating = x.Rating,
                        viewsNumber = x.ViewsNumber,
                        probabilityWeight = x.ProbabilityWeight,
                    }));

                connection.Execute(@"INSERT FurTypes(FurType) VALUES (@furTypes)",
                    furs.Select(x => new
                    {
                        furTypes = x
                    }));

                var catsWithId = connection.Query<Cat>(@"SELECT Id, CatUrl, IsAStar, IsTopOne, IsAlone, Rating FROM Cats");

                cats.ToList().ForEach(x => x.Id = catsWithId.Where(y => y.CatUrl == x.CatUrl).Select(z => z.Id).FirstOrDefault());

                var association = new List<Tuple<int, int>>();
                foreach (var cat in cats)
                {
                    foreach (var type in cat.FurTypes)
                    {
                        association.Add(new Tuple<int, int>(cat.Id, (int) type));
                    }
                }

                connection.Execute(@"INSERT CatsFurs(CatId, FurTypeId) VALUES (@catId, @furTypeId)",
                    association.Select(x => new
                    {
                        catId = x.Item1,
                        furTypeId = x.Item2
                    }));
            }
        }

        public static IEnumerable<Cat> DesirializeCats()
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

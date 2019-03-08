using CatMash.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private readonly static string connectionString =
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

            Console.ReadLine();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Execute(@"INSERT Cats(CatUrl,IsAStar,IsTopOne,IsAlone,Rating) VALUES (@catUrl,@isAStar,@isTopOne,@isAlone,@rating)",
                    cats.Select(x => new
                    {
                        catUrl = x.CatUrl,
                        isAStar = x.IsAStar,
                        isTopOne = x.IsTopOne,
                        isAlone = x.IsAlone,
                        rating = x.Rating
                    }));
                connection.Execute(@"INSERT FurTypes(FurType) VALUES (@furTypes)",
                    furs.Select(x => new
                    {
                        furTypes = x
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
                        IsAlone = (items.Count() > 1) ? false : true
                    });
                }
            }
            return cats;
        }
    }
}

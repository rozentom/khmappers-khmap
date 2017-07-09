using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using khmap.DataBaseProviders;
using khmap.Models;
using MongoDB.Bson;

namespace khmapTests.UnitTests
{
    [TestClass]
    public class SearchUnitTest
    {
        private MapDB _mapManeger;
        private Random _rnd;

        [TestInitialize]
        public void setUp()
        {
            _rnd = new Random();
            _mapManeger = new MapDB(new Settings());
            int mapsAmmount = _rnd.Next(1, 21);
            for(int i=0; i< mapsAmmount; i++)
            {
                Map map = randomMap();
                _mapManeger.AddMap(map);
            }
        }

        [TestCleanup]
        public void cleanUp()
        {
            cleanMaps();
        }

        [TestMethod]
        public void search_Exectly_Equal_Existing_Map()
        {
            var maps = _mapManeger.GetAllMaps();
            foreach (Map map in maps)
            {

            }
        }


        private static Map randomMap()
        {
            Random rnd = new Random();
            Map map = new Map();
            map.Name = randomString();
            BsonDocument document = randomBsonDocument();
            map.Model = document;
            return map;
        }
        private static string randomString()
        {
            Random rnd = new Random();
            int stringSize = rnd.Next(1, 100);  // 1 <= month < 10000
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string finalString = "";
            for(int i=0; i< stringSize; i++)
            {
                int index = rnd.Next(0, chars.Length);
                finalString = finalString + chars[index];
            }
            return finalString;
        }

        private static BsonDocument randomBsonDocument()
        {
            Random rnd = new Random();
            int documntSize = rnd.Next(1, 100);

            var document = new BsonDocument
            {
                { "nodeDataArray",  new BsonArray
                                    {

                                    }
                }
            };

            for(int i=0; i<documntSize; i++)
            {

                document["nodeDataArray"].AsBsonArray.Add
                    (new BsonDocument
                         {
                            { "text", randomString() },
                         });
            }

            return document;

        }

        private static void cleanMaps()
        {
            var maps = _mapManeger.GetAllMaps();
            foreach (Map map in maps)
            {
                _mapManeger.RemoveMap(map.Id);
            }
        }
    }
}

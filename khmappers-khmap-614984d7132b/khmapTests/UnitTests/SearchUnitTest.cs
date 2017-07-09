using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using khmap.DataBaseProviders;
using khmap.Models;
using MongoDB.Bson;
using khmap.SearchFolder;
using System.Collections.Generic;

namespace khmapTests.UnitTests
{
    [TestClass]
    public class SearchUnitTest
    {
        private static readonly int STRING_SIZE_MAX = 10;
        private static readonly int DOCUMENT_SIZE_MAX = 10;
        private static readonly int LOOP_SIZE = 100;

        private MapDB _mapManeger;
        private UserDB _userManager;
        private GroupDB _groupManeger;
        private Random _rnd;
        private ISearch _searchObj;

        [TestInitialize]
        public void setUp()
        {
            _searchObj = new Search();
            _mapManeger = new MapDB(new Settings());
            _userManager = new UserDB(new Settings());
            _groupManeger = new GroupDB(new Settings());
            _rnd = new Random();
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




        //////////////////////////////////////searchMapFunc Tests////////////////////////////////////
        [TestMethod]
        public void searchMapsFunc_search_Exectly_Equal_Existing_Map_byName()
        {
            var maps = _mapManeger.GetAllMaps();
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                foreach (Map map in maps)
                {
                    List<Map> result = (List<Map>)_searchObj.searchMaps(map.Name);
                    bool foundSomthing = result.Count > 0;
                    Assert.IsTrue(foundSomthing);
                    bool isfound = false;
                    foreach (Map resMap in result)
                    {
                        if (resMap.Name.Equals(map.Name))
                        {
                            isfound = true;
                            break;
                        }
                    }
                    Assert.IsTrue(isfound);
                }
            }
        }
        [TestMethod]
        public void searchMapsFunc_search_Containing_Existing_Map_byName()
        {
            var maps = _mapManeger.GetAllMaps();
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                foreach (Map map in maps)
                {
                    List<Map> result = (List<Map>)_searchObj.searchMaps(map.Name.Substring(_rnd.Next(0, map.Name.Length)));
                    bool foundSomthing = result.Count > 0;
                    Assert.IsTrue(foundSomthing);
                    bool isfound = false;
                    foreach (Map resMap in result)
                    {
                        if (resMap.Name.Equals(map.Name))
                        {
                            isfound = true;
                            break;
                        }
                    }
                    Assert.IsTrue(isfound);
                }
            }
        }

        [TestMethod]
        public void searchMapsFunc_search_Exectly_Equal_Existing_Map_by_Element()
        {
            var maps = _mapManeger.GetAllMaps();
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                foreach (Map map in maps)
                {
                    foreach (var element in map.Model["nodeDataArray"].AsBsonArray)
                    {
                        List<Map> result = (List<Map>)_searchObj.searchMaps(element["text"].ToString());

                        bool foundSomthing = result.Count > 0;
                        Assert.IsTrue(foundSomthing);
                        bool isfound = false;
                        foreach (Map resMap in result)
                        {
                            if (resMap.Name.Equals(map.Name))
                            {
                                isfound = true;
                                break;
                            }
                        }
                        Assert.IsTrue(isfound);
                    }
                }
            }
        }

        [TestMethod]
        public void searchMapsFunc_search_Containing_Existing_Map_by_Element()
        {
            var maps = _mapManeger.GetAllMaps();
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                foreach (Map map in maps)
                {
                    foreach (var element in map.Model["nodeDataArray"].AsBsonArray)
                    {
                        string text = element["text"].ToString();
                        List<Map> result = (List<Map>)_searchObj.searchMaps(text.Substring(_rnd.Next(0, text.Length)));

                        bool foundSomthing = result.Count > 0;
                        Assert.IsTrue(foundSomthing);
                        bool isfound = false;
                        foreach (Map resMap in result)
                        {
                            if (resMap.Name.Equals(map.Name))
                            {
                                isfound = true;
                                break;
                            }
                        }
                        Assert.IsTrue(isfound);
                    }
                }
            }
        }
        [TestMethod]
        public void searchMapsFunc_search_Not_Existing_Map_byNameAndElementNAme()
        {
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                string mapName = randomString(11, 12);
                List<Map> result = (List<Map>)_searchObj.searchMaps(mapName);
                Assert.AreEqual(result.Count, 0);
            }
        }

        //////////////////////////////////////searchUserFunc Tests////////////////////////////////////
        [TestMethod]
        public void searchUserFunc_search_Exectly_Equal_Existing_Map_byName()
        {
            var users = _userManager.GetAllUsers();
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                foreach (ApplicationUser user in users)
                {
                    List<ApplicationUser> result = (List<ApplicationUser>)_searchObj.searchUsers(user.FirstName);
                    bool foundSomthing = result.Count > 0;
                    Assert.IsTrue(foundSomthing);
                    bool isfound = false;
                    foreach (ApplicationUser resUser in result)
                    {
                        if (resUser.FirstName.Equals(user.FirstName))
                        {
                            isfound = true;
                            break;
                        }
                    }
                    Assert.IsTrue(isfound);
                }
            }
        }
        [TestMethod]
        public void searchUserFunc_search_Containing_Existing_User_byName()
        {
            var users = _userManager.GetAllUsers();
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                foreach (ApplicationUser user in users)
                {
                    List<ApplicationUser> result = (List<ApplicationUser>)_searchObj.searchUsers(user.FirstName.Substring(_rnd.Next(0, user.FirstName.Length)));
                    bool foundSomthing = result.Count > 0;
                    Assert.IsTrue(foundSomthing);
                    bool isfound = false;
                    foreach (ApplicationUser resMap in result)
                    {
                        if (resMap.FirstName.Equals(user.FirstName))
                        {
                            isfound = true;
                            break;
                        }
                    }
                    Assert.IsTrue(isfound);
                }
            }
        }
        
        [TestMethod]
        public void searchUsersFunc_search_Not_Existing_User_byName()
        {
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                string userName = randomString(11, 12);
                List<ApplicationUser> result = (List<ApplicationUser>)_searchObj.searchUsers(userName);
                Assert.AreEqual(result.Count, 0);
            }
        }


        //////////////////////////////////////searchGroupFunc Tests////////////////////////////////////
        [TestMethod]
        public void searchGroupFunc_search_Exectly_Equal_Existing_Group_byName()
        {
            var groups = _groupManeger.GetAllGroups();
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                foreach (Group group in groups)
                {
                    List<Group> result = (List<Group>)_searchObj.searchGroups(group.Name);
                    bool foundSomthing = result.Count > 0;
                    Assert.IsTrue(foundSomthing);
                    bool isfound = false;
                    foreach (Group resGroup in result)
                    {
                        if (resGroup.Name.Equals(group.Name))
                        {
                            isfound = true;
                            break;
                        }
                    }
                    Assert.IsTrue(isfound);
                }
            }
        }
        [TestMethod]
        public void searchGroupFunc_search_Containing_Existing_Group_byName()
        {
            var groups = _groupManeger.GetAllGroups();
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                foreach (Group group in groups)
                {
                    string groupName = group.Name.Substring(_rnd.Next(0, group.Name.Length));
                    List<Group> result = (List<Group>)_searchObj.searchUsers(groupName);
                    bool foundSomthing = result.Count > 0;
                    Assert.IsTrue(foundSomthing);
                    bool isfound = false;
                    foreach (Group resGroup in result)
                    {
                        if (resGroup.Name.Equals(group.Name))
                        {
                            isfound = true;
                            break;
                        }
                    }
                    Assert.IsTrue(isfound);
                }
            }
        }

        [TestMethod]
        public void searchGroupsFunc_search_Not_Existing_Group_byName()
        {
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                string groupName = randomString(11, 12);
                List<Group> result = (List<Group>)_searchObj.searchGroups(groupName);
                Assert.AreEqual(result.Count, 0);
            }
        }



        //////////////////////////////////////searchFunc Tests////////////////////////////////////
        ///////////searching maps//////////
        [TestMethod]
        public void searchFunc_search_Exectly_Equal_Existing_Map_byName()
        {
            var maps = _mapManeger.GetAllMaps();
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                foreach (Map map in maps)
                {
                    var resultNotFilltered = _searchObj.searchFunc(map.Name, new List<ISearchType>() { new SearchMaps(), new SearchGroups(), new SearchUsers() });
                    List<Map> result = (List<Map>)_searchObj.getMapsOfResult(resultNotFilltered);
                    bool foundSomthing = result.Count > 0;
                    Assert.IsTrue(foundSomthing);
                    bool isfound = false;
                    foreach (Map resMap in result)
                    {
                        if (resMap.Name.Equals(map.Name))
                        {
                            isfound = true;
                            break;
                        }
                    }
                    Assert.IsTrue(isfound);


                }
            }
        }
        [TestMethod]
        public void searchFunc_search_Containing_Existing_Map_byName()
        {
            var maps = _mapManeger.GetAllMaps();
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                foreach (Map map in maps)
                {
                    string text = map.Name.Substring(_rnd.Next(0, map.Name.Length));
                    var resultNotFilltered = _searchObj.searchFunc(text, new List<ISearchType>() { new SearchMaps(), new SearchGroups(), new SearchUsers() });
                    List<Map> result = (List<Map>)_searchObj.getMapsOfResult(resultNotFilltered);
                    bool foundSomthing = result.Count > 0;
                    Assert.IsTrue(foundSomthing);
                    bool isfound = false;
                    foreach (Map resMap in result)
                    {
                        if (resMap.Name.Equals(map.Name))
                        {
                            isfound = true;
                            break;
                        }
                    }
                    Assert.IsTrue(isfound);
                }
            }
        }

        [TestMethod]
        public void searchFunc_search_Exectly_Equal_Existing_Map_by_Element()
        {
            var maps = _mapManeger.GetAllMaps();
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                foreach (Map map in maps)
                {
                    foreach (var element in map.Model["nodeDataArray"].AsBsonArray)
                    {
                        string text = element["text"].ToString();
                        var resultNotFilltered = _searchObj.searchFunc(text, new List<ISearchType>() { new SearchMaps(), new SearchGroups(), new SearchUsers() });
                        List<Map> result = (List<Map>)_searchObj.getMapsOfResult(resultNotFilltered);

                        bool foundSomthing = result.Count > 0;
                        Assert.IsTrue(foundSomthing);
                        bool isfound = false;
                        foreach (Map resMap in result)
                        {
                            if (resMap.Name.Equals(map.Name))
                            {
                                isfound = true;
                                break;
                            }
                        }
                        if (isfound == false)
                        {
                            int x = 0;
                        }
                        Assert.IsTrue(isfound);
                    }
                }
            }
        }

        [TestMethod]
        public void searchFunc_search_Containing_Existing_Map_by_Element()
        {
            var maps = _mapManeger.GetAllMaps();
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                foreach (Map map in maps)
                {
                    foreach (var element in map.Model["nodeDataArray"].AsBsonArray)
                    {
                        string text = element["text"].ToString();
                        text = text.Substring(_rnd.Next(0, text.Length));
                        var resultNotFilltered = _searchObj.searchFunc(text, new List<ISearchType>() { new SearchMaps(), new SearchGroups(), new SearchUsers() });
                        List<Map> result = (List<Map>)_searchObj.getMapsOfResult(resultNotFilltered);

                        bool foundSomthing = result.Count > 0;
                        Assert.IsTrue(foundSomthing);
                        bool isfound = false;
                        foreach (Map resMap in result)
                        {
                            if (resMap.Name.Equals(map.Name))
                            {
                                isfound = true;
                                break;
                            }
                        }
                        Assert.IsTrue(isfound);
                    }
                }
            }
        }
        [TestMethod]
        public void searchFunc_search_Not_Existing_Map_byNameAndElementNAme()
        {
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                string text = randomString(11, 12);
                var resultNotFilltered = _searchObj.searchFunc(text, new List<ISearchType>() { new SearchMaps(), new SearchGroups(), new SearchUsers() });
                List<Map> result = (List<Map>)_searchObj.getMapsOfResult(resultNotFilltered);
                Assert.AreEqual(result.Count, 0);
            }
        }

        /////////search users/////////
        [TestMethod]
        public void searchFunc_search_Exectly_Equal_Existing_User_byName()
        {
            var users = _userManager.GetAllUsers();
            for (int k = 0; k < LOOP_SIZE; k++)
            {

                foreach (ApplicationUser user in users)
                {
                    var resultNotFilltered = _searchObj.searchFunc(user.FirstName, new List<ISearchType>() { new SearchMaps(), new SearchGroups(), new SearchUsers() });
                    List<ApplicationUser> result = (List<ApplicationUser>)_searchObj.getUsersOfResult(resultNotFilltered);
                    bool foundSomthing = result.Count > 0;
                    Assert.IsTrue(foundSomthing);
                    bool isfound = false;
                    foreach (ApplicationUser resUser in result)
                    {
                        if (resUser.FirstName.Equals(user.FirstName))
                        {
                            isfound = true;
                            break;
                        }
                    }
                    Assert.IsTrue(isfound);
                }
            }
        }
        [TestMethod]
        public void searchFunc_search_Containing_Existing_User_byName()
        {
            var users = _userManager.GetAllUsers();
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                foreach (ApplicationUser user in users)
                {
                    string userName = user.FirstName.Substring(_rnd.Next(0, user.FirstName.Length));
                    var resultNotFilltered = _searchObj.searchFunc(userName, new List<ISearchType>() { new SearchMaps(), new SearchGroups(), new SearchUsers() });
                    List<ApplicationUser> result = (List<ApplicationUser>)_searchObj.getUsersOfResult(resultNotFilltered);

                    bool foundSomthing = result.Count > 0;
                    Assert.IsTrue(foundSomthing);
                    bool isfound = false;
                    foreach (ApplicationUser resMap in result)
                    {
                        if (resMap.FirstName.Equals(user.FirstName))
                        {
                            isfound = true;
                            break;
                        }
                    }
                    Assert.IsTrue(isfound);
                }
            }
        }

        [TestMethod]
        public void searchFunc_search_Not_Existing_User_byName()
        {
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                string userName = randomString(11, 12);
                var resultNotFilltered = _searchObj.searchFunc(userName, new List<ISearchType>() { new SearchMaps(), new SearchGroups(), new SearchUsers() });
                List<ApplicationUser> result = (List<ApplicationUser>)_searchObj.getUsersOfResult(resultNotFilltered);
                Assert.AreEqual(result.Count, 0);
            }
        }


        /////////search groups////////
        [TestMethod]
        public void searchFunc_search_Exectly_Equal_Existing_Group_byName()
        {
            var groups = _groupManeger.GetAllGroups();
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                foreach (Group group in groups)
                {
                    var resultNotFilltered = _searchObj.searchFunc(group.Name, new List<ISearchType>() { new SearchMaps(), new SearchGroups(), new SearchUsers() });
                    List<Group> result = (List<Group>)_searchObj.getGroupsOfResult(resultNotFilltered);
                    bool foundSomthing = result.Count > 0;
                    Assert.IsTrue(foundSomthing);
                    bool isfound = false;
                    foreach (Group resGroup in result)
                    {
                        if (resGroup.Name.Equals(group.Name))
                        {
                            isfound = true;
                            break;
                        }
                    }
                    Assert.IsTrue(isfound);
                }
            }
        }

        [TestMethod]
        public void searchFunc_search_Containing_Existing_Group_byName()
        {
            var groups = _groupManeger.GetAllGroups();
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                foreach (Group group in groups)
                {
                    string groupName = group.Name.Substring(_rnd.Next(0, group.Name.Length));
                    var resultNotFilltered = _searchObj.searchFunc(groupName, new List<ISearchType>() { new SearchMaps(), new SearchGroups(), new SearchUsers() });
                    List<Group> result = (List<Group>)_searchObj.getGroupsOfResult(resultNotFilltered);

                    bool foundSomthing = result.Count > 0;
                    Assert.IsTrue(foundSomthing);
                    bool isfound = false;
                    foreach (Group resGroup in result)
                    {
                        if (resGroup.Name.Equals(group.Name))
                        {
                            isfound = true;
                            break;
                        }
                    }
                    Assert.IsTrue(isfound);
                }
            }
        }

        [TestMethod]
        public void searchFunc_search_Not_Existing_Group_byName()
        {
            for (int k = 0; k < LOOP_SIZE; k++)
            {
                string groupName = randomString(11, 12);
                var resultNotFilltered = _searchObj.searchFunc(groupName, new List<ISearchType>() { new SearchMaps(), new SearchGroups(), new SearchUsers() });
                List<Group> result = (List<Group>)_searchObj.getGroupsOfResult(resultNotFilltered);
                Assert.AreEqual(result.Count, 0);
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
            int stringSize = rnd.Next(1, STRING_SIZE_MAX + 1);
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string finalString = "";
            for(int i=0; i< stringSize; i++)
            {
                int index = rnd.Next(0, chars.Length);
                finalString = finalString + chars[index];
            }
            return finalString;
        }

        private static string randomString(int minSize, int maxSize)
        {
            Random rnd = new Random();
            int stringSize = rnd.Next(minSize, maxSize + 1);
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string finalString = "";
            for (int i = 0; i < stringSize; i++)
            {
                int index = rnd.Next(0, chars.Length);
                finalString = finalString + chars[index];
            }
            return finalString;
        }

        private static BsonDocument randomBsonDocument()
        {
            Random rnd = new Random();
            int documntSize = rnd.Next(1, DOCUMENT_SIZE_MAX + 1);

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

        private void cleanMaps()
        {
            var maps = _mapManeger.GetAllMaps();
            foreach (Map map in maps)
            {
                _mapManeger.RemoveMap(map.Id);
            }
        }
    }
}

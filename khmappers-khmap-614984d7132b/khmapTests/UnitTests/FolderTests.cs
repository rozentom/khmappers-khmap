using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using khmap.Controllers;
using khmap.DataBaseProviders;
using khmap.Models;
using MongoDB.Bson;
using AspNet.Identity.MongoDB;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using khmap;



namespace khmapTests.UnitTests
{
    [TestClass]
    public class FolderTests
    {

        private static readonly int STRING_SIZE_MAX = 10;
        private static readonly int DOCUMENT_SIZE_MAX = 10;
        private static readonly int LOOP_SIZE = 2;

        private MapFolderDB _folderManeger;
        private MapDB _mapManeger;
        private UserDB _userManager;
        private GroupDB _groupManeger;
        private Random _rnd;
        private ApplicationUser _user;
        private ObjectId _userId;
        private MapPermissions _permissions;

        [TestInitialize]
        public void setUp()
        {
            _folderManeger = new MapFolderDB(new Settings());
            _mapManeger = new MapDB(new Settings());
            _userManager = new UserDB(new Settings());
            _groupManeger = new GroupDB(new Settings());
            _rnd = new Random();
            _user = new ApplicationUser();
            _user.FirstName = "kkk";
            _user.UserName = "lllll";
            _user.LastName = "ddddddddd";
            _userManager.AddUser(_user);
            _userId = new ObjectId(_user.Id);

            MapPermissions mapPermissions = new MapPermissions();
            mapPermissions.Owner = new KeyValuePair<ObjectId, MapPermissionType>(_userId, MapPermissionType.RW);
            mapPermissions.Users = new Dictionary<ObjectId, MapPermissionType>();
            mapPermissions.Groups = new Dictionary<ObjectId, MapPermissionType>();
            mapPermissions.AllUsers = MapPermissionType.NA;

            mapPermissions.Users.Add(_userId, MapPermissionType.RW);
            _permissions = mapPermissions;

        }

        [TestCleanup]
        public void cleanUp()
        {
            _userManager.RemoveAllUsers();
            _folderManeger.RemoveAllFolders();
            cleanMaps();
        }


        [TestMethod]
        public void addFolder()
        {
            MapFolder folder = new MapFolder();
            folder.Name = "folder";
            folder.Permissions = _permissions;
            _folderManeger.AddFolder(folder);
            MapFolder searchedFolder = _folderManeger.GetMapFolderById(folder.Id);
            Assert.IsNotNull(searchedFolder);
            Assert.AreEqual(folder.Id, searchedFolder.Id);
        }
        [TestMethod]
        public void removeFolder()
        {
            MapFolder folder = new MapFolder();
            folder.Name = "folder";
            folder.Permissions = _permissions;
            _folderManeger.AddFolder(folder);
            _folderManeger.RemoveMapFolderById(folder.Id);
            MapFolder searchedFolder = _folderManeger.GetMapFolderById(folder.Id);
            Assert.IsNull(searchedFolder);
        }
        [TestMethod]
        public void addNewFolderInMapFolderController()
        {
            MapFolder folder = new MapFolder();
            folder.Name = "folder";
            folder.Permissions = _permissions;
            _folderManeger.AddFolder(folder);
            MapFolder searchedFolder = _folderManeger.GetAllFolders().First();
            Assert.IsNotNull(searchedFolder);
            ForTests.addNewFolder(searchedFolder.Id.ToString(), "newFolder", "newFolder");
            Assert.IsTrue(searchedFolder.idOfSubFolders.Count == 1);
            ObjectId subFolderId = folder.idOfSubFolders.First();
            MapFolder subFolder = _folderManeger.GetMapFolderById(subFolderId);
            Assert.IsNotNull(subFolder);
            Assert.AreEqual(subFolderId, subFolder.Id);

        }
        [TestMethod]
        public void addNewFolderInMapFolderController_inNonExsistingFolder()
        {
            MapFolder folder = new MapFolder();
            folder.Name = "folder";
            folder.Permissions = _permissions;
            _folderManeger.AddFolder(folder);
            bool flag = false;
            try
            {
                ForTests.addNewFolder(folder.Id.ToString(), "newFolder", "newFolder");
            }
            catch
            {
                flag = true;
            }
            Assert.IsTrue(flag);
        }
        [TestMethod]
        public void addNewFolderInMapFolderController_adding2FoldersWithTheSameName()
        {
            MapFolder folder = new MapFolder();
            folder.Name = "folder";
            folder.Permissions = _permissions;
            _folderManeger.AddFolder(folder);
            MapFolder searchedFolder = _folderManeger.GetAllFolders().First();
            Assert.IsNotNull(searchedFolder);
            ForTests.addNewFolder(searchedFolder.Id.ToString(), "newFolder", "newFolder");
            ForTests.addNewFolder(searchedFolder.Id.ToString(), "newFolder", "newFolder");
            Assert.IsTrue(searchedFolder.idOfSubFolders.Count == 2);
        }
        [TestMethod]
        public void deleteFolderDB()
        {
            MapFolder folder = new MapFolder();
            folder.Name = "folder";
            folder.Permissions = _permissions;
            _folderManeger.AddFolder(folder);
            _folderManeger.RemoveMapFolderById(folder.Id);
            Assert.AreEqual(_folderManeger.GetAllFolders().Count(), 0);
        }
        public void delete2FolderWhitSameNameDB()
        {
            MapFolder folder = new MapFolder();
            folder.Name = "folder";
            folder.Permissions = _permissions;
            MapFolder folder2 = new MapFolder();
            folder2.Name = "folder";
            folder2.Permissions = _permissions;
            _folderManeger.AddFolder(folder);
            _folderManeger.RemoveMapFolderById(folder.Id);
            Assert.AreEqual(_folderManeger.GetAllFolders().Count(), 1);
            Assert.AreEqual(_folderManeger.GetAllFolders().First().Id, folder2.Id);
        }

        [TestMethod]
        public void deleteFolderController()
        {
            MapFolder folder = new MapFolder();
            folder.Name = "folder";
            folder.Permissions = _permissions;
            _folderManeger.AddFolder(folder);
            MapFolder searchedFolder = _folderManeger.GetAllFolders().First();
            Assert.IsNotNull(searchedFolder);
            ForTests.addNewFolder(searchedFolder.Id.ToString(), "newFolder", "newFolder");
            ForTests.deleteFolder(searchedFolder.idOfSubFolders.First().ToString());
            Assert.IsTrue(_folderManeger.GetAllFolders().Count() == 1);
            Assert.AreEqual(_folderManeger.GetAllFolders().First().Id, searchedFolder.Id);
        }
        [TestMethod]
        public void deleteFolderController_whenThereR2FoldersWithSameName()
        {
            MapFolder folder = new MapFolder();
            folder.Name = "folder";
            folder.Permissions = _permissions;
            _folderManeger.AddFolder(folder);
            MapFolder searchedFolder = _folderManeger.GetAllFolders().First();
            Assert.IsNotNull(searchedFolder);
            ForTests.addNewFolder(searchedFolder.Id.ToString(), "newFolder", "newFolder");
            ObjectId firstFolderId = searchedFolder.idOfSubFolders.First();
            ForTests.addNewFolder(searchedFolder.Id.ToString(), "newFolder", "newFolder");
            searchedFolder.idOfSubFolders.Remove(firstFolderId);
            ObjectId secFolderId = searchedFolder.idOfSubFolders.First();
            ForTests.deleteFolder(firstFolderId.ToString());
            Assert.IsTrue(_folderManeger.GetAllFolders().Count() == 2);
            Assert.AreEqual(_folderManeger.GetAllFolders().First().Id, searchedFolder.Id);
            Assert.AreEqual(searchedFolder.idOfSubFolders.First(), secFolderId);
        }
        [TestMethod]
        public void deleteFolderController_nonExistingFolder()
        {
            MapFolder folder = new MapFolder();
            folder.Name = "folder";
            folder.Permissions = _permissions;
            bool flag = false;
            try
            {
                ForTests.deleteFolder(folder.Id.ToString());
            }
            catch
            {
                flag = true;
            }
            Assert.IsTrue(flag);
        }
        [TestMethod]
        public void moveFolder()
        {
            MapFolder folder1 = new MapFolder();
            folder1.Name = "folder1";
            folder1.Permissions = _permissions;
            folder1.idOfMapsInFolder = new HashSet<ObjectId>();
            folder1.idOfSubFolders = new HashSet<ObjectId>();
            _folderManeger.AddFolder(folder1);
            MapFolder folder2 = new MapFolder();
            folder2.Name = "folder";
            folder2.Permissions = _permissions;
            folder2.idOfMapsInFolder = new HashSet<ObjectId>();
            folder2.idOfSubFolders = new HashSet<ObjectId>();
            _folderManeger.AddFolder(folder2);
            ForTests.moveFolder(folder2.Id.ToString(), folder1.Id.ToString());
            Assert.AreEqual(folder1.idOfSubFolders.Count(), 1);
            Assert.AreEqual(folder1.idOfSubFolders.First(), folder2.Id);
        }
        [TestMethod]
        public void moveFolder_intoItself()
        {
            MapFolder folder1 = new MapFolder();
            folder1.Name = "folder1";
            folder1.Permissions = _permissions;
            folder1.idOfMapsInFolder = new HashSet<ObjectId>();
            folder1.idOfSubFolders = new HashSet<ObjectId>();
            _folderManeger.AddFolder(folder1);
            ForTests.moveFolder(folder1.Id.ToString(), folder1.Id.ToString());
            Assert.AreEqual(folder1.idOfSubFolders.Count(), 0);
        }
        [TestMethod]
        public void moveFolder_intoANonExisting()
        {
            MapFolder folder1 = new MapFolder();
            folder1.Name = "folder1";
            folder1.Permissions = _permissions;
            folder1.idOfMapsInFolder = new HashSet<ObjectId>();
            folder1.idOfSubFolders = new HashSet<ObjectId>();
            _folderManeger.AddFolder(folder1);
            bool flag = false;
            try
            {
                ForTests.moveFolder(folder1.Id.ToString(), randomString());
            }
            catch
            {
                flag = true;
            }
            Assert.IsTrue(flag);
        }
        public void moveMap()
        {
            MapFolder folder1 = new MapFolder();
            folder1.Name = "folder1";
            folder1.Permissions = _permissions;
            folder1.idOfMapsInFolder = new HashSet<ObjectId>();
            folder1.idOfSubFolders = new HashSet<ObjectId>();
            _folderManeger.AddFolder(folder1);
            Map map = randomMap();
            MapPermissions mapPermissions = new MapPermissions();
            mapPermissions.Owner = new KeyValuePair<ObjectId, MapPermissionType>(_userId, MapPermissionType.RW);
            mapPermissions.Users = new Dictionary<ObjectId, MapPermissionType>();
            mapPermissions.Groups = new Dictionary<ObjectId, MapPermissionType>();
            mapPermissions.AllUsers = MapPermissionType.NA;
            mapPermissions.Users.Add(_userId, MapPermissionType.RW);
            map.Permissions = mapPermissions;
            _mapManeger.AddMap(map);
            ForTests.moveMap(map.Id.ToString(), folder1.Id.ToString());
            Assert.AreEqual(folder1.idOfMapsInFolder.Count(), 1);
            Assert.AreEqual(folder1.idOfMapsInFolder.First(), map.Id);
        }
        public void moveMapToCurrentFolder()
        {
            MapFolder folder1 = new MapFolder();
            folder1.Name = "folder1";
            folder1.Permissions = _permissions;
            folder1.idOfMapsInFolder = new HashSet<ObjectId>();
            folder1.idOfSubFolders = new HashSet<ObjectId>();
            
            Map map = randomMap();
            MapPermissions mapPermissions = new MapPermissions();
            mapPermissions.Owner = new KeyValuePair<ObjectId, MapPermissionType>(_userId, MapPermissionType.RW);
            mapPermissions.Users = new Dictionary<ObjectId, MapPermissionType>();
            mapPermissions.Groups = new Dictionary<ObjectId, MapPermissionType>();
            mapPermissions.AllUsers = MapPermissionType.NA;
            mapPermissions.Users.Add(_userId, MapPermissionType.RW);
            map.Permissions = mapPermissions;
            _mapManeger.AddMap(map);
            folder1.idOfMapsInFolder.Add(map.Id);
            _folderManeger.AddFolder(folder1);
            ForTests.moveMap(map.Id.ToString(), folder1.Id.ToString());
            Assert.AreEqual(folder1.idOfMapsInFolder.Count(), 1);
            Assert.AreEqual(folder1.idOfMapsInFolder.First(), map.Id);
        }
        [TestMethod]
        public void moveMap_intoANonExisting()
        {
            MapFolder folder1 = new MapFolder();
            Map map = randomMap();
            MapPermissions mapPermissions = new MapPermissions();
            mapPermissions.Owner = new KeyValuePair<ObjectId, MapPermissionType>(_userId, MapPermissionType.RW);
            mapPermissions.Users = new Dictionary<ObjectId, MapPermissionType>();
            mapPermissions.Groups = new Dictionary<ObjectId, MapPermissionType>();
            mapPermissions.AllUsers = MapPermissionType.NA;
            mapPermissions.Users.Add(_userId, MapPermissionType.RW);
            map.Permissions = mapPermissions;
            _mapManeger.AddMap(map);
            bool flag = false;
            try
            {
                ForTests.moveMap(map.Id.ToString(), randomString());
            }
            catch
            {
                flag = true;
            }
            Assert.IsTrue(flag);
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
            for (int i = 0; i < stringSize; i++)
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

            for (int i = 0; i < documntSize; i++)
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

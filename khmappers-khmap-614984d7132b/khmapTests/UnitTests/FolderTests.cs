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
using System.Web.Mvc;




namespace khmapTests.UnitTests
{
    [TestClass]
    public class FolderTests
    {
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
            _folderManeger.RemoveMapFolderById(folder.Id);
            MapFolderController mfc = new MapFolderController();
            mfc.addNewFolder(folder.Id.ToString(), "newFolder", "newFolder");
            Assert.IsTrue(folder.idOfSubFolders.Count ==1);
            ObjectId subFolderId = folder.idOfSubFolders.First();
            MapFolder subFolder = _folderManeger.GetMapFolderById(subFolderId);
            Assert.IsNotNull(subFolder);
            Assert.AreEqual(subFolderId, subFolder.Id);
        }

    }
}

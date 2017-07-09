using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using khmap.Models;
using khmap.Controllers;
using System.Diagnostics;
using khmap.DataBaseProviders;
using System.Collections.Generic;
using MongoDB.Bson;

namespace KHMAPTest
{
    [TestClass]
    public class UserDbTest
    {

        private UserDB uDB;

        [TestInitialize]
        public void InitDB()
        {
            uDB = new UserDB(new Settings());
        }


        [TestMethod]
        public void TestAddNonExistingUser()
        {
            // Drop test database
            Assert.IsFalse(uDB.IsEmailExist("Bla@bla.com"));
            ApplicationUser user = new ApplicationUser { UserName = "G", Email = "Bla@bla.com", FirstName = "Test", LastName = "S", City = "B-S", Country = "Israel" };
            uDB.AddUser(user);
            Assert.IsTrue(uDB.IsEmailExist("Bla@bla.com"));
            uDB.RemoveUser(new MongoDB.Bson.ObjectId(user.Id));
        }

        [TestMethod]
        public void TestAddExistingUser()
        {
            // Drop test database
            ApplicationUser user = new ApplicationUser { UserName = "G", Email = "Bla2@bla.com", FirstName = "Test", LastName = "S", City = "B-S", Country = "Israel" };
            uDB.AddUser(user);
            Assert.IsTrue(uDB.IsEmailExist("Bla2@bla.com"));
            IEnumerable<ApplicationUser> us = uDB.GetAllUsers();
            List<ApplicationUser> users = new List<ApplicationUser>(us);
            int countUsers = users.Count;
            ApplicationUser eUser = uDB.GetUserByEmail("Bla2@bla.com");
            uDB.AddUser(eUser);
            us = uDB.GetAllUsers();
            users = new List<ApplicationUser>(us);
            int newUserCount = users.Count;
            uDB.RemoveUser(new MongoDB.Bson.ObjectId(eUser.Id));
            Assert.AreEqual(countUsers, newUserCount);
        }

        [TestMethod]
        public void TestGetAllUsers()
        {

            IEnumerable<ApplicationUser> userss = uDB.GetAllUsers();
            List<ApplicationUser> users = new List<ApplicationUser>(userss);
            int userCount = users.Count;

            // Drop test database
            ApplicationUser user1 = new ApplicationUser { UserName = "A", Email = "A@bla.com", FirstName = "Gilad", LastName = "S", City = "B-S", Country = "Israel" };
            uDB.AddUser(user1);

            ApplicationUser user2 = new ApplicationUser { UserName = "B", Email = "B@bla.com", FirstName = "Gilad", LastName = "S", City = "B-S", Country = "Israel" };
            uDB.AddUser(user2);

            ApplicationUser user3 = new ApplicationUser { UserName = "C", Email = "C@bla.com", FirstName = "Gilad", LastName = "S", City = "B-S", Country = "Israel" };
            uDB.AddUser(user3);

            IEnumerable<ApplicationUser> userss2 = uDB.GetAllUsers();
            List<ApplicationUser> users2 = new List<ApplicationUser>(userss2);

            int newUsers = users2.Count - userCount;

            uDB.RemoveUser(new MongoDB.Bson.ObjectId(user1.Id));
            uDB.RemoveUser(new MongoDB.Bson.ObjectId(user2.Id));
            uDB.RemoveUser(new MongoDB.Bson.ObjectId(user3.Id));

            Assert.AreEqual(3, newUsers);
        }

        [TestMethod]
        public void TestGetExistingUserById()
        {
            ApplicationUser user = new ApplicationUser { UserName = "G", Email = "Bla3@bla.com", FirstName = "Gilad", LastName = "S", City = "B-S", Country = "Israel" };
            uDB.AddUser(user);
            ApplicationUser eUser = uDB.GetUserByEmail("Bla3@bla.com");
            ObjectId uId = new MongoDB.Bson.ObjectId(eUser.Id);

            ApplicationUser byIdUser = uDB.GetUserById(uId);
            Assert.IsNotNull(byIdUser);
            Assert.AreEqual(eUser.Id, byIdUser.Id);
            uDB.RemoveUser(new MongoDB.Bson.ObjectId(user.Id));
        }

        [TestMethod]
        public void TestGetNonExistingUserById()
        {
            ObjectId uId = new ObjectId();
            Assert.IsNotNull(uId);

            ApplicationUser user = uDB.GetUserById(uId);
            Assert.IsNull(user);
        }

        [TestMethod]
        public void TestGetUserByEmail()
        {
            ApplicationUser user = new ApplicationUser { UserName = "G", Email = "Bla4@bla.com", FirstName = "Gilad", LastName = "S", City = "B-S", Country = "Israel" };
            uDB.AddUser(user);
            ApplicationUser eUser = uDB.GetUserByEmail("Bla4@bla.com");
            Assert.AreEqual("Bla4@bla.com", eUser.Email);
            uDB.RemoveUser(new MongoDB.Bson.ObjectId(user.Id));
            ApplicationUser neUser = uDB.GetUserByEmail("lala@lala.la");
            Assert.IsNull(neUser);
        }

        [TestMethod]
        public void TestIsUserExist()
        {
            ApplicationUser user = new ApplicationUser { UserName = "G", Email = "Bla5@bla.com", FirstName = "Gilad", LastName = "S", City = "B-S", Country = "Israel" };
            uDB.AddUser(user);
            ApplicationUser eUser = uDB.GetUserByEmail("Bla5@bla.com");
            ObjectId uId = new MongoDB.Bson.ObjectId(eUser.Id);

            bool ans = uDB.IsUserlExist(uId);
            Assert.IsTrue(ans);
            uDB.RemoveUser(new MongoDB.Bson.ObjectId(user.Id));
        }

        [TestMethod]
        public void TestIsEmailExist()
        {
            ApplicationUser user = new ApplicationUser { UserName = "G", Email = "Bla6@bla.com", FirstName = "Gilad", LastName = "S", City = "B-S", Country = "Israel" };
            uDB.AddUser(user);
            ApplicationUser eUser = uDB.GetUserByEmail("Bla6@bla.com");

            bool ans = uDB.IsEmailExist("Bla6@bla.com");
            Assert.IsTrue(ans);

            ans = uDB.IsEmailExist("bla@b.c");
            Assert.IsFalse(ans);
            uDB.RemoveUser(new MongoDB.Bson.ObjectId(user.Id));
        }

        [TestMethod]
        public void TestRemoveUser()
        {
            ApplicationUser user = new ApplicationUser { UserName = "G", Email = "Bla7@bla.com", FirstName = "Gilad", LastName = "S", City = "B-S", Country = "Israel" };
            uDB.AddUser(user);
            ApplicationUser eUser = uDB.GetUserByEmail("Bla7@bla.com");
            ObjectId uId = new MongoDB.Bson.ObjectId(eUser.Id);

            bool res = uDB.RemoveUser(uId);
            Assert.IsTrue(res);

            eUser = uDB.GetUserById(uId);
            Assert.IsNull(eUser);
            uDB.RemoveUser(new MongoDB.Bson.ObjectId(user.Id));
        }

        [TestMethod]
        public void TestUpdateExistingUser()
        {
            ApplicationUser user = new ApplicationUser { UserName = "G", Email = "Bla8@bla.com", FirstName = "Gilad", LastName = "S", City = "B-S", Country = "Israel" };
            uDB.AddUser(user);
            ApplicationUser eUser = uDB.GetUserByEmail("Bla8@bla.com");
            ObjectId uId = new MongoDB.Bson.ObjectId(eUser.Id);

            user = uDB.GetUserById(uId);
            Assert.AreEqual("G", user.UserName);
            Assert.AreEqual("B-S", user.City);
            Assert.IsNull(user.Rankings);

            user.UserName = "GG";
            user.City = "bs";
            user.Rankings = new Dictionary<ObjectId, int>();
            uDB.UpdateUser(user);

            user = uDB.GetUserById(uId);
            Assert.AreEqual("GG", user.UserName);
            Assert.AreEqual("bs", user.City);
            Assert.IsNotNull(user.Rankings);
            uDB.RemoveUser(new MongoDB.Bson.ObjectId(user.Id));
        }
    }

}

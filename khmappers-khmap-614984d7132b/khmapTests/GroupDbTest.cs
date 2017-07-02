using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using khmap.DataBaseProviders;
using khmap.Models;
using khmap.Controllers;
using System.Collections.Generic;
using MongoDB.Bson;

namespace khmapTest
{
    [TestClass]
    public class GroupDbTest
    {
        GroupDB dataProvider;

        int count;

        [TestInitialize]
        public void InitDB()
        {
            dataProvider = new GroupDB(new Settings());
            this.count = CountGroups();

        }

        [TestMethod]
        public void TestAddGroup()
        {
            Group g = new Group { Creator = new ObjectId(), CreationTime = DateTime.Now, Name = "Mappers", Members = new Dictionary<ObjectId, GroupPermissionType>() };
            this.dataProvider.AddGroup(g);
            Assert.AreEqual(CountGroups() - this.count, 1);
            this.dataProvider.RemoveGroup(g.Id);
        }

        [TestMethod]
        public void TestAddNumberOfGroups()
        {
            Group group = new Group { Creator = new ObjectId(), CreationTime = DateTime.Now, Name = "Mappers", Members = new Dictionary<ObjectId, GroupPermissionType>() };
            Group group2 = new Group { Creator = new ObjectId(), CreationTime = DateTime.Now, Name = "Mappers2", Members = new Dictionary<ObjectId, GroupPermissionType>() };
            Group group3 = new Group { Creator = new ObjectId(), CreationTime = DateTime.Now, Name = "Mappers3", Members = new Dictionary<ObjectId, GroupPermissionType>() };
            dataProvider.AddGroup(group);
            dataProvider.AddGroup(group2);
            dataProvider.AddGroup(group3);
            Assert.AreEqual(3, CountGroups() - this.count);
            this.dataProvider.RemoveGroup(group.Id);
            this.dataProvider.RemoveGroup(group2.Id);
            this.dataProvider.RemoveGroup(group3.Id);
        }


        [TestMethod]
        public void TestDeletionOfGroups()
        {
            Group group = new Group { Creator = new ObjectId(), CreationTime = DateTime.Now, Name = "Mappers", Members = new Dictionary<ObjectId, GroupPermissionType>() };
            Group group2 = new Group { Creator = new ObjectId(), CreationTime = DateTime.Now, Name = "Mappers2", Members = new Dictionary<ObjectId, GroupPermissionType>() };
            Group group3 = new Group { Creator = new ObjectId(), CreationTime = DateTime.Now, Name = "Mappers3", Members = new Dictionary<ObjectId, GroupPermissionType>() };
            dataProvider.AddGroup(group);
            dataProvider.AddGroup(group2);
            dataProvider.AddGroup(group3);
            this.dataProvider.RemoveGroup(group.Id);
            this.dataProvider.RemoveGroup(group2.Id);
            this.dataProvider.RemoveGroup(group3.Id);
            Assert.AreEqual(CountGroups(), this.count);
        }

        [TestMethod]
        public void TestGetGroupById()
        {
            Group group = new Group { Creator = new ObjectId(), CreationTime = DateTime.Now, Name = "Mappers", Members = new Dictionary<ObjectId, GroupPermissionType>() };
            this.dataProvider.AddGroup(group);
            IEnumerable<Group> gs = this.dataProvider.GetAllGroups();
            List<Group> groups = new List<Group>(gs);
            ObjectId gId = groups[this.count].Id;
            Group eGroup = this.dataProvider.GetGroupById(gId);
            Assert.IsNotNull(eGroup);
            Group neGroup = this.dataProvider.GetGroupById(new ObjectId());
            Assert.IsNull(neGroup);
            this.dataProvider.RemoveGroup(group.Id);
        }

        [TestMethod]
        public void TestGetAllGroups()
        {
            Group group = new Group { Creator = new ObjectId(), CreationTime = DateTime.Now, Name = "Mappers", Members = new Dictionary<ObjectId, GroupPermissionType>() };
            Group group2 = new Group { Creator = new ObjectId(), CreationTime = DateTime.Now, Name = "Mappers2", Members = new Dictionary<ObjectId, GroupPermissionType>() };
            Group group3 = new Group { Creator = new ObjectId(), CreationTime = DateTime.Now, Name = "Mappers3", Members = new Dictionary<ObjectId, GroupPermissionType>() };
            this.dataProvider.AddGroup(group);
            this.dataProvider.AddGroup(group2);
            this.dataProvider.AddGroup(group3);
            IEnumerable<Group> gs = this.dataProvider.GetAllGroups();
            List<Group> groups = new List<Group>(gs);
            Assert.AreEqual(groups[this.count].Name, "Mappers");
            Assert.AreEqual(groups[this.count + 1].Name, "Mappers2");
            Assert.AreEqual(groups[this.count + 2].Name, "Mappers3");
            Assert.AreEqual(CountGroups() - this.count, 3);
            this.dataProvider.RemoveGroup(group.Id);
            this.dataProvider.RemoveGroup(group2.Id);
            this.dataProvider.RemoveGroup(group3.Id);
        }

        [TestMethod]
        public void TestIsGroupExist()
        {
            Group group = new Group { Creator = new ObjectId(), CreationTime = DateTime.Now, Name = "Mappers", Members = new Dictionary<ObjectId, GroupPermissionType>() };
            this.dataProvider.AddGroup(group);
            IEnumerable<Group> gs = this.dataProvider.GetAllGroups();
            List<Group> groups = new List<Group>(gs);
            ObjectId gId = groups[0].Id;
            bool res = this.dataProvider.IsGroupExist(gId);
            Assert.IsTrue(res);//checks if the existing group is in the database
            res = this.dataProvider.IsGroupExist(new ObjectId());
            Assert.IsFalse(res);//checks if an un existing group is in the database
        }

        [TestMethod]
        public void TestRemoveGroup()
        {
            Group group = new Group { Creator = new ObjectId(), CreationTime = DateTime.Now, Name = "Mappers", Members = new Dictionary<ObjectId, GroupPermissionType>() };
            this.dataProvider.AddGroup(group);
            IEnumerable<Group> gs = this.dataProvider.GetAllGroups();
            List<Group> groups = new List<Group>(gs);
            ObjectId gId = groups[this.count].Id;
            Assert.AreEqual(1, CountGroups() - this.count);
            bool res = this.dataProvider.RemoveGroup(gId);
            Assert.IsTrue(res);
            Assert.AreEqual(0, CountGroups() - this.count);
        }

        [TestMethod]
        public void TestUpdateGroup()
        {
            Group group = new Group { Creator = new ObjectId(), CreationTime = DateTime.Now, Name = "Mappers", Members = new Dictionary<ObjectId, GroupPermissionType>() };
            this.dataProvider.AddGroup(group);
            IEnumerable<Group> gs = this.dataProvider.GetAllGroups();
            List<Group> groups = new List<Group>(gs);
            ObjectId gId = groups[this.count].Id;
            Group g = this.dataProvider.GetGroupById(gId);
            g.Name = "NewMappers";
            g.Members.Add(new ObjectId(), GroupPermissionType.Manager);
            this.dataProvider.UpdateGroup(g);
            g = this.dataProvider.GetGroupById(gId);
            Assert.AreEqual("NewMappers", g.Name);
            Assert.AreEqual(1, g.Members.Keys.Count);
            this.dataProvider.RemoveGroup(g.Id);
        }

        private int CountGroups()
        {
            IEnumerable<Group> groupss = dataProvider.GetAllGroups();
            List<Group> groups = new List<Group>(groupss);
            return groups.Count;
        }
    }
}

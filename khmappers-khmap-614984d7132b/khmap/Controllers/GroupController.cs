using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using khmap.DataBaseProviders;
using khmap.Models;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MongoDB.Bson;

namespace khmap.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        private GroupDB _groupManager;
        private ApplicationUserManager _userManager;
        private UserDB _userService;

        public GroupController()
        {
            this._groupManager = new GroupDB(new Settings());
            this._userService = new UserDB(new Settings());
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }



        // GET: Group
        /*
        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            ObjectId oId = new ObjectId(id);
            IEnumerable<Group> groups = _groupManager.GetAllGroupsOfUser(oId);
            List<GroupListViewModel> lmv = new List<GroupListViewModel>();
            foreach (var group in groups)
            {
                var email = UserManager.GetEmail(group.Creator.ToString());
                GroupListViewModel tmp = new GroupListViewModel { Id = group.Id , Name = group.Name, Creator = email, Description = group.Description, CreationTime = group.CreationTime, Size = group.Members.Count };
                lmv.Add(tmp);
            }
            return View(lmv);
        }
        */


        // GET: Group/Details/5
        public ActionResult Details(string id)
        {
            var oId = new ObjectId(id);
            Group group = _groupManager.GetGroupById(oId);
            var email = UserManager.GetEmail(group.Creator.ToString());
            GroupDetailsViewModel dvm = new GroupDetailsViewModel { Id = group.Id.ToString(), Name = group.Name, Description = group.Description, CreationTime = group.CreationTime, CreatorEmail = email, Members = group.Members };
            ViewBag.GroupID = id;
            return View(dvm);
        }

        // GET: Group/Create
        public ActionResult Create()
        {
            return View();
        }


        private MapFolder createFolderLocaly(string folderName)
        {
            MapFolderDB _mapFolderDataManager = new MapFolderDB(new Settings());

            var id = User.Identity.GetUserId();
            ObjectId UserId = new ObjectId(id);

            MapPermissions mapPermissions = new MapPermissions();
            mapPermissions.Owner = new KeyValuePair<ObjectId, MapPermissionType>(UserId, MapPermissionType.RW);
            mapPermissions.Users = new Dictionary<ObjectId, MapPermissionType>();
            mapPermissions.Groups = new Dictionary<ObjectId, MapPermissionType>();
            mapPermissions.AllUsers = MapPermissionType.NA;

            mapPermissions.Users.Add(UserId, MapPermissionType.RW);

            MapFolder suppFolder = new MapFolder();
            suppFolder.Name = folderName;
            suppFolder.Creator = UserId;
            suppFolder.CreationTime = DateTime.Now;
            suppFolder.Description = "shared folder for group" + folderName;
            suppFolder.Followers = new HashSet<ObjectId>();
            suppFolder.Permissions = mapPermissions;
            suppFolder.idOfMapsInFolder = new HashSet<ObjectId>();
            suppFolder.idOfSubFolders = new HashSet<ObjectId>();

           // MapFolder supFolder = _mapFolderDataManager.GetSuperiorMapFolderOfUser
            suppFolder.ParentDierctory = new ObjectId();
            suppFolder.FirstFolderOfUser = UserId;
            var maps = new MapDB(new Settings()).GetAllMaps();
            foreach (Map map in maps)
            {
                suppFolder.idOfMapsInFolder.Add(map.Id);
            }
            return suppFolder;

        }


        // POST: Group/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GroupCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var id = User.Identity.GetUserId();
                    ObjectId oId = new ObjectId(id);
                    Group group = new Group { Creator = oId, Description = model.Description, CreationTime = DateTime.Now, Name = model.Name, Members = new Dictionary<MongoDB.Bson.ObjectId, GroupPermissionType>() { { oId, GroupPermissionType.Owner } } };
                    _groupManager.AddGroup(group);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return View();
            }
        }

        // GET: Group/Edit/5
        public ActionResult Edit(string id)
        {
            try
            {
                if (IsValidId(id))
                {
                    var group = _groupManager.GetGroupById(new ObjectId(id));
                    if (IsValidGroup(group) && _groupManager.IsGroupOwner(group.Id.ToString(), User.Identity.GetUserId()))
                    {
                        GroupEditViewModel gevm = new GroupEditViewModel { Id = group.Id.ToString(), Description = group.Description, Name = group.Name };
                        return View(gevm);
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Group/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GroupEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Group group = _groupManager.GetGroupById(new ObjectId(model.Id));
                    group.Name = model.Name;
                    group.Description = model.Description;
                    _groupManager.UpdateGroup(group);
                    return RedirectToAction("Index", "Group", new { id = model.Id });
                }
                catch (Exception)
                {
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: Group/Delete/5
        public ActionResult Delete(string id)
        {
            try
            {
                var group = _groupManager.GetGroupById(new ObjectId(id));
                 if (IsValidId(id) && IsValidGroup(group) && _groupManager.IsGroupOwner(group.Id.ToString(), User.Identity.GetUserId()))
                 {
                     var email = UserManager.GetEmail(group.Creator.ToString());
                     GroupDetailsViewModel gdvm = new GroupDetailsViewModel { Id = group.Id.ToString(), Name = group.Name, Description = group.Description, CreationTime = group.CreationTime, CreatorEmail = email, Members = group.Members };
                     return View(gdvm);
                 }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Group/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(GroupDetailsViewModel model)
        {
            try
            {
                _groupManager.RemoveGroup(new ObjectId(model.Id));
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {

                return View();
            }
        }

        public ActionResult MyGroups()
        {
            try
            {
                var id = User.Identity.GetUserId();
                ObjectId oId = new ObjectId(id);
                var groups = _groupManager.GetAllOwnedGroupsOfUser(oId);
                return PartialView("_MyGroupsView", groups);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult MyMemberGroups()
        {
            try
            {
                var id = User.Identity.GetUserId();
                ObjectId oId = new ObjectId(id);
                var groups = _groupManager.GetAllMemberGroupsOfUser(oId);
                return PartialView("_MyGroupsView", groups);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ViewResult Index(string id)
        {
            ObjectId gId = new ObjectId(id);
            var group = _groupManager.GetGroupById(gId);
            List<Tuple<ObjectId, string, GroupPermissionType>> membersPermissions = new List<Tuple<ObjectId, string, GroupPermissionType>>();

            foreach (var item in group.Members)
            {
                var email = UserManager.GetEmail(item.Key.ToString());
                membersPermissions.Add(Tuple.Create(item.Key, email, item.Value));
            }

            GroupViewModel gvm = new GroupViewModel { Id = group.Id, CreationTime = group.CreationTime, CreatorEmail = UserManager.GetEmail(group.Creator.ToString()), Description = group.Description, Name = group.Name, Members = membersPermissions, CreatorId = group.Creator.ToString() };
            ViewBag.isAdmin = (group.Creator.ToString() == User.Identity.GetUserId());
            return View(gvm);
        }

        public int NumberOfUsers(string id)
        {
            var group =_groupManager.GetGroupById(new ObjectId(id));
            return group.Members.Count();
        }


        /*
        public ActionResult AddUserToGroup(string groupId, string string userEmail, )
        {
            
            _groupManager.RemoveGroup(new ObjectId(id));
            return RedirectToAction("Index");
        }
        */

        public ActionResult GetGroupUserPermissionsList(string groupId, int? page, int? limit, string sortBy, string direction, string searchString = null)
        {
            List<GroupUserPermissionViewModel> mupvm = new List<GroupUserPermissionViewModel>();
            var users = _groupManager.GetGroupById(new ObjectId(groupId)).Members;

            // users
            foreach (var uId in users.Keys)
            {
                var u = _userService.GetUserById(uId);
                mupvm.Add(new GroupUserPermissionViewModel { GroupId = groupId, UserId = uId.ToString(), Email = u.Email, FirstName = u.FirstName, LastName = u.LastName, Permission = users[uId], PermissionString = users[uId].ToString() });
            }

            var total = mupvm.Count();
            var records = mupvm.AsQueryable(); ;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                records = records.Where(x => x.Email.Contains(searchString) || x.FirstName.Contains(searchString) || x.LastName.Contains(searchString) || x.Permission.ToString().Contains(searchString));
            }
            //if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
            //{
            //    if (direction.Trim().ToLower() == "asc")
            //    {
            //        records = SortHelper.OrderBy(records, sortBy);
            //    }
            //    else
            //    {
            //        records = SortHelper.OrderByDescending(records, sortBy);
            //    }
            //}
            if (page.HasValue && limit.HasValue)
            {
                int start = (page.Value - 1) * limit.Value;
                records = records.Skip(start).Take(limit.Value);
            }

            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveGroupUserPermission(string gId, string uId, GroupPermissionType permission)
        {
            var group = _groupManager.GetGroupById(new ObjectId(gId));
            if (group.Creator.ToString() == User.Identity.GetUserId() && _userService.IsUserlExist(new ObjectId(uId)) && group.Creator.ToString() != uId)
            {
                if (group.Members.Keys.Contains(new ObjectId(uId)))
                {
                    group.Members[new ObjectId(uId)] = permission;
                }
                else
                {
                    group.Members.Add(new ObjectId(uId), permission);
                }
                _groupManager.UpdateGroup(group);
                return Json(true);
            }
            return Json(false);
        }

        public ActionResult AddGroupUserPermission(string gId, string uEmail, GroupPermissionType permission)
        {
            var uId = _userService.GetUserByEmail(uEmail).Id.ToString();
            return SaveGroupUserPermission(gId, uId, permission);
        }

        public ActionResult RemoveGroupUserPermission(string gId, string uId)
        {
            var res = false;
            var group = _groupManager.GetGroupById(new ObjectId(gId));
            if (group.Creator.ToString() == User.Identity.GetUserId() && _userService.IsUserlExist(new ObjectId(uId)))
            {
                res = group.Members.Remove(new ObjectId(uId));
                _groupManager.UpdateGroup(group);
                
            }
            return Json(res);
        }


        public ActionResult GetGroupPermissions(string groupId)
        {
            try
            {
                var group = _groupManager.GetGroupById(new ObjectId(groupId));
                if (group != null)
                {
                    var gModel = new GroupViewModel { Id = group.Id, Name = group.Name, CreatorId = group.Creator.ToString() };
                    if (User.Identity.GetUserId() == group.Creator.ToString())
                    {
                        return PartialView("_GroupPermissionsView", gModel);
                    }
                    else
                    {
                        return PartialView("_GroupNOPermissionsView", gModel);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult GetGroupsNamesList()
        {
            var groups = _groupManager.GetAllGroups();
            var namesList = groups.Select(u => new GroupNameViewModel { GroupId = u.Id.ToString(), Name = u.Name });
            return Json(namesList, JsonRequestBehavior.AllowGet);
        }

        public bool IsGroupMember(string groupId, string userId)
        {
            var group = _groupManager.GetGroupById(new ObjectId(groupId));
            return group.Members.Keys.Contains(new ObjectId(userId), new IDComparer());
        }

        public IEnumerable<Group> GetGroupsByUserId(string userId)
        {
            return _groupManager.GetAllGroupsOfUser(new ObjectId(userId));
        }

        private bool IsValidId(string groupId)
        {
            return (!string.IsNullOrEmpty(groupId) && !string.IsNullOrWhiteSpace(groupId));
        }

        private bool IsValidGroup(Group group)
        {
            return group != null;
        }
    }
}

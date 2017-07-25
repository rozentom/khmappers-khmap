using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using khmap.DataBaseProviders;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using khmap.Models;

namespace khmap.Controllers
{
    [Authorize]
    public class FolderPermissionController : Controller
    {

        private MapDB _mapManager;
        private UserDB _userService;
        private GroupDB _groupManager;
        private ApplicationUserManager _userManager;
        MapFolderDB _folderManeger;

        public FolderPermissionController()
        {
            this._mapManager = new MapDB(new Settings());
            this._userService = new UserDB(new Settings());
            this._groupManager = new GroupDB(new Settings());
            this._folderManeger = new MapFolderDB(new Settings());

            //_userManager = UserManager;
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


        // GET: MapPermission
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddUserToFolder(string folderId)
        {
            AddUserToFolderViewModel f = new AddUserToFolderViewModel { FId = folderId };
            return View(f);
        }

        [HttpPost]
        public ActionResult AddUserToFolder(AddUserToFolderViewModel model)
        {
            model.UId = UserManager.FindByEmail(model.Email).Id;
            if (AddUserToFolder(model.UId.ToString(), model.FId.ToString(), model.Permission))
            {
                return View();
            }
            return View();
        }

        public Dictionary<ApplicationUser, MapPermissionType> GetUsersPermissions(string folderId)
        {
            var folder = _folderManeger.GetMapFolderById(new ObjectId(folderId));
            Dictionary<ApplicationUser, MapPermissionType> users = new Dictionary<ApplicationUser, MapPermissionType>();
            foreach (var item in folder.Permissions.Users)
            {
                var user = UserManager.FindById(item.Key.ToString());
                users.Add(user, item.Value);
            }
            return users;
        }

        public Dictionary<Group, MapPermissionType> GetGroupsPermissions(string folderId)
        {
            var folder = _folderManeger.GetMapFolderById(new ObjectId(folderId));
            Dictionary<Group, MapPermissionType> groups = new Dictionary<Group, MapPermissionType>();
            foreach (var item in folder.Permissions.Groups)
            {
                var group = _groupManager.GetGroupById(item.Key);
                groups.Add(group, item.Value);
            }
            return groups;
        }

        public MapPermissionType GetGlobalPermission(string folderId)
        {
            var folder = _folderManeger.GetMapFolderById(new ObjectId(folderId));
            return folder.Permissions.AllUsers;
        }

        public MapPermissionType GetOwnerPermission(string folderId)
        {
            var folder = _folderManeger.GetMapFolderById(new ObjectId(folderId));
            return folder.Permissions.Owner.Value;
        }

        public bool IsFolderOwner(string folderId, string userId)
        {
            var folder = _folderManeger.GetMapFolderById(new ObjectId(folderId));
            return folder.Permissions.Owner.Key.ToString().Equals(userId);
        }

        public bool IsOwner(string folderId)
        {
            var folder = _folderManeger.GetMapFolderById(new ObjectId(folderId));
            return User.Identity.GetUserId().Equals(folder.Permissions.Owner.Key.ToString());
        }


        public KeyValuePair<ApplicationUser, MapPermissionType> GetFolderOwner(string folderId)
        {
            var folder = _folderManeger.GetMapFolderById(new ObjectId(folderId));
            var res = new KeyValuePair<ApplicationUser, MapPermissionType>(UserManager.FindById(folder.Permissions.Owner.Key.ToString()), folder.Permissions.Owner.Value);
            return res;
        }

        public MapPermissionType GetUserPermissionToFolderAsUser(string fId, string uId)
        {
            if (string.IsNullOrEmpty(fId) || string.IsNullOrEmpty(uId))
            {
                return MapPermissionType.NA;
            }

            var user = _userService.GetUserById(new ObjectId(uId));
            var folder = _folderManeger.GetMapFolderById(new ObjectId(fId));
            if (user == null || folder == null)
            {
                return MapPermissionType.NA;
            }

            if (folder.Permissions.Owner.Key == new ObjectId(user.Id))
            {
                return folder.Permissions.Owner.Value;
            }
            if (folder.Permissions.Users.Keys.Contains(new ObjectId(user.Id)))
            {
                return folder.Permissions.Users[new ObjectId(user.Id)];
            }
            return MapPermissionType.NA;
        }

        public MapPermissionType GetUserPermissionToFolderByGroup(string fId, string uId)
        {
            GroupController gc = new GroupController();
            var user = _userService.GetUserById(new ObjectId(uId));
            var folder = _folderManeger.GetMapFolderById(new ObjectId(fId));
            var groups = folder.Permissions.Groups.Where(x => gc.IsGroupMember(x.Key.ToString(), uId));
            var pers = groups.Select(x => x.Value).Distinct();
            if (pers.Contains(MapPermissionType.RW))
            {
                return MapPermissionType.RW;
            }
            else if (pers.Contains(MapPermissionType.RO))
            {
                return MapPermissionType.RO;
            }
            return MapPermissionType.NA;
        }

        public bool AddUserToFolder(string uId, string folderId, MapPermissionType per)
        {
            if (IsOwner(folderId) && _userService.IsUserlExist(new ObjectId(uId)))
            {
                var folder = _folderManeger.GetMapFolderById(new ObjectId(folderId));
                if (folder != null && folder.Creator.ToString() != uId)
                {
                    folder.Permissions.Users[new ObjectId(uId)] = per;
                    _folderManeger.UpdateMapFolder(folder);
                    return true;
                }
            }
            return false;
        }

        public bool AddGroupToFolder(string gId, string fId, MapPermissionType per)
        {
            if (_folderManeger.IsMapFolderExist(new ObjectId(fId)) && _groupManager.IsGroupExist(new ObjectId(gId)) && IsOwner(fId))
            {
                var folder = _folderManeger.GetMapFolderById(new ObjectId(fId));
                folder.Permissions.Groups[new ObjectId(gId)] = per;
                _folderManeger.UpdateMapFolder(folder);
                return true;
            }
            return false;
        }


        [HttpPost]
        public ActionResult AddNewUserToFolder(string fId, string Email)
        {
            MapPermissionType Permission = MapPermissionType.NA;
            var user = _userService.GetUserByEmail(Email);
            if (AddUserToFolder(user.Id, fId, Permission))
            {
                //return Json(new { success = true });
                return MyPermissions(fId);
            }
            return MyPermissions(fId);
        }


        public bool UpdateUserFolderPermission(string uId, string folderId, MapPermissionType per)
        {
            return AddUserToFolder(uId, folderId, per);
        }

        public bool UpdateGroupFolderPermission(string gId, string folderId, MapPermissionType per)
        {
            return AddGroupToFolder(gId, folderId, per);
        }

        public bool RemoveUserFromFolder(string uId, string folderId)
        {
            if (IsOwner(folderId) && _userService.IsUserlExist(new ObjectId(uId)))
            {
                var folder = _folderManeger.GetMapFolderById(new ObjectId(folderId));
                var ans = folder.Permissions.Users.Remove(new ObjectId(uId));
                _folderManeger.UpdateMapFolder(folder);
                return ans;
            }
            return false;
        }

        public bool RemoveGroupFromFolder(string gId, string folderId)
        {
            if (IsOwner(folderId) && _groupManager.IsGroupExist(new ObjectId(gId)))
            {
                var folder = _folderManeger.GetMapFolderById(new ObjectId(folderId));
                var ans = folder.Permissions.Groups.Remove(new ObjectId(gId));
                _folderManeger.UpdateMapFolder(folder);
                return ans;
            }
            return false;
        }



        /*
        public bool IsOwner(string id)
        {
            var uId = User.Identity.get
            ObjectId oId = new ObjectId(id);
            return 
        }
        */

        public JsonResult GetFolderUserPermissionList(string folderId, int? page, int? limit, string sortBy, string direction, string searchString = null)
        {
            List<FolderUserPermissionViewModel> mupvm = new List<FolderUserPermissionViewModel>();
            var users = _folderManeger.GetMapFolderById(new ObjectId(folderId)).Permissions.Users;

            // users
            foreach (var uId in users.Keys)
            {
                var u = _userService.GetUserById(uId);
                mupvm.Add(new FolderUserPermissionViewModel { FId = folderId, UId = uId.ToString(), Email = u.Email, FirstName = u.FirstName, LastName = u.LastName, Permission = users[uId], PermissionString = users[uId].ToString() });
            }

            // groups
            //var groups = _mapManager.GetMapById(new ObjectId(mapId)).Permissions.Groups;

            //foreach (var gId in groups.Keys)
            //{
            //    var g = _groupManager.GetGroupById(gId);
            //    mupvm.Add(new MapUserPermissionViewModel { MId = mapId, UId = gId.ToString(), Email = _userService.GetUserById(g.Creator).Email, Name = g.Name, Permission = groups[gId], PermissionString = groups[gId].ToString(), Type = "G" });
            //}

            var total = mupvm.Count();
            var records = mupvm.AsQueryable();;
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

        [HttpPost]
        public JsonResult SaveFolderUserPermission(FolderUserPermissionViewModel user)
        {
            var ans = UpdateUserFolderPermission(user.UId, user.FId, user.Permission);
            return Json(ans);
        }

        [HttpPost]
        public JsonResult RemoveFolderUserPermission(string userId, string folderId)
        {
            var ans = RemoveUserFromFolder(userId, folderId);
            return Json(ans);
        }

        [HttpPost]
        public JsonResult AddFolderUserPermission(string userEmail, string folderId, MapPermissionType permission)
        {
            if (_userService.IsEmailExist(userEmail))
            {
                var uId = _userService.GetUserByEmail(userEmail).Id;
                var ans = AddUserToFolder(uId, folderId, permission);
                return Json(ans);
            }
            return Json(false);
        }


        public JsonResult GetFolderGroupPermissionList(string folderId, int? page, int? limit, string sortBy, string direction, string searchString = null)
        {
            List<FolderGroupPermissionViewModel> mupvm = new List<FolderGroupPermissionViewModel>();
            var groups = _folderManeger.GetMapFolderById(new ObjectId(folderId)).Permissions.Groups;

            foreach (var gId in groups.Keys)
            {
                var g = _groupManager.GetGroupById(gId);
                mupvm.Add(new FolderGroupPermissionViewModel { FId = folderId, GId = gId.ToString(), Email = _userService.GetUserById(g.Creator).Email, Name = g.Name, Permission = groups[gId], PermissionString = groups[gId].ToString() });
            }
            var total = mupvm.Count();
            var records = mupvm.AsQueryable(); ;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                records = records.Where(x => x.Email.Contains(searchString) || x.Name.Contains(searchString) || x.Permission.ToString().Contains(searchString));
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


        [HttpPost]
        public JsonResult SaveFolderGroupPermission(string groupId, string folderId, MapPermissionType permission)
        {
            var ans = UpdateGroupFolderPermission(groupId, folderId, permission);
            return Json(ans);
        }

        [HttpPost]
        public JsonResult RemoveFolderGroupPermission(string groupId, string folderId)
        {
            var ans = RemoveGroupFromFolder(groupId, folderId);
            return Json(ans);
        }

        [HttpPost]
        public JsonResult AddFolderGroupPermission(string groupId, string folderId, MapPermissionType permission)
        {
            if (!string.IsNullOrEmpty(groupId) && !string.IsNullOrEmpty(folderId))
            {
                var ans = AddGroupToFolder(groupId, folderId, permission);
                return Json(ans);
            }
            return Json(false);
        }


        public ActionResult MyPermissions(string folderId)
        {
            FolderPermissionsViewModel folderPermissions = new FolderPermissionsViewModel();
            folderPermissions.Folder = _folderManeger.GetMapFolderById(new ObjectId(folderId));
            folderPermissions.Owner = GetFolderOwner(folderId);
            folderPermissions.Users = GetUsersPermissions(folderId);
            folderPermissions.Groups = GetGroupsPermissions(folderId);
            folderPermissions.AllUsers = GetGlobalPermission(folderId);

            if (IsFolderOwner(folderId, User.Identity.GetUserId()))
            {
                return PartialView("_MyFolderPermissionsView", folderPermissions);
            }
            else
            {
                return PartialView("_MyFolderNOPermissionsView", folderPermissions);
            }
        }

        public ActionResult GetGroupMapPermissions(string folderId)
        {
            var folder = _folderManeger.GetMapFolderById(new ObjectId(folderId));
            MapMiniViewModel mmvm = new MapMiniViewModel { Id = folder.Id.ToString(), Name = folder.Name, CreatorId = folder.Creator.ToString(), CreationTime = folder.CreationTime };

            if (IsFolderOwner(folderId, User.Identity.GetUserId()))
            {
                return PartialView("_MapGroupPermissionsView", mmvm);
            }
            else
            {
                return PartialView("_MapGroupNOPermissionsView", mmvm);
            }
        }
        
    }
}
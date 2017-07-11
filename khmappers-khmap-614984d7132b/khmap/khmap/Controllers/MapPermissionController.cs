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
    public class MapPermissionController : Controller
    {

        private MapDB _mapManager;
        private UserDB _userService;
        private GroupDB _groupManager;
        private ApplicationUserManager _userManager;

        public MapPermissionController()
        {
            this._mapManager = new MapDB(new Settings());
            this._userService = new UserDB(new Settings());
            this._groupManager = new GroupDB(new Settings());
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
        public ActionResult AddUserToMap(string mapId)
        {
            AddUserToMapViewModel m = new AddUserToMapViewModel { MId = mapId };
            return View(m);
        }

        [HttpPost]
        public ActionResult AddUserToMap(AddUserToMapViewModel model)
        {
            model.UId = UserManager.FindByEmail(model.Email).Id;
            if (AddUserToMap(model.UId.ToString(), model.MId.ToString(), model.Permission))
            {
                return View();
            }
            return View();
        }

        public Dictionary<ApplicationUser, MapPermissionType> GetUsersPermissions(string mapId)
        {
            var map = _mapManager.GetMapById(new ObjectId(mapId));
            Dictionary<ApplicationUser, MapPermissionType> users = new Dictionary<ApplicationUser, MapPermissionType>();
            foreach (var item in map.Permissions.Users)
            {
                var user = UserManager.FindById(item.Key.ToString());
                users.Add(user, item.Value);
            }
            return users;
        }

        public Dictionary<Group, MapPermissionType> GetGroupsPermissions(string mapId)
        {
            var map = _mapManager.GetMapById(new ObjectId(mapId));
            Dictionary<Group, MapPermissionType> groups = new Dictionary<Group, MapPermissionType>();
            foreach (var item in map.Permissions.Groups)
            {
                var group = _groupManager.GetGroupById(item.Key);
                groups.Add(group, item.Value);
            }
            return groups;
        }

        public MapPermissionType GetGlobalPermission(string mapId)
        {
            var map = _mapManager.GetMapById(new ObjectId(mapId));
            return map.Permissions.AllUsers;
        }

        public MapPermissionType GetOwnerPermission(string mapId)
        {
            var map = _mapManager.GetMapById(new ObjectId(mapId));
            return map.Permissions.Owner.Value;
        }

        public bool IsMapOwner(string mapId, string userId)
        {
            var map = _mapManager.GetMapById(new ObjectId(mapId));
            return map.Permissions.Owner.Key.ToString().Equals(userId);
        }

        public bool IsOwner(string mapId)
        {
            var map = _mapManager.GetMapById(new ObjectId(mapId));
            return User.Identity.GetUserId().Equals(map.Permissions.Owner.Key.ToString());
        }


        public KeyValuePair<ApplicationUser, MapPermissionType> GetMapOwner(string mapId)
        {
            var map = _mapManager.GetMapById(new ObjectId(mapId));
            var res = new KeyValuePair<ApplicationUser, MapPermissionType>(UserManager.FindById(map.Permissions.Owner.Key.ToString()), map.Permissions.Owner.Value);
            return res;
        }

        public MapPermissionType GetUserPermissionToMapAsUser(string mId, string uId)
        {
            if (string.IsNullOrEmpty(mId) || string.IsNullOrEmpty(uId))
            {
                return MapPermissionType.NA;
            }

            var user = _userService.GetUserById(new ObjectId(uId));
            var map = _mapManager.GetMapById(new ObjectId(mId));
            if (user == null || map == null)
            {
                return MapPermissionType.NA;
            }

            if (map.Permissions.Owner.Key == new ObjectId(user.Id))
            {
                return map.Permissions.Owner.Value;
            }
            if (map.Permissions.Users.Keys.Contains(new ObjectId(user.Id)))
            {
                return map.Permissions.Users[new ObjectId(user.Id)];
            }
            return MapPermissionType.NA;
        }

        public MapPermissionType GetUserPermissionToMapByGroup(string mId, string uId)
        {
            GroupController gc = new GroupController();
            var user = _userService.GetUserById(new ObjectId(uId));
            var map = _mapManager.GetMapById(new ObjectId(mId));
            var groups = map.Permissions.Groups.Where(x => gc.IsGroupMember(x.Key.ToString(), uId));
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

        public bool AddUserToMap(string uId, string mapId, MapPermissionType per)
        {
            if (IsOwner(mapId) && _userService.IsUserlExist(new ObjectId(uId)))
            {
                var map = _mapManager.GetMapById(new ObjectId(mapId));
                if (map != null && map.Creator.ToString() != uId)
                {
                    map.Permissions.Users[new ObjectId(uId)] = per;
                    _mapManager.UpdateMap(map);
                    return true;
                }
            }
            return false;
        }

        public bool AddGroupToMap(string gId, string mapId, MapPermissionType per)
        {
            if (_mapManager.IsMapExist(new ObjectId(mapId)) && _groupManager.IsGroupExist(new ObjectId(gId)) && IsOwner(mapId))
            {
                var map = _mapManager.GetMapById(new ObjectId(mapId));
                map.Permissions.Groups[new ObjectId(gId)] = per;
                _mapManager.UpdateMap(map);
                return true;
            }
            return false;
        }


        [HttpPost]
        public ActionResult AddNewUserToMap(string MId, string Email, MapPermissionType Permission = MapPermissionType.NA)
        {
            var user = _userService.GetUserByEmail(Email);
            if (AddUserToMap(user.Id, MId, Permission))
            {
                //return Json(new { success = true });
                return MyPermissions(MId);
            }
            return MyPermissions(MId);
        }


        public bool UpdateUserMapPermission(string uId, string mapId, MapPermissionType per)
        {
            return AddUserToMap(uId, mapId, per);
        }

        public bool UpdateGroupMapPermission(string gId, string mapId, MapPermissionType per)
        {
            return AddGroupToMap(gId, mapId, per);
        }

        public bool RemoveUserFromMap(string uId, string mapId)
        {
            if (IsOwner(mapId) && _userService.IsUserlExist(new ObjectId(uId)))
            {
                var map = _mapManager.GetMapById(new ObjectId(mapId));
                var ans = map.Permissions.Users.Remove(new ObjectId(uId));
                _mapManager.UpdateMap(map);
                return ans;
            }
            return false;
        }

        public bool RemoveGroupFromMap(string gId, string mapId)
        {
            if (IsOwner(mapId) && _groupManager.IsGroupExist(new ObjectId(gId)))
            {
                var map = _mapManager.GetMapById(new ObjectId(mapId));
                var ans = map.Permissions.Groups.Remove(new ObjectId(gId));
                _mapManager.UpdateMap(map);
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

        public JsonResult GetMapUserPermissionList(string mapId, int? page, int? limit, string sortBy, string direction, string searchString = null)
        {
            List<MapUserPermissionViewModel> mupvm = new List<MapUserPermissionViewModel>();
            var users = _mapManager.GetMapById(new ObjectId(mapId)).Permissions.Users;

            // users
            foreach (var uId in users.Keys)
            {
                var u = _userService.GetUserById(uId);
                mupvm.Add(new MapUserPermissionViewModel { MId = mapId, UId = uId.ToString(), Email = u.Email, FirstName = u.FirstName, LastName = u.LastName, Permission = users[uId], PermissionString = users[uId].ToString() });
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
        public JsonResult SaveMapUserPermission(MapUserPermissionViewModel user)
        {
            var ans = UpdateUserMapPermission(user.UId, user.MId, user.Permission);
            return Json(ans);
        }

        [HttpPost]
        public JsonResult RemoveMapUserPermission(string userId, string mapId)
        {
            var ans = RemoveUserFromMap(userId, mapId);
            return Json(ans);
        }

        [HttpPost]
        public JsonResult AddMapUserPermission(string userEmail, string mapId, MapPermissionType permission)
        {
            if (_userService.IsEmailExist(userEmail))
            {
                var uId = _userService.GetUserByEmail(userEmail).Id;
                var ans = AddUserToMap(uId, mapId, permission);
                return Json(ans);
            }
            return Json(false);
        }


        public JsonResult GetMapGroupPermissionList(string mapId, int? page, int? limit, string sortBy, string direction, string searchString = null)
        {
            List<MapGroupPermissionViewModel> mupvm = new List<MapGroupPermissionViewModel>();
            var groups = _mapManager.GetMapById(new ObjectId(mapId)).Permissions.Groups;

            foreach (var gId in groups.Keys)
            {
                var g = _groupManager.GetGroupById(gId);
                mupvm.Add(new MapGroupPermissionViewModel { MId = mapId, GId = gId.ToString(), Email = _userService.GetUserById(g.Creator).Email, Name = g.Name, Permission = groups[gId], PermissionString = groups[gId].ToString() });
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
        public JsonResult SaveMapGroupPermission(string groupId, string mapId, MapPermissionType permission)
        {
            var ans = UpdateGroupMapPermission(groupId, mapId, permission);
            return Json(ans);
        }

        [HttpPost]
        public JsonResult RemoveMapGroupPermission(string groupId, string mapId)
        {
            var ans = RemoveGroupFromMap(groupId, mapId);
            return Json(ans);
        }

        [HttpPost]
        public JsonResult AddMapGroupPermission(string groupId, string mapId, MapPermissionType permission)
        {
            if (!string.IsNullOrEmpty(groupId) && !string.IsNullOrEmpty(mapId))
            {
                var ans = AddGroupToMap(groupId, mapId, permission);
                return Json(ans);
            }
            return Json(false);
        }


        public ActionResult MyPermissions(string mapId)
        {
            MapPermissionsViewModel mapPermissions = new MapPermissionsViewModel();
            mapPermissions.Map = _mapManager.GetMapById(new ObjectId(mapId));
            mapPermissions.Owner = GetMapOwner(mapId);
            mapPermissions.Users = GetUsersPermissions(mapId);
            mapPermissions.Groups = GetGroupsPermissions(mapId);
            mapPermissions.AllUsers = GetGlobalPermission(mapId);

            if (IsMapOwner(mapId, User.Identity.GetUserId()))
            {
                return PartialView("_MyPermissionsView", mapPermissions);
            }
            else
            {
                return PartialView("_MyNOPermissionsView", mapPermissions);
            }
        }

        public ActionResult GetGroupMapPermissions(string mapId)
        {
            var map = _mapManager.GetMapById(new ObjectId(mapId));
            MapMiniViewModel mmvm = new MapMiniViewModel { Id = map.Id.ToString(), Name = map.Name, CreatorId = map.Creator.ToString(), CreationTime = map.CreationTime };

            if (IsMapOwner(mapId, User.Identity.GetUserId()))
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
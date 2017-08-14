using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using Microsoft.AspNet.Identity;
using khmap.DataBaseProviders;
using khmap.Models;

namespace khmap.Controllers
{
    public class MapFolderController : Controller
    {
        private MapFolderDB _mapFolderDataManager;
        private ApplicationUserManager _userManager;
        private MapDB _mapDataManager;
        private GroupDB _groupManager;
        //public static MapFolder STATIC_FOLDER;
        //public static MapFolder STATIC_INNER_FOLDER;

        public static string OWNED_SUPIRIOR = SharedCodedData.OWNED_SUPIRIOR;
        public static string SHARED_SUPIRIOR = SharedCodedData.SHARED_SUPIRIOR;
        public static string NOT_SUPIRIOR_BUT_SHARED = SharedCodedData.NOT_SUPIRIOR_BUT_SHARED;
        public static string NOT_SUPIRIOR_BUT_OWNED = SharedCodedData.NOT_SUPIRIOR_BUT_OWNED;



        public MapFolderController()
        {
            this._mapFolderDataManager = new MapFolderDB(new Settings());
            this._groupManager = new GroupDB(new Settings());
            this._mapDataManager = new MapDB(new Settings());
        }

        // GET: MapFolder
        public ActionResult Index(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var Folderid = new ObjectId(id);
                var Folder = _mapFolderDataManager.GetMapFolderById(Folderid);
                if (Folder != null)
                {
                    var userId = User.Identity.GetUserId();
                    bool isFollowing = Folder.Followers.Contains(new ObjectId(userId), new IDComparer());
                    ViewBag.isFollowing = isFollowing;
                    MapPermissionController mpc = new MapPermissionController();
                    // MapViewModel mvm = new MapViewModel { Id = map.Id, Name = map.Name, CreationTime = map.CreationTime, CreatorId = map.Creator, CreatorEmail = UserManager.GetEmail(map.Creator.ToString()), Description = map.Description, Model = map.Model, ModelArchive = map.MapsArchive };

                    if (Folder.Permissions.Owner.Key.ToString().Equals(userId))
                    {
                        return View();
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        /** public ActionResult createFolder(MapFolder parent)
         {
             var id = User.Identity.GetUserId();
             ObjectId UserId = new ObjectId(id);
             MapFolder toadd = new MapFolder();
             toadd.Creator = UserId;
         }**/


        public ActionResult MyFirstMapAndFolders()
        {
            try
            {
                var id = User.Identity.GetUserId();
                ObjectId UserId = new ObjectId(id);
                MapFolder superiorMapFolder = _mapFolderDataManager.GetSuperiorMapFolderOfUserOwned(UserId);
                if(superiorMapFolder == null)
                {
                  //  superiorMapFolder = createSuppFolderLocaly(OWNED_SUPIRIOR);
                    _mapFolderDataManager.AddFolder(superiorMapFolder);
                }
                var mapFolders = this._mapFolderDataManager.GetAllSubFolder(superiorMapFolder);
         
                //var mapFolders = this._mapFolderDataManager.GetFirstFoldersOfUser(UserId);
                var maps = this._mapFolderDataManager.GetAllMapsInFolder(superiorMapFolder);
                _mapDataManager = new MapDB(new Settings());
                ViewBag.maps = maps;
                ViewBag.currFolder = superiorMapFolder;
                return PartialView("_MyFoldersView", mapFolders);
            }
            catch (Exception e)
            {
                string exp = e.ToString();
                //return RedirectToAction("Index", "Home");
                _mapDataManager = new MapDB(new Settings());
                ViewBag.maps = _mapDataManager.GetAllMaps();
                return PartialView("_MyFoldersView", new List<MapFolder>());

            }
        }

        public ActionResult Details(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                MapFolderDB _mfManeger = new MapFolderDB(new Settings());
                UserDB _userManeger = new UserDB(new Settings());

                var mId = new ObjectId(id);
                var folder = _mfManeger.GetMapFolderById(mId);
                if (folder != null)
                {
                    var userId = User.Identity.GetUserId();
                    bool isFollowing = folder.Followers.Contains(new ObjectId(userId), new IDComparer());
                    ViewBag.isFollowing = isFollowing;
                    FolderPermissionController fpc = new FolderPermissionController();
                    MapViewModel mvm = new MapViewModel { Id = folder.Id, Name = folder.Name, CreationTime = folder.CreationTime, CreatorId = folder.Creator, CreatorEmail = "not supported yet"/*UserManager.GetEmail(map.Creator.ToString())*/, Description = folder.Description, Model = folder.Model, ModelArchive = null };
                    if (fpc.IsFolderOwner(id, User.Identity.GetUserId()))
                    {
                        return View(mvm);
                    }
                    else
                    {
                        return View(mvm);
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }


        public ActionResult MyFirstMapAndFoldersShared()
        {
            try
            {
                var id = User.Identity.GetUserId();
                ObjectId UserId = new ObjectId(id);
                MapFolder superiorMapFolder = _mapFolderDataManager.GetSuperiorMapFolderOfUserShared(UserId);
                if (superiorMapFolder == null)
                {
                 //   superiorMapFolder = createSuppFolderLocaly(SHARED_SUPIRIOR);
                    _mapFolderDataManager.AddFolder(superiorMapFolder);
                }
                var mapFolders = this._mapFolderDataManager.GetAllSubFolder(superiorMapFolder);

                //var mapFolders = this._mapFolderDataManager.GetFirstFoldersOfUser(UserId);
                var maps = this._mapFolderDataManager.GetAllMapsInFolder(superiorMapFolder);
                _mapDataManager = new MapDB(new Settings());
                ViewBag.maps = maps;
                ViewBag.currFolder = superiorMapFolder;
                return PartialView("_MyFoldersView", mapFolders);
            }
            catch (Exception e)
            {
                string exp = e.ToString();
                //return RedirectToAction("Index", "Home");
                _mapDataManager = new MapDB(new Settings());
                ViewBag.maps = _mapDataManager.GetAllMaps();
                return PartialView("_MyFoldersView", new List<MapFolder>());

            }
        }


        public ActionResult OpenFolder(string Id)
        {
            string userIdAsString = User.Identity.GetUserId();
            ObjectId userObjectID = new ObjectId(userIdAsString);
            ObjectId parentID = new ObjectId(Id);
            var parent = new MapFolderDB(new Settings()).GetMapFolderById(parentID);
            if(parent.Model["type"].Equals(SharedCodedData.OWNED_SUPIRIOR))
            {
                parent = _mapFolderDataManager.GetSuperiorMapFolderOfUserOwned(userObjectID);
                parentID = parent.Id;
            }
            if (parent.Model["type"].Equals(SharedCodedData.SHARED_SUPIRIOR))
            {
                parent = _mapFolderDataManager.GetSuperiorMapFolderOfUserShared(userObjectID);
                parentID = parent.Id;
            }
            ObjectId prevFolderID = parent.ParentDierctory;
            var allFoldersOfUser = _mapFolderDataManager.GetAllMapFoldersOfUser(userObjectID);
            var allFolderOwnedByUsr = new List<MapFolder>();
            foreach(var folder in allFoldersOfUser)
            {
                if (folder.Permissions.Owner.Key.ToString().Equals(userObjectID.ToString()) && !folder.Model["type"].Equals(SharedCodedData.SHARED_SUPIRIOR))
                {
                    allFolderOwnedByUsr.Add(folder);
                }
            }
            ViewBag.allFoldersOfUser = allFolderOwnedByUsr;
            var prevFolder = new MapFolderDB(new Settings()).GetMapFolderById(prevFolderID);
            var mapFolders = this._mapFolderDataManager.GetAllSubFolder(parent);
            var maps = this._mapFolderDataManager.GetAllMapsInFolder(parent);
            ViewBag.maps = maps;
            ViewBag.currFolder = parent;
            ViewBag.currFolderID = parent.Id;
            ViewBag.currFolderIDString = parent.Id.ToString();

            ViewBag.prevFolder = prevFolder;
            if (prevFolder == null)
            {
                ViewBag.prevFolderID = null;
            }
            else
            {
                ViewBag.prevFolderID = prevFolder.Id;
            }
            if (prevFolder != null)
            {
                List<MapFolder> prevFOlderInList = new List<MapFolder>() { prevFolder };
                mapFolders = prevFOlderInList.Union(mapFolders);
                List<MapFolder> viewList = mapFolders.ToList();
            }
            return PartialView("_MyFoldersView", mapFolders);

        }


        public ActionResult OpenFolderShared(string Id)
        {
            string userIdAsString = User.Identity.GetUserId();
            ObjectId userObjectID = new ObjectId(userIdAsString);
            ObjectId parentID = new ObjectId(Id);
            var parent = new MapFolderDB(new Settings()).GetMapFolderById(parentID);
            bool isCurrentFolderSupp = false;
            if (parent.Model["type"].Equals(SharedCodedData.SHARED_SUPIRIOR))
            {
                parent = _mapFolderDataManager.GetSuperiorMapFolderOfUserShared(userObjectID);
                parentID = parent.Id;
                isCurrentFolderSupp = true;
            }
            ObjectId prevFolderID = parent.ParentDierctory;
            var prevFolder = new MapFolderDB(new Settings()).GetMapFolderById(prevFolderID);
            if (prevFolder != null)
            {
                bool isPrevCorrect = false;
                if (prevFolder.Permissions.Users.Keys.Contains(userObjectID))
                {
                    isPrevCorrect = true;
                }
                else
                {
                    foreach (var groupId in prevFolder.Permissions.Groups.Keys)
                    {
                        var group = _groupManager.GetGroupById(groupId);
                        if (group.Members.Keys.Contains(userObjectID))
                        {
                            isPrevCorrect = true;
                            break;
                        }
                    }
                }
                if (!isPrevCorrect)
                {
                    prevFolder = _mapFolderDataManager.GetSuperiorMapFolderOfUserShared(userObjectID);
                    prevFolderID = prevFolder.Id;
                }
            }
            var mapFolders = _mapFolderDataManager.GetAllSubFolder(parent);
            if (isCurrentFolderSupp)
            {
                mapFolders = this._mapFolderDataManager.GetAllFolders();
            }
            var finalMapFolders = new List<MapFolder>();
            foreach(var mapFolder in mapFolders)
            {
                if (mapFolder.Id.Equals(parent.Id))
                {
                    continue;
                }
                if (mapFolder.Permissions.Owner.Key.Equals(userObjectID))
                {
                    continue;
                }
                if (mapFolder.Permissions.Users.Keys.Contains(userObjectID))
                {
                    finalMapFolders.Add(mapFolder);
                }
                else
                {
                    foreach(var groupId in mapFolder.Permissions.Groups.Keys)
                    {
                        var group = _groupManager.GetGroupById(groupId);
                        if (group.Members.Keys.Contains(userObjectID))
                        {
                            finalMapFolders.Add(mapFolder);
                        }
                    }
                }
            }

            mapFolders = finalMapFolders;

            var maps = new List<Map>();
            if (parent.Model["type"].Equals(SharedCodedData.SHARED_SUPIRIOR))
            {
                var allMaps = _mapDataManager.GetAllMaps();
                foreach(var tempMap in allMaps)
                {
                    if (tempMap.Permissions.Owner.Key.Equals(userObjectID))
                    {
                        continue;
                    }
                    else if (tempMap.Permissions.Users.Keys.Contains(userObjectID))
                    {
                        maps.Add(tempMap);
                    }
                    else
                    {
                        var groups = _groupManager.GetAllGroupsOfUser(userObjectID);
                        foreach(var group in groups)
                        {
                            if (tempMap.Permissions.Groups.Keys.Contains(group.Id))
                            {
                                maps.Add(tempMap);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                maps = this._mapFolderDataManager.GetAllMapsInFolder(parent).ToList();
            }
            
            ViewBag.maps = maps;
            ViewBag.currFolder = parent;
            ViewBag.currFolderID = parent.Id;
            ViewBag.currFolderIDString = parent.Id.ToString();

            ViewBag.prevFolder = prevFolder;
            if (prevFolder == null)
            {
                ViewBag.prevFolderID = null;
            }
            else
            {
                ViewBag.prevFolderID = prevFolder.Id;
            }
            if (prevFolder != null)
            {
                List<MapFolder> prevFOlderInList = new List<MapFolder>() { prevFolder };
                mapFolders = prevFOlderInList.Union(mapFolders);
                List<MapFolder> viewList = mapFolders.ToList();
            }
            return PartialView("_MySharedFoldersView", mapFolders);

        }

        public void addNewFolder(string parentID, string folderName, string folderDescription)
        {
            MapFolderDB folderManeger = new MapFolderDB(new Settings());
            var id = User.Identity.GetUserId();
            ObjectId UserId = new ObjectId(id);
            if (parentID == null || parentID.Equals(""))
            {
                parentID = folderManeger.GetSuperiorMapFolderOfUser(UserId).Id.ToString();
            }
            _mapFolderDataManager = new MapFolderDB(new Settings());


            MapPermissions mapPermissions = new MapPermissions();
            mapPermissions.Owner = new KeyValuePair<ObjectId, MapPermissionType>(UserId, MapPermissionType.RW);
            mapPermissions.Users = new Dictionary<ObjectId, MapPermissionType>();
            mapPermissions.Groups = new Dictionary<ObjectId, MapPermissionType>();
            mapPermissions.AllUsers = MapPermissionType.NA;

            mapPermissions.Users.Add(UserId, MapPermissionType.RW);

            MapFolder folder = new MapFolder();
            folder.Name = folderName;
            folder.Creator = UserId;
            folder.CreationTime = DateTime.Now;
            folder.Description = folderDescription;
            folder.Followers = new HashSet<ObjectId>();
            folder.Permissions = mapPermissions;
            folder.idOfMapsInFolder = new HashSet<ObjectId>();
            folder.idOfSubFolders = new HashSet<ObjectId>();
            folder.ParentDierctory = new ObjectId(parentID);
            MapFolder parentFolder = folderManeger.GetMapFolderById(folder.ParentDierctory);
            string path = parentFolder.Model["path"].ToString();
            if (path.Equals(""))
            {
                path = parentFolder.Name;
            }
            else
            {
                path = path + "/" + parentFolder.Name;
            }
            if ((parentFolder.Model["type"]).Equals("sharedSup") || (parentFolder.Model["type"]).Equals("shared"))
            {
                folder.Model = new BsonDocument { { "type", NOT_SUPIRIOR_BUT_SHARED }, {"path", path} };
            }
            else
            {
                folder.Model = new BsonDocument { { "type", NOT_SUPIRIOR_BUT_OWNED }, { "path", path } };

            }

            folderManeger.AddSubFolder(parentFolder, folder);
        }

        public void MoveFolderToFolder (string folderToMoveId, string moveToFolderId)
        {
            if (!folderToMoveId.Equals(moveToFolderId))
            {

                MapFolder folderToMove = _mapFolderDataManager.GetMapFolderById(new ObjectId(folderToMoveId));
                MapFolder moveToFolder = _mapFolderDataManager.GetMapFolderById(new ObjectId(moveToFolderId));
                MapFolder oldPrevFolder = _mapFolderDataManager.GetMapFolderById(folderToMove.ParentDierctory);
                if (moveToFolder.Id.Equals(oldPrevFolder.Id))
                {
                    return;
                }
                oldPrevFolder.idOfSubFolders.Remove(folderToMove.Id);
                folderToMove.ParentDierctory = moveToFolder.Id;
                moveToFolder.idOfSubFolders.Add(folderToMove.Id);

                _mapFolderDataManager.UpdateMapFolder(folderToMove);
                _mapFolderDataManager.UpdateMapFolder(moveToFolder);
                _mapFolderDataManager.UpdateMapFolder(oldPrevFolder);
            }
        }

        public void MoveMapToFolder(string mapToMoveId, string moveToFolderId)
        {
            Map mapToMove = _mapDataManager.GetMapById(new ObjectId(mapToMoveId));
            MapFolder moveToFolder = _mapFolderDataManager.GetMapFolderById(new ObjectId(moveToFolderId));


            MapFolder oldPrevFolder = null;
            string userIdAsString = User.Identity.GetUserId();
            ObjectId userObjectID = new ObjectId(userIdAsString);
            var allFoldersOfUser = _mapFolderDataManager.GetAllMapFoldersOfUser(userObjectID);
            var allFolderOwnedByUsr = new List<MapFolder>();

            foreach (var folder in allFoldersOfUser)
            {
                if (folder.Permissions.Owner.Key.ToString().Equals(userObjectID.ToString()) && !folder.Model["type"].Equals(SharedCodedData.SHARED_SUPIRIOR))
                {
                    if (folder.idOfMapsInFolder.Contains(mapToMove.Id))
                    {
                        oldPrevFolder = folder;
                        break;
                    }
                }
            }
            if (oldPrevFolder.Id.Equals(moveToFolder.Id))
            {
                return;
            }
            oldPrevFolder.idOfMapsInFolder.Remove(mapToMove.Id);
            moveToFolder.idOfMapsInFolder.Add(mapToMove.Id);


            _mapDataManager.UpdateMap(mapToMove);
            _mapFolderDataManager.UpdateMapFolder(moveToFolder);
            _mapFolderDataManager.UpdateMapFolder(oldPrevFolder);
        }

        public void deleteFolder(string currFolder)
        {

            MapFolderDB folderManeger = new MapFolderDB(new Settings());
            ObjectId currFolderID = new ObjectId(currFolder);
            MapFolder currentFolder = folderManeger.GetMapFolderById(currFolderID);
            try
            {
                foreach (ObjectId idOfSubFolder in currentFolder.idOfSubFolders)
                {
                    deleteFolder(idOfSubFolder.ToString());
                }
            }
            catch
            {

            }
            ObjectId prevFolderID = currentFolder.ParentDierctory;
            MapFolder prevFolder = folderManeger.GetMapFolderById(prevFolderID);
            foreach(ObjectId objID in prevFolder.idOfSubFolders)
            {
                if (currFolderID.Equals(objID))
                {
                    currFolderID = objID;
                }
            }
            prevFolder.idOfSubFolders.Remove(currFolderID);
            folderManeger.UpdateMapFolder(prevFolder);
            folderManeger.RemoveMapFolderById(currFolderID);            
            ViewBag.go2 = prevFolderID.ToString();
        }

        public ActionResult Delete(string id)
        {
            try
            {
                // deleteFolder(id);
                ViewBag.folderID = id;
                var folder = new MapFolderDB(new Settings()).GetMapFolderById(new ObjectId(id));
                ViewBag.prevFolderID = folder.ParentDierctory.ToString();
                var map = this._mapFolderDataManager.GetMapFolderById(new ObjectId(id));
                if (IsValidId(id) && IsValidMap(map) )
                {
                    var mdvm = new MapDeleteViewModel { Id = map.Id.ToString(), Name = map.Name, CreatorEmail = "", CreationTime = map.CreationTime, Description = map.Description };
                    return View(mdvm);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        private bool IsValidId(string id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                return false;
            }
            return true;

        }

        private bool IsValidMap(MapFolder folder)
        {
            return folder != null;
        }

        public ActionResult GetUserFolderNames(string folderId)
        {
            var userID = User.Identity.GetUserId();
            var folders = _mapFolderDataManager.GetAllMapFoldersOfUser(new ObjectId(userID));
            var namesList = folders.Select(u => new GroupNameViewModel { GroupId = u.Id.ToString(), Name = u.Name });
            var temp = new List<GroupNameViewModel>();
            foreach(var name in namesList)
            {
                var tempFolder = _mapFolderDataManager.GetMapFolderById(new ObjectId(name.GroupId));
                if (!name.GroupId.ToString().Equals(folderId) && !tempFolder.Model["type"].ToString().Equals(SharedCodedData.SHARED_SUPIRIOR))
                {
                    string path = tempFolder.Model["path"].ToString();
                    if (path.Equals(""))
                    {
                        path = tempFolder.Name;
                    }
                    else
                    {
                        path = path + "/" + tempFolder.Name;
                    }
                    temp.Add(new GroupNameViewModel { GroupId = tempFolder.Id.ToString(), Name =  path});
                }
            }
            namesList = temp;
            return Json(namesList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(string id)
        {
            if (!IsValidId(id))
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var folder = _mapFolderDataManager.GetMapFolderById(new ObjectId(id));
                var prevFolder =_mapFolderDataManager.GetMapFolderById(folder.ParentDierctory);
                var userID = User.Identity.GetUserId();
                if (IsValidMap(folder) && folder.Permissions.Owner.Key.ToString().Equals(userID.ToString()))
                {
                    var mevm = new MapEditViewModel { Id = folder.Id.ToString(), Name = folder.Name, Description = folder.Description, Path = folder.Model["path"].ToString() };
                    return View(mevm);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Map/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MapEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var folder = _mapFolderDataManager.GetMapFolderById(new ObjectId(model.Id));
                    folder.Name = model.Name;
                    folder.Description = model.Description;
                    string path = model.Path;
                    folder.Model["path"] = path;
                    int index = path.LastIndexOf("/");
                    string prevPath = path.Substring(0, index);
                    string prevFolderName = path.Substring(index+1);
                    var userID = User.Identity.GetUserId();
                    var allFoldersOfUser = _mapFolderDataManager.GetAllMapFoldersOfUser(new ObjectId(userID));
                    
                    foreach (var tempFolder in allFoldersOfUser)//tempFolder represents the folder that MIGHT be the new prev folder
                    {
                        if (tempFolder.Name.Equals(prevFolderName) && prevPath.Equals(tempFolder.Model["path"].ToString()))
                        {
                            var prevFolder = _mapFolderDataManager.GetMapFolderById(folder.ParentDierctory);
                            prevFolder.idOfSubFolders.Remove(folder.Id);
                            tempFolder.idOfSubFolders.Add(folder.Id);
                            folder.ParentDierctory = tempFolder.Id;
                            _mapFolderDataManager.UpdateMapFolder(prevFolder);
                            _mapFolderDataManager.UpdateMapFolder(tempFolder);
                            _mapFolderDataManager.UpdateMapFolder(folder);
                            return RedirectToAction("Details", "MapFolder", new { id = model.Id });

                        }
                    }
   //                 _mapFolderDataManager.UpdateMapFolder(folder);
     //               return RedirectToAction("Details", "MapFolder", new { id = model.Id });
                }
                catch
                {
                    return View(model);
                }
            }
            return View(model);
        }



    }
}
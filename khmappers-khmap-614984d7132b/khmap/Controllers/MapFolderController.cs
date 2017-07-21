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
                    if (mpc.IsMapOwner(id, User.Identity.GetUserId()))
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

       private MapFolder createSuppFolderLocaly(string whichSupp)
        {
            _mapFolderDataManager = new MapFolderDB(new Settings());

            var id = User.Identity.GetUserId();
            ObjectId UserId = new ObjectId(id);

            MapPermissions mapPermissions = new MapPermissions();
            mapPermissions.Owner = new KeyValuePair<ObjectId, MapPermissionType>(UserId, MapPermissionType.RW);
            mapPermissions.Users = new Dictionary<ObjectId, MapPermissionType>();
            mapPermissions.Groups = new Dictionary<ObjectId, MapPermissionType>();
            mapPermissions.AllUsers = MapPermissionType.NA;

            mapPermissions.Users.Add(UserId, MapPermissionType.RW);

            MapFolder suppFolder = new MapFolder();
            suppFolder.Name = "suppFolder" + whichSupp;
            suppFolder.Creator = UserId;
            suppFolder.CreationTime = DateTime.Now;
            suppFolder.Description = "Supirior Folder "+ whichSupp;
            suppFolder.Followers = new HashSet<ObjectId>();
            suppFolder.Permissions = mapPermissions;
            suppFolder.idOfMapsInFolder = new HashSet<ObjectId>();
            suppFolder.idOfSubFolders = new HashSet<ObjectId>();
            suppFolder.ParentDierctory = new ObjectId();
            suppFolder.FirstFolderOfUser = UserId;
            suppFolder.Model = new BsonDocument { { "type", whichSupp } };
            var maps = new MapDB(new Settings()).GetAllMaps();
            foreach (Map map in maps)
            {
                suppFolder.idOfMapsInFolder.Add(map.Id);
            }
            return suppFolder;

        }

        public ActionResult MyFirstMapAndFolders()
        {
            try
            {
                var id = User.Identity.GetUserId();
                ObjectId UserId = new ObjectId(id);
                MapFolder superiorMapFolder = _mapFolderDataManager.GetSuperiorMapFolderOfUserOwned(UserId);
                if(superiorMapFolder == null)
                {
                    superiorMapFolder = createSuppFolderLocaly(OWNED_SUPIRIOR);
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


        public ActionResult MyFirstMapAndFoldersShared()
        {
            try
            {
                var id = User.Identity.GetUserId();
                ObjectId UserId = new ObjectId(id);
                MapFolder superiorMapFolder = _mapFolderDataManager.GetSuperiorMapFolderOfUserShared(UserId);
                if (superiorMapFolder == null)
                {
                    superiorMapFolder = createSuppFolderLocaly(SHARED_SUPIRIOR);
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
           // var folders = new MapFolderDB(new Settings()).GetFirstFoldersOfUser(userObjectID);

            // ObjectId FolderId = new ObjectId(toOpenId);
            //var FolderId = new ObjectId(Id);
            //var parent = this._mapFolderDataManager.GetMapFolderById(FolderId);
            ObjectId parentID = new ObjectId(Id);
            var parent = new MapFolderDB(new Settings()).GetMapFolderById(parentID);
            ObjectId prevFolderID = parent.ParentDierctory;
            var prevFolder = new MapFolderDB(new Settings()).GetMapFolderById(prevFolderID);
            //MapFolder superiorMapFolder = _mapFolderDataManager.GetSuperiorMapFolderOfUser(UserId);
            var mapFolders = this._mapFolderDataManager.GetAllSubFolder(parent);
            //var mapFolders = this._mapFolderDataManager.GetAllSubFolder(parent);
            //mapFolders = new List<MapFolder>();
            var maps = this._mapFolderDataManager.GetAllMapsInFolder(parent);
            ViewBag.maps = maps;
            ViewBag.currFolder = parent;
            ViewBag.currFolderID = parent.Id;
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
            if ((parentFolder.Model["type"]).Equals("sharedSup") || (parentFolder.Model["type"]).Equals("shared"))
            {
                folder.Model = new BsonDocument { { "type", NOT_SUPIRIOR_BUT_SHARED } };
            }
            else
            {
                folder.Model = new BsonDocument { { "type", NOT_SUPIRIOR_BUT_OWNED } };

            }
            folderManeger.AddSubFolder(parentFolder, folder);
        }

        public void deleteFolder(string currFolder)
        {
            MapFolderDB folderManeger = new MapFolderDB(new Settings());
            ObjectId currFolderID = new ObjectId(currFolder);
            MapFolder currentFolder = folderManeger.GetMapFolderById(currFolderID);
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


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using khmap.DataBaseProviders;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MongoDB.Bson;
using khmap.Models;
using Newtonsoft.Json;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Web;
using System.Net;

namespace khmap.Controllers
{
    [Authorize]
    public class MapController : Controller
    {
        private MapDB _mapManager;
        private GroupDB _groupManager;
        private ApplicationUserManager _userManager;

        public MapController()
        {
            this._mapManager = new MapDB(new Settings());
            this._groupManager = new GroupDB(new Settings());
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


        // GET: Map
        public ActionResult Index(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var mId = new ObjectId(id);
                var map = _mapManager.GetMapById(mId);
                if (map != null)
                {
                    var userId = User.Identity.GetUserId();
                    bool isFollowing = map.Followers.Contains(new ObjectId(userId), new IDComparer());
                    ViewBag.isFollowing = isFollowing;
                    MapPermissionController mpc = new MapPermissionController();
                    MapViewModel mvm = new MapViewModel { Id = map.Id, Name = map.Name, CreationTime = map.CreationTime, CreatorId = map.Creator, CreatorEmail = UserManager.GetEmail(map.Creator.ToString()), Description = map.Description, Model = map.Model, ModelArchive = map.MapsArchive };
                    if (mpc.IsMapOwner(id, User.Identity.GetUserId()))
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

        // GET: Map/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMap(SaveMapViewModel newMapModel)
        {
            if (ModelState.IsValid)
            {
                var id = User.Identity.GetUserId();
                ObjectId oId = new ObjectId(id);

                MapPermissions mapPermissions = new MapPermissions();
                mapPermissions.Owner = new KeyValuePair<ObjectId, MapPermissionType>(oId, MapPermissionType.RW);
                mapPermissions.Users = new Dictionary<ObjectId, MapPermissionType>();
                mapPermissions.Groups = new Dictionary<ObjectId, MapPermissionType>();
                mapPermissions.AllUsers = MapPermissionType.NA;

                mapPermissions.Users.Add(oId, MapPermissionType.RW);

                BsonDocument bMapModel = BsonDocument.Parse(newMapModel.Model);
                //BsonDocument bMapModel = mapModel.ToBsonDocument();

                Queue<MapVersion> versions = new Queue<MapVersion>(MapVersion.VERSIONS);
                versions.Enqueue(new MapVersion { CreationTime = DateTime.Now, Model = bMapModel });

                Map map = new Map { Name = newMapModel.Name, Creator = oId, CreationTime = DateTime.Now, Description = newMapModel.Description, Model = bMapModel, Permissions = mapPermissions, MapsArchive = versions, Followers = new HashSet<ObjectId>() };
                var mId = _mapManager.AddMap(map);


                return Json(new { mapId = mId, mapName = map.Name }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false });
        }

        //[HttpPost]
        //public ActionResult CreateMap(string mapModel, string mapName, string mapDescription)
        //{
        //    //return Content("lalala");

        //    var id = User.Identity.GetUserId();
        //    ObjectId oId = new ObjectId(id);

        //    MapPermissions mapPermissions = new MapPermissions();
        //    mapPermissions.Owner = new KeyValuePair<ObjectId, MapPermissionType>(oId, MapPermissionType.RW);
        //    mapPermissions.Users = new Dictionary<ObjectId, MapPermissionType>();
        //    mapPermissions.Groups = new Dictionary<ObjectId, MapPermissionType>();
        //    mapPermissions.AllUsers = MapPermissionType.NA;

        //    mapPermissions.Users.Add(oId, MapPermissionType.RW);

        //    BsonDocument bMapModel = BsonDocument.Parse(mapModel);
        //    //BsonDocument bMapModel = mapModel.ToBsonDocument();

        //    Queue<MapVersion> versions = new Queue<MapVersion>(MapVersion.VERSIONS);
        //    versions.Enqueue(new MapVersion { CreationTime = DateTime.Now, Model = bMapModel });

        //    Map map = new Map { Name = mapName, Creator = oId, CreationTime = DateTime.Now, Description = mapDescription, Model = bMapModel, Permissions = mapPermissions, MapsArchive = versions, Followers = new HashSet<ObjectId>() };
        //    var mId = _mapManager.AddMap(map);


        //    return Json(mId, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult UpdateMap(string mapId, string mapModel)
        {
            if (!string.IsNullOrEmpty(mapId) && !string.IsNullOrEmpty(mapModel))
            {
                try
                {
                    var map = _mapManager.GetMapById(new ObjectId(mapId));
                    map.Model = BsonDocument.Parse(mapModel);
                    _mapManager.UpdateMap(map);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
                }
                
            }
            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        }

        // GET: Map/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Map/Create
        [HttpPost]
        public ActionResult Create(MapCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var id = User.Identity.GetUserId();
                ObjectId oId = new ObjectId(id);

                MapPermissions mapPermissions = new MapPermissions();
                mapPermissions.Owner = new KeyValuePair<ObjectId,MapPermissionType>(oId, MapPermissionType.RW);
                mapPermissions.Users = new Dictionary<ObjectId,MapPermissionType>();
                mapPermissions.Groups = new Dictionary<ObjectId,MapPermissionType>();
                mapPermissions.AllUsers = MapPermissionType.NA;

                BsonDocument mapModel = new BsonDocument();

                Queue<MapVersion> versions = new Queue<MapVersion>(MapVersion.VERSIONS);
                versions.Enqueue(new MapVersion { CreationTime = DateTime.Now, Model = mapModel });

                Map map = new Map { Name = model.Name, Creator = oId, CreationTime = DateTime.Now, Description = model.Description, Model = mapModel, Permissions = mapPermissions, MapsArchive = versions };
                _mapManager.AddMap(map);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        // GET: Map/Edit/5
        public ActionResult Edit(string id)
        {
            if (!IsValidId(id))
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var map = _mapManager.GetMapById(new ObjectId(id));
                if (IsValidMap(map) && _mapManager.IsMapOwner(map.Id.ToString(), User.Identity.GetUserId()))
                {
                    var mevm = new MapEditViewModel { Id = map.Id.ToString(), Name = map.Name, Description = map.Description };
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
                    var map = _mapManager.GetMapById(new ObjectId(model.Id));
                    map.Name = model.Name;
                    map.Description = model.Description;
                    _mapManager.UpdateMap(map);
                    return RedirectToAction("Index", "Map", new { id = model.Id });
                }
                catch
                {
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: Map/Delete/5
        public ActionResult Delete(string id)
        {
            try
            {
                var map = _mapManager.GetMapById(new ObjectId(id));
                if (IsValidId(id) && IsValidMap(map) && _mapManager.IsMapOwner(map.Id.ToString(), User.Identity.GetUserId()))
                {
                    var mdvm = new MapDeleteViewModel { Id = map.Id.ToString(), Name = map.Name, CreatorEmail = UserManager.GetEmail(map.Creator.ToString()), CreationTime = map.CreationTime, Description = map.Description };
                    return View(mdvm);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Map/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(MapDeleteViewModel model)
        {
            try
            {
                _mapManager.RemoveMap(new ObjectId(model.Id));
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult MyMaps()
        {
            try
            {
                var id = User.Identity.GetUserId();
                ObjectId oId = new ObjectId(id);
                var maps = _mapManager.GetMapsByCreatorId(oId);
                return PartialView("_MyMapsView", maps);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }
        
        public ActionResult MyFoundMaps(IEnumerable<Map> foundMaps)
        {
            try
            {
                //gettign intersection between created maps and found maps
                var id = User.Identity.GetUserId();
                ObjectId oId = new ObjectId(id);
                var mapsByUser = _mapManager.GetMapsByCreatorId(oId);
                var userFoundMaps = foundMaps.Intersect(mapsByUser, new MapComparer());

                /*getting intesection between shared maps and found maps

                id = User.Identity.GetUserId();
                oId = new ObjectId(id);
                mapsByUser = _mapManager.GetSharedMapsById(oId);

                GroupController gc = new GroupController();
                var groups = gc.GetGroupsByUserId(id);
                var mapsByGroups = _mapManager.GetAllMapContainsGroupsNotOwned(id, groups);

                var sharedMaps = mapsByUser.Union(mapsByGroups, new MapComparer());

                sharedMaps = sharedMaps.Intersect(foundMaps);

                //getting the union between shared found maps and created found maps
                
                var maps = userFoundMaps.Union(sharedMaps, new MapComparer());
                */
                var maps = userFoundMaps;



                return PartialView("_MyMapsView", maps);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult MySharedMaps()
        {
            try
            {
                var id = User.Identity.GetUserId();
                ObjectId oId = new ObjectId(id);
                var mapsByUser = _mapManager.GetSharedMapsById(oId);

                GroupController gc = new GroupController();
                var groups = gc.GetGroupsByUserId(id);
                var mapsByGroups = _mapManager.GetAllMapContainsGroupsNotOwned(id, groups);

                var maps = mapsByUser.Union(mapsByGroups, new MapComparer());
                return PartialView("_MyMapsView", maps);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public ActionResult LauchMap(string id)
        {
            var map = _mapManager.GetMapById(new ObjectId(id));

            var m = map.Model.ToJson();
            var mm = "{ 'class': 'go.GraphLinksModel',  'linkFromPortIdProperty': 'fromPort',  'linkToPortIdProperty': 'toPort',  'nodeDataArray': [ {'text':'Task', 'figure':'Rectangle', 'key':-2, 'loc':'-549 124'},{'text':'Quality', 'figure':'Ellipse', 'key':-1, 'loc':'-664.9999999999998 -104.00000000000006'} ],  'linkDataArray': [ {'from':-1, 'to':-2, 'fromPort':'L', 'toPort':'T', 'text':'Association', 'points':[-701.1104651162791,-104,-711.1104651162791,-104,-711.1104651162791,11.686918604651169,-549,11.686918604651169,-549,101.65,-549,111.65], 'curve':{'Yb':'JumpOver', 'xH':11}} ]}".ToJson();

            ViewBag.mapString = m;

            return View(map);
        }
        public ActionResult LaunchMap2(string id)
        {
            var map = _mapManager.GetMapById(new ObjectId(id));
            var m = map.Model.ToJson();
            ViewBag.mapString = m;
            return View(map);
        }

        public ActionResult LaunchMap3(string id)
        {
            if (id != null)
            {
                var map = _mapManager.GetMapById(new ObjectId(id));
                var m = map.Model.ToJson();
                ViewBag.mapString = "";
                ViewBag.diagramObjects = "";
                ViewBag.input = map.Model.ToJson();
                ViewBag.mapName = map.Name;
                return View(map);
            }
            else
            {
                ViewBag.input = null;
            }
            return View();
        }

        public ActionResult lm()
        {
            return View();
        }

        public ActionResult LaunchMap4(string id)
        {
            if (id != null)
            {

                var map = _mapManager.GetMapById(new ObjectId(id));
                ViewBag.isSaved = true;
                ViewBag.myMap = map.Model.ToJson().ToString();

                var refIDs = GetReferencesIDsFromMapModel(map.Model.ToJson());
                var refsDict = GetReferencesFromIDs(refIDs);
                ViewBag.refs = refsDict;
                //string t = map.Model.ToJson().ToString();
                //ViewBag.myMap = JsonConvert.SerializeObject(map.Model.ToJson());
                return View(map);
            }
            else
            {
                ViewBag.isSaved = false;
                var map = new Map { Name = "(Unsaved File)", Model = null };
                ViewBag.myMap = "{}".ToJson().ToString();
                return View(map);
            }
        }

        public ActionResult LaunchMap6(string id)
        {
            if (id != null)
            {

                var map = _mapManager.GetMapById(new ObjectId(id));
                ViewBag.isSaved = true;
                ViewBag.myMap = map.Model.ToJson().ToString();

                var refIDs = GetReferencesIDsFromMapModel(map.Model.ToJson());
                var refsDict = GetReferencesFromIDs(refIDs);
                ViewBag.refs = refsDict;
                //string t = map.Model.ToJson().ToString();
                //ViewBag.myMap = JsonConvert.SerializeObject(map.Model.ToJson());
                return View(map);
            }
            else
            {
                ViewBag.isSaved = false;
                var map = new Map { Name = "(Unsaved File)", Model = null };
                ViewBag.myMap = "{}".ToJson().ToString();
                return View(map);
            }
        }


        public ActionResult LaunchMap5(string id)
        {
            if (id != null && !_mapManager.IsMapExist(new ObjectId(id)))
            {
                return RedirectToAction("index", "Home");
            }

            if (id != null)
            {
                var userId = User.Identity.GetUserId();
                MapPermissionController mpc = new MapPermissionController();
                MapPermissionType userPermissionsAsUser = mpc.GetUserPermissionToMapAsUser(id, userId);
                MapPermissionType userPermissionsByGroup = mpc.GetUserPermissionToMapByGroup(id, userId);
                MapPermissionType userPermissions = MapPermissionType.NA;

                if (userPermissionsAsUser == MapPermissionType.RW || userPermissionsByGroup == MapPermissionType.RW)
                {
                    userPermissions = MapPermissionType.RW;
                }
                else if (userPermissionsAsUser == MapPermissionType.RO || userPermissionsByGroup == MapPermissionType.RO)
                {
                    userPermissions = MapPermissionType.RO;
                }
                else
                {
                    return RedirectToAction("index", "Home");
                }

                ViewBag.userPermission = userPermissions;

                var map = _mapManager.GetMapById(new ObjectId(id));
                ViewBag.isSaved = true;
                ViewBag.myMap = map.Model.ToJson().ToString();

                //var refIDs = GetReferencesIDsFromMapModel(map.Model.ToJson());
                //var refsDict = GetReferencesFromIDs(refIDs);
                //ViewBag.refs = refsDict;
                //string t = map.Model.ToJson().ToString();
                //ViewBag.myMap = JsonConvert.SerializeObject(map.Model.ToJson());
                return View(map);
            }
            else
            {
                ViewBag.userPermission = MapPermissionType.RW;
                ViewBag.isSaved = false;
                var map = new Map { Name = "(Unsaved File)", Model = null };
                ViewBag.myMap = "{}".ToJson().ToString();
                return View(map);
            }
        }

        [HttpPost]
        public ActionResult LoadMapFromFile(HttpPostedFileBase file = null)
        {
            ViewBag.userPermission = MapPermissionType.RW;
            ViewBag.isSaved = false;
            var map = new Map { Name = "(Unsaved File)", Model = null };
            if (file == null)
            {
                ViewBag.myMap = "{}".ToJson().ToString();
                return Json(new { success = false });
            }
            else
            {
                try
                {
                    string fileContent = new StreamReader(file.InputStream).ReadToEnd();
                    BsonDocument bMapModel = BsonDocument.Parse(fileContent);
                    ViewBag.myMap = bMapModel.ToJson().ToString();
                }
                catch (Exception)
                {
                    //throw new System.Web.Http.HttpResponseException(HttpStatusCode.BadRequest);
                    return RedirectToAction("index", "Home");
                }
                
            }
            return View("LaunchMap5", map);
        }


        public ActionResult LaunchMap5_test(string id)
        {
            if (id != null && !_mapManager.IsMapExist(new ObjectId(id)))
            {
                return RedirectToAction("index", "Home");
            }

            if (id != null)
            {
                var userId = User.Identity.GetUserId();
                MapPermissionController mpc = new MapPermissionController();
                MapPermissionType userPermissionsAsUser = mpc.GetUserPermissionToMapAsUser(id, userId);
                MapPermissionType userPermissionsByGroup = mpc.GetUserPermissionToMapByGroup(id, userId);
                MapPermissionType userPermissions = MapPermissionType.NA;

                if (userPermissionsAsUser == MapPermissionType.RW || userPermissionsByGroup == MapPermissionType.RW)
                {
                    userPermissions = MapPermissionType.RW;
                }
                else if (userPermissionsAsUser == MapPermissionType.RO || userPermissionsByGroup == MapPermissionType.RO)
                {
                    userPermissions = MapPermissionType.RO;
                }
                else
                {
                    return RedirectToAction("index", "Home");
                }

                ViewBag.userPermission = userPermissions;

                var map = _mapManager.GetMapById(new ObjectId(id));
                ViewBag.isSaved = true;
                ViewBag.myMap = map.Model.ToJson().ToString();

                //var refIDs = GetReferencesIDsFromMapModel(map.Model.ToJson());
                //var refsDict = GetReferencesFromIDs(refIDs);
                //ViewBag.refs = refsDict;
                //string t = map.Model.ToJson().ToString();
                //ViewBag.myMap = JsonConvert.SerializeObject(map.Model.ToJson());
                return View(map);
            }
            else
            {
                ViewBag.userPermission = MapPermissionType.RW;
                ViewBag.isSaved = false;
                var map = new Map { Name = "(Unsaved File)", Model = null };
                ViewBag.myMap = "{}".ToJson().ToString();
                return View(map);
            }
        }

        [HttpPost]
        public ActionResult FollowMap(string mapId)
        {
            var map = _mapManager.GetMapById(new ObjectId(mapId));
            if (string.IsNullOrEmpty(mapId) || map == null)
            {
                return Json("Failure", JsonRequestBehavior.AllowGet);
            }

            var userId = User.Identity.GetUserId();
            if (!map.Followers.Contains(new ObjectId(userId), new IDComparer()))
            {
                map.Followers.Add(new ObjectId(userId));
                _mapManager.UpdateMap(map);
                return Json(new { IsFollowing = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                map.Followers.Remove(new ObjectId(userId));
                _mapManager.UpdateMap(map);
                return Json(new { IsFollowing = false }, JsonRequestBehavior.AllowGet);
            }
            //return Json("Failure", JsonRequestBehavior.AllowGet);
        }

        private ISet<string> GetReferencesIDsFromMapModel(string map)
        {
            HashSet<string> refsIDs = new HashSet<string>();
            JObject jObject = JObject.Parse(map);
            JToken nodes = jObject["nodeDataArray"];

            foreach (var node in nodes.ToArray())
            {
                if (node["items"] != null)
                {
                    foreach (var reff in node["items"].ToArray())
                    {
                        refsIDs.Add(reff.ToString());
                    }
                }
            }
            return refsIDs;
        }

        private Dictionary<string, Reference> GetReferencesFromIDs(ISet<string> refs)
        {
            Dictionary<string, Reference> refssDict = new Dictionary<string, Reference>();

            ReferenceController refController = new ReferenceController();
            foreach (var reff in refs)
            {
                refssDict.Add(reff, refController.GetReferenceById(reff));
            }
            return refssDict;
        }

        private bool IsValidId(string id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                return false;
            }
            return true;

        }

        private bool IsValidMap(Map map)
        {
            return map != null;
        }

    }
}

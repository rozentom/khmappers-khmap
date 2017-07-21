using AspNet.Identity.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using khmap.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using MongoDB.Bson;
using khmap.DataBaseProviders;

namespace khmap.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;

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


        private MapFolder createSuppFolderLocaly(string whichSupp)
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
            suppFolder.Name = "suppFolder" + whichSupp;
            suppFolder.Creator = UserId;
            suppFolder.CreationTime = DateTime.Now;
            suppFolder.Description = "Supirior Folder " + whichSupp;
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


        public ActionResult Index(string id)
        {
            //MailSender.SendMailMessage("khmap@bgu.ac.il", "giladsabari@gmail.com", "", "", "(do not reply)", "lalalalalala");
            ViewBag.ownedFolderID = null;
            MapFolderDB mapFolderManeger = new MapFolderDB(new Settings());
            bool isIdOfOwned = false;
            if (id!=null)
            {
                ObjectId oId = new ObjectId(id);
                MapFolder mf = mapFolderManeger.GetMapFolderById(oId);
                if((mf.Model["type"]).Equals(SharedCodedData.OWNED_SUPIRIOR) || (mf.Model["type"]).Equals(SharedCodedData.NOT_SUPIRIOR_BUT_OWNED))
                {
                    ViewBag.ownedFolderID = oId;
                    isIdOfOwned = true;
                }
            }
            if (!isIdOfOwned)
            {
                ObjectId userID = new ObjectId(User.Identity.GetUserId());
                var supFolder = new MapFolderDB(new Settings()).GetSuperiorMapFolderOfUserOwned(userID);
                if (supFolder != null)
                {
                    ObjectId fId = supFolder.Id;
                    ViewBag.ownedFolderID = fId;
                }
                else
                {
                    supFolder = createSuppFolderLocaly(SharedCodedData.OWNED_SUPIRIOR);
                    mapFolderManeger.AddFolder(supFolder);
                    ObjectId fId = supFolder.Id;
                    ViewBag.ownedFolderID = fId;
                }
            }

            ViewBag.sharedFolderID = null;

            bool isIdOfShared = false;
            if (id != null)
            {
                ObjectId oId = new ObjectId(id);
                MapFolder mf = mapFolderManeger.GetMapFolderById(oId);
                if ((mf.Model["type"]).Equals(SharedCodedData.SHARED_SUPIRIOR) || (mf.Model["type"]).Equals(SharedCodedData.NOT_SUPIRIOR_BUT_SHARED))
                {
                    ViewBag.sharedFolderID = oId;
                    isIdOfShared = true;
                }
            }
            if (!isIdOfShared)
            {
                ObjectId userID = new ObjectId(User.Identity.GetUserId());
                var supFolder = new MapFolderDB(new Settings()).GetSuperiorMapFolderOfUserShared(userID);
                if (supFolder != null)
                {
                    ObjectId fId = supFolder.Id;
                    ViewBag.sharedFolderID = fId;
                }
                else
                {
                    supFolder = createSuppFolderLocaly(SharedCodedData.SHARED_SUPIRIOR);
                    mapFolderManeger.AddFolder(supFolder);
                    ObjectId fId = supFolder.Id;
                    ViewBag.sharedFolderID = fId;
                }
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }



        [Authorize]
        public ActionResult Details()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            DetailsViewModel dvm = new DetailsViewModel { Email = user.Email, City = user.City, Country = user.Country, CreationTime = user.CreationTime.ToLocalTime(), Reputation = CalculateReputation(user.Rankings.Values) };

            return View(dvm);
        }

        private double CalculateReputation(IEnumerable<int> rankings)
        {
            if (rankings.Count() == 0)
            {
                return 0;
            }
            return rankings.Average();
        }
    }
}
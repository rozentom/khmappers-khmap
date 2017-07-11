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


        public ActionResult Index(string id)
        {
            //MailSender.SendMailMessage("khmap@bgu.ac.il", "giladsabari@gmail.com", "", "", "(do not reply)", "lalalalalala");
            ViewBag.folderID = null;          
            if (id!=null)
            {
                ViewBag.folderID = new ObjectId(id);
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
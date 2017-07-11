using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MongoDB.Bson;
using khmap.Models;
using khmap.DataBaseProviders;

namespace khmap.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;
        private UserDB _userService;
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

        public UserController()
        {
            this._userService = new UserDB(new Settings());
        }

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public string GetMapCreatorEmail(string uId)
        {
            return UserManager.GetEmail(uId);
        }

        public ActionResult Profile(string id)
        {
            var user = UserManager.FindById(id);
            return View(user);
        }

        public ActionResult Profile2(string id)
        {
            if (IsValidId(id))
            {
                var user = UserManager.FindById(id);
                if (IsValidUser(user))
                {
                    return View(user);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Profile3(string id)
        {
            var user = UserManager.FindById(id);
            return View(user);
        }


        public ActionResult EditProfile(string id)
        {
            var user = _userService.GetUserById(new ObjectId(id));
            return View(user);
        }

        [HttpPost]
        public ActionResult EditProfile(string id, ApplicationUser model)
        {
            var user = _userService.GetUserById(new ObjectId(id));
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Country = model.Country;
            user.City = model.City;
            _userService.UpdateUser(user);
            return RedirectToAction("Profile2", "User", new { id = id });
        }

        public string GetUserFirstNameById(string id)
        {
            //var user = UserManager.FindById(id);
            //return user.FirstName;
            var user = _userService.GetUserById(new ObjectId(id));
            if (user == null)
            {
                return "got null";
            }
            return user.FirstName;
        }

        public string GetUserLastNameById(string id)
        {
            //var user = UserManager.FindById(id);
            //return user.FirstName;
            var user = _userService.GetUserById(new ObjectId(id));
            if(user == null)
            {
                return "";
            }
            return user.LastName;
        }

        public ActionResult GetEmailsList()
        {
            var users = _userService.GetAllUsers();
            var emailsList = users.Select(u => new EmailUserViewModel { Email = u.Email, FName = u.FirstName, LName = u.LastName });
            return Json(emailsList, JsonRequestBehavior.AllowGet);
        }

        private bool IsValidId(string id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                return false;
            }
            return true;

        }
        
        private bool IsValidUser(ApplicationUser user)
        {
            return user != null;
        }
    }
}
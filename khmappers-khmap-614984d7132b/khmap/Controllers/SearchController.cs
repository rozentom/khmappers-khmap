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
using WebApplication2.Search;

namespace khmap.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {

        private MapDB _mapManager;
        private UserDB _userService;
        private GroupDB _groupManager;
        private ApplicationUserManager _userManager;
        private Search _searchObj;

        public SearchController()
        {
            this._mapManager = new MapDB(new Settings());
            this._userService = new UserDB(new Settings());
            this._groupManager = new GroupDB(new Settings());
            _searchObj = new Search();
            //UserManager = UserManager;
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

        // GET: Search
        public ActionResult Index(string text)
        {
            SearchViewModel svm = new SearchViewModel();
            svm.Maps = _mapManager.SearchMap(text);
            svm.Groups = _groupManager.SearchGroup(text);
            svm.Users = _userService.SearchUser(text);
            svm.Maps = _searchObj.searchMaps(text);
            svm.Groups = _searchObj.searchGroups(text);
            svm.Users = _searchObj.searchUsers(text);
            svm.SearchText = text;
            return View(svm);
        }
    }
}
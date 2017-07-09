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
using khmap.SearchFolder;

namespace khmap.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {

        private MapDB _mapManager;
        private UserDB _userService;
        private GroupDB _groupManager;
        private ApplicationUserManager _userManager;
        private ISearch _searchObj;

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


            //-------the way it was------
            //svm.Maps = _mapManager.SearchMap(text);
            //svm.Groups = _groupManager.SearchGroup(text);
            //svm.Users = _userService.SearchUser(text);
            //-------searhching combined------
            //Dictionary<ISearchType, Object> results = _searchObj.searchFunc(text, new List<ISearchType> { new SearchGroups(), new SearchMaps(), new SearchUsers() });
            //svm.Maps = _searchObj.getMapsOfResult(results);
            //svm.Groups = _searchObj.getGroupsOfResult(results);
            //svm.Users = _searchObj.getUsersOfResult(results);

            svm.Maps = _searchObj.searchMaps(text);
            svm.Groups = _searchObj.searchGroups(text);
            svm.Users = _searchObj.searchUsers(text);
            svm.SearchText = text;
            return View(svm);
        }
    }
}
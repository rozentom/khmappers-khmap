using khmap.DataBaseProviders;
using khmap.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace khmap.SearchDirectory
{
    public class SearchUsers : ISearchUsers, ISearchType
    {
        IEnumerable<ApplicationUser> _allUsers;
        private UserDB _userManeger;

        public SearchUsers()
        {
            _userManeger = new UserDB(new Settings());
            _allUsers = _userManeger.GetAllUsers();
        }

        public bool sameType(ISearchType st)
        {
            if (st is SearchUsers)
            {
                return true;
            }
            return false;
        }

        public object search(string text, ObjectId userId)
        {
            return SearchUsersFunc(text, userId);
        }

        public IEnumerable<ApplicationUser> SearchUsersFunc(string userName, ObjectId userId)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            IEnumerable<ApplicationUser> userss = _allUsers;
            foreach (var item in userss)
            {
                if (item.FirstName.ToLower().Contains(userName.ToLower()))
                {
                    users.Add(item);
                }
            }
            return users;
        }
    }
}
﻿using khmap.DataBaseProviders;
using khmap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace khmap.SearchFolder
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

        public object search(string text)
        {
            return SearchUsersFunc(text);
        }

        public IEnumerable<ApplicationUser> SearchUsersFunc(string userName)
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
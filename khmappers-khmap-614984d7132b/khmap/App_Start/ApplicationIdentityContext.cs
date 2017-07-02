using AspNet.Identity.MongoDB;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace khmap.App_Start
{
    public class ApplicationIdentityContext : IdentityContext, IDisposable
    {
        public ApplicationIdentityContext(MongoCollection users, MongoCollection roles)
            : base(users, roles)
        {
        }

        public static ApplicationIdentityContext Create()
        {
            // todo add settings where appropriate to switch server & database in your own application
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetServer().GetDatabase("test1");
            var users = database.GetCollection<IdentityUser>("users");
            var roles = database.GetCollection<IdentityRole>("roles");
            return new ApplicationIdentityContext(users, roles);
        }

        public void Dispose()
        {
        }
    }
}
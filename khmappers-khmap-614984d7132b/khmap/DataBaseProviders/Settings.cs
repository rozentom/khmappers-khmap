using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace khmap.DataBaseProviders
{
    public class Settings
    {
        private string connectionString = "mongodb://localhost";
        private string databaseName = "test1";

        public string ConnectionString { get { return connectionString; } }
        public string DatabaseName { get { return databaseName; } }
        public string UsersCollectionName { get { return "users"; } }
        public string GroupsCollectionName { get { return "groups"; } }
        public string MapsCollectionName { get { return "maps"; } }
        public string ReferencesCollectionName { get { return "references"; } }
        public string ContextsCollectionName { get { return "Contexts"; } }
        public string CommentsCollectionName { get { return "Comments"; } }
    }
}
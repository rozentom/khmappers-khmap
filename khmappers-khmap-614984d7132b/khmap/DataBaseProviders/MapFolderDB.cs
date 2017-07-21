using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using khmap.DataBaseProviders;
using System.Collections.Generic;
using khmap.Models;


namespace khmap.DataBaseProviders
{
    public class MapFolderDB
    {
        private readonly Settings _settings;
        private readonly MongoDatabase _database;
        private readonly string _collectionName;

        //constructors
        public MapFolderDB(Settings settings)
        {
            _settings = settings;
            _collectionName = _settings.MapFoldersCollectionName;
            _database = Connect();
        }
                                //methodes


        //adding a folder to the DB
        public void AddFolder( MapFolder Folder)
        {
            _database.GetCollection<MapFolder>(_collectionName).Save(Folder);
        }

        //add a folder to the parent folder
        public void AddSubFolder(MapFolder parent, MapFolder toAdd)
        {
            var parFolder = this.GetMapFolderById(parent.Id);                        
            this.AddFolder(toAdd);
            if (parFolder.idOfSubFolders==null)
            parFolder.idOfSubFolders = new HashSet<ObjectId>();
            parFolder.idOfSubFolders.Add(toAdd.Id);
            this.UpdateMapFolder(parFolder);
        }


        //returns all the the map folders of the user given
        public IEnumerable<MapFolder> GetAllMapFoldersOfUser(ObjectId userId)
        {
            var query = Query<MapFolder>.EQ(e => e.Creator, userId);
            var folders = _database.GetCollection<MapFolder>(_collectionName).Find(query);
            return folders;
        }

        //this method return all the suprior/first mapFolders of the given user
        public MapFolder GetSuperiorMapFolderOfUser(ObjectId userId)
        {
            var query = Query<MapFolder>.EQ(e => e.FirstFolderOfUser, userId);
            var supfolder = _database.GetCollection<MapFolder>(_collectionName).FindOne(query);
            //if (supfolder == null)
            //{
            //    return new MapFolder();
            //}           
            return supfolder;
        }

        public MapFolder GetSuperiorMapFolderOfUserOwned(ObjectId userId)
        {
            var query = Query<MapFolder>.EQ(e => e.FirstFolderOfUser, userId);
            var folders = GetAllMapFoldersOfUser(userId);
            MapFolder supfolder = null;
            foreach (MapFolder mf in folders)
            {
                if ((mf.Model["type"]).Equals(SharedCodedData.OWNED_SUPIRIOR))
                {
                    supfolder = mf;
                    break;
                }
            }
            return supfolder;
        }

        public MapFolder GetSuperiorMapFolderOfUserShared(ObjectId userId)
        {
            var query = Query<MapFolder>.EQ(e => e.FirstFolderOfUser, userId);
            //var supfolder = _database.GetCollection<MapFolder>(_collectionName).FindOne(query);
            //if (supfolder != null)
            //{
            //    return new MapFolder();
            //}   
            var folders = GetAllMapFoldersOfUser(userId);
            MapFolder supfolder = null;
            foreach(MapFolder mf in folders)
            {
                if ((mf.Model["type"]).Equals(SharedCodedData.SHARED_SUPIRIOR))
                {
                    supfolder = mf;
                    break;
                }
            }      
            return supfolder;
        }

        public IEnumerable<MapFolder> GetFirstFoldersOfUser(ObjectId userId)
        {
            try
            {
                var sup = this.GetSuperiorMapFolderOfUser(userId);
                var query = Query<MapFolder>.EQ(e => e.Id, sup.Id);
                var supfol = this.GetSuperiorMapFolderOfUser(userId);
                var folders = this.GetAllSubFolder(supfol);
                return folders;
            }
            catch (Exception e)
            {
                return new List<MapFolder>();
            }
        }
        
        //this method return all the  sub Folders of the given folder
        public IEnumerable<MapFolder> GetAllSubFolder(MapFolder parent)
        {
            List<MapFolder> mapFoldersList = new List<MapFolder>();            
            foreach (ObjectId MapFolder in parent.idOfSubFolders)
            {
                var toAdd = this.GetMapFolderById(MapFolder);
                mapFoldersList.Add(toAdd);
            }
            return mapFoldersList;
        }


        public IEnumerable<Map> GetAllMapsInFolder(MapFolder parent)
        {
            try
            {
                var mapDataManager = new MapDB(new Settings());
                List<Map> mapList = new List<Map>();
                foreach (ObjectId Map in parent.idOfMapsInFolder)
                {
                    var toAdd = mapDataManager.GetMapById(Map);
                    mapList.Add(toAdd);
                }
                return mapList;
            }
            catch(Exception e)
            {
                return new MapDB(new Settings()).GetAllMaps();
            }
        }

        //gets the folder with the given id
        public MapFolder GetMapFolderById(ObjectId idOfTheFolder)
        {
            var query = Query<MapFolder>.EQ(e => e.Id, idOfTheFolder);
            var folder = _database.GetCollection<MapFolder>(_collectionName).FindOne(query);
            return folder;
        }

        //this method remove a mapFolder from the DB if this method will receive a true 
        //bollean value it will delete all the maps and folders that are in the given folder as well  
        public bool RemoveMapFolder(MapFolder folderToRemove, Boolean removeAllContent)
        {
            var query = Query<MapFolder>.EQ(e => e.Id, folderToRemove.Id);
            var result = _database.GetCollection<Group>(_collectionName).Remove(query);
            if (removeAllContent)
            {
                removeAll(folderToRemove);
            }
            return (GetMapFolderById(folderToRemove.Id) == null);
        }

        //private methode that will delete all the folders in maps in the given map;
        private void removeAll(MapFolder folderToRemove)
        {
            MapDB deleter = new DataBaseProviders.MapDB(new Settings());
            foreach (ObjectId map in folderToRemove.idOfMapsInFolder)
            {
                deleter.RemoveMap(map);
            }
            foreach (ObjectId MapFolder in folderToRemove.idOfSubFolders)
            {
                var subToRemove = this.GetMapFolderById(MapFolder);
                this.RemoveMapFolder(subToRemove, true);
            }
        }

        //remove folder by id
        public bool RemoveMapFolderById(ObjectId id)
        {
            var query = Query<MapFolder>.EQ(e => e.Id, id);
            var result = _database.GetCollection<MapFolder>(_collectionName).Remove(query);

            return GetMapFolderById(id) == null;
        }

        //update MapFolder 
        public void UpdateMapFolder(MapFolder folder)
        {
            var query = Query<MapFolder>.EQ(e => e.Id, folder.Id);
            var update = Update<MapFolder>.Replace(folder);
            _database.GetCollection<MapFolder>(_collectionName).Update(query, update);
        }

        //search for a folder by name
        public IEnumerable<MapFolder> SearchMapFolderByName(string folderName)
        {
            List<MapFolder> folders = new List<MapFolder>();
            IEnumerable<MapFolder> allFolders = _database.GetCollection<MapFolder>(_collectionName).FindAll();
            foreach (var item in allFolders)
            {
                if (item.Name.ToLower().Contains(folderName.ToLower()))
                {
                    folders.Add(item);
                }
            }
            return folders;
        }

        //check if the folder is exist or not
        public bool IsMapFolderExist(ObjectId folderId)
        {
            var folder = GetMapFolderById(folderId);
            return folder != null;
        }

        //conection to the db
        private MongoDatabase Connect()
        {
            var client = new MongoClient(_settings.ConnectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(_settings.DatabaseName);
            return database;
        }

    }
}
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using khmap.Models;

namespace khmap.DataBaseProviders
{
    public class ContextDB
    {
        private readonly Settings _settings;
        private readonly MongoDatabase _database;
        private readonly string _collectionName;

        public ContextDB(Settings settings)
        {
            _settings = settings;
            _collectionName = _settings.ContextsCollectionName;
            _database = Connect();
        }

        public string AddContext(Context context)
        {
            _database.GetCollection<Context>(_collectionName).Save(context);
            return context.Id.ToString();
        }

        public IEnumerable<Context> GetAllContexts()
        {
            var ctxs = _database.GetCollection<Context>(_collectionName).FindAll();
            return ctxs;
        }

        public Context GetContextById(string id)
        {
            var query = Query<Context>.EQ(e => e.Id, id);
            var ctxs = _database.GetCollection<Context>(_collectionName).FindOne(query);
            return ctxs;
        }

        public IEnumerable<Context> GetContextsByIds(IEnumerable<string> ctxsIds)
        {
            HashSet<Context> ctxsResult = new HashSet<Context>();
            foreach (var c in ctxsIds)
            {
                var ctxToAdd = GetContextById(c);
                if (ctxToAdd != null)
                {
                    ctxsResult.Add(ctxToAdd);
                }
            }
            return ctxsResult;
        }

        public IEnumerable<Context> GetFiteredContexts(IEnumerable<string> ctxIds, string text, int categoryId, int jtStartIndex, int jtPageSize, string jtSorting, out int totalCount)
        {
            IEnumerable<Context> ctxsResult = GetContextsByIds(ctxIds);
            if (!string.IsNullOrEmpty(text))
            {
                if (categoryId == 1)
                {
                    ctxsResult = ctxsResult.Where(x => x.Title.Contains(text)).ToList<Context>();
                }
                else // for future extension
                {
                    ctxsResult = ctxsResult.Where(x => x.Title.Contains(text)).ToList<Context>();
                }
            }
            List<Context> cList = new List<Context>(ctxsResult);
            var endIndex = jtStartIndex + jtPageSize <= cList.Count() ? jtPageSize : cList.Count() % jtPageSize;
            totalCount = cList.Count();
            var fList = SortContextsByProperty(cList, jtSorting);
            fList = fList.GetRange(jtStartIndex, endIndex);
            //fList = SortContextsByProperty(fList, jtSorting);
            return fList;
        }

        public List<Context> SortContextsByProperty(List<Context> ctxs, string property)
        {
            var ctxsSortedList = ctxs;
            if (!string.IsNullOrEmpty(property))
            {
                if (property.Contains("ASC"))
                {
                    if (property.Contains("Title"))
                    {
                        ctxsSortedList = ctxsSortedList.OrderBy(x => x.Title).ToList<Context>();
                    }
                    else if (property.Contains("CreationTime"))
                    {
                        ctxsSortedList = ctxsSortedList.OrderBy(x => x.CreationTime).ToList<Context>();
                    }
                }
                else
                {
                    if (property.Contains("Title"))
                    {
                        ctxsSortedList = ctxsSortedList.OrderByDescending(x => x.Title).ToList<Context>();
                    }
                    else if (property.Contains("CreationTime"))
                    {
                        ctxsSortedList = ctxsSortedList.OrderByDescending(x => x.CreationTime).ToList<Context>();
                    }
                }
            }
            return ctxsSortedList;
        }

        public bool RemoveContext(string id)
        {
            var query = Query<Context>.EQ(e => e.Id, id);
            var result = _database.GetCollection<Context>(_collectionName).Remove(query);

            return GetContextById(id) == null;
        }

        public void UpdateContext(Context ctx)
        {
            var query = Query<Context>.EQ(e => e.Id, ctx.Id);
            var update = Update<Context>.Replace(ctx); // update modifiers
            _database.GetCollection<Context>(_collectionName).Update(query, update);
        }

        public IEnumerable<Context> SearchContextByTitle(string ctxTitle)
        {
            List<Context> ctxs = new List<Context>();
            IEnumerable<Context> ctxss = _database.GetCollection<Context>(_collectionName).FindAll();
            foreach (var item in ctxss)
            {
                if (item.Title.ToLower().Contains(ctxTitle.ToLower()))
                {
                    ctxs.Add(item);
                }
            }
            return ctxs;
        }


        public IEnumerable<Context> GetAllOtherContexts(IEnumerable<string> ctxIds, int jtStartIndex, int jtPageSize, string jtSorting)
        {
            IEnumerable<Context> myCtxs = GetContextsByIds(ctxIds);
            List<Context> myList = new List<Context>(myCtxs);

            IEnumerable<Context> ctxsResult = GetAllContexts();
            List<Context> cList = new List<Context>(ctxsResult);

            cList.RemoveAll(x => myList.Contains(x, new ContextComparer()));
            //List<Context> cList = new List<Context>(allOther);
            var endIndex = jtStartIndex + jtPageSize <= cList.Count() ? jtPageSize : cList.Count() % jtPageSize;
            var fList = cList.GetRange(jtStartIndex, endIndex);
            return fList;
        }

        public IEnumerable<Context> GetAllOtherContextsList(IEnumerable<string> ctxsIds)
        {
            IEnumerable<Context> myCtxs = GetContextsByIds(ctxsIds);
            List<Context> myList = new List<Context>(myCtxs);

            IEnumerable<Context> ctxsResult = GetAllContexts();
            List<Context> cList = new List<Context>(ctxsResult);

            cList.RemoveAll(x => myList.Contains(x, new ContextComparer()));
            return cList;
        }

        public IEnumerable<Context> GetAllOtherFilteredContexts(IEnumerable<string> ctxsIds, string text, int categoryId, int jtStartIndex, int jtPageSize, string jtSorting, out int totalCount)
        {
            IEnumerable<Context> allOtherTmp = GetAllOtherContextsList(ctxsIds);
            List<Context> allOther = new List<Context>(allOtherTmp);

            if (!string.IsNullOrEmpty(text))
            {
                if (categoryId == 1)
                {
                    allOther = allOther.Where(x => x.Title.Contains(text)).ToList<Context>();
                }
                else // for future extension
                {
                    allOther = allOther.Where(x => (x.Title.Contains(text))).ToList<Context>();
                }
            }

            var endIndex = jtStartIndex + jtPageSize <= allOther.Count() ? jtPageSize : allOther.Count() % jtPageSize;
            totalCount = allOther.Count();
            allOther = SortContextsByProperty(allOther, jtSorting);
            allOther = allOther.GetRange(jtStartIndex, endIndex);

            //allOther = SortContextsByProperty(allOther, jtSorting);

            return allOther;
        }

        private MongoDatabase Connect()
        {
            var client = new MongoClient(_settings.ConnectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(_settings.DatabaseName);
            return database;
        }
    }


    class ContextComparer : IEqualityComparer<Context>
    {

        public bool Equals(Context x, Context y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Context obj)
        {
            return obj.GetHashCode();
        }
    }

}
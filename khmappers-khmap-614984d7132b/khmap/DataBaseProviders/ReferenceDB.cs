using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using khmap.Models;

namespace khmap.DataBaseProviders
{
    public class ReferenceDB
    {
        private readonly Settings _settings;
        private readonly MongoDatabase _database;
        private readonly string _collectionName;

        public ReferenceDB(Settings settings)
        {
            _settings = settings;
            _collectionName = _settings.ReferencesCollectionName;
            _database = Connect();
        }

        public string AddReference(Reference referene)
        {
            _database.GetCollection<Reference>(_collectionName).Save(referene);
            return referene.Id.ToString();
        }

        public IEnumerable<Reference> GetAllReferences()
        {
            var refs = _database.GetCollection<Reference>(_collectionName).FindAll();
            return refs;
        }

        public Reference GetReferenceById(string id)
        {
            var query = Query<Reference>.EQ(e => e.Id, id);
            var reff = _database.GetCollection<Reference>(_collectionName).FindOne(query);
            return reff;
        }

        public IEnumerable<Reference> GetReferencesByIds(IEnumerable<string> refIds)
        {
            HashSet<Reference> refsResult = new HashSet<Reference>();
            foreach (var r in refIds)
            {
                var refToAdd = GetReferenceById(r);
                if (refToAdd != null)
                {
                    refsResult.Add(refToAdd);
                }
            }
            return refsResult;
        }

        public IEnumerable<Reference> GetFilteredReferences(IEnumerable<string> refIds, string text, int categoryId, int jtStartIndex, int jtPageSize, string jtSorting, out int totalCount)
        {
            IEnumerable<Reference> tmpRefs = GetReferencesByIds(refIds);
            IEnumerable<Reference> refsResult = GetReferencesByIds(refIds);
            if (!string.IsNullOrEmpty(text))
            {
                if (categoryId == 1)
                {
                    refsResult = tmpRefs.Where(x => x.Title.Contains(text)).ToList<Reference>();
                }
                else if (categoryId == 2)
                {
                    refsResult = tmpRefs.Where(x => x.Authors.Contains(text)).ToList<Reference>();
                }
                else if (categoryId == 3)
                {
                    refsResult = tmpRefs.Where(x => x.Publication.Contains(text)).ToList<Reference>();
                }
                else
                {
                    refsResult = tmpRefs.Where(x => (x.Title.Contains(text) || x.Authors.Contains(text) || x.Publication.Contains(text))).ToList<Reference>();
                }
            }
            List<Reference> rList = new List<Reference>(refsResult);
            var endIndex = jtStartIndex + jtPageSize <= rList.Count() ? jtPageSize : rList.Count() % jtPageSize;
            totalCount = rList.Count();
            var fList = SortReferencesByProperty(rList, jtSorting);
            fList = fList.GetRange(jtStartIndex, endIndex);

            //fList = SortReferencesByProperty(fList, jtSorting);

            return fList;
        }


        public List<Reference> SortReferencesByProperty(List<Reference> refs, string property)
        {
            var refsSortedList = refs;
            if (!string.IsNullOrEmpty(property))
            {
                if (property.Contains("ASC"))
                {
                    if (property.Contains("Title"))
                    {
                        refsSortedList = refsSortedList.OrderBy(x => x.Title).ToList<Reference>();
                    }
                    else if (property.Contains("Publication"))
                    {
                        refsSortedList = refsSortedList.OrderBy(x => x.Publication).ToList<Reference>();
                    }
                    else if (property.Contains("CreationTime"))
                    {
                        refsSortedList = refsSortedList.OrderBy(x => x.CreationTime).ToList<Reference>();
                    }
                }
                else
                {
                    if (property.Contains("Title"))
                    {
                        refsSortedList = refsSortedList.OrderByDescending(x => x.Title).ToList<Reference>();
                    }
                    else if (property.Contains("Publication"))
                    {
                        refsSortedList = refsSortedList.OrderByDescending(x => x.Publication).ToList<Reference>();
                    }
                    else if (property.Contains("CreationTime"))
                    {
                        refsSortedList = refsSortedList.OrderByDescending(x => x.CreationTime).ToList<Reference>();
                    }
                }
            }
            return refsSortedList;
        }

        public bool RemoveReference(string id)
        {
            var query = Query<Reference>.EQ(e => e.Id, id);
            var result = _database.GetCollection<Reference>(_collectionName).Remove(query);

            return GetReferenceById(id) == null;
        }

        public void UpdateReference(Reference reff)
        {
            var query = Query<Reference>.EQ(e => e.Id, reff.Id);
            var update = Update<Reference>.Replace(reff); // update modifiers
            _database.GetCollection<Reference>(_collectionName).Update(query, update);
        }

        public IEnumerable<Reference> SearchReferenceByTitle(string refTitle)
        {
            List<Reference> refs = new List<Reference>();
            IEnumerable<Reference> refss = _database.GetCollection<Reference>(_collectionName).FindAll();
            foreach (var item in refss)
            {
                if (item.Title.ToLower().Contains(refTitle.ToLower()))
                {
                    refs.Add(item);
                }
            }
            return refs;
        }

        public IEnumerable<Reference> SearchReferenceByPublication(string pub)
        {
            List<Reference> refs = new List<Reference>();
            IEnumerable<Reference> refss = _database.GetCollection<Reference>(_collectionName).FindAll();
            foreach (var item in refss)
            {
                if (item.Publication.ToLower().Contains(pub.ToLower()))
                {
                    refs.Add(item);
                }
            }
            return refs;
        }

        public IEnumerable<Reference> SearchReferenceByAuthor(string author)
        {
            HashSet<Reference> refs = new HashSet<Reference>();
            IEnumerable<Reference> refss = _database.GetCollection<Reference>(_collectionName).FindAll();
            foreach (var item in refss)
            {
                foreach (var a in item.Authors)
                {
                    if (a.ToLower().Contains(author.ToLower()))
                    {
                        refs.Add(item);
                    }
                }
            }
            return refs;
        }

        public IEnumerable<Reference> GetAllOtherReferences(IEnumerable<string> refsIds, int jtStartIndex, int jtPageSize, string jtSorting)
        {
            IEnumerable<Reference> myRefs = GetReferencesByIds(refsIds);
            List<Reference> myList = new List<Reference>(myRefs);

            IEnumerable<Reference> refsResult = GetAllReferences();
            List<Reference> rList = new List<Reference>(refsResult);

            rList.RemoveAll(x => myList.Contains(x, new ReferenceComparer()));
            //List<Context> cList = new List<Context>(allOther);
            var endIndex = jtStartIndex + jtPageSize <= rList.Count() ? jtPageSize : rList.Count() % jtPageSize;
            var fList = rList.GetRange(jtStartIndex, endIndex);
            return fList;
        }

        public IEnumerable<Reference> GetAllOtherReferencesList(IEnumerable<string> refsIds)
        {
            IEnumerable<Reference> myRefs = GetReferencesByIds(refsIds);
            List<Reference> myList = new List<Reference>(myRefs);

            IEnumerable<Reference> refsResult = GetAllReferences();
            List<Reference> rList = new List<Reference>(refsResult);

            rList.RemoveAll(x => myList.Contains(x, new ReferenceComparer()));
            return rList;
        }

        public IEnumerable<Reference> GetAllOtherFilteredReferences(IEnumerable<string> refsIds, string text, int categoryId, int jtStartIndex, int jtPageSize, string jtSorting, out int totalCount)
        {
            IEnumerable<Reference> allOtherTmp = GetAllOtherReferencesList(refsIds);
            List<Reference> allOther = new List<Reference>(allOtherTmp);

            if (!string.IsNullOrEmpty(text))
            {
                if (categoryId == 1)
                {
                    allOther = allOther.Where(x => x.Title.Contains(text)).ToList<Reference>();
                }
                else if (categoryId == 2)
                {
                    allOther = allOther.Where(x => x.Authors.Contains(text)).ToList<Reference>();
                }
                else if (categoryId == 3)
                {
                    allOther = allOther.Where(x => x.Publication.Contains(text)).ToList<Reference>();
                }
                else
                {
                    allOther = allOther.Where(x => (x.Title.Contains(text) || x.Authors.Contains(text) || x.Publication.Contains(text))).ToList<Reference>();
                }
            }

            var endIndex = jtStartIndex + jtPageSize <= allOther.Count() ? jtPageSize : allOther.Count() % jtPageSize;
            totalCount = allOther.Count();
            allOther = SortReferencesByProperty(allOther, jtSorting);
            allOther = allOther.GetRange(jtStartIndex, endIndex);

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

    class ReferenceComparer : IEqualityComparer<Reference>
    {

        public bool Equals(Reference x, Reference y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Reference obj)
        {
            return obj.GetHashCode();
        }
    }
}
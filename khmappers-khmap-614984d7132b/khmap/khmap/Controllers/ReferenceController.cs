using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using khmap.Models;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using khmap.DataBaseProviders;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace khmap.Controllers
{
    [Authorize]
    public class ReferenceController : Controller
    {

        private ReferenceDB _referenceManager;
        private ApplicationUserManager _userManager;

        public ReferenceController()
        {
            this._referenceManager = new ReferenceDB(new Settings());
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



        // GET: Reference
        public ActionResult Index(string id = "55662bb0a9b48820783871d5")
        {
            ObjectId oId = new ObjectId(id);
            var refs = _referenceManager.GetAllReferences();
            return PartialView("_Index", refs);
        }

        [HttpPost]
        public PartialViewResult MyRefs(string refs)
        {
            string[] strings = JsonConvert.DeserializeObject<string[]>(refs);
            var refss = new List<string>(strings);
            var refsList = _referenceManager.GetReferencesByIds(refss);
            return PartialView("_MyRefsView", refsList);
        }

        // GET: Reference/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult CreateRef()
        {
            return PartialView("_CreateReference", new Reference());
        }

        // GET: Reference/Create
        public ActionResult Create()
        {
            var id = User.Identity.GetUserId();
            ObjectId oId = new ObjectId(id);
            var newRef = new Reference { CreationTime = DateTime.Now, Creator = oId };
            return PartialView("_Create", newRef);
        }


        // POST: Reference/Create
        [HttpPost]
        public ActionResult Create(Reference refModel)
        {
            var id = User.Identity.GetUserId();
            ObjectId oId = new ObjectId(id);
            Reference reff = new Reference { Title = refModel.Title, Authors = new List<string>(), Publication = refModel.Publication, Creator = oId, Description = refModel.Description, Link = refModel.Link, CreationTime = DateTime.Now };
            _referenceManager.AddReference(reff);

            string url = Url.Action("Index", "Reference", new { id = refModel.Id.ToString() });
            return Json(new { success = true, url = url });

            //return RedirectToAction("Index", "Home");
        }


        // POST: Reference/Create
        [HttpPost]
        public ActionResult CreateRef(Reference refModel)
        {
            var id = User.Identity.GetUserId();
            ObjectId oId = new ObjectId(id);
            Reference reff = new Reference { Title = refModel.Title, Authors = new List<string>(), Publication = refModel.Publication, Creator = oId, Description = refModel.Description, Link = refModel.Link, CreationTime = DateTime.Now };
            _referenceManager.AddReference(reff);
            return RedirectToAction("Index", "Home");
        }



        // GET: Reference/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Reference/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reference/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reference/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public Reference GetReferenceById(string refId)
        {
            //ObjectId oId = new ObjectId(refId);
            return _referenceManager.GetReferenceById(refId);
        }



        [HttpPost]
        public JsonResult ReferenceList(string refList, string postData, string filterText = "", int categoryId = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                //string[] postDataValues = JsonConvert.DeserializeObject<string[]>(postData);
                PData postDataValues;
                if (!string.IsNullOrEmpty(postData))
                {
                    postDataValues = JsonConvert.DeserializeObject<PData>(postData);
                    filterText = postDataValues.filterText;
                    categoryId = postDataValues.categoryId;
                }

                string[] strings = JsonConvert.DeserializeObject<string[]>(refList);
                var refss = new List<string>(strings);
                //var refs = _referenceManager.GetReferencesByIds(refss);
                
                int allCount;
                var refs = _referenceManager.GetFilteredReferences(refss, filterText, categoryId, jtStartIndex, jtPageSize, jtSorting, out allCount);

                //var refs = _referenceManager.GetAllReferences();
                return Json(new { Result = "OK", Records = refs, TotalRecordCount = allCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        
        [HttpPost]
        public JsonResult CreateReference(Reference reference)
        {
            try
            {
                if (reference.Id == null)
                {
                    //Person addedPerson = _repository.PersonRepository.AddPerson(person);
                    var id = User.Identity.GetUserId();
                    ObjectId oId = new ObjectId(id);
                    Reference addReference = new Reference { Title = reference.Title ?? "", Authors = reference.Authors ?? new List<string>(), Description = reference.Description ?? "", Link = reference.Link ?? "", Publication = reference.Publication ?? "", CreationTime = DateTime.Now, Creator = oId };
                    var refId = _referenceManager.AddReference(addReference);
                    addReference.Id = refId;
                    return Json(new { Result = "OK", Record = addReference });
                }
                return Json(new { Result = "OK", Record = reference });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateReference(Reference reference)
        {
            try
            {
                //_repository.PersonRepository.UpdatePerson(person);
                var oldRef = _referenceManager.GetReferenceById(reference.Id);
                reference.Creator = oldRef.Creator;
                reference.CreationTime = oldRef.CreationTime;
                _referenceManager.UpdateReference(reference);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult DeleteReference(string Id)
        {
            try
            {
                //_repository.PersonRepository.DeletePerson(personId);
                //_referenceManager.RemoveReference(Id);
                return Json(new { Result = "OK", RefIdToRemove = Id });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        // Search context
        [HttpPost]
        public JsonResult GetAllOtherReferences(string refList, string postData, string filterText = "", int categoryId = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                PData postDataValues;
                if (!string.IsNullOrEmpty(postData))
                {
                    postDataValues = JsonConvert.DeserializeObject<PData>(postData);
                    filterText = postDataValues.filterText;
                    categoryId = postDataValues.categoryId;
                }

                string[] strings = JsonConvert.DeserializeObject<string[]>(refList);
                var refss = new List<string>(strings);

                int allOtherCount;
                //var refs = _referenceManager.GetAllOtherReferences(refss, jtStartIndex, jtPageSize, jtSorting);
                var refs = _referenceManager.GetAllOtherFilteredReferences(refss, filterText, categoryId, jtStartIndex, jtPageSize, jtSorting, out allOtherCount);

                //var allOtherCount = _referenceManager.GetAllReferences().Count() - refss.Count();

                return Json(new { Result = "OK", Records = refs, TotalRecordCount = allOtherCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        private class PData
        {
            public string filterText { get; set; }
            public int categoryId { get; set; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using khmap.DataBaseProviders;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using khmap.Models;
using MongoDB.Bson;

namespace khmap.Controllers
{
    public class ContextController : Controller
    {

        private ContextDB _contextManager;
        private ApplicationUserManager _userManager;

        public ContextController()
        {
            this._contextManager = new ContextDB(new Settings());
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


        // GET: Context
        public ActionResult Index()
        {
            return View();
        }

        // GET: Context/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Context/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Context/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Context/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Context/Edit/5
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

        // GET: Context/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Context/Delete/5
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




        [HttpPost]
        public JsonResult ContextList(string ctxsList, string postData, string filterText = "", int categoryId = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
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

                string[] strings = JsonConvert.DeserializeObject<string[]>(ctxsList);
                var ctxss = new List<string>(strings);

                int allCount;
                var ctxs = _contextManager.GetFiteredContexts(ctxss, filterText, categoryId, jtStartIndex, jtPageSize, jtSorting, out allCount);

                return Json(new { Result = "OK", Records = ctxs, TotalRecordCount = allCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult CreateContext(Context context)
        {
            try
            {
                if (context.Id == null)
                {
                    //Person addedPerson = _repository.PersonRepository.AddPerson(person);
                    var id = User.Identity.GetUserId();
                    ObjectId oId = new ObjectId(id);
                    Context addContext = new Context { Title = context.Title, CreationTime = DateTime.Now, Creator = oId };
                    var ctxId = _contextManager.AddContext(addContext);
                    addContext.Id = ctxId;
                    return Json(new { Result = "OK", Record = addContext });
                }
                return Json(new { Result = "OK", Record = context });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateContext(Context context)
        {
            try
            {
                //_repository.PersonRepository.UpdatePerson(person);
                var oldCtx = _contextManager.GetContextById(context.Id);
                context.Creator = oldCtx.Creator;
                context.CreationTime = oldCtx.CreationTime;
                _contextManager.UpdateContext(context);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult DeleteContext(string Id)
        {
            try
            {
                //_repository.PersonRepository.DeletePerson(personId);
                //_referenceManager.RemoveReference(Id);
                return Json(new { Result = "OK", CtxIdToRemove = Id });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        // Search context
        [HttpPost]
        public JsonResult GetAllOtherContexts(string ctxsList, string postData, string filterText = "", int categoryId = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
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

                string[] strings = JsonConvert.DeserializeObject<string[]>(ctxsList);
                var ctxss = new List<string>(strings);
                
                int allOtherCount;

                var ctxs = _contextManager.GetAllOtherFilteredContexts(ctxss, filterText, categoryId, jtStartIndex, jtPageSize, jtSorting, out allOtherCount);

                //var allOtherCount = _contextManager.GetAllContexts().Count() - ctxss.Count();

                return Json(new { Result = "OK", Records = ctxs, TotalRecordCount = allOtherCount });
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using khmap.DataBaseProviders;
using khmap.Models;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace khmap.Controllers
{
    public class CommentController : Controller
    {

        private CommentDB _commentManager;
        private ApplicationUserManager _userManager;

        public CommentController()
        {
            this._commentManager = new CommentDB(new Settings());
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


        [HttpPost]
        public ActionResult GetComments(string commentId = null)
        {
            Comment comment;
            var userId = User.Identity.GetUserId();

            if (string.IsNullOrEmpty(commentId))
            {
                comment = new Comment { MapId = null, Comments = new List<CommentModel>() };
                _commentManager.AddComment(comment);
                commentId = comment.Id;
            }
            var commentsList = _commentManager.GetCommentById(commentId).Comments;
            commentsList.ForEach(x => x.CreatedByCurrentUser = (x.CreatorId == userId));

            commentsList.ForEach(x => {
                x.CreatedByCurrentUser = (x.CreatorId == userId);
                x.UserHasUpvoted = (x.Voters.Contains(userId));
            });

            var jCommentsList = ConvertToJsonCommentModelList(commentsList);
            return Json(new { cId = commentId, cList = jCommentsList });
        }

        [HttpPost]
        public ActionResult CreateComment(string commentId, CommentModel newComment)
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            
            newComment.CreatorId = userId;
            newComment.Voters = new HashSet<string>();
            newComment.Fullname = user.FirstName + " " + user.LastName;
            newComment.CreatedByCurrentUser = true;

            var comment = _commentManager.GetCommentById(commentId);
            comment.Comments.Add(newComment);
            _commentManager.UpdateComment(comment);
            return Json( ConvertToJsonCommentModel(newComment) );
        }

        [HttpPost]
        public ActionResult EditComment(string commentId, CommentModel newComment)
        {
            var comment = _commentManager.GetCommentById(commentId);
            comment.Comments.Find(x => x.Id == newComment.Id).Content = newComment.Content;
            comment.Comments.Find(x => x.Id == newComment.Id).Modified = DateTime.Now; // to fix
            _commentManager.UpdateComment(comment);
            return Json(ConvertToJsonCommentModel(comment.Comments.Find(x => x.Id == newComment.Id)));
        }

        [HttpPost]
        public ActionResult DeleteComment(string commentId, CommentModel newComment)
        {
            var comment = _commentManager.GetCommentById(commentId);
            HashSet<string> idsToRemove = CollectCommentRecursively(comment.Comments, newComment.Id);
            comment.Comments = DeleteComments(idsToRemove, comment.Comments);
            _commentManager.UpdateComment(comment);
            return Json( true );
        }

        private HashSet<string> CollectCommentRecursively(List<CommentModel> comments, string commentId)
        {
            HashSet<string> idsToRemove = new HashSet<string>();
            idsToRemove.Add(commentId);

            HashSet<string> additionalIds;

            foreach (var cmnt in comments)
            {
                if (cmnt.Parent == commentId)
                {
                    additionalIds = CollectCommentRecursively(comments, cmnt.Id);
                    idsToRemove.UnionWith(additionalIds);
                }
            }
            return idsToRemove;
        }

        private List<CommentModel> DeleteComments(HashSet<string> idsToDelete, List<CommentModel> comments)
        {
            List<CommentModel> commentsList = new List<CommentModel>();
            foreach (var cmnt in comments)
            {
                if (!idsToDelete.Contains(cmnt.Id))
                {
                    commentsList.Add(cmnt);
                }
            }
            return commentsList;
        }

        [HttpPost]
        public ActionResult VoteComment(string commentId, CommentModel newComment)
        {
            var userId = User.Identity.GetUserId();
            var comment = _commentManager.GetCommentById(commentId);
            comment.Comments.Find(x => x.Id == newComment.Id).UpvoteCount = newComment.UpvoteCount;
            if (newComment.UserHasUpvoted)
            {
                comment.Comments.Find(x => x.Id == newComment.Id).Voters.Add(userId);
            }
            else
            {
                comment.Comments.Find(x => x.Id == newComment.Id).Voters.Remove(userId);
            }
            comment.Comments.Find(x => x.Id == newComment.Id).UserHasUpvoted = newComment.UserHasUpvoted;

            _commentManager.UpdateComment(comment);
            return Json(ConvertToJsonCommentModel(comment.Comments.Find(x => x.Id == newComment.Id)));
        }

        private JsonCommentModel ConvertToJsonCommentModel(CommentModel comment)
        {
            //string createdStr = JsonConvert.SerializeObject(comment.Created, new IsoDateTimeConverter());
            string createdStr = String.Format("{0:u}", comment.Created);
            string modifiedStr = String.Format("{0:u}", comment.Modified);
            JsonCommentModel jcm = new JsonCommentModel { Id = comment.Id, Parent = comment.Parent, Created = createdStr, Modified = modifiedStr, Content = comment.Content, Fullname = comment.Fullname, ProfilePic = comment.ProfilePic, CreatedByAdmin = comment.CreatedByAdmin, CreatedByCurrentUser = comment.CreatedByCurrentUser, UpvoteCount = comment.UpvoteCount, UserHasUpvoted = comment.UserHasUpvoted };
            return jcm;
        }

        private List<JsonCommentModel> ConvertToJsonCommentModelList(List<CommentModel> comments)
        {
            List<JsonCommentModel> jcmList = comments.ConvertAll(x => ConvertToJsonCommentModel(x));
            return jcmList;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AphrieTask.BE;
using AphrieTask.Manager;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace AphrieTask.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Post")]
    public class PostController : Controller
    {
        private IBaseRepository<Friend> _friendRepository;
        private IBaseRepository<Post> _postRepository;
        private IBaseRepository<PostInteraction> _postInteractionRepository;
        private IBaseRepository<LoacalizProperty> _loacalizPropertyRepository;

        private PostManager _postManager;



        public PostController(IBaseRepository<Friend> friendRepository,
            IBaseRepository<Post> postRepository,
            IBaseRepository<LoacalizProperty> loacalizPropertyRepository,
            IBaseRepository<PostInteraction> postInteractionRepository)
        {
            _friendRepository = friendRepository;
            _postRepository = postRepository;
            _loacalizPropertyRepository = loacalizPropertyRepository;
            _postInteractionRepository = postInteractionRepository;

            _postManager = new PostManager(_friendRepository, _postRepository, _loacalizPropertyRepository, _postInteractionRepository);
        }


        [HttpGet("GetMyPosts/{profileID}")]
        public IActionResult GetMyPosts(Guid profileID)
        {
            try
            {

                if (!Request.Headers.ContainsKey("Language"))
                {
                    return BadRequest("Header not contain Language");
                }
                var language = Request.Headers["Language"];

                // Get the claims values
                var Claim = User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                               .Select(c => c.Value).SingleOrDefault();

                if (profileID.ToString() != Claim)
                {
                    return Unauthorized();
                }

                var res = _postManager.GetMyPosts(profileID, language);
                return Ok(res);
            }
            catch
            {
                return BadRequest();
            }

        }

        [HttpGet("GetPostsCanSee/{profileID}")]
        public IActionResult GetPostsCanSee(Guid profileID)
        {
            try
            {
                if (!Request.Headers.ContainsKey("Language"))
                {
                    return BadRequest("Header not contain Language");
                }
                var language = Request.Headers["Language"];

                // Get the claims values
                var Claim = User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                                   .Select(c => c.Value).SingleOrDefault();

                if (profileID.ToString() != Claim)
                {
                    return Unauthorized();
                }

                var res = _postManager.GetPostsCanSee(profileID, language);
                return Ok(res);
            }
            catch
            {
                return BadRequest();
            }

        }

        [HttpPost("AddNewPost")]
        public IActionResult AddNewPost([FromBody] BE.PostLocalizeInfo postLocalizeInfo)
        {

            PostAdditionResult result = new PostAdditionResult();

            try
            {
                // Get the claims values
                var Claim = User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                                   .Select(c => c.Value).SingleOrDefault();

                if (postLocalizeInfo.postInfo.profileId.ToString() != Claim)
                {
                    return Unauthorized();
                }

                var res = _postManager.AddNewPost(postLocalizeInfo);
                result.result = res;

                if (res)
                {
                    result.Msg = "your post has been added";

                }
                else
                {
                    result.Msg = "Can not add this post";

                }
                return Ok(result);
            }
            catch
            {
                result.result = false;
                return BadRequest(result);
            }
        }

        [HttpPost("AddNewPostInteraction")]
        public IActionResult AddNewPostInteraction([FromBody] PostInteraction postInteraction)
        {
            try
            {
                // Get the claims values
                var Claim = User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                                   .Select(c => c.Value).SingleOrDefault();

                if (postInteraction.userInteractId.ToString() != Claim)
                {
                    return Unauthorized();
                }

                var res = _postManager.AddNewPostInteraction(postInteraction);
                return Ok(res);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("GetPostInteraction/{postId}")]
        public IActionResult GetPostInteraction(Guid postId)
        {
            try
            {
                var res = _postManager.GetPostInteraction(postId);
                return Ok(res);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("DeletePost/{postId}")]
        public IActionResult DeletePost(Guid postId)
        {
            PostAdditionResult result = new PostAdditionResult();

            try
            {
                _postManager.RemovePost(postId);

                result.result = true;
                result.Msg = "your post has been deleted";

                return Ok(result);
            }
            catch
            {
                result.result = false;
                return BadRequest(result);
            }
        }



    }
}

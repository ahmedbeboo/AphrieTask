using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using AphrieTask.BE;
using AphrieTask.Manager;
using Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace AphrieTask.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Post")]
    public class PostController : Controller
    {
        IWebHostEnvironment _hostingEnvironment;


        private IBaseRepository<Friend> _friendRepository;
        private IBaseRepository<Post> _postRepository;
        private IBaseRepository<PostInteraction> _postInteractionRepository;
        private IBaseRepository<LoacalizProperty> _loacalizPropertyRepository;

        private PostManager _postManager;



        public PostController(IBaseRepository<Friend> friendRepository,
            IBaseRepository<Post> postRepository,
            IBaseRepository<LoacalizProperty> loacalizPropertyRepository,
            IBaseRepository<PostInteraction> postInteractionRepository,
            IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;

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
            string postImagesUrl = "";

            try
            {
                var files = Request.Form.Files;
                
                var folderName = Path.Combine(_hostingEnvironment.WebRootPath, "PostsImages");

                if (files.Any(f => f.Length == 0))
                {
                    return BadRequest();
                }

                string PrefixFileName = Guid.NewGuid().ToString();
                int fileCount = 1;

                foreach (var file in files)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(folderName + "/" + PrefixFileName);
                    if (! directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                        folderName= Path.Combine(folderName, PrefixFileName);
                    }

                    string fileExtention= Path.GetExtension(file.FileName);


                    var fileName = PrefixFileName+"_"+fileCount+"."+fileExtention;
                    var fullPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    postImagesUrl += fileName + ",";
                    fileCount++;
                }

                if (postImagesUrl.Length>0)
                {
                    postImagesUrl = postImagesUrl.Remove(postImagesUrl.Length - 1);
                }
            }
            catch
            { }


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

                postLocalizeInfo.postInfo.imagesUrl = postImagesUrl;
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
        public async Task<IActionResult> AddNewPostInteraction([FromBody] PostInteraction postInteraction)
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

                var res =await _postManager.AddNewPostInteraction(postInteraction);

                if (res)
                    return Ok(res);

                return BadRequest(res);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("GetPostInteraction/{postId}")]
        public async Task<IActionResult> GetPostInteraction(Guid postId)
        {
            try
            {
                var res =await _postManager.GetPostInteraction(postId);
                return Ok(res);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("DeletePost/{postId}")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            PostAdditionResult result = new PostAdditionResult();

            try
            {
                var deleted =await _postManager.RemovePost(postId);

                if (deleted)
                {
                    result.result = true;
                    result.Msg = "your post has been deleted";

                    return Ok(result);

                }
                else
                {
                    result.result = true;
                    result.Msg = "your post has not been deleted";

                    return BadRequest(result);

                }
            }
            catch
            {
                result.result = false;
                return BadRequest(result);
            }
        }


        [HttpDelete("DeletePostInteraction/{postInteractionId}")]
        public async Task<IActionResult> DeletePostInteraction(Guid postInteractionId)
        {
            PostAdditionResult result = new PostAdditionResult();

            try
            {
                var deleted =await _postManager.RemovePostInteraction(postInteractionId);

                if (deleted)
                {
                    result.result = true;
                    result.Msg = "your post has been deleted";

                    return Ok(result);

                }
                else
                {
                    result.result = true;
                    result.Msg = "your post has not been deleted";

                    return BadRequest(result);

                }
            }
            catch
            {
                result.result = false;
                return BadRequest(result);
            }
        }

        [HttpPut("EditPostInteraction")]
        public IActionResult EditPostInteraction([FromBody] PostInteraction postInteraction)
        {
            PostAdditionResult result = new PostAdditionResult();

            try
            {
                var edited = _postManager.EditPostInteraction(postInteraction);

                if (edited)
                {
                    result.result = true;
                    result.Msg = "your Interact has been Edited";

                    return Ok(result);

                }
                else
                {
                    result.result = true;
                    result.Msg = "your Interact has not been Edited";

                    return BadRequest(result);

                }
            }
            catch
            {
                result.result = false;
                return BadRequest(result);
            }
        }


        [HttpPut("EditPostInteractionReact/{postInteractionId}/{reactId}")]
        public async Task<IActionResult> EditPostInteractionReact(Guid postInteractionId, Reacts reactId)
        {
            PostAdditionResult result = new PostAdditionResult();

            try
            {
                var Edited =await _postManager.EditPostInteractionReact(postInteractionId, reactId);

                if (Edited)
                {
                    result.result = true;
                    result.Msg = "your React has been Edited";

                    return Ok(result);
                }
                else
                {
                    result.result = true;
                    result.Msg = "your React has not been Edited";

                    return Ok(result);
                }

            }
            catch
            {
                result.result = false;
                return BadRequest(result);
            }
        }
    }
}

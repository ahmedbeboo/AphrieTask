using AphrieTask.BE;
using Entities;
using Entities.Enums;
using Microsoft.AspNetCore.SignalR;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AphrieTask.Manager
{
    public class PostManager
    {
        private IBaseRepository<Friend> _friendRepository;
        private IBaseRepository<Post> _postRepository;
        private IBaseRepository<PostInteraction> _postInteractionRepository;
        private IBaseRepository<LoacalizProperty> _loacalizPropertyRepository;

        public PostManager(IBaseRepository<Friend> friendRepository,
            IBaseRepository<Post> postRepository,
            IBaseRepository<LoacalizProperty> loacalizPropertyRepository,
            IBaseRepository<PostInteraction> postInteractionRepository)
        {
            _friendRepository = friendRepository;
            _postRepository = postRepository;
            _loacalizPropertyRepository = loacalizPropertyRepository;
            _postInteractionRepository = postInteractionRepository;
        }

        public async Task<List<PostLocalizeInfo>> GetMyPosts(Guid profileID, string language)
        {
            List<PostLocalizeInfo> postLocalizeInfoList = new List<PostLocalizeInfo>();
            try
            {
                var posts =await _postRepository.Where(p => p.profileId == profileID && !p.isDeleted);

                foreach (var post in posts)
                {
                    var LocalizeInfo =await _loacalizPropertyRepository.Where(p => p.localizeEntityId == post.Id && p.localizeLanguge.languageName.ToLower() == language.ToLower()&&!p.isDeleted);

                    PostLocalizeInfo postLocalizeInfo = new PostLocalizeInfo();
                    postLocalizeInfo.postInfo = post;
                    postLocalizeInfo.localizeInfo = LocalizeInfo;


                    if (LocalizeInfo.Count > 0 && LocalizeInfo != null)
                    {
                        var postInteractions =await _postInteractionRepository.Where(p => p.postId == post.Id);
                        postLocalizeInfo.postInteractions = postInteractions;
                        
                        postLocalizeInfoList.Add(postLocalizeInfo);

                    }
                }

                return postLocalizeInfoList;
            }
            catch
            {
                return null;
            }
        }

        public bool AddNewPost(PostLocalizeInfo post)
        {
            try
            {
                _postRepository.Insert(post.postInfo);

                foreach (var item in post.localizeInfo)
                {
                    item.localizeEntityId = post.postInfo.Id;
                }
                _loacalizPropertyRepository.InsertRange(post.localizeInfo);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddNewPostInteraction(PostInteraction postInteraction)
        {
            try
            {
                List<Guid> PostsICanInteract =await GetPostsCanInteract(postInteraction.userInteractId);

                if (PostsICanInteract.Contains(postInteraction.postId))
                {
                    _postInteractionRepository.Insert(postInteraction);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Guid>> GetPostsCanInteract(Guid profileID)
        {
            var friendsIdsWhoICanSee =await GetMyCurrentlyFriendsAsync(profileID);

            List<Guid> postCanInteractIds = new List<Guid>();
            try
            {
                var posts =await _postRepository.Where(p => friendsIdsWhoICanSee.Contains(p.profileId) && !p.isDeleted);

                foreach (var post in posts)
                {
                    postCanInteractIds.Add(post.Id);
                }

                return postCanInteractIds;
            }
            catch
            {
                return null;
            }

        }

        public async Task<List<PostLocalizeInfo>> GetPostsCanSee(Guid profileID, string language)
        {
            var friendsIdsWhoICanSee =await GetMyCurrentlyFriendsAsync(profileID);

            List<PostLocalizeInfo> postLocalizeInfoList = new List<PostLocalizeInfo>();
            try
            {
                var posts =await _postRepository.Where(p => friendsIdsWhoICanSee.Contains(p.profileId) && !p.isDeleted);

                foreach (var post in posts)
                {
                    var LocalizeInfo =await _loacalizPropertyRepository.Where(p => p.localizeEntityId == post.Id && p.localizeLanguge.languageName.ToLower() == language.ToLower());

                    PostLocalizeInfo postLocalizeInfo = new PostLocalizeInfo();
                    postLocalizeInfo.postInfo = post;
                    postLocalizeInfo.localizeInfo = LocalizeInfo;

                    if (LocalizeInfo.Count > 0 && LocalizeInfo != null)
                    {
                        var postInteractions =await _postInteractionRepository.Where(p => p.postId == post.Id);
                        postLocalizeInfo.postInteractions = postInteractions;


                        postLocalizeInfoList.Add(postLocalizeInfo);
                    }
                }

                return postLocalizeInfoList;
            }
            catch
            {
                return null;
            }

        }

        public async Task<List<PostInteraction>> GetPostInteraction(Guid postId)
        {
            try
            {
                var postInteraction = await _postInteractionRepository.Where(p => p.postId == postId && !p.isDeleted);
                return postInteraction;
            }
            catch
            {
                return null;
            }
        }

        private async Task<List<Guid>> GetMyCurrentlyFriendsAsync(Guid? clientId)
        {

            var navfriendProperties = new string[] { "ReceiverProfile" };
            var navsenderProperties = new string[] { "SenderProfile" };
            var mymadefriends = await _friendRepository.Where(x => x.SenderProfile.Id == clientId.Value &&
            x.RelationStatus == RelationStatus.Accepted, navfriendProperties);
            var myivnitelist = await _friendRepository.Where(x => x.ReceiverProfile.Id == clientId.Value &&
            x.RelationStatus == RelationStatus.Accepted, navsenderProperties);
            List<Friend> rslt = new List<Friend>();
            rslt.AddRange(mymadefriends);
            rslt.AddRange(myivnitelist);


            List<Guid> profilesID = new List<Guid>();
            foreach (var item in rslt)
            {
                if (item.SenderProfileId == clientId)
                {
                    profilesID.Add(item.receiverProfileId);
                }
                else if (item.receiverProfileId == clientId)
                {
                    profilesID.Add(item.SenderProfileId);
                }
            }
            return profilesID;


        }

        internal async Task<bool> RemovePost(Guid id)
        {
            try
            {
                var post =await _postRepository.GetById(id);
                post.isDeleted = true;
                _postRepository.Update(post);

                var LocalizeInfo =await _loacalizPropertyRepository.Where(p => p.localizeEntityId == post.Id);

                foreach (var item in LocalizeInfo)
                {
                    item.isDeleted = true;
                    _loacalizPropertyRepository.Update(item);
                }

                var PostInteractions =await _postInteractionRepository.Where(p => p.postId == post.Id);
                foreach (var item in PostInteractions)
                {
                    item.isDeleted = true;
                    _postInteractionRepository.Update(item);
                }

                return true;
            }
            catch {
                return false;
            }


        }
        internal async Task<bool> RemovePostInteraction(Guid id)
        {
            try
            {
                List<PostInteraction> UserPostInteractions =await _postInteractionRepository.Where(p => p.Id == id);

                var PostInteractions = UserPostInteractions.FirstOrDefault();
                PostInteractions.isDeleted = true;
                _postInteractionRepository.Update(PostInteractions);

                return true;

            }
            catch {
                return false;
            }


        }


        internal bool EditPostInteraction(PostInteraction postInteraction)
        {
            try
            {
                _postInteractionRepository.Update(postInteraction);
                return true;
            }
            catch {
                return false;
            }


        }

        internal async Task<bool> EditPostInteractionReact(Guid postInteractionId,Reacts react)
        {
            try
            {
                var PostInteractions =await _postInteractionRepository.GetById(postInteractionId);
                PostInteractions.postReact = react;
                _postInteractionRepository.Update(PostInteractions);

                return true;
            }
            catch {
                return false;
            }


        }


    }
}

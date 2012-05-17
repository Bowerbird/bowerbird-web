/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
using System.Linq;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Factories
{
    public class PostViewFactory : IPostViewFactory
    {
        #region Members

        private readonly IMediaFilePathService _mediaFilePathService;
        private readonly IUserViewFactory _userViewFactory;

        #endregion

        #region Constructors

        public PostViewFactory(
            IMediaFilePathService mediaFilePathService,
            IUserViewFactory userViewFactory
            )
        {
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");
            Check.RequireNotNull(userViewFactory, "userViewFactory");

            _mediaFilePathService = mediaFilePathService;
            _userViewFactory = userViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        /// <summary>
        /// TODO: This could be horribly wrong. Does a post have images and/or documents?
        /// How to sort images from documents...
        /// </summary>
        public object Make(Post post)
        {
            return new
            {
                post.Id,
                post.Subject,
                post.Message,
                Creator = _userViewFactory.Make(post.User.Id),
                Comments = post.Discussion.Comments.Select(x => _commentViewFactory.Make(x)),
                Resources = post.MediaResources.Select(x => _mediaFilePathService.MakeMediaFileUri(x, MediaType.Document))
            };
        }
        
        #endregion      
    }
}
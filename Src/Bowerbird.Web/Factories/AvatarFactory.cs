/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;

namespace Bowerbird.Web.Factories
{
    public class AvatarFactory : IAvatarFactory
    {
        #region Fields

        private readonly IMediaFilePathService _mediaFilePathService;

        #endregion

        #region Constructors

        public AvatarFactory(
            IMediaFilePathService mediaFilePathService
        )
        {
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");

            _mediaFilePathService = mediaFilePathService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object MakeDefaultAvatar(AvatarDefaultType avatarType, string altTag)
        {
            return new
            {
                AltTag = altTag,
                UrlToImage = avatarType.AvatarTypeUris()
            };
        }

        public object Make(Team team)
        {
            return new
            {
                AltTag = team.Description,
                UrlToImage = team.Avatar != null ?
                    _mediaFilePathService.MakeMediaFileUri(team.Avatar.Id, "image", "profile", team.Avatar.Metadata["format"]) :
                    AvatarUris.DefaultTeam
            };
        }

        public object Make(Project project)
        {
            return new
            {
                AltTag = project.Description,
                UrlToImage = project.Avatar != null ?
                    _mediaFilePathService.MakeMediaFileUri(project.Avatar.Id, "image", "profile", project.Avatar.Metadata["format"]) :
                    AvatarUris.DefaultProject
            };
        }

        public object Make(Organisation organisation)
        {
            return new
            {
                AltTag = organisation.Description,
                UrlToImage = organisation.Avatar != null ?
                    _mediaFilePathService.MakeMediaFileUri(organisation.Avatar.Id, "image", "profile", organisation.Avatar.Metadata["format"]) :
                    AvatarUris.DefaultOrganisation
            };
        }

        public object Make(User user)
        {
            return new
            {
                AltTag = string.Format("{0} {1}", user.FirstName, user.LastName),
                UrlToImage = user.Avatar != null ?
                    _mediaFilePathService.MakeMediaFileUri(user.Avatar.Id, "image", "profile", user.Avatar.Metadata["format"]) :
                    AvatarUris.DefaultUser
            };
        }

        #endregion
    }
}
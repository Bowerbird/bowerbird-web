﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
using Bowerbird.Web.ViewModels;

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

        public Avatar GetAvatar(Team team)
        {
            return new Avatar()
            {
                AltTag = team.Description,
                UrlToImage = team.Avatar != null ?
                    _mediaFilePathService.MakeMediaFileUri(team.Avatar.Id, "image", "avatar", team.Avatar.Metadata["metatype"]) :
                    AvatarUris.DefaultTeam
            };
        }

        public Avatar GetAvatar(Project project)
        {
            return new Avatar()
            {
                AltTag = project.Description,
                UrlToImage = project.Avatar != null ?
                    _mediaFilePathService.MakeMediaFileUri(project.Avatar.Id, "image", "avatar", project.Avatar.Metadata["metatype"]) :
                    AvatarUris.DefaultProject
            };
        }

        public Avatar GetAvatar(Organisation organisation)
        {
            return new Avatar()
            {
                AltTag = organisation.Description,
                UrlToImage = organisation.Avatar != null ?
                    _mediaFilePathService.MakeMediaFileUri(organisation.Avatar.Id, "image", "avatar", organisation.Avatar.Metadata["metatype"]) :
                    AvatarUris.DefaultOrganisation
            };
        }

        public Avatar GetAvatar(User user)
        {
            return new Avatar()
            {
                AltTag = string.Format("{0} {1}", user.FirstName, user.LastName),
                UrlToImage = user.Avatar != null ?
                    _mediaFilePathService.MakeMediaFileUri(user.Avatar.Id, "image", "avatar", user.Avatar.Metadata["metatype"]) :
                    AvatarUris.DefaultUser
            };
        }

        #endregion
    }
}
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
using System;

namespace Bowerbird.Core.Factories
{
    public class AvatarFactory : IAvatarFactory
    {
        #region Fields

        //private readonly IMediaFilePathService _mediaFilePathService;

        #endregion

        #region Constructors

        //public AvatarFactory(
        //    IMediaFilePathService mediaFilePathService
        //)
        //{
        //    Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");

        //    _mediaFilePathService = mediaFilePathService;
        //}

        #endregion

        #region Properties

        #endregion

        #region Methods

        public MediaResource MakeDefaultAvatar(AvatarDefaultType avatarType)
        {
            var avatar = new MediaResource("image", null, DateTime.Now);

            var filename = string.Format("default-{0}-avatar.jpg", avatarType.ToString().ToLower());
            var relativeUri = string.Format("/img/{0}", filename);

            avatar.AddImageFile("thumbnail", filename, relativeUri, "jpeg", 42, 42, "jpg");
            avatar.AddImageFile("small", filename, relativeUri, "jpeg", 42, 42, "jpg");
            avatar.AddImageFile("medium", filename, relativeUri, "jpeg", 42, 42, "jpg");
            avatar.AddImageFile("large", filename, relativeUri, "jpeg", 42, 42, "jpg");

            return avatar;
        }

        //public object Make(Team team)
        //{
        //    return new
        //    {
        //        AltTag = team.Description,
        //        UrlToImage = team.Avatar != null ?
        //            _mediaFilePathService.MakeRelativeMediaFileUri(team.Avatar.Id, "image", "avatar", team.Avatar["thumbnail"].Metadata["extension"]) :
        //            AvatarUris.DefaultTeam
        //    };
        //}

        //public object Make(Project project)
        //{
        //    return new
        //    {
        //        AltTag = project.Description,
        //        UrlToImage = project.Avatar != null ?
        //            _mediaFilePathService.MakeRelativeMediaFileUri(project.Avatar.Id, "image", "avatar", project.Avatar["thumbnail"].Metadata["extension"]) :
        //            AvatarUris.DefaultProject
        //    };
        //}

        //public object Make(Organisation organisation)
        //{
        //    return new
        //    {
        //        AltTag = organisation.Description,
        //        UrlToImage = organisation.Avatar != null ?
        //            _mediaFilePathService.MakeRelativeMediaFileUri(organisation.Avatar.Id, "image", "avatar", organisation.Avatar["thumbnail"].Metadata["extension"]) :
        //            AvatarUris.DefaultOrganisation
        //    };
        //}

        //public object Make(User user)
        //{
        //    return new
        //    {
        //        AltTag = string.Format("{0} {1}", user.FirstName, user.LastName),
        //        UrlToImage = user.Avatar != null ?
        //            _mediaFilePathService.MakeRelativeMediaFileUri(user.Avatar.Id, "image", "avatar", user.Avatar["thumbnail"].Metadata["extension"]) :
        //            AvatarUris.DefaultUser
        //    };
        //}

        #endregion
    }
}
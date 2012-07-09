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

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public MediaResource MakeDefaultAvatar(AvatarDefaultType avatarType)
        {
            var avatar = new MediaResource("image", null, DateTime.UtcNow);

            var filename = string.Format("default-{0}-avatar.jpg", avatarType.ToString().ToLower());
            var relativeUri = string.Format("/img/{0}", filename);

            avatar.AddImageFile("thumbnail", filename, relativeUri, "jpeg", 42, 42, "jpg");
            avatar.AddImageFile("small", filename, relativeUri, "jpeg", 42, 42, "jpg");
            avatar.AddImageFile("medium", filename, relativeUri, "jpeg", 42, 42, "jpg");
            avatar.AddImageFile("large", filename, relativeUri, "jpeg", 42, 42, "jpg");

            return avatar;
        }

        #endregion
    }
}
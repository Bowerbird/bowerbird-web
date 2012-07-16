/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;

namespace Bowerbird.Web.Services
{
    public class AudioService : IAudioService
    {
        #region Fields

        private List<string> _acceptedFileTypes;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public AudioService(
            IUserContext userContext)
        {
            Check.RequireNotNull(userContext, "userContext");

            _userContext = userContext;

            InitMembers();
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Save(MediaResourceCreateCommand command, MediaResource mediaResource)
        {
            if(!string.IsNullOrWhiteSpace(command.OriginalFileName) && _acceptedFileTypes.Any(x => x.Contains(Path.GetExtension(command.OriginalFileName))))
            {
                MakeAudioMediaResourceFiles(mediaResource, command);
            }
        }

        private void MakeAudioMediaResourceFiles(MediaResource mediaResource, MediaResourceCreateCommand command)
        {
            mediaResource.AddDocumentFile("Audio", command.Key, _userContext.GetUserFullName(), "audio", Path.GetExtension(command.OriginalFileName));
        }

        private void InitMembers()
        {
            _acceptedFileTypes = new List<string>()
                                      {
                                          ".mp3",
                                          ".wav"
                                      };
        }

        #endregion
    }
}
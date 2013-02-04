/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Collections.Generic;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModelFactories;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Services;
using Bowerbird.Core.Utilities;
using NLog;
using Raven.Client;

namespace Bowerbird.Web.Services
{
    public class AudioService : IAudioService
    {
        #region Fields

        private Logger _logger = LogManager.GetLogger("AudioService");

        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathFactory _mediaFilePathFactory;
        private readonly IMediaResourceFactory _mediaResourceFactory;

        #endregion

        #region Constructors

        public AudioService(
            IUserContext userContext,
            IDocumentSession documentSession,
            IMediaFilePathFactory mediaFilePathFactory,
            IMessageBus messageBus,
            IMediaResourceFactory mediaResourceFactory)
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathFactory, "mediaFilePathFactory");
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(mediaResourceFactory, "mediaResourceFactory");

            _documentSession = documentSession;
            _mediaFilePathFactory = mediaFilePathFactory;
            _mediaResourceFactory = mediaResourceFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public bool Save(MediaResourceCreateCommand command, User createdByUser, out string failureReason, out MediaResource mediaResource)
        {
            failureReason = string.Empty;
            mediaResource = null;

            if (!_documentSession.Load<AppRoot>(Constants.AppRootId).AudioServiceStatus)
            {
                failureReason = "Audio files cannot be uploaded at the moment. Please try again later.";
                return false;
            }

            bool returnValue;

            try
            {
                var audio = AudioUtility.Load(command.FileStream, command.FileName, command.FileMimeType);

                mediaResource = _mediaResourceFactory.MakeContributionAudio(
                    command.Key,
                    createdByUser,
                    command.UploadedOn,
                    command.FileName,
                    new Object(),
                    command.FileMimeType,
                    GetAudioMetadata(audio));

                string filePath = _mediaFilePathFactory.MakeMediaFilePath(mediaResource.Id, "Original", MediaTypeUtility.GetStandardExtensionForMimeType(command.FileMimeType));

                audio.SaveAs(filePath);

                returnValue = true;
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Error saving audio", exception);

                failureReason = "The file is corrupted or not a valid audio file and could not be saved. Please check the file and try again.";
                returnValue = false;
            }

            return returnValue;
        }

        private Dictionary<string, string> GetAudioMetadata(AudioUtility audio)
        {
            var metadata = new Dictionary<string, string>();

            metadata.Add("Description", audio.GetTitleTag());
            metadata.Add("Duration", audio.GetDuration().ToString());

            return metadata;
        }

        #endregion
    }
}
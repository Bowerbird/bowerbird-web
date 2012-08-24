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
using Bowerbird.Core.Factories;
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

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathFactory _mediaFilePathFactory;
        private readonly IMessageBus _messageBus;
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

            _userContext = userContext;
            _documentSession = documentSession;
            _mediaFilePathFactory = mediaFilePathFactory;
            _messageBus = messageBus;
            _mediaResourceFactory = mediaResourceFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public bool Save(MediaResourceCreateCommand command, out string failureReason)
        {
            if (!_documentSession.Load<AppRoot>(Constants.AppRootId).AudioServiceStatus)
            {
                failureReason = "Audio files cannot be uploaded at the moment. Please try again later.";
                return false;
            }

            MediaResource mediaResource = null;
            bool returnValue;

            try
            {
                var createdByUser = _documentSession.Load<User>(command.UserId);

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

                _messageBus.Publish(new DomainModelCreatedEvent<MediaResource>(mediaResource, createdByUser, mediaResource));

                failureReason = string.Empty;
                returnValue = true;
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Error saving audio", exception);

                if (mediaResource != null)
                {
                    _documentSession.Delete(mediaResource);
                    _documentSession.SaveChanges();
                }

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
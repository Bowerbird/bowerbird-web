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
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Services;
using NLog;
using Raven.Client;
using System.Linq;
using Bowerbird.Core.Factories;
using Bowerbird.Web.Utilities;

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

        #endregion

        #region Constructors

        public AudioService(
            IUserContext userContext,
            IDocumentSession documentSession,
            IMediaFilePathFactory mediaFilePathFactory,
            IMessageBus messageBus)
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathFactory, "mediaFilePathFactory");
            Check.RequireNotNull(messageBus, "messageBus");

            _userContext = userContext;
            _documentSession = documentSession;
            _mediaFilePathFactory = mediaFilePathFactory;
            _messageBus = messageBus;
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

                //var audioFile = AudioUtility.Load(command.Stream, command.OriginalFileName, command.MimeType);

                //if (!audioFile.IsValidAudioFile())
                //{
                //    mediaResource = null;
                //    failureReason = "The file is corrupted or not a valid audio file and could not be saved. Please check the file and try again.";
                //    return false;
                //}

                //MakeAudioMediaResourceFiles(mediaResource, command, audioFile);

                //string filePath = _mediaFilePathFactory.MakeMediaFilePath(mediaResource.Id, "audio", "Original", audioFile.GetFileExtension());

                //audioFile.Save(filePath);

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

        //private void MakeAudioMediaResourceFiles(MediaResource mediaResource, MediaResourceCreateCommand command, AudioUtility audioFile)
        //{
        //    string fileName = _mediaFilePathFactory.MakeMediaFileName(mediaResource.Id, "Original", audioFile.GetFileExtension());
        //    string uri = _mediaFilePathFactory.MakeRelativeMediaFileUri(mediaResource.Id, "audio", "Original", audioFile.GetFileExtension());

        //    dynamic original = mediaResource.AddAudioFile("Original", command.OriginalFileName, uri, audioFile.GetMimeType(), audioFile.GetFileExtension());

        //    original.Title = audioFile.GetTitleTagValue();
        //    original.Copyright = audioFile.GetCopyrightTagValue();
        //    original.Comment = audioFile.GetCommentTagValue();

        //    mediaResource.AddAudioFile("Square50", fileName, uri, audioFile.GetMimeType(), audioFile.GetFileExtension());
        //    mediaResource.AddAudioFile("Square100", fileName, uri, audioFile.GetMimeType(), audioFile.GetFileExtension());
        //    mediaResource.AddAudioFile("Square200", fileName, uri, audioFile.GetMimeType(), audioFile.GetFileExtension());
        //    mediaResource.AddAudioFile("Full480", fileName, uri, audioFile.GetMimeType(), audioFile.GetFileExtension());
        //    mediaResource.AddAudioFile("Full768", fileName, uri, audioFile.GetMimeType(), audioFile.GetFileExtension());
        //    mediaResource.AddAudioFile("Full1024", fileName, uri, audioFile.GetMimeType(), audioFile.GetFileExtension());
        //}

        #endregion
    }
}
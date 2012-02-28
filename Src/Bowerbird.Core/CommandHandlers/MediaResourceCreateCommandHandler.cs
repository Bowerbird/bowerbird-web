/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Commands;
using Raven.Client;
using Bowerbird.Core.ImageUtilities;
using System.IO;
using Bowerbird.Core.Services;
using System.Collections.Generic;

namespace Bowerbird.Core.CommandHandlers
{
    public class MediaResourceCreateCommandHandler : ICommandHandler<MediaResourceCreateCommand, string>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathService _mediaFilePathService;

        #endregion

        #region Constructors

        public MediaResourceCreateCommandHandler(
            IDocumentSession documentSession,
            IMediaFilePathService mediaFilePathService)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");

            _documentSession = documentSession;
            _mediaFilePathService = mediaFilePathService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public string Handle(MediaResourceCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            try
            {
                string mediaType = DetemineMediaType(command);
                var metadata = new Dictionary<string, string>();

                var mediaResource = new MediaResource(
                                mediaType,
                                _documentSession.Load<User>(command.UserId),
                                command.UploadedOn,
                                metadata);

                _documentSession.Store(mediaResource);

                SaveOriginalMedia(command.Stream);

                metadata.Add("size", command.Stream.Length.ToString());
                metadata.Add("originalfilename", command.OriginalFileName);

                switch (mediaType)
                {
                    case "image":
                        var imageDimensions = GetImageDimensions();

                        metadata.Add("width", imageDimensions.Width.ToString());
                        metadata.Add("height", imageDimensions.Height.ToString());

                        if (command.Usage == "observation")
                        {
                            SaveObservationImages();
                        }
                        break;
                }

                return mediaResource.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string DetemineMediaType(MediaResourceCreateCommand command)
        {
            // TODO: Determine media type here, assume images only for now
            return "image";
        }

        private ImageDimensions GetImageDimensions()
        {
            //ImageDimensions imageDimensions;

            //ImageUtility
            //    .Load(command.Stream)
            //    .GetImageDimensions(out imageDimensions)
            //    .Cleanup();

            //return 
            throw new NotImplementedException();
        }

        private void SaveOriginalMedia(Stream stream)
        {
            throw new NotImplementedException();
        }

        private void SaveAvatarImages()
        {
            throw new NotImplementedException();
        }

        private void SaveObservationImages()
        {
            //var imageMediaResource = new ImageMediaResource(
            //            _documentSession.Load<User>(_userContext.GetAuthenticatedUserId()),
            //            DateTime.Now,
            //            postedFileName,
            //            Path.GetExtension(postedFileName),
            //            string.Empty,
            //            imageDimensions.Height,
            //            imageDimensions.Width
            //            );

            //_documentSession.Store(imageMediaResource);

            //ImageUtility
            //    .Load(command.Stream)
            //    .GetImageDimensions(out imageDimensions)
            //    .SaveAs(_mediaFilePathService.MakeMediaFilePath(imageMediaResource.Id, "image", "original", Path.GetExtension(postedFileName)))
            //    .Cleanup();
            throw new NotImplementedException();
        }

        #endregion
    }
}
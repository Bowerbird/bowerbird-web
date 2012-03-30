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
    public class MediaResourceCreateCommandHandler : ICommandHandler<MediaResourceCreateCommand, MediaResource>
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

        public MediaResource Handle(MediaResourceCreateCommand command)
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

                metadata.Add("size", command.Stream.Length.ToString());
                metadata.Add("originalfilename", command.OriginalFileName);

                switch (mediaType)
                {
                    case "image":
                        string extension = "jpg";
                        if(Path.HasExtension(command.OriginalFileName))
                        {
                            extension = Path.GetExtension(command.OriginalFileName).Replace(".", string.Empty).ToLower();
                        }

                        ImageDimensions dimensions;
                        SaveOriginalImageMedia(
                            command.Stream,
                            mediaResource.Id,
                            extension,
                            out dimensions);

                        metadata.Add("width", dimensions.Width.ToString());
                        metadata.Add("height", dimensions.Height.ToString());
                        metadata.Add("format", extension);

                        if (command.Usage == "observation")
                        {
                            SaveObservationImages(
                                command.Stream,
                                mediaResource.Id,
                                extension
                                );
                        }
                        else if (command.Usage == "user")
                        {
                            SaveUserImages(
                                command.Stream,
                                mediaResource.Id,
                                extension
                                );
                        }
                        else if (command.Usage == "post")
                        {
                            SavePostImages(
                                command.Stream,
                                mediaResource.Id,
                                extension
                                );
                        }

                        break;
                }

                _documentSession.Store(mediaResource);
                _documentSession.SaveChanges();

                return mediaResource;
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

        private void SaveOriginalImageMedia(Stream stream, string imageMediaResourceId, string extension, out ImageDimensions imageDimensions)
        {
            ImageUtility
                .Load(stream)
                .GetImageDimensions(out imageDimensions)
                .SaveAs(_mediaFilePathService.MakeMediaFilePath(imageMediaResourceId, "image", "original", extension))
                .Cleanup();
        }

        private void SaveUserImages(Stream stream, string imageMediaResourceId, string extension)
        {
            ImageUtility
                .Load(stream)
                .Resize(new ImageDimensions(42, 42), true, ImageResizeMode.Crop)
                .SaveAs(_mediaFilePathService.MakeMediaFilePath(imageMediaResourceId, "image", "thumbnail", extension))
                .Reset()
                .Resize(new ImageDimensions(100, 100), true, ImageResizeMode.Crop)
                .SaveAs(_mediaFilePathService.MakeMediaFilePath(imageMediaResourceId, "image", "profile", extension))
                .Cleanup();
        }

        private void SaveObservationImages(Stream stream, string imageMediaResourceId, string extension)
        {
            ImageUtility
                .Load(stream)
                .Resize(new ImageDimensions(42, 42), true, ImageResizeMode.Crop)
                .SaveAs(_mediaFilePathService.MakeMediaFilePath(imageMediaResourceId, "image", "thumbnail", extension))
                .Reset()
                .Resize(new ImageDimensions(130, 120), true, ImageResizeMode.Crop)
                .SaveAs(_mediaFilePathService.MakeMediaFilePath(imageMediaResourceId, "image", "small", extension))
                .Reset()
                .Resize(new ImageDimensions(670, 600), true, ImageResizeMode.Crop)
                .SaveAs(_mediaFilePathService.MakeMediaFilePath(imageMediaResourceId, "image", "medium", extension))
                .Reset()
                .Resize(new ImageDimensions(1600, 1200), true, ImageResizeMode.Crop)
                .SaveAs(_mediaFilePathService.MakeMediaFilePath(imageMediaResourceId, "image", "large", extension))
                .Cleanup();
        }

        private void SavePostImages(Stream stream, string imageMediaResourceId, string extension)
        {
            ImageUtility
                .Load(stream)
                .Resize(new ImageDimensions(42, 42), true, ImageResizeMode.Crop)
                .SaveAs(_mediaFilePathService.MakeMediaFilePath(imageMediaResourceId, "image", "thumbnail", extension))
                .Reset()
                .Resize(new ImageDimensions(130, 120), true, ImageResizeMode.Crop)
                .SaveAs(_mediaFilePathService.MakeMediaFilePath(imageMediaResourceId, "image", "small", extension))
                .Reset()
                .Resize(new ImageDimensions(670, 600), true, ImageResizeMode.Crop)
                .SaveAs(_mediaFilePathService.MakeMediaFilePath(imageMediaResourceId, "image", "medium", extension))
                .Reset()
                .Resize(new ImageDimensions(1600, 1200), true, ImageResizeMode.Crop)
                .SaveAs(_mediaFilePathService.MakeMediaFilePath(imageMediaResourceId, "image", "large", extension))
                .Cleanup();
        }

        #endregion
    }
}
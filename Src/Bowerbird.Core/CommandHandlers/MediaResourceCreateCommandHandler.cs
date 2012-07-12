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
using System.Linq;
using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.VideoUtilities;
using Bowerbird.Core.ImageUtilities;
using Bowerbird.Core.Factories;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class MediaResourceCreateCommandHandler 
        : ICommandHandler<MediaResourceCreateCommand, MediaResource>,
        ICommandHandler<MediaResourceCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathFactory _mediaFilePathFactory;
        private readonly IVideoUtility _videoUtility;

        #endregion

        #region Constructors

        public MediaResourceCreateCommandHandler(
            IDocumentSession documentSession,
            IMediaFilePathFactory mediaFilePathFactory,
            IVideoUtility videoUtility)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathFactory, "mediaFilePathFactory");
            Check.RequireNotNull(videoUtility, "videoUtility");

            _documentSession = documentSession;
            _mediaFilePathFactory = mediaFilePathFactory;
            _videoUtility = videoUtility;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public MediaResource HandleReturn(MediaResourceCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            ImageUtility image = null;

            try
            {
                string mediaType = DetemineMediaType(command);

                var mediaResource = new MediaResource(
                                mediaType,
                                _documentSession.Load<User>(command.UserId),
                                command.UploadedOn,
                                command.Key);

                _documentSession.Store(mediaResource);

                switch (mediaType)
                {
                    case "image":
                        ImageDimensions imageDimensions;

                        IDictionary<string, object> exifData;
                        var imageCreationTasks = new List<ImageCreationTask>();

                        image = ImageUtility
                            .Load(command.Stream)
                            .GetExifData(out exifData)
                            .GetImageDimensions(out imageDimensions);

                        MakeOriginalImageMediaResourceFile(mediaResource, imageCreationTasks, command.OriginalFileName, command.Stream.Length, imageDimensions, exifData);

                        if (command.Usage == "observation")
                        {
                            MakeObservationImageMediaResourceFiles(mediaResource, imageCreationTasks);
                        }
                        else if (command.Usage == "post")
                        {
                            MakePostImageMediaResourceFiles(mediaResource, imageCreationTasks);
                        }
                        else if (command.Usage == "user")
                        {
                            MakeUserImageMediaResourceFiles(mediaResource, imageCreationTasks);
                        }
                        else if (command.Usage == "group")
                        {
                            MakeGroupImageMediaResourceFiles(mediaResource, imageCreationTasks);
                        }
                        else
                        {
                            MakeOtherImageMediaResourceFiles(mediaResource, imageCreationTasks);
                        }

                        SaveImages(image, mediaResource, imageCreationTasks);
                        break;
                }

                _documentSession.Store(mediaResource);

                return mediaResource;
            }
            catch (Exception ex)
            {
                if (image != null)
                    image.Cleanup();
                
                throw ex;
            }
        }

        public void Handle(MediaResourceCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            ImageUtility image = null;

            try
            {
                var mediaType = command.MediaType;

                var user = _documentSession.Load<User>(command.UserId);

                var mediaResource = new MediaResource(
                                mediaType,
                                user,
                                command.UploadedOn,
                                command.Key);

                switch (mediaType)
                {
                    case "image":
                        {
                            _documentSession.Store(mediaResource);

                            ImageDimensions imageDimensions;

                            IDictionary<string, object> exifData;
                            var imageCreationTasks = new List<ImageCreationTask>();

                            image = ImageUtility
                                .Load(command.Stream)
                                .GetExifData(out exifData)
                                .GetImageDimensions(out imageDimensions);

                            MakeOriginalImageMediaResourceFile(
                                mediaResource, 
                                imageCreationTasks,
                                command.OriginalFileName, 
                                command.Stream.Length,
                                imageDimensions, 
                                exifData);

                            if (command.Usage == "observation")
                            {
                                MakeObservationImageMediaResourceFiles(mediaResource, imageCreationTasks);
                            }
                            else if (command.Usage == "post")
                            {
                                MakePostImageMediaResourceFiles(mediaResource, imageCreationTasks);
                            }
                            else if (command.Usage == "user")
                            {
                                MakeUserImageMediaResourceFiles(mediaResource, imageCreationTasks);
                            }
                            else if (command.Usage == "group")
                            {
                                MakeGroupImageMediaResourceFiles(mediaResource, imageCreationTasks);
                            }
                            else
                            {
                                MakeOtherImageMediaResourceFiles(mediaResource, imageCreationTasks);
                            }

                            SaveImages(image, mediaResource, imageCreationTasks);
                        }
                        break;

                    case "video":
                        {
                            string provider; // playback service - youtube, vimeo et al
                            string videoId; // unique identifier for video on playback service
                            string embedString; // the embed html tags with format options for video id and sizes

                            if (_videoUtility.IsValidVideo(command.LinkUri, out embedString, out videoId, out provider))
                            {
                                mediaResource
                                    .AddMetadata("Url", command.LinkUri)
                                    .AddMetadata("Provider", provider)
                                    .AddMetadata("VideoId", videoId);
                                
                                MakeVideoMediaResourceFiles(
                                    mediaResource,
                                    embedString.AppendWith(videoId),
                                    command.LinkUri,
                                    provider,
                                    videoId);
                            }
                        }

                        break;
                }

                _documentSession.Store(mediaResource);

                mediaResource.FireCreatedEvent(user);
            }
            catch (Exception ex)
            {
                if (image != null)
                    image.Cleanup();
                
                throw ex;
            }
        }

        private string DetemineMediaType(MediaResourceCreateCommand command)
        {
            // TODO: Determine media type here, assume images only for now
            return "image";//command.MediaType;
        }

        private void MakeOriginalImageMediaResourceFile(MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks, string originalFileName, long size, ImageDimensions imageDimensions, IDictionary<string, object> exifData)
        {
            string format = "jpeg"; // TODO: Handle formats other than JPEG
            string extension = "jpg";

            dynamic file = AddImageFile(mediaResource, imageCreationTasks, "Original", format, extension, imageDimensions.Width, imageDimensions.Height, null, null);
            file.Size = size.ToString();
            file.OriginalFilename = originalFileName;
            file.ExifData = exifData;

            if (exifData != null && exifData.Count > 0)
            {
                SetImageExifMetadata(mediaResource, file);
            }
        }

        private void MakeObservationImageMediaResourceFiles(MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks)
        {
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailSmall", "jpeg", "jpg", 42, 42, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailMedium", "jpeg", "jpg", 100, 100, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailLarge", "jpeg", "jpg", 200, 200, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "FullSmall", "jpeg", "jpg", 640, 480, false, ImageResizeMode.Normal);
            AddImageFile(mediaResource, imageCreationTasks, "FullMedium", "jpeg", "jpg", 1024, 768, false, ImageResizeMode.Normal);
            AddImageFile(mediaResource, imageCreationTasks, "FullLarge", "jpeg", "jpg", 1280, 1024, false, ImageResizeMode.Normal);
        }

        private void MakeVideoMediaResourceFiles(MediaResource mediaResource, string embedScript, string linkUri, string provider, string videoId)
        {
            mediaResource.AddVideoFile("Fullsize", linkUri, embedScript, provider, videoId, "672", "400");
            mediaResource.AddVideoFile("Preview", linkUri, embedScript, provider, videoId, "220", "200");
            mediaResource.AddVideoFile("Small", linkUri, embedScript, provider, videoId, "120", "80");
            mediaResource.AddVideoFile("Thumb", linkUri, embedScript, provider, videoId, "60", "40");
        }   

        private void MakePostImageMediaResourceFiles(MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks)
        {
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailSmall", "jpeg", "jpg", 42, 42, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailMedium", "jpeg", "jpg", 100, 100, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailLarge", "jpeg", "jpg", 200, 200, false, ImageResizeMode.Crop);
        }

        private void MakeUserImageMediaResourceFiles(MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks)
        {
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailSmall", "jpeg", "jpg", 42, 42, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailMedium", "jpeg", "jpg", 100, 100, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailLarge", "jpeg", "jpg", 200, 200, false, ImageResizeMode.Crop);
        }

        private void MakeGroupImageMediaResourceFiles(MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks)
        {
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailSmall", "jpeg", "jpg", 42, 42, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailMedium", "jpeg", "jpg", 100, 100, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailLarge", "jpeg", "jpg", 200, 200, false, ImageResizeMode.Crop);
        }

        private void MakeOtherImageMediaResourceFiles(MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks)
        {
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailSmall", "jpeg", "jpg", 42, 42, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailMedium", "jpeg", "jpg", 100, 100, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailLarge", "jpeg", "jpg", 200, 200, false, ImageResizeMode.Crop);
        }

        private MediaResourceFile AddImageFile(MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks, string storedRepresentation, string format, string extension, int width, int height, bool? determineBestOrientation, ImageResizeMode? imageResizeMode)
        {
            var mediaResourceFile = mediaResource.AddImageFile(
                storedRepresentation,
                _mediaFilePathFactory.MakeMediaFileName(mediaResource.Id, storedRepresentation, extension),
                _mediaFilePathFactory.MakeRelativeMediaFileUri(mediaResource.Id, "image", storedRepresentation, extension),
                format,
                width,
                height,
                extension);

            imageCreationTasks.Add(new ImageCreationTask 
                { 
                    File = mediaResourceFile, 
                    StoredRepresentation = storedRepresentation,
                    DetermineBestOrientation = determineBestOrientation, 
                    ImageResizeMode = imageResizeMode
                });

            return mediaResourceFile;
        }

        private void SaveImages(ImageUtility image, MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks)
        {
            foreach (var imageCreationTask in imageCreationTasks)
            {
                dynamic imageFile = imageCreationTask.File;

                var fullPath = _mediaFilePathFactory.MakeMediaFilePath(mediaResource.Id, "image", imageCreationTask.StoredRepresentation, imageFile.Extension);

                if (!imageCreationTask.DoImageManipulation())
                {
                    image
                        .Reset()
                        .SaveAs(fullPath);
                }
                else
                {
                    image
                        .Reset()
                        .Resize(new ImageDimensions(imageFile.Width, imageFile.Height), imageCreationTask.DetermineBestOrientation.Value, imageCreationTask.ImageResizeMode.Value)
                        .SaveAs(fullPath);
                }
            }

            image.Cleanup();
        }

        private void SetImageExifMetadata(MediaResource mediaResource, dynamic originalFile)
        {
            if (originalFile.ExifData.ContainsKey(ExifLib.ExifTags.GPSLatitude.ToString()) &&
                originalFile.ExifData.ContainsKey(ExifLib.ExifTags.GPSLongitude.ToString()) &&
                originalFile.ExifData.ContainsKey(ExifLib.ExifTags.GPSLatitudeRef.ToString()) &&
                originalFile.ExifData.ContainsKey(ExifLib.ExifTags.GPSLongitudeRef.ToString()))
            {
                var latitudeConverted = ConvertDegreeAngleToDouble(
                    originalFile.ExifData[ExifLib.ExifTags.GPSLatitude.ToString()] as double[],
                    originalFile.ExifData[ExifLib.ExifTags.GPSLatitudeRef.ToString()] as string);

                var longitudeConverted = ConvertDegreeAngleToDouble(
                    originalFile.ExifData[ExifLib.ExifTags.GPSLongitude.ToString()] as double[],
                    originalFile.ExifData[ExifLib.ExifTags.GPSLongitudeRef.ToString()] as string);

                if (latitudeConverted.HasValue && longitudeConverted.HasValue)
                {
                    mediaResource.AddMetadata("Latitude", latitudeConverted.Value.ToString());
                    mediaResource.AddMetadata("Longitude", longitudeConverted.Value.ToString());
                }
            }

            if (originalFile.ExifData.ContainsKey(ExifLib.ExifTags.DateTime.ToString()))
            {
                var dateTimeTakenExif = originalFile.ExifData[ExifLib.ExifTags.DateTime.ToString()].ToString();
                
                var convertedDateTime = ConvertDateTime(originalFile.ExifData[ExifLib.ExifTags.DateTime.ToString()].ToString());

                // TODO: Format date into ISO8601 yyyy-MM-ddThh:mm:ssZ - also, is the EXIF datetime local/UTC time?
                mediaResource.AddMetadata("DateTaken", convertedDateTime.ToString("dd MMM yyyy"));
            }
        }

        // In geographic coordinates stored as EXIF data, the coordinates for latitude and longitude are stored
        // as unsigned doubles in an array. Latitudes are the meridian lines parallel with the equator. A negative latitude
        // will hence fall under the equator. Longitudes are perpendicular to the equator. A negative longitude will occur between
        // Grenich in the UK (GMD) and the international Date Line which is in the middle of the Pacific Ocean.
        // Australian latitudes are thus negative and longitudes are positive. In the EXIF data, a latitude is stored in a latitude array,
        // and it's location relative to grenich is stored in a separate field - in our case "GPSLatitudeRef". A ref of "N" will indicate +'ive.
        // A ref of "S" will indicate that the Latitude should be -'ive. The Longitude will have an "GPSLongitudeRef" of "E" for +'ive, "W" for -'ive.
        private double? ConvertDegreeAngleToDouble(double[] coordinates, string orientation)
        {
            if (coordinates == null) return null;

            double degrees = coordinates[0], minutes = coordinates[1], seconds = coordinates[2];

            var negative = orientation.ToLower().Equals("w") || orientation.ToLower().Equals("s");

            if (negative)
            {
                // turn it positive, apply arithmetic, then return as negative again
                return (-1 * degrees) - (minutes / 60) - (seconds / 3600);
            }

            return degrees + (minutes / 60) + (seconds / 3600);
        }

        // DateTime is stored as a string - "yyyy:MM:dd hh:mm:ss" in 24 hour format
        private DateTime ConvertDateTime(string dateTimeExif)
        {
            var dateTimeStringComponents = dateTimeExif.Split(new[] { ':', ' ' });

            if (dateTimeExif != string.Empty && dateTimeStringComponents.Count() == 6)
            {
                var dateTimeIntComponents = new int[dateTimeStringComponents.Count()];

                for (var i = 0; i < dateTimeStringComponents.Length; i++)
                {
                    int convertedSegment;
                    if (Int32.TryParse(dateTimeStringComponents[i], out convertedSegment))
                    {
                        dateTimeIntComponents[i] = convertedSegment;
                    }
                }

                return new DateTime(
                    dateTimeIntComponents[0], // year
                    dateTimeIntComponents[1], // month
                    dateTimeIntComponents[2], // day
                    dateTimeIntComponents[3], // hour
                    dateTimeIntComponents[4], // minute
                    dateTimeIntComponents[5] // second
                    );
            }

            return DateTime.UtcNow;
        }

        #endregion
    }
}
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
using System.IO;
using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;
using NLog;
using Raven.Client;

namespace Bowerbird.Web.Services
{
    public class DocumentService : IDocumentService
    {
        #region Fields

        private Logger _logger = LogManager.GetLogger("DocumentService");

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public DocumentService(
            IUserContext userContext,
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");

            _userContext = userContext;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public bool Save(MediaResourceCreateCommand command, MediaResource mediaResource, out string failureReason)
        {
            if (!_documentSession.Load<AppRoot>(Constants.AppRootId).DocumentServiceStatus)
            {
                failureReason = "Word documents and PDF files cannot be uploaded at the moment. Please try again later.";
                return false;
            }

            try
            {
                var extension = Path.GetExtension(command.OriginalFileName) ?? string.Empty;

                var acceptedFileTypes = new List<string>()
                                      {
                                          "doc",
                                          "docx",
                                          "pdf"
                                      };

                if (acceptedFileTypes.Any(x => x.Contains(extension)))
                {
                    MakeDocumentMediaResourceFiles(mediaResource, command);
                }
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Error saving document", exception);

                failureReason = "The file is corrupted or not a valid Word or PDF document and could not be saved. Please check the file and try again.";
                return false;
            }

            failureReason = string.Empty;
            return true;
        }

        private void MakeDocumentMediaResourceFiles(MediaResource mediaResource, MediaResourceCreateCommand command)
        {
            mediaResource.AddDocumentFile("Document", command.Key, _userContext.GetUserFullName(), Path.GetExtension(command.OriginalFileName).ToLower());
        }

        #endregion
    }
}
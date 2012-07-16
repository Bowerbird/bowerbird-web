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
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Services;
using Bowerbird.Core.Utilities;
using Newtonsoft.Json;

namespace Bowerbird.Web.Services
{
    public class DocumentService : IDocumentService
    {
        #region Fields

        private List<string> _acceptedFileTypes;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public DocumentService(
            IUserContext userContext
            )
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
                MakeDocumentMediaResourceFiles(mediaResource, command);
            }
        }

        private void MakeDocumentMediaResourceFiles(MediaResource mediaResource, MediaResourceCreateCommand command)
        {
            mediaResource.AddDocumentFile("Document", command.Key, _userContext.GetUserFullName(), "document", Path.GetExtension(command.OriginalFileName));
        }

        private void InitMembers()
        {
            _acceptedFileTypes = new List<string>()
                                      {
                                          ".doc",
                                          ".docx",
                                          ".txt",
                                          ".pdf"
                                      };
        }

        #endregion
    }
}
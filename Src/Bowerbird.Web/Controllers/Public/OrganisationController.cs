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
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Paging;
using Bowerbird.Core.Services;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Public
{
    public class OrganisationController : ControllerBase
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathService _mediaFilePathService;
        private readonly IConfigService _configService;

        #endregion

        #region Constructors

        public OrganisationController(
            IDocumentSession documentSession,
            IMediaFilePathService mediaFilePathService,
            IConfigService configService)
            :base()
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");
            Check.RequireNotNull(configService, "configService");

            _documentSession = documentSession;
            _mediaFilePathService = mediaFilePathService;
            _configService = configService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(MakeOrganisationIndex(idInput));
            }

            return View(MakeOrganisationIndex(idInput));
        }

        [HttpGet]
        public ActionResult List(OrganisationListInput organisationListInput)
        {
            return Json(MakeOrganisationList(organisationListInput), JsonRequestBehavior.AllowGet);
        }

        private OrganisationIndex MakeOrganisationIndex(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var organisation = _documentSession.Load<Organisation>(idInput.Id);

            return new OrganisationIndex()
            {
                Name = organisation.Name,
                Description = organisation.Description,
                Website = organisation.Website,
                Avatar = GetAvatar(organisation)
            };
        }

        private OrganisationList MakeOrganisationList(OrganisationListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Organisation>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList() // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds
                .Select(x => new OrganisationView()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Name = x.Name,
                    Website = x.Website,
                    Avatar = GetAvatar(x)
                });

            return new OrganisationList()
            {
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Organisations = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private Avatar GetAvatar(Organisation organisation)
        {
            return new Avatar()
            {
                AltTag = organisation.Description,
                UrlToImage = organisation.Avatar != null ?
                    _mediaFilePathService.MakeMediaFileUri(organisation.Avatar.Id, "image", "avatar", organisation.Avatar.Metadata["metatype"]) :
                    _configService.GetDefaultAvatar("organisation")
            };
        }

        #endregion
    }
}
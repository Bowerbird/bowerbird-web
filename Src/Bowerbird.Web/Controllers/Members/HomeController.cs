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
using System.Linq;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Extensions;

namespace Bowerbird.Web.Controllers.Members
{
    public class HomeController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public HomeController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(HomeIndexInput homeIndexInput)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(MakeHomeIndex(homeIndexInput));
            }
            return View(MakeHomeIndex(homeIndexInput));
        }

        private HomeIndex MakeHomeIndex(HomeIndexInput indexInput)
        {
            var homeIndex = new HomeIndex();

            throw new NotImplementedException();

            //var projectMenu = _documentSession
            //    .Advanced
            //    .LuceneQuery<ProjectMember>("ProjectMember/ByUserId")
            //    .WhereEquals("Id", indexInput.UserId)
            //    .WaitForNonStaleResults()
            //    .Select(
            //        x => 
            //            new MenuItem()
            //                {
            //                    Id = x.Project.Id, 
            //                    Name = x.Project.Name
            //                })
            //    .ToList();

            //var teamMenu = _documentSession
            //    .Advanced
            //    .LuceneQuery<TeamMember>("TeamMember/ByUserId")
            //    .WhereEquals("Id", indexInput.UserId)
            //    .WaitForNonStaleResults()
            //    .Select(
            //        x => 
            //            new MenuItem()
            //                {
            //                    Id = x.Team.Id, 
            //                    Name = x.Team.Name
            //                })
            //    .ToList();

            //var streamItems = _documentSession
            //    .Advanced
            //    .LuceneQuery<StreamItem>("StreamItem/ByParentId")
            //    .WhereContains(
            //        "Id", 
            //        new List<string>()
            //            .AddRangeFromList(projectMenu.Select(x => x.Id).ToList())
            //            .AddRangeFromList(teamMenu.Select(x => x.Id).ToList()))
            //    .WaitForNonStaleResults()
            //    .Select(
            //        x =>
            //        new StreamItemViewModel()
            //            {
            //                Item = x.Item,
            //                ItemId = x.ItemId,
            //                ParentId = x.ParentId,
            //                SubmittedOn = x.CreatedDateTime,
            //                Type = x.Type
            //            })
            //    .ToList();
            
            //var userProfile = _documentSession.Query<User>()
            //    .Where(u => u.Id == indexInput.UserId)
            //    .Select(u => new UserProfile() {Id = u.Id, Name = u.FirstName + " " + u.LastName});

            //return homeIndex;
        }

        #endregion      
    }
}
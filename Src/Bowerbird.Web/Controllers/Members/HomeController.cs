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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bowerbird.Core;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Commands;
using Bowerbird.Web.ViewModelFactories;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Members;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Members
{
    public class HomeController : ControllerBase
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public HomeController(
            ICommandProcessor commandProcessor,
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(documentSession, "documentSession");

            _commandProcessor = commandProcessor;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public ActionResult Index(HomeIndexInput homeIndexInput)
        {
            // HACK
            homeIndexInput.UserId = "frankr";
            homeIndexInput.Page = 1;
            homeIndexInput.PageSize = 10;

            return View(MakeHomeIndex(homeIndexInput));
        }

        [Transaction]
        public ActionResult GenerateData()
        {
            // Create basic data for system functionality
            var setupSystemCommand = new SetupSystemCommand();
            _commandProcessor.Process(setupSystemCommand);

            _commandProcessor.Process(new ObservationCreateCommand()
            {
                Title = "test",
                Address = "aaa",
                UserId = "frankr",
                ObservedOn = DateTime.Now,
                ObservationCategory = "sdsd",
                MediaResources = new List<string>()
            });

            _commandProcessor.Process(new ObservationCreateCommand()
            {
                Title = "test",
                Address = "aaa",
                UserId = "frankr",
                ObservedOn = DateTime.Now,
                ObservationCategory = "sdsd",
                MediaResources = new List<string>()
            });

            _commandProcessor.Process(new ObservationCreateCommand()
            {
                Title = "test",
                Address = "aaa",
                UserId = "frankr",
                ObservedOn = DateTime.Now,
                ObservationCategory = "sdsd",
                MediaResources = new List<string>()
            });

            return RedirectToAction("index");
        }

        private HomeIndex MakeHomeIndex(HomeIndexInput homeIndexInput)
        {
            RavenQueryStatistics stats;

            //int requestedPageSize = pageSize < 1 || pageSize > 30 ? 10 : pageSize;
            //int requestedPage = page < 1 ? 1 : page;

            var streamItems = new List<StreamItem>();

            // Observations
            streamItems.AddRange(_documentSession
                .Query<Observation>()
                .Statistics(out stats)
                //.Where(x => x.Teams.In(subscription.Teams) || x.Projects.In(subscription.Projects) || x.User.Id == homeIndexInput.Username)
                .Where(x => x.User.Id == homeIndexInput.UserId)
                .OrderByDescending(x => x.SubmittedOn)
                .Skip(homeIndexInput.Page)
                .Take(homeIndexInput.PageSize)
                .Select(x => new StreamItem() { Type = "observation", SubmittedOn = x.SubmittedOn, Item = x }) // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds
                .ToList());

            // Posts
            streamItems.AddRange(_documentSession
                .Query<Post>()
                .OrderByDescending(x => x.PostedOn)
                .Skip(homeIndexInput.Page)
                .Take(homeIndexInput.PageSize)
                .Select(x => new StreamItem() { Type = "post", SubmittedOn = x.PostedOn, Item = x }) // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds
                .ToList());

            // Get number required based on page size
            streamItems = streamItems
                .OrderByDescending(x => x.SubmittedOn)
                .Take(homeIndexInput.PageSize)
                .ToList();

            return new HomeIndex()
            {
                //StreamItems = _pagedListFactory.Make(homeIndexInput.Page, homeIndexInput.PageSize, stats.TotalResults, streamItems, null)
            };
        }

        #endregion      

    }
}

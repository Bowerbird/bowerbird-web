///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
 
// Team Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
 
// Funded by:
// * Atlas of Living Australia
 
//*/

//namespace Bowerbird.Web.Controllers
//{
//    #region Namespaces

//    using System;
//    using System.Web.Mvc;

//    using Bowerbird.Core;
//    using Bowerbird.Core.Commands;
//    using Bowerbird.Core.DesignByContract;
//    using Bowerbird.Web.ViewModels;
//    using Bowerbird.Web.Config;

//    #endregion

//    public class TeamController : Controller
//    {
//        #region Members

//        private readonly ICommandProcessor _commandProcessor;
//        private readonly IViewModelRepository _viewModelRepository;
//        private readonly IUserTasks _userTasks;
//        private readonly IUserContext _userContext;

//        #endregion

//        #region Constructors

//        public TeamController(
//            ICommandProcessor commandProcessor,
//            IViewModelRepository viewModelRepository,
//            IUserTasks userTasks,
//            IUserContext userContext)
//        {
//            Check.RequireNotNull(commandProcessor, "commandProcessor");
//            Check.RequireNotNull(viewModelRepository, "viewModelRepository");
//            Check.RequireNotNull(userTasks, "userTasks");
//            Check.RequireNotNull(userContext, "userContext");

//            _commandProcessor = commandProcessor;
//            _viewModelRepository = viewModelRepository;
//            _userTasks = userTasks;
//            _userContext = userContext;
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        //[HttpGet]
//        //public ActionResult List(int? id, int? page, int? pageSize)
//        //{
//        //    return Json("success", JsonRequestBehavior.AllowGet);
//        //}

//        //[HttpGet]
//        //public ActionResult Index(TeamIndexInput indexInput)
//        //{
//        //    return View(_viewModelRepository.Load<TeamIndexInput, TeamIndex>(indexInput));
//        //}

//        [Transaction]
//        [Authorize]
//        [HttpPost]
//        public ActionResult CreateProject(ProjectCreateInput projectCreateInput, TeamProjectCreateInput teamProjectCreateInput)
//        {
//            _commandProcessor.Process(MakeTeamProjectCreateCommand(projectCreateInput, teamProjectCreateInput));

//            return Json("success");
//        }

//        //[Transaction]
//        //[HttpPut]
//        //public ActionResult Update(TeamUpdateInput updateInput)
//        //{
//        //    _commandProcessor.Process(MakeUpdateCommand(updateInput));

//        //    return Json("Success");
//        //}

//        //[Transaction]
//        //[HttpDelete]
//        //public ActionResult Delete(TeamDeleteInput deleteInput)
//        //{
//        //    _commandProcessor.Process(MakeDeleteCommand(deleteInput));

//        //    return Json("success");
//        //}

//        //private TeamCreateCommand MakeCreateCommand(TeamCreateInput createInput)
//        //{
//        //    Check.RequireNotNull(createInput, "createInput");

//        //    return new TeamCreateCommand()
//        //    {
//        //        Description = createInput.Description,
//        //        Name = createInput.Name,
//        //        UserId = _userContext.GetAuthenticatedUserId()
//        //    };
//        //}

//        //private TeamDeleteCommand MakeDeleteCommand(TeamDeleteInput deleteInput)
//        //{
//        //    Check.RequireNotNull(deleteInput, "deleteInput");

//        //    return new TeamDeleteCommand()
//        //    {
//        //        Id = deleteInput.TeamId,
//        //        UserId = _userContext.GetAuthenticatedUserId()
//        //    };
//        //}

//        //private TeamUpdateCommand MakeUpdateCommand(TeamUpdateInput updateInput)
//        //{
//        //    return new TeamUpdateCommand()
//        //    {
//        //        Description = updateInput.Description,
//        //        Name = updateInput.Name,
//        //        UserId = _userContext.GetAuthenticatedUserId()
//        //    };
//        //}

//        private TeamProjectCreateCommand MakeTeamProjectCreateCommand(ProjectCreateInput projectCreateInput, TeamProjectCreateInput teamProjectCreateInput)
//        {
//            return new TeamProjectCreateCommand()
//            {
//                UserId = projectCreateInput.UserId,
//                Name = projectCreateInput.Name,
//                Description = projectCreateInput.Description,
//                Administrators = teamProjectCreateInput.Administrators,
//                Members = teamProjectCreateInput.Members,
//                TeamId = teamProjectCreateInput.ProjectTeamId
//            };
//        }

//        #endregion
//    }
//}
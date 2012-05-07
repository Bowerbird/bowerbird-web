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
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;

namespace Bowerbird.Web.Controllers
{
    public class CommentController : ControllerBase
    {
        #region Fields

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public CommentController(
            ICommandProcessor commandProcessor,
            IUserContext userContext
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(CommentCreateInput createInput)
        {
            if (createInput.ContributionId.Contains("observations/") && !_userContext.HasGroupPermission<Observation>(createInput.ContributionId, PermissionNames.CreateComment))
            {
                return HttpUnauthorized();
            }

            if (createInput.ContributionId.Contains("posts/") && !_userContext.HasGroupPermission<Post>(createInput.ContributionId, PermissionNames.CreateComment))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new CommentCreateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Comment = createInput.Comment,
                    CommentedOn = DateTime.UtcNow,
                    ContributionId = createInput.ContributionId
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpPut]
        public ActionResult Update(CommentUpdateInput updateInput)
        {
            if (updateInput.ContributionId.Contains("observations/") && !_userContext.HasGroupPermission<Observation>(updateInput.ContributionId, PermissionNames.UpdateComment))
            {
                return HttpUnauthorized();
            }

            if (updateInput.ContributionId.Contains("posts/") && !_userContext.HasGroupPermission<Post>(updateInput.ContributionId, PermissionNames.UpdateComment))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new CommentUpdateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Id = updateInput.CommentId,
                    Comment = updateInput.Comment,
                    ContributionId = updateInput.ContributionId,
                    UpdatedOn = DateTime.UtcNow
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpDelete]
        public ActionResult Delete(CommentDeleteInput deleteInput)
        {
            if (deleteInput.ContributionId.Contains("observations/") && !_userContext.HasGroupPermission<Observation>(deleteInput.ContributionId, PermissionNames.DeleteComment))
            {
                return HttpUnauthorized();
            }

            if (deleteInput.ContributionId.Contains("posts/") && !_userContext.HasGroupPermission<Post>(deleteInput.ContributionId, PermissionNames.DeleteComment))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new CommentDeleteCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Id = deleteInput.CommentId,
                    ContributionId = deleteInput.ContributionId
                });

            return JsonSuccess();
        }

        #endregion
    }
}
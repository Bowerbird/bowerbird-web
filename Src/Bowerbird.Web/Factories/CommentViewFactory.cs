/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Web.Factories
{
    public class CommentViewFactory : ICommentViewFactory
    {
        #region Fields

        private readonly IUserViewFactory _userViewFactory;

        #endregion

        #region Constructors

        public CommentViewFactory(
            IUserViewFactory userViewFactory
            )
        {
            Check.RequireNotNull(userViewFactory, "userViewFactory");

            _userViewFactory = userViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(Comment comment)
        {
            return new
            {
                comment.Id,
                comment.CommentedOn,
                comment.Message,
                Creator = _userViewFactory.Make(comment.User.Id)
            };
        }

        #endregion
    }
}
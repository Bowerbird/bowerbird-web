namespace Bowerbird.Core.Commands
{
    public class VoteUpdateCommand : ICommand
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string UserId { get; set; }

        public string ContributionId { get; set; }

        public string SubContributionId { get; set; }

        public int Score { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}
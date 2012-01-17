using System.Collections.Generic;

namespace Bowerbird.Web.ViewModels
{
    public class TeamProjectCreateInput
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// The Id of the Team the project is being added to
        /// </summary>
        public string ProjectTeamId { get; set; }

        public List<string> Administrators { get; set; }

        public List<string> Members { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}

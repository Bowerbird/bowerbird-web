using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Bowerbird.Web.ViewModels.Members
{
    public class TeamPostCreateInput : IViewModel
    {
        #region Members

        #endregion

        #region Constructors

        public TeamPostCreateInput()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        [Required]
        public string TeamId { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public IList<string> MediaResources { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        #endregion

        #region Methods

        public void InitMembers()
        {
            MediaResources = new List<string>();
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Bowerbird.Web.ViewModels.Members
{
    public class WatchlistCreateInput : IViewModel
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Required]
        public string Name { get; set; }

        [Required]
        public string JsonQuerystring { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}
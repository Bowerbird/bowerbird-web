using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Entities;

namespace Bowerbird.Web.ViewModels
{
    public class HomeIndex : IViewModel
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public PagedList<Observation> Observations { get; set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}

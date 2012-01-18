using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.ViewModels.Shared;

namespace Bowerbird.Web.ViewModels
{
    public class HomeIndex : IViewModel
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public PagedList<StreamItem> StreamItems { get; set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}

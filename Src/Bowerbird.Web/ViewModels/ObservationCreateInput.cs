using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Commands;

namespace Bowerbird.Web.ViewModels
{
    public class ObservationCreateInput : IViewModel
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Address { get; set; }

        public string Description { get; set; }
        
        public float Latitude { get; set; }
        
        public float Longitude { get; set; }
        
        public string Title { get; set; }
        
        public string Username { get; set; }
        
        public List<string> MediaResources { get; set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}

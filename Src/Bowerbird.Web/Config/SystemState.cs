using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Config;
using Raven.Client;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Config
{
    public class SystemState
    {

        #region Fields

        #endregion

        #region Constructors

        public SystemState()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string Id { get; set; }

        public DateTime? SystemDataSetupDate { get; set; }

        public bool FireEvents { get; set; }

        public bool SendEmails { get; set; }

        public bool ExecuteCommands { get; set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Id = "settings/systemstate";
        }

        #endregion   
      
    }
}
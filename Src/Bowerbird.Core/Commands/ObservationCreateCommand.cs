using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Core.Commands
{
    public class ObservationCreateCommand : CommandBase
    {

        #region Members

        #endregion

        #region Constructors

        public ObservationCreateCommand()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string Id { get; set; }

        public string Title { get; set; }

        public DateTime ObservedOn { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Address { get; set; }

        public bool IsIdentificationRequired { get; set; }

        public string ObservationCategory { get; set; }

        public virtual List<string> MediaResources { get; set; }

        public virtual string UserId { get; set; }

        #endregion

        #region Methods

        public override ICollection<ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }

        private void InitMembers()
        {
            MediaResources = new List<string>();
        }

        #endregion

    }
}

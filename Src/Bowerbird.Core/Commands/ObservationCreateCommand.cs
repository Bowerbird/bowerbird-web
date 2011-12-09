using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Core.Commands
{
    public class ObservationCreateCommand : CommandBase
    {

        #region Members

        #endregion

        #region Constructors

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

        public List<string> MediaResources { get; set; }

        public string Username { get; set; }

        #endregion

        #region Methods

        public override ICollection<ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}

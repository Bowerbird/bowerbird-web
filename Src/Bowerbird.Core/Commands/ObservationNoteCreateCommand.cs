/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Commands
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    #endregion

    public class ObservationNoteCreateCommand : CommandBase
    {
        #region Members

        #endregion

        #region Constructors

        public ObservationNoteCreateCommand()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string UserId { get; set; }

        public string ObservationId { get; set; }

        public DateTime SubmittedOn { get; set; }

        public string ScientificName { get; set; }

        public string CommonName { get; set; }

        public string Taxonomy { get; set; }

        public string Tags { get; set; }

        public Dictionary<string, string> Descriptions { get; set; }

        public Dictionary<string, string> References { get; set; }

        public string Notes { get; set; }

        #endregion

        #region Methods

        public override ICollection<ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }

        private void InitMembers()
        {
            Descriptions = new Dictionary<string, string>();

            References = new Dictionary<string, string>();
        }

        #endregion

    }
}
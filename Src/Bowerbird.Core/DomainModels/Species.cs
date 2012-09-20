/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Collections.Generic;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class Species : DomainModel
    {
        #region Fields

        #endregion

        #region Constructors

        protected Species()
            : base()
        {
        }

        public Species(
            string category,
            string commonGroupName,
            IEnumerable<string> commonNames,
            string kingdomName,
            string phylumName,
            string className,
            string orderName,
            string familyName,
            string genusName,
            string speciesName,
            DateTime createdOn,
            User createdByUser)
            : base()
        {
            CreatedDateTime = createdOn;
            CreatedByUser = createdByUser;

            SetSpeciesDetails(
                category,
                commonGroupName,
                commonNames,
                kingdomName,
                phylumName,
                className,
                orderName,
                familyName,
                genusName,
                speciesName);

            ApplyEvent(new DomainModelCreatedEvent<Species>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public string Category { get; private set; }

        public string CommonGroupName { get; private set; }

        public IEnumerable<string> CommonNames { get; private set; }

        public string KingdomName { get; private set; }

        public string PhylumName { get; private set; }

        public string ClassName { get; private set; }

        public string OrderName { get; private set; }

        public string FamilyName { get; private set; }

        public string GenusName { get; private set; }

        public string SpeciesName { get; private set; }

        public DenormalisedUserReference CreatedByUser { get; private set; }

        public DateTime CreatedDateTime { get; private set; }

        #endregion

        #region Methods

        public void SetSpeciesDetails(
            string category,
            string commonGroupName,
            IEnumerable<string> commonNames,
            string kingdomName,
            string phylumName,
            string className,
            string orderName,
            string familyName,
            string genusName,
            string speciesName
            )
        {
            Category = category;
            CommonGroupName = commonGroupName;
            CommonNames = commonNames;
            KingdomName = kingdomName;
            PhylumName = phylumName;
            ClassName = className;
            OrderName = orderName;
            FamilyName = familyName;
            GenusName = genusName;
            SpeciesName = speciesName;
        }

        public void UpdateDetails(
            string category,
            string commonGroupName,
            IEnumerable<string> commonNames,
            string kingdomName,
            string phylumName,
            string className,
            string orderName,
            string familyName,
            string genusName,
            string speciesName,
            User updatedByUser
            )
        {
            SetSpeciesDetails(
                category,
                commonGroupName,
                commonNames,
                kingdomName,
                phylumName,
                className,
                orderName,
                familyName,
                genusName,
                speciesName);

            ApplyEvent(new DomainModelUpdatedEvent<Species>(this, updatedByUser, this));
        }

        #endregion
    }
}
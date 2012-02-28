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

using Bowerbird.Core.DesignByContract;
using System;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class ObservationMedia : DomainModel
    {

        #region Members

        #endregion

        #region Constructors

        protected ObservationMedia() : base() { }

        public ObservationMedia(
            MediaResource mediaResource,
            string description)
            : this()
        {
            Check.RequireNotNull(mediaResource, "mediaResource");

            MediaResource = mediaResource;
            Description = description;
        }

        #endregion

        #region Properties

        public MediaResource MediaResource { get; private set; }

        public string Description { get; private set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
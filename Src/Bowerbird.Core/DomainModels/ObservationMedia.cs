/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.DomainModels
{
    public class ObservationMedia : DomainModel
    {
        #region Members

        #endregion

        #region Constructors

        protected ObservationMedia() : base() { }

        public ObservationMedia(
            string id,
            MediaResource mediaResource,
            string description,
            string licence)
            : this()
        {
            Check.RequireNotNull(mediaResource, "mediaResource");

            Id = id;
            MediaResource = mediaResource;

            SetDetails(
                description,
                licence);
        }

        #endregion

        #region Properties

        public MediaResource MediaResource { get; private set; }

        public string Description { get; private set; }

        public string Licence { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(string description, string licence)
        {
            Description = description;
            Licence = licence;
        }

        public ObservationMedia UpdateDetails(string description, string licence)
        {
            SetDetails(
                description,
                licence);

            return this;
        }

        #endregion      
    }
}
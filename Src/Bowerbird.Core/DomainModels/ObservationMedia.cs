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
    public class ObservationMedia
    {
        #region Members

        #endregion

        #region Constructors

        protected ObservationMedia()
            : base()
        {
        }

        public ObservationMedia(
            MediaResource mediaResource,
            string description,
            string licence,
            bool isPrimaryMedia)
            : base()
        {
            Check.RequireNotNull(mediaResource, "mediaResource");

            MediaResource = mediaResource;

            SetDetails(
                description,
                licence,
                isPrimaryMedia);
        }

        #endregion

        #region Properties

        public MediaResource MediaResource { get; private set; }

        public string Description { get; private set; }

        public string Licence { get; private set; }

        public bool IsPrimaryMedia { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(string description, string licence, bool isPrimaryMedia)
        {
            Description = description;
            Licence = licence;
            IsPrimaryMedia = isPrimaryMedia;
        }

        public ObservationMedia UpdateDetails(string description, string licence, bool isPrimaryMedia)
        {
            SetDetails(
                description,
                licence,
                isPrimaryMedia);

            return this;
        }

        #endregion      
    }
}
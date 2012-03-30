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

namespace Bowerbird.Core.DomainModels.DenormalisedReferences
{
    public interface INamedDomainModel
    {
        string Id { get; }

        string Name { get; }
    }

    public class DenormalisedNamedDomainModelReference<T> : ValueObject where T : INamedDomainModel
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; private set; }

        public string Name { get; private set; }

        #endregion

        #region Methods

        public static implicit operator DenormalisedNamedDomainModelReference<T>(T namedDomainModel)
        {
            Check.RequireNotNull(namedDomainModel, "namedDomainModel");

            return new DenormalisedNamedDomainModelReference<T>
            {
                Id = namedDomainModel.Id, 
                Name = namedDomainModel.Name
            };
        }

        #endregion
    }
}
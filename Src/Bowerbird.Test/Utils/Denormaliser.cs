/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Test.Utils
{
    public static class Denormaliser
    {
        public static DenormalisedNamedDomainModelReference<T> DenormalisedNamedDomainModelReference<T>(this T t) where T : INamedDomainModel
        {
            DenormalisedNamedDomainModelReference<T> denormalisedReference = t;

            return denormalisedReference;
        }

        public static DenormalisedMemberReference DenormalisedMemberReference(this Member member)
        {
            DenormalisedMemberReference denormalisedReference = member;

            return denormalisedReference;
        }

        public static DenormalisedObservationReference DenormalisedObservationReference(this Observation obseravtion)
        {
            DenormalisedObservationReference denormalisedReference = obseravtion;

            return denormalisedReference;
        }

        public static DenormalisedUserReference DenormalisedUserReference(this User user)
        {
            DenormalisedUserReference denormalisedReference = user;

            return denormalisedReference;
        }
    }
}
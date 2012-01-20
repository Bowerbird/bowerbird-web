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

using System;
using Ninject;
using FluentValidation;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;
using Ninject.Planning.Bindings;

namespace Bowerbird.Web.Config
{
    public class NinjectValidatorFactory : ValidatorFactoryBase
    {

        #region Members

        private IKernel _kernel;

        #endregion

        #region Constructors

        public NinjectValidatorFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public override IValidator CreateInstance(Type validatorType)
        {
            if (((IList<IBinding>)_kernel.GetBindings(validatorType)).Count == 0)
            {
                return null;
            }

            return _kernel.Get(validatorType) as IValidator;
        }

        #endregion      

    }
}

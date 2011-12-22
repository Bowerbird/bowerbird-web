using System;
using Microsoft.Practices.ServiceLocation;
using Bowerbird.Web.ViewModelFactories;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web
{
    public class ViewModelRepository : IViewModelRepository
    {

        #region Members

        private readonly IServiceLocator _serviceLocator;

        #endregion

        #region Constructors

        public ViewModelRepository(
            IServiceLocator serviceLocator)
        {
            Check.RequireNotNull(serviceLocator, "serviceLocator");

            _serviceLocator = serviceLocator;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public TViewModel Load<TInput, TViewModel>(TInput input)
        {
            Check.RequireNotNull(input, "input");

            var viewModelFactory = _serviceLocator.GetInstance<IViewModelFactory<TInput, TViewModel>>();

            if (viewModelFactory == null)
            {
                throw new Exception("A viewmodel factory for the specified view model type does not exist.");
            }

            return viewModelFactory.Make(input);
        }

        public TViewModel Load<TViewModel>()
        {
            var viewModelFactory = _serviceLocator.GetInstance<IViewModelFactory<TViewModel>>();

            if (viewModelFactory == null)
            {
                throw new Exception("A viewmodel factory for the specified view model type does not exist.");
            }

            return viewModelFactory.Make();
        }

        #endregion      
      
    }
}
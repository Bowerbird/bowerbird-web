using System;
namespace Bowerbird.Web
{
    public interface IViewModelRepository
    {
        TViewModel Load<TInput, TViewModel>(TInput input);

        TViewModel Load<TViewModel>();
    }
}

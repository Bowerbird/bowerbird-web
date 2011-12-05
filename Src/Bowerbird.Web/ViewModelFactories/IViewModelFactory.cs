using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bowerbird.Web.ViewModelFactories
{
    public interface IViewModelFactory<TInput, TViewModel>
    {
        TViewModel Make(TInput input);
    }

    public interface IViewModelFactory<TViewModel>
    {
        TViewModel Make();
    }
}

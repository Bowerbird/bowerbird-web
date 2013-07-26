using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.ViewModelFactories
{
    public interface IMediaResourceViewFactory
    {
        object Make(MediaResource mediaResource, bool includeExifData = false);
    }
}
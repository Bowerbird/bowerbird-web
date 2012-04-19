using Bowerbird.Web.ViewModels;

namespace Bowerbird.Web.Queries
{
    public interface IPostsQuery
    {
        PostList MakePostList(PostListInput listInput);
    }
}
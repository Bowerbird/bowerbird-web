using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using System.Dynamic;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.ViewModelFactories
{
    public class GroupViewFactory : IGroupViewFactory
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(Group group, User authenticatedUser, bool fullDetails = false, int sightingCount = 0, int userCount = 0, int postCount = 0, IEnumerable<Observation> sampleObservations = null)
        {
            Check.RequireNotNull(group, "group");

            dynamic viewModel = new ExpandoObject();

            viewModel.Id = group.Id;
            viewModel.Name = group.Name;
            viewModel.GroupType = group.GroupType;

            if (group is IPublicGroup)
            {
                viewModel.Avatar = ((IPublicGroup)group).Avatar;
                viewModel.CreatedBy = group.User.Id;
            }

            if (fullDetails)
            {
                viewModel.Created = group.CreatedDateTime.ToString("d MMM yyyy");
                viewModel.CreatedDateTimeOrder = group.CreatedDateTime.ToString("yyyyMMddHHmmss");

                if (group is IPublicGroup)
                {
                    viewModel.Background = ((IPublicGroup)group).Background;
                    viewModel.Website = ((IPublicGroup)group).Website;
                    viewModel.Description = ((IPublicGroup)group).Description;
                    viewModel.UserCount = userCount;
                    viewModel.PostCount = postCount;
                }
                if (group is Project)
                {
                    viewModel.SightingCount = sightingCount;
                    viewModel.Categories = ((Project)group).Categories;
                    if (sampleObservations != null)
                    {
                        viewModel.SampleObservations = sampleObservations.Select(x =>
                                                                        new
                                                                            {
                                                                                x.Id,
                                                                                Media = x.PrimaryMedia
                                                                            });
                    }
                    else
                    {
                        viewModel.SampleObservations = new object[] {};
                    }
                }
                if (group is Organisation)
                {
                    viewModel.Categories = ((Organisation)group).Categories;
                }

                if (authenticatedUser != null)
                {
                    viewModel.IsMember = authenticatedUser.Memberships.Any(x => x.Group.Id == group.Id);
                }
            }

            return viewModel;
        }

        #endregion   
    }
}

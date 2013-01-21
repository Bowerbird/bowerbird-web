using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using System.Dynamic;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Factories
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

        public dynamic Make(Group group)
        {
            Check.RequireNotNull(group, "group");

            return MakeBaseGroup(group);
        }

        public dynamic Make(All_Groups.Result result, bool fullDetails = false)
        {
            Check.RequireNotNull(result, "result");

            dynamic viewModel = MakeBaseGroup(result.Group);

            if (fullDetails)
            {
                viewModel.Created = result.Group.CreatedDateTime.ToString("d MMM yyyy");
                viewModel.CreatedDateTimeOrder = result.Group.CreatedDateTime.ToString("yyyyMMddHHmmss");

                if (result.Group is IPublicGroup)
                {
                    viewModel.Background = ((IPublicGroup)result.Group).Background;
                    viewModel.Website = ((IPublicGroup) result.Group).Website;
                    viewModel.Description = ((IPublicGroup) result.Group).Description;
                    viewModel.MemberCount = result.UserIds.Count();
                    viewModel.PostCount = result.PostCount;
                }
                if (result.Group is Project)
                {
                    viewModel.SightingCount = result.SightingCount;
                }
            }

            return viewModel;
        }

        private dynamic MakeBaseGroup(Group group)
        {
            Check.RequireNotNull(group, "group");

            dynamic viewModel = new ExpandoObject();

            viewModel.Id = group.Id;
            viewModel.Name = group.Name;
            viewModel.GroupType = group.GroupType;

            if (group is IPublicGroup)
            {
                viewModel.Avatar = ((IPublicGroup)group).Avatar;
            }

            return viewModel;
        }

        #endregion   
    }
}

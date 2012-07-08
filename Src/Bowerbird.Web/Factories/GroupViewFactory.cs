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

        public object Make(All_Groups.Result result)
        {
            Check.RequireNotNull(result, "result");

            dynamic view = new ExpandoObject();

            view.Id = result.Group.Id;
            view.Name = result.Group.Name;
            view.GroupType = result.Group.GroupType;
            view.MemberCount = result.UserIds == null ? 0 : result.UserIds.Count(); // Not sure why this can be null??

            if (result.Group is IPublicGroup)
            {
                view.Avatar = ((IPublicGroup)result.Group).Avatar;
            }

            return view;
        }

        public object Make(Group group, int memberCount)
        {
            Check.RequireNotNull(group, "group");

            dynamic view = new ExpandoObject();

            view.Id = group.Id;
            view.Name = group.Name;
            view.GroupType = group.GroupType;
            view.MemberCount = memberCount;

            if (group is IPublicGroup)
            {
                view.Avatar = ((IPublicGroup)group).Avatar;
            }

            return view;
        }

        #endregion   
    }
}

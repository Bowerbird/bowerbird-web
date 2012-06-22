using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using System.Dynamic;

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

        public object Make(Group group)
        {
            dynamic view = new ExpandoObject();

            view.Id = group.Id;
            view.Name = group.Name;
            view.GroupType = group.GroupType;

            if(group is IPublicGroup)
            {
                view.Avatar = ((IPublicGroup)group).Avatar;
            }

            return view;
        }

        #endregion   
    }
}

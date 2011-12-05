using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bowerbird.Core.Entities
{
    public class Activity : Entity
    {

        #region Members

        #endregion

        #region Constructors

        protected Activity()
            : base()
        {
        }

        public Activity(
            string type,
            User user,
            object data)
            : base()
        {
            SetDetails(
                type,
                user,
                data);
        }

        #endregion

        #region Properties

        public DateTime OccurredOn { get; private set; }

        public string Type { get; private set; }

        public User User { get; private set; }

        public object Data { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(string type, User user, object data)
        {
            OccurredOn = DateTime.Now;
            Type = type;
            User = user;
            Data = data;
        }

        #endregion      
      
    }
}

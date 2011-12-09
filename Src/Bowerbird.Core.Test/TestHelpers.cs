using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Bowerbird.Core.Entities;

namespace Bowerbird.Core.Test
{
    public static class TestHelpers
    {
        public static User WaitForASecond(this User testUser)
        {
            Thread.Sleep(1000);

            return testUser;
        }

        public static bool IsMoreRecentThan(this DateTime laterDate, DateTime earlierDate)
        {
            return laterDate > earlierDate;
        }
    }
}
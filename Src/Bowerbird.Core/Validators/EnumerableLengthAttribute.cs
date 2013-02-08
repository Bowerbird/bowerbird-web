using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Bowerbird.Core.Validators
{
    public class EnumerableLengthAttribute : ValidationAttribute
    {
        private int minLength;
        private int? maxLength;          

        public EnumerableLengthAttribute(int min)
        {
            minLength = min;
            maxLength = null;
        }

        public EnumerableLengthAttribute(int min, int max)
        {
            minLength = min;
            maxLength = max;
        }

        public override bool IsValid(object value)
        {
            IEnumerable<object> list = value as IEnumerable<object>;

            return list != null && list.Count() >= minLength && (maxLength == null || list.Count() <= maxLength);
        }
    }
}

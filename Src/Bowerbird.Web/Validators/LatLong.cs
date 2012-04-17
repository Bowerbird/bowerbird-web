//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.ComponentModel.DataAnnotations;
//using Bowerbird.Web.ViewModels;

//namespace Bowerbird.Web.Validators
//{
//    [AttributeUsage(AttributeTargets.Class)]
//    public class ValidLatLongAttribute : ValidationAttribute
//    {

//        public override Boolean IsValid(Object value)
//        {
//            if (value != null && value is ObservationCreateInput)
//            {
//                if (((ObservationCreateInput) value).Latitude <= 0 && ((ObservationCreateInput) value).Longitude > 0)
//                {
                    
//                }
//            }

//            return false;
//        }
//    }
//}

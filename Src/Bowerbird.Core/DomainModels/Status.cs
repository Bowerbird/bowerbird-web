///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
 
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
 
// Funded by:
// * Atlas of Living Australia
 
//*/

//using System;

//namespace Bowerbird.Core.DomainModels
//{
//    public static class Connection
//    {
//        public enum ConnectionStatus
//        {
//            /// <summary>
//            /// Online = 0
//            /// </summary>
//            Online = 0,
//            /// <summary>
//            /// Away = 1
//            /// </summary>
//            Away = 1,
//            /// <summary>
//            /// Offline = 2
//            /// </summary>
//            Offline = 2,
//            /// <summary>
//            /// Abandon = 3 (should be deleted)
//            /// </summary>
//            Abandon = 3
//        }

//        public static ConnectionStatus Status(this int status)
//        {
//            switch (status)
//            {
//                case 0:
//                    return ConnectionStatus.Online;
//                case 1:
//                    return ConnectionStatus.Away;
//                case 2:
//                    return ConnectionStatus.Offline;
//                case 3:
//                    return ConnectionStatus.Abandon;

//                default:
//                    return ConnectionStatus.Abandon;
//            }
//        }

//        public static bool Online(this int status)
//        {
//            return status.Status() == ConnectionStatus.Online;
//        }

//        public static bool Offline(this int status)
//        {
//            return status.Status() == ConnectionStatus.Offline;
//        }

//        public static bool Connected(this int status)
//        {
//            return status.Status() != ConnectionStatus.Offline && status.Status() != ConnectionStatus.Abandon;
//        }

//        public static bool Connected(this string status)
//        {
//            int statusInt;
//            Int32.TryParse(status, out statusInt);

//            return statusInt.Status() != ConnectionStatus.Offline && statusInt.Status() != ConnectionStatus.Abandon;
//        }
//    }
//}
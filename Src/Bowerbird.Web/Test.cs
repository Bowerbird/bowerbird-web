//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Dynamic;
//using Raven.Client;
//using Raven.Client.Document;

//namespace Bowerbird.Web
//{
//    public class Test
//    {
//        //[Test]
//        public void SaveDynamicEntityWithStaticProperties()
//        {
//            string id = null;
//            Thing thing = null;

//            using (var store = new DocumentStore() { Url = "http://localhost:8080" })
//            {
//                using (var session = store.OpenSession())
//                {
//                    dynamic newThing = new Thing("Yo dawg, i'm putting some static in your dynamic");

//                    newThing.Id = "";
//                    newThing.AnotherValue = "Do it!"; // This dynamic property will get serialised

//                    session.Store(newThing);
//                    id = newThing.Id;
//                }

//                using (var session = store.OpenSession())
//                {
//                    thing = session.Load<dynamic>(id);
//                }
//            }

//            Assert(thing.SomeValue != null);
//        }

//        public class Thing : DynamicObject
//        {
//            public Thing(string someValue)
//            {
//                SomeValue = someValue;
//            }

//            private Dictionary<string, object> _properties = new Dictionary<string, object>();

//            // This static property will not be serialised
//            public string SomeValue { get; set; }

//            public override IEnumerable<string> GetDynamicMemberNames()
//            {
//                return _properties.Keys;
//            }

//            public override bool TryGetMember(GetMemberBinder binder, out object result)
//            {
//                return _properties.TryGetValue(binder.Name, out result);
//            }

//            public override bool TrySetMember(SetMemberBinder binder, object value)
//            {
//                _properties[binder.Name] = value;
//                return true;
//            }

//            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
//            {
//                dynamic method = _properties[binder.Name];
//                result = method(args);
//                return true;
//            }
//        }
//    }
//}

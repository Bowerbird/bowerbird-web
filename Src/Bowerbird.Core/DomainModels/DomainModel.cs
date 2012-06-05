/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Newtonsoft.Json;

namespace Bowerbird.Core.DomainModels
{
    /// <summary>
    ///     For a discussion of this object, see 
    ///     http://devlicio.us/blogs/billy_mccafferty/archive/2007/04/25/using-equals-gethashcode-effectively.aspx
    /// </summary>
    //[Serializable]
    public abstract class DomainModel : BaseObject, IAssignableId
    {
        /// <summary>
        ///     To help ensure hashcode uniqueness, a carefully selected random number multiplier 
        ///     is used within the calculation.  Goodrich and Tamassia's Data Structures and
        ///     Algorithms in Java asserts that 31, 33, 37, 39 and 41 will produce the fewest number
        ///     of collissions.  See http://computinglife.wordpress.com/2008/11/20/why-do-hash-functions-use-prime-numbers/
        ///     for more information.
        /// </summary>
        private const int HashMultiplier = 31;

        private int? cachedHashcode;

        private string _id;

        /// <summary>
        ///     Id may be of type string, int, custom type, etc.
        ///     Setter is protected to allow unit tests to set this property via reflection and to allow 
        ///     domain objects more flexibility in setting this for those objects with assigned Ids.
        ///     It's virtual to allow NHibernate-backed objects to be lazily loaded.
        /// </summary>
        public string Id 
        {
            get
            {
                return _id;
            }
            protected set
            {
                _id = value;
            }
        }

        /// <summary>
        /// Attempts to return the "short" ID. Eg: Id = "abc/123" returns "123"
        /// </summary>
        public string ShortId()
        {
            if (string.IsNullOrWhiteSpace(_id))
            {
                return string.Empty;
            }

            if (_id.Contains('/'))
            {
                return _id.Split('/')[1];
            }

            return _id;
        }

        /// <summary>
        /// Used to determine if events will be fired. Default is set to false
        /// </summary>
        [JsonIgnore]
        private bool CanFireEvents { get; set; }

        protected void EnableEvents()
        {
            CanFireEvents = true;
        }

        /// <summary>
        /// Fires the specified event if CanFireEvents is true. Pass true for second param to enable events.
        /// </summary>
        protected void FireEvent<T>(T domainEvent, bool enableEvents = false) where T : IDomainEvent
        {
            CanFireEvents = enableEvents;

            if (CanFireEvents)
            {
                EventProcessor.Raise(domainEvent);
            }
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as DomainModel;

            if (ReferenceEquals(this, compareTo))
            {
                return true;
            }

            if (compareTo == null || !this.GetType().Equals(compareTo.GetTypeUnproxied()))
            {
                return false;
            }

            if (this.HasSameNonDefaultIdAs(compareTo))
            {
                return true;
            }

            // Since the Ids aren't the same, both of them must be transient to 
            // compare domain signatures; because if one is transient and the 
            // other is a persisted domainModel, then they cannot be the same object.
            return this.IsTransient() && compareTo.IsTransient() && this.HasSameObjectSignatureAs(compareTo);
        }

        public override int GetHashCode()
        {
            if (this.cachedHashcode.HasValue)
            {
                return this.cachedHashcode.Value;
            }

            if (this.IsTransient())
            {
                this.cachedHashcode = base.GetHashCode();
            }
            else
            {
                unchecked
                {
                    // It's possible for two objects to return the same hash code based on 
                    // identically valued properties, even if they're of two different types, 
                    // so we include the object's type in the hash calculation
                    var hashCode = this.GetType().GetHashCode();
                    this.cachedHashcode = (hashCode * HashMultiplier) ^ this.Id.GetHashCode();
                }
            }

            return this.cachedHashcode.Value;
        }

        /// <summary>
        ///     Transient objects are not associated with an item already in storage.  For instance,
        ///     a Customer is transient if its Id is 0.  It's virtual to allow NHibernate-backed 
        ///     objects to be lazily loaded.
        /// </summary>
        public virtual bool IsTransient()
        {
            return this.Id == null || this.Id.Equals(default(string));
        }

        /// <summary>
        ///     The property getter for SignatureProperties should ONLY compare the properties which make up 
        ///     the "domain signature" of the object.
        /// 
        ///     If you choose NOT to override this method (which will be the most common scenario), 
        ///     then you should decorate the appropriate property(s) with [DomainSignature] and they 
        ///     will be compared automatically.  This is the preferred method of managing the domain
        ///     signature of domainModel objects.
        /// </summary>
        /// <remarks>
        ///     This ensures that the domainModel has at least one property decorated with the 
        ///     [DomainSignature] attribute.
        /// </remarks>
        protected override IEnumerable<PropertyInfo> GetTypeSpecificSignatureProperties()
        {
            return
                this.GetType().GetProperties().Where(
                    p => Attribute.IsDefined(p, typeof(DomainSignatureAttribute), true));
        }

        /// <summary>
        ///     Returns true if self and the provided domainModel have the same Id values 
        ///     and the Ids are not of the default Id value
        /// </summary>
        private bool HasSameNonDefaultIdAs(DomainModel compareTo)
        {
            return !this.IsTransient() && !compareTo.IsTransient() && this.Id.Equals(compareTo.Id);
        }

        public virtual bool IsValid()
        {
            return this.ValidationResults().Count == 0;
        }

        public virtual ICollection<ValidationResult> ValidationResults()
        {
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(this, new ValidationContext(this, null, null), validationResults, true);
            return validationResults;
        }

        void IAssignableId.SetIdTo(string prefix, string assignedId)
        {
            Id = string.Format("{0}/{1}", prefix, assignedId);
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dropman.Mds.Ws
{
    /// <summary>
    /// Describes MDS properties of object property (i. e. "First Name" in MDS is st. like "FirstName" in object model)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MdsAttributeAttribute: Attribute
    {
        public MdsAttributeAttribute(string name)
        {
            this.AttributeName = name;
        }
        /// <summary>
        /// Required: holds and reference between object's property name and entity's attribute name
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// free or domain based
        /// needed for domain based attributes to be correctly mapped to parent mds record
        /// </summary>
        public AttributeType Type { get; set; }

        /// <summary>
        /// contains SIMPLE CLASS NAME for domain based attribute
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// contains ASSEMBLY NAME for domain based attribute (in form without the ".dll" extension)
        /// </summary>
        public string AssemblyName { get; set; }
    }

    public enum AttributeType { Free, DomainBased }
}

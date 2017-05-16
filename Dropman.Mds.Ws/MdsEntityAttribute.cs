using System;

namespace Dropman.Mds.Ws
{

    [AttributeUsage(AttributeTargets.Class)]
    public class MdsEntityAttribute: Attribute
    {
        public string EntityName { get; set; }
        public string ModelName { get; set; }
    }
}

using System;

namespace Dropman.Mds.Ws
{
    public class AttributesNotSetException: ApplicationException
    {
        public AttributesNotSetException(string message) : base(message) { }
    }
}

using System;
using Dropman.Mds.Ws;

namespace Isb.Model
{
    public abstract class EntityBase: IEntityMember
    {
        [MdsAttribute("Code")]
        public string Code { get; set; }

        [MdsAttribute("Name")]
        public string Name { get; set; }

        public virtual void PostInit() { }
    }
}

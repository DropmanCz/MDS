using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dropman.Mds.Ws
{
    /// <summary>
    /// Every entity in mds needs Code and Name. Every business model needs to reflect mds data structure rules
    /// Who want to be a entity member class must to implement this iface
    /// </summary>
    public interface IEntityMember
    {
        string Code { get; set; }
        string Name { get; set; }

        void PostInit();
    }
}

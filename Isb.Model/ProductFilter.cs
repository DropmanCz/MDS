using Dropman.Mds.Ws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isb.Model
{
    [MdsEntity(EntityName = "Product Series Filter", ModelName = "Pulse ISB Cleansing")]
    public class ProductFilter: EntityBase
    {
        [MdsAttribute("Product Group Name")]
        public string ProductGroup { get; set; }

        [MdsAttribute("Product Family Name")]
        public string ProductFamily { get; set; }
    }
}

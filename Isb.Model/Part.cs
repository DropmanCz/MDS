using Dropman.Mds.Ws;

namespace Isb.Model
{
    [MdsEntity(EntityName = "Part", ModelName = "Pulse ISB Cleansing")]
    public class Part: SystemBase
    {
        [MdsAttribute("System", Type = AttributeType.DomainBased, DomainName = "Microscope", AssemblyName = "Isb.Model")]
        public string System { get; set; }

        [MdsAttribute("Item Number")]
        public string ItemNumber { get; set; }

        [MdsAttribute("Source of Data")]
        public string SourceOfData { get; set; }
    }
}

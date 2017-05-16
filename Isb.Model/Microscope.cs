using Dropman.Mds.Ws;

namespace Isb.Model
{
    [MdsEntity( EntityName = "System", ModelName = "Pulse ISB Cleansing")]
    public class Microscope: SystemBase
    {
        [MdsAttribute("ISB Part ISB")]
        public string IsbPartIsb { get; set; }

        [MdsAttribute("ISB Part BOM")]
        public string IsbPartBom { get; set; }

        [MdsAttribute("ISB Part Custom")]
        public string IsbPartCustom { get; set; }

        [MdsAttribute("Selected System Code", Type = AttributeType.Free, DomainName = "SelectedSystemCode", AssemblyName = "Isb.Model")]
        public string SelectedSystemCode { get; set; }

        [MdsAttribute("Issues")]
        public string Issues { get; set; }

        [MdsAttribute("Domain")]
        public string Domain { get; set; }

        [MdsAttribute("BOM Name")]
        public string BomName { get; set; }

        [MdsAttribute("GRID")]
        public string Grid { get; set; }
    }
}

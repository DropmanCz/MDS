using Dropman.Mds.Ws;
using System;

namespace Isb.Model
{
    public abstract class SystemBase: EntityBase
    {
        [MdsAttribute("Serial Number")]
        public string SerialNumber { get; set; }

        [MdsAttribute("Installation Date")]
        public DateTime InstallationDate { get; set; }

        public string InstallationDateForDisplay
        {
            get
            {
                return InstallationDate == DateTime.MinValue ? "": InstallationDate.ToString("dd.MM.yyyy");
            }
        }

        [MdsAttribute("End User ID")]
        public string EndUserId { get; set; }

        [MdsAttribute("Acceptance Date")]
        public DateTime AcceptanceDate { get; set; }

        public string AcceptanceDateForDisplay
        {
            get
            {
                return AcceptanceDate == DateTime.MinValue ? "" : AcceptanceDate.ToString("dd.MM.yyyy");
            }
        }

        [MdsAttribute("Quantity")]
        public double Quantity { get; set; }

        [MdsAttribute("Item Notes")]
        public string ItemNotes { get; set; }

        [MdsAttribute("Service Area")]
        public string ServiceArea { get; set; }

        [MdsAttribute("Eng. Code")]
        public string EngCode { get; set; }

        [MdsAttribute("Comm. Subclass")]
        public string CommSubclass { get; set; }

        [MdsAttribute("CRM Quote Number")]
        public string CrmQuoteNumber { get; set; }

        [MdsAttribute("Product Series")]
        public string ProductSeries { get; set; }

        [MdsAttribute("Rapid Status")]
        public string RapidStatus { get; set; }

        [MdsAttribute("To Be Migrated", Type = AttributeType.DomainBased, DomainName = "YesNo", AssemblyName = "Isb.Model")]
        public string ToBeMigrated { get; set; }

        [MdsAttribute("Approved For Migration", Type = AttributeType.DomainBased, DomainName = "YesNo", AssemblyName = "Isb.Model")]
        public string ApprovedForMigration { get; set; }

        [MdsAttribute("Reviewed", Type = AttributeType.DomainBased, DomainName = "YesNo", AssemblyName = "Isb.Model")]
        public string Reviewed { get; set; }


    }
}

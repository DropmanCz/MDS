using Dropman.Mds.Ws;
using Isb.Model;
using MdsWebServiceClient.TestConsole.MasterDataServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace MdsWebServiceClient.TestConsole
{
    class Program
    {
        /// <summary>
        /// read operation on MDS entity members
        /// </summary>
        /// <param name="proxy">Proxy on MDS WS</param>
        static void SelectMembers(ServiceClient proxy)
        {
            var info = new EntityMembersInformation(); 
            var res = new OperationResult();    //naprazdno nachystane, dodava informace o vysledku (napr. errory), ne vysledek samotny

            var crit = new EntityMembersGetCriteria()
            {
                MemberType = MemberType.Leaf
                , EntityId = new Identifier() { Id = new Guid("581591B5-004A-4B35-977D-87ACA110F401") }
                , ModelId = new Identifier() { Name = "Konfigurace" }
                , VersionId = new Identifier() { Name = "VERSION_1" }
                , MemberReturnOption = MemberReturnOption.Data
                //, SearchTerm = "Name = 'package.dtsx'"
            };

            //cteni dat z entity
            var test = proxy.EntityMembersGet(new International(), crit, new Guid(), out info, out res);

            if (res.Errors.Count() != 0)        //test chyboveho stavu vraceneho z MDS WS (neni to exception)
                Console.WriteLine(res.Errors[0].Description);
            else                                //cvicne zobrazeni vysledku
            {
                Console.WriteLine("Nacteno {0} zaznamu.", test.Members.Count());
                //pro code a name se musi cist memberid prop
                foreach (var item in test.Members)
                {
                    Console.WriteLine("{0}: {1}", item.MemberId.Code, item.MemberId.Name);
                }
            }
        }

        /// <summary>
        /// Insert operation
        /// </summary>
        /// <param name="proxy"></param>
        static void InsertMember(ServiceClient proxy)
        {
            var res = new OperationResult();    //naprazdno nachystane, dodava informace o vysledku (napr. errory), ne vysledek samotny

            Member[] memberList = new Member[1];
            memberList[0] = new Member()
            {
                MemberId = new MemberIdentifier() { Code = "-999", Name = "Do not use! For test purposes only!" }
                //, ValidationStatus = ValidationStatus.ValidationSucceeded
            };

            EntityMembers toInsert = new EntityMembers()
            {
                EntityId = new Identifier() { Id = new Guid("1B5F25C0-C887-4B6D-A418-58766581066F") },
                //ModelId = new Identifier() { Id = new Guid("467F56C7-1531-4C1F-8074-D9B58B149B62") },
                ModelId = new Identifier() { Name = "Demo" },
                VersionId = new Identifier() { Name = "VERSION_1" },
                Members = memberList
            };
            proxy.EntityMembersCreate(new International(), MemberTransactionBehavior.AllOrNothingByMember, toInsert, new Guid(), false, out res);
        }

        static void ValidateMembers(ServiceClient proxy)
        {
            var res = new OperationResult();    //naprazdno nachystane, dodava informace o vysledku (napr. errory), ne vysledek samotny
            ValidationIssue[] valIssue = null;
            ValidationProcessResult valRes = null;
            var valCrit = new ValidationProcessCriteria()
            {
                EntityId = new Identifier() { Name = "System" },
                ModelId = new Identifier() { Name = "Demo" },
                VersionId = new Identifier() { Name = "VERSION_1" }
            };
            proxy.ValidationProcess(new International(), new Guid(), valCrit, new ValidationProcessOptions(), out res, out valIssue, out valRes);
        }

        /// <summary>
        /// update operation on givem member
        /// </summary>
        /// <param name="proxy"></param>
        static void UpdateMember(ServiceClient proxy)
        {
            var memberList = new Member[1];
            memberList[0] = new Member()
            {
                MemberId = new MemberIdentifier() { Code = "-999", Name = "Do not use! For test only!" }
                //, ValidationStatus = ValidationStatus.ValidationSucceeded
            };

            memberList[0].MemberId.Name = "UPD";

            EntityMembers toInsert = new EntityMembers()
            {
                EntityId = new Identifier() { Id = new Guid("1B5F25C0-C887-4B6D-A418-58766581066F") },
                ModelId = new Identifier() { Id = new Guid("467F56C7-1531-4C1F-8074-D9B58B149B62") },
                VersionId = new Identifier() { Name = "VERSION_1" },
                Members = memberList
            };

            var attr = new MasterDataServices.Attribute() { Identifier = new Identifier() { Name = "Name" }, Value = "UPD" };
            memberList[0].Attributes = new MasterDataServices.Attribute[1];
            memberList[0].Attributes[0] = attr;
            var vysl = proxy.EntityMembersUpdate(new International(), MemberTransactionBehavior.AllOrNothingByMember, toInsert, new Guid());
        }

        static void Main(string[] args)
        {
            using (var proxy = new ServiceClient("WSHttpBinding_IService"))
            {
                SelectMembers(proxy);
                //ValidateMembers(proxy);
                //InsertMember(proxy);
                //UpdateMember(proxy);
            }

            Console.WriteLine("POKUS DOKONCEN");
            Console.ReadKey();
        }
    }
}

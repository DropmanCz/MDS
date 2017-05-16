using Dropman.Mds.Ws.MasterDataServices;
using System.Collections.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Dropman.Mds.Ws
{
    /// <summary>
    /// central singleton providing all CRUD actions through mds web service. Heavily uses Reflection
    /// </summary>
    public class ConnectionProvider
    {
        private ConnectionProvider() { }

        private ServiceClient proxy;

        private static ConnectionProvider _instance;
        //zivoty
        private static DateTime lastTouch;
        public static ConnectionProvider GetInstance()
        {
            
            if (_instance == null || (_instance != null && DateTime.Now.Subtract(lastTouch).Minutes >= 5))
            {
                _instance = new ConnectionProvider();
                _instance.proxy = new ServiceClient("WSHttpBinding_IService");
                
            }
            lastTouch = DateTime.Now;
            return _instance;
        }

        /// <summary>
        /// returns a collection of mds entity members transformed into business objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchTerm">if ommited, all data are returned (paging?)</param>
        /// <returns></returns>
        public Collection<T> GetList<T>(string searchTerm = null)
        {
            var ret = new Collection<T>();

            var info = new EntityMembersInformation();
            var res = new OperationResult();
            var crit = new EntityMembersGetCriteria()
            {
                MemberType = MemberType.Leaf,
                EntityId = new Identifier() { /*Id = new Guid("1B5F25C0-C887-4B6D-A418-58766581066F")*/ Name = GetEntityName<T>() },
                ModelId = new Identifier() { /*Id = new Guid("467F56C7-1531-4C1F-8074-D9B58B149B62")*/ Name = GetModelName<T>() },
                VersionId = new Identifier() { Name = "VERSION_1" },
                MemberReturnOption = MemberReturnOption.Data,
                SearchTerm = searchTerm
            };

            var mem = proxy.EntityMembersGet(new International(), crit, new Guid(), out info, out res).Members;

            var attrs = GetAttributeCollection<T>();

            foreach (var item in mem)
            {
                T inst = (T)typeof(T).GetConstructor(Type.EmptyTypes).Invoke(null); //new object
                foreach (var attr in item.Attributes)                               //data for the object
                {
                    if (attr.Value is MemberIdentifier)     // condition for domain-based attributes
                        attrs[attr.Identifier.Name].SetValue(inst, ((MemberIdentifier)attr.Value).Name);
                    else
                        try { attrs[attr.Identifier.Name].SetValue(inst, attr.Value); } catch { }
                }
                //extra action for Code and Name, those columns are not seen in Attributes collection
                attrs["Code"].SetValue(inst, item.MemberId.Code);
                attrs["Name"].SetValue(inst, item.MemberId.Name);

                ((IEntityMember)inst).PostInit();
                ret.Add(inst);
            }

            return ret;
        }

        public void Insert<T>(T item)
        {
            var res = new OperationResult();
            Member[] memberList = new Member[1];

            var attrs = GetAttributeCollection<T>();

            var member = new Member()
            {
                MemberId = new MemberIdentifier()
                {
                    //Name = attrs["Name"].GetValue(item).ToString(),
                    //Code = attrs["Code"].GetValue(item) == null ? null : attrs["Code"].GetValue(item).ToString(),
                    Name = ((IEntityMember)item).Name,
                    Code = ((IEntityMember)item).Code == null ? null : ((IEntityMember)item).Code
                }
            };

            MasterDataServices.Attribute[] memberAttributes = new MasterDataServices.Attribute[attrs.Keys.Count - 2];
            int i = 0;
            foreach (var key in attrs.Keys)
            {
                if (key != "Name" && key != "Code")
                {
                    object val = null;
                    if (((MdsAttributeAttribute)attrs[key].GetCustomAttributes(typeof(MdsAttributeAttribute), false)[0]).Type == AttributeType.DomainBased)
                    {
                        val = GetMemberIdentifier(item, attrs[key]);
                    }
                    else
                        val = attrs[key].GetValue(item);

                    var at = new MasterDataServices.Attribute()
                    {
                        Identifier = new Identifier() { Name = key },
                        Value =  val
                    };
                    memberAttributes[i++] = at;
                }
            }
            member.Attributes = memberAttributes;
            memberList[0] = member;

            EntityMembers toInsert = new EntityMembers()
            {
                EntityId = new Identifier() { Name = GetEntityName<T>() },
                ModelId = new Identifier() { Name = GetModelName<T>() },
                VersionId = new Identifier() { Name = "VERSION_1" },
                Members = memberList,
                MemberType = MemberType.Leaf
            };
            proxy.EntityMembersCreate(new International(), MemberTransactionBehavior.AllOrNothingByMember, toInsert, new Guid(), true, out res);
        }


        public void Update<T>(T item)
        {
            var memberList = new Member[1];

            var attrs = GetAttributeCollection<T>();
            MasterDataServices.Attribute[] memberAttributes = new MasterDataServices.Attribute[attrs.Keys.Count];
            int i = 0;
            foreach (var key in attrs.Keys)
            {
                object val = null;
                if (((MdsAttributeAttribute)attrs[key].GetCustomAttributes(typeof(MdsAttributeAttribute), false)[0]).Type == AttributeType.DomainBased)
                {
                    val = GetMemberIdentifier(item, attrs[key]);
                }
                else
                {
                    val = attrs[key].GetValue(item);
                }
                memberAttributes[i++] = new MasterDataServices.Attribute() { Identifier = new Identifier() { Name = key }, Value = val };
            }

            memberList[0] = new Member()
            {
                MemberId = new MemberIdentifier() { Code = ((IEntityMember)item).Code, Name = ((IEntityMember)item).Name },
                Attributes = memberAttributes
            };

            EntityMembers toUpdate = new EntityMembers()
            {
                EntityId = new Identifier() { Name = GetEntityName<T>() },
                ModelId = new Identifier() { Name = GetModelName<T>() },
                VersionId = new Identifier() { Name = "VERSION_1" },
                Members = memberList
            };

            var rets = proxy.EntityMembersUpdate(new International(), MemberTransactionBehavior.AllOrNothingByMember, toUpdate, new Guid());

        }

        public void Update<T>(List<T> items)
        {
            if (items == null)
                return;

            var memberList = new Member[items.Count];

            var attrs = GetAttributeCollection<T>();
            MasterDataServices.Attribute[] memberAttributes = new MasterDataServices.Attribute[attrs.Keys.Count];
            int i = 0;
            for (int k = 0; k<items.Count;k++)
            {
                var item = items[k];
                foreach (var key in attrs.Keys)
                {
                    object val = null;
                    if (((MdsAttributeAttribute)attrs[key].GetCustomAttributes(typeof(MdsAttributeAttribute), false)[0]).Type == AttributeType.DomainBased)
                    {
                        val = GetMemberIdentifier(item, attrs[key]);
                    }
                    else
                    {
                        val = attrs[key].GetValue(item);
                    }
                    memberAttributes[i++] = new MasterDataServices.Attribute() { Identifier = new Identifier() { Name = key }, Value = val };
                }
                i = 0;
                memberList[k] = new Member()
                {
                    MemberId = new MemberIdentifier() { Code = ((IEntityMember)item).Code, Name = ((IEntityMember)item).Name },
                    Attributes = memberAttributes
                };
            }

            EntityMembers toUpdate = new EntityMembers()
            {
                EntityId = new Identifier() { Name = GetEntityName<T>() },
                ModelId = new Identifier() { Name = GetModelName<T>() },
                VersionId = new Identifier() { Name = "VERSION_1" },
                Members = memberList
            };

            var rets = proxy.EntityMembersUpdate(new International(), MemberTransactionBehavior.AllOrNothingByBatch, toUpdate, new Guid());

        }

        public void Delete<T>(T item)
        {
            var memberList = new Member[1];
            memberList[0] = new Member()
            {
                MemberId = new MemberIdentifier() { Code = ((IEntityMember)item).Code }
            };
            EntityMembers toDelete = new EntityMembers()
            {
                EntityId = new Identifier() { Name = GetEntityName<T>() },
                ModelId = new Identifier() { Name = GetModelName<T>() },
                VersionId = new Identifier() { Name = "VERSION_1" },
                Members = memberList
            };

            var ret = proxy.EntityMembersDelete(new International(), toDelete, new Guid(), true);

        }

        /// <summary>
        /// Method finding and reading MdsEntityAttribute (which gives model name and entity name from pure data model)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private MdsEntityAttribute GetEntityAttribute<T>()
        {
            try
            {
                return (MdsEntityAttribute)typeof(T).GetCustomAttributes(typeof(MdsEntityAttribute), true)[0];
            }
            catch
            {
                throw new AttributesNotSetException(typeof(T).Name + " does not MdsEntityAttribute set.");
            }
        }

        private string GetModelName<T>()
        {
            return GetEntityAttribute<T>().ModelName;
        }

        private string GetEntityName<T>()
        {
            try
            {
                return GetEntityAttribute<T>().EntityName;
            }
            catch
            {
                throw new AttributesNotSetException(typeof(T).Name + " does not MdsEntityAttribute set.");
            }
        }

        /// <summary>
        /// Method establishes dictionary with list of model entity properties and their "reflection" to the MDS model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private Dictionary<string, PropertyInfo> GetAttributeCollection<T>()
        {
            var ret = new Dictionary<string, PropertyInfo>();
            var props = typeof(T).GetProperties();
            foreach (var item in props)
            {
                if (item.GetCustomAttributes(typeof(MdsAttributeAttribute), false).Length == 1)
                {
                    string key = ((MdsAttributeAttribute)item.GetCustomAttributes(typeof(MdsAttributeAttribute), false)[0]).AttributeName;
                    ret.Add(key, item);
                }
            }
            return ret;
        }

        /// <summary>
        /// For domain based attribute finds out code of the "parent" entity member
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="propInfo"></param>
        /// <returns></returns>
        private MemberIdentifier GetMemberIdentifier<T>(T item, PropertyInfo propInfo)
        {
            if (propInfo.GetValue(item) == null)
                return null;

            var domain = ((MdsAttributeAttribute)propInfo.GetCustomAttributes(typeof(MdsAttributeAttribute), false)[0]).DomainName;
            var assembly = ((MdsAttributeAttribute)propInfo.GetCustomAttributes(typeof(MdsAttributeAttribute), false)[0]).AssemblyName;

            //najde se typ "master" domeny
            var assemblyl = Assembly.Load(assembly);
            var typeList = assemblyl.GetTypes();
            var t = typeList.First(p => p.Name == domain);

            //na tomto typu se najde a nastavi metoda GetList tak, aby jeji generikum byla ta "master" domena
            var m = this.GetType().GetMethod("GetList").MakeGenericMethod(t);

            //search parametr metody GetList (hodnota jmena domain based atributu)
            object[] pars = { "Name = '" + propInfo.GetValue(item).ToString() + "'" };
            var vysl = m.Invoke(this, pars);

            //vim, ze vysledek je nejaka kolekce, tak si to prekonvertuju na IEnumerable (non-generic), prectu prvek...
            var obj = (vysl as IEnumerable).GetEnumerator();
            obj.MoveNext();

            //.. a nadrzo jej zkonvertuji dynamickym typem
            var em = Convert.ChangeType(obj.Current, t);

            //nakonec z nej udelam MemberIdentifier (objekt odkazujici na domenu)
            return new MemberIdentifier()
            {
                Name = propInfo.GetValue(item).ToString(),
                Code = em.GetType().GetProperty("Code").GetValue(em).ToString()
            };

        }

        //TOTO: Destroy!
    }
}

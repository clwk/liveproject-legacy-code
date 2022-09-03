using System;
using System.Collections.Generic;
using System.Linq;
using CustomerService.Model;

namespace CustomerService.Logic
{
    public class QueryMaster
    {
        private readonly IEnumerable<Customer> _c;

        public QueryMaster(IEnumerable<Customer> c)
        {
            _c = c;
        }

        public IEnumerable<Customer> GetResults(string cId, QueryType queryType, string data)
        {
            var list = new List<Customer>();
            var list2 = new List<Customer>();
            int i, j;

            var dictionary = new Dictionary<string, List<Customer>>();
            foreach (var c in _c)
            {
                if (c.UCC.Contains(cId))
                {
                    continue;
                }

                if (!dictionary.ContainsKey(c.HA.Country))
                {
                    dictionary[c.HA.Country] = new List<Customer>();
                }

                dictionary[c.HA.Country].Add(c);
            }

            foreach (var uc in dictionary.Keys)
            {
                dictionary[uc] = dictionary[uc].OrderByDescending(c => c.HasDone.Sum(hd => hd.QTY)).ToList();
            }

            if (queryType == QueryType.TypeA_C1)
            {
                list = dictionary[data].Where(c => !c.UCC.Contains(cId)).ToList();
            }
            else if (queryType == QueryType.TypeA_C2)
            {
                foreach (var dictionaryValue in dictionary.Values)
                {
                    foreach (var customer in dictionaryValue.Where(c => !c.UCC.Contains(cId)))
                    {
                        if(customer.HA.City == data) 
                            list.Add(customer);
                    }
                }
            }

            /*else // removed REQ ITEM 123.223.34234
            {
                foreach (var dictionaryValue in dictionary.Values)
                {
                    foreach (var customer in dictionaryValue)
                    {
                        foreach (var pp in customer.HasDone)
                        {
                            if (pp.CAT == data)
                            {
                                list.Add(customer);
                            }
                        }
                    }
            }*/
            else if (queryType == QueryType.TypeB10 || queryType == QueryType.TypeB100)
            {
                var maxQ = queryType == QueryType.TypeB10 ? 10u : 100u;

                foreach (var dictionaryValue in dictionary.Values)
                {
                    foreach (var customer in dictionaryValue.Where(c => !c.UCC.Contains(cId)))
                    {
                        if (customer.HasDone.Sum(hd => hd.QTY) < maxQ)
                            break;

                        list.Add(customer);
                    }
                }
            }
            else if(queryType == QueryType.TypeB10)
            {
                foreach (var dictionaryValue in dictionary.Values)
                {
                    foreach (var customer in dictionaryValue.Where(c => !c.UCC.Contains(cId)))
                    {
                        if (customer.HasDone.Sum(hd => hd.QTY) < 10)
                            break;

                        list.Add(customer);
                    }
                }
            }

            for (j = 0 ; j < list.Count ; j++)
            {
                var customer = list[j];
                if (customer.UCC.Contains(cId))
                    continue;

                list2.Add(customer);
            }

            return list2;
        }
    }
}
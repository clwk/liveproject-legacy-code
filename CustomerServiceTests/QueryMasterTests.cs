using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CustomerService.Logic;
using CustomerService.Model;
using Xunit;

namespace CustomerServiceTests
{
    public class QueryMasterTests
    {
        private readonly QueryMaster _queryMaster = new(_customers);

        [Fact]
        public void CustomerQuery_FreeShippingForCountry_ReturnAllEligibleCustomers()
        {
            var results = _queryMaster.GetResults("c-0", QueryType.TypeA_C1, "USA");

            var actual = results.Select(c => c.Id).ToImmutableSortedSet();

            Assert.Equal(new[] { "1", "3", "4" }, actual);
        }

        [Fact]
        public void CustomerQuery_FreeShippingForCountryAlreadyUsedCoupon_ReturnAllEligibleCustomers()
        {
            var results = _queryMaster.GetResults("c-1", QueryType.TypeA_C1, "USA");

            Assert.Equal(new[] { "4" }, results.Select(c => c.Id));
        }

        [Fact]
        public void CustomerQuery_FreeShippingForCity_ReturnAllEligibleCustomers()
        {
            var results = _queryMaster.GetResults("c-0", QueryType.TypeA_C2, "New York");

            var actual = results.Select(c => c.Id).ToImmutableSortedSet();

            Assert.Equal(new[] { "1", "4" }, actual);
        }

        [Fact]
        public void CustomerQuery_FreeShippingForCityAlreadyUsedCoupon_ReturnAllEligibleCustomers()
        {
            var results = _queryMaster.GetResults("c-1", QueryType.TypeA_C2, "New York");

            Assert.Equal(new[] { "4" }, results.Select(c => c.Id));
        }

        [Fact]
        public void CustomerQuery_PurchasedMoreThan10ItemsInThePast_ReturnAllEligibleCustomers()
        {
            var results = _queryMaster.GetResults("", QueryType.TypeB10, "");

            var actual = results.Select(c => c.Id).ToImmutableSortedSet();

            Assert.Equal(new[] { "2", "3", "4" }, actual);
        }

        [Fact]
        public void CustomerQuery_PurchasedMoreThan100ItemsInThePast_ReturnAllEligibleCustomers()
        {
            var results = _queryMaster.GetResults("", QueryType.TypeB100, "");

            var actual = results.Select(c => c.Id).ToImmutableSortedSet();

            Assert.Equal(new[] { "3", "4" }, actual);
        }

        [Fact]
        public void CustomerQuery_PurchasedMoreThan10ItemsInThePastUsedCoupon_ReturnAllEligibleCustomers()
        {
            var results = _queryMaster.GetResults("c-1", QueryType.TypeB10, "");

            var actual = results.Select(c => c.Id).ToImmutableSortedSet();

            Assert.Equal(new[] { "2", "4" }, actual);
        }

        [Fact]
        public void CustomerQuery_PurchasedMoreThan100ItemsInThePastUsedCoupon_ReturnAllEligibleCustomers()
        {
            var results = _queryMaster.GetResults("c-1", QueryType.TypeB100, "");

            Assert.Equal(new[] { "4" }, results.Select(c => c.Id));
        }

        private static readonly List<Customer> _customers = new()
        {
            new Customer
            {
                Id = "1",
                Name = "Lauren	Chapman",
                HA = new Address("USA", "New York", "Ap #859-7348 Luctus"),
                UCC = new List<string> { "c-1" },
                HasDone = new List<PP> { new PP("i-1", "i1", 5), }
            },
            new Customer
            {
                Id = "2",
                Name = "Pippa Hughes",
                HA = new Address("UK", "London", "9993 Turpis Ave"),
                UCC = new List<string> { "c-2" },
                HasDone = new List<PP> { new PP("i-3", "i3", 5), new PP("i-2", "i2", 6) }
            },
            new Customer
            {
                Id = "3",
                Name = "Robert Lambert",
                HA = new Address("USA", "Boston", "9937 Etiam Rd."),
                UCC = new List<string> { "c-1" },
                HasDone = new List<PP> { new PP("i-5", "i5", 55), new PP("i-21", "i21", 67) }

            },
            new Customer
            {
                Id = "4",
                Name = "Stephen	Peters",
                HA = new Address("USA", "New York", "279-5724 Dolor, Ave"),
                UCC = new List<string> { "c-2" },
                HasDone = new List<PP> { new PP("i-51", "i51", 55), new PP("i-212", "i212", 66) }
            }
        };
    }
}

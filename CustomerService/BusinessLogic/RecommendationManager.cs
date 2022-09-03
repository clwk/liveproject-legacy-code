using System.Collections.Generic;
using System.Linq;
using CustomerService.Config;
using CustomerService.DataAccess;
using CustomerService.Utils;

namespace CustomerService.BusinessLogic
{
    public class RecommendationManager
    {
        private readonly CustomerRepository _customerRepository;

        public RecommendationManager(IDatabaseSettings databaseSettings)
        {
            _customerRepository = new CustomerRepository(databaseSettings);
        }

        public void SendRecommendationsToCustomer(string customerId)
        {
            var customer = _customerRepository.FindById(customerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer {customerId} not found in DB");
            }

            var items = new List<string>();
            foreach (var friendId in customer.Friends)
            {
                var friend = _customerRepository.FindById(friendId);
                if (friend == null)
                    continue;

                foreach (var (id, _, _) in friend.HasDone)
                {
                    if (!items.Contains(id) && customer.HasDone.All(p => p.Id != id))
                    {
                        items.Add(id);
                    }
                }
            }

            if (items.Any())
            {
                // Must create emailClient before sending message and dispose afterwards
                using var emailClient = new EmailClient(customerId);
                emailClient.SendProductLinksToCustomer(items);
            }
        }
    }
}
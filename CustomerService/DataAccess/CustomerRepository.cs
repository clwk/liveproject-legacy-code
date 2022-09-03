using System.Collections.Generic;
using CustomerService.Config;
using CustomerService.Model;
using MongoDB.Driver;

namespace CustomerService.DataAccess
{
    public class CustomerRepository
    {
        private readonly IMongoCollection<Customer> _customers;

        public CustomerRepository(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _customers = database.GetCollection<Customer>("Customers");
        }

        public void Create(Customer customer)
        {
            _customers.InsertOne(customer);
        }

        public Customer FindById(string id)
        {
            return _customers.Find(c => c.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<Customer> GetAll()
        {
            return _customers.Find(_ => true).ToEnumerable();
        }

        public void Update(Customer customer)
        {
            _customers.ReplaceOne(c => c.Id == customer.Id, customer);
        }

        public void Remove(string id)
        {
            _customers.DeleteOne(c => c.Id == id);
        }
    }
}
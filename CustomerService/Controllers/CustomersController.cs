using System;
using System.Collections.Generic;
using System.Security.Claims;
using CustomerService.BusinessLogic;
using CustomerService.Config;
using CustomerService.DataAccess;
using CustomerService.Logic;
using CustomerService.Model;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerRepository _customerRepository;
        private readonly IDatabaseSettings _databaseSettings;

        public CustomersController(CustomerRepository customerRepository, IDatabaseSettings databaseSettings)
        {
            _customerRepository = customerRepository;
            _databaseSettings = databaseSettings;
        }

        [HttpGet]
        public IEnumerable<Customer> GetAll()
        {
            return _customerRepository.GetAll();
        }

        [HttpGet("query")]
        public IEnumerable<Customer> QueryCustomers([FromQuery(Name = "subjectId")] string cid, [FromQuery(Name = "queryType")] QueryType queryType, [FromQuery(Name = "data")] string data)
        {
            var allCustomers = _customerRepository.GetAll();

            var queryMaster = new QueryMaster(allCustomers);
            var results = queryMaster.GetResults(cid, queryType, data);

            var filtered = new List<Customer>();
            foreach (var customer in results)
            {
                if (customer.IsValid)
                {
                    filtered.Add(customer);
                }
            }

            foreach (var customer in filtered)
            {
                customer.UCC.Add(cid);

                _customerRepository.Update(customer);
            }

            return filtered;
        }

        [HttpPost("{id}/recommend")]
        public IActionResult SendRecommendations(string id)
        {
            try
            {
                var recommendationManager = new RecommendationManager(_databaseSettings);
                recommendationManager.SendRecommendationsToCustomer(id);
            }
            catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpGet("{id}/purchases")]
        public ActionResult<IEnumerable<PP>> GetFriendPurchases(string id)
        {
            var userId = HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return NotFound();
            }

            var customer = _customerRepository.FindById(userId);
            if (customer == null)
            {
                return NotFound();
            }

            if (!customer.Friends.Contains(id))
            {
                // Not a friend
                return BadRequest();
            }

            var friend = _customerRepository.FindById(id);

            if (friend == null)
            {
                return NotFound();
            }

            return friend.HasDone;
        }

        [HttpGet("{id:length(24)}", Name = "GetCustomer")]
        public ActionResult<Customer> FindById(string id)
        {
            var customer = _customerRepository.FindById(id);

            if (customer == null) return NotFound();

            return customer;
        }

        [HttpPost]
        public ActionResult<Customer> Create([FromBody] Customer customer)
        {
            _customerRepository.Create(customer);

            return CreatedAtRoute("GetCustomer", new { id = customer.Id }, customer);
        }

        [HttpPut]
        public IActionResult Update([FromBody] Customer customer)
        {
            _customerRepository.Update(customer);

            return Ok();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            _customerRepository.Remove(id);

            return NoContent();
        }
    }
}

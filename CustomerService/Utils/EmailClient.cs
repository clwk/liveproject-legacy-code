using System;
using System.Collections.Generic;

namespace CustomerService.Utils
{
    public interface IEmailClient : IDisposable
    {
        void SendProductLinksToCustomer(IEnumerable<string> items);
    }

    public class EmailClient : IEmailClient
    {
        private readonly string _customerId;
        private bool _disposed;

        public EmailClient(string customerId)
        {
            _customerId = customerId;
            _disposed = false;
        }


        public void Dispose()
        {
            _disposed = true;
        }

        public void SendProductLinksToCustomer(IEnumerable<string> items)
        {
            if (_disposed) throw new AccessViolationException("Cannot access disposed email client");
            //Email logic and code

            Console.WriteLine($"Sending products to {_customerId}");
            foreach (var item in items)
            {
                Console.WriteLine($"{item}");
            }
        }
    }
}
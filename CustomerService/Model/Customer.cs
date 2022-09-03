using System.Collections.Generic;

namespace CustomerService.Model
{
    public record Customer
    {
        public string Id { get; init; }
        public string Name { get; init; }

        public Address HA { get; init; }
        public List<string> UCC { get; init; } = new();
        public List<PP> HasDone { get; init; } = new();
        public List<string> Friends { get; init; } = new();
        public bool IsValid { get; set; } = true;
    }

    public record Address(string Country, string City, string Street);

    public record PP(string Id, string Name, uint QTY);
}
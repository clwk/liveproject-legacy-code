namespace CustomerService.Config
{
    public interface IDatabaseSettings
    {
        string ConnectionString { get; }
        string DatabaseName { get;  }
    }

    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
namespace TechChallenge.Games.Web.Configurations
{
    public class AzureEventHubConfig
    {
        public string ConnectionString { get; set; }
        public string HubName { get; set; }
        public string ConsumerGroup { get; set; }
    }
}

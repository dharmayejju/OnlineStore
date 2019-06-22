using Microsoft.Extensions.Options;

namespace OnlineStore.Infrastructure
{
    internal class DataServiceConnectionSettings : IConnectionSettings
    {
        private readonly IOptions<DataServiceOptions> _options;

        public DataServiceConnectionSettings(IOptions<DataServiceOptions> options)
        {
            _options = options;
        }

        public string ConnectionString => _options.Value.ConnectionString;
    }
}

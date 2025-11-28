using dotnet_etcd;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Etcd
{
    public class EtcdClientManager : IEtcdClientManager
    {
        private readonly IConfiguration _configuration;
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private readonly int _port;
        private readonly EtcdClient client;
        private ILogger<EtcdClientManager> _logger;

        public EtcdClientManager(IConfiguration configuration, ILogger<EtcdClientManager> logger)
        {
            _logger = logger;
            _configuration = configuration;

            _hostName = _configuration.GetValue<string>("Etcd:Default:HostName");
            _port = _configuration.GetValue<int>("Etcd:Default:Port");
            _userName = _configuration.GetValue<string>("Etcd:Default:UserName");
            _password = _configuration.GetValue<string>("Etcd:Default:Password");

            var url = this.GetUrl();
            client = new EtcdClient(url);
        }

        private string GetUrl()
        {
            return $"http://{_hostName}:{_port}";
        }

        private Grpc.Core.Metadata CreateHeader()
        {
            var authRes = client.Authenticate(new Etcdserverpb.AuthenticateRequest()
            {
                Name = _userName,
                Password = _password
            });

            var headers = new Grpc.Core.Metadata();
            headers.Add("Authorization", authRes.Token);

            return headers;
        }

        public async Task<string> GetValAsync(string key)
        {
            try
            {
                var bayResults = await client.GetValAsync(key, this.CreateHeader());
                return bayResults.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"EtcdClientManager GetValAsync Exception =>{ex.Message}");
                return string.Empty;
            }
        }

        public async Task<Dictionary<string, string>> GetRangeAsync(string prefixKey)
        {
            try
            {
                var response = await client.GetRangeAsync(prefixKey, this.CreateHeader());
                return response?.Kvs?.Where(t => t.Key != null).ToDictionary(t => t.Key.ToStringUtf8(), t => t.Value.ToStringUtf8());

            }
            catch (Exception ex)
            {
                _logger.LogError($"EtcdClientManager GetValAsync Exception =>{ex.Message}");
                return null;
            }
        }

        public async Task Watch(string key, Action<WatchEvent[]> method)
        {
            try
            {
                client.Watch(key, method, this.CreateHeader());
            }
            catch (Exception ex)
            {
                _logger.LogError($"EtcdClientManager Watch Exception =>{ex.Message}");
            }
        }
    }
}

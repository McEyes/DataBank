using dotnet_etcd;
using Etcdserverpb;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using AuthenticateRequest = Etcdserverpb.AuthenticateRequest;

namespace ITPortal.Core.Etcd.Configuration
{
    public class EtcdConfigurationRepository : IConfigrationRepository
    {
        private readonly EtcdOptions _etcdOptions;
        private readonly EtcdClient _etcdClient;
        private readonly ILogger<EtcdConfigurationRepository> _logger;
        public EtcdConfigurationRepository(EtcdOptions etcdOptions)
        {
            if (etcdOptions == null) return;
            //_logger = logger;
            if (string.IsNullOrEmpty(etcdOptions.ConnectionString))
            {
                throw new ArgumentNullException($"{nameof(etcdOptions.ConnectionString)} can't be null");
            }

            if (etcdOptions.PrefixKeys == null || !etcdOptions.PrefixKeys.Any())
            {
                throw new ArgumentNullException($"{nameof(etcdOptions.PrefixKeys)} can't be null");
            }

            _etcdOptions = etcdOptions;
            _etcdClient = new EtcdClient(etcdOptions.ConnectionString
                //caCert: _etcdOptions.CaCert,
                //clientCert: _etcdOptions.ClientCert,
                //clientKey: _etcdOptions.ClientKey,
                //publicRootCa: _etcdOptions.PublicRootCa
                );
        }

        private Grpc.Core.Metadata GetHeaders()
        {
            if (!string.IsNullOrEmpty(_etcdOptions.Username) && !string.IsNullOrEmpty(_etcdOptions.Password))
            {
                var authRes = _etcdClient.Authenticate(new AuthenticateRequest { Name = _etcdOptions.Username, Password = _etcdOptions.Password });

                var _headers = new Grpc.Core.Metadata
                {
                    { "Authorization", authRes.Token }
                };

                return _headers;
            }

            return null;
        }

        private async Task<IDictionary<string, string>> GetEtcdConfigAsync()
        {
            var key = _etcdOptions.AppSettings;
            if (_etcdOptions.PrefixKeys.Count > 0)
            {
                key = $"{_etcdOptions.PrefixKeys[0]}{_etcdOptions.AppSettings}";
            }

            var content = await _etcdClient.GetValAsync(key, GetHeaders());
            if (content == null) return null;

            var result = (JObject)JsonConvert.DeserializeObject(content);
            if (result == null) return null;
            
            var dictResult = EtcdJsonParser.Deserialize(result);
            return dictResult;
        }

        public async Task<IDictionary<string, string>> GetConfig()
        {
            try
            {
                return await GetEtcdConfigAsync();
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogInformation(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }

            return null;
        }

        public void Watch(IConfigrationWatcher watcher)
        {
            Task.Run(() =>
            {
                var keys = _etcdOptions.PrefixKeys;

                if (!string.IsNullOrEmpty(_etcdOptions.Env))
                {
                    keys = _etcdOptions.PrefixKeys.Select(prefixKey => $"{_etcdOptions.Env}{prefixKey}").ToList();
                }

                try
                {
                    _etcdClient.WatchRange(keys.ToArray(), (WatchResponse response) =>
                    {
                        if (response.Events.Count > 0)
                        {
                            watcher.FireChange();
                        }
                    }, GetHeaders());
                }
                catch (Exception ex)
                {
                    if (_logger != null)
                    {
                        _logger.LogInformation(ex.Message + Environment.NewLine + ex.StackTrace);
                    }

                }
            });
        }

        #region Dispose

        bool _disposed;
        protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _etcdClient.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~EtcdConfigurationRepository()
        {
            Dispose(false);
        }
        #endregion
    }
}

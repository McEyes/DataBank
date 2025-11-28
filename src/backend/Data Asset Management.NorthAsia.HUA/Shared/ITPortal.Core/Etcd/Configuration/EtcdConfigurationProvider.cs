using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Etcd.Configuration
{
    public class EtcdConfigurationProvider : ConfigurationProvider, IConfigurationSource, IConfigrationWatcher
    {
        private readonly IConfigrationRepository _configRepository;
        private readonly Action<IConfigurationRoot> _actionOnChange;

        public EtcdConfigurationProvider(IConfigrationRepository configRepository, bool reloadOnChange, Action<IConfigurationRoot> actionOnChange)
        {
            _configRepository = configRepository;
            _actionOnChange = actionOnChange;

            if (reloadOnChange || actionOnChange != null)
            {
                _configRepository.Watch(this);
            }
        }

        private void Reload()
        {
            Load();

            //return the latest configuration
            if (_actionOnChange != null)
            {
                var builder = new ConfigurationBuilder().AddInMemoryCollection(Data).Build();
                _actionOnChange.Invoke(builder);
            }
        }

        public override void Load()
        {
            var result = _configRepository.GetConfig().Result;
            if (result == null || result.Count == 0)
                return;
            Data = result;
        }

        public void FireChange()
        {
            Reload();
            OnReload();
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => this;
    }
}

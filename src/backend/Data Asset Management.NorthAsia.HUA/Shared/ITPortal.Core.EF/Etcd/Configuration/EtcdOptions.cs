using ITPortal.Core.Encrypt;
using ITPortal.Extension.System;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Etcd.Configuration
{
    /// <summary>
    /// Etcd Setting Options
    /// </summary>
    public class EtcdOptions
    {
        public bool Enabled { get; set; }
        /// <summary>
        /// Etcd Connection String
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Environment. /dev or /uat or /prod  Default : Empty String
        /// </summary>
        public string Env { get; set; } = string.Empty;

        public string AppSettings { get; set; }

        /// <summary>
        /// config prefixKeys, no need to include env value
        /// </summary>
        public List<string> PrefixKeys { get; set; }


        private int myVar;

        public int MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }


        /// <summary>
        /// String containing username for etcd basic auth. Default : Empty String
        /// </summary>
        private string _Username = string.Empty;
        public string Username
        {
            get
            {
                if (UsernameCipherText.IsNullOrWhiteSpace())
                {
                    return _Username;
                }
                else
                {
                    return DESUtil.Decrypt(UsernameCipherText, DataAssetManagerConst.DESKey);
                }
            }
            set
            {
                _Username = value;
            }
        }

        /// <summary>
        /// String containing password for etcd basic auth. Default : Empty String
        /// </summary>
        private string _Password = string.Empty;
        public string Password
        {
            get
            {
                if (PasswordCipherText.IsNullOrWhiteSpace())
                {
                    return _Password;
                }
                else
                {
                    return DESUtil.Decrypt(PasswordCipherText, DataAssetManagerConst.DESKey);
                }
            }
            set
            {
                _Password = value;
            }
        }
        public string UsernameCipherText { get; set; } = string.Empty;
        public string PasswordCipherText { get; set; } = string.Empty;

        /// <summary>
        /// String containing ca cert when using self signed certificates with etcd. Default : Empty String
        /// </summary>
        public string CaCert { get; set; } = string.Empty;

        /// <summary>
        /// String containing client cert when using self signed certificates with client auth enabled in etcd. Default : Empty String
        /// </summary>
        public string ClientCert { get; set; } = string.Empty;

        /// <summary>
        /// String containing client key when using self signed certificates with client auth enabled in etcd. Default : Empty String
        /// </summary>
        public string ClientKey { get; set; } = string.Empty;

        /// <summary>
        /// Bool depicting whether to use publicy trusted roots to connect to etcd. Default : false.
        /// </summary>
        public bool PublicRootCa { get; set; }

        /// <summary>
        /// Set key mode. Default : Json
        /// </summary>
        public EtcdConfigrationKeyMode KeyMode { get; set; } = EtcdConfigrationKeyMode.Json;

    }
}

using System;
using System.Collections.Concurrent;
using System.Data;
using DataQualityAssessment.Core.Dappers;
using DataQualityAssessment.Core.Helpers;
using DataQualityAssessment.Core.Models;
using Furion.DataEncryption;
using Furion.DependencyInjection;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;

namespace DataQualityAssessment.EntityFramework.Core.Dappers
{
    public class DbHelper : IDbHelper, ITransient
    {
        private static ConcurrentDictionary<string, IDbConnection> connectionDict = new();
        private static object _locker = new object();
        public DbHelper() { }
        public IDbConnection CreateDbConnection(DbSettings settings)
        {
            lock (_locker)
            {
                var key = GetKey(settings);
                if (connectionDict.ContainsKey(key))
                {
                    if (connectionDict[key] != null)
                        return connectionDict[key];
                    else
                        connectionDict.TryRemove(key, out _);
                }

                switch (settings.DbType)
                {
                    case DataQualityAssessment.Core.Enums.DatabaseType.SqlServer:
                        connectionDict.TryAdd(key, new SqlConnection(BuildConnectionString(settings)));
                        break;
                    case DataQualityAssessment.Core.Enums.DatabaseType.Mysql:
                        connectionDict.TryAdd(key, new MySqlConnection(BuildConnectionString(settings)));
                        break;
                    case DataQualityAssessment.Core.Enums.DatabaseType.Postgresql:
                        connectionDict.TryAdd(key, new NpgsqlConnection(BuildConnectionString(settings)));
                        break;
                    default:
                        throw new NotSupportedException();
                }

                return connectionDict[key];
            }
        }

        public void CloseAllConnection()
        {
            if (connectionDict != null)
            {
                foreach (var item in connectionDict.Values)
                {
                    if (item != null)
                    {
                        item.Close();
                        item.Dispose();
                    }
                }

                connectionDict.Clear();
            }
        }

        private string GetKey(DbSettings settings)
        {
            var keyString = MD5Encryption.Encrypt($"{settings.Host}_{settings.Port}_{settings.DbName}_{settings.Username}");
            return keyString;
        }

        public string BuildConnectionString(DbSettings settings)
        {
            switch (settings.DbType)
            {
                case DataQualityAssessment.Core.Enums.DatabaseType.SqlServer:
                    return $"Server={settings.Host.Replace("\\\\","\\")},{settings.Port};Database={settings.DbName};User Id={settings.Username};Password={settings.Password};Encrypt=True;TrustServerCertificate=True;Pooling=true;Connect Timeout=120;";
                case DataQualityAssessment.Core.Enums.DatabaseType.Mysql:
                    return $"Server={settings.Host};Port={settings.Port};Database={settings.DbName};User Id={settings.Username};Password={settings.Password};Charset=utf8;Pooling=false;";
                case DataQualityAssessment.Core.Enums.DatabaseType.Postgresql:
                    return $"Host={settings.Host};Port={settings.Port};Username={settings.Username};Password={settings.Password};Database={settings.DbName};Pooling=true;";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

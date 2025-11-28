using System;
using System.Collections.Generic;
using System.Data;

//using MySql.Data.MySqlClient;

//using Npgsql;

//using Oracle.ManagedDataAccess.Client;
namespace DataAssetManager.DataApiServer.Core
{
    public class DatabaseService
    {

        public List<object> ExecuteSql(RouteRegistrationRequest request)
        {
            var data = new List<object>();
            Console.WriteLine($"request json:{Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
            data.Add(request);
            return data;
        }

        public List<Dictionary<string, object>> ExecuteSql(string connectionString, string sql, Dictionary<string, object> parameters, string dbType)
        {
            var data = new List<Dictionary<string, object>>();
            var item = new Dictionary<string, object>();
            item.Add("connectionString", connectionString);
            item.Add("sql", sql);
            item.Add("parameters", parameters);
            item.Add("dbType", dbType);
            data.Add(item);
            return data;
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            IDbConnection connection = null;

            //switch (dbType.ToLower())
            //{
            //    case "mysql":
            //        connection = new MySqlConnection(connectionString);
            //        break;
            //    case "postgresql":
            //        connection = new NpgsqlConnection(connectionString);
            //        break;
            //    case "oracle":
            //        connection = new OracleConnection(connectionString);
            //        break;
            //    default:
            //        throw new ArgumentException("Unsupported database type");
            //}

            try
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    foreach (var param in parameters)
                    {
                        var dbParam = command.CreateParameter();
                        dbParam.ParameterName = param.Key;
                        dbParam.Value = param.Value;
                        command.Parameters.Add(dbParam);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            result.Add(row);
                        }
                    }
                }
            }
            finally
            {
                connection.Close();
            }

            return result;
        }
    }
}
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Etcd.Configuration
{
    public class EtcdJsonParser
    {
        static Dictionary<string, string> dict = new Dictionary<string, string>();
        static List<string> keyList = new List<string>();

        public static Dictionary<string, string> Deserialize(JObject jObject)
        {
            dict.Clear();
            keyList.Clear();

            foreach (var item in jObject)
            {
                keyList.Clear();
                keyList.Add(item.Key);
                if (item.Value != null)
                    Recurse(item.Value);
            }

            return dict;
        }
        private static void Recurse(JToken jToken)
        {
            if (jToken is JArray)
            {
                var index = 0;
                var jarray = (JArray)jToken;
                var temp = new List<string>(keyList);
                foreach (var item in jarray)
                {
                    keyList = new List<string>(temp);
                    keyList.Add(index.ToString());
                    Recurse(item);
                    index++;
                }
            }
            else if (jToken is JObject)
            {
                var temp = new List<string>(keyList);
                var jObject = (JObject)jToken;
                foreach (var item in jObject)
                {
                    keyList = new List<string>(temp);
                    keyList.Add(item.Key);

                    if (item.Value != null)
                        Recurse(item.Value);
                }
            }
            else if (jToken is JValue)
            {
                var jValue = (JValue)jToken;
                var key = string.Join(":", keyList);
                if (!dict.ContainsKey(key))
                    dict.Add(key, jValue.Value<string>());
                if (keyList.Count > 0)
                    keyList.RemoveAt(keyList.Count - 1);
            }
        }
    }
}

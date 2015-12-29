using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.CacheClient.Configuration
{
    public class RedisHost : ConfigurationElement
    {
        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get
            {
                return this["host"] as string;
            }
        }

        [ConfigurationProperty("cachePort", IsRequired = true)]
        public int CachePort
        {
            get
            {
                var config = this["cachePort"];
                if (config != null)
                {
                    var value = config.ToString();

                    if (!string.IsNullOrEmpty(value))
                    {
                        int result;

                        if (int.TryParse(value, out result))
                        {
                            return result;
                        }
                    }
                }


                throw new Exception("Redis Cahe port must be number.");
            }
        }
    }
}

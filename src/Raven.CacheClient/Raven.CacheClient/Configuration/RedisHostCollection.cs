using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.CacheClient.Configuration
{
    public class RedisHostCollection : ConfigurationElementCollection
    {
        public RedisHost this[int index]
        {
            get
            {
                return BaseGet(index) as RedisHost;
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }

                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RedisHost();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            RedisHost ele = element as RedisHost;
            return ele.Host + ":" + ele.Port;
        }
    }
}

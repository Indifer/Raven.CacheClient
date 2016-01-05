using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.CacheClient
{
    public enum SetType
    {
        /// <summary>
        /// 
        /// </summary>
        Always = 0,
        /// <summary>
        /// 
        /// </summary>
        Exists = 1,
        /// <summary>
        /// 
        /// </summary>
        NotExists = 2
    }
}

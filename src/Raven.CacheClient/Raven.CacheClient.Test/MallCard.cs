using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.CacheClient.Test
{
    public interface ICacheKey
    {
        string GetKey();
    }

    public class MallCard : ICacheKey
    {
        public string Name { get; set; }

        /// <summary>
        ///自增主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        ///// 商场id
        ///// </summary>
        public long MallID { get; set; }

        ///// <summary>
        ///// 用户UID[为0表示未绑定猫酷用户]
        ///// </summary>
        public long UID { get; set; }

        ///// <summary>
        ///// 会员卡类型ID
        ///// </summary>
        public long? CardTypeID { get; set; }

        public string GetKey()
        {
            return string.Concat("MallCard", ID);
        }
    }

    public class MallCard2 : ICacheKey
    {

        /// <summary>
        ///// 商场id
        ///// </summary>
        public long MallID { get; set; }

        /// <summary>
        ///自增主键
        /// </summary>
        public string ID { get; set; }

        ///// <summary>
        ///// 会员卡类型ID
        ///// </summary>
        public long? CardTypeID { get; set; }

        public string Name { get; set; }

        public string GetKey()
        {
            return string.Concat("MallCard", ID);
        }
    }

}

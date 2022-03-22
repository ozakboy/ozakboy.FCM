using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozakboy.FCM
{
    public interface IFCM
    {
        /// <summary>
        /// 訂閱主題
        /// </summary>
        /// <param name="token">使用者Token</param>
        /// <param name="topic">主題名稱</param>
          void SubscribeTopic(string token, string topic);
    }
}

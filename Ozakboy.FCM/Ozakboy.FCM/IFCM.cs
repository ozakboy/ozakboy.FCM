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


        /// <summary>
        /// 發送訊息
        /// </summary>
        /// <param name="token">使用者Token</param>
        /// <param name="title">標題</param>
        /// <param name="message">內文</param>
        /// <param name="Clicek_Url">點擊前往網站</param>
        /// <param name="Image_Uri">要顯示的圖片</param>
        void FcmSend(string token, string title, string message, string Clicek_Url, string Image_Uri);

        /// <summary>
        /// 發送訊息
        /// </summary>
        /// <param name="token">使用者Token</param>
        /// <param name="title">標題</param>
        /// <param name="message">內文</param>
        void FcmSend(string token, string title, string message);


        /// <summary>
        /// 發送訊息
        /// </summary>
        /// <param name="token">使用者Token</param>
        /// <param name="title">標題</param>
        /// <param name="message">內文</param>
        /// <param name="Clicek_Url">點擊前往網站</param>
        void FcmSend(string token, string title, string message, string Clicek_Url);
    }
}

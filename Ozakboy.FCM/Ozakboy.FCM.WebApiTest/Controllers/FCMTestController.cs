using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ozakboy.FCM.WebApiTest.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FCMTestController : ControllerBase
    {
        private IFCM _ifcm;
        private string  Token = "c_4mjMz0QoBbPeBhn8FNJf:APA91bF7YgCMjgTUYOkcVKEB1_1feQp92e8h_klOwCa4-8VEd51uTiNwI5qgk2qh_oKxu4fzG7UtG4IxB3R46Q7TlO_GqoYxRAh-PRu9n7p-hiAvVOuRpcLZ4Z-SnOEXgHAKxiTq52lg";
       
        public FCMTestController(IFCM ifcm)
        {
            _ifcm = ifcm;
        }

        [HttpGet]
        public IActionResult TestGet()
        {
            _ifcm.FcmSend(Token,"測試","發送");
            return Ok();
        }

        [HttpGet]
        public IActionResult SetGroup()
        {
             _ifcm.SubscribeTopic(Token, "UserEnd");
            return Ok();
        }
        [HttpGet]

        public IActionResult SendGroupMessage()
        {
            _ifcm.FcmSendTopic("UserEnd","測試群組","群組訊息");
            return Ok();
        }
    }
}

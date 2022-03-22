using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Ozakboy.FCM.ViewModels;
using System.Net.Mime;



namespace Ozakboy.FCM
{
    public class FCM : IFCM
    {
        private VFCMSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public FCM(IConfiguration _configuration , IHttpClientFactory httpClientFactory)
        {
            _settings = _configuration.Get<VFCMSettings>();
            _httpClientFactory = httpClientFactory;
        }

        public void SubscribeTopic(string token, string topic)
        {
          
            if (String.IsNullOrEmpty(token) || String.IsNullOrEmpty(topic))
              throw new Exception($"請輸入用戶 Token");
                
            var client =  _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://iid.googleapis.com");
            client.DefaultRequestHeaders.Add("Authorization" , $"key={_settings.API_Key}");
                
            var data = new
            {
                to = String.Format("/topics/{0}", topic),
                registration_tokens = new string[] { token }
            };

            var JsonData = JsonConvert.SerializeObject(data);

            HttpContent contentPost = new StringContent(JsonData, Encoding.UTF8, MediaTypeNames.Application.Json);
            client.PostAsync("/iid/v1:batchAdd", contentPost);
        }
           
    }
}

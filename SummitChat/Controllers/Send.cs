using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using NuGet.Protocol.Plugins;
using SummitChat.Data;
using System.Net.Http.Headers;
using System.Text;



namespace SummitChat.Controllers
{
    public class Send : Controller
    {
        public class MessageDto
        {
            public string phone { get; set; }
            public string message { get; set; }
            public string callback_url { get; set; }
        }
        public class MessageIdDto
        {
            public string id { get; set; }
        }
        private readonly ApplicationDbContext _context;

        public Send(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddMessageToDatabase(string messageID, int senderID, string receiverID)
        {
            var message = new Models.Message
            {
                MessageID = messageID,
                SenderID = senderID,
                ReceiverID = receiverID
            };

            _context.Add(message);
            await _context.SaveChangesAsync();
        }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> SendMessage(string userInput, string phoneInput)
        {

            Console.WriteLine("Сообщение: " + userInput + "\nТелефон:" + phoneInput);
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string msgUuid = await Post(client, userInput, phoneInput);
                    string messageID = msgUuid;
                    int senderID = 1;
                    await AddMessageToDatabase(messageID, senderID, phoneInput);
                    return Json(new { success = true, message = "Сообщение успешно отправлено." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка: " + ex.Message);
                    return Json(new { success = false, message = "Произошла ошибка при отправке сообщения." });
                }
            }
        }
        static async Task<string> Post(HttpClient client, string msg, string phoneInput)
        {
            string postUrl = "http://10.250.2.2:2050/api/Messages";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI3NWZjNzBhYy1hMDI2LTQyNjgtYmRmMC05MzA1ZWY0YzY5NmIiLCJ1bmlxdWVfbmFtZSI6Imdyb3VwLjYiLCJuYmYiOjE2OTM3NDg2MDksImV4cCI6MTcyNDg1MjYwOSwiaWF0IjoxNjkzNzQ4NjA5LCJpc3MiOiJodHRwczovL2RvYnJvemFpbS5ydS8iLCJhdWQiOiJodHRwczovL2RvYnJvemFpbS5ydS8ifQ.T2LG-NJv4qJKqm-HVzI7sEBDPPXu1dJuOFx9xpFFbNo");
            var messageDto = new
            {
                phone = phoneInput,
                message = msg,
                callback_url = "http://localhost:5002/"
            };
            string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(messageDto);

            var postContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage postResponse = await client.PostAsync(postUrl, postContent);
            if (postResponse.IsSuccessStatusCode)
            {
                string postResponseContent = await postResponse.Content.ReadAsStringAsync();
                var messageIdDto = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageIdDto>(postResponseContent);
                Console.WriteLine(messageIdDto.id);
                return messageIdDto.id;
            }
            else
            {
                Console.WriteLine("Ошибка POST-запроса: " + postResponse.StatusCode);
                return null;
            }
        }
        static async Task<MessageDto> Get(HttpClient client, string msgUuid)
        {
            string getUrl = "http://10.250.2.2:2050/api/Messages/" + msgUuid;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI3NWZjNzBhYy1hMDI2LTQyNjgtYmRmMC05MzA1ZWY0YzY5NmIiLCJ1bmlxdWVfbmFtZSI6Imdyb3VwLjYiLCJuYmYiOjE2OTM3NDg2MDksImV4cCI6MTcyNDg1MjYwOSwiaWF0IjoxNjkzNzQ4NjA5LCJpc3MiOiJodHRwczovL2RvYnJvemFpbS5ydS8iLCJhdWQiOiJodHRwczovL2RvYnJvemFpbS5ydS8ifQ.T2LG-NJv4qJKqm-HVzI7sEBDPPXu1dJuOFx9xpFFbNo");
            HttpResponseMessage getResponse = await client.GetAsync(getUrl);
            if (getResponse.IsSuccessStatusCode)
            {
                string getContent = await getResponse.Content.ReadAsStringAsync();
                var messageDto = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageDto>(getContent);
                Console.WriteLine(messageDto.message);
                return messageDto;
            }
            else
            {
                Console.WriteLine("Ошибка GET-запроса: " + getResponse.StatusCode);
                return null;
            }
        }
    }
}

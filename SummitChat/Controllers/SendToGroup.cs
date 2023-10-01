using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using SummitChat.Data;
using System.Net.Http.Headers;
using System.Text;
using static SummitChat.Controllers.Send;

namespace SummitChat.Controllers
{

    public class SendToGroup : Controller
    {
        private readonly ApplicationDbContext _context;

        public SendToGroup(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public List<string> phone_numbers = new List<string>();
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
        [HttpPost]
        public async Task SendMessengeToGroup(List<string> usersPhone, string inputToGroup)
        {
            foreach(var phone in usersPhone)
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        string msgUuid = await Post(client, inputToGroup, phone);
                        await AddMessageToDatabase(msgUuid, 1, phone);
                        await Task.Delay(1000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Произошла ошибка: " + ex.Message);
                    }
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
    }

}

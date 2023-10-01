using Microsoft.AspNetCore.Mvc;
using SummitChat.Data;
using System.Net.Http.Headers;
using System.Text;

namespace SummitChat.Controllers
{

    public class AddUser : Controller
    {
        private readonly ApplicationDbContext _context;

        public AddUser(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> AddUserToDatabase(string userName, string userPhone)
        {
            var user = new Models.User
            {
                Name = userName,
                Photo = "url/",
                Phone = userPhone
            };

            _context.Add(user);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Пользователь успешно добавлен." });
        }
        

        [HttpPost]
        public async Task AddUserToChat(string userName, string userPhone)
        {

            Console.WriteLine("имя: " + userName + "\nТелефон:" + userPhone);
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string name = userName;
                    string phone = userPhone;

                    await AddUserToDatabase(userName, userPhone);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка: " + ex.Message);
                }
            }
        }
       
    }
}

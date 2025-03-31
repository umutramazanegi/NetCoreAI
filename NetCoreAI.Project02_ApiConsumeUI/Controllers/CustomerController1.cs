using Microsoft.AspNetCore.Mvc;
using NetCoreAI.Project02_ApiConsumeUI.Dtos;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Text;

namespace NetCoreAI.Project02_ApiConsumeUI.Controllers
{
    public class CustomerController1 : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CustomerController1(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> CustomerList()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("http://localhost:5045/api/Customers");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCustomerDto>>(jsonData);
                return View(values);
            }
            return View();
        }
        [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CreateCustomerDto createCustomerDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createCustomerDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("http://localhost:5045/api/Customers", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("CustomerList");
            }
            return View();
        }
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            // HttpClient oluşturuyorum.
            var client = _httpClientFactory.CreateClient();
            // Belirtilen ID'ye sahip müşteriyi silmek için DELETE isteği gönderiyorum. String interpolation kullanarak URL'yi oluşturuyorum.
            var responseMessage = await client.DeleteAsync($"http://localhost:5045/api/Customers/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                // Başarılı ise müşteri listesine yönlendiriyorum.
                return RedirectToAction("CustomerList");
            }

            // Silme işlemi başarısız olursa ne yapacağımı bilmiyorum. Hata mesajını alıp, loglamam ve kullanıcıya göstermem lazım. Şimdilik listeye geri dönüyorum.
            var errorContent = await responseMessage.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Müşteriyi silerken hata oluştu: {responseMessage.StatusCode} - {errorContent}");
            return RedirectToAction("CustomerList");
        }
    }
}

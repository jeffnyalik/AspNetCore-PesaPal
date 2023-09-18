using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace AngOauth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PesaFlowController : ControllerBase
    {   
        private const string pesaflow_sandbox_url = "https://test.pesaflow.com/PaymentAPI/iframev2.1.php";
        private const string apiClientID = "76";
        private const string secret = "tZSznIZE0/jY98c3/FFemua4LeAs2c7v";
        private const int serviceID = 48766;
        private const string callBackURLONSuccess = "";
        private const string notificationURL = "";
        private readonly HttpClient _httpClient;
        public PesaFlowController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        [HttpPost("secure-hash")]
        public string GenerateSecureHash()
        {
            string datatoHash = "76" + "INVb2040" + "tZSznIZE0/jY98c3/FFemua4LeAs2c7v";
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                byte[] dataToHashBytes = Encoding.UTF8.GetBytes(datatoHash);
                byte[] hashBytes = hmac.ComputeHash(dataToHashBytes);
                string secureHash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
                return secureHash;
            }
        }
        
        [HttpPost("hash")]
        public string GetHash(string text, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key)
;
            byte[] messageBytes = Encoding.UTF8.GetBytes(text);
            using HMACSHA256 hmac = new HMACSHA256(keyBytes);
            byte[] hashBytes = hmac.ComputeHash(messageBytes);

            var hex = string.Concat(hashBytes.Select(x => x.ToString("x2").ToLower()));
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(hex));
        }

        [HttpPost("pesaflow-checkout")]
        public async Task<IActionResult>ProcessOnlineCheckout()
        { 
            // var data = new
            // {
            //     apiClientID = "76",
            //     serviceID = 48766,
            //     billDesc = "Fee Payment",
            //     currency = "USD",
            //     billRefNumber = "INV2308002",
            //     clientMSISDN = "254716431039",
            //     clientName = "John Doe",
            //     clientIDNumber = "20305040",
            //     clientEmail = "webmaster@ca.go.ke",
            //     callBackURLONSuccess = "https://localhost:7099/api/pesaflow/pesaflow-checkout",
            //     amountExpected = "100",
            //     notificationURL = "https://localhost:7099/api/pesaflow/pesaflow-checkout",
            //     secureHash = "YzE0YzYxMmEyMWMwMjk3NWU3NTQyN2U0YmQ4OWNiYmM3ZjExOGUzZjNkOWQwMDZjNmM0ZDgwMzFjMmIxYzA0ZQ==",
            //     format = "JSON",
            //     sendSTK = "true",
            //     PictureURL = ""
            // };

            // var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            // var content = new StringContent(json, Encoding.UTF8, "application/json");


            // try
            // {
            // using (var httpClient = new HttpClient())
            // {
            //     var response = await httpClient.PostAsync(pesaflow_sandbox_url, data);
            //     response.EnsureSuccessStatusCode();

            //     var result = await response.Content.ReadAsStringAsync();
            //     Console.WriteLine(result);
            // }
            // }
            // catch(HttpRequestException ex)
            // {
            //     Console.WriteLine("Error occurred during the HTTP request:");
            //     Console.WriteLine(ex.Message);
            //     // You can also log the exception stack trace if needed
            //     Console.WriteLine(ex.StackTrace);
            //     throw; 
            // }

           
            var requestBody = new 
            {
                apiClientID = "76",
                serviceID = "48766",
                billDesc = "Some random bill description",
                currency = "KES",
                billRefNumber = "INVb2040",
                clientMSISDN = "254716431039",
                clientName = "Nyake",
                clientIDNumber = "27929387",
                clientEmail = "nyakeoloo@gmail.com",
                callBackURLONSuccess = "https://localhost:7099/api/pesaflow/pesaflow-checkout",
                amountExpected = 1.00,
                notificationURL = "https://localhost:7099/api/pesaflow/pesaflow-checkout",
                secureHash = "MTA2Zjk1OTVmYzQwNGYxNzk2ZmVlOWQ3YWZjZjJjNzRkYjkxNDY2OTYwNTY1MTMwOWMxNGFlY2E4ZGFlYWRjZA==",
                
            };

          

            // Send POST REQUEST
            var response = await _httpClient.PostAsJsonAsync(pesaflow_sandbox_url, requestBody);
            if(response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return Ok(responseBody);
            }
            throw new Exception("FAILED TO SEND THE REQUEST TO THE SERVER");
        }

    }
}
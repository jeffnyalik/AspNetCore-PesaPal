using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using AngOauth.ViewModels;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text.Json;

namespace AngOauth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PesaFlowController : ControllerBase
    {   
        private const string pesaflow_sandbox_url = "https://test.pesaflow.com/PaymentAPI/iframev2.1.php";
        private const string apiClientID = "122";
        private const string pesaFlowPaymentStatus = "https://test.pesaflow.com/api/invoice/payment/status";
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
        public async Task<IActionResult> TestCheckout([FromBody] PesaFlowViewModel viewModel)
        {
            try
            {
                var secret = "3viBpycUQGfhyOa4xesx7R8sBlcMIuho";
                var key = "XHwoKTcKMzItNJRv";
                string dataString = $"{viewModel.apiClientID}{viewModel.amountExpected}{viewModel.serviceID}{viewModel.clientIDNumber}{viewModel.currency}{viewModel.billRefNumber}{viewModel.billDesc}{viewModel.clientName}{secret}";

                string secureHash = GenerateSecureHash(dataString, key);

                var responseBody = new
                {
                    viewModel.apiClientID,
                    viewModel.amountExpected,
                    viewModel.serviceID,
                    viewModel.clientIDNumber,
                    viewModel.clientEmail,
                    viewModel.currency,
                    viewModel.billRefNumber,
                    viewModel.clientMSISDN,
                    viewModel.billDesc,
                    viewModel.clientName,
                    viewModel.notificationURL,
                    viewModel.callBackURLOnSuccess,
                    viewModel.format,
                    viewModel.sendSTK,
                    secureHash
                };

                // Send POST REQUEST
                var response = await _httpClient.PostAsJsonAsync(pesaflow_sandbox_url, responseBody);
                Console.WriteLine(response.Content.ReadAsStringAsync());
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    // Console.WriteLine("RESPONSE FROM THE SERVER: " + res);
                    return Ok(res);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("ERROR RESPONSE FROM THE SERVER: " + errorContent);
                    throw new Exception("FAILED TO SEND THE REQUEST TO THE SERVER: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                // Handle the exception or return an appropriate error response
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
        public static string GenerateSecureHash(string data, string key)
        {
            byte[] hashBytes = hmacSHA256(data, key);
            string hexString = BytesToHexString(hashBytes);
            string base64EncodedHash = Base64Encode(hexString);
            return base64EncodedHash;
        }

        static byte[] hmacSHA256(string data, string key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string BytesToHexString(byte[] bytes)
        {
            var builder = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                builder.AppendFormat("{0:x2}", b);
            }
            return builder.ToString();
        }
                
        [HttpPost("pesaflow-checkout")]
        public async Task<IActionResult>ProcessOnlineCheckout([FromBody] PesaFlowViewModel viewModel)
        {   


            var requestBody = new 
            {
                apiClientID = viewModel.apiClientID,
                serviceID = viewModel.serviceID,
                billDesc = viewModel.billDesc,
                currency = viewModel.currency,
                billRefNumber = viewModel.billRefNumber,
                clientMSISDN = viewModel.clientMSISDN,
                clientName = viewModel.clientName,
                clientIDNumber = viewModel.clientIDNumber,
                clientEmail = viewModel.clientEmail,
                callBackURLOnSuccess = viewModel.callBackURLOnSuccess,
                amountExpected = viewModel.amountExpected,
                notificationURL = viewModel.notificationURL,
                secureHash = viewModel.secureHash,
                
            };
          

            // Send POST REQUEST
            var response = await _httpClient.PostAsJsonAsync(pesaflow_sandbox_url, requestBody);
            if(response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("RESPONSE FROM THE SERVER: " + responseBody);
                return Ok(responseBody);
            }
            throw new Exception("FAILED TO SEND THE REQUEST TO THE SERVER");
        }

        /**PESAFLOW IPN OR NOTIFICATATION URL*/
        [HttpPost("notification-url")]
        public async Task<IActionResult>PesaFlowNotify()
        {    
            using (var document = await JsonDocument.ParseAsync(Request.Body))
            {
                // Access specific values from the payment response
                string status = document.RootElement.GetProperty("status").GetString();
                string phoneNumber = document.RootElement.GetProperty("phone_number").GetString();
                string paymentReference = document.RootElement
                    .GetProperty("payment_reference")[0]
                    .GetProperty("payment_reference").GetString();

                // Display the specific values in the console
                Console.WriteLine("Status: " + status);
                Console.WriteLine("Phone Number: " + phoneNumber);
                Console.WriteLine("Payment Reference: " + paymentReference);
            }
            return Ok();
        }
        
        /**Query Payment Status API*/
        [HttpPost("payment-status")]
        public async Task<IActionResult>QueryPaymentStatus([FromBody] PaymentStatus paymentStatus)
        {   
            var key = "XHwoKTcKMzItNJRv";
            var data_string = $"{paymentStatus.api_client_id}{paymentStatus.ref_no}";
            string secureHash = GenerateSecureHash(data_string, key);
            string url = $"https://test.pesaflow.com/api/invoice/payment/status?api_client_id={paymentStatus.api_client_id}&ref_no={paymentStatus.ref_no}&secure_hash={secureHash}";
            var response = await _httpClient.GetAsync(url);
            if(response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return Ok(responseBody);
            }
            throw new Exception("An error has occured");

        }
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using AngOauth.ViewModels;
using System.Net.Http.Headers;

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
            Console.WriteLine("RESPONSE FROM THE SERVER: " + res);
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
                
//         [HttpPost("hash")]
//         public string GetHash(string text, string key)
//         {
//             byte[] keyBytes = Encoding.UTF8.GetBytes(key)
// ;
//             byte[] messageBytes = Encoding.UTF8.GetBytes(text);
//             using HMACSHA256 hmac = new HMACSHA256(keyBytes);
//             byte[] hashBytes = hmac.ComputeHash(messageBytes);

//             var hex = string.Concat(hashBytes.Select(x => x.ToString("x2").ToLower()));
//             return Convert.ToBase64String(Encoding.UTF8.GetBytes(hex));
//         }

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

    }
}
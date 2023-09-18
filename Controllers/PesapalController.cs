using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using AngOauth.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace AngOauth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PesapalController : ControllerBase
    {   
        private readonly HttpClient _httpClient;
        private readonly HttpResponseMessage _response =  new HttpResponseMessage();
        private const string consumerKey = "qkio1BGGYAXTu2JOfm7XSXNruoZsrqEW";
        private const string consumerSecret = "osGQ364R49cXKeOYSpaOnT++rHs=";
        private const string tokenEndpointUrl = "https://cybqa.pesapal.com/pesapalv3/api/Auth/RequestToken";
        private const string ipnURLRegistration = "https://cybqa.pesapal.com/pesapalv3/api/URLSetup/RegisterIPN";
        private const string ipnListURI = "https://cybqa.pesapal.com/pesapalv3/api/URLSetup/GetIpnList";
        private const string submitOrderURL = "https://cybqa.pesapal.com/pesapalv3/api/Transactions/SubmitOrderRequest";

        public PesapalController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [HttpGet("names")]
        public IActionResult GetNames()
        {
            string[] names = new string[]{
                "name1",
                "name2",
                "name3",
                "name4",
                "name5",
                "name6"
            };

            return Ok(names);
        }
        
        [HttpPost("token")]
        public async Task<IActionResult>TokenEndpoint()
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var data = new 
            {
                consumer_key = consumerKey,
                consumer_secret = consumerSecret
            };

            var response  = await _httpClient.PostAsJsonAsync(tokenEndpointUrl, data);
            response.EnsureSuccessStatusCode();

            var response_content = await response.Content.ReadAsStringAsync();
            var token_data = JsonConvert.DeserializeObject<Dictionary<string, string>>(response_content);
            if(token_data.ContainsKey("token"))
            {
                return Ok(token_data["token"]);
            }
            
            throw new Exception("Failed to load access token");

        }
        
        [HttpPost("ipn-register")]
        public async Task<IActionResult>RegisterIPN()
        {
            var token_result = await TokenEndpoint();
            if(token_result is OkObjectResult okResult && okResult.Value is string bearerToken)
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }
           
            var data = new 
            {
                url = "https://localhost:7099/api/pesapal/ipn",
                ipn_notification_type = "GET"
            };

            var response = await _httpClient.PostAsJsonAsync(ipnURLRegistration, data);
            if(response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return Ok(responseData);
            }

            throw new Exception("Failed to register IPN URL endpoint");
        }

        [HttpGet("ipn-reg-endpoints")]
        public async Task<IActionResult>GetIPNEndpoints()
        {
            var token_result = await TokenEndpoint();
            if(token_result is OkObjectResult okResult && okResult.Value is string bearerToken)
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            var response = await _httpClient.GetAsync(ipnListURI);
            return Ok(response.Content.ReadAsStringAsync());
        }

        [HttpPost("submit-order")]
        public async Task<IActionResult>SubmitOrder([FromBody] PesaPalViewModel pesaPalViewModel)
        {
            var token_result = await TokenEndpoint();
            if(token_result is OkObjectResult okResult && okResult.Value is string bearerToken)
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            var data = new  
            {   
                id = pesaPalViewModel.id,
                currency = pesaPalViewModel.currency,
                amount = pesaPalViewModel.amount,
                callback_url = pesaPalViewModel.callback_url,
                notification_id = pesaPalViewModel.notification_id,
                branch = pesaPalViewModel.branch,
                redirect_mode = pesaPalViewModel.redirect_mode,
                description = pesaPalViewModel.description,
                email_address = pesaPalViewModel.email_address,
                billing_address =  new BillingAddress{
                    email_address = pesaPalViewModel.billing_address.email_address,
                    phone_number = pesaPalViewModel.billing_address.phone_number,
                    first_name = pesaPalViewModel.billing_address.first_name,
                    country_code = pesaPalViewModel.billing_address.country_code
                }
            };

            //send POST REQUEST
            // return Ok(data);
            var response = await _httpClient.PostAsJsonAsync(submitOrderURL, data);
            if(response.IsSuccessStatusCode)
            {   
                var responseBody = await response.Content.ReadAsStringAsync();
                return Ok(responseBody);
            }

            throw new Exception("Failed to sumbit the request");

        }
        
        [HttpGet("status")]
        public async Task<IActionResult>GetTransanctionStatus(string orderTrackingId)
        {   
            var token_result = await TokenEndpoint();
            if(token_result is OkObjectResult okResult && okResult.Value is string bearerToken)
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }
            string orderTrackUrl=$"https://cybqa.pesapal.com/pesapalv3/api/Transactions/GetTransactionStatus?orderTrackingId={orderTrackingId}";
            
            var response = await _httpClient.GetAsync(orderTrackUrl);
            if(response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return Ok(responseBody);
            }
            throw new Exception("An error has occured");
        }
    }
}
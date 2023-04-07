using Newtonsoft.Json;
using Pollux.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pollux.Domain.Connection
{
    public class PolarisConnectionHandler
    {
        private const string _apiAdress = "https://aucxis-polaris-test-web.azurewebsites.net/";
        private const string _loginEndpoint = "api/Authentication/Login";
        private const string _movementEndpoint = "apiinternal/ActionConfirmation/Movement";
        private const string _tlmEndpoint = "apiinternal/BleTag/PostBleTLM";

        private static readonly HttpClient _polarisclient = new HttpClient();
        private static readonly PolarisAuthentication _authentication = new PolarisAuthentication() { UserName = "Pollux", Password = "Pollux01" };

        private PolarisAuthenticationResult _authResult;

        public PolarisConnectionHandler()
        {
            _polarisclient.BaseAddress = new Uri(_apiAdress);
            InjectPolarisAuthentication();
        }

        public async Task<bool> SubmitMovement(PolarisMovementResult mr)
        {
            if (mr != null)
            {
                var response = await SendJsonToServer(JsonConvert.SerializeObject(mr), _movementEndpoint);
                if (response != null)
                {
                    return response.IsSuccessStatusCode;
                }
            }
            return false;
        }

        public async Task<bool> SubmitTlm(List<PolarisTLMMessage> messages)
        {
            if (messages != null && messages.Any())
            {
                var j = JsonConvert.SerializeObject(messages);
                var response = await SendJsonToServer(JsonConvert.SerializeObject(messages), _tlmEndpoint);
                if (response != null)
                {
                    return response.IsSuccessStatusCode;
                }
            }
            return false;
        }

        private async Task<HttpResponseMessage> SendJsonToServer(string json, string endpoint)
        {
            try
            {
                var contentString = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _polarisclient.PostAsync(endpoint, contentString);
                return response;
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is WebException)
            {
                Console.WriteLine("###ERROR: Unable to communicate with the Polaris server, is it online?");
                Console.WriteLine(ex.InnerException.Message);
            }catch(Exception ex)
            {

            }
            return null;
        }

        private async void InjectPolarisAuthentication()
        {
            HttpResponseMessage response = await SendJsonToServer(JsonConvert.SerializeObject(_authentication), _loginEndpoint);
            if (response != null && response.IsSuccessStatusCode)
            {
                using (var content = response.Content)
                {
                    var json = await content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<PolarisAuthenticationResult>(json);
                    if (result != null)
                    {
                        _authResult = result;
                        _polarisclient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.JwtToken);
                    }
                }
            }
        }
    }
}

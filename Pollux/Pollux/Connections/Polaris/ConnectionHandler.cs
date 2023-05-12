using Newtonsoft.Json;
using Pollux.Domain.Data;
using Pollux.Domain.Processing;
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
        private readonly PolarisConnectionSettings _settings;
        private readonly HttpClient _polarisclient;
        private readonly PolarisAuthentication _authentication;

        private PolarisAuthenticationResult _authResult;

        public PolarisConnectionHandler()
        {
            _settings = SettingsHandler.GetPolarisConnectionSettings();
            _authentication = new PolarisAuthentication() { UserName = _settings.PolarisUsername, Password = _settings.PolarisPassword };
            _polarisclient = new HttpClient();
            _polarisclient.BaseAddress = new Uri(_settings.ApiAdress);
            InjectPolarisAuthentication();
        }

        public async Task<bool> SubmitMovement(PolarisMovementResult mr)
        {
            if (mr != null)
            {
                var json = JsonConvert.SerializeObject(mr);
                var response = await SendJsonToServer(json, _settings.MovementEndpoint);
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
                var json = JsonConvert.SerializeObject(messages);
                var response = await SendJsonToServer(json, _settings.TlmEndpoint);
                if (response != null)
                {
                    return response.IsSuccessStatusCode;
                }
            }
            return false;
        }

        public async Task<bool> SubmitDeviceInfo(PolarisDeviceInfoResult dir)
        {
            if (dir != null)
            {
                var json = JsonConvert.SerializeObject(dir);
                var response = await SendJsonToServer(json, _settings.DeviceInfoEndpoint);
                if (response != null)
                {
                    return response.IsSuccessStatusCode;
                }
            }
            return false;
        }

        public async Task<bool> SubmitPing(PolarisPingData ping)
        {
            if (ping != null)
            {
                var json = JsonConvert.SerializeObject(ping);
                var response = await SendJsonToServer(json, _settings.PingEndpoint);
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
            HttpResponseMessage response = await SendJsonToServer(JsonConvert.SerializeObject(_authentication), _settings.LoginEndpoint);
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

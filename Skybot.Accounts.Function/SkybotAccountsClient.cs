using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Skybot.Accounts.Function
{
    public class SkybotAccountsClient
    {
        private readonly HttpClient _httpClient;
        private string _token;

        public SkybotAccountsClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> HasAccount(string phoneNumber)
        {
            await CheckToken();

            var requestBody = new
            {
                PhoneNumber = phoneNumber
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");

            var response = await _httpClient.PostAsJsonAsync($"{Settings.SkybotAccountsApiUri}/api/accounts/check", requestBody);

            return response.StatusCode.Equals(HttpStatusCode.OK);
        }

        public async Task<UserAccount> GetAccount(string phoneNumber)
        {
            await CheckToken();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");

            var response = await _httpClient.GetAsync($"{Settings.SkybotAccountsApiUri}/api/accounts/{phoneNumber}");
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<UserAccount>(responseContent);
        }

        public async Task CreateAccount(string phoneNumber)
        {
            await CheckToken();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");

            await _httpClient.PutAsJsonAsync(
                $"{Settings.SkybotAccountsApiUri}/api/accounts/create", new UserAccount
                {
                    PhoneNumber = phoneNumber
                });
        }

        private async Task CheckToken()
        {
            if (string.IsNullOrEmpty(_token))
            {
                await RequestToken();
            }
        }

        private async Task RequestToken()
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"client_id", Settings.SkybotAuthClientId },
                {"client_secret", Settings.SkybotAuthClientSecret },
                {"grant_type", "client_credentials" }
            });

            var response = await _httpClient.PostAsync($"{Settings.SkybotAuthUri}/connect/token", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var deserializedContent = JsonConvert.DeserializeObject<dynamic>(responseContent);

            _token = deserializedContent.access_token;
        }
    }
}

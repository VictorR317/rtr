using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CipherUnlockPro
{
    public class BasicHttpManager
    {
        private static readonly string API_URL = "https://www.cipherunlock.xyz/register.php";
        private static BasicHttpManager _instance;
        private static readonly object _lock = new object();

        public static BasicHttpManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new BasicHttpManager();
                }
                return _instance;
            }
        }

        private BasicHttpManager() { }

        public async Task<RegistrationResponse> RegisterUserAsync(string username, string email, string password)
        {
            try
            {
                using (var handler = new HttpClientHandler())
                {
                    handler.AllowAutoRedirect = true;
                    handler.MaxAutomaticRedirections = 10;

                    using (var client = new HttpClient(handler))
                    {
                        client.Timeout = TimeSpan.FromSeconds(30);
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                        client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");

                        var requestData = new
                        {
                            username = username,
                            email = email,
                            password = password
                        };

                        string json = JsonConvert.SerializeObject(requestData);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync(API_URL, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            return new RegistrationResponse
                            {
                                Success = false,
                                Error = $"HTTP Error: {response.StatusCode}",
                                Message = response.ReasonPhrase
                            };
                        }

                        string responseText = await response.Content.ReadAsStringAsync();

                        if (string.IsNullOrEmpty(responseText) || !responseText.TrimStart().StartsWith("{"))
                        {
                            return new RegistrationResponse
                            {
                                Success = false,
                                Error = "Invalid response format",
                                Message = $"Received: {responseText.Substring(0, Math.Min(100, responseText.Length))}"
                            };
                        }

                        var result = JsonConvert.DeserializeObject<ApiResponse>(responseText);

                        return new RegistrationResponse
                        {
                            Success = result?.Success ?? false,
                            Message = result?.Message ?? "Unknown error",
                            Error = result?.Success == false ? result?.Message : null
                        };
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                return new RegistrationResponse
                {
                    Success = false,
                    Error = "Connection error",
                    Message = httpEx.Message
                };
            }
            catch (TaskCanceledException)
            {
                return new RegistrationResponse
                {
                    Success = false,
                    Error = "Timeout",
                    Message = "Server took too long to respond"
                };
            }
            catch (JsonException)
            {
                return new RegistrationResponse
                {
                    Success = false,
                    Error = "Invalid JSON response",
                    Message = "Server returned invalid JSON"
                };
            }
            catch (Exception ex)
            {
                return new RegistrationResponse
                {
                    Success = false,
                    Error = "Unexpected error",
                    Message = ex.Message
                };
            }
        }
    }

    public class ApiResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }

    public class RegistrationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
    }
}
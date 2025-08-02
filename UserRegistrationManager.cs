using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CipherUnlockPro
{
    /// <summary>
    /// Manejador simple para registro de usuarios
    /// </summary>
    public class UserRegistrationManager
    {
        private static readonly string REGISTRATION_API_URL = "https://www.cipherunlock.xyz/register.php";
        private static UserRegistrationManager _registrationInstance;
        private static readonly object _registrationLock = new object();

        public static UserRegistrationManager Instance
        {
            get
            {
                lock (_registrationLock)
                {
                    if (_registrationInstance == null)
                        _registrationInstance = new UserRegistrationManager();
                }
                return _registrationInstance;
            }
        }

        private UserRegistrationManager() { }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        public async Task<SimpleUserRegistrationResult> RegisterUserAsync(string username, string email, string password)
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

                        var response = await client.PostAsync(REGISTRATION_API_URL, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            return new SimpleUserRegistrationResult
                            {
                                Success = false,
                                Error = $"HTTP Error: {response.StatusCode}",
                                Message = response.ReasonPhrase ?? "Unknown HTTP error"
                            };
                        }

                        string responseText = await response.Content.ReadAsStringAsync();

                        if (string.IsNullOrEmpty(responseText) || !responseText.TrimStart().StartsWith("{"))
                        {
                            return new SimpleUserRegistrationResult
                            {
                                Success = false,
                                Error = "Invalid response format",
                                Message = $"Received: {responseText.Substring(0, Math.Min(100, responseText.Length))}"
                            };
                        }

                        var result = JsonConvert.DeserializeObject<SimpleApiResult>(responseText);

                        return new SimpleUserRegistrationResult
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
                return new SimpleUserRegistrationResult
                {
                    Success = false,
                    Error = "Connection error",
                    Message = httpEx.Message
                };
            }
            catch (TaskCanceledException)
            {
                return new SimpleUserRegistrationResult
                {
                    Success = false,
                    Error = "Timeout",
                    Message = "Server took too long to respond"
                };
            }
            catch (JsonException)
            {
                return new SimpleUserRegistrationResult
                {
                    Success = false,
                    Error = "Invalid JSON response",
                    Message = "Server returned invalid JSON"
                };
            }
            catch (Exception ex)
            {
                return new SimpleUserRegistrationResult
                {
                    Success = false,
                    Error = "Unexpected error",
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Test de conectividad
        /// </summary>
        public static async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var handler = new HttpClientHandler())
                {
                    handler.AllowAutoRedirect = true;

                    using (var client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                        client.Timeout = TimeSpan.FromSeconds(10);

                        var testData = new { test = "connection" };
                        string jsonData = JsonConvert.SerializeObject(testData);
                        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync(REGISTRATION_API_URL, content);
                        return response.IsSuccessStatusCode;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Respuesta de la API
    /// </summary>
    public class SimpleApiResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }

    /// <summary>
    /// Resultado del registro
    /// </summary>
    public class SimpleUserRegistrationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
    }
}
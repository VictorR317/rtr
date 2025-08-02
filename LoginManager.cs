using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace CipherUnlockPro
{
    public class LoginManager
    {
        private static readonly string LOGIN_API_URL = "https://www.cipherunlock.xyz/api/login.php";
        private static LoginManager _instance;
        private static readonly object _lock = new object();

        public static LoginManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new LoginManager();
                }
                return _instance;
            }
        }

        private LoginManager() { }

        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            var debugInfo = new StringBuilder();

            try
            {
                debugInfo.AppendLine($"=== LOGIN DEBUG INFO ===");
                debugInfo.AppendLine($"Usuario: {username}");
                debugInfo.AppendLine($"URL: {LOGIN_API_URL}");
                debugInfo.AppendLine($"Timestamp: {DateTime.Now}");
                debugInfo.AppendLine();

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("User-Agent", "CipherUnlockPro/1.0");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    var requestData = new
                    {
                        action = "login",
                        username = username,
                        password = password,
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    string json = JsonConvert.SerializeObject(requestData, Formatting.Indented);
                    debugInfo.AppendLine($"JSON enviado:");
                    debugInfo.AppendLine(json);
                    debugInfo.AppendLine();

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(LOGIN_API_URL, content);
                    string responseText = await response.Content.ReadAsStringAsync();

                    debugInfo.AppendLine($"Status Code: {response.StatusCode}");
                    debugInfo.AppendLine($"Content-Type: {response.Content.Headers.ContentType}");
                    debugInfo.AppendLine($"Respuesta completa: {responseText}");
                    debugInfo.AppendLine();

                    // Validar respuesta
                    var validacion = ValidarRespuestaJSON(responseText, debugInfo);

                    if (!validacion.EsValida)
                    {
                        MessageBox.Show(debugInfo.ToString(), "DEBUG - Login Error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return new LoginResult
                        {
                            IsSuccess = false,
                            ErrorMessage = validacion.MensajeError,
                            Username = username
                        };
                    }

                    // Parsear respuesta JSON
                    LoginApiResponse result = null;
                    try
                    {
                        string cleanResponse = responseText.Trim().TrimStart('\uFEFF', '\u200B');

                        // Detectar formato español o inglés
                        if (cleanResponse.Contains("\"Éxito\"") || cleanResponse.Contains("\"Mensaje\""))
                        {
                            debugInfo.AppendLine("Detectado formato de login en ESPAÑOL");
                            var spanishResult = JsonConvert.DeserializeObject<SpanishLoginResponse>(cleanResponse);
                            result = new LoginApiResponse
                            {
                                Success = spanishResult.Éxito,
                                Message = spanishResult.Mensaje,
                                Error = spanishResult.Error,
                                SessionToken = spanishResult.Token_Sesion,
                                UserData = spanishResult.Datos_Usuario
                            };
                        }
                        else if (cleanResponse.Contains("\"Success\"") || cleanResponse.Contains("\"Message\""))
                        {
                            debugInfo.AppendLine("Detectado formato de login en INGLÉS");
                            result = JsonConvert.DeserializeObject<LoginApiResponse>(cleanResponse);
                        }
                        else
                        {
                            throw new Exception("Formato de respuesta de login no reconocido");
                        }

                        debugInfo.AppendLine($"✅ Login JSON parseado exitosamente:");
                        debugInfo.AppendLine($"  Success: {result.Success}");
                        debugInfo.AppendLine($"  Message: {result.Message}");
                        debugInfo.AppendLine($"  Session Token: {!string.IsNullOrEmpty(result.SessionToken)}");
                    }
                    catch (JsonException jsonEx)
                    {
                        debugInfo.AppendLine($"❌ Error parseando JSON de login: {jsonEx.Message}");

                        MessageBox.Show(debugInfo.ToString(), "DEBUG - Login JSON Error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return new LoginResult
                        {
                            IsSuccess = false,
                            ErrorMessage = $"Error parseando respuesta: {jsonEx.Message}",
                            Username = username
                        };
                    }

                    // Mostrar debug exitoso
                    debugInfo.AppendLine("=== LOGIN PROCESO COMPLETADO ===");
                    MessageBox.Show(debugInfo.ToString(), "DEBUG - Login Respuesta",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return new LoginResult
                    {
                        IsSuccess = result?.Success ?? false,
                        ErrorMessage = result?.Error,
                        SuccessMessage = result?.Message,
                        Username = username,
                        SessionToken = result?.SessionToken,
                        UserData = result?.UserData
                    };
                }
            }
            catch (Exception ex)
            {
                debugInfo.AppendLine($"❌ Error en login: {ex.Message}");
                MessageBox.Show(debugInfo.ToString(), "DEBUG - Login Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);

                return new LoginResult
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                    Username = username
                };
            }
        }

        private ValidacionRespuesta ValidarRespuestaJSON(string responseText, StringBuilder debugInfo)
        {
            if (string.IsNullOrWhiteSpace(responseText))
            {
                debugInfo.AppendLine("❌ Respuesta de login vacía del servidor");
                return new ValidacionRespuesta
                {
                    EsValida = false,
                    MensajeError = "El servidor devolvió una respuesta de login vacía"
                };
            }

            var trimmedResponse = responseText.TrimStart();

            // Verificar respuesta HTML
            if (trimmedResponse.StartsWith("<"))
            {
                debugInfo.AppendLine("❌ El servidor de login devolvió HTML en lugar de JSON");
                debugInfo.AppendLine($"HTML recibido: {responseText.Substring(0, Math.Min(300, responseText.Length))}");

                return new ValidacionRespuesta
                {
                    EsValida = false,
                    MensajeError = "El servidor de login devolvió HTML. Verifica la URL del endpoint de login."
                };
            }

            // Verificar errores PHP
            if (trimmedResponse.StartsWith("Error") ||
                trimmedResponse.StartsWith("Fatal error") ||
                trimmedResponse.StartsWith("Warning"))
            {
                debugInfo.AppendLine("❌ Error PHP en servidor de login");
                debugInfo.AppendLine($"Error: {responseText}");

                return new ValidacionRespuesta
                {
                    EsValida = false,
                    MensajeError = $"Error PHP en login: {responseText.Substring(0, Math.Min(100, responseText.Length))}"
                };
            }

            // Verificar que parezca JSON
            if (!trimmedResponse.StartsWith("{") && !trimmedResponse.StartsWith("["))
            {
                debugInfo.AppendLine("❌ La respuesta de login no parece JSON");
                debugInfo.AppendLine($"Respuesta: '{responseText.Substring(0, Math.Min(50, responseText.Length))}'");

                return new ValidacionRespuesta
                {
                    EsValida = false,
                    MensajeError = $"Formato de login inválido: {trimmedResponse.Substring(0, Math.Min(50, trimmedResponse.Length))}"
                };
            }

            debugInfo.AppendLine("✅ Respuesta de login parece JSON válido");
            return new ValidacionRespuesta { EsValida = true };
        }

        public async Task<bool> TestLoginConnectionAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("User-Agent", "CipherUnlockPro/1.0");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    var testData = new
                    {
                        action = "test_login",
                        test = true,
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    string json = JsonConvert.SerializeObject(testData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(LOGIN_API_URL, content);
                    string responseText = await response.Content.ReadAsStringAsync();

                    var debugMessage = $"=== TEST LOGIN CONNECTION ===\n\n" +
                                     $"URL: {LOGIN_API_URL}\n" +
                                     $"Status: {response.StatusCode}\n" +
                                     $"Success: {response.IsSuccessStatusCode}\n\n" +
                                     $"JSON enviado:\n{json}\n\n" +
                                     $"Respuesta:\n{responseText}";

                    MessageBox.Show(debugMessage, "Test Login Connection",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Test login falló:\n\nError: {ex.Message}\n\nURL: {LOGIN_API_URL}",
                              "Error Test Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }

    // Clases para respuestas de login
    public class SpanishLoginResponse
    {
        [JsonProperty("Éxito")]
        public bool Éxito { get; set; }

        [JsonProperty("Mensaje")]
        public string Mensaje { get; set; }

        [JsonProperty("Error")]
        public string Error { get; set; }

        [JsonProperty("Token_Sesion")]
        public string Token_Sesion { get; set; }

        [JsonProperty("Datos_Usuario")]
        public object Datos_Usuario { get; set; }

        [JsonProperty("Debug_Timestamp")]
        public string Debug_Timestamp { get; set; }
    }

    public class LoginApiResponse
    {
        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("Error")]
        public string Error { get; set; }

        [JsonProperty("SessionToken")]
        public string SessionToken { get; set; }

        [JsonProperty("UserData")]
        public object UserData { get; set; }

        [JsonProperty("Debug_Timestamp")]
        public string DebugTimestamp { get; set; }
    }

    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string Username { get; set; }
        public string SessionToken { get; set; }
        public object UserData { get; set; }
    }

    public class ValidacionRespuesta
    {
        public bool EsValida { get; set; }
        public string MensajeError { get; set; }
    }
}
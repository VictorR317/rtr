using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace CipherUnlockPro
{
    public class DebugUserManager
    {
        private static readonly string API_URL = "https://cipherunlock.xyz/api/register.php";
        private static DebugUserManager _instance;
        private static readonly object _lock = new object();

        public static DebugUserManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new DebugUserManager();
                }
                return _instance;
            }
        }

        private DebugUserManager() { }

        public async Task<UserRegistrationResult> RegisterUserAsync(string username, string email, string password)
        {
            var debugInfo = new StringBuilder();

            try
            {
                debugInfo.AppendLine($"=== INFORMACIÓN DE DEBUG ===");
                debugInfo.AppendLine($"Usuario: {username}");
                debugInfo.AppendLine($"Email: {email}");
                debugInfo.AppendLine($"URL: {API_URL}");
                debugInfo.AppendLine($"Timestamp: {DateTime.Now}");
                debugInfo.AppendLine();

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("User-Agent", "CipherUnlockPro/1.0");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    // CAMBIO: Enviar datos en el formato que tu servidor espera
                    var requestData = new
                    {
                        username = username,
                        email = email,
                        password = password,
                        action = "register"  // Agregar acción para el servidor
                    };

                    string json = JsonConvert.SerializeObject(requestData, Formatting.Indented);
                    debugInfo.AppendLine($"JSON enviado:");
                    debugInfo.AppendLine(json);
                    debugInfo.AppendLine();

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(API_URL, content);
                    string responseText = await response.Content.ReadAsStringAsync();

                    debugInfo.AppendLine($"Status Code: {response.StatusCode}");
                    debugInfo.AppendLine($"Content-Type: {response.Content.Headers.ContentType}");
                    debugInfo.AppendLine($"Respuesta completa: {responseText}");
                    debugInfo.AppendLine();

                    // CAMBIO CRÍTICO: Detectar si es formato español o inglés
                    ApiResponseBase result = null;

                    try
                    {
                        string cleanResponse = responseText.Trim().TrimStart('\uFEFF', '\u200B');

                        // Intentar parsear como respuesta en español primero
                        if (cleanResponse.Contains("\"Éxito\"") || cleanResponse.Contains("\"Mensaje\""))
                        {
                            debugInfo.AppendLine("Detectado formato de respuesta en ESPAÑOL");
                            var spanishResult = JsonConvert.DeserializeObject<SpanishApiResponse>(cleanResponse);
                            result = new ApiResponseBase
                            {
                                Success = spanishResult.Éxito,
                                Message = spanishResult.Mensaje,
                                Error = spanishResult.Error,
                                DebugTimestamp = spanishResult.Debug_Timestamp
                            };
                        }
                        else if (cleanResponse.Contains("\"Success\"") || cleanResponse.Contains("\"Message\""))
                        {
                            debugInfo.AppendLine("Detectado formato de respuesta en INGLÉS");
                            var englishResult = JsonConvert.DeserializeObject<EnglishApiResponse>(cleanResponse);
                            result = new ApiResponseBase
                            {
                                Success = englishResult.Success,
                                Message = englishResult.Message,
                                Error = englishResult.Error,
                                DebugTimestamp = englishResult.Debug_Timestamp
                            };
                        }
                        else
                        {
                            throw new Exception("Formato de respuesta no reconocido");
                        }

                        debugInfo.AppendLine($"✅ JSON parseado exitosamente:");
                        debugInfo.AppendLine($"  Success/Éxito: {result.Success}");
                        debugInfo.AppendLine($"  Message/Mensaje: {result.Message}");
                        debugInfo.AppendLine($"  Error: {result.Error}");
                    }
                    catch (JsonException jsonEx)
                    {
                        debugInfo.AppendLine($"❌ Error parseando JSON: {jsonEx.Message}");

                        // Mostrar debug completo
                        MessageBox.Show(debugInfo.ToString(), "DEBUG - Error JSON",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return new UserRegistrationResult
                        {
                            IsSuccess = false,
                            Message = $"Error parseando respuesta JSON: {jsonEx.Message}",
                            Username = username,
                            Email = email
                        };
                    }

                    // Mostrar debug exitoso
                    debugInfo.AppendLine("=== PROCESO COMPLETADO ===");
                    MessageBox.Show(debugInfo.ToString(), "DEBUG - Respuesta Recibida",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return new UserRegistrationResult
                    {
                        IsSuccess = result?.Success ?? false,
                        Message = result?.Message ?? result?.Error ?? "Respuesta desconocida",
                        Username = username,
                        Email = email
                    };
                }
            }
            catch (Exception ex)
            {
                debugInfo.AppendLine($"❌ Error: {ex.Message}");
                MessageBox.Show(debugInfo.ToString(), "DEBUG - Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);

                return new UserRegistrationResult
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Username = username,
                    Email = email
                };
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(15);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("User-Agent", "CipherUnlockPro/1.0");
                    client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
                    client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

                    // Test con estructura mejorada
                    var testData = new
                    {
                        action = "test",
                        username = "test_user",
                        email = "test@test.com",
                        password = "test123",
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        client_info = "CipherUnlockPro C# Client"
                    };

                    string json = JsonConvert.SerializeObject(testData, Formatting.Indented);

                    // IMPORTANTE: Crear StringContent con encoding explícito
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Log de headers que se van a enviar
                    var requestInfo = new StringBuilder();
                    requestInfo.AppendLine("=== INFORMACIÓN DE SOLICITUD ===");
                    requestInfo.AppendLine($"Método: POST");
                    requestInfo.AppendLine($"URL: {API_URL}");
                    requestInfo.AppendLine($"Content-Type: application/json; charset=utf-8");
                    requestInfo.AppendLine("Headers:");
                    foreach (var header in client.DefaultRequestHeaders)
                    {
                        requestInfo.AppendLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                    }
                    requestInfo.AppendLine();
                    requestInfo.AppendLine("JSON ENVIADO:");
                    requestInfo.AppendLine(json);
                    requestInfo.AppendLine();

                    var response = await client.PostAsync(API_URL, content);
                    string responseText = await response.Content.ReadAsStringAsync();

                    requestInfo.AppendLine("=== INFORMACIÓN DE RESPUESTA ===");
                    requestInfo.AppendLine($"Status Code: {response.StatusCode} ({(int)response.StatusCode})");
                    requestInfo.AppendLine($"Reason Phrase: {response.ReasonPhrase}");
                    requestInfo.AppendLine($"Content-Type: {response.Content.Headers.ContentType}");
                    requestInfo.AppendLine($"Content-Length: {response.Content.Headers.ContentLength}");
                    requestInfo.AppendLine();
                    requestInfo.AppendLine("Headers de Respuesta:");
                    foreach (var header in response.Headers)
                    {
                        requestInfo.AppendLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                    }
                    requestInfo.AppendLine();
                    requestInfo.AppendLine("RESPUESTA COMPLETA:");
                    requestInfo.AppendLine(responseText);
                    requestInfo.AppendLine();
                    requestInfo.AppendLine("=== ANÁLISIS ===");
                    requestInfo.AppendLine($"- Éxito HTTP: {response.IsSuccessStatusCode}");
                    requestInfo.AppendLine($"- Longitud: {responseText.Length} caracteres");
                    requestInfo.AppendLine($"- Formato: {(responseText.Contains("Éxito") ? "Español" : responseText.Contains("Success") ? "Inglés" : "Desconocido")}");
                    requestInfo.AppendLine($"- Es JSON válido: {(responseText.TrimStart().StartsWith("{") ? "Sí" : "No")}");

                    MessageBox.Show(requestInfo.ToString(), "Test de Conexión Detallado",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                var errorInfo = $"=== ERROR EN TEST DE CONEXIÓN ===\n\n" +
                               $"URL: {API_URL}\n" +
                               $"Error: {ex.Message}\n" +
                               $"Tipo: {ex.GetType().Name}\n\n";

                if (ex.InnerException != null)
                {
                    errorInfo += $"Inner Exception: {ex.InnerException.Message}\n\n";
                }

                errorInfo += $"Stack Trace:\n{ex.StackTrace}";

                MessageBox.Show(errorInfo, "Error en Test de Conexión",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }

    // NUEVAS CLASES: Soporte para respuestas en español e inglés

    // Para respuestas en español como la que recibiste
    public class SpanishApiResponse
    {
        [JsonProperty("Éxito")]
        public bool Éxito { get; set; }

        [JsonProperty("Mensaje")]
        public string Mensaje { get; set; }

        [JsonProperty("Error")]
        public string Error { get; set; }

        [JsonProperty("Debug_Timestamp")]
        public string Debug_Timestamp { get; set; }

        [JsonProperty("Datos")]
        public object Datos { get; set; }
    }

    // Para respuestas en inglés (si cambias el servidor)
    public class EnglishApiResponse
    {
        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("Error")]
        public string Error { get; set; }

        [JsonProperty("Debug_Timestamp")]
        public string Debug_Timestamp { get; set; }

        [JsonProperty("Data")]
        public object Data { get; set; }
    }

    // Clase base unificada
    public class ApiResponseBase
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
        public string DebugTimestamp { get; set; }
        public object Data { get; set; }
    }

    public class UserRegistrationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
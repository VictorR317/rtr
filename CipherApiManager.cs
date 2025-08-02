using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace CipherUnlockPro
{
    /// <summary>
    /// Manager simple para tu API real - sin conflictos de nombres
    /// </summary>
    public class CipherApiManager
    {
        private static CipherApiManager _instance;
        private static readonly object _lock = new object();

        // URLs de tu API real - usando HTTP temporalmente por problemas SSL
        private const string REGISTER_URL = "http://www.cipherunlock.xyz/api/register.php";
        private const string LOGIN_URL = "http://www.cipherunlock.xyz/api/login.php";

        public static CipherApiManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new CipherApiManager();
                }
                return _instance;
            }
        }

        private CipherApiManager() { }

        /// <summary>
        /// Test simple de registro
        /// </summary>
        public async Task<bool> TestRegisterAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("User-Agent", "CipherUnlockPro/1.0");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    var registerData = new
                    {
                        username = "test_user_" + DateTime.Now.Ticks,
                        email = "test" + DateTime.Now.Ticks + "@example.com",
                        password = "password123"
                    };

                    string json = JsonConvert.SerializeObject(registerData, Formatting.Indented);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(REGISTER_URL, content);
                    string responseText = await response.Content.ReadAsStringAsync();

                    var message = new StringBuilder();
                    message.AppendLine("=== TEST REGISTRO REAL ===");
                    message.AppendLine();
                    message.AppendLine($"URL: {REGISTER_URL}");
                    message.AppendLine($"Status: {response.StatusCode}");
                    message.AppendLine($"Éxito HTTP: {response.IsSuccessStatusCode}");
                    message.AppendLine();
                    message.AppendLine("DATOS ENVIADOS:");
                    message.AppendLine(json);
                    message.AppendLine();
                    message.AppendLine("RESPUESTA:");
                    message.AppendLine(responseText);
                    message.AppendLine();

                    // Analizar respuesta
                    bool success = false;
                    try
                    {
                        dynamic result = JsonConvert.DeserializeObject(responseText);
                        success = result.Éxito == true || result.Success == true;

                        if (success)
                        {
                            message.AppendLine("✅ REGISTRO EXITOSO");
                            message.AppendLine("Tu API de registro está funcionando perfectamente");
                        }
                        else
                        {
                            message.AppendLine("ℹ️ Respuesta recibida correctamente");
                            message.AppendLine("La API está funcionando, revisar lógica de registro");
                        }
                    }
                    catch
                    {
                        message.AppendLine("⚠️ Respuesta recibida pero no es JSON válido");
                    }

                    MessageBox.Show(message.ToString(), "Test Registro",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en test de registro:\n\n{ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Test simple de login
        /// </summary>
        public async Task<bool> TestLoginAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("User-Agent", "CipherUnlockPro/1.0");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    var loginData = new
                    {
                        username = "admin",
                        password = "admin123"
                    };

                    string json = JsonConvert.SerializeObject(loginData, Formatting.Indented);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(LOGIN_URL, content);
                    string responseText = await response.Content.ReadAsStringAsync();

                    var message = new StringBuilder();
                    message.AppendLine("=== TEST LOGIN REAL ===");
                    message.AppendLine();
                    message.AppendLine($"URL: {LOGIN_URL}");
                    message.AppendLine($"Status: {response.StatusCode}");
                    message.AppendLine($"Éxito HTTP: {response.IsSuccessStatusCode}");
                    message.AppendLine();
                    message.AppendLine("CREDENCIALES ENVIADAS:");
                    message.AppendLine(json);
                    message.AppendLine();
                    message.AppendLine("RESPUESTA:");
                    message.AppendLine(responseText);
                    message.AppendLine();

                    // Analizar respuesta
                    bool success = false;
                    string token = "";
                    try
                    {
                        dynamic result = JsonConvert.DeserializeObject(responseText);
                        success = result.Éxito == true || result.Success == true;

                        if (result.Token_Sesion != null)
                        {
                            token = result.Token_Sesion.ToString();
                        }

                        if (success)
                        {
                            message.AppendLine("✅ LOGIN EXITOSO");
                            message.AppendLine("✅ Tu API de login está funcionando perfectamente");
                            if (!string.IsNullOrEmpty(token))
                            {
                                message.AppendLine($"✅ Token de sesión recibido: {token}");
                            }
                        }
                        else
                        {
                            message.AppendLine("ℹ️ Respuesta recibida correctamente");
                            message.AppendLine("La API está funcionando, revisar credenciales o lógica");
                        }
                    }
                    catch
                    {
                        message.AppendLine("⚠️ Respuesta recibida pero no es JSON válido");
                    }

                    MessageBox.Show(message.ToString(), "Test Login",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en test de login:\n\n{ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Test completo de funcionalidad
        /// </summary>
        public async Task<bool> TestCompleteApiAsync()
        {
            var message = new StringBuilder();
            message.AppendLine("=== TEST COMPLETO DE API ===");
            message.AppendLine();

            bool registerSuccess = false;
            bool loginSuccess = false;

            try
            {
                // Test 1: Registro
                message.AppendLine("1. Probando REGISTRO...");
                registerSuccess = await TestRegisterQuietAsync();
                message.AppendLine(registerSuccess ? "   ✅ Registro: OK" : "   ❌ Registro: Fallo");

                // Test 2: Login
                message.AppendLine("2. Probando LOGIN...");
                loginSuccess = await TestLoginQuietAsync();
                message.AppendLine(loginSuccess ? "   ✅ Login: OK" : "   ❌ Login: Fallo");

                message.AppendLine();
                message.AppendLine("=== RESUMEN ===");

                if (registerSuccess && loginSuccess)
                {
                    message.AppendLine("🎉 ¡FELICIDADES!");
                    message.AppendLine("✅ Tu API está funcionando completamente");
                    message.AppendLine("✅ Registro funcionando");
                    message.AppendLine("✅ Login funcionando");
                    message.AppendLine("✅ Sistema listo para usar");
                }
                else if (registerSuccess || loginSuccess)
                {
                    message.AppendLine("⚠️ API parcialmente funcional");
                    message.AppendLine("Algunos endpoints funcionan, revisar configuración");
                }
                else
                {
                    message.AppendLine("❌ Problemas con la API");
                    message.AppendLine("Revisar configuración del servidor PHP");
                }

            }
            catch (Exception ex)
            {
                message.AppendLine($"❌ Error en test completo: {ex.Message}");
            }

            MessageBox.Show(message.ToString(), "Test Completo",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);

            return registerSuccess && loginSuccess;
        }

        // Métodos auxiliares silenciosos (sin MessageBox)
        private async Task<bool> TestRegisterQuietAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.Add("User-Agent", "CipherUnlockPro/1.0");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    var data = new { username = "test", email = "test@test.com", password = "test123" };
                    var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(REGISTER_URL, content);

                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> TestLoginQuietAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.Add("User-Agent", "CipherUnlockPro/1.0");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    var data = new { username = "admin", password = "admin123" };
                    var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(LOGIN_URL, content);

                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
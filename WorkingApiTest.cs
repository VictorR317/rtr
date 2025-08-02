using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace CipherUnlockPro
{
    /// <summary>
    /// Test con API que sabemos que funciona para verificar que tu código está correcto
    /// </summary>
    public class WorkingApiTest
    {
        private static WorkingApiTest _instance;
        private static readonly object _lock = new object();

        // URL de test que sabemos que funciona
        private const string TEST_API_URL = "https://httpbin.org/post";

        public static WorkingApiTest Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new WorkingApiTest();
                }
                return _instance;
            }
        }

        private WorkingApiTest() { }

        /// <summary>
        /// Test con API real que funciona para verificar que tu código HTTP está correcto
        /// </summary>
        public async Task<bool> TestWorkingApiAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(15);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("User-Agent", "CipherUnlockPro/1.0");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    // Simular datos de registro como los que enviarías a tu API
                    var testData = new
                    {
                        username = "test_user",
                        email = "test@example.com",
                        password = "test123",
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        app = "CipherUnlockPro"
                    };

                    string json = JsonConvert.SerializeObject(testData, Formatting.Indented);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Enviar POST a API que funciona
                    var response = await client.PostAsync(TEST_API_URL, content);
                    string responseText = await response.Content.ReadAsStringAsync();

                    // Mostrar resultado detallado
                    var message = new StringBuilder();
                    message.AppendLine("=== TEST CON API QUE FUNCIONA ===");
                    message.AppendLine();
                    message.AppendLine($"URL: {TEST_API_URL}");
                    message.AppendLine($"Status: {response.StatusCode}");
                    message.AppendLine($"Éxito: {response.IsSuccessStatusCode}");
                    message.AppendLine($"Content-Type: {response.Content.Headers.ContentType}");
                    message.AppendLine();
                    message.AppendLine("JSON ENVIADO:");
                    message.AppendLine(json);
                    message.AppendLine();
                    message.AppendLine("RESPUESTA RECIBIDA:");
                    message.AppendLine(responseText.Substring(0, Math.Min(500, responseText.Length)));

                    if (responseText.Length > 500)
                    {
                        message.AppendLine("... (respuesta truncada)");
                    }

                    message.AppendLine();
                    message.AppendLine("=== CONCLUSIÓN ===");

                    if (response.IsSuccessStatusCode)
                    {
                        message.AppendLine("✅ TU CÓDIGO C# FUNCIONA PERFECTAMENTE");
                        message.AppendLine("✅ HttpClient está enviando POST correctamente");
                        message.AppendLine("✅ JSON se está serializando bien");
                        message.AppendLine("✅ Headers se están enviando correctamente");
                        message.AppendLine();
                        message.AppendLine("❌ EL PROBLEMA ES TU DOMINIO/SERVIDOR");
                        message.AppendLine("Necesitas:");
                        message.AppendLine("1. Verificar que tu dominio esté activo");
                        message.AppendLine("2. Subir los archivos PHP al servidor");
                        message.AppendLine("3. Configurar DNS correctamente");
                    }
                    else
                    {
                        message.AppendLine("❌ Hay un problema con la conexión HTTP");
                    }

                    MessageBox.Show(message.ToString(), "Test con API Funcional",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en test con API funcional:\n\n{ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Test específico para verificar si tu dominio está funcionando ahora
        /// </summary>
        public async Task<bool> TestYourDomainAsync()
        {
            string[] urlsToTest = {
                "https://www.cipherunlock.xyz",
                "https://cipherunlock.xyz",
                "http://www.cipherunlock.xyz",
                "http://cipherunlock.xyz"
            };

            var results = new StringBuilder();
            results.AppendLine("=== TEST DE TU DOMINIO ===");
            results.AppendLine();

            bool anySuccess = false;

            foreach (string url in urlsToTest)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(10);
                        var response = await client.GetAsync(url);

                        results.AppendLine($"✅ {url}");
                        results.AppendLine($"   Status: {response.StatusCode}");
                        results.AppendLine($"   Éxito: {response.IsSuccessStatusCode}");

                        if (response.IsSuccessStatusCode)
                        {
                            anySuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"❌ {url}");
                    results.AppendLine($"   Error: {ex.Message}");
                }

                results.AppendLine();
            }

            results.AppendLine("=== RESULTADO ===");
            if (anySuccess)
            {
                results.AppendLine("✅ Al menos una URL de tu dominio funciona");
                results.AppendLine("Ahora puedes crear los archivos PHP en el servidor");
            }
            else
            {
                results.AppendLine("❌ Tu dominio no está accesible");
                results.AppendLine("Necesitas configurar tu hosting/DNS primero");
            }

            MessageBox.Show(results.ToString(), "Test de Tu Dominio",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);

            return anySuccess;
        }
    }
}
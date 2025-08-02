using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CipherUnlockPro
{
    /// <summary>
    /// Test simple de conectividad sin dependencias complejas
    /// </summary>
    public class SimpleConnectionTest
    {
        private static SimpleConnectionTest _instance;
        private static readonly object _lock = new object();

        public static SimpleConnectionTest Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new SimpleConnectionTest();
                }
                return _instance;
            }
        }

        private SimpleConnectionTest() { }

        /// <summary>
        /// Test básico que debería funcionar sin errores
        /// </summary>
        public async Task<bool> TestBasicConnectionAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);

                    // Test 1: Google (debería funcionar siempre)
                    string googleResult = await TestUrl(client, "https://www.google.com");

                    // Test 2: Tu dominio
                    string domainResult = await TestUrl(client, "https://www.cipherunlock.xyz");

                    // Test 3: Dominio sin www
                    string noDomainResult = await TestUrl(client, "https://cipherunlock.xyz");

                    // Mostrar resultados
                    string message = "=== RESULTADOS DE CONECTIVIDAD ===\n\n" +
                                   $"Google: {googleResult}\n" +
                                   $"www.cipherunlock.xyz: {domainResult}\n" +
                                   $"cipherunlock.xyz: {noDomainResult}\n\n" +
                                   $"Timestamp: {DateTime.Now}";

                    MessageBox.Show(message, "Test de Conectividad", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en test: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async Task<string> TestUrl(HttpClient client, string url)
        {
            try
            {
                var response = await client.GetAsync(url);
                return $"✅ {response.StatusCode}";
            }
            catch (HttpRequestException httpEx)
            {
                return $"❌ HTTP Error: {httpEx.Message}";
            }
            catch (TaskCanceledException)
            {
                return "❌ Timeout";
            }
            catch (Exception ex)
            {
                return $"❌ Error: {ex.Message}";
            }
        }
    }
}
using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CipherUnlockProV1
{
    public partial class StartupForm : Form
    {
        private Label lblTitle;
        private Label lblStatus;
        private Button btnTestApi;
        private Label lblInfo;

        public StartupForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Configuración del formulario
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(40, 40, 40);
            this.ClientSize = new Size(600, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StartupForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "CipherUnlock Pro - Test de API";

            // Título
            this.lblTitle = new Label();
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Arial", 18F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Location = new Point(50, 30);
            this.lblTitle.Size = new Size(400, 29);
            this.lblTitle.Text = "CipherUnlock Pro - Test de API";
            this.Controls.Add(this.lblTitle);

            // Información
            this.lblInfo = new Label();
            this.lblInfo.Font = new Font("Arial", 11F);
            this.lblInfo.ForeColor = Color.LightGray;
            this.lblInfo.Location = new Point(50, 80);
            this.lblInfo.Size = new Size(500, 80);
            this.lblInfo.Text = "Este test verificará si tu API está funcionando correctamente.\n\n" +
                              "URLs que se probarán:\n" +
                              "• http://www.cipherunlock.xyz/api/register.php\n" +
                              "• http://www.cipherunlock.xyz/api/login.php";
            this.Controls.Add(this.lblInfo);

            // Botón Test API
            this.btnTestApi = new Button();
            this.btnTestApi.BackColor = Color.FromArgb(0, 120, 215);
            this.btnTestApi.FlatStyle = FlatStyle.Flat;
            this.btnTestApi.Font = new Font("Arial", 14F, FontStyle.Bold);
            this.btnTestApi.ForeColor = Color.White;
            this.btnTestApi.Location = new Point(50, 180);
            this.btnTestApi.Size = new Size(250, 60);
            this.btnTestApi.Text = "🚀 PROBAR MI API";
            this.btnTestApi.UseVisualStyleBackColor = false;
            this.btnTestApi.Click += BtnTestApi_Click;
            this.Controls.Add(this.btnTestApi);

            // Status
            this.lblStatus = new Label();
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new Font("Arial", 12F, FontStyle.Bold);
            this.lblStatus.ForeColor = Color.Yellow;
            this.lblStatus.Location = new Point(50, 260);
            this.lblStatus.Size = new Size(300, 19);
            this.lblStatus.Text = "Listo para probar tu API";
            this.Controls.Add(this.lblStatus);

            this.ResumeLayout(false);
        }

        private async void BtnTestApi_Click(object sender, EventArgs e)
        {
            await TestMyApi();
        }

        private async Task TestMyApi()
        {
            // Cambiar estado
            btnTestApi.Enabled = false;
            btnTestApi.Text = "⏳ PROBANDO...";
            lblStatus.Text = "Ejecutando tests de API...";
            lblStatus.ForeColor = Color.Yellow;

            var log = new System.Text.StringBuilder();
            log.AppendLine("═══════════════════════════════════════════════════");
            log.AppendLine("🔧 CIPHER UNLOCK PRO - TEST DE API COMPLETO");
            log.AppendLine("═══════════════════════════════════════════════════");
            log.AppendLine($"📅 Fecha/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            log.AppendLine($"🌐 Servidor: www.cipherunlock.xyz");
            log.AppendLine();

            bool registerSuccess = false;
            bool loginSuccess = false;

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Add("User-Agent", "CipherUnlockPro-ApiTester/1.0");
                    client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");

                    // ================================
                    // TEST 1: API DE REGISTRO
                    // ================================
                    log.AppendLine("🧪 TEST 1: API DE REGISTRO");
                    log.AppendLine("─────────────────────────────────────────────────");
                    log.AppendLine("📡 URL: http://www.cipherunlock.xyz/api/register.php");

                    try
                    {
                        // Datos únicos para el registro
                        var timestamp = DateTime.Now.Ticks;
                        var registerData = new
                        {
                            username = $"testuser_{timestamp}",
                            email = $"test_{timestamp}@example.com",
                            password = "TestPassword123!",
                            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        };

                        var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(registerData, Newtonsoft.Json.Formatting.Indented);
                        log.AppendLine("📤 Datos enviados:");
                        log.AppendLine(jsonData);
                        log.AppendLine();

                        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                        log.AppendLine("⏱️ Enviando solicitud...");
                        var response = await client.PostAsync("http://www.cipherunlock.xyz/api/register.php", content);

                        log.AppendLine($"📊 Status HTTP: {response.StatusCode} ({(int)response.StatusCode})");
                        log.AppendLine($"✅ Éxito HTTP: {response.IsSuccessStatusCode}");
                        log.AppendLine($"📋 Content-Type: {response.Content.Headers.ContentType}");

                        var responseText = await response.Content.ReadAsStringAsync();
                        log.AppendLine();
                        log.AppendLine("📥 RESPUESTA COMPLETA DEL SERVIDOR:");
                        log.AppendLine("┌" + new string('─', 70) + "┐");
                        log.AppendLine("│ " + responseText.Replace("\n", "\n│ ").PadRight(69) + "│");
                        log.AppendLine("└" + new string('─', 70) + "┘");
                        log.AppendLine();

                        if (response.IsSuccessStatusCode)
                        {
                            log.AppendLine("🎉 RESULTADO: API DE REGISTRO FUNCIONANDO CORRECTAMENTE");
                            registerSuccess = true;

                            // Analizar respuesta JSON
                            try
                            {
                                dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseText);
                                if (jsonResponse?.Éxito == true || jsonResponse?.Success == true)
                                {
                                    log.AppendLine("✅ Respuesta JSON indica éxito");
                                }
                                else
                                {
                                    log.AppendLine("⚠️ Respuesta JSON indica error en el registro");
                                }
                            }
                            catch
                            {
                                log.AppendLine("ℹ️ No se pudo analizar como JSON, pero HTTP fue exitoso");
                            }
                        }
                        else
                        {
                            log.AppendLine("❌ RESULTADO: ERROR EN API DE REGISTRO");
                            log.AppendLine($"   Código de error HTTP: {response.StatusCode}");
                        }
                    }
                    catch (HttpRequestException httpEx)
                    {
                        log.AppendLine($"🔌 ERROR DE CONEXIÓN EN REGISTRO: {httpEx.Message}");
                    }
                    catch (TaskCanceledException)
                    {
                        log.AppendLine("⏰ TIMEOUT EN REGISTRO: El servidor tardó demasiado en responder");
                    }
                    catch (Exception ex)
                    {
                        log.AppendLine($"💥 ERROR INESPERADO EN REGISTRO: {ex.Message}");
                    }

                    log.AppendLine();
                    log.AppendLine();

                    // ================================
                    // TEST 2: API DE LOGIN
                    // ================================
                    log.AppendLine("🔐 TEST 2: API DE LOGIN");
                    log.AppendLine("─────────────────────────────────────────────────");
                    log.AppendLine("📡 URL: http://www.cipherunlock.xyz/api/login.php");

                    try
                    {
                        var loginData = new
                        {
                            username = "admin",
                            password = "admin123",
                            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        };

                        var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(loginData, Newtonsoft.Json.Formatting.Indented);
                        log.AppendLine("🔑 Credenciales enviadas:");
                        log.AppendLine(jsonData);
                        log.AppendLine();

                        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                        log.AppendLine("⏱️ Enviando solicitud...");
                        var response = await client.PostAsync("http://www.cipherunlock.xyz/api/login.php", content);

                        log.AppendLine($"📊 Status HTTP: {response.StatusCode} ({(int)response.StatusCode})");
                        log.AppendLine($"✅ Éxito HTTP: {response.IsSuccessStatusCode}");
                        log.AppendLine($"📋 Content-Type: {response.Content.Headers.ContentType}");

                        var responseText = await response.Content.ReadAsStringAsync();
                        log.AppendLine();
                        log.AppendLine("📥 RESPUESTA COMPLETA DEL SERVIDOR:");
                        log.AppendLine("┌" + new string('─', 70) + "┐");
                        log.AppendLine("│ " + responseText.Replace("\n", "\n│ ").PadRight(69) + "│");
                        log.AppendLine("└" + new string('─', 70) + "┘");
                        log.AppendLine();

                        if (response.IsSuccessStatusCode)
                        {
                            log.AppendLine("🎉 RESULTADO: API DE LOGIN FUNCIONANDO CORRECTAMENTE");
                            loginSuccess = true;

                            // Buscar token de sesión
                            if (responseText.Contains("Token_Sesion") || responseText.Contains("SessionToken") || responseText.Contains("token"))
                            {
                                log.AppendLine("🎫 TOKEN DE SESIÓN DETECTADO EN LA RESPUESTA");

                                try
                                {
                                    dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseText);
                                    if (jsonResponse?.Token_Sesion != null)
                                    {
                                        log.AppendLine($"🔐 Token extraído: {jsonResponse.Token_Sesion}");
                                    }
                                }
                                catch
                                {
                                    log.AppendLine("ℹ️ Token presente pero no se pudo extraer automáticamente");
                                }
                            }
                            else
                            {
                                log.AppendLine("⚠️ No se detectó token de sesión en la respuesta");
                            }
                        }
                        else
                        {
                            log.AppendLine("❌ RESULTADO: ERROR EN API DE LOGIN");
                            log.AppendLine($"   Código de error HTTP: {response.StatusCode}");
                        }
                    }
                    catch (HttpRequestException httpEx)
                    {
                        log.AppendLine($"🔌 ERROR DE CONEXIÓN EN LOGIN: {httpEx.Message}");
                    }
                    catch (TaskCanceledException)
                    {
                        log.AppendLine("⏰ TIMEOUT EN LOGIN: El servidor tardó demasiado en responder");
                    }
                    catch (Exception ex)
                    {
                        log.AppendLine($"💥 ERROR INESPERADO EN LOGIN: {ex.Message}");
                    }
                }
            }
            catch (Exception generalEx)
            {
                log.AppendLine($"💥 ERROR CRÍTICO GENERAL: {generalEx.Message}");
            }

            // ================================
            // ANÁLISIS FINAL
            // ================================
            log.AppendLine();
            log.AppendLine();
            log.AppendLine("═══════════════════════════════════════════════════");
            log.AppendLine("📊 ANÁLISIS FINAL DE RESULTADOS");
            log.AppendLine("═══════════════════════════════════════════════════");

            if (registerSuccess && loginSuccess)
            {
                log.AppendLine("🎉🎉🎉 ¡FELICIDADES! TU API ESTÁ FUNCIONANDO PERFECTAMENTE 🎉🎉🎉");
                log.AppendLine();
                log.AppendLine("✅ API de Registro: FUNCIONANDO");
                log.AppendLine("✅ API de Login: FUNCIONANDO");
                log.AppendLine();
                log.AppendLine("🚀 TU SERVIDOR ESTÁ LISTO PARA PRODUCCIÓN");
                log.AppendLine("💡 Puedes integrar estas APIs en tu aplicación principal");
                log.AppendLine("🔒 Recuerda configurar HTTPS para mayor seguridad");

                lblStatus.Text = "🎉 ¡API funcionando perfectamente!";
                lblStatus.ForeColor = Color.Green;
            }
            else if (registerSuccess || loginSuccess)
            {
                log.AppendLine("⚠️ TU API ESTÁ PARCIALMENTE FUNCIONAL");
                log.AppendLine();
                log.AppendLine($"{(registerSuccess ? "✅" : "❌")} API de Registro: {(registerSuccess ? "FUNCIONANDO" : "CON PROBLEMAS")}");
                log.AppendLine($"{(loginSuccess ? "✅" : "❌")} API de Login: {(loginSuccess ? "FUNCIONANDO" : "CON PROBLEMAS")}");
                log.AppendLine();
                log.AppendLine("🔧 RECOMENDACIÓN: Revisar la configuración del endpoint que falla");

                lblStatus.Text = "⚠️ API parcialmente funcional";
                lblStatus.ForeColor = Color.Orange;
            }
            else
            {
                log.AppendLine("❌ PROBLEMAS DETECTADOS EN TU API");
                log.AppendLine();
                log.AppendLine("❌ API de Registro: CON PROBLEMAS");
                log.AppendLine("❌ API de Login: CON PROBLEMAS");
                log.AppendLine();
                log.AppendLine("🔧 POSIBLES CAUSAS Y SOLUCIONES:");
                log.AppendLine("   • Archivos PHP no subidos: Verifica que register.php y login.php estén en /api/");
                log.AppendLine("   • Errores de sintaxis PHP: Revisa los logs del servidor");
                log.AppendLine("   • Permisos incorrectos: Asegúrate que los archivos sean ejecutables");
                log.AppendLine("   • Configuración del servidor: Verifica que PHP esté habilitado");
                log.AppendLine("   • Base de datos: Si usas BD, verifica la conexión");

                lblStatus.Text = "❌ Problemas detectados en la API";
                lblStatus.ForeColor = Color.Red;
            }

            log.AppendLine();
            log.AppendLine("═══════════════════════════════════════════════════");
            log.AppendLine($"🏁 TEST COMPLETADO A LAS {DateTime.Now:HH:mm:ss}");
            log.AppendLine("═══════════════════════════════════════════════════");

            // Restaurar botón
            btnTestApi.Enabled = true;
            btnTestApi.Text = "🚀 PROBAR MI API";

            // Mostrar resultados
            ShowDetailedResults(log.ToString());
        }

        private void ShowDetailedResults(string results)
        {
            var resultForm = new Form();
            resultForm.Text = "🔬 Resultados Detallados del Test de API - CipherUnlock Pro";
            resultForm.Size = new Size(1000, 800);
            resultForm.StartPosition = FormStartPosition.CenterParent;
            resultForm.BackColor = Color.FromArgb(20, 20, 20);

            var txtResults = new TextBox();
            txtResults.Multiline = true;
            txtResults.ScrollBars = ScrollBars.Both;
            txtResults.Font = new Font("Consolas", 10);
            txtResults.BackColor = Color.Black;
            txtResults.ForeColor = Color.Lime;
            txtResults.Text = results;
            txtResults.ReadOnly = true;
            txtResults.Dock = DockStyle.Fill;

            resultForm.Controls.Add(txtResults);
            resultForm.ShowDialog(this);
        }
    }
}
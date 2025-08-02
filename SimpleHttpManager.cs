using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CipherUnlockPro
{
    /// <summary>
    /// Formulario de inicio limpio solo para testing de API
    /// </summary>
    public partial class StartupFormClean : Form
    {
        public StartupFormClean()
        {
            InitializeComponent();
            SetupForm();
            AddTestButtons();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Configuración básica del formulario
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(40, 40, 40);
            this.ClientSize = new Size(600, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StartupFormClean";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "CipherUnlock Pro - Test de API";

            this.ResumeLayout(false);
        }

        private void SetupForm()
        {
            // Título
            var lblTitle = new Label();
            lblTitle.Text = "CipherUnlock Pro - Test de API";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(50, 30);
            this.Controls.Add(lblTitle);

            // Instrucciones
            var lblInstructions = new Label();
            lblInstructions.Text = "Haz clic en los botones para probar tu API:";
            lblInstructions.Font = new Font("Segoe UI", 10);
            lblInstructions.ForeColor = Color.LightGray;
            lblInstructions.AutoSize = true;
            lblInstructions.Location = new Point(50, 80);
            this.Controls.Add(lblInstructions);
        }

        private void AddTestButtons()
        {
            // Botón 1: Test de Conexión
            var btnConnection = new Button();
            btnConnection.Text = "Test Conexión";
            btnConnection.Size = new Size(120, 40);
            btnConnection.Location = new Point(50, 150);
            btnConnection.BackColor = Color.FromArgb(0, 120, 215);
            btnConnection.ForeColor = Color.White;
            btnConnection.FlatStyle = FlatStyle.Flat;
            btnConnection.Click += async (s, e) => await TestConnection();
            this.Controls.Add(btnConnection);

            // Botón 2: Test de Registro
            var btnRegister = new Button();
            btnRegister.Text = "Test Registro";
            btnRegister.Size = new Size(120, 40);
            btnRegister.Location = new Point(190, 150);
            btnRegister.BackColor = Color.FromArgb(40, 167, 69);
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Click += async (s, e) => await TestRegister();
            this.Controls.Add(btnRegister);

            // Botón 3: Test de Login
            var btnLogin = new Button();
            btnLogin.Text = "Test Login";
            btnLogin.Size = new Size(120, 40);
            btnLogin.Location = new Point(330, 150);
            btnLogin.BackColor = Color.FromArgb(255, 140, 0);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Click += async (s, e) => await TestLogin();
            this.Controls.Add(btnLogin);

            // Botón 4: Test Completo
            var btnComplete = new Button();
            btnComplete.Text = "Test Completo";
            btnComplete.Size = new Size(120, 40);
            btnComplete.Location = new Point(470, 150);
            btnComplete.BackColor = Color.FromArgb(108, 117, 125);
            btnComplete.ForeColor = Color.White;
            btnComplete.FlatStyle = FlatStyle.Flat;
            btnComplete.Click += async (s, e) => await TestComplete();
            this.Controls.Add(btnComplete);

            // Status
            var lblStatus = new Label();
            lblStatus.Text = "Listo para probar APIs";
            lblStatus.Font = new Font("Segoe UI", 9);
            lblStatus.ForeColor = Color.Yellow;
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(50, 220);
            this.Controls.Add(lblStatus);

            // Información
            var lblInfo = new Label();
            lblInfo.Text = "URLs de API:\n" +
                          "• http://www.cipherunlock.xyz/api/register.php\n" +
                          "• http://www.cipherunlock.xyz/api/login.php\n\n" +
                          "Credenciales de prueba: admin / admin123";
            lblInfo.Font = new Font("Segoe UI", 9);
            lblInfo.ForeColor = Color.LightGray;
            lblInfo.Location = new Point(50, 250);
            lblInfo.Size = new Size(500, 100);
            this.Controls.Add(lblInfo);
        }

        // Métodos de test
        private async Task TestConnection()
        {
            try
            {
                await SimpleConnectionTest.Instance.TestBasicConnectionAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en test de conexión: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task TestRegister()
        {
            try
            {
                await CipherApiManager.Instance.TestRegisterAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en test de registro: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task TestLogin()
        {
            try
            {
                await CipherApiManager.Instance.TestLoginAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en test de login: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task TestComplete()
        {
            try
            {
                await CipherApiManager.Instance.TestCompleteApiAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en test completo: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
using System;
using System.Windows.Forms;
using CipherUnlockProV1.Licensing;

namespace CipherUnlockProV1.Forms
{
    public partial class LicenseActivationForm : Form
    {
        private readonly int userId;
        private object SessionManager;

        public LicenseActivationForm(int userId)
        {
            InitializeComponent();
            this.userId = userId;
        }

        private async void btnActivate_Click(object sender, EventArgs e)
        {
            string licenseKey = txtLicenseKey.Text.Trim();

            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                MessageBox.Show("Please enter a valid license key.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Usar el nuevo sistema de licenciamiento
                var sessionInfo = SessionManager();
                if (sessionInfo == null)
                {
                    MessageBox.Show("Session expired. Please login again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var response = await RemoteLicenseManager.Instance.ActivateLicenseAsync(
                    sessionInfo.Username, licenseKey, sessionInfo.SessionToken);

                if (response.Success && response.Data != null)
                {
                    MessageBox.Show("License activated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Actualizar sesión con nueva licencia
                    sessionInfo.LicenseKey = response.Data.LicenseKey;
                    SessionManager(sessionInfo);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(response.Error ?? "License activation failed.", "Activation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during license activation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
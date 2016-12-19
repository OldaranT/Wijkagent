using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using WijkAgent.Model;
using System.IO;

namespace WijkAgent
{
    public delegate void LogInButtonClick(string username);

    public partial class LogInScreen : Form
    {
        public event LogInButtonClick OnLogInButtonClick;
        public SQLConnection sqlConn = new SQLConnection();
        private Color policeBlue;
        private Color policeGold;
        private Font labelFont;

        #region Constructor
        public LogInScreen()
        {
            policeBlue = Color.FromArgb(0, 70, 130);
            policeGold = Color.FromArgb(190, 150, 90);
            labelFont = new Font("Calibri", 12, FontStyle.Bold);
            InitializeComponent();
            GetLastUsedUsername();

            //als er al een keer een gebr.naam is onthouden is de checkbox auto checked
            if (Properties.Settings.Default.LastUsername.Length > 0)
            {
                stayLoggedIn_checkbox.Checked = true;
            }
        }
        #endregion

        #region Inloggen
        private void logIn_button_Click(object sender, EventArgs e)
        {
            // variabel maken van ingevoerde waardes
            string textbox_password = getSHA512(logIn_password_textbox.Text);
            string textbox_username = logIn_username_textbox.Text;

            // open database connectie voor 1e query
            sqlConn.conn.Open();

            // kijk of gebruikersnaam voorkomt in de database.
            // als aantal 0 is, dan is username incorrect.
            // als aantal 1 is, dan is username correct en wordt het wachtwoord gecontroleerd.
            string stm = "SELECT count(idaccount) FROM account WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(stm, sqlConn.conn);
            cmd.Parameters.AddWithValue("@username", textbox_username);
            sqlConn.rdr = cmd.ExecuteReader();
            sqlConn.rdr.Read();
            int amount = Convert.ToInt32(sqlConn.rdr.GetString(0));

            // sluit verbinding voor 1e query
            sqlConn.conn.Close();

            // als gebruikernsaam voorkomt, controleer dan wachtwoord
            if (amount != 0)
            {
                // open database connectie voor 2e query
                sqlConn.conn.Open();

                // haal wachtwoord op uit de database
                stm = "SELECT password FROM account WHERE username = @username";
                cmd = new MySqlCommand(stm, sqlConn.conn);
                cmd.Parameters.AddWithValue("@username", textbox_username);
                sqlConn.rdr = cmd.ExecuteReader();
                sqlConn.rdr.Read();
                string dbPassword = sqlConn.rdr.GetString(0).ToLower();

                // sluit verbinding voor 1e query
                sqlConn.conn.Close();

                // als wachtwoord gelijk is aan het opgehaalde wachtwoord uit de database, kan er worden ingelogd
                if (dbPassword == textbox_password)
                {
                    sqlConn.conn.Open();

                    // Controleren of er al iemand is ingelogd op dit account
                    stm = "SELECT idaccount FROM account WHERE username = @username AND longitude is NULL";
                    cmd = new MySqlCommand(stm, sqlConn.conn);
                    cmd.Parameters.AddWithValue("@username", textbox_username);
                    sqlConn.rdr = cmd.ExecuteReader();

                    //Wanneer iemand al is ingelogd 
                    if (!sqlConn.rdr.Read())
                    {
                        //Bericht tonen dat iemand al is ingelogd
                        MessageBox.Show("Er is al iemand ingelogd op dit account!");
                    }
                    //Wanneer niemand is ingelogd op dit account
                    else
                    {
                        // als de checkbox is aangevinkt, voer de functie uit
                        if (stayLoggedIn_checkbox.Checked)
                        {
                            SetLastUsedUsername();
                        }
                        else
                        {
                            Properties.Settings.Default.LastUsername = "";
                            Properties.Settings.Default.Save();
                        }

                        // open applicatie, sluit inlogscherm
                        if (OnLogInButtonClick != null)
                        {
                            OnLogInButtonClick(textbox_username);
                        }
                    }

                    // sluit verbinding voor 1e query
                    sqlConn.conn.Close();               
                }
                else
                {
                    // geef foutmelding omdat wachtwoord verkeerd is
                    PrintErrorLabel();
                }
            }
            else
            {
                // geef foutmelding omdat gebruikersnaam verkeerd is
                PrintErrorLabel();
            }
        }
        #endregion

        #region Hashen van password(SHA-512)
        public string getSHA512(string text)
        {
            // sha512 hash
            SHA512CryptoServiceProvider sh = new SHA512CryptoServiceProvider();
            sh.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] re = sh.Hash;
            StringBuilder sb = new StringBuilder();
            foreach (byte b in re)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
        #endregion

        #region Geef foutmelding
        private void PrintErrorLabel()
        {
            string errorMessage = "Inloggegevens zijn incorrect!";
            string headerMessage = "Fout Melding!";
            logIn_password_textbox.Text = "";
            MessageBox.Show(errorMessage, headerMessage, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
        }
        #endregion

        #region GetLastUsedUsername
        public void GetLastUsedUsername()
        {
            // haal gebruikersnaam uit de settings
            logIn_username_textbox.Text = Properties.Settings.Default.LastUsername;
        }
        #endregion

        #region SetLastUsedUsername
        public void SetLastUsedUsername()
        {
            // sla laatst gebruikte gebruikersnaam op in de settings
            Properties.Settings.Default.LastUsername = logIn_username_textbox.Text;
            Properties.Settings.Default.Save();
        }
        #endregion

        #region LogInScreen_Load
        private void LogInScreen_Load(object sender, EventArgs e)
        {
            this.BackColor = policeBlue;
            logIn_password_textbox.Clear();
            if (Properties.Settings.Default.LastUsername.Length == 0)
            {
                logIn_username_textbox.Clear();
            }
        }
        #endregion
    }
}

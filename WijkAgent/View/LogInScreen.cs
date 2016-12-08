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
        }
        #endregion

        #region Inloggen
        private void logIn_button_Click(object sender, EventArgs e)
        {
            //Variabel maken van ingevoerde waardes
            string textbox_password = getSHA512(logIn_password_textbox.Text);
            string textbox_username = logIn_username_textbox.Text;

            //Open database connectie voor 1e query
            sqlConn.conn.Open();

            //Kijk of gebruikersnaam voorkomt in de database.
            //Als aantal 0 is, dan is username incorrect.
            //Als aantal 1 is, dan is username correct en wordt het wachtwoord gecontroleerd.
            string stm = "SELECT count(idaccount) FROM account WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(stm, sqlConn.conn);
            cmd.Parameters.AddWithValue("@username", textbox_username);
            sqlConn.rdr = cmd.ExecuteReader();
            sqlConn.rdr.Read();
            int amount = Convert.ToInt32(sqlConn.rdr.GetString(0));

            //Sluit verbinding voor 1e query
            sqlConn.conn.Close();

            //Als gebruikernsaam voorkomt, controleer dan wachtwoord
            if (amount != 0)
            {
                //Open database connectie voor 2e query
                sqlConn.conn.Open();

                //Haal wachtwoord op uit de database
                stm = "SELECT password FROM account WHERE username = @username";
                cmd = new MySqlCommand(stm, sqlConn.conn);
                cmd.Parameters.AddWithValue("@username", textbox_username);
                sqlConn.rdr = cmd.ExecuteReader();
                sqlConn.rdr.Read();
                string dbPassword = sqlConn.rdr.GetString(0).ToLower();

                //Sluit verbinding voor 1e query
                sqlConn.conn.Close();

                //Als wachtwoord gelijk is aan wachtwoord uit de database, kan er worden ingelogd
                if (dbPassword == textbox_password)
                {
                    //als de checkbox is aangevinkt voor de fucntie uit
                    if (stayLoggedIn_checkbox.Checked)
                    {
                        SetLastUsedUsername();
                    }

                    //Open applicatie, sluit inlogscherm
                    if (OnLogInButtonClick != null)
                    {
                        OnLogInButtonClick(textbox_username);
                    }
                }
                else
                {
                    //Geef foutmelding omdat wachtwoord verkeerd is
                    PrintErrorLabel();
                }
            }
            else
            {
                //Geef foutmelding omdat gebruikersnaam verkeerd is
                PrintErrorLabel();
            }
        }
        #endregion

        #region Hashen van password(SHA-512)
        public string getSHA512(string text)
        {
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
            MessageBox.Show(errorMessage, headerMessage, MessageBoxButtons.OK,MessageBoxIcon.Exclamation,MessageBoxDefaultButton.Button1);
        }
        #endregion

        #region GetLastUsedUsername
        public void GetLastUsedUsername()
        {
            logIn_username_textbox.Text = Properties.Settings.Default.LastUsername;
        }
        #endregion

        #region SetLastUsedUsername
        public void SetLastUsedUsername()
        {
            Properties.Settings.Default.LastUsername = logIn_username_textbox.Text;
            Properties.Settings.Default.Save();
        }
        #endregion

        private void LogInScreen_Load(object sender, EventArgs e)
        {
            this.BackColor = policeBlue;
        }
    }
}

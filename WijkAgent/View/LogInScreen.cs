using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WijkAgent
{
    public delegate void LogInButtonClick();

    public partial class LogInScreen : Form
    {
        public event LogInButtonClick OnLogInButtonClick;

        public LogInScreen()
        {
            InitializeComponent();
        }

        private void logIn_button_Click(object sender, EventArgs e)
        {
            if (OnLogInButtonClick != null)
                OnLogInButtonClick();
        }
    }
}

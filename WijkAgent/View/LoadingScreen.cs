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
    public partial class LoadingScreen : Form
    {
        public LoadingScreen()
        {
            InitializeComponent();
        }

        //Methode om scherm te laten zien
        public void ShowLoadingScreen()
        {
            this.Show();
        }

        //Methode om scherm te verbergen
        public void HideLoadingScreen()
        {
            this.Hide();
        }
    }
}

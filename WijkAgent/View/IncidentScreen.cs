using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows.Forms;
using WijkAgent.Model;

namespace WijkAgent
{
    public partial class IncidentScreen : Form
    {
        SQLConnection sql = new SQLConnection();
        List<CheckBox> TwitterCheckboxes = new List<CheckBox>();
        //zodat alles netjes onderelkaar komt
        int top = 20;
        private Color policeBlue;
        private Color policeGold;
        private Font labelFont;

        public IncidentScreen(int _districtId)
        {
            InitializeComponent();
            policeBlue = Color.FromArgb(0, 70, 130);
            policeGold = Color.FromArgb(190, 150, 90);
            labelFont = new Font("Calibri", 12, FontStyle.Bold);

            //alle categorien en twitter berichten ophalen
            Dictionary<int,string> categories = sql.GetAllCategory();
            Dictionary<int,string> twitterMessages = sql.GetAllTwitterMessageFromDistrictToday(_districtId);

            //zodat je iet kan rezien en sluiten + scrollbar
            twitterIncidentPanel.AutoScroll = true;
            this.ControlBox = false;

            //click functies doorsturen naar andere methodes
            cancelIncidentButton.Click += CancelButtonClick;
            selectAllCheck.CheckStateChanged += SelectAllCheckClicked;
            saveIncidentButton.Click += saveIncident;

            foreach (KeyValuePair<int,string> entry in categories)
            {
                categoryCombo.Items.Add(entry.Value);
            }

            foreach (KeyValuePair<int,string> entry in twitterMessages)
            {
                CheckBox checkMessage = new CheckBox() {Left = this.Left + 31, Top = this.top, AutoSize = true, Name = entry.Key.ToString()};
                Label twitterMessage = new Label() { Font = labelFont, Text=entry.Value, Top = this.top, AutoSize = true, MaximumSize = new Size(300, 0), Left = checkMessage.Width, Name = entry.Key.ToString() };

                twitterIncidentPanel.Controls.Add(checkMessage);
                TwitterCheckboxes.Add(checkMessage);
                twitterIncidentPanel.Controls.Add(twitterMessage);

                this.top = this.top + 20 + twitterMessage.Height;
            }
        }
        
        private void saveIncident(object sender, EventArgs e)
        {
            if (categoryCombo.SelectedIndex > -1)
            {
                //selected categorie ophalen
                string _selectedCategory = categoryCombo.SelectedItem.ToString();
                List<CheckBox> _selectedTwitterCheckboxes = new List<CheckBox>();

                foreach (CheckBox box in TwitterCheckboxes)
                {
                    if (box.Checked)
                    {
                        //voeg in de list
                        _selectedTwitterCheckboxes.Add(box);
                    }
                }
                if(_selectedTwitterCheckboxes.Count > 0)
                {
                    //database update
                    foreach (CheckBox box in _selectedTwitterCheckboxes)
                    {
                        //de naam van elke checkbox heeft het bericht id dit is een string maar moet naar een int
                        sql.updateTwitterMessageCategory(Int32.Parse(box.Name), _selectedCategory);
                        MessageBox.Show("Voorval toegevoegd");
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Kies tenminste 1 bericht die moet worden gecombineerd aan het gewenste categorie");
                }
            }
            else
            {
                MessageBox.Show("Kies een categorie");
            }
        }
        //als die veranderdt kijken of die check of unchecked is en dan de rest checken of unchecken
        private void SelectAllCheckClicked(object sender, EventArgs e)
        {
            if(selectAllCheck.Checked)
            {
                foreach (CheckBox box in TwitterCheckboxes)
                {
                    box.Checked = true;
                }
            }else if (!selectAllCheck.Checked)
            {
                foreach (CheckBox box in TwitterCheckboxes)
                {
                    box.Checked = false;
                }
            }
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void IncidentScreen_Load(object sender, EventArgs e)
        {
            this.BackColor = policeBlue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitterAPI.Model;
using WijkAgent.Model;
using MySql.Data.MySqlClient;

namespace WijkAgent
{
    public partial class View : Form
    {
        
        private ModelClass modelClass;
        Map map = new Map();
        private bool provinceButtonsCreated = false;
        private bool cityButtonsCreated = false;
        private bool districtButtonsCreated = false;
        private int buttonSizeX;
        private int buttonSizeY;
        private Color policeBlue;
        private Color policeGold;
        private Font buttonFont;

        public View()
        {
            policeBlue = Color.FromArgb(0, 70, 130);
            policeGold = Color.FromArgb(190, 150, 90);
            buttonFont = new Font("Microsoft Sans Serif", 16, FontStyle.Bold);
            buttonSizeX = 300;
            buttonSizeY = 75;
            InitializeComponent();
            this.SetTopLevel(true);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Maximized;
            modelClass = new ModelClass();

            //init funcite aanroepen
            map.initialize();

            //wb is de webbrowser waar de map in staat. Ook even dezelfde breedte/hoogte geven ;)
            map.wb.Dock = DockStyle.Fill;
            map_panel.Controls.Add(map.wb);


            //Console.ReadLine();

        }

        private void View_Load(object sender, EventArgs e)
        {
            main_menu_panel_for_label.BackColor = policeBlue;
            province_panel_for_label.BackColor = policeBlue;
            city_panel_for_label.BackColor = policeBlue;
            district_panel_for_label.BackColor = policeBlue;

            go_to_province_panel_button_from_main_menu_tab.BackColor = policeBlue;
            go_to_province_panel_button_from_main_menu_tab.ForeColor = Color.White;
            go_to_province_panel_button_from_main_menu_tab.Font = buttonFont;

            go_to_main_menu_panel_button.BackColor = policeBlue;
            go_to_main_menu_panel_button.ForeColor = Color.White;
            go_to_main_menu_panel_button.Font = buttonFont;

            go_to_province_panel_button_from_city_tab.BackColor = policeBlue;
            go_to_province_panel_button_from_city_tab.ForeColor = Color.White;
            go_to_province_panel_button_from_city_tab.Font = buttonFont;

            go_to_city_panel_button_from_district_tab.BackColor = policeBlue;
            go_to_city_panel_button_from_district_tab.ForeColor = Color.White;
            go_to_city_panel_button_from_district_tab.Font = buttonFont;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!provinceButtonsCreated) {

                //Open database connectie
                modelClass.databaseConnectie.conn.Open();

                //Selectie Query die de namen van allke province selecteer en ordered.
                string stm = "SELECT * FROM province ORDER BY name DESC";
                MySqlCommand cmd = new MySqlCommand(stm, modelClass.databaseConnectie.conn);
                modelClass.databaseConnectie.rdr = cmd.ExecuteReader();

                // Hier word de database lijst uitgelezen
                while (modelClass.databaseConnectie.rdr.Read())
                {
                    Button buttonCreate = new Button();
                    buttonCreate.Text = modelClass.databaseConnectie.rdr.GetString(1);
                    buttonCreate.Name = modelClass.databaseConnectie.rdr.GetString(0).ToLower();
                    buttonLayout(buttonCreate);
                    provnice_scroll_panel.Controls.Add(buttonCreate);
                    buttonCreate.Click += ProvinceButton_Click;
                }
                modelClass.databaseConnectie.conn.Close();
                provinceButtonsCreated = true;
            }
            
            main_menu_tabcontrol.SelectTab(1);
        }

        private void go_to_main_menu_panel_button_Click(object sender, EventArgs e)
        {

            main_menu_tabcontrol.SelectTab(0);

        }
        //Kijkt of er een ProvinceGenerated Button is ingedrukt.
        public void ProvinceButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            //Test writeline later verwijderen
            Console.WriteLine(clickedButton.Text.ToString());
            if (!cityButtonsCreated)
            {
                    int idProvince = Convert.ToInt32(clickedButton.Name);

                    //Open database connectie
                    modelClass.databaseConnectie.conn.Open();

                    //Selectie Query die de namen van allke province selecteer en ordered.
                    string stm = "SELECT * FROM city WHERE idprovince = @idprovince ORDER BY name DESC";
                    MySqlCommand cmd = new MySqlCommand(stm, modelClass.databaseConnectie.conn);
                    cmd.Parameters.AddWithValue("@idprovince", idProvince);
                    modelClass.databaseConnectie.rdr = cmd.ExecuteReader();

                    // Hier word de database lijst uitgelezen
                    while (modelClass.databaseConnectie.rdr.Read())
                    {
                        Button buttonCreate = new Button();
                        buttonCreate.Text = modelClass.databaseConnectie.rdr.GetString(2);
                        buttonCreate.Name = modelClass.databaseConnectie.rdr.GetString(0).ToLower();
                        buttonLayout(buttonCreate);
                        city_scroll_panel.Controls.Add(buttonCreate);
                        buttonCreate.Click += CityButton_Click;
                    }
                    modelClass.databaseConnectie.conn.Close();

                
                cityButtonsCreated = true;
            }


            main_menu_tabcontrol.SelectTab(2);
        }

        //Kijkt of er een CityGenerated Button is ingedrukt.
        public void CityButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            //Test writeline later verwijderen
            Console.WriteLine(clickedButton.Text.ToString());
            if (!districtButtonsCreated)
            {
                int idCity = Convert.ToInt32(clickedButton.Name);

                //Open database connectie
                modelClass.databaseConnectie.conn.Open();

                //Selectie Query die de namen van allke province selecteer en ordered.
                string stm = "SELECT * FROM district WHERE idcity = @idcity ORDER BY name DESC";
                MySqlCommand cmd = new MySqlCommand(stm, modelClass.databaseConnectie.conn);
                cmd.Parameters.AddWithValue("@idcity", idCity);
                modelClass.databaseConnectie.rdr = cmd.ExecuteReader();

                // Hier word de database lijst uitgelezen
                while (modelClass.databaseConnectie.rdr.Read())
                {
                    Button buttonCreate = new Button();
                    buttonCreate.Text = modelClass.databaseConnectie.rdr.GetString(2);
                    buttonCreate.Name = modelClass.databaseConnectie.rdr.GetString(0).ToLower();
                    buttonLayout(buttonCreate);
                    district_scroll_panel.Controls.Add(buttonCreate);
                    buttonCreate.Click += DistrictButton_Click;
                }
                modelClass.databaseConnectie.conn.Close();
                districtButtonsCreated = true;
            }


            main_menu_tabcontrol.SelectTab(3);
        }

        //Kijkt of er een DistrictGenerated Button is ingedrukt.
        public void DistrictButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            //Test writeline later verwijderen
            Console.WriteLine(clickedButton.Text.ToString());

            int idDistrict = Convert.ToInt32(clickedButton.Name);

            //Open database connectie
            modelClass.databaseConnectie.conn.Open();

            //Selectie Query die de namen van allke province selecteer en ordered.
            string stm = "SELECT * FROM district WHERE idcity = @idcity ORDER BY name DESC";
            MySqlCommand cmd = new MySqlCommand(stm, modelClass.databaseConnectie.conn);
            cmd.Parameters.AddWithValue("@idcity", idDistrict);
            modelClass.databaseConnectie.rdr = cmd.ExecuteReader();


            
            // Hier word de database lijst uitgelezen
            while (modelClass.databaseConnectie.rdr.Read())
            {
            }
            modelClass.databaseConnectie.conn.Close();

            //if (clickedButton.Text.ToString() == "Spoolde")
            //{
            //    Console.WriteLine("Ja dit is Spoolde");
            //    //de wijk veranderen
            //    map.changeDistrict(modelClass.districtList1[2]);

            //}

            //else if (clickedButton.Text.ToString() == "Diezerpoort")
            //{
            //    Console.WriteLine("Ja dit is Diezerpoort");
            //    //de wijk veranderen
            //    map.changeDistrict(modelClass.districtList1[1]);

            //}
        }
        //Als de terug button wordt ingedruk op de city tab
        private void go_to_province_panel_button_from_city_tab_Click(object sender, EventArgs e)
        {
            //cleared alles in city scroll panel
            city_scroll_panel.Controls.Clear();
            main_menu_tabcontrol.SelectTab(1);
            cityButtonsCreated = false;

        }

        private void go_to_city_panel_button_from_district_tab_Click(object sender, EventArgs e)
        {
            //cleared alles in stad scroll panel
            district_scroll_panel.Controls.Clear();
            main_menu_tabcontrol.SelectTab(2);
            districtButtonsCreated = false;

        }

        private void buttonLayout(Button _button)
        {
            _button.Size = new Size(buttonSizeX, buttonSizeY);
            _button.Dock = DockStyle.Top;
            _button.BackColor = policeBlue;
            _button.ForeColor = Color.White;
            _button.Font = buttonFont;
            _button.FlatStyle = FlatStyle.Flat;
            _button.FlatAppearance.BorderColor = policeGold;
            _button.FlatAppearance.BorderSize = 1;

        }
    }
}

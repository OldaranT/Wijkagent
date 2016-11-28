using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WijkAgent.Model;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace WijkAgent
{
    public delegate void RefreshButtonClick();

    public partial class View : Form
    {
        public ModelClass modelClass;
        private bool provinceButtonsCreated = false;
        private bool cityButtonsCreated = false;
        private bool districtButtonsCreated = false;
        private int buttonSizeX;
        private int buttonSizeY;
        private Color policeBlue;
        private Color policeGold;
        private Font buttonFont;
        private LoadingScreen loadingScreen;

        //events
        public event RefreshButtonClick OnRefreshButtonClick;

        public View()
        {
            modelClass = new ModelClass();
            policeBlue = Color.FromArgb(0, 70, 130);
            policeGold = Color.FromArgb(190, 150, 90);
            buttonFont = new Font("Microsoft Sans Serif", 16, FontStyle.Bold);
            buttonSizeX = 300;
            buttonSizeY = 75;
            InitializeComponent();
            this.SetTopLevel(true);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Maximized;

            //init funcite aanroepen
            modelClass.map.initialize();

            //wb is de webbrowser waar de map in staat. Ook even dezelfde breedte/hoogte geven ;)
            modelClass.map.wb.Dock = DockStyle.Fill;
            map_panel.Controls.Add(modelClass.map.wb);

            //Voegt methodes van Loading class toe aan de events in de Twitter class
            loadingScreen = new LoadingScreen();
            modelClass.map.twitter.startTwitterSearch += loadingScreen.ShowLoadingScreen;
            modelClass.map.twitter.doneTwitterSearch += loadingScreen.HideLoadingScreen;

            refresh_waypoints_button.Hide();
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

        #region SelectDestrictButtonOnMainMenu_Clicked
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!provinceButtonsCreated)
            {
                try
                {
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
                        province_scroll_panel.Controls.Add(buttonCreate);
                        buttonCreate.Click += ProvinceButton_Click;
                    }
                    modelClass.databaseConnectie.conn.Close();
                    provinceButtonsCreated = true;
                }
                catch (Exception ex)
                {
                    //Laat een bericht zien wanneer er GEEN connectie met de database is gemaakt
                    Console.WriteLine(ex.Message);
                    Label labelCreate = new Label();
                    labelCreate.Width = 200;
                    labelCreate.Height = 200;
                    labelCreate.Text = "Kon geen verbinding maken met de database.";
                    province_scroll_panel.Controls.Add(labelCreate);
                }
            }
            main_menu_tabcontrol.SelectTab(1);
        }
        #endregion

        private void go_to_main_menu_panel_button_Click(object sender, EventArgs e)
        {
            main_menu_tabcontrol.SelectTab(0);
        }

        #region GeneratedProvinceButton_Clicked
        //Kijkt of er een ProvinceGenerated Button is ingedrukt.
        public void ProvinceButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            //Test writeline later verwijderen
            Console.WriteLine(clickedButton.Text.ToString());
            if (!cityButtonsCreated)
            {
                try
                {
                    //Alles opschonen
                    city_scroll_panel.Controls.Clear();

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
                catch (Exception ex)
                {
                    //Laat een bericht zien wanneer er GEEN connectie met de database is gemaakt
                    Console.WriteLine(ex.Message);
                    Label labelCreate = new Label();
                    labelCreate.Width = 200;
                    labelCreate.Height = 200;
                    labelCreate.Text = "Kon geen verbinding maken met de database.";
                    province_scroll_panel.Controls.Add(labelCreate);
                }
            }

            main_menu_tabcontrol.SelectTab(2);
        }
        #endregion

        #region GeneratedCityButton_Clicked
        //Kijkt of er een CityGenerated Button is ingedrukt.
        public void CityButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            //Test writeline later verwijderen
            Console.WriteLine(clickedButton.Text.ToString());
            if (!districtButtonsCreated)
            {
                try
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
                catch (Exception ex)
                {
                    //Laat een bericht zien wanneer er GEEN connectie met de database is gemaakt
                    Console.WriteLine(ex.Message);
                    Label labelCreate = new Label();
                    labelCreate.Width = 200;
                    labelCreate.Height = 200;
                    labelCreate.Text = "Kon geen verbinding maken met de database.";
                    province_scroll_panel.Controls.Add(labelCreate);
                }
            }

            main_menu_tabcontrol.SelectTab(3);
        }
        #endregion

        #region GeneratedDistrictButton_Clicked
        //Kijkt of er een DistrictGenerated Button is ingedrukt.
        public void DistrictButton_Click(object sender, EventArgs e)
        {

            //twitterTrendingList
            List<string> trendingTweetWord = new List<string>();

            twitter_messages_scroll_panel.Controls.Clear();
            Button clickedButton = (Button)sender;
            //Test writeline later verwijderen
            Console.WriteLine(clickedButton.Text.ToString());
            int idDistrict = Convert.ToInt32(clickedButton.Name);
            List<double> latitudeList = new List<double>();
            List<double> longtitudeList = new List<double>();

            //Open database connectie
            modelClass.databaseConnectie.conn.Open();

            //Selectie Query die de namen van allke province selecteer en ordered.
            string stm = "SELECT * FROM coordinate WHERE iddistrict = @iddistrict ORDER BY idcoordinate DESC";
            MySqlCommand cmd = new MySqlCommand(stm, modelClass.databaseConnectie.conn);
            cmd.Parameters.AddWithValue("@iddistrict", idDistrict);
            modelClass.databaseConnectie.rdr = cmd.ExecuteReader();

            // Hier word de database lijst uitgelezen
            while (modelClass.databaseConnectie.rdr.Read())
            {
                latitudeList.Add(Convert.ToDouble(modelClass.databaseConnectie.rdr.GetString(2)));
                longtitudeList.Add(Convert.ToDouble(modelClass.databaseConnectie.rdr.GetString(3)));
            }
            modelClass.map.changeDistrict(latitudeList, longtitudeList);

            if (!modelClass.map.twitter.tweetsList.Any())
            {
                string infoMessage = ("Er zijn geen tweets in deze wijk.");
                Label tweetMessageLabel = new Label();
                tweetMessageLabel.Text = infoMessage;
                twitterLabelLayout(tweetMessageLabel);
                twitter_messages_scroll_panel.Controls.Add(tweetMessageLabel);
            }
            else
            {
                //twitter trending
                var _tekst = "";

                foreach (var tweets in modelClass.map.twitter.tweetsList)
                {
                    _tekst += tweets.message + " ";
                }

                var words =
                Regex.Split(_tekst.ToLower(), @"\W+")
                .Where(s => s.Length > 3)
                .GroupBy(s => s)
                .OrderByDescending(g => g.Count());

                foreach(var word in words){
                    trendingTweetWord.Add(word.Key);
                }

                //WEGVOORDEMO!!!!
                //twitter_trending_topic_label.Text = "Trending topics:\n" + "1: " + trendingTweetWord[0] + "\n2: " + trendingTweetWord[1] + "\n3: " + trendingTweetWord[2];


                //twitter aanroep
                foreach (var tweets in modelClass.map.twitter.tweetsList)
                {
                    string tweetMessage = tweets.user + "\n" + tweets.message + "\n" + tweets.date;
                    foreach (string link in tweets.links)
                    {
                        tweetMessage += "\n" + link;
                    }
                    Label tweetMessageLabel = new Label();
                    tweetMessageLabel.Text = tweetMessage;
                    tweetMessageLabel.Name = Convert.ToString(tweets.id);
                    twitterLabelLayout(tweetMessageLabel);

                    //Als de muis over twitter label hovert wordt die goud.
                    tweetMessageLabel.MouseEnter += on_enter_hover_twitter_message;
                    tweetMessageLabel.MouseLeave += on_exit_hover_twitter_message;

                    //WEGVOORDEMO!!!!
                    //twitter_messages_scroll_panel.Controls.Add(tweetMessageLabel);
                }
            }

            modelClass.databaseConnectie.conn.Close();

            main_menu_tabcontrol.SelectTab(0);

            //WEGVOORDEMO!!!!
            //Controleerd of er een wijk is geselecteerd
            //if (modelClass.map.districtSelected)
            //    refresh_waypoints_button.Show();
        }
        #endregion

        #region BackToProvincePanelFromCityPanelButton_Clicked
        //Als de terug button wordt ingedruk op de city tab
        private void go_to_province_panel_button_from_city_tab_Click(object sender, EventArgs e)
        {
            //cleared alles in city scroll panel
            city_scroll_panel.Controls.Clear();
            main_menu_tabcontrol.SelectTab(1);
            cityButtonsCreated = false;
        }
        #endregion

        #region BackToCityPanelFromDistrictPanelButton_Clicked
        private void go_to_city_panel_button_from_district_tab_Click(object sender, EventArgs e)
        {
            //cleared alles in stad scroll panel
            district_scroll_panel.Controls.Clear();
            main_menu_tabcontrol.SelectTab(2);
            districtButtonsCreated = false;
        }
        #endregion

        #region GeneratedButtonStyle_Method
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
        #endregion

        #region GeneratedTextBoxStyle_Method
        private void twitterLabelLayout(Label _label)
        {
            int twitterLabelSizeX = 275;
            int twitterLabelSizeY = 0;
            _label.AutoSize = true;
            _label.MinimumSize = new Size(twitterLabelSizeX, twitterLabelSizeY);
            _label.MaximumSize = new Size(twitterLabelSizeX, twitterLabelSizeY);
            _label.Font = new Font("Calibri", 16);
            _label.BorderStyle = BorderStyle.Fixed3D;
            _label.ForeColor = Color.White;
            _label.BackColor = policeBlue;
            _label.Dock = DockStyle.Top;
        }
        #endregion

        #region RefreshButton_Clicked
        private void refresh_waypoints_button_Click(object sender, EventArgs e)
        {
            if (OnRefreshButtonClick != null)
                OnRefreshButtonClick();
        }
        #endregion

        #region OnHoverTwitterMessage
        private void on_enter_hover_twitter_message(object sender, EventArgs e)
        {
            Label hoverTweet = (Label)sender;
            hoverTweet.BackColor = policeGold;

        }
        private void on_exit_hover_twitter_message(object sender, EventArgs e)
        {
            Label hoverTweet = (Label)sender;
            hoverTweet.BackColor = policeBlue;

        }
        #endregion
    }
}

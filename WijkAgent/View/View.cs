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
using System.IO;

namespace WijkAgent
{
    public delegate void VoidWithNoArguments();

    public partial class View : Form
    {
        public ModelClass modelClass;
        private bool provinceButtonsCreated = false;
        private int buttonSizeX;
        private int buttonSizeY;
        private Color policeBlue;
        private Color policeGold;
        private Font mainFont;
        private LoadingScreen loadingScreen;

        // laats geklikte label
        private Label lastClickedLabel;

        // placeholders
        private string searchDistrict = "Zoek een wijk . . .";
        private string searchUser = "Zoek een gebruiker . . .";
        private string searchKeyWord = "Zoek een trefwoord . . .";
        private string emptyString = "";
        private string noTagsMessage = "Er zijn geen tags getweet.";
        private string emptyAdjacentDistrict = "Er zijn geen omliggende \nwijken beschikbaar.";


        private int lastTagLabelSelected;
        private string selectedTagLabelText;

        // events
        public event VoidWithNoArguments OnRefreshButtonClick;
        public event VoidWithNoArguments OnLogOutButtonClick;
        public event VoidWithNoArguments OnCleanDistrictTweetsButtonClick;
        public event TwitterSearch doneTwitterSearch;

        #region Constructor
        public View(string _username)
        {
            selectedTagLabelText = emptyString;
            modelClass = new ModelClass(_username);
            policeBlue = Color.FromArgb(0, 70, 130);
            policeGold = Color.FromArgb(190, 150, 90);
            mainFont = new Font("Calibri", 16, FontStyle.Bold);
            buttonSizeX = 300;
            buttonSizeY = 75;
            InitializeComponent();
            this.SetTopLevel(true);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Maximized;

            // init functie aanroepen
            modelClass.map.initialize();

            // wb is de webbrowser waar de map in staat
            // Ook even dezelfde breedte/hoogte geven
            modelClass.map.wb.Dock = DockStyle.Fill;
            map_panel.Controls.Add(modelClass.map.wb);

            // voegt methodes van Loading class toe aan de events in de Twitter class
            loadingScreen = new LoadingScreen();
            modelClass.map.twitter.startTwitterSearch += loadingScreen.ShowLoadingScreen;
            doneTwitterSearch += loadingScreen.HideLoadingScreen;

            refresh_waypoints_button.Hide();

            // welkomstbericht voor gebruiker
            main_menu_label.Text = "Welkom, \n" + getUser();

            // laatst geselecteerde wijk openen
            GoToLatestDistrictFromUser();
        }
        #endregion

        #region View Load
        private void View_Load(object sender, EventArgs e)
        {
            fillSearchSuggestions();
        }
        #endregion

        #region SelectDestrictButtonOnMainMenu_Clicked
        private void button1_Click_1(object sender, EventArgs e)
        {
            map_tabcontrol.SelectTab(0);
            twitter_tabcontrol.SelectTab(0);

            if (modelClass.map.districtSelected)
            {
                try
                {
                    save_incedents_button.Show();
                    main_menu_area_district_scrollable_panel.Show();
                    main_menu_selected_district_panel.Show();
                    clean_district_tweets_button.Show();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            if (!provinceButtonsCreated)
            {
                // open database connectie
                modelClass.databaseConnectie.conn.Open();
                try
                {
                    // selectie query die de namen van alle province selecteer en ordered.
                    string stm = "SELECT * FROM province ORDER BY name DESC";
                    MySqlCommand cmd = new MySqlCommand(stm, modelClass.databaseConnectie.conn);
                    modelClass.databaseConnectie.rdr = cmd.ExecuteReader();

                    // hier word de database lijst uitgelezen
                    while (modelClass.databaseConnectie.rdr.Read())
                    {
                        Button buttonCreate = new Button();
                        buttonCreate.Text = modelClass.databaseConnectie.rdr.GetString(1);
                        buttonCreate.Name = modelClass.databaseConnectie.rdr.GetString(0).ToLower();
                        buttonLayout(buttonCreate);
                        province_scroll_panel.Controls.Add(buttonCreate);
                        buttonCreate.Click += ProvinceButton_Click;
                    }

                    provinceButtonsCreated = true;
                }
                catch (Exception ex)
                {
                    // laat een bericht zien wanneer er GEEN connectie met de database is gemaakt
                    Console.WriteLine(ex.Message);
                    Label labelCreate = new Label();
                    labelCreate.Width = 200;
                    labelCreate.Height = 200;
                    labelCreate.Text = "Kon geen verbinding maken met de database.";
                    province_scroll_panel.Controls.Add(labelCreate);
                }
                // sluit database connectie
                modelClass.databaseConnectie.conn.Close();
            }
            main_menu_tabcontrol.SelectTab(1);
        }
        #endregion

        #region BackToMainMenuPanelButton_Clicked
        private void go_to_main_menu_panel_button_Click(object sender, EventArgs e)
        {
            main_menu_tabcontrol.SelectTab(0);
        }
        #endregion

        #region GeneratedProvinceButton_Clicked
        // kijkt of er een ProvinceGenerated button is ingedrukt.
        public void ProvinceButton_Click(object sender, EventArgs e)
        {
            // alles opschonen
            city_scroll_panel.Controls.Clear();

            Button clickedButton = (Button)sender;

            // open database connectie
            modelClass.databaseConnectie.conn.Open();
            try
            {
                int idProvince = Convert.ToInt32(clickedButton.Name);

                // selectie query die de namen van alle provincies selecteert en ordered.
                string stm = "SELECT * FROM city WHERE idprovince = @idprovince ORDER BY name DESC";
                MySqlCommand cmd = new MySqlCommand(stm, modelClass.databaseConnectie.conn);
                cmd.Parameters.AddWithValue("@idprovince", idProvince);
                modelClass.databaseConnectie.rdr = cmd.ExecuteReader();

                // hier word de database lijst uitgelezen
                while (modelClass.databaseConnectie.rdr.Read())
                {
                    Button buttonCreate = new Button();
                    buttonCreate.Text = modelClass.databaseConnectie.rdr.GetString(2);
                    buttonCreate.Name = modelClass.databaseConnectie.rdr.GetString(0).ToLower();
                    buttonLayout(buttonCreate);
                    city_scroll_panel.Controls.Add(buttonCreate);
                    buttonCreate.Click += CityButton_Click;
                }
                if (city_scroll_panel.Controls.Count == 0)
                {
                    Label label = new Label();
                    label.Text = "Er zijn geen steden gevonden bij deze provincie.";
                    twitterLabelLayout(label);
                    city_scroll_panel.Controls.Add(label);
                    label.Dock = DockStyle.Top;
                }
            }
            catch (Exception ex)
            {
                // laat een bericht zien wanneer er GEEN connectie met de database is gemaakt
                Console.WriteLine(ex.Message);
                Label labelCreate = new Label();
                labelCreate.Width = 200;
                labelCreate.Height = 200;
                labelCreate.Text = "Kon geen verbinding maken met de database.";
                province_scroll_panel.Controls.Add(labelCreate);
            }
            // sluit database connectie
            modelClass.databaseConnectie.conn.Close();

            main_menu_tabcontrol.SelectTab(2);
        }
        #endregion

        #region GeneratedCityButton_Clicked
        // kijkt of er een CityGenerated button is ingedrukt.
        public void CityButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            district_scroll_panel.Controls.Clear();

            // open database connectie
            modelClass.databaseConnectie.conn.Open();
            try
            {
                int idCity = Convert.ToInt32(clickedButton.Name);

                // selectie query die de namen van alle provincies selecteert en ordered.
                string stm = "SELECT * FROM district WHERE idcity = @idcity ORDER BY name DESC";
                MySqlCommand cmd = new MySqlCommand(stm, modelClass.databaseConnectie.conn);
                cmd.Parameters.AddWithValue("@idcity", idCity);
                modelClass.databaseConnectie.rdr = cmd.ExecuteReader();

                // hier wordt de database lijst uitgelezen
                while (modelClass.databaseConnectie.rdr.Read())
                {
                    Button buttonCreate = new Button();
                    buttonCreate.Text = modelClass.databaseConnectie.rdr.GetString(2);
                    buttonCreate.Name = modelClass.databaseConnectie.rdr.GetString(0).ToLower();
                    buttonLayout(buttonCreate);
                    district_scroll_panel.Controls.Add(buttonCreate);
                    buttonCreate.Click += DistrictButton_Click;
                }
            }
            catch (Exception ex)
            {
                // laat een bericht zien wanneer er GEEN connectie met de database is gemaakt
                Console.WriteLine(ex.Message);
                Label labelCreate = new Label();
                labelCreate.Width = 200;
                labelCreate.Height = 200;
                labelCreate.Text = "Kon geen verbinding maken met de database.";
                province_scroll_panel.Controls.Add(labelCreate);
            }
            // sluit database connectie
            modelClass.databaseConnectie.conn.Close();

            main_menu_tabcontrol.SelectTab(3);
        }
        #endregion

        #region GeneratedDistrictButton_Clicked
        // kijkt of er een DistrictGenerated button is ingedrukt.
        public void DistrictButton_Click(object sender, EventArgs e)
        {

            ResetClickEventTwitterTag();

            twitter_messages_scroll_panel.Controls.Clear();

            Button clickedButton = (Button)sender;

            // id van wijk ophalen
            modelClass.map.idDistrict = Convert.ToInt32(clickedButton.Name);

            //mag alleen aborten als er al een district geselecteerd is
            if(modelClass.map.districtSelected == true)
            {
                modelClass.map.mapThread.Abort();
            }
            
            // wijk veranderen
            modelClass.ChangeDistrict();

            // verander geselecteerde label text
            ChangeSelectedDistrictText(clickedButton.Text);

            // twitter panel updaten
            UpdateTwitterpanel();

            // laat zien wat nodig is(refresh knop)
            ShowWhatsNeeded();

            main_menu_tabcontrol.SelectTab(0);
        }
        #endregion

        #region BackToProvincePanelFromCityPanelButton_Clicked
        // als de terug button wordt ingedruk op de city tab
        private void go_to_province_panel_button_from_city_tab_Click(object sender, EventArgs e)
        {
            // cleared alles in city scroll panel
            city_scroll_panel.Controls.Clear();
            main_menu_tabcontrol.SelectTab(1);
        }
        #endregion

        #region BackToCityPanelFromDistrictPanelButton_Clicked
        private void go_to_city_panel_button_from_district_tab_Click(object sender, EventArgs e)
        {
            // cleared alles in stad scroll panel
            district_scroll_panel.Controls.Clear();
            main_menu_tabcontrol.SelectTab(2);
        }
        #endregion

        #region GeneratedButtonStyle_Method
        private void buttonLayout(Button _button)
        {
            _button.Size = new Size(buttonSizeX, buttonSizeY);
            _button.Dock = DockStyle.Top;
            _button.BackColor = policeBlue;
            _button.ForeColor = Color.White;
            _button.Font = mainFont;
            _button.FlatStyle = FlatStyle.Flat;
            _button.FlatAppearance.BorderColor = policeGold;
            _button.FlatAppearance.BorderSize = 2;
        }
        #endregion

        #region GeneratedPanelStyle_Method
        private void panelLayout(Panel _panel)
        {
            _panel.AutoSize = true;
            _panel.Dock = DockStyle.Top;
            _panel.BackColor = Color.White;
            _panel.BorderStyle = BorderStyle.Fixed3D;
        }
        #endregion

        #region GeneratedLabelStyle_Method
        private void labelLayout(Label _label)
        {
            _label.AutoSize = true;
            _label.Dock = DockStyle.Fill;
            _label.ForeColor = policeBlue;
            _label.Font = mainFont;
        }
        #endregion

        #region GeneratedTextBoxStyle_Method
        private void twitterLabelLayout(Label _label)
        {
            int twitterLabelSizeX = 300;
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

            if (doneTwitterSearch != null)
            {
                doneTwitterSearch();
            }
            refresh_waypoints_button.Hide();

        }
        #endregion

        #region RefreshButton_unhide
        public void RefreshThreatAction()
        {
            refresh_waypoints_button.Show();
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
            if (hoverTweet != lastClickedLabel)
            {
                hoverTweet.BackColor = policeBlue;
            }
        }
        #endregion

        #region OnTwitterMessageClick
        private void TweetMessageOnClick(object sender, EventArgs e)
        {
            if (lastClickedLabel != null)
            {
                lastClickedLabel.BackColor = policeBlue;
            }

            Label _label = (Label)sender;
            lastClickedLabel = _label;
            lastClickedLabel.BackColor = policeGold;

            // label naam is het id van de tweet,
            // maar ik wil het in een int hebben, dus zet ik hem om
            int _labelId = Int32.Parse(_label.Name);

            // kleur veranderen van de label
            modelClass.map.hightlightMarker(_labelId);
        }
        #endregion

        #region View_Closed
        private void View_FormClosed(object sender, FormClosedEventArgs e)
        {
            //HIER MICHELLA!!!!! <-------
            modelClass.databaseConnectie.ChangeAccountLocation(modelClass.username, null, null);
            Environment.Exit(0);
        }
        #endregion

        #region Go_to_history_panel_button_from_main_menu_tab_Click
        private void go_to_history_panel_button_from_main_menu_tab_Click(object sender, EventArgs e)
        {
            map_tabcontrol.SelectTab(1);
            twitter_tabcontrol.SelectTab(1);

            try
            {
                save_incedents_button.Hide();
                main_menu_area_district_scrollable_panel.Hide();
                main_menu_selected_district_panel.Hide();
                clean_district_tweets_button.Hide();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion

        #region Placeholders(enter en leaver events van textbox)

        private void history_district_textbox_Enter(object sender, EventArgs e)
        {
            if (history_district_textbox.Text == searchDistrict)
            {
                history_district_textbox.Text = "";
                history_district_textbox.ForeColor = Color.Black;
            }
        }

        private void history_district_textbox_Leave(object sender, EventArgs e)
        {
            if (!history_district_textbox.Text.Any())
            {
                history_district_textbox.ForeColor = Color.DimGray;
                history_district_textbox.Text = searchDistrict;
            }
        }

        private void history_user_textbox_Enter(object sender, EventArgs e)
        {
            if (history_user_textbox.Text == searchUser)
            {
                history_user_textbox.Text = "";
                history_user_textbox.ForeColor = Color.Black;
            }
        }

        private void history_user_textbox_Leave(object sender, EventArgs e)
        {
            if (!history_user_textbox.Text.Any())
            {
                history_user_textbox.ForeColor = Color.DimGray;
                history_user_textbox.Text = searchUser;
            }
        }

        private void history_keyword_textbox_Enter(object sender, EventArgs e)
        {
            if (history_keyword_textbox.Text == searchKeyWord)
            {
                history_keyword_textbox.Text = "";
                history_keyword_textbox.ForeColor = Color.Black;
            }
        }

        private void history_keyword_textbox_Leave(object sender, EventArgs e)
        {
            if (!history_keyword_textbox.Text.Any())
            {
                history_keyword_textbox.ForeColor = Color.DimGray;
                history_keyword_textbox.Text = searchKeyWord;
            }
        }
        #endregion

        #region TwitterTrending
        public void TwitterTrending()
        {
            //twitterTrendingList
            List<string> trendingTags = new List<string>();

            //Haal tags op
            trendingTags = modelClass.map.twitter.TrendingTags();
            int trendingTagCounter = 0;
            if (trendingTags.Count() > 0)
            {
                twitter_trending_tag_label.Text = "Trending tags:";
                foreach (string tag in trendingTags)
                {
                    trendingTagCounter++;
                    if (trendingTagCounter == 1)
                    {
                        twitter_taglabel1.Text = "1: " + tag;
                        twitter_taglabel2.Text = emptyString;
                        twitter_taglabel3.Text = emptyString;
                    }
                    else if (trendingTagCounter == 2)
                    {
                        twitter_taglabel2.Text = "2: " + tag;
                        twitter_taglabel3.Text = emptyString;

                    }
                    else if (trendingTagCounter == 3)
                    {
                        twitter_taglabel3.Text = "3: " + tag;
                    }
                }

            }
            //Zet alles in default waarden.
            else
            {
                twitter_trending_tag_label.Text = noTagsMessage;
                twitter_taglabel1.Text = emptyString;
                twitter_taglabel2.Text = emptyString;
                twitter_taglabel3.Text = emptyString;
            }
        }
        #endregion

        #region Create suggetions for all filters
        public void fillSearchSuggestions()
        {
            // roep district naam suggeties aan.
            // open database connectie
            modelClass.databaseConnectie.conn.Open();

            // selectie query die de namen van elke provincie selecteert en ordered.
            string stm = "SELECT DISTINCT name FROM district";
            MySqlCommand cmd = new MySqlCommand(stm, modelClass.databaseConnectie.conn);
            modelClass.databaseConnectie.rdr = cmd.ExecuteReader();

            // hier wordt de database lijst uitgelezen
            while (modelClass.databaseConnectie.rdr.Read())
            {
                history_district_textbox.AutoCompleteCustomSource.Add(modelClass.databaseConnectie.rdr.GetString(0));
            }

            // sluit database connectie
            modelClass.databaseConnectie.conn.Close();

            // roep twitter user suggeties aan
            // open database connectie
            modelClass.databaseConnectie.conn.Open();

            // selectie query die de namen van elke provincie selecteert en ordered.
            string stm2 = "SELECT DISTINCT user FROM twitter";
            MySqlCommand cmd2 = new MySqlCommand(stm2, modelClass.databaseConnectie.conn);
            modelClass.databaseConnectie.rdr = cmd2.ExecuteReader();

            // hier wordt de database lijst uitgelezen
            while (modelClass.databaseConnectie.rdr.Read())
            {
                history_user_textbox.AutoCompleteCustomSource.Add(modelClass.databaseConnectie.rdr.GetString(0));
            }

            // sluit database connectie
            modelClass.databaseConnectie.conn.Close();

            history_category_combobox.Text = "test";

            // roep twitter user suggeties aan
            // open database connectie
            modelClass.databaseConnectie.conn.Open();

            // selectie query die de namen van elke provincie selecteert en ordered
            string stm3 = "SELECT DISTINCT name FROM category ORDER BY name";
            MySqlCommand cmd3 = new MySqlCommand(stm3, modelClass.databaseConnectie.conn);
            modelClass.databaseConnectie.rdr = cmd3.ExecuteReader();

            // hier wordt de database lijst uitgelezen
            while (modelClass.databaseConnectie.rdr.Read())
            {
                history_category_combobox.Items.Add(modelClass.databaseConnectie.rdr.GetString(0).First().ToString().ToUpper() + String.Join("", modelClass.databaseConnectie.rdr.GetString(0).Skip(1)));
            }

            // sluit database connectie
            modelClass.databaseConnectie.conn.Close();
        }
        #endregion

        #region Min/max for datepicker
        private void history_from_datetimepicker_ValueChanged(object sender, EventArgs e)
        {
            history_till_datetimepicker.MinDate = history_from_datetimepicker.Value;
        }

        private void history_till_datetimepicker_ValueChanged(object sender, EventArgs e)
        {
            history_from_datetimepicker.MaxDate = history_till_datetimepicker.Value;
        }
        #endregion

        #region GetNameOfUser
        public string getUser()
        {
            // open database connectie
            modelClass.databaseConnectie.conn.Open();

            // haal idAccount op
            string stm = "SELECT person.naam, person.achternaam FROM account JOIN person ON account.idaccount = person.idaccount WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(stm, modelClass.databaseConnectie.conn);
            cmd.Parameters.AddWithValue("@username", modelClass.username);
            modelClass.databaseConnectie.rdr = cmd.ExecuteReader();
            modelClass.databaseConnectie.rdr.Read();
            string fullName = modelClass.databaseConnectie.rdr.GetString(0) + " " + modelClass.databaseConnectie.rdr.GetString(1);

            // sluit database connectie
            modelClass.databaseConnectie.conn.Close();

            return fullName;
        }
        #endregion

        #region UpdateLatestSelectedDisctrictUser
        public void UpdateLatestSelectedDisctrictUser()
        {
            // default wijk opslaan van gebruiker
            modelClass.databaseConnectie.SaveDefaultDistrictUser(modelClass.username, modelClass.map.idDistrict);
        }
        #endregion

        #region Filter and show twitter results from database
        private void history_search_button_Click(object sender, EventArgs e)
        {
            int resultsCount = 0;
            int resultMax = 75;
            history_scroll_panel.Controls.Clear();
            string districtInput = history_district_textbox.Text;
            string userInput = history_user_textbox.Text;
            string categoryInput = history_category_combobox.Text;
            string keyWordInput = history_keyword_textbox.Text;
            string stm = "";
            string headerResults = "Aantal resultaten: ";
            string historyHeaderDefault = "Geschiedenis van: ";
            string nothingFoundMessage = "Geen resultaten gevonden.";
            string noFilterSelectedMessage = "U heeft geen filter gekozen.";
            DateTime fromDateInput = history_from_datetimepicker.Value;
            DateTime tillDateInput = history_till_datetimepicker.Value;

            // hier wordt een standaard search query aangemaakt
            stm = modelClass.databaseConnectie.AddSelectTwitterToQuery(stm);
            string tempSearch = "Geschiedenis van: " + Environment.NewLine + Environment.NewLine;

            // als district checkbox checked is
            // wordt er een join gemaakt naar de collumn van district
            if (history_district_checkbox.Checked)
            {
                stm = modelClass.databaseConnectie.JoinDistrictQuery(stm);
            }

            // als categorie checkbox checked is 
            // wordt er een join gemaakt naar de collumn van categorie
            if (history_category_checkbox.Checked && history_category_combobox.SelectedIndex > -1)
            {
                stm = modelClass.databaseConnectie.JoinCatgoryQuery(stm);
            }
            //Van af hier begint de WHERE van de query.
            if (history_district_checkbox.Checked || history_user_checkbox.Checked || (history_category_checkbox.Checked && history_category_combobox.SelectedIndex > -1) || history_date_checkbox.Checked || history_keyword_checkbox.Checked)
            {
                stm = modelClass.databaseConnectie.AddWhereToQuery(stm);
            }

            // als district checkbox checked is wordt input van district toegevoegd aan de query
            if (history_district_checkbox.Checked)
            {
                tempSearch = tempSearch + "Wijk: " + districtInput + Environment.NewLine;
                stm = modelClass.databaseConnectie.WhereDistrictQuery(stm);
            }

            // als district checkbox checked is wordt input van user toegevoegd aan de query
            if (history_user_checkbox.Checked)
            {
                tempSearch = tempSearch + "Gebruiker: " + userInput + Environment.NewLine;
                if (history_district_checkbox.Checked)
                {
                    stm = modelClass.databaseConnectie.AddAndToQuery(stm);
                }
                stm = modelClass.databaseConnectie.WhereUserQuery(stm);
            }

            // als district checkbox checked is word input van catgorie toegevoegd aan de query
            if (history_category_checkbox.Checked && history_category_combobox.SelectedIndex > -1)
            {
                if (history_district_checkbox.Checked || history_user_checkbox.Checked)
                {
                    stm = modelClass.databaseConnectie.AddAndToQuery(stm);
                }
                stm = modelClass.databaseConnectie.WhereCategoryQuery(stm);
            }
            if (history_category_checkbox.Checked)
            {
                tempSearch = tempSearch + "Categorie: " + categoryInput + Environment.NewLine;
            }

            if (history_keyword_checkbox.Checked)
            {
                tempSearch = tempSearch + "Trefwoord: " + keyWordInput + Environment.NewLine;
                if (history_district_checkbox.Checked || history_user_checkbox.Checked || history_category_checkbox.Checked)
                {
                    stm = modelClass.databaseConnectie.AddAndToQuery(stm);
                }
                stm = modelClass.databaseConnectie.WhereKeyWordQuery(stm);
            }

            // als district checkbox checked is wordt input van date toegevoegd aan de query
            if (history_date_checkbox.Checked)
            {
                tempSearch = tempSearch + "Datum van: " + fromDateInput.ToString("yyyy-MM-dd 00:00:0001") + " tot: " + tillDateInput.ToString("yyyy-MM-dd 23:59:0000");
                if (history_district_checkbox.Checked || history_user_checkbox.Checked || history_category_checkbox.Checked || history_keyword_checkbox.Checked)

                {
                    stm = modelClass.databaseConnectie.AddAndToQuery(stm);
                }
                string tempDateWhereQuery = "twitter.datetime BETWEEN '" + fromDateInput.ToString("yyyy-MM-dd ") + " 00:00:01.000000' AND '" + tillDateInput.ToString("yyyy-MM-dd") + " 23:59:59.000000'";
                stm = stm + tempDateWhereQuery;
            }

            // hier wordt alles geordent op datum zodat de nieuwste datum bovenaan komt
            stm = modelClass.databaseConnectie.AddOrderByTimeToQuery(stm);
            stm = modelClass.databaseConnectie.AddLimitToQeury(stm, resultMax);

            // check of er uberhaupt een checkbox gecheckt is
            if (history_district_checkbox.Checked || history_user_checkbox.Checked || (history_category_checkbox.Checked && history_category_combobox.SelectedIndex > -1) || history_date_checkbox.Checked || history_keyword_checkbox.Checked)
            {

                // open database connectie
                modelClass.databaseConnectie.conn.Open();
                try
                {
                    // roep districte naam suggeties aan

                    // selectie query die de namen van allke province selecteer en ordered
                    MySqlCommand cmd = new MySqlCommand(stm, modelClass.databaseConnectie.conn);
                    cmd.Parameters.AddWithValue("@districtInput", districtInput);
                    cmd.Parameters.AddWithValue("@userInput", userInput);
                    if (history_category_combobox.SelectedIndex > -1)
                    {
                        cmd.Parameters.AddWithValue("@categoryInput", categoryInput.ToLower());
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@categoryInput", "Er is geen catgorie geselecteert.");
                    }
                    cmd.Parameters.AddWithValue("@keyWordInput", "%" + keyWordInput + "%");
                    cmd.Parameters.AddWithValue("@fromDateInput", fromDateInput.ToString("yyyy-MM-dd 00:00:0001"));
                    cmd.Parameters.AddWithValue("@tillDateInput", tillDateInput.ToString("yyyy-MM-dd 23:59:0000"));
                    modelClass.databaseConnectie.rdr = cmd.ExecuteReader();

                    // hier wordt de database lijst uitgelezen
                    while (modelClass.databaseConnectie.rdr.Read())
                    {
                        // teller wordt geupdate per resultaat
                        resultsCount++;

                        // text wordt hier aangemaakt voor elke label
                        string tempLabelText;
                        tempLabelText = ("Gebruiker: " + modelClass.databaseConnectie.rdr.GetString(3) + Environment.NewLine
                                        + "Twitter bericht: " + Environment.NewLine + modelClass.databaseConnectie.rdr.GetString(6) + Environment.NewLine
                                        + Environment.NewLine + "Datum: " + modelClass.databaseConnectie.rdr.GetString(7) + Environment.NewLine);

                        // maak panel om straks de labels in te bewaren
                        Panel createHistoryPanel = new Panel();
                        createHistoryPanel.Name = modelClass.databaseConnectie.rdr.GetString(0).ToString();
                        panelLayout(createHistoryPanel);

                        // panel wordt toegevoegd aan scroll panel van history
                        history_scroll_panel.Controls.Add(createHistoryPanel);

                        // hier wordt de label aangemaakt om alle info van database in te printen
                        Label createHistorylabel = new Label();
                        createHistorylabel.Name = modelClass.databaseConnectie.rdr.GetString(0).ToString();
                        createHistorylabel.Text = tempLabelText;
                        labelLayout(createHistorylabel);

                        // label wordt toegevoegd aan panel
                        createHistoryPanel.Controls.Add(createHistorylabel);

                    }

                    if (resultsCount == 0)
                    {
                        Label createNoResultAlert = new Label();
                        createNoResultAlert.Text = nothingFoundMessage;
                        labelLayout(createNoResultAlert);
                        history_scroll_panel.Controls.Add(createNoResultAlert);
                    }

                    // hier wordt de resultaat label geupdate met het aantal resultaten
                    history_header_results_label.Text = "Aantal resultaten: " + resultsCount.ToString();

                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex);
                }

                // sluit database connectie
                modelClass.databaseConnectie.conn.Close();
                // header label wordt geupdate met de zoek resultaten die zijn gebruikt
                History_header_label.Text = tempSearch;

            }
            else
            {
                Label createEmptyAlert = new Label();
                createEmptyAlert.Text = noFilterSelectedMessage;
                History_header_label.Text = historyHeaderDefault;
                history_header_results_label.Text = headerResults;
                labelLayout(createEmptyAlert);
                history_scroll_panel.Controls.Add(createEmptyAlert);
            }

        }
        #endregion

        #region Update the new tweets label
        public void UpdateNewTweetsLabel()
        {
            twitter_number_of_new_tweets_label.Text = "Aantal nieuwe tweets: " + modelClass.newTweets;
        }
        #endregion

        #region LogOut_Button_Click
        private void view_logOut_button_Click(object sender, EventArgs e)
        {
            if (OnLogOutButtonClick != null)
                OnLogOutButtonClick();
        }
        #endregion

        #region Save_Incedents_Button_Click
        private void save_incedents_button_Click(object sender, EventArgs e)
        {
            IncidentScreen incident = new IncidentScreen(modelClass.map.idDistrict);
            incident.ShowDialog();
        }
        #endregion

        #region Update_Twitter_Panel
        public void UpdateTwitterpanel()
        {
            twitter_messages_scroll_panel.Controls.Clear();
            //twitter trending
            if (!modelClass.map.twitter.tweetsList.Any())
            {
                string infoMessage = ("Er zijn geen tweets in deze wijk.");
                Label tweetMessageLabel = new Label();
                tweetMessageLabel.Text = infoMessage;
                twitterLabelLayout(tweetMessageLabel);
                twitter_messages_scroll_panel.Controls.Add(tweetMessageLabel);
                twitter_trending_topic_label.Text = infoMessage;

                //Check of twitter tags zijn.
                TwitterTrending();
            }
            else
            {
                twitter_trending_topic_label.Text = modelClass.map.twitter.TrendingTopics();
                //Omdraaien van de array, zodat de nieuwste bovenaan staan
                modelClass.map.twitter.tweetsList.Reverse();

                if (selectedTagLabelText != "")
                {
                    List<Tweet> filteredTweetList = new List<Tweet>();
                    filteredTweetList = modelClass.map.twitter.getTweetsWithSelectedTag(selectedTagLabelText);
                    //twitter aanroep
                    foreach (var tweets in filteredTweetList)
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
                        //onclick label voor de marker highlight
                        tweetMessageLabel.Click += TweetMessageOnClick;
                        twitter_messages_scroll_panel.Controls.Add(tweetMessageLabel);

                    }
                }
                else
                {
                    //Check of twitter tags zijn.
                    TwitterTrending();
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
                        //onclick label voor de marker highlight
                        tweetMessageLabel.Click += TweetMessageOnClick;
                        twitter_messages_scroll_panel.Controls.Add(tweetMessageLabel);

                    }
                }
            }

            // twitter berichten in database opslaan 
            modelClass.TweetsToDb();

            // aantal nieuwe tweets updaten
            UpdateNewTweetsLabel();

            // standaard wijk van gebruiker updaten
            UpdateLatestSelectedDisctrictUser();
        }
        #endregion

        #region Show_all_whats_needed
        public void ShowWhatsNeeded()
        {
            // controleerd of er een wijk is geselecteerd
            if (modelClass.map.districtSelected)
                refresh_waypoints_button.Show();

            // laat voorvallen/dichtbij liggende wijken knop/panel zien
            try
            {
                save_incedents_button.Show();
                main_menu_area_district_scrollable_panel.Show();
                main_menu_selected_district_panel.Show();
                clean_district_tweets_button.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            // laad scherm verbergen
            if (doneTwitterSearch != null)
                doneTwitterSearch();
        }
        #endregion

        #region Go_to_latest_selected_district_from_user
        public void GoToLatestDistrictFromUser()
        {
            // ophalen van idDistrict
            int idDistrict = modelClass.databaseConnectie.GetLatestSelectedDisctrictFromUser(modelClass.username);
            string districtName = modelClass.databaseConnectie.GetSelectedDistrictName(idDistrict);

            // als idDistrict lager is dan 0 
            // betekend dit dat er geen iddisctrict is opgeslagen bij deze gebruiker
            if (idDistrict > 0)
            {
                modelClass.ChangeDistrict(idDistrict);
                ChangeSelectedDistrictText(districtName);
                UpdateTwitterpanel();
                ShowWhatsNeeded();
                add_buttons_for_adjacent_districts();
            }
        }
        #endregion

        #region ChangeSelectedDistrictText
        public void ChangeSelectedDistrictText(string _districtName)
        {
            main_menu_selected_district_label.Text = "Laatste geselecteerde wijk: " + Environment.NewLine + _districtName;
        }
        #endregion

        #region TagLabel Clickevents
        //Deze events zorgen er voor dat elk label gekleurt wordt en twitter overzicht geupdate word.
        private void twitter_taglabel1_Click(object sender, EventArgs e)
        {
            Label clickedLabel = (Label)sender;
            if (lastTagLabelSelected == 1)
            {
                //Reset alle labels naar zwart en laaste geselecteerde label naar default.
                ResetClickEventTwitterTag();
            }
            else
            {
                twitter_taglabel1.ForeColor = policeGold;
                twitter_taglabel2.ForeColor = Color.Black;
                twitter_taglabel3.ForeColor = Color.Black;
                lastTagLabelSelected = 1;
                selectedTagLabelText = filterStringToTag(clickedLabel.Text);
            }

            //Getoonde twitter bericten updaten
            UpdateTwitterpanel();
        }

        private void twitter_taglabel2_Click(object sender, EventArgs e)
        {
            Label clickedLabel = (Label)sender;
            if (lastTagLabelSelected == 2)
            {
                //Reset alle labels naar zwart en laaste geselecteerde label naar default.
                ResetClickEventTwitterTag();
            }
            else
            {
                twitter_taglabel1.ForeColor = Color.Black;
                twitter_taglabel2.ForeColor = policeGold;
                twitter_taglabel3.ForeColor = Color.Black;
                lastTagLabelSelected = 2;
                selectedTagLabelText = filterStringToTag(clickedLabel.Text);
            }

            //Getoonde twitter bericten updaten
            UpdateTwitterpanel();
        }

        private void twitter_taglabel3_Click(object sender, EventArgs e)
        {
            Label clickedLabel = (Label)sender;
            if (lastTagLabelSelected == 3)
            {
                //Reset alle labels naar zwart en laaste geselecteerde label naar default.
                ResetClickEventTwitterTag();
            }
            else
            {
                twitter_taglabel1.ForeColor = Color.Black;
                twitter_taglabel2.ForeColor = Color.Black;
                twitter_taglabel3.ForeColor = policeGold;
                lastTagLabelSelected = 3;
                selectedTagLabelText = filterStringToTag(clickedLabel.Text);
            }

            //Getoonde twitter bericten updaten
            UpdateTwitterpanel();
        }

        public void ResetClickEventTwitterTag()
        {
            twitter_taglabel1.ForeColor = Color.Black;
            twitter_taglabel2.ForeColor = Color.Black;
            twitter_taglabel3.ForeColor = Color.Black;
            lastTagLabelSelected = 0;
            selectedTagLabelText = emptyString;
        }
        #endregion

        #region Clean_district_button_clicked
        private void clean_district_tweets_button_Click(object sender, EventArgs e)
        {
            if (OnCleanDistrictTweetsButtonClick != null)
                OnCleanDistrictTweetsButtonClick();
        }
        #endregion

        #region Buttons for adjacent districts
        public void add_buttons_for_adjacent_districts()
        {
            // maak de lijst met aanliggende wijken leeg
            main_menu_area_district_scrollable_panel.Controls.Clear();

            // haal alle aanliggende wijken op
            Dictionary<int, string> adjacentDistricts = modelClass.databaseConnectie.GetAllAdjacentDistricts(modelClass.map.idDistrict);
            if (adjacentDistricts.Count != 0)
            {
                foreach (KeyValuePair<int, string> entry in adjacentDistricts)
                {
                    // maak buttons aan als er aanliggende wijken zijn
                    Button buttonCreate = new Button();
                    buttonCreate.Text = entry.Value;
                    Console.WriteLine(entry.Key.ToString());
                    buttonCreate.Name = entry.Key.ToString();
                    buttonLayout(buttonCreate);
                    main_menu_area_district_scrollable_panel.Controls.Add(buttonCreate);
                    buttonCreate.Click += DistrictButton_Click;
                }
            }
            else
            {
                // maak een label met een melding als er geen aanliggende wijken zijn
                Label lab = new Label();
                lab.Text = emptyAdjacentDistrict;
                labelLayout(lab);
                main_menu_area_district_scrollable_panel.Controls.Add(lab);
            }
        }
        #endregion

        #region Filter String to tag
        public string filterStringToTag(string _stringWithTag)
        {
            string returntag = "";

            var strings = Regex.Split(_stringWithTag, @"\s+")
            .Where(a => a.StartsWith("#"));

            foreach (var tag in strings)
            {
                returntag = tag;
            }

            return returntag;
        }
        #endregion
    }
}

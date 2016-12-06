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
        private int panelSizeX;
        private int panelSizeY;
        private Color policeBlue;
        private Color policeGold;
        private Font mainFont;
        private LoadingScreen loadingScreen;
        //laats geklikte label
        private Label lastClickedLabel;

        //maximale trending lengte
        private int tagLengte = 14;
        private int wordLengte = 10;

        //placeholders
        private string searchDistrict = "Zoek een wijk . . .";
        private string searchUser = "Zoek een gebruiker . . .";
        private string searchKeyWord = "Zoek een trefwoord . . .";

        //events
        public event VoidWithNoArguments OnRefreshButtonClick;
        public event VoidWithNoArguments OnLogOutButtonClick;
        public event TwitterSearch doneTwitterSearch;

        public View(string _username)
        {
            modelClass = new ModelClass(_username);
            policeBlue = Color.FromArgb(0, 70, 130);
            policeGold = Color.FromArgb(190, 150, 90);
            mainFont = new Font("Calibri", 16, FontStyle.Bold);
            buttonSizeX = 300;
            buttonSizeY = 75;
            panelSizeX = 300;
            panelSizeY = 250;
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
            doneTwitterSearch += loadingScreen.HideLoadingScreen;

            refresh_waypoints_button.Hide();

            //Welkombericht voor gebruiker
            main_menu_label.Text = "Welkom, \n" + getUser();
        }

        #region View Load
        private void View_Load(object sender, EventArgs e)
        {
            fillSearchSuggestions();

            #region Layout Colours toevoegen.
            main_menu_panel_for_label.BackColor = policeBlue;
            province_panel_for_label.BackColor = policeBlue;
            city_panel_for_label.BackColor = policeBlue;
            district_panel_for_label.BackColor = policeBlue;
            history_option_panel_for_label.BackColor = policeBlue;
            history_header_panel.BackColor = policeBlue;
            twitter_number_of_new_tweets_panel.BackColor = policeGold;
            twitter_panel.BackColor = policeGold;
            //Zoek button
            history_search_button.BackColor = policeBlue;
            history_search_button.ForeColor = Color.White;
            history_search_button.Font = mainFont;

            //history button
            go_to_history_panel_button_from_main_menu_tab.BackColor = policeBlue;
            go_to_history_panel_button_from_main_menu_tab.ForeColor = Color.White;
            go_to_history_panel_button_from_main_menu_tab.Font = mainFont;

            //selecteer wijk
            go_to_province_panel_button_from_main_menu_tab.BackColor = policeBlue;
            go_to_province_panel_button_from_main_menu_tab.ForeColor = Color.White;
            go_to_province_panel_button_from_main_menu_tab.Font = mainFont;

            go_to_main_menu_panel_button.BackColor = policeBlue;
            go_to_main_menu_panel_button.ForeColor = Color.White;
            go_to_main_menu_panel_button.Font = mainFont;

            go_to_province_panel_button_from_city_tab.BackColor = policeBlue;
            go_to_province_panel_button_from_city_tab.ForeColor = Color.White;
            go_to_province_panel_button_from_city_tab.Font = mainFont;

            go_to_city_panel_button_from_district_tab.BackColor = policeBlue;
            go_to_city_panel_button_from_district_tab.ForeColor = Color.White;
            go_to_city_panel_button_from_district_tab.Font = mainFont;

            save_incedents_button.BackColor = policeBlue;
            save_incedents_button.ForeColor = Color.White;
            save_incedents_button.Font = mainFont;
            
            view_logOut_button.BackColor = policeBlue;
            view_logOut_button.ForeColor = Color.White;
            view_logOut_button.Font = mainFont;
            #endregion
        }
        #endregion

        #region SelectDestrictButtonOnMainMenu_Clicked
        private void button1_Click_1(object sender, EventArgs e)
        {
            map_tabcontrol.SelectTab(0);
            twitter_tabcontrol.SelectTab(0);
            if (!provinceButtonsCreated)
            {
                try
                {
                    //Open database connectie
                    modelClass.databaseConnectie.conn.Open();

                    //Selectie Query die de namen van alle province selecteer en ordered.
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

        #region backToMainMenuPanelButton_Clicked
        private void go_to_main_menu_panel_button_Click(object sender, EventArgs e)
        {
            main_menu_tabcontrol.SelectTab(0);
        }
        #endregion

        #region GeneratedProvinceButton_Clicked
        //Kijkt of er een ProvinceGenerated Button is ingedrukt.
        public void ProvinceButton_Click(object sender, EventArgs e)
        {
            //Alles opschonen
            city_scroll_panel.Controls.Clear();

            Button clickedButton = (Button)sender;
            try
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
                if (city_scroll_panel.Controls.Count == 0)
                {
                    Label label = new Label();
                    label.Text = "Er zijn geen steden gevonden bij deze provincie.";
                    twitterLabelLayout(label);
                    city_scroll_panel.Controls.Add(label);
                    label.Dock = DockStyle.Top;
                }

                modelClass.databaseConnectie.conn.Close();
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

            main_menu_tabcontrol.SelectTab(2);
        }
        #endregion

        #region GeneratedCityButton_Clicked
        //Kijkt of er een CityGenerated Button is ingedrukt.
        public void CityButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            district_scroll_panel.Controls.Clear();
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

            main_menu_tabcontrol.SelectTab(3);
        }
        #endregion

        #region GeneratedDistrictButton_Clicked
        //Kijkt of er een DistrictGenerated Button is ingedrukt.
        public void DistrictButton_Click(object sender, EventArgs e)
        {
            //twitterTrendingList
            List<string> trendingTweetWord = new List<string>();
            List<string> trendingTags = new List<string>();

            twitter_messages_scroll_panel.Controls.Clear();
            Button clickedButton = (Button)sender;

            modelClass.map.idDistrict = Convert.ToInt32(clickedButton.Name);
            List<double> latitudeList = new List<double>();
            List<double> longtitudeList = new List<double>();

            //Open database connectie
            modelClass.databaseConnectie.conn.Open();

            //Selectie Query die de namen van allke province selecteer en ordered.
            string stm = "SELECT * FROM coordinate WHERE iddistrict = @iddistrict ORDER BY idcoordinate DESC";
            MySqlCommand cmd = new MySqlCommand(stm, modelClass.databaseConnectie.conn);
            cmd.Parameters.AddWithValue("@iddistrict", modelClass.map.idDistrict);
            modelClass.databaseConnectie.rdr = cmd.ExecuteReader();

            // Hier word de database lijst uitgelezen
            while (modelClass.databaseConnectie.rdr.Read())
            {
                latitudeList.Add(Convert.ToDouble(modelClass.databaseConnectie.rdr.GetString(2)));
                longtitudeList.Add(Convert.ToDouble(modelClass.databaseConnectie.rdr.GetString(3)));
            }
            //Databse connectie sluiten
            modelClass.databaseConnectie.conn.Close();

            modelClass.map.changeDistrict(latitudeList, longtitudeList);

            if (!modelClass.map.twitter.tweetsList.Any())
            {
                string infoMessage = ("Er zijn geen tweets in deze wijk.");
                Label tweetMessageLabel = new Label();
                tweetMessageLabel.Text = infoMessage;
                twitterLabelLayout(tweetMessageLabel);
                twitter_messages_scroll_panel.Controls.Add(tweetMessageLabel);
                twitter_trending_tag_label.Text = "";
                twitter_trending_topic_label.Text = "";
            }
            else
            {
                //twitter trending
                TwitterTrending();

                //Omdraaien van de array, zodat de nieuwste bovenaan staan
                modelClass.map.twitter.tweetsList.Reverse();

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

            //Twitter berichten in database opslaan 
            modelClass.TweetsToDb();

            //Standaart wijk van gebruiker updaten
            UpdateLatestSelectedDisctrictUser();

            //Aantal nieuwe tweets updaten
            UpdateNewTweetsLabel();

            main_menu_tabcontrol.SelectTab(0);

            //Controleerd of er een wijk is geselecteerd
            if (modelClass.map.districtSelected)
                refresh_waypoints_button.Show();

            //laat voorvallen knop zien
            try
            {
                save_incedents_button.Show();
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            //Laad scherm verbergen
            if (doneTwitterSearch != null)
                doneTwitterSearch();
        }
        #endregion

        #region BackToProvincePanelFromCityPanelButton_Clicked
        //Als de terug button wordt ingedruk op de city tab
        private void go_to_province_panel_button_from_city_tab_Click(object sender, EventArgs e)
        {
            //cleared alles in city scroll panel
            city_scroll_panel.Controls.Clear();
            main_menu_tabcontrol.SelectTab(1);
        }
        #endregion

        #region BackToCityPanelFromDistrictPanelButton_Clicked
        private void go_to_city_panel_button_from_district_tab_Click(object sender, EventArgs e)
        {
            //cleared alles in stad scroll panel
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
            _button.FlatAppearance.BorderSize = 1;
        }
        #endregion

        #region GeneratedPanelStyle_Method
        private void panelLayout(Panel _panel)
        {
            _panel.Size = new Size(panelSizeX, panelSizeY);
            _panel.Dock = DockStyle.Top;
            _panel.BackColor = Color.White;
            _panel.BorderStyle = BorderStyle.Fixed3D;
        }
        #endregion

        #region GeneratedLabelStyle_Method
        private void labelLayout(Label _label)
        {
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
            //label naam is het id van de tweet maar ik wil het in een int hebben dus parse ik hem
            int _labelId = Int32.Parse(_label.Name);
            //kleur veranderen van de label
            modelClass.map.hightlightMarker(_labelId);
        }
        #endregion

        #region View_Closed
        private void View_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Zorgt er voor dat alles voor gesloten
            Environment.Exit(0);
        }
        #endregion

        #region go_to_history_panel_button_from_main_menu_tab_Click
        private void go_to_history_panel_button_from_main_menu_tab_Click(object sender, EventArgs e)
        {
            map_tabcontrol.SelectTab(1);
            twitter_tabcontrol.SelectTab(1);
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
            List<string> trendingTweetWord = new List<string>();
            List<string> trendingTags = new List<string>();

            //Initaliseren van _tekst
            var _tekst = "";

            //Maak een lange string van alle twitterberichten          
            foreach (var tweets in modelClass.map.twitter.tweetsList)
            {
                _tekst += tweets.message + " ";
            }

            //Haal een string op, filter alle woorden eruit
            //Groepeer de woorden die groter zijn dan 3 tekens
            //Sorteer van groot naar klein (van meest voorkomende naar minst voorkomende)
            var words =
            Regex.Split(_tekst.ToLower(), @"\W+")
            .Where(s => s.Length > 3)
            .GroupBy(s => s)
            .OrderByDescending(g => g.Count());

            //Pak alle twitterberichten die een hashtag bevatten
            var tagsMessage =
                from tweet in modelClass.map.twitter.tweetsList
                where tweet.message.Contains("#")
                select tweet.message;

            //Initialiseren van messageTagsString
            string messageTagsString = "";

            //Maak een lange string van alle woorden
            foreach (string tagMessageWord in tagsMessage)
            {
                messageTagsString += tagMessageWord + " ";
            }

            //Stop alle hashtags in een array
            var tags = Regex.Split(messageTagsString.ToLower(), @"\s+")
                .Where(a => a.StartsWith("#"))
                .GroupBy(s => s)
                .OrderByDescending(g => g.Count());

            //Controleer of het woord langer is dan een specifiek aantal karakters
            //Voor de hashtags
            //Zo ja, split het woord en voeg het woord toe
            //Zo nee, voeg het wooord alleen toe, zonder aanpassing
            foreach (var tag in tags)
            {
                if (tag.Key.Length > tagLengte)
                {
                    string splittedTag = "";
                    var tagSplit = tag.Key.SplitInParts(tagLengte);
                    foreach (string split in tagSplit)
                    {
                        splittedTag += split + " ";
                    }
                    trendingTags.Add(splittedTag);
                }
                else
                {
                    trendingTags.Add(tag.Key);
                }
            }
           
            //Controleer of het woord langer is dan een specifiek aantal karakters
            //Voor de woorden
            //Zo ja, split het woord en voeg het woord toe
            //Zo nee, voeg het wooord alleen toe, zonder aanpassing
            foreach (var word in words)
            {
                if (word.Key.Length > wordLengte)
                {
                    string splittedTweetWord = "";
                    var wordSplit = word.Key.SplitInParts(wordLengte);
                    foreach (string split in wordSplit)
                    {
                        splittedTweetWord += split + " ";
                    }
                    trendingTweetWord.Add(splittedTweetWord);
                }
                else
                {
                    trendingTweetWord.Add(word.Key);
                }
            }

            //Print de trending woorden op het scherm in een label
            int _wordCount = trendingTweetWord.Count();
            if (_wordCount < 3)
            {
                twitter_trending_topic_label.Text = "Trending topics:\n";
                for (int i = 0; i < _wordCount; i++)
                {
                    twitter_trending_topic_label.Text += (i + 1) + ": " + trendingTweetWord[i] + "\n";
                }
            }
            else
            {
                twitter_trending_topic_label.Text = "Trending topics:\n" + "1: " + trendingTweetWord[0] + "\n2: " + trendingTweetWord[1] + "\n3: " + trendingTweetWord[2];
            }

            //Print de trending hashtags op het scherm in een label
            int _tagCount = trendingTags.Count();
            if (_tagCount == 0)
            {
                twitter_trending_tag_label.Text = "Er zijn geen tags getweet!";
            }
            else if (_tagCount < 3)
            {
                twitter_trending_tag_label.Text = "Trending tags:\n";
                for (int i = 0; i < _tagCount; i++)
                {
                    twitter_trending_tag_label.Text += (i + 1) + ": " + trendingTags[i] + "\n";
                }
            }
            else
            {
                twitter_trending_tag_label.Text = "Trending tags:\n" + "1: " + trendingTags[0] + "\n2: " + trendingTags[1] + "\n3: " + trendingTags[2];
            }

        }
        #endregion

        #region Create suggetions for all filters
        public void fillSearchSuggestions()
        {
            //Roep districte naam suggeties aan.
            //Open database connectie
            modelClass.databaseConnectie.conn.Open();

            //Selectie Query die de namen van allke province selecteer en ordered.
            string stm = "SELECT DISTINCT name FROM district";
            MySqlCommand cmd = new MySqlCommand(stm, modelClass.databaseConnectie.conn);
            modelClass.databaseConnectie.rdr = cmd.ExecuteReader();

            // Hier word de database lijst uitgelezen
            while (modelClass.databaseConnectie.rdr.Read())
            {
                history_district_textbox.AutoCompleteCustomSource.Add(modelClass.databaseConnectie.rdr.GetString(0));
            }

            //sluit database connectie
            modelClass.databaseConnectie.conn.Close();

            //Roep twitter user suggeties aan.
            //Open database connectie
            modelClass.databaseConnectie.conn.Open();
            //Selectie Query die de namen van allke province selecteer en ordered.
            string stm2 = "SELECT DISTINCT user FROM twitter";
            MySqlCommand cmd2 = new MySqlCommand(stm2, modelClass.databaseConnectie.conn);
            modelClass.databaseConnectie.rdr = cmd2.ExecuteReader();

            // Hier word de database lijst uitgelezen
            while (modelClass.databaseConnectie.rdr.Read())
            {
                history_user_textbox.AutoCompleteCustomSource.Add(modelClass.databaseConnectie.rdr.GetString(0));
            }

            //sluit database connectie
            modelClass.databaseConnectie.conn.Close();


            history_category_combobox.Text = "test";
            //Roep twitter user suggeties aan.
            //Open database connectie
            modelClass.databaseConnectie.conn.Open();
            //Selectie Query die de namen van allke province selecteer en ordered.
            string stm3 = "SELECT DISTINCT name FROM category ORDER BY name";
            MySqlCommand cmd3 = new MySqlCommand(stm3, modelClass.databaseConnectie.conn);
            modelClass.databaseConnectie.rdr = cmd3.ExecuteReader();

            // Hier word de database lijst uitgelezen
            while (modelClass.databaseConnectie.rdr.Read())
            {
                history_category_combobox.Items.Add(modelClass.databaseConnectie.rdr.GetString(0).First().ToString().ToUpper() + String.Join("", modelClass.databaseConnectie.rdr.GetString(0).Skip(1)));
            }

            //sluit database connectie
            modelClass.databaseConnectie.conn.Close();

        }
        #endregion

        #region min/max for datepicker
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
            //Open database connectie
            modelClass.databaseConnectie.conn.Open();

            //Haal idAccount op
            string stm = "SELECT * FROM account JOIN person ON account.idaccount = person.idaccount WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(stm, modelClass.databaseConnectie.conn);
            cmd.Parameters.AddWithValue("@username", modelClass.username);
            modelClass.databaseConnectie.rdr = cmd.ExecuteReader();
            modelClass.databaseConnectie.rdr.Read();
            string fullName = modelClass.databaseConnectie.rdr.GetString(6) + " " + modelClass.databaseConnectie.rdr.GetString(7);

            //Sluit database connectie
            modelClass.databaseConnectie.conn.Close();

            return fullName;
        }
        #endregion

        #region UpdateLatestSelectedDisctrictUser
        public void UpdateLatestSelectedDisctrictUser()
        {
            //Default wijk opslaan van gebruiker
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
            DateTime fromDateInput = history_from_datetimepicker.Value;
            DateTime tillDateInput = history_till_datetimepicker.Value;

            //Hier word standaar search query aangemaakt
            stm = modelClass.databaseConnectie.AddSelectTwitterToQuery(stm);
            string tempSearch = "Geschiedenis van: " + Environment.NewLine + Environment.NewLine;

            //Als District checkbox checked is word er een join gemaakt naar de collum van district
            if (history_district_checkbox.Checked)
            {
                stm = modelClass.databaseConnectie.JoinDistrictQuery(stm);
            }

            //Als catgorie checkbox checked is word er een join gemaakt naar de collum van categorie
            if (history_category_checkbox.Checked && history_category_combobox.SelectedIndex > -1)
            {
                stm = modelClass.databaseConnectie.JoinCatgoryQuery(stm);
            }
            //Van af hier begint de WHERE van de query.
            if(history_district_checkbox.Checked || history_user_checkbox.Checked || (history_category_checkbox.Checked && history_category_combobox.SelectedIndex > -1) || history_date_checkbox.Checked || history_keyword_checkbox.Checked)
            {
                stm = modelClass.databaseConnectie.AddWhereToQuery(stm);
            }

            //Als District checkbox checked is word input van district toegevoegd aan de query.
            if (history_district_checkbox.Checked)
            {
                tempSearch = tempSearch + "Wijk: " + districtInput + Environment.NewLine;
                stm = modelClass.databaseConnectie.WhereDistrictQuery(stm);
            }

            //Als District checkbox checked is word input van user toegevoegd aan de query.
            if (history_user_checkbox.Checked)
            {
                tempSearch = tempSearch + "Gebruiker: " + userInput + Environment.NewLine;
                if (history_district_checkbox.Checked)
                {
                    stm = modelClass.databaseConnectie.AddAndToQuery(stm);
                }
                stm = modelClass.databaseConnectie.WhereUserQuery(stm);
            }

            //Als District checkbox checked is word input van catgorie toegevoegd aan de query.
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
                if(history_district_checkbox.Checked || history_user_checkbox.Checked || history_category_checkbox.Checked)
                {
                    stm = modelClass.databaseConnectie.AddAndToQuery(stm);
                }
                stm = modelClass.databaseConnectie.WhereKeyWordQuery(stm);
            }

            //Als District checkbox checked is word input van date toegevoegd aan de query.
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

            //Hier wordt alles georderd op datum zodat nieuwste datum boven aan komt.
            stm = modelClass.databaseConnectie.AddOrderByTimeToQuery(stm);
            stm = modelClass.databaseConnectie.AddLimitToQeury(stm, resultMax);

            //Check of er ubehoud een checkbox gecheckt is.
            if (history_district_checkbox.Checked || history_user_checkbox.Checked || (history_category_checkbox.Checked && history_category_combobox.SelectedIndex > -1) || history_date_checkbox.Checked || history_keyword_checkbox.Checked)
            {
                try
                {
                    //Roep districte naam suggeties aan.
                    //Open database connectie
                    modelClass.databaseConnectie.conn.Open();

                    //Selectie Query die de namen van allke province selecteer en ordered.
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
                    cmd.Parameters.AddWithValue("@keyWordInput","%" + keyWordInput + "%");
                    cmd.Parameters.AddWithValue("@fromDateInput", fromDateInput.ToString("yyyy-MM-dd 00:00:0001"));
                    cmd.Parameters.AddWithValue("@tillDateInput", tillDateInput.ToString("yyyy-MM-dd 23:59:0000"));
                    modelClass.databaseConnectie.rdr = cmd.ExecuteReader();

                    // Hier word de database lijst uitgelezen
                    while (modelClass.databaseConnectie.rdr.Read())
                    {
                        //Teller wordt geupdate per resultaat.
                        resultsCount++;

                        //Text wordt hier aangemaakt voor elke label.
                        string tempLabelText;
                        tempLabelText = ("Gebruiker: " + modelClass.databaseConnectie.rdr.GetString(3) + Environment.NewLine
                                        + "Twitter bericht: " + Environment.NewLine + modelClass.databaseConnectie.rdr.GetString(6) + Environment.NewLine
                                        + Environment.NewLine + "Datum: " + modelClass.databaseConnectie.rdr.GetString(7) + Environment.NewLine);
                        //Panel om straks de labels in te bewaren.
                        Panel createHistoryPanel = new Panel();
                        createHistoryPanel.Name = modelClass.databaseConnectie.rdr.GetString(0).ToString();
                        panelLayout(createHistoryPanel);

                        //Panel word toegevoegd aan scroll panel van history.
                        history_scroll_panel.Controls.Add(createHistoryPanel);

                        //Hier word de label aangemaakt om alle info van database in te printen.
                        Label createHistorylabel = new Label();
                        createHistorylabel.Name = modelClass.databaseConnectie.rdr.GetString(0).ToString();
                        createHistorylabel.Text = tempLabelText;
                        labelLayout(createHistorylabel);

                        //Label wordt toegevoegd aan panel
                        createHistoryPanel.Controls.Add(createHistorylabel);

                    }


                    if (resultsCount == 0)
                    {
                        Label createNoResultAlert = new Label();
                        createNoResultAlert.Text = "Geen resultaten gevonden.";
                        labelLayout(createNoResultAlert);
                        history_scroll_panel.Controls.Add(createNoResultAlert);
                    }

                    //Hier word de resultaat label geupdate met het aantal resultaten.
                    history_header_results_label.Text = "Aantal resultaten: " + resultsCount.ToString();

                    //sluit database connectie
                    modelClass.databaseConnectie.conn.Close();
                }
                catch(MySqlException ex)
                {
                    Console.WriteLine(ex);
                }

                //header label word geupdate met de zoek resultaten die zijn gebruikt.
                History_header_label.Text = tempSearch;

            }
            else
            {
                Label createEmptyAlert = new Label();
                createEmptyAlert.Text = "U heeft geen filter gekozen.";
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
            incident.Show();
        }
        #endregion
    }
}

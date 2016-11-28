namespace WijkAgent
{
    partial class LogInScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogInScreen));
            this.logIn_button_panel = new System.Windows.Forms.Panel();
            this.logIn_picterbox_panel = new System.Windows.Forms.Panel();
            this.logIn_input_panel = new System.Windows.Forms.Panel();
            this.logIn_tabeLayout_panel = new System.Windows.Forms.TableLayoutPanel();
            this.logIn_username_label = new System.Windows.Forms.Label();
            this.logIn_password_label = new System.Windows.Forms.Label();
            this.logIn_username_textbox = new System.Windows.Forms.TextBox();
            this.logIn_password_textbox = new System.Windows.Forms.TextBox();
            this.logIn_button_tableLayout_panel = new System.Windows.Forms.TableLayoutPanel();
            this.logIn_button = new System.Windows.Forms.Button();
            this.stayLoggedIn_checkbox = new System.Windows.Forms.CheckBox();
            this.logIn_picturebox = new System.Windows.Forms.PictureBox();
            this.logIn_button_panel.SuspendLayout();
            this.logIn_picterbox_panel.SuspendLayout();
            this.logIn_input_panel.SuspendLayout();
            this.logIn_tabeLayout_panel.SuspendLayout();
            this.logIn_button_tableLayout_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logIn_picturebox)).BeginInit();
            this.SuspendLayout();
            // 
            // logIn_button_panel
            // 
            this.logIn_button_panel.Controls.Add(this.logIn_button_tableLayout_panel);
            this.logIn_button_panel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logIn_button_panel.Location = new System.Drawing.Point(0, 180);
            this.logIn_button_panel.Name = "logIn_button_panel";
            this.logIn_button_panel.Size = new System.Drawing.Size(616, 85);
            this.logIn_button_panel.TabIndex = 0;
            // 
            // logIn_picterbox_panel
            // 
            this.logIn_picterbox_panel.Controls.Add(this.logIn_picturebox);
            this.logIn_picterbox_panel.Dock = System.Windows.Forms.DockStyle.Left;
            this.logIn_picterbox_panel.Location = new System.Drawing.Point(0, 0);
            this.logIn_picterbox_panel.Name = "logIn_picterbox_panel";
            this.logIn_picterbox_panel.Size = new System.Drawing.Size(180, 180);
            this.logIn_picterbox_panel.TabIndex = 1;
            // 
            // logIn_input_panel
            // 
            this.logIn_input_panel.Controls.Add(this.logIn_tabeLayout_panel);
            this.logIn_input_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logIn_input_panel.Location = new System.Drawing.Point(180, 0);
            this.logIn_input_panel.Name = "logIn_input_panel";
            this.logIn_input_panel.Size = new System.Drawing.Size(436, 180);
            this.logIn_input_panel.TabIndex = 2;
            // 
            // logIn_tabeLayout_panel
            // 
            this.logIn_tabeLayout_panel.ColumnCount = 2;
            this.logIn_tabeLayout_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.logIn_tabeLayout_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.logIn_tabeLayout_panel.Controls.Add(this.logIn_username_label, 0, 0);
            this.logIn_tabeLayout_panel.Controls.Add(this.logIn_password_label, 0, 1);
            this.logIn_tabeLayout_panel.Controls.Add(this.logIn_username_textbox, 1, 0);
            this.logIn_tabeLayout_panel.Controls.Add(this.logIn_password_textbox, 1, 1);
            this.logIn_tabeLayout_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logIn_tabeLayout_panel.Location = new System.Drawing.Point(0, 0);
            this.logIn_tabeLayout_panel.Name = "logIn_tabeLayout_panel";
            this.logIn_tabeLayout_panel.RowCount = 2;
            this.logIn_tabeLayout_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.logIn_tabeLayout_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.logIn_tabeLayout_panel.Size = new System.Drawing.Size(436, 180);
            this.logIn_tabeLayout_panel.TabIndex = 0;
            // 
            // logIn_username_label
            // 
            this.logIn_username_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.logIn_username_label.AutoSize = true;
            this.logIn_username_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logIn_username_label.Location = new System.Drawing.Point(13, 32);
            this.logIn_username_label.Name = "logIn_username_label";
            this.logIn_username_label.Size = new System.Drawing.Size(191, 25);
            this.logIn_username_label.TabIndex = 0;
            this.logIn_username_label.Text = "Gebruikersnaam:";
            // 
            // logIn_password_label
            // 
            this.logIn_password_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.logIn_password_label.AutoSize = true;
            this.logIn_password_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logIn_password_label.Location = new System.Drawing.Point(35, 122);
            this.logIn_password_label.Name = "logIn_password_label";
            this.logIn_password_label.Size = new System.Drawing.Size(148, 25);
            this.logIn_password_label.TabIndex = 1;
            this.logIn_password_label.Text = "Wachtwoord:";
            // 
            // logIn_username_textbox
            // 
            this.logIn_username_textbox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.logIn_username_textbox.Location = new System.Drawing.Point(268, 35);
            this.logIn_username_textbox.Name = "logIn_username_textbox";
            this.logIn_username_textbox.Size = new System.Drawing.Size(118, 20);
            this.logIn_username_textbox.TabIndex = 2;
            // 
            // logIn_password_textbox
            // 
            this.logIn_password_textbox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.logIn_password_textbox.Location = new System.Drawing.Point(268, 125);
            this.logIn_password_textbox.Name = "logIn_password_textbox";
            this.logIn_password_textbox.Size = new System.Drawing.Size(117, 20);
            this.logIn_password_textbox.TabIndex = 3;
            // 
            // logIn_button_tableLayout_panel
            // 
            this.logIn_button_tableLayout_panel.ColumnCount = 1;
            this.logIn_button_tableLayout_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.logIn_button_tableLayout_panel.Controls.Add(this.logIn_button, 0, 1);
            this.logIn_button_tableLayout_panel.Controls.Add(this.stayLoggedIn_checkbox, 0, 0);
            this.logIn_button_tableLayout_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logIn_button_tableLayout_panel.Location = new System.Drawing.Point(0, 0);
            this.logIn_button_tableLayout_panel.Name = "logIn_button_tableLayout_panel";
            this.logIn_button_tableLayout_panel.RowCount = 2;
            this.logIn_button_tableLayout_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.logIn_button_tableLayout_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.logIn_button_tableLayout_panel.Size = new System.Drawing.Size(616, 85);
            this.logIn_button_tableLayout_panel.TabIndex = 0;
            // 
            // logIn_button
            // 
            this.logIn_button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.logIn_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logIn_button.Location = new System.Drawing.Point(264, 41);
            this.logIn_button.Name = "logIn_button";
            this.logIn_button.Size = new System.Drawing.Size(88, 36);
            this.logIn_button.TabIndex = 0;
            this.logIn_button.Text = "Log In";
            this.logIn_button.UseVisualStyleBackColor = true;
            this.logIn_button.Click += new System.EventHandler(this.logIn_button_Click);
            // 
            // stayLoggedIn_checkbox
            // 
            this.stayLoggedIn_checkbox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.stayLoggedIn_checkbox.AutoSize = true;
            this.stayLoggedIn_checkbox.Location = new System.Drawing.Point(258, 8);
            this.stayLoggedIn_checkbox.Name = "stayLoggedIn_checkbox";
            this.stayLoggedIn_checkbox.Size = new System.Drawing.Size(100, 17);
            this.stayLoggedIn_checkbox.TabIndex = 1;
            this.stayLoggedIn_checkbox.Text = "Ingelogd blijven";
            this.stayLoggedIn_checkbox.UseVisualStyleBackColor = true;
            // 
            // logIn_picturebox
            // 
            this.logIn_picturebox.BackColor = System.Drawing.Color.Transparent;
            this.logIn_picturebox.BackgroundImage = global::WijkAgent.Properties.Resources.politie_categorie;
            this.logIn_picturebox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.logIn_picturebox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logIn_picturebox.Location = new System.Drawing.Point(0, 0);
            this.logIn_picturebox.Name = "logIn_picturebox";
            this.logIn_picturebox.Size = new System.Drawing.Size(180, 180);
            this.logIn_picturebox.TabIndex = 0;
            this.logIn_picturebox.TabStop = false;
            // 
            // LogInScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 265);
            this.Controls.Add(this.logIn_input_panel);
            this.Controls.Add(this.logIn_picterbox_panel);
            this.Controls.Add(this.logIn_button_panel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogInScreen";
            this.Text = "log In ";
            this.logIn_button_panel.ResumeLayout(false);
            this.logIn_picterbox_panel.ResumeLayout(false);
            this.logIn_input_panel.ResumeLayout(false);
            this.logIn_tabeLayout_panel.ResumeLayout(false);
            this.logIn_tabeLayout_panel.PerformLayout();
            this.logIn_button_tableLayout_panel.ResumeLayout(false);
            this.logIn_button_tableLayout_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logIn_picturebox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel logIn_button_panel;
        private System.Windows.Forms.Panel logIn_picterbox_panel;
        private System.Windows.Forms.PictureBox logIn_picturebox;
        private System.Windows.Forms.Panel logIn_input_panel;
        private System.Windows.Forms.TableLayoutPanel logIn_tabeLayout_panel;
        private System.Windows.Forms.Label logIn_username_label;
        private System.Windows.Forms.Label logIn_password_label;
        private System.Windows.Forms.TextBox logIn_username_textbox;
        private System.Windows.Forms.TextBox logIn_password_textbox;
        private System.Windows.Forms.TableLayoutPanel logIn_button_tableLayout_panel;
        private System.Windows.Forms.Button logIn_button;
        private System.Windows.Forms.CheckBox stayLoggedIn_checkbox;
    }
}
namespace WijkAgent
{
    partial class IncidentScreen
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
            this.selectAllCheck = new System.Windows.Forms.CheckBox();
            this.categoryCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.twitterIncidentPanel = new System.Windows.Forms.Panel();
            this.saveIncidentButton = new System.Windows.Forms.Button();
            this.cancelIncidentButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // selectAllCheck
            // 
            this.selectAllCheck.AutoSize = true;
            this.selectAllCheck.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectAllCheck.ForeColor = System.Drawing.Color.White;
            this.selectAllCheck.Location = new System.Drawing.Point(43, 45);
            this.selectAllCheck.Name = "selectAllCheck";
            this.selectAllCheck.Size = new System.Drawing.Size(136, 23);
            this.selectAllCheck.TabIndex = 0;
            this.selectAllCheck.Text = "Alles Selecteren";
            this.selectAllCheck.UseVisualStyleBackColor = true;
            // 
            // categoryCombo
            // 
            this.categoryCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.categoryCombo.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.categoryCombo.FormattingEnabled = true;
            this.categoryCombo.Location = new System.Drawing.Point(345, 41);
            this.categoryCombo.Name = "categoryCombo";
            this.categoryCombo.Size = new System.Drawing.Size(121, 27);
            this.categoryCombo.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(253, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "Categorie*:";
            // 
            // twitterIncidentPanel
            // 
            this.twitterIncidentPanel.BackColor = System.Drawing.Color.White;
            this.twitterIncidentPanel.Location = new System.Drawing.Point(12, 79);
            this.twitterIncidentPanel.Name = "twitterIncidentPanel";
            this.twitterIncidentPanel.Size = new System.Drawing.Size(513, 422);
            this.twitterIncidentPanel.TabIndex = 3;
            // 
            // saveIncidentButton
            // 
            this.saveIncidentButton.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.saveIncidentButton.Location = new System.Drawing.Point(440, 518);
            this.saveIncidentButton.Name = "saveIncidentButton";
            this.saveIncidentButton.Size = new System.Drawing.Size(85, 35);
            this.saveIncidentButton.TabIndex = 4;
            this.saveIncidentButton.Text = "Opslaan";
            this.saveIncidentButton.UseVisualStyleBackColor = true;
            // 
            // cancelIncidentButton
            // 
            this.cancelIncidentButton.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.cancelIncidentButton.Location = new System.Drawing.Point(345, 518);
            this.cancelIncidentButton.Name = "cancelIncidentButton";
            this.cancelIncidentButton.Size = new System.Drawing.Size(89, 35);
            this.cancelIncidentButton.TabIndex = 5;
            this.cancelIncidentButton.Text = "Annuleren";
            this.cancelIncidentButton.UseVisualStyleBackColor = true;
            // 
            // IncidentScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Blue;
            this.ClientSize = new System.Drawing.Size(537, 565);
            this.Controls.Add(this.cancelIncidentButton);
            this.Controls.Add(this.saveIncidentButton);
            this.Controls.Add(this.twitterIncidentPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.categoryCombo);
            this.Controls.Add(this.selectAllCheck);
            this.MaximizeBox = false;
            this.Name = "IncidentScreen";
            this.Text = "IncidentScreen";
            this.Load += new System.EventHandler(this.IncidentScreen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox selectAllCheck;
        private System.Windows.Forms.ComboBox categoryCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel twitterIncidentPanel;
        private System.Windows.Forms.Button saveIncidentButton;
        private System.Windows.Forms.Button cancelIncidentButton;
    }
}
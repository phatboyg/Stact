namespace AppFrame
{
    partial class MainForm
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
            this.triggerButton = new System.Windows.Forms.Button();
            this.triggerEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // triggerButton
            // 
            this.triggerButton.Location = new System.Drawing.Point(246, 87);
            this.triggerButton.Name = "triggerButton";
            this.triggerButton.Size = new System.Drawing.Size(75, 23);
            this.triggerButton.TabIndex = 0;
            this.triggerButton.Text = "Trigger";
            this.triggerButton.UseVisualStyleBackColor = true;
            // 
            // triggerEnabledCheckBox
            // 
            this.triggerEnabledCheckBox.AutoSize = true;
            this.triggerEnabledCheckBox.Location = new System.Drawing.Point(328, 92);
            this.triggerEnabledCheckBox.Name = "triggerEnabledCheckBox";
            this.triggerEnabledCheckBox.Size = new System.Drawing.Size(65, 17);
            this.triggerEnabledCheckBox.TabIndex = 1;
            this.triggerEnabledCheckBox.Text = "Enabled";
            this.triggerEnabledCheckBox.UseVisualStyleBackColor = true;
            this.triggerEnabledCheckBox.CheckedChanged += new System.EventHandler(this.triggerEnabledCheckBox_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 397);
            this.Controls.Add(this.triggerEnabledCheckBox);
            this.Controls.Add(this.triggerButton);
            this.Name = "MainForm";
            this.Text = "AppFrame Sample";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button triggerButton;
        private System.Windows.Forms.CheckBox triggerEnabledCheckBox;
    }
}


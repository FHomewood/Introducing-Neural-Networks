namespace sol
{
    partial class NeuralNet
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
            this.components = new System.ComponentModel.Container();
            this._timer = new System.Windows.Forms.Timer(this.components);
            this._rtbOutput = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // _timer
            // 
            this._timer.Enabled = true;
            this._timer.Interval = 1000;
            this._timer.Tick += new System.EventHandler(this.Update);
            // 
            // _rtbOutput
            // 
            this._rtbOutput.Location = new System.Drawing.Point(12, 12);
            this._rtbOutput.Name = "_rtbOutput";
            this._rtbOutput.Size = new System.Drawing.Size(675, 500);
            this._rtbOutput.TabIndex = 0;
            this._rtbOutput.Text = "";
            this._rtbOutput.ZoomFactor = 0.8F;
            // 
            // NeuralNet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1166, 523);
            this.Controls.Add(this._rtbOutput);
            this.Name = "NeuralNet";
            this.Text = "NeuralNetcs";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer _timer;
        private System.Windows.Forms.RichTextBox _rtbOutput;
    }
}
namespace Front_inz_meil
{
    partial class Fourier_chart
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
            this.fourierChart = new LiveCharts.WinForms.CartesianChart();
            this.SuspendLayout();
            // 
            // fourierChart
            // 
            this.fourierChart.BackColor = System.Drawing.Color.White;
            this.fourierChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fourierChart.Location = new System.Drawing.Point(0, 0);
            this.fourierChart.Name = "fourierChart";
            this.fourierChart.Size = new System.Drawing.Size(800, 450);
            this.fourierChart.TabIndex = 0;
            this.fourierChart.Text = "cartesianChart1";
            // 
            // Fourier_chart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.fourierChart);
            this.Name = "Fourier_chart";
            this.Text = "Fourier_chart";
            this.ResumeLayout(false);

        }

        #endregion

        private LiveCharts.WinForms.CartesianChart fourierChart;
    }
}
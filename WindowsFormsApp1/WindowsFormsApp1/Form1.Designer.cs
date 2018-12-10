namespace WindowsFormsApp1
{
    partial class Form1
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.PointChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.NextIteration = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PointChart)).BeginInit();
            this.SuspendLayout();
            // 
            // PointChart
            // 
            chartArea1.Name = "ChartArea1";
            this.PointChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.PointChart.Legends.Add(legend1);
            this.PointChart.Location = new System.Drawing.Point(0, 0);
            this.PointChart.Name = "PointChart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.PointChart.Series.Add(series1);
            this.PointChart.Size = new System.Drawing.Size(1001, 718);
            this.PointChart.TabIndex = 0;
            this.PointChart.Text = "chart1";
            // 
            // NextIteration
            // 
            this.NextIteration.Location = new System.Drawing.Point(1034, 78);
            this.NextIteration.Name = "NextIteration";
            this.NextIteration.Size = new System.Drawing.Size(75, 23);
            this.NextIteration.TabIndex = 1;
            this.NextIteration.Text = "button1";
            this.NextIteration.UseVisualStyleBackColor = true;
            this.NextIteration.Click += new System.EventHandler(this.NextIteration_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1143, 715);
            this.Controls.Add(this.NextIteration);
            this.Controls.Add(this.PointChart);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PointChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart PointChart;
        private System.Windows.Forms.Button NextIteration;
    }
}


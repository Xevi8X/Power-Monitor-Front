using LiveCharts.Wpf.Charts.Base;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Front_inz_meil.MainForm;
using LiveCharts.Definitions.Series;
using LiveCharts.Defaults;

namespace Front_inz_meil
{
    public partial class Fourier_chart : Form
    {
        private double[] hz;
        private double[] mag;


        public Fourier_chart(double[] hz, double[] mag) 
        {
            this.hz = hz;
            this.mag = mag;
            InitializeComponent();
            drawChart();
        }

        private void drawChart()
        {
            ScatterPoint[] scatterPoints = new ScatterPoint[hz.Length];
            for (int i = 0; i < scatterPoints.Length; i++)
            {
                scatterPoints[i] = new ScatterPoint(hz[i], mag[i],1);
            }
            fourierChart.Series.Clear();
            fourierChart.AxisX.Clear();
            fourierChart.AxisY.Clear();

            fourierChart.Series.Add(new LineSeries
            {
                Values = new ChartValues<ScatterPoint>(scatterPoints),
                ScalesYAt = 0,
                ScalesXAt = 0,
                Fill = System.Windows.Media.Brushes.Transparent,
            }); ;


            fourierChart.AxisX.Add(new Axis
            {
                Foreground = System.Windows.Media.Brushes.DodgerBlue,
                Title = "Frequency [Hz]",
                DisableAnimations = true,
                MinValue = 0.0,
                MaxValue = 500.0,
                Separator = new Separator()
                {
                    Step = 50.0
                }
            }); ; ;

            fourierChart.AxisY.Add(new Axis
            {
                Foreground = System.Windows.Media.Brushes.DodgerBlue,
                Title = "Magnitude [V]",
                DisableAnimations = true,
                MinValue = 0.0,
                MaxValue = 250.0,
            }); ;
        }
    }
}

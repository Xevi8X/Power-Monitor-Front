using System.IO.Ports;
using LiveCharts; //Core of the library
using LiveCharts.Wpf; //The WPF controls
using LiveCharts.WinForms; //The WPF controls
using System.ComponentModel.Design;
using System.Globalization;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf.Charts.Base;
using LiveCharts.Defaults;
using LiveCharts.Definitions.Charts;
using System.Windows.Media;
using System.Text;

namespace Front_inz_meil
{
    public partial class MainForm : Form
    {
        SerialPort serialPort = new SerialPort();

        STATE state = STATE.AUTO;
        bool[] compensatorsSwitches = new bool[8];
        List<Measure> measures = new List<Measure>();
        float[] compensators = new float[8];

        public MainForm()
        {
            InitializeComponent();
            InitData();
        }

        private void InitData()
        {
            serialPort.BaudRate = 115200;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = Handshake.None;
            serialPort.RtsEnable = true;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorReceivedHandler);
            serialPort.PinChanged += new SerialPinChangedEventHandler(PinChangedReceivedHandler);

            buttonEnability();

            setGauges();
            setCompensatorsParams();
            setCompensatorsState();
        }

        private void buttonEnability()
        {
            btnAuto.Enabled = state != STATE.AUTO;

            btnManual.Enabled = state != STATE.MANUAL;
        }

        private void setGauges()
        {
            solidGaugeV.From = 205.0;
            solidGaugeV.To = 255.0;

            solidGaugeI.From = 0.0;
            solidGaugeI.To = 16.0;

            solidGaugeFi.From = -90.0;
            solidGaugeFi.To = 90.0;

            solidGaugeP.From = 0.0;
            solidGaugeP.To = 3680.0;

            solidGaugeQ.From = -3680.0;
            solidGaugeQ.To = 3680.0;

            solidGaugeS.From = 0.0;
            solidGaugeS.To = 3680.0;
        }



        private void MainForm_Load(object sender, EventArgs e)
        {
            changePort();
        }

        public void setSelectedPort(string port)
        {
            if (serialPort.IsOpen) serialPort.Close();
            serialPort.PortName = port;
            try
            {
                serialPort.Open();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                changePort();
            }
        }

        private void changePort()
        {
            DialogCOM dialog = new DialogCOM(serialPort.PortName);
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.Show(this);
            this.Enabled = false;
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            string indata = serialPort.ReadLine();
            this.BeginInvoke(delegate () { handlerCommingMessage(indata); });
            this.BeginInvoke(delegate () { richTxtRaw.AppendText(indata + "\n", System.Drawing.Color.Green, HorizontalAlignment.Left, getAutoScroll(), false); });
        }

        private void handlerCommingMessage(string indata)
        {
            if (indata[0] != '@') return;
            char[] separator = { ';', '#' };
            List<string> args = indata.Split(separator).ToList();
            if (args == null || args.Count == 0) return;
            string command = args[0];
            args.RemoveAt(0);
            command = command.Substring(1);

            int commandNo;
            if (int.TryParse(command, out commandNo))
            {
                switch (commandNo)
                {
                    case 1:
                        this.BeginInvoke(delegate () { setDashboard(args); });
                        break;
                    case 2:
                        this.BeginInvoke(delegate () { setStateOfCompensator(args); });
                        break;
                    case 3:
                        this.BeginInvoke(delegate () { measures = new List<Measure>(); });
                        break;
                    case 4:
                        this.BeginInvoke(delegate () { takeMeasure(args); });
                        break;
                    case 5:
                        this.BeginInvoke(delegate () { updateChart(); });
                        break;
                    case 6:
                        this.BeginInvoke(delegate () { setCompensatorsParams(args); });
                        break;

                }
            }
        }

        private void setDashboard(List<string> args)
        {
            if (args.Count != 6) return;
            double val;
            args = args.Select(x => x.Replace('.', ',')).ToList();
            if (double.TryParse(args[0], out val)) solidGaugeV.Value = val;
            else solidGaugeV.Value = 0.0;
            if (double.TryParse(args[1], out val)) solidGaugeI.Value = val;
            else solidGaugeI.Value = 0.0;
            if (double.TryParse(args[2], out val)) solidGaugeP.Value = val;
            else solidGaugeP.Value = 0.0;
            if (double.TryParse(args[3], out val)) solidGaugeQ.Value = val;
            else solidGaugeQ.Value = 0.0;
            if (double.TryParse(args[4], out val)) solidGaugeS.Value = val;
            else solidGaugeS.Value = 0.0;
            if (double.TryParse(args[5], out val)) solidGaugeFi.Value = val;
            else solidGaugeFi.Value = 0.0;
        }

        private void setStateOfCompensator(List<string> args)
        {
            int val;
            if (int.TryParse(args[0], out val))
            {
                for (int i = 0; i < 8; i++)
                {
                    if ((val & 1) == 1) compensatorsSwitches[i] = true;
                    else compensatorsSwitches[i] = false;
                    val >>= 1;
                }
                setCompensatorsState();
            }
            if (int.TryParse(args[1], out val))
            {
                state = (STATE)val;
                buttonEnability();
            }

        }

        private void takeMeasure(List<string> args)
        {
            if (args.Count != 3) return;
            Measure measure;
            long time;
            double val;
            if (long.TryParse(args[0], out time)) measure.microsecounds = time;
            else return;
            if (double.TryParse(args[1].Replace('.', ','), out val)) measure.voltage = val;
            else return;
            if (double.TryParse(args[2].Replace('.', ','), out val)) measure.current = val;
            else return;
            measures.Add(measure);
        }

        private void updateChart()
        {
            const int skip = 200;
            chart.Series.Clear();
            chart.AxisX.Clear();
            chart.AxisY.Clear();
            chart.Series.Add(new LineSeries
            {
                Values = new ChartValues<double>(measures.Skip(skip).SkipLast(skip).Select(m => m.voltage)),
                ScalesYAt = 0,
                Fill = System.Windows.Media.Brushes.Transparent,
                PointGeometrySize = 0

            }); ;
            chart.Series.Add(new LineSeries
            {
                Values = new ChartValues<double>(measures.Skip(skip).SkipLast(skip).Select(m => m.current)),
                ScalesYAt = 1,
                Fill = System.Windows.Media.Brushes.Transparent,
                PointGeometrySize = 0
            });

            chart.AxisY.Add(new Axis
            {
                Foreground = System.Windows.Media.Brushes.DodgerBlue,
                Title = "Voltage [V]",
                MinValue = -350.0,
                MaxValue = 350.0,
                DisableAnimations = true,
            }); ;
            chart.AxisY.Add(new Axis
            {
                Foreground = System.Windows.Media.Brushes.IndianRed,
                Title = "Current [A]",
                MinValue = -16.0,
                MaxValue = 16.0,
                DisableAnimations = true,
                Position = AxisPosition.RightTop
            }); ; ;
        }

        private void setCompensatorsParams(List<string> args)
        {
            if (args.Count != 8 && args.Count != 9) return;
            float val;
            args = args.Select(x => x.Replace('.', ',')).ToList();
            for (int i = 0; i < 8; i++)
            {
                if (float.TryParse(args[i], out val)) compensators[i] = val;
            }
            setCompensatorsParams();
        }

        private void ErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e)
        {
            MessageBox.Show(e.EventType.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void PinChangedReceivedHandler(object sender, SerialPinChangedEventArgs e)
        {
            MessageBox.Show(e.EventType.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            richTxtRaw.Text = "";
            richTxtRaw.AppendText("...Start...", System.Drawing.Color.Black);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort.IsOpen) serialPort.Close();
        }

        public enum STATE
        {
            AUTO = 0, MANUAL = 1
        }

        private struct Measure
        {
            public long microsecounds;
            public double voltage;
            public double current;
        }

        private void btnCOM_Click(object sender, EventArgs e)
        {
            changePort();
        }

        private void tabCtrl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabCtrl.SelectedIndex)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        Transmit(Message.GET_MODE, null);
                        Transmit(Message.GET_COMPENSATOR_PARAM, null);
                        break;
                    }
                case 2:
                    {
                        break;
                    }
            }
        }

        private void SendData(string outdata)
        {

            if (serialPort.IsOpen)
            {
                serialPort.WriteLine(outdata);
                richTxtRaw.AppendText(outdata + "\n", System.Drawing.Color.Red, HorizontalAlignment.Right, getAutoScroll(),false);
            }
            else richTxtRaw.AppendText(outdata + "\n", System.Drawing.Color.Gray, HorizontalAlignment.Right, getAutoScroll(),false);
        }

        private void Transmit(Message message, int? arg)
        {
            string outdata = $"@{((int)message)}#";
            if (arg != null && arg >= 0 && arg < 256) outdata += $"{arg.Value:D3}#";
            SendData(outdata);
        }

        public bool getAutoScroll()
        {
            return checkBoxAutoScroll.Checked;
        }

        public void setCompensatorsParams()
        {
            Label[] lbls = new Label[] { lblSw0, lblSw1, lblSw2, lblSw3, lblSw4, lblSw5, lblSw6, lblSw7 };

            for (int i = 0; i < 8; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(i + ": ");
                if (compensators[i] > 20.0f) sb.Append("inductive");
                else if (compensators[i] < -20.0f) sb.Append("capacitive");
                else sb.Append("unconnected");
                sb.Append("\nQ = ");
                sb.Append(compensators[i].ToString());
                sb.Append(" Var");
                lbls[i].Text = sb.ToString();
            }

        }

        public void setCompensatorsState()
        {
            PictureBox[] pics = new PictureBox[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4,
                pictureBox5, pictureBox6, pictureBox7, pictureBox8 };

            for (int i = 0; i < 8; i++)
            {
                if (compensatorsSwitches[i]) pics[i].Image = Image.FromFile(@"Images\BulbOn.png");
                else pics[i].Image = Image.FromFile(@"Images\BulbOff.png");
                pics[i].SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void btnAuto_Click(object sender, EventArgs e)
        {
            Transmit(Message.SET_MODE, 0);
        }

        private void btnManual_Click(object sender, EventArgs e)
        {
            Transmit(Message.SET_MODE, 1);
        }

        int swToBytes()
        {
            int b = 0;
            for(int i = 0; i < 8; i++)
            {
                if (compensatorsSwitches[i]) b |= (1 << i);
            }
            return b;
        }

        private void toggleSwitch(int i)
        {
            int sw = swToBytes();
            sw ^=(1 << i);
            Transmit(Message.SET_SWITCH_STATE, sw);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            toggleSwitch(0);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            toggleSwitch(1);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            toggleSwitch(2);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            toggleSwitch(3);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            toggleSwitch(4);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            toggleSwitch(5);
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            toggleSwitch(6);
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            toggleSwitch(7);
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            Transmit(Message.GET_DATA, null);
        }

        private void btnCalib_Click(object sender, EventArgs e)
        {
            Transmit(Message.CALIBRATE, null);
        }

        private void btnComponents_Click(object sender, EventArgs e)
        {
            Transmit(Message.CALIBRATE_COMPONENTS, null);
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Front_inz_meil
{
    public partial class DialogCOM : Form
    {
        public DialogCOM(String port)
        {
            InitializeComponent();
            this.CenterToParent();
            cmbPorts.Items.AddRange(SerialPort.GetPortNames());
            cmbPorts.SelectedValue = port;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Owner.Enabled = true;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(!SerialPort.GetPortNames().Contains((string)cmbPorts.SelectedItem))
            {
                MessageBox.Show("Choose correct port!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ((MainForm)this.Owner).setSelectedPort((string)cmbPorts.SelectedItem);
            this.Owner.Enabled = true;
            this.Close();
        }
    }
}

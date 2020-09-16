using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmniaGUI
{
    public partial class MonitorForm : Form
    {
        delegate void MonitorTextCallback(string text);
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
        public MonitorForm()
        {
            InitializeComponent();
        }

        private void MonitorForm_Load(object sender, EventArgs e)
        {

        }

        private void Monitortxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void Heartbeatcheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if(Master_Controls.Debug_Display == false)
            {
                Master_Controls.Debug_Display = true;
            }else
            {
                Master_Controls.Debug_Display = false;
            }
        }

        private void MonitorPausecheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(oCode.monitor_suspend == false)
            {
                oCode.monitor_suspend = true;
            }
            else
            {
                oCode.monitor_suspend = false;
            }
        }
    }
}

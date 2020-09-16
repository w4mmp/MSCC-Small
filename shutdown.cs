using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OmniaGUI.Properties;
using System.IO;
using ZedGraph;

namespace OmniaGUI
{
    public partial class shutdown : Form
    {
        private const int CP_NOCLOSE_BUTTON = 0x200;
        /*protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }*/
        public shutdown()
        {
            InitializeComponent();
        }

        private void shutdown_Load(object sender, EventArgs e)
        {
            if (Master_Controls.FTP_File_Found == false)
            {
                label3.Text = "FTP Initialization file NOT FOUND";
            }
            if (Master_Controls.Initialize_network_status == true)
            {
                oCode.SendCommand(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, oCode.CMD_SET_STOP, 1);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}

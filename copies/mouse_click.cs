using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;

namespace OmniaGUI
{
    public partial class Mouse_Click : Form
    {
        private void Zero_Frequency()
        {
            Int32 remainder = 0;
            MonitorTextBoxText(" Zero_Frequency -> Called");
            switch (oCode.FreqDigit)
            {
                case 7: //10 Millions
                    remainder = oCode.DisplayFreq % 10000000;
                    MonitorTextBoxText(" Zero_Frequency -> FreqDigit: " + Convert.ToString(oCode.FreqDigit)
                         + " Remainder: " + Convert.ToString(remainder));
                    break;
                case 6: //Millions
                    remainder = oCode.DisplayFreq % 1000000;
                    MonitorTextBoxText(" Zero_Frequency -> FreqDigit: " + Convert.ToString(oCode.FreqDigit)
                         + " Remainder: " + Convert.ToString(remainder));
                    break;
                case 5: 
                    remainder = oCode.DisplayFreq % 100000;
                    MonitorTextBoxText(" Zero_Frequency -> FreqDigit: " + Convert.ToString(oCode.FreqDigit) 
                        + " Remainder: " + Convert.ToString(remainder));
                    break;
                case 4: 
                    remainder = oCode.DisplayFreq % 10000;
                    MonitorTextBoxText(" Zero_Frequency -> FreqDigit: " + Convert.ToString(oCode.FreqDigit)
                        + " Remainder: " + Convert.ToString(remainder));
                    break;
                case 3: 
                    remainder = oCode.DisplayFreq % 1000;
                    MonitorTextBoxText(" Zero_Frequency -> FreqDigit: " + Convert.ToString(oCode.FreqDigit)
                         + " Remainder: " + Convert.ToString(remainder));
                    break;
                case 2: 
                    remainder = oCode.DisplayFreq % 100;
                    MonitorTextBoxText(" Zero_Frequency -> FreqDigit: " + Convert.ToString(oCode.FreqDigit)
                        + " Remainder: " + Convert.ToString(remainder));
                    break;
                case 1: 
                    remainder = oCode.DisplayFreq % 10;
                    MonitorTextBoxText(" Zero_Frequency -> FreqDigit: " + Convert.ToString(oCode.FreqDigit)
                         + " Remainder: " + Convert.ToString(remainder));
                    break;
                
                default:
                    MonitorTextBoxText(" Zero_Frequency -> default. FreqDigit: " +  Convert.ToString(oCode.FreqDigit) +
                        " Remainder: " + Convert.ToString(remainder));
                    break;
            }
            oCode.DisplayFreq = oCode.DisplayFreq - remainder;
            /*if(Freq_Digits.previous_frequency < oCode.DisplayFreq)
            {
                oCode.DisplayFreq = oCode.DisplayFreq - remainder;
            }
            else
            {
                oCode.DisplayFreq = oCode.DisplayFreq - remainder;
            }*/
            //Freq_Digits.previous_frequency = oCode.DisplayFreq;
        }
        private void Update_Frequency()
        {
            if (oCode.DisplayFreq < 0) return;
            if (oCode.DisplayFreq > 40000000) oCode.DisplayFreq = 40000000;
            //Zero_Frequency();
            Display_Main_Freq();
            Panadapter_Controls.Freq_Set_By_Master = true;
            int freq_plus_rit = oCode.DisplayFreq + Rit_Controls.Offset;
            int MHz = freq_plus_rit / 1000000;
            int KHz = (freq_plus_rit - (MHz * 1000000)) / 1000;
            int Hz = freq_plus_rit - (MHz * 1000000) - (KHz * 1000);
            ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
            Panadapter_Controls.Frequency = oCode.DisplayFreq;
            oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, oCode.DisplayFreq);
            Mouse_controls.Silent_Update = false;
            if (oCode.current_band == oCode.general_band)
            {
                Last_used.GEN.Freq = oCode.DisplayFreq;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Mouse_Click
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Mouse_Click";
            //this.Load += new System.EventHandler(this.frmGUI_Load);
            this.ResumeLayout(false);

        }

        //private void frmGUI_Load(object sender, EventArgs e)
        //{
//
        //}
    }
}
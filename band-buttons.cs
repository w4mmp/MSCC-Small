 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace OmniaGUI
{
 
 public partial class Main_form : Form
    {
        private void Set_mode_display(char mode)
        {
            Last_used.Current_mode = mode;
            MonitorTextBoxText(  " Set_mode_display called. Last_used.Current_mode: " + Last_used.Current_mode );
            switch (mode)
            {
                case 'U':
                    mainmodebutton2.Text = "USB";
                    break;
                case 'L':
                    mainmodebutton2.Text = "LSB";
                      break;
                case 'A':
                    mainmodebutton2.Text = "AM";
                       break;
                case 'F':
                    mainmodebutton2.Text = "FM";
                    break;
                case 'C':
                    mainmodebutton2.Text = "CW";
                    break;
                case 'E':
                    mainmodebutton2.Text = "ECSS";
                    break;
                case 'D':
                    mainmodebutton2.Text = "DRM";
                   break;
            }
            MonitorTextBoxText(  " Set_mode_display called -> Finished");
        }

        private short Convert_mode_char_to_digit(char mode)
        {
            short mode_number;
            mode_number = 0;
            switch (mode)
            {
                case 'A':
                    mainmodebutton2.Text = "AM";
                    mode_number = 0;
                    break;

                case 'L':
                    mainmodebutton2.Text = "LSB";
                    mode_number = 1;
                    break;

                case 'U':
                    mainmodebutton2.Text = "USB";
                    mode_number = 2;
                    break;

                case 'C':
                    mainmodebutton2.Text = "CW";
                    mode_number = 3;
                    break;

                case 'F':
                    mainmodebutton2.Text = "FM";
                    mode_number = 4;
                    break;
                
                case 'E':
                    mainmodebutton2.Text = "ECSS";
                    mode_number = 5;
                    break;

                case 'D':
                    mainmodebutton2.Text = "DRM";
                    mode_number = 6;
                    break;
            }
            return mode_number;
        }

        private void main160radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            int MHz;
            int KHz;
            int Hz;
            
            if (oCode.isLoading == true) return;
            if (oCode.previous_main_band != 160)
            {
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_LAST_USED_FREQ, oCode.previous_main_band);
                oCode.previous_main_band = 160;
                oCode.gen_band_active = false;
                MonitorTextBoxText(  " m160radiobutton called " );
                Set_mode_display(Last_used.B160.Mode);
                oCode.current_band = 160;
                if (!Power_Calibration_Controls.Power_Tab_Active && !Amplifier_Power_Controls.Tab_Active)
                {
                    short mode_number = Convert_mode_char_to_digit(Last_used.B160.Mode);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B160.Freq);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B160.Freq);
                    oCode.modeswitch = mode_number;
                    Filter_listBox1.SetSelected(Last_used.B160.Filter_High_Index, true);
                    CW_Filter_listBox1.SetSelected(Last_used.B160.Filter_CW_index, true);
                    Filter_Low_listBox1.SetSelected(Last_used.B160.Filter_Low_Index, true);
                }
                oCode.DisplayFreq = Last_used.B160.Freq;
                Display_Main_Freq();
                int freq_plus_rit = Last_used.B160.Freq + Rit_Controls.Offset;
                Rit_Controls.Rit_Freq = Last_used.B160.Freq;
                MHz = freq_plus_rit / 1000000;
                KHz = (freq_plus_rit - (MHz * 1000000)) / 1000;
                Hz = freq_plus_rit - (MHz * 1000000) - (KHz * 1000);
                ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
                Panadapter_Controls.Frequency = oCode.DisplayFreq;
                //Panadapter_Controls.Updated_Frequency = oCode.DisplayFreq;
                Panadapter_Controls.Freq_Set_By_Master = true;
#if FTDI
                Relay_Board_Controls.sentBytes[0] = (byte)(Relay_Board_Controls.sentBytes[0] = 1);
#endif
                Master_Controls.Band_Change_Toggle = true;
                MonitorTextBoxText(  " m160radiobutton -> " + "Current Mode: " + Last_used.B160.Mode );
                MonitorTextBoxText(  " m160radiobutton Finished " );
            }
           
        }

        private void main80radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            int MHz;
            int KHz;
            int Hz;
            
            if (oCode.isLoading == true) return;
            if (oCode.previous_main_band != 80)
            {
                MonitorTextBoxText(  " m80radiobutton called " );
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_LAST_USED_FREQ, oCode.previous_main_band);
                Set_mode_display(Last_used.B80.Mode);
                oCode.gen_band_active = false;
                oCode.previous_main_band = 80;
                oCode.current_band = 80;
                if (!Power_Calibration_Controls.Power_Tab_Active && !Amplifier_Power_Controls.Tab_Active)
                {
                    short mode_number = Convert_mode_char_to_digit(Last_used.B80.Mode);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B80.Freq);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B80.Freq);
                    Filter_listBox1.SetSelected(Last_used.B80.Filter_High_Index, true);
                    CW_Filter_listBox1.SetSelected(Last_used.B80.Filter_CW_index, true);
                    Filter_Low_listBox1.SetSelected(Last_used.B80.Filter_Low_Index, true);
                    oCode.modeswitch = mode_number;
                    //Band_button_clicked_get_freq_and_mode();
                }
                oCode.DisplayFreq = Last_used.B80.Freq;
                Display_Main_Freq();
                int freq_plus_rit = Last_used.B80.Freq + Rit_Controls.Offset;
                Rit_Controls.Rit_Freq = Last_used.B80.Freq;
                MHz = freq_plus_rit / 1000000;
                KHz = (freq_plus_rit - (MHz * 1000000)) / 1000;
                Hz = freq_plus_rit - (MHz * 1000000) - (KHz * 1000);
                ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
                Panadapter_Controls.Frequency = oCode.DisplayFreq;
                //Panadapter_Controls.Updated_Frequency = oCode.DisplayFreq;
                Panadapter_Controls.Freq_Set_By_Master = true;
#if FTDI
                Relay_Board_Controls.sentBytes[0] = (byte)(Relay_Board_Controls.sentBytes[0] = 2);
#endif
                Master_Controls.Band_Change_Toggle = true;
                MonitorTextBoxText(  " m80radiobutton -> " + "Current Mode: " + Last_used.B80.Mode );
                MonitorTextBoxText(  " m80radiobutton Finished " );
            }
        }

        private void main60radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            int MHz;
            int KHz;
            int Hz;
            
            if (oCode.isLoading == true) return;
            if (oCode.previous_main_band != 60)
            {
                MonitorTextBoxText(  " m60radiobutton called " );
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_LAST_USED_FREQ, oCode.previous_main_band);
                Set_mode_display(Last_used.B60.Mode);
                oCode.gen_band_active = false;
                oCode.previous_main_band = 60;
                oCode.current_band = 60;

                if (!Power_Calibration_Controls.Power_Tab_Active && !Amplifier_Power_Controls.Tab_Active)
                {
                    short mode_number = Convert_mode_char_to_digit(Last_used.B60.Mode);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B60.Freq);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B60.Freq);
                    Filter_listBox1.SetSelected(Last_used.B60.Filter_High_Index, true);
                    CW_Filter_listBox1.SetSelected(Last_used.B60.Filter_CW_index, true);
                    Filter_Low_listBox1.SetSelected(Last_used.B60.Filter_Low_Index, true);
                    oCode.modeswitch = mode_number;
                    //Band_button_clicked_get_freq_and_mode();
                }
                oCode.DisplayFreq = Last_used.B60.Freq;
                Display_Main_Freq();
                int freq_plus_rit = Last_used.B60.Freq + Rit_Controls.Offset;
                Rit_Controls.Rit_Freq = Last_used.B60.Freq;
                MHz = freq_plus_rit / 1000000;
                KHz = (freq_plus_rit - (MHz * 1000000)) / 1000;
                Hz = freq_plus_rit - (MHz * 1000000) - (KHz * 1000);
                ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
                Panadapter_Controls.Frequency = oCode.DisplayFreq;
                //Updated_Frequency = oCode.DisplayFreq;
                Panadapter_Controls.Freq_Set_By_Master = true;
#if FTDI
                Relay_Board_Controls.sentBytes[0] = (byte)(Relay_Board_Controls.sentBytes[0] = 3);
#endif
                Master_Controls.Band_Change_Toggle = true;
                MonitorTextBoxText(  " m60radiobutton -> " + "Current Mode: " + Last_used.B60.Mode );
                MonitorTextBoxText(  " m60radiobutton Finished " );

            }
        }

        private void main40radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            int MHz;
            int KHz;
            int Hz;

            if (oCode.isLoading == true) return;
            if (oCode.previous_main_band != 40)
            {
                MonitorTextBoxText(  " m40radiobutton called " );
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_LAST_USED_FREQ, oCode.previous_main_band);
                Set_mode_display(Last_used.B40.Mode);
                oCode.gen_band_active = false;
                oCode.previous_main_band = 40;
                oCode.DisplayFreq = Last_used.B40.Freq;
                Display_Main_Freq();
                oCode.current_band = 40;
                int freq_plus_rit = Last_used.B40.Freq + Rit_Controls.Offset;
                Rit_Controls.Rit_Freq = Last_used.B40.Freq;
                MonitorTextBoxText(  " m40radiobutton called -> Rit_Controls.Rit_Freq: " + 
                    Convert.ToString(Rit_Controls.Rit_Freq) );
                MHz = freq_plus_rit / 1000000;
                KHz = (freq_plus_rit - (MHz * 1000000)) / 1000;
                Hz = freq_plus_rit - (MHz * 1000000) - (KHz * 1000);
                ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
                Panadapter_Controls.Frequency = oCode.DisplayFreq;
                //Panadapter_Controls.Updated_Frequency = oCode.DisplayFreq;
                Panadapter_Controls.Freq_Set_By_Master = true;
#if FTDI
                Relay_Board_Controls.sentBytes[0] = (byte)(Relay_Board_Controls.sentBytes[0] = 4);
#endif
                Master_Controls.Band_Change_Toggle = true;
                if (!Power_Calibration_Controls.Power_Tab_Active && !Amplifier_Power_Controls.Tab_Active)
                {
                    short mode_number = Convert_mode_char_to_digit(Last_used.B40.Mode);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B40.Freq);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B40.Freq);
                    Filter_listBox1.SetSelected(Last_used.B40.Filter_High_Index, true);
                    CW_Filter_listBox1.SetSelected(Last_used.B40.Filter_CW_index, true);
                    Filter_Low_listBox1.SetSelected(Last_used.B40.Filter_Low_Index, true);
                    oCode.modeswitch = mode_number;
                }
            }
        }

        private void main30radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            int MHz;
            int KHz;
            int Hz;
            if (oCode.isLoading == true) return;
            if (oCode.previous_main_band != 30)
            {
                MonitorTextBoxText(  " m30radiobutton called " );
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_LAST_USED_FREQ, oCode.previous_main_band);
                Set_mode_display(Last_used.B30.Mode);
                oCode.gen_band_active = false;
                oCode.previous_main_band = 30;
                oCode.DisplayFreq = Last_used.B30.Freq;
                Display_Main_Freq();
                oCode.current_band = 30;
                int freq_plus_rit = Last_used.B30.Freq + Rit_Controls.Offset;
                Rit_Controls.Rit_Freq = Last_used.B30.Freq;
                MHz = freq_plus_rit / 1000000;
                KHz = (freq_plus_rit - (MHz * 1000000)) / 1000;
                Hz = freq_plus_rit - (MHz * 1000000) - (KHz * 1000);
                ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
                Panadapter_Controls.Frequency = oCode.DisplayFreq;
                //Panadapter_Controls.Updated_Frequency = oCode.DisplayFreq;
                Panadapter_Controls.Freq_Set_By_Master = true;
#if FTDI
                Relay_Board_Controls.sentBytes[0] = (byte)(Relay_Board_Controls.sentBytes[0] = 5);
#endif
                Master_Controls.Band_Change_Toggle = true;
                if (!Power_Calibration_Controls.Power_Tab_Active && !Amplifier_Power_Controls.Tab_Active)
                {
                    short mode_number = Convert_mode_char_to_digit(Last_used.B30.Mode);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B30.Freq);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B30.Freq);
                    Filter_listBox1.SetSelected(Last_used.B30.Filter_High_Index, true);
                    CW_Filter_listBox1.SetSelected(Last_used.B30.Filter_CW_index, true);
                    Filter_Low_listBox1.SetSelected(Last_used.B30.Filter_Low_Index, true);
                    oCode.modeswitch = mode_number;
                    //Band_button_clicked_get_freq_and_mode();
                }
            }
        }

        private void main20radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            int MHz;
            int KHz;
            int Hz;
            if (oCode.isLoading == true) return;
            if (oCode.previous_main_band != 20)
            {
                MonitorTextBoxText(  " m20radiobutton called " );
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_LAST_USED_FREQ, oCode.previous_main_band);
                Set_mode_display(Last_used.B20.Mode);
                oCode.gen_band_active = false;
                oCode.previous_main_band = 20;
                oCode.DisplayFreq = Last_used.B20.Freq;
                Display_Main_Freq();
                oCode.current_band = 20;
                int freq_plus_rit = Last_used.B20.Freq + Rit_Controls.Offset;
                Rit_Controls.Rit_Freq = Last_used.B20.Freq;
                MHz = freq_plus_rit / 1000000;
                KHz = (freq_plus_rit - (MHz * 1000000)) / 1000;
                Hz = freq_plus_rit - (MHz * 1000000) - (KHz * 1000);
                ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
                Panadapter_Controls.Frequency = oCode.DisplayFreq;
                //Panadapter_Controls.Updated_Frequency = oCode.DisplayFreq;
                Panadapter_Controls.Freq_Set_By_Master = true;
#if FTDI
                Relay_Board_Controls.sentBytes[0] = (byte)(Relay_Board_Controls.sentBytes[0] = 6);
#endif
                Master_Controls.Band_Change_Toggle = true;
                if (!Power_Calibration_Controls.Power_Tab_Active && !Amplifier_Power_Controls.Tab_Active)
                {
                    short mode_number = Convert_mode_char_to_digit(Last_used.B20.Mode);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B20.Freq);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B20.Freq);
                    Filter_listBox1.SetSelected(Last_used.B20.Filter_High_Index, true);
                    CW_Filter_listBox1.SetSelected(Last_used.B20.Filter_CW_index, true);
                    Filter_Low_listBox1.SetSelected(Last_used.B20.Filter_Low_Index, true);
                    oCode.modeswitch = mode_number;
                }
            }
        }

        private void main17radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            int MHz;
            int KHz;
            int Hz;
            if (oCode.isLoading == true) return;
            if (oCode.previous_main_band != 17)
            {
                MonitorTextBoxText(  " m17radiobutton called " );
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_LAST_USED_FREQ, oCode.previous_main_band);
                Set_mode_display(Last_used.B17.Mode);
                oCode.gen_band_active = false;
                oCode.previous_main_band = 17;
                oCode.DisplayFreq = Last_used.B17.Freq;
                Display_Main_Freq();
                oCode.current_band = 17;
                int freq_plus_rit = Last_used.B17.Freq + Rit_Controls.Offset;
                Rit_Controls.Rit_Freq = Last_used.B17.Freq;
                MHz = freq_plus_rit / 1000000;
                KHz = (freq_plus_rit - (MHz * 1000000)) / 1000;
                Hz = freq_plus_rit - (MHz * 1000000) - (KHz * 1000);
                ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
                Panadapter_Controls.Frequency = oCode.DisplayFreq;
                //Panadapter_Controls.Updated_Frequency = oCode.DisplayFreq;
                Panadapter_Controls.Freq_Set_By_Master = true;
#if FTDI
                Relay_Board_Controls.sentBytes[0] = (byte)(Relay_Board_Controls.sentBytes[0] = 7);
#endif
                Master_Controls.Band_Change_Toggle = true;
                if (!Power_Calibration_Controls.Power_Tab_Active && !Amplifier_Power_Controls.Tab_Active)
                {
                    short mode_number = Convert_mode_char_to_digit(Last_used.B17.Mode);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B17.Freq);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B17.Freq);
                    Filter_listBox1.SetSelected(Last_used.B17.Filter_High_Index, true);
                    CW_Filter_listBox1.SetSelected(Last_used.B17.Filter_CW_index, true);
                    Filter_Low_listBox1.SetSelected(Last_used.B17.Filter_Low_Index, true);
                    oCode.modeswitch = mode_number;
                    //Band_button_clicked_get_freq_and_mode();
                }
            }
        }

        private void main15radiobutton_CheckedChanged(object sender, EventArgs e)
        {
            int MHz;
            int KHz;
            int Hz;
            if (oCode.isLoading == true) return;
            if (oCode.previous_main_band != 15)
            {
                MonitorTextBoxText(  " m15radiobutton called " );
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_LAST_USED_FREQ, oCode.previous_main_band);
                Set_mode_display(Last_used.B15.Mode);
                oCode.gen_band_active = false;
                oCode.previous_main_band = 15;
                oCode.DisplayFreq = Last_used.B15.Freq;
                Display_Main_Freq();
                oCode.current_band = 15;
                int freq_plus_rit = Last_used.B15.Freq + Rit_Controls.Offset;
                Rit_Controls.Rit_Freq = Last_used.B15.Freq;
                MHz = freq_plus_rit / 1000000;
                KHz = (freq_plus_rit - (MHz * 1000000)) / 1000;
                Hz = freq_plus_rit - (MHz * 1000000) - (KHz * 1000);
                ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
                Panadapter_Controls.Frequency = oCode.DisplayFreq;
                //Panadapter_Controls.Updated_Frequency = oCode.DisplayFreq;
                Panadapter_Controls.Freq_Set_By_Master = true;
#if FTDI
                Relay_Board_Controls.sentBytes[0] = (byte)(Relay_Board_Controls.sentBytes[0] = 8);
#endif
                Master_Controls.Band_Change_Toggle = true;
                if (!Power_Calibration_Controls.Power_Tab_Active && !Amplifier_Power_Controls.Tab_Active)
                {
                    short mode_number = Convert_mode_char_to_digit(Last_used.B15.Mode);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B15.Freq);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B15.Freq);
                    Filter_listBox1.SetSelected(Last_used.B15.Filter_High_Index, true);
                    CW_Filter_listBox1.SetSelected(Last_used.B15.Filter_CW_index, true);
                    Filter_Low_listBox1.SetSelected(Last_used.B15.Filter_Low_Index, true);
                    oCode.modeswitch = mode_number;
                    //Band_button_clicked_get_freq_and_mode();
                }
            }
        }

        private void main12radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            int MHz;
            int KHz;
            int Hz;
            if (oCode.isLoading == true) return;
            if (oCode.previous_main_band != 12)
            {
                MonitorTextBoxText(  " m12radiobutton called " );
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_LAST_USED_FREQ, oCode.previous_main_band);
                Set_mode_display(Last_used.B12.Mode);
                oCode.gen_band_active = false;
                oCode.previous_main_band = 12;
                oCode.DisplayFreq = Last_used.B12.Freq;
                Display_Main_Freq();
                oCode.current_band = 12;
                int freq_plus_rit = Last_used.B12.Freq + Rit_Controls.Offset;
                Rit_Controls.Rit_Freq = Last_used.B12.Freq;
                MHz = freq_plus_rit / 1000000;
                KHz = (freq_plus_rit - (MHz * 1000000)) / 1000;
                Hz = freq_plus_rit - (MHz * 1000000) - (KHz * 1000);
                ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
                Panadapter_Controls.Frequency = oCode.DisplayFreq;
                //Panadapter_Controls.Updated_Frequency = oCode.DisplayFreq;
                Panadapter_Controls.Freq_Set_By_Master = true;
#if FTDI
                Relay_Board_Controls.sentBytes[0] = (byte)(Relay_Board_Controls.sentBytes[0] = 9);
#endif
                Master_Controls.Band_Change_Toggle = true;
                if (!Power_Calibration_Controls.Power_Tab_Active && !Amplifier_Power_Controls.Tab_Active)
                {
                    short mode_number = Convert_mode_char_to_digit(Last_used.B12.Mode);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B12.Freq);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B12.Freq);
                    Filter_listBox1.SetSelected(Last_used.B12.Filter_High_Index, true);
                    CW_Filter_listBox1.SetSelected(Last_used.B12.Filter_CW_index, true);
                    Filter_Low_listBox1.SetSelected(Last_used.B12.Filter_Low_Index, true);
                    oCode.modeswitch = mode_number;
                    //Band_button_clicked_get_freq_and_mode();
                }
            }
        }

        private void main10radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            int MHz;
            int KHz;
            int Hz;
            if (oCode.isLoading == true) return;
            if (oCode.previous_main_band != 10)
            {
                MonitorTextBoxText(  " m10radiobutton called " );
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_LAST_USED_FREQ, oCode.previous_main_band);
                Set_mode_display(Last_used.B10.Mode);
                oCode.gen_band_active = false;
                oCode.previous_main_band = 10;
                oCode.DisplayFreq = Last_used.B10.Freq;
                Display_Main_Freq();
                oCode.current_band = 10;
                int freq_plus_rit = Last_used.B10.Freq + Rit_Controls.Offset;
                Rit_Controls.Rit_Freq = Last_used.B10.Freq;
                MHz = freq_plus_rit / 1000000;
                KHz = (freq_plus_rit - (MHz * 1000000)) / 1000;
                Hz = freq_plus_rit - (MHz * 1000000) - (KHz * 1000);
                ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
                Panadapter_Controls.Frequency = oCode.DisplayFreq;
                //Panadapter_Controls.Updated_Frequency = oCode.DisplayFreq;
                Panadapter_Controls.Freq_Set_By_Master = true;
#if FTDI
                Relay_Board_Controls.sentBytes[0] = (byte)(Relay_Board_Controls.sentBytes[0] = 10);
#endif
                Master_Controls.Band_Change_Toggle = true;
                if (!Power_Calibration_Controls.Power_Tab_Active && !Amplifier_Power_Controls.Tab_Active)
                {
                    short mode_number = Convert_mode_char_to_digit(Last_used.B10.Mode);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B10.Freq);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B10.Freq);
                    Filter_listBox1.SetSelected(Last_used.B10.Filter_High_Index, true);
                    CW_Filter_listBox1.SetSelected(Last_used.B10.Filter_CW_index, true);
                    Filter_Low_listBox1.SetSelected(Last_used.B10.Filter_Low_Index, true);
                    oCode.modeswitch = mode_number;
                    //Band_button_clicked_get_freq_and_mode();
                }
            }
        }

        private void genradioButton_CheckedChanged(object sender, EventArgs e)
        {
            int MHz;
            int KHz;
            int Hz;

            short mode_number;
            if (oCode.isLoading == true) return;
            if (oCode.previous_main_band != 200)
            {
                MonitorTextBoxText(  " genradiobutton called " );
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_LAST_USED_FREQ, oCode.previous_main_band);
                Set_mode_display(Last_used.GEN.Mode);
                oCode.gen_band_active = true;
                oCode.previous_main_band = 200;
                oCode.DisplayFreq = Last_used.GEN.Freq;
                Display_Main_Freq();
                oCode.current_band = 200;
                int freq_plus_rit = Last_used.GEN.Freq + Rit_Controls.Offset;
                Rit_Controls.Rit_Freq = Last_used.GEN.Freq;
                MHz = freq_plus_rit / 1000000;
                KHz = (freq_plus_rit - (MHz * 1000000)) / 1000;
                Hz = freq_plus_rit - (MHz * 1000000) - (KHz * 1000);
                ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
                Panadapter_Controls.Frequency = oCode.DisplayFreq;
                //Panadapter_Controls.Updated_Frequency = oCode.DisplayFreq;
                Panadapter_Controls.Freq_Set_By_Master = true;
#if FTDI
                Relay_Board_Controls.sentBytes[0] = (byte)(Relay_Board_Controls.sentBytes[0] = 0);
#endif
                if (!Power_Calibration_Controls.Power_Tab_Active && !Amplifier_Power_Controls.Tab_Active)
                {
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.GEN.Freq);
                    mode_number = Convert_mode_char_to_digit(Last_used.GEN.Mode);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                    Filter_listBox1.SetSelected(Last_used.GEN.Filter_High_Index, true);
                    CW_Filter_listBox1.SetSelected(Last_used.GEN.Filter_CW_index, true);
                    Filter_Low_listBox1.SetSelected(Last_used.GEN.Filter_Low_Index, true);
                    oCode.modeswitch = mode_number;
                    //Band_button_clicked_get_freq_and_mode();
                }
                MonitorTextBoxText(  " genradiobutton -> " + "Current Mode: " + Last_used.GEN.Mode );
                MonitorTextBoxText(  " genradiobutton Finished " );
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Main_form
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Main_form";
            this.Load += new System.EventHandler(this.Main_form_Load);
            this.ResumeLayout(false);

        }

        private void Main_form_Load(object sender, EventArgs e)
        {

        }
    }
}
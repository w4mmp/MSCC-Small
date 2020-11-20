using System;
using System.Threading;
using System.Windows.Forms;
using OmniaGUI.Properties;
using System.IO;
namespace OmniaGUI
{
    partial class Main_form
    {
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        /// Form stop = new shutdown();
        protected override void Dispose(bool disposing)
        {
            String Comm_file;
            
            //Form stop = new shutdown();
            //stop.Activate();
            //stop.Show();
            //stop.Focus();
            Comm_file = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#if RPI
            RPi_Settings.RPi_Needs_Updated = true;
            Update_RPi_Settings();
            Comm_file += "/mscc/comm-port.ini";
#else
            Comm_file += "\\multus-sdr-client\\comm-port.ini";
#endif
            if (File.Exists(Comm_file))
            {
                File.Delete(Comm_file);
            }
            Thread.Sleep(5000);
            //oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_STOP, 1);
#if FTDI
            uint bytes_written = 0;
            byte[] sentBytes = new byte[2];
            if (Tuning_Knob_Controls.Open)
            {
                sentBytes[0] = (byte)(sentBytes[0] | 0xFF);
                Tuning_Knob_Controls.Device.Write(sentBytes, 1, ref bytes_written);
                Tuning_Knob_Controls.Device.Close();
            }
#endif
            if (Monitor_window != null) Monitor_window.Close();

            if (Window_controls.Panadapter_Controls.Panadapter_Control_Window != null)
            {
                if (Window_controls.Panadapter_Controls.Panadapter_Control_Window.WindowState != FormWindowState.Minimized)
                {
                    Settings.Default.Panadapter_Control_Location =
                        Window_controls.Panadapter_Controls.Panadapter_Control_Window.Location;
                }
                Window_controls.Panadapter_Controls.Panadapter_Control_Window.Close();
            }

              
            if (this != null)
            {
                //oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_STOP, 1);
                if (Waterfall_thread.IsAlive)
                {
                    Waterfall_thread.Abort();
                }
                if (this.WindowState != FormWindowState.Minimized)
                {
                    Settings.Default.Main_Window_Location = this.Location;
                    Settings.Default.Main_Window_Size = this.Size;
                }
            }

            Settings.Default.Save();
            //Properties.mscc.Default.test = 2;
            Properties.mscc.Default.Save();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main_form));
            this.powertabControl1 = new System.Windows.Forms.TabControl();
            this.mainPage = new System.Windows.Forms.TabPage();
            this.Microphone_textBox2 = new System.Windows.Forms.Label();
            this.Volume_textBox2 = new System.Windows.Forms.Label();
            this.mainlistBox1 = new System.Windows.Forms.ListBox();
            this.MFC_Knob_label38 = new System.Windows.Forms.Label();
            this.MFC_C_label38 = new System.Windows.Forms.Label();
            this.MFC_B_label38 = new System.Windows.Forms.Label();
            this.MFC_A_label38 = new System.Windows.Forms.Label();
            this.UTC_Date_label46 = new System.Windows.Forms.Label();
            this.Time_display_UTC_label34 = new System.Windows.Forms.Label();
            this.Zedgraph_Control = new ZedGraph.ZedGraphControl();
            this.NR_button3 = new System.Windows.Forms.Button();
            this.Minimize_checkBox2 = new System.Windows.Forms.CheckBox();
            this.Main_Power_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.Meter_Mode_button8 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Tenthousand_Top_button6 = new System.Windows.Forms.Button();
            this.Hundred_Thousand_Top_button5 = new System.Windows.Forms.Button();
            this.Million_Top_button4 = new System.Windows.Forms.Button();
            this.TenMillion_Bottom_button8 = new System.Windows.Forms.Button();
            this.Million_Bottom_button7 = new System.Windows.Forms.Button();
            this.Hundred_Thousand_Button_button6 = new System.Windows.Forms.Button();
            this.Tenthousand_Bottom_button5 = new System.Windows.Forms.Button();
            this.Thousand_Bottom_button = new System.Windows.Forms.Button();
            this.Hundreds_Top_button8 = new System.Windows.Forms.Button();
            this.Hundred_Bottom_button3 = new System.Windows.Forms.Button();
            this.Tens_Top_button = new System.Windows.Forms.Button();
            this.Tens_Bottom_button2 = new System.Windows.Forms.Button();
            this.Ones_Bottom_button2 = new System.Windows.Forms.Button();
            this.Ones_Top_button2 = new System.Windows.Forms.Button();
            this.Tenmillions = new System.Windows.Forms.Label();
            this.Decimal_label58 = new System.Windows.Forms.Label();
            this.Decimal_label59 = new System.Windows.Forms.Label();
            this.Millions = new System.Windows.Forms.Label();
            this.Hundredthousand = new System.Windows.Forms.Label();
            this.Tenthousands = new System.Windows.Forms.Label();
            this.Hundreds = new System.Windows.Forms.Label();
            this.Tens = new System.Windows.Forms.Label();
            this.Ones = new System.Windows.Forms.Label();
            this.Thousand_Top_button7 = new System.Windows.Forms.Button();
            this.Thousands = new System.Windows.Forms.Label();
            this.Ten_Million_Top_button3 = new System.Windows.Forms.Button();
            this.Audio_Digital_button3 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.Spectrum_Controls_button3 = new System.Windows.Forms.Button();
            this.RIT_groupBox4 = new System.Windows.Forms.GroupBox();
            this.StartUP_label44 = new System.Windows.Forms.Label();
            this.ritfreqtextBox1 = new System.Windows.Forms.TextBox();
            this.buttReset = new System.Windows.Forms.Button();
            this.ritbutton1 = new System.Windows.Forms.Button();
            this.ritScroll = new System.Windows.Forms.HScrollBar();
            this.button6 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.ACG_button = new System.Windows.Forms.Button();
            this.Compression_button4 = new System.Windows.Forms.Button();
            this.Auto_Zero_checkBox2 = new System.Windows.Forms.CheckBox();
            this.Filter_listBox1 = new System.Windows.Forms.ListBox();
            this.CW_Filter_listBox1 = new System.Windows.Forms.ListBox();
            this.Filter_Low_listBox1 = new System.Windows.Forms.ListBox();
            this.Local_Date_label46 = new System.Windows.Forms.Label();
            this.NB_button2 = new System.Windows.Forms.Button();
            this.Time_display_label33 = new System.Windows.Forms.Label();
            this.TX_Mute_button2 = new System.Windows.Forms.Button();
            this.Volume_Mute_button2 = new System.Windows.Forms.Button();
            this.MicVolume_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.Volume_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.Freqbutton3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.genradioButton = new System.Windows.Forms.RadioButton();
            this.mainmodebutton2 = new System.Windows.Forms.Button();
            this.main10radioButton1 = new System.Windows.Forms.RadioButton();
            this.main12radioButton2 = new System.Windows.Forms.RadioButton();
            this.main15radiobutton = new System.Windows.Forms.RadioButton();
            this.main17radioButton4 = new System.Windows.Forms.RadioButton();
            this.main20radioButton5 = new System.Windows.Forms.RadioButton();
            this.main30radioButton6 = new System.Windows.Forms.RadioButton();
            this.main40radioButton7 = new System.Windows.Forms.RadioButton();
            this.main60radioButton8 = new System.Windows.Forms.RadioButton();
            this.main80radioButton9 = new System.Windows.Forms.RadioButton();
            this.main160radioButton10 = new System.Windows.Forms.RadioButton();
            this.buttTune = new System.Windows.Forms.Button();
            this.Band_Change_Auto_Tune_checkBox2 = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.Power_Value_label2 = new System.Windows.Forms.Label();
            this.vuMeter1 = new VU_MeterLibrary.VuMeter();
            this.picWaterfall = new System.Windows.Forms.PictureBox();
            this.TX = new System.Windows.Forms.TabPage();
            this.Meter_hold_label43 = new System.Windows.Forms.Label();
            this.Power_Meter_Hold = new System.Windows.Forms.NumericUpDown();
            this.Reverse_Power_label43 = new System.Windows.Forms.Label();
            this.Forward_Power_label43 = new System.Windows.Forms.Label();
            this.SWR_Value_label43 = new System.Windows.Forms.Label();
            this.Reverse_label58 = new System.Windows.Forms.Label();
            this.Forward_label43 = new System.Windows.Forms.Label();
            this.Reverse_Meter = new VU_MeterLibrary.VuMeter();
            this.SWR_label1 = new System.Windows.Forms.Label();
            this.Forward_Meter = new VU_MeterLibrary.VuMeter();
            this.Full_Power_checkBox1 = new System.Windows.Forms.CheckBox();
            this.NR_label5 = new System.Windows.Forms.Label();
            this.NR_Button = new System.Windows.Forms.Button();
            this.AGC_label57 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.AGC_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.label53 = new System.Windows.Forms.Label();
            this.Default_High_Cut_listBox1 = new System.Windows.Forms.ListBox();
            this.label52 = new System.Windows.Forms.Label();
            this.Default_CW_Filter_listBox1 = new System.Windows.Forms.ListBox();
            this.label51 = new System.Windows.Forms.Label();
            this.Default_Low_Cut_listBox1 = new System.Windows.Forms.ListBox();
            this.label50 = new System.Windows.Forms.Label();
            this.Tune_vButton2 = new System.Windows.Forms.Button();
            this.PA_vButton1 = new System.Windows.Forms.Button();
            this.NB_Threshold_label1 = new System.Windows.Forms.Label();
            this.NB_label16 = new System.Windows.Forms.Label();
            this.NB_ON_OFF_button2 = new System.Windows.Forms.Button();
            this.NB_Threshold_label16 = new System.Windows.Forms.Label();
            this.NB_Threshold_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.NB_Width_label16 = new System.Windows.Forms.Label();
            this.NB_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.NR_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.NR_label2 = new System.Windows.Forms.Label();
            this.AGC_listBox1 = new System.Windows.Forms.ListBox();
            this.AGC_label2 = new System.Windows.Forms.Label();
            this.Tune_Power_label37 = new System.Windows.Forms.Label();
            this.Tune_Power_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.label36 = new System.Windows.Forms.Label();
            this.CW_Power_label36 = new System.Windows.Forms.Label();
            this.SSB_Power_label36 = new System.Windows.Forms.Label();
            this.AM_Carrier_label36 = new System.Windows.Forms.Label();
            this.CW_Power_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.label34 = new System.Windows.Forms.Label();
            this.Power_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.Power_label34 = new System.Windows.Forms.Label();
            this.TX_Bandwidth_listBox1 = new System.Windows.Forms.ListBox();
            this.AM_Carrier_label2 = new System.Windows.Forms.Label();
            this.AM_Carrier_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.label14 = new System.Windows.Forms.Label();
            this.band_stack = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.General_listView1 = new System.Windows.Forms.ListView();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.Favorites_textBox2 = new System.Windows.Forms.TextBox();
            this.B10_Favs_listView1 = new System.Windows.Forms.ListView();
            this.B12_Favs_listView1 = new System.Windows.Forms.ListView();
            this.B15_Favs_listView1 = new System.Windows.Forms.ListView();
            this.B17_Favs_listView1 = new System.Windows.Forms.ListView();
            this.B20_Favs_listView1 = new System.Windows.Forms.ListView();
            this.B30_Favs_listView1 = new System.Windows.Forms.ListView();
            this.B40_Favs_listView1 = new System.Windows.Forms.ListView();
            this.B60_Favs_listView1 = new System.Windows.Forms.ListView();
            this.B80_Favs_listView1 = new System.Windows.Forms.ListView();
            this.B160_Favs_listView1 = new System.Windows.Forms.ListView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.band_stack_textBox1 = new System.Windows.Forms.TextBox();
            this.band_stack_label29 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.band_stack_update_button1 = new System.Windows.Forms.Button();
            this.freqcaltab = new System.Windows.Forms.TabPage();
            this.IQ_groupBox3 = new System.Windows.Forms.GroupBox();
            this.IQ_Tune_Power_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.Reset_Freq_button3 = new System.Windows.Forms.Button();
            this.IQ_UP24KHz_checkBox2 = new System.Windows.Forms.CheckBox();
            this.IQ_Freq_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.Cal_Freq_textBox2 = new System.Windows.Forms.TextBox();
            this.IQ_TX_button = new System.Windows.Forms.Button();
            this.IQ_Reset_All_button2 = new System.Windows.Forms.Button();
            this.LeftResetbutton2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.IQ12_radioButton2 = new System.Windows.Forms.RadioButton();
            this.IQ160_radioButton10 = new System.Windows.Forms.RadioButton();
            this.IQ80_radioButton9 = new System.Windows.Forms.RadioButton();
            this.IQ60_radioButton8 = new System.Windows.Forms.RadioButton();
            this.IQ40_radioButton7 = new System.Windows.Forms.RadioButton();
            this.IQ30_radioButton6 = new System.Windows.Forms.RadioButton();
            this.IQ20_radioButton5 = new System.Windows.Forms.RadioButton();
            this.IQ17_radioButton4 = new System.Windows.Forms.RadioButton();
            this.IQ15_radioButton3 = new System.Windows.Forms.RadioButton();
            this.IQ10_radioButton1 = new System.Windows.Forms.RadioButton();
            this.IQ_RX_button = new System.Windows.Forms.Button();
            this.LefthScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.IQ_Commit_button2 = new System.Windows.Forms.Button();
            this.IQLefttextBox2 = new System.Windows.Forms.TextBox();
            this.IQ_Tune_button2 = new System.Windows.Forms.Button();
            this.Freq_Cal_groupBox4 = new System.Windows.Forms.GroupBox();
            this.Freq_Cal_label59 = new System.Windows.Forms.Label();
            this.Freq_CAl_Progress_Lable = new System.Windows.Forms.Label();
            this.label57 = new System.Windows.Forms.Label();
            this.Freq_Check_Button = new System.Windows.Forms.Button();
            this.Standard_Carrier_listBox1 = new System.Windows.Forms.ListBox();
            this.Calibration_progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Freq_Cal_checkBox3 = new System.Windows.Forms.CheckBox();
            this.Freq_Cal_Reset_button4 = new System.Windows.Forms.Button();
            this.calibratebutton1 = new System.Windows.Forms.Button();
            this.powertabPage1 = new System.Windows.Forms.TabPage();
            this.XCRV_Power_Display_label33 = new System.Windows.Forms.Label();
            this.B10radioButton = new System.Windows.Forms.RadioButton();
            this.B12radioButton = new System.Windows.Forms.RadioButton();
            this.B15radioButton = new System.Windows.Forms.RadioButton();
            this.B17radioButton = new System.Windows.Forms.RadioButton();
            this.B20radioButton = new System.Windows.Forms.RadioButton();
            this.B30radioButton = new System.Windows.Forms.RadioButton();
            this.B40radioButton = new System.Windows.Forms.RadioButton();
            this.B60radioButton = new System.Windows.Forms.RadioButton();
            this.B80radioButton = new System.Windows.Forms.RadioButton();
            this.B160radioButton = new System.Windows.Forms.RadioButton();
            this.power_slider_reset_button1 = new System.Windows.Forms.Button();
            this.powerrestorebutton2 = new System.Windows.Forms.Button();
            this.powertunebutton1 = new System.Windows.Forms.Button();
            this.powerhScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.powerlabel14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.Audio_tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Side_Tone_label32 = new System.Windows.Forms.Label();
            this.Side_Tone_Volume_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.label17 = new System.Windows.Forms.Label();
            this.ritoffsetlistBox1 = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.ritlistBox1 = new System.Windows.Forms.ListBox();
            this.CW_Hold_numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.CW_Lag_numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.CW_Paddle_listBox1 = new System.Windows.Forms.ListBox();
            this.CW_Weight_listBox1 = new System.Windows.Forms.ListBox();
            this.CW_Space_listBox1 = new System.Windows.Forms.ListBox();
            this.CW_Type_listBox1 = new System.Windows.Forms.ListBox();
            this.CW_Mode_listBox1 = new System.Windows.Forms.ListBox();
            this.label61 = new System.Windows.Forms.Label();
            this.label60 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.label58 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.CW_Pitch_listBox1 = new System.Windows.Forms.ListBox();
            this.label35 = new System.Windows.Forms.Label();
            this.semicheckBox2 = new System.Windows.Forms.CheckBox();
            this.MFC = new System.Windows.Forms.TabPage();
            this.label33 = new System.Windows.Forms.Label();
            this.AMP_comboBox1 = new System.Windows.Forms.ComboBox();
            this.IQBD_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.Antenna_Switch_label43 = new System.Windows.Forms.Label();
            this.Antenna_Switch_comboBox1 = new System.Windows.Forms.ComboBox();
            this.Tuning_Knob_groupBox1 = new System.Windows.Forms.GroupBox();
            this.label39 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.Right_Button_comboBox4 = new System.Windows.Forms.ComboBox();
            this.Knob_comboBox1 = new System.Windows.Forms.ComboBox();
            this.Left_Button_comboBox2 = new System.Windows.Forms.ComboBox();
            this.Middle_Button_comboBox3 = new System.Windows.Forms.ComboBox();
            this.AMP_groupBox3 = new System.Windows.Forms.GroupBox();
            this.Solidus_Tab_Power_Display_label33 = new System.Windows.Forms.Label();
            this.Solidus_Bias_button8 = new System.Windows.Forms.Button();
            this.Power_calibration_label58 = new System.Windows.Forms.Label();
            this.AMP_Raw_Bias_label5 = new System.Windows.Forms.Label();
            this.AMP_Calibration_label58 = new System.Windows.Forms.Label();
            this.AMP_Power_Output_label5 = new System.Windows.Forms.Label();
            this.AMP_Temp_MFC_AMP_label5 = new System.Windows.Forms.Label();
            this.AMP_Calibrate_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.AMP_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.AMP_label57 = new System.Windows.Forms.Label();
            this.AMP_Tune_button4 = new System.Windows.Forms.Button();
            this.IQBD_groupBox4 = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.IQBD_hScrollBar2 = new System.Windows.Forms.HScrollBar();
            this.IQBD_Apply_button8 = new System.Windows.Forms.Button();
            this.IQBD_ONOFF = new System.Windows.Forms.Button();
            this.IQBD_Monitor_label = new System.Windows.Forms.Label();
            this.IQBD_Tune_button8 = new System.Windows.Forms.Button();
            this.metertab = new System.Windows.Forms.TabPage();
            this.Freq_Comp_label32 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.Compression_Level_hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.Volume_Attn_listBox1 = new System.Windows.Forms.ListBox();
            this.Compression_button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Compression_label44 = new System.Windows.Forms.Label();
            this.Mic_Gain_label2 = new System.Windows.Forms.Label();
            this.Mic_Gain_Step_listBox1 = new System.Windows.Forms.ListBox();
            this.Time_checkBox2 = new System.Windows.Forms.CheckBox();
            this.SMeter_groupBox4 = new System.Windows.Forms.GroupBox();
            this.Meter_Peak_Needle_Color = new System.Windows.Forms.ListBox();
            this.Peak_Needle_Delay_listBox1 = new System.Windows.Forms.ListBox();
            this.Peak_Needle_checkBox2 = new System.Windows.Forms.CheckBox();
            this.Peak_Hold_listBox1 = new System.Windows.Forms.ListBox();
            this.Peak_hold_on_offlistBox1 = new System.Windows.Forms.ListBox();
            this.button5 = new System.Windows.Forms.Button();
            this.RPi_Temperature_label1 = new System.Windows.Forms.Label();
            this.Colors_groupBox = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Freq_Digit_Test_label58 = new System.Windows.Forms.Label();
            this.Freq_Color_button4 = new System.Windows.Forms.Button();
            this.Boarder_Color_button4 = new System.Windows.Forms.Button();
            this.AMP_Current_label5 = new System.Windows.Forms.Label();
            this.Amplifier_temperature_label58 = new System.Windows.Forms.Label();
            this.Temperature_label57 = new System.Windows.Forms.Label();
            this.Relay_Board_checkBox2 = new System.Windows.Forms.CheckBox();
            this.MSCC_Core_Version_label45 = new System.Windows.Forms.Label();
            this.MSCC_Display_label44 = new System.Windows.Forms.Label();
            this.HR50_PPT_listBox1 = new System.Windows.Forms.ListBox();
            this.HR50_checkBox2 = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.HR50_listBox1 = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.Zip_button2 = new System.Windows.Forms.Button();
            this.Restore_button2 = new System.Windows.Forms.Button();
            this.SDRcore_Trans_Version = new System.Windows.Forms.Label();
            this.SDRcore_Recv_Version_label16 = new System.Windows.Forms.Label();
            this.MS_SDR_Version_label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.firmwarelabel16 = new System.Windows.Forms.Label();
            this.Transvertercheckbox = new System.Windows.Forms.CheckBox();
            this.Monitorbutton = new System.Windows.Forms.Button();
            this.Keep_Alive_timer = new System.Windows.Forms.Timer(this.components);
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.Freq_Cal_timer4 = new System.Windows.Forms.Timer(this.components);
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.Window_Refresh_timer = new System.Windows.Forms.Timer(this.components);
            this.Smeter_Timer = new System.Windows.Forms.Timer(this.components);
            this.Smeter_toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.Cursor_timer2 = new System.Windows.Forms.Timer(this.components);
            this.Waterfall_timer = new System.Windows.Forms.Timer(this.components);
            this.RPi_Display_Timer = new System.Windows.Forms.Timer(this.components);
            this.powertabControl1.SuspendLayout();
            this.mainPage.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.RIT_groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWaterfall)).BeginInit();
            this.TX.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Power_Meter_Hold)).BeginInit();
            this.band_stack.SuspendLayout();
            this.freqcaltab.SuspendLayout();
            this.IQ_groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.Freq_Cal_groupBox4.SuspendLayout();
            this.powertabPage1.SuspendLayout();
            this.Audio_tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CW_Hold_numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CW_Lag_numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.MFC.SuspendLayout();
            this.Tuning_Knob_groupBox1.SuspendLayout();
            this.AMP_groupBox3.SuspendLayout();
            this.IQBD_groupBox4.SuspendLayout();
            this.metertab.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SMeter_groupBox4.SuspendLayout();
            this.Colors_groupBox.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // powertabControl1
            // 
            this.powertabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.powertabControl1.Controls.Add(this.mainPage);
            this.powertabControl1.Controls.Add(this.TX);
            this.powertabControl1.Controls.Add(this.band_stack);
            this.powertabControl1.Controls.Add(this.freqcaltab);
            this.powertabControl1.Controls.Add(this.powertabPage1);
            this.powertabControl1.Controls.Add(this.Audio_tabPage1);
            this.powertabControl1.Controls.Add(this.MFC);
            this.powertabControl1.Controls.Add(this.metertab);
            this.powertabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.powertabControl1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.powertabControl1.HotTrack = true;
            this.powertabControl1.ItemSize = new System.Drawing.Size(96, 21);
            this.powertabControl1.Location = new System.Drawing.Point(0, 0);
            this.powertabControl1.Name = "powertabControl1";
            this.powertabControl1.Padding = new System.Drawing.Point(0, 0);
            this.powertabControl1.SelectedIndex = 0;
            this.powertabControl1.ShowToolTips = true;
            this.powertabControl1.Size = new System.Drawing.Size(800, 480);
            this.powertabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.powertabControl1.TabIndex = 0;
            this.powertabControl1.SelectedIndexChanged += new System.EventHandler(this.powertabControl1_SelectedIndexChanged);
            // 
            // mainPage
            // 
            this.mainPage.AutoScroll = true;
            this.mainPage.BackColor = System.Drawing.Color.Black;
            this.mainPage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mainPage.Controls.Add(this.Microphone_textBox2);
            this.mainPage.Controls.Add(this.Volume_textBox2);
            this.mainPage.Controls.Add(this.mainlistBox1);
            this.mainPage.Controls.Add(this.MFC_Knob_label38);
            this.mainPage.Controls.Add(this.MFC_C_label38);
            this.mainPage.Controls.Add(this.MFC_B_label38);
            this.mainPage.Controls.Add(this.MFC_A_label38);
            this.mainPage.Controls.Add(this.UTC_Date_label46);
            this.mainPage.Controls.Add(this.Time_display_UTC_label34);
            this.mainPage.Controls.Add(this.Zedgraph_Control);
            this.mainPage.Controls.Add(this.NR_button3);
            this.mainPage.Controls.Add(this.Minimize_checkBox2);
            this.mainPage.Controls.Add(this.Main_Power_hScrollBar1);
            this.mainPage.Controls.Add(this.Meter_Mode_button8);
            this.mainPage.Controls.Add(this.groupBox3);
            this.mainPage.Controls.Add(this.Audio_Digital_button3);
            this.mainPage.Controls.Add(this.button7);
            this.mainPage.Controls.Add(this.Spectrum_Controls_button3);
            this.mainPage.Controls.Add(this.RIT_groupBox4);
            this.mainPage.Controls.Add(this.button6);
            this.mainPage.Controls.Add(this.button4);
            this.mainPage.Controls.Add(this.ACG_button);
            this.mainPage.Controls.Add(this.Compression_button4);
            this.mainPage.Controls.Add(this.Auto_Zero_checkBox2);
            this.mainPage.Controls.Add(this.Filter_listBox1);
            this.mainPage.Controls.Add(this.CW_Filter_listBox1);
            this.mainPage.Controls.Add(this.Filter_Low_listBox1);
            this.mainPage.Controls.Add(this.Local_Date_label46);
            this.mainPage.Controls.Add(this.NB_button2);
            this.mainPage.Controls.Add(this.Time_display_label33);
            this.mainPage.Controls.Add(this.TX_Mute_button2);
            this.mainPage.Controls.Add(this.Volume_Mute_button2);
            this.mainPage.Controls.Add(this.MicVolume_hScrollBar1);
            this.mainPage.Controls.Add(this.Volume_hScrollBar1);
            this.mainPage.Controls.Add(this.Freqbutton3);
            this.mainPage.Controls.Add(this.button1);
            this.mainPage.Controls.Add(this.genradioButton);
            this.mainPage.Controls.Add(this.mainmodebutton2);
            this.mainPage.Controls.Add(this.main10radioButton1);
            this.mainPage.Controls.Add(this.main12radioButton2);
            this.mainPage.Controls.Add(this.main15radiobutton);
            this.mainPage.Controls.Add(this.main17radioButton4);
            this.mainPage.Controls.Add(this.main20radioButton5);
            this.mainPage.Controls.Add(this.main30radioButton6);
            this.mainPage.Controls.Add(this.main40radioButton7);
            this.mainPage.Controls.Add(this.main60radioButton8);
            this.mainPage.Controls.Add(this.main80radioButton9);
            this.mainPage.Controls.Add(this.main160radioButton10);
            this.mainPage.Controls.Add(this.buttTune);
            this.mainPage.Controls.Add(this.Band_Change_Auto_Tune_checkBox2);
            this.mainPage.Controls.Add(this.panel2);
            this.mainPage.Controls.Add(this.label9);
            this.mainPage.Controls.Add(this.Power_Value_label2);
            this.mainPage.Controls.Add(this.vuMeter1);
            this.mainPage.Controls.Add(this.picWaterfall);
            this.mainPage.ForeColor = System.Drawing.Color.Black;
            this.mainPage.Location = new System.Drawing.Point(4, 25);
            this.mainPage.Margin = new System.Windows.Forms.Padding(0);
            this.mainPage.Name = "mainPage";
            this.mainPage.Size = new System.Drawing.Size(792, 451);
            this.mainPage.TabIndex = 0;
            this.mainPage.Text = "Main";
            this.mainPage.ToolTipText = "Main radio parameters";
            this.mainPage.Click += new System.EventHandler(this.mainPage_Click);
            this.mainPage.Enter += new System.EventHandler(this.mainPage_Enter);
            this.mainPage.Leave += new System.EventHandler(this.mainPage_Leave);
            // 
            // Microphone_textBox2
            // 
            this.Microphone_textBox2.BackColor = System.Drawing.Color.Gainsboro;
            this.Microphone_textBox2.ForeColor = System.Drawing.Color.Black;
            this.Microphone_textBox2.Location = new System.Drawing.Point(543, 170);
            this.Microphone_textBox2.Name = "Microphone_textBox2";
            this.Microphone_textBox2.Size = new System.Drawing.Size(39, 21);
            this.Microphone_textBox2.TabIndex = 158;
            this.Microphone_textBox2.Text = "100";
            this.Microphone_textBox2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Microphone_textBox2.Click += new System.EventHandler(this.Microphone_textBox2_Click);
            // 
            // Volume_textBox2
            // 
            this.Volume_textBox2.BackColor = System.Drawing.Color.Gainsboro;
            this.Volume_textBox2.ForeColor = System.Drawing.Color.Black;
            this.Volume_textBox2.Location = new System.Drawing.Point(212, 170);
            this.Volume_textBox2.Name = "Volume_textBox2";
            this.Volume_textBox2.Size = new System.Drawing.Size(39, 21);
            this.Volume_textBox2.TabIndex = 157;
            this.Volume_textBox2.Text = "100";
            this.Volume_textBox2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Volume_textBox2.Click += new System.EventHandler(this.Volume_textBox2_Click);
            // 
            // mainlistBox1
            // 
            this.mainlistBox1.BackColor = System.Drawing.Color.White;
            this.mainlistBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainlistBox1.ForeColor = System.Drawing.Color.Black;
            this.mainlistBox1.FormattingEnabled = true;
            this.mainlistBox1.Items.AddRange(new object[] {
            "100K",
            "10K",
            "1K",
            "100",
            "10",
            "1"});
            this.mainlistBox1.Location = new System.Drawing.Point(500, 84);
            this.mainlistBox1.Name = "mainlistBox1";
            this.mainlistBox1.ScrollAlwaysVisible = true;
            this.mainlistBox1.Size = new System.Drawing.Size(56, 17);
            this.mainlistBox1.TabIndex = 193;
            this.toolTip.SetToolTip(this.mainlistBox1, "Frequency Step Increment");
            this.mainlistBox1.SelectedIndexChanged += new System.EventHandler(this.mainlistBox1_SelectedIndexChanged);
            // 
            // MFC_Knob_label38
            // 
            this.MFC_Knob_label38.BackColor = System.Drawing.Color.Gainsboro;
            this.MFC_Knob_label38.Location = new System.Drawing.Point(537, 203);
            this.MFC_Knob_label38.Name = "MFC_Knob_label38";
            this.MFC_Knob_label38.Size = new System.Drawing.Size(97, 16);
            this.MFC_Knob_label38.TabIndex = 192;
            this.MFC_Knob_label38.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.MFC_Knob_label38, "MFC - Knob Switch");
            // 
            // MFC_C_label38
            // 
            this.MFC_C_label38.BackColor = System.Drawing.Color.Gainsboro;
            this.MFC_C_label38.Location = new System.Drawing.Point(411, 203);
            this.MFC_C_label38.Name = "MFC_C_label38";
            this.MFC_C_label38.Size = new System.Drawing.Size(97, 16);
            this.MFC_C_label38.TabIndex = 191;
            this.MFC_C_label38.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.MFC_C_label38, "MFC - Button C");
            this.MFC_C_label38.Click += new System.EventHandler(this.MFC_C_label38_Click);
            // 
            // MFC_B_label38
            // 
            this.MFC_B_label38.BackColor = System.Drawing.Color.Gainsboro;
            this.MFC_B_label38.Location = new System.Drawing.Point(285, 203);
            this.MFC_B_label38.Name = "MFC_B_label38";
            this.MFC_B_label38.Size = new System.Drawing.Size(97, 16);
            this.MFC_B_label38.TabIndex = 190;
            this.MFC_B_label38.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.MFC_B_label38, "MFC - Button B");
            this.MFC_B_label38.Click += new System.EventHandler(this.MFC_B_label38_Click);
            // 
            // MFC_A_label38
            // 
            this.MFC_A_label38.BackColor = System.Drawing.Color.Gainsboro;
            this.MFC_A_label38.Location = new System.Drawing.Point(159, 203);
            this.MFC_A_label38.Name = "MFC_A_label38";
            this.MFC_A_label38.Size = new System.Drawing.Size(97, 16);
            this.MFC_A_label38.TabIndex = 189;
            this.MFC_A_label38.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.MFC_A_label38, "MFC - Button A");
            this.MFC_A_label38.Click += new System.EventHandler(this.MFC_A_label38_Click);
            // 
            // UTC_Date_label46
            // 
            this.UTC_Date_label46.BackColor = System.Drawing.Color.Transparent;
            this.UTC_Date_label46.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UTC_Date_label46.ForeColor = System.Drawing.Color.White;
            this.UTC_Date_label46.Location = new System.Drawing.Point(726, 118);
            this.UTC_Date_label46.Name = "UTC_Date_label46";
            this.UTC_Date_label46.Size = new System.Drawing.Size(63, 23);
            this.UTC_Date_label46.TabIndex = 143;
            this.UTC_Date_label46.Text = "12.12.20";
            this.UTC_Date_label46.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.UTC_Date_label46.Visible = false;
            this.UTC_Date_label46.Click += new System.EventHandler(this.UTC_Date_label46_Click);
            // 
            // Time_display_UTC_label34
            // 
            this.Time_display_UTC_label34.BackColor = System.Drawing.Color.Transparent;
            this.Time_display_UTC_label34.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Time_display_UTC_label34.ForeColor = System.Drawing.Color.White;
            this.Time_display_UTC_label34.Location = new System.Drawing.Point(726, 95);
            this.Time_display_UTC_label34.Name = "Time_display_UTC_label34";
            this.Time_display_UTC_label34.Size = new System.Drawing.Size(63, 23);
            this.Time_display_UTC_label34.TabIndex = 114;
            this.Time_display_UTC_label34.Text = "00:00:00";
            this.Time_display_UTC_label34.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Time_display_UTC_label34.Visible = false;
            this.Time_display_UTC_label34.Click += new System.EventHandler(this.Time_display_UTC_label34_Click);
            // 
            // Zedgraph_Control
            // 
            this.Zedgraph_Control.IsAntiAlias = true;
            this.Zedgraph_Control.IsAutoScrollRange = true;
            this.Zedgraph_Control.IsEnableHEdit = true;
            this.Zedgraph_Control.IsEnableSelection = true;
            this.Zedgraph_Control.IsEnableVEdit = true;
            this.Zedgraph_Control.IsEnableVPan = false;
            this.Zedgraph_Control.IsEnableVZoom = false;
            this.Zedgraph_Control.IsEnableWheelZoom = false;
            this.Zedgraph_Control.IsPrintFillPage = false;
            this.Zedgraph_Control.IsShowCursorValues = true;
            this.Zedgraph_Control.Location = new System.Drawing.Point(0, 228);
            this.Zedgraph_Control.Margin = new System.Windows.Forms.Padding(0);
            this.Zedgraph_Control.Name = "Zedgraph_Control";
            this.Zedgraph_Control.PanButtons = System.Windows.Forms.MouseButtons.None;
            this.Zedgraph_Control.PanButtons2 = System.Windows.Forms.MouseButtons.None;
            this.Zedgraph_Control.PanModifierKeys = System.Windows.Forms.Keys.None;
            this.Zedgraph_Control.ScrollGrace = 0D;
            this.Zedgraph_Control.ScrollMaxX = 10D;
            this.Zedgraph_Control.ScrollMaxY = 0D;
            this.Zedgraph_Control.ScrollMaxY2 = 0D;
            this.Zedgraph_Control.ScrollMinX = 800D;
            this.Zedgraph_Control.ScrollMinY = 0D;
            this.Zedgraph_Control.ScrollMinY2 = 0D;
            this.Zedgraph_Control.SelectButtons = System.Windows.Forms.MouseButtons.None;
            this.Zedgraph_Control.Size = new System.Drawing.Size(792, 112);
            this.Zedgraph_Control.TabIndex = 179;
            this.Zedgraph_Control.UseExtendedPrintDialog = true;
            this.Zedgraph_Control.Visible = false;
            this.Zedgraph_Control.ZoomButtons = System.Windows.Forms.MouseButtons.None;
            this.Zedgraph_Control.Load += new System.EventHandler(this.Zedgraph_Control_Load);
            this.Zedgraph_Control.MouseEnter += new System.EventHandler(this.Spectrum_Enter);
            this.Zedgraph_Control.MouseLeave += new System.EventHandler(this.Spectrum_Leave);
            // 
            // NR_button3
            // 
            this.NR_button3.BackColor = System.Drawing.Color.Gainsboro;
            this.NR_button3.Enabled = false;
            this.NR_button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.NR_button3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NR_button3.ForeColor = System.Drawing.Color.Black;
            this.NR_button3.Location = new System.Drawing.Point(103, 130);
            this.NR_button3.Name = "NR_button3";
            this.NR_button3.Size = new System.Drawing.Size(23, 21);
            this.NR_button3.TabIndex = 138;
            this.NR_button3.Text = "N";
            this.toolTip.SetToolTip(this.NR_button3, "Noise Reduction\r\nUnder Construction");
            this.NR_button3.UseVisualStyleBackColor = false;
            this.NR_button3.Visible = false;
            this.NR_button3.Click += new System.EventHandler(this.NR_button3_Click);
            // 
            // Minimize_checkBox2
            // 
            this.Minimize_checkBox2.BackColor = System.Drawing.Color.Transparent;
            this.Minimize_checkBox2.ForeColor = System.Drawing.Color.Black;
            this.Minimize_checkBox2.Location = new System.Drawing.Point(637, 46);
            this.Minimize_checkBox2.Name = "Minimize_checkBox2";
            this.Minimize_checkBox2.Size = new System.Drawing.Size(20, 20);
            this.Minimize_checkBox2.TabIndex = 188;
            this.toolTip.SetToolTip(this.Minimize_checkBox2, "Minimize MSCC Window");
            this.Minimize_checkBox2.UseVisualStyleBackColor = false;
            this.Minimize_checkBox2.CheckedChanged += new System.EventHandler(this.Minimize_checkBox2_CheckedChanged);
            // 
            // Main_Power_hScrollBar1
            // 
            this.Main_Power_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.Main_Power_hScrollBar1.Location = new System.Drawing.Point(588, 144);
            this.Main_Power_hScrollBar1.Maximum = 109;
            this.Main_Power_hScrollBar1.Name = "Main_Power_hScrollBar1";
            this.Main_Power_hScrollBar1.Size = new System.Drawing.Size(185, 10);
            this.Main_Power_hScrollBar1.TabIndex = 187;
            this.Main_Power_hScrollBar1.TabStop = true;
            this.toolTip.SetToolTip(this.Main_Power_hScrollBar1, "Power Output Adjustment  for current mode.");
            this.Main_Power_hScrollBar1.Value = 30;
            this.Main_Power_hScrollBar1.Visible = false;
            this.Main_Power_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Main_Power_hScrollBar1_Scroll);
            // 
            // Meter_Mode_button8
            // 
            this.Meter_Mode_button8.BackColor = System.Drawing.Color.Gainsboro;
            this.Meter_Mode_button8.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Meter_Mode_button8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Meter_Mode_button8.ForeColor = System.Drawing.Color.Black;
            this.Meter_Mode_button8.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Meter_Mode_button8.Location = new System.Drawing.Point(55, 71);
            this.Meter_Mode_button8.Name = "Meter_Mode_button8";
            this.Meter_Mode_button8.Size = new System.Drawing.Size(66, 20);
            this.Meter_Mode_button8.TabIndex = 186;
            this.Meter_Mode_button8.Text = "MODE";
            this.toolTip.SetToolTip(this.Meter_Mode_button8, "Common Controls for Spectrum and Waterfall\r\n");
            this.Meter_Mode_button8.UseVisualStyleBackColor = false;
            this.Meter_Mode_button8.Visible = false;
            this.Meter_Mode_button8.Click += new System.EventHandler(this.Meter_Mode_button8_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.Tenthousand_Top_button6);
            this.groupBox3.Controls.Add(this.Hundred_Thousand_Top_button5);
            this.groupBox3.Controls.Add(this.Million_Top_button4);
            this.groupBox3.Controls.Add(this.TenMillion_Bottom_button8);
            this.groupBox3.Controls.Add(this.Million_Bottom_button7);
            this.groupBox3.Controls.Add(this.Hundred_Thousand_Button_button6);
            this.groupBox3.Controls.Add(this.Tenthousand_Bottom_button5);
            this.groupBox3.Controls.Add(this.Thousand_Bottom_button);
            this.groupBox3.Controls.Add(this.Hundreds_Top_button8);
            this.groupBox3.Controls.Add(this.Hundred_Bottom_button3);
            this.groupBox3.Controls.Add(this.Tens_Top_button);
            this.groupBox3.Controls.Add(this.Tens_Bottom_button2);
            this.groupBox3.Controls.Add(this.Ones_Bottom_button2);
            this.groupBox3.Controls.Add(this.Ones_Top_button2);
            this.groupBox3.Controls.Add(this.Tenmillions);
            this.groupBox3.Controls.Add(this.Decimal_label58);
            this.groupBox3.Controls.Add(this.Decimal_label59);
            this.groupBox3.Controls.Add(this.Millions);
            this.groupBox3.Controls.Add(this.Hundredthousand);
            this.groupBox3.Controls.Add(this.Tenthousands);
            this.groupBox3.Controls.Add(this.Hundreds);
            this.groupBox3.Controls.Add(this.Tens);
            this.groupBox3.Controls.Add(this.Ones);
            this.groupBox3.Controls.Add(this.Thousand_Top_button7);
            this.groupBox3.Controls.Add(this.Thousands);
            this.groupBox3.Controls.Add(this.Ten_Million_Top_button3);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(220, 0);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(409, 69);
            this.groupBox3.TabIndex = 161;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "VFO A";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // Tenthousand_Top_button6
            // 
            this.Tenthousand_Top_button6.BackColor = System.Drawing.Color.Black;
            this.Tenthousand_Top_button6.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Tenthousand_Top_button6.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.Tenthousand_Top_button6.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.Tenthousand_Top_button6.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.Tenthousand_Top_button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Tenthousand_Top_button6.ForeColor = System.Drawing.Color.MediumOrchid;
            this.Tenthousand_Top_button6.Location = new System.Drawing.Point(175, 12);
            this.Tenthousand_Top_button6.Name = "Tenthousand_Top_button6";
            this.Tenthousand_Top_button6.Size = new System.Drawing.Size(18, 10);
            this.Tenthousand_Top_button6.TabIndex = 131;
            this.Tenthousand_Top_button6.UseVisualStyleBackColor = false;
            this.Tenthousand_Top_button6.Click += new System.EventHandler(this.Tenthousand_Top_button6_Click);
            // 
            // Hundred_Thousand_Top_button5
            // 
            this.Hundred_Thousand_Top_button5.BackColor = System.Drawing.Color.Black;
            this.Hundred_Thousand_Top_button5.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Hundred_Thousand_Top_button5.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.Hundred_Thousand_Top_button5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.Hundred_Thousand_Top_button5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.Hundred_Thousand_Top_button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Hundred_Thousand_Top_button5.ForeColor = System.Drawing.Color.MediumOrchid;
            this.Hundred_Thousand_Top_button5.Location = new System.Drawing.Point(134, 12);
            this.Hundred_Thousand_Top_button5.Name = "Hundred_Thousand_Top_button5";
            this.Hundred_Thousand_Top_button5.Size = new System.Drawing.Size(18, 10);
            this.Hundred_Thousand_Top_button5.TabIndex = 132;
            this.Hundred_Thousand_Top_button5.UseVisualStyleBackColor = false;
            this.Hundred_Thousand_Top_button5.Click += new System.EventHandler(this.Hundred_Thousand_Top_button5_Click);
            // 
            // Million_Top_button4
            // 
            this.Million_Top_button4.BackColor = System.Drawing.Color.Black;
            this.Million_Top_button4.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Million_Top_button4.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.Million_Top_button4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.Million_Top_button4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.Million_Top_button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Million_Top_button4.ForeColor = System.Drawing.Color.MediumOrchid;
            this.Million_Top_button4.Location = new System.Drawing.Point(73, 12);
            this.Million_Top_button4.Name = "Million_Top_button4";
            this.Million_Top_button4.Size = new System.Drawing.Size(18, 10);
            this.Million_Top_button4.TabIndex = 133;
            this.Million_Top_button4.UseVisualStyleBackColor = false;
            this.Million_Top_button4.Click += new System.EventHandler(this.Million_Top_button4_Click);
            // 
            // TenMillion_Bottom_button8
            // 
            this.TenMillion_Bottom_button8.BackColor = System.Drawing.Color.Black;
            this.TenMillion_Bottom_button8.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.TenMillion_Bottom_button8.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.TenMillion_Bottom_button8.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.TenMillion_Bottom_button8.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.TenMillion_Bottom_button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TenMillion_Bottom_button8.ForeColor = System.Drawing.Color.Black;
            this.TenMillion_Bottom_button8.Location = new System.Drawing.Point(32, 56);
            this.TenMillion_Bottom_button8.Name = "TenMillion_Bottom_button8";
            this.TenMillion_Bottom_button8.Size = new System.Drawing.Size(18, 10);
            this.TenMillion_Bottom_button8.TabIndex = 126;
            this.TenMillion_Bottom_button8.UseVisualStyleBackColor = false;
            this.TenMillion_Bottom_button8.Click += new System.EventHandler(this.TenMillion_Bottom_button8_Click);
            // 
            // Million_Bottom_button7
            // 
            this.Million_Bottom_button7.BackColor = System.Drawing.Color.Black;
            this.Million_Bottom_button7.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Million_Bottom_button7.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.Million_Bottom_button7.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.Million_Bottom_button7.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.Million_Bottom_button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Million_Bottom_button7.ForeColor = System.Drawing.Color.Black;
            this.Million_Bottom_button7.Location = new System.Drawing.Point(71, 55);
            this.Million_Bottom_button7.Name = "Million_Bottom_button7";
            this.Million_Bottom_button7.Size = new System.Drawing.Size(18, 10);
            this.Million_Bottom_button7.TabIndex = 125;
            this.Million_Bottom_button7.UseVisualStyleBackColor = false;
            this.Million_Bottom_button7.Click += new System.EventHandler(this.Million_Bottom_button7_Click);
            // 
            // Hundred_Thousand_Button_button6
            // 
            this.Hundred_Thousand_Button_button6.BackColor = System.Drawing.Color.Black;
            this.Hundred_Thousand_Button_button6.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Hundred_Thousand_Button_button6.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.Hundred_Thousand_Button_button6.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.Hundred_Thousand_Button_button6.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.Hundred_Thousand_Button_button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Hundred_Thousand_Button_button6.ForeColor = System.Drawing.Color.Black;
            this.Hundred_Thousand_Button_button6.Location = new System.Drawing.Point(134, 55);
            this.Hundred_Thousand_Button_button6.Name = "Hundred_Thousand_Button_button6";
            this.Hundred_Thousand_Button_button6.Size = new System.Drawing.Size(18, 10);
            this.Hundred_Thousand_Button_button6.TabIndex = 124;
            this.Hundred_Thousand_Button_button6.UseVisualStyleBackColor = false;
            this.Hundred_Thousand_Button_button6.Click += new System.EventHandler(this.Hundred_Thousand_Button_button6_Click);
            // 
            // Tenthousand_Bottom_button5
            // 
            this.Tenthousand_Bottom_button5.BackColor = System.Drawing.Color.Black;
            this.Tenthousand_Bottom_button5.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Tenthousand_Bottom_button5.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.Tenthousand_Bottom_button5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.Tenthousand_Bottom_button5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.Tenthousand_Bottom_button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Tenthousand_Bottom_button5.ForeColor = System.Drawing.Color.Black;
            this.Tenthousand_Bottom_button5.Location = new System.Drawing.Point(175, 55);
            this.Tenthousand_Bottom_button5.Name = "Tenthousand_Bottom_button5";
            this.Tenthousand_Bottom_button5.Size = new System.Drawing.Size(18, 10);
            this.Tenthousand_Bottom_button5.TabIndex = 123;
            this.Tenthousand_Bottom_button5.UseVisualStyleBackColor = false;
            this.Tenthousand_Bottom_button5.Click += new System.EventHandler(this.Tenthousand_Bottom_button5_Click);
            // 
            // Thousand_Bottom_button
            // 
            this.Thousand_Bottom_button.BackColor = System.Drawing.Color.Black;
            this.Thousand_Bottom_button.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Thousand_Bottom_button.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.Thousand_Bottom_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.Thousand_Bottom_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.Thousand_Bottom_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Thousand_Bottom_button.ForeColor = System.Drawing.Color.Black;
            this.Thousand_Bottom_button.Location = new System.Drawing.Point(217, 55);
            this.Thousand_Bottom_button.Name = "Thousand_Bottom_button";
            this.Thousand_Bottom_button.Size = new System.Drawing.Size(18, 10);
            this.Thousand_Bottom_button.TabIndex = 122;
            this.Thousand_Bottom_button.UseVisualStyleBackColor = false;
            this.Thousand_Bottom_button.Click += new System.EventHandler(this.Thousand_Bottom_button_Click);
            // 
            // Hundreds_Top_button8
            // 
            this.Hundreds_Top_button8.BackColor = System.Drawing.Color.Black;
            this.Hundreds_Top_button8.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Hundreds_Top_button8.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.Hundreds_Top_button8.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.Hundreds_Top_button8.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.Hundreds_Top_button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Hundreds_Top_button8.ForeColor = System.Drawing.Color.MediumOrchid;
            this.Hundreds_Top_button8.Location = new System.Drawing.Point(275, 12);
            this.Hundreds_Top_button8.Name = "Hundreds_Top_button8";
            this.Hundreds_Top_button8.Size = new System.Drawing.Size(18, 10);
            this.Hundreds_Top_button8.TabIndex = 129;
            this.Hundreds_Top_button8.UseVisualStyleBackColor = false;
            this.Hundreds_Top_button8.Click += new System.EventHandler(this.Hundreds_Top_button8_Click);
            // 
            // Hundred_Bottom_button3
            // 
            this.Hundred_Bottom_button3.BackColor = System.Drawing.Color.Black;
            this.Hundred_Bottom_button3.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Hundred_Bottom_button3.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.Hundred_Bottom_button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.Hundred_Bottom_button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.Hundred_Bottom_button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Hundred_Bottom_button3.ForeColor = System.Drawing.Color.Black;
            this.Hundred_Bottom_button3.Location = new System.Drawing.Point(275, 56);
            this.Hundred_Bottom_button3.Name = "Hundred_Bottom_button3";
            this.Hundred_Bottom_button3.Size = new System.Drawing.Size(18, 10);
            this.Hundred_Bottom_button3.TabIndex = 121;
            this.Hundred_Bottom_button3.UseVisualStyleBackColor = false;
            this.Hundred_Bottom_button3.Click += new System.EventHandler(this.Hundred_Bottom_button3_Click);
            // 
            // Tens_Top_button
            // 
            this.Tens_Top_button.BackColor = System.Drawing.Color.Black;
            this.Tens_Top_button.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Tens_Top_button.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.Tens_Top_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.Tens_Top_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.Tens_Top_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Tens_Top_button.ForeColor = System.Drawing.Color.MediumOrchid;
            this.Tens_Top_button.Location = new System.Drawing.Point(318, 12);
            this.Tens_Top_button.Name = "Tens_Top_button";
            this.Tens_Top_button.Size = new System.Drawing.Size(18, 10);
            this.Tens_Top_button.TabIndex = 128;
            this.Tens_Top_button.UseVisualStyleBackColor = false;
            this.Tens_Top_button.Click += new System.EventHandler(this.Tens_Top_button_Click);
            // 
            // Tens_Bottom_button2
            // 
            this.Tens_Bottom_button2.BackColor = System.Drawing.Color.Black;
            this.Tens_Bottom_button2.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Tens_Bottom_button2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.Tens_Bottom_button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.Tens_Bottom_button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.Tens_Bottom_button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Tens_Bottom_button2.ForeColor = System.Drawing.Color.Black;
            this.Tens_Bottom_button2.Location = new System.Drawing.Point(318, 56);
            this.Tens_Bottom_button2.Name = "Tens_Bottom_button2";
            this.Tens_Bottom_button2.Size = new System.Drawing.Size(18, 10);
            this.Tens_Bottom_button2.TabIndex = 120;
            this.Tens_Bottom_button2.UseVisualStyleBackColor = false;
            this.Tens_Bottom_button2.Click += new System.EventHandler(this.Tens_Bottom_button2_Click);
            // 
            // Ones_Bottom_button2
            // 
            this.Ones_Bottom_button2.BackColor = System.Drawing.Color.Black;
            this.Ones_Bottom_button2.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Ones_Bottom_button2.FlatAppearance.CheckedBackColor = System.Drawing.Color.MediumOrchid;
            this.Ones_Bottom_button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.MediumOrchid;
            this.Ones_Bottom_button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.MediumOrchid;
            this.Ones_Bottom_button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Ones_Bottom_button2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Ones_Bottom_button2.ForeColor = System.Drawing.Color.Black;
            this.Ones_Bottom_button2.Location = new System.Drawing.Point(359, 55);
            this.Ones_Bottom_button2.Name = "Ones_Bottom_button2";
            this.Ones_Bottom_button2.Size = new System.Drawing.Size(18, 10);
            this.Ones_Bottom_button2.TabIndex = 119;
            this.Ones_Bottom_button2.UseVisualStyleBackColor = false;
            this.Ones_Bottom_button2.Click += new System.EventHandler(this.Ones_Bottom_button2_Click);
            // 
            // Ones_Top_button2
            // 
            this.Ones_Top_button2.BackColor = System.Drawing.Color.Black;
            this.Ones_Top_button2.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Ones_Top_button2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.Ones_Top_button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.Ones_Top_button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.Ones_Top_button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Ones_Top_button2.ForeColor = System.Drawing.Color.Transparent;
            this.Ones_Top_button2.Location = new System.Drawing.Point(359, 12);
            this.Ones_Top_button2.Name = "Ones_Top_button2";
            this.Ones_Top_button2.Size = new System.Drawing.Size(18, 10);
            this.Ones_Top_button2.TabIndex = 127;
            this.Ones_Top_button2.UseVisualStyleBackColor = false;
            this.Ones_Top_button2.Click += new System.EventHandler(this.Ones_Top_button2_Click);
            // 
            // Tenmillions
            // 
            this.Tenmillions.AutoSize = true;
            this.Tenmillions.BackColor = System.Drawing.Color.Transparent;
            this.Tenmillions.Font = new System.Drawing.Font("Times New Roman", 26.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tenmillions.ForeColor = System.Drawing.Color.White;
            this.Tenmillions.Location = new System.Drawing.Point(23, 19);
            this.Tenmillions.Name = "Tenmillions";
            this.Tenmillions.Size = new System.Drawing.Size(36, 41);
            this.Tenmillions.TabIndex = 95;
            this.Tenmillions.Text = "1";
            this.Tenmillions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Tenmillions.Click += new System.EventHandler(this.Tenmillions_Click);
            this.Tenmillions.MouseEnter += new System.EventHandler(this.Tenmillions_MouseEnter);
            this.Tenmillions.MouseLeave += new System.EventHandler(this.Tenmillions_MouseExit);
            // 
            // Decimal_label58
            // 
            this.Decimal_label58.AutoSize = true;
            this.Decimal_label58.BackColor = System.Drawing.Color.Transparent;
            this.Decimal_label58.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Decimal_label58.ForeColor = System.Drawing.Color.White;
            this.Decimal_label58.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Decimal_label58.Location = new System.Drawing.Point(247, 35);
            this.Decimal_label58.Name = "Decimal_label58";
            this.Decimal_label58.Size = new System.Drawing.Size(15, 22);
            this.Decimal_label58.TabIndex = 159;
            this.Decimal_label58.Text = ".";
            this.Decimal_label58.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Decimal_label59
            // 
            this.Decimal_label59.AutoSize = true;
            this.Decimal_label59.BackColor = System.Drawing.Color.Transparent;
            this.Decimal_label59.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Decimal_label59.ForeColor = System.Drawing.Color.White;
            this.Decimal_label59.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Decimal_label59.Location = new System.Drawing.Point(104, 35);
            this.Decimal_label59.Name = "Decimal_label59";
            this.Decimal_label59.Size = new System.Drawing.Size(15, 22);
            this.Decimal_label59.TabIndex = 160;
            this.Decimal_label59.Text = ".";
            this.Decimal_label59.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Millions
            // 
            this.Millions.AutoSize = true;
            this.Millions.BackColor = System.Drawing.Color.Transparent;
            this.Millions.Font = new System.Drawing.Font("Times New Roman", 26.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Millions.ForeColor = System.Drawing.Color.White;
            this.Millions.Location = new System.Drawing.Point(64, 19);
            this.Millions.Name = "Millions";
            this.Millions.Size = new System.Drawing.Size(36, 41);
            this.Millions.TabIndex = 94;
            this.Millions.Text = "4";
            this.Millions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Millions.Click += new System.EventHandler(this.Millions_Click);
            this.Millions.MouseEnter += new System.EventHandler(this.Millions_MouseEnter);
            this.Millions.MouseLeave += new System.EventHandler(this.Millions_MouseExit);
            // 
            // Hundredthousand
            // 
            this.Hundredthousand.AutoSize = true;
            this.Hundredthousand.BackColor = System.Drawing.Color.Transparent;
            this.Hundredthousand.Font = new System.Drawing.Font("Times New Roman", 26.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Hundredthousand.ForeColor = System.Drawing.Color.White;
            this.Hundredthousand.Location = new System.Drawing.Point(125, 19);
            this.Hundredthousand.Name = "Hundredthousand";
            this.Hundredthousand.Size = new System.Drawing.Size(36, 41);
            this.Hundredthousand.TabIndex = 93;
            this.Hundredthousand.Text = "1";
            this.Hundredthousand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Hundredthousand.Click += new System.EventHandler(this.Hundredthousand_Click);
            this.Hundredthousand.MouseEnter += new System.EventHandler(this.Hundredthousand_MouseEnter);
            this.Hundredthousand.MouseLeave += new System.EventHandler(this.Hundredthousand_MouseExit);
            // 
            // Tenthousands
            // 
            this.Tenthousands.AutoSize = true;
            this.Tenthousands.BackColor = System.Drawing.Color.Transparent;
            this.Tenthousands.Font = new System.Drawing.Font("Times New Roman", 26.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tenthousands.ForeColor = System.Drawing.Color.White;
            this.Tenthousands.Location = new System.Drawing.Point(166, 19);
            this.Tenthousands.Name = "Tenthousands";
            this.Tenthousands.Size = new System.Drawing.Size(36, 41);
            this.Tenthousands.TabIndex = 92;
            this.Tenthousands.Text = "9";
            this.Tenthousands.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Tenthousands.Click += new System.EventHandler(this.Tenthousands_Click);
            this.Tenthousands.MouseEnter += new System.EventHandler(this.Tenthousands_MouseEnter);
            this.Tenthousands.MouseLeave += new System.EventHandler(this.Tenthousands_MouseExit);
            // 
            // Hundreds
            // 
            this.Hundreds.AutoSize = true;
            this.Hundreds.BackColor = System.Drawing.Color.Transparent;
            this.Hundreds.Font = new System.Drawing.Font("Times New Roman", 26.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Hundreds.ForeColor = System.Drawing.Color.White;
            this.Hundreds.Location = new System.Drawing.Point(266, 19);
            this.Hundreds.Name = "Hundreds";
            this.Hundreds.Size = new System.Drawing.Size(36, 41);
            this.Hundreds.TabIndex = 90;
            this.Hundreds.Text = "1";
            this.Hundreds.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Hundreds.Click += new System.EventHandler(this.Hundreds_Click);
            this.Hundreds.MouseEnter += new System.EventHandler(this.Hundreds_MouseEnter);
            this.Hundreds.MouseLeave += new System.EventHandler(this.Hundreds_MouseExit);
            // 
            // Tens
            // 
            this.Tens.AutoSize = true;
            this.Tens.BackColor = System.Drawing.Color.Transparent;
            this.Tens.Font = new System.Drawing.Font("Times New Roman", 26.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tens.ForeColor = System.Drawing.Color.White;
            this.Tens.Location = new System.Drawing.Point(309, 19);
            this.Tens.Name = "Tens";
            this.Tens.Size = new System.Drawing.Size(36, 41);
            this.Tens.TabIndex = 89;
            this.Tens.Text = "1";
            this.Tens.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Tens.Click += new System.EventHandler(this.Tens_Click);
            this.Tens.MouseEnter += new System.EventHandler(this.Tens_MouseEnter);
            this.Tens.MouseLeave += new System.EventHandler(this.Tens_MouseExit);
            // 
            // Ones
            // 
            this.Ones.AutoSize = true;
            this.Ones.BackColor = System.Drawing.Color.Transparent;
            this.Ones.Font = new System.Drawing.Font("Times New Roman", 26.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Ones.ForeColor = System.Drawing.Color.White;
            this.Ones.Location = new System.Drawing.Point(350, 19);
            this.Ones.Name = "Ones";
            this.Ones.Size = new System.Drawing.Size(36, 41);
            this.Ones.TabIndex = 88;
            this.Ones.Text = "0";
            this.Ones.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Ones.Click += new System.EventHandler(this.Ones_Click);
            this.Ones.MouseEnter += new System.EventHandler(this.Ones_MouseEnter);
            this.Ones.MouseLeave += new System.EventHandler(this.Ones_MouseExit);
            // 
            // Thousand_Top_button7
            // 
            this.Thousand_Top_button7.BackColor = System.Drawing.Color.Black;
            this.Thousand_Top_button7.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Thousand_Top_button7.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.Thousand_Top_button7.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.Thousand_Top_button7.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.Thousand_Top_button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Thousand_Top_button7.ForeColor = System.Drawing.Color.MediumOrchid;
            this.Thousand_Top_button7.Location = new System.Drawing.Point(217, 12);
            this.Thousand_Top_button7.Name = "Thousand_Top_button7";
            this.Thousand_Top_button7.Size = new System.Drawing.Size(18, 10);
            this.Thousand_Top_button7.TabIndex = 130;
            this.Thousand_Top_button7.UseVisualStyleBackColor = false;
            this.Thousand_Top_button7.Click += new System.EventHandler(this.Thousand_Top_button7_Click);
            // 
            // Thousands
            // 
            this.Thousands.AutoSize = true;
            this.Thousands.BackColor = System.Drawing.Color.Transparent;
            this.Thousands.Font = new System.Drawing.Font("Times New Roman", 26.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Thousands.ForeColor = System.Drawing.Color.White;
            this.Thousands.Location = new System.Drawing.Point(208, 19);
            this.Thousands.Name = "Thousands";
            this.Thousands.Size = new System.Drawing.Size(36, 41);
            this.Thousands.TabIndex = 91;
            this.Thousands.Text = "5";
            this.Thousands.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Thousands.Click += new System.EventHandler(this.Thousands_Click);
            this.Thousands.MouseEnter += new System.EventHandler(this.Thousands_MouseEnter);
            this.Thousands.MouseLeave += new System.EventHandler(this.Thousands_MouseExit);
            // 
            // Ten_Million_Top_button3
            // 
            this.Ten_Million_Top_button3.BackColor = System.Drawing.Color.Black;
            this.Ten_Million_Top_button3.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Ten_Million_Top_button3.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.Ten_Million_Top_button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.Ten_Million_Top_button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.Ten_Million_Top_button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Ten_Million_Top_button3.ForeColor = System.Drawing.Color.Transparent;
            this.Ten_Million_Top_button3.Location = new System.Drawing.Point(32, 14);
            this.Ten_Million_Top_button3.Name = "Ten_Million_Top_button3";
            this.Ten_Million_Top_button3.Size = new System.Drawing.Size(18, 10);
            this.Ten_Million_Top_button3.TabIndex = 134;
            this.Ten_Million_Top_button3.UseVisualStyleBackColor = false;
            this.Ten_Million_Top_button3.Click += new System.EventHandler(this.Ten_Million_Top_button3_Click);
            // 
            // Audio_Digital_button3
            // 
            this.Audio_Digital_button3.BackColor = System.Drawing.Color.Gainsboro;
            this.Audio_Digital_button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Audio_Digital_button3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Audio_Digital_button3.ForeColor = System.Drawing.Color.Black;
            this.Audio_Digital_button3.Location = new System.Drawing.Point(723, 157);
            this.Audio_Digital_button3.Name = "Audio_Digital_button3";
            this.Audio_Digital_button3.Size = new System.Drawing.Size(26, 21);
            this.Audio_Digital_button3.TabIndex = 185;
            this.Audio_Digital_button3.Text = "V";
            this.toolTip.SetToolTip(this.Audio_Digital_button3, "Preferred MIC Gain\r\nV - Voice\r\nD - Digital (Disables Compression)");
            this.Audio_Digital_button3.UseVisualStyleBackColor = false;
            this.Audio_Digital_button3.Click += new System.EventHandler(this.Audio_Digital_button3_Click);
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.Color.Gainsboro;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.ForeColor = System.Drawing.Color.Black;
            this.button7.Location = new System.Drawing.Point(169, 58);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(43, 22);
            this.button7.TabIndex = 184;
            this.button7.Text = "SPT";
            this.toolTip.SetToolTip(this.button7, "Operate Split\r\n(Not yet implemented)");
            this.button7.UseVisualStyleBackColor = false;
            // 
            // Spectrum_Controls_button3
            // 
            this.Spectrum_Controls_button3.BackColor = System.Drawing.Color.Gainsboro;
            this.Spectrum_Controls_button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Spectrum_Controls_button3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Spectrum_Controls_button3.ForeColor = System.Drawing.Color.Black;
            this.Spectrum_Controls_button3.Location = new System.Drawing.Point(679, 58);
            this.Spectrum_Controls_button3.Name = "Spectrum_Controls_button3";
            this.Spectrum_Controls_button3.Size = new System.Drawing.Size(43, 22);
            this.Spectrum_Controls_button3.TabIndex = 181;
            this.Spectrum_Controls_button3.Text = "S/W";
            this.toolTip.SetToolTip(this.Spectrum_Controls_button3, "Common Controls for Spectrum and Waterfall\r\n");
            this.Spectrum_Controls_button3.UseVisualStyleBackColor = false;
            this.Spectrum_Controls_button3.Click += new System.EventHandler(this.Spectrum_Controls_button3_Click);
            // 
            // RIT_groupBox4
            // 
            this.RIT_groupBox4.Controls.Add(this.StartUP_label44);
            this.RIT_groupBox4.Controls.Add(this.ritfreqtextBox1);
            this.RIT_groupBox4.Controls.Add(this.buttReset);
            this.RIT_groupBox4.Controls.Add(this.ritbutton1);
            this.RIT_groupBox4.Controls.Add(this.ritScroll);
            this.RIT_groupBox4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RIT_groupBox4.ForeColor = System.Drawing.Color.White;
            this.RIT_groupBox4.Location = new System.Drawing.Point(286, 130);
            this.RIT_groupBox4.Name = "RIT_groupBox4";
            this.RIT_groupBox4.Size = new System.Drawing.Size(221, 61);
            this.RIT_groupBox4.TabIndex = 176;
            this.RIT_groupBox4.TabStop = false;
            this.RIT_groupBox4.Text = "RIT";
            // 
            // StartUP_label44
            // 
            this.StartUP_label44.BackColor = System.Drawing.Color.Black;
            this.StartUP_label44.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartUP_label44.ForeColor = System.Drawing.Color.LightGreen;
            this.StartUP_label44.Location = new System.Drawing.Point(-22, 2);
            this.StartUP_label44.Name = "StartUP_label44";
            this.StartUP_label44.Size = new System.Drawing.Size(264, 59);
            this.StartUP_label44.TabIndex = 141;
            this.StartUP_label44.Text = "MSCC INITIALIZING\r\nPLEASE WAIT";
            this.StartUP_label44.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.StartUP_label44.Click += new System.EventHandler(this.StartUP_label44_Click);
            // 
            // ritfreqtextBox1
            // 
            this.ritfreqtextBox1.BackColor = System.Drawing.Color.Black;
            this.ritfreqtextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ritfreqtextBox1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ritfreqtextBox1.ForeColor = System.Drawing.Color.White;
            this.ritfreqtextBox1.Location = new System.Drawing.Point(72, 18);
            this.ritfreqtextBox1.Name = "ritfreqtextBox1";
            this.ritfreqtextBox1.ReadOnly = true;
            this.ritfreqtextBox1.Size = new System.Drawing.Size(77, 16);
            this.ritfreqtextBox1.TabIndex = 156;
            this.ritfreqtextBox1.Text = "00000000";
            this.ritfreqtextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip.SetToolTip(this.ritfreqtextBox1, "RIT Offset Frequency");
            this.ritfreqtextBox1.TextChanged += new System.EventHandler(this.ritfreqtextBox1_TextChanged_1);
            // 
            // buttReset
            // 
            this.buttReset.BackColor = System.Drawing.Color.Gainsboro;
            this.buttReset.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttReset.ForeColor = System.Drawing.Color.Black;
            this.buttReset.Location = new System.Drawing.Point(6, 15);
            this.buttReset.Name = "buttReset";
            this.buttReset.Size = new System.Drawing.Size(53, 23);
            this.buttReset.TabIndex = 2;
            this.buttReset.Text = "RESET";
            this.buttReset.UseVisualStyleBackColor = false;
            this.buttReset.Click += new System.EventHandler(this.buttReset_Click);
            // 
            // ritbutton1
            // 
            this.ritbutton1.BackColor = System.Drawing.Color.Gainsboro;
            this.ritbutton1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ritbutton1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ritbutton1.ForeColor = System.Drawing.Color.Black;
            this.ritbutton1.Location = new System.Drawing.Point(162, 15);
            this.ritbutton1.Name = "ritbutton1";
            this.ritbutton1.Size = new System.Drawing.Size(53, 23);
            this.ritbutton1.TabIndex = 33;
            this.ritbutton1.Text = "RIT";
            this.ritbutton1.UseVisualStyleBackColor = false;
            this.ritbutton1.Click += new System.EventHandler(this.ritbutton1_Click);
            // 
            // ritScroll
            // 
            this.ritScroll.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.ritScroll.Location = new System.Drawing.Point(6, 41);
            this.ritScroll.Maximum = 1009;
            this.ritScroll.Minimum = -1000;
            this.ritScroll.Name = "ritScroll";
            this.ritScroll.Size = new System.Drawing.Size(209, 15);
            this.ritScroll.TabIndex = 0;
            this.toolTip.SetToolTip(this.ritScroll, "Scroll to change RIT Offset");
            this.ritScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ritScroll_Scroll);
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.Gainsboro;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.ForeColor = System.Drawing.Color.Black;
            this.button6.Location = new System.Drawing.Point(169, 8);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(43, 22);
            this.button6.TabIndex = 175;
            this.button6.Text = "VFO";
            this.toolTip.SetToolTip(this.button6, "Switch Between VFO A and VFO B\r\n(Not yet implemented)");
            this.button6.UseVisualStyleBackColor = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.Gainsboro;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.Color.Black;
            this.button4.Location = new System.Drawing.Point(726, 58);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(43, 22);
            this.button4.TabIndex = 167;
            this.button4.Text = "EXT";
            this.toolTip.SetToolTip(this.button4, "Stop MSCC");
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // ACG_button
            // 
            this.ACG_button.BackColor = System.Drawing.Color.Gainsboro;
            this.ACG_button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ACG_button.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ACG_button.ForeColor = System.Drawing.Color.Black;
            this.ACG_button.Location = new System.Drawing.Point(679, 33);
            this.ACG_button.Name = "ACG_button";
            this.ACG_button.Size = new System.Drawing.Size(43, 22);
            this.ACG_button.TabIndex = 164;
            this.ACG_button.Text = "SLO";
            this.toolTip.SetToolTip(this.ACG_button, "AGC\r\nSLO - SLOW\r\nMED - MEDIUM\r\nFST - FAST");
            this.ACG_button.UseVisualStyleBackColor = false;
            this.ACG_button.Click += new System.EventHandler(this.ACG_button_Click);
            // 
            // Compression_button4
            // 
            this.Compression_button4.BackColor = System.Drawing.Color.Gainsboro;
            this.Compression_button4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Compression_button4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Compression_button4.ForeColor = System.Drawing.Color.Black;
            this.Compression_button4.Location = new System.Drawing.Point(726, 33);
            this.Compression_button4.Name = "Compression_button4";
            this.Compression_button4.Size = new System.Drawing.Size(43, 22);
            this.Compression_button4.TabIndex = 163;
            this.Compression_button4.Text = "CMP";
            this.toolTip.SetToolTip(this.Compression_button4, "Audio Compression");
            this.Compression_button4.UseVisualStyleBackColor = false;
            this.Compression_button4.Click += new System.EventHandler(this.Compression_button4_Click);
            // 
            // Auto_Zero_checkBox2
            // 
            this.Auto_Zero_checkBox2.BackColor = System.Drawing.Color.Black;
            this.Auto_Zero_checkBox2.Checked = true;
            this.Auto_Zero_checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Auto_Zero_checkBox2.Location = new System.Drawing.Point(637, 12);
            this.Auto_Zero_checkBox2.Name = "Auto_Zero_checkBox2";
            this.Auto_Zero_checkBox2.Size = new System.Drawing.Size(20, 20);
            this.Auto_Zero_checkBox2.TabIndex = 150;
            this.toolTip.SetToolTip(this.Auto_Zero_checkBox2, "Auto Zero");
            this.Auto_Zero_checkBox2.UseVisualStyleBackColor = false;
            this.Auto_Zero_checkBox2.CheckedChanged += new System.EventHandler(this.Auto_Zero_checkBox2_CheckedChanged);
            // 
            // Filter_listBox1
            // 
            this.Filter_listBox1.BackColor = System.Drawing.Color.White;
            this.Filter_listBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Filter_listBox1.ForeColor = System.Drawing.Color.Black;
            this.Filter_listBox1.FormattingEnabled = true;
            this.Filter_listBox1.Items.AddRange(new object[] {
            "5.5KHz",
            "4.0KHz",
            "3.0KHz",
            "2.7KHz",
            "2.4KHz"});
            this.Filter_listBox1.Location = new System.Drawing.Point(425, 84);
            this.Filter_listBox1.Name = "Filter_listBox1";
            this.Filter_listBox1.ScrollAlwaysVisible = true;
            this.Filter_listBox1.Size = new System.Drawing.Size(70, 17);
            this.Filter_listBox1.TabIndex = 147;
            this.toolTip.SetToolTip(this.Filter_listBox1, "Sets the High Side of the Bandwidth filter.\r\nNo effect in CW mode.");
            this.Filter_listBox1.SelectedIndexChanged += new System.EventHandler(this.Filter_listBox1_SelectedIndexChanged_1);
            // 
            // CW_Filter_listBox1
            // 
            this.CW_Filter_listBox1.BackColor = System.Drawing.Color.White;
            this.CW_Filter_listBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CW_Filter_listBox1.ForeColor = System.Drawing.Color.Black;
            this.CW_Filter_listBox1.FormattingEnabled = true;
            this.CW_Filter_listBox1.Items.AddRange(new object[] {
            "1.8KHz",
            "400Hz",
            "200Hz"});
            this.CW_Filter_listBox1.Location = new System.Drawing.Point(275, 84);
            this.CW_Filter_listBox1.Name = "CW_Filter_listBox1";
            this.CW_Filter_listBox1.Size = new System.Drawing.Size(70, 17);
            this.CW_Filter_listBox1.TabIndex = 146;
            this.toolTip.SetToolTip(this.CW_Filter_listBox1, "CW Bandwidth\r\n200Hz, 400Hz 1.8KHz\r\n");
            this.CW_Filter_listBox1.SelectedIndexChanged += new System.EventHandler(this.CW_Filter_listBox1_SelectedIndexChanged);
            // 
            // Filter_Low_listBox1
            // 
            this.Filter_Low_listBox1.BackColor = System.Drawing.Color.White;
            this.Filter_Low_listBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Filter_Low_listBox1.ForeColor = System.Drawing.Color.Black;
            this.Filter_Low_listBox1.FormattingEnabled = true;
            this.Filter_Low_listBox1.Items.AddRange(new object[] {
            "500Hz",
            "300Hz",
            "200Hz",
            "100Hz",
            "75Hz"});
            this.Filter_Low_listBox1.Location = new System.Drawing.Point(350, 84);
            this.Filter_Low_listBox1.Name = "Filter_Low_listBox1";
            this.Filter_Low_listBox1.Size = new System.Drawing.Size(70, 17);
            this.Filter_Low_listBox1.TabIndex = 145;
            this.toolTip.SetToolTip(this.Filter_Low_listBox1, "Sets the low side of the Filter Bandwidth. \r\nNo effect in CW mode\r\n");
            this.Filter_Low_listBox1.SelectedIndexChanged += new System.EventHandler(this.Filter_Low_listBox1_SelectedIndexChanged_1);
            // 
            // Local_Date_label46
            // 
            this.Local_Date_label46.BackColor = System.Drawing.Color.Transparent;
            this.Local_Date_label46.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Local_Date_label46.ForeColor = System.Drawing.Color.White;
            this.Local_Date_label46.Location = new System.Drawing.Point(3, 118);
            this.Local_Date_label46.Name = "Local_Date_label46";
            this.Local_Date_label46.Size = new System.Drawing.Size(70, 23);
            this.Local_Date_label46.TabIndex = 142;
            this.Local_Date_label46.Text = "12.12.20";
            this.Local_Date_label46.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Local_Date_label46.Visible = false;
            this.Local_Date_label46.Click += new System.EventHandler(this.Local_Date_label46_Click);
            // 
            // NB_button2
            // 
            this.NB_button2.BackColor = System.Drawing.Color.Gainsboro;
            this.NB_button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.NB_button2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NB_button2.ForeColor = System.Drawing.Color.Black;
            this.NB_button2.Location = new System.Drawing.Point(40, 157);
            this.NB_button2.Name = "NB_button2";
            this.NB_button2.Size = new System.Drawing.Size(23, 21);
            this.NB_button2.TabIndex = 137;
            this.NB_button2.Text = "B";
            this.toolTip.SetToolTip(this.NB_button2, "Noise Blanker");
            this.NB_button2.UseVisualStyleBackColor = false;
            this.NB_button2.Click += new System.EventHandler(this.NB_button2_Click);
            // 
            // Time_display_label33
            // 
            this.Time_display_label33.BackColor = System.Drawing.Color.Transparent;
            this.Time_display_label33.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Time_display_label33.ForeColor = System.Drawing.Color.White;
            this.Time_display_label33.Location = new System.Drawing.Point(3, 95);
            this.Time_display_label33.Name = "Time_display_label33";
            this.Time_display_label33.Size = new System.Drawing.Size(70, 23);
            this.Time_display_label33.TabIndex = 113;
            this.Time_display_label33.Text = "00:00:00";
            this.Time_display_label33.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Time_display_label33.Visible = false;
            this.Time_display_label33.Click += new System.EventHandler(this.Time_display_label33_Click);
            // 
            // TX_Mute_button2
            // 
            this.TX_Mute_button2.BackColor = System.Drawing.Color.Gainsboro;
            this.TX_Mute_button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.TX_Mute_button2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TX_Mute_button2.ForeColor = System.Drawing.Color.Black;
            this.TX_Mute_button2.Location = new System.Drawing.Point(644, 157);
            this.TX_Mute_button2.Name = "TX_Mute_button2";
            this.TX_Mute_button2.Size = new System.Drawing.Size(73, 21);
            this.TX_Mute_button2.TabIndex = 106;
            this.TX_Mute_button2.Text = "Mic Gain";
            this.TX_Mute_button2.UseVisualStyleBackColor = false;
            this.TX_Mute_button2.Click += new System.EventHandler(this.TX_Mute_button2_Click);
            // 
            // Volume_Mute_button2
            // 
            this.Volume_Mute_button2.BackColor = System.Drawing.Color.Gainsboro;
            this.Volume_Mute_button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Volume_Mute_button2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Volume_Mute_button2.ForeColor = System.Drawing.Color.Black;
            this.Volume_Mute_button2.Location = new System.Drawing.Point(82, 157);
            this.Volume_Mute_button2.Name = "Volume_Mute_button2";
            this.Volume_Mute_button2.Size = new System.Drawing.Size(65, 21);
            this.Volume_Mute_button2.TabIndex = 105;
            this.Volume_Mute_button2.Text = "Volume";
            this.Volume_Mute_button2.UseVisualStyleBackColor = false;
            this.Volume_Mute_button2.Click += new System.EventHandler(this.Volume_Mute_button2_Click);
            // 
            // MicVolume_hScrollBar1
            // 
            this.MicVolume_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.MicVolume_hScrollBar1.Location = new System.Drawing.Point(588, 181);
            this.MicVolume_hScrollBar1.Maximum = 109;
            this.MicVolume_hScrollBar1.Name = "MicVolume_hScrollBar1";
            this.MicVolume_hScrollBar1.Size = new System.Drawing.Size(185, 10);
            this.MicVolume_hScrollBar1.TabIndex = 102;
            this.MicVolume_hScrollBar1.TabStop = true;
            this.MicVolume_hScrollBar1.Value = 30;
            this.MicVolume_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.MicVolume_hScrollBar1_Scroll);
            // 
            // Volume_hScrollBar1
            // 
            this.Volume_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.Volume_hScrollBar1.Location = new System.Drawing.Point(20, 181);
            this.Volume_hScrollBar1.Maximum = 108;
            this.Volume_hScrollBar1.Name = "Volume_hScrollBar1";
            this.Volume_hScrollBar1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Volume_hScrollBar1.Size = new System.Drawing.Size(185, 10);
            this.Volume_hScrollBar1.TabIndex = 101;
            this.Volume_hScrollBar1.TabStop = true;
            this.Volume_hScrollBar1.Value = 30;
            this.Volume_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Volume_hScrollBar1_Scroll);
            // 
            // Freqbutton3
            // 
            this.Freqbutton3.BackColor = System.Drawing.Color.Gainsboro;
            this.Freqbutton3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Freqbutton3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Freqbutton3.ForeColor = System.Drawing.Color.Black;
            this.Freqbutton3.Location = new System.Drawing.Point(166, 157);
            this.Freqbutton3.Name = "Freqbutton3";
            this.Freqbutton3.Size = new System.Drawing.Size(23, 21);
            this.Freqbutton3.TabIndex = 66;
            this.Freqbutton3.Text = "A";
            this.toolTip.SetToolTip(this.Freqbutton3, "Auto Notch");
            this.Freqbutton3.UseVisualStyleBackColor = false;
            this.Freqbutton3.Click += new System.EventHandler(this.Freqbutton3_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Gainsboro;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(679, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(43, 22);
            this.button1.TabIndex = 64;
            this.button1.Text = "PTT";
            this.toolTip.SetToolTip(this.button1, "Turns on TX\r\nPTT is ONLY permitted when the \r\nDial Frequency is within a legal Am" +
        "ateur Radio Band");
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // genradioButton
            // 
            this.genradioButton.AutoSize = true;
            this.genradioButton.BackColor = System.Drawing.Color.Transparent;
            this.genradioButton.ForeColor = System.Drawing.Color.White;
            this.genradioButton.Location = new System.Drawing.Point(662, 109);
            this.genradioButton.Name = "genradioButton";
            this.genradioButton.Size = new System.Drawing.Size(52, 20);
            this.genradioButton.TabIndex = 56;
            this.genradioButton.Text = "GEN";
            this.genradioButton.UseVisualStyleBackColor = false;
            this.genradioButton.CheckedChanged += new System.EventHandler(this.genradioButton_CheckedChanged);
            // 
            // mainmodebutton2
            // 
            this.mainmodebutton2.BackColor = System.Drawing.Color.Gainsboro;
            this.mainmodebutton2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.mainmodebutton2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainmodebutton2.ForeColor = System.Drawing.Color.Black;
            this.mainmodebutton2.Location = new System.Drawing.Point(169, 33);
            this.mainmodebutton2.Name = "mainmodebutton2";
            this.mainmodebutton2.Size = new System.Drawing.Size(43, 22);
            this.mainmodebutton2.TabIndex = 53;
            this.mainmodebutton2.Text = "USB";
            this.mainmodebutton2.UseVisualStyleBackColor = false;
            this.mainmodebutton2.Click += new System.EventHandler(this.mainmodebutton2_Click);
            // 
            // main10radioButton1
            // 
            this.main10radioButton1.AutoSize = true;
            this.main10radioButton1.BackColor = System.Drawing.Color.Transparent;
            this.main10radioButton1.ForeColor = System.Drawing.Color.White;
            this.main10radioButton1.Location = new System.Drawing.Point(604, 109);
            this.main10radioButton1.Name = "main10radioButton1";
            this.main10radioButton1.Size = new System.Drawing.Size(53, 20);
            this.main10radioButton1.TabIndex = 50;
            this.main10radioButton1.Text = "10M";
            this.main10radioButton1.UseVisualStyleBackColor = false;
            this.main10radioButton1.CheckedChanged += new System.EventHandler(this.main10radioButton1_CheckedChanged);
            // 
            // main12radioButton2
            // 
            this.main12radioButton2.AutoSize = true;
            this.main12radioButton2.BackColor = System.Drawing.Color.Transparent;
            this.main12radioButton2.ForeColor = System.Drawing.Color.White;
            this.main12radioButton2.Location = new System.Drawing.Point(546, 109);
            this.main12radioButton2.Name = "main12radioButton2";
            this.main12radioButton2.Size = new System.Drawing.Size(53, 20);
            this.main12radioButton2.TabIndex = 49;
            this.main12radioButton2.Text = "12M";
            this.main12radioButton2.UseVisualStyleBackColor = false;
            this.main12radioButton2.CheckedChanged += new System.EventHandler(this.main12radioButton2_CheckedChanged);
            // 
            // main15radiobutton
            // 
            this.main15radiobutton.AutoSize = true;
            this.main15radiobutton.BackColor = System.Drawing.Color.Transparent;
            this.main15radiobutton.ForeColor = System.Drawing.Color.White;
            this.main15radiobutton.Location = new System.Drawing.Point(488, 109);
            this.main15radiobutton.Name = "main15radiobutton";
            this.main15radiobutton.Size = new System.Drawing.Size(53, 20);
            this.main15radiobutton.TabIndex = 48;
            this.main15radiobutton.Text = "15M";
            this.main15radiobutton.UseVisualStyleBackColor = false;
            this.main15radiobutton.CheckedChanged += new System.EventHandler(this.main15radiobutton_CheckedChanged);
            // 
            // main17radioButton4
            // 
            this.main17radioButton4.AutoSize = true;
            this.main17radioButton4.BackColor = System.Drawing.Color.Transparent;
            this.main17radioButton4.ForeColor = System.Drawing.Color.White;
            this.main17radioButton4.Location = new System.Drawing.Point(430, 109);
            this.main17radioButton4.Name = "main17radioButton4";
            this.main17radioButton4.Size = new System.Drawing.Size(53, 20);
            this.main17radioButton4.TabIndex = 47;
            this.main17radioButton4.Text = "17M";
            this.main17radioButton4.UseVisualStyleBackColor = false;
            this.main17radioButton4.CheckedChanged += new System.EventHandler(this.main17radioButton4_CheckedChanged);
            // 
            // main20radioButton5
            // 
            this.main20radioButton5.AutoSize = true;
            this.main20radioButton5.BackColor = System.Drawing.Color.Transparent;
            this.main20radioButton5.ForeColor = System.Drawing.Color.White;
            this.main20radioButton5.Location = new System.Drawing.Point(372, 109);
            this.main20radioButton5.Name = "main20radioButton5";
            this.main20radioButton5.Size = new System.Drawing.Size(53, 20);
            this.main20radioButton5.TabIndex = 46;
            this.main20radioButton5.Text = "20M";
            this.main20radioButton5.UseVisualStyleBackColor = false;
            this.main20radioButton5.CheckedChanged += new System.EventHandler(this.main20radioButton5_CheckedChanged);
            // 
            // main30radioButton6
            // 
            this.main30radioButton6.AutoSize = true;
            this.main30radioButton6.BackColor = System.Drawing.Color.Transparent;
            this.main30radioButton6.ForeColor = System.Drawing.Color.White;
            this.main30radioButton6.Location = new System.Drawing.Point(314, 109);
            this.main30radioButton6.Name = "main30radioButton6";
            this.main30radioButton6.Size = new System.Drawing.Size(53, 20);
            this.main30radioButton6.TabIndex = 45;
            this.main30radioButton6.Text = "30M";
            this.main30radioButton6.UseVisualStyleBackColor = false;
            this.main30radioButton6.CheckedChanged += new System.EventHandler(this.main30radioButton6_CheckedChanged);
            // 
            // main40radioButton7
            // 
            this.main40radioButton7.AutoSize = true;
            this.main40radioButton7.BackColor = System.Drawing.Color.Transparent;
            this.main40radioButton7.ForeColor = System.Drawing.Color.White;
            this.main40radioButton7.Location = new System.Drawing.Point(256, 109);
            this.main40radioButton7.Name = "main40radioButton7";
            this.main40radioButton7.Size = new System.Drawing.Size(53, 20);
            this.main40radioButton7.TabIndex = 44;
            this.main40radioButton7.Text = "40M";
            this.main40radioButton7.UseVisualStyleBackColor = false;
            this.main40radioButton7.CheckedChanged += new System.EventHandler(this.main40radioButton7_CheckedChanged);
            // 
            // main60radioButton8
            // 
            this.main60radioButton8.AutoSize = true;
            this.main60radioButton8.BackColor = System.Drawing.Color.Transparent;
            this.main60radioButton8.ForeColor = System.Drawing.Color.White;
            this.main60radioButton8.Location = new System.Drawing.Point(198, 109);
            this.main60radioButton8.Name = "main60radioButton8";
            this.main60radioButton8.Size = new System.Drawing.Size(53, 20);
            this.main60radioButton8.TabIndex = 43;
            this.main60radioButton8.Text = "60M";
            this.main60radioButton8.UseVisualStyleBackColor = false;
            this.main60radioButton8.CheckedChanged += new System.EventHandler(this.main60radioButton8_CheckedChanged);
            // 
            // main80radioButton9
            // 
            this.main80radioButton9.BackColor = System.Drawing.Color.Transparent;
            this.main80radioButton9.ForeColor = System.Drawing.Color.White;
            this.main80radioButton9.Location = new System.Drawing.Point(143, 109);
            this.main80radioButton9.Margin = new System.Windows.Forms.Padding(0, 3, 3, 5);
            this.main80radioButton9.Name = "main80radioButton9";
            this.main80radioButton9.Size = new System.Drawing.Size(51, 20);
            this.main80radioButton9.TabIndex = 42;
            this.main80radioButton9.Text = "80M";
            this.main80radioButton9.UseVisualStyleBackColor = false;
            this.main80radioButton9.CheckedChanged += new System.EventHandler(this.main80radioButton9_CheckedChanged);
            // 
            // main160radioButton10
            // 
            this.main160radioButton10.AutoSize = true;
            this.main160radioButton10.BackColor = System.Drawing.Color.Transparent;
            this.main160radioButton10.ForeColor = System.Drawing.Color.White;
            this.main160radioButton10.Location = new System.Drawing.Point(79, 109);
            this.main160radioButton10.Margin = new System.Windows.Forms.Padding(3, 3, 0, 5);
            this.main160radioButton10.Name = "main160radioButton10";
            this.main160radioButton10.Size = new System.Drawing.Size(61, 20);
            this.main160radioButton10.TabIndex = 41;
            this.main160radioButton10.Text = "160M";
            this.main160radioButton10.UseVisualStyleBackColor = false;
            this.main160radioButton10.CheckedChanged += new System.EventHandler(this.main160radioButton10_CheckedChanged);
            // 
            // buttTune
            // 
            this.buttTune.BackColor = System.Drawing.Color.Gainsboro;
            this.buttTune.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttTune.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttTune.ForeColor = System.Drawing.Color.Black;
            this.buttTune.Location = new System.Drawing.Point(726, 8);
            this.buttTune.Name = "buttTune";
            this.buttTune.Size = new System.Drawing.Size(43, 22);
            this.buttTune.TabIndex = 4;
            this.buttTune.Text = "TUN";
            this.toolTip.SetToolTip(this.buttTune, "Sets Rig to Tune Power and Turns on TX\r\nThe TUNING is ONLY permitted when the \r\nD" +
        "ial Frequency is within a legal Amateur Radio Band.");
            this.buttTune.UseVisualStyleBackColor = false;
            this.buttTune.Click += new System.EventHandler(this.buttTune_Click);
            // 
            // Band_Change_Auto_Tune_checkBox2
            // 
            this.Band_Change_Auto_Tune_checkBox2.BackColor = System.Drawing.Color.Black;
            this.Band_Change_Auto_Tune_checkBox2.ForeColor = System.Drawing.Color.Black;
            this.Band_Change_Auto_Tune_checkBox2.Location = new System.Drawing.Point(637, 29);
            this.Band_Change_Auto_Tune_checkBox2.Name = "Band_Change_Auto_Tune_checkBox2";
            this.Band_Change_Auto_Tune_checkBox2.Size = new System.Drawing.Size(20, 20);
            this.Band_Change_Auto_Tune_checkBox2.TabIndex = 151;
            this.toolTip.SetToolTip(this.Band_Change_Auto_Tune_checkBox2, "Auto Tune on Band Change");
            this.Band_Change_Auto_Tune_checkBox2.UseVisualStyleBackColor = false;
            this.Band_Change_Auto_Tune_checkBox2.CheckedChanged += new System.EventHandler(this.Band_Change_Auto_Tune_checkBox2_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.ForeColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(71, 107);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(651, 25);
            this.panel2.TabIndex = 183;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.Black;
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(54, 74);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 17);
            this.label9.TabIndex = 171;
            this.label9.Text = "dBm";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label9.Click += new System.EventHandler(this.label9_Click);
            // 
            // Power_Value_label2
            // 
            this.Power_Value_label2.BackColor = System.Drawing.Color.Black;
            this.Power_Value_label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Power_Value_label2.ForeColor = System.Drawing.Color.White;
            this.Power_Value_label2.Location = new System.Drawing.Point(89, 74);
            this.Power_Value_label2.Name = "Power_Value_label2";
            this.Power_Value_label2.Size = new System.Drawing.Size(34, 17);
            this.Power_Value_label2.TabIndex = 172;
            this.Power_Value_label2.Text = "-100";
            this.Power_Value_label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Power_Value_label2.Click += new System.EventHandler(this.Power_Value_label2_Click);
            // 
            // vuMeter1
            // 
            this.vuMeter1.AnalogMeter = true;
            this.vuMeter1.BackColor = System.Drawing.Color.Black;
            this.vuMeter1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.vuMeter1.DialBackground = System.Drawing.Color.Moccasin;
            this.vuMeter1.DialTextNegative = System.Drawing.Color.Red;
            this.vuMeter1.DialTextPositive = System.Drawing.Color.Black;
            this.vuMeter1.DialTextZero = System.Drawing.Color.DarkGreen;
            this.vuMeter1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vuMeter1.Led1ColorOff = System.Drawing.Color.Black;
            this.vuMeter1.Led1ColorOn = System.Drawing.Color.LimeGreen;
            this.vuMeter1.Led1Count = 9;
            this.vuMeter1.Led2ColorOff = System.Drawing.Color.Black;
            this.vuMeter1.Led2ColorOn = System.Drawing.Color.Yellow;
            this.vuMeter1.Led2Count = 0;
            this.vuMeter1.Led3ColorOff = System.Drawing.Color.Black;
            this.vuMeter1.Led3ColorOn = System.Drawing.Color.Red;
            this.vuMeter1.Led3Count = 5;
            this.vuMeter1.LedSize = new System.Drawing.Size(3, 14);
            this.vuMeter1.LedSpace = 0;
            this.vuMeter1.Level = 0;
            this.vuMeter1.LevelMax = 150;
            this.vuMeter1.Location = new System.Drawing.Point(3, 2);
            this.vuMeter1.MeterScale = VU_MeterLibrary.MeterScale.Analog;
            this.vuMeter1.Name = "vuMeter1";
            this.vuMeter1.NeedleColor = System.Drawing.Color.Black;
            this.vuMeter1.PeakHold = false;
            this.vuMeter1.Peakms = 1000;
            this.vuMeter1.PeakNeedleColor = System.Drawing.Color.White;
            this.vuMeter1.ShowDialOnly = false;
            this.vuMeter1.ShowLedPeak = false;
            this.vuMeter1.ShowTextInDial = true;
            this.vuMeter1.Size = new System.Drawing.Size(160, 128);
            this.vuMeter1.TabIndex = 168;
            this.vuMeter1.TextInDial = new string[] {
        "[S]",
        "1",
        "3",
        "5",
        "7",
        "9",
        "+20",
        "+40",
        "+60"};
            this.vuMeter1.UseLedLight = true;
            this.vuMeter1.VerticalBar = false;
            this.vuMeter1.VuText = "S Meter";
            this.vuMeter1.Load += new System.EventHandler(this.vuMeter1_Load);
            // 
            // picWaterfall
            // 
            this.picWaterfall.BackColor = System.Drawing.Color.Transparent;
            this.picWaterfall.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picWaterfall.Cursor = System.Windows.Forms.Cursors.Cross;
            this.picWaterfall.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.picWaterfall.Location = new System.Drawing.Point(0, 340);
            this.picWaterfall.Margin = new System.Windows.Forms.Padding(0);
            this.picWaterfall.Name = "picWaterfall";
            this.picWaterfall.Size = new System.Drawing.Size(792, 111);
            this.picWaterfall.TabIndex = 178;
            this.picWaterfall.TabStop = false;
            this.picWaterfall.Visible = false;
            this.picWaterfall.Click += new System.EventHandler(this.picWaterfall_Click_1);
            this.picWaterfall.Paint += new System.Windows.Forms.PaintEventHandler(this.picWaterfall_paint);
            // 
            // TX
            // 
            this.TX.BackColor = System.Drawing.Color.Black;
            this.TX.Controls.Add(this.Meter_hold_label43);
            this.TX.Controls.Add(this.Power_Meter_Hold);
            this.TX.Controls.Add(this.Reverse_Power_label43);
            this.TX.Controls.Add(this.Forward_Power_label43);
            this.TX.Controls.Add(this.SWR_Value_label43);
            this.TX.Controls.Add(this.Reverse_label58);
            this.TX.Controls.Add(this.Forward_label43);
            this.TX.Controls.Add(this.Reverse_Meter);
            this.TX.Controls.Add(this.SWR_label1);
            this.TX.Controls.Add(this.Forward_Meter);
            this.TX.Controls.Add(this.Full_Power_checkBox1);
            this.TX.Controls.Add(this.NR_label5);
            this.TX.Controls.Add(this.NR_Button);
            this.TX.Controls.Add(this.AGC_label57);
            this.TX.Controls.Add(this.label56);
            this.TX.Controls.Add(this.label55);
            this.TX.Controls.Add(this.AGC_hScrollBar1);
            this.TX.Controls.Add(this.label53);
            this.TX.Controls.Add(this.Default_High_Cut_listBox1);
            this.TX.Controls.Add(this.label52);
            this.TX.Controls.Add(this.Default_CW_Filter_listBox1);
            this.TX.Controls.Add(this.label51);
            this.TX.Controls.Add(this.Default_Low_Cut_listBox1);
            this.TX.Controls.Add(this.label50);
            this.TX.Controls.Add(this.Tune_vButton2);
            this.TX.Controls.Add(this.PA_vButton1);
            this.TX.Controls.Add(this.NB_Threshold_label1);
            this.TX.Controls.Add(this.NB_label16);
            this.TX.Controls.Add(this.NB_ON_OFF_button2);
            this.TX.Controls.Add(this.NB_Threshold_label16);
            this.TX.Controls.Add(this.NB_Threshold_hScrollBar1);
            this.TX.Controls.Add(this.NB_Width_label16);
            this.TX.Controls.Add(this.NB_hScrollBar1);
            this.TX.Controls.Add(this.NR_hScrollBar1);
            this.TX.Controls.Add(this.NR_label2);
            this.TX.Controls.Add(this.AGC_listBox1);
            this.TX.Controls.Add(this.AGC_label2);
            this.TX.Controls.Add(this.Tune_Power_label37);
            this.TX.Controls.Add(this.Tune_Power_hScrollBar1);
            this.TX.Controls.Add(this.label36);
            this.TX.Controls.Add(this.CW_Power_label36);
            this.TX.Controls.Add(this.SSB_Power_label36);
            this.TX.Controls.Add(this.AM_Carrier_label36);
            this.TX.Controls.Add(this.CW_Power_hScrollBar1);
            this.TX.Controls.Add(this.label34);
            this.TX.Controls.Add(this.Power_hScrollBar1);
            this.TX.Controls.Add(this.Power_label34);
            this.TX.Controls.Add(this.TX_Bandwidth_listBox1);
            this.TX.Controls.Add(this.AM_Carrier_label2);
            this.TX.Controls.Add(this.AM_Carrier_hScrollBar1);
            this.TX.Controls.Add(this.label14);
            this.TX.ForeColor = System.Drawing.Color.Black;
            this.TX.Location = new System.Drawing.Point(4, 25);
            this.TX.Margin = new System.Windows.Forms.Padding(0);
            this.TX.Name = "TX";
            this.TX.Size = new System.Drawing.Size(792, 451);
            this.TX.TabIndex = 11;
            this.TX.Text = "Rx/Tx";
            this.TX.Click += new System.EventHandler(this.TX_Click);
            // 
            // Meter_hold_label43
            // 
            this.Meter_hold_label43.AutoSize = true;
            this.Meter_hold_label43.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Meter_hold_label43.ForeColor = System.Drawing.Color.White;
            this.Meter_hold_label43.Location = new System.Drawing.Point(542, 220);
            this.Meter_hold_label43.Name = "Meter_hold_label43";
            this.Meter_hold_label43.Size = new System.Drawing.Size(123, 16);
            this.Meter_hold_label43.TabIndex = 192;
            this.Meter_hold_label43.Text = "SWR Meter Hold";
            this.Meter_hold_label43.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Power_Meter_Hold
            // 
            this.Power_Meter_Hold.BackColor = System.Drawing.Color.White;
            this.Power_Meter_Hold.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Power_Meter_Hold.ForeColor = System.Drawing.Color.Black;
            this.Power_Meter_Hold.Location = new System.Drawing.Point(689, 218);
            this.Power_Meter_Hold.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.Power_Meter_Hold.Name = "Power_Meter_Hold";
            this.Power_Meter_Hold.Size = new System.Drawing.Size(42, 22);
            this.Power_Meter_Hold.TabIndex = 191;
            this.toolTip.SetToolTip(this.Power_Meter_Hold, "Power Meter Hold Time.\r\nSet to zero (0) for no hold time.");
            this.Power_Meter_Hold.ValueChanged += new System.EventHandler(this.Power_Meter_Hold_ValueChanged_1);
            // 
            // Reverse_Power_label43
            // 
            this.Reverse_Power_label43.BackColor = System.Drawing.Color.Gainsboro;
            this.Reverse_Power_label43.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Reverse_Power_label43.ForeColor = System.Drawing.Color.Black;
            this.Reverse_Power_label43.Location = new System.Drawing.Point(632, 148);
            this.Reverse_Power_label43.Name = "Reverse_Power_label43";
            this.Reverse_Power_label43.Size = new System.Drawing.Size(63, 17);
            this.Reverse_Power_label43.TabIndex = 190;
            this.Reverse_Power_label43.Text = "0";
            this.Reverse_Power_label43.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Reverse_Power_label43.Click += new System.EventHandler(this.Reverse_Power_label43_Click_1);
            // 
            // Forward_Power_label43
            // 
            this.Forward_Power_label43.BackColor = System.Drawing.Color.Gainsboro;
            this.Forward_Power_label43.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Forward_Power_label43.ForeColor = System.Drawing.Color.Black;
            this.Forward_Power_label43.Location = new System.Drawing.Point(632, 107);
            this.Forward_Power_label43.Name = "Forward_Power_label43";
            this.Forward_Power_label43.Size = new System.Drawing.Size(63, 17);
            this.Forward_Power_label43.TabIndex = 189;
            this.Forward_Power_label43.Text = "0";
            this.Forward_Power_label43.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Forward_Power_label43.Click += new System.EventHandler(this.Forward_Power_label43_Click_1);
            // 
            // SWR_Value_label43
            // 
            this.SWR_Value_label43.BackColor = System.Drawing.Color.Gainsboro;
            this.SWR_Value_label43.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SWR_Value_label43.ForeColor = System.Drawing.Color.Black;
            this.SWR_Value_label43.Location = new System.Drawing.Point(632, 189);
            this.SWR_Value_label43.Name = "SWR_Value_label43";
            this.SWR_Value_label43.Size = new System.Drawing.Size(52, 17);
            this.SWR_Value_label43.TabIndex = 188;
            this.SWR_Value_label43.Text = "1.0";
            this.SWR_Value_label43.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.SWR_Value_label43.Click += new System.EventHandler(this.SWR_Value_label43_Click_1);
            // 
            // Reverse_label58
            // 
            this.Reverse_label58.BackColor = System.Drawing.Color.Gainsboro;
            this.Reverse_label58.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Reverse_label58.ForeColor = System.Drawing.Color.Black;
            this.Reverse_label58.Location = new System.Drawing.Point(603, 148);
            this.Reverse_label58.Name = "Reverse_label58";
            this.Reverse_label58.Size = new System.Drawing.Size(23, 17);
            this.Reverse_label58.TabIndex = 187;
            this.Reverse_label58.Text = "R";
            this.Reverse_label58.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Forward_label43
            // 
            this.Forward_label43.BackColor = System.Drawing.Color.Gainsboro;
            this.Forward_label43.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Forward_label43.ForeColor = System.Drawing.Color.Black;
            this.Forward_label43.Location = new System.Drawing.Point(603, 107);
            this.Forward_label43.Name = "Forward_label43";
            this.Forward_label43.Size = new System.Drawing.Size(23, 17);
            this.Forward_label43.TabIndex = 186;
            this.Forward_label43.Text = "F";
            this.Forward_label43.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Reverse_Meter
            // 
            this.Reverse_Meter.AnalogMeter = false;
            this.Reverse_Meter.BackColor = System.Drawing.Color.Moccasin;
            this.Reverse_Meter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Reverse_Meter.DialBackground = System.Drawing.Color.Moccasin;
            this.Reverse_Meter.DialTextNegative = System.Drawing.Color.Red;
            this.Reverse_Meter.DialTextPositive = System.Drawing.Color.Black;
            this.Reverse_Meter.DialTextZero = System.Drawing.Color.DarkGreen;
            this.Reverse_Meter.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Reverse_Meter.Led1ColorOff = System.Drawing.Color.Snow;
            this.Reverse_Meter.Led1ColorOn = System.Drawing.Color.Green;
            this.Reverse_Meter.Led1Count = 12;
            this.Reverse_Meter.Led2ColorOff = System.Drawing.Color.Snow;
            this.Reverse_Meter.Led2ColorOn = System.Drawing.Color.Green;
            this.Reverse_Meter.Led2Count = 13;
            this.Reverse_Meter.Led3ColorOff = System.Drawing.Color.Snow;
            this.Reverse_Meter.Led3ColorOn = System.Drawing.Color.Red;
            this.Reverse_Meter.Led3Count = 2;
            this.Reverse_Meter.LedSize = new System.Drawing.Size(7, 12);
            this.Reverse_Meter.LedSpace = 1;
            this.Reverse_Meter.Level = 0;
            this.Reverse_Meter.LevelMax = 110;
            this.Reverse_Meter.Location = new System.Drawing.Point(550, 170);
            this.Reverse_Meter.MeterScale = VU_MeterLibrary.MeterScale.Analog;
            this.Reverse_Meter.Name = "Reverse_Meter";
            this.Reverse_Meter.NeedleColor = System.Drawing.Color.Black;
            this.Reverse_Meter.PeakHold = false;
            this.Reverse_Meter.Peakms = 1000;
            this.Reverse_Meter.PeakNeedleColor = System.Drawing.Color.Red;
            this.Reverse_Meter.ShowDialOnly = false;
            this.Reverse_Meter.ShowLedPeak = false;
            this.Reverse_Meter.ShowTextInDial = true;
            this.Reverse_Meter.Size = new System.Drawing.Size(217, 14);
            this.Reverse_Meter.TabIndex = 185;
            this.Reverse_Meter.TextInDial = new string[] {
        "[S]",
        "1",
        "3",
        "5",
        "7",
        "9",
        "+20",
        "+40",
        "+60"};
            this.Reverse_Meter.UseLedLight = true;
            this.Reverse_Meter.VerticalBar = false;
            this.Reverse_Meter.VuText = "ALC";
            this.Reverse_Meter.Load += new System.EventHandler(this.Reverse_Meter_Load_1);
            // 
            // SWR_label1
            // 
            this.SWR_label1.BackColor = System.Drawing.Color.Gainsboro;
            this.SWR_label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SWR_label1.ForeColor = System.Drawing.Color.Black;
            this.SWR_label1.Location = new System.Drawing.Point(574, 189);
            this.SWR_label1.Name = "SWR_label1";
            this.SWR_label1.Size = new System.Drawing.Size(52, 17);
            this.SWR_label1.TabIndex = 184;
            this.SWR_label1.Text = "SWR";
            this.SWR_label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Forward_Meter
            // 
            this.Forward_Meter.AnalogMeter = false;
            this.Forward_Meter.BackColor = System.Drawing.Color.Moccasin;
            this.Forward_Meter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Forward_Meter.DialBackground = System.Drawing.Color.Moccasin;
            this.Forward_Meter.DialTextNegative = System.Drawing.Color.Red;
            this.Forward_Meter.DialTextPositive = System.Drawing.Color.Black;
            this.Forward_Meter.DialTextZero = System.Drawing.Color.DarkGreen;
            this.Forward_Meter.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Forward_Meter.Led1ColorOff = System.Drawing.Color.Snow;
            this.Forward_Meter.Led1ColorOn = System.Drawing.Color.Green;
            this.Forward_Meter.Led1Count = 12;
            this.Forward_Meter.Led2ColorOff = System.Drawing.Color.Snow;
            this.Forward_Meter.Led2ColorOn = System.Drawing.Color.Green;
            this.Forward_Meter.Led2Count = 13;
            this.Forward_Meter.Led3ColorOff = System.Drawing.Color.Snow;
            this.Forward_Meter.Led3ColorOn = System.Drawing.Color.Red;
            this.Forward_Meter.Led3Count = 2;
            this.Forward_Meter.LedSize = new System.Drawing.Size(7, 12);
            this.Forward_Meter.LedSpace = 1;
            this.Forward_Meter.Level = 0;
            this.Forward_Meter.LevelMax = 110;
            this.Forward_Meter.Location = new System.Drawing.Point(550, 129);
            this.Forward_Meter.MeterScale = VU_MeterLibrary.MeterScale.Analog;
            this.Forward_Meter.Name = "Forward_Meter";
            this.Forward_Meter.NeedleColor = System.Drawing.Color.Black;
            this.Forward_Meter.PeakHold = false;
            this.Forward_Meter.Peakms = 2000;
            this.Forward_Meter.PeakNeedleColor = System.Drawing.Color.Red;
            this.Forward_Meter.ShowDialOnly = false;
            this.Forward_Meter.ShowLedPeak = false;
            this.Forward_Meter.ShowTextInDial = true;
            this.Forward_Meter.Size = new System.Drawing.Size(217, 14);
            this.Forward_Meter.TabIndex = 183;
            this.Forward_Meter.TextInDial = new string[] {
        "[S]",
        "1",
        "3",
        "5",
        "7",
        "9",
        "+20",
        "+40",
        "+60"};
            this.Forward_Meter.UseLedLight = true;
            this.Forward_Meter.VerticalBar = false;
            this.Forward_Meter.VuText = "ALC";
            this.Forward_Meter.Load += new System.EventHandler(this.Forward_Meter_Load_1);
            // 
            // Full_Power_checkBox1
            // 
            this.Full_Power_checkBox1.AutoSize = true;
            this.Full_Power_checkBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.Full_Power_checkBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Full_Power_checkBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Full_Power_checkBox1.ForeColor = System.Drawing.Color.Black;
            this.Full_Power_checkBox1.Location = new System.Drawing.Point(341, 187);
            this.Full_Power_checkBox1.MinimumSize = new System.Drawing.Size(110, 20);
            this.Full_Power_checkBox1.Name = "Full_Power_checkBox1";
            this.Full_Power_checkBox1.Size = new System.Drawing.Size(110, 20);
            this.Full_Power_checkBox1.TabIndex = 170;
            this.Full_Power_checkBox1.Text = "Power TUNE";
            this.toolTip.SetToolTip(this.Full_Power_checkBox1, "Toggle between Configured TUNE Output Level and\r\nFull Power TUNE.");
            this.Full_Power_checkBox1.UseVisualStyleBackColor = false;
            this.Full_Power_checkBox1.CheckedChanged += new System.EventHandler(this.Full_Power_checkBox1_CheckedChanged);
            // 
            // NR_label5
            // 
            this.NR_label5.BackColor = System.Drawing.Color.Gainsboro;
            this.NR_label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.NR_label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NR_label5.ForeColor = System.Drawing.Color.Black;
            this.NR_label5.Location = new System.Drawing.Point(356, 351);
            this.NR_label5.Name = "NR_label5";
            this.NR_label5.Size = new System.Drawing.Size(60, 18);
            this.NR_label5.TabIndex = 167;
            this.NR_label5.Text = "100%";
            this.NR_label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.NR_label5.Click += new System.EventHandler(this.NR_label5_Click);
            // 
            // NR_Button
            // 
            this.NR_Button.BackColor = System.Drawing.Color.Gainsboro;
            this.NR_Button.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NR_Button.ForeColor = System.Drawing.Color.Black;
            this.NR_Button.Location = new System.Drawing.Point(367, 290);
            this.NR_Button.Name = "NR_Button";
            this.NR_Button.Size = new System.Drawing.Size(45, 23);
            this.NR_Button.TabIndex = 166;
            this.NR_Button.Text = "OFF";
            this.toolTip.SetToolTip(this.NR_Button, "Noise Reduction - Not Implemented");
            this.NR_Button.UseVisualStyleBackColor = false;
            this.NR_Button.Click += new System.EventHandler(this.NR_Button_Click);
            // 
            // AGC_label57
            // 
            this.AGC_label57.BackColor = System.Drawing.Color.Gainsboro;
            this.AGC_label57.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.AGC_label57.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AGC_label57.ForeColor = System.Drawing.Color.Black;
            this.AGC_label57.Location = new System.Drawing.Point(619, 335);
            this.AGC_label57.Name = "AGC_label57";
            this.AGC_label57.Size = new System.Drawing.Size(67, 18);
            this.AGC_label57.TabIndex = 165;
            this.AGC_label57.Text = "1000 ms";
            this.AGC_label57.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.AGC_label57.Click += new System.EventHandler(this.AGC_label57_Click);
            // 
            // label56
            // 
            this.label56.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label56.ForeColor = System.Drawing.Color.White;
            this.label56.Location = new System.Drawing.Point(38, 226);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(40, 20);
            this.label56.TabIndex = 164;
            this.label56.Text = "NB";
            this.label56.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label55
            // 
            this.label55.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label55.ForeColor = System.Drawing.Color.White;
            this.label55.Location = new System.Drawing.Point(602, 284);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(106, 20);
            this.label55.TabIndex = 163;
            this.label55.Text = "AGC RELEASE";
            this.label55.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AGC_hScrollBar1
            // 
            this.AGC_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.AGC_hScrollBar1.LargeChange = 100;
            this.AGC_hScrollBar1.Location = new System.Drawing.Point(567, 312);
            this.AGC_hScrollBar1.Maximum = 1098;
            this.AGC_hScrollBar1.Minimum = 1;
            this.AGC_hScrollBar1.Name = "AGC_hScrollBar1";
            this.AGC_hScrollBar1.Size = new System.Drawing.Size(185, 15);
            this.AGC_hScrollBar1.SmallChange = 10;
            this.AGC_hScrollBar1.TabIndex = 162;
            this.toolTip.SetToolTip(this.AGC_hScrollBar1, "Set the AGC Release time in FAST mode only.");
            this.AGC_hScrollBar1.Value = 20;
            this.AGC_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.AGC_hScrollBar1_Scroll);
            // 
            // label53
            // 
            this.label53.BackColor = System.Drawing.Color.Transparent;
            this.label53.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label53.ForeColor = System.Drawing.Color.White;
            this.label53.Location = new System.Drawing.Point(174, 149);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(76, 21);
            this.label53.TabIndex = 161;
            this.label53.Text = "HIGH CUT";
            this.label53.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Default_High_Cut_listBox1
            // 
            this.Default_High_Cut_listBox1.BackColor = System.Drawing.Color.White;
            this.Default_High_Cut_listBox1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Default_High_Cut_listBox1.ForeColor = System.Drawing.Color.Black;
            this.Default_High_Cut_listBox1.FormattingEnabled = true;
            this.Default_High_Cut_listBox1.ItemHeight = 14;
            this.Default_High_Cut_listBox1.Items.AddRange(new object[] {
            "5.5KHz",
            "4.0KHz",
            "3.0KHz",
            "2.7KHz",
            "2.4KHz"});
            this.Default_High_Cut_listBox1.Location = new System.Drawing.Point(177, 126);
            this.Default_High_Cut_listBox1.Name = "Default_High_Cut_listBox1";
            this.Default_High_Cut_listBox1.ScrollAlwaysVisible = true;
            this.Default_High_Cut_listBox1.Size = new System.Drawing.Size(70, 18);
            this.Default_High_Cut_listBox1.TabIndex = 160;
            this.toolTip.SetToolTip(this.Default_High_Cut_listBox1, "Sets the High Side of the Bandwidth filter.\r\nHas no effect in CW mode.");
            this.Default_High_Cut_listBox1.SelectedIndexChanged += new System.EventHandler(this.Default_High_Cut_listBox1_SelectedIndexChanged);
            // 
            // label52
            // 
            this.label52.BackColor = System.Drawing.Color.Transparent;
            this.label52.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label52.ForeColor = System.Drawing.Color.White;
            this.label52.Location = new System.Drawing.Point(92, 149);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(72, 21);
            this.label52.TabIndex = 159;
            this.label52.Text = "CW BW";
            this.label52.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Default_CW_Filter_listBox1
            // 
            this.Default_CW_Filter_listBox1.BackColor = System.Drawing.Color.White;
            this.Default_CW_Filter_listBox1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Default_CW_Filter_listBox1.ForeColor = System.Drawing.Color.Black;
            this.Default_CW_Filter_listBox1.FormattingEnabled = true;
            this.Default_CW_Filter_listBox1.ItemHeight = 14;
            this.Default_CW_Filter_listBox1.Items.AddRange(new object[] {
            "1.8KHz",
            "400Hz",
            "200Hz"});
            this.Default_CW_Filter_listBox1.Location = new System.Drawing.Point(95, 126);
            this.Default_CW_Filter_listBox1.Name = "Default_CW_Filter_listBox1";
            this.Default_CW_Filter_listBox1.Size = new System.Drawing.Size(70, 18);
            this.Default_CW_Filter_listBox1.TabIndex = 158;
            this.toolTip.SetToolTip(this.Default_CW_Filter_listBox1, "Sets CW Bandwidth\r\n");
            this.Default_CW_Filter_listBox1.SelectedIndexChanged += new System.EventHandler(this.Default_CW_Filter_listBox1_SelectedIndexChanged);
            // 
            // label51
            // 
            this.label51.BackColor = System.Drawing.Color.Transparent;
            this.label51.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label51.ForeColor = System.Drawing.Color.White;
            this.label51.Location = new System.Drawing.Point(10, 149);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(72, 21);
            this.label51.TabIndex = 157;
            this.label51.Text = "LOW CUT";
            this.label51.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Default_Low_Cut_listBox1
            // 
            this.Default_Low_Cut_listBox1.BackColor = System.Drawing.Color.White;
            this.Default_Low_Cut_listBox1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Default_Low_Cut_listBox1.ForeColor = System.Drawing.Color.Black;
            this.Default_Low_Cut_listBox1.FormattingEnabled = true;
            this.Default_Low_Cut_listBox1.ItemHeight = 14;
            this.Default_Low_Cut_listBox1.Items.AddRange(new object[] {
            "500Hz",
            "300Hz",
            "200Hz",
            "100Hz",
            "75Hz"});
            this.Default_Low_Cut_listBox1.Location = new System.Drawing.Point(13, 126);
            this.Default_Low_Cut_listBox1.Name = "Default_Low_Cut_listBox1";
            this.Default_Low_Cut_listBox1.Size = new System.Drawing.Size(70, 18);
            this.Default_Low_Cut_listBox1.TabIndex = 156;
            this.toolTip.SetToolTip(this.Default_Low_Cut_listBox1, "Sets the low side of the Filter Bandwidth. \r\nNo effect in CW mode\r\n");
            this.Default_Low_Cut_listBox1.SelectedIndexChanged += new System.EventHandler(this.Default_Low_Cut_listBox1_SelectedIndexChanged);
            // 
            // label50
            // 
            this.label50.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label50.ForeColor = System.Drawing.Color.White;
            this.label50.Location = new System.Drawing.Point(59, 99);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(139, 17);
            this.label50.TabIndex = 155;
            this.label50.Text = "Default Filters";
            this.label50.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label50.Click += new System.EventHandler(this.label50_Click);
            // 
            // Tune_vButton2
            // 
            this.Tune_vButton2.BackColor = System.Drawing.Color.Gainsboro;
            this.Tune_vButton2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Tune_vButton2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tune_vButton2.ForeColor = System.Drawing.Color.Black;
            this.Tune_vButton2.Location = new System.Drawing.Point(281, 184);
            this.Tune_vButton2.Name = "Tune_vButton2";
            this.Tune_vButton2.Size = new System.Drawing.Size(51, 26);
            this.Tune_vButton2.TabIndex = 145;
            this.Tune_vButton2.Text = "TUN";
            this.toolTip.SetToolTip(this.Tune_vButton2, "Turns on TX and applies Tune Output Power\r\nTUNE is ONLY permitted when the \r\nDial" +
        " Frequency is within a legal Amateur Radio Band");
            this.Tune_vButton2.UseVisualStyleBackColor = false;
            this.Tune_vButton2.Click += new System.EventHandler(this.Tune_vButton2_Click_1);
            // 
            // PA_vButton1
            // 
            this.PA_vButton1.BackColor = System.Drawing.Color.Gainsboro;
            this.PA_vButton1.Enabled = false;
            this.PA_vButton1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.PA_vButton1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PA_vButton1.ForeColor = System.Drawing.Color.Black;
            this.PA_vButton1.Location = new System.Drawing.Point(460, 184);
            this.PA_vButton1.Name = "PA_vButton1";
            this.PA_vButton1.Size = new System.Drawing.Size(51, 26);
            this.PA_vButton1.TabIndex = 144;
            this.PA_vButton1.Text = "QRP";
            this.toolTip.SetToolTip(this.PA_vButton1, "Power Amplifier Bypass \r\nQRO - Power Amplifier Active\r\nQRP - Power Amplifier Bypa" +
        "ssed\r\n");
            this.PA_vButton1.UseVisualStyleBackColor = false;
            this.PA_vButton1.Click += new System.EventHandler(this.PA_vButton1_Click_1);
            // 
            // NB_Threshold_label1
            // 
            this.NB_Threshold_label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NB_Threshold_label1.ForeColor = System.Drawing.Color.White;
            this.NB_Threshold_label1.Location = new System.Drawing.Point(60, 328);
            this.NB_Threshold_label1.Name = "NB_Threshold_label1";
            this.NB_Threshold_label1.Size = new System.Drawing.Size(90, 18);
            this.NB_Threshold_label1.TabIndex = 143;
            this.NB_Threshold_label1.Text = "THRESHOLD";
            this.NB_Threshold_label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NB_label16
            // 
            this.NB_label16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NB_label16.ForeColor = System.Drawing.Color.White;
            this.NB_label16.Location = new System.Drawing.Point(52, 256);
            this.NB_label16.Name = "NB_label16";
            this.NB_label16.Size = new System.Drawing.Size(106, 18);
            this.NB_label16.TabIndex = 142;
            this.NB_label16.Text = "PULSE WIDTH";
            this.NB_label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NB_ON_OFF_button2
            // 
            this.NB_ON_OFF_button2.BackColor = System.Drawing.Color.Gainsboro;
            this.NB_ON_OFF_button2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NB_ON_OFF_button2.ForeColor = System.Drawing.Color.Black;
            this.NB_ON_OFF_button2.Location = new System.Drawing.Point(84, 226);
            this.NB_ON_OFF_button2.Name = "NB_ON_OFF_button2";
            this.NB_ON_OFF_button2.Size = new System.Drawing.Size(45, 23);
            this.NB_ON_OFF_button2.TabIndex = 141;
            this.NB_ON_OFF_button2.Text = "OFF";
            this.toolTip.SetToolTip(this.NB_ON_OFF_button2, "Noise Blanker \r\nON/OFF");
            this.NB_ON_OFF_button2.UseVisualStyleBackColor = false;
            this.NB_ON_OFF_button2.Click += new System.EventHandler(this.NB_ON_OFF_button2_Click);
            // 
            // NB_Threshold_label16
            // 
            this.NB_Threshold_label16.BackColor = System.Drawing.Color.Gainsboro;
            this.NB_Threshold_label16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.NB_Threshold_label16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NB_Threshold_label16.ForeColor = System.Drawing.Color.Black;
            this.NB_Threshold_label16.Location = new System.Drawing.Point(72, 375);
            this.NB_Threshold_label16.Name = "NB_Threshold_label16";
            this.NB_Threshold_label16.Size = new System.Drawing.Size(60, 18);
            this.NB_Threshold_label16.TabIndex = 128;
            this.NB_Threshold_label16.Text = "20%";
            this.NB_Threshold_label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NB_Threshold_hScrollBar1
            // 
            this.NB_Threshold_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.NB_Threshold_hScrollBar1.LargeChange = 20;
            this.NB_Threshold_hScrollBar1.Location = new System.Drawing.Point(13, 353);
            this.NB_Threshold_hScrollBar1.Maximum = 1009;
            this.NB_Threshold_hScrollBar1.Minimum = 1;
            this.NB_Threshold_hScrollBar1.Name = "NB_Threshold_hScrollBar1";
            this.NB_Threshold_hScrollBar1.Size = new System.Drawing.Size(185, 15);
            this.NB_Threshold_hScrollBar1.SmallChange = 5;
            this.NB_Threshold_hScrollBar1.TabIndex = 127;
            this.toolTip.SetToolTip(this.NB_Threshold_hScrollBar1, "Relative Signal Strength of Pulse");
            this.NB_Threshold_hScrollBar1.Value = 20;
            this.NB_Threshold_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.NB_Threshold_hScrollBar1_Scroll);
            // 
            // NB_Width_label16
            // 
            this.NB_Width_label16.BackColor = System.Drawing.Color.Gainsboro;
            this.NB_Width_label16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.NB_Width_label16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NB_Width_label16.ForeColor = System.Drawing.Color.Black;
            this.NB_Width_label16.Location = new System.Drawing.Point(72, 303);
            this.NB_Width_label16.Name = "NB_Width_label16";
            this.NB_Width_label16.Size = new System.Drawing.Size(67, 18);
            this.NB_Width_label16.TabIndex = 126;
            this.NB_Width_label16.Text = "500 uS";
            this.NB_Width_label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.NB_Width_label16.Click += new System.EventHandler(this.NB_Width_label16_Click);
            // 
            // NB_hScrollBar1
            // 
            this.NB_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.NB_hScrollBar1.Location = new System.Drawing.Point(13, 281);
            this.NB_hScrollBar1.Maximum = 510;
            this.NB_hScrollBar1.Minimum = 10;
            this.NB_hScrollBar1.Name = "NB_hScrollBar1";
            this.NB_hScrollBar1.Size = new System.Drawing.Size(185, 15);
            this.NB_hScrollBar1.TabIndex = 125;
            this.toolTip.SetToolTip(this.NB_hScrollBar1, "Approximate Width of Pulse Interference");
            this.NB_hScrollBar1.Value = 30;
            this.NB_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.NB_hScrollBar1_Scroll);
            // 
            // NR_hScrollBar1
            // 
            this.NR_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.NR_hScrollBar1.LargeChange = 20;
            this.NR_hScrollBar1.Location = new System.Drawing.Point(297, 328);
            this.NR_hScrollBar1.Minimum = 10;
            this.NR_hScrollBar1.Name = "NR_hScrollBar1";
            this.NR_hScrollBar1.Size = new System.Drawing.Size(185, 15);
            this.NR_hScrollBar1.SmallChange = 10;
            this.NR_hScrollBar1.TabIndex = 123;
            this.toolTip.SetToolTip(this.NR_hScrollBar1, "Not yet implemented");
            this.NR_hScrollBar1.Value = 20;
            this.NR_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.NR_hScrollBar1_Scroll);
            // 
            // NR_label2
            // 
            this.NR_label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NR_label2.ForeColor = System.Drawing.Color.White;
            this.NR_label2.Location = new System.Drawing.Point(309, 292);
            this.NR_label2.Name = "NR_label2";
            this.NR_label2.Size = new System.Drawing.Size(48, 18);
            this.NR_label2.TabIndex = 122;
            this.NR_label2.Text = "NR";
            this.NR_label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AGC_listBox1
            // 
            this.AGC_listBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.AGC_listBox1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AGC_listBox1.ForeColor = System.Drawing.Color.Black;
            this.AGC_listBox1.FormattingEnabled = true;
            this.AGC_listBox1.ItemHeight = 14;
            this.AGC_listBox1.Items.AddRange(new object[] {
            "SLOW",
            "MED",
            "FAST"});
            this.AGC_listBox1.Location = new System.Drawing.Point(616, 256);
            this.AGC_listBox1.Name = "AGC_listBox1";
            this.AGC_listBox1.ScrollAlwaysVisible = true;
            this.AGC_listBox1.Size = new System.Drawing.Size(70, 18);
            this.AGC_listBox1.TabIndex = 121;
            this.toolTip.SetToolTip(this.AGC_listBox1, "Automatic Gain Control\r\nSlow,Medimum,Fast\r\n\r\n");
            this.AGC_listBox1.SelectedIndexChanged += new System.EventHandler(this.AGC_listBox1_SelectedIndexChanged);
            // 
            // AGC_label2
            // 
            this.AGC_label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AGC_label2.ForeColor = System.Drawing.Color.White;
            this.AGC_label2.Location = new System.Drawing.Point(701, 256);
            this.AGC_label2.Name = "AGC_label2";
            this.AGC_label2.Size = new System.Drawing.Size(40, 20);
            this.AGC_label2.TabIndex = 120;
            this.AGC_label2.Text = "AGC";
            this.AGC_label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Tune_Power_label37
            // 
            this.Tune_Power_label37.BackColor = System.Drawing.Color.Gainsboro;
            this.Tune_Power_label37.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Tune_Power_label37.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tune_Power_label37.Location = new System.Drawing.Point(366, 152);
            this.Tune_Power_label37.Name = "Tune_Power_label37";
            this.Tune_Power_label37.Size = new System.Drawing.Size(61, 23);
            this.Tune_Power_label37.TabIndex = 119;
            this.Tune_Power_label37.Text = "50";
            this.Tune_Power_label37.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Tune_Power_label37.Click += new System.EventHandler(this.Tune_Power_label37_Click);
            // 
            // Tune_Power_hScrollBar1
            // 
            this.Tune_Power_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.Tune_Power_hScrollBar1.LargeChange = 5;
            this.Tune_Power_hScrollBar1.Location = new System.Drawing.Point(286, 129);
            this.Tune_Power_hScrollBar1.Maximum = 104;
            this.Tune_Power_hScrollBar1.Name = "Tune_Power_hScrollBar1";
            this.Tune_Power_hScrollBar1.Size = new System.Drawing.Size(221, 15);
            this.Tune_Power_hScrollBar1.TabIndex = 118;
            this.Tune_Power_hScrollBar1.Value = 50;
            this.Tune_Power_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Tune_Power_hScrollBar1_Scroll);
            // 
            // label36
            // 
            this.label36.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.ForeColor = System.Drawing.Color.White;
            this.label36.Location = new System.Drawing.Point(309, 99);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(174, 17);
            this.label36.TabIndex = 117;
            this.label36.Text = "TUNE Output Level";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label36.Click += new System.EventHandler(this.label36_Click_1);
            // 
            // CW_Power_label36
            // 
            this.CW_Power_label36.BackColor = System.Drawing.Color.Gainsboro;
            this.CW_Power_label36.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CW_Power_label36.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CW_Power_label36.Location = new System.Drawing.Point(618, 55);
            this.CW_Power_label36.Name = "CW_Power_label36";
            this.CW_Power_label36.Size = new System.Drawing.Size(61, 23);
            this.CW_Power_label36.TabIndex = 116;
            this.CW_Power_label36.Text = "100";
            this.CW_Power_label36.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // SSB_Power_label36
            // 
            this.SSB_Power_label36.BackColor = System.Drawing.Color.Gainsboro;
            this.SSB_Power_label36.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SSB_Power_label36.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SSB_Power_label36.Location = new System.Drawing.Point(366, 55);
            this.SSB_Power_label36.Name = "SSB_Power_label36";
            this.SSB_Power_label36.Size = new System.Drawing.Size(61, 23);
            this.SSB_Power_label36.TabIndex = 115;
            this.SSB_Power_label36.Text = "100%";
            this.SSB_Power_label36.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.SSB_Power_label36.Click += new System.EventHandler(this.SSB_Power_label36_Click);
            // 
            // AM_Carrier_label36
            // 
            this.AM_Carrier_label36.BackColor = System.Drawing.Color.Gainsboro;
            this.AM_Carrier_label36.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AM_Carrier_label36.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AM_Carrier_label36.Location = new System.Drawing.Point(114, 55);
            this.AM_Carrier_label36.Name = "AM_Carrier_label36";
            this.AM_Carrier_label36.Size = new System.Drawing.Size(61, 23);
            this.AM_Carrier_label36.TabIndex = 114;
            this.AM_Carrier_label36.Text = "100";
            this.AM_Carrier_label36.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.AM_Carrier_label36.Click += new System.EventHandler(this.AM_Carrier_label36_Click);
            // 
            // CW_Power_hScrollBar1
            // 
            this.CW_Power_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.CW_Power_hScrollBar1.LargeChange = 2;
            this.CW_Power_hScrollBar1.Location = new System.Drawing.Point(538, 30);
            this.CW_Power_hScrollBar1.Maximum = 101;
            this.CW_Power_hScrollBar1.Name = "CW_Power_hScrollBar1";
            this.CW_Power_hScrollBar1.Size = new System.Drawing.Size(221, 15);
            this.CW_Power_hScrollBar1.TabIndex = 113;
            this.CW_Power_hScrollBar1.Value = 50;
            this.CW_Power_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.CW_Power_hScrollBar1_Scroll);
            // 
            // label34
            // 
            this.label34.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.ForeColor = System.Drawing.Color.White;
            this.label34.Location = new System.Drawing.Point(571, 3);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(154, 17);
            this.label34.TabIndex = 112;
            this.label34.Text = "CW Output Level";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Power_hScrollBar1
            // 
            this.Power_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.Power_hScrollBar1.LargeChange = 2;
            this.Power_hScrollBar1.Location = new System.Drawing.Point(286, 30);
            this.Power_hScrollBar1.Maximum = 101;
            this.Power_hScrollBar1.Name = "Power_hScrollBar1";
            this.Power_hScrollBar1.Size = new System.Drawing.Size(221, 15);
            this.Power_hScrollBar1.TabIndex = 111;
            this.Power_hScrollBar1.Value = 50;
            this.Power_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Power_hScrollBar1_Scroll);
            // 
            // Power_label34
            // 
            this.Power_label34.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Power_label34.ForeColor = System.Drawing.Color.White;
            this.Power_label34.Location = new System.Drawing.Point(319, 3);
            this.Power_label34.Name = "Power_label34";
            this.Power_label34.Size = new System.Drawing.Size(154, 17);
            this.Power_label34.TabIndex = 110;
            this.Power_label34.Text = "AM / SSB Output Level";
            this.Power_label34.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TX_Bandwidth_listBox1
            // 
            this.TX_Bandwidth_listBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.TX_Bandwidth_listBox1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TX_Bandwidth_listBox1.ForeColor = System.Drawing.Color.Black;
            this.TX_Bandwidth_listBox1.FormattingEnabled = true;
            this.TX_Bandwidth_listBox1.ItemHeight = 14;
            this.TX_Bandwidth_listBox1.Items.AddRange(new object[] {
            "2.4KHz",
            "2.7KHz",
            "3.0KHz",
            "5.5KHz"});
            this.TX_Bandwidth_listBox1.Location = new System.Drawing.Point(94, 190);
            this.TX_Bandwidth_listBox1.Name = "TX_Bandwidth_listBox1";
            this.TX_Bandwidth_listBox1.ScrollAlwaysVisible = true;
            this.TX_Bandwidth_listBox1.Size = new System.Drawing.Size(68, 18);
            this.TX_Bandwidth_listBox1.TabIndex = 109;
            this.toolTip.SetToolTip(this.TX_Bandwidth_listBox1, "Set the TX bandwidth. ");
            this.TX_Bandwidth_listBox1.SelectedIndexChanged += new System.EventHandler(this.TX_Bandwidth_listBox1_SelectedIndexChanged);
            // 
            // AM_Carrier_label2
            // 
            this.AM_Carrier_label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AM_Carrier_label2.ForeColor = System.Drawing.Color.White;
            this.AM_Carrier_label2.Location = new System.Drawing.Point(67, 3);
            this.AM_Carrier_label2.Name = "AM_Carrier_label2";
            this.AM_Carrier_label2.Size = new System.Drawing.Size(154, 17);
            this.AM_Carrier_label2.TabIndex = 79;
            this.AM_Carrier_label2.Text = "AM Carrier Level";
            this.AM_Carrier_label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AM_Carrier_hScrollBar1
            // 
            this.AM_Carrier_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.AM_Carrier_hScrollBar1.LargeChange = 2;
            this.AM_Carrier_hScrollBar1.Location = new System.Drawing.Point(34, 30);
            this.AM_Carrier_hScrollBar1.Maximum = 101;
            this.AM_Carrier_hScrollBar1.Name = "AM_Carrier_hScrollBar1";
            this.AM_Carrier_hScrollBar1.Size = new System.Drawing.Size(221, 15);
            this.AM_Carrier_hScrollBar1.TabIndex = 78;
            this.AM_Carrier_hScrollBar1.Value = 50;
            this.AM_Carrier_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.AM_Carrier_hScrollBar1_Scroll);
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.White;
            this.label14.Location = new System.Drawing.Point(72, 170);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(113, 17);
            this.label14.TabIndex = 77;
            this.label14.Text = "TX Bandwidth";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // band_stack
            // 
            this.band_stack.AutoScroll = true;
            this.band_stack.BackColor = System.Drawing.Color.Black;
            this.band_stack.Controls.Add(this.label3);
            this.band_stack.Controls.Add(this.General_listView1);
            this.band_stack.Controls.Add(this.checkBox1);
            this.band_stack.Controls.Add(this.Favorites_textBox2);
            this.band_stack.Controls.Add(this.B10_Favs_listView1);
            this.band_stack.Controls.Add(this.B12_Favs_listView1);
            this.band_stack.Controls.Add(this.B15_Favs_listView1);
            this.band_stack.Controls.Add(this.B17_Favs_listView1);
            this.band_stack.Controls.Add(this.B20_Favs_listView1);
            this.band_stack.Controls.Add(this.B30_Favs_listView1);
            this.band_stack.Controls.Add(this.B40_Favs_listView1);
            this.band_stack.Controls.Add(this.B60_Favs_listView1);
            this.band_stack.Controls.Add(this.B80_Favs_listView1);
            this.band_stack.Controls.Add(this.B160_Favs_listView1);
            this.band_stack.Controls.Add(this.textBox1);
            this.band_stack.Controls.Add(this.label31);
            this.band_stack.Controls.Add(this.label30);
            this.band_stack.Controls.Add(this.label29);
            this.band_stack.Controls.Add(this.band_stack_textBox1);
            this.band_stack.Controls.Add(this.band_stack_label29);
            this.band_stack.Controls.Add(this.label28);
            this.band_stack.Controls.Add(this.label27);
            this.band_stack.Controls.Add(this.label26);
            this.band_stack.Controls.Add(this.label25);
            this.band_stack.Controls.Add(this.label24);
            this.band_stack.Controls.Add(this.label23);
            this.band_stack.Controls.Add(this.label22);
            this.band_stack.Controls.Add(this.label21);
            this.band_stack.Controls.Add(this.label20);
            this.band_stack.Controls.Add(this.label19);
            this.band_stack.Controls.Add(this.band_stack_update_button1);
            this.band_stack.ForeColor = System.Drawing.Color.Black;
            this.band_stack.Location = new System.Drawing.Point(4, 25);
            this.band_stack.Name = "band_stack";
            this.band_stack.Padding = new System.Windows.Forms.Padding(3);
            this.band_stack.Size = new System.Drawing.Size(792, 451);
            this.band_stack.TabIndex = 8;
            this.band_stack.Text = "Favs";
            this.band_stack.ToolTipText = "Favorite Frequencies";
            this.band_stack.Click += new System.EventHandler(this.band_stack_Click);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(540, 326);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 33);
            this.label3.TabIndex = 85;
            this.label3.Text = "GEN";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // General_listView1
            // 
            this.General_listView1.BackColor = System.Drawing.Color.Gainsboro;
            this.General_listView1.HideSelection = false;
            this.General_listView1.Location = new System.Drawing.Point(258, 318);
            this.General_listView1.MultiSelect = false;
            this.General_listView1.Name = "General_listView1";
            this.General_listView1.Size = new System.Drawing.Size(276, 50);
            this.General_listView1.TabIndex = 84;
            this.toolTip.SetToolTip(this.General_listView1, "General Favorites.\r\nNot Yet Implemented");
            this.General_listView1.UseCompatibleStateImageBehavior = false;
            this.General_listView1.View = System.Windows.Forms.View.List;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.checkBox1.ForeColor = System.Drawing.Color.Black;
            this.checkBox1.Location = new System.Drawing.Point(361, 273);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(71, 20);
            this.checkBox1.TabIndex = 83;
            this.checkBox1.Text = "Sorted";
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Favorites_textBox2
            // 
            this.Favorites_textBox2.BackColor = System.Drawing.Color.Gainsboro;
            this.Favorites_textBox2.ForeColor = System.Drawing.Color.Black;
            this.Favorites_textBox2.Location = new System.Drawing.Point(239, 390);
            this.Favorites_textBox2.MaxLength = 15;
            this.Favorites_textBox2.Name = "Favorites_textBox2";
            this.Favorites_textBox2.Size = new System.Drawing.Size(146, 23);
            this.Favorites_textBox2.TabIndex = 82;
            this.Favorites_textBox2.Text = "Enter Name";
            this.Favorites_textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip.SetToolTip(this.Favorites_textBox2, "ENTER NAME (Optional)\r\nbefore pressing\r\nUPDATE FAVORITES\r\n");
            this.Favorites_textBox2.TextChanged += new System.EventHandler(this.Favorites_textBox2_TextChanged);
            this.Favorites_textBox2.Enter += new System.EventHandler(this.Favorites_textBox2_Entered);
            // 
            // B10_Favs_listView1
            // 
            this.B10_Favs_listView1.BackColor = System.Drawing.Color.Gainsboro;
            this.B10_Favs_listView1.HideSelection = false;
            this.B10_Favs_listView1.Location = new System.Drawing.Point(456, 262);
            this.B10_Favs_listView1.MultiSelect = false;
            this.B10_Favs_listView1.Name = "B10_Favs_listView1";
            this.B10_Favs_listView1.Size = new System.Drawing.Size(276, 50);
            this.B10_Favs_listView1.TabIndex = 81;
            this.B10_Favs_listView1.UseCompatibleStateImageBehavior = false;
            this.B10_Favs_listView1.View = System.Windows.Forms.View.List;
            this.B10_Favs_listView1.SelectedIndexChanged += new System.EventHandler(this.B10_Favs_listView1_SelectedIndexChanged);
            // 
            // B12_Favs_listView1
            // 
            this.B12_Favs_listView1.BackColor = System.Drawing.Color.Gainsboro;
            this.B12_Favs_listView1.HideSelection = false;
            this.B12_Favs_listView1.Location = new System.Drawing.Point(456, 198);
            this.B12_Favs_listView1.MultiSelect = false;
            this.B12_Favs_listView1.Name = "B12_Favs_listView1";
            this.B12_Favs_listView1.Size = new System.Drawing.Size(276, 50);
            this.B12_Favs_listView1.TabIndex = 80;
            this.B12_Favs_listView1.UseCompatibleStateImageBehavior = false;
            this.B12_Favs_listView1.View = System.Windows.Forms.View.List;
            this.B12_Favs_listView1.SelectedIndexChanged += new System.EventHandler(this.B12_Favs_listView1_SelectedIndexChanged);
            // 
            // B15_Favs_listView1
            // 
            this.B15_Favs_listView1.BackColor = System.Drawing.Color.Gainsboro;
            this.B15_Favs_listView1.HideSelection = false;
            this.B15_Favs_listView1.Location = new System.Drawing.Point(456, 134);
            this.B15_Favs_listView1.MultiSelect = false;
            this.B15_Favs_listView1.Name = "B15_Favs_listView1";
            this.B15_Favs_listView1.Size = new System.Drawing.Size(276, 50);
            this.B15_Favs_listView1.TabIndex = 79;
            this.B15_Favs_listView1.UseCompatibleStateImageBehavior = false;
            this.B15_Favs_listView1.View = System.Windows.Forms.View.List;
            this.B15_Favs_listView1.SelectedIndexChanged += new System.EventHandler(this.B15_Favs_listView1_SelectedIndexChanged);
            // 
            // B17_Favs_listView1
            // 
            this.B17_Favs_listView1.BackColor = System.Drawing.Color.Gainsboro;
            this.B17_Favs_listView1.HideSelection = false;
            this.B17_Favs_listView1.Location = new System.Drawing.Point(456, 70);
            this.B17_Favs_listView1.MultiSelect = false;
            this.B17_Favs_listView1.Name = "B17_Favs_listView1";
            this.B17_Favs_listView1.Size = new System.Drawing.Size(276, 50);
            this.B17_Favs_listView1.TabIndex = 78;
            this.B17_Favs_listView1.UseCompatibleStateImageBehavior = false;
            this.B17_Favs_listView1.View = System.Windows.Forms.View.List;
            this.B17_Favs_listView1.SelectedIndexChanged += new System.EventHandler(this.B17_Favs_listView1_SelectedIndexChanged);
            // 
            // B20_Favs_listView1
            // 
            this.B20_Favs_listView1.BackColor = System.Drawing.Color.Gainsboro;
            this.B20_Favs_listView1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.B20_Favs_listView1.ForeColor = System.Drawing.Color.Black;
            this.B20_Favs_listView1.HideSelection = false;
            this.B20_Favs_listView1.LabelEdit = true;
            this.B20_Favs_listView1.Location = new System.Drawing.Point(456, 6);
            this.B20_Favs_listView1.MultiSelect = false;
            this.B20_Favs_listView1.Name = "B20_Favs_listView1";
            this.B20_Favs_listView1.Size = new System.Drawing.Size(276, 50);
            this.B20_Favs_listView1.TabIndex = 77;
            this.B20_Favs_listView1.UseCompatibleStateImageBehavior = false;
            this.B20_Favs_listView1.View = System.Windows.Forms.View.List;
            this.B20_Favs_listView1.SelectedIndexChanged += new System.EventHandler(this.B20_Favs_listView1_SelectedIndexChanged);
            // 
            // B30_Favs_listView1
            // 
            this.B30_Favs_listView1.BackColor = System.Drawing.Color.Gainsboro;
            this.B30_Favs_listView1.HideSelection = false;
            this.B30_Favs_listView1.Location = new System.Drawing.Point(58, 262);
            this.B30_Favs_listView1.MultiSelect = false;
            this.B30_Favs_listView1.Name = "B30_Favs_listView1";
            this.B30_Favs_listView1.Size = new System.Drawing.Size(276, 50);
            this.B30_Favs_listView1.TabIndex = 76;
            this.B30_Favs_listView1.UseCompatibleStateImageBehavior = false;
            this.B30_Favs_listView1.View = System.Windows.Forms.View.List;
            this.B30_Favs_listView1.SelectedIndexChanged += new System.EventHandler(this.B30_Favs_listView1_SelectedIndexChanged);
            // 
            // B40_Favs_listView1
            // 
            this.B40_Favs_listView1.BackColor = System.Drawing.Color.Gainsboro;
            this.B40_Favs_listView1.ForeColor = System.Drawing.Color.Black;
            this.B40_Favs_listView1.HideSelection = false;
            this.B40_Favs_listView1.Location = new System.Drawing.Point(58, 198);
            this.B40_Favs_listView1.MultiSelect = false;
            this.B40_Favs_listView1.Name = "B40_Favs_listView1";
            this.B40_Favs_listView1.Size = new System.Drawing.Size(276, 50);
            this.B40_Favs_listView1.TabIndex = 75;
            this.B40_Favs_listView1.UseCompatibleStateImageBehavior = false;
            this.B40_Favs_listView1.View = System.Windows.Forms.View.List;
            this.B40_Favs_listView1.SelectedIndexChanged += new System.EventHandler(this.B40_Favs_listView1_SelectedIndexChanged);
            // 
            // B60_Favs_listView1
            // 
            this.B60_Favs_listView1.BackColor = System.Drawing.Color.Gainsboro;
            this.B60_Favs_listView1.ForeColor = System.Drawing.Color.Black;
            this.B60_Favs_listView1.HideSelection = false;
            this.B60_Favs_listView1.Location = new System.Drawing.Point(58, 134);
            this.B60_Favs_listView1.MultiSelect = false;
            this.B60_Favs_listView1.Name = "B60_Favs_listView1";
            this.B60_Favs_listView1.Size = new System.Drawing.Size(276, 50);
            this.B60_Favs_listView1.TabIndex = 74;
            this.B60_Favs_listView1.UseCompatibleStateImageBehavior = false;
            this.B60_Favs_listView1.View = System.Windows.Forms.View.List;
            this.B60_Favs_listView1.SelectedIndexChanged += new System.EventHandler(this.B60_Favs_listView1_SelectedIndexChanged);
            // 
            // B80_Favs_listView1
            // 
            this.B80_Favs_listView1.BackColor = System.Drawing.Color.Gainsboro;
            this.B80_Favs_listView1.ForeColor = System.Drawing.Color.Black;
            this.B80_Favs_listView1.HideSelection = false;
            this.B80_Favs_listView1.Location = new System.Drawing.Point(58, 70);
            this.B80_Favs_listView1.MultiSelect = false;
            this.B80_Favs_listView1.Name = "B80_Favs_listView1";
            this.B80_Favs_listView1.Size = new System.Drawing.Size(276, 50);
            this.B80_Favs_listView1.TabIndex = 73;
            this.B80_Favs_listView1.UseCompatibleStateImageBehavior = false;
            this.B80_Favs_listView1.View = System.Windows.Forms.View.List;
            this.B80_Favs_listView1.SelectedIndexChanged += new System.EventHandler(this.B80_Favs_listView1_SelectedIndexChanged);
            // 
            // B160_Favs_listView1
            // 
            this.B160_Favs_listView1.BackColor = System.Drawing.Color.Gainsboro;
            this.B160_Favs_listView1.ForeColor = System.Drawing.Color.Black;
            this.B160_Favs_listView1.HideSelection = false;
            this.B160_Favs_listView1.Location = new System.Drawing.Point(58, 6);
            this.B160_Favs_listView1.MultiSelect = false;
            this.B160_Favs_listView1.Name = "B160_Favs_listView1";
            this.B160_Favs_listView1.Size = new System.Drawing.Size(276, 50);
            this.B160_Favs_listView1.TabIndex = 72;
            this.B160_Favs_listView1.UseCompatibleStateImageBehavior = false;
            this.B160_Favs_listView1.View = System.Windows.Forms.View.List;
            this.B160_Favs_listView1.SelectedIndexChanged += new System.EventHandler(this.B160_Favs_listView1_SelectedIndexChanged_2);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.textBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.Color.Black;
            this.textBox1.Location = new System.Drawing.Point(657, 326);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(64, 21);
            this.textBox1.TabIndex = 67;
            this.textBox1.Text = "USB";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged_1);
            // 
            // label31
            // 
            this.label31.BackColor = System.Drawing.Color.Gainsboro;
            this.label31.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.ForeColor = System.Drawing.Color.Black;
            this.label31.Location = new System.Drawing.Point(601, 326);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(50, 21);
            this.label31.TabIndex = 66;
            this.label31.Text = "MODE";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label31.Click += new System.EventHandler(this.label31_Click);
            // 
            // label30
            // 
            this.label30.BackColor = System.Drawing.Color.Gainsboro;
            this.label30.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.ForeColor = System.Drawing.Color.Black;
            this.label30.Location = new System.Drawing.Point(55, 325);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(50, 21);
            this.label30.TabIndex = 65;
            this.label30.Text = "QRG";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label30.Click += new System.EventHandler(this.label30_Click);
            // 
            // label29
            // 
            this.label29.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.ForeColor = System.Drawing.Color.Cyan;
            this.label29.Location = new System.Drawing.Point(64, 289);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(50, 33);
            this.label29.TabIndex = 64;
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label29.Click += new System.EventHandler(this.label29_Click);
            // 
            // band_stack_textBox1
            // 
            this.band_stack_textBox1.BackColor = System.Drawing.Color.LightGray;
            this.band_stack_textBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.band_stack_textBox1.ForeColor = System.Drawing.Color.Black;
            this.band_stack_textBox1.Location = new System.Drawing.Point(111, 325);
            this.band_stack_textBox1.Name = "band_stack_textBox1";
            this.band_stack_textBox1.Size = new System.Drawing.Size(67, 21);
            this.band_stack_textBox1.TabIndex = 63;
            this.band_stack_textBox1.Text = "30000000";
            this.band_stack_textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.band_stack_textBox1.TextChanged += new System.EventHandler(this.band_stack_textBox1_TextChanged_1);
            // 
            // band_stack_label29
            // 
            this.band_stack_label29.BackColor = System.Drawing.Color.Gainsboro;
            this.band_stack_label29.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.band_stack_label29.Font = new System.Drawing.Font("Verdana", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.band_stack_label29.ForeColor = System.Drawing.Color.Black;
            this.band_stack_label29.Location = new System.Drawing.Point(323, 423);
            this.band_stack_label29.Name = "band_stack_label29";
            this.band_stack_label29.Size = new System.Drawing.Size(128, 23);
            this.band_stack_label29.TabIndex = 61;
            this.band_stack_label29.Text = "FAVORITES";
            this.band_stack_label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.band_stack_label29.Click += new System.EventHandler(this.band_stack_label29_Click);
            // 
            // label28
            // 
            this.label28.BackColor = System.Drawing.Color.Transparent;
            this.label28.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.ForeColor = System.Drawing.Color.White;
            this.label28.Location = new System.Drawing.Point(738, 273);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(35, 33);
            this.label28.TabIndex = 60;
            this.label28.Text = "10";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label28.Click += new System.EventHandler(this.label28_Click);
            // 
            // label27
            // 
            this.label27.BackColor = System.Drawing.Color.Transparent;
            this.label27.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.ForeColor = System.Drawing.Color.White;
            this.label27.Location = new System.Drawing.Point(738, 208);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(35, 33);
            this.label27.TabIndex = 59;
            this.label27.Text = "12";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label27.Click += new System.EventHandler(this.label27_Click);
            // 
            // label26
            // 
            this.label26.BackColor = System.Drawing.Color.Transparent;
            this.label26.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.ForeColor = System.Drawing.Color.White;
            this.label26.Location = new System.Drawing.Point(738, 148);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(35, 33);
            this.label26.TabIndex = 58;
            this.label26.Text = "15";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label26.Click += new System.EventHandler(this.label26_Click);
            // 
            // label25
            // 
            this.label25.BackColor = System.Drawing.Color.Transparent;
            this.label25.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.Color.White;
            this.label25.Location = new System.Drawing.Point(738, 83);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(35, 33);
            this.label25.TabIndex = 57;
            this.label25.Text = "17";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label25.Click += new System.EventHandler(this.label25_Click);
            // 
            // label24
            // 
            this.label24.BackColor = System.Drawing.Color.Transparent;
            this.label24.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.ForeColor = System.Drawing.Color.White;
            this.label24.Location = new System.Drawing.Point(738, 15);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(35, 33);
            this.label24.TabIndex = 56;
            this.label24.Text = "20";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label24.Click += new System.EventHandler(this.label24_Click);
            // 
            // label23
            // 
            this.label23.BackColor = System.Drawing.Color.Transparent;
            this.label23.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.ForeColor = System.Drawing.Color.White;
            this.label23.Location = new System.Drawing.Point(19, 273);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(33, 33);
            this.label23.TabIndex = 55;
            this.label23.Text = "30";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label23.Click += new System.EventHandler(this.label23_Click);
            // 
            // label22
            // 
            this.label22.BackColor = System.Drawing.Color.Transparent;
            this.label22.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.White;
            this.label22.Location = new System.Drawing.Point(19, 208);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(33, 33);
            this.label22.TabIndex = 54;
            this.label22.Text = "40";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label22.Click += new System.EventHandler(this.label22_Click);
            // 
            // label21
            // 
            this.label21.BackColor = System.Drawing.Color.Transparent;
            this.label21.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.ForeColor = System.Drawing.Color.White;
            this.label21.Location = new System.Drawing.Point(19, 148);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(33, 33);
            this.label21.TabIndex = 53;
            this.label21.Text = "60";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label21.Click += new System.EventHandler(this.label21_Click);
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.Transparent;
            this.label20.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.White;
            this.label20.Location = new System.Drawing.Point(19, 83);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(33, 33);
            this.label20.TabIndex = 52;
            this.label20.Text = "80";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label20.Click += new System.EventHandler(this.label20_Click);
            // 
            // label19
            // 
            this.label19.BackColor = System.Drawing.Color.Transparent;
            this.label19.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.Color.White;
            this.label19.Location = new System.Drawing.Point(19, 15);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(33, 33);
            this.label19.TabIndex = 51;
            this.label19.Text = "160";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label19.Click += new System.EventHandler(this.label19_Click);
            // 
            // band_stack_update_button1
            // 
            this.band_stack_update_button1.BackColor = System.Drawing.Color.Gainsboro;
            this.band_stack_update_button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.band_stack_update_button1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.band_stack_update_button1.ForeColor = System.Drawing.Color.Black;
            this.band_stack_update_button1.Location = new System.Drawing.Point(408, 387);
            this.band_stack_update_button1.Name = "band_stack_update_button1";
            this.band_stack_update_button1.Size = new System.Drawing.Size(146, 28);
            this.band_stack_update_button1.TabIndex = 40;
            this.band_stack_update_button1.Text = "UPDATE FAVORITES";
            this.band_stack_update_button1.UseVisualStyleBackColor = false;
            this.band_stack_update_button1.Click += new System.EventHandler(this.band_stack_update_button1_Click_1);
            // 
            // freqcaltab
            // 
            this.freqcaltab.BackColor = System.Drawing.Color.Black;
            this.freqcaltab.Controls.Add(this.IQ_groupBox3);
            this.freqcaltab.Controls.Add(this.Freq_Cal_groupBox4);
            this.freqcaltab.ForeColor = System.Drawing.Color.Black;
            this.freqcaltab.Location = new System.Drawing.Point(4, 25);
            this.freqcaltab.Name = "freqcaltab";
            this.freqcaltab.Padding = new System.Windows.Forms.Padding(10);
            this.freqcaltab.Size = new System.Drawing.Size(792, 451);
            this.freqcaltab.TabIndex = 6;
            this.freqcaltab.Text = "XCVR Cal";
            this.freqcaltab.ToolTipText = "Frequency and I/Q Calibration";
            this.freqcaltab.Click += new System.EventHandler(this.freqcaltab_Click);
            this.freqcaltab.Enter += new System.EventHandler(this.freqcaltab_Enter);
            this.freqcaltab.Leave += new System.EventHandler(this.freqcaltab_Leave);
            // 
            // IQ_groupBox3
            // 
            this.IQ_groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.IQ_groupBox3.Controls.Add(this.IQ_Tune_Power_hScrollBar1);
            this.IQ_groupBox3.Controls.Add(this.Reset_Freq_button3);
            this.IQ_groupBox3.Controls.Add(this.IQ_UP24KHz_checkBox2);
            this.IQ_groupBox3.Controls.Add(this.IQ_Freq_hScrollBar1);
            this.IQ_groupBox3.Controls.Add(this.Cal_Freq_textBox2);
            this.IQ_groupBox3.Controls.Add(this.IQ_TX_button);
            this.IQ_groupBox3.Controls.Add(this.IQ_Reset_All_button2);
            this.IQ_groupBox3.Controls.Add(this.LeftResetbutton2);
            this.IQ_groupBox3.Controls.Add(this.groupBox2);
            this.IQ_groupBox3.Controls.Add(this.IQ_RX_button);
            this.IQ_groupBox3.Controls.Add(this.LefthScrollBar1);
            this.IQ_groupBox3.Controls.Add(this.IQ_Commit_button2);
            this.IQ_groupBox3.Controls.Add(this.IQLefttextBox2);
            this.IQ_groupBox3.Controls.Add(this.IQ_Tune_button2);
            this.IQ_groupBox3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.IQ_groupBox3.ForeColor = System.Drawing.Color.White;
            this.IQ_groupBox3.Location = new System.Drawing.Point(10, 206);
            this.IQ_groupBox3.Name = "IQ_groupBox3";
            this.IQ_groupBox3.Size = new System.Drawing.Size(772, 235);
            this.IQ_groupBox3.TabIndex = 126;
            this.IQ_groupBox3.TabStop = false;
            this.IQ_groupBox3.Text = "I/Q CALIBRATION";
            this.IQ_groupBox3.Enter += new System.EventHandler(this.IQ_groupBox3_Enter);
            // 
            // IQ_Tune_Power_hScrollBar1
            // 
            this.IQ_Tune_Power_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.IQ_Tune_Power_hScrollBar1.LargeChange = 5;
            this.IQ_Tune_Power_hScrollBar1.Location = new System.Drawing.Point(276, 137);
            this.IQ_Tune_Power_hScrollBar1.Maximum = 104;
            this.IQ_Tune_Power_hScrollBar1.Name = "IQ_Tune_Power_hScrollBar1";
            this.IQ_Tune_Power_hScrollBar1.Size = new System.Drawing.Size(221, 15);
            this.IQ_Tune_Power_hScrollBar1.TabIndex = 144;
            this.toolTip.SetToolTip(this.IQ_Tune_Power_hScrollBar1, "Adjust Output TUNE Power");
            this.IQ_Tune_Power_hScrollBar1.Visible = false;
            this.IQ_Tune_Power_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.IQ_Tune_Power_hScrollBar1_Scroll);
            // 
            // Reset_Freq_button3
            // 
            this.Reset_Freq_button3.BackColor = System.Drawing.Color.Black;
            this.Reset_Freq_button3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Reset_Freq_button3.ForeColor = System.Drawing.Color.White;
            this.Reset_Freq_button3.Location = new System.Drawing.Point(197, 134);
            this.Reset_Freq_button3.Name = "Reset_Freq_button3";
            this.Reset_Freq_button3.Size = new System.Drawing.Size(110, 23);
            this.Reset_Freq_button3.TabIndex = 143;
            this.Reset_Freq_button3.Text = "RESET FREQ";
            this.toolTip.SetToolTip(this.Reset_Freq_button3, "Reset the Slider to Zero");
            this.Reset_Freq_button3.UseVisualStyleBackColor = false;
            this.Reset_Freq_button3.Visible = false;
            this.Reset_Freq_button3.Click += new System.EventHandler(this.Reset_Freq_button3_Click);
            // 
            // IQ_UP24KHz_checkBox2
            // 
            this.IQ_UP24KHz_checkBox2.AutoSize = true;
            this.IQ_UP24KHz_checkBox2.BackColor = System.Drawing.Color.Gainsboro;
            this.IQ_UP24KHz_checkBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IQ_UP24KHz_checkBox2.ForeColor = System.Drawing.Color.Black;
            this.IQ_UP24KHz_checkBox2.Location = new System.Drawing.Point(341, 131);
            this.IQ_UP24KHz_checkBox2.Name = "IQ_UP24KHz_checkBox2";
            this.IQ_UP24KHz_checkBox2.Size = new System.Drawing.Size(81, 17);
            this.IQ_UP24KHz_checkBox2.TabIndex = 142;
            this.IQ_UP24KHz_checkBox2.Text = "UP 24KHz";
            this.IQ_UP24KHz_checkBox2.UseVisualStyleBackColor = false;
            this.IQ_UP24KHz_checkBox2.Visible = false;
            this.IQ_UP24KHz_checkBox2.CheckedChanged += new System.EventHandler(this.IQ_UP24KHz_checkBox2_CheckedChanged);
            // 
            // IQ_Freq_hScrollBar1
            // 
            this.IQ_Freq_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.IQ_Freq_hScrollBar1.Location = new System.Drawing.Point(71, 107);
            this.IQ_Freq_hScrollBar1.Maximum = 2009;
            this.IQ_Freq_hScrollBar1.Minimum = -1000;
            this.IQ_Freq_hScrollBar1.Name = "IQ_Freq_hScrollBar1";
            this.IQ_Freq_hScrollBar1.Size = new System.Drawing.Size(627, 15);
            this.IQ_Freq_hScrollBar1.TabIndex = 127;
            this.toolTip.SetToolTip(this.IQ_Freq_hScrollBar1, "Frequency Fine Tune\r\nUsing Scroll Bar, tune for Image Signal");
            this.IQ_Freq_hScrollBar1.Visible = false;
            this.IQ_Freq_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.IQ_Freq_hScrollBar1_Scroll);
            // 
            // Cal_Freq_textBox2
            // 
            this.Cal_Freq_textBox2.BackColor = System.Drawing.Color.Black;
            this.Cal_Freq_textBox2.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cal_Freq_textBox2.ForeColor = System.Drawing.Color.White;
            this.Cal_Freq_textBox2.Location = new System.Drawing.Point(318, 27);
            this.Cal_Freq_textBox2.Name = "Cal_Freq_textBox2";
            this.Cal_Freq_textBox2.ReadOnly = true;
            this.Cal_Freq_textBox2.Size = new System.Drawing.Size(131, 27);
            this.Cal_Freq_textBox2.TabIndex = 126;
            this.Cal_Freq_textBox2.Text = "30000000";
            this.Cal_Freq_textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Cal_Freq_textBox2.Visible = false;
            this.Cal_Freq_textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // IQ_TX_button
            // 
            this.IQ_TX_button.BackColor = System.Drawing.Color.Gainsboro;
            this.IQ_TX_button.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IQ_TX_button.ForeColor = System.Drawing.Color.Black;
            this.IQ_TX_button.Location = new System.Drawing.Point(599, 27);
            this.IQ_TX_button.Name = "IQ_TX_button";
            this.IQ_TX_button.Size = new System.Drawing.Size(99, 26);
            this.IQ_TX_button.TabIndex = 120;
            this.IQ_TX_button.Text = "TX IQ MODE";
            this.toolTip.SetToolTip(this.IQ_TX_button, "Set the Mode for Transmit I/Q Calibration");
            this.IQ_TX_button.UseVisualStyleBackColor = false;
            this.IQ_TX_button.Click += new System.EventHandler(this.IQ_TX_button_Click);
            // 
            // IQ_Reset_All_button2
            // 
            this.IQ_Reset_All_button2.BackColor = System.Drawing.Color.Red;
            this.IQ_Reset_All_button2.ForeColor = System.Drawing.Color.White;
            this.IQ_Reset_All_button2.Location = new System.Drawing.Point(466, 192);
            this.IQ_Reset_All_button2.Name = "IQ_Reset_All_button2";
            this.IQ_Reset_All_button2.Size = new System.Drawing.Size(110, 23);
            this.IQ_Reset_All_button2.TabIndex = 125;
            this.IQ_Reset_All_button2.Text = "DEFAULT IQ ";
            this.toolTip.SetToolTip(this.IQ_Reset_All_button2, "Resets the tranceiver I/Q values to default values.");
            this.IQ_Reset_All_button2.UseVisualStyleBackColor = false;
            this.IQ_Reset_All_button2.Visible = false;
            this.IQ_Reset_All_button2.Click += new System.EventHandler(this.IQ_Reset_All_button2_Click);
            // 
            // LeftResetbutton2
            // 
            this.LeftResetbutton2.BackColor = System.Drawing.Color.Black;
            this.LeftResetbutton2.ForeColor = System.Drawing.Color.White;
            this.LeftResetbutton2.Location = new System.Drawing.Point(197, 192);
            this.LeftResetbutton2.Name = "LeftResetbutton2";
            this.LeftResetbutton2.Size = new System.Drawing.Size(110, 23);
            this.LeftResetbutton2.TabIndex = 115;
            this.LeftResetbutton2.Text = "RESET IQ";
            this.toolTip.SetToolTip(this.LeftResetbutton2, "Reset the Slider to Zero");
            this.LeftResetbutton2.UseVisualStyleBackColor = false;
            this.LeftResetbutton2.Visible = false;
            this.LeftResetbutton2.Click += new System.EventHandler(this.LeftResetbutton2_Click_1);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.IQ12_radioButton2);
            this.groupBox2.Controls.Add(this.IQ160_radioButton10);
            this.groupBox2.Controls.Add(this.IQ80_radioButton9);
            this.groupBox2.Controls.Add(this.IQ60_radioButton8);
            this.groupBox2.Controls.Add(this.IQ40_radioButton7);
            this.groupBox2.Controls.Add(this.IQ30_radioButton6);
            this.groupBox2.Controls.Add(this.IQ20_radioButton5);
            this.groupBox2.Controls.Add(this.IQ17_radioButton4);
            this.groupBox2.Controls.Add(this.IQ15_radioButton3);
            this.groupBox2.Controls.Add(this.IQ10_radioButton1);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(71, 62);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(627, 42);
            this.groupBox2.TabIndex = 121;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "BAND SELECTION  ";
            this.toolTip.SetToolTip(this.groupBox2, "Band Selection for TX MODE I/Q Calibration");
            this.groupBox2.Visible = false;
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // IQ12_radioButton2
            // 
            this.IQ12_radioButton2.AutoSize = true;
            this.IQ12_radioButton2.ForeColor = System.Drawing.Color.White;
            this.IQ12_radioButton2.Location = new System.Drawing.Point(508, 16);
            this.IQ12_radioButton2.Name = "IQ12_radioButton2";
            this.IQ12_radioButton2.Size = new System.Drawing.Size(53, 20);
            this.IQ12_radioButton2.TabIndex = 107;
            this.IQ12_radioButton2.Text = "12M";
            this.IQ12_radioButton2.UseVisualStyleBackColor = true;
            this.IQ12_radioButton2.CheckedChanged += new System.EventHandler(this.IQ12_radioButton2_CheckedChanged_1);
            // 
            // IQ160_radioButton10
            // 
            this.IQ160_radioButton10.AutoSize = true;
            this.IQ160_radioButton10.ForeColor = System.Drawing.Color.White;
            this.IQ160_radioButton10.Location = new System.Drawing.Point(5, 16);
            this.IQ160_radioButton10.Name = "IQ160_radioButton10";
            this.IQ160_radioButton10.Size = new System.Drawing.Size(61, 20);
            this.IQ160_radioButton10.TabIndex = 99;
            this.IQ160_radioButton10.Text = "160M";
            this.IQ160_radioButton10.UseVisualStyleBackColor = true;
            this.IQ160_radioButton10.CheckedChanged += new System.EventHandler(this.IQ160_radioButton10_CheckedChanged_1);
            // 
            // IQ80_radioButton9
            // 
            this.IQ80_radioButton9.AutoSize = true;
            this.IQ80_radioButton9.ForeColor = System.Drawing.Color.White;
            this.IQ80_radioButton9.Location = new System.Drawing.Point(74, 16);
            this.IQ80_radioButton9.Name = "IQ80_radioButton9";
            this.IQ80_radioButton9.Size = new System.Drawing.Size(53, 20);
            this.IQ80_radioButton9.TabIndex = 100;
            this.IQ80_radioButton9.Text = "80M";
            this.IQ80_radioButton9.UseVisualStyleBackColor = true;
            this.IQ80_radioButton9.CheckedChanged += new System.EventHandler(this.IQ80_radioButton9_CheckedChanged_1);
            // 
            // IQ60_radioButton8
            // 
            this.IQ60_radioButton8.AutoSize = true;
            this.IQ60_radioButton8.ForeColor = System.Drawing.Color.White;
            this.IQ60_radioButton8.Location = new System.Drawing.Point(136, 16);
            this.IQ60_radioButton8.Name = "IQ60_radioButton8";
            this.IQ60_radioButton8.Size = new System.Drawing.Size(53, 20);
            this.IQ60_radioButton8.TabIndex = 101;
            this.IQ60_radioButton8.Text = "60M";
            this.IQ60_radioButton8.UseVisualStyleBackColor = true;
            this.IQ60_radioButton8.CheckedChanged += new System.EventHandler(this.IQ60_radioButton8_CheckedChanged_1);
            // 
            // IQ40_radioButton7
            // 
            this.IQ40_radioButton7.AutoSize = true;
            this.IQ40_radioButton7.ForeColor = System.Drawing.Color.White;
            this.IQ40_radioButton7.Location = new System.Drawing.Point(198, 16);
            this.IQ40_radioButton7.Name = "IQ40_radioButton7";
            this.IQ40_radioButton7.Size = new System.Drawing.Size(53, 20);
            this.IQ40_radioButton7.TabIndex = 102;
            this.IQ40_radioButton7.Text = "40M";
            this.IQ40_radioButton7.UseVisualStyleBackColor = true;
            this.IQ40_radioButton7.CheckedChanged += new System.EventHandler(this.IQ40_radioButton7_CheckedChanged_1);
            // 
            // IQ30_radioButton6
            // 
            this.IQ30_radioButton6.AutoSize = true;
            this.IQ30_radioButton6.ForeColor = System.Drawing.Color.White;
            this.IQ30_radioButton6.Location = new System.Drawing.Point(260, 16);
            this.IQ30_radioButton6.Name = "IQ30_radioButton6";
            this.IQ30_radioButton6.Size = new System.Drawing.Size(53, 20);
            this.IQ30_radioButton6.TabIndex = 103;
            this.IQ30_radioButton6.Text = "30M";
            this.IQ30_radioButton6.UseVisualStyleBackColor = true;
            this.IQ30_radioButton6.CheckedChanged += new System.EventHandler(this.IQ30_radioButton6_CheckedChanged_1);
            // 
            // IQ20_radioButton5
            // 
            this.IQ20_radioButton5.AutoSize = true;
            this.IQ20_radioButton5.ForeColor = System.Drawing.Color.White;
            this.IQ20_radioButton5.Location = new System.Drawing.Point(322, 16);
            this.IQ20_radioButton5.Name = "IQ20_radioButton5";
            this.IQ20_radioButton5.Size = new System.Drawing.Size(53, 20);
            this.IQ20_radioButton5.TabIndex = 104;
            this.IQ20_radioButton5.Text = "20M";
            this.IQ20_radioButton5.UseVisualStyleBackColor = true;
            this.IQ20_radioButton5.CheckedChanged += new System.EventHandler(this.IQ20_radioButton5_CheckedChanged_1);
            // 
            // IQ17_radioButton4
            // 
            this.IQ17_radioButton4.AutoSize = true;
            this.IQ17_radioButton4.ForeColor = System.Drawing.Color.White;
            this.IQ17_radioButton4.Location = new System.Drawing.Point(384, 16);
            this.IQ17_radioButton4.Name = "IQ17_radioButton4";
            this.IQ17_radioButton4.Size = new System.Drawing.Size(53, 20);
            this.IQ17_radioButton4.TabIndex = 105;
            this.IQ17_radioButton4.Text = "17M";
            this.IQ17_radioButton4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.IQ17_radioButton4.UseVisualStyleBackColor = true;
            this.IQ17_radioButton4.CheckedChanged += new System.EventHandler(this.IQ17_radioButton4_CheckedChanged_1);
            // 
            // IQ15_radioButton3
            // 
            this.IQ15_radioButton3.AutoSize = true;
            this.IQ15_radioButton3.ForeColor = System.Drawing.Color.White;
            this.IQ15_radioButton3.Location = new System.Drawing.Point(446, 16);
            this.IQ15_radioButton3.Name = "IQ15_radioButton3";
            this.IQ15_radioButton3.Size = new System.Drawing.Size(53, 20);
            this.IQ15_radioButton3.TabIndex = 106;
            this.IQ15_radioButton3.Text = "15M";
            this.IQ15_radioButton3.UseVisualStyleBackColor = true;
            this.IQ15_radioButton3.CheckedChanged += new System.EventHandler(this.IQ15_radioButton3_CheckedChanged_1);
            // 
            // IQ10_radioButton1
            // 
            this.IQ10_radioButton1.AutoSize = true;
            this.IQ10_radioButton1.ForeColor = System.Drawing.Color.White;
            this.IQ10_radioButton1.Location = new System.Drawing.Point(570, 16);
            this.IQ10_radioButton1.Name = "IQ10_radioButton1";
            this.IQ10_radioButton1.Size = new System.Drawing.Size(53, 20);
            this.IQ10_radioButton1.TabIndex = 108;
            this.IQ10_radioButton1.Text = "10M";
            this.IQ10_radioButton1.UseVisualStyleBackColor = true;
            this.IQ10_radioButton1.CheckedChanged += new System.EventHandler(this.IQ10_radioButton1_CheckedChanged_1);
            // 
            // IQ_RX_button
            // 
            this.IQ_RX_button.BackColor = System.Drawing.Color.Gainsboro;
            this.IQ_RX_button.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IQ_RX_button.ForeColor = System.Drawing.Color.Black;
            this.IQ_RX_button.Location = new System.Drawing.Point(74, 28);
            this.IQ_RX_button.Name = "IQ_RX_button";
            this.IQ_RX_button.Size = new System.Drawing.Size(98, 26);
            this.IQ_RX_button.TabIndex = 119;
            this.IQ_RX_button.Text = "RX IQ MODE";
            this.toolTip.SetToolTip(this.IQ_RX_button, "Set the Mode to Receive I/Q Calibration.\r\nA band must be selected on the Main Tab" +
        " before \r\nselecting this mode.  Return to the Main Tab to \r\nselect another band " +
        "for calibration.");
            this.IQ_RX_button.UseVisualStyleBackColor = false;
            this.IQ_RX_button.Click += new System.EventHandler(this.IQ_RX_button_Click);
            // 
            // LefthScrollBar1
            // 
            this.LefthScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.LefthScrollBar1.Location = new System.Drawing.Point(147, 164);
            this.LefthScrollBar1.Maximum = 209;
            this.LefthScrollBar1.Minimum = -200;
            this.LefthScrollBar1.Name = "LefthScrollBar1";
            this.LefthScrollBar1.Size = new System.Drawing.Size(454, 15);
            this.LefthScrollBar1.TabIndex = 114;
            this.toolTip.SetToolTip(this.LefthScrollBar1, "Adjust the IQ balance for minimum RX or TX image. \r\nClick in the scroll bar for c" +
        "ourse adjustment (10).\r\nUse Mouse Wheel for fine adjustment (1).");
            this.LefthScrollBar1.Visible = false;
            this.LefthScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.LefthScrollBar1_Scroll_1);
            // 
            // IQ_Commit_button2
            // 
            this.IQ_Commit_button2.BackColor = System.Drawing.Color.Gainsboro;
            this.IQ_Commit_button2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IQ_Commit_button2.ForeColor = System.Drawing.Color.Black;
            this.IQ_Commit_button2.Location = new System.Drawing.Point(548, 131);
            this.IQ_Commit_button2.Name = "IQ_Commit_button2";
            this.IQ_Commit_button2.Size = new System.Drawing.Size(81, 26);
            this.IQ_Commit_button2.TabIndex = 113;
            this.IQ_Commit_button2.Text = "APPLY";
            this.toolTip.SetToolTip(this.IQ_Commit_button2, "Records the current I/Q Value");
            this.IQ_Commit_button2.UseVisualStyleBackColor = false;
            this.IQ_Commit_button2.Visible = false;
            this.IQ_Commit_button2.Click += new System.EventHandler(this.IQ_Commit_button2_Click_1);
            // 
            // IQLefttextBox2
            // 
            this.IQLefttextBox2.BackColor = System.Drawing.Color.Black;
            this.IQLefttextBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IQLefttextBox2.ForeColor = System.Drawing.Color.White;
            this.IQLefttextBox2.Location = new System.Drawing.Point(358, 192);
            this.IQLefttextBox2.Name = "IQLefttextBox2";
            this.IQLefttextBox2.ReadOnly = true;
            this.IQLefttextBox2.Size = new System.Drawing.Size(53, 21);
            this.IQLefttextBox2.TabIndex = 110;
            this.IQLefttextBox2.Text = "0";
            this.IQLefttextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.IQLefttextBox2.Visible = false;
            this.IQLefttextBox2.TextChanged += new System.EventHandler(this.IQLefttextBox2_TextChanged_1);
            // 
            // IQ_Tune_button2
            // 
            this.IQ_Tune_button2.BackColor = System.Drawing.Color.Gainsboro;
            this.IQ_Tune_button2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IQ_Tune_button2.ForeColor = System.Drawing.Color.Black;
            this.IQ_Tune_button2.Location = new System.Drawing.Point(144, 131);
            this.IQ_Tune_button2.Name = "IQ_Tune_button2";
            this.IQ_Tune_button2.Size = new System.Drawing.Size(81, 26);
            this.IQ_Tune_button2.TabIndex = 112;
            this.IQ_Tune_button2.Text = "TUNE OFF";
            this.toolTip.SetToolTip(this.IQ_Tune_button2, "Set the tranceiver to Transmite Mode");
            this.IQ_Tune_button2.UseVisualStyleBackColor = false;
            this.IQ_Tune_button2.Visible = false;
            this.IQ_Tune_button2.Click += new System.EventHandler(this.IQ_Tune_button2_Click_1);
            // 
            // Freq_Cal_groupBox4
            // 
            this.Freq_Cal_groupBox4.BackColor = System.Drawing.Color.Transparent;
            this.Freq_Cal_groupBox4.Controls.Add(this.Freq_Cal_label59);
            this.Freq_Cal_groupBox4.Controls.Add(this.Freq_CAl_Progress_Lable);
            this.Freq_Cal_groupBox4.Controls.Add(this.label57);
            this.Freq_Cal_groupBox4.Controls.Add(this.Freq_Check_Button);
            this.Freq_Cal_groupBox4.Controls.Add(this.Standard_Carrier_listBox1);
            this.Freq_Cal_groupBox4.Controls.Add(this.Calibration_progressBar1);
            this.Freq_Cal_groupBox4.Controls.Add(this.Freq_Cal_checkBox3);
            this.Freq_Cal_groupBox4.Controls.Add(this.Freq_Cal_Reset_button4);
            this.Freq_Cal_groupBox4.Controls.Add(this.calibratebutton1);
            this.Freq_Cal_groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.Freq_Cal_groupBox4.ForeColor = System.Drawing.Color.White;
            this.Freq_Cal_groupBox4.Location = new System.Drawing.Point(10, 10);
            this.Freq_Cal_groupBox4.Name = "Freq_Cal_groupBox4";
            this.Freq_Cal_groupBox4.Size = new System.Drawing.Size(772, 159);
            this.Freq_Cal_groupBox4.TabIndex = 127;
            this.Freq_Cal_groupBox4.TabStop = false;
            this.Freq_Cal_groupBox4.Text = "FREQUENCY CALIBRATION";
            this.Freq_Cal_groupBox4.Enter += new System.EventHandler(this.Freq_Cal_groupBox4_Enter);
            // 
            // Freq_Cal_label59
            // 
            this.Freq_Cal_label59.BackColor = System.Drawing.Color.SlateGray;
            this.Freq_Cal_label59.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Freq_Cal_label59.Enabled = false;
            this.Freq_Cal_label59.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Freq_Cal_label59.ForeColor = System.Drawing.Color.White;
            this.Freq_Cal_label59.Location = new System.Drawing.Point(256, 71);
            this.Freq_Cal_label59.Name = "Freq_Cal_label59";
            this.Freq_Cal_label59.Size = new System.Drawing.Size(261, 85);
            this.Freq_Cal_label59.TabIndex = 152;
            this.Freq_Cal_label59.Text = "INITIALIZING\r\nPLEASE WAIT";
            this.Freq_Cal_label59.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Freq_Cal_label59.Visible = false;
            this.Freq_Cal_label59.Click += new System.EventHandler(this.Freq_Cal_label59_Click);
            // 
            // Freq_CAl_Progress_Lable
            // 
            this.Freq_CAl_Progress_Lable.AutoSize = true;
            this.Freq_CAl_Progress_Lable.BackColor = System.Drawing.Color.Gainsboro;
            this.Freq_CAl_Progress_Lable.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Freq_CAl_Progress_Lable.ForeColor = System.Drawing.Color.Black;
            this.Freq_CAl_Progress_Lable.Location = new System.Drawing.Point(367, 118);
            this.Freq_CAl_Progress_Lable.Name = "Freq_CAl_Progress_Lable";
            this.Freq_CAl_Progress_Lable.Size = new System.Drawing.Size(26, 13);
            this.Freq_CAl_Progress_Lable.TabIndex = 159;
            this.Freq_CAl_Progress_Lable.Text = "0%";
            this.Freq_CAl_Progress_Lable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Freq_CAl_Progress_Lable.Visible = false;
            this.Freq_CAl_Progress_Lable.Click += new System.EventHandler(this.Freq_CAl_Progress_Lable_Click);
            // 
            // label57
            // 
            this.label57.BackColor = System.Drawing.Color.Gainsboro;
            this.label57.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label57.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label57.ForeColor = System.Drawing.Color.Black;
            this.label57.Location = new System.Drawing.Point(286, 18);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(200, 23);
            this.label57.TabIndex = 141;
            this.label57.Text = "STANDARD CARRIER";
            this.label57.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label57.Click += new System.EventHandler(this.label57_Click);
            // 
            // Freq_Check_Button
            // 
            this.Freq_Check_Button.BackColor = System.Drawing.Color.Gainsboro;
            this.Freq_Check_Button.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Freq_Check_Button.ForeColor = System.Drawing.Color.Black;
            this.Freq_Check_Button.Location = new System.Drawing.Point(431, 133);
            this.Freq_Check_Button.Name = "Freq_Check_Button";
            this.Freq_Check_Button.Size = new System.Drawing.Size(75, 23);
            this.Freq_Check_Button.TabIndex = 122;
            this.Freq_Check_Button.Text = "CHECK";
            this.toolTip.SetToolTip(this.Freq_Check_Button, "Check the current calibration (Temperature compensation\r\nis turned off)");
            this.Freq_Check_Button.UseVisualStyleBackColor = false;
            this.Freq_Check_Button.Visible = false;
            this.Freq_Check_Button.Click += new System.EventHandler(this.Freq_Check_Button_Click);
            // 
            // Standard_Carrier_listBox1
            // 
            this.Standard_Carrier_listBox1.BackColor = System.Drawing.Color.Black;
            this.Standard_Carrier_listBox1.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Standard_Carrier_listBox1.ForeColor = System.Drawing.Color.White;
            this.Standard_Carrier_listBox1.FormattingEnabled = true;
            this.Standard_Carrier_listBox1.ItemHeight = 18;
            this.Standard_Carrier_listBox1.Items.AddRange(new object[] {
            "2.500.000",
            "3.330.000",
            "5.000.000",
            "7.850.000",
            "9.996.000",
            "10.000.000",
            "14.670.000",
            "15.000.000",
            "20.000.000",
            "30.000.000"});
            this.Standard_Carrier_listBox1.Location = new System.Drawing.Point(322, 46);
            this.Standard_Carrier_listBox1.Name = "Standard_Carrier_listBox1";
            this.Standard_Carrier_listBox1.Size = new System.Drawing.Size(129, 22);
            this.Standard_Carrier_listBox1.TabIndex = 120;
            this.Standard_Carrier_listBox1.SelectedIndexChanged += new System.EventHandler(this.Standard_Carrier_listBox1_SelectedIndexChanged);
            // 
            // Calibration_progressBar1
            // 
            this.Calibration_progressBar1.BackColor = System.Drawing.Color.Gainsboro;
            this.Calibration_progressBar1.ForeColor = System.Drawing.Color.Red;
            this.Calibration_progressBar1.Location = new System.Drawing.Point(321, 118);
            this.Calibration_progressBar1.Name = "Calibration_progressBar1";
            this.Calibration_progressBar1.Size = new System.Drawing.Size(130, 13);
            this.Calibration_progressBar1.Step = 1;
            this.Calibration_progressBar1.TabIndex = 119;
            this.Calibration_progressBar1.Visible = false;
            this.Calibration_progressBar1.Click += new System.EventHandler(this.Calibration_progressBar1_Click);
            // 
            // Freq_Cal_checkBox3
            // 
            this.Freq_Cal_checkBox3.AutoSize = true;
            this.Freq_Cal_checkBox3.BackColor = System.Drawing.Color.Gainsboro;
            this.Freq_Cal_checkBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Freq_Cal_checkBox3.ForeColor = System.Drawing.Color.Black;
            this.Freq_Cal_checkBox3.Location = new System.Drawing.Point(353, 137);
            this.Freq_Cal_checkBox3.Name = "Freq_Cal_checkBox3";
            this.Freq_Cal_checkBox3.Size = new System.Drawing.Size(67, 17);
            this.Freq_Cal_checkBox3.TabIndex = 157;
            this.Freq_Cal_checkBox3.Text = "LOOSE";
            this.toolTip.SetToolTip(this.Freq_Cal_checkBox3, "Configures the calibration routine for a Loose calibration.\r\nThis lowers the requ" +
        "ired signal level for a successful calibration.\r\n\r\nWARNING: The resulting calibr" +
        "ation will not be as accurate.");
            this.Freq_Cal_checkBox3.UseVisualStyleBackColor = false;
            this.Freq_Cal_checkBox3.Visible = false;
            this.Freq_Cal_checkBox3.CheckedChanged += new System.EventHandler(this.Freq_Cal_checkBox3_CheckedChanged);
            // 
            // Freq_Cal_Reset_button4
            // 
            this.Freq_Cal_Reset_button4.BackColor = System.Drawing.Color.Gainsboro;
            this.Freq_Cal_Reset_button4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Freq_Cal_Reset_button4.ForeColor = System.Drawing.Color.Black;
            this.Freq_Cal_Reset_button4.Location = new System.Drawing.Point(269, 133);
            this.Freq_Cal_Reset_button4.Name = "Freq_Cal_Reset_button4";
            this.Freq_Cal_Reset_button4.Size = new System.Drawing.Size(75, 23);
            this.Freq_Cal_Reset_button4.TabIndex = 156;
            this.Freq_Cal_Reset_button4.Text = "RESET";
            this.toolTip.SetToolTip(this.Freq_Cal_Reset_button4, "This resets the frequency calibration to default values.");
            this.Freq_Cal_Reset_button4.UseVisualStyleBackColor = false;
            this.Freq_Cal_Reset_button4.Visible = false;
            this.Freq_Cal_Reset_button4.Click += new System.EventHandler(this.Freq_Cal_Reset_button4_Click);
            // 
            // calibratebutton1
            // 
            this.calibratebutton1.BackColor = System.Drawing.Color.Gainsboro;
            this.calibratebutton1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.calibratebutton1.ForeColor = System.Drawing.Color.Black;
            this.calibratebutton1.Location = new System.Drawing.Point(321, 74);
            this.calibratebutton1.Name = "calibratebutton1";
            this.calibratebutton1.Size = new System.Drawing.Size(130, 40);
            this.calibratebutton1.TabIndex = 31;
            this.calibratebutton1.Text = "CALIBRATE";
            this.calibratebutton1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip.SetToolTip(this.calibratebutton1, "Click to Calibrate.  Please wait for the Calibraton to finish.");
            this.calibratebutton1.UseVisualStyleBackColor = false;
            this.calibratebutton1.Visible = false;
            this.calibratebutton1.Click += new System.EventHandler(this.calibratebutton1_Click_1);
            // 
            // powertabPage1
            // 
            this.powertabPage1.BackColor = System.Drawing.Color.Black;
            this.powertabPage1.Controls.Add(this.XCRV_Power_Display_label33);
            this.powertabPage1.Controls.Add(this.B10radioButton);
            this.powertabPage1.Controls.Add(this.B12radioButton);
            this.powertabPage1.Controls.Add(this.B15radioButton);
            this.powertabPage1.Controls.Add(this.B17radioButton);
            this.powertabPage1.Controls.Add(this.B20radioButton);
            this.powertabPage1.Controls.Add(this.B30radioButton);
            this.powertabPage1.Controls.Add(this.B40radioButton);
            this.powertabPage1.Controls.Add(this.B60radioButton);
            this.powertabPage1.Controls.Add(this.B80radioButton);
            this.powertabPage1.Controls.Add(this.B160radioButton);
            this.powertabPage1.Controls.Add(this.power_slider_reset_button1);
            this.powertabPage1.Controls.Add(this.powerrestorebutton2);
            this.powertabPage1.Controls.Add(this.powertunebutton1);
            this.powertabPage1.Controls.Add(this.powerhScrollBar1);
            this.powertabPage1.Controls.Add(this.powerlabel14);
            this.powertabPage1.Controls.Add(this.label13);
            this.powertabPage1.Controls.Add(this.label10);
            this.powertabPage1.ForeColor = System.Drawing.Color.Black;
            this.powertabPage1.Location = new System.Drawing.Point(4, 25);
            this.powertabPage1.Name = "powertabPage1";
            this.powertabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.powertabPage1.Size = new System.Drawing.Size(792, 451);
            this.powertabPage1.TabIndex = 3;
            this.powertabPage1.Text = "XCVR  Pwr";
            this.powertabPage1.ToolTipText = "Power Output Calibration";
            this.powertabPage1.Click += new System.EventHandler(this.powertabPage1_Click);
            this.powertabPage1.Enter += new System.EventHandler(this.powertabPage1_Enter);
            this.powertabPage1.Leave += new System.EventHandler(this.powertabPage1_Leave);
            // 
            // XCRV_Power_Display_label33
            // 
            this.XCRV_Power_Display_label33.BackColor = System.Drawing.Color.Gainsboro;
            this.XCRV_Power_Display_label33.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.XCRV_Power_Display_label33.ForeColor = System.Drawing.Color.Black;
            this.XCRV_Power_Display_label33.Location = new System.Drawing.Point(293, 247);
            this.XCRV_Power_Display_label33.Name = "XCRV_Power_Display_label33";
            this.XCRV_Power_Display_label33.Size = new System.Drawing.Size(196, 27);
            this.XCRV_Power_Display_label33.TabIndex = 190;
            this.XCRV_Power_Display_label33.Text = "XCVR POWER: ";
            this.XCRV_Power_Display_label33.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.XCRV_Power_Display_label33.Click += new System.EventHandler(this.XCRV_Power_Display_label33_Click);
            // 
            // B10radioButton
            // 
            this.B10radioButton.AutoSize = true;
            this.B10radioButton.BackColor = System.Drawing.Color.Transparent;
            this.B10radioButton.ForeColor = System.Drawing.Color.White;
            this.B10radioButton.Location = new System.Drawing.Point(646, 157);
            this.B10radioButton.Name = "B10radioButton";
            this.B10radioButton.Size = new System.Drawing.Size(53, 20);
            this.B10radioButton.TabIndex = 51;
            this.B10radioButton.Text = "10M";
            this.B10radioButton.UseVisualStyleBackColor = false;
            this.B10radioButton.CheckedChanged += new System.EventHandler(this.B10radioButton_CheckedChanged);
            // 
            // B12radioButton
            // 
            this.B12radioButton.AutoSize = true;
            this.B12radioButton.BackColor = System.Drawing.Color.Transparent;
            this.B12radioButton.ForeColor = System.Drawing.Color.White;
            this.B12radioButton.Location = new System.Drawing.Point(584, 157);
            this.B12radioButton.Name = "B12radioButton";
            this.B12radioButton.Size = new System.Drawing.Size(53, 20);
            this.B12radioButton.TabIndex = 50;
            this.B12radioButton.Text = "12M";
            this.B12radioButton.UseVisualStyleBackColor = false;
            this.B12radioButton.CheckedChanged += new System.EventHandler(this.B12radioButton_CheckedChanged);
            // 
            // B15radioButton
            // 
            this.B15radioButton.AutoSize = true;
            this.B15radioButton.BackColor = System.Drawing.Color.Transparent;
            this.B15radioButton.ForeColor = System.Drawing.Color.White;
            this.B15radioButton.Location = new System.Drawing.Point(522, 157);
            this.B15radioButton.Name = "B15radioButton";
            this.B15radioButton.Size = new System.Drawing.Size(53, 20);
            this.B15radioButton.TabIndex = 49;
            this.B15radioButton.Text = "15M";
            this.B15radioButton.UseVisualStyleBackColor = false;
            this.B15radioButton.CheckedChanged += new System.EventHandler(this.B15radioButton_CheckedChanged);
            // 
            // B17radioButton
            // 
            this.B17radioButton.AutoSize = true;
            this.B17radioButton.BackColor = System.Drawing.Color.Transparent;
            this.B17radioButton.ForeColor = System.Drawing.Color.White;
            this.B17radioButton.Location = new System.Drawing.Point(460, 157);
            this.B17radioButton.Name = "B17radioButton";
            this.B17radioButton.Size = new System.Drawing.Size(53, 20);
            this.B17radioButton.TabIndex = 48;
            this.B17radioButton.Text = "17M";
            this.B17radioButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.B17radioButton.UseVisualStyleBackColor = false;
            this.B17radioButton.CheckedChanged += new System.EventHandler(this.B17radioButton_CheckedChanged);
            // 
            // B20radioButton
            // 
            this.B20radioButton.AutoSize = true;
            this.B20radioButton.BackColor = System.Drawing.Color.Transparent;
            this.B20radioButton.ForeColor = System.Drawing.Color.White;
            this.B20radioButton.Location = new System.Drawing.Point(398, 157);
            this.B20radioButton.Name = "B20radioButton";
            this.B20radioButton.Size = new System.Drawing.Size(53, 20);
            this.B20radioButton.TabIndex = 47;
            this.B20radioButton.Text = "20M";
            this.B20radioButton.UseVisualStyleBackColor = false;
            this.B20radioButton.CheckedChanged += new System.EventHandler(this.B20radioButton_CheckedChanged);
            // 
            // B30radioButton
            // 
            this.B30radioButton.AutoSize = true;
            this.B30radioButton.BackColor = System.Drawing.Color.Transparent;
            this.B30radioButton.ForeColor = System.Drawing.Color.White;
            this.B30radioButton.Location = new System.Drawing.Point(336, 157);
            this.B30radioButton.Name = "B30radioButton";
            this.B30radioButton.Size = new System.Drawing.Size(53, 20);
            this.B30radioButton.TabIndex = 46;
            this.B30radioButton.Text = "30M";
            this.B30radioButton.UseVisualStyleBackColor = false;
            this.B30radioButton.CheckedChanged += new System.EventHandler(this.B30radioButton_CheckedChanged);
            // 
            // B40radioButton
            // 
            this.B40radioButton.AutoSize = true;
            this.B40radioButton.BackColor = System.Drawing.Color.Transparent;
            this.B40radioButton.ForeColor = System.Drawing.Color.White;
            this.B40radioButton.Location = new System.Drawing.Point(274, 157);
            this.B40radioButton.Name = "B40radioButton";
            this.B40radioButton.Size = new System.Drawing.Size(53, 20);
            this.B40radioButton.TabIndex = 45;
            this.B40radioButton.Text = "40M";
            this.B40radioButton.UseVisualStyleBackColor = false;
            this.B40radioButton.CheckedChanged += new System.EventHandler(this.B40radioButton_CheckedChanged);
            // 
            // B60radioButton
            // 
            this.B60radioButton.AutoSize = true;
            this.B60radioButton.BackColor = System.Drawing.Color.Transparent;
            this.B60radioButton.ForeColor = System.Drawing.Color.White;
            this.B60radioButton.Location = new System.Drawing.Point(212, 157);
            this.B60radioButton.Name = "B60radioButton";
            this.B60radioButton.Size = new System.Drawing.Size(53, 20);
            this.B60radioButton.TabIndex = 44;
            this.B60radioButton.Text = "60M";
            this.B60radioButton.UseVisualStyleBackColor = false;
            this.B60radioButton.CheckedChanged += new System.EventHandler(this.B60radioButton_CheckedChanged);
            // 
            // B80radioButton
            // 
            this.B80radioButton.AutoSize = true;
            this.B80radioButton.BackColor = System.Drawing.Color.Transparent;
            this.B80radioButton.ForeColor = System.Drawing.Color.White;
            this.B80radioButton.Location = new System.Drawing.Point(150, 157);
            this.B80radioButton.Name = "B80radioButton";
            this.B80radioButton.Size = new System.Drawing.Size(53, 20);
            this.B80radioButton.TabIndex = 43;
            this.B80radioButton.Text = "80M";
            this.B80radioButton.UseVisualStyleBackColor = false;
            this.B80radioButton.CheckedChanged += new System.EventHandler(this.B80radioButton_CheckedChanged);
            // 
            // B160radioButton
            // 
            this.B160radioButton.AutoSize = true;
            this.B160radioButton.BackColor = System.Drawing.Color.Transparent;
            this.B160radioButton.ForeColor = System.Drawing.Color.White;
            this.B160radioButton.Location = new System.Drawing.Point(81, 157);
            this.B160radioButton.Name = "B160radioButton";
            this.B160radioButton.Size = new System.Drawing.Size(61, 20);
            this.B160radioButton.TabIndex = 42;
            this.B160radioButton.Text = "160M";
            this.B160radioButton.UseVisualStyleBackColor = false;
            this.B160radioButton.CheckedChanged += new System.EventHandler(this.B160radioButton_CheckedChanged);
            // 
            // power_slider_reset_button1
            // 
            this.power_slider_reset_button1.BackColor = System.Drawing.Color.Gainsboro;
            this.power_slider_reset_button1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.power_slider_reset_button1.Location = new System.Drawing.Point(235, 320);
            this.power_slider_reset_button1.Name = "power_slider_reset_button1";
            this.power_slider_reset_button1.Size = new System.Drawing.Size(152, 27);
            this.power_slider_reset_button1.TabIndex = 33;
            this.power_slider_reset_button1.Text = "RESET SLIDER";
            this.power_slider_reset_button1.UseVisualStyleBackColor = false;
            this.power_slider_reset_button1.Click += new System.EventHandler(this.power_slider_reset_button1_Click);
            // 
            // powerrestorebutton2
            // 
            this.powerrestorebutton2.BackColor = System.Drawing.Color.Gainsboro;
            this.powerrestorebutton2.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.powerrestorebutton2.Location = new System.Drawing.Point(406, 320);
            this.powerrestorebutton2.Name = "powerrestorebutton2";
            this.powerrestorebutton2.Size = new System.Drawing.Size(152, 27);
            this.powerrestorebutton2.TabIndex = 31;
            this.powerrestorebutton2.Text = "FACTORY DEFAULTS";
            this.toolTip.SetToolTip(this.powerrestorebutton2, "Restore the transceiver output power to factory defaults.");
            this.powerrestorebutton2.UseVisualStyleBackColor = false;
            this.powerrestorebutton2.Click += new System.EventHandler(this.powerrestorebutton2_Click);
            // 
            // powertunebutton1
            // 
            this.powertunebutton1.BackColor = System.Drawing.Color.Gainsboro;
            this.powertunebutton1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.powertunebutton1.Location = new System.Drawing.Point(513, 249);
            this.powertunebutton1.Name = "powertunebutton1";
            this.powertunebutton1.Size = new System.Drawing.Size(98, 27);
            this.powertunebutton1.TabIndex = 30;
            this.powertunebutton1.Text = "TUNE";
            this.toolTip.SetToolTip(this.powertunebutton1, "Turns on TX in Tune mode.");
            this.powertunebutton1.UseVisualStyleBackColor = false;
            this.powertunebutton1.Click += new System.EventHandler(this.powertunebutton1_Click);
            // 
            // powerhScrollBar1
            // 
            this.powerhScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.powerhScrollBar1.LargeChange = 5;
            this.powerhScrollBar1.Location = new System.Drawing.Point(170, 292);
            this.powerhScrollBar1.Maximum = 104;
            this.powerhScrollBar1.Minimum = 2;
            this.powerhScrollBar1.Name = "powerhScrollBar1";
            this.powerhScrollBar1.Size = new System.Drawing.Size(452, 15);
            this.powerhScrollBar1.TabIndex = 27;
            this.toolTip.SetToolTip(this.powerhScrollBar1, "Set the transceiver output power.  I\r\nIt is relative value from 0 to 100.");
            this.powerhScrollBar1.Value = 50;
            this.powerhScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.powerhScrollBar1_Scroll);
            // 
            // powerlabel14
            // 
            this.powerlabel14.BackColor = System.Drawing.Color.Gainsboro;
            this.powerlabel14.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.powerlabel14.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.powerlabel14.Location = new System.Drawing.Point(182, 251);
            this.powerlabel14.Name = "powerlabel14";
            this.powerlabel14.Size = new System.Drawing.Size(87, 23);
            this.powerlabel14.TabIndex = 26;
            this.powerlabel14.Text = " NO VALUE";
            this.powerlabel14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.powerlabel14, "A relative value of Transceiver Output Power.");
            this.powerlabel14.Click += new System.EventHandler(this.powerlabel14_Click);
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Arial Black", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.Red;
            this.label13.Location = new System.Drawing.Point(300, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(199, 37);
            this.label13.TabIndex = 25;
            this.label13.Text = "! WARNING !";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label13.Click += new System.EventHandler(this.label13_Click);
            // 
            // label10
            // 
            this.label10.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label10.Font = new System.Drawing.Font("Arial Black", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Red;
            this.label10.Location = new System.Drawing.Point(150, 37);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(478, 117);
            this.label10.TabIndex = 14;
            this.label10.Text = resources.GetString("label10.Text");
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label10.Click += new System.EventHandler(this.label10_Click_1);
            // 
            // Audio_tabPage1
            // 
            this.Audio_tabPage1.BackColor = System.Drawing.Color.Black;
            this.Audio_tabPage1.Controls.Add(this.groupBox1);
            this.Audio_tabPage1.Location = new System.Drawing.Point(4, 25);
            this.Audio_tabPage1.Name = "Audio_tabPage1";
            this.Audio_tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.Audio_tabPage1.Size = new System.Drawing.Size(792, 451);
            this.Audio_tabPage1.TabIndex = 12;
            this.Audio_tabPage1.Text = "CW";
            this.Audio_tabPage1.ToolTipText = "Audio and CW Controls";
            this.Audio_tabPage1.Click += new System.EventHandler(this.Audio_tabPage1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Side_Tone_label32);
            this.groupBox1.Controls.Add(this.Side_Tone_Volume_hScrollBar1);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.ritoffsetlistBox1);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.ritlistBox1);
            this.groupBox1.Controls.Add(this.CW_Hold_numericUpDown2);
            this.groupBox1.Controls.Add(this.CW_Lag_numericUpDown2);
            this.groupBox1.Controls.Add(this.numericUpDown1);
            this.groupBox1.Controls.Add(this.CW_Paddle_listBox1);
            this.groupBox1.Controls.Add(this.CW_Weight_listBox1);
            this.groupBox1.Controls.Add(this.CW_Space_listBox1);
            this.groupBox1.Controls.Add(this.CW_Type_listBox1);
            this.groupBox1.Controls.Add(this.CW_Mode_listBox1);
            this.groupBox1.Controls.Add(this.label61);
            this.groupBox1.Controls.Add(this.label60);
            this.groupBox1.Controls.Add(this.label59);
            this.groupBox1.Controls.Add(this.label58);
            this.groupBox1.Controls.Add(this.label45);
            this.groupBox1.Controls.Add(this.label43);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.CW_Pitch_listBox1);
            this.groupBox1.Controls.Add(this.label35);
            this.groupBox1.Controls.Add(this.semicheckBox2);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(16, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(760, 332);
            this.groupBox1.TabIndex = 145;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CW Controls";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // Side_Tone_label32
            // 
            this.Side_Tone_label32.BackColor = System.Drawing.Color.Gainsboro;
            this.Side_Tone_label32.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Side_Tone_label32.ForeColor = System.Drawing.Color.Black;
            this.Side_Tone_label32.Location = new System.Drawing.Point(266, 105);
            this.Side_Tone_label32.Name = "Side_Tone_label32";
            this.Side_Tone_label32.Size = new System.Drawing.Size(229, 23);
            this.Side_Tone_label32.TabIndex = 161;
            this.Side_Tone_label32.Text = "SIDE TONE VOLUME";
            this.Side_Tone_label32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Side_Tone_Volume_hScrollBar1
            // 
            this.Side_Tone_Volume_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.Side_Tone_Volume_hScrollBar1.Location = new System.Drawing.Point(244, 141);
            this.Side_Tone_Volume_hScrollBar1.Maximum = 108;
            this.Side_Tone_Volume_hScrollBar1.Name = "Side_Tone_Volume_hScrollBar1";
            this.Side_Tone_Volume_hScrollBar1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Side_Tone_Volume_hScrollBar1.Size = new System.Drawing.Size(269, 19);
            this.Side_Tone_Volume_hScrollBar1.TabIndex = 160;
            this.Side_Tone_Volume_hScrollBar1.TabStop = true;
            this.Side_Tone_Volume_hScrollBar1.Value = 30;
            this.Side_Tone_Volume_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Side_Tone_Volume_hScrollBar1_Scroll);
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.Color.Gainsboro;
            this.label17.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.Black;
            this.label17.Location = new System.Drawing.Point(441, 247);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(130, 28);
            this.label17.TabIndex = 159;
            this.label17.Text = "RIT LIMITS";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ritoffsetlistBox1
            // 
            this.ritoffsetlistBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.ritoffsetlistBox1.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ritoffsetlistBox1.ForeColor = System.Drawing.Color.Black;
            this.ritoffsetlistBox1.FormattingEnabled = true;
            this.ritoffsetlistBox1.ItemHeight = 23;
            this.ritoffsetlistBox1.Items.AddRange(new object[] {
            "+/- 100",
            "+/- 500",
            "+/- 1000"});
            this.ritoffsetlistBox1.Location = new System.Drawing.Point(441, 287);
            this.ritoffsetlistBox1.Name = "ritoffsetlistBox1";
            this.ritoffsetlistBox1.ScrollAlwaysVisible = true;
            this.ritoffsetlistBox1.Size = new System.Drawing.Size(116, 27);
            this.ritoffsetlistBox1.TabIndex = 158;
            this.ritoffsetlistBox1.SelectedIndexChanged += new System.EventHandler(this.ritoffsetlistBox1_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Gainsboro;
            this.label7.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(219, 248);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 27);
            this.label7.TabIndex = 157;
            this.label7.Text = "RIT STEP";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ritlistBox1
            // 
            this.ritlistBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.ritlistBox1.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ritlistBox1.ForeColor = System.Drawing.Color.Black;
            this.ritlistBox1.FormattingEnabled = true;
            this.ritlistBox1.ItemHeight = 23;
            this.ritlistBox1.Items.AddRange(new object[] {
            "10",
            "20",
            "30",
            "40",
            "50"});
            this.ritlistBox1.Location = new System.Drawing.Point(268, 287);
            this.ritlistBox1.Name = "ritlistBox1";
            this.ritlistBox1.ScrollAlwaysVisible = true;
            this.ritlistBox1.Size = new System.Drawing.Size(55, 27);
            this.ritlistBox1.TabIndex = 156;
            this.ritlistBox1.SelectedIndexChanged += new System.EventHandler(this.ritlistBox1_SelectedIndexChanged);
            // 
            // CW_Hold_numericUpDown2
            // 
            this.CW_Hold_numericUpDown2.BackColor = System.Drawing.Color.Gainsboro;
            this.CW_Hold_numericUpDown2.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CW_Hold_numericUpDown2.ForeColor = System.Drawing.Color.Black;
            this.CW_Hold_numericUpDown2.Location = new System.Drawing.Point(49, 51);
            this.CW_Hold_numericUpDown2.Maximum = new decimal(new int[] {
            35,
            0,
            0,
            0});
            this.CW_Hold_numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CW_Hold_numericUpDown2.Name = "CW_Hold_numericUpDown2";
            this.CW_Hold_numericUpDown2.Size = new System.Drawing.Size(47, 31);
            this.CW_Hold_numericUpDown2.TabIndex = 153;
            this.CW_Hold_numericUpDown2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CW_Hold_numericUpDown2.ValueChanged += new System.EventHandler(this.CW_Hold_numericUpDown2_ValueChanged);
            // 
            // CW_Lag_numericUpDown2
            // 
            this.CW_Lag_numericUpDown2.BackColor = System.Drawing.Color.Gainsboro;
            this.CW_Lag_numericUpDown2.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CW_Lag_numericUpDown2.ForeColor = System.Drawing.Color.Black;
            this.CW_Lag_numericUpDown2.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.CW_Lag_numericUpDown2.Location = new System.Drawing.Point(665, 51);
            this.CW_Lag_numericUpDown2.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.CW_Lag_numericUpDown2.Name = "CW_Lag_numericUpDown2";
            this.CW_Lag_numericUpDown2.Size = new System.Drawing.Size(47, 31);
            this.CW_Lag_numericUpDown2.TabIndex = 152;
            this.toolTip.SetToolTip(this.CW_Lag_numericUpDown2, "Not Yet Implemented");
            this.CW_Lag_numericUpDown2.ValueChanged += new System.EventHandler(this.CW_Lag_numericUpDown2_ValueChanged);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.BackColor = System.Drawing.Color.Gainsboro;
            this.numericUpDown1.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown1.ForeColor = System.Drawing.Color.Black;
            this.numericUpDown1.Location = new System.Drawing.Point(276, 205);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(47, 31);
            this.numericUpDown1.TabIndex = 151;
            this.numericUpDown1.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // CW_Paddle_listBox1
            // 
            this.CW_Paddle_listBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.CW_Paddle_listBox1.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CW_Paddle_listBox1.ForeColor = System.Drawing.Color.Black;
            this.CW_Paddle_listBox1.FormattingEnabled = true;
            this.CW_Paddle_listBox1.ItemHeight = 23;
            this.CW_Paddle_listBox1.Items.AddRange(new object[] {
            "NORMAL",
            "REVERSE"});
            this.CW_Paddle_listBox1.Location = new System.Drawing.Point(536, 53);
            this.CW_Paddle_listBox1.Name = "CW_Paddle_listBox1";
            this.CW_Paddle_listBox1.Size = new System.Drawing.Size(112, 27);
            this.CW_Paddle_listBox1.TabIndex = 149;
            this.CW_Paddle_listBox1.SelectedIndexChanged += new System.EventHandler(this.CW_Paddle_listBox1_SelectedIndexChanged);
            // 
            // CW_Weight_listBox1
            // 
            this.CW_Weight_listBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.CW_Weight_listBox1.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CW_Weight_listBox1.ForeColor = System.Drawing.Color.Black;
            this.CW_Weight_listBox1.FormattingEnabled = true;
            this.CW_Weight_listBox1.ItemHeight = 23;
            this.CW_Weight_listBox1.Items.AddRange(new object[] {
            "25",
            "50",
            "75"});
            this.CW_Weight_listBox1.Location = new System.Drawing.Point(467, 53);
            this.CW_Weight_listBox1.Name = "CW_Weight_listBox1";
            this.CW_Weight_listBox1.Size = new System.Drawing.Size(52, 27);
            this.CW_Weight_listBox1.TabIndex = 148;
            this.CW_Weight_listBox1.SelectedIndexChanged += new System.EventHandler(this.CW_Weight_listBox1_SelectedIndexChanged);
            // 
            // CW_Space_listBox1
            // 
            this.CW_Space_listBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.CW_Space_listBox1.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CW_Space_listBox1.ForeColor = System.Drawing.Color.Black;
            this.CW_Space_listBox1.FormattingEnabled = true;
            this.CW_Space_listBox1.ItemHeight = 23;
            this.CW_Space_listBox1.Items.AddRange(new object[] {
            "ELEMENT",
            "CHAR",
            "WORD"});
            this.CW_Space_listBox1.Location = new System.Drawing.Point(333, 53);
            this.CW_Space_listBox1.Name = "CW_Space_listBox1";
            this.CW_Space_listBox1.Size = new System.Drawing.Size(117, 27);
            this.CW_Space_listBox1.TabIndex = 147;
            this.CW_Space_listBox1.SelectedIndexChanged += new System.EventHandler(this.CW_Space_listBox1_SelectedIndexChanged);
            // 
            // CW_Type_listBox1
            // 
            this.CW_Type_listBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.CW_Type_listBox1.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CW_Type_listBox1.ForeColor = System.Drawing.Color.Black;
            this.CW_Type_listBox1.FormattingEnabled = true;
            this.CW_Type_listBox1.ItemHeight = 23;
            this.CW_Type_listBox1.Items.AddRange(new object[] {
            "A",
            "B",
            "DIT",
            "DAH"});
            this.CW_Type_listBox1.Location = new System.Drawing.Point(248, 53);
            this.CW_Type_listBox1.Name = "CW_Type_listBox1";
            this.CW_Type_listBox1.Size = new System.Drawing.Size(68, 27);
            this.CW_Type_listBox1.TabIndex = 146;
            this.CW_Type_listBox1.SelectedIndexChanged += new System.EventHandler(this.CW_Type_listBox1_SelectedIndexChanged);
            // 
            // CW_Mode_listBox1
            // 
            this.CW_Mode_listBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.CW_Mode_listBox1.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CW_Mode_listBox1.ForeColor = System.Drawing.Color.Black;
            this.CW_Mode_listBox1.FormattingEnabled = true;
            this.CW_Mode_listBox1.ItemHeight = 23;
            this.CW_Mode_listBox1.Items.AddRange(new object[] {
            "IAMBIC",
            "UTIMATIC",
            "BUG",
            "STRAIGHT"});
            this.CW_Mode_listBox1.Location = new System.Drawing.Point(113, 53);
            this.CW_Mode_listBox1.Name = "CW_Mode_listBox1";
            this.CW_Mode_listBox1.Size = new System.Drawing.Size(118, 27);
            this.CW_Mode_listBox1.TabIndex = 145;
            this.CW_Mode_listBox1.SelectedIndexChanged += new System.EventHandler(this.CW_Mode_listBox1_SelectedIndexChanged);
            // 
            // label61
            // 
            this.label61.BackColor = System.Drawing.Color.Gainsboro;
            this.label61.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label61.ForeColor = System.Drawing.Color.Black;
            this.label61.Location = new System.Drawing.Point(670, 20);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(59, 23);
            this.label61.TabIndex = 144;
            this.label61.Text = "LAG";
            this.label61.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label60
            // 
            this.label60.BackColor = System.Drawing.Color.Gainsboro;
            this.label60.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label60.ForeColor = System.Drawing.Color.Black;
            this.label60.Location = new System.Drawing.Point(556, 20);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(92, 23);
            this.label60.TabIndex = 143;
            this.label60.Text = "PADDLE";
            this.label60.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label60.Click += new System.EventHandler(this.label60_Click);
            // 
            // label59
            // 
            this.label59.BackColor = System.Drawing.Color.Gainsboro;
            this.label59.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label59.ForeColor = System.Drawing.Color.Black;
            this.label59.Location = new System.Drawing.Point(451, 20);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(83, 23);
            this.label59.TabIndex = 142;
            this.label59.Text = "WEIGHT";
            this.label59.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label58
            // 
            this.label58.BackColor = System.Drawing.Color.Gainsboro;
            this.label58.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label58.ForeColor = System.Drawing.Color.Black;
            this.label58.Location = new System.Drawing.Point(348, 20);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(81, 23);
            this.label58.TabIndex = 141;
            this.label58.Text = "SPACE";
            this.label58.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label45
            // 
            this.label45.BackColor = System.Drawing.Color.Gainsboro;
            this.label45.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label45.ForeColor = System.Drawing.Color.Black;
            this.label45.Location = new System.Drawing.Point(244, 170);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(79, 23);
            this.label45.TabIndex = 140;
            this.label45.Text = "SPEED";
            this.label45.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label43
            // 
            this.label43.BackColor = System.Drawing.Color.Gainsboro;
            this.label43.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.ForeColor = System.Drawing.Color.Black;
            this.label43.Location = new System.Drawing.Point(257, 20);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(69, 23);
            this.label43.TabIndex = 139;
            this.label43.Text = "TYPE";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Gainsboro;
            this.label5.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(156, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 23);
            this.label5.TabIndex = 138;
            this.label5.Text = "MODE";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Gainsboro;
            this.label8.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(32, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(102, 26);
            this.label8.TabIndex = 132;
            this.label8.Text = "TX Hold";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CW_Pitch_listBox1
            // 
            this.CW_Pitch_listBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.CW_Pitch_listBox1.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CW_Pitch_listBox1.ForeColor = System.Drawing.Color.Black;
            this.CW_Pitch_listBox1.FormattingEnabled = true;
            this.CW_Pitch_listBox1.ItemHeight = 23;
            this.CW_Pitch_listBox1.Items.AddRange(new object[] {
            "400Hz",
            "500Hz",
            "600Hz",
            "700Hz",
            "800Hz"});
            this.CW_Pitch_listBox1.Location = new System.Drawing.Point(441, 207);
            this.CW_Pitch_listBox1.Name = "CW_Pitch_listBox1";
            this.CW_Pitch_listBox1.Size = new System.Drawing.Size(88, 27);
            this.CW_Pitch_listBox1.TabIndex = 137;
            this.CW_Pitch_listBox1.SelectedIndexChanged += new System.EventHandler(this.CW_Pitch_listBox1_SelectedIndexChanged);
            // 
            // label35
            // 
            this.label35.BackColor = System.Drawing.Color.Gainsboro;
            this.label35.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.ForeColor = System.Drawing.Color.Black;
            this.label35.Location = new System.Drawing.Point(441, 170);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(71, 23);
            this.label35.TabIndex = 136;
            this.label35.Text = "PITCH";
            this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label35.Click += new System.EventHandler(this.label35_Click);
            // 
            // semicheckBox2
            // 
            this.semicheckBox2.AutoSize = true;
            this.semicheckBox2.BackColor = System.Drawing.Color.Gainsboro;
            this.semicheckBox2.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.semicheckBox2.ForeColor = System.Drawing.Color.Black;
            this.semicheckBox2.Location = new System.Drawing.Point(342, 207);
            this.semicheckBox2.Name = "semicheckBox2";
            this.semicheckBox2.Size = new System.Drawing.Size(77, 27);
            this.semicheckBox2.TabIndex = 135;
            this.semicheckBox2.Text = "Semi";
            this.semicheckBox2.UseVisualStyleBackColor = false;
            this.semicheckBox2.CheckedChanged += new System.EventHandler(this.semicheckBox2_CheckedChanged);
            // 
            // MFC
            // 
            this.MFC.BackColor = System.Drawing.Color.Black;
            this.MFC.Controls.Add(this.label33);
            this.MFC.Controls.Add(this.AMP_comboBox1);
            this.MFC.Controls.Add(this.IQBD_hScrollBar1);
            this.MFC.Controls.Add(this.Antenna_Switch_label43);
            this.MFC.Controls.Add(this.Antenna_Switch_comboBox1);
            this.MFC.Controls.Add(this.Tuning_Knob_groupBox1);
            this.MFC.Controls.Add(this.AMP_groupBox3);
            this.MFC.Controls.Add(this.IQBD_groupBox4);
            this.MFC.ForeColor = System.Drawing.Color.Black;
            this.MFC.Location = new System.Drawing.Point(4, 25);
            this.MFC.Name = "MFC";
            this.MFC.Padding = new System.Windows.Forms.Padding(3);
            this.MFC.Size = new System.Drawing.Size(792, 451);
            this.MFC.TabIndex = 14;
            this.MFC.Text = "Solidus";
            this.MFC.Click += new System.EventHandler(this.MFC_Click);
            this.MFC.Enter += new System.EventHandler(this.MFC_Enter);
            this.MFC.Leave += new System.EventHandler(this.MFC_Leave);
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.ForeColor = System.Drawing.Color.White;
            this.label33.Location = new System.Drawing.Point(636, 216);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(106, 32);
            this.label33.TabIndex = 194;
            this.label33.Text = "CALIBRATION\r\nBAND";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AMP_comboBox1
            // 
            this.AMP_comboBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.AMP_comboBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AMP_comboBox1.ForeColor = System.Drawing.Color.Black;
            this.AMP_comboBox1.FormattingEnabled = true;
            this.AMP_comboBox1.Items.AddRange(new object[] {
            "160M",
            "80M",
            "60M",
            "40M",
            "30M",
            "20M",
            "17M",
            "15M",
            "12M",
            "10M"});
            this.AMP_comboBox1.Location = new System.Drawing.Point(659, 251);
            this.AMP_comboBox1.Name = "AMP_comboBox1";
            this.AMP_comboBox1.Size = new System.Drawing.Size(61, 21);
            this.AMP_comboBox1.TabIndex = 193;
            this.AMP_comboBox1.SelectedIndexChanged += new System.EventHandler(this.AMP_comboBox1_SelectedIndexChanged);
            // 
            // IQBD_hScrollBar1
            // 
            this.IQBD_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.IQBD_hScrollBar1.Location = new System.Drawing.Point(162, 420);
            this.IQBD_hScrollBar1.Maximum = 209;
            this.IQBD_hScrollBar1.Minimum = -200;
            this.IQBD_hScrollBar1.Name = "IQBD_hScrollBar1";
            this.IQBD_hScrollBar1.Size = new System.Drawing.Size(454, 15);
            this.IQBD_hScrollBar1.TabIndex = 189;
            this.toolTip.SetToolTip(this.IQBD_hScrollBar1, "Adjust the IQ balance for minimum RX or TX image. \r\nClick in the scroll bar for c" +
        "ourse adjustment (10).\r\nUse Mouse Wheel for fine adjustment (1).");
            this.IQBD_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.IQBD_hScrollBar1_Scroll);
            // 
            // Antenna_Switch_label43
            // 
            this.Antenna_Switch_label43.AutoSize = true;
            this.Antenna_Switch_label43.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Antenna_Switch_label43.ForeColor = System.Drawing.Color.White;
            this.Antenna_Switch_label43.Location = new System.Drawing.Point(513, 216);
            this.Antenna_Switch_label43.Name = "Antenna_Switch_label43";
            this.Antenna_Switch_label43.Size = new System.Drawing.Size(75, 32);
            this.Antenna_Switch_label43.TabIndex = 185;
            this.Antenna_Switch_label43.Text = "ANTENNA\r\nSWITCH";
            this.Antenna_Switch_label43.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Antenna_Switch_comboBox1
            // 
            this.Antenna_Switch_comboBox1.BackColor = System.Drawing.Color.White;
            this.Antenna_Switch_comboBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Antenna_Switch_comboBox1.ForeColor = System.Drawing.Color.Black;
            this.Antenna_Switch_comboBox1.FormattingEnabled = true;
            this.Antenna_Switch_comboBox1.Items.AddRange(new object[] {
            "160M",
            "80M",
            "60M",
            "40M",
            "30M",
            "20M",
            "17M",
            "15M",
            "12M",
            "10M"});
            this.Antenna_Switch_comboBox1.Location = new System.Drawing.Point(513, 251);
            this.Antenna_Switch_comboBox1.Name = "Antenna_Switch_comboBox1";
            this.Antenna_Switch_comboBox1.Size = new System.Drawing.Size(75, 21);
            this.Antenna_Switch_comboBox1.TabIndex = 184;
            this.toolTip.SetToolTip(this.Antenna_Switch_comboBox1, "Antenna Switch Band Select.\r\nBands on or Above the Selected Band Activates the An" +
        "tenna Switch.\r\nBands below the Selected Band de-activates the Antenna Switch.\r\n");
            this.Antenna_Switch_comboBox1.SelectedIndexChanged += new System.EventHandler(this.Antenna_Switch_comboBox1_SelectedIndexChanged);
            // 
            // Tuning_Knob_groupBox1
            // 
            this.Tuning_Knob_groupBox1.Controls.Add(this.label39);
            this.Tuning_Knob_groupBox1.Controls.Add(this.label37);
            this.Tuning_Knob_groupBox1.Controls.Add(this.label2);
            this.Tuning_Knob_groupBox1.Controls.Add(this.label46);
            this.Tuning_Knob_groupBox1.Controls.Add(this.Right_Button_comboBox4);
            this.Tuning_Knob_groupBox1.Controls.Add(this.Knob_comboBox1);
            this.Tuning_Knob_groupBox1.Controls.Add(this.Left_Button_comboBox2);
            this.Tuning_Knob_groupBox1.Controls.Add(this.Middle_Button_comboBox3);
            this.Tuning_Knob_groupBox1.ForeColor = System.Drawing.Color.White;
            this.Tuning_Knob_groupBox1.Location = new System.Drawing.Point(8, 206);
            this.Tuning_Knob_groupBox1.Name = "Tuning_Knob_groupBox1";
            this.Tuning_Knob_groupBox1.Size = new System.Drawing.Size(469, 86);
            this.Tuning_Knob_groupBox1.TabIndex = 182;
            this.Tuning_Knob_groupBox1.TabStop = false;
            this.Tuning_Knob_groupBox1.Text = "MFC";
            this.toolTip.SetToolTip(this.Tuning_Knob_groupBox1, "Multi Function Controller");
            this.Tuning_Knob_groupBox1.Enter += new System.EventHandler(this.Tuning_Knob_groupBox1_Enter);
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.ForeColor = System.Drawing.Color.White;
            this.label39.Location = new System.Drawing.Point(397, 19);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(14, 14);
            this.label39.TabIndex = 191;
            this.label39.Text = "K";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.ForeColor = System.Drawing.Color.White;
            this.label37.Location = new System.Drawing.Point(284, 19);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(15, 14);
            this.label37.TabIndex = 190;
            this.label37.Text = "C";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(171, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 14);
            this.label2.TabIndex = 189;
            this.label2.Text = "B";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label46.ForeColor = System.Drawing.Color.White;
            this.label46.Location = new System.Drawing.Point(58, 20);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(15, 14);
            this.label46.TabIndex = 188;
            this.label46.Text = "A";
            this.label46.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Right_Button_comboBox4
            // 
            this.Right_Button_comboBox4.BackColor = System.Drawing.Color.Gainsboro;
            this.Right_Button_comboBox4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Right_Button_comboBox4.ForeColor = System.Drawing.Color.Black;
            this.Right_Button_comboBox4.FormattingEnabled = true;
            this.Right_Button_comboBox4.Items.AddRange(new object[] {
            "NONE",
            "STEP",
            "PTT",
            "TUNE",
            "MODE",
            "RIT ON-OFF",
            "BAND",
            "FREQ/VOL",
            "CW BW",
            "HI BW",
            "RIT OFFSET",
            "FAVS"});
            this.Right_Button_comboBox4.Location = new System.Drawing.Point(239, 37);
            this.Right_Button_comboBox4.Name = "Right_Button_comboBox4";
            this.Right_Button_comboBox4.Size = new System.Drawing.Size(104, 21);
            this.Right_Button_comboBox4.TabIndex = 186;
            this.toolTip.SetToolTip(this.Right_Button_comboBox4, "MFC - Button C");
            this.Right_Button_comboBox4.SelectedIndexChanged += new System.EventHandler(this.Right_Button_comboBox4_SelectedIndexChanged);
            // 
            // Knob_comboBox1
            // 
            this.Knob_comboBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.Knob_comboBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Knob_comboBox1.ForeColor = System.Drawing.Color.Black;
            this.Knob_comboBox1.FormattingEnabled = true;
            this.Knob_comboBox1.Items.AddRange(new object[] {
            "NONE",
            "STEP",
            "PTT",
            "TUNE",
            "MODE",
            "RIT ON-OFF",
            "BAND",
            "FREQ/VOL",
            "CW BW",
            "HI BW",
            "RIT OFFSET",
            "FAVS"});
            this.Knob_comboBox1.Location = new System.Drawing.Point(352, 37);
            this.Knob_comboBox1.Name = "Knob_comboBox1";
            this.Knob_comboBox1.Size = new System.Drawing.Size(104, 21);
            this.Knob_comboBox1.TabIndex = 187;
            this.toolTip.SetToolTip(this.Knob_comboBox1, "MFC - Knob Switch");
            this.Knob_comboBox1.SelectedIndexChanged += new System.EventHandler(this.Knob_comboBox1_SelectedIndexChanged);
            // 
            // Left_Button_comboBox2
            // 
            this.Left_Button_comboBox2.BackColor = System.Drawing.Color.Gainsboro;
            this.Left_Button_comboBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Left_Button_comboBox2.ForeColor = System.Drawing.Color.Black;
            this.Left_Button_comboBox2.FormattingEnabled = true;
            this.Left_Button_comboBox2.Items.AddRange(new object[] {
            "NONE",
            "STEP",
            "PTT",
            "TUNE",
            "MODE",
            "RIT ON-OFF",
            "BAND",
            "FREQ/VOL",
            "CW BW",
            "HI BW",
            "RIT OFFSET",
            "FAVS"});
            this.Left_Button_comboBox2.Location = new System.Drawing.Point(13, 37);
            this.Left_Button_comboBox2.Name = "Left_Button_comboBox2";
            this.Left_Button_comboBox2.Size = new System.Drawing.Size(104, 21);
            this.Left_Button_comboBox2.TabIndex = 184;
            this.toolTip.SetToolTip(this.Left_Button_comboBox2, "MFC - Button A");
            this.Left_Button_comboBox2.SelectedIndexChanged += new System.EventHandler(this.Left_Button_comboBox2_SelectedIndexChanged);
            // 
            // Middle_Button_comboBox3
            // 
            this.Middle_Button_comboBox3.BackColor = System.Drawing.Color.Gainsboro;
            this.Middle_Button_comboBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Middle_Button_comboBox3.ForeColor = System.Drawing.Color.Black;
            this.Middle_Button_comboBox3.FormattingEnabled = true;
            this.Middle_Button_comboBox3.Items.AddRange(new object[] {
            "NONE",
            "STEP",
            "PTT",
            "TUNE",
            "MODE",
            "RIT ON-OFF",
            "BAND",
            "FREQ/VOL",
            "CW BW",
            "HI BW",
            "RIT OFFSET",
            "FAVS"});
            this.Middle_Button_comboBox3.Location = new System.Drawing.Point(126, 37);
            this.Middle_Button_comboBox3.Name = "Middle_Button_comboBox3";
            this.Middle_Button_comboBox3.Size = new System.Drawing.Size(104, 21);
            this.Middle_Button_comboBox3.TabIndex = 185;
            this.toolTip.SetToolTip(this.Middle_Button_comboBox3, "MFC - Button B");
            this.Middle_Button_comboBox3.SelectedIndexChanged += new System.EventHandler(this.Middle_Button_comboBox3_SelectedIndexChanged);
            // 
            // AMP_groupBox3
            // 
            this.AMP_groupBox3.Controls.Add(this.Solidus_Tab_Power_Display_label33);
            this.AMP_groupBox3.Controls.Add(this.Solidus_Bias_button8);
            this.AMP_groupBox3.Controls.Add(this.Power_calibration_label58);
            this.AMP_groupBox3.Controls.Add(this.AMP_Raw_Bias_label5);
            this.AMP_groupBox3.Controls.Add(this.AMP_Calibration_label58);
            this.AMP_groupBox3.Controls.Add(this.AMP_Power_Output_label5);
            this.AMP_groupBox3.Controls.Add(this.AMP_Temp_MFC_AMP_label5);
            this.AMP_groupBox3.Controls.Add(this.AMP_Calibrate_hScrollBar1);
            this.AMP_groupBox3.Controls.Add(this.AMP_hScrollBar1);
            this.AMP_groupBox3.Controls.Add(this.AMP_label57);
            this.AMP_groupBox3.Controls.Add(this.AMP_Tune_button4);
            this.AMP_groupBox3.ForeColor = System.Drawing.Color.White;
            this.AMP_groupBox3.Location = new System.Drawing.Point(8, 16);
            this.AMP_groupBox3.Name = "AMP_groupBox3";
            this.AMP_groupBox3.Size = new System.Drawing.Size(776, 169);
            this.AMP_groupBox3.TabIndex = 148;
            this.AMP_groupBox3.TabStop = false;
            this.AMP_groupBox3.Text = "POWER AMPLIFIER";
            this.AMP_groupBox3.Enter += new System.EventHandler(this.AMP_groupBox3_Enter);
            // 
            // Solidus_Tab_Power_Display_label33
            // 
            this.Solidus_Tab_Power_Display_label33.BackColor = System.Drawing.Color.Gainsboro;
            this.Solidus_Tab_Power_Display_label33.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Solidus_Tab_Power_Display_label33.ForeColor = System.Drawing.Color.Black;
            this.Solidus_Tab_Power_Display_label33.Location = new System.Drawing.Point(287, 23);
            this.Solidus_Tab_Power_Display_label33.Name = "Solidus_Tab_Power_Display_label33";
            this.Solidus_Tab_Power_Display_label33.Size = new System.Drawing.Size(207, 23);
            this.Solidus_Tab_Power_Display_label33.TabIndex = 190;
            this.Solidus_Tab_Power_Display_label33.Text = "PA Power:";
            this.Solidus_Tab_Power_Display_label33.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Solidus_Tab_Power_Display_label33.Click += new System.EventHandler(this.Solidus_Tab_Power_Display_label33_Click);
            // 
            // Solidus_Bias_button8
            // 
            this.Solidus_Bias_button8.BackColor = System.Drawing.Color.Gainsboro;
            this.Solidus_Bias_button8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Solidus_Bias_button8.ForeColor = System.Drawing.Color.Black;
            this.Solidus_Bias_button8.Location = new System.Drawing.Point(127, 15);
            this.Solidus_Bias_button8.Name = "Solidus_Bias_button8";
            this.Solidus_Bias_button8.Size = new System.Drawing.Size(90, 27);
            this.Solidus_Bias_button8.TabIndex = 186;
            this.Solidus_Bias_button8.Text = "BIAS";
            this.toolTip.SetToolTip(this.Solidus_Bias_button8, "BIAS - CALIBRATE AMPLIFIER BIAS");
            this.Solidus_Bias_button8.UseVisualStyleBackColor = false;
            this.Solidus_Bias_button8.Click += new System.EventHandler(this.Solidus_Bias_button8_Click);
            // 
            // Power_calibration_label58
            // 
            this.Power_calibration_label58.BackColor = System.Drawing.Color.Gainsboro;
            this.Power_calibration_label58.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Power_calibration_label58.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Power_calibration_label58.ForeColor = System.Drawing.Color.Black;
            this.Power_calibration_label58.Location = new System.Drawing.Point(559, 59);
            this.Power_calibration_label58.Name = "Power_calibration_label58";
            this.Power_calibration_label58.Size = new System.Drawing.Size(90, 27);
            this.Power_calibration_label58.TabIndex = 151;
            this.Power_calibration_label58.Text = "0";
            this.Power_calibration_label58.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Power_calibration_label58.Click += new System.EventHandler(this.Power_calibration_label58_Click);
            // 
            // AMP_Raw_Bias_label5
            // 
            this.AMP_Raw_Bias_label5.BackColor = System.Drawing.Color.Gainsboro;
            this.AMP_Raw_Bias_label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.AMP_Raw_Bias_label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AMP_Raw_Bias_label5.ForeColor = System.Drawing.Color.Black;
            this.AMP_Raw_Bias_label5.Location = new System.Drawing.Point(287, 92);
            this.AMP_Raw_Bias_label5.Name = "AMP_Raw_Bias_label5";
            this.AMP_Raw_Bias_label5.Size = new System.Drawing.Size(207, 23);
            this.AMP_Raw_Bias_label5.TabIndex = 170;
            this.AMP_Raw_Bias_label5.Text = "Amplifier PA I (ma): 00000 ma";
            this.AMP_Raw_Bias_label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.AMP_Raw_Bias_label5.Click += new System.EventHandler(this.AMP_Raw_Bias_label5_Click);
            // 
            // AMP_Calibration_label58
            // 
            this.AMP_Calibration_label58.BackColor = System.Drawing.Color.Gainsboro;
            this.AMP_Calibration_label58.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.AMP_Calibration_label58.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AMP_Calibration_label58.ForeColor = System.Drawing.Color.Black;
            this.AMP_Calibration_label58.Location = new System.Drawing.Point(555, 95);
            this.AMP_Calibration_label58.Name = "AMP_Calibration_label58";
            this.AMP_Calibration_label58.Size = new System.Drawing.Size(214, 23);
            this.AMP_Calibration_label58.TabIndex = 159;
            this.AMP_Calibration_label58.Text = "POWER CALIBRATION";
            this.AMP_Calibration_label58.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.AMP_Calibration_label58.Click += new System.EventHandler(this.AMP_Calibration_label58_Click);
            // 
            // AMP_Power_Output_label5
            // 
            this.AMP_Power_Output_label5.BackColor = System.Drawing.Color.Gainsboro;
            this.AMP_Power_Output_label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.AMP_Power_Output_label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AMP_Power_Output_label5.ForeColor = System.Drawing.Color.Black;
            this.AMP_Power_Output_label5.Location = new System.Drawing.Point(10, 95);
            this.AMP_Power_Output_label5.Name = "AMP_Power_Output_label5";
            this.AMP_Power_Output_label5.Size = new System.Drawing.Size(214, 23);
            this.AMP_Power_Output_label5.TabIndex = 158;
            this.AMP_Power_Output_label5.Text = "POWER OUTPUT";
            this.AMP_Power_Output_label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.AMP_Power_Output_label5.Click += new System.EventHandler(this.AMP_Power_Output_label5_Click);
            // 
            // AMP_Temp_MFC_AMP_label5
            // 
            this.AMP_Temp_MFC_AMP_label5.BackColor = System.Drawing.Color.Gainsboro;
            this.AMP_Temp_MFC_AMP_label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.AMP_Temp_MFC_AMP_label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AMP_Temp_MFC_AMP_label5.ForeColor = System.Drawing.Color.Black;
            this.AMP_Temp_MFC_AMP_label5.Location = new System.Drawing.Point(287, 59);
            this.AMP_Temp_MFC_AMP_label5.Name = "AMP_Temp_MFC_AMP_label5";
            this.AMP_Temp_MFC_AMP_label5.Size = new System.Drawing.Size(207, 23);
            this.AMP_Temp_MFC_AMP_label5.TabIndex = 156;
            this.AMP_Temp_MFC_AMP_label5.Text = "PA Temperature: 0";
            this.AMP_Temp_MFC_AMP_label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.AMP_Temp_MFC_AMP_label5, "10 minute warm cycle after the transceiver is powered on");
            this.AMP_Temp_MFC_AMP_label5.Click += new System.EventHandler(this.AMP_Temp_MFC_AMP_label5_Click);
            // 
            // AMP_Calibrate_hScrollBar1
            // 
            this.AMP_Calibrate_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.AMP_Calibrate_hScrollBar1.LargeChange = 5;
            this.AMP_Calibrate_hScrollBar1.Location = new System.Drawing.Point(555, 130);
            this.AMP_Calibrate_hScrollBar1.Maximum = 4;
            this.AMP_Calibrate_hScrollBar1.Minimum = -99;
            this.AMP_Calibrate_hScrollBar1.Name = "AMP_Calibrate_hScrollBar1";
            this.AMP_Calibrate_hScrollBar1.Size = new System.Drawing.Size(214, 21);
            this.AMP_Calibrate_hScrollBar1.TabIndex = 154;
            this.toolTip.SetToolTip(this.AMP_Calibrate_hScrollBar1, "Adjust Slider to set Amplifer Calibration\r\n");
            this.AMP_Calibrate_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.AMP_Calibrate_hScrollBar1_Scroll);
            // 
            // AMP_hScrollBar1
            // 
            this.AMP_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.AMP_hScrollBar1.Location = new System.Drawing.Point(10, 132);
            this.AMP_hScrollBar1.Maximum = 109;
            this.AMP_hScrollBar1.Minimum = 1;
            this.AMP_hScrollBar1.Name = "AMP_hScrollBar1";
            this.AMP_hScrollBar1.Size = new System.Drawing.Size(213, 21);
            this.AMP_hScrollBar1.SmallChange = 5;
            this.AMP_hScrollBar1.TabIndex = 145;
            this.toolTip.SetToolTip(this.AMP_hScrollBar1, "Adjust Slider to Set the Amplifer Output Power\r\n");
            this.AMP_hScrollBar1.Value = 50;
            this.AMP_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.AMP_hScrollBar1_Scroll);
            // 
            // AMP_label57
            // 
            this.AMP_label57.BackColor = System.Drawing.Color.Gainsboro;
            this.AMP_label57.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.AMP_label57.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AMP_label57.ForeColor = System.Drawing.Color.Black;
            this.AMP_label57.Location = new System.Drawing.Point(127, 59);
            this.AMP_label57.Name = "AMP_label57";
            this.AMP_label57.Size = new System.Drawing.Size(90, 27);
            this.AMP_label57.TabIndex = 142;
            this.AMP_label57.Text = "0 %";
            this.AMP_label57.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.AMP_label57.Click += new System.EventHandler(this.AMP_label57_Click);
            // 
            // AMP_Tune_button4
            // 
            this.AMP_Tune_button4.BackColor = System.Drawing.Color.Gainsboro;
            this.AMP_Tune_button4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AMP_Tune_button4.ForeColor = System.Drawing.Color.Black;
            this.AMP_Tune_button4.Location = new System.Drawing.Point(559, 15);
            this.AMP_Tune_button4.Name = "AMP_Tune_button4";
            this.AMP_Tune_button4.Size = new System.Drawing.Size(90, 27);
            this.AMP_Tune_button4.TabIndex = 146;
            this.AMP_Tune_button4.Text = "TUNE";
            this.toolTip.SetToolTip(this.AMP_Tune_button4, "Set TUNE Mode for Adjusting\r\nPower Output and Power Calibration");
            this.AMP_Tune_button4.UseVisualStyleBackColor = false;
            this.AMP_Tune_button4.Click += new System.EventHandler(this.AMP_Tune_button4_Click);
            // 
            // IQBD_groupBox4
            // 
            this.IQBD_groupBox4.Controls.Add(this.label18);
            this.IQBD_groupBox4.Controls.Add(this.label4);
            this.IQBD_groupBox4.Controls.Add(this.IQBD_hScrollBar2);
            this.IQBD_groupBox4.Controls.Add(this.IQBD_Apply_button8);
            this.IQBD_groupBox4.Controls.Add(this.IQBD_ONOFF);
            this.IQBD_groupBox4.Controls.Add(this.IQBD_Monitor_label);
            this.IQBD_groupBox4.Controls.Add(this.IQBD_Tune_button8);
            this.IQBD_groupBox4.ForeColor = System.Drawing.Color.White;
            this.IQBD_groupBox4.Location = new System.Drawing.Point(7, 314);
            this.IQBD_groupBox4.Name = "IQBD_groupBox4";
            this.IQBD_groupBox4.Size = new System.Drawing.Size(778, 129);
            this.IQBD_groupBox4.TabIndex = 192;
            this.IQBD_groupBox4.TabStop = false;
            this.IQBD_groupBox4.Text = "TX IQBD";
            this.IQBD_groupBox4.Enter += new System.EventHandler(this.IQBD_groupBox4_Enter);
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.Color.Gainsboro;
            this.label18.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label18.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.ForeColor = System.Drawing.Color.Black;
            this.label18.Location = new System.Drawing.Point(344, 70);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(90, 26);
            this.label18.TabIndex = 187;
            this.label18.Text = "I/Q BALANCE";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Gainsboro;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(344, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 26);
            this.label4.TabIndex = 193;
            this.label4.Text = "TUNE POWER";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // IQBD_hScrollBar2
            // 
            this.IQBD_hScrollBar2.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.IQBD_hScrollBar2.LargeChange = 5;
            this.IQBD_hScrollBar2.Location = new System.Drawing.Point(279, 48);
            this.IQBD_hScrollBar2.Maximum = 104;
            this.IQBD_hScrollBar2.Name = "IQBD_hScrollBar2";
            this.IQBD_hScrollBar2.Size = new System.Drawing.Size(221, 15);
            this.IQBD_hScrollBar2.TabIndex = 190;
            this.toolTip.SetToolTip(this.IQBD_hScrollBar2, "Adjust Output TUNE Power");
            this.IQBD_hScrollBar2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.IQBD_hScrollBar2_Scroll);
            // 
            // IQBD_Apply_button8
            // 
            this.IQBD_Apply_button8.BackColor = System.Drawing.Color.Gainsboro;
            this.IQBD_Apply_button8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IQBD_Apply_button8.ForeColor = System.Drawing.Color.Black;
            this.IQBD_Apply_button8.Location = new System.Drawing.Point(535, 48);
            this.IQBD_Apply_button8.Name = "IQBD_Apply_button8";
            this.IQBD_Apply_button8.Size = new System.Drawing.Size(81, 26);
            this.IQBD_Apply_button8.TabIndex = 191;
            this.IQBD_Apply_button8.Text = "APPLY";
            this.toolTip.SetToolTip(this.IQBD_Apply_button8, "Records the current I/Q Value");
            this.IQBD_Apply_button8.UseVisualStyleBackColor = false;
            this.IQBD_Apply_button8.Click += new System.EventHandler(this.IQBD_Apply_button8_Click);
            // 
            // IQBD_ONOFF
            // 
            this.IQBD_ONOFF.BackColor = System.Drawing.Color.Gainsboro;
            this.IQBD_ONOFF.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IQBD_ONOFF.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IQBD_ONOFF.ForeColor = System.Drawing.Color.Black;
            this.IQBD_ONOFF.Location = new System.Drawing.Point(162, 13);
            this.IQBD_ONOFF.Name = "IQBD_ONOFF";
            this.IQBD_ONOFF.Size = new System.Drawing.Size(81, 26);
            this.IQBD_ONOFF.TabIndex = 186;
            this.IQBD_ONOFF.Text = "IQBD";
            this.toolTip.SetToolTip(this.IQBD_ONOFF, "Records the current I/Q Value");
            this.IQBD_ONOFF.UseVisualStyleBackColor = false;
            this.IQBD_ONOFF.Click += new System.EventHandler(this.IQBD_ONOFF_Click);
            // 
            // IQBD_Monitor_label
            // 
            this.IQBD_Monitor_label.BackColor = System.Drawing.Color.Gainsboro;
            this.IQBD_Monitor_label.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IQBD_Monitor_label.ForeColor = System.Drawing.Color.Black;
            this.IQBD_Monitor_label.Location = new System.Drawing.Point(535, 13);
            this.IQBD_Monitor_label.Name = "IQBD_Monitor_label";
            this.IQBD_Monitor_label.Size = new System.Drawing.Size(81, 26);
            this.IQBD_Monitor_label.TabIndex = 187;
            this.IQBD_Monitor_label.Text = "I/Q VALUE";
            this.IQBD_Monitor_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.IQBD_Monitor_label, "Displays the frequency deviation from the \r\nstandard carrier frequency after a CH" +
        "ECK or\r\nCALIBRATE is performed.");
            this.IQBD_Monitor_label.Click += new System.EventHandler(this.IQBD_Monitor_Click);
            // 
            // IQBD_Tune_button8
            // 
            this.IQBD_Tune_button8.BackColor = System.Drawing.Color.Gainsboro;
            this.IQBD_Tune_button8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IQBD_Tune_button8.ForeColor = System.Drawing.Color.Black;
            this.IQBD_Tune_button8.Location = new System.Drawing.Point(162, 48);
            this.IQBD_Tune_button8.Name = "IQBD_Tune_button8";
            this.IQBD_Tune_button8.Size = new System.Drawing.Size(81, 26);
            this.IQBD_Tune_button8.TabIndex = 188;
            this.IQBD_Tune_button8.Text = "TUNE";
            this.toolTip.SetToolTip(this.IQBD_Tune_button8, "Set the tranceiver to Transmite Mode");
            this.IQBD_Tune_button8.UseVisualStyleBackColor = false;
            this.IQBD_Tune_button8.Click += new System.EventHandler(this.IQBD_Tune_button8_Click);
            // 
            // metertab
            // 
            this.metertab.BackColor = System.Drawing.Color.Black;
            this.metertab.Controls.Add(this.Freq_Comp_label32);
            this.metertab.Controls.Add(this.groupBox4);
            this.metertab.Controls.Add(this.Time_checkBox2);
            this.metertab.Controls.Add(this.SMeter_groupBox4);
            this.metertab.Controls.Add(this.button5);
            this.metertab.Controls.Add(this.RPi_Temperature_label1);
            this.metertab.Controls.Add(this.Colors_groupBox);
            this.metertab.Controls.Add(this.AMP_Current_label5);
            this.metertab.Controls.Add(this.Amplifier_temperature_label58);
            this.metertab.Controls.Add(this.Temperature_label57);
            this.metertab.Controls.Add(this.Relay_Board_checkBox2);
            this.metertab.Controls.Add(this.MSCC_Core_Version_label45);
            this.metertab.Controls.Add(this.MSCC_Display_label44);
            this.metertab.Controls.Add(this.HR50_PPT_listBox1);
            this.metertab.Controls.Add(this.HR50_checkBox2);
            this.metertab.Controls.Add(this.label16);
            this.metertab.Controls.Add(this.HR50_listBox1);
            this.metertab.Controls.Add(this.button2);
            this.metertab.Controls.Add(this.Zip_button2);
            this.metertab.Controls.Add(this.Restore_button2);
            this.metertab.Controls.Add(this.SDRcore_Trans_Version);
            this.metertab.Controls.Add(this.SDRcore_Recv_Version_label16);
            this.metertab.Controls.Add(this.MS_SDR_Version_label16);
            this.metertab.Controls.Add(this.label15);
            this.metertab.Controls.Add(this.label12);
            this.metertab.Controls.Add(this.label11);
            this.metertab.Controls.Add(this.label6);
            this.metertab.Controls.Add(this.firmwarelabel16);
            this.metertab.Controls.Add(this.Transvertercheckbox);
            this.metertab.Controls.Add(this.Monitorbutton);
            this.metertab.ForeColor = System.Drawing.Color.Black;
            this.metertab.Location = new System.Drawing.Point(4, 25);
            this.metertab.Name = "metertab";
            this.metertab.Padding = new System.Windows.Forms.Padding(3);
            this.metertab.Size = new System.Drawing.Size(792, 451);
            this.metertab.TabIndex = 7;
            this.metertab.Text = "Audio/Sys";
            this.metertab.Click += new System.EventHandler(this.metertab_Click);
            // 
            // Freq_Comp_label32
            // 
            this.Freq_Comp_label32.BackColor = System.Drawing.Color.Gainsboro;
            this.Freq_Comp_label32.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Freq_Comp_label32.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Freq_Comp_label32.ForeColor = System.Drawing.Color.Black;
            this.Freq_Comp_label32.Location = new System.Drawing.Point(320, 295);
            this.Freq_Comp_label32.Name = "Freq_Comp_label32";
            this.Freq_Comp_label32.Size = new System.Drawing.Size(162, 23);
            this.Freq_Comp_label32.TabIndex = 183;
            this.Freq_Comp_label32.Text = "Freq Comp:  0 Hz";
            this.Freq_Comp_label32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Freq_Comp_label32.Click += new System.EventHandler(this.Freq_Comp_label32_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.Compression_Level_hScrollBar1);
            this.groupBox4.Controls.Add(this.Volume_Attn_listBox1);
            this.groupBox4.Controls.Add(this.Compression_button2);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.Compression_label44);
            this.groupBox4.Controls.Add(this.Mic_Gain_label2);
            this.groupBox4.Controls.Add(this.Mic_Gain_Step_listBox1);
            this.groupBox4.ForeColor = System.Drawing.Color.White;
            this.groupBox4.Location = new System.Drawing.Point(190, 134);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(400, 118);
            this.groupBox4.TabIndex = 182;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Audio";
            // 
            // Compression_Level_hScrollBar1
            // 
            this.Compression_Level_hScrollBar1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.Compression_Level_hScrollBar1.LargeChange = 5;
            this.Compression_Level_hScrollBar1.Location = new System.Drawing.Point(13, 47);
            this.Compression_Level_hScrollBar1.Maximum = 24;
            this.Compression_Level_hScrollBar1.Name = "Compression_Level_hScrollBar1";
            this.Compression_Level_hScrollBar1.Size = new System.Drawing.Size(120, 15);
            this.Compression_Level_hScrollBar1.TabIndex = 161;
            this.Compression_Level_hScrollBar1.TabStop = true;
            this.toolTip.SetToolTip(this.Compression_Level_hScrollBar1, "Set the Compression Value");
            this.Compression_Level_hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Compression_Level_hScrollBar1_Scroll);
            // 
            // Volume_Attn_listBox1
            // 
            this.Volume_Attn_listBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.Volume_Attn_listBox1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Volume_Attn_listBox1.ForeColor = System.Drawing.Color.Black;
            this.Volume_Attn_listBox1.FormattingEnabled = true;
            this.Volume_Attn_listBox1.ItemHeight = 16;
            this.Volume_Attn_listBox1.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.Volume_Attn_listBox1.Location = new System.Drawing.Point(304, 45);
            this.Volume_Attn_listBox1.Name = "Volume_Attn_listBox1";
            this.Volume_Attn_listBox1.Size = new System.Drawing.Size(42, 20);
            this.Volume_Attn_listBox1.TabIndex = 181;
            this.Volume_Attn_listBox1.SelectedIndexChanged += new System.EventHandler(this.Volume_Attn_listBox1_SelectedIndexChanged);
            // 
            // Compression_button2
            // 
            this.Compression_button2.BackColor = System.Drawing.Color.Gainsboro;
            this.Compression_button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Compression_button2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Compression_button2.ForeColor = System.Drawing.Color.Black;
            this.Compression_button2.Location = new System.Drawing.Point(13, 19);
            this.Compression_button2.Name = "Compression_button2";
            this.Compression_button2.Size = new System.Drawing.Size(120, 23);
            this.Compression_button2.TabIndex = 160;
            this.Compression_button2.Text = "Compression OFF";
            this.Compression_button2.UseVisualStyleBackColor = false;
            this.Compression_button2.Click += new System.EventHandler(this.Compression_button2_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Gainsboro;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(265, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 23);
            this.label1.TabIndex = 180;
            this.label1.Text = "VOLUME ATTN";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Compression_label44
            // 
            this.Compression_label44.BackColor = System.Drawing.Color.Gainsboro;
            this.Compression_label44.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Compression_label44.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Compression_label44.ForeColor = System.Drawing.Color.Black;
            this.Compression_label44.Location = new System.Drawing.Point(50, 70);
            this.Compression_label44.Name = "Compression_label44";
            this.Compression_label44.Size = new System.Drawing.Size(47, 15);
            this.Compression_label44.TabIndex = 162;
            this.Compression_label44.Text = "0";
            this.Compression_label44.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Mic_Gain_label2
            // 
            this.Mic_Gain_label2.BackColor = System.Drawing.Color.Gainsboro;
            this.Mic_Gain_label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Mic_Gain_label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Mic_Gain_label2.ForeColor = System.Drawing.Color.Black;
            this.Mic_Gain_label2.Location = new System.Drawing.Point(139, 19);
            this.Mic_Gain_label2.Name = "Mic_Gain_label2";
            this.Mic_Gain_label2.Size = new System.Drawing.Size(120, 23);
            this.Mic_Gain_label2.TabIndex = 164;
            this.Mic_Gain_label2.Text = "MIC PRE GAIN";
            this.Mic_Gain_label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Mic_Gain_label2.Click += new System.EventHandler(this.Mic_Gain_label2_Click);
            // 
            // Mic_Gain_Step_listBox1
            // 
            this.Mic_Gain_Step_listBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.Mic_Gain_Step_listBox1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Mic_Gain_Step_listBox1.ForeColor = System.Drawing.Color.Black;
            this.Mic_Gain_Step_listBox1.FormattingEnabled = true;
            this.Mic_Gain_Step_listBox1.ItemHeight = 16;
            this.Mic_Gain_Step_listBox1.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.Mic_Gain_Step_listBox1.Location = new System.Drawing.Point(178, 45);
            this.Mic_Gain_Step_listBox1.Name = "Mic_Gain_Step_listBox1";
            this.Mic_Gain_Step_listBox1.Size = new System.Drawing.Size(42, 20);
            this.Mic_Gain_Step_listBox1.TabIndex = 165;
            this.Mic_Gain_Step_listBox1.SelectedIndexChanged += new System.EventHandler(this.Mic_Gain_Step_listBox1_SelectedIndexChanged);
            // 
            // Time_checkBox2
            // 
            this.Time_checkBox2.BackColor = System.Drawing.Color.Gainsboro;
            this.Time_checkBox2.Enabled = false;
            this.Time_checkBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Time_checkBox2.ForeColor = System.Drawing.Color.Black;
            this.Time_checkBox2.Location = new System.Drawing.Point(683, 8);
            this.Time_checkBox2.Name = "Time_checkBox2";
            this.Time_checkBox2.Size = new System.Drawing.Size(104, 20);
            this.Time_checkBox2.TabIndex = 179;
            this.Time_checkBox2.Text = "Time Display";
            this.toolTip.SetToolTip(this.Time_checkBox2, "Auto Zero");
            this.Time_checkBox2.UseVisualStyleBackColor = false;
            this.Time_checkBox2.CheckedChanged += new System.EventHandler(this.Time_checkBox2_CheckedChanged);
            // 
            // SMeter_groupBox4
            // 
            this.SMeter_groupBox4.Controls.Add(this.Meter_Peak_Needle_Color);
            this.SMeter_groupBox4.Controls.Add(this.Peak_Needle_Delay_listBox1);
            this.SMeter_groupBox4.Controls.Add(this.Peak_Needle_checkBox2);
            this.SMeter_groupBox4.Controls.Add(this.Peak_Hold_listBox1);
            this.SMeter_groupBox4.Controls.Add(this.Peak_hold_on_offlistBox1);
            this.SMeter_groupBox4.ForeColor = System.Drawing.Color.White;
            this.SMeter_groupBox4.Location = new System.Drawing.Point(609, 134);
            this.SMeter_groupBox4.Name = "SMeter_groupBox4";
            this.SMeter_groupBox4.Size = new System.Drawing.Size(175, 118);
            this.SMeter_groupBox4.TabIndex = 178;
            this.SMeter_groupBox4.TabStop = false;
            this.SMeter_groupBox4.Text = "Meter";
            // 
            // Meter_Peak_Needle_Color
            // 
            this.Meter_Peak_Needle_Color.BackColor = System.Drawing.Color.Gainsboro;
            this.Meter_Peak_Needle_Color.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Meter_Peak_Needle_Color.ForeColor = System.Drawing.Color.Black;
            this.Meter_Peak_Needle_Color.FormattingEnabled = true;
            this.Meter_Peak_Needle_Color.ItemHeight = 16;
            this.Meter_Peak_Needle_Color.Items.AddRange(new object[] {
            "RED",
            "BLUE",
            "GREEN",
            "YELLOW",
            "WHITE",
            "BLACK"});
            this.Meter_Peak_Needle_Color.Location = new System.Drawing.Point(50, 83);
            this.Meter_Peak_Needle_Color.Name = "Meter_Peak_Needle_Color";
            this.Meter_Peak_Needle_Color.Size = new System.Drawing.Size(75, 20);
            this.Meter_Peak_Needle_Color.TabIndex = 183;
            this.toolTip.SetToolTip(this.Meter_Peak_Needle_Color, "Meter Peak Needle Color");
            this.Meter_Peak_Needle_Color.SelectedIndexChanged += new System.EventHandler(this.Meter_Peak_Needle_Color_SelectedIndexChanged);
            // 
            // Peak_Needle_Delay_listBox1
            // 
            this.Peak_Needle_Delay_listBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.Peak_Needle_Delay_listBox1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Peak_Needle_Delay_listBox1.ForeColor = System.Drawing.Color.Black;
            this.Peak_Needle_Delay_listBox1.FormattingEnabled = true;
            this.Peak_Needle_Delay_listBox1.ItemHeight = 16;
            this.Peak_Needle_Delay_listBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.Peak_Needle_Delay_listBox1.Location = new System.Drawing.Point(101, 57);
            this.Peak_Needle_Delay_listBox1.Name = "Peak_Needle_Delay_listBox1";
            this.Peak_Needle_Delay_listBox1.Size = new System.Drawing.Size(42, 20);
            this.Peak_Needle_Delay_listBox1.TabIndex = 182;
            this.toolTip.SetToolTip(this.Peak_Needle_Delay_listBox1, "PEAK NEEDLE DECAY TIME");
            this.Peak_Needle_Delay_listBox1.SelectedIndexChanged += new System.EventHandler(this.Peak_Needle_Delay_listBox1_SelectedIndexChanged);
            // 
            // Peak_Needle_checkBox2
            // 
            this.Peak_Needle_checkBox2.AutoSize = true;
            this.Peak_Needle_checkBox2.BackColor = System.Drawing.Color.Gainsboro;
            this.Peak_Needle_checkBox2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Peak_Needle_checkBox2.ForeColor = System.Drawing.Color.Black;
            this.Peak_Needle_checkBox2.Location = new System.Drawing.Point(19, 57);
            this.Peak_Needle_checkBox2.Name = "Peak_Needle_checkBox2";
            this.Peak_Needle_checkBox2.Size = new System.Drawing.Size(58, 20);
            this.Peak_Needle_checkBox2.TabIndex = 178;
            this.Peak_Needle_checkBox2.Text = "Peak";
            this.toolTip.SetToolTip(this.Peak_Needle_checkBox2, "DISPLAY PEAK NEEDLE");
            this.Peak_Needle_checkBox2.UseVisualStyleBackColor = false;
            this.Peak_Needle_checkBox2.CheckedChanged += new System.EventHandler(this.Peak_Needle_checkBox2_CheckedChanged);
            // 
            // Peak_Hold_listBox1
            // 
            this.Peak_Hold_listBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.Peak_Hold_listBox1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Peak_Hold_listBox1.ForeColor = System.Drawing.Color.Black;
            this.Peak_Hold_listBox1.FormattingEnabled = true;
            this.Peak_Hold_listBox1.ItemHeight = 16;
            this.Peak_Hold_listBox1.Items.AddRange(new object[] {
            "10",
            "20",
            "30",
            "50",
            "100"});
            this.Peak_Hold_listBox1.Location = new System.Drawing.Point(27, 26);
            this.Peak_Hold_listBox1.Name = "Peak_Hold_listBox1";
            this.Peak_Hold_listBox1.ScrollAlwaysVisible = true;
            this.Peak_Hold_listBox1.Size = new System.Drawing.Size(43, 20);
            this.Peak_Hold_listBox1.TabIndex = 176;
            this.toolTip.SetToolTip(this.Peak_Hold_listBox1, "SMOOTHING DECAY TIME");
            this.Smeter_toolTip.SetToolTip(this.Peak_Hold_listBox1, "SMOOTHING DECAY TIME");
            this.Peak_Hold_listBox1.SelectedIndexChanged += new System.EventHandler(this.Peak_Hold_listBox1_SelectedIndexChanged_2);
            // 
            // Peak_hold_on_offlistBox1
            // 
            this.Peak_hold_on_offlistBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.Peak_hold_on_offlistBox1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Peak_hold_on_offlistBox1.ForeColor = System.Drawing.Color.Black;
            this.Peak_hold_on_offlistBox1.FormattingEnabled = true;
            this.Peak_hold_on_offlistBox1.ItemHeight = 16;
            this.Peak_hold_on_offlistBox1.Items.AddRange(new object[] {
            "OFF",
            "ON"});
            this.Peak_hold_on_offlistBox1.Location = new System.Drawing.Point(97, 26);
            this.Peak_hold_on_offlistBox1.Name = "Peak_hold_on_offlistBox1";
            this.Peak_hold_on_offlistBox1.ScrollAlwaysVisible = true;
            this.Peak_hold_on_offlistBox1.Size = new System.Drawing.Size(50, 20);
            this.Peak_hold_on_offlistBox1.TabIndex = 177;
            this.toolTip.SetToolTip(this.Peak_hold_on_offlistBox1, "SMOOTHING");
            this.Smeter_toolTip.SetToolTip(this.Peak_hold_on_offlistBox1, "SMOOTHING");
            this.Peak_hold_on_offlistBox1.SelectedIndexChanged += new System.EventHandler(this.Peak_hold_on_offlistBox1_SelectedIndexChanged_1);
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.Gainsboro;
            this.button5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.ForeColor = System.Drawing.Color.Black;
            this.button5.Location = new System.Drawing.Point(7, 58);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(87, 22);
            this.button5.TabIndex = 168;
            this.button5.Text = "MOVE";
            this.toolTip.SetToolTip(this.button5, "Click and hold this button and the move the mouse\r\nto move MSCC to another locati" +
        "on on the screen.\r\n");
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            this.button5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Move_Window);
            this.button5.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Move_Window_MouseMove);
            this.button5.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Move_Window_MouseUP);
            // 
            // RPi_Temperature_label1
            // 
            this.RPi_Temperature_label1.BackColor = System.Drawing.Color.Gainsboro;
            this.RPi_Temperature_label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.RPi_Temperature_label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RPi_Temperature_label1.ForeColor = System.Drawing.Color.Black;
            this.RPi_Temperature_label1.Location = new System.Drawing.Point(220, 338);
            this.RPi_Temperature_label1.Name = "RPi_Temperature_label1";
            this.RPi_Temperature_label1.Size = new System.Drawing.Size(162, 23);
            this.RPi_Temperature_label1.TabIndex = 166;
            this.RPi_Temperature_label1.Text = "Processor: 0°C";
            this.RPi_Temperature_label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.RPi_Temperature_label1.Click += new System.EventHandler(this.RPi_Temperature_label1_Click);
            // 
            // Colors_groupBox
            // 
            this.Colors_groupBox.Controls.Add(this.button3);
            this.Colors_groupBox.Controls.Add(this.panel1);
            this.Colors_groupBox.Controls.Add(this.Freq_Color_button4);
            this.Colors_groupBox.Controls.Add(this.Boarder_Color_button4);
            this.Colors_groupBox.ForeColor = System.Drawing.Color.White;
            this.Colors_groupBox.Location = new System.Drawing.Point(7, 134);
            this.Colors_groupBox.Name = "Colors_groupBox";
            this.Colors_groupBox.Size = new System.Drawing.Size(164, 118);
            this.Colors_groupBox.TabIndex = 156;
            this.Colors_groupBox.TabStop = false;
            this.Colors_groupBox.Text = "Main Colors";
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Gainsboro;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.Black;
            this.button3.Location = new System.Drawing.Point(6, 21);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(108, 29);
            this.button3.TabIndex = 148;
            this.button3.Text = "BACKGROUND";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gainsboro;
            this.panel1.Controls.Add(this.Freq_Digit_Test_label58);
            this.panel1.ForeColor = System.Drawing.Color.IndianRed;
            this.panel1.Location = new System.Drawing.Point(120, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(38, 51);
            this.panel1.TabIndex = 154;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // Freq_Digit_Test_label58
            // 
            this.Freq_Digit_Test_label58.AutoSize = true;
            this.Freq_Digit_Test_label58.BackColor = System.Drawing.Color.Gainsboro;
            this.Freq_Digit_Test_label58.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Freq_Digit_Test_label58.ForeColor = System.Drawing.Color.Black;
            this.Freq_Digit_Test_label58.Location = new System.Drawing.Point(4, 14);
            this.Freq_Digit_Test_label58.Name = "Freq_Digit_Test_label58";
            this.Freq_Digit_Test_label58.Size = new System.Drawing.Size(25, 25);
            this.Freq_Digit_Test_label58.TabIndex = 151;
            this.Freq_Digit_Test_label58.Text = "8";
            this.Freq_Digit_Test_label58.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Freq_Digit_Test_label58.Click += new System.EventHandler(this.Freq_Digit_Test_label58_Click);
            // 
            // Freq_Color_button4
            // 
            this.Freq_Color_button4.BackColor = System.Drawing.Color.Gainsboro;
            this.Freq_Color_button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Freq_Color_button4.ForeColor = System.Drawing.Color.Black;
            this.Freq_Color_button4.Location = new System.Drawing.Point(6, 54);
            this.Freq_Color_button4.Name = "Freq_Color_button4";
            this.Freq_Color_button4.Size = new System.Drawing.Size(108, 25);
            this.Freq_Color_button4.TabIndex = 150;
            this.Freq_Color_button4.Text = "TEXT";
            this.Freq_Color_button4.UseVisualStyleBackColor = false;
            this.Freq_Color_button4.Click += new System.EventHandler(this.Freq_Color_button4_Click);
            // 
            // Boarder_Color_button4
            // 
            this.Boarder_Color_button4.BackColor = System.Drawing.Color.Gainsboro;
            this.Boarder_Color_button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Boarder_Color_button4.ForeColor = System.Drawing.Color.Black;
            this.Boarder_Color_button4.Location = new System.Drawing.Point(6, 83);
            this.Boarder_Color_button4.Name = "Boarder_Color_button4";
            this.Boarder_Color_button4.Size = new System.Drawing.Size(108, 24);
            this.Boarder_Color_button4.TabIndex = 153;
            this.Boarder_Color_button4.Text = "BORDER";
            this.Boarder_Color_button4.UseVisualStyleBackColor = false;
            this.Boarder_Color_button4.Click += new System.EventHandler(this.Boarder_Color_button4_Click);
            // 
            // AMP_Current_label5
            // 
            this.AMP_Current_label5.BackColor = System.Drawing.Color.Gainsboro;
            this.AMP_Current_label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.AMP_Current_label5.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AMP_Current_label5.ForeColor = System.Drawing.Color.Black;
            this.AMP_Current_label5.Location = new System.Drawing.Point(602, 338);
            this.AMP_Current_label5.Name = "AMP_Current_label5";
            this.AMP_Current_label5.Size = new System.Drawing.Size(162, 23);
            this.AMP_Current_label5.TabIndex = 145;
            this.AMP_Current_label5.Text = "PA I (ma): 00000 ma";
            this.AMP_Current_label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.AMP_Current_label5.Click += new System.EventHandler(this.AMP_Current_label5_Click);
            // 
            // Amplifier_temperature_label58
            // 
            this.Amplifier_temperature_label58.BackColor = System.Drawing.Color.Gainsboro;
            this.Amplifier_temperature_label58.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Amplifier_temperature_label58.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Amplifier_temperature_label58.ForeColor = System.Drawing.Color.Black;
            this.Amplifier_temperature_label58.Location = new System.Drawing.Point(411, 338);
            this.Amplifier_temperature_label58.Name = "Amplifier_temperature_label58";
            this.Amplifier_temperature_label58.Size = new System.Drawing.Size(162, 23);
            this.Amplifier_temperature_label58.TabIndex = 144;
            this.Amplifier_temperature_label58.Text = "Amplifier PA: 0°C";
            this.Amplifier_temperature_label58.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Amplifier_temperature_label58.Click += new System.EventHandler(this.Amplifier_temperature_label58_Click);
            // 
            // Temperature_label57
            // 
            this.Temperature_label57.BackColor = System.Drawing.Color.Gainsboro;
            this.Temperature_label57.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Temperature_label57.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Temperature_label57.ForeColor = System.Drawing.Color.Black;
            this.Temperature_label57.Location = new System.Drawing.Point(29, 338);
            this.Temperature_label57.Name = "Temperature_label57";
            this.Temperature_label57.Size = new System.Drawing.Size(162, 23);
            this.Temperature_label57.TabIndex = 141;
            this.Temperature_label57.Text = "Transceiver: WARMING";
            this.Temperature_label57.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.Temperature_label57, "10 minute warm cycle after the transceiver is powered on");
            this.Temperature_label57.Click += new System.EventHandler(this.Temperature_label57_Click_1);
            // 
            // Relay_Board_checkBox2
            // 
            this.Relay_Board_checkBox2.AutoSize = true;
            this.Relay_Board_checkBox2.BackColor = System.Drawing.Color.Gainsboro;
            this.Relay_Board_checkBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Relay_Board_checkBox2.ForeColor = System.Drawing.Color.Black;
            this.Relay_Board_checkBox2.Location = new System.Drawing.Point(165, 32);
            this.Relay_Board_checkBox2.Name = "Relay_Board_checkBox2";
            this.Relay_Board_checkBox2.Size = new System.Drawing.Size(122, 20);
            this.Relay_Board_checkBox2.TabIndex = 131;
            this.Relay_Board_checkBox2.Text = "RELAY BOARD";
            this.Relay_Board_checkBox2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Relay_Board_checkBox2.UseVisualStyleBackColor = false;
            this.Relay_Board_checkBox2.Visible = false;
            this.Relay_Board_checkBox2.CheckedChanged += new System.EventHandler(this.Relay_Board_checkBox2_CheckedChanged);
            // 
            // MSCC_Core_Version_label45
            // 
            this.MSCC_Core_Version_label45.BackColor = System.Drawing.Color.Gainsboro;
            this.MSCC_Core_Version_label45.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.MSCC_Core_Version_label45.Location = new System.Drawing.Point(39, 417);
            this.MSCC_Core_Version_label45.Name = "MSCC_Core_Version_label45";
            this.MSCC_Core_Version_label45.Size = new System.Drawing.Size(123, 23);
            this.MSCC_Core_Version_label45.TabIndex = 126;
            this.MSCC_Core_Version_label45.Text = "Firmware Version: ";
            this.MSCC_Core_Version_label45.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.MSCC_Core_Version_label45.Click += new System.EventHandler(this.MSCC_Core_Version_label45_Click);
            // 
            // MSCC_Display_label44
            // 
            this.MSCC_Display_label44.BackColor = System.Drawing.Color.Gainsboro;
            this.MSCC_Display_label44.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.MSCC_Display_label44.Location = new System.Drawing.Point(48, 381);
            this.MSCC_Display_label44.Name = "MSCC_Display_label44";
            this.MSCC_Display_label44.Size = new System.Drawing.Size(105, 23);
            this.MSCC_Display_label44.TabIndex = 125;
            this.MSCC_Display_label44.Text = "MSCC";
            this.MSCC_Display_label44.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // HR50_PPT_listBox1
            // 
            this.HR50_PPT_listBox1.BackColor = System.Drawing.Color.White;
            this.HR50_PPT_listBox1.Enabled = false;
            this.HR50_PPT_listBox1.ForeColor = System.Drawing.Color.Black;
            this.HR50_PPT_listBox1.FormattingEnabled = true;
            this.HR50_PPT_listBox1.ItemHeight = 16;
            this.HR50_PPT_listBox1.Items.AddRange(new object[] {
            "PPT",
            "COR",
            "QRP"});
            this.HR50_PPT_listBox1.Location = new System.Drawing.Point(476, 60);
            this.HR50_PPT_listBox1.Name = "HR50_PPT_listBox1";
            this.HR50_PPT_listBox1.ScrollAlwaysVisible = true;
            this.HR50_PPT_listBox1.Size = new System.Drawing.Size(84, 20);
            this.HR50_PPT_listBox1.TabIndex = 124;
            this.toolTip.SetToolTip(this.HR50_PPT_listBox1, "Sets the HR50 Keying Mode.\r\nPTT - HR50 is keyed via the Proficio AMP port.\r\nCOR -" +
        " HR50 is keyed via Carrier Operated Relay.\r\nQRP - ??\r\n");
            this.HR50_PPT_listBox1.Visible = false;
            this.HR50_PPT_listBox1.SelectedIndexChanged += new System.EventHandler(this.HR50_PPT_listBox1_SelectedIndexChanged);
            // 
            // HR50_checkBox2
            // 
            this.HR50_checkBox2.AutoSize = true;
            this.HR50_checkBox2.BackColor = System.Drawing.Color.Gainsboro;
            this.HR50_checkBox2.Enabled = false;
            this.HR50_checkBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HR50_checkBox2.ForeColor = System.Drawing.Color.Black;
            this.HR50_checkBox2.Location = new System.Drawing.Point(276, 77);
            this.HR50_checkBox2.Name = "HR50_checkBox2";
            this.HR50_checkBox2.Size = new System.Drawing.Size(91, 20);
            this.HR50_checkBox2.TabIndex = 123;
            this.HR50_checkBox2.Text = "BY PASS";
            this.HR50_checkBox2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.HR50_checkBox2, "Sets the HR50 into By Pass Mode.\r\nThe HR50 will NOT key.");
            this.HR50_checkBox2.UseVisualStyleBackColor = false;
            this.HR50_checkBox2.Visible = false;
            this.HR50_checkBox2.CheckedChanged += new System.EventHandler(this.HR50_checkBox2_CheckedChanged);
            // 
            // label16
            // 
            this.label16.BackColor = System.Drawing.Color.Gainsboro;
            this.label16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label16.Enabled = false;
            this.label16.Location = new System.Drawing.Point(595, 98);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(136, 23);
            this.label16.TabIndex = 122;
            this.label16.Text = "HARDROCK 50";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.label16, "Set the Hardrock 50 to:\r\nBaudrate: 9600\r\nParity: NONE\r\nDatabits: 8\r\nStopbits: 1");
            this.label16.Visible = false;
            // 
            // HR50_listBox1
            // 
            this.HR50_listBox1.BackColor = System.Drawing.Color.White;
            this.HR50_listBox1.Enabled = false;
            this.HR50_listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HR50_listBox1.ForeColor = System.Drawing.Color.Black;
            this.HR50_listBox1.FormattingEnabled = true;
            this.HR50_listBox1.ItemHeight = 15;
            this.HR50_listBox1.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4"});
            this.HR50_listBox1.Location = new System.Drawing.Point(466, 35);
            this.HR50_listBox1.Name = "HR50_listBox1";
            this.HR50_listBox1.Size = new System.Drawing.Size(94, 19);
            this.HR50_listBox1.TabIndex = 121;
            this.toolTip.SetToolTip(this.HR50_listBox1, "Select Port Attached to the HR 50\r\nSet the Hardrock 50 to:\r\nBaudrate: 9600\r\nParit" +
        "y: NONE\r\nDatabits: 8\r\nStopbits: 1\r\n");
            this.HR50_listBox1.Visible = false;
            this.HR50_listBox1.SelectedIndexChanged += new System.EventHandler(this.HR50_listBox1_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Gainsboro;
            this.button2.Enabled = false;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.Location = new System.Drawing.Point(429, 99);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(94, 22);
            this.button2.TabIndex = 83;
            this.button2.Text = "CLOSED";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Zip_button2
            // 
            this.Zip_button2.BackColor = System.Drawing.Color.Gainsboro;
            this.Zip_button2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Zip_button2.ForeColor = System.Drawing.Color.Black;
            this.Zip_button2.Location = new System.Drawing.Point(7, 6);
            this.Zip_button2.Name = "Zip_button2";
            this.Zip_button2.Size = new System.Drawing.Size(87, 22);
            this.Zip_button2.TabIndex = 82;
            this.Zip_button2.Text = "Backup";
            this.toolTip.SetToolTip(this.Zip_button2, "Creates a zip file of files in the backup folder.\r\nDo this BEFORE stopping MSCC-C" +
        "ore.");
            this.Zip_button2.UseVisualStyleBackColor = false;
            this.Zip_button2.Click += new System.EventHandler(this.Zip_button2_Click);
            // 
            // Restore_button2
            // 
            this.Restore_button2.BackColor = System.Drawing.Color.Gainsboro;
            this.Restore_button2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Restore_button2.ForeColor = System.Drawing.Color.Black;
            this.Restore_button2.Location = new System.Drawing.Point(364, 46);
            this.Restore_button2.Name = "Restore_button2";
            this.Restore_button2.Size = new System.Drawing.Size(87, 25);
            this.Restore_button2.TabIndex = 81;
            this.Restore_button2.Text = "Initialize";
            this.toolTip.SetToolTip(this.Restore_button2, "Initializes ALL Configurations to Default Values");
            this.Restore_button2.UseVisualStyleBackColor = false;
            this.Restore_button2.Visible = false;
            this.Restore_button2.Click += new System.EventHandler(this.Restore_button2_Click);
            // 
            // SDRcore_Trans_Version
            // 
            this.SDRcore_Trans_Version.BackColor = System.Drawing.Color.Gainsboro;
            this.SDRcore_Trans_Version.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SDRcore_Trans_Version.Location = new System.Drawing.Point(486, 417);
            this.SDRcore_Trans_Version.Name = "SDRcore_Trans_Version";
            this.SDRcore_Trans_Version.Size = new System.Drawing.Size(123, 23);
            this.SDRcore_Trans_Version.TabIndex = 79;
            this.SDRcore_Trans_Version.Text = "Firmware Version: ";
            this.SDRcore_Trans_Version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.SDRcore_Trans_Version.Click += new System.EventHandler(this.SDRcore_Trans_Version_Click);
            // 
            // SDRcore_Recv_Version_label16
            // 
            this.SDRcore_Recv_Version_label16.BackColor = System.Drawing.Color.Gainsboro;
            this.SDRcore_Recv_Version_label16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SDRcore_Recv_Version_label16.Location = new System.Drawing.Point(337, 417);
            this.SDRcore_Recv_Version_label16.Name = "SDRcore_Recv_Version_label16";
            this.SDRcore_Recv_Version_label16.Size = new System.Drawing.Size(123, 23);
            this.SDRcore_Recv_Version_label16.TabIndex = 78;
            this.SDRcore_Recv_Version_label16.Text = "Firmware Version: ";
            this.SDRcore_Recv_Version_label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.SDRcore_Recv_Version_label16.Click += new System.EventHandler(this.SDRcore_Recv_Version_label16_Click);
            // 
            // MS_SDR_Version_label16
            // 
            this.MS_SDR_Version_label16.BackColor = System.Drawing.Color.Gainsboro;
            this.MS_SDR_Version_label16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.MS_SDR_Version_label16.Location = new System.Drawing.Point(188, 417);
            this.MS_SDR_Version_label16.Name = "MS_SDR_Version_label16";
            this.MS_SDR_Version_label16.Size = new System.Drawing.Size(123, 23);
            this.MS_SDR_Version_label16.TabIndex = 77;
            this.MS_SDR_Version_label16.Text = "Firmware Version: ";
            this.MS_SDR_Version_label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.MS_SDR_Version_label16.Click += new System.EventHandler(this.MS_SDR_Version_label16_Click);
            // 
            // label15
            // 
            this.label15.BackColor = System.Drawing.Color.Gainsboro;
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label15.Location = new System.Drawing.Point(495, 381);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(105, 23);
            this.label15.TabIndex = 76;
            this.label15.Text = "SDRcore-trans";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.Gainsboro;
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label12.Location = new System.Drawing.Point(346, 381);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(105, 23);
            this.label12.TabIndex = 75;
            this.label12.Text = "SDRcore-recv";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.Gainsboro;
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label11.Location = new System.Drawing.Point(197, 381);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(105, 23);
            this.label11.TabIndex = 74;
            this.label11.Text = "MS-sdr";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Gainsboro;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label6.Location = new System.Drawing.Point(644, 381);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(105, 23);
            this.label6.TabIndex = 73;
            this.label6.Text = "Firmware";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // firmwarelabel16
            // 
            this.firmwarelabel16.BackColor = System.Drawing.Color.Gainsboro;
            this.firmwarelabel16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.firmwarelabel16.Location = new System.Drawing.Point(635, 417);
            this.firmwarelabel16.Name = "firmwarelabel16";
            this.firmwarelabel16.Size = new System.Drawing.Size(123, 23);
            this.firmwarelabel16.TabIndex = 71;
            this.firmwarelabel16.Text = "Firmware Version: ";
            this.firmwarelabel16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.firmwarelabel16.Click += new System.EventHandler(this.firmwarelabel16_Click_1);
            // 
            // Transvertercheckbox
            // 
            this.Transvertercheckbox.AutoSize = true;
            this.Transvertercheckbox.BackColor = System.Drawing.Color.Gainsboro;
            this.Transvertercheckbox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Transvertercheckbox.ForeColor = System.Drawing.Color.Black;
            this.Transvertercheckbox.Location = new System.Drawing.Point(683, 35);
            this.Transvertercheckbox.Name = "Transvertercheckbox";
            this.Transvertercheckbox.Size = new System.Drawing.Size(92, 17);
            this.Transvertercheckbox.TabIndex = 69;
            this.Transvertercheckbox.Text = "Transverter";
            this.Transvertercheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.Transvertercheckbox, "Expands the 10M frequency range to 30MHz");
            this.Transvertercheckbox.UseVisualStyleBackColor = false;
            this.Transvertercheckbox.CheckedChanged += new System.EventHandler(this.Transvertercheckbox_CheckedChanged);
            // 
            // Monitorbutton
            // 
            this.Monitorbutton.BackColor = System.Drawing.Color.Gainsboro;
            this.Monitorbutton.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Monitorbutton.ForeColor = System.Drawing.Color.Black;
            this.Monitorbutton.Location = new System.Drawing.Point(7, 32);
            this.Monitorbutton.Name = "Monitorbutton";
            this.Monitorbutton.Size = new System.Drawing.Size(87, 22);
            this.Monitorbutton.TabIndex = 67;
            this.Monitorbutton.Text = "Log Monitor";
            this.Monitorbutton.UseVisualStyleBackColor = false;
            this.Monitorbutton.Click += new System.EventHandler(this.Monitorbutton_Click);
            // 
            // Keep_Alive_timer
            // 
            this.Keep_Alive_timer.Interval = 250;
            this.Keep_Alive_timer.Tick += new System.EventHandler(this.Keep_Alive_timer1_Tick);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 32000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            this.toolTip.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip_Popup);
            // 
            // timer2
            // 
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timer3
            // 
            this.timer3.Interval = 500;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // Freq_Cal_timer4
            // 
            this.Freq_Cal_timer4.Interval = 500;
            this.Freq_Cal_timer4.Tick += new System.EventHandler(this.Freq_Cal_timer4_Tick);
            // 
            // colorDialog1
            // 
            this.colorDialog1.AnyColor = true;
            this.colorDialog1.FullOpen = true;
            // 
            // fontDialog1
            // 
            this.fontDialog1.Apply += new System.EventHandler(this.fontDialog1_Apply);
            // 
            // Window_Refresh_timer
            // 
            this.Window_Refresh_timer.Enabled = true;
            this.Window_Refresh_timer.Tick += new System.EventHandler(this.Window_Refresh_timer_Tick);
            // 
            // Smeter_Timer
            // 
            this.Smeter_Timer.Enabled = true;
            this.Smeter_Timer.Interval = 20;
            this.Smeter_Timer.Tick += new System.EventHandler(this.Smeter_Timer_Tick);
            // 
            // Smeter_toolTip
            // 
            this.Smeter_toolTip.AutoPopDelay = 32000;
            this.Smeter_toolTip.InitialDelay = 500;
            this.Smeter_toolTip.ReshowDelay = 100;
            // 
            // Cursor_timer2
            // 
            this.Cursor_timer2.Interval = 5000;
            // 
            // Waterfall_timer
            // 
            this.Waterfall_timer.Tick += new System.EventHandler(this.Waterfall_timer_Tick);
            // 
            // RPi_Display_Timer
            // 
            this.RPi_Display_Timer.Tick += new System.EventHandler(this.RPi_Display_Timer_Tick);
            // 
            // Main_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.powertabControl1);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main_form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MSCC SOLIDUS";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Main_MouseWheel);
            this.Resize += new System.EventHandler(this.Frm_state);
            this.powertabControl1.ResumeLayout(false);
            this.mainPage.ResumeLayout(false);
            this.mainPage.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.RIT_groupBox4.ResumeLayout(false);
            this.RIT_groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWaterfall)).EndInit();
            this.TX.ResumeLayout(false);
            this.TX.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Power_Meter_Hold)).EndInit();
            this.band_stack.ResumeLayout(false);
            this.band_stack.PerformLayout();
            this.freqcaltab.ResumeLayout(false);
            this.IQ_groupBox3.ResumeLayout(false);
            this.IQ_groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.Freq_Cal_groupBox4.ResumeLayout(false);
            this.Freq_Cal_groupBox4.PerformLayout();
            this.powertabPage1.ResumeLayout(false);
            this.powertabPage1.PerformLayout();
            this.Audio_tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CW_Hold_numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CW_Lag_numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.MFC.ResumeLayout(false);
            this.MFC.PerformLayout();
            this.Tuning_Knob_groupBox1.ResumeLayout(false);
            this.Tuning_Knob_groupBox1.PerformLayout();
            this.AMP_groupBox3.ResumeLayout(false);
            this.IQBD_groupBox4.ResumeLayout(false);
            this.metertab.ResumeLayout(false);
            this.metertab.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.SMeter_groupBox4.ResumeLayout(false);
            this.SMeter_groupBox4.PerformLayout();
            this.Colors_groupBox.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

#endregion
        private System.Windows.Forms.TabPage mainPage;
        private System.Windows.Forms.HScrollBar ritScroll;
        private System.Windows.Forms.Button buttReset;
        private System.Windows.Forms.TabControl powertabControl1;
        private System.Windows.Forms.TabPage powertabPage1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.HScrollBar powerhScrollBar1;
        private System.Windows.Forms.Label powerlabel14;
        private System.Windows.Forms.Button ritbutton1;
        private System.Windows.Forms.TabPage freqcaltab;
        private System.Windows.Forms.Button calibratebutton1;
        private System.Windows.Forms.Button powerrestorebutton2;
        private System.Windows.Forms.Button powertunebutton1;
        private System.Windows.Forms.TabPage band_stack;
        private System.Windows.Forms.Button band_stack_update_button1;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label band_stack_label29;
        private System.Windows.Forms.TextBox band_stack_textBox1;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Button power_slider_reset_button1;
        private System.Windows.Forms.Button mainmodebutton2;
        private System.Windows.Forms.RadioButton main10radioButton1;
        private System.Windows.Forms.RadioButton main12radioButton2;
        private System.Windows.Forms.RadioButton main15radiobutton;
        private System.Windows.Forms.RadioButton main17radioButton4;
        private System.Windows.Forms.RadioButton main20radioButton5;
        private System.Windows.Forms.RadioButton main30radioButton6;
        private System.Windows.Forms.RadioButton main40radioButton7;
        private System.Windows.Forms.RadioButton main60radioButton8;
        private System.Windows.Forms.RadioButton main80radioButton9;
        private System.Windows.Forms.RadioButton main160radioButton10;
        private System.Windows.Forms.RadioButton genradioButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button Freqbutton3;
        private System.Windows.Forms.Label Thousands;
        private System.Windows.Forms.Label Tenmillions;
        private System.Windows.Forms.Label Millions;
        private System.Windows.Forms.Label Hundredthousand;
        private System.Windows.Forms.Label Tenthousands;
        private System.Windows.Forms.Label Hundreds;
        private System.Windows.Forms.Label Tens;
        private System.Windows.Forms.Label Ones;
        private System.Windows.Forms.Button buttTune;
        private System.Windows.Forms.RadioButton B160radioButton;
        private System.Windows.Forms.RadioButton B10radioButton;
        private System.Windows.Forms.RadioButton B12radioButton;
        private System.Windows.Forms.RadioButton B15radioButton;
        private System.Windows.Forms.RadioButton B17radioButton;
        private System.Windows.Forms.RadioButton B20radioButton;
        private System.Windows.Forms.RadioButton B30radioButton;
        private System.Windows.Forms.RadioButton B40radioButton;
        private System.Windows.Forms.RadioButton B60radioButton;
        private System.Windows.Forms.RadioButton B80radioButton;
        private System.Windows.Forms.HScrollBar MicVolume_hScrollBar1;
        private System.Windows.Forms.HScrollBar Volume_hScrollBar1;
        private System.Windows.Forms.TabPage TX;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button TX_Mute_button2;
        private System.Windows.Forms.Button Volume_Mute_button2;
        private System.Windows.Forms.Label AM_Carrier_label2;
        private System.Windows.Forms.HScrollBar AM_Carrier_hScrollBar1;
        private System.Windows.Forms.TabPage Audio_tabPage1;
        private System.Windows.Forms.Label Time_display_label33;
        private System.Windows.Forms.Timer Keep_Alive_timer;
        private System.Windows.Forms.Label Time_display_UTC_label34;
        private System.Windows.Forms.ListBox TX_Bandwidth_listBox1;
        private System.Windows.Forms.HScrollBar Power_hScrollBar1;
        private System.Windows.Forms.Label Power_label34;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.HScrollBar CW_Power_hScrollBar1;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label CW_Power_label36;
        private System.Windows.Forms.Label SSB_Power_label36;
        private System.Windows.Forms.Label AM_Carrier_label36;
        private System.Windows.Forms.Label Tune_Power_label37;
        private System.Windows.Forms.HScrollBar Tune_Power_hScrollBar1;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button LeftResetbutton2;
        private System.Windows.Forms.HScrollBar LefthScrollBar1;
        private System.Windows.Forms.Button IQ_Commit_button2;
        private System.Windows.Forms.Button IQ_Tune_button2;
        private System.Windows.Forms.TextBox IQLefttextBox2;
        private System.Windows.Forms.RadioButton IQ10_radioButton1;
        private System.Windows.Forms.RadioButton IQ12_radioButton2;
        private System.Windows.Forms.RadioButton IQ15_radioButton3;
        private System.Windows.Forms.RadioButton IQ17_radioButton4;
        private System.Windows.Forms.RadioButton IQ20_radioButton5;
        private System.Windows.Forms.RadioButton IQ30_radioButton6;
        private System.Windows.Forms.RadioButton IQ40_radioButton7;
        private System.Windows.Forms.RadioButton IQ60_radioButton8;
        private System.Windows.Forms.RadioButton IQ80_radioButton9;
        private System.Windows.Forms.RadioButton IQ160_radioButton10;
        private System.Windows.Forms.HScrollBar NB_hScrollBar1;
        private System.Windows.Forms.HScrollBar NR_hScrollBar1;
        private System.Windows.Forms.Label NR_label2;
        private System.Windows.Forms.ListBox AGC_listBox1;
        private System.Windows.Forms.Label AGC_label2;
        private Button IQ_TX_button;
        private Button IQ_RX_button;
        private System.Windows.Forms.Timer timer2;
        private GroupBox groupBox2;
        private Button IQ_Reset_All_button2;
        private GroupBox Freq_Cal_groupBox4;
        private GroupBox IQ_groupBox3;
        private ProgressBar Calibration_progressBar1;
        private ListBox Standard_Carrier_listBox1;
        private ListView B160_Favs_listView1;
        private ListView B80_Favs_listView1;
        private ListView B60_Favs_listView1;
        private ListView B10_Favs_listView1;
        private ListView B12_Favs_listView1;
        private ListView B15_Favs_listView1;
        private ListView B17_Favs_listView1;
        private ListView B20_Favs_listView1;
        private ListView B30_Favs_listView1;
        private ListView B40_Favs_listView1;
        private TextBox Favorites_textBox2;
        private Button Million_Bottom_button7;
        private Button Hundred_Thousand_Button_button6;
        private Button Tenthousand_Bottom_button5;
        private Button Thousand_Bottom_button;
        private Button Hundred_Bottom_button3;
        private Button Tens_Bottom_button2;
        private Button Ones_Bottom_button2;
        private Button Ones_Top_button2;
        private Button Ten_Million_Top_button3;
        private Button Million_Top_button4;
        private Button Hundred_Thousand_Top_button5;
        private Button Tenthousand_Top_button6;
        private Button Thousand_Top_button7;
        private Button Hundreds_Top_button8;
        private Button Tens_Top_button;
        private CheckBox checkBox1;
        private Button NR_button3;
        private Button NB_button2;
        private Button NB_ON_OFF_button2;
        private Label NB_Threshold_label16;
        private HScrollBar NB_Threshold_hScrollBar1;
        private Label NB_Width_label16;
        private Label NB_Threshold_label1;
        private Label NB_label16;
        private TabPage metertab;
        private Label SDRcore_Trans_Version;
        private Label SDRcore_Recv_Version_label16;
        private Label MS_SDR_Version_label16;
        private Label label15;
        private Label label12;
        private Label label11;
        private Label label6;
        private Label firmwarelabel16;
        private CheckBox Transvertercheckbox;
        private Button Monitorbutton;
        private Button button2;
        private ListBox HR50_listBox1;
        private Label label16;
        private CheckBox HR50_checkBox2;
        private ListBox HR50_PPT_listBox1;
        private Label StartUP_label44;
        private System.Windows.Forms.Timer timer3;
        private Button Zip_button2;
        private Button Restore_button2;
        private Label MSCC_Core_Version_label45;
        private Label MSCC_Display_label44;
        private TextBox Cal_Freq_textBox2;
        private Button PA_vButton1;
        private Button Tune_vButton2;
        private HScrollBar IQ_Freq_hScrollBar1;
        private CheckBox IQ_UP24KHz_checkBox2;
        private Button Reset_Freq_button3;
        //private Nevron.UI.WinForm.Docking.NDockManager nDockManager1;
        //private Nevron.UI.WinForm.Controls.NButton Test_nButton1;
        //private Nevron.UI.WinForm.Controls.NButton nButton1;
        private Label UTC_Date_label46;
        private Label Local_Date_label46;
        private Button Freq_Check_Button;
        private ListBox Filter_listBox1;
        private ListBox CW_Filter_listBox1;
        private ListBox Filter_Low_listBox1;
        private Label label53;
        private ListBox Default_High_Cut_listBox1;
        private Label label52;
        private ListBox Default_CW_Filter_listBox1;
        private Label label51;
        private ListBox Default_Low_Cut_listBox1;
        private Label label50;
        private CheckBox Relay_Board_checkBox2;
        private HScrollBar AGC_hScrollBar1;
        private Label label55;
        private Label label56;
        private Label AGC_label57;
        private TabPage MFC;
        private Button AMP_Tune_button4;
        private HScrollBar AMP_hScrollBar1;
        private Label AMP_label57;
        private GroupBox AMP_groupBox3;
        private Label label57;
        private Label Temperature_label57;
        private CheckBox Auto_Zero_checkBox2;
        private System.Windows.Forms.Timer Freq_Cal_timer4;
        private Label Freq_Cal_label59;
        private CheckBox Band_Change_Auto_Tune_checkBox2;
        private Label Amplifier_temperature_label58;
        private Label AMP_Current_label5;
        private HScrollBar AMP_Calibrate_hScrollBar1;
        private Label AMP_Temp_MFC_AMP_label5;
        private Label AMP_Calibration_label58;
        private Label AMP_Power_Output_label5;
        private Button NR_Button;
        private Label NR_label5;
        private HScrollBar IQ_Tune_Power_hScrollBar1;
        private Label AMP_Raw_Bias_label5;
        private Label Power_calibration_label58;
        private CheckBox Full_Power_checkBox1;
        private Button button3;
        private ColorDialog colorDialog1;
        private Button Boarder_Color_button4;
        private Label Freq_Digit_Test_label58;
        private Button Freq_Color_button4;
        private Panel panel1;
        private GroupBox groupBox3;
        private Label Decimal_label58;
        private Label Decimal_label59;
        private GroupBox Colors_groupBox;
        private Button Compression_button4;
        private Button ACG_button;
        private Label Reverse_Power_label43;
        private Label Forward_Power_label43;
        private Label SWR_Value_label43;
        private Label Reverse_label58;
        private Label Forward_label43;
        private VU_MeterLibrary.VuMeter Reverse_Meter;
        private Label SWR_label1;
        private VU_MeterLibrary.VuMeter Forward_Meter;
        private GroupBox groupBox1;
        private Label label8;
        private ListBox CW_Pitch_listBox1;
        private Label label35;
        private CheckBox semicheckBox2;
        private NumericUpDown CW_Hold_numericUpDown2;
        private NumericUpDown CW_Lag_numericUpDown2;
        private NumericUpDown numericUpDown1;
        private ListBox CW_Paddle_listBox1;
        private ListBox CW_Weight_listBox1;
        private ListBox CW_Space_listBox1;
        private ListBox CW_Type_listBox1;
        private ListBox CW_Mode_listBox1;
        private Label label61;
        private Label label60;
        private Label label59;
        private Label label58;
        private Label label45;
        private Label label43;
        private Label label5;
        private Button Compression_button2;
        private HScrollBar Compression_Level_hScrollBar1;
        private Label Compression_label44;
        private CheckBox Freq_Cal_checkBox3;
        private Button Freq_Cal_Reset_button4;
        private Label Freq_CAl_Progress_Lable;
        private Label Mic_Gain_label2;
        private ListBox Mic_Gain_Step_listBox1;
        private Label Antenna_Switch_label43;
        private ComboBox Antenna_Switch_comboBox1;
        private GroupBox Tuning_Knob_groupBox1;
        private Label label39;
        private Label label37;
        private Label label2;
        private Label label46;
        private ComboBox Right_Button_comboBox4;
        private ComboBox Knob_comboBox1;
        private ComboBox Left_Button_comboBox2;
        private ComboBox Middle_Button_comboBox3;
        private Label Meter_hold_label43;
        private NumericUpDown Power_Meter_Hold;
        private Label RPi_Temperature_label1;
        private FontDialog fontDialog1;
        private Button button4;
        private Button button5;
        private System.Windows.Forms.Timer Window_Refresh_timer;
        private ToolTip Smeter_toolTip;
        private Label Power_Value_label2;
        private Label label9;
        private System.Windows.Forms.Timer Smeter_Timer;
        public VU_MeterLibrary.VuMeter vuMeter1;
        private ListBox Peak_Hold_listBox1;
        private ListBox Peak_hold_on_offlistBox1;
        private Label label17;
        private ListBox ritoffsetlistBox1;
        private Label label7;
        private ListBox ritlistBox1;
        private Button button6;
        private GroupBox RIT_groupBox4;
        public PictureBox picWaterfall;
        private System.Windows.Forms.Timer Cursor_timer2;
        private System.Windows.Forms.Timer Waterfall_timer;
        public ZedGraph.ZedGraphControl Zedgraph_Control;
        private Button Spectrum_Controls_button3;
        private GroupBox SMeter_groupBox4;
        private Panel panel2;
        private Button button7;
        private Button Audio_Digital_button3;
        private CheckBox Time_checkBox2;
        private Button Meter_Mode_button8;
        private ListBox Volume_Attn_listBox1;
        private Label label1;
        private Label label3;
        private ListView General_listView1;
        private Button TenMillion_Bottom_button8;
        private Button Solidus_Bias_button8;
        private CheckBox Peak_Needle_checkBox2;
        private HScrollBar Main_Power_hScrollBar1;
        private CheckBox Minimize_checkBox2;
        private TextBox ritfreqtextBox1;
        private ListBox Peak_Needle_Delay_listBox1;
        private ListBox Meter_Peak_Needle_Color;
        private Button IQBD_Tune_button8;
        private Label IQBD_Monitor_label;
        private Button IQBD_ONOFF;
        private HScrollBar IQBD_hScrollBar2;
        private HScrollBar IQBD_hScrollBar1;
        private Button IQBD_Apply_button8;
        private GroupBox IQBD_groupBox4;
        private Label label4;
        private Label label18;
        private Label Solidus_Tab_Power_Display_label33;
        private Label XCRV_Power_Display_label33;
        private ComboBox AMP_comboBox1;
        private Label label33;
        private GroupBox groupBox4;
        private Label MFC_A_label38;
        private Label MFC_B_label38;
        private Label MFC_Knob_label38;
        private Label MFC_C_label38;
        private System.Windows.Forms.Timer RPi_Display_Timer;
        private ListBox mainlistBox1;
        private Label Volume_textBox2;
        private Label Microphone_textBox2;
        private Label Freq_Comp_label32;
        private HScrollBar Side_Tone_Volume_hScrollBar1;
        private Label Side_Tone_label32;
        //private Label Freq_CAl_Progress_Label;
    }
}


/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;*/
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using OmniaGUI.Properties;

namespace OmniaGUI
{
    public partial class Smeter_Form1 : Form
    {
        public String[] smeter_dial = new String[9];
        public String[] vu_dial = new string[10];
        FormWindowState LastWindowState = FormWindowState.Maximized;
      

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

        private void Frm_state(object sender, EventArgs e)
        {
            if (WindowState != LastWindowState)
            {
                LastWindowState = WindowState;
                if (WindowState == FormWindowState.Maximized)
                {
                    Write_Debug_Message(" Smeter Window Maximized");
                    Window_controls.smeter_minimized = false;
                }
                if (WindowState == FormWindowState.Minimized)
                {
                    Write_Debug_Message(" Smeter Window Minimized");
                    if (Window_controls.smeter_display_visable)
                    {
                        Window_controls.smeter_minimized = true;
                    }
                }
                if (WindowState == FormWindowState.Normal)
                {
                    Write_Debug_Message(" Smeter Window Restored");
                    Window_controls.smeter_minimized = false;
                }
            }
        }

        public void Write_Debug_Message(String Message)
        {
            var thisForm2 = Application.OpenForms.OfType<Main_form>().Single();
            thisForm2.Write_Message(Message);
        }

        public void Smeter_closing(object send, FormClosingEventArgs e)
        {
            
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                //Write_Debug_Message(" Smeter Window would close");
            }
            else
            {
                e.Cancel = false;
               
            }
        }
        
        public int Db_to_Smeter(int db)
        {
            int smeter_value = 0;
            //Write_Debug_Message(" Db_to_Smeter -> db: " + Convert.ToString(db));
            if (db <= -130) return 0;
            if (db <= -121) return 1;
            if (db <= -115) return 2;
            if (db <= -109) return 3;
            if (db <= -103) return 4;
            if (db <= -97) return 5;
            if (db <= -91) return 6;
            if (db <= -85) return 7;
            if (db <= -79) return 8;
            if (db <= -73) return 9;
            if (db <= -63) return 10;
            if (db <= -53) return 11;
            if (db <= -43) return 12;
            if (db <= -33) return 13;
            if (db <= -23) return 14;
            if (db <= -13) return 15;
            return smeter_value;
        }

        /*public static int ConvertRange(
            int originalStart, int originalEnd, // original range
            int newStart, int newEnd, // desired range
            int value) // value to convert
        {
            double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
            return (int)(newStart + ((value - originalStart) * scale));
        }*/
               
        public Smeter_Form1()
        {
            InitializeComponent();
            Peak_hold_on_offlistBox1.SelectedIndex = 1;
            Peak_hold_on_offlistBox1_SelectedIndexChanged(null, null);
            Peak_Hold_listBox1.SelectedIndex = 2;
            Peak_Hold_listBox1_SelectedIndexChanged(null, null);
            smeter_dial[0] = "[S]";
            smeter_dial[1] = "1";
            smeter_dial[2] = "3";
            smeter_dial[3] = "5";
            smeter_dial[4] = "7";
            smeter_dial[5] = "9";
            smeter_dial[6] = "+20";
            smeter_dial[7] = "+40";
            smeter_dial[8] = "+60";

            vu_dial[0] = "-20";
            vu_dial[1] = "-10";
            vu_dial[2] = "-7";
            vu_dial[3] = "-5";
            vu_dial[4] = "-3";
            vu_dial[5] = "0";
            vu_dial[6] = "+1";
            vu_dial[7] = "+2";
            vu_dial[8] = "+3";
            vu_dial[9] = " ";
        }

        private void Smeter_Form1_Load(object sender, EventArgs e)
        {
            //if (Settings.Default.S_meter_Location != null)
            //{
            //    this.Location = Settings.Default.S_meter_Location;
            //}
        }

        private void Smeter_timer_Tick(object sender, EventArgs e)
        {
         
            int db_value = 0;
           
            int vu_display_value = 0;
            int power_display_value = 0;
            //int smeter_value_temp = 0;
                       
            if (!Master_Controls.Transmit_Mode)
            {
                Smeter_controls.Display_mode = Smeter_controls.S_METER_MODE;
                if (Smeter_controls.Previous_mode != Smeter_controls.Display_mode)
                {
                    Smeter_controls.Display_mode = Smeter_controls.S_METER_MODE;
                    vuMeter1.MeterScale = VU_MeterLibrary.MeterScale.Analog;
                    vuMeter1.SuspendLayout();
                    vuMeter1.VuText = "S Meter";
                    vuMeter1.TextInDial = smeter_dial;
                    vuMeter1.Led1Count = 9;
                    vuMeter1.Led2Count = 0;
                    vuMeter1.Led3Count = 5;
                    Power_Value_label2.Visible = true;
                    vuMeter1.LevelMax = 145;
                    label1.Visible = true;
                    ALC_Meter.Visible = false;
                    ALC_label5.Visible = false;
                    vuMeter1.ResumeLayout();
                    Smeter_controls.Previous_mode = Smeter_controls.Display_mode;
                }
            }
            else
            {
                Smeter_controls.Display_mode = Smeter_controls.VU_MODE;
                if (Smeter_controls.Previous_mode != Smeter_controls.Display_mode)
                {
                    vuMeter1.SuspendLayout();
                    Smeter_controls.Display_mode = Smeter_controls.VU_MODE;
                    vuMeter1.MeterScale = VU_MeterLibrary.MeterScale.Log10;
                    vuMeter1.VuText = "VU";
                    vuMeter1.TextInDial = vu_dial;
                    vuMeter1.Led1Count = 6;
                    vuMeter1.Led2Count = 0;
                    vuMeter1.Led3Count = 4;
                    Power_Value_label2.Visible = false;
                    label1.Visible = false;
                    ALC_Meter.Visible = true;
                    ALC_label5.Visible = true;
                    //vuMeter1.LevelMax = 700;
                    vuMeter1.LevelMax = 700;
                    vuMeter1.ResumeLayout();
                    Smeter_controls.Previous_mode = Smeter_controls.Display_mode;
                }
            }
            Smeter_controls.smeter_display_value = Db_to_Smeter(Smeter_controls.smeter_value) * 10;
            if (Smeter_controls.Smeter_Hold_On)
            {
                if (Smeter_controls.smeter_display_value < Smeter_controls.Previous_Low)
                {
                    Smeter_controls.Previous_Low = Smeter_controls.Previous_Low - Smeter_controls.Smeter_decrement;
                    Smeter_controls.smeter_display_value = Smeter_controls.Previous_Low;
                   
                }
                else
                {
                    Smeter_controls.Previous_Low = Smeter_controls.smeter_display_value;
                }
            }
            if (Smeter_controls.Display_mode == Smeter_controls.S_METER_MODE)
            {
                vuMeter1.Level = Smeter_controls.smeter_display_value;
                db_value = Smeter_controls.smeter_value;
                Power_Value_label2.Text = Convert.ToString(db_value);
            }
            else
            {
                vu_display_value = Smeter_controls.smeter_value;
                if (Smeter_controls.Smeter_Hold_On)
                {
                    if (vu_display_value < Smeter_controls.VU.Previous_Low)
                    {
                        Smeter_controls.VU.Previous_Low = Smeter_controls.VU.Previous_Low - (Smeter_controls.Smeter_decrement + 10);
                        vu_display_value = Smeter_controls.VU.Previous_Low;

                    }
                    else
                    {
                        Smeter_controls.VU.Previous_Low = vu_display_value;
                    }
                }
                
                vuMeter1.Level = vu_display_value;
                if (vu_display_value >= 0)
                {
                    power_display_value = (int)((float)vu_display_value * 0.500f);
                    if(power_display_value > 105)
                    {
                        Power_Value_label2.ForeColor = System.Drawing.Color.Red;
                        Power_Value_label2.BackColor = System.Drawing.Color.White;
                    }
                    else
                    {
                        Power_Value_label2.ForeColor = System.Drawing.Color.Cyan;
                        Power_Value_label2.BackColor = System.Drawing.Color.Black;
                    }
                    Power_Value_label2.Text = Convert.ToString(power_display_value) + " %";
                }
                //Write_Debug_Message(" vuMeter Value: " + Convert.ToString(vu_display_value));
            }
        }

        public void SetMeter(Int32 meterValue)
        { 
            Smeter_controls.smeter_value = meterValue;
            if (Master_Controls.Debug_Display)
            {
                Write_Debug_Message(" SetMeter -> meterValue: " + Convert.ToString(meterValue));
            }
        }

        public void Set_ALC_Meter(Int32 alcValue)
        {
            ALC_Meter.Level = alcValue;
        }
        
        private void Peak_Hold_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            short index;
            if (oCode.isLoading == true) return;
            Peak_hold_on_offlistBox1.SetSelected(1, true);
            index = (short)Peak_Hold_listBox1.SelectedIndex;
            switch (index)
            {
                case 0:
                    Smeter_controls.smeter_hold_time = 10;
                    Smeter_Timer.Interval = Smeter_controls.smeter_hold_time;
                    break;
                case 1:
                    Smeter_controls.smeter_hold_time = 20;
                    Smeter_Timer.Interval = Smeter_controls.smeter_hold_time;
                    break;
                case 2:
                    Smeter_controls.smeter_hold_time = 30;
                    Smeter_Timer.Interval = Smeter_controls.smeter_hold_time;
                    break;
                case 3:
                    Smeter_controls.smeter_hold_time = 50;
                    Smeter_Timer.Interval = Smeter_controls.smeter_hold_time;
                    break;
                case 4:
                    Smeter_controls.smeter_hold_time = 100;
                    Smeter_Timer.Interval = Smeter_controls.smeter_hold_time;
                    break;
            }
        }

        private void Peak_hold_on_offlistBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            short index;
            if (oCode.isLoading == true) return;
            index = (short)Peak_hold_on_offlistBox1.SelectedIndex;
            switch (index)
            {
                case 0:
                    Smeter_controls.Smeter_Hold_On = false;
                    Smeter_Timer.Interval = 1;
                    break;
                case 1:
                    Smeter_Timer.Interval = Smeter_controls.smeter_hold_time;
                    Smeter_controls.Smeter_Hold_On = true;
                    break;
            }
        }

        private void vuMeter1_Load(object sender, EventArgs e)
        {

        }  

        private void Smeter_Value_label1_Click_1(object sender, EventArgs e)
        {

        }

        private void Smeter_checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if(!Smeter_controls.Smeter_convert)
            {
                vuMeter1.MeterScale = VU_MeterLibrary.MeterScale.Analog;
                Smeter_controls.Smeter_convert = true;
                Write_Debug_Message("Meter type Analog");
            }
            else
            {
                vuMeter1.MeterScale = VU_MeterLibrary.MeterScale.Log10;
                Smeter_controls.Smeter_convert = false;
                Write_Debug_Message("Meter type Log10");
            }
        }

        private void Power_Value_label2_Click(object sender, EventArgs e)
        {

        }

        private void Power_label2_Click(object sender, EventArgs e)
        {

        }

        private void ALC_Meter_Load(object sender, EventArgs e)
        {

        }

        private void ALC_label5_Click(object sender, EventArgs e)
        {

        }
    }
}




 public const int low_thread_hold = 500;
        public static Size pic_window_size;
        public static Size previous_form_size;
    
        public static Size form_window_size;
        public static Size max_size;
        public static Size panel_size;
        public static int H = 194;
        public static int W = (Panadapter_Controls.Max_X * 2);
        private const int CP_NOCLOSE_BUTTON = 0x200;
        public static int Previous_freq = 0;
        public static bool once = true;
        public static int picture_width = 0;
        public static int freq_per_pixel = 0;
        public static Point Freq_0_position;
        public static Point Freq_1_position;
        public static Point Freq_2_position;
        public static Point Mouse_location;
        public static Point Previous_Mouse_location;
        public static bool waterfall_run = true;
        public static bool waterfall_operation_complete = false;
        public static bool startup = true;
        public static int Waterfall_freq = 0;
        public static int previous_filter_size = Window_controls.Waterfall_Controls.Markers.band_marker_high;
        public static char previous_mode = Last_used.Current_mode;
        

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        public static void Write_Debug_Message(String Message)
        {
            var thisForm2 = Application.OpenForms.OfType<Main_form>().Single();
            thisForm2.Write_Message(Message);
        }

        public void Restore_window()
        {
            Window_controls.Waterfall_Controls.Waterfall_window.Size = new Size(800, 480);
            //Resize_Windows(Window_controls.Waterfall_Controls.Waterfall_window.Size);
        }

        public void Waterfall_active(object sender, EventArgs e)
        {
            //Window_controls.Waterfall_Controls.Waterfall_window.Activate();
            if (!Settings.Default.Docked)
            {
                this.picWaterfall.Focus();
            }
            //Window_controls.Docking_Controls.Waterfall_docked_size = this.Size;
            //Window_controls.Docking_Controls.Waterfall_location = this.Location;
        }

        public void Waterfall_moved(object sender, EventArgs e)
        {
            //Window_controls.Waterfall_Controls.Waterfall_window.Activate();
            //this.picWaterfall.Focus();
            //Window_controls.Docking_Controls.Waterfall_docked_size = this.Size;
            Window_controls.Docking_Controls.Waterfall_location = this.Location;
        }

        private void Frm_state(object sender, EventArgs e)
        {
            if (startup)
            {
                startup = false;
                return;
            }
            if (WindowState != LastWindowState)
            {
                LastWindowState = WindowState;
                if (WindowState == FormWindowState.Maximized)
                {
                    //Write_Debug_Message(" Frm_state -> Waterfall Window Maximized");
                    Window_controls.Waterfall_Controls.window_minimized = false;
                }
                if (WindowState == FormWindowState.Minimized)
                {
                    //Write_Debug_Message(" Frm_state -> Waterfall Window Minimized");
                    if (Window_controls.Waterfall_Controls.window_visable)
                    {
                        //waterfall_run = false;
                        Window_controls.Waterfall_Controls.window_minimized = true;

                    }
                }
                if (WindowState == FormWindowState.Normal)
                {
                    //Write_Debug_Message(" Frm_state -> Waterfall Window Restored");
                    Window_controls.Waterfall_Controls.window_minimized = false;
                    Set_freq_label(true);
                    //waterfall_run = true;
                }
            }
            else
            {
                Window_controls.Waterfall_Controls.new_size = this.Size;
                String Message = " Frm_state -> Form New Size: " +
                    Convert.ToString(Window_controls.Waterfall_Controls.new_size) + " Previous Form Size: " +
                    Convert.ToString(previous_form_size);
                Write_Debug_Message(Message);
                if (previous_form_size != Window_controls.Waterfall_Controls.new_size)
                {
                    if(Window_controls.Waterfall_Controls.new_size.Width != 800)
                    {
                        Window_controls.Waterfall_Controls.new_size.Width = 800;
                        this.Size = Window_controls.Waterfall_Controls.new_size;
                    }
                    //Resize_Windows(Window_controls.Waterfall_Controls.new_size);
                    previous_form_size = Window_controls.Waterfall_Controls.new_size;
                }
            }
        }

        public void Restart_Waterfall()
        {
            bool close_status = false;

            Waterfall_form.waterfall_run = false;
            Thread.Sleep(100);
            close_status = Display_GDI.Close();
            if (close_status)
            {
                Thread.Sleep(10);
                Window_controls.Waterfall_Controls.Pic_Waterfall_graphics.Dispose();
                this.picWaterfall.ClientSize = new Size(pic_window_size.Width, pic_window_size.Height);
                Window_controls.Waterfall_Controls.Pic_Waterfall_graphics = this.picWaterfall.CreateGraphics();
                Display_GDI.Window_size = pic_window_size;
                Window_controls.Waterfall_Controls.Display_Operation_Complete = true;
                Display_GDI.WaterfallDataReady = false;
                Display_GDI.Init();
                Display_GDI.MOX = false;
                Display_GDI.CurrentDisplayMode = DisplayMode.WATERFALL;
                Display_GDI.WaterfallLowThreshold = 1.0f;
                Display_GDI.WaterfallHighThreshold = 3090.0f;
                Waterfall_form.waterfall_run = true;
            }
        }

        public void Resize_Windows(Size new_size)
        {
            bool close_status = false;
            
            String Message = " Resize_Windows -> Size: " + Convert.ToString(new_size);
            Write_Debug_Message(Message);
            waterfall_run = false;
            Thread.Sleep(100);
            if (waterfall_operation_complete == true && new_size.Width == 800)
            {
                close_status = Display_GDI.Close();
                if (close_status)
                {
                    Thread.Sleep(10);
                    Window_controls.Waterfall_Controls.Pic_Waterfall_graphics.Dispose();
                    
                    if (!Settings.Default.Docked)
                    {
                        pic_window_size.Height = new_size.Height - Window_controls.Waterfall_Controls.Window_delta_height;
                        pic_window_size.Width = new_size.Width - Window_controls.Waterfall_Controls.Window_delta_width;
                    }
                    else
                    {
                        pic_window_size.Height = new_size.Height;
                        pic_window_size.Width = new_size.Width - Window_controls.Waterfall_Controls.Window_delta_width;
                    }
                    Window_controls.Docking_Controls.Last_Sized.Order = Window_controls.Docking_Controls.Last_Sized.Waterfall;
                    Window_controls.Docking_Controls.Last_Sized.Window_Size.Waterfall = this.Size;
                    Message = " Resize_Windows -> Delta Width: " + Convert.ToString(Window_controls.Waterfall_Controls.Window_delta_width) + 
                        " Delta Height: " + Convert.ToString( Window_controls.Waterfall_Controls.Window_delta_height);
                    Write_Debug_Message(Message);
                    this.picWaterfall.ClientSize = new Size(pic_window_size.Width,pic_window_size.Height);
                    Message = " Resize_Windows -> New form size: " + Convert.ToString(this.Size);
                    Write_Debug_Message(Message);
                    Message = " Resize_Windows -> New waterfall size: " + Convert.ToString(this.picWaterfall.Size);
                    Write_Debug_Message(Message);
                    Window_controls.Waterfall_Controls.Pic_Waterfall_graphics = this.picWaterfall.CreateGraphics();
                    Display_GDI.Window_size = pic_window_size;
                    Window_controls.Waterfall_Controls.Display_Operation_Complete = true;
                    Display_GDI.WaterfallDataReady = false;
                    Display_GDI.Init();
                    Display_GDI.MOX = false;
                    Display_GDI.CurrentDisplayMode = DisplayMode.WATERFALL;
                    Display_GDI.WaterfallLowThreshold = 1.0f;
                    Display_GDI.WaterfallHighThreshold = 3090.0f;
                    waterfall_run = true;
                }
            }
        }

        public void Resize_From_Main(Size new_size)
        {
            new_size.Width = 800;
            this.Size = new_size;
            //Resize_Windows(new_size);
        }

        public Waterfall_form()
        {
            InitializeComponent();
        }

        public void Set_freq_label(bool now)
        {
            if (now)
            {
                Previous_freq = 0;
            }
           
            if (Previous_freq != oCode.DisplayFreq)
            {
                try
                {
                    waterfall_run = false;
                    picture_width = pic_window_size.Width;
                    if(picture_width <= 0)
                    {
                        picture_width = 1;
                    }
                    freq_per_pixel = Window_controls.Waterfall_Controls.Markers.DISPLAY_BANDWIDTH / picture_width;
                    int freq = oCode.DisplayFreq - (freq_per_pixel * picture_width / 2);
                    int meg10 = freq / 10000000;
                    int meg = (freq - (meg10 * 10000000)) / 1000000;
                    int hundred_thousand = (freq - (meg10 * 10000000) - (meg * 1000000))
                        / 100000;
                    int ten_thousand = (freq - (meg10 * 10000000) - (meg * 1000000) -
                        (hundred_thousand * 100000)) / 10000;
                    int thousand = (freq - (meg10 * 10000000) - (meg * 1000000) -
                        (hundred_thousand * 100000) - (ten_thousand * 10000)) / 1000;
                    //Freq_label1_0.Text = Convert.ToString(meg10) + Convert.ToString(meg) + "." + Convert.ToString(hundred_thousand) +
                     //   Convert.ToString(ten_thousand) + Convert.ToString(thousand);

                    freq = oCode.DisplayFreq;
                    meg10 = freq / 10000000;
                    meg = (freq - (meg10 * 10000000)) / 1000000;
                    hundred_thousand = (freq - (meg10 * 10000000) - (meg * 1000000))
                        / 100000;
                    ten_thousand = (freq - (meg10 * 10000000) - (meg * 1000000) -
                        (hundred_thousand * 100000)) / 10000;
                    thousand = (freq - (meg10 * 10000000) - (meg * 1000000) -
                        (hundred_thousand * 100000) - (ten_thousand * 10000)) / 1000;
                    //Freq_label1_1.Text = Convert.ToString(meg10) + Convert.ToString(meg) + "." + Convert.ToString(hundred_thousand) +
                    //    Convert.ToString(ten_thousand) + Convert.ToString(thousand);

                    freq = oCode.DisplayFreq + (freq_per_pixel * picture_width / 2);
                    meg10 = freq / 10000000;
                    meg = (freq - (meg10 * 10000000)) / 1000000;
                    hundred_thousand = (freq - (meg10 * 10000000) - (meg * 1000000))
                        / 100000;
                    ten_thousand = (freq - (meg10 * 10000000) - (meg * 1000000) -
                        (hundred_thousand * 100000)) / 10000;
                    thousand = (freq - (meg10 * 10000000) - (meg * 1000000) -
                        (hundred_thousand * 100000) - (ten_thousand * 10000)) / 1000;
                    //Freq_label1_2.Text = Convert.ToString(meg10) + Convert.ToString(meg) + "." + Convert.ToString(hundred_thousand) +
                    //    Convert.ToString(ten_thousand) + Convert.ToString(thousand);

                    String Message = " Set_freq_label -> Freq Changed: " + Convert.ToString(oCode.DisplayFreq) +
                        " freq: " + Convert.ToString(freq) + " Picture Width: " + Convert.ToString(picture_width) +
                        " Freq/Pixel: " + Convert.ToString(freq_per_pixel);
                    //Write_Debug_Message(Message);
                    Previous_freq = oCode.DisplayFreq;
                    waterfall_run = true;
                }
                catch (Exception ex)
                {
                    String Message = " Set_freq_label -> " + ex.ToString();
                    Write_Debug_Message(Message);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //if (!Settings.Default.Waterfall_Location.IsEmpty)
            //{
            //    this.Location = Settings.Default.Waterfall_Location;
            //}
            //if (!Settings.Default.Waterfall_Size.IsEmpty)
            //{
            //    this.Size = Settings.Default.Waterfall_Size;
            //}
            String Message = " Form1_Load -> Waterfall_form.Form1_Load Starting";
            Write_Debug_Message(Message);
            form_window_size = this.Size;
            previous_form_size = this.Size;
            pic_window_size = this.picWaterfall.ClientSize;
            //Window_controls.Waterfall_Controls.new_size = this.Size;
            //pic_window_size.Height = this.Height - Window_controls.Waterfall_Controls.Window_delta_height;
            //pic_window_size.Width = this.Width - Window_controls.Waterfall_Controls.Window_delta_width;
            //Window_controls.Waterfall_Controls.picture_box_width = pic_window_size.Width;
            //this.picWaterfall.ClientSize = new Size(pic_window_size.Width, pic_window_size.Height);
            //pic_window_size = this.picWaterfall.Size;
            this.picWaterfall.BackColor = Color.Transparent;
            Window_controls.Waterfall_Controls.Pic_Waterfall_graphics = this.picWaterfall.CreateGraphics();
            //Window_controls.Waterfall_Controls.Waterfall_graphics = this.CreateGraphics();
            this.picWaterfall.Show();
            
            Display_GDI.Window_size = pic_window_size;
            Message = " Form1_Load -> Form Size -> W: " + Convert.ToString(form_window_size.Width) + " H: " +
                     Convert.ToString(form_window_size.Height);
            Write_Debug_Message(Message);
            Message = " Form1_Load -> Pic Size -> W: " + Convert.ToString(pic_window_size.Width) + " H: " +
                       Convert.ToString(pic_window_size.Height);
            Write_Debug_Message(Message);
            Message = " Form1_Load -> Delta -> W: " + Convert.ToString(Window_controls.Waterfall_Controls.Window_delta_width) + " H: " +
                    Convert.ToString(Window_controls.Waterfall_Controls.Window_delta_height);
            Write_Debug_Message(Message);
            Window_controls.Waterfall_Controls.Display_Operation_Complete = true;
            Display_GDI.WaterfallDataReady = false;
            Display_GDI.Init();
            Display_GDI.MOX = false;
            Display_GDI.CurrentDisplayMode = DisplayMode.WATERFALL;
            Display_GDI.WaterfallLowThreshold = 1.0f;
            Display_GDI.WaterfallHighThreshold = 3090.0f;
            //Display_GDI.ReverseWaterfall = true;
            Waterfall_thread.Name = "Waterfall";
            //Waterfall_thread.Priority = ThreadPriority.AboveNormal;
            Waterfall_thread.Start();
            //Freq_0_position = Freq_label1_0.Location;
            //Freq_1_position = Freq_label1_1.Location;
            //Freq_2_position = Freq_label1_2.Location;
            //Window_controls.Waterfall_Controls.Waterfall_window.FormBorderStyle = FormBorderStyle.None;
            Message = " Waterfall_form.Form1_Load Finished";
            Write_Debug_Message(Message);
        }

        public void Display_Main_Freq_by_Waterfall()
        {

            var thisForm2 = Application.OpenForms.OfType<Main_form>().Single();
            thisForm2.Update_Main_Display();
        }

        public void Set_Frequency(int freq)
        {
            int delta = 0;
            int new_freq = 0;
            //Write_Debug_Message(" Set_Frequency -> Called -> frequency: " + Convert.ToString(freq));
            if (Last_used.Current_mode != 'C')
            {
                if (Panadapter_Controls.Auto_Snap.Auto_Snap_Checkbox)
                {
                    delta = freq % Panadapter_Controls.Auto_Snap.Snap_Value;
                    if (delta != 0)
                    {
                        switch (Panadapter_Controls.Auto_Snap.Snap_Value)
                        {
                            case 1000:
                                if (delta > (Panadapter_Controls.Auto_Snap.Snap_Value / 2))
                                {
                                    new_freq = Panadapter_Controls.Auto_Snap.Snap_Value - delta;
                                    freq = freq + new_freq;
                                }
                                else
                                {
                                    freq = freq - delta;
                                }
                                break;
                            case 500:
                                new_freq = freq - delta;
                                delta = new_freq % 1000;
                                if (delta == 0)
                                {
                                    freq = new_freq;
                                }
                                else
                                {
                                    delta = freq % 500;
                                    freq = freq - delta;
                                }
                                break;
                            case 100:
                                new_freq = freq - delta;
                                delta = new_freq % 500;
                                if (delta == 0)
                                {
                                    freq = new_freq;
                                }
                                else
                                {
                                    delta = freq % 100;
                                    freq = freq - delta;
                                }
                                break;
                        }
                    }
                    Write_Debug_Message(" Set_Frequency -> New frequency: " + Convert.ToString(freq));
                }
            }
            oCode.DisplayFreq = freq;
            switch (oCode.current_band)
            {
                case 160:
                    Last_used.B160.Freq = freq;
                    break;
                case 80:
                    Last_used.B80.Freq = freq;
                    break;
                case 60:
                    Last_used.B60.Freq = freq;
                    break;
                case 40:
                    Last_used.B40.Freq = freq;
                    break;
                case 30:
                    Last_used.B30.Freq = freq;
                    break;
                case 20:
                    Last_used.B20.Freq = freq;
                    break;
                case 17:
                    Last_used.B17.Freq = freq;
                    break;
                case 15:

                    Last_used.B15.Freq = freq;
                    break;
                case 12:
                    Last_used.B12.Freq = freq;
                    break;
                case 10:
                    Last_used.B10.Freq = freq;
                    break;
                default:
                    Last_used.GEN.Freq = freq;
                    break;
            }
            oCode.SendCommand32(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, oCode.CMD_SET_MAIN_FREQ,
                (freq));
            Display_Main_Freq_by_Waterfall();
            Write_Debug_Message(" Set_Frequency -> Finished");
        }

        private void Calculate_frequency(Point mouse_location)
        {
            int x = 0;
            //String Display_freq;
            int viewport = 72000;
            int cw_offset = 0;
            if (Last_used.Current_mode == 'C')
            {
                cw_offset = Filter_control.CW_Pitch;
            }
            x = mouse_location.X;
            int start_freq = (oCode.DisplayFreq - 36000);
            Waterfall_freq = start_freq + ((72000 / pic_window_size.Width) * (x) - cw_offset);
            int MHz = (Waterfall_freq) / 1000000;
            int KHz = ((Waterfall_freq) - (MHz * 1000000)) / 1000;
            int Hz = (Waterfall_freq) - (MHz * 1000000) - (KHz * 1000);
            var Display_freq = Convert.ToString(MHz) + "." + 
                string.Format("{0:000}", KHz);
            Cursor_textBox1.Text = "Cursor: " + Display_freq + " MHz"
                + "\r\n" + "VIEWPORT: " + Convert.ToString(viewport);
          
        }

        private void picWaterfall_Click(object sender, EventArgs e)
        {
            Set_Frequency(Waterfall_freq);
            Write_Debug_Message(" picWaterfall_Click: " + Convert.ToString(Mouse_location));
        }

        public void picWaterfall_MouseWheel(object sender, MouseEventArgs e)
        {
            int n = e.Delta;

            if (n > 0) n = 1; else n = -1;      // convert mousewheel delta to either 1 or -1
            
            if (!Window_controls.Waterfall_Controls.Wheel_zoom_status)
            {
                //Write_Debug_Message(" Mouse Wheel n: " + Convert.ToString(n));
                if (Mouse_controls.Allow_Frequency_Updates == false) return;
                switch (oCode.FreqDigit)
                {
                    case 0:
                        switch (n)
                        {
                            case 1:
                                if (oCode.DisplayFreq < 40000000) oCode.DisplayFreq += 1;
                                break;

                            default:
                                if (oCode.DisplayFreq >= 1) oCode.DisplayFreq -= 1;
                                break;
                        }

                        break;

                    case 1:
                        switch (n)
                        {
                            case 1:
                                if (oCode.DisplayFreq < 40000000) oCode.DisplayFreq += 10;
                                break;

                            default:
                                if (oCode.DisplayFreq >= 10) oCode.DisplayFreq -= 10;
                                break;
                        }

                        break;

                    case 2:
                        switch (n)
                        {
                            case 1:
                                if (oCode.DisplayFreq < 40000000) oCode.DisplayFreq += 100;
                                break;

                            default:
                                if (oCode.DisplayFreq >= 100) oCode.DisplayFreq -= 100;
                                break;
                        }
                        break;

                    case 3:
                        switch (n)
                        {
                            case 1:
                                if (oCode.DisplayFreq < 40000000) oCode.DisplayFreq += 1000;
                                break;

                            default:
                                if (oCode.DisplayFreq >= 1000) oCode.DisplayFreq -= 1000;
                                break;
                        }
                        break;

                    case 4:
                        switch (n)
                        {
                            case 1:
                                if (oCode.DisplayFreq < 40000000) oCode.DisplayFreq += 10000;
                                break;

                            default:
                                if (oCode.DisplayFreq >= 10000) oCode.DisplayFreq -= 10000;
                                break;
                        }
                        break;

                    case 5:
                        Mouse_controls.Silent_Update = true;
                        switch (n)
                        {
                            case 1:
                                if (oCode.DisplayFreq < 40000000) oCode.DisplayFreq += 100000;
                                break;

                            default:
                                if (oCode.DisplayFreq >= 100000) oCode.DisplayFreq -= 100000;
                                break;
                        }
                        break;

                    case 6:
                        Mouse_controls.Silent_Update = true;
                        switch (n)
                        {
                            case 1:
                                if (oCode.DisplayFreq < 40000000) oCode.DisplayFreq += 1000000;
                                break;

                            default:
                                if (oCode.DisplayFreq >= 1000000) oCode.DisplayFreq -= 1000000;
                                break;
                        }
                        break;

                    case 7:
                        Mouse_controls.Silent_Update = true;
                        switch (n)
                        {
                            case 1:
                                if (oCode.DisplayFreq < 40000000) oCode.DisplayFreq += 10000000;
                                break;

                            default:
                                if (oCode.DisplayFreq >= 10000000) oCode.DisplayFreq -= 10000000;
                                break;
                        }
                        break;

                    default:

                        break;
                }
                if (oCode.DisplayFreq < 0) return;
                if (oCode.DisplayFreq > 40000000) oCode.DisplayFreq = 40000000;
                Display_Main_Freq_by_Waterfall();
                oCode.SendCommand32(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, oCode.CMD_SET_MAIN_FREQ,
                    (oCode.DisplayFreq));
                Mouse_controls.Silent_Update = false;
                if (oCode.current_band == oCode.general_band)
                {
                    Last_used.GEN.Freq = oCode.DisplayFreq;
                }
            }
            else
            {
                if (!Window_controls.Waterfall_Controls.Zoom_process)
                {
                    switch (n)
                    {
                        case 1:
                            Window_controls.Waterfall_Controls.Wheel_value =
                                Window_controls.Waterfall_Controls.Wheel_value + 2;
                            break;
                        default:
                            Window_controls.Waterfall_Controls.Wheel_value =
                                Window_controls.Waterfall_Controls.Wheel_value - 2;
                            if (Window_controls.Waterfall_Controls.Wheel_value <= 0)
                            {
                                Window_controls.Waterfall_Controls.Wheel_value = 0;
                            }
                            break;
                    }
                }
            }
        }

        private void picWaterfall_MouseMove(object send, EventArgs e)
        {
            Point Cursor_Location;
            if (!Master_Controls.Transmit_Mode)
            {
                Mouse_location = this.picWaterfall.PointToClient(Cursor.Position);
                if ((Previous_Mouse_location.X != Mouse_location.X) && (Previous_Mouse_location.Y != Mouse_location.Y))
                {
                    Cursor_timer2.Enabled = false;
                    Cursor_Location = Mouse_location;
                    Calculate_frequency(Mouse_location);
                    if (Mouse_location.X >= pic_window_size.Width - Cursor_textBox1.Size.Width)
                    {
                        Cursor_Location.X = Mouse_location.X - Cursor_textBox1.Size.Width;
                    }
                    else
                    {
                        Cursor_Location.X = Mouse_location.X + 20;
                    }
                    Display_GDI.CursorPosition = Cursor_Location.X;
                    Cursor_textBox1.Location = Cursor_Location;
                    Previous_Mouse_location = Mouse_location;
                    Cursor_timer2.Enabled = true;
                    Cursor_textBox1.Show();
                    //Write_Debug_Message(" MouseMove : " + Convert.ToString(Mouse_location));
                }
            }
        }

        private void picWaterfall_MouseDown(object send, EventArgs e)
        {
            Set_Frequency(Waterfall_freq);
            Write_Debug_Message(" picWaterfall_MouseDown: " + Convert.ToString(Mouse_location));
        }

        private void picWaterfall_MouseLeave(object send, EventArgs e)
        {
            Cursor_textBox1.Hide();
            Cursor_timer2.Enabled = false;
        }

        private void picWaterfall_MouseUp(object send, EventArgs e)
        {
            //Write_Debug_Message(" picWaterfall_MouseUp: " + Convert.ToString(Mouse_location));
        }

        private void picWaterfall_MouseEnter(object send, EventArgs e)
        {
            if (!Settings.Default.Docked)
            {
                this.picWaterfall.Focus();
            }
            if (!Master_Controls.Transmit_Mode)
            {
                Cursor_textBox1.Show();
            }
        }

        public void Freq_label1_2_Click(object sender, EventArgs e)
        {

        }

        public void Freq_label1_1_Click(object sender, EventArgs e)
        {

        }

        public void Freq_label1_0_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Set_freq_label(false);
            if (Window_controls.Waterfall_Controls.CW_Snap.button_clicked)
            {
                if (Window_controls.Waterfall_Controls.CW_Snap.CW_snap_status == false && 
                    Window_controls.Waterfall_Controls.CW_Snap.CW_button == true)
                {
                    //Panadapter_CW_Snap_button1.BackColor = Color.White;
                    Window_controls.Waterfall_Controls.CW_Snap.CW_button = false;
                    Window_controls.Waterfall_Controls.CW_Snap.button_clicked = false;
                }
            }
            if(previous_filter_size != Window_controls.Waterfall_Controls.Markers.band_marker_high || 
               previous_mode != Last_used.Current_mode)
            {
                Restart_Waterfall();
                previous_filter_size = Window_controls.Waterfall_Controls.Markers.band_marker_high;
                previous_mode = Last_used.Current_mode;
            }
         
            if (Window_controls.Waterfall_Controls.restore_size)
            {
                Window_controls.Waterfall_Controls.restore_size = false;
                Settings.Default.Docked = false;
                Window_controls.Waterfall_Controls.Waterfall_window.Size = new Size(800, 480);
                Size restore_size = new Size(800, 480);
                //Resize_Windows(restore_size);
            }
            if (Settings.Default.Docked)
            {
                if (Window_controls.Docking_Controls.Waterfall_docked_size !=
                    this.Size)
                {
                    Window_controls.Docking_Controls.Waterfall_docked_size =
                    this.Size;
                }
            }
        }

        private void Cursor_timer2_Tick(object sender, EventArgs e)
        {
            Cursor_textBox1.Hide();
            Write_Debug_Message(" Cursor_timer. Timeout");
            Cursor_timer2.Enabled = false;
        }

        private void Panadapter_CW_Snap_button1_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (Last_used.Current_mode == 'C')
            {
                if (!Window_controls.Waterfall_Controls.CW_Snap.CW_button)
                {
                    //Panadapter_CW_Snap_button1.BackColor = Color.Red;
                    Window_controls.Waterfall_Controls.CW_Snap.Button_Color = Color.Red;
                    Window_controls.Waterfall_Controls.CW_Snap.CW_button = true;
                    Window_controls.Waterfall_Controls.CW_Snap.button_clicked = true;
                    Window_controls.Waterfall_Controls.CW_Snap.CW_snap_status = true;
                    oCode.SendCommand32(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget,
                                        Panadapter_Controls.CMD_CW_SNAP_START, Waterfall_form.Waterfall_freq);

                }
            }
        }

        /*private void Wheel_checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            byte wheel_status = 0;
            byte[] buf = new byte[2];

            if (oCode.isLoading) return;
            buf[0] = Master_Controls.Extended_Commands.CMD_SET_WATERFALL_GRID;
            if (!Window_controls.Waterfall_Controls.Wheel_zoom_status)
            {
                //Wheel_checkBox1.Text = "Wheel Zoom";
                Window_controls.Waterfall_Controls.Wheel_zoom_status = true;
                wheel_status = 1;
            }
            else
            {
                //Wheel_checkBox1.Text = "Wheel Freq";
                Window_controls.Waterfall_Controls.Wheel_zoom_status = false;
                wheel_status = 0;
            }
            buf[1] = wheel_status;
            oCode.SendCommand_MultiByte(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget,
                    Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
        }*/
    }
}



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OmniaGUI.Properties;
using System.IO;
using ZedGraph;

namespace OmniaGUI
{
    
    public partial class Panadapter_1 : Form
    {
        public Panadapter_1 Spectrum_Window;
        private const int CP_NOCLOSE_BUTTON = 0x200;
        private const string V = " Send log files to Multus SDR, LLC.";
        public static Thread Spectrum_thread = new Thread(new ThreadStart(Display_Panadapter));
        FormWindowState LastWindowState = FormWindowState.Maximized;
        //int min_y = 7000;
        //int max_y = 0;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        //public panadapter_control Panadapter_Control_Window;
        static PointPairList list = new PointPairList();
        
        public static GraphPane myPane;
        public static LineItem myCurve;
        public static LineObj cursor_low = new LineObj();
        public static LineObj cursor_center = new LineObj();
        public static LineObj cursor_high = new LineObj();
        public static LineObj cursor_user = new LineObj();
        public static bool first_pass = true;
        public static String Mode = "AM";
        public static bool code_triggered = true;
        public static int loop_count = 0;
        public static String Message;
        public static int gain_value = 0;
        public static int cw_offset = 0;
        public static String Cursor_message = "";

        private void Frm_state(object sender, EventArgs e)
        {
            if (WindowState != LastWindowState)
            {
                LastWindowState = WindowState;
                if (WindowState == FormWindowState.Maximized)
                {
                    Write_Debug_Message(" Spectrum Window Maximized");
                    Window_controls.Panadapter_New.window_minimized = false;
                }
                if (WindowState == FormWindowState.Minimized)
                {
                    Write_Debug_Message(" Spectrum Window Minimized");
                    if (Window_controls.Panadapter_New.window_visable)
                    {
                        Window_controls.Panadapter_New.window_minimized = true;
                    }
                }
                if (WindowState == FormWindowState.Normal)
                {
                    Write_Debug_Message(" Spectrum Window Restored");
                    Window_controls.Panadapter_New.window_minimized = false;
                    Window_controls.Panadapter_window_new.WindowState = FormWindowState.Normal;
                    Window_controls.Panadapter_window_new.Activate();
                    Window_controls.Panadapter_window_new.Show();
                    Window_controls.Panadapter_window_new.Focus();
                    zedGraphControl2.Refresh();
                }
            }
        }

        public void Spectrum_active(object sender,EventArgs e)
        {
            Window_controls.Panadapter_window_new.Focus();
            zedGraphControl2.Focus();
        }

        public static void Write_Debug_Message(String Message)
        {
            var thisForm2 = Application.OpenForms.OfType<frmGUI>().Single();
            thisForm2.Write_Message(Message);
        }

        public void Spectrum_closing(object send, FormClosingEventArgs e)
        {
            //Write_Debug_Message(" Spectrum Window would close");
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
            //Window_controls.Panadapter_window_new.
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        public float Filter_size(int filter_index)
        {
            float size = 0.0f;
            switch (filter_index)
            {
                case 4:
                    size = 2400f;
                    break;
                case 3:
                    size = 2700f;
                    break;
                case 2:
                    size = 3000f;
                    break;
                case 1:
                    size = 4000f;
                    break;
                case 0:
                    size = 5500f;
                    break;
            }
            return size;
        }

        public void Calculate_Band_Marker(char mode, float low_cut, float high_cut)
        {
            int cursor_marker = 0;
            //double display_width = 0.0d;
            double zoom = 0.0d;

            zoom = myPane.XAxis.Scale.Max - myPane.XAxis.Scale.Min;
            Panadapter_Controls.Filter_Markers.Window_Size = zedGraphControl2.Width;
            Panadapter_Controls.Filter_Markers.Display_Center = 399;
            cursor_marker = ((int)high_cut / (Panadapter_Controls.Filter_Markers.DISPLAY_BANDWIDTH / (int)zoom));
            /*if (Master_Controls.Debug_Display)
            {
                Message = "Calculate_Band_Marker -> Zoom: " + Convert.ToString((int)zoom) ;
                Write_Debug_Message(Message);
                Message = "Calculate_Band_Marker -> Window Size: " + 
                    Convert.ToString(Panadapter_Controls.Filter_Markers.Window_Size)
                    ;
                Write_Debug_Message(Message);
                Message = "Calculate_Band_Marker -> Display_Center:  " + 
                    Convert.ToString(Panadapter_Controls.Filter_Markers.Display_Center)
                    ;
                Write_Debug_Message(Message);
                Message = "Calculate_Band_Marker -> Cursor Marker: " + 
                    Convert.ToString(cursor_marker) ;
                Write_Debug_Message(Message);
            }*/
            switch (mode)
            {
                case 'A':
                    Panadapter_Controls.Filter_Markers.G_band_marker_low = Panadapter_Controls.Filter_Markers.Display_Center -
                        cursor_marker;
                    Panadapter_Controls.Filter_Markers.G_band_marker_high = Panadapter_Controls.Filter_Markers.Display_Center +
                        cursor_marker;
                    break;
                case 'L':
                    Panadapter_Controls.Filter_Markers.G_band_marker_low = Panadapter_Controls.Filter_Markers.Display_Center -
                        cursor_marker;
                    Panadapter_Controls.Filter_Markers.G_band_marker_high = 0;
                    break;
                case 'U':
                    Panadapter_Controls.Filter_Markers.G_band_marker_high = Panadapter_Controls.Filter_Markers.Display_Center +
                        cursor_marker;
                    Panadapter_Controls.Filter_Markers.G_band_marker_low = 0;
                    break;
                case 'C':
                    switch (Filter_control.CW_Pitch)
                    {
                        case 400:
                            Panadapter_Controls.Filter_Markers.CW_Offset = 5;
                            break;
                        case 500:
                            Panadapter_Controls.Filter_Markers.CW_Offset = 6;
                            break;
                        case 600:
                            Panadapter_Controls.Filter_Markers.CW_Offset = 8;
                            break;
                        case 700:
                            Panadapter_Controls.Filter_Markers.CW_Offset = 9;
                            break;
                        case 800:
                            Panadapter_Controls.Filter_Markers.CW_Offset = 10;
                            break;
                    }
                    Panadapter_Controls.Filter_Markers.G_band_center = Panadapter_Controls.Filter_Markers.Display_Center +
                                                                                            Panadapter_Controls.Filter_Markers.CW_Offset ;
                    Panadapter_Controls.Filter_Markers.G_band_marker_low = 0;
                    //Message = "G_band_center: " + Convert.ToString(Panadapter_Controls.Filter_Markers.G_band_center) ;
                    //Write_Debug_Message(Message);
                    //Message = "Current CW_Pitch: " + Convert.ToString(Filter_control.CW_Pitch) ;
                    //Write_Debug_Message(Message);
                    break;

            }
        }
        
        public void Graph_name(String Mode)
        {
            //int j = 0;
            int band_name_position = 0;
            String Band_name = "";
            String Band_Printed = "";
            String Filter = "";
            String Filter_Printed = "";

            if (Last_used.Current_mode == 'C')
            {
                Filter = Panadapter_Controls.CW_Filter_Size_Name + " " + Mode;
            }
            else
            {
                Filter = Panadapter_Controls.Filter_Size_Name + " " + Mode;
            }
            Filter_Printed = Filter.PadLeft(10);

            if (oCode.current_band != 0)
            {
                Band_Printed = Band_name.PadLeft(band_name_position);
                Mode_label1.Text = Filter_Printed;
            }
        }

        public void Start_Panadapter_1()
        {
            String Message = "Start_Panadapter_1 Starting \r\n";
            Write_Debug_Message(Message);
            //Thread.Sleep(3000);
            InitializeComponent();
            CreateGraph(zedGraphControl2);
            Create_Marker_lines();
            zedGraphControl2.CursorValueEvent += new ZedGraphControl.CursorValueHandler(myGraph_CursorValueEvent);
            
            zedGraphControl2.ZoomEvent += new ZedGraphControl.ZoomEventHandler(zg1_ZoomEvent);
            zedGraphControl2.MouseEnter += new  EventHandler (Display_User_Cursor);
            zedGraphControl2.MouseLeave += new EventHandler(Hide_User_Cursor);
            //Panadapter_Controls.Filter_Markers.MAX_PIXELS = this.Width;
            Panadapter_Controls.Filter_Markers.Window_Size = zedGraphControl2.Width;
            //Panadapter_Controls.Filter_Markers.Display_Center = ((int)myPane.XAxis.Scale.Max / 2)- 1;
            zedGraphControl2.MouseClick += new MouseEventHandler(Mouse_Set_freq);
            Update_User_values();
            Message = "Start_Panadapter_1 Finished " + Convert.ToString(Panadapter_Controls.Filter_Markers.Display_Center) ;
            Write_Debug_Message(Message);
        }

        void Display_User_Cursor(object sender, EventArgs e) {
            cursor_user.IsVisible = true;
        }

        void Hide_User_Cursor(object send,EventArgs e)
        {
            cursor_user.IsVisible = false;
        }

        public string myGraph_CursorValueEvent(ZedGraphControl send, GraphPane pane, Point mousePt)
        {
            double x, y;
            double start_freq = 0.0;
            int freq = 0;
            double cw_offset = 0.0;
            

            pane.ReverseTransform(mousePt, out x, out y);
            if (Panadapter_Controls.Mouse_event.x == x && Panadapter_Controls.Mouse_event.y == y) {
                return Cursor_message; ;
            }
            Panadapter_Controls.Mouse_event.x = x;
            Panadapter_Controls.Mouse_event.y = y;
            Panadapter_Controls.Mouse_event.User_Cursor_x = x;
            Panadapter_Controls.Mouse_event.User_Cursor_y = y;
            start_freq = (double)(Panadapter_Controls.Display_freq - 36000);
            if (Last_used.Current_mode == 'C')
            {
                //cw_offset = 600.0d;
                cw_offset = (double)Filter_control.CW_Pitch;
            }
            else
            {
                cw_offset = 0.0d;
            }
            Panadapter_Controls.Graph_Freq = start_freq + ((72000.0d / 800.0d) * (x) - cw_offset);
            //String Message = "X Position: " + Panadapter_Controls.Graph_Freq.ToString("f2") ;
            //Write_Debug_Message(Message);
            freq = (int)Panadapter_Controls.Graph_Freq;
            int MHz = (freq) / 1000000;
            int KHz = ((freq) - (MHz * 1000000)) / 1000;
            int Hz = (freq) - (MHz * 1000000) - (KHz * 1000);
            Panadapter_Controls.Mouse_event.Display_Freq = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + string.Format("{0:000}", Hz);
            //Panadapter_Mouse_label1.Text = Panadapter_Controls.Mouse_event.Display_Freq;
            cursor_user.Location.X = Panadapter_Controls.Mouse_event.User_Cursor_x;
            zedGraphControl2.AxisChange();
            zedGraphControl2.Refresh();
            Cursor_message = "Cursor: " + Panadapter_Controls.Mouse_event.Display_Freq
                + "\r\n" + "VIEWPORT: " +
                Convert.ToString((Panadapter_Controls.Freq_Bounds.High_Freq - Panadapter_Controls.Freq_Bounds.Low_Freq));
            //return Panadapter_Controls.Mouse_event.Display_Freq;
            return Cursor_message;
        }

        public void Display_Main_Freq_by_Panadapter()
        {

            var thisForm2 = Application.OpenForms.OfType<frmGUI>().Single();
            thisForm2.Update_Main_Display();
        }

        public void Set_Frequency(int freq)
        {
            int delta = 0;
            int new_freq = 0;

            //int temp_freq = 0;
            Write_Debug_Message(" Set_Frequency -> Called -> frequency: " + Convert.ToString(freq));
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
            Panadapter_Controls.Display_freq = freq;
            oCode.SendCommand32(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, oCode.CMD_SET_MAIN_FREQ,
                (Panadapter_Controls.Display_freq));
            Display_Main_Freq_by_Panadapter();
            Write_Debug_Message(" Set_Frequency -> Finished");
        }

        public void Mouse_Set_freq(object sender, EventArgs e)
        {
            Panadapter_Controls.Fine_Tune_Delta = 0;
            Set_Frequency((int)Panadapter_Controls.Graph_Freq);
            //MessageBox.Show("Mouse Event. X Value: " + Convert.ToString(Panadapter_Controls.Graph_Freq),"MSCC", 
            //  MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Display_Panadapter()
        {
            int i = 0;
            int panadapter_y_value;
            int spectrum_base_line_temp = 0;
            double x = 0;
            double y = 0;
            int max_limit = 0;
            float filter_size = 0f;
            int high_temp = 0;
            Panadapter_1 to_non_static = new Panadapter_1();

            while (true)
            {
                if (Window_controls.Panadapter_New.window_created &&
                                                       Window_controls.Panadapter_window_new.WindowState != FormWindowState.Minimized &&
                         Master_Controls.Transmit_Mode == false)
                {
                    if (Panadapter_Controls.Sequence_0_Complete && Panadapter_Controls.Sequence_1_Complete)
                    {
                        if (Master_Controls.Debug_Display)
                        {
                            Write_Debug_Message(" Display_Panadapter data Start. Time: " +
                                DateTime.Now.ToString("mm:ss"));
                        }
                        if (Panadapter_Controls.Freq_Set_By_Master)
                        {
                            Panadapter_Controls.Display_freq = oCode.DisplayFreq;
                            Panadapter_Controls.Fine_Tune_Delta = 0;
                            Panadapter_Controls.Freq_Set_By_Master = false;
                            Panadapter_Controls.Fine_Tuning = false;
                        }
                        if (Panadapter_Controls.CW_Snap.CW_snap_status == false && Panadapter_Controls.CW_Snap.CW_button == true)
                        {
                            to_non_static.Panadapter_CW_Snap_button1.BackColor = Color.White;
                            Panadapter_Controls.CW_Snap.CW_button = false;
                        }
                        if (Panadapter_Controls.Panadapter_Colors.Previous_Line_Color != Panadapter_Controls.Panadapter_Colors.Line_Color)
                        {
                            myCurve.Line.Color = Panadapter_Controls.Panadapter_Colors.Line_Color;
                            Panadapter_Controls.Panadapter_Colors.Previous_Line_Color = Panadapter_Controls.Panadapter_Colors.Line_Color;
                        }
                        if (Panadapter_Controls.Panadapter_Colors.Previous_Fill_Color != Panadapter_Controls.Panadapter_Colors.Fill_Color)
                        {
                            myCurve.Line.Fill = new Fill(Panadapter_Controls.Panadapter_Colors.Fill_Color, Panadapter_Controls.Panadapter_Colors.Fill_Color,
                            Panadapter_Controls.Panadapter_Colors.Fill_Color);
                            Panadapter_Controls.Panadapter_Colors.Previous_Fill_Color = Panadapter_Controls.Panadapter_Colors.Fill_Color;
                        }
                        if (Panadapter_Controls.Panadapter_Colors.Previous_Background_Color != Panadapter_Controls.Panadapter_Colors.Background_Color)
                        {
                            myPane.Chart.Fill = new Fill(Panadapter_Controls.Panadapter_Colors.Background_Color, Color.LightGoldenrodYellow, 45F);
                            Panadapter_Controls.Panadapter_Colors.Previous_Background_Color = Panadapter_Controls.Panadapter_Colors.Background_Color;
                        }
                        if (Panadapter_Controls.Panadapter_Colors.Cursor_Color != Panadapter_Controls.Panadapter_Colors.Previous_Cursor_Color)
                        {
                            cursor_user.Line.Color = Panadapter_Controls.Panadapter_Colors.Cursor_Color;
                            Panadapter_Controls.Panadapter_Colors.Previous_Cursor_Color = Panadapter_Controls.Panadapter_Colors.Cursor_Color;
                        }
                        if (Panadapter_Controls.Previous_Anti_Alias != Panadapter_Controls.Anti_Alias)
                        {
                            to_non_static.zedGraphControl2.IsAntiAlias = Panadapter_Controls.Anti_Alias;
                            Panadapter_Controls.Previous_Anti_Alias = Panadapter_Controls.Anti_Alias;
                        }

                        filter_size = to_non_static.Filter_size(Filter_control.Filter_High_Index);
                        to_non_static.Calculate_Band_Marker(Last_used.Current_mode, 0.0f, filter_size);
                        cursor_center.Location.X = Panadapter_Controls.Filter_Markers.Display_Center;
                        cursor_high.Location.X = Panadapter_Controls.Filter_Markers.G_band_marker_high;
                        cursor_low.Location.X = Panadapter_Controls.Filter_Markers.G_band_marker_low;

                        switch (Last_used.Current_mode)
                        {
                            case 'C':
                                cursor_center.Location.X = Panadapter_Controls.Filter_Markers.G_band_center;
                                //Message = "Display_Panadapter -> G_band_center: " + Convert.ToString(Panadapter_Controls.Filter_Markers.G_band_center) ;
                                //Write_Debug_Message(Message);
                                cursor_high.IsVisible = false;
                                cursor_low.IsVisible = false;
                                Mode = "CW";
                                break;
                            case 'A':
                                cursor_high.IsVisible = true;
                                cursor_low.IsVisible = true;
                                Mode = "AM";
                                break;
                            case 'L':
                                cursor_high.IsVisible = false;
                                cursor_low.IsVisible = true;
                                Mode = "LSB";
                                break;
                            case 'U':
                                cursor_high.IsVisible = true;
                                cursor_low.IsVisible = false;
                                Mode = "USB";
                                break;
                        }

                        if (Panadapter_Controls.Previous_mode != Last_used.Current_mode)
                        {
                            to_non_static.Graph_name(Mode);
                            Panadapter_Controls.Previous_mode = Last_used.Current_mode;
                        }
                        if (Panadapter_Controls.Previous_Filter_Index != Panadapter_Controls.Filter_Index)
                        {
                            to_non_static.Graph_name(Mode);
                            Panadapter_Controls.Previous_Filter_Index = Panadapter_Controls.Filter_Index;
                        }

                        //if (Panadapter_Controls.Display_Operation_Complete == false) return;
                        if (Panadapter_Controls.view_grid)
                        {
                            myPane.YAxis.IsVisible = true;
                        }
                        else
                        {
                            myPane.YAxis.IsVisible = false;
                        }
                        cursor_low.Line.Color = Panadapter_Controls.Panadapter_Colors.Marker_Color;
                        cursor_high.Line.Color = Panadapter_Controls.Panadapter_Colors.Marker_Color;
                        cursor_user.Line.Color = Panadapter_Controls.Panadapter_Colors.Cursor_Color;

                        Panadapter_Controls.Freq_Bounds.Current_X_Min = (int)myPane.XAxis.Scale.Min;
                        Panadapter_Controls.Freq_Bounds.Current_X_Max = (int)myPane.XAxis.Scale.Max;
                        //if (Master_Controls.Debug_Display)
                        //{
                        //    Message = Convert.ToString("Display Panadapter -> Current X min " + 
                        //        Panadapter_Controls.Freq_Bounds.Current_X_Min) +
                        //        " Current X Max " + Convert.ToString(Panadapter_Controls.Freq_Bounds.Current_X_Max) ;
                        //   Write_Debug_Message(Message);
                        //}
                        if (!Panadapter_Controls.Fine_Tuning)
                        {
                            if (Panadapter_Controls.Freq_Bounds.Current_X_Min >= 0)
                            {
                                Panadapter_Controls.Freq_Bounds.Low_Freq = (Panadapter_Controls.Display_freq -
                                    (Panadapter_Controls.Filter_Markers.DISPLAY_BANDWIDTH / 2)) +
                                    ((Panadapter_Controls.Filter_Markers.DISPLAY_BANDWIDTH / Panadapter_Controls.Filter_Markers.STARTUP_MAX_PIXELS) *
                                                                Panadapter_Controls.Freq_Bounds.Current_X_Min);
                                high_temp = Panadapter_Controls.Filter_Markers.STARTUP_MAX_PIXELS - Panadapter_Controls.Freq_Bounds.Current_X_Max;
                                Panadapter_Controls.Freq_Bounds.High_Freq = (Panadapter_Controls.Display_freq +
                                    (Panadapter_Controls.Filter_Markers.DISPLAY_BANDWIDTH / 2)) -
                                    ((Panadapter_Controls.Filter_Markers.DISPLAY_BANDWIDTH / Panadapter_Controls.Filter_Markers.STARTUP_MAX_PIXELS) *
                                                    high_temp);

                                to_non_static.Low_Freq_label1.Text = Convert.ToString(Panadapter_Controls.Freq_Bounds.Low_Freq);
                                to_non_static.Hiqh_Freq_label1.Text = Convert.ToString(Panadapter_Controls.Freq_Bounds.High_Freq);
                            }
                        }
                        myPane.YAxis.Scale.Max = Panadapter_Controls.Spectrum_Gain;
                        list.Clear();
                        spectrum_base_line_temp = Panadapter_Controls.Spectrum_Base_Line;
                        max_limit = Panadapter_Controls.Max_X * 2;
                        for (i = 0; i < max_limit; i++)
                        {
                            x = (double)i;
                            panadapter_y_value = (Panadapter_Controls.Display_Buffer.Y_value[i]);
                            panadapter_y_value = panadapter_y_value + spectrum_base_line_temp;
                            if (panadapter_y_value <= 0)
                            {
                                panadapter_y_value = 1;
                            }
                            if (panadapter_y_value > 7000)
                            {
                                panadapter_y_value = 1;
                            }
                            /*if (Master_Controls.Debug_Display)
                            {
                                if (panadapter_y_value > max_y)
                                {
                                    Message = ("Display Panadapter -> Max Y value: " + Convert.ToString(panadapter_y_value));
                                    Write_Debug_Message(Message);
                                    max_y = panadapter_y_value;
                                }
                                if (panadapter_y_value < min_y)
                                {
                                    Message = ("Display Panadapter -> Min Y value: " + Convert.ToString(panadapter_y_value));
                                    Write_Debug_Message(Message);
                                    min_y = panadapter_y_value;
                                }

                                Message = (" Display Panadapter -> Y value: " + Convert.ToString(panadapter_y_value));
                                Write_Debug_Message(Message);
                            }*/
                            y = (double)panadapter_y_value;
                            list.Add(x, y);
                        }

                        int MHz = (Panadapter_Controls.Display_freq + Panadapter_Controls.Fine_Tune_Delta) / 1000000;
                        int KHz = ((Panadapter_Controls.Display_freq + Panadapter_Controls.Fine_Tune_Delta) - (MHz * 1000000)) / 1000;
                        int Hz = (Panadapter_Controls.Display_freq + Panadapter_Controls.Fine_Tune_Delta) - (MHz * 1000000) - (KHz * 1000);
                        to_non_static.Panadapter_Freq_Label1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
                        if (first_pass)
                        {
                            ZoomState oldState = null;
                            ZoomState newState = null;
                            to_non_static.zg1_ZoomEvent(to_non_static.zedGraphControl2, oldState, newState);
                            to_non_static.Panadapter_Gain_hScrollBar1_Scroll(null, null);
                            first_pass = false;
                        }
                        to_non_static.zedGraphControl2.AxisChange();
                        to_non_static.zedGraphControl2.Refresh();
                        Panadapter_Controls.Display_Operation_Complete = true;
                        if (Master_Controls.Debug_Display)
                        {
                            Write_Debug_Message(" Display_Panadapter data Finished. Time: " +
                                DateTime.Now.ToString("mm:ss"));
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }

        private void zedGraphControl2_Load(object sender, EventArgs e)
        {
            //CreateGraph(zedGraphControl2);
        }

        private void CreateGraph(ZedGraphControl zgc)
        {
            String Message = "Create_Graph Starting \r\n";
            Write_Debug_Message(Message);
            Panadapter_Controls.Panadapter_Colors.Line_Color = Color.Blue;
            myPane = zgc.GraphPane;
            myCurve = myPane.AddCurve("", list, Panadapter_Controls.Panadapter_Colors.Line_Color,
                                            SymbolType.None);
            // Set the titles and axis labels
            
            myPane.Title.Text = "";
            myPane.XAxis.Title.Text = "";
            myPane.YAxis.Title.Text = "";
            

            myPane.YAxis.Type = AxisType.Text;
            myPane.YAxis.Scale.MagAuto = false;
            myPane.YAxis.Scale.MinAuto = false;
            myPane.YAxis.Scale.MaxAuto = false;
            myPane.YAxis.Scale.Max = 5000;
            myPane.YAxis.Scale.Min = 0;
            myPane.YAxis.Scale.Mag = 6000;
            myPane.YAxis.IsVisible = false;
            myPane.YAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MinorGrid.IsVisible = false;          
        

            myPane.XAxis.Type = AxisType.Ordinal;
            myPane.XAxis.Scale.MagAuto = true;
            myPane.XAxis.Scale.Mag = 10;
            myPane.XAxis.Scale.MinAuto = false;
            myPane.XAxis.Scale.MaxAuto = false;
            myPane.XAxis.Scale.Max = 800;
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.IsVisible = false;
            myPane.XAxis.MajorGrid.IsVisible = true;
            myCurve.IsOverrideOrdinal = true;
            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45F);
            // Fill the pane background with a color gradient
            myPane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45F);
            //zgc.RestoreScale(myPane);
            // Calculate the Axis Scale Ranges
            zgc.AxisChange();
            zgc.Refresh();
            Message = "Create_Graph Finished \r\n";
            Write_Debug_Message(Message);
        }
              
        private void Panadapter_Freq_Label_Click(object sender, EventArgs e)
        {

        }     
          
        public void Create_Marker_lines()
        {
            String Message = "Create_Marker_lines Starting \r\n";
            Write_Debug_Message(Message);
            
            Panadapter_Controls.Filter_Markers.Window_Size = zedGraphControl2.Width;

            //LineObj cursor = new LineObj();
            /*
            // Again Double.MinValue and Double.MaxValue doesn't give the desired result so I use the
            // min and max value of the x-axis again. But this time I trippled the length of the line
            // because sometimes the end of the line was visible when panning which doesn't look good.
            cursor.Location.Width = (zedGraphControl2.GraphPane.XAxis.Scale.Max - zedGraphControl2.GraphPane.XAxis.Scale.Min) * 3;
            cursor.Location.Height = 0; // height must be zero
            cursor.Location.X = zedGraphControl2.GraphPane.XAxis.Scale.Min - (cursor.Location.Width / 3);
            cursor.Location.Y = 37;
            cursor.Line.Color = Color.Red;
            cursor.Line.Style = System.Drawing.Drawing2D.DashStyle.Solid;
            cursor.Line.Width = 20;
            cursor.IsClippedToChartRect = true; // when true, line isn't drawn outside the boundaries of the chart rectangle
            cursor.Tag = "cursorY1";
            cursor.ZOrder = ZOrder.D_BehindAxis; // sets the order of the cursor in front of the curves, filling and gridlines but behind axis, border and legend. You can choose whatever you like of course.
            zedGraphControl2.GraphPane.GraphObjList.Add(cursor); // just add the cursor to the grapphane and that's it!

            // Adding the second line. This time done a little different just to show it can also be done in this way.
            double xAxisLength = zedGraphControl2.GraphPane.XAxis.Scale.Max - zedGraphControl2.GraphPane.XAxis.Scale.Min;
            cursor = new LineObj(Color.Black, zedGraphControl2.GraphPane.XAxis.Scale.Min - xAxisLength, 50, zedGraphControl2.GraphPane.XAxis.Scale.Max + xAxisLength, 50);
            cursor.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            cursor.IsClippedToChartRect = true;
            cursor.Tag = "cursorY2";
            cursor.ZOrder = ZOrder.D_BehindAxis;
            zedGraphControl2.GraphPane.GraphObjList.Add(cursor);*/

            
            //Create the center marker
            cursor_center.Location.Width = 0; // this time width must be zero
            cursor_center.Location.Height = (zedGraphControl2.GraphPane.YAxis.Scale.Max - zedGraphControl2.GraphPane.YAxis.Scale.Min) * 3;
            cursor_center.Location.X = Panadapter_Controls.Filter_Markers.Display_Center;
            cursor_center.Location.Y = zedGraphControl2.GraphPane.YAxis.Scale.Min - (cursor_center.Location.Width / 3);
            cursor_center.Line.Color = Color.Red;
            cursor_center.Line.Width = 2;
            cursor_center.Line.Style = System.Drawing.Drawing2D.DashStyle.DashDot;
            //cursor_center.Line.Style = System.Drawing.Drawing2D.HatchStyle.Cross;
            cursor_center.IsClippedToChartRect = true; // when true, line isn't drawn outside the boundaries of the chart rectangle
            cursor_center.Tag = "cursorX1";
            cursor_center.ZOrder = ZOrder.D_BehindAxis; // sets the order of the cursor in front of the curves, filling and gridlines but behind axis, border and legend. You can choose whatever you like of course.
            zedGraphControl2.GraphPane.GraphObjList.Add(cursor_center); // just add the cursor to the grapphane and that's it!

            //Create the high marker
            double yAxisLength = zedGraphControl2.GraphPane.YAxis.Scale.Max - zedGraphControl2.GraphPane.YAxis.Scale.Min;
            cursor_high.Line.Style = System.Drawing.Drawing2D.DashStyle.Solid;
            cursor_high.IsClippedToChartRect = true;
            cursor_high.Tag = "cursorX2";
            cursor_high.ZOrder = ZOrder.D_BehindAxis;
            cursor_high.Line.Width = 1;
            cursor_high.Line.Color = Color.Black;
            zedGraphControl2.GraphPane.GraphObjList.Add(cursor_high);

            //Now create the low marker
            double yAxisLength_1 = zedGraphControl2.GraphPane.YAxis.Scale.Max - zedGraphControl2.GraphPane.YAxis.Scale.Min;
            cursor_low.Line.Style = System.Drawing.Drawing2D.DashStyle.Solid;
            cursor_low.IsClippedToChartRect = true;
            cursor_low.Tag = "cursorX3";
            cursor_low.ZOrder = ZOrder.D_BehindAxis;
            cursor_low.Line.Width = 1;
            cursor_low.Line.Color = Color.Black;
            zedGraphControl2.GraphPane.GraphObjList.Add(cursor_low);

            //Now create the user cursor
            double yAxisLength_3 = zedGraphControl2.GraphPane.YAxis.Scale.Max - zedGraphControl2.GraphPane.YAxis.Scale.Min;
            cursor_user.Line.Style = System.Drawing.Drawing2D.DashStyle.Solid;
            cursor_user.IsClippedToChartRect = true;
            cursor_user.Tag = "cursorX4";
            cursor_user.ZOrder = ZOrder.D_BehindAxis;
            cursor_user.Line.Width = 1;
            cursor_user.Line.Color = Color.OrangeRed;
            zedGraphControl2.GraphPane.GraphObjList.Add(cursor_user);

            zedGraphControl2.AxisChange();
            zedGraphControl2.Refresh();
            Message = "Create_Marker_lines Finished \r\n";
            Write_Debug_Message(Message);
        }

        void zg1_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            foreach (GraphObj graphObj in sender.GraphPane.GraphObjList)
            {
                if (graphObj.Tag.ToString().Contains("cursorY"))
                {
                    graphObj.Location.Height = 0; // just to be sure
                    graphObj.Location.Width = (zedGraphControl2.GraphPane.XAxis.Scale.Max - 
                        zedGraphControl2.GraphPane.XAxis.Scale.Min) * 3;
                    graphObj.Location.X = zedGraphControl2.GraphPane.XAxis.Scale.Min - (graphObj.Location.Width / 3);
                }
                else if (graphObj.Tag.ToString().Contains("cursorX"))
                {
                    graphObj.Location.Width = 0; // just to be sure
                    graphObj.Location.Height = (zedGraphControl2.GraphPane.YAxis.Scale.Max - 
                        zedGraphControl2.GraphPane.YAxis.Scale.Min) * 3;
                    graphObj.Location.Y = zedGraphControl2.GraphPane.YAxis.Scale.Min - (graphObj.Location.Height / 3);
                }
            }
        }     

        private void zedGraphControl2_Load_1(object sender, EventArgs e)
        {
            InitializeComponent();
        }

        private void Panadapter_1_Load_1(object sender, EventArgs e)
        {
            String Message = "Panadapter_1_Load_1 Starting";
            Write_Debug_Message(Message);
            if (!Settings.Default.Panadapter_Location.IsEmpty)
            {
                this.Location = Settings.Default.Panadapter_Location;
            }
            if(!Settings.Default.Panadapter_Size.IsEmpty)
            {
                this.Size = Settings.Default.Panadapter_Size;
            }
            //InitializeComponent();
            Start_Panadapter_1();
            zedGraphControl2.Show();
            Spectrum_thread.Start();

            Message = "Panadapter_1_Load_1 Finished";
            Write_Debug_Message(Message);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Window_controls.Panadapter_Controls.Panadapter_Control_Window != null)
            {
             
                Window_controls.Panadapter_Controls.panadapter_form_displosed = 
                    Window_controls.Panadapter_Controls.Panadapter_Control_Window.IsDisposed;
              
            }
            if (Window_controls.Panadapter_Controls.Panadapter_Control_Window == null)
            {
                Window_controls.Panadapter_Controls.Panadapter_Control_Window = new panadapter_control();
              
                Window_controls.Panadapter_Controls.panadapter_form_displosed = false;
                if (Window_controls.Panadapter_Controls.Panadapter_Control_Window != null)
                {
                    Window_controls.Panadapter_Controls.panadapter_window_created = true;
                }
            }
            if (Window_controls.Panadapter_Controls.Panadapter_Control_Window != null && 
                !Window_controls.Panadapter_Controls.panadapter_form_displosed)
            {
                Window_controls.Panadapter_Controls.panadapter_form_state = 
                    (int)Window_controls.Panadapter_Controls.Panadapter_Control_Window.WindowState;
               

                if (Window_controls.Panadapter_Controls.display_panadapter_form)
                {
                    Window_controls.Panadapter_Controls.Panadapter_Control_Window.Show();
                    Window_controls.Panadapter_Controls.panadapter_window_displayed = true;
                    Window_controls.Panadapter_Controls.display_panadapter_form = false;
                  
                    Window_controls.Panadapter_Controls.panadapter_form_state = 
                        (int)Window_controls.Panadapter_Controls.Panadapter_Control_Window.WindowState;
                   
                }
                else
                {
                    if (Window_controls.Panadapter_Controls.panadapter_form_state == 1)
                    {
                        Window_controls.Panadapter_Controls.Panadapter_Control_Window.WindowState = FormWindowState.Normal;
                        Window_controls.Panadapter_Controls.Panadapter_Control_Window.Activate();
                        Window_controls.Panadapter_Controls.Panadapter_Control_Window.Show();
                        Window_controls.Panadapter_Controls.Panadapter_Control_Window.Focus();
                        Window_controls.Panadapter_Controls.panadapter_window_displayed = true;
                    }
                    else
                    {
                        Window_controls.Panadapter_Controls.Panadapter_Control_Window.Hide();
                        Window_controls.Panadapter_Controls.display_panadapter_form = true;
                        Window_controls.Panadapter_Controls.panadapter_window_displayed = false;
                        //MonitorTextBoxText(  " Panadapter Popup Hide" );
                    }
                }
            }
            else
            {
                if (Window_controls.Panadapter_Controls.Panadapter_Control_Window.IsDisposed)
                {
                    Window_controls.Panadapter_Controls.Panadapter_Control_Window.Close();
                    Window_controls.Panadapter_Controls.Panadapter_Control_Window = null;
                    Window_controls.Panadapter_New.window_displayed = false;
                    Window_controls.Panadapter_New.display_form = true;
                }
            }
        }

        private void Panadapter_Mouse_label1_Click_1(object sender, EventArgs e)
        {

        }

        private void Fine_Tune_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int index = 0;
            int fine_tune_freq = 0;
            int high_temp = 0;

           
            if (Panadapter_Controls.Fine_Tune_Delta == index) return;
          
            Panadapter_Controls.Fine_Tuning = true;
            fine_tune_freq = Panadapter_Controls.Display_freq + Panadapter_Controls.Fine_Tune_Delta;
            oCode.SendCommand32(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, oCode.CMD_SET_MAIN_FREQ, 
                fine_tune_freq);
            oCode.DisplayFreq = fine_tune_freq;
            Panadapter_Controls.Freq_Bounds.Current_X_Min = (int)myPane.XAxis.Scale.Min;
            Panadapter_Controls.Freq_Bounds.Current_X_Max = (int)myPane.XAxis.Scale.Max;
            //Message = Convert.ToString(Panadapter_Controls.Freq_Bounds.Current_X_Min) + " " +
            //Convert.ToString(Panadapter_Controls.Freq_Bounds.Current_X_Max) ;
            //Write_Debug_Message(Message);
            if (Panadapter_Controls.Freq_Bounds.Current_X_Min >= 0)
            {
                Panadapter_Controls.Freq_Bounds.Low_Freq = (Panadapter_Controls.Display_freq -
                        (Panadapter_Controls.Filter_Markers.DISPLAY_BANDWIDTH / 2)) +
                        ((Panadapter_Controls.Filter_Markers.DISPLAY_BANDWIDTH / Panadapter_Controls.Filter_Markers.STARTUP_MAX_PIXELS) *
                                                    Panadapter_Controls.Freq_Bounds.Current_X_Min);
                high_temp = Panadapter_Controls.Filter_Markers.STARTUP_MAX_PIXELS - Panadapter_Controls.Freq_Bounds.Current_X_Max;
                Panadapter_Controls.Freq_Bounds.High_Freq = (Panadapter_Controls.Display_freq +
                    (Panadapter_Controls.Filter_Markers.DISPLAY_BANDWIDTH / 2)) -
                    ((Panadapter_Controls.Filter_Markers.DISPLAY_BANDWIDTH / Panadapter_Controls.Filter_Markers.STARTUP_MAX_PIXELS) *
                                    high_temp);
                Low_Freq_label1.Text = Convert.ToString(Panadapter_Controls.Freq_Bounds.Low_Freq);
                Hiqh_Freq_label1.Text = Convert.ToString(Panadapter_Controls.Freq_Bounds.High_Freq);
            }
            Low_Freq_label1.Text = Convert.ToString(Panadapter_Controls.Freq_Bounds.Low_Freq);
            Hiqh_Freq_label1.Text = Convert.ToString(Panadapter_Controls.Freq_Bounds.High_Freq);
            Display_Main_Freq_by_Panadapter();
        }

        private void Reset_Freq_button1_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            oCode.DisplayFreq = Panadapter_Controls.Display_freq;
            Panadapter_Controls.Fine_Tune_Delta = 0;
           
            Panadapter_Controls.Freq_Bounds.Current_X_Min = (int)myPane.XAxis.Scale.Min;
            Panadapter_Controls.Freq_Bounds.Current_X_Max = (int)myPane.XAxis.Scale.Max;
            if (Panadapter_Controls.Freq_Bounds.Current_X_Min >= 0)
            {
                Panadapter_Controls.Freq_Bounds.Low_Freq = (Panadapter_Controls.Display_freq -
                    (Panadapter_Controls.Filter_Markers.DISPLAY_BANDWIDTH / 2)) + Panadapter_Controls.Freq_Bounds.Current_X_Min;

                Panadapter_Controls.Freq_Bounds.High_Freq = (Panadapter_Controls.Display_freq +
                    (Panadapter_Controls.Filter_Markers.DISPLAY_BANDWIDTH / 2)) -
                                        (Panadapter_Controls.Filter_Markers.STARTUP_MAX_PIXELS - Panadapter_Controls.Freq_Bounds.Current_X_Max);
            }
            Low_Freq_label1.Text = Convert.ToString(Panadapter_Controls.Freq_Bounds.Low_Freq);
            Hiqh_Freq_label1.Text = Convert.ToString(Panadapter_Controls.Freq_Bounds.High_Freq);
            Display_Main_Freq_by_Panadapter();
            oCode.SendCommand32(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, oCode.CMD_SET_MAIN_FREQ,
                                                    Panadapter_Controls.Display_freq);
            Panadapter_Controls.Fine_Tuning = false;
        }

        public bool Update_User_values()
        {
            String path, line;
            System.IO.StreamReader file;
            short temp_int = 0;
            String temp_string;
            short fill_value = 0;
            short line_value = 0;
            short marker_value = 0;
            int base_value = 0;
            short auto_snap_status = 0;
            short auto_snap_index = 0;

            List<string> MyList = new List<string>();

            String Message = "Update_User_values started";
            Write_Debug_Message(Message);
            oCode.Platform = (int)Environment.OSVersion.Platform;
            // get path to local Appdata folder
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            // add our folder and file name
            if ((oCode.Platform == 4) || (oCode.Platform == 6) || (oCode.Platform == 128))
            {// A kludge to check for non Windows OS.  
             //These values may change in the future.
                path += "//multus-sdr-mfc//user_controls.ini";
            }
            else
            {
                path += "\\multus-sdr-mfc\\user_controls.ini";
            }
            // try to open the file
            try
            {
                //file = new System.IO.StreamReader(path);
                file = new System.IO.StreamReader(File.OpenRead(path));
            }

            // if the file open fails, whine prettily and return false
            catch (IOException e)
            {
                string er = e.Message;
                DialogResult ret = MessageBox.Show("user_controls.ini Open Failed: " + er + V,
                    "MSCC-Core", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            while ((line = file.ReadLine()) != null)
            {
                temp_string = getBetween(line, "PANADAPTER_GAIN=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    gain_value = temp_int;
                    Message =   " PANADAPTER_GAIN: " + Convert.ToString(temp_int) ;
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "PANADAPTER_BASE=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    base_value = temp_int;
                    Message =   " PANADAPTER_BASE: " + Convert.ToString(temp_int) ;
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "PANADAPTER_FILL=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    fill_value = temp_int;
                    Message =   " PANADAPTER_FILL: " + Convert.ToString(temp_int) ;
                    Write_Debug_Message(Message);
                    switch (fill_value)
                    {
                        case 0:
                            Panadapter_Controls.Panadapter_Colors.Fill_Color = Color.Red;
                            break;
                        case 1:
                            Panadapter_Controls.Panadapter_Colors.Fill_Color = Color.Blue;
                            break;
                        case 2:
                            Panadapter_Controls.Panadapter_Colors.Fill_Color = Color.Green;
                            break;
                        case 3:
                            Panadapter_Controls.Panadapter_Colors.Fill_Color = Color.Yellow;
                            break;
                        case 4:
                            Panadapter_Controls.Panadapter_Colors.Fill_Color = Color.LightGoldenrodYellow;
                            break;
                        case 5:
                            Panadapter_Controls.Panadapter_Colors.Fill_Color = Color.Black;
                            break;
                    }
                }
                temp_string = getBetween(line, "PANADAPTER_LINE=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    line_value = temp_int;
                    Message =   " PANADAPTER_LINE: " + Convert.ToString(temp_int) ;
                    Write_Debug_Message(Message);
                    switch (line_value)
                    {
                        case 0:
                            Panadapter_Controls.Panadapter_Colors.Line_Color = Color.Red;
                            break;
                        case 1:
                            Panadapter_Controls.Panadapter_Colors.Line_Color = Color.Blue;
                            break;
                        case 2:
                            Panadapter_Controls.Panadapter_Colors.Line_Color = Color.Green;
                            break;
                        case 3:
                            Panadapter_Controls.Panadapter_Colors.Line_Color = Color.Yellow;
                            break;
                        case 4:
                            Panadapter_Controls.Panadapter_Colors.Line_Color = Color.LightGoldenrodYellow;
                            break;
                        case 5:
                            Panadapter_Controls.Panadapter_Colors.Line_Color = Color.Black;
                            break;
                    }
                }
                
                temp_string = getBetween(line, "PANADAPTER_MARKER=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    marker_value = temp_int;
                    Message =   " PANADAPTER_MARKER: " + Convert.ToString(temp_int) ;
                    Write_Debug_Message(Message);
                    switch (marker_value)
                    {
                        case 0:
                            Panadapter_Controls.Panadapter_Colors.Marker_Color = Color.Red;
                            break;
                        case 1:
                            Panadapter_Controls.Panadapter_Colors.Marker_Color = Color.Blue;
                            break;
                        case 2:
                            Panadapter_Controls.Panadapter_Colors.Marker_Color = Color.Green;
                            break;
                        case 3:
                            Panadapter_Controls.Panadapter_Colors.Marker_Color = Color.Yellow;
                            break;
                        case 4:
                            Panadapter_Controls.Panadapter_Colors.Marker_Color = Color.LightGoldenrodYellow;
                            break;
                        case 5:
                            Panadapter_Controls.Panadapter_Colors.Marker_Color = Color.Black;
                            break;
                    }
                }
                temp_string = getBetween(line, "PANADAPTER_BACKGROUND=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    line_value = temp_int;
                    Message =   " PANADAPTER_BACKGROUND: " + Convert.ToString(temp_int) ;
                    Write_Debug_Message(Message);
                    switch (line_value)
                    {
                        case 0:
                            Panadapter_Controls.Panadapter_Colors.Background_Color = Color.Red;
                            break;
                        case 1:
                            Panadapter_Controls.Panadapter_Colors.Background_Color = Color.Blue;
                            break;
                        case 2:
                            Panadapter_Controls.Panadapter_Colors.Background_Color = Color.Green;
                            break;
                        case 3:
                            Panadapter_Controls.Panadapter_Colors.Background_Color = Color.Yellow;
                            break;
                        case 4:
                            Panadapter_Controls.Panadapter_Colors.Background_Color = Color.LightGoldenrodYellow;
                            break;
                        case 5:
                            Panadapter_Controls.Panadapter_Colors.Background_Color = Color.Black;
                            break;
                    }
                }
                temp_string = getBetween(line, "PANADAPTER_CURSOR=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    line_value = temp_int;
                    Message =   " PANADAPTER_CURSOR: " + Convert.ToString(temp_int) ;
                    Write_Debug_Message(Message);
                    switch (line_value)
                    {
                        case 0:
                            Panadapter_Controls.Panadapter_Colors.Cursor_Color = Color.Red;
                            break;
                        case 1:
                            Panadapter_Controls.Panadapter_Colors.Cursor_Color = Color.Blue;
                            break;
                        case 2:
                            Panadapter_Controls.Panadapter_Colors.Cursor_Color = Color.Green;
                            break;
                        case 3:
                            Panadapter_Controls.Panadapter_Colors.Cursor_Color = Color.Yellow;
                            break;
                        case 4:
                            Panadapter_Controls.Panadapter_Colors.Cursor_Color = Color.LightGoldenrodYellow;
                            break;
                        case 5:
                            Panadapter_Controls.Panadapter_Colors.Cursor_Color = Color.Black;
                            break;
                    }
                }
                temp_string = getBetween(line, "AUTO_SNAP_STATUS=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    auto_snap_status = temp_int;
                    Message = " AUTO_SNAP_STATUS: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }
                temp_string = getBetween(line, "AUTO_SNAP_INDEX=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    auto_snap_index = temp_int;
                    Message = " AUTO_SNAP_INDEX: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }
            }
            file.Close();
            code_triggered = false;
            Panadapter_Gain_hScrollBar1.Value = gain_value;
            //Panadapter_Controls.Spectrum_Gain = (6000 - gain_value);
            //Panadapter_Gain_hScrollBar1_Scroll(null, null);
            Panadapter_Base_line_hScrollBar1.Value = base_value;
            Panadapter_Base_line_hScrollBar1_Scroll(null, null);
            if (auto_snap_status == 0)
            {
                Panadapter_Controls.Auto_Snap.Auto_Snap_Checkbox = false;
            }
            else
            {
                Panadapter_Controls.Auto_Snap.Auto_Snap_Checkbox = true;
            }
            switch (auto_snap_index)
            {
                case 0:
                    Panadapter_Controls.Auto_Snap.Snap_Value = 1000;
                    break;
                case 1:
                    Panadapter_Controls.Auto_Snap.Snap_Value = 500;
                    break;
                case 2:
                    Panadapter_Controls.Auto_Snap.Snap_Value = 100;
                    break;
                case 3:
                    Panadapter_Controls.Auto_Snap.Snap_Value = 10;
                    break;
            }
           
            code_triggered = false;
            Message = "Update_User_values Finished";
            Write_Debug_Message(Message);
            return true;
        }

        private void Panadapter_Window_Restore_button1_Click(object sender, EventArgs e)
        {
            this.Size = new Size(800, 480);
        }

        public void Panadapter_CW_Snap_button1_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (Last_used.Current_mode == 'C')
            {
                if (!Panadapter_Controls.CW_Snap.CW_button)
                {
                    Panadapter_CW_Snap_button1.BackColor = Color.Red;
                    Panadapter_Controls.CW_Snap.Button_Color = Color.Red;
                    Panadapter_Controls.CW_Snap.CW_button = true;
                    Panadapter_Controls.CW_Snap.CW_snap_status = true;
                    oCode.SendCommand32(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget,
                                        Panadapter_Controls.CMD_CW_SNAP_START, Panadapter_Controls.Display_freq);

                }
            }
        }

        private void Panadapter_Gain_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value;
            if (oCode.isLoading) return;
            if (!code_triggered)
            {
                value = Panadapter_Gain_hScrollBar1.Value;
                if (value == Panadapter_Controls.Spectrum_Gain) return;
                //MonitorTextBoxText(  "Panadapter Gain Called" );
                Panadapter_Controls.Spectrum_Gain = (6000 - value);
                oCode.SendCommand32(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, Panadapter_Controls.CMD_GET_SET_PANADAPTER_GAIN, value);
            }
        }

        private void Panadapter_Base_line_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value;
            if (oCode.isLoading) return;
            if (!code_triggered)
            {
                value = Panadapter_Base_line_hScrollBar1.Value;
                if (Panadapter_Controls.Spectrum_Base_Line == value) return;
                Panadapter_Controls.Spectrum_Base_Line = (value * 10);
                oCode.SendCommand32(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, Panadapter_Controls.CMD_GET_SET_PANADAPTER_BASE, value);
                //Panadapter_Controls.Spectrum_Base_Line = (value * 50);
            }
        }

        private void Panadapter_Freq_Label1_Click(object sender, EventArgs e)
        {

        }

        private void Low_Freq_label1_Click(object sender, EventArgs e)
        {

        }

        private void Hiqh_Freq_label1_Click(object sender, EventArgs e)
        {

        }

        private void Mode_label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using OmniaGUI.Properties;

namespace OmniaGUI
{
    public partial class panadapter_control : Form
    {
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
        static public bool code_triggered = false;
        public static int gain_value = 0;

        public panadapter_control()
        {
            InitializeComponent();
            Init_User_values();
#if RPI
            Panadapter_Base_line_hScrollBar1.Maximum = 500;
            Panadapter_Base_line_hScrollBar1.Minimum = 0;
#endif
        }

        public bool Init_User_values()
        {
            String path, line;
            System.IO.StreamReader file;
            short temp_int = 0;
            String temp_string;
            short fill_value = 0;
            short background_value = 0;
            short line_value = 0;
            short marker_value = 0;
            short cursor_value = 0;
            short base_value = 0;
            short auto_snap_status = 0;
            short auto_snap_index = 0;
            short average_value = 0;
            short refresh_value = 0;

            List<string> MyList = new List<string>();

            String Message = "Panadapter_Control -> Init_User_values started";
            Write_Debug_Message(Message);
            // get path to local Appdata folder
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            // add our folder and file name
#if RPI
            path += "/mscc/user_controls.ini";
#else
            path += "\\multus-sdr-client\\user_controls.ini";
#endif

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
                DialogResult ret = MessageBox.Show("user_controls.ini Open Failed: " + er,
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
                    Message = " PANADAPTER_GAIN: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "PANADAPTER_BASE=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    base_value = temp_int;
                    Message = " PANADAPTER_BASE: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "PANADAPTER_FILL=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    fill_value = temp_int;
                    Message = " PANADAPTER_FILL: " + Convert.ToString(temp_int);
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
                    Message = " PANADAPTER_LINE: " + Convert.ToString(temp_int);
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
                    Message = " PANADAPTER_MARKER: " + Convert.ToString(temp_int);
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
                    background_value = temp_int;
                    Message = " PANADAPTER_BACKGROUND: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                    switch (background_value)
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
                    cursor_value = temp_int;
                    Message = " PANADAPTER_CURSOR: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                    switch (cursor_value)
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
                temp_string = getBetween(line, "PANADAPTER_AVERAGE=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    Message = " PANADAPTER_AVERAGE: " + Convert.ToString(temp_int);
                    average_value = temp_int;
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "PANADAPTER_REFRESH=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    Message = " PANADAPTER_AVERAGE: " + Convert.ToString(temp_int);
                    refresh_value = temp_int;
                    Write_Debug_Message(Message);
                }
            }
            file.Close();
            code_triggered = false;
            if (base_value > 100)
            {
                base_value = 100;
            }
            Panadapter_Base_line_hScrollBar1.Value = base_value;
            if (gain_value > 6000)
            {
                gain_value = 6000;
            }
            Panadapter_Gain_hScrollBar1.Value = gain_value;
            CW_Snap_checkBox1.CheckedChanged -= CW_Snap_checkBox1_CheckedChanged_1;
            if (auto_snap_status == 0)
            {
                CW_Snap_checkBox1.Checked = false;
                Panadapter_Controls.Auto_Snap.Auto_Snap_Checkbox = false;
            }
            else
            {
                CW_Snap_checkBox1.Checked = true;
                Panadapter_Controls.Auto_Snap.Auto_Snap_Checkbox = true;
            }
            CW_Snap_checkBox1.CheckedChanged += CW_Snap_checkBox1_CheckedChanged_1;
            listBox2.SelectedIndex = auto_snap_index;
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
            Panadaper_Color_listBox1.SelectedIndex = fill_value;
            Panadapter_Line_listBox1.SelectedIndex = line_value;
            Panadapter_Marker_listBox1.SelectedIndex = marker_value;
            Panadapter_Background_listBox1.SelectedIndex = background_value;
            listBox1.SelectedIndex = cursor_value;
            Panadapter_Controls.Smoothing_Index = (byte)average_value;
            Panadapter_Smooth_ListBox1.SelectedIndex = Panadapter_Controls.Smoothing_Index;
            Refresh_listBox2.SelectedIndex = refresh_value;
            Panadapter_Controls.Refresh_Index = (byte)refresh_value;

            Color_listBox1.SelectedIndex = Window_controls.Waterfall_Controls.pallet_index;
            Waterfall_Gain_hScrollBar1.Value = Window_controls.Waterfall_Controls.gain;
            Display_GDI.WaterfallHighThreshold = 7000.0f - (float)Window_controls.Waterfall_Controls.gain;
            Waterfall_zero_hScrollBar1.Value = Window_controls.Waterfall_Controls.zero;
            Speed_hScrollBar1.Value = Window_controls.Waterfall_Controls.window_speed;
            Waterfall_Marker_listBox3.SelectedIndex = Settings.Default.Waterfall_Marker_Color_Index;
            Message = "Panadapter_Control -> Init_User_values Finished";
            Write_Debug_Message(Message);
            return true;
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void Panadapter_Fill_label44_Click(object sender, EventArgs e)
        {

        }

        private void Panadaper_Color_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;

            if (oCode.isLoading) return;
            if (!code_triggered)
            {
                index = Panadaper_Color_listBox1.SelectedIndex;
                switch (index)
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
                oCode.SendCommand(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, Panadapter_Controls.CMD_GET_SET_PANADAPTER_FILL,
                    (short)index);
            }
        }

        private void Panadapter_Line_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;

            if (oCode.isLoading) return;
            if (!code_triggered)
            {
                index = Panadapter_Line_listBox1.SelectedIndex;
                switch (index)
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
                oCode.SendCommand(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, Panadapter_Controls.CMD_GET_SET_PANADAPTER_LINE,
                                    (short)index);
            }
        }

        private void Panadapter_Marker_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;

            if (oCode.isLoading) return;
            if (!code_triggered)
            {
                index = Panadapter_Marker_listBox1.SelectedIndex;
                switch (index)
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
                oCode.SendCommand(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, Panadapter_Controls.CMD_GET_SET_PANADAPTER_MARKER,
                   (short)index);
            }
        }

        private void Panadapter_Base_Linelabel44_Click(object sender, EventArgs e)
        {

        }

        private void Panadapter_Gain_label44_Click(object sender, EventArgs e)
        {

        }

        private void Panadapter_Marker_label44_Click(object sender, EventArgs e)
        {

        }

        private void Panadapter_Line_label44_Click(object sender, EventArgs e)
        {

        }

        private void panadapter_control_Load(object sender, EventArgs e)
        {
            if (Settings.Default.Panadapter_Control_Location != null)
            {
                this.Location = Settings.Default.Panadapter_Control_Location;
            }
            Time_Grid_button1.Text = Convert.ToString(Settings.Default.Time_Grid);
        }

        private void Panadapter_Grid_checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if(!Panadapter_Controls.view_grid)
            {
                Panadapter_Controls.view_grid = true;
            }
            else
            {
                Panadapter_Controls.view_grid = false;
            }
        }

        private void TX_Panadapter_checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (!Panadapter_Controls.TX_Panadapter)
            {
                //TX_Panadapter_checkBox1.Text = "TX PANADAPTER ON";
                Panadapter_Controls.TX_Panadapter = true;
            }
            else
            {
                //TX_Panadapter_checkBox1.Text = "TX PANADAPTER OFF";
                Panadapter_Controls.TX_Panadapter = false;
            }
        }

        public void Write_Debug_Message(String Message)
        {
            var thisForm2 = Application.OpenForms.OfType<Main_form>().Single();
            thisForm2.Write_Message(Message);
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

        public bool Update_Control_values()
        {
            String path, line;
            System.IO.StreamReader file;
            short temp_int = 0;
            String temp_string;
            short fill_value = 0;
            short line_value = 0;
            short marker_value = 0;
            short background_value = 0;
            short cursor_value = 0;
            short average_value = 0;
            short base_value = 0;
            short refresh = 0;
            short auto_snap_index = 0;
            short auto_snap_status = 0;


            List<string> MyList = new List<string>();
            // get path to local Appdata folder
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            // add our folder and file name
#if RPI
            path += "/mscc/user_controls.ini";
#else
            path += "\\multus-sdr-client\\user_controls.ini";
#endif
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
                DialogResult ret = MessageBox.Show("panadapter-controls -> user_controls.ini Open Failed: " + er +
                    " Make note of the error and contact Multus SDR, LLC.",
                    "MSCC -Core", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            while ((line = file.ReadLine()) != null)
            {
                temp_string = getBetween(line, "PANADAPTER_AVERAGE=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    String Message = " PANADAPTER_AVERAGE: " + Convert.ToString(temp_int);
                    average_value = temp_int;
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "PANADAPTER_REFRESH=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    String Message = " PANADAPTER_AVERAGE: " + Convert.ToString(temp_int);
                    refresh = temp_int;
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "PANADAPTER_FILL=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    fill_value = temp_int;
                    String Message = " PANADAPTER_FILL: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "PANADAPTER_LINE=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    line_value = temp_int;
                    String Message = " PANADAPTER_LINE: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }
                temp_string = getBetween(line, "PANADAPTER_MARKER=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    marker_value = temp_int;
                    String Message = " PANADAPTER_MARKER: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }
                temp_string = getBetween(line, "PANADAPTER_BACKGROUND=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    background_value = temp_int;
                    String Message = " PANADAPTER_MARKER: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }
                temp_string = getBetween(line, "PANADAPTER_CURSOR=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    cursor_value = temp_int;
                    String Message = " PANADAPTER_CURSOR: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }
                temp_string = getBetween(line, "PANADAPTER_SMOOTHING=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    average_value = temp_int;
                    String Message = " PANADAPTER_CURSOR: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }
                temp_string = getBetween(line, "AUTO_SNAP_STATUS=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    auto_snap_status = temp_int;
                    String Message = " AUTO_SNAP_STATUS: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }
                temp_string = getBetween(line, "AUTO_SNAP_INDEX=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    auto_snap_index = temp_int;
                    String Message = " AUTO_SNAP_INDEX: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }
                temp_string = getBetween(line, "PANADAPTER_BASE=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    base_value = temp_int;
                    Write_Debug_Message(" PANADAPTER_BASE: " + Convert.ToString(temp_int));
                }

            }
            file.Close();
            if(base_value > 100)
            {
                base_value = 100;
            }
            Panadapter_Base_line_hScrollBar1.Value = base_value;
            if(gain_value > 6000)
            {
                gain_value = 6000;
            }
            Panadapter_Gain_hScrollBar1.Value = gain_value;
            Panadaper_Color_listBox1.SelectedIndex = fill_value;
            Panadapter_Line_listBox1.SelectedIndex = line_value;
            Panadapter_Marker_listBox1.SelectedIndex = marker_value;
            Panadapter_Background_listBox1.SelectedIndex = background_value;
            listBox1.SelectedIndex = cursor_value;
            Panadapter_Controls.Smoothing_Index = (byte)average_value;
            Panadapter_Smooth_ListBox1.SelectedIndex = Panadapter_Controls.Smoothing_Index;
            Refresh_listBox2.SelectedIndex = refresh;
            Panadapter_Controls.Refresh_Index = (byte)refresh;
            Master_Controls.code_triggered = true;
            //CW_Snap_checkBox1_CheckedChanged
            if (auto_snap_status == 0)
            {
                CW_Snap_checkBox1.CheckState = CheckState.Unchecked;
                Panadapter_Controls.Auto_Snap.Auto_Snap_Checkbox = false;
            }
            else
            {
                CW_Snap_checkBox1.CheckState = CheckState.Checked;
                Panadapter_Controls.Auto_Snap.Auto_Snap_Checkbox = true;
            }
            Master_Controls.code_triggered = false;

            listBox2.SelectedIndex = auto_snap_index;
            if (Display_GDI.ReverseWaterfall == true)
            {
                Direction_checkBox1.Text = "Direction Reversed";
            }
            else
            {
                Direction_checkBox1.Text = "Direction Normal";
            }
            Color_listBox1.SelectedIndex = Window_controls.Waterfall_Controls.pallet_index;
            Waterfall_Gain_hScrollBar1.Value = Window_controls.Waterfall_Controls.gain;
            Display_GDI.WaterfallHighThreshold = 7000.0f - (float)Window_controls.Waterfall_Controls.gain;
            Waterfall_zero_hScrollBar1.Value = Window_controls.Waterfall_Controls.zero;
            Speed_hScrollBar1.Value = Window_controls.Waterfall_Controls.window_speed;
            Waterfall_Marker_listBox3.SelectedIndex = Settings.Default.Waterfall_Marker_Color_Index;
            return true;
        }

        private void Panadapter_Background_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;

            if (oCode.isLoading) return;
            if (!code_triggered)
            {
                index = Panadapter_Background_listBox1.SelectedIndex;
                switch (index)
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
                oCode.SendCommand(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, Panadapter_Controls.CMD_GET_SET_PANADAPTER_BACKGROUND,
                   (short)index);
            }
        }

        private void Line_Alias_checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (Panadapter_Controls.Anti_Alias)
            {
                Panadapter_Controls.Anti_Alias = false;
            }
            else
            {
                Panadapter_Controls.Anti_Alias = true;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;

            if (oCode.isLoading) return;
            if (!code_triggered)
            {
                index = listBox1.SelectedIndex;
                switch (index)
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
                oCode.SendCommand(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, Panadapter_Controls.CMD_GET_SET_PANADAPTER_CURSOR,
                    (short)index);
            }
        }

        private void Refresh_label1_Click(object sender, EventArgs e)
        {

        }

        private void Refresh_listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;
            if (oCode.isLoading) return;
            if (!code_triggered)
            {
                index = Refresh_listBox2.SelectedIndex;
                Panadapter_Controls.Refresh_Index = (byte) index;
                oCode.SendCommand(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, Panadapter_Controls.CMD_GET_SET_PANADAPTER_REFRESH,
                    (byte)index);
            }
        }

        private void Panadapter_Smooth_ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (!code_triggered)
            {
                Panadapter_Controls.Smoothing_Index = (byte)Panadapter_Smooth_ListBox1.SelectedIndex;
                oCode.SendCommand(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget,
                           Panadapter_Controls.CMD_GET_SET_PANADAPTER_SMOOTHING, Panadapter_Controls.Smoothing_Index);
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index;
            if (oCode.isLoading) return;
            index = listBox2.SelectedIndex;
            switch (index)
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
            oCode.SendCommand(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget,
                           Panadapter_Controls.CMD_SET_AUTO_SNAP_INDEX, (short)index);
        }

        private void CW_Snap_checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (!Master_Controls.code_triggered)
            {
                if (Panadapter_Controls.Auto_Snap.Auto_Snap_Checkbox == false)
                {
                    Panadapter_Controls.Auto_Snap.Auto_Snap_Checkbox = true;
                    oCode.SendCommand(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget,
                               Panadapter_Controls.CMD_SET_AUTO_SNAP_STATUS, 1);
                    Write_Debug_Message(" CW_Snap_checkBox1 -> Status: 1");
                }
                else
                {
                    Panadapter_Controls.Auto_Snap.Auto_Snap_Checkbox = false;
                    oCode.SendCommand(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget,
                               Panadapter_Controls.CMD_SET_AUTO_SNAP_STATUS, 0);
                    Write_Debug_Message(" CW_Snap_checkBox1 -> Status: 0");
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void Waterfall_Marker_listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;
            index = Waterfall_Marker_listBox3.SelectedIndex;
            Settings.Default.Waterfall_Marker_Color_Index = Waterfall_Marker_listBox3.SelectedIndex;
            switch (index)
            {
                case 0:
                    Display_GDI.MarkerFilterColor = Color.Red;
                    Settings.Default.Waterfall_Marker = Color.Red;
                    break;
                case 1:
                    Display_GDI.MarkerFilterColor = Color.Blue;
                    Settings.Default.Waterfall_Marker = Color.Blue;
                    break;
                case 2:
                    Display_GDI.MarkerFilterColor = Color.Green;
                    Settings.Default.Waterfall_Marker = Color.Green;
                    break;
                case 3:
                    Display_GDI.MarkerFilterColor = Color.Yellow;
                    Settings.Default.Waterfall_Marker = Color.Yellow;
                    break;
                case 4:
                    Display_GDI.MarkerFilterColor = Color.White;
                    Settings.Default.Waterfall_Marker = Color.White;
                    break;
                case 5:
                    Display_GDI.MarkerFilterColor = Color.Black;
                    Settings.Default.Waterfall_Marker = Color.Black;
                    break;

            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Waterfall_Cursor_listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;
            index = Waterfall_Marker_listBox3.SelectedIndex;
            switch (index)
            {
                case 0:
                    Display_GDI.CursorColor = Color.Red;
                    break;
                case 1:
                    Display_GDI.CursorColor = Color.Blue;
                    break;
                case 2:
                    Display_GDI.CursorColor = Color.Green;
                    break;
                case 3:
                    Display_GDI.CursorColor = Color.Yellow;
                    break;
                case 4:
                    Display_GDI.CursorColor = Color.White;
                    break;
                case 5:
                    Display_GDI.CursorColor = Color.Black;
                    break;

            }
        }

        private void Panadapter_Window_Restore_button1_Click(object sender, EventArgs e)
        {
            
           //Window_controls.Panadapter_window_new.Size = new Size(800, 480);
        }

        public void Panadapter_Gain_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value;
            if (oCode.isLoading) return;
            if (!code_triggered)
            {
                value = Panadapter_Gain_hScrollBar1.Value;
                if (value == Panadapter_Controls.Spectrum_Gain) return;
                //MonitorTextBoxText(  "Panadapter Gain Called" );
                Panadapter_Controls.Spectrum_Gain = (6000 - value);
                oCode.SendCommand32(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, 
                    Panadapter_Controls.CMD_GET_SET_PANADAPTER_GAIN, value);
            }
        }

        public void Panadapter_Base_line_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value;
            if (oCode.isLoading) return;
            if (!code_triggered)
            {
                value = Panadapter_Base_line_hScrollBar1.Value;
                if (Panadapter_Controls.Spectrum_Base_Line == value) return;
#if !RPI
                Panadapter_Controls.Spectrum_Base_Line = (value * 10);
#else
                Panadapter_Controls.Spectrum_Base_Line = (value);
#endif
                oCode.SendCommand32(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, 
                    Panadapter_Controls.CMD_GET_SET_PANADAPTER_BASE, value);
            }
        }

        /*private void Wheel_checkBox1_CheckedChanged(object sender, EventArgs e)
        {
           
            if (!Panadapter_Controls.Freq_zoom)
            {
                Panadapter_Controls.Freq_zoom = true;
                Wheel_checkBox1.Text = "Wheel Freq";
            }
            else
            {
                Panadapter_Controls.Freq_zoom = false;
                Wheel_checkBox1.Text = "Wheel Zoom";
            }
        }*/

        private void Waterfall_Window_Restore_button1_Click(object sender, EventArgs e)
        {
            Window_controls.Waterfall_Controls.restore_size = true;
            //Waterfall_form.Restore_window();
        }

        /*private void Time_grid_button1_Click(object sender, EventArgs e)
        {
            byte[] buf = new byte[2];

            buf[0] = Master_Controls.Extended_Commands.CMD_SET_WATERFALL_GRID;
            if (oCode.isLoading) return;
            Window_controls.Waterfall_Controls.Time_grid++;
            if (Window_controls.Waterfall_Controls.Time_grid == 4)
            {
                Window_controls.Waterfall_Controls.Time_grid = 0;
            }
            Time_grid_button1.Text = Convert.ToString(Window_controls.Waterfall_Controls.Time_grid);
            buf[1] = Window_controls.Waterfall_Controls.Time_grid;
            oCode.SendCommand_MultiByte(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget,
                    Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
        }*/

        public void Color_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            byte[] buf = new byte[2];

            buf[0] = Master_Controls.Extended_Commands.CMD_SET_WATERFALL_PALET;
            Window_controls.Waterfall_Controls.pallet_index = Color_listBox1.SelectedIndex;
            switch (Window_controls.Waterfall_Controls.pallet_index)
            {
                case 0:
                    Display_GDI.ColorSheme = ColorSheme.original;
                    break;
                case 1:
                    Display_GDI.ColorSheme = ColorSheme.enhanced;
                    break;
                case 2:
                    Display_GDI.ColorSheme = ColorSheme.SPECTRAN;
                    break;
                case 3:
                    Display_GDI.ColorSheme = ColorSheme.BLACKWHITE;
                    break;
            }
            buf[1] = (byte)Window_controls.Waterfall_Controls.pallet_index;
            oCode.SendCommand_MultiByte(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget,
                    Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
        }

        public void Direction_checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            byte[] buf = new byte[2];

            buf[0] = Master_Controls.Extended_Commands.CMD_SET_WATERFALL_DIRECTION;
            if (Window_controls.Waterfall_Controls.direction_normal)
            {
                Display_GDI.ReverseWaterfall = true;
                Window_controls.Waterfall_Controls.direction_normal = false;
                Direction_checkBox1.Text = "Direction Reversed";
                buf[1] = 1;
            }
            else
            {
                Display_GDI.ReverseWaterfall = false;
                Window_controls.Waterfall_Controls.direction_normal = true;
                Direction_checkBox1.Text = "Direction Normal";
                buf[1] = 0;
            }
            oCode.SendCommand_MultiByte(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget,
                    Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
        }

        private void Waterfall_Gain_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int[] value = new int[1];
            value[0] = Waterfall_Gain_hScrollBar1.Value;
            if (value[0] != Window_controls.Waterfall_Controls.gain)
            {
                Window_controls.Waterfall_Controls.gain = value[0];
                byte[] intbytes = new byte[sizeof(int) + 1];
                intbytes[0] = Master_Controls.Extended_Commands.CMD_SET_WATERFALL_GAIN;
                Buffer.BlockCopy(value, 0, intbytes, 1, sizeof(int));
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(intbytes);
                byte[] result = intbytes;
                oCode.SendCommand_MultiByte(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget,
                        Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, result, result.Length);
                String Message = " Waterfall Gain: " + Convert.ToString(value[0] +
                    " Length: " + Convert.ToString(result.Length));
                Write_Debug_Message(Message);
                Display_GDI.WaterfallHighThreshold = 7000.0f - (float)value[0];
            }
        }

        private void Waterfall_zero_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int[] value = new int[1];
            //Write_Debug_Message(" Waterfall_zero_hScrollBar1. value length: " + value.Length);
            value[0] = Waterfall_zero_hScrollBar1.Value;
            if (value[0] != Window_controls.Waterfall_Controls.zero)
            {
                Window_controls.Waterfall_Controls.zero = Waterfall_zero_hScrollBar1.Value;
                byte[] intbytes = new byte[sizeof(int) + sizeof(byte)];
                intbytes[0] = Master_Controls.Extended_Commands.CMD_SET_WATERFALL_ZERO;
                Buffer.BlockCopy(value, 0, intbytes, 1, sizeof(int));
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(intbytes);
                byte[] result = intbytes;
                oCode.SendCommand_MultiByte(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget,
                        Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, result, result.Length);
            }
            String Message = " Waterfall zero: " + Convert.ToString(value[0]);
            Write_Debug_Message(Message);
        }

        private void Speed_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int[] value = new int[1];
            Write_Debug_Message(" Speed_hScrollBar1_Scroll. value length: " + value.Length);
            value[0] = Speed_hScrollBar1.Value;
            if (value[0] != Window_controls.Waterfall_Controls.window_speed)
            {
                Window_controls.Waterfall_Controls.window_speed = Speed_hScrollBar1.Value;
                byte[] intbytes = new byte[sizeof(int) + 1];
                intbytes[0] = Master_Controls.Extended_Commands.CMD_SET_WATERFALL_SPEED;
                Buffer.BlockCopy(value, 0, intbytes, 1, value.Length);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(intbytes);
                byte[] result = intbytes;
                oCode.SendCommand_MultiByte(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget,
                        Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, result, result.Length);
            }
        }

        private void Waterfall_groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void Time_Grid_button1_Click(object sender, EventArgs e)
        {
            byte[] buf = new byte[2];

            buf[0] = Master_Controls.Extended_Commands.CMD_SET_WATERFALL_GRID;
            if (oCode.isLoading) return;
            Settings.Default.Time_Grid++;
            if (Settings.Default.Time_Grid >= 4)
            {
                Settings.Default.Time_Grid = 0;
            }
            Time_Grid_button1.Text = Convert.ToString(Settings.Default.Time_Grid);
            buf[1] = (byte)Settings.Default.Time_Grid;
            oCode.SendCommand_MultiByte(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget,
                    Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
        }
    }
   
}

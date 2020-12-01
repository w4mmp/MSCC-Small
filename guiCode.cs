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
using System.Text;
using System.Reflection;
using System.Drawing;

using OmniaGUI;

//using FTD2XX_NET;

public static class Master_Controls
{
    public static int previous_second = 0;
    public static bool Shutdown = false;
    public static int Main_frequency = 0;
    public static int Main_volume = 0;
    public static int Previous_Main_frequency = 0;
    public static int Log_retry_count = 0;
    //public static bool sdrcore_running = true;
    public static bool Transmit_Mode = false;
    public static bool PPT_Mode = false;
    public static bool Tuning_Mode = false;
    public static bool Two_Tone_Mode = false;
    public const byte CMD_RPI_SET_TEMPERATURE = 0x12;
    public const byte CMD_SET_DISPLAY_FREQ = 0xBB;
    public const byte CMD_SET_TX_SET_BY_SERVER = 0xBC;
    public const byte CMD_GET_SET_FIRMWARE_VERSION = 0xB2;
    public const byte CMD_GET_SET_MSSDR_VERSION = 0xB3;
    public const byte CMD_GET_SET_SDRCORE_RECV_VERSION = 0xB5;
    public const byte CMD_GET_SET_SDRCORE_TRANS_VERSION = 0xBD;
    public const byte CMD_MODE_SET_BY_SERVER = 0xA8;
    public const byte CMD_SET_TWO_TONE = 0x88;
    public const byte CMD_SET_PA_BYPASS = 0xF7;
    public const byte CMD_GET_MIA_STATUS = 0xBE;
    public const byte CMD_GET_TRANSCEIVER_TEMP = 0xBF;
    public const byte CMD_SEND_GUI_STATUS = 0xFE;
    public const short NO_STARTUP_BAND = 345;
    public static int Vertical_scroll_size;
    public static bool Socket_Error = false;
    public static bool Keep_Alive = false;
    public static short Keep_Alive_Counter = 0;
    public static short Keep_Alive_Pulse = 0;
    //public static bool Initialized = false;
    public static short Startup_Band = NO_STARTUP_BAND;
    public static bool Step_Sent = false;
    public static bool Network_Receive_Busy = false;
    public static bool MSSDR_running = false;
    public static bool GUI_check_status = true;
    public static int Check_Server_Status_Count = 0;
    public const int Check_Server_Limit = 20;
    public static bool Initialize_network_status = false;
    public static bool Get_Network_file = false;
    public static Point Main_Smeter_Position;
    public static int Power_Meter_Max_Level = 110;
    public static bool FTP_File_Found = true;
    public static bool Main_Tab_Active = true;
    public static bool Post_Init = false;
   
    public static class Extended_Commands
    {
        public const byte CMD_SET_EXTENDED_COMMAND = 0x0B;
        //Waterfall Commands
        public const byte CMD_SET_WATERFALL_DISPLAY = 0x00;
        public const byte CMD_SET_WATERFALL_GAIN = 0x01;
        public const byte CMD_SET_WATERFALL_GRID = 0x02;
        public const byte CMD_SET_WATERFALL_ZERO = 0x03;
        public const byte CMD_SET_WATERFALL_SPEED = 0x04;
        public const byte CMD_SET_WATERFALL_DIRECTION = 0x05;
        public const byte CMD_SET_WATERFALL_PALET = 0x06;
        public const byte CMD_SET_WATERFALL_WHEEL_STATUS = 0x07;

        //Solidus Commands
        public const byte CMD_SET_ANTENNA_SWITCH = 0x08;
        public const byte CMD_SET_IQBD_MONITOR = 0x09;
        public const byte CMD_SET_IQBD_DATA = 0x0A;
        public const byte CMD_SET_FORWARD_POWER = 0x0B;
        public const byte CMD_SET_REVERSE_POWER = 0x0C;
        public const byte CMD_SET_SWR = 0x0D;
        public const byte CMD_SET_SOLIDUS_STATUS = 0x0E;
        //RPi Commands
        public const byte CMD_SET_REBOOT = 0X40;
        public const byte CMD_SET_SHUTDOWN = 0X41;

        //Docking Commands
        public const byte CMD_SET_DOCKED = 0x10;
        public const byte CMD_SET_AUTODOCKED = 0x11;

        //Calibration Commands
        public const byte CMD_CALIBRATION_CANCEL = 0x12;

        // MFC Server Commands
        public const byte CMD_MFC_AUTO_ZERO = 0x13;
        public const byte CMD_MFC_SET_ZERO = 0x14;
        public const byte CMD_MFC_SET_BAND = 0x15;
        public const byte CMD_MFC_SET_FAVS = 0x16;
        public const byte CMD_MFC_SET_STEP = 0x17;
        public const byte CMD_MFC_SET_TUNE = 0x18;
        public const byte CMD_MFC_SET_MODE = 0x19;
        public const byte CMD_MFC_SET_RIT_MODE = 0x1A;
        public const byte CMD_MFC_SET_CW_BW = 0x1B;
        public const byte CMD_MFC_SET_HI_BW = 0x1C;
        public const byte CMD_MFC_SET_RIT = 0x1D;
        public const byte CMD_SET_GUI_STAR = 0x1E;
        public const byte Knob_switch_star = 0x10;
        public const byte Button_A_switch_star = 0x20;
        public const byte Button_B_switch_star = 0x30;
        public const byte Button_C_switch_star = 0x40;

        public const byte CMD_SET_KNOB_SWITCH = 0x20;
        public const byte CMD_SET_LEFT_SWITCH = 0x21;
        public const byte CMD_SET_MIDDLE_SWITCH = 0x22;
        public const byte CMD_SET_RIGHT_SWITCH = 0x23;
        public const byte CMD_SET_PTT_SWITCH = 0x24;
        public const byte CMD_SET_METER_HOLD = 0x26;
        // Server Messages
        public const byte CMD_SET_SERVER_ERROR = 0x30;
        public const byte CMD_SET_SERVER_MSG = 0x31;
    }

    public static bool TX_Inhibited = true;
    public static bool code_triggered = false;
    public static bool Startup_Label_Toggle = false;
    public static short Startup_Label_Tick_count = 0;
    public static string productVersion = "120.11.24";
    public static bool Debug_Display = false;
    public static bool Two_Tone = false;
    public static bool QRP_Mode = true;
    public static bool QRP_Startup_Mode_Status = false;
    public static bool Transceiver_Warming = true;
    //public static bool Band_Change_Tune = false;
    public static bool Band_Change_Toggle = false;
    public static byte Band_Change_Timer = 0;
    public static bool Band_Change_Tune_Called = false;
    public static byte Band_Change_Delay = 0;
    public static int Power_Bar_Value = 0;

    public struct Firmware_Version
    {
        public static int Major;
        public static int Minor;
    }
    public struct SDRcore_trans_verison
    {
        public static int Major;
        public static int Minor;
    }
    public struct SDRcore_recv_verison
    {
        public static int Major;
        public static int Minor;
    }
    public struct mssdr_verison
    {
        public static int Major;
        public static int Minor;
    }
    public struct Minimum_Firmware
    {
        public static readonly int Major = 119;
        public static readonly int Minor = 20;
    }
    public static TabPage Current_tab;
}

public static class VFO_Controls
{
    public static bool VFO_A = true;
}
public static class Solidus_Controls
{
    public static bool Solidus_Status = false;
    public static bool Solidus_Status_Set = false;
    public static bool Mia_Status = false;
    public static bool Mia_Status_Set = false;
}
public static class RPi_Settings
{
    
    public static int Peak_Needle = 0;
    public static int Peak_Needle_Delay_Index = 0;
    public static int Peak_Needle_Color_Index = 0;
    public static int Time_Display = 0;
    public static int Meter_Mode = 0;
    //public static bool RPi_Needs_Updated = false;
    public static class Volume_Settings{
        public static int Volume_ATTN_Index = 0;
        public static int Previous_Speaker_Volume = 0;
        public static int Speaker_Volume = 0;
        public static int Speaker_Mute = 0;
        public static int Mic_Volume = 0;
        public static int Mic_Mute = 0;
        public static int Mic_Pre_Gain = 0;
        public static int Mic_Mode = 0;
    }
    public static class Controls
    {
        public static int Previous_Freq_Step = 0;
        public static int Freq_Step = 0;
        public static short Freq_Digit = 30;
        public static short Antenna_Switch = 0;

    }
}

public static class Freq_Digits
{
    public static Int32 meg10 = 0;
    public static Int32 meg = 0;
    public static Int32 hundred_thousand = 0;
    public static Int32 ten_thousand = 0;
    public static Int32 thousand = 0;
    public static Int32 hundred = 0;
    public static Int32 ten = 0;
    public static Int32 one = 0;
    public static Int32 previous_frequency = 0;
}

public static class Tuning_Knob_Controls
{
    public static int VERSION = 1;
    public static bool MFC_Initialized = false;
    public static bool Continue = true;
    //public static FTDI Device = new FTDI();
    //public static FTDI.FT_STATUS Status;
    public static bool encoder_A = false;
    public static bool encoder_B = false;
    public static UInt16 store = 0;
    public static byte prevNextCode = 0;
    public static sbyte[] rec_enc_table = { 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0 };
    //public static sbyte[] switch_table = { 20, 20, 20, 20 };
    public static sbyte[] previous_switch_table = { 0, 0, 0, 0, 0 };
    public const sbyte KNOB_SWITCH = 0;
    public const sbyte LEFT_SWITCH = 1;
    public const sbyte MIDDLE_SWITCH = 2;
    public const sbyte RIGHT_SWITCH = 3;
    public const sbyte PTT_SWITCH = 4;

    public static bool Open = false;
    public static byte[] sentBytes = new byte[2];
    public static bool On = false;
    public static uint Received_Bytes = 0;
    public static bool Write_failure = false;
    public static bool Read_failure = false;
    public static byte Previous_relays_States = 0;
    public static int loop_count = 0;
    public static int previous_loop_count = 0;
    public static int loop_count_limit = 20;
    public static int Freq = 0;
    //public static int Volume = 0;

    //public static bool MFC_volume_active = false;
    public static byte Set_Band_loop_count = 0;
    public static byte Switches_Control = 0;
    public const byte CMD_SET_TRANSCEIVER_DISPLAY = 0xCC;
    public const byte CMD_SET_STAR = 0xCD;
    public const byte CMD_SET_STEP_VALUE = 0xCE;

    public static byte Knob_switch_function = 0;
    public static byte Knob_switch = 0x80;

    public static byte Button_left_function = 0;
    public const byte Button_left_switch = 0x10;

    public static byte Button_middle_function = 0;
    public const byte Button_middle_switch = 0x08;

    public static byte Button_right_function = 0;
    public const byte Button_right_switch = 0x04;

    public static byte PTT_switch_function = 0;
    public const byte PTT_switch = 0x01;
    public static class Button_Text
    {
        public static string Button_left_text;
        public static string Button_middle_text;
        public static string Button_right_text;
        public static string Knob_text;
    }
    public static class Freq_Queue
    {
        public const int MAX_COMMAND_QUEUE = 1000;
        public const int QUEUE_FAILED = 2000;
        public const byte QUEUE_SUCCESS = 1;
        public const byte QUEUE_BUSY = 2;
        public const byte QUEUE_CHECK_DELAY = 1;
        public const byte QUEUE_PROCESS_DELAY = 25;
        public const byte DISPLAY_COUNT = 2;
        public static int[] E_freq_queue = new int[MAX_COMMAND_QUEUE];
        public static int E_freq_queue_front = -1;
        public static int E_freq_queue_rear = -1;
        public static int E_queue_count = 0;
        public static bool E_queue_busy = false;
    }
    public static class Star_Position
    {
        public static byte Knob = 0;
        public static byte Left = 0;
        public static byte Middle = 0;
        public static byte Right = 0;
    }
    public static class Button_Function_List
    {
        public const byte NONE = 0;
        public const byte STEP = 1;
        public const byte PTT = 2;
        public const byte TUNE = 3;
        public const byte MODE = 4;
        public const byte RIT = 5;
        public const byte BAND = 6;
        public const byte FREQ_VOLUME = 7;
        public const byte CW_BW = 8;
        public const byte HI_CUT = 9;
        public const byte RIT_OFFSET = 10;
        public const byte FAVORITES = 11;
        public const byte PTT_REAR_CONNECTOR = 12;
        //public const byte VOLUME = 12;
    }
    public static class Active_Functions
    {
        public static bool Freq_Active = false;
        public static bool RIT_Offset_Active = false;
        public static bool Volume_Active = false;
    }
    public static class RIT_OFFSET_Function
    {
        public static bool Switch_Toggle = false;
        public static bool Action_Toggle = false;
        public static bool RIT_Active = false;
        public static bool RIT_Slider_Active = false;
        public static int RIT_Offset_Value = 0;
        public static int RIT_Main_Offset_Value = 0;
        public static int RIT_Max_Offset_Value = 0;
        public static int RIT_Min_Offset_Value = 0;
        public static int RIT_Step = 0;
    }
    public static class Freq_Function
    {
        public static bool Switch_Toggle = false;
        public static bool Action_Toggle = false;
        public static bool Active = false;
        public static bool First_Pass = true;
    }
    public static class Favs_Function
    {
        public static bool Switch_Toggle = false;
        public static bool Action_Toggle = false;
    }
    public static class Step_Function
    {
        public static bool Switch_Toggle = false;
        public static int Step_increment = 0;
        public static int Multiplier = 0;
    }
    public static class CW_BW_Function
    {
        public static bool Switch_Toggle = false;
        public static int BW_index = 0;
    }
    public static class CW_HI_CUT_Function
    {
        public static bool Switch_Toggle = false;
        public static int HI_CUT_index = 0;
    }
    public static class Band_Function
    {
        public static bool Switch_Toggle = false;
    }
    public static class Tune_Function
    {
        public static bool Switch_Toggle = false;
        public static bool Action_Toggle = false;
    }
    public static class PTT_Function
    {
        public static bool Switch_Toggle = false;
        public static bool Action_Toggle = false;
    }
    public static class PTT_Function_Rear
    {
        public static bool Switch_Toggle = false;
        public static bool Action_Toggle = false;
    }
    public static class RIT_Function
    {
        public static bool Switch_Toggle = false;
        public static bool Action_Toggle = false;
    }
    public static class Mode_Function
    {
        public static bool Switch_Toggle = false;
    }
    public static class Volume_Function
    {
        public static bool Switch_Toggle = false;
        public static bool Action_Toggle = true;
        public static bool Volume_Slider_Active = false;
        public static int Volume = 0;
        public static bool Volume_active = false;
        public static int Max_volume = 0;
    }
}

public static class Relay_Board_Controls
{
    //public static FTDI Device = new FTDI();
    //public static FTDI.FT_STATUS Status;
    public static bool Open = false;
    public static byte[] sentBytes = new byte[2];
    public static bool On = false;
    public static byte Previous_Relay = 0;
    public static uint Received_Bytes = 0;
    public static bool Write_failure = false;
}

public static class NB_Controls
{
    public static bool NB_Button_On = false;
    public static bool NB_Main_Button_On = false;
    public static int NB_Pulse_Width = 0;
    public static int NB_Threshold = 0;
    public const byte NB_ENABLE = 0x80;
    public const byte NB_PULSE_WIDTH = 0x81;
    public const byte NB_THRESHOLD = 0x82;
}

public static class NR_Controls
{
    public static bool NR_Button_On = false;
    public static int NR_Value = 0;
}

public static class Favorites_Controls
{
    public const byte CMD_SET_UPDATE_BAND_STACK = 0xC1;
    public const byte CMD_GET_BAND_STACK = 0xC2;
    public const byte CMD_SET_BAND_STACK = 0xC3;
    public const byte CMD_GET_BAND_STACK_MODE = 0xC4;
    public const byte CMD_GET_BAND_STACK_FREQ = 0xC5;
    public const byte CMD_SET_BAND_STACK_INDEX = 0xC6;
    public const byte CMD_GET_BAND_STACK_BAND = 0xC7;
    public const byte CMD_SET_BAND_STACK_NAME = 0xC8;
    public static bool Name_Entered = false;
    public static bool Sorted = false;
    public static class Count
    {
        public static int B10;
        public static int B12;
        public static int B15;
        public static int B17;
        public static int B20;
        public static int B30;
        public static int B40;
        public static int B60;
        public static int B80;
        public static int B160;
    }
    public static int B10_Count = 0;
    public static int B12_Count = 0;
    public static int B15_Count = 0;
    public static int B17_Count = 0;
    public static int B20_Count = 0;
    public static int B30_Count = 0;
    public static int B40_Count = 0;
    public static int B60_Count = 0;
    public static int B80_Count = 0;
    public static int B160_Count = 0;
    public static int Index_Count = 0;
    public static int Band_Count_Limit = 0;
    public static int Current_Band = 150;
}

public static class Rit_Controls
{
    public static int Offset = 0;
    public static int Rit_Freq_Plus_Offset = 0;
    public static int Rit_Freq;
    public static bool Rit_On = false;
    public const byte CMD_SET_RIT_FREQ = 0x89;
    public const byte CMD_SET_RIT_STATUS = 0x8A;
}

public static class Panadapter_Controls
{
    public struct Buffer_0
    {
        public static UInt16[] X_value = new UInt16[800];
        public static UInt16[] Y_value = new UInt16[400];
    }
    public struct Buffer_1
    {
        public static UInt16[] X_value = new UInt16[800];
        public static UInt16[] Y_value = new UInt16[400];
    }
    public struct Display_Buffer
    {
        public static UInt16[] X_value = new UInt16[800];
        public static UInt16[] Y_value = new UInt16[800];
    }
    public struct Filter_Markers
    {
        public static int MARKER_OUT_OF_RANGE = 1000;
        public static int DISPLAY_BANDWIDTH = 72000;
        public static int STARTUP_MAX_PIXELS = 800;
        public static int Window_Size = 0;
        public static int Display_Center = 399;
        public static int G_band_marker_low = 1;
        public static int G_band_marker_high = 2;
        public static int G_band_center = 0;
        public static int CW_Offset = 0;
    }

    public struct Panadapter_Colors
    {
        public static System.Drawing.Color Line_Color;
        public static System.Drawing.Color Previous_Line_Color = System.Drawing.Color.Azure;
        public static System.Drawing.Color Fill_Color = System.Drawing.Color.Azure;
        public static System.Drawing.Color Previous_Fill_Color = System.Drawing.Color.Azure;
        public static System.Drawing.Color Background_Color = System.Drawing.Color.Azure;
        public static System.Drawing.Color Previous_Background_Color = System.Drawing.Color.Azure;
        public static System.Drawing.Color Marker_Color = System.Drawing.Color.Azure;
        public static System.Drawing.Color Previous_Marker_Color = System.Drawing.Color.Azure;
        public static System.Drawing.Color Previous_Cursor_Color = System.Drawing.Color.Azure;
        public static System.Drawing.Color Cursor_Color = System.Drawing.Color.Azure;
    }
    public static int X_Scroll_Bar_value = 0;
    public static int Max_X = 400;
    public static char Previous_mode = 'B';
    public static bool Sequence_0_Complete = false;
    public static bool Sequence_1_Complete = true;
    public static bool Display_Operation_Complete = true;
    public static bool Display_buffer_ready = false;
    public static int Frequency = 0;
    public static int Display_freq;
    //public static int Updated_Frequency = 0;
    public static bool Freq_Set_By_Master = true;
    public static IPEndPoint txtarget;
    public static Socket txsocket;
    public static bool TX_Panadapter = false;
    public static bool Display_Panadatper = true;
    public static int Spectrum_Base_Line = 0;
    public static int Spectrum_Gain = 6000;
    public static int Drift = 0;
    public const byte CMD_GET_SET_PANADAPTER = 0xD5;
    public const byte CMD_GET_SET_PANADAPTER_SMOOTHING = 0xDA;
    public const byte CMD_GET_SET_PANADAPTER_STATUS = 0x83;
    public const byte CMD_GET_SET_PANADAPTER_FILL = 0x59;
    public const byte CMD_GET_SET_PANADAPTER_LINE = 0x5A;
    public const byte CMD_GET_SET_PANADAPTER_MARKER = 0x5B;
    public const byte CMD_GET_SET_PANADAPTER_BACKGROUND = 0x5D;
    public const byte CMD_GET_SET_PANADAPTER_CURSOR = 0x5E;
    public const byte CMD_GET_SET_PANADAPTER_REFRESH = 0x5F;
    public const byte CMD_GET_SET_PANADAPTER_GAIN = 0x85;
    public const byte CMD_GET_SET_PANADAPTER_BASE = 0x86;
    public const byte CMD_CW_SNAP_START = 0x6C;
    public const byte CMD_CW_SNAP_FINISHED = 0x6D;
    public const byte CMD_SET_CW_SNAP_FREQ = 0x67;
    public const byte CMD_SET_AUTO_SNAP_STATUS = 0xC9;
    public const byte CMD_SET_AUTO_SNAP_INDEX = 0xCA;
    public const byte CMD_SET_DRIFT = 0x35;

    public static byte Smoothing_Index = 0;
    public static byte Refresh_Index = 0;
    public static string Filter_Size_Name;
    public static string CW_Filter_Size_Name;
    public static int Filter_Index = 0;
    public static int Previous_Filter_Index = 0;
    public static double X_Position = 0;
    public static double Graph_Freq = 0;
    public static int Fine_Tune_Delta = 0;
    public static bool Fine_Tuning = false;
    public static bool view_grid = false;
    public static bool Reset_Freq = false;
    public static bool Anti_Alias = false;
    public static bool Previous_Anti_Alias = true;
    public static long Data_average = 0;
    public const int Data_recieve_limit = 50;
    public static bool Freq_zoom = false;
    public static bool Previous_Freq_zoom = false;
    
    public struct CW_Snap
    {
        public static bool CW_button = false;
        public static bool CW_check_box = false;
        public static bool CW_snap_status = false;
        public static System.Drawing.Color Button_Color = System.Drawing.Color.Black;
    }
    public struct Mouse_event
    {
        public static double x = 0;
        public static double y = 0;
        public static String Display_Freq;
        public static double User_Cursor_x = 0;
        public static double User_Cursor_y = 0;
        public static bool Mouse_Event = false;
    }
    public struct Freq_Bounds
    {
        public static int Low_Freq;
        public static int High_Freq;
        public static int Current_X_Min = 0;
        public static int Current_X_Max = 0;
    }
    public struct Auto_Snap
    {
        public static int Snap_Value = 0;
        public static bool Auto_Snap_Checkbox = false;
    }
}

public static class Power_Calibration_Controls
{
    public static int New_Power_Value = 0;
    public static int Old_Power_Value = 0;
    public static int Band = 0;
    public static int Previous_Band = 0;
    public static bool Band_Is_Selected = false;
    public static bool Tuning_Mode = false;
    public static bool Tune_Toggle = false;
    public static bool Warning_Accepted = false;
    public static bool Applied_Button_Active = false;
    public static bool Power_Tab_Active = false;
    public const byte CMD_CALIBRATION_MASTER_RESET = 0xAB;
    public const byte CMD_CALIBRATION_TUNE = 0xAC;
    public const byte CMD_SET_COMMIT_POWER_VALUES = 0xAD;
    public static bool Update_Pending = false;
    public static bool Committing = false;
    public static bool Factory_Defaults = false;
    public static bool QRP_Mode = false;
    public static bool Transceiver_mode = false;
}

public static class Amplifier_Power_Controls
{
    public static int New_Power_Value = 0;
    public static int Band = 0;
    public static bool Band_Is_Selected = false;
    public static bool Tuning_Mode = false;
    public static bool Applied_Button_Active = false;
    public static bool Tab_Active = false;
    public static bool Solidus_Band_Selected = false;
    public const byte CMD_GET_AMP_POWER = 0x05;
    public const byte CMD_GET_POTENTIA_TEMPERATURE = 0x06;
    public const byte CMD_GET_POTENTIA_BIAS = 0x07;
    public const byte CMD_SET_POTENTIA_CALIBRATION = 0x08;
    public const byte CMD_SET_SET_WIPER = 0x09;
    public const byte CMD_SET_AMPLIFIER_INITIALIZE = 0xF9;
    public const byte CMD_SET_AMPLIFIER_POWER = 0xFA;
    public const byte CMD_GET_AMPLIFIER_POWER = 0xFB;
    public static bool Factory_Defaults = false;
    public static int Previous_Tune_power = 0;
    public static bool Display_Wait = false;
    public static short Display_Wait_count = 0;
    public static short Display_Wait_count_limit = 5;
    public static bool Tune_Inhibit = false;
    public static bool Bias_On = false;
    public static int Calibration_Value = 0;
    public static bool Overtemp_Flag = false;
    public static bool Bias_ON_OFF = false;
    public const byte BIAS_OPERATION_FAILED = 0;
    public const byte BIAS_OPERATION_SUCCESS = 1;
    public const byte BIAS_OPERATION_UNCALIBRATED = 2;
    public const byte BIAS_OPERATION_CALIBRATED = 3;
    public const byte BIAS_OPERATION_WARMING = 4;
    public const byte BIAS_OPERATION_COOLING = 5;
    public const byte BIAS_OPERATION_CALIBRATING = 6;
    //public const byte WIPER_0 = 0x00;
    //public const byte WIPER_1 = 0x01;
}

public static class Frequency_Calibration_controls
{
    public const byte CMD_SET_CALIBRATION_START = 0x60;
    public const byte CMD_SET_CALIBRATION_DATA = 0x61;
    public const byte CMD_SET_CALIBRATION_FINISHED = 0x62;
    public const byte CMD_SET_CAL_LOOSE = 0x63;
    public const byte CMD_SET_CAL_MID_VALUE = 0x64;
    public const byte CMD_SET_CAL_HIGH_VALUE = 0x65;
    public const byte CMD_SET_CAL_DATA_PROCESSED = 0x66;
    public const byte CMD_SET_CAL_RESET = 0x68;
    public const byte CMD_SET_CAL_FINE = 0x69;
    public const byte CMD_SET_CALIBRATIION_PROGRESS = 0x6A;
    public const byte CMD_GET_SET_CAL_FREQ_DELTA = 0x6B;
    public const byte CMD_SET_FREQ_CAL_CHECK = 0x8C;
    public const byte CMD_START_CALIBRATE = 0xA7;
    public const byte CMD_SET_STANDARD_CARRIER = 0xAF;
    public static bool standard_carrier_selected = false;
    public static bool si5351_reset = false;
    public static bool freq_text_box_cleared = false;
    public static short standard_carrier = 0;
    public static bool Course_Resolution_Set = false;
    public static bool Fine_Resolution_Set = false;
    public static bool Calibration_In_Progress = false;
    public static Int32 Calibration_frequency = 0;
    public static bool Check_only = false;
    public static short Frequency_Drift = 0;
    public static bool Auto_Zero = true;
    public static bool Display_Wait = false;
    public static short Display_Wait_count = 0;
    public static bool Calibration_Checked = true;
    public static bool Freq_Cal_Loose = false;
    public static bool Reset = false;
}

public static class Comm_Port_Controls
{
    public const int Value_Not_Set = 200;
    public struct Box_indexes
    {
        public static int Baud_Rate = Value_Not_Set;
        public static int Parity = Value_Not_Set;
        public static int Stop_Bits = Value_Not_Set;
        public static int Data_Bits = Value_Not_Set;
        public static int Comm_Name_Index = Value_Not_Set;
        public static int Previous_Index = 0;
        public static int Comm_Port_Pins = Value_Not_Set;
        public static bool Set_By_Server = false;
    }
    public static bool Comm_Port_Open = false;
    public static bool Button_Toggle = false;
    public static byte DCD_Selected = 0;
    public static byte CTS_Selected = 0;

    public struct HR50_Controls
    {
        public static bool Comm_Port_Open = false;
        public static int Comm_Name_Index = 200;
        public static bool Button_Toggle = false;
        public static bool Set_By_Server = false;
        public static bool Pass_Thru = false;
        public static short PTT_Mode = 1;
        public static bool Comm_Port_Selected = false;
        public static bool Comm_Port_Open_by_server = false;
    }

    public const byte CMD_GET_SET_COMM_PORT = 0x40;
    public const byte CMD_GET_SET_COMM_BAUD_RATE = 0x41;
    public const byte CMD_GET_SET_COMM_PARITY = 0x42;
    public const byte CMD_GET_SET_COMM_DATA_BITS = 0x43;
    public const byte CMD_GET_SET_COMM_STOP_BITS = 0x44;
    public const byte CMD_GET_SET_COMM_START = 0x45;
    public const byte CMD_GET_SET_COMM_NAME_INDEX = 0x46;
    public const byte CMD_DELETE_COMM_PORT_INIT = 0x47;
    public const byte CMD_GET_SET_COMM_PORT_PINS = 0x48;
    public const byte CMD_GET_SET_HR50_COMM_NAME_INDEX = 0x49;
    public const byte CMD_GET_SET_HR50_COMM_PORT = 0x4A;
    public const byte CMD_GET_SET_HR50_COMM_SEND_BAND_INFO = 0x4B;
    public const byte CMD_GET_SET_HR50_COMM_START = 0x4C;
    public const byte CMD_GET_SET_HR50_COMM_SEND_FREQ_INFO = 0x4D;
    public const byte CMD_GET_SET_HR50_COMM_PASS_THRU = 0x4E;
}

public static class AGC_ALC_Notch_Controls
{
    public static bool Notch_Button_On = false;
    public static int AGC_Level = 0;
    public static int AGC_SLOW = 0;
    public static int AGC_MED = 1;
    public static int AGC_FAST = 2;
    public static int AGC_Release = 1;
    public const byte CMD_GET_SET_AGC = 0x87;
    public const byte CMD_GET_SET_AUTO_NOTCH = 0x8E;
    public const byte CMD_SET_AGC_FAST_LEVEL = 0xCB;
    public const byte CMD_SET_ALC = 0x4F;
}

public static class IQ_Controls
{
    public static int IQ_Offset = 0;
    public static int IQ_Offset_Total = 0;
    public static int Previous_IQ_Offset = 0;
    public static short B160 = 9;
    public static short B80 = 8;
    public static short B60 = 7;
    public static short B40 = 6;
    public static short B30 = 5;
    public static short B20 = 4;
    public static short B17 = 3;
    public static short B15 = 2;
    public static short B12 = 1;
    public static short B10 = 0;
    public static bool band_selected = false;
    public static bool Tuning_Mode = false;
    public static short Previous_band = 200;
    public static bool Tab_Active = false;
    public static bool IQ_TX_MODE_ACTIVE = false;
    public static bool IQ_RX_MODE_ACTIVE = false;
    public static bool IQ_Calibrating = false;
    public static byte Multiplier = 0;
    public static bool RX_toggle = false;
    public static bool TX_toggle = false;
    public static bool Called_From_RX_Button = false;
    public static short Current_TX_Band = 150;
    public static short Current_RX_Band = 150;
    public static short Previous_RX_Band = 150;
    public static short Tune_power = 0;
    public static bool Up_24KHz = false;
    public static int IQ_RX_Freq = 0;
    public static int IQ_RX_Up_24K_freq = 24000;
    public static int IQ_RX_Offset = 0;
    public static int[] iq_calibration_freqs = { 28010000, 24900000, 21010000, 18110000,
        14010000, 10110000, 7010000, 5330500, 3510000, 1810000 };
    public const byte CMD_SET_IQ_OFFSET = 0x52;
    public const byte IQ_CALIBRATION_TUNE = 0x54;
    public const byte IQ_CALIBRATION_RX_TX = 0x55;
    public const byte IQ_OPERATION_COMPLETE = 0x56;
    public const byte CMD_GET_IQ_VALUE = 0x8B;
    public const byte CMD_SET_COMMIT_IQ = 0x57;
    public const byte CMD_SET_IQ_BAND = 0x58;
    public const byte CMD_SET_IQ_DEFAULTS = 0x8D;
    public static short IQ_INVALID_BAND = 150;
    public static bool IQBD_MONITOR = false;
    public static bool Previous_QRP_Mode = false;
    public static class RPi
    {
        public static int Calibration_Slider_Value = 0;
        public static int Previous_Calibration_Slider_Value = 0;
    }
}

public static class Band_Stack_Controls
{
    public static int Frequency = 0;
    public static short Band = 0;
    public static char Mode = 'N';
    public static short Band_Stack_Complete = 0;
    public struct band_stack
    {
        public static int freq;
        public static char mode;
        public static short band;
    }
    
}

public static class SDRprocesses
{
    public static Process sdrcore_recv;
    public static Process sdrcore_trans;
    public static Process ms_sdr;
    public static Process initializer;
    public static bool ms_sdr_running = true;
    public static bool sdrcore_recv_running = true;
    public static bool sdrcore_trans_running = true;

}

public static class Audio_Device_Controls
{
    public static short Selected_Speaker = 0;
    public static short Default_Speaker = 0;
    public static short Selected_Mic = 0;
    public static short Default_Mic = 0;
    public const byte CMD_DELETE_SDRCORE_INIT = 0xEC;
    public const byte CMD_SET_MICROPHONE_STATUS = 0x8F;
    public static bool Device_Reset_Button_Active = false;
    public static bool Update_Microphone_Status = false;
    public static bool Update_Speaker_Status = false;
    public static bool Overdriven_Alert = false;
    public static bool Microphone_Status = false;
}

public static class Window_controls
{
    public struct Panadapter_New
    {
        public static bool form_displosed = false;
        public static int form_state = 0;
        public static bool display_form = true;
        public static bool window_created = false;
        public static bool window_displayed = false;
        public static bool window_visable = false;
        public static bool window_minimized = false;
        //public static bool spectrum_window_hidden = false;
        public static bool window_minimized_by_user = false;
        public static bool window_normallized_by_user = false;
        public static bool button_clicked = false;
        public static bool first_pass = true;

    }
    public struct Docking_Controls
    {
        //public static bool Docked = false;
        public static bool Previous_Docked = false;
        public struct Last_Sized
        {
            public static byte Order = 0;
            public const byte None = 0;
            public const byte Spectrum = 1;
            public const byte Waterfall = 2;
            public struct Window_Size
            {
                public static Size Spectrum = new Size(0,0);
                public static Size Previous_Spectrum = new Size(0, 0);
                public static Size Waterfall = new Size(0,0);
                public static Size Previous_Waterfall = new Size(0, 0);
            }
        }
        public static Size Main_docked_size;
        public static Point Main_location;
        public static Point Previous_Main_location = new Point(0, 0);
        public static Size Spectrum_docked_size;
        public static Point Spectrum_location;
        public static Size Waterfall_docked_size;
        public static Point Waterfall_location;
    }
    public struct Waterfall_Controls
    {
        public struct CW_Snap
        {
            public static bool CW_button = false;
            public static bool CW_check_box = false;
            public static bool CW_snap_status = false;
            public static System.Drawing.Color Button_Color = System.Drawing.Color.Black;
            public static bool button_clicked = false;
        }

        public struct Set_by_server
        {
            public static bool grid = false;
            public static bool direction = false;
            public static bool gain = false;
            public static bool zero = false;
            public static bool speed = false;
            public static bool pallet = false;
        }

        public struct Markers
        {
            public static int MARKER_OUT_OF_RANGE = 1000;
            public const int DISPLAY_BANDWIDTH = 72000;
            public static int STARTUP_MAX_PIXELS = 800;
            public static int Window_Size = 0;
            public static int Display_Center = 399;
            public static int band_marker_low = 1;
            public static int band_marker_high = 2;
            public static int band_center = 0;
            public static int CW_Offset = 0;
            public static Color cursor_color = Color.White;
            public static Pen cursor_pen;
            public static int Waterfall_Marker_Color_index = 0;
        }
        public static bool form_displosed = false;
        public static Size new_size;
        public static int form_state = 0;
        public static bool display_form = true;
        public static bool window_created = false;
        public static bool window_displayed = false;
        public static bool window_visable = false;
        public static bool window_minimized = false;
        public static bool window_minimized_by_user = false;
        public static bool window_normallized_by_user = false;
        public static bool button_clicked = false;
        public static bool first_pass = true;
        //public static PictureBox Waterfall_picture_box;
        public static Graphics Pic_Waterfall_graphics;
        public static Graphics Waterfall_graphics;
        public static bool Start_toggle = false;
        public static bool Display_Operation_Complete = false;
        public static bool Waterfall_thread_running = false;
        public static bool direction_normal = true;
        public static int seconds = 0;
        public static byte Time_grid = 0;
        public static int window_speed = 1;
        public static int zero = 410;
        public static int gain = 3090;
        public static int picture_box_width = 764;
        public const int Window_delta_width = 36;
        public const int Window_delta_height =67;
        public static int Display_width_delta = 20;
        public static bool Wheel_zoom_status = false;
        public static int Wheel_value = 0;
        public static bool Zoom_process = false;
        public static int pallet_index = 0;
        public static bool restore_size = false;
        public struct Display_Buffer
        {
            public static UInt16[] X_value = new UInt16[800];
            public static UInt16[] Y_value = new UInt16[800];
        }
    }
    public struct Panadapter_Controls
    {
        public static bool panadapter_form_displosed = false;
        public static int panadapter_form_state = 0;
        public static bool display_panadapter_form = true;
        public static bool panadapter_window_created = false;
        public static bool panadapter_window_displayed = false;
        public static panadapter_control Panadapter_Control_Window;
    }
        
    public static bool freq_form_displosed = false;
    public static int freq_form_state = 0;
    public static bool display_freq_form = true;
    public static bool monitor_form_displosed = false;
    public static int monitor_form_state = 0;
    public static bool display_monitor_form = true;
    public static bool smeter_created = false;
    public static bool smeter_first_pass = true;
    public static bool smeter_form_displosed = false;
    public static int smeter_form_state = 0;
    public static bool display_smeter_form = true;
    public static bool smeter_displayed = false;
    public static bool smeter_display_visable = false;
    public static bool smeter_minimized = false;
    public static bool smeter_window_minimized_by_user = false;
    public static bool smeter_window_normallized_by_user = false;
    public static bool smeter_button_clicked = false;
    public static bool Panadapter_Toggle = false;
    public static bool Smeter_Toggle = false;
    public static bool Waterfall_Toggle = false;
    public static bool Panadapter_On_By_Server = false;
    public static bool Smeter_On_By_Server = false;
    public static bool Waterfall_On_By_Server = false;
    public const byte CMD_SET_PANADAPTER_DISPLAY = 0xDF;
    public const byte CMD_SET_SMETER_DISPLAY = 0xCF;
}

public static class Volume_Controls
{
    //public static short Speaker_Volume = 50;
    public static short Speaker_SSB_Value = 0;
    public static short Speaker_CW_Value = 0;
    public static short Speaker_TUNE_Value = 0;
    public static short Speaker_AM_Value = 0;
    //public static bool Speaker_MutED = false;
    public static short Mic_Volume = 50;
    public static bool Mic_MutED = false;
    public const byte CMD_SET_MIC_GAIN = 0xE0;
    public const byte CMD_SET_VOLUME_ATTN = 0xE1;
    public const byte CMD_SET_MIC_VOLUME = 0xE6;
    public const byte CMD_SET_SPEAKER_MUTE = 0xE7;
    public const byte CMD_SET_SPEAKER_VOLUME = 0xE5;
    public const byte CMD_SET_MIC_MUTE = 0xE8;
    public const byte CMD_SET_OVERDRIVEN = 0xED;
    public const byte CMD_SET_COMPRESSION_STATE = 0xEE;
    public const byte CMD_SET_COMPRESSION_LEVEL = 0xEF;

    public static bool Overdriven_Warning = false;
    public static bool Overdriven_Warning_Toggle = true;
    public static short Compression_State = 0;
    public static short Compresson_Value = 0;
    public static short Previous_Compression_Value = 0;
    public static bool Label_Normal = true;
    public static bool Voice_Digital_Toggle = false;
    public static byte Volume_Slider_Mode = 0;
    public static byte Previous_Slider_Mode = 0;
    public const byte SLIDER_AM_MODE = 0;
    public const byte SLIDER_SSB_MODE = 1;
    public const byte SLIDER_CW_MODE = 2;
    public const byte SLIDER_TUNE_MODE = 3;
}

public static class Power_Controls
{
    public static short Main_Power = 0;
    public static short CW_Power = 0;
    public static short AM_Power = 0;
    public static short TUNE_Power = 50;
    public static short Main_Tab_Power = 0;
    public static bool Tx_Warning = false;
    public static bool Full_power = false;
    public static int Previous_TX_Bandwidth = 0;
    public const byte CMD_SET_MAIN_POWER = 0xE2;
    public const byte CMD_SET_AM_POWER = 0xE3;
    public const byte CMD_SET_TUNE_POWER = 0xE9;
    public const byte CMD_SET_CW_POWER = 0xE4;
    public static int Previous_Tune_power = 0;
}

public static class Mode
{
    public const short AM_mode = 0;
    public const short LSB_mode = 1;
    public const short USB_mode = 2;
    public const short CW_mode = 3;
    public const short TUNE_mode = 4;
}

public static class Smeter_controls
{
    public static class Smeter
    {
        public static String[] smeter_dial = new String[9];
        public static String[] vu_dial = new string[10];
        public static String[] swr_dial = new string[8];
        public static String[] power_dial_QRP = new string[7];
        public static String[] power_dial_QRO = new string[7];
    }
    public static int smeter_hold_time = 30;
    public static int smeter_value = 250;
    public static bool Smeter_Hold_On = false;
    public static int Previous_Low = 0;
    public static int Previous_High = 0;
    public static int Smeter_decrement = 1;
    public static int Current_Mode = 10;
    public static int smeter_display_value = 0;
    public static int Power_Value = 0;
    public const byte S_METER_MODE = 3;
    public const byte ALC_MODE = 4;
    public const byte SWR_MODE = 5;
    public const byte POWER_MODE = 6;
    public static float Smeter_scaling = 1.0f;
    public static float Smeter_base = 2.6f;
    public const byte CMD_GET_SET_SMETER = 0xD4;
    public static bool Smeter_convert = false;
    public static float SWR_Value = 0;
    public struct VU
    {
        public static int Previous_Low = 0;
    }
}

public static class Main_Smeter_controls
{
    public static int Previous_meter_value = 0;
    public static int SMeter_value = 0;
    public static int Meter_mode = 0;
    public static int VU_Meter_value = 0;
    public static int Previous_Low = 0;
    public static int Smeter_decrement = 1;
    public static bool Smeter_Hold_On = true;
    public static int Display_mode = 3;
    public static byte S_METER_MODE = 3;
    public static byte VU_MODE = 4;
}

public static class Udp_data
{
    public static UdpClient rxsocket;
    public static IPEndPoint rxtarget;
}

public static class Filter_control
{
    public static short Filter_High_Index = 1;
    public static short Filter_Low_Index = 4;
    public static short Previous_Filter_High_Index = 0;
    public static short Previous_Filter_Low_Index = 0;
    public static short Filter_TX_High_Index = 0;
    public static short Default_Filter_High_Index = 1;
    public static short Default_Filter_Low_Index = 4;
    public static short Default_Filter_CW_Index = 2;
    public static bool Band_Stack_Updating = false;
    public const byte CMD_SET_BW_LOCUT = 0xD0;
    public const byte CMD_SET_BW_HICUT = 0xD1;
    public const byte CMD_SET_CW_PITCH = 0xD2;
    public const byte CMD_SET_TX_HICUT = 0xD3;
    public const byte CMD_SET_CW_BW = 0xDB;
    public const byte CMD_SET_BW_LOCUT_DEFAULT = 0xDC;
    public const byte CMD_SET_BW_HICUT_DEFAULT = 0xDD;
    public const byte CMD_SET_CW_BW_DEFAULT = 0xDE;
    public static int CW_Pitch = 0;
    public static int CW_Bw = 0;
    public static int CW_Pitch_Index = 0;
}

public static class Mouse_controls
{
    public static bool Allow_Frequency_Updates = true;
    public static bool Silent_Update = false;
}

public static class Last_used
{
    public static char Current_mode;
    public static short Previous_Main_Band = 0;
    public struct B160
    {
        public static Int32 Freq;
        public static char Mode;
        public static short Filter_High_Index = 2;
        public static short Filter_Low_Index = 4;
        public static short Filter_CW_index = 2;
        public static short AM_power = 50;
        public static short SSB_power = 50;
        public static short CW_power = 50;
    }
    public struct B80
    {
        public static Int32 Freq;
        public static char Mode;
        public static short Filter_High_Index = 2;
        public static short Filter_Low_Index = 4;
        public static short Filter_CW_index = 2;
        public static short AM_power = 50;
        public static short SSB_power = 50;
        public static short CW_power = 50;
    }
    public struct B60
    {
        public static Int32 Freq;
        public static char Mode;
        public static short Filter_High_Index = 2;
        public static short Filter_Low_Index = 4;
        public static short Filter_CW_index = 2;
        public static short AM_power = 50;
        public static short SSB_power = 50;
        public static short CW_power = 50;
    }
    public struct B40
    {
        public static Int32 Freq;
        public static char Mode;
        public static short Filter_High_Index = 2;
        public static short Filter_Low_Index = 4;
        public static short Filter_CW_index = 2;
        public static short AM_power = 50;
        public static short SSB_power = 50;
        public static short CW_power = 50;
    }
    public struct B30
    {
        public static Int32 Freq;
        public static char Mode;
        public static short Filter_High_Index = 2;
        public static short Filter_Low_Index = 4;
        public static short Filter_CW_index = 2;
        public static short AM_power = 50;
        public static short SSB_power = 50;
        public static short CW_power = 50;
    }
    public struct B20
    {
        public static Int32 Freq;
        public static char Mode;
        public static short Filter_High_Index = 2;
        public static short Filter_Low_Index = 4;
        public static short Filter_CW_index = 2;
        public static short AM_power = 50;
        public static short SSB_power = 50;
        public static short CW_power = 50;
    }
    public struct B17
    {
        public static Int32 Freq;
        public static char Mode;
        public static short Filter_High_Index = 2;
        public static short Filter_Low_Index = 4;
        public static short Filter_CW_index = 2;
        public static short AM_power = 50;
        public static short SSB_power = 50;
        public static short CW_power = 50;
    }
    public struct B15
    {
        public static Int32 Freq;
        public static char Mode;
        public static short Filter_High_Index = 2;
        public static short Filter_Low_Index = 4;
        public static short Filter_CW_index = 2;
        public static short AM_power = 50;
        public static short SSB_power = 50;
        public static short CW_power = 50;
    }
    public struct B12
    {
        public static Int32 Freq;
        public static char Mode;
        public static short Filter_High_Index = 2;
        public static short Filter_Low_Index = 4;
        public static short Filter_CW_index = 2;
        public static short AM_power = 50;
        public static short SSB_power = 50;
        public static short CW_power = 50;
    }
    public struct B10
    {
        public static Int32 Freq;
        public static char Mode;
        public static short Filter_High_Index = 2;
        public static short Filter_Low_Index = 4;
        public static short Filter_CW_index = 2;
        public static short AM_power = 50;
        public static short SSB_power = 50;
        public static short CW_power = 50;
    }
    public struct GEN
    {
        public static Int32 Freq = 10000000;
        public static char Mode = 'A';
        public static short Filter_High_Index = 2;
        public static short Filter_Low_Index = 4;
        public static short Filter_CW_index = 2;
    }
}

public static class CW_Parameters
{
    public static short CW_Firmware_Option;
    public static short CW_Restore_Defaults;
    public static short CW_Iambic_Mode_On_Off;
    public static short CW_Iambic_Type;
    public static short CW_Iambic_Calibrate;
    public static short CW_Memory;
    public static short CW_Spacing;
    public static short CW_Paddle;
    public static short CW_Weight;
    public static short CW_Tx_Hold;
    public static short CW_Speed;
    public static short CW_Semi_Break_In;
    public static short CW_Semi_Control;
    public static short CW_Side_Tone_Volume;
}

public static class oCode
{
    public static int oldTxHold = 0;
    public static int oldIambic = 0;
    public static int oldSemiBreakin = 0;
    public static int oldSideTone = 0;
    public static int oldCW = 0;
    public static int oldTransverter = 0;
    public static bool transverter_warning_accepted = false;
    public static bool transverter_warning_displayed = false;
    public static bool trans_on = false;
    //public static bool isTuning = false;
    public static bool isLoading = false;
    public static short band_stack_band = 20;
    public static short band_stack_element = 0;
    public static bool band_stack_updating = false;
    public static bool display_messages = true;
    //public static short tune_toggle = 1;
    //public static bool is_tuning_mode = false;
    public static long line_count = 0;
    public static short modeswitch = 0;
    public const short general_band = 200;
    public static short current_band = 0;
    public static short freq_scroll_bar_index = 0;
    public static bool gen_band_active = false;
    //public static bool display_heartbeat = false;
    public static bool monitor_suspend = false;
    public static short FreqDigit = 0;
    public static short Previous_FreqDigit = 0;
    public static Int32 DisplayFreq = 1407600;
    public static short Freq_Tune_Index = 0;

    public static int Platform = 0;
    public static short Last_band = 0;
    //public static short MSSDR_running = 0;
    public static short previous_main_band = 200;
    public static int major_version = 0;
    public static int minor_version = 0;
    //public static bool version_first = false;

    //CW 
    public const byte SET_CW_MODE = 0x70;
    public const byte SET_IAMBIC_MODE = 0x71;
    public const byte SET_LAG = 0x72;
    public const byte SET_CW_PADDLE = 0x73;
    public const byte SET_IAMBIC_TYPE = 0x74;
    public const byte SET_SPACING = 0x75;
    public const byte SET_MEMORY_TYPE = 0x76;
    public const byte SET_WEIGHT = 0x77;
    public const byte SET_TX_HOLD = 0x7A;
    public const byte SET_WPM = 0x7B;
    public const byte SET_SEMI_BREAKIN = 0x78;
    public const byte SET_SIDE_TONE_VOLUME = 0x7F;


    // 0xA4 NOT AVAILABLE Reserved for CMD_GET_KEY_DOWN 0xA4
    public const byte CMD_SET_RELOAD_FILE = 0xA5;
    public const byte CMD_SET_RIG_TUNE = 0xA6;
    //public static byte CMD_SET_CALIBRATE = 0xA7;
    public const byte CMD_SET_TRANSVERTER = 0xA9;
    //public static byte CMD_SET_STANDARD_CARRIER = 0xAF;

    //Power Calibration
    public const byte CMD_SET_BAND_POWER_BAND = 0xA1;
    public const byte CMD_SET_BAND_POWER_POWER = 0xA2;
    public const byte CMD_SET_BAND_POWER_DEFAULTS = 0xAA;
    public const byte CMD_GET_BAND_POWER = 0xB4;

    //Band Stacking 
    public const byte CMD_SET_UPDATE_BAND_STACK = 0xC1;
    public const byte CMD_GET_BAND_STACK = 0xC2;
    public const byte CMD_SET_BAND_STACK = 0xC3;
    public const byte CMD_GET_BAND_STACK_MODE = 0xC4;
    public const byte CMD_GET_BAND_STACK_FREQ = 0xC5;
    public const byte CMD_SET_BAND_STACK_INDEX = 0xC6;
    public const byte CMD_GET_BAND_STACK_BAND = 0xC7;

    //Last Used
    public const byte CMD_GET_SET_LAST_USED_FREQ = 0xD7;
    public const byte CMD_GET_SET_LAST_USED_MODE = 0xD8;
    public const byte CMD_GET_SET_LAST_USED_BAND = 0xD9;

    //Volume Calibration
    public const byte CMD_SET_LEFT_VOLUME = 0xE0;
    public const byte CMD_SET_RIGHT_VOLUME = 0xE1;


    //Audio Device Control
    public const byte CMD_GET_SET_SPEAKER_DEVICE = 0xEA;
    public const byte CMD_GET_SET_MIC_DEVICE = 0xEB;

    public const byte CMD_GET_FREQ_INIT = 0xB0;
    public const byte CMD_SET_MAIN_FREQ = 0xB6;
    public const byte CMD_SET_MAIN_MODE = 0xB7;
    public const byte CMD_GET_MODE_INIT = 0xB8;
    public const byte CMD_GET_BAND_INIT = 0xB9;
    public const byte CMD_SET_TX_ON = 0xBA;

    public const byte CMD_SET_HDSDR_STATUS = 0xF0;
    public const byte CMD_GET_HDSDR_STATUS = 0xF1;
    public const byte CMD_SET_POWER_MODE_STATE = 0xF2;
    public const byte CMD_SET_KEEP_ALIVE = 0xF4;
    public const byte CMD_GET_SET_MSSDR_STATUS = 0xF5;
    public const byte CMD_GET_SET_STARTUP_BAND = 0xF6;
    public const byte CMD_GUI_RUNNING = 0xFE;
    public const byte CMD_SET_STOP = 0xFF;

    public struct iniStates
    {
        /*public static short CW_Firmware_Option;
        public static short CW_Restore_Defaults;
        public static short CW_Iambic_Mode_On_Off;
        public static short CW_Iambic_Type;
        public static short CW_Iambic_Calibrate;
        public static short CW_Memory;
        public static short CW_Spacing;
        public static short CW_Paddle;
        public static short CW_Weight;
        public static short CW_Tx_Hold;
        public static short CW_Speed;
        public static short CW_Semi_Break_In;
        public static short CW_Semi_Control;
        public static short CW_Side_Tone_Volume;
        */
        public static int DLL_PORT;
        public static int GUI_PORT;
        public static string DLL_IP;
        public static string GUI_IP;
    }

    public static void guiCode_Write_Debug_Message(String Message)
    {
        var thisForm2 = Application.OpenForms.OfType<Main_form>().Single();
        thisForm2.Write_Message(Message);
    }

    public static void SendCommand(Socket s, IPEndPoint target, Byte opcode, short data)
    {
        Byte[] msg = new Byte[3];
        Byte[] sbytes = new Byte[2];
        String Message;
        //int send_retry = 0;
        //bool message_sent = false;

        Message = " Send Command called -> opcode: " + String.Format("#{0:X}", opcode);
        //guiCode_Write_Debug_Message(Message);
        if (Master_Controls.Initialize_network_status)
        {
            msg[0] = opcode;
            sbytes = BitConverter.GetBytes(data);
            msg[1] = sbytes[0];
            msg[2] = sbytes[1];

            try
            {
                s.SendTo(msg, target);
                //s.BeginSendTo(msg, 0, 3, SocketFlags.None, target, null, null);
            }
            catch (SocketException e)
            {
                if (!Master_Controls.Socket_Error)
                {
                    Master_Controls.Socket_Error = true;
                    MessageBox.Show("Network Error: " + e.ToString() + " MSCC will stop", "MSCC",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
        }
        msg = null;
        sbytes = null;
    }

    public static void SendCommand32(Socket s, IPEndPoint target, Byte opcode, int data)
    {
        Byte[] msg = new Byte[5];
        Byte[] sbytes = new Byte[4];

        msg[0] = opcode;
        sbytes = BitConverter.GetBytes(data);
        msg[1] = sbytes[0];
        msg[2] = sbytes[1];
        msg[3] = sbytes[2];
        msg[4] = sbytes[3];

        s.SendTo(msg, target);

        msg = null;
        sbytes = null;
    }

    public static void SendCommand_String(Socket s, IPEndPoint target, Byte opcode, String name_data, int length)
    {
        int i = 0;
        Byte[] msg = new Byte[length + 1];

        byte[] msg_data = Encoding.ASCII.GetBytes(name_data);
        msg[0] = opcode;
        for (i = 1; i <= length; i++)
        {
            msg[i] = msg_data[(i - 1)];
        }

        s.SendTo(msg, target);
        msg = null;
    }

    public static void SendCommand_MultiByte(Socket s, IPEndPoint target, Byte opcode, byte[] name_data, int length)
    {
        int i = 0;
        Byte[] msg = new Byte[length + 1];

        byte[] msg_data = name_data;
        msg[0] = opcode;
        for (i = 1; i <= length; i++)
        {
            msg[i] = msg_data[(i - 1)];
        }

        s.SendTo(msg, target);
        msg = null;
    }

    public static bool getIniStates()
    {
        String path, line;
        System.IO.StreamReader file;
        //int p;

        // get path to local Appdata folder
        path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        // add our folder and file name
#if RPI
        path += "/mscc/mscc-rpi.ini";
#else
        path += "\\multus-sdr-client\\Multus_mscc.ini";
#endif
        guiCode_Write_Debug_Message(" getIiniStates -> ini file: " + path);

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
            MessageBox.Show("IO Exception opening INI file." + er + " Have you run the INI creation utility?", "MSCC", MessageBoxButtons.OK,
                                                                                MessageBoxIcon.Exclamation);
            return false;
        }
        if (!System.IO.File.Exists(path))
        {
            MessageBox.Show("IO Exception opening INI file. \r\n Have you run the INI creation utility?", "MSCC", MessageBoxButtons.OK,
                                                                               MessageBoxIcon.Exclamation);
            return false;
        }


        iniStates.GUI_PORT = 0;
        iniStates.DLL_PORT = 0;
        // Parse the INI file. Doing it this way eliminates the requirement that the parameters be stored in any particular order.
        // The parms are stored in a public static struct, which can be reached by the form-level GUI code.
        while ((line = file.ReadLine()) != null)
        {
#if !RPI
            if (line.Contains("CW_Firmware_Option")) CW_Parameters.CW_Firmware_Option = getIntFromIniString(line);
            if (line.Contains("CW_Restore_Defaults")) CW_Parameters.CW_Restore_Defaults = getIntFromIniString(line);
            if (line.Contains("CW_Iambic_Mode_On_Off")) CW_Parameters.CW_Iambic_Mode_On_Off = getIntFromIniString(line);
            if (line.Contains("CW_Iambic_Type")) CW_Parameters.CW_Iambic_Type = getIntFromIniString(line);
            if (line.Contains("CW_Iambic_Calibrate")) CW_Parameters.CW_Iambic_Calibrate = getIntFromIniString(line);
            if (line.Contains("CW_Memory")) CW_Parameters.CW_Memory = getIntFromIniString(line);
            if (line.Contains("CW_Spacing")) CW_Parameters.CW_Spacing = getIntFromIniString(line);
            if (line.Contains("CW_Paddle")) CW_Parameters.CW_Paddle = getIntFromIniString(line);
            if (line.Contains("CW_Weight")) CW_Parameters.CW_Weight = getIntFromIniString(line);
            if (line.Contains("CW_Tx_Hold")) CW_Parameters.CW_Tx_Hold = getIntFromIniString(line);
            if (line.Contains("CW_Speed")) CW_Parameters.CW_Speed = getIntFromIniString(line);
            if (line.Contains("CW_Semi_Break_In")) CW_Parameters.CW_Semi_Break_In = getIntFromIniString(line);
            if (line.Contains("CW_Semi_Control")) CW_Parameters.CW_Semi_Control = getIntFromIniString(line);
            if (line.Contains("CW_Side_Tone_Volume")) CW_Parameters.CW_Side_Tone_Volume = getIntFromIniString(line);
#endif
            if (line.Contains("MSCC_PORT")) iniStates.GUI_PORT = getIntFromIni32String(line);
            if (line.Contains("PROFICIO_DLL_PORT")) iniStates.DLL_PORT = getIntFromIni32String(line);
            if (line.Contains("MSCC_IP")) iniStates.GUI_IP = getStringFromIniString(line);
            if (line.Contains("PROFICIO_DLL_IP")) iniStates.DLL_IP = getStringFromIniString(line);
        }

        file.Close();
        return true;

    }

    public static bool Get_CW_Params()
    {
        String path, line;
        System.IO.StreamReader file;
        //int p;

        // get path to local Appdata folder
        path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        // add our folder and file name

        path += "/mscc/Multus_mscc.ini";
        guiCode_Write_Debug_Message(" Get_CW_Params -> ini file: " + path);

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
            MessageBox.Show("IO Exception opening Multus_mscc.ini file." + er +
                " Have you run the INI creation utility?", "MSCC", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            return false;
        }
        if (!System.IO.File.Exists(path))
        {
            MessageBox.Show("IO Exception opening Multus_mscc.ini file. \r\n Have you run the INI creation utility?",
                "MSCC", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            return false;
        }
        // Parse the INI file. Doing it this way eliminates the requirement that the parameters be stored in any particular order.
        // The parms are stored in a public static struct, which can be reached by the form-level GUI code.
        while ((line = file.ReadLine()) != null)
        {
            if (line.Contains("CW_Firmware_Option")) CW_Parameters.CW_Firmware_Option = getIntFromIniString(line);
            if (line.Contains("CW_Restore_Defaults")) CW_Parameters.CW_Restore_Defaults = getIntFromIniString(line);
            if (line.Contains("CW_Iambic_Mode_On_Off")) CW_Parameters.CW_Iambic_Mode_On_Off = getIntFromIniString(line);
            if (line.Contains("CW_Iambic_Type")) CW_Parameters.CW_Iambic_Type = getIntFromIniString(line);
            if (line.Contains("CW_Iambic_Calibrate")) CW_Parameters.CW_Iambic_Calibrate = getIntFromIniString(line);
            if (line.Contains("CW_Memory")) CW_Parameters.CW_Memory = getIntFromIniString(line);
            if (line.Contains("CW_Spacing")) CW_Parameters.CW_Spacing = getIntFromIniString(line);
            if (line.Contains("CW_Paddle")) CW_Parameters.CW_Paddle = getIntFromIniString(line);
            if (line.Contains("CW_Weight")) CW_Parameters.CW_Weight = getIntFromIniString(line);
            if (line.Contains("CW_Tx_Hold")) CW_Parameters.CW_Tx_Hold = getIntFromIniString(line);
            if (line.Contains("CW_Speed")) CW_Parameters.CW_Speed = getIntFromIniString(line);
            if (line.Contains("CW_Semi_Break_In")) CW_Parameters.CW_Semi_Break_In = getIntFromIniString(line);
            if (line.Contains("CW_Semi_Control")) CW_Parameters.CW_Semi_Control = getIntFromIniString(line);
            if (line.Contains("CW_Side_Tone_Volume")) CW_Parameters.CW_Side_Tone_Volume = getIntFromIniString(line);
        }

        file.Close();
        return true;

    }

    public static string getStringFromIniString(string str)
    {
        int eqpos;
        string nstr;


        if (str.Contains("=") == false) return null;       // ain't no dang equals sign in the string
        eqpos = str.IndexOf("=");                       // get the position of the equals sign in the string
        nstr = str.Substring(eqpos + 1, str.Length - eqpos - 2);   // parse everything between the "=" and the ";" into another string
        return nstr;
    }

    public static short getIntFromIniString(string str)
    {
        int eqpos;
        short output;
        string nstr;


        if (str.Contains("=") == false) return 0;       // ain't no dang equals sign in the string
        eqpos = str.IndexOf("=");                       // get the position of the equals sign in the string
        nstr = str.Substring(eqpos + 1, str.Length - eqpos - 2);   // parse everything between the "=" and the ";" into another string
        Int16.TryParse(nstr, out output);               // a complicated, but best-practice way to extract a short from a string
        return output;
    }

    public static int getIntFromIni32String(string str)
    {
        int eqpos;
        int output;
        string nstr;
        string oput;

        if (str.Contains("=") == false) return 0;       // ain't no dang equals sign in the string
        eqpos = str.IndexOf("=");                       // get the position of the equals sign in the string
        nstr = str.Substring(eqpos + 1, str.Length - eqpos - 2);   // parse everything between the "=" and the ";" into another string
        int.TryParse(nstr, out output);               // a complicated, but best-practice way to extract a short from a string
        oput = Convert.ToString(output);
        return output;
    }

    public static bool Start_subsystem(int start_stop)
    {
        String Ms_sdr_path;
        String path;
        String Sdr_core_recv_path;
        String SDR_core_trans_path;

        if (start_stop == 1)
        {
            //oCode.Platform = (int)Environment.OSVersion.Platform;
            path = AppDomain.CurrentDomain.BaseDirectory;
            // A kludge to check for non Windows OS.  
            //These values may change in the future.

#if RPI
            Ms_sdr_path = (path + "/ms-sdr");
            Sdr_core_recv_path = (path + "/sdrcore-recv");
            SDR_core_trans_path = (path + "/sdrcore-trans");
#else
            Ms_sdr_path = (path + "\\ms-sdr.exe");
            Sdr_core_recv_path = (path + "\\sdrcore-recv.exe");
            SDR_core_trans_path = (path + "\\sdrcore-trans.exe");
#endif


            SDRprocesses.sdrcore_trans = new Process();
            try
            {
                SDRprocesses.sdrcore_trans.StartInfo.UseShellExecute = false;
                SDRprocesses.sdrcore_trans.StartInfo.FileName = SDR_core_trans_path;
                SDRprocesses.sdrcore_trans.StartInfo.CreateNoWindow = true;
                SDRprocesses.sdrcore_trans.StartInfo.Arguments = "test";
                SDRprocesses.sdrcore_trans.Start();
            }
            catch (Exception e)
            {
                Master_Controls.MSSDR_running = false;
                SDRprocesses.sdrcore_trans_running = false;
                DialogResult ret = MessageBox.Show("SDRcore-trans did not start. \r\n" + e.Message, "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

            SDRprocesses.sdrcore_recv = new Process();
            try
            {
                SDRprocesses.sdrcore_recv.StartInfo.UseShellExecute = false;
                SDRprocesses.sdrcore_recv.StartInfo.FileName = Sdr_core_recv_path;
                SDRprocesses.sdrcore_recv.StartInfo.CreateNoWindow = true;
                SDRprocesses.sdrcore_recv.StartInfo.Arguments = "test";
                SDRprocesses.sdrcore_recv.Start();
            }
            catch (Exception e)
            {
                Master_Controls.MSSDR_running = false;
                SDRprocesses.sdrcore_recv_running = false;
                DialogResult ret = MessageBox.Show("SDRcore-recv did not start. \r\n" + e.Message, "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

            SDRprocesses.ms_sdr = new Process();
            try
            {
                SDRprocesses.ms_sdr.StartInfo.UseShellExecute = false;
                SDRprocesses.ms_sdr.StartInfo.FileName = Ms_sdr_path;
                SDRprocesses.ms_sdr.StartInfo.CreateNoWindow = true;
                SDRprocesses.ms_sdr.StartInfo.Arguments = "test";
                SDRprocesses.ms_sdr.Start();
            }
            catch (Exception e)
            {
                Master_Controls.MSSDR_running = false;
                SDRprocesses.ms_sdr_running = false;
                DialogResult ret = MessageBox.Show("MS-SDR did not start.\r\n" + e.Message + "\r\n" + path + "\r\n", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

        }
        else
        {
            //Thread.Sleep(1000);
            if (SDRprocesses.ms_sdr_running == true)
            {
                try
                {
                    SDRprocesses.ms_sdr.Kill();
                }
                catch (Exception e)
                {
                    DialogResult ret = MessageBox.Show("MS-SDR is not running. Nothing to stop " + e.Message + "\r\n", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            if (SDRprocesses.sdrcore_recv_running == true)
            {
                try
                {
                    SDRprocesses.sdrcore_recv.Kill();
                }
                catch (Exception e)
                {
                    DialogResult ret = MessageBox.Show("sdrcore_recv is not running. Nothing to stop " + e.Message + "\r\n", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            if (SDRprocesses.sdrcore_trans_running == true)
            {
                try
                {
                    SDRprocesses.sdrcore_trans.Kill();
                }
                catch (Exception e)
                {
                    DialogResult ret = MessageBox.Show("sdrcore_trans is not running. Nothing to stop " + e.Message + "\r\n", "MSCC",
                                                                                      MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }

        }
        return Master_Controls.MSSDR_running;
    }
}




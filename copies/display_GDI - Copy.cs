//=================================================================
// display.cs
//=================================================================
// PowerSDR is a C# implementation of a Software Defined Radio.
// Copyright (C) 2004, 2005, 2006  FlexRadio Systems
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// You may contact us via email at: sales@flex-radio.com.
// Paper mail may be sent to: 
//    FlexRadio Systems
//    12100 Technology Blvd.
//    Austin, TX 78727
//    USA
//=================================================================

/*
 *  Changes for Multus SDR, LLC.
 *  Copyright (C)2019
*/

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using OmniaGUI.Properties;


namespace OmniaGUI
{

    public enum RenderType
    {
        HARDWARE = 0,
        SOFTWARE,
        NONE,
    }


    static class Display_GDI
    {
        #region Variable Declaration
        //private static int pass_count = 0;
        //private static double avg_last_ddsfreq = 0;				// Used to move the display average when tuning
        //private static double avg_last_dttsp_osc = 0;

        public static Bitmap panadapter_bmp;  				    // Bitmap for use when drawing
        private static Bitmap waterfall_bmp;					// saved waterfall picture for display
        private static int[] histogram_data;					// histogram display buffer
        private static int[] histogram_history;					// histogram counter
        public const float CLEAR_FLAG = -999.999F;				// for resetting buffers
        public const int BUFFER_SIZE = 4096;
        public static float[] new_display_data;					// Buffer used to store the new data from the DSP for the display
        public static float[] new_scope_data;					// Buffer used to store the new data from the DSP for the scope
        public static float[] new_waterfall_data;    			// Buffer used to store the new data from the DSP for the waterfall
        public static float[] current_display_data;				// Buffer used to store the current data for the display
        public static float[] current_scope_data;   		    // Buffer used to store the current data for the scope
        public static float[] current_waterfall_data;		    // Buffer used to store the current data for the waterfall
        public static float[] waterfall_display_data;            // Buffer for waterfall

        public static float[] average_buffer;					// Averaged display data buffer for Panadapter
        public static float[] average_waterfall_buffer;  		// Averaged display data buffer for Waterfall
        public static float[] peak_buffer;						// Peak hold display data buffer
        public static Mutex display_data_mutex = new Mutex();

        public static int server_W = 1024;                       // for Server screen width
        public static int client_W = 1024;                       // for Client screen width
        public static byte[] server_display_data;
        public static byte[] client_display_data;
        private static System.Drawing.Font swr_font = new System.Drawing.Font("Arial", 14, FontStyle.Bold);
        public static string panadapter_img = "";
        public static int sample_rate = 96000;
        public static Bitmap img = Properties.Resources.BITMAP1;
        public static double SampleRate
        {
            get { return sample_rate; }
            set
            {
                sample_rate = (int)value;
                //int i = SetSampleRate(sample_rate);
            }
        }
        public static int increament = 0;


        #endregion

        #region Properties

        private static ColorSheme color_sheme = ColorSheme.original;        // yt7pwr
        public static ColorSheme ColorSheme
        {
            get { return color_sheme; }

            set { color_sheme = value; }
        }

        private static bool reverse_waterfall = false;
        public static bool ReverseWaterfall
        {
            get { return reverse_waterfall; }
            set { reverse_waterfall = value; }
        }

        public static bool smooth_line = false;
        public static bool pan_fill = false;
        public static int filter_low = 0;
        public static int filter_high = 0;
        public static bool TUN = false;
        //public static bool mox = false;
        private static Font panadapter_font = new System.Drawing.Font("Arial", 9);
        public static Font PanadapterFont
        {
            get { return panadapter_font; }
            set { panadapter_font = value; }
        }

        private static Color pan_fill_color = Color.FromArgb(100, 0, 0, 127);
        public static Color PanFillColor
        {
            get { return pan_fill_color; }
            set { pan_fill_color = value; }
        }

        private static Color scope_color = Color.FromArgb(100, 0, 0, 127);
        public static Color ScopeColor
        {
            get { return scope_color; }
            set { scope_color = value; }
        }

        private static Color display_text_background = Color.FromArgb(127, 0, 0, 0);
        public static Color DisplayTextBackground
        {
            get { return display_text_background; }
            set { display_text_background = value; }
        }

        private static Color display_filter_color = Color.FromArgb(65, 0, 255, 0);
        public static Color DisplayFilterColor
        {
            get { return display_filter_color; }
            set
            {
                display_filter_color = value;
            }
        }

        private static bool show_horizontal_grid = false;
        public static bool Show_Horizontal_Grid
        {
            get { return show_horizontal_grid; }
            set { show_horizontal_grid = value; }
        }

        private static bool show_vertical_grid = false;
        public static bool Show_Vertical_Grid
        {
            get { return show_vertical_grid; }
            set { show_vertical_grid = value; }
        }

        private static int phase_num_pts = 100;
        public static int PhaseNumPts
        {
            get { return phase_num_pts; }
            set { phase_num_pts = value; }
        }

        private static Color main_rx_zero_line_color = Color.LightSkyBlue;
        public static Color MainRXZeroLine
        {
            get { return main_rx_zero_line_color; }
            set
            {
                main_rx_zero_line_color = value;
            }
        }

        private static Color sub_rx_zero_line_color = Color.Red;
        public static Color SubRXZeroLine
        {
            get { return sub_rx_zero_line_color; }
            set
            {
                sub_rx_zero_line_color = value;
            }
        }

        private static Color main_rx_filter_color = Color.FromArgb(127, 0, 128, 128);
        public static Color MainRXFilterColor
        {
            get { return main_rx_filter_color; }
            set
            {
                main_rx_filter_color = value;
            }
        }

        private static Color sub_rx_filter_color = Color.FromArgb(127, 0, 0, 255);  // blue
        public static Color SubRXFilterColor
        {
            get { return sub_rx_filter_color; }
            set
            {
                sub_rx_filter_color = value;
            }
        }

        private static bool sub_rx_enabled = false;
        public static bool SubRXEnabled
        {
            get { return sub_rx_enabled; }
            set
            {
                sub_rx_enabled = value;
            }
        }

        private static bool split_enabled = false;
        public static bool SplitEnabled
        {
            get { return split_enabled; }
            set
            {
                split_enabled = value;
            }
        }

        private static bool show_freq_offset = false;
        public static bool ShowFreqOffset
        {
            get { return show_freq_offset; }
            set
            {
                show_freq_offset = value;
            }
        }

        private static Color band_edge_color = Color.Red;
        public static Color BandEdgeColor
        {
            get { return band_edge_color; }
            set
            {
                band_edge_color = value;
            }
        }

        private static long losc_hz = 0; // yt7pwr
        public static long LOSC
        {
            get { return losc_hz; }
            set
            {
                losc_hz = value;
            }
        }

        private static long vfoa_hz;
        public static long VFOA
        {
            get { return vfoa_hz; }
            set
            {
                vfoa_hz = value;
            }
        }

        private static long vfob_hz;
        public static long VFOB
        {
            get { return vfob_hz; }
            set
            {
                vfob_hz = value;
            }
        }

        private static int rit_hz;
        public static int RIT
        {
            get { return rit_hz; }
            set
            {
                rit_hz = value;
            }
        }

        private static int xit_hz;
        public static int XIT
        {
            get { return xit_hz; }
            set
            {
                xit_hz = value;
            }
        }

        private static int cw_pitch = 600;
        public static int CWPitch
        {
            get { return cw_pitch; }
            set { cw_pitch = value; }
        }

        private static int H = 397;	// target height
        private static int W = 760;	// target width
        //private static Control target = null;
        /*public static Control Target
        {
            get { return target; }
            set
            {
                target = value;
                H = target.Height;
                W = target.Width;
            }
        }*/
        public static Size window_size;
        public static Size Window_size
        {
            get { return window_size; }
            set
            {
                window_size = value;
                H = window_size.Height;
                W = window_size.Width;
            }
        }

        public static bool DisplayNotchFilter = false;
        private static int rx_display_notch_low_cut = -4000;        // yt7pwr
        public static int RXDisplayNotchLowCut
        {
            set { rx_display_notch_low_cut = value; }
        }

        private static int rx_display_notch_high_cut = 4000;        // yt7pwr
        public static int RXDisplayNotchHighCut
        {
            set { rx_display_notch_high_cut = value; }
        }

        private static int rx_display_low = -48000;
        public static int RXDisplayLow
        {
            get { return rx_display_low; }
            set { rx_display_low = value; }
        }

        private static int rx_display_high = 48000;
        public static int RXDisplayHigh
        {
            get { return rx_display_high; }
            set { rx_display_high = value; }
        }

        private static int tx_display_low = -48000;
        public static int TXDisplayLow
        {
            get { return tx_display_low; }
            set { tx_display_low = value; }
        }

        private static int tx_display_high = 4000;
        public static int TXDisplayHigh
        {
            get { return tx_display_high; }
            set { tx_display_high = value; }
        }

        private static float display_cal_offset = -82.621f;					// display calibration offset per volume setting in dB
        public static float DisplayCalOffset
        {
            get { return display_cal_offset; }
            set { display_cal_offset = value; }
        }

        private static int display_cursor_x;						// x-coord of the cursor when over the display
        public static int DisplayCursorX
        {
            get { return display_cursor_x; }
            set { display_cursor_x = value; }
        }

        private static int display_cursor_y;						// y-coord of the cursor when over the display
        public static int DisplayCursorY
        {
            get { return display_cursor_y; }
            set { display_cursor_y = value; }
        }

        private static ClickTuneMode current_click_tune_mode = ClickTuneMode.Off;
        public static ClickTuneMode CurrentClickTuneMode
        {
            get { return current_click_tune_mode; }
            set { current_click_tune_mode = value; }
        }

        //private static int sample_rate = 48000;
        public static int sSampleRate
        {
            get { return sample_rate; }
            set { sample_rate = value; }
        }

        private static bool high_swr = false;
        public static bool HighSWR
        {
            get { return high_swr; }
            set { high_swr = value; }
        }

        private static bool mox = false;
        public static bool MOX
        {
            get { return mox; }
            set { mox = value; }
        }

        private static DSPMode current_dsp_mode_subRX = DSPMode.USB;  // yt7pwr
        public static DSPMode CurrentDSPModeSubRX
        {
            get { return current_dsp_mode_subRX; }
            set { current_dsp_mode_subRX = value; }
        }

        private static DSPMode current_dsp_mode = DSPMode.USB;
        public static DSPMode CurrentDSPMode
        {
            get { return current_dsp_mode; }
            set { current_dsp_mode = value; }
        }

        private static DisplayMode current_display_mode = DisplayMode.WATERFALL;
        public static DisplayMode CurrentDisplayMode // changes yt7pwr
        {
            get { return current_display_mode; }
            set
            {
                current_display_mode = value;

                switch (current_display_mode)
                {
                    case DisplayMode.PANAFALL:
                    case DisplayMode.PANAFALL_INV:
                    case DisplayMode.WATERFALL:
                        break;
                        //case DisplayMode.PANADAPTER:                        

                }

                switch (current_display_mode)
                {
                    case DisplayMode.PHASE:
                    case DisplayMode.PHASE2:
                        //Audio.phase = true;
                        break;
                    default:
                        //Audio.phase = false;
                        break;
                }

                //if (average_on) ResetDisplayAverage();
                //if (peak_on) ResetDisplayPeak();
            }
        }

        private static float max_x;								// x-coord of maxmimum over one display pass
        public static float MaxX
        {
            get { return max_x; }
            set { max_x = value; }
        }

        private static float scope_max_x;								// x-coord of maxmimum over one display pass
        public static float ScopeMaxX
        {
            get { return scope_max_x; }
            set { scope_max_x = value; }
        }

        private static float max_y;								// y-coord of maxmimum over one display pass
        public static float MaxY
        {
            get { return max_y; }
            set { max_y = value; }
        }

        private static float scope_max_y;								// y-coord of maxmimum over one display pass
        public static float ScopeMaxY
        {
            get { return scope_max_y; }
            set { scope_max_y = value; }
        }

        //private static bool average_on;							// True if the Average button is pressed
        /*public static bool AverageOn
        {
            get { return average_on; }
            set
            {
                average_on = value;
                if (!average_on) ResetDisplayAverage();
            }
        }*/

        public static bool scope_data_ready = false;
        private static bool data_ready;					// True when there is new display data ready from the DSP
        public static bool DataReady
        {
            get { return data_ready; }
            set { data_ready = value; }
        }

        private static bool waterfall_data_ready = false;	    // True when there is new display data ready from the DSP
        public static bool WaterfallDataReady
        {
            get { return waterfall_data_ready; }
            set { waterfall_data_ready = value; }
        }

        public static float display_avg_mult_old = 1 - (float)1 / 2;
        public static float display_avg_mult_new = (float)1 / 2;
        private static int display_avg_num_blocks = 2;
        public static int DisplayAvgBlocks
        {
            get { return display_avg_num_blocks; }
            set
            {
                display_avg_num_blocks = value;
                display_avg_mult_old = 1 - (float)1 / display_avg_num_blocks;
                display_avg_mult_new = (float)1 / display_avg_num_blocks;
            }
        }

        public static float waterfall_avg_mult_old = 1 - (float)1 / 18;
        public static float waterfall_avg_mult_new = (float)1 / 18;
        private static int waterfall_avg_num_blocks = 18;
        public static int WaterfallAvgBlocks
        {
            get { return waterfall_avg_num_blocks; }
            set
            {
                waterfall_avg_num_blocks = value;
                waterfall_avg_mult_old = 1 - (float)1 / waterfall_avg_num_blocks;
                waterfall_avg_mult_new = (float)1 / waterfall_avg_num_blocks;
            }
        }

        private static int spectrum_grid_max = 800;
        public static int SpectrumGridMax
        {
            get { return spectrum_grid_max; }
            set
            {
                spectrum_grid_max = value;
            }
        }

        private static int spectrum_grid_min = 0;
        public static int SpectrumGridMin
        {
            get { return spectrum_grid_min; }
            set
            {
                spectrum_grid_min = value;
            }
        }

        private static int spectrum_grid_step = 10;
        public static int SpectrumGridStep
        {
            get { return spectrum_grid_step; }
            set
            {
                spectrum_grid_step = value;
            }
        }

        private static Color grid_text_color = Color.White;
        public static Color GridTextColor
        {
            get { return grid_text_color; }
            set
            {
                grid_text_color = value;
            }
        }

        private static Color grid_zero_color = Color.Red;
        public static Color GridZeroColor
        {
            get { return grid_zero_color; }
            set
            {
                grid_zero_color = value;
            }
        }

        private static Color grid_color = Color.Green;
        public static Color GridColor
        {
            get { return grid_color; }
            set
            {
                grid_color = value;
            }
        }

        private static Pen data_line_pen = new Pen(new SolidBrush(Color.LightGreen), display_line_width);

        private static Color data_line_color = Color.LightGreen;
        public static Color DataLineColor
        {
            get { return data_line_color; }
            set
            {
                data_line_color = value;
                data_line_pen = new Pen(new SolidBrush(data_line_color), display_line_width);
            }
        }

        private static Color display_filter_tx_color = Color.Yellow;
        public static Color DisplayFilterTXColor
        {
            get { return display_filter_tx_color; }
            set
            {
                display_filter_tx_color = value;
            }
        }

        private static bool draw_tx_filter = false;
        public static bool DrawTXFilter
        {
            get { return draw_tx_filter; }
            set
            {
                draw_tx_filter = value;
            }
        }

        private static bool draw_tx_cw_freq = false;
        public static bool DrawTXCWFreq
        {
            get { return draw_tx_cw_freq; }
            set
            {
                draw_tx_cw_freq = value;
            }
        }

        private static Color display_background_color = Color.Black;
        public static Color DisplayBackgroundColor
        {
            get { return display_background_color; }
            set
            {
                display_background_color = value;
            }
        }

        private static Color waterfall_low_color = Color.Black;
        public static Color WaterfallLowColor
        {
            get { return waterfall_low_color; }
            set { waterfall_low_color = value; }
        }

        private static Color waterfall_mid_color = Color.Red;
        public static Color WaterfallMidColor
        {
            get { return waterfall_mid_color; }
            set { waterfall_mid_color = value; }
        }

        private static Color waterfall_high_color = Color.Yellow;
        public static Color WaterfallHighColor
        {
            get { return waterfall_high_color; }
            set { waterfall_high_color = value; }
        }

        private static float waterfall_high_threshold = -80.0F;
        public static float WaterfallHighThreshold
        {
            get { return waterfall_high_threshold; }
            set { waterfall_high_threshold = value; }
        }

        private static float waterfall_low_threshold = -110.0F;
        public static float WaterfallLowThreshold
        {
            get { return waterfall_low_threshold; }
            set { waterfall_low_threshold = value; }
        }

        private static float display_line_width = 1.0F;
        public static float DisplayLineWidth
        {
            get { return display_line_width; }
            set
            {
                display_line_width = value;
                data_line_pen = new Pen(new SolidBrush(data_line_color), display_line_width);
            }
        }

        private static DisplayLabelAlignment display_label_align = DisplayLabelAlignment.LEFT;

        public static DisplayLabelAlignment DisplayLabelAlign
        {
            get { return display_label_align; }
            set
            {
                display_label_align = value;
            }
        }

        #endregion

        #region General Routines

        public static void Init()               // changes yt7pwr
        {
            try
            {
                histogram_data = new int[W];
                histogram_history = new int[W];
                for (int i = 0; i < W; i++)
                {
                    histogram_data[i] = Int32.MaxValue;
                    histogram_history[i] = 0;
                }

                if (waterfall_bmp != null)
                    waterfall_bmp.Dispose();

                if (panadapter_bmp != null)
                    panadapter_bmp.Dispose();

                if (panadapter_img != "")
                {
                    try
                    {
                        panadapter_bmp = new Bitmap(System.Drawing.Image.FromFile(panadapter_img, true));	// initialize panadapter display
                    }
                    catch (Exception ex)
                    {
                        panadapter_bmp = new Bitmap(W, H, PixelFormat.Format24bppRgb);	                    // initialize panadapter display
                        String Message = " Init " + ex.ToString();
                        Write_Debug_Message(Message);
                        Debug.Write(ex.ToString());
                    }
                }
                else
                    panadapter_bmp = new Bitmap(W, H, PixelFormat.Format24bppRgb);	                    // initialize paterfall display

                waterfall_bmp = new Bitmap(W, H, PixelFormat.Format24bppRgb);	                        // initialize waterfall display
                average_buffer = new float[BUFFER_SIZE];	                                            // initialize averaging buffer array
                average_buffer[0] = CLEAR_FLAG;		                                                    // set the clear flag
                average_waterfall_buffer = new float[BUFFER_SIZE];	                                    // initialize averaging buffer array
                average_waterfall_buffer[0] = CLEAR_FLAG;		                                        // set the clear flag
                peak_buffer = new float[BUFFER_SIZE];
                peak_buffer[0] = CLEAR_FLAG;
                //scope_min = new float[W];
                //scope_max = new float[W];
                new_display_data = new float[BUFFER_SIZE];
                new_scope_data = new float[BUFFER_SIZE];
                new_waterfall_data = new float[BUFFER_SIZE];
                current_display_data = new float[BUFFER_SIZE];
                current_scope_data = new float[BUFFER_SIZE];
                current_waterfall_data = new float[BUFFER_SIZE];
                waterfall_display_data = new float[BUFFER_SIZE];

                for (int i = 0; i < BUFFER_SIZE; i++)
                {
                    new_display_data[i] = -200.0f;
                    new_scope_data[i] = -200.0f;
                    new_waterfall_data[i] = -200.0f;
                    current_display_data[i] = -200.0f;
                    current_scope_data[i] = -200.0f;
                    current_waterfall_data[i] = -200.0f;
                    waterfall_display_data[i] = -200.0f;
                }

                if (display_data_mutex == null)
                    display_data_mutex = new Mutex();

                server_display_data = new byte[server_W];
                client_display_data = new byte[client_W];
            }
            catch (Exception ex)
            {
                String Message = " Init " + ex.ToString();
                Write_Debug_Message(Message);
                Debug.Write(ex.ToString());
            }
        }

        public static bool Close() // yt7pwr
        {
            bool status = false;
            try
            {
                histogram_data = null;
                histogram_history = null;

                if (waterfall_bmp != null)
                    waterfall_bmp.Dispose();

                average_buffer = null;
                average_waterfall_buffer = null;
                peak_buffer = null;
                //scope_min = null;
                //scope_max = null;
                new_display_data = null;
                new_scope_data = null;
                new_waterfall_data = null;
                current_display_data = null;
                current_scope_data = null;
                current_waterfall_data = null;
                waterfall_display_data = null;
                if (display_data_mutex != null)
                    display_data_mutex = null;
                server_display_data = null;
                client_display_data = null;
            }
            catch (Exception ex)
            {
                String Message = " Close " + ex.ToString();
                Write_Debug_Message(Message);
                Debug.Write(ex.ToString());
            }
            status = true;
            return status;
        }

        #endregion

        #region GDI+

        /*private static void UpdateDisplayPeak()
        {
            if (peak_buffer[0] == CLEAR_FLAG)
            {
                for (int i = 0; i < BUFFER_SIZE; i++)
                    peak_buffer[i] = current_display_data[i];
            }
            else
            {
                for (int i = 0; i < BUFFER_SIZE; i++)
                {
                    if (current_display_data[i] > peak_buffer[i])
                        peak_buffer[i] = current_display_data[i];
                    current_display_data[i] = peak_buffer[i];
                }
            }
        }*/

        #endregion

        #region Drawing Routines

        public static int center_line_x = 415;
        public static int filter_left_x = 150;
        public static int filter_right_x = 2550;

        public static void Write_Debug_Message(String Message)
        {
            var thisForm2 = Application.OpenForms.OfType<frmGUI>().Single();
            thisForm2.Write_Message(Message);
        }

        private static float[] waterfall_data;

        unsafe static bool DrawWaterfall(Graphics g, int W, int H)  // changes yt7pwr
        {
            String Message;
            try
            {
                //Write_Debug_Message(" DrawWaterfall -> Called");
                if (waterfall_data == null || waterfall_data.Length < W)
                {
                    waterfall_data = new float[W];
                    Write_Debug_Message(" DrawWaterfall - > waterfall_data created");// array of points to display
                }
                int Low = 0;
                int High = 0;
                int i = 0;
                Low = rx_display_low;
                High = rx_display_high;
                max_y = Int32.MinValue;
                int R = 0, G = 0, B = 0;
                int pixel_size = 3;
                byte* row = null;
                int seconds = 0;
                int my_x0 = 5;
                int my_y0 = 0;
                int my_x1 = waterfall_bmp.Width;
                int my_y1 = 0;

                System.Drawing.Font font = new System.Drawing.Font("Arial", 8);
                SolidBrush grid_text_brush = new SolidBrush(Color.White);

                if (waterfall_data_ready)
                {
                    //Write_Debug_Message(" DrawWaterfall - > waterfall_data_ready");
                    fixed (void* rptr = &new_waterfall_data[0]) //This is a global public variable
                    fixed (void* wptr = &current_waterfall_data[0])
                        Win32.memcpy(wptr, rptr, W * sizeof(float));
                    //Write_Debug_Message(" DrawWaterfall - > waterfall_data_ready - Finished");

                }

                fixed (void* dptr = &waterfall_data[0])
                fixed (void* cptr = &current_waterfall_data[0])
                    Win32.memcpy(dptr, cptr, (W * sizeof(float)));

                if (Window_controls.Waterfall_Controls.Time_grid != 0)
                {
                    using (Graphics gr = Graphics.FromImage(waterfall_bmp))
                    {
                        gr.SmoothingMode = SmoothingMode.AntiAlias;
                        Pen thick_pen = new Pen(Color.White, 1);
                        seconds = DateTime.Now.Second;
                        if (Window_controls.Waterfall_Controls.seconds != seconds)
                        {
                            if ((seconds % Window_controls.Waterfall_Controls.Time_grid) == 0)
                            {
                                if (!reverse_waterfall)
                                {
                                    my_x0 = 0;
                                    my_y0 = 0;
                                    my_x1 = waterfall_bmp.Width;
                                    my_y1 = 0;
                                }
                                else
                                {
                                    my_x0 = 0;
                                    my_y0 = waterfall_bmp.Height - 12;
                                    my_x1 = waterfall_bmp.Width;
                                    my_y1 = waterfall_bmp.Height - 12;

                                    Message = " Reverse -> x0: " + Convert.ToString(my_x0) +
                                        " y0: " + Convert.ToString(my_y0) + " x1: " + Convert.ToString(my_x1) +
                                        " y1: " + Convert.ToString(my_y1);
                                    //Write_Debug_Message(Message);
                                }
                                gr.DrawLine(thick_pen, (my_x0 + 30), (my_y0 + 10), (my_x1), (my_y1 + 10));
                                gr.DrawString(DateTime.UtcNow.ToString("mm:ss"), font, grid_text_brush, my_x0, my_y0);

                                Window_controls.Waterfall_Controls.seconds = seconds;
                            }
                        }
                    }
                }

                Rectangle rect = new Rectangle(0, 0, waterfall_bmp.Width, waterfall_bmp.Height);
                BitmapData bitmapData = waterfall_bmp.LockBits(rect, ImageLockMode.ReadWrite, waterfall_bmp.PixelFormat);
                //BitmapData bitmapData = waterfall_bmp.LockBits(
                //        new Rectangle(0, 0, waterfall_bmp.Width, waterfall_bmp.Height),
                //        ImageLockMode.ReadWrite,
                //        waterfall_bmp.PixelFormat);

                Message = " bitmap -> Width: " + Convert.ToString(waterfall_bmp.Width) + " Height: " +
                    Convert.ToString(waterfall_bmp.Height);
                //Write_Debug_Message(Message);

                if (reverse_waterfall)
                {
                    try
                    {
                        // first scroll image up
                        int total_size = bitmapData.Stride * bitmapData.Height;     // find buffer size
                        Win32.memcpy(bitmapData.Scan0.ToPointer(),
                            new IntPtr((int)bitmapData.Scan0 + bitmapData.Stride).ToPointer(),
                            total_size - bitmapData.Stride);
                        row = (byte*)(bitmapData.Scan0.ToInt32() + total_size - bitmapData.Stride);
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.ToString());
                        Message = " DrawWaterfallDisplay -> Reverse: " + ex.ToString();
                        Write_Debug_Message(Message);
                    }
                }
                else
                {
                    // first scroll image down
                    int total_size = bitmapData.Stride * bitmapData.Height;     // find buffer size
                    Win32.memcpy(new IntPtr((int)bitmapData.Scan0 + bitmapData.Stride).ToPointer(),
                        bitmapData.Scan0.ToPointer(),
                        total_size - bitmapData.Stride);

                    Message = " bitmap -> Stride: " + Convert.ToString(bitmapData.Stride) + " Height: " +
                        Convert.ToString(bitmapData.Height) + " Total Size: " + Convert.ToString(total_size);
                    //Write_Debug_Message(Message);
                    row = (byte*)(bitmapData.Scan0.ToInt32());
                    //Write_Debug_Message(" Normal(2)");
                }

                i = 0;


                switch (color_sheme)
                {
                    case (ColorSheme.original):                        // tre color only
                        {
                            // draw new data
                            for (i = 0; i < W; i++) // for each pixel in the new line
                            {
                                Message = " Data: " + Convert.ToString(waterfall_data[i]) +
                                      " Low Threadhold: " + Convert.ToString(waterfall_low_threshold) +
                                      " High Threahold: " + Convert.ToString(waterfall_high_threshold) +
                                      " i: " + Convert.ToString(i) + " W: " + Convert.ToString(W);
                                //Write_Debug_Message(Message);
                                if (waterfall_data[i] <= waterfall_low_threshold) // if less than low threshold,
                                {                                                 // just use low color
                                    R = WaterfallLowColor.R;
                                    G = WaterfallLowColor.G;
                                    B = WaterfallLowColor.B;
                                    Message = " low threadhold Data: " + Convert.ToString(waterfall_data[i]) +
                                        " Threadhold: " + Convert.ToString(waterfall_low_threshold);
                                    //Write_Debug_Message(Message);
                                }
                                else
                                    if (waterfall_data[i] >= waterfall_high_threshold)   // if more than high threshold, 
                                {                                                           //just use high color
                                    R = WaterfallHighColor.R;
                                    G = WaterfallHighColor.G;
                                    B = WaterfallHighColor.B;
                                    Message = " high threadhold Data: " + Convert.ToString(waterfall_data[i]) +
                                        " Threadhold: " + Convert.ToString(waterfall_high_threshold);
                                    //Write_Debug_Message(Message);
                                }
                                else
                                    if (waterfall_data[i] > waterfall_low_threshold && waterfall_data[i] <
                                    waterfall_high_threshold) // use a color between high and low
                                {

                                    float percent = (waterfall_data[i] - waterfall_low_threshold) /
                                        (waterfall_high_threshold - waterfall_low_threshold);
                                    if (percent <= 0.5) // use a gradient between low and mid colors
                                    {
                                        percent *= 2;

                                        R = (int)((1 - percent) * WaterfallLowColor.R + percent * WaterfallMidColor.R);
                                        G = (int)((1 - percent) * WaterfallLowColor.G + percent * WaterfallMidColor.G);
                                        B = (int)((1 - percent) * WaterfallLowColor.B + percent * WaterfallMidColor.B);
                                    }
                                    else                // use a gradient between mid and high colors
                                    {
                                        percent = (float)(percent - 0.5) * 2;

                                        R = (int)((1 - percent) * WaterfallMidColor.R + percent * WaterfallHighColor.R);
                                        G = (int)((1 - percent) * WaterfallMidColor.G + percent * WaterfallHighColor.G);
                                        B = (int)((1 - percent) * WaterfallMidColor.B + percent * WaterfallHighColor.B);
                                    }
                                    Message = " mid threadhold Data: " + Convert.ToString(waterfall_data[i]) + " R: " +
                                        Convert.ToString(R) + " G: " + Convert.ToString(G) + " B" + Convert.ToString(B);
                                    //Write_Debug_Message(Message);

                                }
                                Message = "Pixel Size(1): " + Convert.ToString(pixel_size);
                                //Write_Debug_Message(Message);
                                // set pixel color
                                row[i * pixel_size + 0] = (byte)B;  // set color in memory
                                row[i * pixel_size + 1] = (byte)G;
                                row[i * pixel_size + 2] = (byte)R;
                                Message = "Pixel Size(2): " + Convert.ToString(pixel_size);
                                //Write_Debug_Message(Message);
                            }
                        }
                        break;

                    case (ColorSheme.enhanced):
                        {
                            // draw new data
                            for (i = 0; i < W; i++) // for each pixel in the new line
                            {
                                if (waterfall_data[i] <= waterfall_low_threshold)
                                {
                                    R = WaterfallLowColor.R;
                                    G = WaterfallLowColor.G;
                                    B = WaterfallLowColor.B;
                                }
                                else if (waterfall_data[i] >= WaterfallHighThreshold)
                                {
                                    R = 192;
                                    G = 124;
                                    B = 255;
                                }
                                else // value is between low and high
                                {
                                    float range = WaterfallHighThreshold - waterfall_low_threshold;
                                    float offset = waterfall_data[i] - waterfall_low_threshold;
                                    float overall_percent = offset / range; // value from 0.0 to 1.0 where 1.0 is high and 0.0 is low.

                                    if (overall_percent < (float)2 / 9) // background to blue
                                    {
                                        float local_percent = overall_percent / ((float)2 / 9);
                                        R = (int)((1.0 - local_percent) * WaterfallLowColor.R);
                                        G = (int)((1.0 - local_percent) * WaterfallLowColor.G);
                                        B = (int)(WaterfallLowColor.B + local_percent * (255 - WaterfallLowColor.B));
                                    }
                                    else if (overall_percent < (float)3 / 9) // blue to blue-green
                                    {
                                        float local_percent = (overall_percent - (float)2 / 9) / ((float)1 / 9);
                                        R = 0;
                                        G = (int)(local_percent * 255);
                                        B = 255;
                                    }
                                    else if (overall_percent < (float)4 / 9) // blue-green to green
                                    {
                                        float local_percent = (overall_percent - (float)3 / 9) / ((float)1 / 9);
                                        R = 0;
                                        G = 255;
                                        B = (int)((1.0 - local_percent) * 255);
                                    }
                                    else if (overall_percent < (float)5 / 9) // green to red-green
                                    {
                                        float local_percent = (overall_percent - (float)4 / 9) / ((float)1 / 9);
                                        R = (int)(local_percent * 255);
                                        G = 255;
                                        B = 0;
                                    }
                                    else if (overall_percent < (float)7 / 9) // red-green to red
                                    {
                                        float local_percent = (overall_percent - (float)5 / 9) / ((float)2 / 9);
                                        R = 255;
                                        G = (int)((1.0 - local_percent) * 255);
                                        B = 0;
                                    }
                                    else if (overall_percent < (float)8 / 9) // red to red-blue
                                    {
                                        float local_percent = (overall_percent - (float)7 / 9) / ((float)1 / 9);
                                        R = 255;
                                        G = 0;
                                        B = (int)(local_percent * 255);
                                    }
                                    else // red-blue to purple end
                                    {
                                        float local_percent = (overall_percent - (float)8 / 9) / ((float)1 / 9);
                                        R = (int)((0.75 + 0.25 * (1.0 - local_percent)) * 255);
                                        G = (int)(local_percent * 255 * 0.5);
                                        B = 255;
                                    }
                                }

                                // set pixel color
                                row[i * pixel_size + 0] = (byte)B;  // set color in memory
                                row[i * pixel_size + 1] = (byte)G;
                                row[i * pixel_size + 2] = (byte)R;
                            }
                        }
                        break;

                    case (ColorSheme.SPECTRAN):
                        {
                            // draw new data
                            for (i = 0; i < W; i++) // for each pixel in the new line
                            {
                                if (waterfall_data[i] <= waterfall_low_threshold)
                                {
                                    R = 0;
                                    G = 0;
                                    B = 0;
                                }
                                else if (waterfall_data[i] >= WaterfallHighThreshold) // white
                                {
                                    R = 240;
                                    G = 240;
                                    B = 240;
                                }
                                else // value is between low and high
                                {
                                    float range = WaterfallHighThreshold - waterfall_low_threshold;
                                    float offset = waterfall_data[i] - waterfall_low_threshold;
                                    float local_percent = ((100.0f * offset) / range);

                                    if (local_percent < 5.0f)
                                    {
                                        R = G = 0;
                                        B = (int)local_percent * 5;
                                    }
                                    else if (local_percent < 11.0f)
                                    {
                                        R = G = 0;
                                        B = (int)local_percent * 5;
                                    }
                                    else if (local_percent < 22.0f)
                                    {
                                        R = G = 0;
                                        B = (int)local_percent * 5;
                                    }
                                    else if (local_percent < 44.0f)
                                    {
                                        R = G = 0;
                                        B = (int)local_percent * 5;
                                    }
                                    else if (local_percent < 51.0f)
                                    {
                                        R = G = 0;
                                        B = (int)local_percent * 5;
                                    }
                                    else if (local_percent < 66.0f)
                                    {
                                        R = G = (int)(local_percent - 50) * 2;
                                        B = 255;
                                    }
                                    else if (local_percent < 77.0f)
                                    {
                                        R = G = (int)(local_percent - 50) * 3;
                                        B = 255;
                                    }
                                    else if (local_percent < 88.0f)
                                    {
                                        R = G = (int)(local_percent - 50) * 4;
                                        B = 255;
                                    }
                                    else if (local_percent < 99.0f)
                                    {
                                        R = G = (int)(local_percent - 50) * 5;
                                        B = 255;
                                    }
                                }

                                // set pixel color
                                row[i * pixel_size + 0] = (byte)B;  // set color in memory
                                row[i * pixel_size + 1] = (byte)G;
                                row[i * pixel_size + 2] = (byte)R;
                            }
                        }
                        break;

                    case (ColorSheme.BLACKWHITE):
                        {
                            // draw new data
                            for (i = 0; i < W; i++) // for each pixel in the new line
                            {
                                if (waterfall_data[i] <= waterfall_low_threshold)
                                {
                                    R = 0;
                                    G = 0;
                                    B = 0;
                                }
                                else if (waterfall_data[i] >= WaterfallHighThreshold) // white
                                {
                                    R = 255;
                                    G = 255;
                                    B = 255;
                                }
                                else // value is between low and high
                                {
                                    float range = WaterfallHighThreshold - waterfall_low_threshold;
                                    float offset = waterfall_data[i] - waterfall_low_threshold;
                                    float overall_percent = offset / range; // value from 0.0 to 1.0 where 1.0 is high and 0.0 is low.
                                    float local_percent = ((100.0f * offset) / range);
                                    //float contrast = (console.SetupForm.DisplayContrast / 100);
                                    R = (int)((local_percent / 100) * 255);
                                    G = R;
                                    B = R;
                                }

                                // set pixel color
                                row[i * pixel_size + 0] = (byte)B;  // set color in memory
                                row[i * pixel_size + 1] = (byte)G;
                                row[i * pixel_size + 2] = (byte)R;
                            }
                        }
                        break;
                }

                Message = " Unlock Start";
                //Write_Debug_Message(Message);
                waterfall_bmp.UnlockBits(bitmapData);
                Message = " Unlock Stop";
                //Write_Debug_Message(Message);

                if (current_display_mode == DisplayMode.WATERFALL)
                {
                    Message = " Drawing Image Waterfall";
                    //Write_Debug_Message(Message);
                    g.DrawImage(waterfall_bmp, 0, 0);  // draw the image on the background	
                }
                else
                {
                    Message = " Drawing Image";
                    //Write_Debug_Message(Message);
                    g.DrawImage(waterfall_bmp, 0, 0);
                }
                waterfall_data_ready = false;
                //Write_Debug_Message(" DrawWaterfallDisplay -> Finished");
                return true;
            }
            catch (Exception ex)
            {
                Message = " DrawWaterfallDisplay -> " + ex.ToString();
                Write_Debug_Message(Message);
                Debug.Write(ex.ToString());
                return false;
            }
        }
        public static void Waterfall_Callback()
        {
            bool waterfall_state = false;
            int Y = 0;
            int pan_buff_count = 0;
            int pan_buff_limit = Panadapter_Controls.Max_X * 2;
            int water_fall_count = 0;
            bool seq_0 = false;
            bool seq_1 = false;
            bool waterfall_status = false;
            String Message;

            Write_Debug_Message(" Waterfall_Callback Started");

            unsafe bool DrawWaterfall(Graphics g, int W, int H)  // changes yt7pwr
            {
                try
                {
                    //Write_Debug_Message(" DrawWaterfall -> Called");
                    if (waterfall_data == null || waterfall_data.Length < W)
                    {
                        waterfall_data = new float[W];
                        Write_Debug_Message(" DrawWaterfall - > waterfall_data created");// array of points to display
                    }
                    int Low = 0;
                    int High = 0;
                    int i = 0;
                    Low = rx_display_low;
                    High = rx_display_high;
                    max_y = Int32.MinValue;
                    int R = 0, G = 0, B = 0;
                    int pixel_size = 3;
                    byte* row = null;
                    int seconds = 0;
                    int my_x0 = 5;
                    int my_y0 = 0;
                    int my_x1 = waterfall_bmp.Width;
                    int my_y1 = 0;

                    System.Drawing.Font font = new System.Drawing.Font("Arial", 8);
                    SolidBrush grid_text_brush = new SolidBrush(Color.White);

                    if (waterfall_data_ready)
                    {
                        //Write_Debug_Message(" DrawWaterfall - > waterfall_data_ready");
                        fixed (void* rptr = &new_waterfall_data[0]) //This is a global public variable
                        fixed (void* wptr = &current_waterfall_data[0])
                            Win32.memcpy(wptr, rptr, W * sizeof(float));
                        //Write_Debug_Message(" DrawWaterfall - > waterfall_data_ready - Finished");

                    }

                    fixed (void* dptr = &waterfall_data[0])
                    fixed (void* cptr = &current_waterfall_data[0])
                        Win32.memcpy(dptr, cptr, (W * sizeof(float)));

                    if (Window_controls.Waterfall_Controls.Time_grid != 0)
                    {
                        using (Graphics gr = Graphics.FromImage(waterfall_bmp))
                        {
                            gr.SmoothingMode = SmoothingMode.AntiAlias;
                            Pen thick_pen = new Pen(Color.White, 1);
                            seconds = DateTime.Now.Second;
                            if (Window_controls.Waterfall_Controls.seconds != seconds)
                            {
                                if ((seconds % Window_controls.Waterfall_Controls.Time_grid) == 0)
                                {
                                    if (!reverse_waterfall)
                                    {
                                        my_x0 = 0;
                                        my_y0 = 0;
                                        my_x1 = waterfall_bmp.Width;
                                        my_y1 = 0;
                                    }
                                    else
                                    {
                                        my_x0 = 0;
                                        my_y0 = waterfall_bmp.Height - 12;
                                        my_x1 = waterfall_bmp.Width;
                                        my_y1 = waterfall_bmp.Height - 12;

                                        Message = " Reverse -> x0: " + Convert.ToString(my_x0) +
                                            " y0: " + Convert.ToString(my_y0) + " x1: " + Convert.ToString(my_x1) +
                                            " y1: " + Convert.ToString(my_y1);
                                        //Write_Debug_Message(Message);
                                    }
                                    gr.DrawLine(thick_pen, (my_x0 + 30), (my_y0 + 10), (my_x1), (my_y1 + 10));
                                    gr.DrawString(DateTime.UtcNow.ToString("mm:ss"), font, grid_text_brush, my_x0, my_y0);

                                    Window_controls.Waterfall_Controls.seconds = seconds;
                                }
                            }
                        }
                    }

                    Rectangle rect = new Rectangle(0, 0, waterfall_bmp.Width, waterfall_bmp.Height);
                    BitmapData bitmapData = waterfall_bmp.LockBits(rect, ImageLockMode.ReadWrite, waterfall_bmp.PixelFormat);
                    //BitmapData bitmapData = waterfall_bmp.LockBits(
                    //        new Rectangle(0, 0, waterfall_bmp.Width, waterfall_bmp.Height),
                    //        ImageLockMode.ReadWrite,
                    //        waterfall_bmp.PixelFormat);

                    Message = " bitmap -> Width: " + Convert.ToString(waterfall_bmp.Width) + " Height: " +
                        Convert.ToString(waterfall_bmp.Height);
                    //Write_Debug_Message(Message);

                    if (reverse_waterfall)
                    {
                        try
                        {
                            // first scroll image up
                            int total_size = bitmapData.Stride * bitmapData.Height;     // find buffer size
                            Win32.memcpy(bitmapData.Scan0.ToPointer(),
                                new IntPtr((int)bitmapData.Scan0 + bitmapData.Stride).ToPointer(),
                                total_size - bitmapData.Stride);
                            row = (byte*)(bitmapData.Scan0.ToInt32() + total_size - bitmapData.Stride);
                        }
                        catch (Exception ex)
                        {
                            Debug.Write(ex.ToString());
                            Message = " DrawWaterfallDisplay -> Reverse: " + ex.ToString();
                            Write_Debug_Message(Message);
                        }
                    }
                    else
                    {
                        // first scroll image down
                        int total_size = bitmapData.Stride * bitmapData.Height;     // find buffer size
                        Win32.memcpy(new IntPtr((int)bitmapData.Scan0 + bitmapData.Stride).ToPointer(),
                            bitmapData.Scan0.ToPointer(),
                            total_size - bitmapData.Stride);

                        Message = " bitmap -> Stride: " + Convert.ToString(bitmapData.Stride) + " Height: " +
                            Convert.ToString(bitmapData.Height) + " Total Size: " + Convert.ToString(total_size);
                        //Write_Debug_Message(Message);
                        row = (byte*)(bitmapData.Scan0.ToInt32());
                        //Write_Debug_Message(" Normal(2)");
                    }

                    i = 0;


                    switch (color_sheme)
                    {
                        case (ColorSheme.original):                        // tre color only
                            {
                                // draw new data
                                for (i = 0; i < W; i++) // for each pixel in the new line
                                {
                                    Message = " Data: " + Convert.ToString(waterfall_data[i]) +
                                          " Low Threadhold: " + Convert.ToString(waterfall_low_threshold) +
                                          " High Threahold: " + Convert.ToString(waterfall_high_threshold) +
                                          " i: " + Convert.ToString(i) + " W: " + Convert.ToString(W);
                                    //Write_Debug_Message(Message);
                                    if (waterfall_data[i] <= waterfall_low_threshold) // if less than low threshold,
                                    {                                                 // just use low color
                                        R = WaterfallLowColor.R;
                                        G = WaterfallLowColor.G;
                                        B = WaterfallLowColor.B;
                                        Message = " low threadhold Data: " + Convert.ToString(waterfall_data[i]) +
                                            " Threadhold: " + Convert.ToString(waterfall_low_threshold);
                                        //Write_Debug_Message(Message);
                                    }
                                    else
                                        if (waterfall_data[i] >= waterfall_high_threshold)   // if more than high threshold, 
                                    {                                                           //just use high color
                                        R = WaterfallHighColor.R;
                                        G = WaterfallHighColor.G;
                                        B = WaterfallHighColor.B;
                                        Message = " high threadhold Data: " + Convert.ToString(waterfall_data[i]) +
                                            " Threadhold: " + Convert.ToString(waterfall_high_threshold);
                                        //Write_Debug_Message(Message);
                                    }
                                    else
                                        if (waterfall_data[i] > waterfall_low_threshold && waterfall_data[i] <
                                        waterfall_high_threshold) // use a color between high and low
                                    {

                                        float percent = (waterfall_data[i] - waterfall_low_threshold) /
                                            (waterfall_high_threshold - waterfall_low_threshold);
                                        if (percent <= 0.5) // use a gradient between low and mid colors
                                        {
                                            percent *= 2;

                                            R = (int)((1 - percent) * WaterfallLowColor.R + percent * WaterfallMidColor.R);
                                            G = (int)((1 - percent) * WaterfallLowColor.G + percent * WaterfallMidColor.G);
                                            B = (int)((1 - percent) * WaterfallLowColor.B + percent * WaterfallMidColor.B);
                                        }
                                        else                // use a gradient between mid and high colors
                                        {
                                            percent = (float)(percent - 0.5) * 2;

                                            R = (int)((1 - percent) * WaterfallMidColor.R + percent * WaterfallHighColor.R);
                                            G = (int)((1 - percent) * WaterfallMidColor.G + percent * WaterfallHighColor.G);
                                            B = (int)((1 - percent) * WaterfallMidColor.B + percent * WaterfallHighColor.B);
                                        }
                                        Message = " mid threadhold Data: " + Convert.ToString(waterfall_data[i]) + " R: " +
                                            Convert.ToString(R) + " G: " + Convert.ToString(G) + " B" + Convert.ToString(B);
                                        //Write_Debug_Message(Message);

                                    }
                                    Message = "Pixel Size(1): " + Convert.ToString(pixel_size);
                                    //Write_Debug_Message(Message);
                                    // set pixel color
                                    row[i * pixel_size + 0] = (byte)B;  // set color in memory
                                    row[i * pixel_size + 1] = (byte)G;
                                    row[i * pixel_size + 2] = (byte)R;
                                    Message = "Pixel Size(2): " + Convert.ToString(pixel_size);
                                    //Write_Debug_Message(Message);
                                }
                            }
                            break;

                        case (ColorSheme.enhanced):
                            {
                                // draw new data
                                for (i = 0; i < W; i++) // for each pixel in the new line
                                {
                                    if (waterfall_data[i] <= waterfall_low_threshold)
                                    {
                                        R = WaterfallLowColor.R;
                                        G = WaterfallLowColor.G;
                                        B = WaterfallLowColor.B;
                                    }
                                    else if (waterfall_data[i] >= WaterfallHighThreshold)
                                    {
                                        R = 192;
                                        G = 124;
                                        B = 255;
                                    }
                                    else // value is between low and high
                                    {
                                        float range = WaterfallHighThreshold - waterfall_low_threshold;
                                        float offset = waterfall_data[i] - waterfall_low_threshold;
                                        float overall_percent = offset / range; // value from 0.0 to 1.0 where 1.0 is high and 0.0 is low.

                                        if (overall_percent < (float)2 / 9) // background to blue
                                        {
                                            float local_percent = overall_percent / ((float)2 / 9);
                                            R = (int)((1.0 - local_percent) * WaterfallLowColor.R);
                                            G = (int)((1.0 - local_percent) * WaterfallLowColor.G);
                                            B = (int)(WaterfallLowColor.B + local_percent * (255 - WaterfallLowColor.B));
                                        }
                                        else if (overall_percent < (float)3 / 9) // blue to blue-green
                                        {
                                            float local_percent = (overall_percent - (float)2 / 9) / ((float)1 / 9);
                                            R = 0;
                                            G = (int)(local_percent * 255);
                                            B = 255;
                                        }
                                        else if (overall_percent < (float)4 / 9) // blue-green to green
                                        {
                                            float local_percent = (overall_percent - (float)3 / 9) / ((float)1 / 9);
                                            R = 0;
                                            G = 255;
                                            B = (int)((1.0 - local_percent) * 255);
                                        }
                                        else if (overall_percent < (float)5 / 9) // green to red-green
                                        {
                                            float local_percent = (overall_percent - (float)4 / 9) / ((float)1 / 9);
                                            R = (int)(local_percent * 255);
                                            G = 255;
                                            B = 0;
                                        }
                                        else if (overall_percent < (float)7 / 9) // red-green to red
                                        {
                                            float local_percent = (overall_percent - (float)5 / 9) / ((float)2 / 9);
                                            R = 255;
                                            G = (int)((1.0 - local_percent) * 255);
                                            B = 0;
                                        }
                                        else if (overall_percent < (float)8 / 9) // red to red-blue
                                        {
                                            float local_percent = (overall_percent - (float)7 / 9) / ((float)1 / 9);
                                            R = 255;
                                            G = 0;
                                            B = (int)(local_percent * 255);
                                        }
                                        else // red-blue to purple end
                                        {
                                            float local_percent = (overall_percent - (float)8 / 9) / ((float)1 / 9);
                                            R = (int)((0.75 + 0.25 * (1.0 - local_percent)) * 255);
                                            G = (int)(local_percent * 255 * 0.5);
                                            B = 255;
                                        }
                                    }

                                    // set pixel color
                                    row[i * pixel_size + 0] = (byte)B;  // set color in memory
                                    row[i * pixel_size + 1] = (byte)G;
                                    row[i * pixel_size + 2] = (byte)R;
                                }
                            }
                            break;

                        case (ColorSheme.SPECTRAN):
                            {
                                // draw new data
                                for (i = 0; i < W; i++) // for each pixel in the new line
                                {
                                    if (waterfall_data[i] <= waterfall_low_threshold)
                                    {
                                        R = 0;
                                        G = 0;
                                        B = 0;
                                    }
                                    else if (waterfall_data[i] >= WaterfallHighThreshold) // white
                                    {
                                        R = 240;
                                        G = 240;
                                        B = 240;
                                    }
                                    else // value is between low and high
                                    {
                                        float range = WaterfallHighThreshold - waterfall_low_threshold;
                                        float offset = waterfall_data[i] - waterfall_low_threshold;
                                        float local_percent = ((100.0f * offset) / range);

                                        if (local_percent < 5.0f)
                                        {
                                            R = G = 0;
                                            B = (int)local_percent * 5;
                                        }
                                        else if (local_percent < 11.0f)
                                        {
                                            R = G = 0;
                                            B = (int)local_percent * 5;
                                        }
                                        else if (local_percent < 22.0f)
                                        {
                                            R = G = 0;
                                            B = (int)local_percent * 5;
                                        }
                                        else if (local_percent < 44.0f)
                                        {
                                            R = G = 0;
                                            B = (int)local_percent * 5;
                                        }
                                        else if (local_percent < 51.0f)
                                        {
                                            R = G = 0;
                                            B = (int)local_percent * 5;
                                        }
                                        else if (local_percent < 66.0f)
                                        {
                                            R = G = (int)(local_percent - 50) * 2;
                                            B = 255;
                                        }
                                        else if (local_percent < 77.0f)
                                        {
                                            R = G = (int)(local_percent - 50) * 3;
                                            B = 255;
                                        }
                                        else if (local_percent < 88.0f)
                                        {
                                            R = G = (int)(local_percent - 50) * 4;
                                            B = 255;
                                        }
                                        else if (local_percent < 99.0f)
                                        {
                                            R = G = (int)(local_percent - 50) * 5;
                                            B = 255;
                                        }
                                    }

                                    // set pixel color
                                    row[i * pixel_size + 0] = (byte)B;  // set color in memory
                                    row[i * pixel_size + 1] = (byte)G;
                                    row[i * pixel_size + 2] = (byte)R;
                                }
                            }
                            break;

                        case (ColorSheme.BLACKWHITE):
                            {
                                // draw new data
                                for (i = 0; i < W; i++) // for each pixel in the new line
                                {
                                    if (waterfall_data[i] <= waterfall_low_threshold)
                                    {
                                        R = 0;
                                        G = 0;
                                        B = 0;
                                    }
                                    else if (waterfall_data[i] >= WaterfallHighThreshold) // white
                                    {
                                        R = 255;
                                        G = 255;
                                        B = 255;
                                    }
                                    else // value is between low and high
                                    {
                                        float range = WaterfallHighThreshold - waterfall_low_threshold;
                                        float offset = waterfall_data[i] - waterfall_low_threshold;
                                        float overall_percent = offset / range; // value from 0.0 to 1.0 where 1.0 is high and 0.0 is low.
                                        float local_percent = ((100.0f * offset) / range);
                                        //float contrast = (console.SetupForm.DisplayContrast / 100);
                                        R = (int)((local_percent / 100) * 255);
                                        G = R;
                                        B = R;
                                    }

                                    // set pixel color
                                    row[i * pixel_size + 0] = (byte)B;  // set color in memory
                                    row[i * pixel_size + 1] = (byte)G;
                                    row[i * pixel_size + 2] = (byte)R;
                                }
                            }
                            break;
                    }

                    Message = " Unlock Start";
                    //Write_Debug_Message(Message);
                    waterfall_bmp.UnlockBits(bitmapData);
                    Message = " Unlock Stop";
                    //Write_Debug_Message(Message);

                    if (current_display_mode == DisplayMode.WATERFALL)
                    {
                        Message = " Drawing Image Waterfall";
                        //Write_Debug_Message(Message);
                        g.DrawImage(waterfall_bmp, 0, 0);  // draw the image on the background	
                    }
                    else
                    {
                        Message = " Drawing Image";
                        //Write_Debug_Message(Message);
                        g.DrawImage(waterfall_bmp, 0, 0);
                    }
                    waterfall_data_ready = false;
                    //Write_Debug_Message(" DrawWaterfallDisplay -> Finished");
                    return true;
                }
                catch (Exception ex)
                {
                    Message = " DrawWaterfallDisplay -> " + ex.ToString();
                    Write_Debug_Message(Message);
                    Debug.Write(ex.ToString());
                    return false;
                }
            }
            while (true)
            {
                try
                {
                    if (Waterfall_form.waterfall_run)
                    {
                        Waterfall_form.waterfall_operation_complete = false;
                        waterfall_state = Display_GDI.WaterfallDataReady;
                        seq_0 = Panadapter_Controls.Sequence_0_Complete;
                        seq_1 = Panadapter_Controls.Sequence_1_Complete;
                        waterfall_status = waterfall_state;
                        if (Panadapter_Controls.Sequence_0_Complete && Panadapter_Controls.Sequence_1_Complete &&
                            waterfall_state == false &&
                            Window_controls.Waterfall_Controls.Waterfall_window.WindowState != FormWindowState.Minimized &&
                            Master_Controls.Transmit_Mode == false)

                        {
                            Display_GDI.WaterfallDataReady = false;
                            water_fall_count = 0;
                            Message = " Waterfall_Callback Sequence: " + Convert.ToString(Panadapter_Controls.Sequence_1_Complete) +
                                " Waterfall State: " + Convert.ToString(waterfall_state);
                            //Write_Debug_Message(Message);
                            //Draw_filter();
                            for (pan_buff_count = 0; pan_buff_count < pan_buff_limit; pan_buff_count++)
                            {
                                Y = ((Window_controls.Waterfall_Controls.Display_Buffer.Y_value[pan_buff_count]));
                                if (Y <= Waterfall_form.low_thread_hold)
                                {
                                    Y = Y + Waterfall_form.zero_gain;
                                }
                                if ((pan_buff_count % 20) == 0)
                                {
                                    Display_GDI.new_waterfall_data[water_fall_count] = Y;
                                    Message = " Modulous applied: " + Convert.ToString(pan_buff_count);
                                    //Write_Debug_Message(Message);
                                }
                                else
                                {
                                    Display_GDI.new_waterfall_data[water_fall_count++] = Y;
                                    Message = " Modulous not applied";
                                    //Write_Debug_Message(Message);
                                }

                            }
                            Display_GDI.WaterfallDataReady = true;
                            DrawWaterfall(Window_controls.Waterfall_Controls.Pic_Waterfall_graphics, Waterfall_form.pic_window_size.Width, H);
                            Waterfall_form.waterfall_operation_complete = true;
                        }
                        else
                        {
                            if (Window_controls.Waterfall_Controls.Waterfall_window.WindowState != FormWindowState.Minimized)
                            {
                                if (Master_Controls.Transmit_Mode == false)
                                {
                                    Message = " Waterfall_Callback -> No Data Available. Seq0: " +
                                        Convert.ToString(seq_0) + " Seq1: " +
                                        Convert.ToString(seq_1) + " Waterfall State: " +
                                        Convert.ToString(waterfall_status);
                                    Write_Debug_Message(Message);
                                }
                            }
                        }
                        Message = " Form size -> W: " + Convert.ToString(Waterfall_form.pic_window_size.Width) + " H: " +
                            Convert.ToString(Waterfall_form.pic_window_size.Height);
                        //Write_Debug_Message(Message);

                    }
                    else
                    {
                        Waterfall_form.waterfall_operation_complete = true;
                    }
                }
                catch (Exception ex)
                {
                    Message = " Waterfall_Callback -> " + ex.ToString();
                    Write_Debug_Message(Message);
                }
                //Write_Debug_Message(" Waterfall_Callback -> Finished");
                Thread.Sleep(Window_controls.Waterfall_Controls.window_speed);
            }
        }



        /*public static void DrawWaterfallGrid(ref Graphics g, int W, int H)  // changes yt7pwr
        {
            int low = rx_display_low;					// initialize variables
            int high = rx_display_high;
            int mid_w = W / 2;
            int[] step_list = { 10, 20, 25, 50 };
            int step_power = 1;
            int step_index = 0;
            int freq_step_size = 25;

            System.Drawing.Font font = new System.Drawing.Font("Arial", 9);
            SolidBrush grid_text_brush = new SolidBrush(grid_text_color);
            Pen grid_pen = new Pen(grid_color);
            Pen tx_filter_pen = new Pen(display_filter_tx_color);
            int y_range = spectrum_grid_max - spectrum_grid_min;
            int filter_low, filter_high;

            int center_line_x = (int)(-(double)low / (high - low) * W);
            //filter_low = DttSP.RXFilterLowCut;
            //filter_high = DttSP.RXFilterHighCut;
            filter_low = 2200;
            filter_high = 2500;
            // Calculate horizontal step size
            int width = high - low;
            while (width / freq_step_size > 10)
            {
                freq_step_size = step_list[step_index] * (int)Math.Pow(10.0, step_power);
                step_index = (step_index + 1) % 4;
                if (step_index == 0) step_power++;
            }
            double w_pixel_step = (double)W * freq_step_size / width;
            int w_steps = width / freq_step_size;

            // calculate vertical step size
            int h_steps = (spectrum_grid_max - spectrum_grid_min) / spectrum_grid_step;
            double h_pixel_step = (double)H / h_steps;
            int top = (int)((double)spectrum_grid_step * H / y_range);
            int filter_left_x = (int)((float)(filter_low - low + vfob_hz - losc_hz) / (high - low) * W);
            int filter_right_x = (int)((float)(filter_high - low + vfob_hz - losc_hz) / (high - low) * W);

            // make the filter display at least one pixel wide.
            if (filter_left_x == filter_right_x) filter_right_x = filter_left_x + 1;

            // draw rx filter
            g.FillRectangle(new SolidBrush(sub_rx_filter_color),	// draw filter overlay
            filter_left_x, 0, filter_right_x - filter_left_x, top);

            // draw Sub RX 0Hz line
            int x = (int)((float)(vfob_hz - losc_hz - low) / (high - low) * W);
            g.DrawLine(new Pen(sub_rx_zero_line_color), x, 0, x, top);
            g.DrawLine(new Pen(sub_rx_zero_line_color), x - 1, 0, x - 1, top);
            // draw RX filter
            // get filter screen coordinates
            filter_left_x = (int)((float)(-low - ((filter_high - filter_low) / 2) + vfoa_hz - losc_hz) / (high - low) * W);
            filter_right_x = (int)((float)(-low + ((filter_high - filter_low) / 2) + vfoa_hz - losc_hz) / (high - low) * W);

            // make the filter display at least one pixel wide.
            if (filter_left_x == filter_right_x) filter_right_x = filter_left_x + 1;

            // draw rx filter
            g.FillRectangle(new SolidBrush(main_rx_filter_color),   // draw filter overlay
                filter_left_x, 0, filter_right_x - filter_left_x, top);

            // draw Sub RX 0Hz line
            x = (filter_right_x - filter_left_x) / 2;
            x += filter_left_x;
            g.DrawLine(new Pen(main_rx_zero_line_color), x, 0, x, top);
            g.DrawLine(new Pen(main_rx_zero_line_color), x - 1, 0, x - 1, top);
            double vfo;
            vfo = losc_hz; // +rit_hz;
            long vfo_round = ((long)(vfo / freq_step_size)) * freq_step_size;
            long vfo_delta = (long)(vfo - vfo_round);

            // Draw vertical lines
            for (int i = 0; i <= h_steps + 1; i++)
            {
                string label;
                int offsetL;

                int fgrid = i * freq_step_size + (low / freq_step_size) * freq_step_size;
                double actual_fgrid = ((double)(vfo_round + fgrid)) / 1000000;
                int vgrid = (int)((double)(fgrid - vfo_delta - low) / (high - low) * W);

                if (actual_fgrid == 1.8 || actual_fgrid == 2.0 ||
                    actual_fgrid == 3.5 || actual_fgrid == 4.0 ||
                    actual_fgrid == 7.0 || actual_fgrid == 7.3 ||
                    actual_fgrid == 10.1 || actual_fgrid == 10.15 ||
                    actual_fgrid == 14.0 || actual_fgrid == 14.35 ||
                    actual_fgrid == 18.068 || actual_fgrid == 18.168 ||
                    actual_fgrid == 21.0 || actual_fgrid == 21.45 ||
                    actual_fgrid == 24.89 || actual_fgrid == 24.99 ||
                    actual_fgrid == 21.0 || actual_fgrid == 21.45 ||
                    actual_fgrid == 28.0 || actual_fgrid == 29.7 ||
                    actual_fgrid == 50.0 || actual_fgrid == 54.0 ||
                    actual_fgrid == 144.0 || actual_fgrid == 148.0)
                {
                    g.DrawLine(new Pen(band_edge_color), vgrid, top, vgrid, H);

                    label = actual_fgrid.ToString("f3");
                    if (actual_fgrid < 10) offsetL = (int)((label.Length + 1) * 4.1) - 14;
                    else if (actual_fgrid < 100.0) offsetL = (int)((label.Length + 1) * 4.1) - 11;
                    else offsetL = (int)((label.Length + 1) * 4.1) - 8;

                    g.DrawString(label, font, new SolidBrush(band_edge_color), vgrid - offsetL, (float)Math.Floor(H * .01));
                }
                else
                {

                    if (freq_step_size >= 2000)
                    {
                        double t100;
                        double t1000;
                        t100 = (actual_fgrid * 100);
                        t1000 = (actual_fgrid * 1000);

                        int it100 = (int)t100;
                        int it1000 = (int)t1000;

                        int it100x10 = it100 * 10;

                        if (it100x10 == it1000)
                        {
                        }
                        else
                        {
                            grid_pen.DashStyle = DashStyle.Dot;
                        }
                    }
                    else
                    {
                        if (freq_step_size == 1000)
                        {
                            double t200;
                            double t2000;
                            t200 = (actual_fgrid * 200);
                            t2000 = (actual_fgrid * 2000);

                            int it200 = (int)t200;
                            int it2000 = (int)t2000;

                            int it200x10 = it200 * 10;

                            if (it200x10 == it2000)
                            {
                            }
                            else
                            {
                                grid_pen.DashStyle = DashStyle.Dot;
                            }
                        }
                        else
                        {
                            double t1000;
                            double t10000;
                            t1000 = (actual_fgrid * 1000);
                            t10000 = (actual_fgrid * 10000);

                            int it1000 = (int)t1000;
                            int it10000 = (int)t10000;

                            int it1000x10 = it1000 * 10;

                            if (it1000x10 == it10000)
                            {
                            }
                            else
                            {
                                grid_pen.DashStyle = DashStyle.Dot;
                            }
                        }
                    }
                    //g.DrawLine(grid_pen, vgrid, top, vgrid, H);			//wa6ahl
                    grid_pen.DashStyle = DashStyle.Solid;

                    if (((double)((int)(actual_fgrid * 1000))) == actual_fgrid * 1000)
                    {
                        label = actual_fgrid.ToString("f3"); //wa6ahl

                        //if(actual_fgrid > 1300.0)
                        //	label = label.Substring(label.Length-4);

                        if (actual_fgrid < 10) offsetL = (int)((label.Length + 1) * 4.1) - 14;
                        else if (actual_fgrid < 100.0) offsetL = (int)((label.Length + 1) * 4.1) - 11;
                        else offsetL = (int)((label.Length + 1) * 4.1) - 8;
                    }
                    else
                    {
                        string temp_string;
                        int jper;
                        label = actual_fgrid.ToString("f4");
                        temp_string = label;
                        jper = label.IndexOf('.') + 4;
                        label = label.Insert(jper, " ");

                        //if(actual_fgrid > 1300.0)
                        //	label = label.Substring(label.Length-4);

                        if (actual_fgrid < 10) offsetL = (int)((label.Length) * 4.1) - 14;
                        else if (actual_fgrid < 100.0) offsetL = (int)((label.Length) * 4.1) - 11;
                        else offsetL = (int)((label.Length) * 4.1) - 8;
                    }

                    g.DrawString(label, font, grid_text_brush, vgrid - offsetL, (float)Math.Floor(H * .01));
                }
            }

            int[] band_edge_list = { 135700, 137800, 415000, 525000, 18068000, 18168000, 1800000, 2000000, 3500000, 4000000,
                                       7000000, 7300000, 10100000, 10150000, 14000000, 14350000, 21000000, 21450000,
                                       24890000, 24990000, 28000000, 29700000, 50000000, 54000000, 144000000, 148000000 };

            for (int i = 0; i < band_edge_list.Length; i++)
            {
                double band_edge_offset = band_edge_list[i] - losc_hz;
                if (band_edge_offset >= low && band_edge_offset <= high)
                {
                    int temp_vline = (int)((double)(band_edge_offset - low) / (high - low) * W);//wa6ahl
                    g.DrawLine(new Pen(band_edge_color), temp_vline, 0, temp_vline, top);//wa6ahl
                }
                if (i == 1 && !show_freq_offset) break;
            }

            // Draw 0Hz vertical line if visible
            if (center_line_x >= 0 && center_line_x <= W)
            {
                g.DrawLine(new Pen(grid_zero_color), center_line_x, 0, center_line_x, top);
                g.DrawLine(new Pen(grid_zero_color), center_line_x - 1, 0, center_line_x - 1, top);
            }
        }*/

        /*public static void ResetDisplayAverage()
        {
            try
            {
                average_buffer[0] = CLEAR_FLAG;	// set reset flag
                average_waterfall_buffer[0] = CLEAR_FLAG;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }*/

        /*private static float[] scope_min;
        public static float[] ScopeMin
        {
            get { return scope_min; }
            set { scope_min = value; }
        }

        private static float[] scope_max;
        public static float[] ScopeMax
        {
            get { return scope_max; }
            set { scope_max = value; }
        }*/

        /*private static bool UpdateDisplayWaterfallAverage()
        {
            try
            {
                String Message = " UpdateDisplayWaterfallAverage Starting";
                Write_Debug_Message(Message);
                double dttsp_osc = (losc_hz - vfoa_hz) * 1e6;
                // Debug.WriteLine("last vfo: " + avg_last_ddsfreq + " vfo: " + DDSFreq); 
                if (Display_GDI.average_waterfall_buffer[0] == Display_GDI.CLEAR_FLAG)
                {
                    //Debug.WriteLine("Clearing average buf"); 
                    for (int i = 0; i < Display_GDI.BUFFER_SIZE; i++)
                        Display_GDI.average_waterfall_buffer[i] = Display_GDI.current_waterfall_data[i];
                }

                float new_mult = 0.0f;
                float old_mult = 0.0f;

                new_mult = Display_GDI.waterfall_avg_mult_new;
                old_mult = Display_GDI.waterfall_avg_mult_old;

                for (int i = 0; i < Display_GDI.BUFFER_SIZE; i++)
                    Display_GDI.average_waterfall_buffer[i] = Display_GDI.current_waterfall_data[i] =
                        (float)(Display_GDI.current_waterfall_data[i] * new_mult +
                        Display_GDI.average_waterfall_buffer[i] * old_mult);

                if (Display_GDI.average_waterfall_buffer[0] == Display_GDI.CLEAR_FLAG)
                {
                    avg_last_ddsfreq = 0;
                    avg_last_dttsp_osc = 0;
                }
                else
                {
                    avg_last_ddsfreq = losc_hz;
                    avg_last_dttsp_osc = dttsp_osc;
                }

                return true;
            }
            catch (Exception ex)
            {
                String Message = " UpdateDisplayWaterfallAverage -> " + ex.ToString();
                Write_Debug_Message(Message);
                Debug.Write(ex.ToString());
                return false;
            }
        }*/

        #endregion

    }
}
//=================================================================
// console.cs
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
 *  Changes for GenesisRadio
 *  Copyright (C)2008-2013 YT7PWR Goran Radivojevic
 *  contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
*/

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Windows.Forms;
//using SDRSerialSupportII;
//using Splash_Screen;
//using GenesisG59;
//using GenesisG11;
//using GenesisNetBox;
//using GenesisG6;

#if DirectX
using SlimDX;
#endif

namespace OmniaGUI
{
    #region Enums

    public enum ColorSheme
    {
        original = 0,
        enhanced,
        SPECTRAN,
        BLACKWHITE,
        off,
    }

    public enum DisplayMode
    {
        FIRST = -1,
        SPECTRUM,
        PANADAPTER,
        SCOPE,
        PHASE,
        PHASE2,
        WATERFALL,
        HISTOGRAM,
        PANAFALL,
        PANAFALL_INV,
        PANASCOPE,
        OFF,
        LAST,
    }

    public enum DSPMode
    {
        FIRST = -1,
        LSB,
        USB,
        DSB,
        CWL,
        CWU,
        FMN,
        AM,
        DIGU,
        SPEC,
        DIGL,
        SAM,
        DRM,
        WFM,
        LAST,
    }

    public enum DisplayLabelAlignment
    {
        FIRST = -1,
        LEFT,
        CENTER,
        RIGHT,
        AUTO,
        OFF,
        LAST,
    }

    public enum ClickTuneMode
    {
        Off = 0,
        VFOA,
        VFOB,
    }

    #endregion

    #region structures

    [StructLayout(LayoutKind.Sequential)]
    public struct XBand
    {
        public double freq_min;
        public double freq_max;
        public double losc;
        public double pa_gain;
        public double pa_pwr;
        public bool rx_only;
        public string name;
    }

    #endregion

    unsafe public partial class Console : System.Windows.Forms.Form
    {
        const string GSDR_revision = "  v2.0.16";
      
#if(WIN32)
        const string GSDR_version = " 32bit";
#endif
#if(WIN64)
        const string GSDR_version = "  64bit";
#endif

        #region DLL import

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr window, int message, int wparam, int lparam);

        #endregion

        #region Variable Declarations

        #region Genesis variable

        public bool CalibrationInProgress = false;
    
        public bool booting = false;
      
        public System.OperatingSystem OSInfo = System.Environment.OSVersion;
     
        //private delegate void GDICallbackFunction(string name);
      
        //public XBand[] xBand = new XBand[13];

        #endregion

        #endregion

        #region Windows Form Generated Code

        public System.Windows.Forms.PictureBox picDisplay;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Console
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Console";
            this.Load += new System.EventHandler(this.Console_Load);
            this.ResumeLayout(false);

        }

        private void Console_Load(object sender, EventArgs e)
        {

        }
    }
}


#endregion
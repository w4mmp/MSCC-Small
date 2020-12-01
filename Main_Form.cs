using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using OmniaGUI.Properties;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Drawing.Text;
using System.Diagnostics.Tracing;
using ZedGraph;
using VU_MeterLibrary;
using System.Net.WebSockets;
//using Renci.SshNet;

//using System.Speech.Synthesis;

#if FTDI

using FTD2XX_NET;

#endif

namespace OmniaGUI
{
    public partial class Main_form : Form
    {
        public IPEndPoint txtarget;
        public Socket txsocket;

        private delegate void MonitorTextCallback(string text);

        public String Log_Path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        //public static String Log_Path = AppDomain.CurrentDomain.BaseDirectory;
        public static String Log_file;

        //public String Sound_Path = AppDomain.CurrentDomain.BaseDirectory;
        public MonitorForm Monitor_window;

        //public Panadapter_1 Panadapter_window_new;
        //public Smeter_Form1 Smeter_window;

        public shutdown Shutdown_window;
        //public static Thread Main_Smeter_thread = new Thread(new ThreadStart(Main_Smeter));
        //public static frmGUI Local_Smeter_form = new frmGUI();
        public static String Sound_Path = AppDomain.CurrentDomain.BaseDirectory + "\\sounds\\overdriven.wav";
        private System.Media.SoundPlayer player = new System.Media.SoundPlayer(Sound_Path);
        public static String Initializer_path = AppDomain.CurrentDomain.BaseDirectory + "\\initialize.exe";
        public static String Multus_mscc_ini_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static String Multus_client_run_path;
        public static bool frmGui_called = false;
        public static Size Main_tab_size = new Size(0, 0);
        public static Size Full_size = new Size(0, 0);
        public static Point RPI_location = new Point(0, 4);


        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        public static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)]
                                     string lpFileName);
        public static String Font_Path = AppDomain.CurrentDomain.BaseDirectory + "/timesbi.ttf";

        // public static SpeechSynthesizer Voice_output = new SpeechSynthesizer();

        private String[] MFC_arr = new string[7];

        public static Band_Stack_Controls.band_stack[] Band_Stack = new Band_Stack_Controls.band_stack[3];

        private FormWindowState LastWindowState = FormWindowState.Maximized;

        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MINIMIZE = 0xF020;
        private Size main_window_maximum = new Size(800, 480);
        private Size main_window_minimum = new Size(800, 39);

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case WM_SYSCOMMAND:
                    int command = m.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MINIMIZE)
                    {
                        Settings.Default.Docked = false;
                    }
                    // If you don't want to do the default action then break
                    break;
            }
        }

        [DefaultValue(typeof(Color), "Gainsboro")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        public Main_form()
        {
            frmGui_called = true;
            InitializeComponent();
            this.BackColor = Color.Gainsboro;
            this.DoubleBuffered = true;

#if RPI 
            Full_size = this.Size;
            Full_size.Height = 462;
            Main_tab_size = this.Size;
            Main_tab_size.Height = 314;
            
#endif

            /*String Log_directory = Log_Path += "\\multus-sdr-client-logs";
            if (!Directory.Exists(Log_directory))
            {
                DirectoryInfo di = Directory.CreateDirectory(Log_directory);
                Thread.Sleep(3000);
            }*/

            if (Monitor_window == null)
            {
                Monitor_window = new MonitorForm();
                //SetTextBoxText(  " Monitor Popup Created" );
                Window_controls.monitor_form_displosed = false;
            }
            ToolTip toolTip1 = new ToolTip();
            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 32000;
            toolTip1.InitialDelay = 1;
            toolTip1.ReshowDelay = 250;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = false;
            toolTip1.SetToolTip(this.powertunebutton1, "Turns on TX");
            toolTip1.AutoPopDelay = 3200;
            toolTip1.SetToolTip(this.mainmodebutton2, "Click to change operating mode");
        }

#if FTDI
       
        public void Open_Relay_Board()
        {
            Relay_Board_Controls.Status = Relay_Board_Controls.Device.OpenByIndex(0);
            if (Relay_Board_Controls.Status != FTDI.FT_STATUS.FT_OK)
            {
                Relay_Board_Controls.Open = false;
                Relay_Board_Controls.Device.Close();
                MonitorTextBoxText(" Relay Board Open FAILED");
            }
            else
            {
                Relay_Board_Controls.Open = true;
                Relay_Board_Controls.Device.ResetDevice();
                Relay_Board_Controls.Device.SetBitMode(0xFF, FTD2XX_NET.FTDI.FT_BIT_MODES.FT_BIT_MODE_ASYNC_BITBANG);
                MonitorTextBoxText(" Relay Board Open Success");
            }
        }

#endif
        public void LogWrite(string logMessage)
        {
            try
            {
                using (StreamWriter w = File.AppendText(Log_file))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception)
            {
                Master_Controls.Log_retry_count++;
                if (Master_Controls.Log_retry_count > 10)
                {
                    Application.Exit();
                    MessageBox.Show("LogWrite -> Log File Write FAILED", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                txtWriter.WriteLine("  :{0}", logMessage);
                txtWriter.WriteLine("-------------------------------");
            }
            catch (Exception)
            {
                Application.Exit();
                MessageBox.Show("Log -> Log File Write FAILED", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        public void MonitorTextBoxText(string text)
        {
            String Message_text;

            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.

            if (this.Monitor_window.Monitortxt.InvokeRequired)
            {
                MonitorTextCallback d = new MonitorTextCallback(MonitorTextBoxText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                Message_text = Convert.ToString(oCode.line_count++) + text;
                if (oCode.monitor_suspend == false) Monitor_window.Monitortxt.AppendText(Message_text + "\r\n");
                LogWrite(Message_text);
            }
        }

        public void Write_Debug_Message(String Message)
        {
            var thisForm2 = Application.OpenForms.OfType<Main_form>().Single();
            thisForm2.Write_Message(Message);
        }

        /*private void Main_window_active(object sender, EventArgs e)
        {
           bool timer_status = true;
            timer_status = timer3.Enabled;
            if (timer_status == false)
            {
                if (Settings.Default.Docked)
                {
                    if (Window_controls.Panadapter_Controls.panadapter_window_created)
                    {
                        Window_controls.Panadapter_window_new.Focus();
                    }
                    if (Window_controls.Waterfall_Controls.window_created)
                    {
                        Window_controls.Waterfall_Controls.Waterfall_window.Focus();
                    }
                }
                this.Focus();
            }
        }*/

        private void Spectrum_Enter(object sender, EventArgs e)
        {
            MonitorTextBoxText(" Spectrum_Enter");
            Mouse_controls.Allow_Frequency_Updates = false;
            Zedgraph_Control.Focus();
        }

        private void Spectrum_Leave(object sender, EventArgs e)
        {
            MonitorTextBoxText(" Spectrum_Leave");
            Mouse_controls.Allow_Frequency_Updates = true;
            this.Focus();
        }

        private void Main_window_moved(object sender, EventArgs e)
        {
            Point delta_location = new Point(0, 0);
            //MonitorTextBoxText(" Main Window Moved. Called ");
            this.Focus();
            Window_controls.Docking_Controls.Main_location = this.Location;
            if (Settings.Default.Docked)
            {
                if (Window_controls.Docking_Controls.Main_location.X >= Window_controls.Docking_Controls.Previous_Main_location.X)
                {
                    delta_location.X = Window_controls.Docking_Controls.Main_location.X -
                        Window_controls.Docking_Controls.Previous_Main_location.X;

                }
                else
                {
                    delta_location.X = Window_controls.Docking_Controls.Previous_Main_location.X -
                        Window_controls.Docking_Controls.Main_location.X;


                }
                if (Window_controls.Docking_Controls.Main_location.Y >= Window_controls.Docking_Controls.Previous_Main_location.Y)
                {
                    delta_location.Y = Window_controls.Docking_Controls.Main_location.Y -
                        Window_controls.Docking_Controls.Previous_Main_location.Y;



                }
                else
                {
                    delta_location.Y = Window_controls.Docking_Controls.Previous_Main_location.Y -
                        Window_controls.Docking_Controls.Main_location.Y;

                }


            }
            Window_controls.Docking_Controls.Previous_Main_location = Window_controls.Docking_Controls.Main_location;
        }

        private void Frm_state(object sender, EventArgs e)
        {
            if (this.WindowState != LastWindowState)
            {
                LastWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Minimized)
                {
                    MonitorTextBoxText(" Frm_state Called State: " + this.WindowState);
                }
                else if (this.WindowState == FormWindowState.Normal)
                {
                    MonitorTextBoxText(" Frm_state Called State: " + this.WindowState);
                    if (Window_controls.Docking_Controls.Previous_Docked != Settings.Default.Docked)
                    {
                        Settings.Default.Docked = Window_controls.Docking_Controls.Previous_Docked;
                    }


                }
            }
            else
            {
                MonitorTextBoxText(" Frm_state Called State: " + this.WindowState);
                MonitorTextBoxText(" Main Window Resized: " + Convert.ToString(this.Size));
                Window_controls.Docking_Controls.Main_docked_size = this.Size;
            }
        }

        /*public void Initialize_Main_Comm_Port()
        {
            String path, line;
            System.IO.StreamReader file;
            String[] arr = new string[4];
            String value;
            //ListViewItem itm;
            String temp_string;
            int param_pos;
            int end_pos;
            short temp_int = 0;
            //string[] portNames;
            List<string> MyList = new List<string>();

            Master_Controls.code_triggered = true;
            //Get the master list of comm ports.
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path += "\\multus-sdr-client\\comm-port-master.ini";
            try
            {
                file = new System.IO.StreamReader(File.OpenRead(path));
            }
            catch (IOException e)
            {
                string er = e.Message;
                DialogResult ret = MessageBox.Show("IO Exception opening: " + path + "\r\n Error: " + er + 
                    "\r\nMake note of the error and contact Multus SDR,LLC.",
                    "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            while ((line = file.ReadLine()) != null)
            {
                param_pos = line.IndexOf("COMM_PORT_NAME");  // get the position of the COMM_PORT_NAME parameter
                temp_string = line.Substring((param_pos + 15), (line.Length - param_pos - 15));   // parse everything between the end of COMM_PORT_NAME= and the end of the line.
                end_pos = temp_string.IndexOf(";"); //temp string will start with the DEVICE_NAME parameter value. Find the trailing semi colon
                arr[2] = temp_string.Substring(0, end_pos);
                MyList.Add(arr[2]);
            }
            file.Close();
            cmbPortName.DataSource = MyList;
            Comm_Port_Controls.Box_indexes.Comm_Name_Index = 200;

            //Determine if a comm port is initialized.
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path += "\\multus-sdr-client\\comm-port.ini";
            try
            {
                file = new System.IO.StreamReader(File.OpenRead(path));
            }
            catch (IOException e)
            {
                string er = e.Message;
                MonitorTextBoxText(" Initialize_Main_Comm_Port -> comm-port.ini Error: " + er);
                return;
            }
            btnOpenPort.BackColor = Color.Red;
            btnOpenPort.ForeColor = Color.White;
            btnOpenPort.Text = "ACTIVE";
            btnOpenPort.FlatStyle = FlatStyle.Popup;
            Comm_Port_Controls.Comm_Port_Open = true;
            Comm_Port_Controls.Button_Toggle = true;
            while ((line = file.ReadLine()) != null)
            {
                param_pos = line.IndexOf("COMM_PORT_INDEX");  // get the position of the COMM_PORT_NAME parameter
                temp_string = line.Substring((param_pos + 16), (line.Length - param_pos - 16));   // parse everything between the end of COMM_PORT_NAME= and the end of the line.
                end_pos = temp_string.IndexOf(","); //temp string will start with the DEVICE_NAME parameter value. Find the trailing semi colon
                value = temp_string.Substring(0, end_pos);
                Int16.TryParse(value, out temp_int);
                cmbPortName.SelectedIndex = temp_int;
                param_pos = line.IndexOf("PIN");  // get the position of the COMM_PORT_NAME parameter
                temp_string = line.Substring((param_pos + 4), (line.Length - param_pos - 4));   // parse everything between the end of COMM_PORT_NAME= and the end of the line.
                end_pos = temp_string.IndexOf(";"); //temp string will start with the DEVICE_NAME parameter value. Find the trailing semi colon
                value = temp_string.Substring(0, end_pos);
                Int16.TryParse(value, out temp_int);
                Line_Signal_listBox1.SelectedIndex = temp_int;
            }
            file.Close();
            Master_Controls.code_triggered = false;
        }*/

        public void Initialize_HR50_Comm_Port()
        {
            String path, line;
            System.IO.StreamReader file;
            String[] arr = new string[4];
            //ListViewItem itm;
            String temp_string;
            int param_pos;
            int end_pos;
            //string[] portNames;
            List<string> MyList = new List<string>();

            Master_Controls.code_triggered = true;
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path += "\\multus-sdr-client\\comm-port-master.ini";
            try
            {
                file = new System.IO.StreamReader(File.OpenRead(path));
            }
            catch (IOException e)
            {
                string er = e.Message;
                DialogResult ret = MessageBox.Show("IO Exception opening: " + path + "\r\n Error: " + er +
                    "\r\nMake note of the error and contact Multus SDR,LLC.",
                    "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            while ((line = file.ReadLine()) != null)
            {
                param_pos = line.IndexOf("COMM_PORT_NAME");  // get the position of the COMM_PORT_NAME parameter
                temp_string = line.Substring((param_pos + 15), (line.Length - param_pos - 15));   // parse everything between the end of COMM_PORT_NAME= and the end of the line.
                end_pos = temp_string.IndexOf(";"); //temp string will start with the DEVICE_NAME parameter value. Find the trailing semi colon
                arr[2] = temp_string.Substring(0, end_pos);
                MyList.Add(arr[2]);
            }
            file.Close();


            //HR50_portNames = SerialPort.GetPortNames();
            //foreach (string port in HR50_portNames)
            //{
            //    HR50_List.Add(port);
            //}
            HR50_listBox1.DataSource = MyList;
            Comm_Port_Controls.HR50_Controls.Comm_Name_Index = 200;
            HR50_listBox1.SetSelected(0, false);
            Master_Controls.code_triggered = false;
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    MonitorTextBoxText(" GetLocalIPAddress: " + ip.ToString());
#if RPI
                    return ("127.0.1.1");
#else
                    return ip.ToString();
#endif
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string GetRemoteIPAddress()
        {
            IPAddress[] host = Dns.GetHostAddresses(oCode.iniStates.DLL_IP);
            try
            {

                foreach (IPAddress ipaddr in host)
                {
                    if (ipaddr.AddressFamily == AddressFamily.InterNetwork)
                    {
#if RPI
                        return ("127.0.1.1");
#else
                        return ipaddr.ToString();
#endif
                    }
                }
                throw new Exception("No network adapters with an IPv4 address in the system!");
            }
            catch (SocketException)
            {
                DialogResult ret = MessageBox.Show("SERVER HOST NAME NOT FOUND: " + oCode.iniStates.DLL_IP + "\r\n", "MSCC",
                                  MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            return "No Address Found";
        }

        public bool Setup_Receive_Connection()
        {
            bool status = true;
            string oput_string;
            bool myIP;
            string my_IP_address;
            System.Net.IPAddress mylocalIP;

            //SET UP THE UDP RECEIVE CONNECTION
            //SET Up the Socket
            if (oCode.iniStates.GUI_PORT == 0)
            {
                oCode.iniStates.GUI_PORT = 8889;
                MonitorTextBoxText(" Invalid MSCC Port. Using default: 8889");
            }
            else
            {
                oput_string = Convert.ToString(oCode.iniStates.GUI_PORT);
                MonitorTextBoxText(" MSCC Port Address: " + oput_string);
            }
            try
            {
                Udp_data.rxsocket = new UdpClient(oCode.iniStates.GUI_PORT);
                //Udp_data.rxsocket = new UdpClient()
            }
            catch (SocketException)
            {
                MonitorTextBoxText(" GUI Receive Socket Error");
                status = false;
            }
            catch (Exception)
            {
                MonitorTextBoxText(" GUI Receive Socket Error");
                status = false;
            }
            //Set up the IP Address
            try
            {
                my_IP_address = GetLocalIPAddress();
                myIP = IPAddress.TryParse(my_IP_address, out mylocalIP);
                //myIP = IPAddress.TryParse(oCode.iniStates.GUI_IP, out mylocalIP);
                if (myIP)
                {
                    Udp_data.rxtarget = new IPEndPoint(mylocalIP, oCode.iniStates.GUI_PORT);
                    MonitorTextBoxText(" MSCC IP ADDRESS: " + mylocalIP);
                }
                else
                {
                    MonitorTextBoxText(" MSCC IP ADDRESS FAILED " + oCode.iniStates.GUI_IP);
                    status = false;
                }
            }
            catch (Exception)
            {
                MonitorTextBoxText(" GUI IP: Endpoint Failure");
                status = false;
            }
            if (status != false)
            {
                Udp_data.rxsocket.BeginReceive(new AsyncCallback(OnUdpData), Udp_data.rxsocket);
            }
            return status;
        }

        public bool Initialize_Network()
        {
            bool status = true;
            string oput_string;
            bool setup_status = false;
            //bool myIP1;
            bool Init_file;
            string remote_ip_address;


            MonitorTextBoxText(" Initialize_Network Called");
            Init_file = oCode.getIniStates();
            if (!Init_file)
            {
                MonitorTextBoxText(" Initialization File READ Operation Failed. MSCC is terminating");
                MessageBox.Show("Initialization File READ Operation Failed. MSCC is terminating",
                        "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            //SET UP THE UDP RECEIVE CONNECTION
            setup_status = Setup_Receive_Connection();
            if (setup_status)
            {
                //NOW SET UP THE NETWORK FOR TRANSMITING UDP MS-SDR
                //Set up the Socket
                if (oCode.iniStates.DLL_PORT == 0)
                {
                    oCode.iniStates.DLL_PORT = 8888;
                    MonitorTextBoxText(" Invalid DLL Port. Using default: 8888");
                }
                else
                {
                    oput_string = Convert.ToString(oCode.iniStates.DLL_PORT);
                    MonitorTextBoxText(" DLL Port Address: " + oput_string);
                }
                txsocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                if (txsocket != null)
                {
#if RPI
                    remote_ip_address = GetLocalIPAddress();
#else
                    remote_ip_address = GetRemoteIPAddress();
#endif
                    MonitorTextBoxText(" Initialize_Network -> Remote Host: " + Convert.ToString(remote_ip_address));
                    Panadapter_Controls.txsocket = txsocket;
                    txtarget = new IPEndPoint(IPAddress.Parse(remote_ip_address), oCode.iniStates.DLL_PORT);
                    if (txtarget != null)
                    {
                        Panadapter_Controls.txtarget = txtarget;
                        MonitorTextBoxText(" Initialize_Network -> MS-SDR Target IP Address: " +
                            Convert.ToString(IPAddress.Parse(remote_ip_address)) + " Port: " +
                            Convert.ToString(oCode.iniStates.DLL_PORT));
                    }
                    else
                    {
                        MonitorTextBoxText(" Initialize_Network -> new IPEndPoint FAILED");
                        status = false;
                    }
                }
                else
                {
                    MonitorTextBoxText(" Initialize_Network -> new Socket FAILED");
                    status = false;
                }
            }

            return status;
        }

        public void Initialize_RIT()
        {
            ritlistBox1.SetSelected(Settings.Default.RIT_Step, true);
            switch (Settings.Default.RIT_Step)
            {
                case 0:
                    ritScroll.LargeChange = 10;
                    break;
                case 1:
                    ritScroll.LargeChange = 20;
                    break;
                case 2:
                    ritScroll.LargeChange = 30;
                    break;
                case 3:
                    ritScroll.LargeChange = 40;
                    break;
                case 4:
                    ritScroll.LargeChange = 50;
                    break;
            }
#if FTDI
            switch (Settings.Default.RIT_Step)
            {
                case 0:
                    Tuning_Knob_Controls.RIT_OFFSET_Function.RIT_Step = 10;
                    break;

                case 1:
                    Tuning_Knob_Controls.RIT_OFFSET_Function.RIT_Step = 20;
                    break;

                case 2:
                    Tuning_Knob_Controls.RIT_OFFSET_Function.RIT_Step = 30;
                    break;

                case 3:
                    Tuning_Knob_Controls.RIT_OFFSET_Function.RIT_Step = 40;
                    break;

                case 4:
                    Tuning_Knob_Controls.RIT_OFFSET_Function.RIT_Step = 50;
                    break;
            }
#endif
            ritoffsetlistBox1.SetSelected(Settings.Default.RIT_Limits, true);
            ritoffsetlistBox1.SelectedIndex = Settings.Default.RIT_Limits;
            Manage_Rit(ritScroll.LargeChange, ritoffsetlistBox1.SelectedIndex);
        }

        public void Initialize_Docking()
        {
            Window_controls.Docking_Controls.Previous_Docked = Settings.Default.Docked;
        }

        public void Get_FTP_Init_File()
        {
            int Initializer_exit_code;

            Multus_mscc_ini_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Multus_mscc_ini_path += "\\multus-sdr-client";
            System.IO.DirectoryInfo di = new DirectoryInfo(Multus_mscc_ini_path);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            Multus_client_run_path = AppDomain.CurrentDomain.BaseDirectory;
            Multus_client_run_path += "\\ftp.txt";
            if (File.Exists(Multus_client_run_path))
            {
                SDRprocesses.initializer = new Process();
                try
                {
                    SDRprocesses.initializer.StartInfo.UseShellExecute = false;
                    SDRprocesses.initializer.StartInfo.FileName = "cmd.exe";
                    SDRprocesses.initializer.StartInfo.CreateNoWindow = true;
                    SDRprocesses.initializer.StartInfo.Arguments = "/C ftp -s:ftp.txt";
                    SDRprocesses.initializer.Start();
                    SDRprocesses.initializer.WaitForExit(6000);
                    Initializer_exit_code = SDRprocesses.initializer.ExitCode;
                    if (Initializer_exit_code != 0)
                    {
                        timer3.Stop();
                        timer3.Enabled = false;
                        MonitorTextBoxText(" FTP did not run.  Exit Code: " + Convert.ToString(Initializer_exit_code));
                        DialogResult ret = MessageBox.Show("FTP Initializer FAILED\r\n", "MSCC",
                                   MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        Master_Controls.FTP_File_Found = false;

                        Application.Exit();
                    }
                }
                catch (Exception)
                {
                    timer3.Stop();
                    timer3.Enabled = false;
                    DialogResult ret = MessageBox.Show(" FTP New Process FAILED \r\n", "MSCC",
                                   MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    Master_Controls.FTP_File_Found = false;

                    Application.Exit();
                }
            }
            else
            {
                timer3.Stop();
                timer3.Enabled = false;
                MessageBox.Show(" FTP Configuration File - NOT FOUND", "MSCC",
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Master_Controls.FTP_File_Found = false;

                Application.Exit();
            }
            Multus_mscc_ini_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Multus_mscc_ini_path += "\\multus-sdr-client\\Multus_mscc.ini";
            if (!File.Exists(Multus_mscc_ini_path))
            {
                timer3.Stop();
                timer3.Enabled = false;
                Master_Controls.FTP_File_Found = false;
                DialogResult ret = MessageBox.Show("FTP Did NOT retrieve the initialization files. \r\n", "MSCC",
                                   MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                Application.Exit();
            }
        }

        public void Set_Font()
        {
            FontFamily[] fontFamilies;
            PrivateFontCollection My_fonts = new PrivateFontCollection();
            My_fonts.AddFontFile(Font_Path);

            fontFamilies = My_fonts.Families;
            Ones.ForeColor = (Settings.Default.Freq_Color);
            Tens.ForeColor = Settings.Default.Freq_Color;
            Hundreds.ForeColor = Settings.Default.Freq_Color;
            Thousands.ForeColor = Settings.Default.Freq_Color;
            Tenthousands.ForeColor = Settings.Default.Freq_Color;
            Hundredthousand.ForeColor = Settings.Default.Freq_Color;
            Millions.ForeColor = Settings.Default.Freq_Color;
            Tenmillions.ForeColor = Settings.Default.Freq_Color;
            Decimal_label58.ForeColor = Settings.Default.Freq_Color;
            Decimal_label59.ForeColor = Settings.Default.Freq_Color;
        }

        public void Set_Button_Color(bool active,Button mybutton)
        {
            switch (active)
            {
                case true:
                    mybutton.ForeColor = Color.White;
                    mybutton.BackColor = Color.Red;
                    break;
                default:
                    mybutton.ForeColor = Color.Black;
                    mybutton.BackColor = Color.Gainsboro;
                    break;
            }
        }

        private void RPi_Display_Timer_Tick(object sender, EventArgs e)
        {
#if RPI
            Manage_RPI_Controls();
#endif
        }

        public void Manage_RPI_Controls()
        {
            byte[] buf = new byte[2];

            if (RPi_Settings.Volume_Settings.Previous_Speaker_Volume != RPi_Settings.Volume_Settings.Speaker_Volume)
            {
                Volume_hScrollBar1.Value = RPi_Settings.Volume_Settings.Speaker_Volume;
                RPi_Settings.Volume_Settings.Previous_Speaker_Volume = RPi_Settings.Volume_Settings.Speaker_Volume;
                Volume_textBox2.Text = Convert.ToString(RPi_Settings.Volume_Settings.Speaker_Volume);
            }
            if(RPi_Settings.Controls.Previous_Freq_Step != RPi_Settings.Controls.Freq_Step)
            {
                mainlistBox1.SelectedIndex = RPi_Settings.Controls.Freq_Step;
                RPi_Settings.Controls.Previous_Freq_Step = RPi_Settings.Controls.Freq_Step;
            }
            if(RPi_Settings.Controls.Freq_Digit != oCode.FreqDigit)
            {
                buf[0] = Master_Controls.Extended_Commands.CMD_MFC_SET_ZERO;
                buf[1] = (byte)oCode.FreqDigit;
                oCode.SendCommand_MultiByte(txsocket, txtarget,Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, 
                                                                        buf, buf.Length);
                RPi_Settings.Controls.Freq_Digit = oCode.FreqDigit;
            }
            if(IQ_Controls.RPi.Calibration_Slider_Value != IQ_Controls.RPi.Previous_Calibration_Slider_Value)
            {
                IQBD_hScrollBar1.Value = IQ_Controls.RPi.Calibration_Slider_Value;
                IQ_Controls.RPi.Previous_Calibration_Slider_Value = IQ_Controls.RPi.Calibration_Slider_Value;
            }
            
        }

        public bool Update_RPi_Settings()
        {
            String path;
            System.IO.StreamWriter file;
            Int32 Background_argb;
            Int32 Text_argb;
            Int32 Boarder_argb;
            String Background_color;
            String Text_color;
            String Boarder_color;

            MonitorTextBoxText(" Update_RPi_Settings");
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#if RPI
            path += "/mscc/rpi-settings.ini";
#endif
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            try
            {
                //file = new System.IO.StreamReader(path);
                file = new System.IO.StreamWriter(File.OpenWrite(path));
            }

            // if the file open fails, whine prettily and return false
            catch (IOException e)
            {
                string er = e.Message;
                DialogResult ret = MessageBox.Show("rpi-settings.ini Open Failed: " + er,
                    "MSCC-Core", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            Background_argb = Settings.Default.Background_Color.ToArgb();
            Background_color = Background_argb.ToString();

            Text_argb = Settings.Default.Freq_Color.ToArgb();
            Text_color = Text_argb.ToString();

            Boarder_argb = Settings.Default.Boarder_Color.ToArgb();
            Boarder_color = Boarder_argb.ToString();

            file.WriteLine("BACKGROUND_COLOR=" + Background_color + ";");
            file.WriteLine("TEXT_COLOR=" + Text_color + ";");
            file.WriteLine("BOARDER_COLOR=" + Boarder_color + ";");
            file.WriteLine("PEAK_NEEDLE=" + Convert.ToString(RPi_Settings.Peak_Needle) + ";");
            file.WriteLine("PEAK_NEEDLE_DELAY_INDEX=" + Convert.ToString(RPi_Settings.Peak_Needle_Delay_Index) + ";");
            file.WriteLine("PEAK_NEEDLE_COLOR_INDEX=" + Convert.ToString(RPi_Settings.Peak_Needle_Color_Index) + ";");
            file.WriteLine("TIME_DISPLAY=" + Convert.ToString(RPi_Settings.Time_Display) + ";");
            file.WriteLine("METER_MODE=" + Convert.ToString(RPi_Settings.Meter_Mode) + ";");
            file.WriteLine("SPEAKER_ATTN=" + Convert.ToString(RPi_Settings.Volume_Settings.Volume_ATTN_Index) + ";");
            file.WriteLine("SPEAKER_VOLUME=" + Convert.ToString(RPi_Settings.Volume_Settings.Speaker_Volume) + ";");
            file.WriteLine("SPEAKER_MUTE=" + Convert.ToString(RPi_Settings.Volume_Settings.Speaker_Mute) + ";");
            file.WriteLine("MIC_MODE=" + Convert.ToString(RPi_Settings.Volume_Settings.Mic_Mode) + ";");
            file.WriteLine("MIC_VOLUME=" + Convert.ToString(RPi_Settings.Volume_Settings.Mic_Volume) + ";");
            file.WriteLine("MIC_MUTE=" + Convert.ToString(RPi_Settings.Volume_Settings.Mic_Mute) + ";");
            file.WriteLine("MIC_PRE_GAIN=" + Convert.ToString(RPi_Settings.Volume_Settings.Mic_Pre_Gain) + ";");
            file.WriteLine("ANTENNA_SWITCH=" + Convert.ToString(RPi_Settings.Controls.Antenna_Switch) + ";");

            file.Close();
            //RPi_Settings.RPi_Needs_Updated = false;
            return true;
        }

        public bool Init_CW_Settings()
        {
#if RPI 
            oCode.Get_CW_Params();
#endif
            CW_Hold_numericUpDown2.Value = CW_Parameters.CW_Tx_Hold;
            CW_Mode_listBox1.SelectedIndex = CW_Parameters.CW_Iambic_Mode_On_Off;
            CW_Type_listBox1.SelectedIndex = CW_Parameters.CW_Iambic_Type;
            CW_Space_listBox1.SelectedIndex = CW_Parameters.CW_Spacing;
            switch (CW_Parameters.CW_Weight)
            {
                case 25:
                    CW_Weight_listBox1.SelectedIndex = 0;
                    break;
                case 50:
                    CW_Weight_listBox1.SelectedIndex = 1;
                    break;
                case 75:
                    CW_Weight_listBox1.SelectedIndex = 2;
                    break;
            }
            CW_Paddle_listBox1.SelectedIndex = CW_Parameters.CW_Paddle;
            MonitorTextBoxText(" CW_Side_Tone_Volume: " + Convert.ToString(CW_Parameters.CW_Side_Tone_Volume));
            Side_Tone_Volume_hScrollBar1.Value = CW_Parameters.CW_Side_Tone_Volume;
            numericUpDown1.Value = CW_Parameters.CW_Speed;
            semicheckBox2.CheckedChanged -= semicheckBox2_CheckedChanged;
            MonitorTextBoxText(" CW_Semi_Break_In: " + Convert.ToString(CW_Parameters.CW_Semi_Break_In));
            switch (CW_Parameters.CW_Semi_Break_In)
            {
                case 0:
                    semicheckBox2.Checked = false;
                    break;
                case 1:
                    semicheckBox2.Checked = true;
                    break;
            }
            semicheckBox2.CheckedChanged += semicheckBox2_CheckedChanged;
            return true;
        }

        public bool Init_RPi_Settings()
        {
            String path, line;
            System.IO.StreamReader file;
            short temp_int = 0;
            Int32 temp_32 = 0;
            String temp_string;
            int line_count = 0;
            int volume_count = 0;
            byte[] buf = new byte[2];

            String Message = " Main_Form -> Init_RPi_Settings started";
            Write_Debug_Message(Message);      
            // get path to local Appdata folder
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            // add our folder and file name
#if RPI
            path += "/mscc/rpi-settings.ini";
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
                DialogResult ret = MessageBox.Show(" Open Failed: rpi-settings.ini" + er,
                    "MSCC-Core", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            while ((line = file.ReadLine()) != null)
            {
                line_count++;

                temp_string = getBetween(line, "ANTENNA_SWITCH=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    RPi_Settings.Controls.Antenna_Switch = temp_int;
                    Settings.Default.Antenna_Switch = (byte)temp_int;
                    Antenna_Switch_comboBox1.SelectedIndex = RPi_Settings.Controls.Antenna_Switch;
                    buf[0] = Master_Controls.Extended_Commands.CMD_SET_ANTENNA_SWITCH;
                    buf[1] = Set_Antenna_Switch_Value(temp_int);
                    oCode.SendCommand_MultiByte(txsocket, txtarget,
                          Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
                    Message = " ANTENNA_SWITCH: " + Convert.ToString(RPi_Settings.Controls.Antenna_Switch);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "SPEAKER_VOLUME=", ";");
                if (temp_string != "")
                {
                    volume_count++;
                    Int16.TryParse(temp_string, out temp_int);
                    RPi_Settings.Volume_Settings.Speaker_Volume = temp_int;
                    Volume_hScrollBar1.Value = RPi_Settings.Volume_Settings.Speaker_Volume;
                    Settings.Default.Speaker_Volume = RPi_Settings.Volume_Settings.Speaker_Volume;
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_VOLUME,
                                                        (short)RPi_Settings.Volume_Settings.Speaker_Volume);
                    Message = " SPEAKER_VOLUME: " + Convert.ToString(RPi_Settings.Volume_Settings.Speaker_Volume);
                    Write_Debug_Message(Message);
                    Message = " SPEAKER_VOLUME Count: " + Convert.ToString(volume_count);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "SPEAKER_ATTN=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    RPi_Settings.Volume_Settings.Volume_ATTN_Index = temp_int;
                    Settings.Default.Volume_Attn_Index = temp_int;
                    Volume_Attn_listBox1.SelectedIndex = RPi_Settings.Volume_Settings.Volume_ATTN_Index;
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_VOLUME_ATTN, 
                                                        (short)RPi_Settings.Volume_Settings.Volume_ATTN_Index);
                    Message = " SPEAKER_ATTN: " + Convert.ToString(RPi_Settings.Volume_Settings.Volume_ATTN_Index);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "MIC_VOLUME=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    RPi_Settings.Volume_Settings.Mic_Volume = temp_int;
                    MicVolume_hScrollBar1.Value = RPi_Settings.Volume_Settings.Mic_Volume;
                    Volume_Controls.Mic_Volume = (short)RPi_Settings.Volume_Settings.Mic_Volume;
                    switch (RPi_Settings.Volume_Settings.Mic_Mode)
                    {
                        case 1:
                            Settings.Default.Digital_Volume = Volume_Controls.Mic_Volume;
                            MicVolume_hScrollBar1.Value = Settings.Default.Digital_Volume;
                            Microphone_textBox2.Text = Convert.ToString(Settings.Default.Digital_Volume);
                            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_VOLUME, Settings.Default.Digital_Volume);
                            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_COMPRESSION_STATE, 0);
                            break;
                        default:
                            Settings.Default.Voice_Volume = Volume_Controls.Mic_Volume;
                            MicVolume_hScrollBar1.Value = Settings.Default.Voice_Volume;
                            Microphone_textBox2.Text = Convert.ToString(Settings.Default.Voice_Volume);
                            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_VOLUME, Settings.Default.Voice_Volume);
                            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_COMPRESSION_STATE,
                                Volume_Controls.Compression_State);
                            break;
                    }
                    Microphone_textBox2.Text = Convert.ToString(RPi_Settings.Volume_Settings.Mic_Volume);
                    Message = " MIC_VOLUME: " + Convert.ToString(RPi_Settings.Volume_Settings.Mic_Volume);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "MIC_MUTE=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    RPi_Settings.Volume_Settings.Mic_Mute = temp_int;
                    if(RPi_Settings.Volume_Settings.Mic_Mute == 1)
                    {
                        TX_Mute_button2.ForeColor = Color.Red;
                        TX_Mute_button2.Text = "MUTED";
                        Volume_Controls.Mic_MutED = true;
                        oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_MUTE, 1);
                    }
                    else
                    {
                        TX_Mute_button2.ForeColor = Color.Black;
                        TX_Mute_button2.Text = "Mic Gain";
                        oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_MUTE, 0);
                        Volume_Controls.Mic_MutED = false;
                    }
                    Message = " MIC_MUTE: " + Convert.ToString(RPi_Settings.Volume_Settings.Mic_Mute);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "MIC_PRE_GAIN=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    RPi_Settings.Volume_Settings.Mic_Pre_Gain = temp_int;
                    Mic_Gain_Step_listBox1.SelectedIndex = RPi_Settings.Volume_Settings.Mic_Pre_Gain;
                    Message = " MIC_PRE_GAIN: " + Convert.ToString(RPi_Settings.Volume_Settings.Mic_Pre_Gain);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "MIC_MODE=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    RPi_Settings.Volume_Settings.Mic_Mode = temp_int;
                    switch (RPi_Settings.Volume_Settings.Mic_Mode)
                    {
                        case 1:
                            Audio_Digital_button3.Text = "D";
                            Settings.Default.Mic_Is_Digital = true;
                            Compression_button4.Enabled = false;
                            Compression_button2.Enabled = false;
                         
                            break;

                        default:
                            Audio_Digital_button3.Text = "V";
                            Compression_button4.Enabled = true;
                            Compression_button2.Enabled = true;
                            Settings.Default.Mic_Is_Digital = false;
                         
                            break;
                    }

                    Message = " MIC_MODE: " + Convert.ToString(RPi_Settings.Volume_Settings.Mic_Pre_Gain);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "SPEAKER_MUTE=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    RPi_Settings.Volume_Settings.Speaker_Mute = temp_int;
                    if(temp_int == 1)
                    {
                        Settings.Default.Speaker_MutED = true;
                        Volume_Mute_button2.ForeColor = Color.Red;
                        Volume_Mute_button2.Text = "MUTED";
                        oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 1);
                    }
                    else
                    {
                        Settings.Default.Speaker_MutED = false;
                        Volume_Mute_button2.ForeColor = Color.Black;
                        Volume_Mute_button2.Text = "Volume";
                        oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                    }
                    Message = " SPEAKER_MUTE: " + Convert.ToString(RPi_Settings.Volume_Settings.Speaker_Mute);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "BACKGROUND_COLOR=", ";");
                if (temp_string != "")
                {
                    Int32.TryParse(temp_string, out temp_32);
                    Settings.Default.Background_Color = Color.FromArgb(temp_32);
                    //Message = " BACKGROUND_COLOR: " + RPi_Settings.Background_Color;
                    //Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "TEXT_COLOR=", ";");
                if (temp_string != "")
                {
                    Int32.TryParse(temp_string, out temp_32);
                    Settings.Default.Freq_Color = Color.FromArgb(temp_32);
                    //Message = " TEXT_COLOR: " + RPi_Settings.Text_Color;
                    //Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "BOARDER_COLOR=", ";");
                if (temp_string != "")
                {
                    Int32.TryParse(temp_string, out temp_32);
                    Settings.Default.Boarder_Color = Color.FromArgb(temp_32);
                    //Message = " BOARDER_COLOR: " + RPi_Settings.Boarder_Color;
                    //Write_Debug_Message(Message);

                }
                temp_string = getBetween(line, "PEAK_NEEDLE=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    RPi_Settings.Peak_Needle = temp_int;
                    if(RPi_Settings.Peak_Needle == 1)
                    {
                        Settings.Default.Meter_Peak_Needle = true;
                    }
                    else
                    {
                        Settings.Default.Meter_Peak_Needle = false;
                    }
                    Message = " PEAK_NEEDLE: " + Convert.ToString(RPi_Settings.Peak_Needle);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "PEAK_NEEDLE_DELAY_INDEX=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    RPi_Settings.Peak_Needle_Delay_Index = temp_int;
                    Peak_Needle_Delay_listBox1.SelectedIndex = RPi_Settings.Peak_Needle_Delay_Index;
                    Settings.Default.Meter_Peak_Hold = (byte)RPi_Settings.Peak_Needle_Delay_Index;
                    Message = " PEAK_NEEDLE_DELAY_INDEX: " + Convert.ToString(RPi_Settings.Peak_Needle_Delay_Index);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "PEAK_NEEDLE_COLOR_INDEX=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    RPi_Settings.Peak_Needle_Color_Index = temp_int;
                    Meter_Peak_Needle_Color.SelectedIndex = RPi_Settings.Peak_Needle_Color_Index;
                    Settings.Default.Meter_Peak_Needle_Color = (byte)RPi_Settings.Peak_Needle_Color_Index;
                    Message = " PEAK_NEEDLE_COLOR_INDEX: " + Convert.ToString(RPi_Settings.Peak_Needle_Color_Index);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "TIME_DISPLAY=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    RPi_Settings.Time_Display = temp_int;
                    if(RPi_Settings.Time_Display == 1)
                    {
                        Settings.Default.Time_Check_Box = true;
                        Time_checkBox2.Checked = true;
                    }
                    else
                    {
                        Settings.Default.Time_Check_Box = false;
                        Time_checkBox2.Checked = false;
                    }
                    Message = " TIME_DISPLAY: " + Convert.ToString(RPi_Settings.Time_Display);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween(line, "METER_MODE=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    RPi_Settings.Meter_Mode = temp_int;
                    Settings.Default.Meter_Mode = (byte)RPi_Settings.Meter_Mode;
                    switch (Settings.Default.Meter_Mode)
                    {
                        case Smeter_controls.POWER_MODE:
                            Meter_Mode_button8.Text = "PWR";
                            break;
                        case Smeter_controls.SWR_MODE:
                            Meter_Mode_button8.Text = "SWR";
                            break;
                        case Smeter_controls.ALC_MODE:
                            Meter_Mode_button8.Text = "ALC";
                            break;
                    }
                    Message = " METER_MODE: " + Convert.ToString(RPi_Settings.Meter_Mode);
                    Write_Debug_Message(Message);
                }
            }
            file.Close();
            Init_CW_Settings();
            Manage_Colors();
            Message = " Main_form -> Init_RPi_Settings Finished. Line_Count: " + Convert.ToString(line_count);
            Write_Debug_Message(Message);
            return true;
        }

        public void Finish_Initialization()
        {
            byte[] buf = new byte[2];

#if RPI
            Init_RPi_Settings();
#endif
            Master_Controls.Current_tab = powertabControl1.SelectedTab;
            Multus_mscc_ini_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Multus_mscc_ini_path += "/mscc/Multus_mscc.ini";
            oCode.FreqDigit = 2;    //Select the startup frequency digit to control
            MSCC_Core_Version_label45.Text = Master_Controls.productVersion;
            //Set up the start up defaults for the filters
            Filter_listBox1.SetSelected(4, true);
            Filter_control.Previous_Filter_High_Index = 4;
            Filter_control.Previous_Filter_Low_Index = 0;
            //Initialize_Main_Comm_Port();
            //Initialize_HR50_Comm_Port();
            TX_Bandwidth_listBox1.SetSelected(1, true);
            //Update_Speaker_listbox();
            //Update_Mic_listbox();
            Update_Power_values();
            AGC_listBox1.SetSelected(0, true);
            //this.MouseWheel += new MouseEventHandler(Main_MouseWheel);
            powerhScrollBar1.Value = 40;
            powerlabel14.Text = "NO VALUE";
            switch (Settings.Default.Auto_Zero)
            {
                case false:
                    Auto_Zero_checkBox2.Checked = false;
                    break;

                case true:
                    Auto_Zero_checkBox2.Checked = true;
                    break;
            }
            switch (Settings.Default.Band_Change_Tune)
            {
                case false:
                    Band_Change_Auto_Tune_checkBox2.Checked = false;
                    break;
                case true:
                    Band_Change_Auto_Tune_checkBox2.Checked = true;
                    break;
            }
            Display_GDI.MarkerFilterColor = Settings.Default.Waterfall_Marker;
            if (Settings.Default.Mic_Is_Digital)
            {
                Audio_Digital_button3.Text = "D";
            }
            else
            {
                Audio_Digital_button3.Text = "V";
                MicVolume_hScrollBar1.Value = Settings.Default.Voice_Volume;
                Mic_Gain_Step_listBox1.SelectedIndex = Settings.Default.Mic_Gain_Step;
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_GAIN, Settings.Default.Mic_Gain_Step);
                Thread.Sleep(100);
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_VOLUME, Settings.Default.Voice_Volume);
            }
            Power_Meter_Hold.Value = Settings.Default.SWR_Meter_Hold;
            Default_Low_Cut_listBox1.SelectedIndex = Settings.Default.Lo_Cut_Default;
            Default_CW_Filter_listBox1.SelectedIndex = Settings.Default.CW_Default;
            Default_High_Cut_listBox1.SelectedIndex = Settings.Default.Hi_Cut_Default;
            set_default_filter_configuration();
            //txholdlabel8.Text = Convert.ToString(displayValue) + " TX Hold";
            Initialize_RIT();
            Initialize_Docking();
            Manage_Colors();
            oCode.FreqDigit = 2;    //Select the startup frequency digit to control
            mainlistBox1.SetSelected(3, true);
            oCode.Freq_Tune_Index = 3;
            CW_Filter_listBox1.SetSelected(2, true);
            Default_CW_Filter_listBox1.SetSelected(2, true);
            Set_Freq_Digit_Pointer();

            Time_checkBox2.Enabled = true;

            this.HorizontalScroll.Maximum = 0;
            this.HorizontalScroll.Minimum = 0;
            this.HorizontalScroll.Visible = false;
            this.AutoScroll = false;
            this.VerticalScroll.Visible = false;
            this.AutoScroll = true;
            this.HorizontalScroll.Enabled = false;
            this.HorizontalScroll.Visible = false;

            switch (Settings.Default.Meter_Mode)
            {
                case 4:
                    Meter_Mode_button8.Text = "PWR";
                    break;
                case 5:
                    Meter_Mode_button8.Text = "SWR";
                    break;
                case 6:
                    Meter_Mode_button8.Text = "VU";
                    break;
            }

            this.LocationChanged += new System.EventHandler(this.Main_window_moved);
            buf[0] = Master_Controls.Extended_Commands.CMD_SET_ANTENNA_SWITCH;
            buf[1] = Settings.Default.Antenna_Switch;
            oCode.SendCommand_MultiByte(txsocket, txtarget, Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND,
                buf, buf.Length);
            Antenna_Switch_comboBox1.SelectedIndex = Settings.Default.Antenna_Switch_Index;
            oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_STEP_VALUE, oCode.Freq_Tune_Index);
            buf[0] = Master_Controls.Extended_Commands.CMD_SET_METER_HOLD;
            buf[1] = (byte)Settings.Default.SWR_Meter_Hold;
            oCode.SendCommand_MultiByte(txsocket, txtarget,
                                         Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
            oCode.isLoading = false;        // All done, clear the global and carry on.
        }

        public void Post_Initialization()
        {
            byte[] buf = new byte[2];
            
            MonitorTextBoxText(" Post_Initialization Called");
            Set_Startup_Band();
            Volume_Attn_listBox1.SelectedIndex = Settings.Default.Volume_Attn_Index;
            Volume_textBox2.Text = Convert.ToString(Settings.Default.Speaker_Volume);
            Volume_hScrollBar1.Value = Settings.Default.Speaker_Volume;
#if RPI
            buf[0] = Master_Controls.Extended_Commands.CMD_MFC_AUTO_ZERO;
            buf[1] = 1;
            oCode.SendCommand_MultiByte(txsocket, txtarget, Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND,
                                                                buf, buf.Length);
#endif

#if !RPI
            switch (Settings.Default.Speaker_MutED)
            {
                case true:
                    Volume_Mute_button2.ForeColor = Color.Red;
                    Volume_Mute_button2.Text = "MUTED";
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 1);
                    MonitorTextBoxText(" Post_Initialization Called -> Settings.Default.Speaker_MutED: " +
                                                    Convert.ToString(Settings.Default.Speaker_MutED));
                    break;
                default:
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_VOLUME, (short)Settings.Default.Speaker_Volume);
                    MonitorTextBoxText(" Post_Initialization Called -> Settings.Default.Speaker_MutED: " +
                                                                Convert.ToString(Settings.Default.Speaker_MutED));
                    break;
            }
#endif
            AMP_Tune_button4.Enabled = true;
            AMP_hScrollBar1.Enabled = true;
            Peak_Needle_checkBox2.CheckedChanged -= Peak_Needle_checkBox2_CheckedChanged;
            switch (Settings.Default.Meter_Peak_Needle)
            {
                case true:
                    Peak_Needle_checkBox2.Checked = true;
                    vuMeter1.PeakHold = true;
                    break;
                default:
                    Peak_Needle_checkBox2.Checked = false;
                    vuMeter1.PeakHold = false;
                    break;
            }
            Peak_Needle_checkBox2.CheckedChanged += Peak_Needle_checkBox2_CheckedChanged;
            Peak_Needle_Delay_listBox1.SelectedIndex = Settings.Default.Meter_Peak_Hold;
            Meter_Peak_Needle_Color.SetSelected(Settings.Default.Meter_Peak_Needle_Color, true);
            if (Settings.Default.Time_Check_Box == true)
            {
                Time_display_label33.Visible = true;
                Local_Date_label46.Visible = true;
                Time_display_UTC_label34.Visible = true;
                UTC_Date_label46.Visible = true;
                Time_checkBox2.Checked = true;
            }
            else
            {
                Time_display_label33.Visible = false;
                Local_Date_label46.Visible = false;
                Time_display_UTC_label34.Visible = false;
                UTC_Date_label46.Visible = false;
                Time_checkBox2.Checked = false;
            }

#if STEW
            Initialize_Spectrum();
            Initialize_Waterfall();
#endif

#if WINDOWS
            Initialize_Spectrum();
            Initialize_Waterfall();
#endif
            Initialize_MFC();
#if RPI
            MonitorTextBoxText(" Full Size: " + Convert.ToString(Full_size));
#endif
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
#if RPI
            String Message;
#endif
            byte[] buf = new byte[2];
            Size pic_size = new Size(0, 0);

#if RPI
            Log_file = "/home/mscc/mscc/mscc-logs/mscc.log";

            if (File.Exists(Log_file))
            {
                //File.Delete(Log_file);
                try
                {
                    File.Delete(Log_file);
                }
                catch (Exception error)
                {
                    Message = "Spectrum Log FILE Delete FAILED" + Convert.ToString(error);
                    MessageBox.Show(Message, "MSCC Spectrum", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }

#else
            Log_file = Log_Path += "\\multus-sdr-client-logs\\Multus_mscc.log";
#endif
            if (File.Exists(Log_file))
            {
                File.Delete(Log_file);
            }

            if (!Settings.Default.Main_Window_Location.IsEmpty)
            {
                this.Location = Settings.Default.Main_Window_Location;
            }
            if (!Settings.Default.Main_Window_Size.IsEmpty)
            {
                this.Size = Settings.Default.Main_Window_Size;
            }
#if RPI
            this.Location = RPI_location;
            //Form.ActiveForm.TopMost = true;
#endif
            Window_controls.Docking_Controls.Previous_Main_location = this.Location;
            //Application.UseWaitCursor = true;
            //Cursor.Hide();
            Master_Controls.Power_Meter_Max_Level = Forward_Meter.LevelMax;
            oCode.isLoading = true;         // keep control events from sending messages to DLL, or worse starting a feedback loop.
            Thread.Sleep(3000);
            if (frmGui_called)
            {
                MonitorTextBoxText(" frmGUI Called");
            }
            MonitorTextBoxText(" frmMain_Load Called");
            MonitorTextBoxText(" MSCC Version: " + Convert.ToString(Master_Controls.productVersion));
/*#if RPI
            CW_Filter_listBox1.Size = new Size(70, 30);
            Filter_Low_listBox1.Size = new Size(70, 30);
            Filter_listBox1.Size = new Size(70, 30);
            mainlistBox1.Size = new Size(70, 30);
#endif*/
            timer3.Enabled = true;
        }

        private void Zero_Frequency()
        {
            Int32 remainder = 0;
            byte[] buf = new byte[2];
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
                    MonitorTextBoxText(" Zero_Frequency -> default. FreqDigit: " + Convert.ToString(oCode.FreqDigit) +
                        " Remainder: " + Convert.ToString(remainder));
                    break;
            }
            buf[0] = Master_Controls.Extended_Commands.CMD_MFC_SET_ZERO;
            buf[1] = (byte)oCode.FreqDigit;
            oCode.DisplayFreq = oCode.DisplayFreq - remainder;
            oCode.SendCommand_MultiByte(txsocket, txtarget,
                                             Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);

        }

        /*private void Update_Frequency()
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
        */
        public void Display_Main_Freq()
        {
            short power = 0;
            short mode_number = 0;
            //Decompose the frequency into individual digits
#if !RPI
            if (Settings.Default.Auto_Zero)
            {
                Zero_Frequency();
            }
#endif
            Freq_Digits.meg10 = oCode.DisplayFreq / 10000000;
            Freq_Digits.meg = (oCode.DisplayFreq - (Freq_Digits.meg10 * 10000000)) / 1000000;
            Freq_Digits.hundred_thousand = (oCode.DisplayFreq - (Freq_Digits.meg10 * 10000000) - (Freq_Digits.meg * 1000000))
                / 100000;
            Freq_Digits.ten_thousand = (oCode.DisplayFreq - (Freq_Digits.meg10 * 10000000) - (Freq_Digits.meg * 1000000) -
                (Freq_Digits.hundred_thousand * 100000)) / 10000;
            Freq_Digits.thousand = (oCode.DisplayFreq - (Freq_Digits.meg10 * 10000000) - (Freq_Digits.meg * 1000000) -
                (Freq_Digits.hundred_thousand * 100000) - (Freq_Digits.ten_thousand * 10000)) / 1000;
            Freq_Digits.hundred = (oCode.DisplayFreq - (Freq_Digits.meg10 * 10000000) - (Freq_Digits.meg * 1000000) -
                (Freq_Digits.hundred_thousand * 100000) - (Freq_Digits.ten_thousand * 10000) - (Freq_Digits.thousand * 1000)) / 100;
            Freq_Digits.ten = (oCode.DisplayFreq - (Freq_Digits.meg10 * 10000000) - (Freq_Digits.meg * 1000000) -
                (Freq_Digits.hundred_thousand * 100000) - (Freq_Digits.ten_thousand * 10000) - (Freq_Digits.thousand * 1000) -
                                                   (Freq_Digits.hundred * 100)) / 10;
            Freq_Digits.one = (oCode.DisplayFreq - (Freq_Digits.meg10 * 10000000) - (Freq_Digits.meg * 1000000) -
                (Freq_Digits.hundred_thousand * 100000) - (Freq_Digits.ten_thousand * 10000) - (Freq_Digits.thousand * 1000) -
                                                   (Freq_Digits.hundred * 100) - (Freq_Digits.ten * 10));

            Ones.Text = Convert.ToString(Freq_Digits.one);
            Tens.Text = Convert.ToString(Freq_Digits.ten);
            Hundreds.Text = Convert.ToString(Freq_Digits.hundred);
            Thousands.Text = Convert.ToString(Freq_Digits.thousand);
            Tenthousands.Text = Convert.ToString(Freq_Digits.ten_thousand);
            Hundredthousand.Text = Convert.ToString(Freq_Digits.hundred_thousand);
            Millions.Text = Convert.ToString(Freq_Digits.meg);
            if (Freq_Digits.meg10 == 0)
            {
                MonitorTextBoxText(" meg10 Position is zero");
                Tenmillions.Text = " ";
            }
            else
            {
                Tenmillions.Text = Convert.ToString(Freq_Digits.meg10);
            }
            //Voice_output.SpeakAsync(Convert.ToString(oCode.DisplayFreq) +"  Mega Hertz");
            Panadapter_Controls.Frequency = oCode.DisplayFreq;
            //Panadapter_Controls.Updated_Frequency = oCode.DisplayFreq;
            Panadapter_Controls.Freq_Set_By_Master = true;

            Master_Controls.TX_Inhibited = true;
            if (oCode.DisplayFreq >= 1800000 && oCode.DisplayFreq <= 2000000) Master_Controls.TX_Inhibited = false;
            if (oCode.DisplayFreq >= 3500000 && oCode.DisplayFreq <= 4000000) Master_Controls.TX_Inhibited = false;
            if (oCode.DisplayFreq >= 5330000 && oCode.DisplayFreq <= 5404000) Master_Controls.TX_Inhibited = false;
            if (oCode.DisplayFreq >= 7000000 && oCode.DisplayFreq <= 7300000) Master_Controls.TX_Inhibited = false;
            if (oCode.DisplayFreq >= 10100000 && oCode.DisplayFreq <= 10150000) Master_Controls.TX_Inhibited = false;
            if (oCode.DisplayFreq >= 14000000 && oCode.DisplayFreq <= 14350000) Master_Controls.TX_Inhibited = false;
            if (oCode.DisplayFreq >= 18068000 && oCode.DisplayFreq <= 18168000) Master_Controls.TX_Inhibited = false;
            if (oCode.DisplayFreq >= 21000000 && oCode.DisplayFreq <= 21450000) Master_Controls.TX_Inhibited = false;
            if (oCode.DisplayFreq >= 24890000 && oCode.DisplayFreq <= 24990000) Master_Controls.TX_Inhibited = false;
            if (oCode.DisplayFreq >= 28000000 && oCode.DisplayFreq <= 30000000) Master_Controls.TX_Inhibited = false;
            if (Master_Controls.TX_Inhibited)
            {
                if (Master_Controls.PPT_Mode)
                {
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_TX_ON, 0);
                    button1.BackColor = Color.Gainsboro;
                    button1.ForeColor = Color.Black;
                    button1.Text = "PTT";
                    Master_Controls.PPT_Mode = false;
                    Master_Controls.Transmit_Mode = false;
                    Master_Controls.Two_Tone = false;
                }
                if (Master_Controls.Tuning_Mode)
                {
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_RIG_TUNE, 0);
                    power = get_previous_power(oCode.current_band);
                    mode_number = Convert_mode_char_to_digit(Last_used.Current_mode);
                    MonitorTextBoxText(" Tune -> Current Band: " + oCode.current_band +
                        " Current Mode Number: " + mode_number);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                    buttTune.BackColor = Color.Gainsboro;
                    buttTune.ForeColor = Color.Black;
                    Master_Controls.Tuning_Mode = false;
                    Master_Controls.Transmit_Mode = false;
                    Tune_vButton2.BackColor = Color.Gainsboro;
                    Tune_vButton2.ForeColor = Color.Black;
                    Tune_vButton2.Text = "TUN";
                }
            }
#if FTDI
            Tuning_Knob_Controls.Freq = oCode.DisplayFreq;
            Master_Controls.Main_frequency = oCode.DisplayFreq;
            Freq_Digits.previous_frequency = oCode.DisplayFreq;
#endif
        }

        /*private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // somebody typed something here, but WE DON'T CARE...
        }
        */
        private void ritLab_Click(object sender, EventArgs e)
        {
        }

        /*private void label4_Click(object sender, EventArgs e)
        {
        }

        private void icallibratelabel8_Click(object sender, EventArgs e)
        {
        }

        private void txtUDP_TextChanged(object sender, EventArgs e)
        {
        }
        */

        /*private void label10_Click(object sender, EventArgs e)
        {
        }

        private void label12_Click(object sender, EventArgs e)
        {
        }

        private void txtUDP_TextChanged_1(object sender, EventArgs e)
        {
        }
        */
        private void ritfreqtextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void powerbutton1_Click(object sender, EventArgs e)
        {
            //bool power_toggle = false;
            MonitorTextBoxText(" powerbutton1 Clicked");
            if (Power_Calibration_Controls.Tuning_Mode)
            {
                DialogResult ret = MessageBox.Show("Changing Power Value in TUNE mode is not permitted\r\nTurn TUNE off",
                    "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (Power_Calibration_Controls.Band_Is_Selected)
            {
                // Power Calibration Apply button pressed.
                if (Power_Calibration_Controls.Warning_Accepted)
                {
                    if (!Power_Calibration_Controls.Applied_Button_Active)
                    {
                        //powerbutton1.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        Power_Calibration_Controls.Applied_Button_Active = true;
                        //oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_BAND_POWER_BAND, (short)Power_Calibration_Controls.Band);
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_BAND_POWER_POWER,
                            (short)Power_Calibration_Controls.New_Power_Value);
                        powerlabel14.Text = "WAIT";
                        powerlabel14.ForeColor = Color.Red;
                        //Power_Calibration_Controls.Update_Pending = true;
                    }
                }
                else
                {
                    DialogResult ret = MessageBox.Show("I have read the warning regarding the use of the Power Calibration routine.\n\r" +
                        "I further agree that Multus SDR, LLC has no liability " +
                        "for possible improper functioning of the transceiver after the use of said calibration routine.\n\r" +
                        "Click YES if you agree.", "MSCC", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                    if (ret == DialogResult.Yes)
                    {
                        Power_Calibration_Controls.Warning_Accepted = true;
                    }
                }
            }
            else
            {
                DialogResult ret = MessageBox.Show("Select a Band", "MSCC",
                      MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            MonitorTextBoxText(" powerbutton1 Finished");
        }

        private void powerhScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (oCode.isLoading) { return; }
            int powerValue = powerhScrollBar1.Value;
            if (powerValue == Power_Calibration_Controls.New_Power_Value) return;           // nothing happens if value doesn't change from previous
            Power_Calibration_Controls.New_Power_Value = powerValue;
            if (Power_Calibration_Controls.Band_Is_Selected == true)
            {
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_BAND_POWER_POWER,
                               (short)Power_Calibration_Controls.New_Power_Value);
                powerlabel14.Text = Convert.ToString(powerValue) + " POWER";
            }
            else
            {
                DialogResult ret = MessageBox.Show("Select a Band", "MSCC",
                      MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void powerlabel14_Click(object sender, EventArgs e)
        {
        }

        /*private void ritonofflabel11_Click(object sender, EventArgs e)
        {
        }*/

        private void ritbutton1_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (Rit_Controls.Rit_On == false)
            {
                Set_Button_Color(true, ritbutton1);
                Rit_Controls.Rit_On = true;
                oCode.SendCommand(txsocket, txtarget, Rit_Controls.CMD_SET_RIT_STATUS, 1);
            }
            else
            {
                Set_Button_Color(false, ritbutton1);
                Rit_Controls.Rit_On = false;
                oCode.SendCommand(txsocket, txtarget, Rit_Controls.CMD_SET_RIT_STATUS, 0);
            }
        }

        private void calibratebutton1_Click_1(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (!Frequency_Calibration_controls.standard_carrier_selected)
            {
                MessageBox.Show("No Standard Carrier Selected. Select a Standard Carrier", "MSCC-Core",
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (Filter_control.CW_Pitch != 600)
            {
                MessageBox.Show("CW Pitch must be set to 600", "MSCC",
                               MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (Frequency_Calibration_controls.Calibration_Checked)
            {
                DialogResult ret = MessageBox.Show("The Transceiver Oscillator will be calibrated\r\n" +
                      "Please wait until the Calibrate Button displays: CALIBRATED\r\n" +
                      "DO NOT LEAVE THIS PAGE OR PRESS ANY OTHER BUTTONS OR SLIDERS" +
                      " UNTIL THE CALIBRATION HAS COMPLETED\r\n" +
                      "DOING OTHERWISE WILL CAUSE THE CALIBRATION TO FAIL WITH INDETERMINATE RESULTS \r\n" +
                       "\r\n THE SIGNAL LEVEL OF THE STANDARD CARRIER SIGNAL MUST BE S7 OR GREATER UNLESS THE LOOSE OPTION IS ON\r\n"
                       + "\r\n" +
                      "DO YOU WISH TO PROCEED?",
                      "MSCC", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                if (ret == DialogResult.Yes)
                {
                    Frequency_Calibration_controls.Fine_Resolution_Set = true;
                    Frequency_Calibration_controls.Course_Resolution_Set = false;
                    Calibration_progressBar1.Value = 0;
                    oCode.SendCommand(txsocket, txtarget, Frequency_Calibration_controls.CMD_SET_CAL_FINE, 1);
                    oCode.SendCommand(txsocket, txtarget, Frequency_Calibration_controls.CMD_SET_FREQ_CAL_CHECK, 0);
                    oCode.SendCommand32(txsocket, txtarget, Frequency_Calibration_controls.CMD_START_CALIBRATE,
                        Frequency_Calibration_controls.Calibration_frequency);
                    Frequency_Calibration_controls.si5351_reset = false;
                    Frequency_Calibration_controls.freq_text_box_cleared = false;
                    Frequency_Calibration_controls.standard_carrier_selected = false;
                    calibratebutton1.BackColor = Color.Red;
                    calibratebutton1.ForeColor = Color.White;
                    calibratebutton1.FlatStyle = FlatStyle.Popup;
                    calibratebutton1.Text = "CALIBRATING";
                    Freq_CAl_Progress_Lable.Visible = true;
                    Frequency_Calibration_controls.Calibration_In_Progress = true;
                }
            }
            else
            {
                MessageBox.Show("Run a Calibration Check before performing a calibration\r\n" +
                    "To determine if the Standard Carrier signal is sufficient for a calibration", "MSCC",
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        /*private void freqtextBox1_TextChanged_2(object sender, EventArgs e)
        {
            if (oCode.freq_text_box_cleared == false)
            {
                freqtextBox1.Text = "";
                oCode.freq_text_box_cleared = true;
            }
        }

        private void firmwarelabel16_Click(object sender, EventArgs e)
        {
        }

        private void label3_Click_1(object sender, EventArgs e)
        {
        }
        */
        private void Manage_Rit(int step, int limit_index)
        {
            switch (step)
            {
                case 10:
                    switch (limit_index)
                    {
                        case 0:
                            ritScroll.Minimum = -100;
                            ritScroll.Maximum = 109;
                            break;
                        case 1:
                            ritScroll.Minimum = -500;
                            ritScroll.Maximum = 509;
                            break;
                        case 2:
                            ritScroll.Minimum = -1000;
                            ritScroll.Maximum = 1009;
                            break;
                    }
                    break;
                case 20:
                    switch (limit_index)
                    {
                        case 0:
                            ritScroll.Minimum = -100;
                            ritScroll.Maximum = 119;
                            break;
                        case 1:
                            ritScroll.Minimum = -500;
                            ritScroll.Maximum = 519;
                            break;
                        case 2:
                            ritScroll.Minimum = -1000;
                            ritScroll.Maximum = 1019;
                            break;
                    }
                    break;
                case 30:
                    switch (limit_index)
                    {
                        case 0:
                            ritScroll.Minimum = -100;
                            ritScroll.Maximum = 129;
                            break;
                        case 1:
                            ritScroll.Minimum = -500;
                            ritScroll.Maximum = 529;
                            break;
                        case 2:
                            ritScroll.Minimum = -1000;
                            ritScroll.Maximum = 1029;
                            break;
                    }
                    break;
                case 40:
                    switch (limit_index)
                    {
                        case 0:
                            ritScroll.Minimum = -100;
                            ritScroll.Maximum = 139;
                            break;
                        case 1:
                            ritScroll.Minimum = -500;
                            ritScroll.Maximum = 539;
                            break;
                        case 2:
                            ritScroll.Minimum = -1000;
                            ritScroll.Maximum = 1039;
                            break;
                    }
                    break;
                case 50:
                    switch (limit_index)
                    {
                        case 0:
                            ritScroll.Minimum = -100;
                            ritScroll.Maximum = 149;
                            break;
                        case 1:
                            ritScroll.Minimum = -500;
                            ritScroll.Maximum = 549;
                            break;
                        case 2:
                            ritScroll.Minimum = -1000;
                            ritScroll.Maximum = 1049;
                            break;
                    }
                    break;
            }
        }

        private void ritlistBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index;

            if (oCode.isLoading == true) return;

            index = ritlistBox1.SelectedIndex;
            Settings.Default.RIT_Step = index;
            switch (index)
            {
                case 0:
                    ritScroll.LargeChange = 10;
                    break;

                case 1:
                    ritScroll.LargeChange = 20;
                    break;

                case 2:
                    ritScroll.LargeChange = 30;
                    break;

                case 3:
                    ritScroll.LargeChange = 40;
                    break;

                case 4:
                    ritScroll.LargeChange = 50;
                    break;
            }
#if FTDI
            switch (index)
            {
                case 0:
                    Tuning_Knob_Controls.RIT_OFFSET_Function.RIT_Step = 10;
                    break;

                case 1:
                    Tuning_Knob_Controls.RIT_OFFSET_Function.RIT_Step = 20;
                    break;

                case 2:
                    Tuning_Knob_Controls.RIT_OFFSET_Function.RIT_Step = 30;
                    break;

                case 3:
                    Tuning_Knob_Controls.RIT_OFFSET_Function.RIT_Step = 40;
                    break;

                case 4:
                    Tuning_Knob_Controls.RIT_OFFSET_Function.RIT_Step = 50;
                    break;
            }
#endif
            Manage_Rit(ritScroll.LargeChange, ritoffsetlistBox1.SelectedIndex);
        }

        private void label7_Click(object sender, EventArgs e)
        {
        }

        private void ritoffsetlistBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //#if FTDI
            int index = 0;

            if (oCode.isLoading == true) return;
            index = ritoffsetlistBox1.SelectedIndex;
            Settings.Default.RIT_Limits = index;
            /*switch (index)
            {
                case 0:
                    ritScroll.Minimum = -100;
                    ritScroll.Maximum = 109;
                    break;

                case 1:
                    ritScroll.Minimum = -500;
                    ritScroll.Maximum = 509;
                    break;

                case 2:
                    ritScroll.Minimum = -1000;
                    ritScroll.Maximum = 1009;
                    break;
            }*/
            Manage_Rit(ritScroll.LargeChange, ritoffsetlistBox1.SelectedIndex);
            //#endif
        }

        private void powertunebutton1_Click(object sender, EventArgs e)
        {
            MonitorTextBoxText(" powertunebutton1 Entered ");
            if (!Master_Controls.Transceiver_Warming)
            {
                if (Power_Calibration_Controls.Band_Is_Selected)
                {
                    if (Power_Calibration_Controls.Warning_Accepted)
                    {
                        if (!Power_Calibration_Controls.Tuning_Mode)
                        {
                            Power_Calibration_Controls.Tuning_Mode = true;
                            //oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, Mode.TUNE_mode);
                            oCode.SendCommand(txsocket, txtarget, Power_Calibration_Controls.CMD_CALIBRATION_TUNE, 1);
                            powertunebutton1.ForeColor = Color.Red;
                            powertunebutton1.Text = "TUNING";
                            powertunebutton1.FlatStyle = FlatStyle.Popup;
                            Master_Controls.Transmit_Mode = true;
                        }
                        else
                        {
                            Power_Calibration_Controls.Tuning_Mode = false;
                            oCode.SendCommand(txsocket, txtarget, Power_Calibration_Controls.CMD_CALIBRATION_TUNE, 0);
                            powertunebutton1.ForeColor = Control.DefaultForeColor;
                            powertunebutton1.Text = "TUNE";
                            powertunebutton1.FlatStyle = FlatStyle.Standard;
                            Master_Controls.Transmit_Mode = false;
                        }
                    }
                    else
                    {
                        DialogResult ret = MessageBox.Show("I have read the warning regarding the use of the Power Calibration routine.\n\r" +
                            "I further agree that Multus SDR, LLC has no liability " +
                            "for possible improper functioning of the transceiver after the use of said calibration routine.\n\r" +
                            "Click YES if you agree.", "MSCC", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                        if (ret == DialogResult.Yes)
                        {
                            Power_Calibration_Controls.Warning_Accepted = true;
                        }
                    }
                }
                else
                {
                    DialogResult ret = MessageBox.Show("Select a Band", "MSCC",
                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            else
            {
                MessageBox.Show("Transceiver is in Warm Up Mode. Try again later after warm up is complete", "MSCC",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            MonitorTextBoxText(" powertunebutton1 finished ");
        }

        private void powerrestorebutton2_Click(object sender, EventArgs e)
        {
            if (!Power_Calibration_Controls.Factory_Defaults)
            {
                DialogResult ret = MessageBox.Show("This resets the power values to factory defaults for all bands\n" + "Clicking Yes will perform a reset\r\n"
                    + "Do you wish to continue ?", "MSCC", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                if (ret == DialogResult.Yes)
                {
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_BAND_POWER_DEFAULTS, 1);
                    Thread.Sleep(50);
                    powerrestorebutton2.Text = "WAIT";
                    powerrestorebutton2.ForeColor = Color.Red;
                    //oCode.SendCommand(txsocket, txt;arget, oCode.CMD_GET_BAND_POWER, (short)Power_Calibration_Controls.Band);
                    Power_Calibration_Controls.Factory_Defaults = true;
                }
            }
        }

        /*private void label18_Click(object sender, EventArgs e)
        {
        }
        */
        private void band_stack_update_button1_Click_1(object sender, EventArgs e)
        {
            int len = 0;
            String my_name = "None";
            if (Favorites_Controls.Name_Entered)
            {
                len = Favorites_textBox2.TextLength;
                my_name = Favorites_textBox2.Text;

                Favorites_Controls.Name_Entered = false;
                Favorites_textBox2.Text = "Enter Name";
            }
            else
            {
                my_name = Convert.ToString(oCode.DisplayFreq);
                len = my_name.Length;
                // DialogResult ret = MessageBox.Show("Enter a Name for this Update", "MSCC",MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Favorites_textBox2.Text = "Enter Name";
            }
            oCode.SendCommand_String(txsocket, txtarget, Favorites_Controls.CMD_SET_BAND_STACK_NAME, my_name, len);
            oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_UPDATE_BAND_STACK, 1);
            //Get_FTP_Init_File();
        }

        private void label19_Click(object sender, EventArgs e)
        {
        }

        private void label20_Click(object sender, EventArgs e)
        {
        }

        /*private void band_stack_textBox1_TextChanged(object sender, EventArgs e)
        {
        }*/

        private void band_stack_label29_Click(object sender, EventArgs e)
        {
        }

        private void label29_Click(object sender, EventArgs e)
        {
        }

        private void power_slider_reset_button1_Click(object sender, EventArgs e)
        {
            powerhScrollBar1.Value = 50;
            powerlabel14.Text = 50 + " POWER";
            Power_Calibration_Controls.Old_Power_Value = 50;
            Power_Calibration_Controls.New_Power_Value = 50;
        }

        /*private void carrierbutton1_Click(object sender, EventArgs e)
        {
            Int32 fr = 0;
            short mode_number;
            if (oCode.isLoading) return;
            resetbutton1.Visible = true;
            label57.Visible = true;
            calibratebutton1.Visible = true;
            Freq_Cal_listBox1.Visible = true;
            Calibration_progressBar1.Visible = true;
            //Progress_label58.Visible = true;

            switch (Frequency_Calibration_controls.standard_carrier)
            {
                case 0:
                    fr = 2500000;
                    break;

                case 1:
                    fr = 3330000;
                    break;

                case 2:
                    fr = 5000000;
                    break;

                case 3:
                    fr = 7850000;
                    break;

                case 4:
                    fr = 10000000;
                    break;

                case 5:
                    fr = 14670000;
                    break;

                case 6:
                    fr = 15000000;
                    break;

                case 7:
                    fr = 20000000;
                    break;
            }
            label57.Text = Convert.ToString(fr);
            Frequency_Calibration_controls.standard_carrier_selected = true;
            Frequency_Calibration_controls.standard_carrier++;
            if (Frequency_Calibration_controls.standard_carrier > 7) Frequency_Calibration_controls.standard_carrier = 0;
            mode_number = Convert_mode_char_to_digit('A');
            oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
            oCode.SendCommand32(txsocket, txtarget, Frequency_Calibration_controls.CMD_SET_STANDARD_CARRIER, fr);
            Last_used.GEN.Freq = fr;
            //oCode.oldTunefreq = fr;
            Last_used.GEN.Mode = 'A';
            genradioButton.Checked = true;
            oCode.DisplayFreq = Last_used.GEN.Freq;
            Display_Main_Freq();
            int MHz = fr / 1000000;
            int KHz = (fr - (MHz * 1000000)) / 1000;
            int Hz = fr - (MHz * 1000000) - (KHz * 1000);
            Panadapter_Controls.Frequency = oCode.DisplayFreq;
            //Panadapter_Controls.Updated_Frequency = oCode.DisplayFreq;
            Panadapter_Controls.Freq_Set_By_Master = true;

            ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
        }

        private void mainfreqtextBox2_TextChanged(object sender, EventArgs e)
        {
        }
        */
        private void mainmodebutton2_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            // mode button clicked
            MonitorTextBoxText(" mainmodebutton2 Entered ");
            char mode = 'N';
            oCode.modeswitch++;
            if (oCode.modeswitch > 3) oCode.modeswitch = 0;
            switch (oCode.modeswitch)
            {
                case 0:
                    mainmodebutton2.Text = "AM";
                    mode = 'A';
                    Volume_Controls.Volume_Slider_Mode = Volume_Controls.SLIDER_AM_MODE;
                    break;

                case 1:
                    mainmodebutton2.Text = "LSB";
                    //Voice_output.SpeakAsync("mode    " + "LSB");
                    mode = 'L';
                    Volume_Controls.Volume_Slider_Mode = Volume_Controls.SLIDER_SSB_MODE;
                    break;

                case 2:
                    mainmodebutton2.Text = "USB";
                    mode = 'U';
                    Volume_Controls.Volume_Slider_Mode = Volume_Controls.SLIDER_SSB_MODE;
                    break;

                case 3:
                    mainmodebutton2.Text = "CW";
                    mode = 'C';
                    Volume_Controls.Volume_Slider_Mode = Volume_Controls.SLIDER_CW_MODE;
                    break;
            }
            Last_used.Current_mode = mode;
            MonitorTextBoxText(" mainmodebutton2 -> New Mode: " + mode + ", Current_band: " +
                oCode.current_band);
            switch (oCode.current_band)
            {
                case 160:
                    Last_used.B160.Mode = mode;
                    break;
                case 80:
                    Last_used.B80.Mode = mode;
                    break;
                case 60:
                    Last_used.B60.Mode = mode;
                    break;
                case 40:
                    Last_used.B40.Mode = mode;
                    break;
                case 30:
                    Last_used.B30.Mode = mode;
                    break;
                case 20:
                    Last_used.B20.Mode = mode;
                    break;
                case 17:
                    Last_used.B17.Mode = mode;
                    break;
                case 15:
                    Last_used.B15.Mode = mode;
                    break;
                case 12:
                    Last_used.B12.Mode = mode;
                    break;
                case 10:
                    Last_used.B10.Mode = mode;
                    break;
                default:
                    Last_used.GEN.Mode = mode;
                    break;
            }
            Last_used.Current_mode = mode;
            oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, (short)oCode.modeswitch);
            MonitorTextBoxText(" mainmodebutton2 finished ");
        }

        /*private void label32_Click(object sender, EventArgs e)
        {
        }*/

        public void ritScroll_Scroll(object sender, ScrollEventArgs e)
        {
            int MHz;
            int KHz;
            int Hz;
            int freq_plus_rit;

            int ritValue = ritScroll.Value;
            if (ritValue == Rit_Controls.Offset) return;           // nothing happens if value doesn't change from previous
#if FTDI
            Tuning_Knob_Controls.RIT_OFFSET_Function.RIT_Slider_Active = true;
#endif

            Rit_Controls.Rit_Freq = oCode.DisplayFreq;
            Rit_Controls.Offset = ritValue;
            freq_plus_rit = Rit_Controls.Rit_Freq + Rit_Controls.Offset;
            Rit_Controls.Rit_Freq_Plus_Offset = Rit_Controls.Rit_Freq + Rit_Controls.Offset;

            MHz = freq_plus_rit / 1000000;
            KHz = (freq_plus_rit - (MHz * 1000000)) / 1000;
            Hz = freq_plus_rit - (MHz * 1000000) - (KHz * 1000);
            //Rit_Controls.Rit_Freq = freq_plus_rit;
            ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
            oCode.SendCommand32(txsocket, txtarget, Rit_Controls.CMD_SET_RIT_FREQ, Rit_Controls.Offset);
#if FTDI
            Tuning_Knob_Controls.RIT_OFFSET_Function.RIT_Slider_Active = false;
#endif
        }

        private void buttReset_Click(object sender, EventArgs e)
        {
            ritScroll.Value = 0;            // Zero out RIT scrollbar
#if FTDI
            Tuning_Knob_Controls.RIT_OFFSET_Function.RIT_Main_Offset_Value = 0;
#endif
            ritScroll_Scroll(null, null);
        }

        private short get_previous_power(short band)
        {
            short power = 0;
            switch (band)
            {
                case 160:
                    switch (Last_used.B160.Mode)
                    {
                        case 'A':
                            power = Last_used.B160.AM_power;
                            break;

                        case 'U':
                            power = Last_used.B160.SSB_power;
                            break;

                        case 'L':
                            power = Last_used.B160.SSB_power;
                            break;

                        case 'C':
                            power = Last_used.B160.CW_power;
                            break;
                    }
                    break;

                case 80:
                    switch (Last_used.B80.Mode)
                    {
                        case 'A':
                            power = Last_used.B80.AM_power;
                            break;

                        case 'U':
                            power = Last_used.B80.SSB_power;
                            break;

                        case 'L':
                            power = Last_used.B80.SSB_power;
                            break;

                        case 'C':
                            power = Last_used.B80.CW_power;
                            break;
                    }
                    break;

                case 60:
                    switch (Last_used.B60.Mode)
                    {
                        case 'A':
                            power = Last_used.B60.AM_power;
                            break;

                        case 'U':
                            power = Last_used.B60.SSB_power;
                            break;

                        case 'L':
                            power = Last_used.B60.SSB_power;
                            break;

                        case 'C':
                            power = Last_used.B60.CW_power;
                            break;
                    }
                    break;

                case 40:
                    switch (Last_used.B40.Mode)
                    {
                        case 'A':
                            power = Last_used.B40.AM_power;
                            break;

                        case 'U':
                            power = Last_used.B40.SSB_power;
                            break;

                        case 'L':
                            power = Last_used.B40.SSB_power;
                            break;

                        case 'C':
                            power = Last_used.B40.CW_power;
                            break;
                    }
                    break;

                case 30:
                    switch (Last_used.B30.Mode)
                    {
                        case 'A':
                            power = Last_used.B30.AM_power;
                            break;

                        case 'U':
                            power = Last_used.B30.SSB_power;
                            break;

                        case 'L':
                            power = Last_used.B30.SSB_power;
                            break;

                        case 'C':
                            power = Last_used.B30.CW_power;
                            break;
                    }
                    break;

                case 20:
                    switch (Last_used.B20.Mode)
                    {
                        case 'A':
                            power = Last_used.B20.AM_power;
                            break;

                        case 'U':
                            power = Last_used.B20.SSB_power;
                            break;

                        case 'L':
                            power = Last_used.B20.SSB_power;
                            break;

                        case 'C':
                            power = Last_used.B20.CW_power;
                            break;
                    }
                    break;

                case 17:
                    switch (Last_used.B17.Mode)
                    {
                        case 'A':
                            power = Last_used.B17.AM_power;
                            break;

                        case 'U':
                            power = Last_used.B17.SSB_power;
                            break;

                        case 'L':
                            power = Last_used.B17.SSB_power;
                            break;

                        case 'C':
                            power = Last_used.B17.CW_power;
                            break;
                    }
                    break;

                case 15:
                    switch (Last_used.B15.Mode)
                    {
                        case 'A':
                            power = Last_used.B15.AM_power;
                            break;

                        case 'U':
                            power = Last_used.B15.SSB_power;
                            break;

                        case 'L':
                            power = Last_used.B15.SSB_power;
                            break;

                        case 'C':
                            power = Last_used.B15.CW_power;
                            break;
                    }
                    break;

                case 12:
                    switch (Last_used.B12.Mode)
                    {
                        case 'A':
                            power = Last_used.B12.AM_power;
                            break;

                        case 'U':
                            power = Last_used.B12.SSB_power;
                            break;

                        case 'L':
                            power = Last_used.B12.SSB_power;
                            break;

                        case 'C':
                            power = Last_used.B12.CW_power;
                            break;
                    }
                    break;

                case 10:
                    switch (Last_used.B10.Mode)
                    {
                        case 'A':
                            power = Last_used.B10.AM_power;
                            break;

                        case 'U':
                            power = Last_used.B10.SSB_power;
                            break;

                        case 'L':
                            power = Last_used.B10.SSB_power;
                            break;

                        case 'C':
                            power = Last_used.B10.CW_power;
                            break;
                    }
                    break;
            }
            return power;
        }

        private void buttTune_Click(object sender, EventArgs e)
        {
            int status = 0;
            short power = 0;
            short mode_number = 0;
            // Tune button clicked
            MonitorTextBoxText(" Tune Entered ");
            if (!Master_Controls.TX_Inhibited)
            {
                if (status == 0)
                {
                    if (Master_Controls.Tuning_Mode == false)
                    {
                        oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 1);
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, Mode.TUNE_mode);
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_RIG_TUNE, 1);
                        buttTune.BackColor = Color.Red;
                        buttTune.ForeColor = Color.White;
                        Master_Controls.Tuning_Mode = true;
                        Master_Controls.Transmit_Mode = true;
                        AM_Carrier_hScrollBar1.Enabled = false;
                        Power_hScrollBar1.Enabled = false;
                        CW_Power_hScrollBar1.Enabled = false;
                        button1.Enabled = false;
                        Set_Button_Color(true, Tune_vButton2);
                        Volume_Controls.Previous_Slider_Mode = Volume_Controls.Volume_Slider_Mode;
                        Volume_Controls.Volume_Slider_Mode = Volume_Controls.SLIDER_TUNE_MODE;
                    }
                    else
                    {
                        if (!Settings.Default.Speaker_MutED)
                        {
                            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                        }
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_RIG_TUNE, 0);
                        power = get_previous_power(oCode.current_band);
                        mode_number = Convert_mode_char_to_digit(Last_used.Current_mode);
                        MonitorTextBoxText(" Tune -> Current Band: " + oCode.current_band +
                            " Current Mode Number: " + mode_number);
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                        buttTune.BackColor = Color.Gainsboro;
                        buttTune.ForeColor = Color.Black;
                        Master_Controls.Tuning_Mode = false;
                        Master_Controls.Transmit_Mode = false;
                        AM_Carrier_hScrollBar1.Enabled = true;
                        Power_hScrollBar1.Enabled = true;
                        CW_Power_hScrollBar1.Enabled = true;
                        button1.Enabled = true;
                        Set_Button_Color(false, Tune_vButton2);
                        Volume_Controls.Volume_Slider_Mode = Volume_Controls.Previous_Slider_Mode;
                    }
                }
            }
            status = 0;
            MonitorTextBoxText(" Tune -> finished ");
        }

        private void mainPage_Click(object sender, EventArgs e)
        {
        }

        /*private void VolumetextBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void FiltertextBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void ritfreqlabel10_Click(object sender, EventArgs e)
        {
        }
        */
        private void button1_Click_1(object sender, EventArgs e)
        {
            int status = 0;
            MonitorTextBoxText(" PTT Entered (button1)");

            if (!Master_Controls.TX_Inhibited)
            {
                if (status == 0)
                {
                    if (Master_Controls.PPT_Mode == false)
                    {
                        //oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 1);
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_TX_ON, 1);
                        button1.BackColor = Color.Red;
                        button1.ForeColor = Color.White;
                        button1.Text = "PTT";
                        button1.FlatStyle = FlatStyle.Popup;
                        Master_Controls.PPT_Mode = true;
                        Master_Controls.Transmit_Mode = true;
                        Tune_Power_hScrollBar1.Enabled = false;
                        buttTune.Enabled = false;
                        Main_Power_hScrollBar1.Visible = true;
                        switch (oCode.modeswitch)
                        {
                            case 0:
                                Main_Power_hScrollBar1.Value = Power_Controls.AM_Power;
                                break;
                            case 1:
                                Main_Power_hScrollBar1.Value = Power_Controls.Main_Power;
                                break;
                            case 2:
                                Main_Power_hScrollBar1.Value = Power_Controls.Main_Power;
                                break;
                            case 3:
                                Main_Power_hScrollBar1.Value = Power_Controls.CW_Power;
                                break;
                        }
                    }
                    else
                    {
                        //if (!Volume_Controls.Speaker_MutED)
                        //{
                        //    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                        //}
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_TX_ON, 0);
                        button1.BackColor = Color.Gainsboro;
                        button1.ForeColor = Color.Black;
                        button1.Text = "PTT";
                        button1.FlatStyle = FlatStyle.Standard;
                        Master_Controls.PPT_Mode = false;
                        Master_Controls.Transmit_Mode = false;
                        buttTune.Enabled = true;
                        Master_Controls.Two_Tone = false;
                        //ALC_Meter.Visible = false;
                        //ALC_label5.Visible = false;
                        Tune_Power_hScrollBar1.Enabled = true;
                        Main_Power_hScrollBar1.Visible = false;
                    }
                }
            }
            status = 0;
            MonitorTextBoxText(" PTT finished (button1) ");
        }
        private void Freqbutton3_Click(object sender, EventArgs e)
        {
            if (!AGC_ALC_Notch_Controls.Notch_Button_On)
            {
                Freqbutton3.BackColor = Color.Red;
                Freqbutton3.ForeColor = Color.White;
                //Freqbutton3.Text = "N";
                Freqbutton3.FlatStyle = FlatStyle.Popup;
                AGC_ALC_Notch_Controls.Notch_Button_On = true;
                oCode.SendCommand(txsocket, txtarget, AGC_ALC_Notch_Controls.CMD_GET_SET_AUTO_NOTCH, 1);
            }
            else
            {
                Freqbutton3.BackColor = Color.Gainsboro;
                Freqbutton3.ForeColor = Color.Black;
                //Freqbutton3.Text = "N";
                Freqbutton3.FlatStyle = FlatStyle.Standard;
                AGC_ALC_Notch_Controls.Notch_Button_On = false;
                oCode.SendCommand(txsocket, txtarget, AGC_ALC_Notch_Controls.CMD_GET_SET_AUTO_NOTCH, 0);
            }
        }

        /*private void label33_Click(object sender, EventArgs e)
        {
        }

        private void LowtextBox2_TextChanged(object sender, EventArgs e)
        {
        }
        */
        /*private void transvertertabPage3_Click(object sender, EventArgs e)
        {
        }*/

        private void Monitorbutton_Click(object sender, EventArgs e)
        {
            MonitorTextBoxText(" Monitor Popup Entered");
            if (Monitor_window != null)
            {
                MonitorTextBoxText(" Monitor Popup is not null");
                Window_controls.monitor_form_displosed = Monitor_window.IsDisposed;
                if (Window_controls.monitor_form_displosed)
                {
                    MonitorTextBoxText(" Monitor Popup Disposed");
                }
                else
                {
                    MonitorTextBoxText(" Monitor Popup Not disposed");
                }
            }
            if (Monitor_window == null)
            {
                Monitor_window = new MonitorForm();
                MonitorTextBoxText(" Monitor Popup Created");
                Window_controls.monitor_form_displosed = false;
            }
            if (Monitor_window != null && !Window_controls.monitor_form_displosed)
            {
                Window_controls.monitor_form_state = (int)Monitor_window.WindowState;
                MonitorTextBoxText(" Monitor Popup State: " + Monitor_window.WindowState + " " + Convert.ToString(Monitor_window.WindowState));

                if (Window_controls.display_monitor_form)
                {
                    Monitor_window.Show();
                    Window_controls.display_monitor_form = false;
                    MonitorTextBoxText(" Monitor Popup Show");
                    Window_controls.monitor_form_state = (int)Monitor_window.WindowState;
                    //MonitorTextBoxText(  " Freq Popup Show -> Window State: " + Convert.ToString(Window_controls.freq_form_state) );
                }
                else
                {
                    if (Window_controls.monitor_form_state == 1)
                    {
                        MonitorTextBoxText(" Monitor Popup State:" + Monitor_window.WindowState);
                        MonitorTextBoxText(" Monitor Popup Restoring");

                        Monitor_window.WindowState = FormWindowState.Normal;
                        Monitor_window.Activate();
                        Monitor_window.Show();
                        Monitor_window.Focus();
                    }
                    else
                    {
                        Monitor_window.Hide();
                        Window_controls.display_monitor_form = true;
                        MonitorTextBoxText(" Monitor Popup Hide");
                    }
                }
            }
            else
            {
                if (Monitor_window == null)
                {
                    MonitorTextBoxText(" Monitor Popup is NULL");
                }
                else
                {
                    MonitorTextBoxText(" Monitor Popup not NULL");
                }
                if (Monitor_window.IsDisposed)
                {
                    MonitorTextBoxText(" Monitor Popup is Disposed");
                    MonitorTextBoxText(" Monitor Popup -> Would close");
                    Monitor_window.Close();
                    Window_controls.freq_form_displosed = false;
                    Monitor_window = null;
                    Window_controls.display_monitor_form = true;
                }
                else
                {
                    MonitorTextBoxText(" Monitor Popup is not Disposed");
                }
            }
            MonitorTextBoxText(" Monitor Popup Finished");
        }

        private void Transvertercheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            short trans_toggle = 0;
            MonitorTextBoxText(" Transverter Called");
            if (oCode.transverter_warning_accepted)
            {
                if (oCode.trans_on == false)
                {
                    MonitorTextBoxText(" Transverter Warning Accepted, Turning Transverter ON");
                    trans_toggle = 1;
                    oCode.trans_on = true;
                }
                else
                {
                    MonitorTextBoxText(" Transverter Warning Accepted, Turning Transverter OFF");
                    trans_toggle = 0;
                    oCode.trans_on = false;
                }
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_TRANSVERTER, trans_toggle);
            }
            else
            {
                MonitorTextBoxText(" Transverter Posting Warning");
                Transvertercheckbox.Checked = false;
                if (!oCode.transverter_warning_displayed)
                {
                    DialogResult ret = MessageBox.Show("Enabling the Transverter Option extends the transmit frequency of the 10M band\n\r" +
                     "It is for use for use with transverters only\n\r" +
                     "I agree that Multus SDR, LLC has no liability for improper transmission that may extend beyond the 10M band\n\r" +
                     "Click YES if you agree.", "MSCC", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                    if (ret == DialogResult.Yes) oCode.transverter_warning_accepted = true;
                    oCode.transverter_warning_displayed = true;
                }
            }
            MonitorTextBoxText(" Transverter Finished");
        }

        /*private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
        }*/

        /*private void TXtextbox_TextChanged(object sender, EventArgs e)
        {
        }
        */

        public void Main_MouseWheel(object sender, MouseEventArgs e)
        {
            Int32 n = e.Delta;
            if (Mouse_controls.Allow_Frequency_Updates == false) return;

            if (n > 0) n = 1; else n = -1;      // convert mousewheel delta to either 1 or -1
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

            //MonitorTextBoxText(Convert.ToString(oCode.DisplayFreq) );
            if (oCode.DisplayFreq < 0) return;
            if (oCode.DisplayFreq > 40000000) oCode.DisplayFreq = 40000000;
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

        private void Set_Default_Digit()
        {
            switch (oCode.Freq_Tune_Index)
            {
                case 0:
                    oCode.FreqDigit = 5;
                    break;

                case 1:
                    oCode.FreqDigit = 4;
                    break;

                case 2:
                    oCode.FreqDigit = 3;
                    break;

                case 3:
                    oCode.FreqDigit = 2;
                    break;

                case 4:
                    oCode.FreqDigit = 1;
                    break;

                case 5:
                    oCode.FreqDigit = 0;
                    break;
            }
            MonitorTextBoxText(" Set_Default_Digit: " + Convert.ToString(oCode.Freq_Tune_Index));
        }

        private void Ones_MouseEnter(object send, EventArgs e)
        {
            oCode.FreqDigit = 0;
            Ones.ForeColor = Color.Yellow;
            //this.MouseDoubleClick += new MouseEventHandler(My_MouseClick);
            //My_MouseClick(null, null);
            //Ones.BackColor = Color.White;
        }

        private void Ones_MouseExit(object send, EventArgs e)
        {
            Set_Default_Digit();
            Ones.ForeColor = Settings.Default.Freq_Color;
        }

        private void Tens_MouseEnter(object send, EventArgs e)
        {
            oCode.FreqDigit = 1;
            Tens.ForeColor = Color.Yellow;
        }

        private void Tens_MouseExit(object send, EventArgs e)
        {
            Set_Default_Digit();
            Tens.ForeColor = Settings.Default.Freq_Color;
        }

        private void Hundreds_MouseEnter(object send, EventArgs e)
        {
            oCode.FreqDigit = 2;
            Hundreds.ForeColor = Color.Yellow;
        }

        private void Hundreds_MouseExit(object send, EventArgs e)
        {
            Set_Default_Digit();
            Hundreds.ForeColor = Settings.Default.Freq_Color;
        }

        private void Thousands_MouseEnter(object send, EventArgs e)
        {
            oCode.FreqDigit = 3;
            Thousands.ForeColor = Color.Yellow;
        }

        private void Thousands_MouseExit(object send, EventArgs e)
        {
            Set_Default_Digit();
            Thousands.ForeColor = Settings.Default.Freq_Color;
        }

        private void Tenthousands_MouseEnter(object send, EventArgs e)
        {
            oCode.FreqDigit = 4;
            Tenthousands.ForeColor = Color.Yellow;
        }

        private void Tenthousands_MouseExit(object send, EventArgs e)
        {
            Set_Default_Digit();
            Tenthousands.ForeColor = Settings.Default.Freq_Color;
        }

        private void Hundredthousand_MouseEnter(object send, EventArgs e)
        {
            oCode.FreqDigit = 5;
            Hundredthousand.ForeColor = Color.Yellow;
        }

        private void Hundredthousand_MouseExit(object send, EventArgs e)
        {
            Set_Default_Digit();
            Hundredthousand.ForeColor = Settings.Default.Freq_Color;
        }

        private void Millions_MouseEnter(object send, EventArgs e)
        {
            oCode.FreqDigit = 6;
            Millions.ForeColor = Color.Yellow;
        }

        private void Millions_MouseExit(object send, EventArgs e)
        {
            Set_Default_Digit();
            Millions.ForeColor = Settings.Default.Freq_Color;
        }

        private void Tenmillions_MouseEnter(object send, EventArgs e)
        {
            oCode.FreqDigit = 7;
            Tenmillions.ForeColor = Color.Yellow;
        }

        private void Tenmillions_MouseExit(object send, EventArgs e)
        {
            Set_Default_Digit();
            Tenmillions.ForeColor = Settings.Default.Freq_Color;
        }

        private void Tenthousands_Click(object sender, EventArgs e)
        {
        }

        private void Tenmillions_Click(object sender, EventArgs e)
        {
        }

        private void B160radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (Power_Calibration_Controls.Tuning_Mode)
            {
                DialogResult ret = MessageBox.Show("Band Change while in TUNE mode is not permitted\r\nTurn TUNE off", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (Power_Calibration_Controls.Previous_Band != 160)
            {
                //oCode.SendCommand(txsocket, txtarget, Power_Calibration_Controls.CMD_CALIBRATION_MASTER_RESET, 1);
                MonitorTextBoxText(" 160m Power Button Clicked");
                Power_Calibration_Controls.Band_Is_Selected = true;
                Power_Calibration_Controls.Previous_Band = 160;
                Power_Calibration_Controls.Band = 160;
                powerhScrollBar1.Value = 40;
                //powerlabel14.Text = 40 + " POWER";
                powerlabel14.Text = "WAIT";
                powerlabel14.ForeColor = Color.Red;
                Power_Calibration_Controls.Old_Power_Value = 40;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_BAND_POWER_BAND, (short)Power_Calibration_Controls.Band);
                //oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_BAND_POWER, (short)Power_Calibration_Controls.Band);
            }
        }

        private void B80radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (Master_Controls.Tuning_Mode)
            {
                DialogResult ret = MessageBox.Show("Band Change while in TUNE mode is not permitted\r\nTurn TUNE off", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (Power_Calibration_Controls.Previous_Band != 80)
            {
                //oCode.SendCommand(txsocket, txtarget, Power_Calibration_Controls.CMD_CALIBRATION_MASTER_RESET, 1);
                MonitorTextBoxText(" 80m Power Button Clicked");
                Power_Calibration_Controls.Band_Is_Selected = true;
                Power_Calibration_Controls.Previous_Band = 80;
                Power_Calibration_Controls.Band = 80;
                powerhScrollBar1.Value = 40;
                //powerlabel14.Text = 40 + " POWER";
                powerlabel14.Text = "WAIT";
                powerlabel14.ForeColor = Color.Red;
                Power_Calibration_Controls.Old_Power_Value = 40;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_BAND_POWER_BAND, (short)Power_Calibration_Controls.Band);
                //oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_BAND_POWER, (short)Power_Calibration_Controls.Band);
            }
        }

        private void B60radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (Master_Controls.Tuning_Mode)
            {
                DialogResult ret = MessageBox.Show("Band Change while in TUNE mode is not permitted\r\nTurn TUNE off", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (Power_Calibration_Controls.Previous_Band != 60)
            {
                //oCode.SendCommand(txsocket, txtarget, Power_Calibration_Controls.CMD_CALIBRATION_MASTER_RESET, 1);
                MonitorTextBoxText(" 60m Power Button Clicked");
                Power_Calibration_Controls.Band_Is_Selected = true;
                Power_Calibration_Controls.Previous_Band = 60;
                Power_Calibration_Controls.Band = 60;
                powerhScrollBar1.Value = 40;
                //powerlabel14.Text = 40 + " POWER";
                powerlabel14.Text = "WAIT";
                powerlabel14.ForeColor = Color.Red;
                Power_Calibration_Controls.Old_Power_Value = 40;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_BAND_POWER_BAND, (short)Power_Calibration_Controls.Band);
                //oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_BAND_POWER, (short)Power_Calibration_Controls.Band);
            }
        }

        private void B40radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (Master_Controls.Tuning_Mode)
            {
                DialogResult ret = MessageBox.Show("Band Change while in TUNE mode is not permitted\r\nTurn TUNE off", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (Power_Calibration_Controls.Previous_Band != 40)
            {
                //oCode.SendCommand(txsocket, txtarget, Power_Calibration_Controls.CMD_CALIBRATION_MASTER_RESET, 1);
                MonitorTextBoxText(" 40m Power Button Clicked");
                Power_Calibration_Controls.Band_Is_Selected = true;
                Power_Calibration_Controls.Previous_Band = 40;
                Power_Calibration_Controls.Band = 40;
                powerhScrollBar1.Value = 40;
                //powerlabel14.Text = 40 + " POWER";
                powerlabel14.Text = "WAIT";
                powerlabel14.ForeColor = Color.Red;
                Power_Calibration_Controls.Old_Power_Value = 40;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_BAND_POWER_BAND, (short)Power_Calibration_Controls.Band);
                //oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_BAND_POWER, (short)Power_Calibration_Controls.Band);
            }
        }

        private void B30radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (Master_Controls.Tuning_Mode)
            {
                DialogResult ret = MessageBox.Show("Band Change while in TUNE mode is not permitted\r\nTurn TUNE off", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (Power_Calibration_Controls.Previous_Band != 30)
            {
                //oCode.SendCommand(txsocket, txtarget, Power_Calibration_Controls.CMD_CALIBRATION_MASTER_RESET, 1);
                MonitorTextBoxText(" 30m Power Button Clicked");
                Power_Calibration_Controls.Band_Is_Selected = true;
                Power_Calibration_Controls.Previous_Band = 30;
                Power_Calibration_Controls.Band = 30;
                powerhScrollBar1.Value = 40;
                //powerlabel14.Text = 40 + " POWER";
                powerlabel14.Text = "WAIT";
                powerlabel14.ForeColor = Color.Red;
                Power_Calibration_Controls.Old_Power_Value = 40;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_BAND_POWER_BAND, (short)Power_Calibration_Controls.Band);
                //oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_BAND_POWER, (short)Power_Calibration_Controls.Band);
            }
        }

        private void B20radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (Master_Controls.Tuning_Mode)
            {
                DialogResult ret = MessageBox.Show("Band Change while in TUNE mode is not permitted\r\nTurn TUNE off", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (Power_Calibration_Controls.Previous_Band != 20)
            {
                //oCode.SendCommand(txsocket, txtarget, Power_Calibration_Controls.CMD_CALIBRATION_MASTER_RESET, 1);
                MonitorTextBoxText(" 20m Power Button Clicked\r\n");
                Power_Calibration_Controls.Band_Is_Selected = true;
                Power_Calibration_Controls.Previous_Band = 20;
                Power_Calibration_Controls.Band = 20;
                powerhScrollBar1.Value = 40;
                //powerlabel14.Text = 40 + " POWER";
                powerlabel14.Text = "WAIT";
                powerlabel14.ForeColor = Color.Red;
                Power_Calibration_Controls.Old_Power_Value = 40;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_BAND_POWER_BAND, (short)Power_Calibration_Controls.Band);
                //oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_BAND_POWER, (short)Power_Calibration_Controls.Band);
            }
        }

        private void B17radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (Master_Controls.Tuning_Mode)
            {
                DialogResult ret = MessageBox.Show("Band Change while in TUNE mode is not permitted\r\nTurn TUNE off", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (Power_Calibration_Controls.Previous_Band != 17)
            {
                //oCode.SendCommand(txsocket, txtarget, Power_Calibration_Controls.CMD_CALIBRATION_MASTER_RESET, 1);
                MonitorTextBoxText(" 170m Power Button Clicked");
                Power_Calibration_Controls.Band_Is_Selected = true;
                Power_Calibration_Controls.Previous_Band = 17;
                Power_Calibration_Controls.Band = 17;
                powerhScrollBar1.Value = 40;
                //powerlabel14.Text = 40 + " POWER";
                powerlabel14.Text = "WAIT";
                powerlabel14.ForeColor = Color.Red;
                Power_Calibration_Controls.Old_Power_Value = 40;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_BAND_POWER_BAND, (short)Power_Calibration_Controls.Band);
                //oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_BAND_POWER, (short)Power_Calibration_Controls.Band);
            }
        }

        private void B15radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (Master_Controls.Tuning_Mode)
            {
                DialogResult ret = MessageBox.Show("Band Change while in TUNE mode is not permitted\r\nTurn TUNE off", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (Power_Calibration_Controls.Previous_Band != 15)
            {
                //oCode.SendCommand(txsocket, txtarget, Power_Calibration_Controls.CMD_CALIBRATION_MASTER_RESET, 1);
                MonitorTextBoxText(" 15m Power Button Clicked");
                Power_Calibration_Controls.Band_Is_Selected = true;
                Power_Calibration_Controls.Previous_Band = 15;
                Power_Calibration_Controls.Band = 15;
                powerhScrollBar1.Value = 40;
                //powerlabel14.Text = 40 + " POWER";
                powerlabel14.Text = "WAIT";
                powerlabel14.ForeColor = Color.Red;
                Power_Calibration_Controls.Old_Power_Value = 40;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_BAND_POWER_BAND, (short)Power_Calibration_Controls.Band);
                //oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_BAND_POWER, (short)Power_Calibration_Controls.Band);
            }
        }

        private void B12radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (Master_Controls.Tuning_Mode)
            {
                DialogResult ret = MessageBox.Show("Band Change while in TUNE mode is not permitted\r\nTurn TUNE off", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (Power_Calibration_Controls.Previous_Band != 12)
            {
                //SendCommand(txsocket, txtarget, Power_Calibration_Controls.CMD_CALIBRATION_MASTER_RESET, 1);
                MonitorTextBoxText(" 12m Power Button Clicked");
                Power_Calibration_Controls.Band_Is_Selected = true;
                Power_Calibration_Controls.Previous_Band = 12;
                Power_Calibration_Controls.Band = 12;
                powerhScrollBar1.Value = 40;
                //powerlabel14.Text = 40 + " POWER";
                powerlabel14.Text = "WAIT";
                powerlabel14.ForeColor = Color.Red;
                Power_Calibration_Controls.Old_Power_Value = 40;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_BAND_POWER_BAND, (short)Power_Calibration_Controls.Band);
                //oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_BAND_POWER, (short)Power_Calibration_Controls.Band);
            }
        }

        private void B10radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (Master_Controls.Tuning_Mode)
            {
                DialogResult ret = MessageBox.Show("Band Change while in TUNE mode is not permitted\r\nTurn TUNE off", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (Power_Calibration_Controls.Previous_Band != 10)
            {
                //oCode.SendCommand(txsocket, txtarget, Power_Calibration_Controls.CMD_CALIBRATION_MASTER_RESET, 1);
                MonitorTextBoxText(" 10m Power Button Clicked");
                Power_Calibration_Controls.Band_Is_Selected = true;
                Power_Calibration_Controls.Previous_Band = 10;
                Power_Calibration_Controls.Band = 10;
                powerhScrollBar1.Value = 40;
                //powerlabel14.Text = 40 + " POWER";
                powerlabel14.Text = "WAIT";
                powerlabel14.ForeColor = Color.Red;
                Power_Calibration_Controls.Old_Power_Value = 40;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_BAND_POWER_BAND, (short)Power_Calibration_Controls.Band);
                //oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_BAND_POWER, (short)Power_Calibration_Controls.Band);
            }
        }

        private void label10_Click_1(object sender, EventArgs e)
        {
        }

        private void powertabPage1_Click(object sender, EventArgs e)
        {
            //MonitorTextBoxText("Power tab clicked \r\n");
        }

        private void MFC_Enter(object sender, EventArgs e)
        {
            MonitorTextBoxText(" MFC_Enter -> MFC tab entered");
            if (Solidus_Controls.Mia_Status)
            {
                AMP_groupBox3.Enabled = true;
                oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 1);
                //Solidus_Band_label4.Text = Convert.ToString(oCode.current_band) + " M";
                if (Master_Controls.Tuning_Mode || Master_Controls.Transmit_Mode)
                {
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_TX_ON, 0);
                    if (!Settings.Default.Speaker_MutED)
                    {
                        oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                    }
                }
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_RIG_TUNE, 0);
                MonitorTextBoxText(" MFC_Enter -> Current Band: " + oCode.current_band);
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, Mode.TUNE_mode);
                buttTune.BackColor = Color.Gainsboro;
                buttTune.ForeColor = Color.Black;
                Tune_vButton2.BackColor = Color.Gainsboro;
                Tune_vButton2.ForeColor = Color.Black;
                Tune_vButton2.Text = "TUN";
                button1.BackColor = Color.Gainsboro;
                button1.ForeColor = Color.Black;
                button1.Text = "PTT";
                buttTune.BackColor = Color.Gainsboro;
                buttTune.ForeColor = Color.Black;
                Master_Controls.PPT_Mode = false;
                Master_Controls.Transmit_Mode = false;
                Master_Controls.Two_Tone = false;
                oCode.SendCommand(txsocket, txtarget, Power_Controls.CMD_SET_TUNE_POWER, 100);
                AMP_Calibrate_hScrollBar1.Enabled = true;
                AMP_hScrollBar1.Enabled = true;
                Solidus_Bias_button8.Enabled = true;
                AMP_Tune_button4.Enabled = true;
                Amplifier_Power_Controls.Tab_Active = true;
                TabPage CurrrentTabProperty = powertabControl1.SelectedTab;
            }
#if RPI
            MonitorTextBoxText(" MFC_Enter -> Antenna_Switch: " +
                    Convert.ToString(RPi_Settings.Controls.Antenna_Switch));
            Antenna_Switch_comboBox1.SelectedIndex = RPi_Settings.Controls.Antenna_Switch;
#endif
        }

        private void MFC_Leave(object sender, EventArgs e)
        {
            byte[] buf = new byte[2];

            buf[0] = Master_Controls.Extended_Commands.CMD_SET_IQBD_MONITOR;
            if (Master_Controls.QRP_Mode)
            {
                oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 0);
            }
            else
            {
                oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 1);
            }
            oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_RIG_TUNE, 0);
            oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_TX_ON, 0);
            AMP_Tune_button4.ForeColor = Color.Black;
            AMP_Tune_button4.BackColor = Color.Gainsboro;
            Amplifier_Power_Controls.Tuning_Mode = false;
            AMP_Calibrate_hScrollBar1.Enabled = false;
            AMP_Tune_button4.Enabled = false;
            AMP_hScrollBar1.Enabled = false;
            Master_Controls.Transmit_Mode = false;
            Master_Controls.Tuning_Mode = false;
            Master_Controls.PPT_Mode = false;
            Amplifier_Power_Controls.Bias_On = false;
            //Amplifier_Power_Controls.Solidus_Band_Selected = false;
            button1.Enabled = true;
            buttTune.BackColor = Color.Gainsboro;
            buttTune.ForeColor = Color.Black;
            button1.BackColor = Color.Gainsboro;
            button1.ForeColor = Color.Black;
            Solidus_Bias_button8.BackColor = Color.Gainsboro;
            Solidus_Bias_button8.ForeColor = Color.Black;
            oCode.SendCommand(txsocket, txtarget, Amplifier_Power_Controls.CMD_GET_POTENTIA_BIAS, 0);
            AMP_label57.Text = "0 %";
            Amplifier_Power_Controls.Tab_Active = false;      
            oCode.SendCommand(txsocket, txtarget, Power_Controls.CMD_SET_TUNE_POWER, Power_Controls.TUNE_Power);
            IQBD_ONOFF.ForeColor = Color.Black;
            //IQBD_ONOFF.Text = "IQBD OFF";
            IQBD_Tune_button8.ForeColor = Color.Black;
            IQBD_Tune_button8.Text = "TUNE OFF";
            IQ_Controls.Tuning_Mode = false;
            buf[1] = 0;
            oCode.SendCommand_MultiByte(txsocket, txtarget,
                                    Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
            IQ_Controls.IQ_Calibrating = false;
            IQ_Controls.IQBD_MONITOR = false;
            oCode.previous_main_band = 0;
            MonitorTextBoxText(" MFC_Leave -> code.current_band: " + Convert.ToString(oCode.current_band));
            switch (oCode.current_band)
            {
                case 160:
                    main160radioButton10_CheckedChanged(null, null);
                    break;

                case 80:
                    main80radioButton9_CheckedChanged(null, null);
                    break;

                case 60:
                    main60radioButton8_CheckedChanged(null, null);
                    break;

                case 40:
                    main40radioButton7_CheckedChanged(null, null);
                    break;

                case 30:
                    main30radioButton6_CheckedChanged(null, null);
                    break;

                case 20:
                    main20radioButton5_CheckedChanged(null, null);
                    break;

                case 17:
                    main17radioButton4_CheckedChanged(null, null);
                    break;

                case 15:
                    main15radiobutton_CheckedChanged(null, null);
                    break;

                case 12:
                    main12radioButton2_CheckedChanged(null, null);
                    break;

                case 10:
                    main10radioButton1_CheckedChanged(null, null);
                    break;

                default:
                    genradioButton_CheckedChanged(null, null);
                    break;
            }

            MonitorTextBoxText(" MFC tab leave");
            Int32 CurrentTabIndex = powertabControl1.SelectedIndex;
            MonitorTextBoxText(" Tab index is " + Convert.ToString(CurrentTabIndex));
            TabPage CurrrentTabProperty = powertabControl1.SelectedTab;
        }

        private void powertabPage1_Enter(object sender, EventArgs e)
        {
            MonitorTextBoxText(" Power tab entered");
            oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 0);
            Power_Controls.Previous_Tune_power = Power_Controls.TUNE_Power;
            oCode.SendCommand(txsocket, txtarget, Power_Controls.CMD_SET_TUNE_POWER, 100);
            Power_Calibration_Controls.Power_Tab_Active = true;
            TabPage CurrrentTabProperty = powertabControl1.SelectedTab;
            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 1);
            if (Power_Calibration_Controls.Band != 0)
            {
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_BAND_POWER_BAND, (short)Power_Calibration_Controls.Band);
            }
        }

        private void mainPage_Enter(object sender, EventArgs e)
        {
            MonitorTextBoxText(" Main tab entered");
#if RPI
            this.Size = Main_tab_size;
            this.Location = RPI_location;

#endif
            Master_Controls.Current_tab = powertabControl1.SelectedTab;
            Mouse_controls.Allow_Frequency_Updates = true;
            Master_Controls.Main_Tab_Active = true;
            
            //powertabPage1.BackColor = Color.Red;
            //CurrrentTabProperty.
        }

        private void mainPage_Leave(object sender, EventArgs e)
        {
            MonitorTextBoxText(" Main tab Leave");
#if RPI
            this.Size = Full_size;
            //UTC_Date_label46.Visible = false;
            //Time_display_UTC_label34.Visible = false;
#endif
            TabPage CurrrentTabProperty = powertabControl1.SelectedTab;
            Mouse_controls.Allow_Frequency_Updates = false;
            Master_Controls.Main_Tab_Active = false;
            //powertabPage1.BackColor = Color.Red;
            //CurrrentTabProperty.
        }

        private void powertabPage1_Leave(object sender, EventArgs e)
        {
            //short mode_number = 0;
            if (Power_Calibration_Controls.Update_Pending)
            {
                DialogResult ret = MessageBox.Show("There is an Uncommited Power Calibration Value\r\n" + "" +
                    "Do you wish Commit?", "MSCC", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                if (ret == DialogResult.Yes)
                {
                    oCode.SendCommand(txsocket, txtarget, Power_Calibration_Controls.CMD_SET_COMMIT_POWER_VALUES, 1);
                }
            }
            Power_Calibration_Controls.Update_Pending = false;
            //Power_Calibration_Controls.Band_Is_Selected = false;
            if (Power_Calibration_Controls.Tuning_Mode)
            {
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_RIG_TUNE, 0);
            }

            //Power_Calibration_Controls.Band = 0;
            powertunebutton1.ForeColor = Control.DefaultForeColor;
            powertunebutton1.Text = "TX OFF";
            powertunebutton1.FlatStyle = FlatStyle.Standard;
            powerhScrollBar1.Value = 40;
            powerlabel14.Text = "NO VALUE";
            Power_Calibration_Controls.Old_Power_Value = 40;
            Power_Calibration_Controls.New_Power_Value = 40;
            Power_Calibration_Controls.Previous_Band = 0;
            Power_Calibration_Controls.Power_Tab_Active = false;
            Power_Controls.TUNE_Power = (short)Power_Controls.Previous_Tune_power;
            MonitorTextBoxText(" powertabPage1_Leave -> code.current_band: " + Convert.ToString(oCode.current_band));
            oCode.previous_main_band = 0;
            switch (oCode.current_band)
            {
                case 160:
                    main160radioButton10_CheckedChanged(null, null);

                    break;

                case 80:
                    main80radioButton9_CheckedChanged(null, null);

                    break;

                case 60:
                    main60radioButton8_CheckedChanged(null, null);

                    break;

                case 40:
                    main40radioButton7_CheckedChanged(null, null);

                    break;

                case 30:
                    main30radioButton6_CheckedChanged(null, null);

                    break;

                case 20:
                    main20radioButton5_CheckedChanged(null, null);

                    break;

                case 17:
                    main17radioButton4_CheckedChanged(null, null);

                    break;

                case 15:
                    main15radiobutton_CheckedChanged(null, null);

                    break;

                case 12:
                    main12radioButton2_CheckedChanged(null, null);

                    break;

                case 10:
                    main10radioButton1_CheckedChanged(null, null);

                    break;

                default:
                    genradioButton_CheckedChanged(null, null);
                    break;
            }

            Thread.Sleep(500);//Prevents clicks in Speaker Audio
            if (!Settings.Default.Speaker_MutED)
            {
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
            }
            oCode.SendCommand(txsocket, txtarget, Power_Calibration_Controls.CMD_CALIBRATION_MASTER_RESET, 1);

            //CurrrentTabProperty.BackColor = Color.White;
            if (Master_Controls.QRP_Mode)
            {
                oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 0);
            }
            else
            {
                oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 1);
            }
            MonitorTextBoxText(" Power tab leave");
            Int32 CurrentTabIndex = powertabControl1.SelectedIndex;
            MonitorTextBoxText(oCode.line_count + " Tab index is " + Convert.ToString(CurrentTabIndex));
            TabPage CurrrentTabProperty = powertabControl1.SelectedTab;
        }

        private void metertab_Click(object sender, EventArgs e)
        {
        }

        private void Ones_Click(object sender, EventArgs e)
        {
        }

        private void Millions_Click(object sender, EventArgs e)
        {
        }

        private void LefthScrollBar1_Scroll_1(object sender, ScrollEventArgs e)
        {
            if (oCode.isLoading == true) return;
            int value = LefthScrollBar1.Value;
            if (value == IQ_Controls.IQ_Offset) return;
            if (IQ_Controls.IQ_RX_MODE_ACTIVE)
            {
                IQ_Controls.IQ_Offset = value;
                IQLefttextBox2.Text = Convert.ToString(IQ_Controls.IQ_Offset);
                MonitorTextBoxText("LefthScrollBar1_Scroll RX IQ_Controls.Volume:  " + (short)IQ_Controls.IQ_Offset);
                oCode.SendCommand32(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_OFFSET, (IQ_Controls.IQ_Offset));
            }
            else
            {
                if (IQ_Controls.IQ_TX_MODE_ACTIVE)
                {
                    IQ_Controls.IQ_Offset = value;
                    IQLefttextBox2.Text = Convert.ToString(IQ_Controls.IQ_Offset);
                    MonitorTextBoxText("LefthScrollBar1_Scroll TX IQ_Controls.Volume:  " + (short)IQ_Controls.IQ_Offset);
                    oCode.SendCommand32(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_OFFSET, (IQ_Controls.IQ_Offset));
                }
                else
                {
                    DialogResult ret = MessageBox.Show("Select a Band", "MSCC",
                                  MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }

        private void label15_Click(object sender, EventArgs e)
        {
        }

        private void label36_Click(object sender, EventArgs e)
        {
        }

        private void IQLefttextBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void LeftResetbutton2_Click_1(object sender, EventArgs e)
        {
            LefthScrollBar1.Value = 0;
            IQLefttextBox2.Text = "0";
            LefthScrollBar1_Scroll_1(null, null);
        }

        private void HightextBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void label34_Click(object sender, EventArgs e)
        {
        }

        private void powertabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void freqcaltab_Click(object sender, EventArgs e)
        {
        }

        private void freqcaltab_Enter(object sender, EventArgs e)
        {
            IQ_Controls.Tab_Active = true;
            oCode.previous_main_band = 0;
            MonitorTextBoxText(" freqcaltab_Enter -> entered");
            TabPage CurrrentTabProperty = powertabControl1.SelectedTab;
            oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_LAST_USED_FREQ, oCode.current_band);
            IQ_Controls.Tune_power = Power_Controls.TUNE_Power;
            //IQ_Controls.IQ_TX_MODE_ACTIVE = false;
            //IQ_Controls.IQ_RX_MODE_ACTIVE = false;
        }

        private void freqcaltab_Leave(object sender, EventArgs e)
        {
            byte[] buf = new byte[2];
            MonitorTextBoxText(" freqcaltab_Leave -> Entered");
            if (IQ_Controls.IQ_Calibrating || Frequency_Calibration_controls.Calibration_In_Progress)
            {
                buf[0] = Master_Controls.Extended_Commands.CMD_CALIBRATION_CANCEL;
                buf[1] = 0;
                oCode.SendCommand_MultiByte(txsocket, txtarget,
                                                    Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
            }
            IQ_Controls.IQ_RX_MODE_ACTIVE = false;
            oCode.SendCommand(txsocket, txtarget, Power_Controls.CMD_SET_TUNE_POWER, Power_Controls.TUNE_Power);
            if (IQ_Controls.Tuning_Mode)
            {
                oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 0);
                IQ_Tune_button2.ForeColor = Control.DefaultForeColor;
                IQ_Tune_button2.Text = "TUNE";
                //buttTune_Click(1, null);
            }
            Frequency_Calibration_controls.Reset = false;
            IQ_TX_button.BackColor = Color.Gainsboro;
            IQ_TX_button.ForeColor = Color.Black;
            IQ_TX_button.Visible = true;
            IQ_Tune_button2.Visible = true;
            IQ_RX_button.BackColor = Color.Gainsboro;
            IQ_RX_button.ForeColor = Color.Black;
            IQ_RX_button.Visible = true;
            calibratebutton1.Text = "CALIBRATE";

            IQ_Controls.Tuning_Mode = false;
            IQ_Controls.IQ_Offset = 0;
            IQ_Controls.IQ_RX_Offset = 0;
            IQ_Controls.RX_toggle = false;
            IQ_Controls.IQ_RX_MODE_ACTIVE = false;
            IQ_Controls.band_selected = false;
            IQ_Controls.TX_toggle = false;
            IQ_Controls.IQ_TX_MODE_ACTIVE = false;
            if (IQ_Controls.Up_24KHz)
            {
                IQ_UP24KHz_checkBox2_CheckedChanged(null, null);
                IQ_UP24KHz_checkBox2.Checked = false;
                IQ_Controls.Up_24KHz = false;
                IQ_UP24KHz_checkBox2.BackColor = Color.Gainsboro;
                IQ_UP24KHz_checkBox2.ForeColor = Color.Green;
            }
            IQ_Freq_hScrollBar1.Value = 0;
            IQLefttextBox2.Text = "0";
            LefthScrollBar1.Value = 0;
            Frequency_Calibration_controls.standard_carrier = 0;
            label57.Text = "STANDARD CARRIER";
            Int32 CurrentTabIndex = powertabControl1.SelectedIndex;
            MonitorTextBoxText(oCode.line_count + " Tab index is " + Convert.ToString(CurrentTabIndex));
            TabPage CurrrentTabProperty = powertabControl1.SelectedTab;
            IQ_Controls.Tab_Active = false;
            groupBox2.Enabled = true;
            groupBox2.Visible = true;
            IQ_groupBox3.Visible = true;
            IQ_groupBox3.Enabled = true;
            Freq_Cal_groupBox4.Enabled = true;
            Freq_Cal_groupBox4.Visible = true;
            IQ_RX_button.Visible = true;
            IQ_Tune_button2.Visible = false;
            IQ_Commit_button2.Visible = false;
            LefthScrollBar1.Visible = false;
            LeftResetbutton2.Visible = false;
            IQ_Reset_All_button2.Visible = false;
            groupBox2.Visible = false;
            IQLefttextBox2.Visible = false;
            calibratebutton1.Visible = false;
            Calibration_progressBar1.Visible = false;
            Calibration_progressBar1.Value = 0;
            Cal_Freq_textBox2.Visible = false;
            IQ_Freq_hScrollBar1.Visible = false;
            IQ_UP24KHz_checkBox2.Visible = false;
            Reset_Freq_button3.Visible = false;
            Freq_Check_Button.Visible = false;
            Freq_Cal_Reset_button4.Visible = false;
            Freq_Cal_checkBox3.Visible = false;
            Freq_CAl_Progress_Lable.Visible = false;
            IQ_Tune_Power_hScrollBar1.Visible = false;

            if (Master_Controls.QRP_Mode)
            {
                oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 0);
            }
            else
            {
                oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 1);
            }
            oCode.SendCommand(txsocket, txtarget, Power_Controls.CMD_SET_TUNE_POWER, Power_Controls.TUNE_Power);
            buf[0] = Master_Controls.Extended_Commands.CMD_SET_IQBD_MONITOR;
            buf[1] = 0;
            oCode.SendCommand_MultiByte(txsocket, txtarget,
                                                Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
            Frequency_Calibration_controls.Calibration_Checked = true;
            switch (oCode.current_band)
            {
                case 160:
                    main160radioButton10_CheckedChanged(null, null);

                    break;

                case 80:
                    main80radioButton9_CheckedChanged(null, null);

                    break;

                case 60:
                    main60radioButton8_CheckedChanged(null, null);

                    break;

                case 40:
                    main40radioButton7_CheckedChanged(null, null);

                    break;

                case 30:
                    main30radioButton6_CheckedChanged(null, null);

                    break;

                case 20:
                    main20radioButton5_CheckedChanged(null, null);

                    break;

                case 17:
                    main17radioButton4_CheckedChanged(null, null);

                    break;

                case 15:
                    main15radiobutton_CheckedChanged(null, null);

                    break;

                case 12:
                    main12radioButton2_CheckedChanged(null, null);

                    break;

                case 10:
                    main10radioButton1_CheckedChanged(null, null);

                    break;

                default:
                    genradioButton_CheckedChanged(null, null);
                    break;
            }
            MonitorTextBoxText(" freqcaltab_Leave -> Finished");
        }

        private void Bw_label2_Click(object sender, EventArgs e)
        {
        }

        private void Thousands_Click(object sender, EventArgs e)
        {
        }

        private void Hundredthousand_Click(object sender, EventArgs e)
        {
        }

        private void Hundreds_Click(object sender, EventArgs e)
        {
        }

        private void Tens_Click(object sender, EventArgs e)
        {
        }

        /*private void label38_Click(object sender, EventArgs e)
        {
        }
        */
        private void label17_Click(object sender, EventArgs e)
        {
        }

        /*private void label1_Click(object sender, EventArgs e)
        {
        }
        */
        private void band_stack_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void label31_Click(object sender, EventArgs e)
        {
        }

        private void label30_Click(object sender, EventArgs e)
        {
        }

        private void band_stack_textBox1_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void label28_Click(object sender, EventArgs e)
        {
        }

        private void label27_Click(object sender, EventArgs e)
        {
        }

        private void label26_Click(object sender, EventArgs e)
        {
        }

        private void label25_Click(object sender, EventArgs e)
        {
        }

        private void label24_Click(object sender, EventArgs e)
        {
        }

        private void label23_Click(object sender, EventArgs e)
        {
        }

        private void label22_Click(object sender, EventArgs e)
        {
        }

        private void label21_Click(object sender, EventArgs e)
        {
        }

        /*private void cwPage_Click(object sender, EventArgs e)
        {
        }

        private void CWpitchlabel36_Click(object sender, EventArgs e)
        {
        }

        private void label35_Click(object sender, EventArgs e)
        {
        }

        private void label8_Click(object sender, EventArgs e)
        {
        }

        private void label11_Click(object sender, EventArgs e)
        {
        }*/

        private void label13_Click(object sender, EventArgs e)
        {
        }

        /*private void IQRighttextBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void label18_Click_1(object sender, EventArgs e)
        {
        }

        private void label14_Click(object sender, EventArgs e)
        {
        }

        private void iqlabel32_Click(object sender, EventArgs e)
        {
        }

        private void PANADAPTER_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }*/

        private void label3_Click(object sender, EventArgs e)
        {
        }

        /*private void label2_Click_1(object sender, EventArgs e)
        {
        }*/

        private void Volume_hScroolBar1_mouseenter(object sender,EventArgs e)
        {
            Volume_hScrollBar1.Focus();
        }

        private void Volume_hScroolBar1_mouseleave(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void Volume_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value;
            if (oCode.isLoading == true) return;
            value = Volume_hScrollBar1.Value;
            if (Settings.Default.Speaker_Volume == value) return;

#if FTDI
            Tuning_Knob_Controls.Volume_Function.Volume_Slider_Active = true;
#endif
            Settings.Default.Speaker_Volume = value;
            RPi_Settings.Volume_Settings.Speaker_Volume = value;
            //RPi_Settings.RPi_Needs_Updated = true;

#if FTDI
            Tuning_Knob_Controls.Volume_Function.Volume = Volume_Controls.Speaker_Volume;
#endif
            Volume_textBox2.Text = Convert.ToString(Settings.Default.Speaker_Volume);

            if (Settings.Default.Speaker_MutED)
            {
                Volume_Mute_button2.ForeColor = Control.DefaultForeColor;
                Volume_Mute_button2.Text = "Volume";
                Volume_Mute_button2.FlatStyle = FlatStyle.Standard;
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                Settings.Default.Speaker_MutED = false;
                RPi_Settings.Volume_Settings.Speaker_Mute = 0;
                //RPi_Settings.RPi_Needs_Updated = true;
            }
            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_VOLUME, (short)Settings.Default.Speaker_Volume);
#if FTDI
            Tuning_Knob_Controls.Volume_Function.Volume_Slider_Active = false;
#endif
        }

        /*private void Filter3_radioButton2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void Filter5_radioButton4_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void Filter1_radioButton1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void label5_Click(object sender, EventArgs e)
        {
        }

        private void Reset_Filter_listBox1(short index)
        {
            MonitorTextBoxText(" Reset_Filter_listBox1 -> index: " + index);
            Filter_listBox1.SetSelected(index, true);
        }

        private void Filter_Low_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            Filter_control.Filter_Low_Index = (short)Filter_Low_listBox1.SelectedIndex;
            if (Filter_control.Previous_Filter_Low_Index == Filter_control.Filter_Low_Index) return;
            Filter_control.Previous_Filter_Low_Index = Filter_control.Filter_Low_Index;
            switch (oCode.current_band)
            {
                case 160:
                    Last_used.B160.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 80:
                    Last_used.B80.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 60:
                    Last_used.B60.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 40:
                    Last_used.B40.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 30:
                    Last_used.B30.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 20:
                    Last_used.B20.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 15:
                    Last_used.B15.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 17:
                    Last_used.B17.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 12:
                    Last_used.B12.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 10:
                    Last_used.B10.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                default:
                    Last_used.GEN.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;
            }
            oCode.SendCommand(txsocket, txtarget, Filter_control.CMD_SET_BW_LOCUT, Filter_control.Filter_Low_Index);
        }
        */

        /*public void Set_Waterfall_Size(Size new_size)
        {
            var thisForm2 = Application.OpenForms.OfType<Waterfall_form>().Single();
            //Main_Smeter_controls.SMeter_value = value;
            thisForm2.Resize_From_Main(new_size);
        }*/

        /*public void SetMeter(Int32 meterValue)
        {
            Smeter_controls.smeter_value = meterValue;
            if (Master_Controls.Debug_Display)
            {
                Write_Debug_Message(" SetMeter -> meterValue: " + Convert.ToString(meterValue));
            }
        }*/

        public void Update_ALC_Meter(Int32 value)
        {
            Set_ALC_Meter(value);
        }

        public void Update_Main_Display()
        {
            int MHz;
            int KHz;
            int Hz;
            int freq_plus_rit;

            Rit_Controls.Rit_Freq = oCode.DisplayFreq;
            freq_plus_rit = Rit_Controls.Rit_Freq + Rit_Controls.Offset;
            Rit_Controls.Rit_Freq_Plus_Offset = Rit_Controls.Rit_Freq + Rit_Controls.Offset;

            MHz = freq_plus_rit / 1000000;
            KHz = (freq_plus_rit - (MHz * 1000000)) / 1000;
            Hz = freq_plus_rit - (MHz * 1000000) - (KHz * 1000);
            ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
            //oCode.SendCommand32(txsocket, txtarget, Rit_Controls.CMD_SET_RIT_FREQ, Rit_Controls.Offset);
            Display_Main_Freq();
        }

        /*private void Smeter_button2_Click(object sender, EventArgs e)
        {
            if (sender == null)
            {
                MonitorTextBoxText(" S Meter Popup Entered -> NULL sender");
                Window_controls.smeter_window_function_by_user = false;
            }
            else
            {
                MonitorTextBoxText(" S Meter Popup Entered -> Button Clicked");
                Window_controls.smeter_window_function_by_user = true;
            }

            if (Smeter_window != null)
            {
                MonitorTextBoxText("  S Meter Popup is not null");
                Window_controls.smeter_form_displosed = Smeter_window.IsDisposed;
                if (Window_controls.smeter_form_displosed)
                {
                    MonitorTextBoxText(" S Meter Popup Disposed");
                    //oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_SMETER, 0);
                }
                else
                {
                    MonitorTextBoxText(" S Meter Popup Not disposed");
                }
            }
            if (Smeter_window == null)
            {
                Smeter_window = new Smeter_Form1();
                MonitorTextBoxText(" S Meter Popup Created");
                Window_controls.smeter_form_displosed = false;
                Window_controls.smeter_created = true;
                Smeter_window.MinimizeBox = false;
            }
            if (Smeter_window != null && !Window_controls.smeter_form_displosed)
            {
                Window_controls.smeter_form_state = (int)Smeter_window.WindowState;
                MonitorTextBoxText(" S Meter Popup State: " + Smeter_window.WindowState + " " + Convert.ToString(Smeter_window.WindowState));

                if (Window_controls.display_smeter_form)
                {
                    Window_controls.smeter_display_visable = true;
                    //Window_controls.smeter_minimized = false;
                    Window_controls.smeter_displayed = true;
                    Window_controls.display_smeter_form = false;
                    Window_controls.smeter_hidden = false;
                    Smeter_window.WindowState = FormWindowState.Normal;
                    Smeter_window.Show();
                    MonitorTextBoxText(" S Meter Popup Show");
                    Window_controls.smeter_form_state = (int)Smeter_window.WindowState;
                    //oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_SMETER, 1);
                }
                else
                {
                    if (Window_controls.smeter_form_state == 1)
                    {
                        MonitorTextBoxText(" S Meter Popup State:" + Smeter_window.WindowState);
                        MonitorTextBoxText(" S Meter Popup Restoring \r\n");
                        //Window_controls.smeter_minimized = false;
                        Window_controls.smeter_display_visable = true;
                        Window_controls.smeter_hidden = false;
                        Smeter_window.WindowState = FormWindowState.Normal;
                        Smeter_window.Activate();
                        Smeter_window.Show();
                        Smeter_window.Focus();
                        //oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_SMETER, 1);
                        Window_controls.smeter_displayed = true;
                    }
                    else
                    {
                        Window_controls.smeter_display_visable = false;
                        Window_controls.display_smeter_form = true;
                        //Window_controls.smeter_minimized = true;
                        Window_controls.smeter_hidden = true;
                        Window_controls.smeter_displayed = false;
                        Smeter_window.WindowState = FormWindowState.Minimized;
                        MonitorTextBoxText(" S Meter Popup Minimized");
                    }
                }
            }
            else
            {
                if (Smeter_window == null)
                {
                    MonitorTextBoxText(" S Meter Popup is NULL");
                }
                else
                {
                    MonitorTextBoxText(" S Meter Popup not NULL");
                }
                if (Smeter_window.IsDisposed)
                {
                    MonitorTextBoxText(" S Meter Popup is Disposed");
                    MonitorTextBoxText(" S Meter Popup -> Would close");
                    Smeter_window.Close();
                    //Window_controls.freq_form_displosed = false;
                    Smeter_window = null;
                    Window_controls.display_smeter_form = true;
                    Window_controls.smeter_displayed = false;
                }
                else
                {
                    MonitorTextBoxText(" S Meter Popup is not Disposed");
                }
            }
            MonitorTextBoxText(" S Meter Popup Finished");
        }*/

        /*private void Smeter_button2_Click(object sender, EventArgs e)
        {
            Point S_meter_location;

            MonitorTextBoxText(" S Meter Popup Entered");
            if (sender == null)
            {
                MonitorTextBoxText(" S Meter Popup Entered -> NULL sender");
                Window_controls.smeter_button_clicked = false;
            }
            else
            {
                MonitorTextBoxText(" S Meter Popup Entered -> Button Clicked");
                Window_controls.smeter_button_clicked = true;
            }
            if (Smeter_window != null)
            {
                MonitorTextBoxText("  S Meter Popup is not null");
                Window_controls.smeter_form_displosed = Smeter_window.IsDisposed;
                if (Window_controls.smeter_form_displosed)
                {
                    MonitorTextBoxText(" S Meter Popup Disposed");
                    //oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_SMETER, 0);
                }
                else
                {
                    MonitorTextBoxText(" S Meter Popup Not disposed");
                }
            }
            if (Smeter_window == null)
            {
                Smeter_window = new Smeter_Form1();
                MonitorTextBoxText(" S Meter Popup Created");
                if (Smeter_window != null)
                {
                    Smeter_window.WindowState = FormWindowState.Normal;
                    Smeter_window.Activate();
                    Smeter_window.Show();
                    Smeter_window.Focus();
                    Window_controls.smeter_form_displosed = false;
                    Window_controls.smeter_created = true;
                    MonitorTextBoxText(" Smeter_button2_Click -> Main Window Location: " + Convert.ToString(this.Location));
                    S_meter_location = this.Location;
                    S_meter_location.X += 4;
                    S_meter_location.Y += 25;
                    Smeter_window.Location = S_meter_location;
                }
            }
            if (Smeter_window != null && !Window_controls.smeter_form_displosed)
            {
                Window_controls.smeter_form_state = (int)Smeter_window.WindowState;
                MonitorTextBoxText(" S Meter Popup State: " + Smeter_window.WindowState);
                if (!Window_controls.smeter_first_pass)
                {
                    MonitorTextBoxText(" S Meter Popup State: -> smeter_first_pass: " +
                                                        Convert.ToString(Window_controls.smeter_first_pass));
                    if (Smeter_window.WindowState == FormWindowState.Minimized)
                    {
                        MonitorTextBoxText(" S Meter Popup is Minimized");
                        Smeter_window.WindowState = FormWindowState.Normal;
                        //Smeter_window.Show();
                        Window_controls.smeter_displayed = true;
                        Window_controls.display_smeter_form = false;
                        MonitorTextBoxText(" S Meter Popup Now Normal");
                        if (Window_controls.smeter_button_clicked)
                        {
                            Window_controls.smeter_window_normallized_by_user = true;
                            Window_controls.smeter_window_minimized_by_user = false;
                        }
                        else
                        {
                            Window_controls.smeter_window_normallized_by_user = false;
                        }
                        Window_controls.smeter_form_state = (int)Smeter_window.WindowState;
                    }
                    else
                    {
                        if (Smeter_window.WindowState == FormWindowState.Normal)
                        {
                            MonitorTextBoxText(" S Meter Popup is Normal");
                            MonitorTextBoxText(" S Meter Popup State:" + Smeter_window.WindowState);
                            Smeter_window.WindowState = FormWindowState.Minimized;
                            Window_controls.smeter_displayed = false;
                            if (Window_controls.smeter_button_clicked)
                            {
                                Window_controls.smeter_window_minimized_by_user = true;
                            }
                            else
                            {
                                Window_controls.smeter_window_minimized_by_user = false;
                            }
                            MonitorTextBoxText(" S Meter Popup Now Minimized");
                        }
                    }
                }
            }
            else
            {
                if (Smeter_window == null)
                {
                    MonitorTextBoxText(" S Meter Popup is NULL");
                }
                else
                {
                    MonitorTextBoxText(" S Meter Popup not NULL");
                }
                if (Smeter_window.IsDisposed)
                {
                    MonitorTextBoxText(" S Meter Popup is Disposed");
                    MonitorTextBoxText(" S Meter Popup -> Would close");
                    Smeter_window.Close();
                    //Window_controls.freq_form_displosed = false;
                    Smeter_window = null;
                    Window_controls.display_smeter_form = true;
                    Window_controls.smeter_displayed = false;
                }
                else
                {
                    MonitorTextBoxText(" S Meter Popup is not Disposed");
                }
            }
            Window_controls.smeter_first_pass = false;
            MonitorTextBoxText(" S Meter Popup Finished");
        }*/

        private void Time_display_label33_Click(object sender, EventArgs e)
        {
        }

        private void Keep_Alive_timer1_Tick(object sender, EventArgs e)
        {
            int seconds;
            seconds = DateTime.Now.Second;
            if (Master_Controls.previous_second != seconds)
            {
                Time_display_label33.Text = DateTime.Now.ToString("HH:mm:ss");
                Local_Date_label46.Text = DateTime.Now.ToString("MM.dd.yy");
                Time_display_UTC_label34.Text = DateTime.UtcNow.ToString("HH:mm:ss");
                UTC_Date_label46.Text = DateTime.UtcNow.ToString("MM.dd.yy");
#if !RPI
                try
                {
                    Win32.SetThreadExecutionState(Win32.ES_CONTINUOUS | Win32.ES_SYSTEM_REQUIRED);
                } catch (Exception es)
                {
                    MonitorTextBoxText(" SetThreadExecutionState FAILED: " + Convert.ToString(es));
                }
#endif

                if (Master_Controls.Keep_Alive_Pulse++ > 1)
                {
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_KEEP_ALIVE, 1);
                    Master_Controls.Keep_Alive_Pulse = 0;
                }
                Master_Controls.previous_second = seconds;
                if (Master_Controls.MSSDR_running)
                {
                    if (Master_Controls.Post_Init == false)
                    {
                        Post_Initialization();
                        Master_Controls.Post_Init = true;
#if RPI
                        RPi_Display_Timer.Enabled = true;
                        this.Location = RPI_location;
#endif
                    }
                    if (!Master_Controls.Step_Sent)
                    {
                        oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_STEP_VALUE, oCode.Freq_Tune_Index);
                        Master_Controls.Step_Sent = true;
                    }

                    //MonitorTextBoxText(" Sdrcore_running ");
                    if (++Master_Controls.Keep_Alive_Counter >= 15)
                    {
                        MonitorTextBoxText(" Keep Alive Counter: " + Convert.ToString(Master_Controls.Keep_Alive_Counter));
#if RPI
                        Update_RPi_Settings();
#endif
                        if (Master_Controls.Keep_Alive == false)
                        {
                            Keep_Alive_timer.Stop();
                            Keep_Alive_timer.Enabled = false;
                            MessageBox.Show("Server did not send Keep Alive.  MSCC will now shut down.", "MSCC",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }
                        Master_Controls.Keep_Alive_Counter = 0;
                        Master_Controls.Keep_Alive = false;
                    }
                }
#if FTDI
                if (Relay_Board_Controls.Write_failure)
                {
                    Relay_Board_Controls.Write_failure = false;
                    MessageBox.Show("Relay Board Write FAILED", "MSCC", MessageBoxButtons.OK,
                               MessageBoxIcon.Asterisk);
                    return;
                }
                if (Relay_Board_Controls.Open)
                {
                    if (Relay_Board_Controls.Previous_Relay != Relay_Board_Controls.sentBytes[0])
                    {
                        Relay_Board_Controls.Status = Relay_Board_Controls.Device.Write(Relay_Board_Controls.sentBytes,
                                                                           1, ref Relay_Board_Controls.Received_Bytes);
                        if (Relay_Board_Controls.Status != FTDI.FT_STATUS.FT_OK)
                        {
                            Relay_Board_Controls.On = false;
                            Relay_Board_Controls.Status = Relay_Board_Controls.Device.Close();
                            Relay_Board_Controls.Open = false;
                            Master_Controls.code_triggered = true;
                            Relay_Board_checkBox2.Checked = false;
                            Master_Controls.code_triggered = false;
                            Relay_Board_Controls.Write_failure = true;
                        }
                        Relay_Board_Controls.Previous_Relay = Relay_Board_Controls.sentBytes[0];
                    }
                }
#endif
            }
        }

        private void TX_Bandwidth_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            Filter_control.Filter_TX_High_Index = (short)TX_Bandwidth_listBox1.SelectedIndex;
            MonitorTextBoxText(" Set TX Bandwidth called: " + Filter_control.Filter_TX_High_Index);
            int value = Filter_control.Filter_TX_High_Index;
            //if (value == Power_Controls.Previous_TX_Bandwidth) return;
            Power_Controls.Previous_TX_Bandwidth = value;
            if (value > 1 && Power_Controls.Tx_Warning == false)
            {
                DialogResult ret = MessageBox.Show("This sets the TX bandwidth greater than normal operating standards", "MSCC",
                                                                                          MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                if (ret == DialogResult.Yes)
                {
                    Power_Controls.Tx_Warning = true;
                    MonitorTextBoxText(" Set TX Bandwidth Warning accepted");
                    oCode.SendCommand(txsocket, txtarget, Filter_control.CMD_SET_TX_HICUT, (short)value);
                }
                else
                {
                    TX_Bandwidth_listBox1.SelectedIndex = Power_Controls.Previous_TX_Bandwidth;
                }
            }
            else
            {
                oCode.SendCommand(txsocket, txtarget, Filter_control.CMD_SET_TX_HICUT, (short)value);
            }
            MonitorTextBoxText(" Set TX Bandwidth finished");
            //oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_TX_HICUT, Filter_control.Filter_TX_High_Index);
        }

        private void Power_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value;

            if (oCode.isLoading == true) return;
            if (Master_Controls.code_triggered)
            {
                value = Power_Controls.Main_Tab_Power;
                Power_hScrollBar1.Value = value;
            }
            else
            {
                value = Power_hScrollBar1.Value;
            }
            if (Power_Controls.Main_Power == (short)value) return;
            Power_Controls.Main_Power = (short)value;
            SSB_Power_label36.Text = Convert.ToString(value) + " %";
            Volume_Controls.Speaker_SSB_Value = (short)value;
            if (!Master_Controls.code_triggered)
            {
                Main_Power_hScrollBar1.Value = value;
            }
            Master_Controls.code_triggered = false;
            switch (oCode.current_band)
            {
                case 160:
                    Last_used.B160.SSB_power = (short)value;
                    break;

                case 80:
                    Last_used.B80.SSB_power = (short)value;
                    break;

                case 60:
                    Last_used.B60.SSB_power = (short)value;
                    break;

                case 40:
                    Last_used.B40.SSB_power = (short)value;
                    break;

                case 30:
                    Last_used.B30.SSB_power = (short)value;
                    break;

                case 20:
                    Last_used.B20.SSB_power = (short)value;
                    break;

                case 17:
                    Last_used.B17.SSB_power = (short)value;
                    break;

                case 15:
                    Last_used.B15.SSB_power = (short)value;
                    break;

                case 12:
                    Last_used.B12.SSB_power = (short)value;
                    break;

                case 10:
                    Last_used.B10.SSB_power = (short)value;
                    break;
            }
            oCode.SendCommand(txsocket, txtarget, Power_Controls.CMD_SET_MAIN_POWER, (short)value);
        }

        private void AM_Carrier_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value;
            if (oCode.isLoading == true) return;
            value = AM_Carrier_hScrollBar1.Value;
            if (Power_Controls.AM_Power == (short)value) return;
            Power_Controls.AM_Power = (short)value;
            AM_Carrier_label36.Text = Convert.ToString(value) + " %";
            Volume_Controls.Speaker_AM_Value = (short)value;
            switch (oCode.current_band)
            {
                case 160:
                    Last_used.B160.AM_power = (short)value;
                    break;

                case 80:
                    Last_used.B80.AM_power = (short)value;
                    break;

                case 60:
                    Last_used.B60.AM_power = (short)value;
                    break;

                case 40:
                    Last_used.B40.AM_power = (short)value;
                    break;

                case 30:
                    Last_used.B30.AM_power = (short)value;
                    break;

                case 20:
                    Last_used.B20.AM_power = (short)value;
                    break;

                case 17:
                    Last_used.B17.AM_power = (short)value;
                    break;

                case 15:
                    Last_used.B15.AM_power = (short)value;
                    break;

                case 12:
                    Last_used.B12.AM_power = (short)value;
                    break;

                case 10:
                    Last_used.B10.AM_power = (short)value;
                    break;
            }
            oCode.SendCommand(txsocket, txtarget, Power_Controls.CMD_SET_AM_POWER, (short)value);
        }

        private void CW_Power_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value;
            if (oCode.isLoading == true) return;
            if (Master_Controls.code_triggered)
            {
                value = Power_Controls.Main_Tab_Power;
                CW_Power_hScrollBar1.Value = value;
            }
            else
            {
                value = CW_Power_hScrollBar1.Value;
            }
            if (Power_Controls.CW_Power == (short)value) return;
            Power_Controls.CW_Power = (short)value;
            CW_Power_label36.Text = Convert.ToString(value) + " %";
            Volume_Controls.Speaker_CW_Value = (short)value;
            if (!Master_Controls.code_triggered)
            {
                Main_Power_hScrollBar1.Value = value;
            }
            Master_Controls.code_triggered = false;
            switch (oCode.current_band)
            {
                case 160:
                    Last_used.B160.CW_power = (short)value;
                    break;

                case 80:
                    Last_used.B80.CW_power = (short)value;
                    break;

                case 60:
                    Last_used.B60.CW_power = (short)value;
                    break;

                case 40:
                    Last_used.B40.CW_power = (short)value;
                    break;

                case 30:
                    Last_used.B30.CW_power = (short)value;
                    break;

                case 20:
                    Last_used.B20.CW_power = (short)value;
                    break;

                case 17:
                    Last_used.B17.CW_power = (short)value;
                    break;

                case 15:
                    Last_used.B15.CW_power = (short)value;
                    break;

                case 12:
                    Last_used.B12.CW_power = (short)value;
                    break;

                case 10:
                    Last_used.B10.CW_power = (short)value;
                    break;
            }
            oCode.SendCommand(txsocket, txtarget, Power_Controls.CMD_SET_CW_POWER, (short)value);
        }

        private void TX_Click(object sender, EventArgs e)
        {
        }

        private void AM_Carrier_label36_Click(object sender, EventArgs e)
        {
        }

        private void Tune_Power_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value;
            if (oCode.isLoading == true) return;
            value = Tune_Power_hScrollBar1.Value;
            if (Power_Controls.TUNE_Power == (short)value) return;
            Power_Controls.TUNE_Power = (short)value;
            Tune_Power_label37.Text = Convert.ToString(value) + " %";
            IQ_Controls.Tune_power = (short)value;
            IQ_Tune_Power_hScrollBar1.Value = value;
            Volume_Controls.Speaker_TUNE_Value = (short)value;
            oCode.SendCommand(txsocket, txtarget, Power_Controls.CMD_SET_TUNE_POWER, (short)value);
        }

        private void Volume_Mute_button2_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (!Settings.Default.Speaker_MutED)
            {
                MonitorTextBoxText(" Volume_Mute_button2 -> Called -> Set Mute ON");
                Volume_Mute_button2.ForeColor = Color.Red;
                Volume_Mute_button2.Text = "MUTED";
                MonitorTextBoxText(" Volume_Mute_button2 -> CMD_SET_SPEAKER_MUTE: 1");
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 1);
                RPi_Settings.Volume_Settings.Speaker_Mute = 1;
                Settings.Default.Speaker_MutED = true;
                //RPi_Settings.RPi_Needs_Updated = true;
            }
            else
            {
                MonitorTextBoxText(" Volume_Mute_button2 -> Called -> Set Mute OFF");
                Volume_Mute_button2.ForeColor = Color.Black;
                Volume_Mute_button2.Text = "Volume";
                MonitorTextBoxText(" Volume_Mute_button2 -> CMD_SET_SPEAKER_MUTE: 0");
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_VOLUME_ATTN,
                    (short)Settings.Default.Volume_Attn_Index);
                RPi_Settings.Volume_Settings.Speaker_Mute = 0;
                Settings.Default.Speaker_MutED = false;
                //RPi_Settings.RPi_Needs_Updated = true;
            }
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

        public bool Update_Power_values()
        {
            String path, line;
            System.IO.StreamReader file;
            short temp_int = 0;
            String temp_string;

            MonitorTextBoxText(" Update_Power_values -> Entered");
            List<string> MyList = new List<string>();
            //oCode.Platform = (int)Environment.OSVersion.Platform;
            // get path to local Appdata folder
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#if RPI
            path += "/mscc/power.ini";
#else
            path += "\\multus-sdr-client\\power.ini";
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
                DialogResult ret = MessageBox.Show(" Power.ini Open Failed: " + er + " Make note of the error and contact Multus SDR, LLC.",
                    "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            while ((line = file.ReadLine()) != null)
            {   //Set the USB and LSB Power values
                temp_string = getBetween(line, "AM_POWER=", ",");
                Int16.TryParse(temp_string, out temp_int);
                AM_Carrier_hScrollBar1.Value = temp_int;
                Power_Controls.AM_Power = temp_int;
                AM_Carrier_label36.Text = Convert.ToString(temp_int) + " %";
                MonitorTextBoxText(" AM Power: " + temp_int + " AM Text " + Convert.ToString(temp_int));

                temp_string = getBetween(line, "LSB_POWER=", ",");
                Int16.TryParse(temp_string, out temp_int);
                Power_hScrollBar1.Value = temp_int;
                Power_Controls.Main_Power = temp_int;
                SSB_Power_label36.Text = Convert.ToString(temp_int) + " %";
                MonitorTextBoxText(" LSB Power: " + temp_int + " USB Text " + Convert.ToString(temp_int));

                temp_string = getBetween(line, "USB_POWER=", ",");
                Int16.TryParse(temp_string, out temp_int);
                Power_hScrollBar1.Value = temp_int;
                Power_Controls.Main_Power = temp_int;
                SSB_Power_label36.Text = Convert.ToString(temp_int) + " %";
                MonitorTextBoxText(" USB Power: " + temp_int + " LSB Text " + Convert.ToString(temp_int));

                temp_string = getBetween(line, "CW_POWER=", ",");
                Int16.TryParse(temp_string, out temp_int);
                CW_Power_hScrollBar1.Value = temp_int;
                Power_Controls.CW_Power = temp_int;
                CW_Power_label36.Text = Convert.ToString(temp_int) + " %";
                MonitorTextBoxText(" CW Power: " + temp_int + " CW Text " + Convert.ToString(temp_int));

                temp_string = getBetween(line, "TUNE_POWER=", ";");
                Int16.TryParse(temp_string, out temp_int);
                Tune_Power_hScrollBar1.Value = temp_int;
                Power_Controls.TUNE_Power = temp_int;
                IQ_Controls.Tune_power = temp_int;
                Tune_Power_label37.Text = Convert.ToString(temp_int) + " %";
                MonitorTextBoxText(" TUNE Power: " + temp_int + " TUNE Text " + Convert.ToString(temp_int));
            }
            file.Close();
            MonitorTextBoxText(" Update_Power_values -> finished");
            return true;
        }

        /*public bool Update_Mic_listbox()
        {
            String path, line;
            System.IO.StreamReader file;
            String Input_param_value = "test";
            String temp_string;
            int param_pos;
            int end_pos;
            int index = 0;
            short temp_int = 0;

            MonitorTextBoxText("Update_Mic_listbox -> Entered");
            List<string> MyList = new List<string>();
            //oCode.Platform = (int)Environment.OSVersion.Platform;
            // get path to local Appdata folder
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            // add our folder and file name
            if ((oCode.Platform == 4) || (oCode.Platform == 6) || (oCode.Platform == 128))
            {// A kludge to check for non Windows OS.
             //These values may change in the future.
                path += "/mscc/sdrcore_trans.ini";
            }
            else
            {
                path += "\\multus-sdr-client\\sdrcore_trans.ini";
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
                //MessageBox.Show("IO Exception opening sdrcore_trans.ini file.\n\r" + er +
                //    " Make note of the error and contact Multus SDR,LLC.", "MSCC");
                Audio_Device_Controls.Update_Microphone_Status = false;
                return false;
            }
            while ((line = file.ReadLine()) != null)
            {   //Get the Output Device names and populate the Microphone Listbox
                param_pos = line.IndexOf("DEVICE_NAME");                       // get the position of the DEVICE_NAME parameter
                temp_string = line.Substring((param_pos + 12), (line.Length - param_pos - 12));   // parse everything between the end of DEVICE_NAME= and the end of the line.
                end_pos = temp_string.IndexOf("^"); //temp string will start with the DEVICE_NAME parameter value. Find the trailing comma
                Input_param_value = temp_string.Substring(0, end_pos);   // temp string will start with the DEVICE_NAME parameter value. Extract the value
                MyList.Add(Input_param_value);

                //Determine if this is a system default input microphone
                param_pos = line.IndexOf("DEFAULT");
                temp_string = line.Substring((param_pos + 8), (1));
                Int16.TryParse(temp_string, out temp_int);
                if (temp_int == 1)
                {
                    Audio_Device_Controls.Default_Mic = (short)index;
                    MonitorTextBoxText(" Default Mic: " + Input_param_value + "Index Value: " + index);
                }

                //Determine if this device is the selected input microphone
                param_pos = line.IndexOf("SELECTED");
                temp_string = line.Substring((param_pos + 9), (1));
                Int16.TryParse(temp_string, out temp_int);
                if (temp_int == 1)
                {
                    Audio_Device_Controls.Selected_Mic = (short)index;

                    MonitorTextBoxText(" Selected Mic " + Input_param_value + "Index Value: " + index);
                }

                index++;
            }
            Microphone_listBox1.DataSource = MyList;
            if (Audio_Device_Controls.Selected_Mic != 0)
            {
                Microphone_listBox1.SelectedIndex = Audio_Device_Controls.Selected_Mic;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_MIC_DEVICE, Audio_Device_Controls.Selected_Mic);
                Current_Mic_label41.Text = Microphone_listBox1.Text;
            }
            else
            {
                Microphone_listBox1.SelectedIndex = Audio_Device_Controls.Default_Mic;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_MIC_DEVICE, Audio_Device_Controls.Default_Mic);
                Current_Mic_label41.Text = Microphone_listBox1.Text;
            }
            file.Close();
            MonitorTextBoxText("Update_Mic_listbox -> Finished");
            Audio_Device_Controls.Update_Microphone_Status = true;
            return true;
        }*/

        private void MicVolume_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value;
            if (oCode.isLoading == true) return;
            value = MicVolume_hScrollBar1.Value;
            if (Volume_Controls.Mic_Volume == value) return;
            Volume_Controls.Mic_Volume = (short)value;
            if (Settings.Default.Mic_Is_Digital)
            {
                Settings.Default.Digital_Volume = Volume_Controls.Mic_Volume;
                RPi_Settings.Volume_Settings.Mic_Volume = Settings.Default.Digital_Volume;
                //RPi_Settings.RPi_Needs_Updated = true;
                MonitorTextBoxText(" MicVolume_hScrollBar1 -> Digital: " +
                    Convert.ToString(Settings.Default.Digital_Volume));
            }
            else
            {
                Settings.Default.Voice_Volume = Volume_Controls.Mic_Volume;
                RPi_Settings.Volume_Settings.Mic_Volume = Settings.Default.Voice_Volume;
                //RPi_Settings.RPi_Needs_Updated = true;
                MonitorTextBoxText(" MicVolume_hScrollBar1 -> Voice: " +
                    Convert.ToString(Settings.Default.Voice_Volume));
            }
            Microphone_textBox2.Text = Convert.ToString(value);
            if (Volume_Controls.Mic_MutED)
            {
                TX_Mute_button2.ForeColor = Color.Black;
                TX_Mute_button2.Text = "Mic Gain";
                Volume_Controls.Mic_MutED = false;
                RPi_Settings.Volume_Settings.Mic_Mute = 0;
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_MUTE, 0);
            }
            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_VOLUME, Volume_Controls.Mic_Volume);
        }

        /*private void Microphone_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index;

            if (oCode.isLoading == true) return;
            index = Microphone_listBox1.SelectedIndex;
            if (index == Audio_Device_Controls.Selected_Mic) return;
            Audio_Device_Controls.Selected_Mic = (short)index;
            Current_Mic_label41.Text = Microphone_listBox1.Text;
            oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_MIC_DEVICE, Audio_Device_Controls.Selected_Mic);
        }*/

        /*public bool Update_Speaker_listbox()
        {
            String path, line;
            System.IO.StreamReader file;
            String Output_param_value = "test";
            String temp_string;
            int param_pos;
            int end_pos;
            int index = 0;
            short temp_int = 0;

            MonitorTextBoxText("Update_Speaker_listbox -> Entered");
            List<string> MyList = new List<string>();
            //oCode.Platform = (int)Environment.OSVersion.Platform;
            // get path to local Appdata folder
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            // add our folder and file name
            if ((oCode.Platform == 4) || (oCode.Platform == 6) || (oCode.Platform == 128))
            {// A kludge to check for non Windows OS.
             //These values may change in the future.
                path += "/mscc/sdrcore_recv.ini";
            }
            else
            {
                path += "\\multus-sdr-client\\sdrcore_recv.ini";
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
                //MessageBox.Show("IO Exception opening sdrcore_recv.ini file.\n\r" + er +
                //    " Make note of the error and contact Multus SDR,LLC.", "MSCC");
                Audio_Device_Controls.Update_Speaker_Status = false;
                return false;
            }

            // Parse the INI file. Doing it this way eliminates the requirement that the parameters be stored in any particular order.
            // The parms are stored in a public static struct, which can be reached by the form-level GUI code.
            while ((line = file.ReadLine()) != null)
            {   //Get the Output Device names and populate the Speaker Listbox
                param_pos = line.IndexOf("DEVICE_NAME");                       // get the position of the DEVICE_NAME parameter
                temp_string = line.Substring((param_pos + 12), (line.Length - param_pos - 12));   // parse everything between the end of DEVICE_NAME= and the end of the line.
                end_pos = temp_string.IndexOf("^"); //temp string will start with the DEVICE_NAME parameter value. Find the trailing comma
                Output_param_value = temp_string.Substring(0, end_pos);   // temp string will start with the DEVICE_NAME parameter value. Extract the value
                MyList.Add(Output_param_value);

                //Determine if this is a system default output device
                param_pos = line.IndexOf("DEFAULT");
                temp_string = line.Substring((param_pos + 8), (1));
                Int16.TryParse(temp_string, out temp_int);
                if (temp_int == 1)
                {
                    Audio_Device_Controls.Default_Speaker = (short)index;
                    MonitorTextBoxText(" Update_Speaker_listbox -> Default Speaker: " + Output_param_value + " Index Value: "
                        + (short)index);
                }

                //Determine if this device is the selected device
                param_pos = line.IndexOf("SELECTED");
                temp_string = line.Substring((param_pos + 9), (1));
                Int16.TryParse(temp_string, out temp_int);
                if (temp_int == 1)
                {
                    Audio_Device_Controls.Selected_Speaker = (short)index;

                    MonitorTextBoxText(" Update_Speaker_listbox -> Selected Speaker " + Output_param_value + "Index Value: "
                                                                                                                        + (short)index);
                }

                index++;
            }
            Speaker_listBox1.DataSource = MyList;
            if (Audio_Device_Controls.Selected_Speaker != 0)
            {
                MonitorTextBoxText(" Update_Speaker_listbox -> Selected Speaker -> Setting Speaker_listbox1.Selected_Index: "
                                                                    + Audio_Device_Controls.Selected_Speaker);
                Speaker_listBox1.SelectedIndex = Audio_Device_Controls.Selected_Speaker;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_SPEAKER_DEVICE, Audio_Device_Controls.Selected_Speaker);
                Current_Speaker_label41.Text = Speaker_listBox1.Text;
            }
            else
            {
                Speaker_listBox1.SelectedIndex = Audio_Device_Controls.Default_Speaker;
                MonitorTextBoxText(" Update_Speaker_listbox -> Selected Speaker -> Setting Speaker_listbox1.Selected_Index: "
                                                                    + Audio_Device_Controls.Default_Speaker);
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_SPEAKER_DEVICE, Audio_Device_Controls.Default_Speaker);
                Current_Speaker_label41.Text = Speaker_listBox1.Text;
            }
            file.Close();
            MonitorTextBoxText("Update_Speaker_listbox -> Finished");
            Audio_Device_Controls.Update_Speaker_Status = true;
            return true;
        }*/

        /*public void Speaker_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index;

            if (oCode.isLoading == true) return;
            index = Speaker_listBox1.SelectedIndex;
            if (index == Audio_Device_Controls.Selected_Speaker) return;
            Audio_Device_Controls.Selected_Speaker = (short)index;
            Current_Speaker_label41.Text = Speaker_listBox1.Text;
            oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_SPEAKER_DEVICE, Audio_Device_Controls.Selected_Speaker);
        }*/

        /*private void Current_Speaker_label41_Click(object sender, EventArgs e)
        {
        }
        */
        private void TX_Mute_button2_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (!Volume_Controls.Mic_MutED)
            {
                MonitorTextBoxText(" TX_Mute_Button2 -> Called -> Set Mute ON");
                TX_Mute_button2.ForeColor = Color.Red;
                TX_Mute_button2.Text = "MUTED";
                Volume_Controls.Mic_MutED = true;
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_MUTE, 1);
                RPi_Settings.Volume_Settings.Mic_Mute = 1;
                //RPi_Settings.RPi_Needs_Updated = true;
            }
            else
            {
                MonitorTextBoxText(" TX_Mute_Button2 -> Called -> Set Mute OFF");
                TX_Mute_button2.ForeColor = Color.Black;
                TX_Mute_button2.Text = "Mic Gain";
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_MUTE, 0);
                Volume_Controls.Mic_MutED = false;
                RPi_Settings.Volume_Settings.Mic_Mute = 0;
                //RPi_Settings.RPi_Needs_Updated = true;
            }
        }

        /*private void Current_Mic_label41_Click(object sender, EventArgs e)
        {
        }
        */
        /*private void tmrCheckComPorts_Tick(object sender, EventArgs e)
        {
        }

        private void cmbBaudRate_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void lblBaudRate_Click(object sender, EventArgs e)
        {
        }

        private void chkCD_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (!Master_Controls.code_triggered)
            {
                if (Comm_Port_Controls.Comm_Port_Open != false)
                {
                    if (Comm_Port_Controls.DCD_Selected == 0)
                    {
                        Comm_Port_Controls.DCD_Selected = 1;
                        oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_PORT_PINS, 2);
                    }
                    else
                    {
                        Comm_Port_Controls.DCD_Selected = 0;

                        oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_PORT_PINS, 0);
                    }
                    if (Comm_Port_Controls.CTS_Selected == 1)
                    {
                        //Comm_Port_Controls.CTS_Selected = 0;
                    }
                }
                else
                {
                    DialogResult ret = MessageBox.Show("The COM PORT Must BE ACTIVE Before Selecting a Pin (DCD)", "MSCC",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }*/

        private void toolTip_Popup(object sender, PopupEventArgs e)
        {
        }

        /*private void label43_Click(object sender, EventArgs e)
        {
        }

        private void cmbPortName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //String temp_string;
            String comm_name;
            //int param_pos;
            int index = 0;
            //short temp_int = 0;
            int comm_name_lenght = 0;

            if (oCode.isLoading) return;
            if (!Master_Controls.code_triggered)
            {
                index = cmbPortName.SelectedIndex;
                Comm_Port_Controls.Box_indexes.Previous_Index = index;
                Comm_Port_Controls.Box_indexes.Comm_Name_Index = index;
                comm_name = cmbPortName.Text;
                comm_name_lenght = System.Text.ASCIIEncoding.ASCII.GetByteCount(comm_name);
                if (Comm_Port_Controls.Comm_Port_Open == false)
                {
                    if (Comm_Port_Controls.Box_indexes.Comm_Name_Index == Comm_Port_Controls.HR50_Controls.Comm_Name_Index)
                    {
                        if (Comm_Port_Controls.HR50_Controls.Comm_Port_Open == false)
                        {
                            oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_NAME_INDEX, (short)index);
                            oCode.SendCommand_String(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_PORT, comm_name, 
                                comm_name_lenght);
                        }
                        else
                        {
                            MessageBox.Show("Port is use by Hardrock 50 port.  Select a different Port", "MSCC",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_NAME_INDEX, (short)index);
                        oCode.SendCommand_String(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_PORT, comm_name,
                                 comm_name_lenght);
                    }
                }
                else
                {
                    if (Comm_Port_Controls.Box_indexes.Set_By_Server == false)
                    {
                        MessageBox.Show("Close port before selecting a new port", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //Comm_Port_Controls.Box_indexes.Set_By_Server = false;
                    }
                }
            }
        }*/

        private void IQ_Tune_button2_Click_1(object sender, EventArgs e)
        {
            if (IQ_Controls.IQ_RX_MODE_ACTIVE)
            {
                DialogResult ret = MessageBox.Show("TUNING NOT PERMITTED IN RX MODE", "MSCC",
                                              MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (IQ_Controls.band_selected)
            {
                if (IQ_Controls.IQ_TX_MODE_ACTIVE)
                {
                    if (!IQ_Controls.Tuning_Mode)
                    {
                        oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 1);
                        IQ_Controls.Tuning_Mode = true;
                        oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 1);
                        IQ_Tune_button2.ForeColor = Color.Red;
                        IQ_Tune_button2.Text = "TUNE ON";
                        IQ_Tune_button2.FlatStyle = FlatStyle.Popup;
                        Master_Controls.Transmit_Mode = true;
                    }
                    else
                    {
                        IQ_Controls.Tuning_Mode = false;
                        oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 0);
                        IQ_Tune_button2.ForeColor = Control.DefaultForeColor;
                        IQ_Tune_button2.Text = "TUNE OFF";
                        IQ_Tune_button2.FlatStyle = FlatStyle.Standard;
                        Master_Controls.Transmit_Mode = false;
                        if (!Settings.Default.Speaker_MutED)
                        {
                            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                        }
                    }
                }
            }
            else
            {
                DialogResult ret = MessageBox.Show("Select a Band", "MSCC",
                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void Display_IQ_freq(int freq)
        {
            int MHz = freq / 1000000;
            int KHz = (freq - (MHz * 1000000)) / 1000;
            int Hz = freq - (MHz * 1000000) - (KHz * 1000);
            Cal_Freq_textBox2.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." +
                (string.Format("{0:000}", Hz));
        }

        private void IQ160_radioButton10_CheckedChanged_1(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;

            if (IQ_Controls.Tuning_Mode)
            {
                IQ_Controls.Tuning_Mode = false;
                oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 0);
                IQ_Tune_button2.ForeColor = Control.DefaultForeColor;
                IQ_Tune_button2.Text = "TX OFF";
                IQ_Tune_button2.FlatStyle = FlatStyle.Standard;
                Master_Controls.Transmit_Mode = false;
                if (!Settings.Default.Speaker_MutED)
                {
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                }
            }
            if (IQ_Controls.IQ_TX_MODE_ACTIVE)
            {
                if (IQ_Controls.Previous_band != IQ_Controls.B160)
                {
                    MonitorTextBoxText(" 160m IQ Band Button Clicked." + " Previous Band: " + IQ_Controls.Previous_band);
                    IQLefttextBox2.Text = "0";
                    LefthScrollBar1.Value = 0;
                    IQ_Controls.Previous_band = IQ_Controls.B160;
                    IQ_Controls.Current_TX_Band = IQ_Controls.B160;
                    //IQ_Controls.band = IQ_Controls.B160;
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B160);
                    //oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_OFFSET, 0);
                    IQ_Controls.band_selected = true;
                    Display_IQ_freq(IQ_Controls.iq_calibration_freqs[IQ_Controls.B160]);
                }
            }
            else
            {
                DialogResult ret = MessageBox.Show("Set TX MODE to ON before selecting a band", "MSCC",
                                                                                         MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void IQ80_radioButton9_CheckedChanged_1(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;

            if (IQ_Controls.Tuning_Mode)
            {
                IQ_Controls.Tuning_Mode = false;
                oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 0);
                IQ_Tune_button2.ForeColor = Control.DefaultForeColor;
                IQ_Tune_button2.Text = "TX OFF";
                IQ_Tune_button2.FlatStyle = FlatStyle.Standard;
                Master_Controls.Transmit_Mode = false;
                if (!Settings.Default.Speaker_MutED)
                {
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                }
            }
            if (IQ_Controls.IQ_TX_MODE_ACTIVE)
            {
                if (IQ_Controls.Previous_band != IQ_Controls.B80)
                {
                    MonitorTextBoxText(" 80m IQ Band Button Clicked");
                    IQLefttextBox2.Text = "0";
                    LefthScrollBar1.Value = 0;
                    IQ_Controls.Previous_band = IQ_Controls.B80;
                    IQ_Controls.Current_TX_Band = IQ_Controls.B80;
                    //IQ_Controls.band = IQ_Controls.B80;
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B80);
                    //oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_OFFSET, 0);
                    IQ_Controls.band_selected = true;
                    Display_IQ_freq(IQ_Controls.iq_calibration_freqs[IQ_Controls.B80]);
                }
            }
            else
            {
                DialogResult ret = MessageBox.Show("Set TX MODE to ON before selecting a band", "MSCC",
                                                                                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void IQ60_radioButton8_CheckedChanged_1(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;

            if (IQ_Controls.Tuning_Mode)
            {
                IQ_Controls.Tuning_Mode = false;
                oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 0);
                IQ_Tune_button2.ForeColor = Control.DefaultForeColor;
                IQ_Tune_button2.Text = "TX OFF";
                IQ_Tune_button2.FlatStyle = FlatStyle.Standard;
                Master_Controls.Transmit_Mode = false;
                if (!Settings.Default.Speaker_MutED)
                {
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                }
            }
            if (IQ_Controls.IQ_TX_MODE_ACTIVE)
            {
                if (IQ_Controls.Previous_band != IQ_Controls.B60)
                {
                    MonitorTextBoxText(" 60m IQ Band Button Clicked");
                    IQLefttextBox2.Text = "0";
                    LefthScrollBar1.Value = 0;
                    IQ_Controls.Previous_band = IQ_Controls.B60;
                    IQ_Controls.Current_TX_Band = IQ_Controls.B60;
                    //IQ_Controls.band = IQ_Controls.B60;
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B60);
                    //oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_OFFSET, 0);
                    IQ_Controls.band_selected = true;
                    Display_IQ_freq(IQ_Controls.iq_calibration_freqs[IQ_Controls.B60]);
                }
            }
            else
            {
                DialogResult ret = MessageBox.Show("Set TX MODE to ON before selecting a band", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void IQ40_radioButton7_CheckedChanged_1(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;

            if (IQ_Controls.Tuning_Mode)
            {
                IQ_Controls.Tuning_Mode = false;
                oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 0);
                IQ_Tune_button2.ForeColor = Control.DefaultForeColor;
                IQ_Tune_button2.Text = "TX OFF";
                IQ_Tune_button2.FlatStyle = FlatStyle.Standard;
                Master_Controls.Transmit_Mode = false;
                if (!Settings.Default.Speaker_MutED)
                {
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                }
            }
            if (IQ_Controls.IQ_TX_MODE_ACTIVE)
            {
                if (IQ_Controls.Previous_band != IQ_Controls.B40)
                {
                    MonitorTextBoxText(" 40m IQ Band Button Clicked");
                    IQLefttextBox2.Text = "0";
                    LefthScrollBar1.Value = 0;
                    IQ_Controls.Previous_band = IQ_Controls.B40;
                    IQ_Controls.Current_TX_Band = IQ_Controls.B40;
                    //IQ_Controls.band = IQ_Controls.B40;
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B40);
                    //oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_OFFSET, 0);
                    IQ_Controls.band_selected = true;
                    Display_IQ_freq(IQ_Controls.iq_calibration_freqs[IQ_Controls.B40]);
                }
            }
            else
            {
                DialogResult ret = MessageBox.Show("Set TX MODE to ON before selecting a band", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void IQ30_radioButton6_CheckedChanged_1(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;

            if (IQ_Controls.Tuning_Mode)
            {
                IQ_Controls.Tuning_Mode = false;
                oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 0);
                IQ_Tune_button2.ForeColor = Control.DefaultForeColor;
                IQ_Tune_button2.Text = "TX OFF";
                IQ_Tune_button2.FlatStyle = FlatStyle.Standard;
                Master_Controls.Transmit_Mode = false;
                if (!Settings.Default.Speaker_MutED)
                {
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                }
            }
            if (IQ_Controls.IQ_TX_MODE_ACTIVE)
            {
                if (IQ_Controls.Previous_band != IQ_Controls.B30)
                {
                    MonitorTextBoxText(" 30m IQ Band Button Clicked");
                    IQLefttextBox2.Text = "0";
                    LefthScrollBar1.Value = 0;

                    IQ_Controls.Previous_band = IQ_Controls.B30;
                    IQ_Controls.Current_TX_Band = IQ_Controls.B30;
                    //IQ_Controls.band = IQ_Controls.B30;
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B30);
                    //oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_OFFSET, 0);
                    IQ_Controls.band_selected = true;
                    Display_IQ_freq(IQ_Controls.iq_calibration_freqs[IQ_Controls.B30]);
                }
            }
            else
            {
                DialogResult ret = MessageBox.Show("Set TX MODE to ON before selecting a band", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void IQ20_radioButton5_CheckedChanged_1(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (IQ_Controls.Tuning_Mode)
            {
                IQ_Controls.Tuning_Mode = false;
                oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 0);
                IQ_Tune_button2.ForeColor = Control.DefaultForeColor;
                IQ_Tune_button2.Text = "TX OFF";
                IQ_Tune_button2.FlatStyle = FlatStyle.Standard;
                Master_Controls.Transmit_Mode = false;
                if (!Settings.Default.Speaker_MutED)
                {
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                }
            }
            if (IQ_Controls.IQ_TX_MODE_ACTIVE)
            {
                if (IQ_Controls.Previous_band != IQ_Controls.B20)
                {
                    MonitorTextBoxText(" 20m IQ Band Button Clicked");
                    IQLefttextBox2.Text = "0";
                    LefthScrollBar1.Value = 0;

                    IQ_Controls.Previous_band = IQ_Controls.B20;
                    IQ_Controls.Current_TX_Band = IQ_Controls.B20;
                    //IQ_Controls.band = IQ_Controls.B20;
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B20);
                    //oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_OFFSET, 0);
                    IQ_Controls.band_selected = true;
                    Display_IQ_freq(IQ_Controls.iq_calibration_freqs[IQ_Controls.B20]);
                }
            }
            else
            {
                DialogResult ret = MessageBox.Show("Set TX MODE to ON before selecting a band", "MSCC",
                                                                                      MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void IQ17_radioButton4_CheckedChanged_1(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (IQ_Controls.Tuning_Mode)
            {
                IQ_Controls.Tuning_Mode = false;
                oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 0);
                IQ_Tune_button2.ForeColor = Control.DefaultForeColor;
                IQ_Tune_button2.Text = "TX OFF";
                IQ_Tune_button2.FlatStyle = FlatStyle.Standard;
                Master_Controls.Transmit_Mode = false;
                if (!Settings.Default.Speaker_MutED)
                {
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                }
            }
            if (IQ_Controls.IQ_TX_MODE_ACTIVE)
            {
                if (IQ_Controls.Previous_band != IQ_Controls.B17)
                {
                    MonitorTextBoxText(" 17m IQ Band Button Clicked");
                    IQLefttextBox2.Text = "0";
                    LefthScrollBar1.Value = 0;

                    IQ_Controls.Previous_band = IQ_Controls.B17;
                    IQ_Controls.Current_TX_Band = IQ_Controls.B17;
                    //IQ_Controls.band = IQ_Controls.B17;
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B17);
                    //oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_OFFSET, 0);
                    IQ_Controls.band_selected = true;
                    Display_IQ_freq(IQ_Controls.iq_calibration_freqs[IQ_Controls.B17]);
                }
            }
            else
            {
                DialogResult ret = MessageBox.Show("Set TX MODE to ON before selecting a band", "MSCC",
                                                                                      MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void IQ15_radioButton3_CheckedChanged_1(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;

            if (IQ_Controls.Tuning_Mode)
            {
                IQ_Controls.Tuning_Mode = false;
                oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 0);
                IQ_Tune_button2.ForeColor = Control.DefaultForeColor;
                IQ_Tune_button2.Text = "TX OFF";
                IQ_Tune_button2.FlatStyle = FlatStyle.Standard;
                Master_Controls.Transmit_Mode = false;
                if (!Settings.Default.Speaker_MutED)
                {
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                }
            }
            if (IQ_Controls.IQ_TX_MODE_ACTIVE)
            {
                if (IQ_Controls.Previous_band != IQ_Controls.B15)
                {
                    MonitorTextBoxText(" 15m IQ Band Button Clicked");
                    IQLefttextBox2.Text = "0";
                    LefthScrollBar1.Value = 0;

                    IQ_Controls.Previous_band = IQ_Controls.B15;
                    IQ_Controls.Current_TX_Band = IQ_Controls.B15;
                    //IQ_Controls.band = IQ_Controls.B15;
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B15);
                    //oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_OFFSET, 0);
                    IQ_Controls.band_selected = true;
                    Display_IQ_freq(IQ_Controls.iq_calibration_freqs[IQ_Controls.B15]);
                }
            }
            else
            {
                DialogResult ret = MessageBox.Show("Set TX MODE to ON before selecting a band", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void IQ12_radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;

            if (IQ_Controls.Tuning_Mode)
            {
                IQ_Controls.Tuning_Mode = false;
                oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 0);
                IQ_Tune_button2.ForeColor = Control.DefaultForeColor;
                IQ_Tune_button2.Text = "TX OFF";
                IQ_Tune_button2.FlatStyle = FlatStyle.Standard;
                Master_Controls.Transmit_Mode = false;
                if (!Settings.Default.Speaker_MutED)
                {
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                }
            }
            if (IQ_Controls.IQ_TX_MODE_ACTIVE)
            {
                if (IQ_Controls.Previous_band != IQ_Controls.B12)
                {
                    MonitorTextBoxText(" 12m IQ Band Button Clicked");
                    IQLefttextBox2.Text = "0";
                    LefthScrollBar1.Value = 0;

                    IQ_Controls.Previous_band = IQ_Controls.B12;
                    IQ_Controls.Current_TX_Band = IQ_Controls.B12;
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B12);
                    //oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_OFFSET, 0);
                    IQ_Controls.band_selected = true;
                    Display_IQ_freq(IQ_Controls.iq_calibration_freqs[IQ_Controls.B12]);
                }
            }
            else
            {
                DialogResult ret = MessageBox.Show("Set TX MODE to ON before selecting a band", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void IQ10_radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (IQ_Controls.Tuning_Mode)
            {
                IQ_Controls.Tuning_Mode = false;
                oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 0);
                IQ_Tune_button2.ForeColor = Control.DefaultForeColor;
                IQ_Tune_button2.Text = "TX OFF";
                IQ_Tune_button2.FlatStyle = FlatStyle.Standard;
                Master_Controls.Transmit_Mode = false;
                if (!Settings.Default.Speaker_MutED)
                {
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                }
            }
            if (IQ_Controls.IQ_TX_MODE_ACTIVE)
            {
                if (IQ_Controls.Previous_band != IQ_Controls.B10)
                {
                    MonitorTextBoxText(" 10m IQ Band Button Clicked");
                    IQLefttextBox2.Text = "0";
                    LefthScrollBar1.Value = 0;

                    IQ_Controls.Previous_band = IQ_Controls.B10;
                    IQ_Controls.Current_TX_Band = IQ_Controls.B10;
                    //IQ_Controls.band = IQ_Controls.B10;
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B10);
                    //oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_OFFSET, 0);
                    IQ_Controls.band_selected = true;
                    Display_IQ_freq(IQ_Controls.iq_calibration_freqs[IQ_Controls.B10]);
                }
            }
            else
            {
                DialogResult ret = MessageBox.Show("Set TX MODE to ON before selecting a band", "MSCC",
                                                                                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void IQ_Commit_button2_Click_1(object sender, EventArgs e)
        {
            if (IQ_Controls.IQ_RX_MODE_ACTIVE)
            {
                IQ_Commit_button2.ForeColor = Color.Red;
                IQ_Commit_button2.Text = "APPLYING";
                IQ_Commit_button2.FlatStyle = FlatStyle.Popup;
                oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_COMMIT_IQ, 0);
                IQ_Controls.IQ_Calibrating = true;
                //IQ_Controls.IQ_RX_MODE_ACTIVE = false;
            }
            else
            {
                if (IQ_Controls.IQ_TX_MODE_ACTIVE)
                {
                    if (IQ_Controls.band_selected)
                    {
                        if (IQ_Controls.IQ_TX_MODE_ACTIVE)
                        {
                            {
                                IQ_Commit_button2.ForeColor = Color.Red;
                                IQ_Commit_button2.Text = "APPLYING";
                                IQ_Commit_button2.FlatStyle = FlatStyle.Popup;
                                oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_COMMIT_IQ, 0);
                                IQ_Controls.IQ_Calibrating = true;
                                //IQ_Controls.IQ_TX_MODE_ACTIVE = false;
                            }
                        }
                        else
                        {
                            DialogResult ret = MessageBox.Show("TX MODE NOT SET ON \r\nSET TX MODE to ON", "MSCC",
                                  MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                    }
                    else
                    {
                        DialogResult ret = MessageBox.Show("Select a Band", "MSCC",
                                  MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
            }
        }

        private void IQLefttextBox2_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void AGC_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;
            if (oCode.isLoading) return;
            if (!Master_Controls.code_triggered)
            {
                index = AGC_listBox1.SelectedIndex;
                if (AGC_ALC_Notch_Controls.AGC_Level == index) return;
                AGC_ALC_Notch_Controls.AGC_Level = index;
                switch (AGC_ALC_Notch_Controls.AGC_Level)
                {
                    case 0:
                        ACG_button.Text = "S";
                        break;

                    case 1:
                        ACG_button.Text = "M";
                        break;

                    case 2:
                        ACG_button.Text = "F";
                        break;
                }
                oCode.SendCommand(txsocket, txtarget, AGC_ALC_Notch_Controls.CMD_GET_SET_AGC, (byte)AGC_ALC_Notch_Controls.AGC_Level);
            }
        }

        /*private void cmbParity_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int index;
            if (oCode.isLoading) return;
            if (!Master_Controls.code_triggered)
            {
                //index = cmbParity.SelectedIndex;
                //Comm_Port_Controls.Box_indexes.Parity = index;
                if (Comm_Port_Controls.Comm_Port_Open == false)
                {
                    //oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_PARITY, (short)index);
                }
            }
        }

        private void btnOpenPort_Click_1(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            switch (Comm_Port_Controls.Button_Toggle)
            {
                case false:
                    if (Comm_Port_Controls.Comm_Port_Open == false)
                    {
                        if (Comm_Port_Controls.Box_indexes.Comm_Name_Index == 100)
                        {
                            DialogResult ret = MessageBox.Show("Select a Comm Port", "MSCC",
                               MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            return;
                        }
                        if (Comm_Port_Controls.Box_indexes.Baud_Rate == 100)
                        {
                            DialogResult ret = MessageBox.Show("Select a Baud Rate", "MSCC",
                               MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            return;
                        }
                        if (Comm_Port_Controls.Box_indexes.Parity == 100)
                        {
                            DialogResult ret = MessageBox.Show("Select a Parity", "MSCC",
                               MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            return;
                        }
                        if (Comm_Port_Controls.Box_indexes.Data_Bits == 100)
                        {
                            DialogResult ret = MessageBox.Show("Select the Data Bits", "MSCC",
                               MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            return;
                        }
                        if (Comm_Port_Controls.Box_indexes.Stop_Bits == 100)
                        {
                            DialogResult ret = MessageBox.Show("Select the Stop Bits", "MSCC",
                               MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            return;
                        }

                        oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_START, 1);
                        btnOpenPort.BackColor = Color.Red;
                        btnOpenPort.ForeColor = Color.White;
                        btnOpenPort.Text = "ACTIVE";
                        btnOpenPort.FlatStyle = FlatStyle.Popup;
                        Comm_Port_Controls.Comm_Port_Open = true;
                        Comm_Port_Controls.Button_Toggle = true;
                    }
                   
                    break;

                case true:
                    MonitorTextBoxText(" btnOpenPort_Click_1 -> Comm Port is Closed ");
                    btnOpenPort.BackColor = Control.DefaultBackColor;
                    btnOpenPort.ForeColor = Color.Green;
                    btnOpenPort.Text = "CLOSED";
                    btnOpenPort.FlatStyle = FlatStyle.Standard;
                    Comm_Port_Controls.CTS_Selected = 0;
                    Comm_Port_Controls.DCD_Selected = 0;
                    Comm_Port_Controls.Comm_Port_Open = false;
                    Comm_Port_Controls.Button_Toggle = false;
                    Comm_Port_Controls.Box_indexes.Comm_Name_Index = 100;
                    oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_START, 0);
                    break;
            }
        }

        private void cmbBaudRate_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //int index;
            if (oCode.isLoading) return;
            if (!Master_Controls.code_triggered)
            {
                //index = cmbBaudRate.SelectedIndex;
               // Comm_Port_Controls.Box_indexes.Baud_Rate = index;
                if (Comm_Port_Controls.Comm_Port_Open == false)
                {
                 //   oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_BAUD_RATE, (short)index);
                }
            }
        }

        private void cmbDataBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int index;
            if (oCode.isLoading) return;
            if (!Master_Controls.code_triggered)
            {
                //index = cmbDataBits.SelectedIndex;
                //Comm_Port_Controls.Box_indexes.Data_Bits = index;
                if (Comm_Port_Controls.Comm_Port_Open == false)
                {
                    //oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_DATA_BITS, (short)index);
                }
            }
        }*/

        /*private void cmbStopBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int index;
            if (oCode.isLoading) return;
            if (!Master_Controls.code_triggered)
            {
                //index = cmbStopBits.SelectedIndex;
                //Comm_Port_Controls.Box_indexes.Stop_Bits = index;
                if (Comm_Port_Controls.Comm_Port_Open == false)
                {
                    //oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_STOP_BITS, (short)index);
                }
            }
        }

        private void gbPortSettings_Enter(object sender, EventArgs e)
        {
            if (Comm_Port_Controls.Comm_Port_Open)
            {
                DialogResult ret1 = MessageBox.Show("The Comm Port is Open. Close comm port before selecting comm port parameters \r\n",
                    "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void Freq_Reset_button2_Click(object sender, EventArgs e)
        {
        }
        */
        /*private void Freq_Cal_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;
            if (oCode.isLoading) return;

            if (!Frequency_Calibration_controls.Calibration_In_Progress)
            {
                index = Freq_Cal_listBox1.SelectedIndex;
                switch (index)
                {
                    case 0:
                        Frequency_Calibration_controls.Course_Resolution_Set = true;
                        Frequency_Calibration_controls.Fine_Resolution_Set = false;
                        oCode.SendCommand(txsocket, txtarget, Frequency_Calibration_controls.CMD_SET_CAL_COURSE, 1);
                        Calibration_progressBar1.Maximum = 241;
                        break;

                    case 1:
                        Frequency_Calibration_controls.Fine_Resolution_Set = true;
                        Frequency_Calibration_controls.Course_Resolution_Set = false;
                        oCode.SendCommand(txsocket, txtarget, Frequency_Calibration_controls.CMD_SET_CAL_FINE, 1);
                        Calibration_progressBar1.Maximum = 101;
                        break;
                }
                calibratebutton1.Text = "CALIBRATE";
            }
            else
            {
                DialogResult ret1 = MessageBox.Show("Calibration in Progress. \r\nPlease wait until the calibration is finished. \r\n",
                    "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }*/

        private void Time_display_UTC_label34_Click(object sender, EventArgs e)
        {
        }

        private void Audio_tabPage1_Click(object sender, EventArgs e)
        {
        }

        /*private void Audio_Reset_button2_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (!Audio_Device_Controls.Device_Reset_Button_Active)
            {
                DialogResult ret = MessageBox.Show("Are you sure you want to reset the Audio Devices list?", "MSCC",
                                                                        MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                if (ret == DialogResult.Yes)
                {
                    MessageBox.Show("The Audio Devices List will be reset on next startup of MSCC\n" +
                    "MSCC will now STOP", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    Audio_Reset_button2.Text = "RESET";
                    Audio_Reset_button2.BackColor = Control.DefaultBackColor;
                    Audio_Reset_button2.ForeColor = Color.Red;
                    Audio_Reset_button2.FlatStyle = FlatStyle.Standard;
                    oCode.SendCommand(txsocket, txtarget, Audio_Device_Controls.CMD_DELETE_SDRCORE_INIT, 1);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_STOP, 1);
                    Audio_Device_Controls.Device_Reset_Button_Active = true;
                    Thread.Sleep(500);
                    Application.Exit();
                }
            }
        }*/

        private int Set_IQ_RX_Band()
        {
            int status = 0;
            switch (oCode.current_band)
            {
                case 160:
                    IQ_Controls.IQ_RX_Freq = Last_used.B160.Freq;
                    //IQ160_radioButton10_CheckedChanged_1(null, null);
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B160);
                    IQ_Controls.Current_RX_Band = IQ_Controls.B160;
                    Display_IQ_freq(Last_used.B160.Freq);
                    break;

                case 80:
                    IQ_Controls.IQ_RX_Freq = Last_used.B80.Freq;
                    //IQ80_radioButton9_CheckedChanged_1(null, null);
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B80);
                    IQ_Controls.Current_RX_Band = IQ_Controls.B80;
                    Display_IQ_freq(Last_used.B80.Freq);
                    break;

                case 60:
                    IQ_Controls.IQ_RX_Freq = Last_used.B60.Freq;
                    //IQ60_radioButton8_CheckedChanged_1(null, null);
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B60);
                    IQ_Controls.Current_RX_Band = IQ_Controls.B60;
                    Display_IQ_freq(Last_used.B60.Freq);
                    break;

                case 40:
                    IQ_Controls.IQ_RX_Freq = Last_used.B40.Freq;
                    //IQ40_radioButton7_CheckedChanged_1(null, null);
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B40);
                    IQ_Controls.Current_RX_Band = IQ_Controls.B40;
                    Display_IQ_freq(Last_used.B40.Freq);
                    break;

                case 30:
                    IQ_Controls.IQ_RX_Freq = Last_used.B30.Freq;
                    //IQ30_radioButton6_CheckedChanged_1(null, null);
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B30);
                    IQ_Controls.Current_RX_Band = IQ_Controls.B30;
                    break;

                case 20:
                    IQ_Controls.IQ_RX_Freq = Last_used.B20.Freq;
                    //IQ20_radioButton5_CheckedChanged_1(null, null);
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B20);
                    IQ_Controls.Current_RX_Band = IQ_Controls.B20;
                    Display_IQ_freq(Last_used.B20.Freq);
                    break;

                case 17:
                    IQ_Controls.IQ_RX_Freq = Last_used.B17.Freq;
                    //IQ17_radioButton4_CheckedChanged_1(null, null);
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B17);
                    IQ_Controls.Current_RX_Band = IQ_Controls.B17;
                    Display_IQ_freq(Last_used.B17.Freq);
                    break;

                case 15:
                    IQ_Controls.IQ_RX_Freq = Last_used.B15.Freq;
                    //IQ15_radioButton3_CheckedChanged_1(null, null);
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B15);
                    IQ_Controls.Current_RX_Band = IQ_Controls.B15;
                    Display_IQ_freq(Last_used.B15.Freq);
                    break;

                case 12:
                    IQ_Controls.IQ_RX_Freq = Last_used.B12.Freq;
                    //IQ12_radioButton2_CheckedChanged_1(null, null);
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B12);
                    IQ_Controls.Current_RX_Band = IQ_Controls.B12;
                    Display_IQ_freq(Last_used.B12.Freq);
                    break;

                case 10:
                    IQ_Controls.IQ_RX_Freq = Last_used.B10.Freq;
                    //IQ10_radioButton1_CheckedChanged_1(null, null);
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B10);
                    IQ_Controls.Current_RX_Band = IQ_Controls.B10;
                    Display_IQ_freq(Last_used.B10.Freq);
                    break;

                default:
                    status = 1;
                    break;
            }
            return status;
        }

        private void IQ_RX_button_Click(object sender, EventArgs e)
        {
            int status = 0;
            if (oCode.isLoading) return;
            IQ_Freq_hScrollBar1.Visible = true;
            IQ_UP24KHz_checkBox2.Visible = true;
            Cal_Freq_textBox2.Visible = true;
            IQ_TX_button.Visible = false;
            IQ_Tune_button2.Visible = false;
            IQ_Commit_button2.Visible = true;
            LefthScrollBar1.Visible = true;
            IQ_Reset_All_button2.Visible = true;
            //IQ_listBox1.Visible = true;
            LeftResetbutton2.Visible = true;
            //label6.Visible = true;
            IQLefttextBox2.Visible = true;
            Reset_Freq_button3.Visible = true;
            IQ_RX_button.Visible = false;
            status = Set_IQ_RX_Band();
            if (status == 1)
            {
                DialogResult ret = MessageBox.Show("Invalid Band (General). \r\n Return to the Main Tab and select an Amateur band",
                    "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (IQ_Controls.RX_toggle == false)
            {
                IQ_RX_button.BackColor = Color.Red;
                IQ_RX_button.ForeColor = Color.White;
                IQ_RX_button.Text = "RX IQ MODE ON";
                IQ_RX_button.FlatStyle = FlatStyle.Popup;
                IQ_Controls.RX_toggle = true;
                IQ_Controls.IQ_RX_MODE_ACTIVE = true;
                oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_RX_TX, 0);
                status = Set_IQ_RX_Band();
                groupBox2.Enabled = false;
            }
        }

        private void IQ_TX_button_Click(object sender, EventArgs e)
        {
            IQ_RX_button.Visible = false;
            IQ_Freq_hScrollBar1.Visible = false;
            IQ_UP24KHz_checkBox2.Visible = false;
            IQ_Commit_button2.Visible = true;
            LefthScrollBar1.Visible = true;
            IQ_Reset_All_button2.Visible = true;
            //IQ_listBox1.Visible = true;
            LeftResetbutton2.Visible = true;
            groupBox2.Visible = true;
            IQ_Tune_button2.Visible = true;
            IQLefttextBox2.Visible = true;
            Cal_Freq_textBox2.Visible = true;
            IQ_Tune_Power_hScrollBar1.Visible = true;

            if (IQ_Controls.TX_toggle == false)
            {
                IQ_TX_button.BackColor = Color.Red;
                IQ_TX_button.ForeColor = Color.White;
                //IQ_TX_button.Text = "TX IQ MODE ON";
                IQ_TX_button.Visible = false;
                IQ_TX_button.FlatStyle = FlatStyle.Popup;
                IQ_Controls.TX_toggle = true;
                IQ_Controls.IQ_TX_MODE_ACTIVE = true;
                IQ_Tune_Power_hScrollBar1.Value = Power_Controls.TUNE_Power;
                oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_RX_TX, 1);
                if (!Master_Controls.QRP_Mode)
                {
                    oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 0);
                }
                if (IQ_Controls.Current_TX_Band != IQ_Controls.IQ_INVALID_BAND)
                {
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.Current_TX_Band);
                    //oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_OFFSET, 0);
                    IQ_Controls.band_selected = true;
                    MonitorTextBoxText(" IQ_TX_button_Click -> Band Selected " + IQ_Controls.Current_TX_Band);
                }
                else
                {
                    IQ_Controls.band_selected = false;
                    MonitorTextBoxText(" IQ_TX_button_Click -> No Band Selected");
                }
            }
        }

        private void label4_Click_1(object sender, EventArgs e)
        {
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (Settings.Default.Band_Change_Tune && Master_Controls.Band_Change_Toggle)
            {
                if (Master_Controls.Band_Change_Delay++ >= 2)
                {
                    if (Master_Controls.Band_Change_Timer++ != 3)
                    {
                        if (!Master_Controls.Band_Change_Tune_Called)
                        {
                            Master_Controls.Band_Change_Tune_Called = true;
                            buttTune_Click(null, null);
                        }
                    }
                    else
                    {
                        buttTune_Click(null, null);
                        Master_Controls.Band_Change_Tune_Called = false;
                        Master_Controls.Band_Change_Timer = 0;
                        Master_Controls.Band_Change_Toggle = false;
                        Master_Controls.Band_Change_Delay = 0;
                    }
                }
            }

            if (Window_controls.Panadapter_On_By_Server)
            {

                Window_controls.Panadapter_On_By_Server = false;
            }
            if (Window_controls.Smeter_On_By_Server)
            {
                Window_controls.Smeter_On_By_Server = false;
            }
            if (Window_controls.Waterfall_On_By_Server)
            {

                Window_controls.Waterfall_On_By_Server = false;
            }
            if (Volume_Controls.Overdriven_Warning)
            {
                Volume_Controls.Label_Normal = false;
                if (Volume_Controls.Overdriven_Warning_Toggle)
                {
                    Volume_Controls.Overdriven_Warning_Toggle = false;
                    if (Audio_Device_Controls.Overdriven_Alert)
                    {
                        try
                        {
                            player.Play();
                        }
                        catch (FileNotFoundException)
                        {
                            MessageBox.Show("Sound File not found. Try re-installing mscc-install.exe", "MSCC",
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            return;
                        }
                    }
                }
                else
                {
                    Volume_Controls.Overdriven_Warning_Toggle = true;
                }
            }
            else
            {
                if (Volume_Controls.Label_Normal == false)
                {
                    //label4.FlatStyle = FlatStyle.Standard;
                    Volume_Controls.Label_Normal = true;
                }
            }
        }

        /*private void IQ_Text_Box_TextChanged(object sender, EventArgs e)
        {
        }*/

        private void groupBox2_Enter(object sender, EventArgs e)
        {
            MonitorTextBoxText(" groupBox2_Enter -> Called");
        }

        private void IQ_Reset_All_button2_Click(object sender, EventArgs e)
        {
            bool reset = false;

            DialogResult ret = MessageBox.Show("THIS RESETS ALL I/Q VALUES TO DEFAULT. \r\nARE YOU SURE YOU WANT TO CONTINUE?",
                "MSCC", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            if (ret == DialogResult.Yes)
            {
                if (IQ_Controls.IQ_RX_MODE_ACTIVE)
                {
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_RX_TX, 0);
                    reset = true;
                }
                else
                {
                    if (IQ_Controls.IQ_TX_MODE_ACTIVE)
                    {
                        oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_RX_TX, 1);
                        reset = true;
                    }
                }
                if (reset)
                {
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_DEFAULTS, 1);
                    DialogResult reset_ret = MessageBox.Show(" All I/Q values have been to default values",
                                "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    IQ_Reset_All_button2.Text = "RESET ALL";
                    LefthScrollBar1.Value = 0;
                    IQLefttextBox2.Text = "0";
                    if (IQ_Controls.IQ_TX_MODE_ACTIVE)
                    {
                        if (IQ_Controls.Current_TX_Band != IQ_Controls.IQ_INVALID_BAND)
                        {
                            oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.Current_TX_Band);
                        }
                    }
                    if (IQ_Controls.IQ_RX_MODE_ACTIVE)
                    {
                        if (IQ_Controls.Current_RX_Band != IQ_Controls.IQ_INVALID_BAND)
                        {
                            oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.Current_RX_Band);
                        }
                    }
                }
                else
                {
                    DialogResult mode_ret = MessageBox.Show("Calibration Mode not Set.  Select a calibration mode and retry",
                                "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }

        private void Freq_Cal_groupBox4_Enter(object sender, EventArgs e)
        {
            MonitorTextBoxText(" Freq_Cal_groupBox4_Enter -> Called");
            IQ_groupBox3.Visible = true;
            IQ_groupBox3.Enabled = false;
        }

        private void IQ_groupBox3_Enter(object sender, EventArgs e)
        {
            MonitorTextBoxText(" IQ_groupBox3_Enter -> Called");
            Freq_Cal_groupBox4.Enabled = false;
            Freq_Cal_groupBox4.Visible = true;
        }

        /*private void label6_Click(object sender, EventArgs e)
        {
        }*/

        private void Volume_textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void Calibration_progressBar1_Click(object sender, EventArgs e)
        {
        }

        private void Standard_Carrier_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 fr = 0;
            short mode_number;
            int value = 0;

            if (oCode.isLoading) return;
            if (!Master_Controls.Transceiver_Warming)
            {
                MonitorTextBoxText(" Standard_Carrier_listBox1_SelectedIndexChanged -> Called.");
                label57.Visible = true;
                calibratebutton1.Visible = true;
                Calibration_progressBar1.Visible = true;
                //Progress_label58.Visible = true;
                Frequency_Calibration_controls.Display_Wait_count = 9;
                Freq_Cal_timer4.Enabled = true;
                Freq_Check_Button.Visible = true;
                Freq_Cal_Reset_button4.Visible = true;
                Freq_Cal_checkBox3.Visible = true;
                value = Standard_Carrier_listBox1.SelectedIndex;
                Frequency_Calibration_controls.standard_carrier_selected = true;
                switch (value)
                {
                    case 0:
                        fr = 2500000;
                        break;

                    case 1:
                        fr = 3330000;
                        break;

                    case 2:
                        fr = 5000000;
                        break;

                    case 3:
                        fr = 7850000;
                        break;

                    case 4:
                        fr = 9996000;
                        break;

                    case 5:
                        fr = 10000000;
                        break;

                    case 6:
                        fr = 14670000;
                        break;

                    case 7:
                        fr = 15000000;
                        break;

                    case 8:
                        fr = 20000000;
                        break;

                    case 9:
                        fr = 30000000;
                        break;
                }
                //freqtextBox1.Text = Convert.ToString(fr);
                Frequency_Calibration_controls.standard_carrier_selected = true;
                Frequency_Calibration_controls.Calibration_frequency = fr;
                Frequency_Calibration_controls.Display_Wait = true;

                mode_number = Convert_mode_char_to_digit('A');
                Last_used.GEN.Freq = fr;
                Last_used.GEN.Mode = 'A';
                genradioButton.Checked = true;
                genradioButton_CheckedChanged(null, null);
                oCode.DisplayFreq = Last_used.GEN.Freq;
                Display_Main_Freq();
                int MHz = fr / 1000000;
                int KHz = (fr - (MHz * 1000000)) / 1000;
                int Hz = fr - (MHz * 1000000) - (KHz * 1000);
                Panadapter_Controls.Frequency = oCode.DisplayFreq;
                //Panadapter_Controls.Updated_Frequency = oCode.DisplayFreq;
                Panadapter_Controls.Freq_Set_By_Master = true;
                ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
                oCode.SendCommand32(txsocket, txtarget, Frequency_Calibration_controls.CMD_SET_STANDARD_CARRIER, fr);
                //oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                //oCode.SendCommand(txsocket, txtarget, Filter_control.CMD_SET_BW_HICUT, Filter_control.High_Cut_Filter_200_Index);
                //oCode.SendCommand(txsocket, txtarget, Filter_control.CMD_SET_CW_PITCH, Filter_control.CW_Pitch_600_Index);
                MonitorTextBoxText(" Standard_Carrier_listBox1_SelectedIndexChanged -> Finished.");
            }
            else
            {
                MessageBox.Show("Transceiver is in Warm Up Mode. Try again later after warm up is complete", "MSCC",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Favorites_textBox2_TextChanged(object sender, EventArgs e)
        {
            if (Favorites_textBox2.Text == "Enter Name")
            {
                Favorites_Controls.Name_Entered = false;
            }
            else
            {
                Favorites_Controls.Name_Entered = true;
            }
        }

        private void Favorites_textBox2_Entered(object sender, EventArgs e)
        {
            Favorites_textBox2.Text = "";
            Favorites_Controls.Name_Entered = false;
        }

        private void firmwarelabel16_Click_1(object sender, EventArgs e)
        {
        }

        private void MS_SDR_Version_label16_Click(object sender, EventArgs e)
        {
        }

        private void SDRcore_Recv_Version_label16_Click(object sender, EventArgs e)
        {
        }

        private void SDRcore_Trans_Version_Click(object sender, EventArgs e)
        {
        }

        /*private void Compression_Level_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value;
            if (oCode.isLoading) return;
            value = Compression_Level_hScrollBar1.Value;
            if (Volume_Controls.Previous_Compression_Value == value) return;
            Volume_Controls.Previous_Compression_Value = (short)value;
            Compression_label44.Text = Convert.ToString(Volume_Controls.Previous_Compression_Value) + " db";
            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_COMPRESSION_LEVEL, Volume_Controls.Previous_Compression_Value);
        }

        private void Compression_button2_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (Volume_Controls.Compression_State == 0)
            {
                Volume_Controls.Compression_State = 1;
                Compression_button2.BackColor = Color.Red;
                Compression_button2.ForeColor = Color.White;
                Compression_button2.FlatStyle = FlatStyle.Popup;
                Compression_button2.Text = "Compression ON";

                Compression_button4.BackColor = Color.Red;
                Compression_button4.ForeColor = Color.White;
                Compression_button4.FlatStyle = FlatStyle.Popup;
                Compression_button4.Text = "C";
            }
            else
            {
                Volume_Controls.Compression_State = 0;
                Compression_button2.BackColor = Control.DefaultBackColor;
                Compression_button2.ForeColor = Color.Green;
                Compression_button2.FlatStyle = FlatStyle.Standard;
                Compression_button2.Text = "Compression OFF";

                Compression_button4.BackColor = Control.DefaultBackColor;
                Compression_button4.ForeColor = Color.Green;
                Compression_button4.FlatStyle = FlatStyle.Standard;
                Compression_button4.Text = "C";
            }
            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_COMPRESSION_STATE, Volume_Controls.Compression_State);
        }
        */
        /*private void Compression_Main_Tab_label16_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (Volume_Controls.Compression_State == 0)
            {
                Volume_Controls.Compression_State = 1;
                Compression_button2.BackColor = Color.Red;
                Compression_button2.ForeColor = Color.White;
                Compression_button2.FlatStyle = FlatStyle.Popup;
                Compression_button2.Text = "Compression ON";

                Compression_Main_Tab_label16.BackColor = Color.Red;
                Compression_Main_Tab_label16.ForeColor = Color.White;
                Compression_Main_Tab_label16.FlatStyle = FlatStyle.Popup;
                Compression_Main_Tab_label16.Text = "C";
            }
            else
            {
                Volume_Controls.Compression_State = 0;
                Compression_button2.BackColor = Control.DefaultBackColor;
                Compression_button2.ForeColor = Color.Green;
                Compression_button2.FlatStyle = FlatStyle.Standard;
                Compression_button2.Text = "Compression OFF";

                Compression_Main_Tab_label16.BackColor = Control.DefaultBackColor;
                Compression_Main_Tab_label16.ForeColor = Color.Green;
                Compression_Main_Tab_label16.FlatStyle = FlatStyle.Standard;
                Compression_Main_Tab_label16.Text = "C";
            }
            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_COMPRESSION_STATE, Volume_Controls.Compression_State);
        }*/

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!Favorites_Controls.Sorted)
            {
                MonitorTextBoxText(" checkBox1 -> Called -> Sort");
                B160_Favs_listView1.Sorting = SortOrder.Ascending;
                B80_Favs_listView1.Sorting = SortOrder.Ascending;
                B60_Favs_listView1.Sorting = SortOrder.Ascending;
                B40_Favs_listView1.Sorting = SortOrder.Ascending;
                B30_Favs_listView1.Sorting = SortOrder.Ascending;
                B20_Favs_listView1.Sorting = SortOrder.Ascending;
                B17_Favs_listView1.Sorting = SortOrder.Ascending;
                B15_Favs_listView1.Sorting = SortOrder.Ascending;
                B12_Favs_listView1.Sorting = SortOrder.Ascending;
                B10_Favs_listView1.Sorting = SortOrder.Ascending;
                B160_Favs_listView1.Sort();
                B80_Favs_listView1.Sort();
                B60_Favs_listView1.Sort();
                B40_Favs_listView1.Sort();
                B30_Favs_listView1.Sort();
                B20_Favs_listView1.Sort();
                B17_Favs_listView1.Sort();
                B15_Favs_listView1.Sort();
                B12_Favs_listView1.Sort();
                B10_Favs_listView1.Sort();
                Initialize_Favorites();
                Favorites_Controls.Sorted = true;
            }
            else
            {
                MonitorTextBoxText(" checkBox1 -> Called -> No Sort");
                Favorites_Controls.Sorted = false;
                B160_Favs_listView1.Sorting = SortOrder.None;
                B80_Favs_listView1.Sorting = SortOrder.None;
                B60_Favs_listView1.Sorting = SortOrder.None;
                B40_Favs_listView1.Sorting = SortOrder.None;
                B30_Favs_listView1.Sorting = SortOrder.None;
                B20_Favs_listView1.Sorting = SortOrder.None;
                B17_Favs_listView1.Sorting = SortOrder.None;
                B15_Favs_listView1.Sorting = SortOrder.None;
                B12_Favs_listView1.Sorting = SortOrder.None;
                B10_Favs_listView1.Sorting = SortOrder.None;
                B160_Favs_listView1.Sort();
                B80_Favs_listView1.Sort();
                B60_Favs_listView1.Sort();
                B40_Favs_listView1.Sort();
                B30_Favs_listView1.Sort();
                B20_Favs_listView1.Sort();
                B17_Favs_listView1.Sort();
                B15_Favs_listView1.Sort();
                B12_Favs_listView1.Sort();
                B10_Favs_listView1.Sort();
                Initialize_Favorites();
            }
        }

        /*private void AGC_label16_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            MonitorTextBoxText(" AGC_label16_Click -> AGC_ALC_Notch_Controls.AGC_Level: " + AGC_ALC_Notch_Controls.AGC_Level);
            AGC_ALC_Notch_Controls.AGC_Level++;
            if (AGC_ALC_Notch_Controls.AGC_Level > 2) AGC_ALC_Notch_Controls.AGC_Level = 0;

            switch (AGC_ALC_Notch_Controls.AGC_Level)
            {
                case 0:
                    AGC_label16.Text = "S";
                    AGC_listBox1.SelectedIndex = 0;
                    break;

                case 1:
                    AGC_label16.Text = "M";
                    AGC_listBox1.SelectedIndex = 1;
                    break;

                case 2:
                    AGC_label16.Text = "F";
                    AGC_listBox1.SelectedIndex = 2;
                    break;
            }
            oCode.SendCommand(txsocket, txtarget, AGC_ALC_Notch_Controls.CMD_GET_SET_AGC, (byte)AGC_ALC_Notch_Controls.AGC_Level);
            MonitorTextBoxText(" AGC_label16_Click -> AGC_ALC_Notch_Controls.AGC_Level: " + AGC_ALC_Notch_Controls.AGC_Level);
        }*/

        private void Backup_button2_Click(object sender, EventArgs e)
        {
            String Logfile_path;
            String path;
            String backup_path;
            string fileName = "test.txt";

            if (oCode.isLoading) return;

            //oCode.Platform = (int)Environment.OSVersion.Platform;

#if RPI
            path = "/home/mscc/mscc/mscc-logs";
            Logfile_path = path;
            backup_path = path + "/backup";
#else
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Logfile_path = (path + "\\multus-sdr-client");
            backup_path = (path + "\\multus-sdr-client\\backup");
#endif

            MonitorTextBoxText(" (0) Backup_button2_Click -> Ms_sdr_path " + Logfile_path + " backup_path " + backup_path);
            string destFile = System.IO.Path.Combine(backup_path, fileName);

            System.IO.Directory.CreateDirectory(backup_path);
            MonitorTextBoxText(" (1) Backup_button2_Click -> Ms_sdr_path " + Logfile_path + " backup_path " + backup_path);
            if (System.IO.Directory.Exists(Logfile_path))
            {
                MonitorTextBoxText(" (3) Backup_button2_Click -> Ms_sdr_path " + Logfile_path + " backup_path " + backup_path);
                string[] files = System.IO.Directory.GetFiles(Logfile_path);
                foreach (string s in files)
                {
                    fileName = System.IO.Path.GetFileName(s);
                    destFile = System.IO.Path.Combine(backup_path, fileName);
                    System.IO.File.Copy(s, destFile, true);
                }
            }
        }

        private void Restore_button2_Click(object sender, EventArgs e)
        {
            String path;
            int Delete_exit_code = 0;
            DialogResult ret;

            if (oCode.isLoading) return;
            ret = MessageBox.Show("Are you sure you want to" + "\r\n" + "" +
                "RESET ALL INITIALIZATION FILES TO DEFAULT?" + "\r\n" +
                "A BACK UP WILL BE PERFORMED BEFORE INITIALIZATION" + "\r\n" +
                "MSCC Will STOP after this operation." + "\r\n" + "MSCC will need restarted"
                , "MSCC",
               MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (ret == DialogResult.Yes)
            {
                Zip_button2_Click(null, null);
                path = AppDomain.CurrentDomain.BaseDirectory + "\\delete-all.exe";
                MonitorTextBoxText(" Restore_button2_Click -> Ms_sdr_path " + path);
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_STOP, 1);
                Thread.Sleep(500);
                SDRprocesses.initializer = new Process();
                try
                {
                    SDRprocesses.initializer.StartInfo.UseShellExecute = true;
                    SDRprocesses.initializer.StartInfo.FileName = path;
                    SDRprocesses.initializer.StartInfo.CreateNoWindow = false;
                    SDRprocesses.initializer.StartInfo.Arguments = "yes";
                    SDRprocesses.initializer.Start();
                    SDRprocesses.initializer.WaitForExit(60000);
                    Delete_exit_code = SDRprocesses.initializer.ExitCode;
                    if (Delete_exit_code != 0)
                    {
                        ret = MessageBox.Show("Delete ALL FAILED \r\n", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                catch (Exception)
                {
                    ret = MessageBox.Show("Delete ALL Did not run \r\n", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    Delete_exit_code = 1;
                }
                if (Delete_exit_code == 0)
                {
                    Thread.Sleep(500);
                    Application.Exit();
                }
            }
        }

        private void NB_button2_Click(object sender, EventArgs e)
        {
            if (!NB_Controls.NB_Button_On)
            {
                NB_button2.BackColor = Color.Red;
                NB_button2.ForeColor = Color.White;
                NB_button2.FlatStyle = FlatStyle.Popup;
                NB_Controls.NB_Button_On = true;

                NB_ON_OFF_button2.BackColor = Color.Red;
                NB_ON_OFF_button2.ForeColor = Color.White;
                NB_ON_OFF_button2.FlatStyle = FlatStyle.Popup;
                NB_ON_OFF_button2.Text = "ON";
                NB_Controls.NB_Main_Button_On = true;
                oCode.SendCommand(txsocket, txtarget, NB_Controls.NB_ENABLE, 1);
            }
            else
            {
                NB_button2.BackColor = Color.Gainsboro;
                NB_button2.ForeColor = Color.Black;
                NB_Controls.NB_Button_On = false;

                NB_ON_OFF_button2.BackColor = Color.Gainsboro;
                NB_ON_OFF_button2.ForeColor = Color.Black;
                NB_ON_OFF_button2.Text = "OFF";
                NB_Controls.NB_Main_Button_On = false;
                oCode.SendCommand(txsocket, txtarget, NB_Controls.NB_ENABLE, 0);
            }
        }

        private void NR_button3_Click(object sender, EventArgs e)
        {
            if (!NR_Controls.NR_Button_On)
            {
                NR_button3.BackColor = Color.Red;
                NR_button3.ForeColor = Color.White;
                NR_Controls.NR_Button_On = true;
            }
            else
            {
                NR_button3.BackColor = Color.Gainsboro;
                NR_button3.ForeColor = Color.Black;
                NR_Controls.NR_Button_On = false;
            }
        }

        private void NB_ON_OFF_button2_Click(object sender, EventArgs e)
        {
            if (!NB_Controls.NB_Main_Button_On)
            {
                NB_ON_OFF_button2.BackColor = Color.Red;
                NB_ON_OFF_button2.ForeColor = Color.White;
                NB_ON_OFF_button2.Text = "ON";

                NB_button2.BackColor = Color.Red;
                NB_button2.ForeColor = Color.White;
                NB_Controls.NB_Button_On = true;
                NB_Controls.NB_Main_Button_On = true;
                oCode.SendCommand(txsocket, txtarget, NB_Controls.NB_ENABLE, 1);
            }
            else
            {
                NB_ON_OFF_button2.BackColor = Color.Gainsboro;
                NB_ON_OFF_button2.ForeColor = Color.Black;
                NB_ON_OFF_button2.Text = "OFF";

                NB_button2.BackColor = Color.Gainsboro;
                NB_button2.ForeColor = Color.Black;
                NB_Controls.NB_Button_On = false;
                NB_Controls.NB_Main_Button_On = false;
                oCode.SendCommand(txsocket, txtarget, NB_Controls.NB_ENABLE, 0);
            }
        }

        private void NB_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value = 0;
            if (oCode.isLoading) return;
            value = NB_hScrollBar1.Value;
            if (NB_Controls.NB_Pulse_Width == value) return;
            NB_Controls.NB_Pulse_Width = value;
            NB_Width_label16.Text = Convert.ToString(NB_Controls.NB_Pulse_Width) + " uS";
            oCode.SendCommand32(txsocket, txtarget, NB_Controls.NB_PULSE_WIDTH, NB_Controls.NB_Pulse_Width);
        }

        private void NB_Threshold_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value = 0;
            float percent = 0.0f;
            if (oCode.isLoading) return;
            if (!Master_Controls.code_triggered)
            {
                value = NB_Threshold_hScrollBar1.Value;
                if (NB_Controls.NB_Threshold == value) return;
                NB_Controls.NB_Threshold = value;
                percent = ((float)value / 1000.0f) * 100.0f;
                NB_Threshold_label16.Text = percent.ToString("F1") + " %";
                //NB_Threshold_label16.Text = Convert.ToString((percent * 100)) + " %";
                oCode.SendCommand32(txsocket, txtarget, NB_Controls.NB_THRESHOLD, NB_Controls.NB_Threshold);
            }
        }

        private void NB_Width_label16_Click(object sender, EventArgs e)
        {
        }

        private void Zip_button2_Click(object sender, EventArgs e)
        {
            String Ms_sdr_path;
            String path;
            String Zip_path;
            bool create_failed = false;

            if (oCode.isLoading) return;
            //Master_Controls.Zip_Backup = true;
            Backup_button2_Click(null, null);
            //oCode.Platform = (int)Environment.OSVersion.Platform;

#if RPI
            path = "/home/mscc/mscc/mscc-logs";
            Ms_sdr_path = (path + "/backup");
            Zip_path = Ms_sdr_path + "ziplog.zip";
#else
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Ms_sdr_path = (path + "\\multus-sdr-client\\backup");
            Zip_path = AppDomain.CurrentDomain.BaseDirectory;
            Zip_path = (Zip_path + "ziplog.zip");
#endif
            if (System.IO.Directory.Exists(Ms_sdr_path))
            {
                if (System.IO.File.Exists(Zip_path))
                {
                    System.IO.File.Delete(Zip_path);
                    MonitorTextBoxText(" Zip_button2_Click -> Zip File Deleted");
                }
                try
                {
                    ZipFile.CreateFromDirectory(Ms_sdr_path, Zip_path);
                }
                catch
                {
                    MessageBox.Show("Zip File Creation FAILED.  Is this a Windows 7 PC? Please report to Multus SDR, LLC.", "MSCC",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MonitorTextBoxText(" Zip_button2_Click -> Zip File Create FAILED");
                    create_failed = true;
                    throw;
                }
                if (!create_failed)
                {
                    MessageBox.Show("A zip file has been created of all Initialization and Log Files at: \r\n" + Zip_path, "MSCC",
                         MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            //Master_Controls.Zip_Backup = false;
            MonitorTextBoxText(" Zip_button2_Click -> Finished");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            switch (Comm_Port_Controls.HR50_Controls.Button_Toggle)
            {
                case false:
                    if (Comm_Port_Controls.HR50_Controls.Comm_Port_Selected == true &&
                        !Comm_Port_Controls.HR50_Controls.Comm_Port_Open_by_server)
                    {
                        //if (Comm_Port_Controls.HR50_Controls.Comm_Name_Index == 200)
                        //{
                        //    DialogResult ret = MessageBox.Show("Select a Comm Port", "MSCC",
                        //       MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        //   return;
                        //}
                        oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_HR50_COMM_START, 1);
                        button2.BackColor = Color.Red;
                        button2.ForeColor = Color.White;
                        button2.Text = "ACTIVE";
                        button2.FlatStyle = FlatStyle.Popup;
                        Comm_Port_Controls.HR50_Controls.Comm_Port_Open = true;
                        Comm_Port_Controls.HR50_Controls.Button_Toggle = true;
                    }
                    else
                    {
                        if (!Comm_Port_Controls.HR50_Controls.Comm_Port_Open_by_server)
                        {
                            DialogResult ret = MessageBox.Show("Select a Comm Port", "MSCC",
                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            return;
                        }
                    }
                    break;

                case true:
                    MonitorTextBoxText(" button2 -> Comm Port is Closed ");
                    button2.BackColor = Color.Gainsboro;
                    button2.ForeColor = Color.Black;
                    button2.Text = "CLOSED";
                    button2.FlatStyle = FlatStyle.Standard;
                    Comm_Port_Controls.HR50_Controls.Comm_Port_Open = false;
                    Comm_Port_Controls.HR50_Controls.Button_Toggle = false;
                    Comm_Port_Controls.HR50_Controls.Comm_Name_Index = 200;
                    Comm_Port_Controls.HR50_Controls.Comm_Port_Open_by_server = false;
                    oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_HR50_COMM_START, 0);
                    break;
            }
        }

        private void HR50_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            String temp_string;
            String comm_name;
            int param_pos;
            int index = 0;
            short temp_int = 0;
            int comm_name_lenght = 0;

            if (oCode.isLoading) return;
            if (!Master_Controls.code_triggered)
            {
                MonitorTextBoxText(" HR50_listBox1_SelectedIndexChanged ");
                index = HR50_listBox1.SelectedIndex;
                Comm_Port_Controls.HR50_Controls.Comm_Name_Index = index;
                comm_name = HR50_listBox1.Text;
                comm_name_lenght = System.Text.ASCIIEncoding.ASCII.GetByteCount(comm_name);
                param_pos = HR50_listBox1.Text.IndexOf("COM");
                comm_name_lenght = comm_name_lenght - 3;
                temp_string = comm_name.Substring((param_pos + 3), comm_name_lenght);
                Int16.TryParse(temp_string, out temp_int);
                Comm_Port_Controls.HR50_Controls.Comm_Port_Selected = true;
                if (Comm_Port_Controls.HR50_Controls.Comm_Port_Open == false)
                {
                    if (Comm_Port_Controls.Box_indexes.Comm_Name_Index ==
                        Comm_Port_Controls.HR50_Controls.Comm_Name_Index)
                    {
                        if (Comm_Port_Controls.Comm_Port_Open == false)
                        {
                            oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_HR50_COMM_NAME_INDEX, (short)index);
                            oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_HR50_COMM_PORT, temp_int);
                        }
                        else
                        {
                            MessageBox.Show("Port is used by Main Port.  Select a different Port", "MSCC",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_HR50_COMM_NAME_INDEX, (short)index);
                        oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_HR50_COMM_PORT, temp_int);
                    }
                }
                else
                {
                    if (Comm_Port_Controls.HR50_Controls.Set_By_Server == false)
                    {
                        MessageBox.Show("Close port before selecting a new port", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Favorites_Update_button3_Click(object sender, EventArgs e)
        {
            int len = 0;
            String favs_name = "None";
            if (Favorites_Controls.Name_Entered)
            {
                len = Favorites_textBox2.TextLength;
                favs_name = Favorites_textBox2.Text;

                Favorites_Controls.Name_Entered = false;
                Favorites_textBox2.Text = "Enter Name";
            }
            else
            {
                favs_name = Convert.ToString(oCode.DisplayFreq);
                len = favs_name.Length;
                // DialogResult ret = MessageBox.Show("Enter a Name for this Update", "MSCC",MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Favorites_textBox2.Text = "Enter Name";
            }
            oCode.SendCommand_String(txsocket, txtarget, Favorites_Controls.CMD_SET_BAND_STACK_NAME, favs_name, len);
            oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_UPDATE_BAND_STACK, 1);
            //Get_FTP_Init_File();
        }

        /*private void Panadapter_1_Button_Click(object sender, EventArgs e)
        {
            if (sender == null)
            {
                MonitorTextBoxText(" Panadapter Popup Entered -> NULL sender");
                Window_controls.Panadapter_New.spectrum_window_function_by_user = false;
            }
            else
            {
                MonitorTextBoxText(" Panadapter Entered -> Button Clicked");
                Window_controls.Panadapter_New.spectrum_window_function_by_user = true;
            }
            if (Window_controls.Panadapter_window_new != null)
            {
                MonitorTextBoxText("  Panadapter Popup is not null");
                Window_controls.Panadapter_New.panadapter_form_displosed = Window_controls.Panadapter_window_new.IsDisposed;
                if (Window_controls.Panadapter_New.panadapter_form_displosed)
                {
                    MonitorTextBoxText(" Panadapter Popup Disposed");
                }
                else
                {
                    MonitorTextBoxText(" Panadapter Popup Not disposed");
                }
            }
            if (Window_controls.Panadapter_window_new == null)
            {
                Window_controls.Panadapter_window_new = new Panadapter_1();
                MonitorTextBoxText(" Panadapter Popup Created");
                Window_controls.Panadapter_New.panadapter_form_displosed = false;
                if (Window_controls.Panadapter_window_new != null)
                {
                    Window_controls.Panadapter_New.panadapter_window_created = true;
                }
            }
            if (Window_controls.Panadapter_window_new != null && !Window_controls.Panadapter_New.panadapter_form_displosed)
            {
                Window_controls.Panadapter_New.panadapter_form_state = (int)Window_controls.Panadapter_window_new.WindowState;
                MonitorTextBoxText(" Panadapter Popup State: " + Window_controls.Panadapter_window_new.WindowState);

                if (Window_controls.Panadapter_New.display_panadapter_form)
                {
                    Window_controls.Panadapter_New.spectrum_window_visable = true;
                    Window_controls.Panadapter_New.spectrum_window_minimized = false;
                    Window_controls.Panadapter_New.spectrum_window_hidden = false;
                    Window_controls.Panadapter_New.panadapter_window_displayed = true;
                    Window_controls.Panadapter_New.display_panadapter_form = false;
                    Window_controls.Panadapter_window_new.WindowState = FormWindowState.Normal;
                    Window_controls.Panadapter_window_new.Show();
                    MonitorTextBoxText(" Panadapter Popup Show");
                    Window_controls.Panadapter_New.panadapter_form_state =
                        (int)Window_controls.Panadapter_window_new.WindowState;
                }
                else
                {
                    //if (Window_controls.Panadapter_New.panadapter_form_state == 1)
                    if(Window_controls.Panadapter_window_new.WindowState == FormWindowState.Minimized)
                    {
                        MonitorTextBoxText(" Panadapter Popup State:" + Window_controls.Panadapter_window_new.WindowState);
                        MonitorTextBoxText(" Panadapter Popup Restoring");
                        Window_controls.Panadapter_window_new.WindowState = FormWindowState.Normal;
                        Window_controls.Panadapter_New.spectrum_window_visable = true;
                        Window_controls.Panadapter_New.spectrum_window_minimized = false;
                        Window_controls.Panadapter_New.spectrum_window_hidden = false;
                        Window_controls.Panadapter_window_new.Activate();
                        Window_controls.Panadapter_window_new.Show();
                        Window_controls.Panadapter_window_new.Focus();
                        Window_controls.Panadapter_New.panadapter_window_displayed = true;
                        MonitorTextBoxText(" Panadapter Popup State:" + Window_controls.Panadapter_window_new.WindowState);
                    }
                    else
                    {
                        MonitorTextBoxText(" Panadapter Popup State:" + Window_controls.Panadapter_window_new.WindowState);
                        Window_controls.Panadapter_New.spectrum_window_minimized = true;
                        Window_controls.Panadapter_New.panadapter_window_displayed = false;
                        Window_controls.Panadapter_New.spectrum_window_hidden = true;
                        Window_controls.Panadapter_New.spectrum_window_visable = false;
                        Window_controls.Panadapter_New.display_panadapter_form = true;
                        Window_controls.Panadapter_window_new.WindowState = FormWindowState.Minimized;
                        MonitorTextBoxText(" Panadapter Popup State:" + Window_controls.Panadapter_window_new.WindowState);
                        MonitorTextBoxText(" Panadapter Popup Minimized");
                    }
                }
            }
            else
            {
                if (Window_controls.Panadapter_window_new == null)
                {
                    MonitorTextBoxText(" Panadapter Popup is NULL");
                }
                else
                {
                    MonitorTextBoxText(" Panadapter Popup not NULL");
                }
                if (Window_controls.Panadapter_window_new.IsDisposed)
                {
                    MonitorTextBoxText(" Panadapter Popup is Disposed");
                    MonitorTextBoxText(" Panadapter Popup -> Would close");
                    Window_controls.Panadapter_window_new.Close();

                    //Window_controls.freq_form_displosed = false;
                    Window_controls.Panadapter_window_new = null;
                    Window_controls.Panadapter_New.panadapter_window_displayed = false;
                    Window_controls.Panadapter_New.display_panadapter_form = true;
                }
                else
                {
                    MonitorTextBoxText(" Panadapter Popup is not Disposed");
                }
            }
            MonitorTextBoxText(" Panadapter Popup Finished");
        }*/



        //public void Update_Panadapter_1(uint Sequence)
        //{
        //    if (Window_controls.Panadapter_New.window_created &&
        //       Window_controls.Panadapter_window_new.WindowState != FormWindowState.Minimized)
        //   {
        //       var thisForm2 = Application.OpenForms.OfType<Panadapter_1>().Single();
        //       thisForm2.Display_Panadapter(Sequence);
        // }
        //}

        /*public void Start_Panadapter(uint Sequence)
        {
            MonitorTextBoxText(" Start_Panadapter Called");
            //Spectrum_checkBox2.Checked = true;
            //Panadapter_1_Button_Click(null, null);
            //Thread.Sleep(3000);
            //var thisForm2 = Application.OpenForms.OfType<Panadapter_1>().Single();
        }*/

        private void HR50_checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (!Comm_Port_Controls.HR50_Controls.Pass_Thru)
            {
                HR50_checkBox2.BackColor = Color.Red;
                HR50_checkBox2.ForeColor = Color.White;
                HR50_checkBox2.FlatStyle = FlatStyle.Popup;
                Comm_Port_Controls.HR50_Controls.Pass_Thru = true;
                oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_HR50_COMM_PASS_THRU, 0);
            }
            else
            {
                HR50_checkBox2.BackColor = Control.DefaultBackColor;
                HR50_checkBox2.ForeColor = Color.Green;
                HR50_checkBox2.FlatStyle = FlatStyle.Standard;
                Comm_Port_Controls.HR50_Controls.Pass_Thru = false;
                oCode.SendCommand(txsocket, txtarget,
                    Comm_Port_Controls.CMD_GET_SET_HR50_COMM_PASS_THRU, Comm_Port_Controls.HR50_Controls.PTT_Mode);
            }
        }

        private void HR50_PPT_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index;
            if (oCode.isLoading) return;
            index = HR50_PPT_listBox1.SelectedIndex;
            switch (index)
            {
                case 0:
                    Comm_Port_Controls.HR50_Controls.PTT_Mode = 1;
                    break;

                case 1:
                    Comm_Port_Controls.HR50_Controls.PTT_Mode = 2;
                    break;

                case 2:
                    Comm_Port_Controls.HR50_Controls.PTT_Mode = 3;
                    break;
            }
        }

        /*private void TX_Panadapter_checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
        }*/

        public void Write_Message(String Message)
        {
            MonitorTextBoxText(Message);
        }

        /*private void zedGraphControl1_Load(object sender, EventArgs e)
        {
        }
        */
        private void StartUP_label44_Click(object sender, EventArgs e)
        {
        }

        private void Set_Startup_Band()
        {
            MonitorTextBoxText(" timer2_Tick -> Setting Start UP Band: " + Convert.ToString(Master_Controls.Startup_Band));
            switch (Master_Controls.Startup_Band)
            {
                case 160:
                    main160radioButton10.Checked = true;
                    main160radioButton10_CheckedChanged(null, null);
                    break;
                case 80:
                    main80radioButton9.Checked = true;
                    main80radioButton9_CheckedChanged(null, null);
                    break;
                case 60:
                    main60radioButton8.Checked = true;
                    main60radioButton8_CheckedChanged(null, null);
                    break;
                case 40:
                    main40radioButton7.Checked = true;
                    main40radioButton7_CheckedChanged(null, null);
                    break;
                case 30:
                    main30radioButton6.Checked = true;
                    main30radioButton6_CheckedChanged(null, null);
                    break;
                case 20:
                    main20radioButton5.Checked = true;
                    main20radioButton5_CheckedChanged(null, null);
                    break;
                case 17:
                    main17radioButton4.Checked = true;
                    main17radioButton4_CheckedChanged(null, null);
                    break;
                case 15:
                    main15radiobutton.Checked = true;
                    main15radiobutton_CheckedChanged(null, null);
                    break;
                case 12:
                    main12radioButton2.Checked = true;
                    main12radioButton2_CheckedChanged(null, null);
                    break;
                case 10:
                    main10radioButton1.Checked = true;
                    main10radioButton1_CheckedChanged(null, null);
                    break;
                default:
                    genradioButton.Checked = true;
                    break;
            }
        }

        /*private void MFC_Set_Proficio()
        {    
#if FTDI
            transceiver_display = (byte)(0x10 | Tuning_Knob_Controls.Button_left_function);
            oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_TRANSCEIVER_DISPLAY, transceiver_display);
            transceiver_display = (byte)(0x20 | Tuning_Knob_Controls.Button_middle_function);
            oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_TRANSCEIVER_DISPLAY, transceiver_display);
            transceiver_display = (byte)(0x30 | Tuning_Knob_Controls.Button_right_function);
            oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_TRANSCEIVER_DISPLAY, transceiver_display);
            oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_STEP_VALUE, oCode.Freq_Tune_Index);
            oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_STAR,
                Tuning_Knob_Controls.Star_Position.Knob);
            oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_STAR,
                Tuning_Knob_Controls.Star_Position.Left);
            oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_STAR,
                Tuning_Knob_Controls.Star_Position.Middle);
            oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_STAR,
                Tuning_Knob_Controls.Star_Position.Right);
#endif
        }

        /*void Finish_initialization()
        {
           
            if (Audio_Device_Controls.Selected_Speaker != 0)
            {
                MonitorTextBoxText(
                    " Even_more_initialization -> Update_Speaker_listbox -> Selected Speaker -> Setting Speaker_listbox1.Selected_Index: "
                                + Audio_Device_Controls.Selected_Speaker);
                Speaker_listBox1.SelectedIndex = Audio_Device_Controls.Selected_Speaker;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_SPEAKER_DEVICE, Audio_Device_Controls.Selected_Speaker);
                Current_Speaker_label41.Text = Speaker_listBox1.Text;
            }
            else
            {
                Speaker_listBox1.SelectedIndex = Audio_Device_Controls.Default_Speaker;
                MonitorTextBoxText(
                    " Even_more_initialization -> Update_Speaker_listbox -> Selected Speaker -> Setting Speaker_listbox1.Selected_Index: "
                                + Audio_Device_Controls.Default_Speaker);
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_SPEAKER_DEVICE, Audio_Device_Controls.Default_Speaker);
                Current_Speaker_label41.Text = Speaker_listBox1.Text;
            }
            if (Audio_Device_Controls.Selected_Mic != 0)
            {
                Microphone_listBox1.SelectedIndex = Audio_Device_Controls.Selected_Mic;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_MIC_DEVICE, Audio_Device_Controls.Selected_Mic);
                Current_Mic_label41.Text = Microphone_listBox1.Text;
            }
            else
            {
                Microphone_listBox1.SelectedIndex = Audio_Device_Controls.Default_Mic;
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_MIC_DEVICE, Audio_Device_Controls.Default_Mic);
                Current_Mic_label41.Text = Microphone_listBox1.Text;
            }
        }*/

        private void timer3_Tick(object sender, EventArgs e)
        {
            MonitorTextBoxText(" timer3_Tick -> RUNNING");
            if (Master_Controls.Startup_Label_Toggle)
            {
                if (Master_Controls.Get_Network_file == false)
                {
#if !RPI
                    Get_FTP_Init_File();//Get the initialization files for the network addresses.
#endif
                    if (Master_Controls.FTP_File_Found == false)
                    {
                        timer3.Stop();
                        timer3.Enabled = false;
                        MonitorTextBoxText(" timer3_Tick -> Get_FTP_Init_File: FAILED");
                    }
                    Master_Controls.Get_Network_file = true;
                }
                if (Master_Controls.Initialize_network_status == false)
                {
                    Master_Controls.Initialize_network_status = Initialize_Network();
                    if (Master_Controls.Initialize_network_status == false)
                    {
                        timer3.Stop();
                        timer3.Enabled = false;
                        MonitorTextBoxText(" timer3_Tick -> Initialize_Network: FAILED");
                        Application.Exit();
                    }
                }
                Master_Controls.Initialize_network_status = true;
                StartUP_label44.ForeColor = Color.White;
                Master_Controls.Startup_Label_Toggle = false;
                if (Master_Controls.GUI_check_status == true)
                {
                    MonitorTextBoxText(" timer3_Tick -> Sending CMD_SEND_GUI_STATUS");
                    oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SEND_GUI_STATUS, 1);
                    Master_Controls.GUI_check_status = false;
                }

                if (Master_Controls.MSSDR_running == false && Master_Controls.Check_Server_Status_Count++ >=
                                                      Master_Controls.Check_Server_Limit)
                {
                    MonitorTextBoxText(" timer3_Tick -> Check_Server_Status_Count: " +
                        Convert.ToString(Master_Controls.Check_Server_Status_Count) + "Sending CMD_SEND_GUI_STATUS");
                    oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SEND_GUI_STATUS, 1);
                    Master_Controls.Check_Server_Status_Count = 0;
                }


                if (Master_Controls.MSSDR_running == true)
                {
                    MonitorTextBoxText(" Master_Controls.MSSDR_running");
                    StartUP_label44.Visible = false;
                    timer3.Stop();
                    timer3.Enabled = false;
                    Application.UseWaitCursor = false;
                    Cursor.Show();
                    //Get_FTP_Init_File();//Get the initialization files again in case solidus-mssdr changed the contents
                    Finish_Initialization();
                    Initialize_Favorites();
                    Initialize_Smeter();

                    //Update_Power_values();
                    checkBox1.Checked = true;
                    //if (!Tuning_Knob_Controls.MFC_Initialized)
                    //{
                    //    Initialize_MFC();
                    //}
                    MonitorTextBoxText(" Main Window Location: " + Convert.ToString(this.Location));
                    Keep_Alive_timer.Enabled = true;
                    timer2.Enabled = true;
                }
            }
            else
            {
                StartUP_label44.ForeColor = Color.Red;
                Master_Controls.Startup_Label_Toggle = true;
                Master_Controls.Startup_Label_Tick_count++;
                if (Master_Controls.Startup_Label_Tick_count > 60)
                {
                    timer3.Enabled = false;
                    Application.UseWaitCursor = true;
                    Cursor.Show();
                    MonitorTextBoxText(" Server did not respond at startup");
                    MessageBox.Show("Server Did not Respond at Startup \n\r MSCC Shutting Down", "MSCC",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
        }

        private void MSCC_Core_Version_label45_Click(object sender, EventArgs e)
        {
            //MSCC_Core_Version_label45.Text = Master_Controls.Version;
        }

        /*private void Line_Signal_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;
            if (oCode.isLoading) return;
            if (!Master_Controls.code_triggered)
            {
                if (Comm_Port_Controls.Comm_Port_Open != false)
                {
                    index = Line_Signal_listBox1.SelectedIndex;
                    switch (index)
                    {
                        case 0:
                            oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_PORT_PINS, 0);
                            break;

                        case 1:
                            oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_PORT_PINS, 1);
                            break;

                        case 2:
                            oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_PORT_PINS, 2);
                            break;
                    }
                }
                else
                {
                    DialogResult ret = MessageBox.Show("The COM PORT Must BE ACTIVE Before Selecting a Pin (CTS)", "MSCC",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void vButton1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Button Clicked", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }*/

        private void Tune_vButton2_Click_1(object sender, EventArgs e)
        {
            int status = 0;
            short power = 0;
            short mode_number = 0;
            // Tune button clicked
            MonitorTextBoxText(" Tune_vButton2 ");
            if (!Master_Controls.TX_Inhibited)
            {
                if (status == 0)
                {
                    if (Master_Controls.Tuning_Mode == false)
                    {
                        oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 1);
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, Mode.TUNE_mode);
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_RIG_TUNE, 1);
                        Tune_vButton2.BackColor = Color.Red;
                        Tune_vButton2.ForeColor = Color.White;
                        Tune_vButton2.Text = "TUN";
                        //buttTune.Text = "TUNING";
                        Master_Controls.Tuning_Mode = true;
                        Master_Controls.Transmit_Mode = true;
                        AM_Carrier_hScrollBar1.Enabled = false;
                        Power_hScrollBar1.Enabled = false;
                        CW_Power_hScrollBar1.Enabled = false;
                        buttTune.BackColor = Color.Red;
                        buttTune.ForeColor = Color.White;
                    }
                    else
                    {
                        if (!Settings.Default.Speaker_MutED)
                        {
                            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                        }
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_RIG_TUNE, 0);
                        power = get_previous_power(oCode.current_band);
                        mode_number = Convert_mode_char_to_digit(Last_used.Current_mode);
                        MonitorTextBoxText(" Tune_vButton2 -> Current Band: " + oCode.current_band +
                            " Current Mode Number: " + mode_number);
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                        Tune_vButton2.BackColor = Color.Gainsboro;
                        Tune_vButton2.ForeColor = Color.Black;
                        Tune_vButton2.Text = "TUN";
                        //buttTune.Text = "TUNE";
                        Master_Controls.Tuning_Mode = false;
                        Master_Controls.Transmit_Mode = false;
                        AM_Carrier_hScrollBar1.Enabled = true;
                        Power_hScrollBar1.Enabled = true;
                        CW_Power_hScrollBar1.Enabled = true;
                        buttTune.BackColor = Color.Gainsboro;
                        buttTune.ForeColor = Color.Black;
                    }
                }
            }
            status = 0;
            MonitorTextBoxText(" Tune_vButton2 -> finished ");
        }

        private void PA_vButton1_Click_1(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (Solidus_Controls.Mia_Status)
            {
                if (Master_Controls.QRP_Mode)
                {
                    PA_vButton1.BackColor = Color.Red;
                    PA_vButton1.ForeColor = Color.White;
                    PA_vButton1.FlatStyle = FlatStyle.Popup;
                    PA_vButton1.Text = "QRO";
                    Master_Controls.QRP_Mode = false;
                    oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 1);
                    MonitorTextBoxText(" PA_vButton1_Click -> Set QRO Mode");
                }
                else
                {
                    PA_vButton1.Text = "QRP";
                    PA_vButton1.BackColor = Color.Gainsboro;
                    PA_vButton1.ForeColor = Color.Black;
                    Master_Controls.QRP_Mode = true;
                    oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 0);
                    MonitorTextBoxText(" PA_vButton1_Click -> Set QRP Mode");
                }
            }
            else
            {
                MessageBox.Show("Meter or Amplifier NOT attached", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void IQ_Freq_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value;
            int total_freq;
            if (oCode.isLoading) return;
            value = IQ_Freq_hScrollBar1.Value;
            if (IQ_Controls.IQ_RX_Offset != value)
            {
                IQ_Controls.IQ_RX_Offset = value;
                if (IQ_Controls.Up_24KHz)
                {
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, (IQ_Controls.IQ_RX_Freq +
                    IQ_Controls.IQ_RX_Up_24K_freq + IQ_Controls.IQ_RX_Offset));
                    total_freq = IQ_Controls.IQ_RX_Freq + IQ_Controls.IQ_RX_Up_24K_freq + IQ_Controls.IQ_RX_Offset;
                }
                else
                {
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, (IQ_Controls.IQ_RX_Freq + IQ_Controls.IQ_RX_Offset));
                    total_freq = IQ_Controls.IQ_RX_Freq + IQ_Controls.IQ_RX_Offset;
                }
                Display_IQ_freq(total_freq);
            }
        }

        private void IQ_UP24KHz_checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (IQ_Controls.Previous_RX_Band != IQ_Controls.Current_RX_Band)
            {
                oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.Current_RX_Band);
                IQ_Controls.Previous_RX_Band = IQ_Controls.Current_RX_Band;
            }
            if (!IQ_Controls.Up_24KHz)
            {
                IQ_Controls.Up_24KHz = true;
                IQ_UP24KHz_checkBox2.ForeColor = Color.White;
                IQ_UP24KHz_checkBox2.BackColor = Color.Red;

                Display_IQ_freq(IQ_Controls.IQ_RX_Freq + IQ_Controls.IQ_RX_Up_24K_freq + IQ_Controls.IQ_RX_Offset);
                oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, (IQ_Controls.IQ_RX_Freq +
                    IQ_Controls.IQ_RX_Up_24K_freq + IQ_Controls.IQ_RX_Offset));
            }
            else
            {
                IQ_Controls.Up_24KHz = false;
                IQ_UP24KHz_checkBox2.BackColor = Color.Gainsboro;
                IQ_UP24KHz_checkBox2.ForeColor = Color.Black;
                Display_IQ_freq(IQ_Controls.IQ_RX_Freq + IQ_Controls.IQ_RX_Offset);

                oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, (IQ_Controls.IQ_RX_Freq + IQ_Controls.IQ_RX_Offset));
            }
        }

        private void Reset_Freq_button3_Click(object sender, EventArgs e)
        {
            IQ_Freq_hScrollBar1.Value = 0;
            IQ_Controls.IQ_RX_Offset = 0;
            if (IQ_Controls.Up_24KHz)
            {
                Display_IQ_freq(IQ_Controls.IQ_RX_Freq + IQ_Controls.IQ_RX_Up_24K_freq);
                oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, IQ_Controls.IQ_RX_Freq + IQ_Controls.IQ_RX_Up_24K_freq);
            }
            else
            {
                Display_IQ_freq(IQ_Controls.IQ_RX_Freq);
                oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, IQ_Controls.IQ_RX_Freq);
            }
        }

        private void Tune_Power_label37_Click(object sender, EventArgs e)
        {
        }

        /*private void Freq_Cal_Commit_button3_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
        }*/

        private void Freq_Check_Button_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (!Frequency_Calibration_controls.Calibration_In_Progress)
            {
                if (!Frequency_Calibration_controls.standard_carrier_selected)
                {
                    MessageBox.Show("No Standard Carrier Selected. Select a Standard Carrier", "MSCC",
                                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
                if (Filter_control.CW_Pitch != 600)
                {
                    MessageBox.Show("CW Pitch must be set to 600", "MSCC",
                                   MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
                Calibration_progressBar1.Value = 0;
                oCode.SendCommand32(txsocket, txtarget,
                    Frequency_Calibration_controls.CMD_SET_FREQ_CAL_CHECK, 1);
                Frequency_Calibration_controls.Calibration_In_Progress = true;
                Frequency_Calibration_controls.Check_only = true;
                calibratebutton1.Text = "CHECKING";
                Frequency_Calibration_controls.Calibration_Checked = true;
                Freq_CAl_Progress_Lable.Visible = true;
            }
            else
            {
                MessageBox.Show("Frequency Calibration Check In Progress.", "MSCC-Core",
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
        }

        private void UTC_Date_label46_Click(object sender, EventArgs e)
        {
        }

        /*private void Freq_Check_Button_Click_1(object sender, EventArgs e)
        {
        }

        private void label48_Click(object sender, EventArgs e)
        {
        }*/

        /*private void Middle_Button_comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
#if FTDI
            int index = 0;
            if (!Master_Controls.code_triggered)
            {
                index = Middle_Button_comboBox3.SelectedIndex;
                Tuning_Knob_Controls.Button_middle_function = (byte)index;
                Update_MFC_Init_File();
            }
#endif
        }*/

        /*private void Tuning_Knob_checkBox2_CheckedChanged(object sender, EventArgs e)
        {
           /* {
                if (oCode.isLoading) return;
#if FTDI
                uint bytes_written = 0;
                byte[] sentBytes = new byte[2];
                if (!Master_Controls.code_triggered)
                {
                    if (!Relay_Board_Controls.On)
                    {
                        if (!Tuning_Knob_Controls.On)
                        {
                        
                            if (Tuning_Knob_Controls.Open)
                            {
                                Tuning_Knob_Controls.On = true;
                                Update_MFC_Init_File();
                                MonitorTextBoxText(" Tuning Knob Open. Multiplier: "
                                    + Convert.ToString(Tuning_Knob_Controls.Step_Function.Multiplier));
                            }
                            else
                            {
                               
                                Tuning_Knob_checkBox2.Checked = false;
                                Update_MFC_Init_File();
                                MessageBox.Show("Tuning Knob OPEN FAILED", "MSCC",
                                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                        }
                        else
                        {
                            if (Tuning_Knob_Controls.Open)
                            {
                                sentBytes[0] = (byte)(sentBytes[0] | 0xFF);
                                Tuning_Knob_Controls.Device.Write(sentBytes, 1, ref bytes_written);
                                Tuning_Knob_Controls.Device.Close();
                                MonitorTextBoxText(" Tuning Knob CLOSED");
                            }
                            Tuning_Knob_Controls.On = false;
                           
                            Update_MFC_Init_File();
                        }
                    }
                    else
                    {
                        Tuning_Knob_checkBox2.Checked = false;
                        Update_MFC_Init_File();
                        MessageBox.Show("Relay Board In use. Turn off Relay Board", "MSCC",
                                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
#else
            MessageBox.Show("Incorrect MSCC Version. The FTDI Version is required", "MSCC",
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
             Tuning_Knob_checkBox2.Checked = false;
#endif
            }
        }*/

        private void Filter_Low_listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //int swap = 0;
            if (oCode.isLoading == true) return;
            //swap = Swap_lowcut_listbox_index(Filter_Low_listBox1.SelectedIndex);
            Filter_control.Filter_Low_Index = (short)Filter_Low_listBox1.SelectedIndex;
            if (Filter_control.Previous_Filter_Low_Index == Filter_control.Filter_Low_Index) return;
            Filter_control.Previous_Filter_Low_Index = Filter_control.Filter_Low_Index;
            switch (oCode.current_band)
            {
                case 160:
                    Last_used.B160.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 80:
                    Last_used.B80.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 60:
                    Last_used.B60.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 40:
                    Last_used.B40.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 30:
                    Last_used.B30.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 20:
                    Last_used.B20.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 15:
                    Last_used.B15.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 17:
                    Last_used.B17.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 12:
                    Last_used.B12.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                case 10:
                    Last_used.B10.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;

                default:
                    Last_used.GEN.Filter_Low_Index = Filter_control.Filter_Low_Index;
                    break;
            }
            oCode.SendCommand(txsocket, txtarget, Filter_control.CMD_SET_BW_LOCUT, Filter_control.Filter_Low_Index);
        }

        private void CW_Filter_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            {
                int index;
                if (oCode.isLoading) return;
                index = CW_Filter_listBox1.SelectedIndex;
                switch (oCode.current_band)
                {
                    case 160:
                        Last_used.B160.Filter_CW_index = (short)index;
                        break;

                    case 80:
                        Last_used.B80.Filter_CW_index = (short)index;
                        break;

                    case 60:
                        Last_used.B60.Filter_CW_index = (short)index;
                        break;

                    case 40:
                        Last_used.B40.Filter_CW_index = (short)index;
                        break;

                    case 30:
                        Last_used.B30.Filter_CW_index = (short)index;
                        break;

                    case 20:
                        Last_used.B20.Filter_CW_index = (short)index;
                        break;

                    case 17:
                        Last_used.B17.Filter_CW_index = (short)index;
                        break;

                    case 15:
                        Last_used.B15.Filter_CW_index = (short)index;
                        break;

                    case 12:
                        Last_used.B12.Filter_CW_index = (short)index;
                        break;

                    case 10:
                        Last_used.B10.Filter_CW_index = (short)index;
                        break;

                    default:
                        Last_used.GEN.Filter_CW_index = (short)index;
                        break;
                }
                Panadapter_Controls.Filter_Index = index;
                Filter_control.CW_Bw = index;
#if FTDI
                Tuning_Knob_Controls.CW_BW_Function.BW_index = index;
#endif
                Panadapter_Controls.CW_Filter_Size_Name = CW_Filter_listBox1.GetItemText(CW_Filter_listBox1.SelectedItem);
                oCode.SendCommand(txsocket, txtarget, Filter_control.CMD_SET_CW_BW, (short)Filter_control.CW_Bw);
                oCode.SendCommand(txsocket, txtarget, Filter_control.CMD_SET_CW_PITCH, (short)Filter_control.CW_Pitch_Index);
            }
        }

        private void Filter_listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            {
                short filter_index_local;

                if (oCode.isLoading == true) return;
                Filter_control.Filter_High_Index = (short)Filter_listBox1.SelectedIndex;
                filter_index_local = (short)Filter_listBox1.SelectedIndex;

                MonitorTextBoxText(" Filter_listBox1 Called -> Filter Index: " + filter_index_local);

                Filter_control.Previous_Filter_High_Index = filter_index_local;
                MonitorTextBoxText(" Filter_listBox1 -> filter_index_local: " + Convert.ToString(filter_index_local));
                Panadapter_Controls.Filter_Index = filter_index_local;
                Window_controls.Waterfall_Controls.Markers.band_marker_high = filter_index_local;
                Panadapter_Controls.Filter_Size_Name = Filter_listBox1.GetItemText(Filter_listBox1.SelectedItem);

                switch (oCode.current_band)
                {
                    case 160:
                        Last_used.B160.Filter_High_Index = filter_index_local;
                        break;

                    case 80:
                        Last_used.B80.Filter_High_Index = filter_index_local;
                        break;

                    case 60:
                        Last_used.B60.Filter_High_Index = filter_index_local;
                        break;

                    case 40:
                        Last_used.B40.Filter_High_Index = filter_index_local;
                        break;

                    case 30:
                        Last_used.B30.Filter_High_Index = filter_index_local;
                        break;

                    case 20:
                        Last_used.B20.Filter_High_Index = filter_index_local;
                        break;

                    case 17:
                        Last_used.B17.Filter_High_Index = filter_index_local;
                        break;

                    case 15:
                        Last_used.B15.Filter_High_Index = filter_index_local;
                        break;

                    case 12:
                        Last_used.B12.Filter_High_Index = filter_index_local;
                        break;

                    case 10:
                        Last_used.B10.Filter_High_Index = filter_index_local;
                        break;

                    default:
                        Last_used.GEN.Filter_High_Index = filter_index_local;
                        break;
                }
#if FTDI
                Tuning_Knob_Controls.CW_HI_CUT_Function.HI_CUT_index = filter_index_local;
#endif
                oCode.SendCommand(txsocket, txtarget, Filter_control.CMD_SET_BW_HICUT, filter_index_local);
                MonitorTextBoxText(" Filter_listBox1 -> Finished -> Filter Index: " + filter_index_local);
            }
        }

        private int Swap_tuning_step(int step)
        {
            int[] swapped = { 5, 4, 3, 2, 1, 0 };
            return swapped[step];
        }

        private void step_list_box_leave(object send,EventArgs e)
        {
            this.mainPage.Focus();
            MonitorTextBoxText(" step_list_box_leave");
        }

        private void mainlistBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            {
                if (oCode.isLoading == true) return;
                oCode.Freq_Tune_Index = (short)mainlistBox1.SelectedIndex;
                oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_STEP_VALUE, oCode.Freq_Tune_Index);
                oCode.FreqDigit = (short)Swap_tuning_step(oCode.Freq_Tune_Index);
                Set_Freq_Digit_Pointer();
            }
        }

        private void Default_Low_Cut_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            {
                Filter_control.Default_Filter_Low_Index = (short)Default_Low_Cut_listBox1.SelectedIndex;
                MonitorTextBoxText(" Default_Low_Cut_listBox1 Called -> Filter Index: " + Default_Low_Cut_listBox1.SelectedIndex +
                    ", Filter_control.Default_Filter_Low_Index: " + Filter_control.Default_Filter_Low_Index);
                Settings.Default.Lo_Cut_Default = Default_Low_Cut_listBox1.SelectedIndex;
                oCode.SendCommand(txsocket, txtarget, Filter_control.CMD_SET_BW_LOCUT_DEFAULT,
                  (short)Default_Low_Cut_listBox1.SelectedIndex);
            }
        }

        private void Default_CW_Filter_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            {
                Filter_control.Default_Filter_CW_Index = (short)Default_CW_Filter_listBox1.SelectedIndex;
                MonitorTextBoxText(" Default_CW_Filter_listBox1 Called -> Filter Index: " + Default_CW_Filter_listBox1.SelectedIndex +
                    ", Filter_control.Default_CW_Filter_listBox1: " + Filter_control.Default_Filter_CW_Index);
                Settings.Default.CW_Default = Default_CW_Filter_listBox1.SelectedIndex;
                oCode.SendCommand(txsocket, txtarget, Filter_control.CMD_SET_CW_BW_DEFAULT,
                   (short)Default_CW_Filter_listBox1.SelectedIndex);
            }
        }

        private void Default_High_Cut_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            {
                Filter_control.Default_Filter_High_Index = (short)Default_High_Cut_listBox1.SelectedIndex;
                MonitorTextBoxText(" Filter_listBox1 Called -> Filter Index: " + Default_High_Cut_listBox1.SelectedIndex +
                    ", Filter_control.Filter_TX_High_Index: " + Filter_control.Default_Filter_High_Index);
                Settings.Default.Hi_Cut_Default = Default_High_Cut_listBox1.SelectedIndex;
                oCode.SendCommand(txsocket, txtarget, Filter_control.CMD_SET_BW_HICUT_DEFAULT,
                    (short)Default_High_Cut_listBox1.SelectedIndex);
            }
        }

        private void Relay_Board_checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
#if FTDI
            if (!Master_Controls.code_triggered)
            {
                if (!Tuning_Knob_Controls.On)
                {
                    if (!Relay_Board_Controls.On)
                    {
                        Open_Relay_Board();
                        if (Relay_Board_Controls.Open)
                        {
                            Relay_Board_Controls.On = true;
                        }
                        else
                        {
                            Relay_Board_checkBox2.Checked = false;
                            MessageBox.Show("Relay Board OPEN FAILED", "MSCC",
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                    }
                    else
                    {
                        if (Relay_Board_Controls.Open)
                        {
                            Relay_Board_Controls.Device.Close();
                            MonitorTextBoxText(" Relay Board CLOSED");
                        }
                        Relay_Board_Controls.On = false;
                        Relay_Board_Controls.Write_failure = false;
                        Relay_Board_Controls.Open = false;
                    }
                }
                else
                {
                    Relay_Board_checkBox2.Checked = false;
                    MessageBox.Show("Tuning Knob in Use. Turn off Tuning Knob", "MSCC",
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
#endif
        }

        private void AGC_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value = 0;
            if (oCode.isLoading) return;
            value = AGC_hScrollBar1.Value;
            if (value == AGC_ALC_Notch_Controls.AGC_Release) return;
            AGC_ALC_Notch_Controls.AGC_Release = value;
            AGC_label57.Text = Convert.ToString(AGC_ALC_Notch_Controls.AGC_Release) + " mS";
            oCode.SendCommand32(txsocket, txtarget, AGC_ALC_Notch_Controls.CMD_SET_AGC_FAST_LEVEL,
                AGC_ALC_Notch_Controls.AGC_Release);
        }

        private void AGC_label57_Click(object sender, EventArgs e)
        {
        }

        /*private void Comm_Port_Reset_button3_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            DialogResult ret = MessageBox.Show("Are you sure you want to reset the Com Port Devices list?", "MSCC",
                                                                       MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            if (ret == DialogResult.Yes)
            {
                MessageBox.Show("The Com Port Devices List will be reset on next startup of MSCC\n" +
                "MSCC will now STOP", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Comm_Port_Controls.CTS_Selected = 0;
                Comm_Port_Controls.DCD_Selected = 0;
                Comm_Port_Controls.Comm_Port_Open = false;
                Comm_Port_Controls.Button_Toggle = false;
                Comm_Port_Controls.Box_indexes.Comm_Name_Index = 100;
                oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_START, 0);
                Thread.Sleep(500);
                Application.Exit();
            }
        }

        private void label33_Click_1(object sender, EventArgs e)
        {
        }

        private sbyte read_rotary()
        {
            sbyte status = 0;
            Tuning_Knob_Controls.prevNextCode <<= 2;
            if (Tuning_Knob_Controls.encoder_A)
            {
                Tuning_Knob_Controls.prevNextCode |= 0x02;
            }
            if (Tuning_Knob_Controls.encoder_B)
            {
                Tuning_Knob_Controls.prevNextCode |= 0x01;
            }
            Tuning_Knob_Controls.prevNextCode &= 0x0f;
            if (Tuning_Knob_Controls.rec_enc_table[Tuning_Knob_Controls.prevNextCode] == 1)
            {
                Tuning_Knob_Controls.store <<= 4;
                Tuning_Knob_Controls.store |= Tuning_Knob_Controls.prevNextCode;
                if ((Tuning_Knob_Controls.store & 0xff) == 0x2b) status = -1;
                if ((Tuning_Knob_Controls.store & 0x17) == 0x17) status = 1;
            }
            return status;
        }

        private void Relay_States_to_bool(int switch_A, int switch_B)
        {
            switch (switch_A)
            {
                case 0:
                    Tuning_Knob_Controls.encoder_A = false;
                    break;

                default:
                    Tuning_Knob_Controls.encoder_A = true;
                    break;
            }
            switch (switch_B)
            {
                case 0:
                    Tuning_Knob_Controls.encoder_B = false;
                    break;

                default:
                    Tuning_Knob_Controls.encoder_B = true;
                    break;
            }
        }
        */
        /*private bool Check_Active_State(int function_number)
        {
            //Only one function may control the tuning knob
            bool status = true;

            if (function_number == Tuning_Knob_Controls.Button_Function_List.FREQ_VOLUME)
            {
                if (Tuning_Knob_Controls.Active_Functions.RIT_Offset_Active)
                {
                    status = false;
                }
            }
            return status;
        }

        private bool Button_Function(byte function, byte button)
        {
            bool status = true;
            bool function_status = false;

            switch (function)
            {
                case Tuning_Knob_Controls.Button_Function_List.PTT_REAR_CONNECTOR:

                    status = MFC_PTT_Function_Rear(button);
                    break;

                case Tuning_Knob_Controls.Button_Function_List.STEP:
                    status = MFC_Step_Function(button);
                    break;

                case Tuning_Knob_Controls.Button_Function_List.PTT:
                    status = MFC_PTT_Function(button);
                    break;

                case Tuning_Knob_Controls.Button_Function_List.TUNE:
                    status = MFC_TUNE_Function(button);
                    break;

                case Tuning_Knob_Controls.Button_Function_List.MODE:
                    status = MFC_MODE_Function(button);
                    break;

                case Tuning_Knob_Controls.Button_Function_List.RIT:
                    status = MFC_RIT_Function(button);
                    break;

                case Tuning_Knob_Controls.Button_Function_List.NONE:
                    status = true;
                    break;

                case Tuning_Knob_Controls.Button_Function_List.BAND:
                    status = MFC_Band_Function(button);
                    break;

                case Tuning_Knob_Controls.Button_Function_List.FREQ_VOLUME:
                    function_status = Check_Active_State(Tuning_Knob_Controls.Button_Function_List.FREQ_VOLUME);
                    if (function_status)
                    {
                        status = MFC_FREQ_VOLUME_Function(button);
                    }
                    break;

                case Tuning_Knob_Controls.Button_Function_List.CW_BW:
                    status = MFC_CW_BW_Function(button);
                    break;

                case Tuning_Knob_Controls.Button_Function_List.HI_CUT:
                    status = MFC_HIGH_CUT_BW_Function(button);
                    break;

                case Tuning_Knob_Controls.Button_Function_List.RIT_OFFSET:
                    status = MFC_RIT_OFFSET_Function(button);
                    break;

                case Tuning_Knob_Controls.Button_Function_List.FAVORITES:
                    status = MFC_FAVS_Function(button);
                    break;
            }
            return status;
        }

        public void Button_Set_Band(byte band)
        {
            MonitorTextBoxText(" Button_Set_Band  VALUE : " + band);
            switch (band)
            {
                case 0:
                    main160radioButton10.Checked = true;
                    main160radioButton10_CheckedChanged(null, null);
                    break;

                case 1:
                    main80radioButton9.Checked = true;
                    main80radioButton9_CheckedChanged(null, null);
                    break;

                case 2:
                    main60radioButton8.Checked = true;
                    main60radioButton8_CheckedChanged(null, null);
                    break;

                case 3:
                    main40radioButton7.Checked = true;
                    main40radioButton7_CheckedChanged(null, null);
                    break;

                case 4:
                    main30radioButton6.Checked = true;
                    main30radioButton6_CheckedChanged(null, null);
                    break;

                case 5:
                    main20radioButton5.Checked = true;
                    main20radioButton5_CheckedChanged(null, null);
                    break;

                case 6:
                    main17radioButton4.Checked = true;
                    main17radioButton4_CheckedChanged(null, null);
                    break;

                case 7:
                    main15radiobutton.Checked = true;
                    main15radiobutton_CheckedChanged(null, null);
                    break;

                case 8:
                    main12radioButton2.Checked = true;
                    main12radioButton2_CheckedChanged(null, null);
                    break;

                case 9:
                    main10radioButton1.Checked = true;
                    main10radioButton1_CheckedChanged(null, null);
                    break;

                default:
                    genradioButton.Checked = true;
                    break;
            }
        }

        public void Set_MFC_Increment()
        {
            Tuning_Knob_Controls.Step_Function.Step_increment = oCode.Freq_Tune_Index;
            switch (Tuning_Knob_Controls.Step_Function.Step_increment)
            {
                case 0:
                    Tuning_Knob_Controls.Step_Function.Multiplier = 100000;
                    break;

                case 1:
                    Tuning_Knob_Controls.Step_Function.Multiplier = 10000;
                    break;

                case 2:
                    Tuning_Knob_Controls.Step_Function.Multiplier = 1000;
                    break;

                case 3:
                    Tuning_Knob_Controls.Step_Function.Multiplier = 100;
                    break;

                case 4:
                    Tuning_Knob_Controls.Step_Function.Multiplier = 10;
                    break;

                case 5:
                    Tuning_Knob_Controls.Step_Function.Multiplier = 1;
                    break;
            }
        }

        public byte Set_Star(int MFC_Switch)
        {
            byte star = 0;
            //MonitorTextBoxText(" Set_Star --> Called. Switch: 0x" + MFC_Switch.ToString("X"));
            switch (MFC_Switch)
            {
                case Tuning_Knob_Controls.Button_left_switch:
                    star = Tuning_Knob_Controls.Star_Position.Left;
                    break;

                case Tuning_Knob_Controls.Button_right_switch:
                    star = Tuning_Knob_Controls.Star_Position.Right;
                    break;

                case Tuning_Knob_Controls.Button_middle_switch:
                    star = Tuning_Knob_Controls.Star_Position.Middle;
                    break;
            }
            //MonitorTextBoxText(" Set_Star --> Called. Star: 0x" + star.ToString("X"));
            return star;
        }*/

        public void Set_Favs()
        {
            MonitorTextBoxText(" Set_Favs - > Called");
            switch (oCode.current_band)
            {
                case 10:
                    Favorites_Controls.Band_Count_Limit = Favorites_Controls.B10_Count;
                    break;

                case 12:
                    Favorites_Controls.Band_Count_Limit = Favorites_Controls.B12_Count;
                    break;

                case 15:
                    Favorites_Controls.Band_Count_Limit = Favorites_Controls.B15_Count;
                    break;

                case 17:
                    Favorites_Controls.Band_Count_Limit = Favorites_Controls.B17_Count;
                    break;

                case 20:
                    Favorites_Controls.Band_Count_Limit = Favorites_Controls.B20_Count;
                    break;

                case 30:
                    Favorites_Controls.Band_Count_Limit = Favorites_Controls.B30_Count;
                    break;

                case 40:
                    Favorites_Controls.Band_Count_Limit = Favorites_Controls.B40_Count;
                    break;

                case 60:
                    Favorites_Controls.Band_Count_Limit = Favorites_Controls.B60_Count;
                    break;

                case 80:
                    Favorites_Controls.Band_Count_Limit = Favorites_Controls.B80_Count;
                    break;

                case 160:
                    Favorites_Controls.Band_Count_Limit = Favorites_Controls.B160_Count;
                    break;
            }

            if (Favorites_Controls.Band_Count_Limit != 0)
            {
                MonitorTextBoxText(" Set_Favs - > Favorites_Controls.Band_Count_Limit: " +
                                                                Convert.ToString(Favorites_Controls.Band_Count_Limit));
                Favorites_Controls.Index_Count++;
                if (Favorites_Controls.Index_Count >= Favorites_Controls.Band_Count_Limit)
                {
                    Favorites_Controls.Index_Count = 0;
                }
                MonitorTextBoxText(" Set_Favs - > oCode.current_band: " +
                                                                Convert.ToString(oCode.current_band));
                switch (oCode.current_band)
                {
                    case 10:
                        B10_Favs_listView1.Items[Favorites_Controls.Index_Count].Selected = true;
                        B10_Favs_listView1.Select();
                        break;

                    case 12:
                        B12_Favs_listView1.Items[Favorites_Controls.Index_Count].Selected = true;
                        B12_Favs_listView1.Select();
                        break;

                    case 15:
                        B15_Favs_listView1.Items[Favorites_Controls.Index_Count].Selected = true;
                        B15_Favs_listView1.Select();
                        break;

                    case 17:
                        B17_Favs_listView1.Items[Favorites_Controls.Index_Count].Selected = true;
                        B17_Favs_listView1.Select();
                        break;

                    case 20:
                        B20_Favs_listView1.Items[Favorites_Controls.Index_Count].Selected = true;
                        B20_Favs_listView1.Select();
                        break;

                    case 30:
                        B30_Favs_listView1.Items[Favorites_Controls.Index_Count].Selected = true;
                        B30_Favs_listView1.Select();
                        break;

                    case 40:
                        B40_Favs_listView1.Items[Favorites_Controls.Index_Count].Selected = true;
                        B40_Favs_listView1.Select();
                        break;

                    case 60:
                        B60_Favs_listView1.Items[Favorites_Controls.Index_Count].Selected = true;
                        B60_Favs_listView1.Select();
                        break;

                    case 80:
                        B80_Favs_listView1.Items[Favorites_Controls.Index_Count].Selected = true;
                        B80_Favs_listView1.Select();
                        break;

                    case 160:
                        B160_Favs_listView1.Items[Favorites_Controls.Index_Count].Selected = true;
                        B60_Favs_listView1.Select();
                        break;
                }
            }
        }
        /*
        public bool MFC_Band_Function(byte controller_switch)
        {
            bool status = true;
            byte relays_States = 0;
            byte button = 0;
            byte current_band = 0;
            byte band = 0;

            if (!Tuning_Knob_Controls.Freq_Function.Active && !Tuning_Knob_Controls.Volume_Function.Volume_active)
            {
                Tuning_Knob_Controls.Status = Tuning_Knob_Controls.Device.GetPinStates(ref relays_States);
                if (Tuning_Knob_Controls.Status == FTDI.FT_STATUS.FT_OK)
                {
                    current_band = (byte)oCode.current_band;
                    switch (current_band)
                    {
                        case 160:
                            band = 0;
                            break;

                        case 80:
                            band = 1;
                            break;

                        case 60:
                            band = 2;
                            break;

                        case 40:
                            band = 3;
                            break;

                        case 30:
                            band = 4;
                            break;

                        case 20:
                            band = 5;
                            break;

                        case 17:
                            band = 6;
                            break;

                        case 15:
                            band = 7;
                            break;

                        case 12:
                            band = 8;
                            break;

                        case 10:
                            band = 9;
                            break;

                        default:
                            band = 10;
                            break;
                    }
                    button = (byte)(relays_States & controller_switch);
                    if (button == 0)
                    {
                        //MonitorTextBoxText(" MFC_Band_Function Called -> button: 0");
                        if (Tuning_Knob_Controls.Band_Function.Switch_Toggle == false)
                        {
                            //MonitorTextBoxText(" MFC_Band_Function -> switch_toggle FALSE");
                            Tuning_Knob_Controls.Band_Function.Switch_Toggle = true;
                            //MonitorTextBoxText(" MFC_Band_Function -> switch_toggle TRUE");
                            band++;
                            if (band > 10)
                            {
                                band = 0;
                            }
                            //MonitorTextBoxText(" MFC_Band_Function Called -> Calling Button_Set_Band. Band: " +
                            //Convert.ToString(band));
                            Button_Set_Band(band);
                            status = false;
                        }
                    }
                    else
                    {
                        Tuning_Knob_Controls.Band_Function.Switch_Toggle = false;
                        //MonitorTextBoxText(" MFC_Band_Function -> switch_toggle FALSE -> button: 1");
                        status = true;
                    }
                }
                else
                {
                    Tuning_Knob_Controls.Read_failure = true;
                    status = false;
                }
            }
            return status;
        }

        public bool MFC_PTT_Function(byte controller_switch)
        {
            bool status = true;
            byte relays_States = 0;
            byte button = 0;

            if (!Tuning_Knob_Controls.Freq_Function.Active && !Tuning_Knob_Controls.Volume_Function.Volume_active)
            {
                Tuning_Knob_Controls.Status = Tuning_Knob_Controls.Device.GetPinStates(ref relays_States);
                if (Tuning_Knob_Controls.Status != FTDI.FT_STATUS.FT_OK)
                {
                    Tuning_Knob_Controls.Read_failure = true;
                    status = false;
                }
                else
                {
                    button = (byte)(relays_States & controller_switch);
                    if (button == 0)
                    {
                        MonitorTextBoxText(" MFC_PTT_Function Called");
                        if (Tuning_Knob_Controls.PTT_Function.Switch_Toggle == false)
                        {
                            Tuning_Knob_Controls.PTT_Function.Switch_Toggle = true;
                            if (Tuning_Knob_Controls.PTT_Function.Action_Toggle)
                            {
                                button1_Click_1(null, null);
                                Tuning_Knob_Controls.PTT_Function.Action_Toggle = false;
                            }
                            else
                            {
                                button1_Click_1(null, null);
                                Tuning_Knob_Controls.PTT_Function.Action_Toggle = true;
                            }
                        }
                        status = false;
                    }
                    else
                    {
                        Tuning_Knob_Controls.PTT_Function.Switch_Toggle = false;
                        status = true;
                    }
                }
            }
            return status;
        }

        public bool MFC_PTT_Function_Rear(byte controller_switch)
        {
            bool status = true;
            byte relays_States = 0;
            byte button = 0;

            if (!Tuning_Knob_Controls.Freq_Function.Active && !Tuning_Knob_Controls.Volume_Function.Volume_active)
            {
                Tuning_Knob_Controls.Status = Tuning_Knob_Controls.Device.GetPinStates(ref relays_States);
                if (Tuning_Knob_Controls.Status != FTDI.FT_STATUS.FT_OK)
                {
                    Tuning_Knob_Controls.Read_failure = true;
                    status = false;
                }
                else
                {
                    button = (byte)(relays_States & controller_switch);
                    if (button == 0)
                    {
                        MonitorTextBoxText(" MFC_PTT_Function_Rear Called-> controller_switch: " +
                            Convert.ToString(controller_switch) + ", relay_States: " + Convert.ToString(relays_States));
                        if (Tuning_Knob_Controls.PTT_Function_Rear.Switch_Toggle == false)
                        {
                            Tuning_Knob_Controls.PTT_Function_Rear.Switch_Toggle = true;
                            if (Tuning_Knob_Controls.PTT_Function_Rear.Action_Toggle)
                            {
                                button1_Click_1(null, null);
                                Tuning_Knob_Controls.PTT_Function_Rear.Action_Toggle = false;
                            }
                            else
                            {
                                button1_Click_1(null, null);
                                Tuning_Knob_Controls.PTT_Function_Rear.Action_Toggle = true;
                            }
                        }
                        status = false;
                    }
                    else
                    {
                        Tuning_Knob_Controls.PTT_Function_Rear.Switch_Toggle = false;
                        status = true;
                    }
                }
            }
            return status;
        }

        public bool MFC_RIT_Function(byte controller_switch)
        {
            bool status = true;
            byte relays_States = 0;
            byte button = 0;
            byte star = 0;

            if (!Tuning_Knob_Controls.Freq_Function.Active && !Tuning_Knob_Controls.Volume_Function.Volume_active)
            {
                Tuning_Knob_Controls.Status = Tuning_Knob_Controls.Device.GetPinStates(ref relays_States);
                if (Tuning_Knob_Controls.Status != FTDI.FT_STATUS.FT_OK)
                {
                    Tuning_Knob_Controls.Read_failure = true;
                    status = false;
                }
                else
                {
                    star = Set_Star(controller_switch);
                    button = (byte)(relays_States & controller_switch);
                    if (button == 0)
                    {
                        MonitorTextBoxText(" MFC_RIT_Function Called.");
                        if (Tuning_Knob_Controls.RIT_Function.Switch_Toggle == false)
                        {
                            Tuning_Knob_Controls.RIT_Function.Switch_Toggle = true;
                            if (Tuning_Knob_Controls.RIT_Function.Action_Toggle)
                            {
                                ritbutton1_Click(null, null);
                                Tuning_Knob_Controls.RIT_Function.Action_Toggle = false;
                                star = (byte)(star | 0x00);
                                MonitorTextBoxText(" MFC_RIT_Function Called - 0. Star: 0x" + star.ToString("X"));
                                oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_STAR, star);
                            }
                            else
                            {
                                ritbutton1_Click(null, null);
                                Tuning_Knob_Controls.RIT_Function.Action_Toggle = true;
                                star = (byte)(star | 0x80);
                                MonitorTextBoxText(" MFC_RIT_Function Called - 1. Star: 0x" + star.ToString("X"));
                                oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_STAR, star);
                            }
                        }
                        status = false;
                    }
                    else
                    {
                        Tuning_Knob_Controls.RIT_Function.Switch_Toggle = false;
                        status = true;
                    }
                }
            }
            return status;
        }

        public bool MFC_MODE_Function(byte controller_switch)
        {
            bool status = true;
            byte relays_States = 0;
            byte button = 0;

            if (!Tuning_Knob_Controls.Freq_Function.Active && !Tuning_Knob_Controls.Volume_Function.Volume_active)
            {
                Tuning_Knob_Controls.Status = Tuning_Knob_Controls.Device.GetPinStates(ref relays_States);
                if (Tuning_Knob_Controls.Status != FTDI.FT_STATUS.FT_OK)
                {
                    Tuning_Knob_Controls.Read_failure = true;
                    status = false;
                }
                else
                {
                    button = (byte)(relays_States & controller_switch);
                    if (button == 0)
                    {
                        MonitorTextBoxText(" MFC_MODE_Function Called.");
                        if (Tuning_Knob_Controls.Mode_Function.Switch_Toggle == false)
                        {
                            Tuning_Knob_Controls.Mode_Function.Switch_Toggle = true;
                            mainmodebutton2_Click(null, null);
                        }
                        status = false;
                    }
                    else
                    {
                        Tuning_Knob_Controls.Mode_Function.Switch_Toggle = false;
                        status = true;
                    }
                }
            }
            return status;
        }

        public bool MFC_TUNE_Function(byte controller_switch)
        {
            bool status = true;
            byte relays_States = 0;
            byte button = 0;

            if (!Tuning_Knob_Controls.Freq_Function.Active && !Tuning_Knob_Controls.Volume_Function.Volume_active)
            {
                Tuning_Knob_Controls.Status = Tuning_Knob_Controls.Device.GetPinStates(ref relays_States);
                if (Tuning_Knob_Controls.Status != FTDI.FT_STATUS.FT_OK)
                {
                    Tuning_Knob_Controls.Read_failure = true;
                    status = false;
                }
                else
                {
                    button = (byte)(relays_States & controller_switch);
                    if (button == 0)
                    {
                        MonitorTextBoxText(" MFC_TUNE_Function Called.");
                        if (Tuning_Knob_Controls.Tune_Function.Switch_Toggle == false)
                        {
                            Tuning_Knob_Controls.Tune_Function.Switch_Toggle = true;
                            if (Tuning_Knob_Controls.Tune_Function.Action_Toggle)
                            {
                                buttTune_Click(null, null);
                                Tuning_Knob_Controls.Tune_Function.Action_Toggle = false;
                            }
                            else
                            {
                                buttTune_Click(null, null);
                                Tuning_Knob_Controls.Tune_Function.Action_Toggle = true;
                            }
                        }
                        status = false;
                    }
                    else
                    {
                        Tuning_Knob_Controls.Tune_Function.Switch_Toggle = false;
                        status = true;
                    }
                }
            }
            return status;
        }

        public bool MFC_Step_Function(byte controller_switch)
        {
            bool status = false;
            byte relays_States = 0;
            byte button = 0;

            if (!Tuning_Knob_Controls.Freq_Function.Active && !Tuning_Knob_Controls.Volume_Function.Volume_active)
            {
                Set_MFC_Increment();
                Tuning_Knob_Controls.Status = Tuning_Knob_Controls.Device.GetPinStates(ref relays_States);
                if (Tuning_Knob_Controls.Status != FTDI.FT_STATUS.FT_OK)
                {
                    Tuning_Knob_Controls.Read_failure = true;
                    status = false;
                }
                else
                {
                    button = (byte)(relays_States & controller_switch);
                    if (button == 0)
                    {
                        MonitorTextBoxText(" MFC_Step_Function Called.");
                        if (Tuning_Knob_Controls.Step_Function.Switch_Toggle == false)
                        {
                            Tuning_Knob_Controls.Step_Function.Switch_Toggle = true;
                            MonitorTextBoxText(" MFC_Step_Function Called -> Calling: mainlistBox1_SelectedIndexChanged_1");
                            Tuning_Knob_Controls.Step_Function.Step_increment--;
                            if (Tuning_Knob_Controls.Step_Function.Step_increment < 0)
                            {
                                Tuning_Knob_Controls.Step_Function.Step_increment = 5;
                            }
                            mainlistBox1.SelectedIndex = Tuning_Knob_Controls.Step_Function.Step_increment;
                            Set_MFC_Increment();
                            mainlistBox1_SelectedIndexChanged_1(null, null);
                            oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_STEP_VALUE,
                                                        (short)Tuning_Knob_Controls.Step_Function.Step_increment);
                        }
                        status = false;
                    }
                    else
                    {
                        Tuning_Knob_Controls.Step_Function.Switch_Toggle = false;
                        status = true;
                    }
                }
            }
            return status;
        }

        public bool MFC_CW_BW_Function(byte controller_switch)
        {
            bool status = true;
            byte relays_States = 0;
            byte button = 0;

            if (!Tuning_Knob_Controls.Freq_Function.Active && !Tuning_Knob_Controls.Volume_Function.Volume_active)
            {
                Tuning_Knob_Controls.Status = Tuning_Knob_Controls.Device.GetPinStates(ref relays_States);
                if (Tuning_Knob_Controls.Status != FTDI.FT_STATUS.FT_OK)
                {
                    Tuning_Knob_Controls.Read_failure = true;
                    status = false;
                }
                else
                {
                    button = (byte)(relays_States & controller_switch);
                    if (button == 0)
                    {
                        MonitorTextBoxText(" MFC_CW_BW_Function Called.");
                        if (Tuning_Knob_Controls.CW_BW_Function.Switch_Toggle == false)
                        {
                            Tuning_Knob_Controls.CW_BW_Function.Switch_Toggle = true;
                            Tuning_Knob_Controls.CW_BW_Function.BW_index--;
                            if (Tuning_Knob_Controls.CW_BW_Function.BW_index < 0)
                            {
                                Tuning_Knob_Controls.CW_BW_Function.BW_index = 2;
                            }
                            CW_Filter_listBox1.SelectedIndex = Tuning_Knob_Controls.CW_BW_Function.BW_index;
                            CW_Filter_listBox1_SelectedIndexChanged(null, null);
                        }
                        status = false;
                    }
                    else
                    {
                        Tuning_Knob_Controls.CW_BW_Function.Switch_Toggle = false;
                        status = true;
                    }
                }
            }
            return status;
        }

        public bool MFC_HIGH_CUT_BW_Function(byte controller_switch)
        {
            bool status = true;
            byte relays_States = 0;
            byte button = 0;

            if (!Tuning_Knob_Controls.Freq_Function.Active && !Tuning_Knob_Controls.Volume_Function.Volume_active)
            {
                Tuning_Knob_Controls.Status = Tuning_Knob_Controls.Device.GetPinStates(ref relays_States);
                if (Tuning_Knob_Controls.Status != FTDI.FT_STATUS.FT_OK)
                {
                    Tuning_Knob_Controls.Read_failure = true;
                    status = false;
                }
                else
                {
                    button = (byte)(relays_States & controller_switch);
                    if (button == 0)
                    {
                        MonitorTextBoxText(" MFC_HIGH_CUT_BW_Function Called.");
                        if (Tuning_Knob_Controls.CW_HI_CUT_Function.Switch_Toggle == false)
                        {
                            Tuning_Knob_Controls.CW_HI_CUT_Function.Switch_Toggle = true;
                            Tuning_Knob_Controls.CW_HI_CUT_Function.HI_CUT_index--;
                            if (Tuning_Knob_Controls.CW_HI_CUT_Function.HI_CUT_index < 0)
                            {
                                Tuning_Knob_Controls.CW_HI_CUT_Function.HI_CUT_index = 4;
                            }
                            Filter_listBox1.SelectedIndex = Tuning_Knob_Controls.CW_HI_CUT_Function.HI_CUT_index;
                            Filter_listBox1_SelectedIndexChanged_1(null, null);
                        }
                        status = false;
                    }
                    else
                    {
                        Tuning_Knob_Controls.CW_HI_CUT_Function.Switch_Toggle = false;
                        status = true;
                    }
                }
            }
            return status;
        }

        public bool MFC_FREQ_VOLUME_Function(byte controller_switch)
        {
            bool status = true;
            byte relays_States = 0;
            byte button = 0;
            byte star = 0;
            byte run = 1;

            if (Tuning_Knob_Controls.Freq_Function.First_Pass)
            {
               
                Tuning_Knob_Controls.Freq_Function.First_Pass = false;
            }
            //if (!Tuning_Knob_Controls.Knob_timer_active && !Tuning_Knob_Controls.Volume_Function.MFC_volume_active)
            if (run == 1)
            {
                Tuning_Knob_Controls.Status = Tuning_Knob_Controls.Device.GetPinStates(ref relays_States);
                if (Tuning_Knob_Controls.Status != FTDI.FT_STATUS.FT_OK)
                {
                    Tuning_Knob_Controls.Read_failure = true;
                    status = false;
                }
                else
                {
                    star = Set_Star(controller_switch);
                    button = (byte)(relays_States & controller_switch);
                    if (button == 0)
                    {
                        MonitorTextBoxText(" MFC_FREQ_VOLUME_Function Called.");
                        if (Tuning_Knob_Controls.Freq_Function.Switch_Toggle == false)
                        {
                            Tuning_Knob_Controls.Freq_Function.Switch_Toggle = true;
                            if (Tuning_Knob_Controls.Freq_Function.Action_Toggle)
                            {
                               
                                Tuning_Knob_Controls.Active_Functions.Volume_Active = false;
                                Tuning_Knob_Controls.Active_Functions.Freq_Active = true;
                                Tuning_Knob_Controls.Freq_Function.Action_Toggle = false;
                                star = (byte)(star | 0x00);
                                MonitorTextBoxText(" MFC_FREQ_VOLUME_Function Called - 0. Star: 0x" + star.ToString("X"));
                                oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_STAR, star);
                            }
                            else
                            {
                              
                                
                                Tuning_Knob_Controls.Active_Functions.Volume_Active = true;
                                Tuning_Knob_Controls.Active_Functions.Freq_Active = false;
                                Tuning_Knob_Controls.Freq_Function.Action_Toggle = true;
                                star = (byte)(star | 0x80);
                                MonitorTextBoxText(" MFC_FREQ_VOLUME_Function Called - 1. Star: 0x" + star.ToString("X"));
                                oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_STAR, star);
                            }
                        }
                        status = false;
                    }
                    else
                    {
                        Tuning_Knob_Controls.Freq_Function.Switch_Toggle = false;
                        status = true;
                    }
                }
            }
            return status;
        }

        public bool MFC_VOLUME_Function(byte controller_switch)
        {
            bool status = true;
            byte relays_States = 0;
            byte button = 0;

            if (!Tuning_Knob_Controls.Freq_Function.Active && !Tuning_Knob_Controls.Volume_Function.Volume_active)
            {
                Tuning_Knob_Controls.Status = Tuning_Knob_Controls.Device.GetPinStates(ref relays_States);
                if (Tuning_Knob_Controls.Status != FTDI.FT_STATUS.FT_OK)
                {
                    Tuning_Knob_Controls.Read_failure = true;
                    status = false;
                }
                else
                {
                    button = (byte)(relays_States & controller_switch);
                    if (button == 0)
                    {
                        MonitorTextBoxText(" MFC_VOLUME_Function Called.");
                        if (Tuning_Knob_Controls.Volume_Function.Switch_Toggle == false)
                        {
                            Tuning_Knob_Controls.Volume_Function.Switch_Toggle = true;
                            if (Tuning_Knob_Controls.Volume_Function.Action_Toggle)
                            {
                                MonitorTextBoxText(" MFC_VOLUME_Function -> Freq tuning on");
                               
                                Tuning_Knob_Controls.Volume_Function.Action_Toggle = false;
                            }
                            else
                            {
                                MonitorTextBoxText(" MFC_VOLUME_Function -> Volume control on");
                               
                                Tuning_Knob_Controls.Volume_Function.Action_Toggle = true;
                            }
                        }
                        status = false;
                    }
                    else
                    {
                        Tuning_Knob_Controls.Volume_Function.Switch_Toggle = false;
                        status = true;
                    }
                }
            }
            return status;
        }

        public bool MFC_RIT_OFFSET_Function(byte controller_switch)
        {
            bool status = true;
            byte relays_States = 0;
            byte button = 0;
            byte star = 0;

            if (!Tuning_Knob_Controls.Freq_Function.Active && !Tuning_Knob_Controls.Volume_Function.Volume_active)
            {
                Tuning_Knob_Controls.Status = Tuning_Knob_Controls.Device.GetPinStates(ref relays_States);
                if (Tuning_Knob_Controls.Status != FTDI.FT_STATUS.FT_OK)
                {
                    Tuning_Knob_Controls.Read_failure = true;
                    status = false;
                }
                else
                {
                    star = Set_Star(controller_switch);
                    button = (byte)(relays_States & controller_switch);
                    if (button == 0)
                    {
                        MonitorTextBoxText(" MFC_RIT_OFFSET_Function Called.");
                        if (Tuning_Knob_Controls.RIT_OFFSET_Function.Switch_Toggle == false)
                        {
                            Tuning_Knob_Controls.RIT_OFFSET_Function.Switch_Toggle = true;
                            if (Tuning_Knob_Controls.RIT_OFFSET_Function.Action_Toggle)
                            {
                              
                                Tuning_Knob_Controls.Active_Functions.Freq_Active = true;
                                Tuning_Knob_Controls.Active_Functions.RIT_Offset_Active = false;
                               
                                star = (byte)(star | 0x00);
                                MonitorTextBoxText(" MFC_RIT_OFFSET_Function Called - 0. Star: 0x" + star.ToString("X"));
                                oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_STAR, star);
                                Tuning_Knob_Controls.RIT_OFFSET_Function.Action_Toggle = false;
                            }
                            else
                            {
                               
                                Tuning_Knob_Controls.Active_Functions.Freq_Active = false;
                                Tuning_Knob_Controls.Active_Functions.RIT_Offset_Active = true;
                                star = (byte)(star | 0x80);
                                MonitorTextBoxText(" MFC_RIT_OFFSET_Function Called - 1. Star: 0x" + star.ToString("X"));
                                oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_STAR, star);
                                Tuning_Knob_Controls.RIT_OFFSET_Function.Action_Toggle = true;
                            }
                        }
                        status = false;
                    }
                    else
                    {
                        Tuning_Knob_Controls.RIT_OFFSET_Function.Switch_Toggle = false;
                        status = true;
                    }
                }
            }
            return status;
        }

        public bool MFC_FAVS_Function(byte controller_switch)
        {
            bool status = false;
            byte relays_States = 0;
            byte button = 0;

            if (!Tuning_Knob_Controls.Freq_Function.Active && !Tuning_Knob_Controls.Volume_Function.Volume_active)
            {
                Tuning_Knob_Controls.Status = Tuning_Knob_Controls.Device.GetPinStates(ref relays_States);
                if (Tuning_Knob_Controls.Status != FTDI.FT_STATUS.FT_OK)
                {
                    Tuning_Knob_Controls.Read_failure = true;
                    status = false;
                }
                else
                {
                    button = (byte)(relays_States & controller_switch);
                    if (button == 0)
                    {
                        MonitorTextBoxText(" MFC_FAVS_Function Called.");
                        if (Tuning_Knob_Controls.Step_Function.Switch_Toggle == false)
                        {
                            Tuning_Knob_Controls.Step_Function.Switch_Toggle = true;
                            MonitorTextBoxText(" MFC_FAVS_Function Called -> Calling: Set_Favs");
                            Set_Favs();
                        }
                        status = false;
                    }
                    else
                    {
                        Tuning_Knob_Controls.Step_Function.Switch_Toggle = false;
                        status = true;
                    }
                }
            }
            return status;
        }
        */
        public bool parse_line(String line)
        {
            //ListViewItem itm;
            String temp_string;
            int param_pos = 0;
            int end_pos = 0;

            param_pos = line.IndexOf("VERSION");
            if (param_pos != -1)
            {
                temp_string = line.Substring((param_pos + 8));   // parse everything between the end of NAME= and the end of the line.
                end_pos = temp_string.IndexOf(";"); //temp string will start with the VERSION parameter value. Find the trailing comma
                MFC_arr[0] = temp_string.Substring(0, end_pos);   // temp string will start with the DEVICE_NAME parameter value.
                param_pos = 0;
            }
            param_pos = line.IndexOf("STATE");
            if (param_pos != -1)
            {
                temp_string = line.Substring((param_pos + 6));   // parse everything between the end of NAME= and the end of the line.
                end_pos = temp_string.IndexOf(";"); //temp string will start with the VERSION parameter value. Find the trailing comma
                MFC_arr[5] = temp_string.Substring(0, end_pos);   // temp string will start with the DEVICE_NAME parameter value.
                param_pos = 0;
            }
            param_pos = line.IndexOf("KNOB_SWITCH");
            if (param_pos != -1)
            {
                temp_string = line.Substring((param_pos + 12));   // parse everything between the end of NAME= and the end of the line.
                end_pos = temp_string.IndexOf(";"); //temp string will start with the DEVICE_NAME parameter value. Find the trailing comma
                MFC_arr[1] = temp_string.Substring(0, end_pos);   // temp string will start with the DEVICE_NAME parameter value.
                param_pos = 0;
            }
            param_pos = line.IndexOf("LEFT_SWITCH");
            if (param_pos != -1)
            {
                temp_string = line.Substring((param_pos + 12));   // parse everything between the end of NAME= and the end of the line.
                end_pos = temp_string.IndexOf(";"); //temp string will start with the DEVICE_NAME parameter value. Find the trailing comma
                MFC_arr[2] = temp_string.Substring(0, end_pos);   // temp string will start with the DEVICE_NAME parameter value.
                param_pos = 0;
            }
            param_pos = line.IndexOf("MIDDLE_SWITCH");
            if (param_pos != -1)
            {
                temp_string = line.Substring((param_pos + 14));   // parse everything between the end of NAME= and the end of the line.
                end_pos = temp_string.IndexOf(";"); //temp string will start with the DEVICE_NAME parameter value. Find the trailing comma
                MFC_arr[3] = temp_string.Substring(0, end_pos);   // temp string will start with the DEVICE_NAME parameter value.
                param_pos = 0;
            }
            param_pos = line.IndexOf("RIGHT_SWITCH");
            if (param_pos != -1)
            {
                temp_string = line.Substring((param_pos + 13));   // parse everything between the end of NAME= and the end of the line.
                end_pos = temp_string.IndexOf(";"); //temp string will start with the DEVICE_NAME parameter value. Find the trailing comma
                MFC_arr[4] = temp_string.Substring(0, end_pos);   // temp string will start with the DEVICE_NAME parameter value.
                param_pos = 0;
            }
            param_pos = line.IndexOf("PTT_SWITCH");
            if (param_pos != -1)
            {
                temp_string = line.Substring((param_pos + 11));   // parse everything between the end of NAME= and the end of the line.
                end_pos = temp_string.IndexOf(";"); //temp string will start with the DEVICE_NAME parameter value. Find the trailing comma
                MFC_arr[6] = temp_string.Substring(0, end_pos);   // temp string will start with the DEVICE_NAME parameter value.
                param_pos = 0;
            }
            return false;
        }

        /*public bool Create_MFC_Init_File()
        {
            bool status = true;
            int version = 0, knob_switch = 0, left_switch = 0, middle_switch = 0, right_switch = 0,
                ptt_switch = Tuning_Knob_Controls.Button_Function_List.PTT_REAR_CONNECTOR;
            String path;
            System.IO.StreamWriter file;

            //oCode.Platform = (int)Environment.OSVersion.Platform;
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if ((oCode.Platform == 4) || (oCode.Platform == 6) || (oCode.Platform == 128))
            {// A kludge to check for non Windows OS.
             //These values may change in the future.
                path += "/mscc/mfc_controller.ini";
            }
            else
            {
                path += "\\multus-sdr-client\\mfc_controller.ini";
            }
            try
            {
                file = new System.IO.StreamWriter(File.OpenWrite(path));
            }
            catch (IOException e)
            {
                string er = e.Message;
                DialogResult ret = MessageBox.Show("IO Exception opening: multi_func_controller.ini. " +
                    "\r\n Error: " + er + "\r\nMake note of the error and send log files to Multus SDR,LLC.",
                    "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            version = Tuning_Knob_Controls.VERSION;

            MonitorTextBoxText(" Create_MFC_Init_File: " + knob_switch + left_switch + middle_switch + right_switch);
            file.Write("VERSION={0};\r\n", version);
            file.Write("STATE={0};\r\n", 0);
            file.Write("KNOB_SWITCH={0};\r\n", knob_switch);
            file.Write("LEFT_SWITCH={0};\r\n", left_switch);
            file.Write("MIDDLE_SWITCH={0};\r\n", middle_switch);
            file.Write("RIGHT_SWITCH={0};\r\n", right_switch);
            file.Write("PTT_SWITCH={0};\r\n", ptt_switch);
            file.Close();
            return status;
        }

        public bool Update_MFC_Init_File()
        {
            bool status = true;
            int version = 0, knob_switch = 0, left_switch = 0, middle_switch = 0, right_switch = 0,
                ptt_switch = Tuning_Knob_Controls.Button_Function_List.PTT_REAR_CONNECTOR, state = 0;
            String path;
            System.IO.StreamWriter file;

            //oCode.Platform = (int)Environment.OSVersion.Platform;
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if ((oCode.Platform == 4) || (oCode.Platform == 6) || (oCode.Platform == 128))
            {// A kludge to check for non Windows OS.
             //These values may change in the future.
                path += "/mscc/mfc_controller.ini";
            }
            else
            {
                path += "\\multus-sdr-client\\mfc_controller.ini";
            }
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            try
            {
                file = new System.IO.StreamWriter(File.OpenWrite(path));
            }
            catch (IOException e)
            {
                string er = e.Message;
                DialogResult ret = MessageBox.Show("IO Exception opening: multi_func_controller.ini. " +
                    "\r\n Error: " + er + "\r\nMake note of the error and send log files to Multus SDR,LLC.",
                    "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            version = Tuning_Knob_Controls.VERSION;
            knob_switch = Knob_comboBox1.SelectedIndex;
            left_switch = Left_Button_comboBox2.SelectedIndex;
            middle_switch = Middle_Button_comboBox3.SelectedIndex;
            right_switch = Right_Button_comboBox4.SelectedIndex;
            ptt_switch = Tuning_Knob_Controls.Button_Function_List.PTT_REAR_CONNECTOR;
            if (Tuning_Knob_Controls.On)
            {
                state = 1;
            }
            else
            {
                state = 0;
            }
            MonitorTextBoxText(" Update_MFC_Init_File: " + knob_switch + left_switch + middle_switch +
                right_switch + ptt_switch);

            file.Write("VERSION={0};\r\n", version);
            file.Write("STATE={0};\r\n", state);
            file.Write("KNOB_SWITCH={0};\r\n", knob_switch);
            file.Write("LEFT_SWITCH={0};\r\n", left_switch);
            file.Write("MIDDLE_SWITCH={0};\r\n", middle_switch);
            file.Write("RIGHT_SWITCH={0};\r\n", right_switch);
            file.Write("PTT_SWITCH={0};\r\n", ptt_switch);
            file.Close();
            return status;
        }
        */
        public bool Initialize_MFC()
        {
            String path, line;
            System.IO.StreamReader file;
            bool status = false;
            int version = 0, knob_switch = 0, left_switch = 0, middle_switch = 0, right_switch = 0,
                ptt_switch = Tuning_Knob_Controls.Button_Function_List.PTT_REAR_CONNECTOR, state = 0;

            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#if RPI
            path += "/mscc/mfc_controller.ini";
#else
            path += "\\multus-sdr-client\\mfc_controller.ini";
#endif
            try
            { 
                file = new System.IO.StreamReader(File.OpenRead(path));
            }
            catch (IOException e)
            {
                string er = e.Message;
                DialogResult ret = MessageBox.Show("IO Exception opening: mfc_controller.ini. " +
                    "\r\n Error: " + er + "\r\nMake note of the error and send log files to Multus SDR,LLC.",
                    "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            while ((line = file.ReadLine()) != null)
            {
                status = parse_line(line);
            }
            file.Close();
            version = Convert.ToInt16(MFC_arr[0]);
            knob_switch = Convert.ToInt16(MFC_arr[1]);
            left_switch = Convert.ToInt16(MFC_arr[2]);
            middle_switch = Convert.ToInt16(MFC_arr[3]);
            right_switch = Convert.ToInt16(MFC_arr[4]);
            state = Convert.ToInt16(MFC_arr[5]);
            ptt_switch = Convert.ToInt16(MFC_arr[6]);

            Master_Controls.code_triggered = true;
            Knob_comboBox1.SelectedIndex = knob_switch;
            Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.KNOB_SWITCH] = (sbyte)knob_switch;
            Tuning_Knob_Controls.Knob_switch_function = (byte)knob_switch;
            MFC_Knob_label38.Text = "K: " + Knob_comboBox1.Text;

            Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.PTT_SWITCH] = (sbyte)ptt_switch;
            Tuning_Knob_Controls.PTT_switch_function = (byte)ptt_switch;

            Left_Button_comboBox2.SelectedIndex = left_switch;
            Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.LEFT_SWITCH] = (sbyte)left_switch;
            Tuning_Knob_Controls.Button_left_function = (byte)left_switch;
            Tuning_Knob_Controls.Star_Position.Left = (byte)(0x10 | left_switch);
            MFC_A_label38.Text = "A: " + Left_Button_comboBox2.Text;

            Middle_Button_comboBox3.SelectedIndex = middle_switch;
            Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.MIDDLE_SWITCH] = (sbyte)middle_switch;
            Tuning_Knob_Controls.Button_middle_function = (byte)middle_switch;
            Tuning_Knob_Controls.Star_Position.Middle = (byte)(0x20 | middle_switch);
            MFC_B_label38.Text = "B: " + Middle_Button_comboBox3.Text;


            Right_Button_comboBox4.SelectedIndex = right_switch;
            Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.RIGHT_SWITCH] = (sbyte)right_switch;
            Tuning_Knob_Controls.Button_right_function = (byte)right_switch;
            Tuning_Knob_Controls.Star_Position.Right = (byte)(0x30 | right_switch);
            MFC_C_label38.Text = "C: " + Right_Button_comboBox4.Text;
            Master_Controls.code_triggered = false;

            if (state == 1)
            {
                Tuning_Knob_Controls.On = true;
                //Tuning_Knob_groupBox1.Enabled = true;
                Knob_comboBox1.Enabled = true;
                Left_Button_comboBox2.Enabled = true;
                Right_Button_comboBox4.Enabled = true;
                Middle_Button_comboBox3.Enabled = true;
            }
            else
            {
                //Tuning_Knob_groupBox1.Enabled = false;
                Knob_comboBox1.Enabled = false;
                Left_Button_comboBox2.Enabled = false;
                Right_Button_comboBox4.Enabled = false;
                Middle_Button_comboBox3.Enabled = false;
            }
            Tuning_Knob_Controls.MFC_Initialized = true;
            MonitorTextBoxText(" Tuning Knob Initialization Finished: " + knob_switch + left_switch + middle_switch + right_switch);
            return true;
        }

        public bool Compare_Switches(sbyte MFC_switch, int switch_value)
        {
            bool status = true;

            MonitorTextBoxText(" Compare_Switches Called. MFC_Switch: " + Convert.ToString(MFC_switch));
            MonitorTextBoxText(" Compare_Switches: " + Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.KNOB_SWITCH] +
                 Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.LEFT_SWITCH] +
                Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.MIDDLE_SWITCH] +
                Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.RIGHT_SWITCH] +
                Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.PTT_switch]);

            switch (MFC_switch)
            {
                case Tuning_Knob_Controls.KNOB_SWITCH:
                    if (switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.LEFT_SWITCH] ||
                        switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.RIGHT_SWITCH] ||
                        switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.MIDDLE_SWITCH] ||
                        switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.PTT_SWITCH])
                    {
                        status = false;
                    }
                    break;

                case Tuning_Knob_Controls.LEFT_SWITCH:
                    if (switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.MIDDLE_SWITCH] ||
                        switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.RIGHT_SWITCH] ||
                        switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.KNOB_SWITCH] ||
                        switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.PTT_SWITCH])
                    {
                        status = false;
                    }
                    break;

                case Tuning_Knob_Controls.MIDDLE_SWITCH:
                    if (switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.LEFT_SWITCH] ||
                        switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.RIGHT_SWITCH] ||
                        switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.KNOB_SWITCH] ||
                        switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.PTT_SWITCH])
                    {
                        status = false;
                    }
                    break;

                case Tuning_Knob_Controls.RIGHT_SWITCH:
                    if (switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.LEFT_SWITCH] ||
                        switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.KNOB_SWITCH] ||
                        switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.MIDDLE_SWITCH] ||
                        switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.PTT_SWITCH])
                    {
                        status = false;
                    }
                    break;

                case Tuning_Knob_Controls.PTT_SWITCH:
                    if (switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.LEFT_SWITCH] ||
                        switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.KNOB_SWITCH] ||
                        switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.MIDDLE_SWITCH] ||
                        switch_value == Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.RIGHT_SWITCH])
                    {
                        status = false;
                    }
                    break;
            }
            if (status == false)
            {
                MessageBox.Show("Multiple switches may NOT have the same function", "MSCC",
                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            MonitorTextBoxText(" Compare_Switches Finished. Status: " + Convert.ToString(status));
            return status;
        }

        private void Tuning_Knob_checkBox2_CheckedChanged_1(object sender, EventArgs e)
        {
            /*
            if (oCode.isLoading) return;
#if FTDI
            uint bytes_written = 0;
            byte[] sentBytes = new byte[2];
            if (!Master_Controls.code_triggered)
            {
                if (!Relay_Board_Controls.On)
                {
                    if (!Tuning_Knob_Controls.On)
                    {
                        
                        if (Tuning_Knob_Controls.Open)
                        {
                            Tuning_Knob_Controls.On = true;
                          
                            Update_MFC_Init_File();
                            MonitorTextBoxText(" Tuning Knob Open. Multiplier: "
                                + Convert.ToString(Tuning_Knob_Controls.Step_Function.Multiplier));
                            Knob_comboBox1.Enabled = true;
                            Left_Button_comboBox2.Enabled = true;
                            Right_Button_comboBox4.Enabled = true;
                            Middle_Button_comboBox3.Enabled = true;
                        }
                        else
                        {
                           
                            Tuning_Knob_checkBox2.Checked = false;
                            
                            Update_MFC_Init_File();
                            Knob_comboBox1.Enabled = false;
                            Left_Button_comboBox2.Enabled = false;
                            Right_Button_comboBox4.Enabled = false;
                            Middle_Button_comboBox3.Enabled = false;
                            MessageBox.Show("Tuning Knob OPEN FAILED", "MSCC",
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                    }
                    else
                    {
                        if (Tuning_Knob_Controls.Open)
                        {
                            sentBytes[0] = (byte)(sentBytes[0] | 0xFF);
                            Tuning_Knob_Controls.Device.Write(sentBytes, 1, ref bytes_written);
                            Tuning_Knob_Controls.Device.Close();

                            MonitorTextBoxText(" Tuning Knob CLOSED");
                        }
                        Tuning_Knob_Controls.On = false;
                       
                        Knob_comboBox1.Enabled = false;
                        Left_Button_comboBox2.Enabled = false;
                        Right_Button_comboBox4.Enabled = false;
                        Middle_Button_comboBox3.Enabled = false;
                        Update_MFC_Init_File();
                    }
                }
                else
                {
                    Tuning_Knob_checkBox2.Checked = false;
                    Update_MFC_Init_File();
                    Knob_comboBox1.Enabled = false;
                    Left_Button_comboBox2.Enabled = false;
                    Right_Button_comboBox4.Enabled = false;
                    Middle_Button_comboBox3.Enabled = false;
                    MessageBox.Show("Relay Board In use. Turn off Relay Board", "MSCC",
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
#else
            MessageBox.Show("Incorrect MSCC Version. The FTDI Version is required", "MSCC",
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
             Tuning_Knob_checkBox2.Checked = false;
#endif
*/
        }

#if FTDI

        private int Freq_queue_add(int command)
        {
            int status = Tuning_Knob_Controls.Freq_Queue.QUEUE_SUCCESS;

            Tuning_Knob_Controls.Freq_Queue.E_queue_busy = true;
            if (Tuning_Knob_Controls.Freq_Queue.E_freq_queue_front == (Tuning_Knob_Controls.Freq_Queue.E_freq_queue_rear +
                1) % Tuning_Knob_Controls.Freq_Queue.MAX_COMMAND_QUEUE)
            {
                Tuning_Knob_Controls.Freq_Queue.E_freq_queue_front = -1;
                Tuning_Knob_Controls.Freq_Queue.E_freq_queue_rear = -1;
                Tuning_Knob_Controls.Freq_Queue.E_queue_count = 0;
                status = Tuning_Knob_Controls.Freq_Queue.QUEUE_FAILED;
                MonitorTextBoxText(" Queue Overflow");
            }
            else
            {
                if (Tuning_Knob_Controls.Freq_Queue.E_freq_queue_front == -1)
                {
                    Tuning_Knob_Controls.Freq_Queue.E_freq_queue_front =
                        Tuning_Knob_Controls.Freq_Queue.E_freq_queue_rear = 0;
                }
                else
                {
                    Tuning_Knob_Controls.Freq_Queue.E_freq_queue_rear = (Tuning_Knob_Controls.Freq_Queue.E_freq_queue_rear
                        + 1) % Tuning_Knob_Controls.Freq_Queue.MAX_COMMAND_QUEUE;
                }
                Tuning_Knob_Controls.Freq_Queue.E_freq_queue[Tuning_Knob_Controls.Freq_Queue.E_freq_queue_rear] = command;
                Tuning_Knob_Controls.Freq_Queue.E_queue_count++;
            }
            Tuning_Knob_Controls.Freq_Queue.E_queue_busy = false;
            return (status);
        }

        private int Dequeue_freq()
        {
            int ret = 0;

            if (Tuning_Knob_Controls.Freq_Queue.E_freq_queue_front == -1)
            {
                ret = 0;
            }
            else
            {
                ret = Tuning_Knob_Controls.Freq_Queue.E_freq_queue[Tuning_Knob_Controls.Freq_Queue.E_freq_queue_front];
                Tuning_Knob_Controls.Freq_Queue.E_freq_queue[Tuning_Knob_Controls.Freq_Queue.E_freq_queue_front] = 0;
                if (Tuning_Knob_Controls.Freq_Queue.E_freq_queue_front == Tuning_Knob_Controls.Freq_Queue.E_freq_queue_rear)
                {
                    Tuning_Knob_Controls.Freq_Queue.E_freq_queue_front =
                        Tuning_Knob_Controls.Freq_Queue.E_freq_queue_rear = -1;
                }
                else
                {
                    Tuning_Knob_Controls.Freq_Queue.E_freq_queue_front = (Tuning_Knob_Controls.Freq_Queue.E_freq_queue_front
                        + 1) % Tuning_Knob_Controls.Freq_Queue.MAX_COMMAND_QUEUE;
                }
            }
            if (ret != 0)
            {
                Tuning_Knob_Controls.Freq_Queue.E_queue_count--;
            }
            return ret;
        }

#endif

        private void AMP_label57_Click(object sender, EventArgs e)
        {
        }

        private void AMP_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (oCode.isLoading) { return; }
            if (Amplifier_Power_Controls.Solidus_Band_Selected)
            {
                int powerValue = AMP_hScrollBar1.Value;
                if (powerValue == Amplifier_Power_Controls.New_Power_Value) return; // nothing happens if value doesn't change from previous
                Amplifier_Power_Controls.New_Power_Value = powerValue;
                AMP_label57.Text = Convert.ToString(powerValue) + " %";
                MonitorTextBoxText(" AMP_hScrollBar1_Scroll -> New_POWER_Value: " +
                    Convert.ToString(Amplifier_Power_Controls.New_Power_Value));
                oCode.SendCommand(txsocket, txtarget, Amplifier_Power_Controls.CMD_SET_AMPLIFIER_POWER,
                           (short)Amplifier_Power_Controls.New_Power_Value);
            }
            else
            {
                MessageBox.Show("Select a Band", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void AMP_Tune_button4_Click(object sender, EventArgs e)
        {
            short power = 0;
            short mode_number = 0;
            MonitorTextBoxText(" AMP_Tune_button4 Pressed ");
            if (Amplifier_Power_Controls.Solidus_Band_Selected)
            {
                if (!Master_Controls.TX_Inhibited)
                {
                    if (Master_Controls.Tuning_Mode == false)
                    {
                        oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 1);
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, Mode.TUNE_mode);
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_RIG_TUNE, 1);
                        AMP_Tune_button4.BackColor = Color.Red;
                        AMP_Tune_button4.ForeColor = Color.White;
                        buttTune.BackColor = Color.Red;
                        buttTune.ForeColor = Color.White;
                        Master_Controls.Tuning_Mode = true;
                        Master_Controls.Transmit_Mode = true;
                        AM_Carrier_hScrollBar1.Enabled = false;
                        Power_hScrollBar1.Enabled = false;
                        CW_Power_hScrollBar1.Enabled = false;
                        button1.Enabled = false;
                        Tune_vButton2.BackColor = Color.Red;
                        Tune_vButton2.ForeColor = Color.White;
                        Volume_Controls.Previous_Slider_Mode = Volume_Controls.Volume_Slider_Mode;
                        Volume_Controls.Volume_Slider_Mode = Volume_Controls.SLIDER_TUNE_MODE;
                    }
                    else
                    {
                        if (!Settings.Default.Speaker_MutED)
                        {
                            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                        }
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_RIG_TUNE, 0);
                        power = get_previous_power(oCode.current_band);
                        mode_number = Convert_mode_char_to_digit(Last_used.Current_mode);
                        MonitorTextBoxText(" Tune -> Current Band: " + oCode.current_band +
                            " Current Mode Number: " + mode_number);
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
                        buttTune.BackColor = Color.Gainsboro;
                        buttTune.ForeColor = Color.Black;
                        AMP_Tune_button4.BackColor = Color.Gainsboro;
                        AMP_Tune_button4.ForeColor = Color.Black;
                        Master_Controls.Tuning_Mode = false;
                        Master_Controls.Transmit_Mode = false;
                        AM_Carrier_hScrollBar1.Enabled = true;
                        Power_hScrollBar1.Enabled = true;
                        CW_Power_hScrollBar1.Enabled = true;
                        button1.Enabled = true;
                        Tune_vButton2.BackColor = Color.Gainsboro;
                        Tune_vButton2.ForeColor = Color.Black;
                        Volume_Controls.Volume_Slider_Mode = Volume_Controls.Previous_Slider_Mode;
                    }
                }
            }
            else
            {
                MessageBox.Show("Select a Band", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

            MonitorTextBoxText(" Tune -> finished ");
        }

        private void AMP_groupBox3_Enter(object sender, EventArgs e)
        {
        }

        private void Auto_Zero_checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            byte[] buf = new byte[2];
            if (oCode.isLoading) return;
            buf[0] = Master_Controls.Extended_Commands.CMD_MFC_AUTO_ZERO;
            if(Auto_Zero_checkBox2.Checked == false)
            {
                Settings.Default.Auto_Zero = false;
                buf[1] = 0;
            }
            else
            {
                Settings.Default.Auto_Zero = true;
                buf[1] = 1;
            }
            oCode.SendCommand_MultiByte(txsocket, txtarget,Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND,
                                                                buf, buf.Length);
        }

        private void Freq_Cal_timer4_Tick(object sender, EventArgs e)
        {
            if (Frequency_Calibration_controls.Display_Wait &&
                Frequency_Calibration_controls.Display_Wait_count != 0)
            {
                Freq_Cal_label59.Visible = true;
                Freq_Cal_label59.Enabled = true;
                Frequency_Calibration_controls.Display_Wait_count--;
            }
            else
            {
                Freq_Cal_label59.Visible = false;
                Freq_Cal_label59.Enabled = false;
                Frequency_Calibration_controls.Display_Wait = false;
                Freq_Cal_timer4.Enabled = false;
                if (Frequency_Calibration_controls.Reset == true)
                {
                    Freq_Cal_Reset_button4.Text = "RESET";
                    MessageBox.Show("Frequency Calibration has been RESET.", "MSCC",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Frequency_Calibration_controls.Reset = false;
                }
            }
        }

        private void Freq_Cal_label59_Click(object sender, EventArgs e)
        {
        }

        private void label57_Click(object sender, EventArgs e)
        {
        }

        private void Spectrum_checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!Window_controls.Panadapter_Toggle)
            {
                oCode.SendCommand(txsocket, txtarget, Window_controls.CMD_SET_PANADAPTER_DISPLAY, 1);
                Window_controls.Panadapter_Toggle = true;
            }
            else
            {
                oCode.SendCommand(txsocket, txtarget, Window_controls.CMD_SET_PANADAPTER_DISPLAY, 0);
                Window_controls.Panadapter_Toggle = false;
            }
        }

        private void Smeter_checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!Window_controls.Smeter_Toggle)
            {
                oCode.SendCommand(txsocket, txtarget, Window_controls.CMD_SET_SMETER_DISPLAY, 1);
                Window_controls.Smeter_Toggle = true;
            }
            else
            {
                oCode.SendCommand(txsocket, txtarget, Window_controls.CMD_SET_SMETER_DISPLAY, 0);
                Window_controls.Smeter_Toggle = false;
            }
        }

        private void Band_Change_Auto_Tune_checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (!Settings.Default.Band_Change_Tune)
            {
                Settings.Default.Band_Change_Tune = true;
            }
            else
            {
                Settings.Default.Band_Change_Tune = false;
            }
        }

        private void Amplifier_temperature_label58_Click(object sender, EventArgs e)
        {
        }

        private void Temperature_label57_Click_1(object sender, EventArgs e)
        {
        }

        /*private void Power_Cal_QRPbutton3_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (Master_Controls.Mia_Status)
            {
                if (Master_Controls.QRP_Mode)
                {
                    PA_vButton1.BackColor = Color.Red;
                    PA_vButton1.ForeColor = Color.White;
                    PA_vButton1.Text = "QRO";

                    Master_Controls.QRP_Mode = false;
                    oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 1);
                    MonitorTextBoxText("Power_Cal_QRPbutton3_Click -> Set QRO Mode");
                }
                else
                {
                    PA_vButton1.Text = "QRP";
                    PA_vButton1.BackColor = Control.DefaultBackColor;
                    PA_vButton1.ForeColor = Color.Green;

                    Master_Controls.QRP_Mode = true;
                    oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 0);
                    MonitorTextBoxText("Power_Cal_QRPbutton3_Click -> Set QRP Mode");
                }
            }
            else
            {
                MessageBox.Show("Meter NOT attached", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }*/

        private void AMP_Calibrate_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        { 
            int value = 0;
            if (oCode.isLoading) return;
            if (Amplifier_Power_Controls.Solidus_Band_Selected)
            {
                value = AMP_Calibrate_hScrollBar1.Value;
                if (Amplifier_Power_Controls.Calibration_Value == value) return;
                Amplifier_Power_Controls.Calibration_Value = value;
                Power_calibration_label58.Text = Convert.ToString(value);
                oCode.SendCommand32(txsocket, txtarget, Amplifier_Power_Controls.CMD_SET_POTENTIA_CALIBRATION,
                    Amplifier_Power_Controls.Calibration_Value);
            }
            else
            {
                MessageBox.Show("Select a Band", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void Solidus_Bias_button8_Click(object sender, EventArgs e)
        {
            if (Amplifier_Power_Controls.Solidus_Band_Selected)
            {
                if (Amplifier_Power_Controls.Bias_On == false)
                {
                    Amplifier_Power_Controls.Bias_On = true;
                    AMP_Tune_button4.Enabled = false;
                    AMP_hScrollBar1.Enabled = false;
                    AMP_Calibrate_hScrollBar1.Enabled = false;
                    oCode.SendCommand(txsocket, txtarget, Power_Controls.CMD_SET_TUNE_POWER, 0);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, Mode.TUNE_mode);
                    //oCode.SendCommand(txsocket, txtarget, Amplifier_Power_Controls.CMD_GET_POTENTIA_BIAS, 1);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_TX_ON, 1);
                    Solidus_Bias_button8.BackColor = Color.Red;
                    Solidus_Bias_button8.ForeColor = Color.White;
                    AMP_Tune_button4.BackColor = Color.Gainsboro;
                    AMP_Tune_button4.ForeColor = Color.Black;
                    Master_Controls.Tuning_Mode = false;
                    Master_Controls.Transmit_Mode = false;
                }
                else
                {
                    Amplifier_Power_Controls.Bias_On = false;
                    AMP_Tune_button4.Enabled = true;
                    AMP_hScrollBar1.Enabled = true;
                    AMP_Calibrate_hScrollBar1.Enabled = true;
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_TX_ON, 0);
                    oCode.SendCommand(txsocket, txtarget, Power_Controls.CMD_SET_TUNE_POWER,
                                            (short)Power_Controls.TUNE_Power);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, Mode.TUNE_mode);
                    //oCode.SendCommand(txsocket, txtarget, Amplifier_Power_Controls.CMD_GET_POTENTIA_BIAS, 0);
                    Solidus_Bias_button8.BackColor = Color.Gainsboro;
                    Solidus_Bias_button8.ForeColor = Color.Black;
                }
            }
            else
            {
                MessageBox.Show("Select a Band", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void NR_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value = 0;
            if (oCode.isLoading) return;
            value = NR_hScrollBar1.Value;
            if (value == NR_Controls.NR_Value) return;
            NR_Controls.NR_Value = value;
            NR_label5.Text = Convert.ToString(NR_Controls.NR_Value);
        }

        private void NR_Button_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (!NR_Controls.NR_Button_On)
            {
                NR_Button.Text = "ON";
                NR_Controls.NR_Button_On = true;
            }
            else
            {
                NR_Button.Text = "OFF";
                NR_Controls.NR_Button_On = false;
            }
        }

        private void NR_label5_Click(object sender, EventArgs e)
        {
        }

        private void AMP_Current_label5_Click(object sender, EventArgs e)
        {
        }

        private void IQ_Tune_Power_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value;
            if (oCode.isLoading == true) return;
            value = IQ_Tune_Power_hScrollBar1.Value;
            if (IQ_Controls.Tune_power == (short)value) return;
            IQ_Controls.Tune_power = (short)value;
            oCode.SendCommand(txsocket, txtarget, Power_Controls.CMD_SET_TUNE_POWER, (short)value);
        }

        private void AMP_Raw_Bias_label5_Click(object sender, EventArgs e)
        {
        }

        private void AMP_Temp_MFC_AMP_label5_Click(object sender, EventArgs e)
        {
        }

        private void AMP_Calibration_label58_Click(object sender, EventArgs e)
        {
        }

        private void Power_calibration_label58_Click(object sender, EventArgs e)
        {
        }

        private void AMP_Power_Output_label5_Click(object sender, EventArgs e)
        {
        }

        private void label50_Click(object sender, EventArgs e)
        {
        }

        private void label36_Click_1(object sender, EventArgs e)
        {
        }

        /*private void ALC_label5_Click(object sender, EventArgs e)
        {
        }*/

        private void Audio_Digital_button3_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (!Settings.Default.Mic_Is_Digital)
            {
                Audio_Digital_button3.Text = "D";
                Settings.Default.Mic_Is_Digital = true;
                Compression_button4.Enabled = false;
                Compression_button2.Enabled = false;
                MonitorTextBoxText(" Audio_Digital_button3: " + Convert.ToString(Settings.Default.Mic_Is_Digital));
                MicVolume_hScrollBar1.Value = Settings.Default.Digital_Volume;
                Microphone_textBox2.Text = Convert.ToString(Settings.Default.Digital_Volume);
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_VOLUME, Settings.Default.Digital_Volume);
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_COMPRESSION_STATE, 0);
                RPi_Settings.Volume_Settings.Mic_Mode = 1;
            }
            else
            {
                Audio_Digital_button3.Text = "V";
                Compression_button4.Enabled = true;
                Compression_button2.Enabled = true;
                Settings.Default.Mic_Is_Digital = false;
                MonitorTextBoxText(" Audio_Digital_button3: " + Convert.ToString(Settings.Default.Mic_Is_Digital));
                MicVolume_hScrollBar1.Value = Settings.Default.Voice_Volume;
                Microphone_textBox2.Text = Convert.ToString(Settings.Default.Voice_Volume);
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_VOLUME, Settings.Default.Voice_Volume);
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_COMPRESSION_STATE, Volume_Controls.Compression_State);
                RPi_Settings.Volume_Settings.Mic_Mode = 0;
            }
        }

        private void Spectrum_Controls_button3_Click(object sender, EventArgs e)
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

        private void Local_Date_label46_Click(object sender, EventArgs e)
        {

        }

        private void Full_Power_checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (!Power_Controls.Full_power)
            {
                Power_Controls.Full_power = true;
                Full_Power_checkBox1.Text = "-Power Full-";
                oCode.SendCommand(txsocket, txtarget, Power_Controls.CMD_SET_TUNE_POWER, 100);
            }
            else
            {
                Power_Controls.Full_power = false;
                Full_Power_checkBox1.Text = "Power TUNE";
                oCode.SendCommand(txsocket, txtarget, Power_Controls.CMD_SET_TUNE_POWER, Power_Controls.TUNE_Power);
            }
        }

        private void Waterfall_checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            byte[] buf = new byte[2];
            if (oCode.isLoading) return;
            buf[0] = Master_Controls.Extended_Commands.CMD_SET_WATERFALL_DISPLAY;
            if (!Window_controls.Waterfall_Toggle)
            {
                buf[1] = 1;
                Window_controls.Waterfall_Toggle = true;
            }
            else
            {
                buf[1] = 0;
                Window_controls.Waterfall_Toggle = false;
            }
            oCode.SendCommand_MultiByte(txsocket, txtarget,
                    Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (!Settings.Default.Docked)
            {
                Window_controls.Docking_Controls.Previous_Docked = true;
                Settings.Default.Docked = true;
            }
            else
            {
                Settings.Default.Docked = false;
                Window_controls.Docking_Controls.Previous_Docked = false;
            }
        }

        public void Main_vumeter_level()
        {
            int smeter_display_value = 0;
            int smeter_display_value_adj = 0;
            float ratio = (float)(150 / 13);
            Point ALC_Location = new Point(0, 0);

            ALC_Location.X = Master_Controls.Main_Smeter_Position.X + 5;
            ALC_Location.Y = Master_Controls.Main_Smeter_Position.Y;
            int Db_to_Smeter(int db)
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
                if (db <= -53) return 20;
                if (db <= -43) return 30;
                if (db <= -33) return 40;
                if (db <= -23) return 50;
                if (db <= -13) return 60;
                return smeter_value;
            }
            if (Master_Controls.Transmit_Mode)
            {
                Main_Smeter_controls.Display_mode = Main_Smeter_controls.VU_MODE;
            }
            else
            {
                Main_Smeter_controls.Display_mode = Main_Smeter_controls.S_METER_MODE;
            }

            /*if (Main_Smeter_controls.Meter_mode != Main_Smeter_controls.Display_mode)
            {
                if (Main_Smeter_controls.Display_mode != Main_Smeter_controls.S_METER_MODE)
                {
                   
                }
                else
                {
        
                }
                Main_Smeter_controls.Meter_mode = Main_Smeter_controls.Display_mode;
            }*/
            if (Main_Smeter_controls.Display_mode == Main_Smeter_controls.S_METER_MODE)
            {
                if (Main_Smeter_controls.Previous_meter_value != Main_Smeter_controls.SMeter_value)
                {
                    smeter_display_value = Db_to_Smeter(Main_Smeter_controls.SMeter_value);
                    if (Main_Smeter_controls.Smeter_Hold_On)
                    {
                        if (smeter_display_value < Main_Smeter_controls.Previous_Low)
                        {
                            Main_Smeter_controls.Previous_Low =
                                Main_Smeter_controls.Previous_Low - Main_Smeter_controls.Smeter_decrement;
                            smeter_display_value = Main_Smeter_controls.Previous_Low;

                        }
                        else
                        {
                            Main_Smeter_controls.Previous_Low = smeter_display_value;
                        }
                    }

                    if (smeter_display_value <= 10)
                    {
                        smeter_display_value_adj = smeter_display_value * 10;
                    }
                    else
                    {
                        switch (smeter_display_value)
                        {
                            case 20:
                                smeter_display_value_adj = (int)(ratio * (float)10);
                                break;
                            case 30:
                                smeter_display_value_adj = (int)(ratio * (float)11);
                                break;
                            case 40:
                                smeter_display_value_adj = (int)(ratio * (float)12);
                                break;
                            case 50:
                                smeter_display_value_adj = (int)(ratio * (float)13);
                                break;
                            case 60:
                                smeter_display_value_adj = (int)(ratio * (float)14);
                                break;
                        }
                    }
                    Main_Smeter_controls.Previous_meter_value = smeter_display_value;
                }
            }
            else
            {
                if (Main_Smeter_controls.Previous_meter_value != Main_Smeter_controls.VU_Meter_value)
                {
                    Main_Smeter_controls.Previous_meter_value = Main_Smeter_controls.VU_Meter_value;

                }
            }
        }

        private void Main_Smeter_timer_Tick(object sender, EventArgs e)
        {
            Main_vumeter_level();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //colorDialog1.ShowHelp = true;
            colorDialog1.ShowDialog();
            Settings.Default.Background_Color = colorDialog1.Color;
            //RPi_Settings.RPi_Needs_Updated = true;
            Manage_Colors();
        }

        private void Manage_Colors()
        {
            mainPage.BackColor = Settings.Default.Background_Color;
            TX.BackColor = Settings.Default.Background_Color;
            powertabPage1.BackColor = Settings.Default.Background_Color;
            band_stack.BackColor = Settings.Default.Background_Color;
            freqcaltab.BackColor = Settings.Default.Background_Color;
            Audio_tabPage1.BackColor = Settings.Default.Background_Color;
            MFC.BackColor = Settings.Default.Background_Color;
            metertab.BackColor = Settings.Default.Background_Color;
            //panel2.BackColor = Settings.Default.Background_Color;
            Band_Change_Auto_Tune_checkBox2.BackColor = Settings.Default.Background_Color;
            Auto_Zero_checkBox2.BackColor = Settings.Default.Background_Color;
            vuMeter1.BackColor = Settings.Default.Background_Color;


            Time_display_label33.ForeColor = Settings.Default.Freq_Color;
            //Time_display_label33.BackColor = Settings.Default.Boarder_Color;
            Local_Date_label46.ForeColor = Settings.Default.Freq_Color;
            //Local_Date_label46.BackColor = Settings.Default.Boarder_Color;
            Time_display_UTC_label34.ForeColor = Settings.Default.Freq_Color;
            //Time_display_UTC_label34.BackColor = Settings.Default.Boarder_Color;
            UTC_Date_label46.ForeColor = Settings.Default.Freq_Color;
            //UTC_Date_label46.BackColor = Settings.Default.Boarder_Color;
            panel1.BackColor = Settings.Default.Boarder_Color;
            panel1.ForeColor = Settings.Default.Freq_Color;
            groupBox3.BackColor = Settings.Default.Boarder_Color;
            ritfreqtextBox1.ForeColor = Settings.Default.Freq_Color;
            ritfreqtextBox1.BackColor = Settings.Default.Boarder_Color;
            Freq_Digit_Test_label58.BackColor = Settings.Default.Boarder_Color;
            Freq_Digit_Test_label58.ForeColor = Settings.Default.Freq_Color;

            Ones.ForeColor = (Settings.Default.Freq_Color);
            Tens.ForeColor = Settings.Default.Freq_Color;
            Hundreds.ForeColor = Settings.Default.Freq_Color;
            Thousands.ForeColor = Settings.Default.Freq_Color;
            Tenthousands.ForeColor = Settings.Default.Freq_Color;
            Hundredthousand.ForeColor = Settings.Default.Freq_Color;
            Millions.ForeColor = Settings.Default.Freq_Color;
            Tenmillions.ForeColor = Settings.Default.Freq_Color;
            Decimal_label58.ForeColor = Settings.Default.Freq_Color;
            Decimal_label59.ForeColor = Settings.Default.Freq_Color;

            Ones.BackColor = Settings.Default.Boarder_Color;
            Tens.BackColor = Settings.Default.Boarder_Color;
            Hundreds.BackColor = Settings.Default.Boarder_Color;
            Thousands.BackColor = Settings.Default.Boarder_Color;
            Tenthousands.BackColor = Settings.Default.Boarder_Color;
            Hundredthousand.BackColor = Settings.Default.Boarder_Color;
            Millions.BackColor = Settings.Default.Boarder_Color;
            Tenmillions.BackColor = Settings.Default.Boarder_Color;
        }

        private void Freq_Color_button4_Click(object sender, EventArgs e)
        {
            DialogResult res = colorDialog1.ShowDialog();
            Color Freq_Color = colorDialog1.Color;
            Settings.Default.Freq_Color = Freq_Color;
            if (res == DialogResult.OK)
            {
                Manage_Colors();
                return;
            }
            //RPi_Settings.RPi_Needs_Updated = true;
        }

        private void Autodock_checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            Point Main_location = this.Location;
            MonitorTextBoxText(" Autodock Value: " + Convert.ToString(Settings.Default.Autodocked));
            Point Spectrum_location = new Point(0, 0);
            Point Waterfall_location = new Point(0, 0);


        }

        private void Freq_Digit_Test_label58_Click(object sender, EventArgs e)
        {

        }

        private void Boarder_Color_button4_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            Color Boarder_Color = colorDialog1.Color;
            Settings.Default.Boarder_Color = Boarder_Color;
            Manage_Colors();
            //RPi_Settings.RPi_Needs_Updated = true;
        }

        /*private void label58_Click(object sender, EventArgs e)
        {

        }*/

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        public void Digit_freq_update(byte n)
        {
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

            //MonitorTextBoxText(Convert.ToString(oCode.DisplayFreq) );
            if (oCode.DisplayFreq < 0) return;
            if (oCode.DisplayFreq > 40000000) oCode.DisplayFreq = 40000000;
            Display_Main_Freq();
            Panadapter_Controls.Freq_Set_By_Master = true;

            int freq_plus_rit = oCode.DisplayFreq + Rit_Controls.Offset;
            int MHz = freq_plus_rit / 1000000;
            int KHz = (freq_plus_rit - (MHz * 1000000)) / 1000;
            int Hz = freq_plus_rit - (MHz * 1000000) - (KHz * 1000);
            ritfreqtextBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
            Panadapter_Controls.Frequency = oCode.DisplayFreq;
            /*if(Mouse_controls.Silent_Update == true)
            {
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_SPEAKER_MUTE, 1);
                Thread.Sleep(100);
            }*/
            oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, oCode.DisplayFreq);
            /*if (Mouse_controls.Silent_Update == true)
            {
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_SPEAKER_MUTE, 0);
            }*/
            Mouse_controls.Silent_Update = false;
            if (oCode.current_band == oCode.general_band)
            {
                Last_used.GEN.Freq = oCode.DisplayFreq;
            }
        }   
   
        private void Main_VU_label58_Click(object sender, EventArgs e)
        {

        }

        private void Overdriven_label58_Click(object sender, EventArgs e)
        {

        }

        private void Dbm_Display_label38_Click(object sender, EventArgs e)
        {

        }

        private void Auto_Size_checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            switch (Window_controls.Docking_Controls.Last_Sized.Order)
            {
                case 0:
                    break;
                case Window_controls.Docking_Controls.Last_Sized.Spectrum:

                    Window_controls.Docking_Controls.Last_Sized.Order = Window_controls.Docking_Controls.Last_Sized.None;
                    break;
                case Window_controls.Docking_Controls.Last_Sized.Waterfall:

                    Window_controls.Docking_Controls.Last_Sized.Order = Window_controls.Docking_Controls.Last_Sized.None;
                    break;
            }
        }

        /*private void Zip_Restore_button4_Click(object sender, EventArgs e)
        {
            String Initialization_path;
            String path;
            String Zip_path;
            String Zip_file;
            bool restore_failed = false;

            if (oCode.isLoading) return;

            //oCode.Platform = (int)Environment.OSVersion.Platform;
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Initialization_path = (path + "\\multus-sdr-client\\backup");
            Zip_path = AppDomain.CurrentDomain.BaseDirectory;
            Zip_file = (Zip_path + "ziplog.zip");
            if (System.IO.Directory.Exists(Zip_path))
            {
                if (System.IO.File.Exists(Zip_file))
                {
                    if (System.IO.Directory.Exists(Initialization_path))
                    {
                        try
                        {
                            System.IO.Compression.ZipFile.ExtractToDirectory(Zip_file, Initialization_path);
                        }
                        catch (IOException)
                        {
                            MessageBox.Show("Zip File Restore FAILED. Please report to Multus SDR, LLC.", "MSCC",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                            MonitorTextBoxText(" Zip_button2_Click -> Zip File Create FAILED");
                            restore_failed = true;
                            throw;
                        }

                    }
                    if (!restore_failed)
                    {
                        MessageBox.Show("The zip file has been restored of all Initialization and Log Files at: \r\n" +
                            Initialization_path, "MSCC",
                             MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("No zip file to restore. \r\n" +
                                Zip_file, "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            MonitorTextBoxText(" Zip_Restore_button4_Click -> Finished");
        }*/

        private void Microphone_textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Compression_button4_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (Volume_Controls.Compression_State == 0)
            {
                Volume_Controls.Compression_State = 1;
                Compression_button2.BackColor = Color.Red;
                Compression_button2.ForeColor = Color.White;
                Compression_button2.Text = "Compression ON";
                Compression_button4.BackColor = Color.Red;
                Compression_button4.ForeColor = Color.White;
            }
            else
            {
                Volume_Controls.Compression_State = 0;
                Compression_button2.BackColor = Color.Gainsboro;
                Compression_button2.ForeColor = Color.Black;
                Compression_button2.Text = "Compression OFF";
                Compression_button4.BackColor = Color.Gainsboro;
                Compression_button4.ForeColor = Color.Black;
            }
            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_COMPRESSION_STATE, Volume_Controls.Compression_State);
        }

        private void ACG_button_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            MonitorTextBoxText(" AGC_label16_Click -> AGC_ALC_Notch_Controls.AGC_Level: " + AGC_ALC_Notch_Controls.AGC_Level);
            AGC_ALC_Notch_Controls.AGC_Level++;
            if (AGC_ALC_Notch_Controls.AGC_Level > 2) AGC_ALC_Notch_Controls.AGC_Level = 0;

            switch (AGC_ALC_Notch_Controls.AGC_Level)
            {
                case 0:
                    ACG_button.Text = "SLO";
                    AGC_listBox1.SelectedIndex = 0;
                    break;

                case 1:
                    ACG_button.Text = "MED";
                    AGC_listBox1.SelectedIndex = 1;
                    break;

                case 2:
                    ACG_button.Text = "FST";
                    AGC_listBox1.SelectedIndex = 2;
                    break;
            }
            oCode.SendCommand(txsocket, txtarget, AGC_ALC_Notch_Controls.CMD_GET_SET_AGC, (byte)AGC_ALC_Notch_Controls.AGC_Level);
            MonitorTextBoxText(" AGC_label16_Click -> AGC_ALC_Notch_Controls.AGC_Level: " + AGC_ALC_Notch_Controls.AGC_Level);
        }

        private char Convert_Favs_Mode(string favs_mode)
        {
            char mode = 'N';

            if (favs_mode == "USB")
            {
                mode = 'U';
            }
            if (favs_mode == "LSB")
            {
                mode = 'L';
            }
            if (favs_mode == "AM")
            {
                mode = 'A';
            }
            if (favs_mode == "CW")
            {
                mode = 'C';
            }
            return mode;
        }

        private void B160_Favs_listView1_SelectedIndexChanged_2(object sender, EventArgs e)
        {
            string fav_freq;
            string fav_mode;
            string fav_name;
            Int32 frequency;
            char mode;

            if (B160_Favs_listView1.SelectedItems.Count != 0)
            {
                fav_name = B160_Favs_listView1.SelectedItems[0].SubItems[0].Text;
                fav_freq = B160_Favs_listView1.SelectedItems[0].SubItems[1].Text;
                fav_mode = B160_Favs_listView1.SelectedItems[0].SubItems[2].Text;
                MonitorTextBoxText(" B160_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode);
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B160.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B160.Mode = mode;
                oCode.previous_main_band = 0;
                main160radioButton10.Checked = true;
                main160radioButton10_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
                oCode.DisplayFreq = frequency;
                Display_Main_Freq();
                B160_Favs_listView1.SelectedIndices.Clear();
            }
        }

        private void B80_Favs_listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fav_freq;
            string fav_mode;
            string fav_name;
            Int32 frequency;
            char mode;

            if (B80_Favs_listView1.SelectedItems.Count != 0)
            {
                fav_name = B80_Favs_listView1.SelectedItems[0].SubItems[0].Text;
                fav_freq = B80_Favs_listView1.SelectedItems[0].SubItems[1].Text;
                fav_mode = B80_Favs_listView1.SelectedItems[0].SubItems[2].Text;
                MonitorTextBoxText(" B80_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode);
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B80.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B80.Mode = mode;
                oCode.previous_main_band = 0;
                main80radioButton9.Checked = true;
                main80radioButton9_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
                oCode.DisplayFreq = frequency;
                Display_Main_Freq();
                B80_Favs_listView1.SelectedIndices.Clear();
            }
        }

        private void B60_Favs_listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fav_freq;
            string fav_mode;
            string fav_name;
            Int32 frequency;
            char mode;

            if (B60_Favs_listView1.SelectedItems.Count != 0)
            {
                fav_name = B60_Favs_listView1.SelectedItems[0].SubItems[0].Text;
                fav_freq = B60_Favs_listView1.SelectedItems[0].SubItems[1].Text;
                fav_mode = B60_Favs_listView1.SelectedItems[0].SubItems[2].Text;
                MonitorTextBoxText(" B60_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode);
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B60.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B60.Mode = mode;
                oCode.previous_main_band = 0;
                main60radioButton8.Checked = true;
                main60radioButton8_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
                oCode.DisplayFreq = frequency;
                Display_Main_Freq();
                B60_Favs_listView1.SelectedIndices.Clear();
            }
        }

        private void B40_Favs_listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fav_freq;
            string fav_mode;
            string fav_name;
            Int32 frequency;
            char mode;

            if (B40_Favs_listView1.SelectedItems.Count != 0)
            {
                fav_name = B40_Favs_listView1.SelectedItems[0].SubItems[0].Text;
                fav_freq = B40_Favs_listView1.SelectedItems[0].SubItems[1].Text;
                fav_mode = B40_Favs_listView1.SelectedItems[0].SubItems[2].Text;
                MonitorTextBoxText(" B40_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode);
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B40.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B40.Mode = mode;
                oCode.previous_main_band = 0;
                main40radioButton7.Checked = true;
                main40radioButton7_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
                oCode.DisplayFreq = frequency;
                Display_Main_Freq();
                B40_Favs_listView1.SelectedIndices.Clear();
            }
        }

        private void B30_Favs_listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fav_freq;
            string fav_mode;
            string fav_name;
            Int32 frequency;
            char mode;

            if (B30_Favs_listView1.SelectedItems.Count != 0)
            {
                fav_name = B30_Favs_listView1.SelectedItems[0].SubItems[0].Text;
                fav_freq = B30_Favs_listView1.SelectedItems[0].SubItems[1].Text;
                fav_mode = B30_Favs_listView1.SelectedItems[0].SubItems[2].Text;
                MonitorTextBoxText(" B30_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode);
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B30.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B30.Mode = mode;
                oCode.previous_main_band = 0;
                main30radioButton6.Checked = true;
                main30radioButton6_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
                oCode.DisplayFreq = frequency;
                Display_Main_Freq();
                B30_Favs_listView1.SelectedIndices.Clear();
            }
        }

        private void B20_Favs_listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fav_freq;
            string fav_mode;
            string fav_name;
            //string fav_filter;
            //string cw_filter;
            Int32 frequency;
            char mode;

            if (B20_Favs_listView1.SelectedItems.Count != 0)
            {
                fav_name = B20_Favs_listView1.SelectedItems[0].SubItems[0].Text;
                fav_freq = B20_Favs_listView1.SelectedItems[0].SubItems[1].Text;
                fav_mode = B20_Favs_listView1.SelectedItems[0].SubItems[2].Text;
                MonitorTextBoxText(" B20_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode);
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B20.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B20.Mode = mode;
                oCode.DisplayFreq = frequency;
                Display_Main_Freq();
                oCode.previous_main_band = 0;
                main20radioButton5.Checked = true;
                main20radioButton5_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
                oCode.DisplayFreq = frequency;
                Display_Main_Freq();
                B20_Favs_listView1.SelectedIndices.Clear();
            }
        }

        private void B17_Favs_listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fav_freq;
            string fav_mode;
            string fav_name;
            Int32 frequency;
            char mode;

            if (B17_Favs_listView1.SelectedItems.Count != 0)
            {
                fav_name = B17_Favs_listView1.SelectedItems[0].SubItems[0].Text;
                fav_freq = B17_Favs_listView1.SelectedItems[0].SubItems[1].Text;
                fav_mode = B17_Favs_listView1.SelectedItems[0].SubItems[2].Text;
                MonitorTextBoxText(" B17_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode);
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B17.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B17.Mode = mode;
                oCode.previous_main_band = 0;
                main17radioButton4.Checked = true;
                main17radioButton4_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
                oCode.DisplayFreq = frequency;
                Display_Main_Freq();
                B17_Favs_listView1.SelectedIndices.Clear();
            }
        }

        private void B15_Favs_listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fav_freq;
            string fav_mode;
            string fav_name;
            Int32 frequency;
            char mode;

            if (B15_Favs_listView1.SelectedItems.Count != 0)
            {
                fav_name = B15_Favs_listView1.SelectedItems[0].SubItems[0].Text;
                fav_freq = B15_Favs_listView1.SelectedItems[0].SubItems[1].Text;
                fav_mode = B15_Favs_listView1.SelectedItems[0].SubItems[2].Text;
                MonitorTextBoxText(" B15_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode);
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B15.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B15.Mode = mode;
                oCode.previous_main_band = 0;
                main15radiobutton.Checked = true;
                main15radiobutton_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
                oCode.DisplayFreq = frequency;
                Display_Main_Freq();
                B15_Favs_listView1.SelectedIndices.Clear();
            }
        }

        private void B12_Favs_listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fav_freq;
            string fav_mode;
            string fav_name;
            Int32 frequency;
            char mode;

            if (B12_Favs_listView1.SelectedItems.Count != 0)
            {
                fav_name = B12_Favs_listView1.SelectedItems[0].SubItems[0].Text;
                fav_freq = B12_Favs_listView1.SelectedItems[0].SubItems[1].Text;
                fav_mode = B12_Favs_listView1.SelectedItems[0].SubItems[2].Text;
                MonitorTextBoxText(" B12_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode);
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B12.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B12.Mode = mode;
                oCode.previous_main_band = 0;
                main12radioButton2.Checked = true;
                main12radioButton2_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
                oCode.DisplayFreq = frequency;
                Display_Main_Freq();
                B12_Favs_listView1.SelectedIndices.Clear();
            }
        }

        private void B10_Favs_listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fav_freq;
            string fav_mode;
            string fav_name;
            Int32 frequency;
            char mode;

            if (B10_Favs_listView1.SelectedItems.Count != 0)
            {
                fav_name = B10_Favs_listView1.SelectedItems[0].SubItems[0].Text;
                fav_freq = B10_Favs_listView1.SelectedItems[0].SubItems[1].Text;
                fav_mode = B10_Favs_listView1.SelectedItems[0].SubItems[2].Text;
                MonitorTextBoxText(" B10_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode);
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B10.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B10.Mode = mode;
                oCode.previous_main_band = 0;
                main10radioButton1.Checked = true;
                main10radioButton1_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
                oCode.DisplayFreq = frequency;
                Display_Main_Freq();
                B10_Favs_listView1.SelectedIndices.Clear();
            }
        }

        public bool Load_Favorites(String Band, int band_number)
        {
            String path, line;
            System.IO.StreamReader file;
            String[] arr = new string[6];
            ListViewItem itm;
            String temp_string;
            int param_pos;
            int end_pos;
            String param;

            //oCode.Platform = (int)Environment.OSVersion.Platform;
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#if RPI
            path += "/mscc/" + Band;
#else
            path += "\\multus-sdr-client\\" + Band;
#endif

            try
            {
                file = new System.IO.StreamReader(File.OpenRead(path));
            }
            catch (IOException e)
            {
                string er = e.Message;
                DialogResult ret = MessageBox.Show("IO Exception opening: " + Band + "\r\n Error: " + er + "\r\nMake note of the error and contact Multus SDR,LLC.",
                    "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            while ((line = file.ReadLine()) != null)
            {
                if (line.Length != 0)
                {
                    param_pos = line.IndexOf("NAME");                       // get the position of the NAME parameter
                    temp_string = line.Substring((param_pos + 5), (line.Length - param_pos - 5));   // parse everything between the end of NAME= and the end of the line.
                    end_pos = temp_string.IndexOf(","); //temp string will start with the NAME parameter value. Find the trailing comma
                    arr[0] = temp_string.Substring(0, end_pos);   // temp string will start with the NAME parameter value. 

                    param_pos = line.IndexOf("FREQ");                       // get the position of the FREQ parameter
                    temp_string = line.Substring((param_pos + 5), (line.Length - param_pos - 5));   // parse everything between the end of FREQ= and the end of the line.
                    end_pos = temp_string.IndexOf(","); //temp string will start with the FREQ parameter value. Find the trailing comma
                    arr[1] = temp_string.Substring(0, end_pos);   // temp string will start with the FREQ parameter value. 

                    param_pos = line.IndexOf("MODE");  // get the position of the MODE parameter
                    param = "MODE=";
                    param_pos = param_pos + param.Length;
                    temp_string = line.Substring(param_pos, line.Length - param_pos);
                    end_pos = temp_string.IndexOf(","); //temp string will start with the MODE parameter value. Find the trailing comma
                    arr[2] = temp_string.Substring(0, end_pos);
                    switch (band_number)
                    {
                        case 0:
                            itm = new ListViewItem(arr);
                            B10_Favs_listView1.Items.Add(itm);
                            Favorites_Controls.Count.B10++;
                            break;
                        case 1:
                            itm = new ListViewItem(arr);
                            B12_Favs_listView1.Items.Add(itm);
                            Favorites_Controls.Count.B12++;
                            break;
                        case 2:
                            itm = new ListViewItem(arr);
                            B15_Favs_listView1.Items.Add(itm);
                            Favorites_Controls.Count.B15++;
                            break;
                        case 3:
                            itm = new ListViewItem(arr);
                            B17_Favs_listView1.Items.Add(itm);
                            Favorites_Controls.Count.B17++;
                            break;
                        case 4:
                            itm = new ListViewItem(arr);
                            B20_Favs_listView1.Items.Add(itm);
                            Favorites_Controls.Count.B20++;
                            break;
                        case 5:
                            itm = new ListViewItem(arr);
                            B30_Favs_listView1.Items.Add(itm);
                            Favorites_Controls.Count.B30++;
                            break;
                        case 6:
                            itm = new ListViewItem(arr);
                            B40_Favs_listView1.Items.Add(itm);
                            Favorites_Controls.Count.B40++;
                            break;
                        case 7:
                            itm = new ListViewItem(arr);
                            B60_Favs_listView1.Items.Add(itm);
                            Favorites_Controls.Count.B60++;
                            break;
                        case 8:
                            itm = new ListViewItem(arr);
                            B80_Favs_listView1.Items.Add(itm);
                            Favorites_Controls.Count.B80++;
                            break;
                        case 9:
                            itm = new ListViewItem(arr);
                            B160_Favs_listView1.Items.Add(itm);
                            Favorites_Controls.Count.B160++;
                            break;
                    }
                }
            }
            file.Close();
            return true;
        }

        public void Initialize_Favorites()
        {
            String band = "b160_favs.ini";
            int band_number = 0;
            bool status = true;

#if !RPI
            Get_FTP_Init_File();
#endif
            B160_Favs_listView1.Clear();
            B80_Favs_listView1.Clear();
            B60_Favs_listView1.Clear();
            B40_Favs_listView1.Clear();
            B30_Favs_listView1.Clear();
            B20_Favs_listView1.Clear();
            B17_Favs_listView1.Clear();
            B15_Favs_listView1.Clear();
            B12_Favs_listView1.Clear();
            B10_Favs_listView1.Clear();

            Initialize_columns();
            band = "b10_favs.ini";
            status = Load_Favorites(band, band_number++);
            band = "b12_favs.ini";
            status = Load_Favorites(band, band_number++);
            band = "b15_favs.ini";
            status = Load_Favorites(band, band_number++);
            band = "b17_favs.ini";
            status = Load_Favorites(band, band_number++);
            band = "b20_favs.ini";
            status = Load_Favorites(band, band_number++);
            band = "b30_favs.ini";
            status = Load_Favorites(band, band_number++);
            band = "b40_favs.ini";
            status = Load_Favorites(band, band_number++);
            band = "b60_favs.ini";
            status = Load_Favorites(band, band_number++);
            band = "b80_favs.ini";
            status = Load_Favorites(band, band_number++);
            band = "b160_favs.ini";
            status = Load_Favorites(band, band_number++);
            if (status == true)
            {
                Favorites_Controls.B10_Count = B10_Favs_listView1.Items.Count;
                Favorites_Controls.B12_Count = B12_Favs_listView1.Items.Count;
                Favorites_Controls.B15_Count = B15_Favs_listView1.Items.Count;
                Favorites_Controls.B17_Count = B17_Favs_listView1.Items.Count;
                Favorites_Controls.B20_Count = B20_Favs_listView1.Items.Count;
                Favorites_Controls.B30_Count = B30_Favs_listView1.Items.Count;
                Favorites_Controls.B40_Count = B40_Favs_listView1.Items.Count;
                Favorites_Controls.B60_Count = B60_Favs_listView1.Items.Count;
                Favorites_Controls.B80_Count = B80_Favs_listView1.Items.Count;
                Favorites_Controls.B160_Count = B160_Favs_listView1.Items.Count;
            }
            else
            {
                Application.Exit();
            }
        }

        private void Initialize_columns()
        {
            B160_Favs_listView1.View = View.Details;
            B160_Favs_listView1.GridLines = true;
            B160_Favs_listView1.FullRowSelect = true;
            B160_Favs_listView1.Columns.Add("Name", 100);
            B160_Favs_listView1.Columns.Add("Freq", 100);
            B160_Favs_listView1.Columns.Add("Mode", 50);

            B80_Favs_listView1.View = View.Details;
            B80_Favs_listView1.GridLines = true;
            B80_Favs_listView1.FullRowSelect = true;
            B80_Favs_listView1.Columns.Add("Name", 100);
            B80_Favs_listView1.Columns.Add("Freq", 100);
            B80_Favs_listView1.Columns.Add("Mode", 50);

            B60_Favs_listView1.View = View.Details;
            B60_Favs_listView1.GridLines = true;
            B60_Favs_listView1.FullRowSelect = true;
            B60_Favs_listView1.Columns.Add("Name", 100);
            B60_Favs_listView1.Columns.Add("Freq", 100);
            B60_Favs_listView1.Columns.Add("Mode", 50);

            B40_Favs_listView1.View = View.Details;
            B40_Favs_listView1.GridLines = true;
            B40_Favs_listView1.FullRowSelect = true;
            B40_Favs_listView1.Columns.Add("Name", 100);
            B40_Favs_listView1.Columns.Add("Freq", 100);
            B40_Favs_listView1.Columns.Add("Mode", 50);

            B30_Favs_listView1.View = View.Details;
            B30_Favs_listView1.GridLines = true;
            B30_Favs_listView1.FullRowSelect = true;
            B30_Favs_listView1.Columns.Add("Name", 100);
            B30_Favs_listView1.Columns.Add("Freq", 100);
            B30_Favs_listView1.Columns.Add("Mode", 50);

            B20_Favs_listView1.View = View.Details;
            B20_Favs_listView1.GridLines = true;
            B20_Favs_listView1.FullRowSelect = true;
            B20_Favs_listView1.Columns.Add("Name", 100);
            B20_Favs_listView1.Columns.Add("Freq", 100);
            B20_Favs_listView1.Columns.Add("Mode", 50);

            B17_Favs_listView1.View = View.Details;
            B17_Favs_listView1.GridLines = true;
            B17_Favs_listView1.FullRowSelect = true;
            B17_Favs_listView1.Columns.Add("Name", 100);
            B17_Favs_listView1.Columns.Add("Freq", 100);
            B17_Favs_listView1.Columns.Add("Mode", 50);

            B15_Favs_listView1.View = View.Details;
            B15_Favs_listView1.GridLines = true;
            B15_Favs_listView1.FullRowSelect = true;
            B15_Favs_listView1.Columns.Add("Name", 100);
            B15_Favs_listView1.Columns.Add("Freq", 100);
            B15_Favs_listView1.Columns.Add("Mode", 50);

            B12_Favs_listView1.View = View.Details;
            B12_Favs_listView1.GridLines = true;
            B12_Favs_listView1.FullRowSelect = true;
            B12_Favs_listView1.Columns.Add("Name", 100);
            B12_Favs_listView1.Columns.Add("Freq", 100);
            B12_Favs_listView1.Columns.Add("Mode", 50);

            B10_Favs_listView1.View = View.Details;
            B10_Favs_listView1.GridLines = true;
            B10_Favs_listView1.FullRowSelect = true;
            B10_Favs_listView1.Columns.Add("Name", 100);
            B10_Favs_listView1.Columns.Add("Freq", 100);
            B10_Favs_listView1.Columns.Add("Mode", 50);

        }

        private void Set_mode_display(char mode)
        {
            Last_used.Current_mode = mode;
            MonitorTextBoxText(" Set_mode_display called. Last_used.Current_mode: " + Last_used.Current_mode);
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
            MonitorTextBoxText(" Set_mode_display called -> Finished");
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
                MonitorTextBoxText(" m160radiobutton called ");
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
                MonitorTextBoxText(" m160radiobutton -> " + "Current Mode: " + Last_used.B160.Mode);
                MonitorTextBoxText(" m160radiobutton Finished ");
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
                MonitorTextBoxText(" m80radiobutton called ");
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
                MonitorTextBoxText(" m80radiobutton -> " + "Current Mode: " + Last_used.B80.Mode);
                MonitorTextBoxText(" m80radiobutton Finished ");
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
                MonitorTextBoxText(" m60radiobutton called ");
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
                MonitorTextBoxText(" m60radiobutton -> " + "Current Mode: " + Last_used.B60.Mode);
                MonitorTextBoxText(" m60radiobutton Finished ");

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
                MonitorTextBoxText(" m40radiobutton called ");
                oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_LAST_USED_FREQ, oCode.previous_main_band);
                Set_mode_display(Last_used.B40.Mode);
                oCode.gen_band_active = false;
                oCode.previous_main_band = 40;
                oCode.DisplayFreq = Last_used.B40.Freq;
                Display_Main_Freq();
                oCode.current_band = 40;
                int freq_plus_rit = Last_used.B40.Freq + Rit_Controls.Offset;
                Rit_Controls.Rit_Freq = Last_used.B40.Freq;
                MonitorTextBoxText(" m40radiobutton called -> Rit_Controls.Rit_Freq: " +
                    Convert.ToString(Rit_Controls.Rit_Freq));
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
                MonitorTextBoxText(" m30radiobutton called ");
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
                MonitorTextBoxText(" m20radiobutton called ");
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
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, mode_number);
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
                MonitorTextBoxText(" m17radiobutton called ");
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
                MonitorTextBoxText(" m15radiobutton called ");
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
                MonitorTextBoxText(" m12radiobutton called ");
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
                MonitorTextBoxText(" m10radiobutton called ");
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
                MonitorTextBoxText(" genradiobutton called ");
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
                MonitorTextBoxText(" genradiobutton -> " + "Current Mode: " + Last_used.GEN.Mode);
                MonitorTextBoxText(" genradiobutton Finished ");
            }
        }

        short band = 180;
        int bias = 0;
        int temperature = 0;
        bool firmware_version_major = false;
        bool mssdr_version_major = false;
        bool sdrcore_recv_version_major = false;
        bool sdrcore_trans_version_major = false;
        bool sdrcore_recv_verion_updated = false;
        bool sdrcore_trans_version_updated = false;
        const int BIAS_CENTER_TEMPERATURE = 40;
        const float BIAS_SCALING = 37.5f;
        int G_Potentia_Temparature_Average = 0;
        long time_now = 0;
        long previous_time = 0;
        int previous_refresh = 0;
        long receive_time = 0;
        const int index_limit = Panadapter_Controls.Data_recieve_limit;
        long[] Data_receive_array = new long[index_limit];

#if FTDI
        //byte transceiver_display = 0;
#endif
        public void Set_Band_By_Server(byte band)
        {
            MonitorTextBoxText(" Button_Set_Band  VALUE : " + band);
            switch (band)
            {
                case 160:
                    main160radioButton10.Checked = true;
                    main160radioButton10_CheckedChanged(null, null);
                    break;

                case 80:
                    main80radioButton9.Checked = true;
                    main80radioButton9_CheckedChanged(null, null);
                    break;

                case 60:
                    main60radioButton8.Checked = true;
                    main60radioButton8_CheckedChanged(null, null);
                    break;

                case 40:
                    main40radioButton7.Checked = true;
                    main40radioButton7_CheckedChanged(null, null);
                    break;

                case 30:
                    main30radioButton6.Checked = true;
                    main30radioButton6_CheckedChanged(null, null);
                    break;

                case 20:
                    main20radioButton5.Checked = true;
                    main20radioButton5_CheckedChanged(null, null);
                    break;

                case 17:
                    main17radioButton4.Checked = true;
                    main17radioButton4_CheckedChanged(null, null);
                    break;

                case 15:
                    main15radiobutton.Checked = true;
                    main15radiobutton_CheckedChanged(null, null);
                    break;

                case 12:
                    main12radioButton2.Checked = true;
                    main12radioButton2_CheckedChanged(null, null);
                    break;

                case 10:
                    main10radioButton1.Checked = true;
                    main10radioButton1_CheckedChanged(null, null);
                    break;

                case 200:
                    genradioButton.Checked = true;
                    break;
            }
        }

        public void Process_extended_commands(ref byte[] extended_packet, int read_size)
        {
            byte op_code = 0;
            int[] value = new int[2];
            byte operand = 0;
            float swr = 0.0f;
            string swr_string;
            int lower = 0;
            int upper = 0;
            //string[] message = new string[80];
            string message_1;
            uint meter_power;
            byte which_switch = 0;
            byte star = 0;
            string Stop_Message = " \r\nMSCC will now STOP";
            string Message;

            op_code = extended_packet[1];
            operand = extended_packet[2];
            //MonitorTextBoxText(" Process_extended_commands-> Read Size: " + Convert.ToString(read_size));
            switch (op_code)
            {
                case Master_Controls.Extended_Commands.CMD_SET_SOLIDUS_STATUS:
                    MonitorTextBoxText(" Process_extended_commands-> CMD_SET_SOLIDUS_STATUS -> Value: " +
                        Convert.ToString(operand));
                    switch (operand)
                    {
                        case 0:
                            Solidus_Controls.Solidus_Status = false;
                            break;
                        default:
                            Solidus_Controls.Solidus_Status = true;
                            break;
                    }
                    Solidus_Controls.Solidus_Status_Set = true;
                    Manage_Solidus_Status();
                    break;

                case Master_Controls.Extended_Commands.CMD_SET_GUI_STAR:
                    MonitorTextBoxText(" Process_extended_commands-> CMD_SET_GUI_STAR -> Value: " +
                        Convert.ToString(operand));
                    which_switch = operand;
                    which_switch = (byte)(which_switch & 0xF0);
                    star = (byte)(operand & 0x0F);
                    switch (which_switch)
                    {
                        case Master_Controls.Extended_Commands.Knob_switch_star:
                            switch (star)
                            {
                                case 1:
                                    MFC_Knob_label38.BackColor = Color.Red;
                                    MFC_Knob_label38.ForeColor = Color.White;
                                    break;
                                default:
                                    MFC_Knob_label38.BackColor = Color.Gainsboro;
                                    MFC_Knob_label38.ForeColor = Color.Black;
                                    break;
                            }
                            break;
                        case Master_Controls.Extended_Commands.Button_C_switch_star:
                            switch (star)
                            {
                                case 1:
                                    MFC_C_label38.BackColor = Color.Red;
                                    MFC_C_label38.ForeColor = Color.White;
                                    break;
                                default:
                                    MFC_C_label38.BackColor = Color.Gainsboro;
                                    MFC_C_label38.ForeColor = Color.Black;
                                    break;
                            }
                            break;
                        case Master_Controls.Extended_Commands.Button_A_switch_star:
                            switch (star)
                            {
                                case 1:
                                    MFC_A_label38.BackColor = Color.Red;
                                    MFC_A_label38.ForeColor = Color.White;
                                    break;
                                default:
                                    MFC_A_label38.BackColor = Color.Gainsboro;
                                    MFC_A_label38.ForeColor = Color.Black;
                                    break;
                            }
                            break;
                        case Master_Controls.Extended_Commands.Button_B_switch_star:
                            switch (star)
                            {
                                case 1:
                                    MFC_B_label38.BackColor = Color.Red;
                                    MFC_B_label38.ForeColor = Color.White;
                                    break;
                                default:
                                    MFC_B_label38.BackColor = Color.Gainsboro;
                                    MFC_B_label38.ForeColor = Color.Black;
                                    break;
                            }
                            break;
                    }
                    break;

                case Master_Controls.Extended_Commands.CMD_MFC_SET_BAND:
                    //MonitorTextBoxText(" Process_extended_commands-> CMD_MFC_SET_BAND -> Value: " + Convert.ToString(operand));
                    Set_Band_By_Server(operand);
                    break;

                case Master_Controls.Extended_Commands.CMD_SET_SERVER_ERROR:
                    //MonitorTextBoxText(" Process_extended_commands-> CMD_SET_SERVER_ERROR -> Value: " +
                    //                                                                 Convert.ToString(operand));
                    try
                    {
                        message_1 = System.Text.Encoding.ASCII.GetString(extended_packet, 2, (read_size - 2));
                    }
                    catch
                    {
                        MessageBox.Show("GetString", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        message_1 = "GetString Error";
                    }
                    //AutoClosingMessageBox.Show(message_1,"MSCC",20000,MessageBoxButtons.OK);
                    Message = message_1 + Stop_Message;
                    Master_Controls.Shutdown = true;
                    if (Master_Controls.Initialize_network_status == true)
                    {
                        oCode.SendCommand(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, 
                            oCode.CMD_SET_STOP, 1);
                    }
                    MessageBox.Show(Message, "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Thread.Sleep(1000);
                    Application.Exit();
                    break;
#if !RPI
                case Master_Controls.Extended_Commands.CMD_SET_SERVER_MSG:
                    try
                    {
                        message_1 = System.Text.Encoding.ASCII.GetString(extended_packet, 2, (read_size - 2));
                        MonitorTextBoxText(" Process_extended_commands-> CMD_SET_SERVER_MSG -> Value: " + message_1);
                    }
                    catch
                    {
                        MessageBox.Show("GetString", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        message_1 = "GetString Error";
                    }
                    AutoClosingMessageBox.Show(message_1,"MSCC",3000);
                    //MessageBox.Show(message_1, "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case Master_Controls.Extended_Commands.CMD_MFC_SET_FAVS:
                    Set_Favs();
                    break;


                case Master_Controls.Extended_Commands.CMD_SET_WATERFALL_DISPLAY:
                    // MonitorTextBoxText(" Process_extended_commands-> CMD_SET_WATERFALL_DISPLAY -> Value: " +
                    //     Convert.ToString(operand));
                    try
                    {
                        if (operand == 1)
                        {
                            Window_controls.Waterfall_On_By_Server = true;
                            Master_Controls.code_triggered = true;
                            Master_Controls.code_triggered = false;
                            Window_controls.Waterfall_Toggle = true;
                        }
                        else
                        {
                            Window_controls.Waterfall_On_By_Server = false;
                            Master_Controls.code_triggered = true;
                            Master_Controls.code_triggered = false;
                            Window_controls.Waterfall_Toggle = false;
                        }

                    }
                    catch
                    {
                        MonitorTextBoxText(" Process_extended_commands-> FAILED");
                    }
                    break;

                case Master_Controls.Extended_Commands.CMD_SET_WATERFALL_DIRECTION:
                    MonitorTextBoxText(" Process_extended_commands-> CMD_SET_WATERFALL_DIRECTION -> Value: " +
                        Convert.ToString(operand));
                    Window_controls.Waterfall_Controls.Set_by_server.direction = true;
                    if (operand == 1)
                    {
                        Display_GDI.ReverseWaterfall = true;
                        Window_controls.Waterfall_Controls.direction_normal = false;
                    }
                    else
                    {
                        Display_GDI.ReverseWaterfall = false;
                        Window_controls.Waterfall_Controls.direction_normal = true;
                    }
                    break;
                case Master_Controls.Extended_Commands.CMD_SET_WATERFALL_PALET:
                    MonitorTextBoxText(" Process_extended_commands-> CMD_SET_WATERFALL_PALET -> Value: " +
                        Convert.ToString(operand));
                    Window_controls.Waterfall_Controls.Set_by_server.pallet = true;
                    Window_controls.Waterfall_Controls.pallet_index = operand;
                    break;
                case Master_Controls.Extended_Commands.CMD_SET_WATERFALL_GAIN:
                    Window_controls.Waterfall_Controls.Set_by_server.gain = true;
                    Buffer.BlockCopy(extended_packet, 2, value, 0, sizeof(int));
                    Window_controls.Waterfall_Controls.gain = value[0];
                    MonitorTextBoxText(" Process_extended_commands-> CMD_SET_WATERFALL_GAIN -> Value: " +
                       Convert.ToString(value[0]));
                    break;
                case Master_Controls.Extended_Commands.CMD_SET_WATERFALL_ZERO:
                    Window_controls.Waterfall_Controls.Set_by_server.zero = true;
                    Buffer.BlockCopy(extended_packet, 2, value, 0, sizeof(int));
                    Window_controls.Waterfall_Controls.zero = value[0];
                    MonitorTextBoxText(" Extended Command Received -> CMD_SET_WATERFALL_ZERO -> Value: " +
                       Convert.ToString(value[0]));
                    break;
                case Master_Controls.Extended_Commands.CMD_SET_WATERFALL_GRID:
                    MonitorTextBoxText(" Process_extended_commands-> CMD_SET_WATERFALL_GRID -> Value: " +
                        Convert.ToString(operand));
                    Window_controls.Waterfall_Controls.Set_by_server.grid = true;
                    Window_controls.Waterfall_Controls.Time_grid = operand;
                    break;
                case Master_Controls.Extended_Commands.CMD_SET_WATERFALL_SPEED:
                    Window_controls.Waterfall_Controls.Set_by_server.speed = true;
                    Buffer.BlockCopy(extended_packet, 2, value, 0, sizeof(int));
                    Window_controls.Waterfall_Controls.window_speed = value[0];
                    MonitorTextBoxText(" Process_extended_commands-> CMD_SET_WATERFALL_SPEED -> Value: " +
                       Convert.ToString(value[0]));
                    break;
#endif
                case Master_Controls.Extended_Commands.CMD_SET_IQBD_DATA:
                    try
                    {
                        Buffer.BlockCopy(extended_packet, 2, value, 0, sizeof(UInt16));
                        //MonitorTextBoxText(" Process_extended_commands-> CMD_SET_IQBD_DATA -> Value: " +
                        //    Convert.ToString(value[0]));
                        IQBD_Monitor_label.Text = Convert.ToString(value[0]);
                    }
                    catch
                    {
                        MonitorTextBoxText(" Process_extended_commands-> CMD_SET_IQBD_DATA -> FAILED");
                    }
                    break;
                case Master_Controls.Extended_Commands.CMD_SET_SWR:
                    //Buffer.BlockCopy(extended_packet, 2, value, 0, sizeof(int));
                    swr = (float)operand / 10.0f;
                    Smeter_controls.SWR_Value = swr;
                    swr_string = string.Format("{0:N1}", swr);
                    
                    SWR_Value_label43.Text = swr_string;
                    //MonitorTextBoxText(" Process_extended_commands-> CMD_SET_SWR -> Value: " + swr_string);
                    break;
                case Master_Controls.Extended_Commands.CMD_SET_FORWARD_POWER:
                    meter_power = BitConverter.ToUInt32(extended_packet, 2);
                    lower = (int)(meter_power & 0x000000FF);
                    upper = (int)(meter_power >> 10);
                    //MonitorTextBoxText(" Process_extended_commands-> CMD_SET_FORWARD_POWER -> upper: " + Convert.ToString(upper)
                    //            + " lower: " + Convert.ToString(lower)); ;
                    if (upper <= Master_Controls.Power_Meter_Max_Level)
                    {

                        if (Master_Controls.QRP_Mode == true)
                        {
                            int QRP_power = upper * 20;
                            if (QRP_power <= Master_Controls.Power_Meter_Max_Level)
                            {
                                Forward_Meter.Level = QRP_power;
                                Smeter_controls.Power_Value = QRP_power;
                            }
                            else
                            {
                                Forward_Meter.Level = Master_Controls.Power_Meter_Max_Level;
                                Smeter_controls.Power_Value = Master_Controls.Power_Meter_Max_Level;
                            }
                        }
                        else
                        {
                            Forward_Meter.Level = upper;
                            Smeter_controls.Power_Value = upper;
                            //MonitorTextBoxText(" CMD_SET_FORWARD_POWER -> Smeter_controls.Power_Value: " + Convert.ToString(Smeter_controls.Power_Value));
                        }
                    }
                    Forward_Power_label43.Text = Convert.ToString(upper) + "." + lower.ToString("D2");
                    XCRV_Power_Display_label33.Text = "XCVR Power: " + Convert.ToString(upper) + "." + lower.ToString("D2");
                    Solidus_Tab_Power_Display_label33.Text = "PA Power: " + Convert.ToString(upper) + "." + lower.ToString("D2");
                    break;

                case Master_Controls.Extended_Commands.CMD_SET_REVERSE_POWER:
                    meter_power = BitConverter.ToUInt32(extended_packet, 2);
                    lower = (int)(meter_power & 0x000000FF);
                    upper = (int)(meter_power >> 10);
                    if (upper <= Master_Controls.Power_Meter_Max_Level)
                    {
                        if (Master_Controls.QRP_Mode == true)
                        {
                            int QRP_power = upper * 20;
                            if (QRP_power <= Master_Controls.Power_Meter_Max_Level)
                            {
                                Reverse_Meter.Level = QRP_power;
                            }
                            else
                            {
                                Reverse_Meter.Level = Master_Controls.Power_Meter_Max_Level;
                            }
                        }
                        else
                        {
                            Reverse_Meter.Level = upper;
                        }
                    }
                    Reverse_Power_label43.Text = Convert.ToString(upper) + "." + Convert.ToString(lower);
                    //MonitorTextBoxText(" Process_extended_commands-> CMD_SET_REVERSE_POWER -> Value: " + upper);
                    break;

                default:
                    MonitorTextBoxText(" Process_extended_commands-> UNKNOWN opcode: " + Convert.ToString(op_code));
                    break;
            }
        }

        public void Average_receive_time(long delta_time)
        {
            long data_average = 0;
            int index = 0;
            int i = 0;

            if (previous_refresh != Panadapter_Controls.Refresh_Index)
            {
                for (index = 0; index < index_limit; index++)
                {
                    Data_receive_array[index] = delta_time;
                }
                Panadapter_Controls.Data_average = delta_time;
            }
            else
            {
                Data_receive_array[0] = delta_time;
                for (index = 0; index < index_limit; index++)
                {
                    data_average = data_average + Data_receive_array[index];
                }
                Panadapter_Controls.Data_average = data_average / (long)index;
                for (i = (index_limit - 1); i > 0; i--)
                {
                    Data_receive_array[i] = Data_receive_array[(i - 1)];
                }
            }
            /*if (Master_Controls.Debug_Display)
            {
                MonitorTextBoxText(" Average_receive_time. delta_time (milliseconds): " +
                    Convert.ToString(delta_time));
                MonitorTextBoxText(" Average_receive_time. Data_receive_average (milliseconds): " +
                    Convert.ToString(Panadapter_Controls.Data_average));
            }*/
        }

        public int adjust_bias(int bias)
        {
            int scaled_bias = 0;
            int temp_i = 0;
            float bias_temp_low = 0.0f;
            float bias_temp_high = 0.0f;

            //temp_i = G_Potentia_Temparature_Average;
            if (G_Potentia_Temparature_Average > BIAS_CENTER_TEMPERATURE)
            {
                temp_i = G_Potentia_Temparature_Average - BIAS_CENTER_TEMPERATURE;
                bias_temp_low = (float)temp_i * BIAS_SCALING;
                scaled_bias = bias - (int)bias_temp_low;
            }
            else
            {
                if (G_Potentia_Temparature_Average < BIAS_CENTER_TEMPERATURE)
                {
                    temp_i = BIAS_CENTER_TEMPERATURE - G_Potentia_Temparature_Average;
                    bias_temp_high = (float)temp_i * BIAS_SCALING;
                    scaled_bias = bias + (int)bias_temp_high;
                }
                else
                {
                    scaled_bias = bias;
                }
            }

            return scaled_bias;
        }

        public short Set_band(Int32 freq)
        {
            short band = 0;
            if (freq >= 1800000 && freq <= 2000000) band = 160;
            if (freq >= 3500000 && freq <= 4000000) band = 80;
            if (freq >= 5330500 && freq <= 5403500) band = 60;
            if (freq >= 7000000 && freq <= 7300000) band = 40;
            if (freq >= 10100000 && freq <= 10150000) band = 30;
            if (freq >= 14000000 && freq <= 14350000) band = 20;
            if (freq >= 18068000 && freq <= 18168000) band = 17;
            if (freq >= 21000000 && freq <= 21450000) band = 15;
            if (freq >= 24890000 && freq <= 24990000) band = 12;
            if (freq >= 28000000 && freq <= 30000000) band = 10;
            return band;
        }

        public void Apply_Band_Stack()
        {
            int MHz;
            int KHz;
            int Hz;
            //System.EventArgs eventArgs;

            if (Band_Stack_Controls.Band_Stack_Complete != 3) return;
            MonitorTextBoxText(" Apply_Band_Stack -> Called -> Applying Params -> Band: " + Band_Stack_Controls.Band + ",Frequency: " +
                Band_Stack_Controls.Frequency + ",MODE: " + Band_Stack_Controls.Mode + ", Complete " +
                Band_Stack_Controls.Band_Stack_Complete);

            MHz = Band_Stack_Controls.Frequency / 1000000;
            KHz = (Band_Stack_Controls.Frequency - (MHz * 1000000)) / 1000;
            Hz = Band_Stack_Controls.Frequency - (MHz * 1000000) - (KHz * 1000);
            band_stack_textBox1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));
            Filter_control.Band_Stack_Updating = true;

            switch (Band_Stack_Controls.Mode)
            {
                case 'A':
                    textBox1.Text = "AM";
                    break;
                case 'L':
                    textBox1.Text = "LSB";
                    break;
                case 'U':
                    textBox1.Text = "USB";
                    break;
                case 'C':
                    textBox1.Text = "CW";
                    break;
            }


            switch (Band_Stack_Controls.Band)
            {
                case 160:
                    Last_used.B160.Freq = Band_Stack_Controls.Frequency;
                    Last_used.B160.Mode = Band_Stack_Controls.Mode;
                    main160radioButton10.Checked = true;
                    oCode.previous_main_band = 0;
                    main160radioButton10_CheckedChanged(1, null);
                    break;
                case 80:
                    Last_used.B80.Freq = Band_Stack_Controls.Frequency;
                    Last_used.B80.Mode = Band_Stack_Controls.Mode;
                    oCode.previous_main_band = 0;
                    main80radioButton9.Checked = true;
                    main80radioButton9_CheckedChanged(1, null);
                    break;
                case 60:
                    Last_used.B60.Freq = Band_Stack_Controls.Frequency;
                    Last_used.B60.Mode = Band_Stack_Controls.Mode;
                    oCode.previous_main_band = 0;
                    main60radioButton8.Checked = true;
                    main60radioButton8_CheckedChanged(1, null);
                    break;
                case 40:
                    Last_used.B40.Freq = Band_Stack_Controls.Frequency;
                    Last_used.B40.Mode = Band_Stack_Controls.Mode;
                    oCode.previous_main_band = 0;
                    main40radioButton7.Checked = true;
                    main40radioButton7_CheckedChanged(1, null);
                    MonitorTextBoxText(" Apply_Band_Stack -> Band 40 -> Freq: " + Last_used.B40.Freq + ", Mode: " + Last_used.B40.Mode);
                    break;
                case 30:
                    Last_used.B30.Freq = Band_Stack_Controls.Frequency;
                    Last_used.B30.Mode = Band_Stack_Controls.Mode;
                    oCode.previous_main_band = 0;
                    main30radioButton6.Checked = true;
                    main30radioButton6_CheckedChanged(1, null);
                    break;
                case 20:
                    Last_used.B20.Freq = Band_Stack_Controls.Frequency;
                    Last_used.B20.Mode = Band_Stack_Controls.Mode;
                    oCode.previous_main_band = 0;
                    main20radioButton5.Checked = true;
                    main20radioButton5_CheckedChanged(1, null);
                    break;
                case 17:
                    Last_used.B17.Freq = Band_Stack_Controls.Frequency;
                    Last_used.B17.Mode = Band_Stack_Controls.Mode;
                    oCode.previous_main_band = 0;
                    main17radioButton4.Checked = true;
                    main17radioButton4_CheckedChanged(1, null);
                    break;
                case 15:
                    Last_used.B15.Freq = Band_Stack_Controls.Frequency;
                    Last_used.B15.Mode = Band_Stack_Controls.Mode;
                    oCode.previous_main_band = 0;
                    main15radiobutton.Checked = true;
                    main15radiobutton_CheckedChanged(1, null);
                    break;
                case 12:
                    Last_used.B12.Freq = Band_Stack_Controls.Frequency;
                    Last_used.B12.Mode = Band_Stack_Controls.Mode;
                    oCode.previous_main_band = 0;
                    main12radioButton2.Checked = true;
                    main12radioButton2_CheckedChanged(1, null);
                    break;
                case 10:
                    Last_used.B10.Freq = Band_Stack_Controls.Frequency;
                    Last_used.B10.Mode = Band_Stack_Controls.Mode;
                    oCode.previous_main_band = 0;
                    main10radioButton1.Checked = true;
                    main10radioButton1_CheckedChanged(1, null);
                    break;
                default:
                    Last_used.GEN.Freq = Band_Stack_Controls.Frequency;
                    Last_used.GEN.Mode = Band_Stack_Controls.Mode;
                    oCode.previous_main_band = 0;
                    genradioButton.Checked = true;
                    genradioButton_CheckedChanged(1, null);
                    break;
            }
            Band_Stack_Controls.Band_Stack_Complete = 0;
            Filter_control.Band_Stack_Updating = false;
        }

        private void set_main_mode_display(char mode)
        {
            MonitorTextBoxText(" set_main_mode_display Entered ");
            switch (mode)
            {
                case 'A':
                    mainmodebutton2.Text = "AM";
                    oCode.modeswitch = 0;
                    break;
                case 'L':
                    mainmodebutton2.Text = "LSB";
                    oCode.modeswitch = 1;
                    break;
                case 'U':
                    mainmodebutton2.Text = "USB";
                    oCode.modeswitch = 2;
                    break;
                case 'C':
                    mainmodebutton2.Text = "CW";
                    oCode.modeswitch = 3;
                    break;

            }
            Last_used.Current_mode = mode;
            MonitorTextBoxText(" set_main_mode_display -> New Mode: " + mode + ", Current_band: " + oCode.current_band);
            switch (oCode.current_band)
            {
                case 160:
                    Last_used.B160.Mode = mode;
                    if (mode == 'C')
                    {
                        Filter_listBox1.SetSelected(Last_used.B160.Filter_CW_index, true);
                    }
                    else
                    {
                        Filter_listBox1.SetSelected(Last_used.B160.Filter_High_Index, true);
                    }
                    break;
                case 80:
                    Last_used.B80.Mode = mode;
                    if (mode == 'C')
                    {
                        Filter_listBox1.SetSelected(Last_used.B80.Filter_CW_index, true);
                    }
                    else
                    {
                        Filter_listBox1.SetSelected(Last_used.B80.Filter_High_Index, true);
                    }
                    break;
                case 60:
                    Last_used.B60.Mode = mode;
                    if (mode == 'C')
                    {
                        Filter_listBox1.SetSelected(Last_used.B60.Filter_CW_index, true);
                    }
                    else
                    {
                        Filter_listBox1.SetSelected(Last_used.B60.Filter_High_Index, true);
                    }
                    break;
                case 40:
                    Last_used.B40.Mode = mode;
                    if (mode == 'C')
                    {
                        Filter_listBox1.SetSelected(Last_used.B40.Filter_CW_index, true);
                        MonitorTextBoxText(" set_main_mode_display -> Band 40 -> New Mode: " + mode + ",CW Index: " + Last_used.B40.Filter_CW_index);
                    }
                    else
                    {
                        Filter_listBox1.SetSelected(Last_used.B40.Filter_High_Index, true);
                        MonitorTextBoxText(" set_main_mode_display -> Band 40 -> New Mode: " + mode + ",High Cut Index: " +
                            Last_used.B40.Filter_High_Index);
                    }
                    break;
                case 30:
                    Last_used.B30.Mode = mode;
                    if (mode == 'C')
                    {
                        Filter_listBox1.SetSelected(Last_used.B30.Filter_CW_index, true);
                    }
                    else
                    {
                        Filter_listBox1.SetSelected(Last_used.B30.Filter_High_Index, true);
                    }
                    break;
                case 20:
                    Last_used.B20.Mode = mode;
                    if (mode == 'C')
                    {
                        Filter_listBox1.SetSelected(Last_used.B20.Filter_CW_index, true);
                    }
                    else
                    {
                        Filter_listBox1.SetSelected(Last_used.B20.Filter_High_Index, true);
                    }
                    break;
                case 17:
                    Last_used.B17.Mode = mode;
                    if (mode == 'C')
                    {
                        Filter_listBox1.SetSelected(Last_used.B17.Filter_CW_index, true);
                    }
                    else
                    {
                        Filter_listBox1.SetSelected(Last_used.B17.Filter_High_Index, true);
                    }
                    break;
                case 15:
                    Last_used.B15.Mode = mode;
                    if (mode == 'C')
                    {
                        Filter_listBox1.SetSelected(Last_used.B15.Filter_CW_index, true);
                    }
                    else
                    {
                        Filter_listBox1.SetSelected(Last_used.B15.Filter_High_Index, true);
                    }
                    break;
                case 12:
                    Last_used.B12.Mode = mode;
                    if (mode == 'C')
                    {
                        Filter_listBox1.SetSelected(Last_used.B12.Filter_CW_index, true);
                    }
                    else
                    {
                        Filter_listBox1.SetSelected(Last_used.B12.Filter_High_Index, true);
                    }
                    break;
                case 10:
                    Last_used.B10.Mode = mode;
                    if (mode == 'C')
                    {
                        Filter_listBox1.SetSelected(Last_used.B10.Filter_CW_index, true);
                    }
                    else
                    {
                        Filter_listBox1.SetSelected(Last_used.B10.Filter_High_Index, true);
                    }
                    break;
                default:
                    Last_used.GEN.Mode = mode;
                    break;
            }
            MonitorTextBoxText(" set_main_mode_display finished ");
        }
             
        private void set_default_filter_configuration()
        {

            MonitorTextBoxText(" set_default_filter_configuration -> Called. High Cut: " +
                Convert.ToString(Settings.Default.Hi_Cut_Default) + " Low Cut: " +
                 Convert.ToString(Settings.Default.Lo_Cut_Default) + " CW BW: " +
                 Convert.ToString(Settings.Default.CW_Default));

            Last_used.B160.Filter_High_Index = (short)Settings.Default.Hi_Cut_Default;
            Last_used.B160.Filter_Low_Index = (short)Settings.Default.Lo_Cut_Default;
            Last_used.B160.Filter_CW_index = (short)Settings.Default.CW_Default;

            Last_used.B80.Filter_High_Index = (short)Settings.Default.Hi_Cut_Default;
            Last_used.B80.Filter_Low_Index = (short)Settings.Default.Lo_Cut_Default;
            Last_used.B80.Filter_CW_index = (short)Settings.Default.CW_Default;

            Last_used.B60.Filter_High_Index = (short)Settings.Default.Hi_Cut_Default;
            Last_used.B60.Filter_Low_Index = (short)Settings.Default.Lo_Cut_Default;
            Last_used.B60.Filter_CW_index = (short)Settings.Default.CW_Default;

            Last_used.B40.Filter_High_Index = (short)Settings.Default.Hi_Cut_Default;
            Last_used.B40.Filter_Low_Index = (short)Settings.Default.Lo_Cut_Default;
            Last_used.B40.Filter_CW_index = (short)Settings.Default.CW_Default;

            Last_used.B30.Filter_High_Index = (short)Settings.Default.Hi_Cut_Default;
            Last_used.B30.Filter_Low_Index = (short)Settings.Default.Lo_Cut_Default;
            Last_used.B30.Filter_CW_index = (short)Settings.Default.CW_Default;

            Last_used.B20.Filter_High_Index = (short)Settings.Default.Hi_Cut_Default;
            Last_used.B20.Filter_Low_Index = (short)Settings.Default.Lo_Cut_Default;
            Last_used.B20.Filter_CW_index = (short)Settings.Default.CW_Default;

            Last_used.B17.Filter_High_Index = (short)Settings.Default.Hi_Cut_Default;
            Last_used.B17.Filter_Low_Index = (short)Settings.Default.Lo_Cut_Default;
            Last_used.B17.Filter_CW_index = (short)Settings.Default.CW_Default;

            Last_used.B15.Filter_High_Index = (short)Settings.Default.Hi_Cut_Default;
            Last_used.B15.Filter_Low_Index = (short)Settings.Default.Lo_Cut_Default;
            Last_used.B15.Filter_CW_index = (short)Settings.Default.CW_Default;

            Last_used.B12.Filter_High_Index = (short)Settings.Default.Hi_Cut_Default;
            Last_used.B12.Filter_Low_Index = (short)Settings.Default.Lo_Cut_Default;
            Last_used.B12.Filter_CW_index = (short)Settings.Default.CW_Default;

            Last_used.B10.Filter_High_Index = (short)Settings.Default.Hi_Cut_Default;
            Last_used.B10.Filter_Low_Index = (short)Settings.Default.Lo_Cut_Default;
            Last_used.B10.Filter_CW_index = (short)Settings.Default.CW_Default;

            Last_used.GEN.Filter_High_Index = (short)Settings.Default.Hi_Cut_Default;
            Last_used.GEN.Filter_Low_Index = (short)Settings.Default.Lo_Cut_Default;
            Last_used.GEN.Filter_CW_index = (short)Settings.Default.CW_Default;
        }

        //THIS RECEIVES DATA FROM MS-SDR
        public void OnUdpData(IAsyncResult result)
        {
            int op_code;
            int operand;
            Int32 Freq;
            Int32 Delta;
            int read_size = 0;
            byte[] buf = new byte[2];
            int waterfall_limit = zed_size.Width;
            char mode = 'N';

            Master_Controls.Network_Receive_Busy = true;
            UdpClient socket = result.AsyncState as UdpClient;
            socket = result.AsyncState as UdpClient;
            // get the actual message and fill out the source:
            byte[] message = new byte[1024];
            message = socket.EndReceive(result, ref Udp_data.rxtarget);
            read_size = message.Length;

            //if (oCode.display_heartbeat) MonitorTextBoxText(  " Rx: " + "0x" + message[0].ToString("X2") + " " +
            //                                                                            BitConverter.ToInt32(message, 1) );
            op_code = message[0];
            operand = message[1];
            //MonitorTextBoxText(" Rx: " + "0x" + message[0].ToString("X2"));
            //MonitorTextBoxText( " Rx: Received Value: " + message[1].ToString("X4"));

            switch (op_code)
            {
                case oCode.CMD_SET_MAIN_MODE:
                    MonitorTextBoxText(" CMD_SET_MAIN_MODE: " + Convert.ToString((char)operand));
                    switch ((char)operand)
                    {
                        case 'A':
                            oCode.modeswitch = 0;
                            mainmodebutton2.Text = "AM";
                            mode = 'A';
                            Volume_Controls.Volume_Slider_Mode = Volume_Controls.SLIDER_AM_MODE;
                            break;
                        case 'L':
                            oCode.modeswitch = 1;
                            mainmodebutton2.Text = "LSB";
                            mode = 'L';
                            Volume_Controls.Volume_Slider_Mode = Volume_Controls.SLIDER_SSB_MODE;
                            break;
                        case 'U':
                            oCode.modeswitch = 2;
                            mainmodebutton2.Text = "USB";
                            mode = 'U';
                            Volume_Controls.Volume_Slider_Mode = Volume_Controls.SLIDER_SSB_MODE;
                            break;
                        case 'C':
                            oCode.modeswitch = 3;
                            mainmodebutton2.Text = "CW";
                            mode = 'C';
                            Volume_Controls.Volume_Slider_Mode = Volume_Controls.SLIDER_CW_MODE;
                            break;
                    }
                    Last_used.Current_mode = mode;
                    switch (oCode.current_band)
                    {
                        case 160:
                            Last_used.B160.Mode = mode;
                            break;
                        case 80:
                            Last_used.B80.Mode = mode;
                            break;
                        case 60:
                            Last_used.B60.Mode = mode;
                            break;
                        case 40:
                            Last_used.B40.Mode = mode;
                            break;
                        case 30:
                            Last_used.B30.Mode = mode;
                            break;
                        case 20:
                            Last_used.B20.Mode = mode;
                            break;
                        case 17:
                            Last_used.B17.Mode = mode;
                            break;
                        case 15:
                            Last_used.B15.Mode = mode;
                            break;
                        case 12:
                            Last_used.B12.Mode = mode;
                            break;
                        case 10:
                            Last_used.B10.Mode = mode;
                            break;
                        default:
                            Last_used.GEN.Mode = mode;
                            break;
                    }
                    Last_used.Current_mode = mode;
                    break;

                case Master_Controls.CMD_SEND_GUI_STATUS:
                    Master_Controls.GUI_check_status = true;
                    MonitorTextBoxText(" OnUdpData -> CMD_SEND_GUI_STATUS Received");
                    break;

                case Tuning_Knob_Controls.CMD_SET_STEP_VALUE:
                    oCode.Freq_Tune_Index = (short)operand;
#if RPI
                        RPi_Settings.Controls.Freq_Step = operand;
#else
                        mainlistBox1.SelectedIndex = operand;
#endif
                    oCode.FreqDigit = (short)Swap_tuning_step(operand);
                    Set_Freq_Digit_Pointer();
                    break;

                case oCode.CMD_SET_KEEP_ALIVE:
                    Master_Controls.Keep_Alive = true;
                    break;

                case AGC_ALC_Notch_Controls.CMD_SET_ALC:

                    if (Master_Controls.Transmit_Mode)
                    {
                        Main_Smeter_controls.VU_Meter_value = BitConverter.ToInt32(message, 1);
                        if (Master_Controls.Debug_Display)
                        {
                            MonitorTextBoxText(" OnUdpData -> CMD_SET_ALC RECIEVED(1). VALUE : " +
                                                                    Convert.ToString(Main_Smeter_controls.VU_Meter_value));
                        }
                        if (Master_Controls.Debug_Display)
                        {
                            MonitorTextBoxText(" OnUdpData -> CMD_SET_ALC RECIEVED(2). VALUE : " +
                               Convert.ToString(BitConverter.ToInt32(message, 1)));
                        }
                        //Forward_Meter.Level = BitConverter.ToInt32(message, 1);
                        Update_ALC_Meter(BitConverter.ToInt32(message, 1));
                    }
                    break;

                case Audio_Device_Controls.CMD_SET_MICROPHONE_STATUS:
                    if (operand == 1)
                    {
                        Master_Controls.code_triggered = true;
                        //MIC_Status_checkBox2.Checked = true;
                        Master_Controls.code_triggered = false;
                        Audio_Device_Controls.Microphone_Status = true;
                        //MIC_Status_checkBox2.Text = "Microphone ON";
                    }
                    else
                    {
                        Master_Controls.code_triggered = true;
                        //MIC_Status_checkBox2.Checked = false;
                        Master_Controls.code_triggered = false;
                        Audio_Device_Controls.Microphone_Status = false;
                        //MIC_Status_checkBox2.Text = "Microphone OFF";
                    }
                    break;

                case Amplifier_Power_Controls.CMD_SET_POTENTIA_CALIBRATION:
                    Amplifier_Power_Controls.Calibration_Value = BitConverter.ToInt16(message, 1);
                    AMP_Calibrate_hScrollBar1.Enabled = true;
                    if (Amplifier_Power_Controls.Calibration_Value >= -99 && Amplifier_Power_Controls.Calibration_Value <= 4)
                    {
                        AMP_Calibrate_hScrollBar1.Value = Amplifier_Power_Controls.Calibration_Value;
                    }
                    Power_calibration_label58.Text = Convert.ToString(Amplifier_Power_Controls.Calibration_Value);
                    MonitorTextBoxText(" OnUdpData -> CMD_SET_POTENTIA_CALIBRATION Called. Calibration Value: " +
                        Convert.ToString(BitConverter.ToInt16(message, 1)));
                    break;

                case Amplifier_Power_Controls.CMD_GET_POTENTIA_TEMPERATURE:
                    temperature = operand;
                    G_Potentia_Temparature_Average = operand;
                    Amplifier_temperature_label58.Text = "PA Temperature: " + Convert.ToString(operand) + "°C";
                    AMP_Temp_MFC_AMP_label5.Text = "PA Temperature: " + Convert.ToString(operand) + "°C";
                    break;

                case Amplifier_Power_Controls.CMD_GET_AMP_POWER:
                    Master_Controls.Power_Bar_Value = operand;
                    if (Master_Controls.Power_Bar_Value >= 0 && Master_Controls.Power_Bar_Value <= 100)
                    {
                        //Power_progressBar1.Value = Master_Controls.Power_Bar_Value;
                        //Power_Meter.Level = Master_Controls.Power_Bar_Value;
                    }
                    break;

                case Window_controls.CMD_SET_PANADAPTER_DISPLAY:
                    if (operand == 1)
                    {
                        Window_controls.Panadapter_On_By_Server = true;
                        Master_Controls.code_triggered = true;
                        Master_Controls.code_triggered = false;
                        Window_controls.Panadapter_Toggle = true;
                    }
                    else
                    {
                        Window_controls.Panadapter_On_By_Server = false;
                        Master_Controls.code_triggered = true;
                        Master_Controls.code_triggered = false;
                        Window_controls.Panadapter_Toggle = false;
                    }
                    break;

                case Amplifier_Power_Controls.CMD_GET_POTENTIA_BIAS:
                    bias = BitConverter.ToInt16(message, 1);
                    //MonitorTextBoxText(" OnUdpData -> CMD_GET_POTENTIA_BIAS Received. Amplifier_Power_Controls.Bias_On: " +
                    //       Amplifier_Power_Controls.Bias_On + " Received Bias: " + Convert.ToString(bias));
                    AMP_Raw_Bias_label5.Text = "BIAS I (ma): " + Convert.ToString(bias);
                    AMP_Current_label5.Text = "PA I (ma): " + Convert.ToString(bias);
                    break;

                case Window_controls.CMD_SET_SMETER_DISPLAY:
                    if (operand == 1)
                    {
                        Window_controls.Smeter_On_By_Server = true;
                        Master_Controls.code_triggered = true;
                        Master_Controls.code_triggered = false;
                        Window_controls.Smeter_Toggle = true;
                    }
                    else
                    {
                        Window_controls.Smeter_On_By_Server = false;
                        Master_Controls.code_triggered = true;
                        Master_Controls.code_triggered = false;
                        Window_controls.Smeter_Toggle = false;
                    }
                    break;

                case Panadapter_Controls.CMD_SET_DRIFT:
                    int drift_temp = 0;
                    drift_temp = BitConverter.ToInt16(message, 1);
                    MonitorTextBoxText(" OnUdpData -> Panadapter_Controls.CMD_SET_DRIFT.  Drift:" +
                        Convert.ToString(drift_temp));
                    Freq_Comp_label32.Text = "Freq Comp: " + Convert.ToString(drift_temp) + " Hz";
                    break;

                case Master_Controls.CMD_GET_TRANSCEIVER_TEMP:
                    Temperature_label57.Text = "Transceiver: " + Convert.ToString(operand) + "°C";
                    Master_Controls.Transceiver_Warming = false;
                    break;

                case Master_Controls.CMD_RPI_SET_TEMPERATURE:
                    RPi_Temperature_label1.Text = "Processor: " + Convert.ToString(operand) + "°C";
                    break;

                case Master_Controls.CMD_GET_MIA_STATUS:
                    MonitorTextBoxText(" OnUdpData -> CMD_GET_MIA_STATUS: " + Convert.ToString(operand));
                    if (operand == 0)
                    {
                        Solidus_Controls.Mia_Status = false;
                        //PA_vButton1.Enabled = false;
                        //PA_vButton1.Text = "QRP";
                        //PA_vButton1.BackColor = Color.Gainsboro;
                        //PA_vButton1.ForeColor = Color.Black;
                        Master_Controls.QRP_Mode = true;
                    }
                    else
                    {
                        Solidus_Controls.Mia_Status = true;
                        //PA_vButton1.Enabled = true;
                    }
                    Solidus_Controls.Mia_Status_Set = true;
                    Manage_Solidus_Status();
                    break;

                case Master_Controls.CMD_SET_PA_BYPASS:
                    MonitorTextBoxText(" OnUdpData -> CMD_SET_PA_BYPASS: " + Convert.ToString(operand));
                    if (operand == 0)
                    {
                        PA_vButton1.Text = "QRP";
                        PA_vButton1.BackColor = Color.Gainsboro;
                        PA_vButton1.ForeColor = Color.Black;
                        Master_Controls.QRP_Mode = true;
                    }
                    else
                    {
                        PA_vButton1.BackColor = Color.Red;
                        PA_vButton1.ForeColor = Color.White;
                        PA_vButton1.Text = "QRO";
                        Master_Controls.QRP_Mode = false;
                    }
                    break;

                case IQ_Controls.CMD_GET_IQ_VALUE:
                    IQ_Controls.RPi.Calibration_Slider_Value = BitConverter.ToInt32(message, 1);
#if !RPI
                    LefthScrollBar1.Value = BitConverter.ToInt32(message, 1);
                    IQLefttextBox2.Text = Convert.ToString(BitConverter.ToInt32(message, 1));
                    IQ_Controls.IQ_Offset = LefthScrollBar1.Value;
                    IQ_Controls.Previous_IQ_Offset = LefthScrollBar1.Value;
                    IQBD_hScrollBar1.Value = BitConverter.ToInt32(message, 1);
#endif
                    MonitorTextBoxText(" CMD_GET_IQ_VALUE: " + Convert.ToString(IQBD_hScrollBar1.Value));
                    break;

                case oCode.CMD_SET_STOP:
                    if (!Master_Controls.Shutdown)
                    {
                        MessageBox.Show("A STOP Command from MS-SDR has been received." + "\r\n" +
                        "MSCC will stop", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        MessageBox.Show("MSCC is Stopping per User Request.", "MSCC", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    Application.Exit();
                    break;

                case AGC_ALC_Notch_Controls.CMD_SET_AGC_FAST_LEVEL:
                    AGC_hScrollBar1.Value = BitConverter.ToInt32(message, 1);
                    AGC_label57.Text = Convert.ToString(AGC_hScrollBar1.Value) + " mS";
                    AGC_ALC_Notch_Controls.AGC_Release = AGC_hScrollBar1.Value;
                    MonitorTextBoxText(" CMD_SET_AGC_FAST_LEVEL: " + Convert.ToString(AGC_ALC_Notch_Controls.AGC_Release));
                    break;

                case AGC_ALC_Notch_Controls.CMD_GET_SET_AGC:
                    Master_Controls.code_triggered = true;
                    AGC_ALC_Notch_Controls.AGC_Level = operand;
                    AGC_listBox1.SelectedIndex = AGC_ALC_Notch_Controls.AGC_Level;
                    switch (AGC_ALC_Notch_Controls.AGC_Level)
                    {
                        case 0:
                            ACG_button.Text = "SLO";
                            break;
                        case 1:
                            ACG_button.Text = "MED";
                            break;
                        case 2:
                            ACG_button.Text = "FST";
                            break;
                    }
                    Master_Controls.code_triggered = false;
                    break;


                case Filter_control.CMD_SET_CW_PITCH:
                    Master_Controls.code_triggered = true;
                    CW_Pitch_listBox1.SelectedIndex = operand;
                    Filter_control.CW_Pitch_Index = operand;
                    switch (operand)
                    {
                        case 0:
                            Filter_control.CW_Pitch = 400;
                            break;
                        case 1:
                            Filter_control.CW_Pitch = 500;
                            break;
                        case 2:
                            Filter_control.CW_Pitch = 600;
                            break;
                        case 3:
                            Filter_control.CW_Pitch = 700;
                            break;
                        case 4:
                            Filter_control.CW_Pitch = 800;
                            break;
                        case 5:
                            Filter_control.CW_Pitch = 1000;
                            break;
                    }
                    Master_Controls.code_triggered = false;
                    break;

                case Panadapter_Controls.CMD_CW_SNAP_FINISHED:
                    MonitorTextBoxText(" CMD_CW_SNAP_FINISHED ->Received");
                    Panadapter_Controls.CW_Snap.Button_Color = Color.Black;
                    Window_controls.Waterfall_Controls.CW_Snap.Button_Color = Color.Black;
                    //Panadapter_Controls.CW_Snap.CW_button = false;
                    Panadapter_Controls.CW_Snap.CW_snap_status = false;
                    Window_controls.Waterfall_Controls.CW_Snap.CW_snap_status = false;
                    break;

                case Panadapter_Controls.CMD_SET_CW_SNAP_FREQ:
                    MonitorTextBoxText(" CMD_SET_CW_SNAP_FREQ ->Received");
                    int CW_Snap_Freq = BitConverter.ToInt32(message, 1);
                    MonitorTextBoxText(" CMD_SET_CW_SNAP_FREQ -> Setting CW SNAP Frequency: " + Convert.ToString(CW_Snap_Freq));
                    oCode.DisplayFreq = CW_Snap_Freq;
                    Display_Main_Freq();
                    break;

                case AGC_ALC_Notch_Controls.CMD_GET_SET_AUTO_NOTCH:
                    if (operand == 1)
                    {
                        Freqbutton3.BackColor = Color.Red;
                        Freqbutton3.ForeColor = Color.Black;
                        Freqbutton3.FlatStyle = FlatStyle.Popup;
                        AGC_ALC_Notch_Controls.Notch_Button_On = true;
                    }
                    else
                    {
                        Freqbutton3.BackColor = Color.Gainsboro;
                        Freqbutton3.ForeColor = Color.Black;
                        AGC_ALC_Notch_Controls.Notch_Button_On = false;
                    }
                    break;

                case NB_Controls.NB_ENABLE:
                    if (operand == 1)
                    {
                        NB_button2.BackColor = Color.Red;
                        NB_button2.ForeColor = Color.White;
                        NB_button2.FlatStyle = FlatStyle.Popup;
                        NB_Controls.NB_Button_On = true;

                        NB_ON_OFF_button2.BackColor = Color.Red;
                        NB_ON_OFF_button2.ForeColor = Color.White;
                        NB_ON_OFF_button2.FlatStyle = FlatStyle.Popup;
                        NB_ON_OFF_button2.Text = "ON";
                        NB_Controls.NB_Main_Button_On = true;
                    }
                    else
                    {
                        NB_Controls.NB_Main_Button_On = false;
                        NB_button2.BackColor = Color.Gainsboro;
                        NB_button2.ForeColor = Color.Black;
                        NB_Controls.NB_Button_On = false;
                        NB_ON_OFF_button2.BackColor = Color.Gainsboro;
                        NB_ON_OFF_button2.ForeColor = Color.Black;
                        NB_ON_OFF_button2.Text = "OFF";
                    }
                    break;

                case NB_Controls.NB_PULSE_WIDTH:
                    NB_hScrollBar1.Value = BitConverter.ToInt32(message, 1);
                    NB_Width_label16.Text = Convert.ToString(NB_hScrollBar1.Value) + " uS";
                    NB_hScrollBar1_Scroll(null, null);
                    break;

                case NB_Controls.NB_THRESHOLD:
                    Master_Controls.code_triggered = true;
                    NB_Threshold_hScrollBar1.Value = BitConverter.ToInt32(message, 1);
                    NB_Threshold_hScrollBar1_Scroll(null, null);
                    float percent = ((float)NB_Threshold_hScrollBar1.Value / 1000.0f) * 100.0f;
                    NB_Threshold_label16.Text = percent.ToString("F1") + " %";
                    Master_Controls.code_triggered = false;
                    break;

                case Frequency_Calibration_controls.CMD_SET_CALIBRATIION_PROGRESS:
                    if (Frequency_Calibration_controls.Calibration_In_Progress)
                    {
                        if (operand <= 100)// Prevent a progress bar out of range condition
                        {
                            Calibration_progressBar1.PerformStep();
                            Freq_CAl_Progress_Lable.Text = Convert.ToString(operand) + "%";
                        }
                    }
                    break;

                case Master_Controls.CMD_SET_TX_SET_BY_SERVER:
                    //data_processed = true;
                    MonitorTextBoxText(" CMD_SET_TX_SET_BY_SERVER RECIEVED. VALUE : " + Convert.ToString(operand));
                    if (operand == 0)
                    {
                        Master_Controls.Transmit_Mode = false;
                        Master_Controls.PPT_Mode = false;
                        button1.BackColor = Color.Gainsboro;
                        button1.ForeColor = Color.Black;
                        button1.Text = "PTT";
                        Main_Power_hScrollBar1.Visible = false;
                        if (Master_Controls.Tuning_Mode == true)
                        {
                            buttTune.BackColor = Color.Gainsboro;
                            buttTune.ForeColor = Color.Black;
                            Master_Controls.Tuning_Mode = false;
                            AM_Carrier_hScrollBar1.Enabled = true;
                            Power_hScrollBar1.Enabled = true;
                            CW_Power_hScrollBar1.Enabled = true;
                            Tune_vButton2.BackColor = Color.Gainsboro;
                            Tune_vButton2.ForeColor = Color.Black;
                            Tune_vButton2.Text = "TUN";
                        }
                    }
                    if (operand == 1)
                    {
                        Master_Controls.Transmit_Mode = true;
                        Master_Controls.PPT_Mode = true;
                        button1.BackColor = Color.Red;
                        button1.ForeColor = Color.White;
                        button1.Text = "PTT";
                        Main_Power_hScrollBar1.Visible = true;
                        switch (oCode.modeswitch)
                        {
                            case 0:
                                Main_Power_hScrollBar1.Value = Power_Controls.AM_Power;
                                break;
                            case 1:
                                Main_Power_hScrollBar1.Value = Power_Controls.Main_Power;
                                break;
                            case 2:
                                Main_Power_hScrollBar1.Value = Power_Controls.Main_Power;
                                break;
                            case 3:
                                Main_Power_hScrollBar1.Value = Power_Controls.CW_Power;
                                break;
                        }
                    }
                    break;

                case Master_Controls.CMD_SET_DISPLAY_FREQ:
                    Freq = BitConverter.ToInt32(message, 1);
                    band = Set_band(Freq);
                   
                    MonitorTextBoxText(" CMD_SET_DISPLAY_FREQ RECIEVED. VALUE : " + BitConverter.ToInt32(message, 1));
                    MonitorTextBoxText(" CMD_SET_DISPLAY_FREQ RECIEVED. Current Band: " + oCode.Last_band);
                    switch (band)
                    {
                        case 160:
                            Last_used.B160.Freq = Freq;
                            break;
                        case 80:
                            Last_used.B80.Freq = Freq;
                            break;
                        case 60:
                            Last_used.B60.Freq = Freq;
                            break;
                        case 40:
                            Last_used.B40.Freq = Freq;
                            break;
                        case 30:
                            Last_used.B30.Freq = Freq;
                            break;
                        case 20:
                            Last_used.B20.Freq = Freq;
                            break;
                        case 17:
                            Last_used.B17.Freq = Freq;
                            break;
                        case 15:
                            Last_used.B15.Freq = Freq;
                            break;
                        case 12:
                            Last_used.B12.Freq = Freq;
                            break;
                        case 10:
                            Last_used.B10.Freq = Freq;
                            break;
                        default:
                            Last_used.GEN.Freq = Freq;
                            break;
                    }
                    oCode.DisplayFreq = Freq;
                    Display_Main_Freq();
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, oCode.DisplayFreq);
                    break;

                case IQ_Controls.IQ_OPERATION_COMPLETE:
                    IQ_Commit_button2.ForeColor = Control.DefaultForeColor;
                    IQ_Commit_button2.Text = "APPLY";
                    IQ_Commit_button2.FlatStyle = FlatStyle.Standard;
                    IQ_Controls.IQ_Calibrating = false;
                    break;

                case Volume_Controls.CMD_SET_OVERDRIVEN:
                    switch (operand)
                    {
                        case 0:
                            Volume_Controls.Overdriven_Warning = false;
                            break;
                        case 1:
                            Volume_Controls.Overdriven_Warning = true;
                            break;
                    }
                    break;

                case Panadapter_Controls.CMD_GET_SET_PANADAPTER:
                    int sequence = 0;
                    int data_size = 0;
                    int temp_x = 0;
                    UInt16 x = 0;
                    UInt16 y = 0;
#if RPI
                    UInt16 waterfall_index = 0;
#endif

                    if (Master_Controls.Debug_Display)
                    {
                        MonitorTextBoxText(" OnUdpData -> CMD_GET_SET_PANADAPTER");
                    }
                    sequence = operand;
                    data_size = read_size - 2;
                    switch (sequence)
                    {
                        case 0:
                            if (Panadapter_Controls.Sequence_1_Complete)
                            {
                                Panadapter_Controls.Sequence_0_Complete = false;
                                x = 0;
                                y = 0;
                                Panadapter_Controls.Buffer_0.X_value[x] = x;
                                //Need skip past the first two control bytes
                                Panadapter_Controls.Buffer_0.Y_value[x] = BitConverter.ToUInt16(message, 2);
                                x++;
                                y = 4;
                                temp_x = 1;
                                while (temp_x < (data_size / 2))
                                {
                                    Panadapter_Controls.Buffer_0.X_value[x] = x;
                                    Panadapter_Controls.Buffer_0.Y_value[x] = BitConverter.ToUInt16(message, y);
                                    x++;
                                    y += 2;
                                    temp_x++;
                                }
                                x = 0;
                                Panadapter_Controls.Sequence_0_Complete = true;
                            }
                            break;

                        case 1:
                            if (Panadapter_Controls.Sequence_0_Complete)
                            {
                                Panadapter_Controls.Sequence_1_Complete = false;
                                //Do the first copy of the message buffer outside of the loop to skip over the control bytes.
                                x = 0;
                                y = 0;
                                Panadapter_Controls.Buffer_1.X_value[x] = x;
                                //Need skip past the first two control bytes
                                Panadapter_Controls.Buffer_1.Y_value[x] = BitConverter.ToUInt16(message, 2);
                                x++;
                                y = 4;
                                temp_x = 1;
                                while (temp_x < (data_size / 2))
                                {
                                    Panadapter_Controls.Buffer_1.X_value[x] = x;
                                    Panadapter_Controls.Buffer_1.Y_value[x] = BitConverter.ToUInt16(message, y);
                                    x++;
                                    y += 2;
                                    temp_x++;
                                }

                                //Now copy the two receive buffers to the Spectrum and Waterfall display buffers.
                                //for (x = 0; x < Panadapter_Controls.Max_X; x++)
                                for (x = 0; x < (data_size / 2); x++)
                                {
                                    Panadapter_Controls.Display_Buffer.X_value[x] = x;
                                    Panadapter_Controls.Display_Buffer.Y_value[x] = Panadapter_Controls.Buffer_0.Y_value[x];
#if !RPI
                                    Window_controls.Waterfall_Controls.Display_Buffer.X_value[x] = x;
                                    Window_controls.Waterfall_Controls.Display_Buffer.Y_value[x] =
                                        Panadapter_Controls.Buffer_0.Y_value[x];
#endif

                                }

                                for (x = 0; x < (data_size / 2); x++)
                                {
                                    Panadapter_Controls.Display_Buffer.X_value[x + Panadapter_Controls.Max_X] =
                                        (UInt16)(x + Panadapter_Controls.Max_X);
                                    Panadapter_Controls.Display_Buffer.Y_value[x + Panadapter_Controls.Max_X] =
                                        Panadapter_Controls.Buffer_1.Y_value[x];
#if !RPI
                                    Window_controls.Waterfall_Controls.Display_Buffer.X_value[x + Panadapter_Controls.Max_X] =
                                        (UInt16)(x + Panadapter_Controls.Max_X);
                                    Window_controls.Waterfall_Controls.Display_Buffer.Y_value[x + Panadapter_Controls.Max_X] =
                                        Panadapter_Controls.Buffer_1.Y_value[x];
#endif
                                }
#if RPI
                                for (x = 0; x < (Panadapter_Controls.Max_X * 2); x++) //Copy Data to Waterfall buffer
                                {
                                    //if(x > 3)
                                    //{
                                    //    waterfall_index = x;
                                    //    waterfall_index = (UInt16)(x - 4);
                                    //}
                                    waterfall_index = x;
                                    Window_controls.Waterfall_Controls.Display_Buffer.Y_value[waterfall_index] =
                                        Panadapter_Controls.Display_Buffer.Y_value[x];
                                }
#endif
                                Panadapter_Controls.Sequence_1_Complete = true;
                                Update_Spectrum();
                                time_now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                                receive_time = time_now - previous_time;
                                if (receive_time > 500)
                                {
                                    receive_time = 500;
                                }
                                Average_receive_time(receive_time);
                                previous_time = time_now;
                            }
                            break;
                    }
                    break;

                case Volume_Controls.CMD_SET_MIC_VOLUME:
                    MonitorTextBoxText(" OnUdpData -> CMD_SET_MIC_VOLUME: " + operand);
#if !RPI
                    MicVolume_hScrollBar1.Value = operand;
                    if (Settings.Default.Mic_Is_Digital)
                    {
                        Settings.Default.Digital_Volume = (short)operand;
                    }
                    else
                    {
                        Settings.Default.Voice_Volume = (short)operand;
                    }
                    Microphone_textBox2.Text = Convert.ToString(operand);
#endif
                    break;

                case Volume_Controls.CMD_SET_COMPRESSION_STATE:
                    MonitorTextBoxText(" OnUdpData ->  CMD_SET_COMPRESSION_STATE: " + operand);
                    Volume_Controls.Compression_State = (short)operand;
                    if (Volume_Controls.Compression_State == 1)
                    {
                        Compression_button2.BackColor = Color.Red;
                        Compression_button2.ForeColor = Color.White;
                        Compression_button2.Text = "Compression ON";
                        Compression_button4.BackColor = Color.Red;
                        Compression_button4.ForeColor = Color.White;
                    }
                    else
                    {
                        Compression_button2.BackColor = Color.Gainsboro;
                        Compression_button2.ForeColor = Color.Black;
                        Compression_button2.Text = "Compression OFF";
                        Compression_button4.BackColor = Color.Gainsboro;
                        Compression_button4.ForeColor = Color.Black;
                    }
                    break;

                case Volume_Controls.CMD_SET_COMPRESSION_LEVEL:
                    MonitorTextBoxText(" OnUdpData ->  CMD_SET_COMPRESSION_LEVEL: " + operand);
                    if (operand > 20)
                    {
                        operand = 20;
                    }
                    Compression_Level_hScrollBar1.Value = operand;
                    Compression_label44.Text = Convert.ToString(operand) + " db";
                    break;

                case Volume_Controls.CMD_SET_SPEAKER_VOLUME:
                    MonitorTextBoxText(" OnUdpData -> CMD_SET_SPEAKER_VOLUME: " + operand);
                    int speaker_value = operand;
                    if(speaker_value > 100)
                    {
                        speaker_value = 100;
                    }
                    RPi_Settings.Volume_Settings.Speaker_Volume = speaker_value;
#if !RPI
                    Volume_hScrollBar1.Value = speaker_value;
                    if (Settings.Default.Speaker_MutED)
                    {
                        Volume_Mute_button2_Click(null, null);
                    }
                    Volume_textBox2.Text = Convert.ToString(operand);
#endif
#if FTDI
                    Tuning_Knob_Controls.Volume_Function.Volume = operand;
#endif
                    break;

                case Volume_Controls.CMD_SET_MIC_MUTE:
                    MonitorTextBoxText(" OnUdpData ->  CMD_SET_MIC_MUTE: " + operand);
                    switch (operand)
                    {
                        case 1:
                            Volume_Controls.Mic_MutED = true;
                            break;
                        default:
                            Volume_Controls.Mic_MutED = false;
                            break;
                    }
                    Master_Controls.code_triggered = true;
                    TX_Mute_button2_Click(null, null);
                    break;

                case Volume_Controls.CMD_SET_SPEAKER_MUTE:
                    MonitorTextBoxText(" OnUdpData -> NOOP -> CMD_SET_SPEAKER_MUTE: " + operand);
                    /*Master_Controls.code_triggered = true;
                    switch (operand)
                    {
                        case 1:
                            Settings.Default.Speaker_MutED = true;
                            Volume_Mute_button2.ForeColor = Color.Red;
                            Volume_Mute_button2.Text = "MUTED";
                            Volume_Mute_button2.FlatStyle = FlatStyle.Popup;
                            break;
                        default:
                            Settings.Default.Speaker_MutED = false;
                            Volume_Mute_button2.ForeColor = Control.DefaultForeColor;
                            Volume_Mute_button2.Text = "Volume";
                            Volume_Mute_button2.FlatStyle = FlatStyle.Standard;
                            break;
                    }*/
                    break;

                case Filter_control.CMD_SET_BW_LOCUT_DEFAULT:
                    //int swapped = 0;
                    MonitorTextBoxText(" OnUdpData ->  CMD_SET_BW_LOCUT_DEFAULT: " + operand);
                    //swapped = Swap_lowcut_listbox_index(operand);
                    Default_Low_Cut_listBox1.SelectedIndex = operand;
                    Settings.Default.Lo_Cut_Default = operand;
                    set_default_filter_configuration();
                    break;

                case Filter_control.CMD_SET_BW_HICUT_DEFAULT:
                    //int swapped = 0;
                    MonitorTextBoxText(" OnUdpData ->  CMD_SET_BW_HICUT_DEFAULT: " + operand);
                    //swapped = Swap_hicut_listbox_index(operand);
                    Default_High_Cut_listBox1.SelectedIndex = operand;
                    Settings.Default.Hi_Cut_Default = operand;
                    set_default_filter_configuration();
                    break;

                case Filter_control.CMD_SET_CW_BW_DEFAULT:
                    //int swapped = 0;
                    MonitorTextBoxText(" OnUdpData ->  CMD_SET_CW_BW_DEFAULT: " + operand);
                    //swapped = Swap_hicut_listbox_index(operand);
                    Default_CW_Filter_listBox1.SelectedIndex = operand;
                    Settings.Default.CW_Default = operand;
                    set_default_filter_configuration();
                    break;
                
                /*case Comm_Port_Controls.CMD_GET_SET_HR50_COMM_NAME_INDEX:
                    Master_Controls.code_triggered = true;
                    MonitorTextBoxText(" OnUdpData ->  CMD_GET_SET_HR50_COMM_NAME_INDEX: " + operand);
                    if (operand != 200)
                    {
                        button2.BackColor = Color.Red;
                        button2.ForeColor = Color.White;
                        button2.Text = "ACTIVE";
                        button2.FlatStyle = FlatStyle.Popup;
                        Comm_Port_Controls.HR50_Controls.Comm_Port_Selected = true;
                        Comm_Port_Controls.HR50_Controls.Comm_Port_Open = true;
                        Comm_Port_Controls.HR50_Controls.Button_Toggle = true;
                        Comm_Port_Controls.HR50_Controls.Comm_Name_Index = operand;
                        Comm_Port_Controls.HR50_Controls.Set_By_Server = true;
                        HR50_listBox1.SelectedIndex = operand;
                        Comm_Port_Controls.HR50_Controls.Comm_Port_Open_by_server = true;
                        Comm_Port_Controls.HR50_Controls.Set_By_Server = false;
                        Comm_Port_Controls.HR50_Controls.Button_Toggle = true;
                    }
                    else
                    {
                        button2.BackColor = Control.DefaultBackColor;
                        button2.ForeColor = Color.Green;
                        button2.Text = "CLOSED";
                        button2.FlatStyle = FlatStyle.Standard;
                        Comm_Port_Controls.HR50_Controls.Comm_Port_Open = false;
                        Comm_Port_Controls.HR50_Controls.Button_Toggle = false;
                        Comm_Port_Controls.HR50_Controls.Comm_Name_Index = operand;
                        Comm_Port_Controls.HR50_Controls.Comm_Port_Selected = false;
                        Comm_Port_Controls.HR50_Controls.Comm_Port_Open_by_server = false;
                        Comm_Port_Controls.HR50_Controls.Set_By_Server = false;
                        Comm_Port_Controls.HR50_Controls.Button_Toggle = false;
                    }
                    Master_Controls.code_triggered = false;
                    break;

                case Comm_Port_Controls.CMD_GET_SET_COMM_NAME_INDEX:
                    MonitorTextBoxText(" OnUdpData ->  CMD_GET_SET_COMM_NAME_INDEX: " + operand);
                    Comm_Port_Controls.Box_indexes.Comm_Name_Index = operand;
                    break;

                case Comm_Port_Controls.CMD_GET_SET_COMM_PARITY:
                    MonitorTextBoxText(" OnUdpData ->  CMD_GET_SET_COMM_PARITY: " + operand);
                    Comm_Port_Controls.Box_indexes.Parity = operand;
                    break;

                case Comm_Port_Controls.CMD_GET_SET_COMM_STOP_BITS:
                    MonitorTextBoxText(" OnUdpData ->  CMD_GET_SET_COMM_STOP_BITS: " + operand);
                    Comm_Port_Controls.Box_indexes.Stop_Bits = operand;
                    break;

                case Comm_Port_Controls.CMD_GET_SET_COMM_DATA_BITS:
                    MonitorTextBoxText(" OnUdpData ->  CMD_GET_SET_COMM_DATA_BITS: " + operand);
                    Comm_Port_Controls.Box_indexes.Data_Bits = operand;
                    break;

                case Comm_Port_Controls.CMD_GET_SET_COMM_BAUD_RATE:
                    MonitorTextBoxText(" OnUdpData ->  CMD_GET_SET_COMM_BAUD_RATE: " + operand);
                    Comm_Port_Controls.Box_indexes.Baud_Rate = operand;
                    break;

                case Comm_Port_Controls.CMD_GET_SET_COMM_PORT_PINS:
                    MonitorTextBoxText(" OnUdpData ->  CMD_GET_SET_COMM_PORT_PINS: " + operand);
                    Comm_Port_Controls.Box_indexes.Comm_Port_Pins = operand;
                    break;
                    */
                case Frequency_Calibration_controls.CMD_SET_CALIBRATION_FINISHED:
                    Frequency_Calibration_controls.Calibration_In_Progress = false;
                    if (operand == 1)
                    {
                        if (Frequency_Calibration_controls.Check_only)
                        {
                            calibratebutton1.Text = "CHECK PASSED";
                            calibratebutton1.BackColor = Color.Gainsboro;
                            calibratebutton1.ForeColor = Color.Black;
                        }
                        else
                        {
                            calibratebutton1.Text = "CALIBRATED";
                            calibratebutton1.BackColor = Color.Gainsboro;
                            calibratebutton1.ForeColor = Color.Black;
                        }

                    }
                    else
                    {
                        if (Frequency_Calibration_controls.Check_only)
                        {
                            calibratebutton1.Text = "CHECK FAILED";
                            calibratebutton1.BackColor = Color.Gainsboro;
                            calibratebutton1.ForeColor = Color.Black;
                        }
                        else
                        {
                            calibratebutton1.Text = "CALIBRATION FAILED";
                            calibratebutton1.BackColor = Color.White;
                            calibratebutton1.ForeColor = Color.Red;
                        }
                    }
                    Frequency_Calibration_controls.Check_only = false;
                    break;

                case Frequency_Calibration_controls.CMD_GET_SET_CAL_FREQ_DELTA:
                    Delta = BitConverter.ToInt32(message, 1);
                    MonitorTextBoxText(" OnUdpData -> CMD_GET_SET_CAL_FREQ_DELTA -> Delta: " +
                        Convert.ToString(Delta));
                    if (Delta < 10000)
                    {
                        calibratebutton1.Text = calibratebutton1.Text + "\n\r" + Convert.ToString(Delta) + " Hz";
                    }
                    break;

                case oCode.CMD_GET_SET_MSSDR_STATUS:
                    Master_Controls.MSSDR_running = true;
                    MonitorTextBoxText(" OnUdpData -> CMD_GET_SET_MSSDR_STATUS -> Master_Controls.MSSDR_running: " +
                       Convert.ToString(Master_Controls.MSSDR_running));
                    break;

                /*case oCode.CMD_GET_SET_SPEAKER_DEVICE:
                    if (Audio_Device_Controls.Selected_Speaker != 0)
                    {
                        MonitorTextBoxText(" OnUdpData -> CMD_GET_SET_SPEAKER_DEVICE RECEIVED. Selected Audio Device: " + 
                            Audio_Device_Controls.Selected_Speaker);
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_SPEAKER_DEVICE, Audio_Device_Controls.Selected_Speaker);
                    }
                    else
                    {
                        MonitorTextBoxText(" OnUdpData -> CMD_GET_SET_SPEAKER_DEVICE RECEIVED. Default Audio Device: " + 
                            Audio_Device_Controls.Default_Speaker);
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_SPEAKER_DEVICE, Audio_Device_Controls.Default_Speaker);
                    }
                    break;

                case oCode.CMD_GET_SET_MIC_DEVICE:
                    if (Audio_Device_Controls.Selected_Mic != 0)
                    {
                        MonitorTextBoxText(" OnUdpData -> CMD_GET_SET_MIC_DEVICE RECEIVED. Selected Audio Device: " + 
                            Audio_Device_Controls.Selected_Mic);
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_MIC_DEVICE, Audio_Device_Controls.Selected_Mic);
                    }
                    else
                    {
                        MonitorTextBoxText(" OnUdpData -> CMD_GET_SET_MIC_DEVICE RECEIVED. Default Audio Device: " + 
                            Audio_Device_Controls.Default_Mic);
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_GET_SET_MIC_DEVICE, Audio_Device_Controls.Default_Mic);
                    }
                    break;
                */

                case oCode.CMD_GET_SET_STARTUP_BAND:
                    MonitorTextBoxText(" OnUdpData -> CMD_GET_SET_STARTUP_BAND RECEIVED. VALUE : " + operand);
                    Master_Controls.Startup_Band = (short)operand;
                    MonitorTextBoxText(" OnUdpData -> CMD_GET_SET_STARTUP_BAND RECEIVED -> Finished: " + operand);
                    break;

                case oCode.CMD_GET_SET_LAST_USED_FREQ:
                    Freq = BitConverter.ToInt32(message, 1);
                    oCode.Last_band = Set_band(Freq);
                    MonitorTextBoxText(" OnUdpData -> CMD_GET_SET_LAST_USED_FREQ RECIEVED. VALUE : " + BitConverter.ToInt32(message, 1));
                    MonitorTextBoxText(" OnUdpData -> CMD_GET_SET_LAST_USED_FREQ RECIEVED. Current Band: " + oCode.Last_band);
                    switch (oCode.Last_band)
                    {
                        case 160:
                            Last_used.B160.Freq = BitConverter.ToInt32(message, 1);
                            break;
                        case 80:
                            Last_used.B80.Freq = BitConverter.ToInt32(message, 1);
                            break;
                        case 60:
                            Last_used.B60.Freq = BitConverter.ToInt32(message, 1);
                            break;
                        case 40:
                            Last_used.B40.Freq = BitConverter.ToInt32(message, 1);
                            break;
                        case 30:
                            Last_used.B30.Freq = BitConverter.ToInt32(message, 1);
                            break;
                        case 20:
                            Last_used.B20.Freq = BitConverter.ToInt32(message, 1);
                            break;
                        case 17:
                            Last_used.B17.Freq = BitConverter.ToInt32(message, 1);
                            break;
                        case 15:
                            Last_used.B15.Freq = BitConverter.ToInt32(message, 1);
                            break;
                        case 12:
                            Last_used.B12.Freq = BitConverter.ToInt32(message, 1);
                            break;
                        case 10:
                            Last_used.B10.Freq = BitConverter.ToInt32(message, 1);
                            break;
                    }
                    break;

                case oCode.CMD_GET_SET_LAST_USED_MODE:
                    MonitorTextBoxText(" OnUdpData -> CMD_GET_SET_LAST_USED_MODE RECIEVED. VALUE : " + (char)operand);
                    MonitorTextBoxText(" OnUdpData -> CMD_GET_SET_LAST_USED_MODE RECIEVED. Current_band: " + oCode.Last_band);
                    switch (oCode.Last_band)
                    {
                        case 160:
                            Last_used.B160.Mode = (char)operand;
                            break;
                        case 80:
                            Last_used.B80.Mode = (char)operand;
                            break;
                        case 60:
                            Last_used.B60.Mode = (char)operand;
                            break;
                        case 40:
                            Last_used.B40.Mode = (char)operand;
                            break;
                        case 30:
                            Last_used.B30.Mode = (char)operand;
                            break;
                        case 20:
                            Last_used.B20.Mode = (char)operand;
                            break;
                        case 17:
                            Last_used.B17.Mode = (char)operand;
                            break;
                        case 15:
                            Last_used.B15.Mode = (char)operand;
                            break;
                        case 12:
                            Last_used.B12.Mode = (char)operand;
                            break;
                        case 10:
                            Last_used.B10.Mode = (char)operand;
                            break;
                    }
                    break;

                case Master_Controls.CMD_MODE_SET_BY_SERVER:
                    MonitorTextBoxText(" OnUdpData -> CMD_MODE_SET_BY_SERVER RECIEVED. VALUE : " + (char)operand);
                    MonitorTextBoxText(" OnUdpData -> CMD_MODE_SET_BY_SERVER RECIEVED. Current_band: " + oCode.Last_band);
                    //Filter_control.Previous_Filter_Mode = (char)operand;
                    set_main_mode_display((char)operand);
                    break;

                case Smeter_controls.CMD_GET_SET_SMETER:
                    Main_Smeter_controls.SMeter_value = BitConverter.ToInt32(message, 1);
                    Update_Smeter(BitConverter.ToInt32(message, 1));
                    //MonitorTextBoxText(" OnUdpData -> CMD_GET_SET_SMETER -> Value: " + BitConverter.ToInt32(message, 1));
                    break;

                case Amplifier_Power_Controls.CMD_GET_AMPLIFIER_POWER:
                    MonitorTextBoxText(" OnUdpData -> CMD_GET_AMPLIFIER_POWER Called. Power Value: " + Convert.ToString(operand));
                    if (operand >= 1 && operand <= 100)
                    {
                       
                        AMP_label57.Text = Convert.ToString(message[1]) + " %";
                        Amplifier_Power_Controls.New_Power_Value = message[1];
                        AMP_hScrollBar1.Value = message[1];
                        /*if (Amplifier_Power_Controls.Applied_Button_Active)
                        {
                            //AMP_Apply_button4.ForeColor = Control.DefaultForeColor;
                            //AMP_Apply_button4.Text = "APPLY";
                            //AMP_Apply_button4.FlatStyle = FlatStyle.Standard;
                            Amplifier_Power_Controls.Applied_Button_Active = false;
                        }
                        if (Amplifier_Power_Controls.Factory_Defaults)
                        {
                            //AMP_Defaults_button3.ForeColor = Control.DefaultForeColor;
                            //AMP_Defaults_button3.Text = "FACTORY DEFAULTS";
                            //AMP_Defaults_button3.FlatStyle = FlatStyle.Standard;
                            Amplifier_Power_Controls.Factory_Defaults = false;
                        }*/
                    }
                    /*else
                    {
                        if (operand == 1)
                        {
                            DialogResult err_ret = MessageBox.Show("Amplifier Update Failed. Error: " + Convert.ToString(operand) +
                            " Retry the operation \n\r", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            //AMP_Apply_button4.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
                            //AMP_Apply_button4.ForeColor = Control.DefaultForeColor;
                            //AMP_Apply_button4.Text = "APPLY";
                            //AMP_label57.Text = "RETRY";
                            //AMP_Apply_button4.FlatStyle = FlatStyle.Standard;
                            Amplifier_Power_Controls.Applied_Button_Active = false;
                        }
                        else
                        {
                            DialogResult ret = MessageBox.Show("Amplifier Update. Error: " + Convert.ToString(operand) +
                                " Please report error to Multus SDR,LLC.\n\r", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            //AMP_Apply_button4.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
                            //AMP_Apply_button4.ForeColor = Control.DefaultForeColor;
                            //AMP_Apply_button4.Text = "APPLY";
                            //AMP_label57.Text = "FAILED";
                            //AMP_Apply_button4.FlatStyle = FlatStyle.Standard;
                            Amplifier_Power_Controls.Applied_Button_Active = false;
                        }
                    }*/
                    break;

                case oCode.CMD_GET_BAND_POWER:
                    if (operand >= 2 && operand <= 100)
                    {
                        Thread.Sleep(500);
                        powerlabel14.Text = Convert.ToString(message[1]) + " POWER";
                        powerlabel14.ForeColor = Color.Black;
                        powerhScrollBar1.Value = message[1];
                        Power_Calibration_Controls.New_Power_Value = message[1];

                        if (Power_Calibration_Controls.Applied_Button_Active)
                        {
                            Power_Calibration_Controls.Applied_Button_Active = false;
                        }
                        //if (Power_Calibration_Controls.Committing)
                        //{
                        //    Commit_button3.ForeColor = Control.DefaultForeColor;
                        //    Commit_button3.Text = "COMMIT";
                        //    Commit_button3.FlatStyle = FlatStyle.Standard;
                        //    Power_Calibration_Controls.Committing = false;
                        //}
                        if (Power_Calibration_Controls.Factory_Defaults)
                        {
                            powerrestorebutton2.ForeColor = Control.DefaultForeColor;
                            powerrestorebutton2.Text = "FACTORY DEFAULTS";
                            powerrestorebutton2.FlatStyle = FlatStyle.Standard;
                            Power_Calibration_Controls.Factory_Defaults = false;
                        }
                    }
                    else
                    {
                        if (operand == 1)
                        {
                            DialogResult err_ret = MessageBox.Show("Power Update Failed. Error: " + Convert.ToString(operand) +
                            " Retry the operation \n\r", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            Power_Calibration_Controls.Applied_Button_Active = false;
                        }
                        else
                        {
                            DialogResult ret = MessageBox.Show("Calibration FAILED. Error: " + Convert.ToString(operand) +
                                " Please report error to Multus SDR,LLC.\n\r", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            Power_Calibration_Controls.Applied_Button_Active = false;
                        }
                    }
                    break;

                case oCode.CMD_GET_BAND_STACK_FREQ:
                    Band_Stack_Controls.Band_Stack_Complete = 0;
                    Band_Stack_Controls.Frequency = BitConverter.ToInt32(message, 1);
                    Band_Stack_Controls.Band_Stack_Complete++;
                    MonitorTextBoxText(" OnUdpData -> CMD_GET_BAND_STACK_FREQ CALLED -> Frequency: , " + Band_Stack_Controls.Frequency +
                                                                     ", Band Stack Count " + Band_Stack_Controls.Band_Stack_Complete);
                    break;

                case oCode.CMD_GET_BAND_STACK_MODE:
                    Band_Stack_Controls.Mode = (char)message[1];
                    Band_Stack_Controls.Band_Stack_Complete++;
                    MonitorTextBoxText(" OnUdpData -> CMD_GET_BAND_STACK_MODE CALLED -> Mode:  " + Band_Stack_Controls.Mode +
                                                                    ", Band Stack Count " + Band_Stack_Controls.Band_Stack_Complete);
                    break;

                case oCode.CMD_GET_BAND_STACK_BAND:
                    Band_Stack_Controls.Band = message[1];
                    Band_Stack_Controls.Band_Stack_Complete++;
                    MonitorTextBoxText(" OnUdpData -> CMD_GET_BAND_STACK_BAND CALLED -> Band: " + Band_Stack_Controls.Band +
                                                                  ", Band Stack Count " + Band_Stack_Controls.Band_Stack_Complete);
                    if (Band_Stack_Controls.Band_Stack_Complete == 3)
                    {
                        MonitorTextBoxText(" OnUdpData -> CMD_GET_BAND_STACK_BAND CALLED -> Calling Apply_Band_Stack ");
                        Apply_Band_Stack();
                    }
                    break;

                case oCode.CMD_GET_BAND_STACK:
                    oCode.band_stack_band = message[1];
                    if (oCode.band_stack_band >= 0 && oCode.band_stack_band <= 20)
                    {
                        band_stack_label29.BackColor = Color.White;
                        band_stack_label29.ForeColor = Color.Red;
                        band_stack_label29.Font = new Font(band_stack_label29.Font, FontStyle.Bold);
                        band_stack_label29.FlatStyle = FlatStyle.Popup;
                        switch (oCode.band_stack_band)
                        {
                            case 0:
                                band_stack_label29.Text = "10M UPDATING";
                                Thread.Sleep(1000);
                                band_stack_label29.Text = "10M Updated";
                                break;
                            case 1:
                                band_stack_label29.Text = "12M UPDATING";
                                Thread.Sleep(1000);
                                band_stack_label29.Text = "12M UPDATED";
                                break;
                            case 2:
                                band_stack_label29.Text = "15M UPDATING";
                                Thread.Sleep(1000);
                                band_stack_label29.Text = "15M UPDATED";
                                break;
                            case 3:
                                band_stack_label29.Text = "17M UPDATING";
                                Thread.Sleep(1000);
                                band_stack_label29.Text = "17M UPDATED";
                                break;
                            case 4:
                                band_stack_label29.Text = "20M UPDATING";
                                Thread.Sleep(1000);
                                band_stack_label29.Text = "20M UPDATED";
                                break;
                            case 5:
                                band_stack_label29.Text = "30M UPDATING";
                                Thread.Sleep(1000);
                                band_stack_label29.Text = "30M UPDATED";
                                break;
                            case 6:
                                band_stack_label29.Text = "40M UPDATING";
                                Thread.Sleep(1000);
                                band_stack_label29.Text = "40M UPDATED";
                                break;
                            case 7:
                                band_stack_label29.Text = "60M UPDATING";
                                Thread.Sleep(1000);
                                band_stack_label29.Text = "60M UPDATED";
                                break;
                            case 8:
                                band_stack_label29.Text = "80M UPDATING";
                                Thread.Sleep(1000);
                                band_stack_label29.Text = "80M UPDATED";
                                break;
                            case 9:
                                band_stack_label29.Text = "160M UPDATING";
                                Thread.Sleep(1000);
                                band_stack_label29.Text = "160M UPDATED";
                                break;
                            default:
                                band_stack_label29.Text = "NOT IN HAM BAND";
                                Thread.Sleep(2000);
                                band_stack_label29.Text = "STACK NOT UPDATED";
                                break;
                        }
                        band_stack_label29.ForeColor = Color.Black;
                        band_stack_label29.BackColor = Color.Gainsboro;
                        //band_stack_label29.FlatStyle = FlatStyle.Standard;
                        Initialize_Favorites();
                        //checkBox1_CheckedChanged(null, null);
                        checkBox1.Checked = true;
                    }
                    break;

                case Master_Controls.CMD_GET_SET_FIRMWARE_VERSION:
                    if (!firmware_version_major)
                    {
                        firmware_version_major = true;
                        Master_Controls.Firmware_Version.Major = BitConverter.ToInt32(message, 1);
                    }
                    else
                    {
                        Master_Controls.Firmware_Version.Minor = BitConverter.ToInt32(message, 1);
                        firmwarelabel16.Text = Convert.ToString(Master_Controls.Firmware_Version.Major) + "." +
                            Convert.ToString(Master_Controls.Firmware_Version.Minor);
                        Master_Controls.Firmware_Version.Minor = message[1];
                    }
                    break;

                case Master_Controls.CMD_GET_SET_MSSDR_VERSION:
                    if (!mssdr_version_major)
                    {
                        Master_Controls.mssdr_verison.Major = BitConverter.ToInt32(message, 1); // message[1];
                        mssdr_version_major = true;
                    }
                    else
                    {
                        Master_Controls.mssdr_verison.Minor = BitConverter.ToInt32(message, 1); // message[1];
                        MS_SDR_Version_label16.Text = Convert.ToString(Master_Controls.mssdr_verison.Major) + "." +
                            Convert.ToString(Master_Controls.mssdr_verison.Minor);
                    }
                    break;

                case Master_Controls.CMD_GET_SET_SDRCORE_TRANS_VERSION:
                    if (!sdrcore_trans_version_updated)
                    {
                        if (!sdrcore_trans_version_major)
                        {
                            Master_Controls.SDRcore_trans_verison.Major = BitConverter.ToInt32(message, 1); // message[1];
                            sdrcore_trans_version_major = true;
                        }
                        else
                        {
                            Master_Controls.SDRcore_trans_verison.Minor = BitConverter.ToInt32(message, 1); // message[1];
                            SDRcore_Trans_Version.Text = Convert.ToString(Master_Controls.SDRcore_trans_verison.Major) + "." +
                                Convert.ToString(Master_Controls.SDRcore_trans_verison.Minor);
                            sdrcore_trans_version_updated = true;
                        }
                    }
                    break;

                case Master_Controls.CMD_GET_SET_SDRCORE_RECV_VERSION:
                    if (!sdrcore_recv_verion_updated)
                    {
                        if (!sdrcore_recv_version_major)
                        {
                            Master_Controls.SDRcore_recv_verison.Major = BitConverter.ToInt32(message, 1); // message[1];
                            sdrcore_recv_version_major = true;
                        }
                        else
                        {
                            Master_Controls.SDRcore_recv_verison.Minor = BitConverter.ToInt32(message, 1); // message[1];
                            SDRcore_Recv_Version_label16.Text = Convert.ToString(Master_Controls.SDRcore_recv_verison.Major) +
                                "." + Convert.ToString(Master_Controls.SDRcore_recv_verison.Minor);
                            sdrcore_recv_verion_updated = true;
                        }
                    }
                    break;

                case Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND:
                    //MonitorTextBoxText(" OnUdpData -> CMD_SET_EXTENDED_COMMAND Received");
                    Process_extended_commands(ref message, read_size);
                    break;

                default:
                    MonitorTextBoxText(" OnUdpData -> COMMAND NOT RECOGNIZED: " + Convert.ToString(op_code));
                    break;
            }
            socket.BeginReceive(new AsyncCallback(OnUdpData), socket);
            Master_Controls.Network_Receive_Busy = false;
        }

        private void Compression_label44_Click(object sender, EventArgs e)
        {

        }

        /*private void cmbPortName_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            String comm_name;
            int index = 0;
            int comm_name_lenght = 0;

            if (oCode.isLoading) return;
            if (!Master_Controls.code_triggered)
            {
                index = cmbPortName.SelectedIndex;
                Comm_Port_Controls.Box_indexes.Previous_Index = index;
                Comm_Port_Controls.Box_indexes.Comm_Name_Index = index;
                comm_name = cmbPortName.Text;
                comm_name_lenght = System.Text.ASCIIEncoding.ASCII.GetByteCount(comm_name);
                if (Comm_Port_Controls.Comm_Port_Open == false)
                {
                    if (Comm_Port_Controls.Box_indexes.Comm_Name_Index == Comm_Port_Controls.HR50_Controls.Comm_Name_Index)
                    {
                        if (Comm_Port_Controls.HR50_Controls.Comm_Port_Open == false)
                        {
                            oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_NAME_INDEX, (short)index);
                            oCode.SendCommand_String(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_PORT, comm_name,
                                comm_name_lenght);
                            oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_BAUD_RATE,3);
                            oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_PARITY,0);
                            oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_DATA_BITS,1);
                            oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_STOP_BITS,0);
                        }
                        else
                        {
                            MessageBox.Show("Port is use by Hardrock 50 port.  Select a different Port", "MSCC",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_NAME_INDEX, (short)index);
                        oCode.SendCommand_String(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_PORT, comm_name,
                                 comm_name_lenght);
                        oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_BAUD_RATE, 3);
                        oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_PARITY, 0);
                        oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_DATA_BITS, 1);
                        oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_STOP_BITS, 0);
                    }
                }
                else
                {
                    if (Comm_Port_Controls.Box_indexes.Set_By_Server == false)
                    {
                        MessageBox.Show("Close port before selecting a new port", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //Comm_Port_Controls.Box_indexes.Set_By_Server = false;
                    }
                }
            }
        }*/

        /*private void btnOpenPort_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            switch (Comm_Port_Controls.Button_Toggle)
            {
                case false:
                    if (Comm_Port_Controls.Comm_Port_Open == false)
                    {
                        if (Comm_Port_Controls.Box_indexes.Comm_Name_Index == 100)
                        {
                            DialogResult ret = MessageBox.Show("Select a Comm Port", "MSCC",
                               MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            return;
                        }
                        oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_START, 1);
                        btnOpenPort.BackColor = Color.Red;
                        btnOpenPort.ForeColor = Color.White;
                        btnOpenPort.Text = "ACTIVE";
                        btnOpenPort.FlatStyle = FlatStyle.Popup;
                        Comm_Port_Controls.Comm_Port_Open = true;
                        Comm_Port_Controls.Button_Toggle = true;
                    }
                    break;

                case true:
                    MonitorTextBoxText(" btnOpenPort_Click -> Comm Port is Closed ");
                    btnOpenPort.BackColor = Control.DefaultBackColor;
                    btnOpenPort.ForeColor = Color.Green;
                    btnOpenPort.Text = "CLOSED";
                    btnOpenPort.FlatStyle = FlatStyle.Standard;
                    Comm_Port_Controls.CTS_Selected = 0;
                    Comm_Port_Controls.DCD_Selected = 0;
                    Comm_Port_Controls.Comm_Port_Open = false;
                    Comm_Port_Controls.Button_Toggle = false;
                    Comm_Port_Controls.Box_indexes.Comm_Name_Index = 100;
                    oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_START, 0);
                    break;
            }
        }

        private void Comm_Port_Reset_button3_Click_1(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            DialogResult ret = MessageBox.Show("Are you sure you want to reset the Com Port Devices list?", "MSCC",
                                                                       MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            if (ret == DialogResult.Yes)
            {
                MessageBox.Show("The Com Port Devices List will be reset on next startup of MSCC\n" +
                "MSCC will now STOP", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Comm_Port_Controls.CTS_Selected = 0;
                Comm_Port_Controls.DCD_Selected = 0;
                Comm_Port_Controls.Comm_Port_Open = false;
                Comm_Port_Controls.Button_Toggle = false;
                Comm_Port_Controls.Box_indexes.Comm_Name_Index = 100;
                oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_START, 0);
                Thread.Sleep(500);
                Application.Exit();
            }
        }

        private void Line_Signal_listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            int index = 0;
            if (oCode.isLoading) return;
            if (!Master_Controls.code_triggered)
            {
                if (Comm_Port_Controls.Comm_Port_Open != false)
                {
                    index = Line_Signal_listBox1.SelectedIndex;
                    switch (index)
                    {
                        case 0:
                            oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_PORT_PINS, 0);
                            break;

                        case 1:
                            oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_PORT_PINS, 1);
                            break;

                        case 2:
                            oCode.SendCommand(txsocket, txtarget, Comm_Port_Controls.CMD_GET_SET_COMM_PORT_PINS, 2);
                            break;
                    }
                }
                else
                {
                    DialogResult ret = MessageBox.Show("The COM PORT Must BE ACTIVE Before Selecting a Pin (CTS)", "MSCC",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }*/

        /*private void gbPortSettings_Enter_1(object sender, EventArgs e)
        {

        }*/

        /*private void Solidus_Button_1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }*/

        private void Solidus_Button_3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Solidus_Button_2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private byte Set_Antenna_Switch_Value(int index)
        {
            byte switch_value = 0;

            switch (index)
            {
                case 0:
                    switch_value = 160;
                    break;
                case 1:
                    switch_value = 80;
                    break;
                case 2:
                    switch_value = 60;
                    break;
                case 3:
                    switch_value = 40;
                    break;
                case 4:
                    switch_value = 30;
                    break;
                case 5:
                    switch_value = 20;
                    break;
                case 6:
                    switch_value = 17;
                    break;
                case 7:
                    switch_value = 15;
                    break;
                case 8:
                    switch_value = 12;
                    break;
                case 9:
                    switch_value = 10;
                    break;
            }
            return switch_value;
        }

        private void Antenna_Switch_comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;
            byte[] buf = new byte[2];

            if (oCode.isLoading) return;
            index = Antenna_Switch_comboBox1.SelectedIndex;
            buf[0] = Master_Controls.Extended_Commands.CMD_SET_ANTENNA_SWITCH;
            Settings.Default.Antenna_Switch_Index = (byte)index;
            buf[1] = Set_Antenna_Switch_Value(index);
            RPi_Settings.Controls.Antenna_Switch = (short)index;
            MonitorTextBoxText(" Antenna_Switch_comboBox1_SelectedIndexChanged: " +
                Convert.ToString(RPi_Settings.Controls.Antenna_Switch));
            oCode.SendCommand_MultiByte(txsocket, txtarget,
                   Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
        }
        /*
        private void Solidus_CW_groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void Solidus_Hold_Time_label43_Click(object sender, EventArgs e)
        {

        }
        
        private void Solidus_CW_Hold_label58_Click(object sender, EventArgs e)
        {

        }

        private void Solidus_CW_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void Solidus_Semi_Break_In_checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Solidus_CW_Pitch_label59_Click(object sender, EventArgs e)
        {

        }

        private void Solidus_CW_Pitch_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        */

        /*private void IQBD_ONOFF_button4_Click(object sender, EventArgs e)
        {
            byte[] buf = new byte[2];

            if (oCode.isLoading) return;
            buf[0] = Master_Controls.Extended_Commands.CMD_SET_IQBD_MONITOR;
            if (IQ_Controls.band_selected)
            {
                if (IQ_Controls.IQ_TX_MODE_ACTIVE)
                {
                    if (!IQ_Controls.IQBD_MONITOR)
                    {
                        IQBD_ONOFF.ForeColor = Color.Red;
                        IQBD_ONOFF.Text = "IQBD ON";
                        IQBD_ONOFF.FlatStyle = FlatStyle.Popup;
                        buf[1] = 1;
                        oCode.SendCommand_MultiByte(txsocket, txtarget,
                                                Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
                        IQ_Controls.IQ_Calibrating = true;
                        //IQ_Controls.IQ_TX_MODE_ACTIVE = false;
                        IQ_Controls.IQBD_MONITOR = true;
                    }
                    else
                    {
                        IQBD_ONOFF.ForeColor = Color.Green;
                        IQBD_ONOFF.Text = "IQBD OFF";
                        IQBD_ONOFF.FlatStyle = FlatStyle.Popup;
                        buf[1] = 0;
                        oCode.SendCommand_MultiByte(txsocket, txtarget,
                                                Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
                        IQ_Controls.IQ_Calibrating = false;
                        //IQ_Controls.IQ_TX_MODE_ACTIVE = false;
                        IQ_Controls.IQBD_MONITOR = false;
                    }
                }
                else
                {
                    DialogResult ret = MessageBox.Show("I/Q TX MODE NOT SET ON \r\nSET I/Q TX MODE to ON", "MSCC",
                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            else
            {
                DialogResult ret = MessageBox.Show("Select a Band", "MSCC",
                          MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }*/

        private void Forward_Power_label43_Click_1(object sender, EventArgs e)
        {

        }

        private void Forward_Meter_Load_1(object sender, EventArgs e)
        {

        }

        private void Reverse_Power_label43_Click_1(object sender, EventArgs e)
        {

        }

        private void Reverse_Meter_Load_1(object sender, EventArgs e)
        {

        }

        private void SWR_Value_label43_Click_1(object sender, EventArgs e)
        {

        }


        /*private void Solidus_CW_groupBox4_Enter_1(object sender, EventArgs e)
        {

        }

        private void Solidus_CW_Hold_label58_Click_1(object sender, EventArgs e)
        {

        }

        private void Solidus_CW_hScrollBar1_Scroll_1(object sender, ScrollEventArgs e)
        {

        }

        private void Solidus_Semi_Break_In_checkBox4_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void Solidus_Hold_Time_label43_Click_1(object sender, EventArgs e)
        {

        }

        private void Solidus_CW_Pitch_listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void Solidus_CW_Pitch_label59_Click_1(object sender, EventArgs e)
        {

        }
        */
        private void Tuning_Knob_groupBox1_Enter_1(object sender, EventArgs e)
        {

        }

        private void Left_Button_comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            byte[] buf = new byte[2];
            if (oCode.isLoading) return;
            int index = 0;
            byte transceiver_display = 0;
            buf[0] = Master_Controls.Extended_Commands.CMD_SET_LEFT_SWITCH;
            if (!Master_Controls.code_triggered)
            {
                index = Left_Button_comboBox2.SelectedIndex;
                if (Compare_Switches(Tuning_Knob_Controls.LEFT_SWITCH, index))
                {

                    Tuning_Knob_Controls.Button_left_function = (byte)index;
                    Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.LEFT_SWITCH] = (sbyte)index;
                    transceiver_display = (byte)(0x10 | index);
                    Tuning_Knob_Controls.Star_Position.Left = transceiver_display;
                    MonitorTextBoxText(" Left Button Star Position: 0x" + Tuning_Knob_Controls.Star_Position.Left.ToString("X"));
                    oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_TRANSCEIVER_DISPLAY, transceiver_display);
                    Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.LEFT_SWITCH] = (sbyte)index;
                    buf[1] = (byte)index;
                    oCode.SendCommand_MultiByte(txsocket, txtarget,
                                               Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
                    MFC_A_label38.Text = "A: " + Left_Button_comboBox2.Text;
                    //Update_MFC_Init_File();
                }
                else
                {
                    Left_Button_comboBox2.SelectedIndex =
                        Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.LEFT_SWITCH];
                }
            }
        }

        private void Middle_Button_comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            byte[] buf = new byte[2];

            if (oCode.isLoading) return;
            int index = 0;
            byte transceiver_display = 0;
            buf[0] = Master_Controls.Extended_Commands.CMD_SET_MIDDLE_SWITCH;
            if (!Master_Controls.code_triggered)
            {
                index = Middle_Button_comboBox3.SelectedIndex;
                if (Compare_Switches(Tuning_Knob_Controls.MIDDLE_SWITCH, index))
                {
                    Tuning_Knob_Controls.Button_middle_function = (byte)index;
                    //Tuning_Knob_Controls.Active_Functions.Middle_switch = index;
                    Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.MIDDLE_SWITCH] = (sbyte)index;
                    //transceiver_display = (byte)(20 + index);
                    transceiver_display = (byte)(0x20 | index);
                    Tuning_Knob_Controls.Star_Position.Middle = transceiver_display;
                    MonitorTextBoxText(" Middle Button Star Position: 0x" + Tuning_Knob_Controls.Star_Position.Middle.ToString("X"));
                    oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_TRANSCEIVER_DISPLAY, transceiver_display);
                    Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.MIDDLE_SWITCH] = (sbyte)index;
                    buf[1] = (byte)index;
                    oCode.SendCommand_MultiByte(txsocket, txtarget,
                                               Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
                    MFC_B_label38.Text = "B: " +Middle_Button_comboBox3.Text;
                    //Update_MFC_Init_File();
                }
                else
                {
                    Middle_Button_comboBox3.SelectedIndex =
                        Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.MIDDLE_SWITCH];
                }
            }
        }

        private void Right_Button_comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            byte[] buf = new byte[2];

            if (oCode.isLoading) return;
            int index = 0;
            byte transceiver_display = 0;
            buf[0] = Master_Controls.Extended_Commands.CMD_SET_RIGHT_SWITCH;
            if (!Master_Controls.code_triggered)
            {
                index = Right_Button_comboBox4.SelectedIndex;
                if (Compare_Switches(Tuning_Knob_Controls.RIGHT_SWITCH, index))
                {
                    Tuning_Knob_Controls.Button_right_function = (byte)index;
                    //Tuning_Knob_Controls.Active_Functions.Right_switch = index;
                    Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.RIGHT_SWITCH] = (sbyte)index;
                    //transceiver_display = (byte)(30 + index);
                    transceiver_display = (byte)(0x30 | index);
                    Tuning_Knob_Controls.Star_Position.Right = transceiver_display;
                    MonitorTextBoxText(" Right Button Star Position: 0x" + Tuning_Knob_Controls.Star_Position.Right.ToString("X"));
                    oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_TRANSCEIVER_DISPLAY, transceiver_display);
                    Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.RIGHT_SWITCH] = (sbyte)index;
                    buf[1] = (byte)index;
                    oCode.SendCommand_MultiByte(txsocket, txtarget,
                                               Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
                    MFC_C_label38.Text = "C: " + Right_Button_comboBox4.Text;
                    Tuning_Knob_Controls.Button_Text.Button_right_text = Right_Button_comboBox4.Text;
                    //Update_MFC_Init_File();
                }
                else
                {
                    Right_Button_comboBox4.SelectedIndex =
                        Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.RIGHT_SWITCH];
                }
            }
        }

        private void Knob_comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            byte[] buf = new byte[2];
            if (oCode.isLoading) return;
            int index = 0;
            byte transceiver_display = 0;
            buf[0] = Master_Controls.Extended_Commands.CMD_SET_KNOB_SWITCH;
            if (!Master_Controls.code_triggered)
            {
                index = Knob_comboBox1.SelectedIndex;
                if (Compare_Switches(Tuning_Knob_Controls.KNOB_SWITCH, index))
                {
                    Tuning_Knob_Controls.Knob_switch_function = (byte)index;
                    //Tuning_Knob_Controls.Active_Functions.Knob_switch = index;

                    Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.KNOB_SWITCH] = (sbyte)index;
                    //transceiver_display = (byte)index;
                    transceiver_display = (byte)(0x00 | index);
                    Tuning_Knob_Controls.Star_Position.Knob = transceiver_display;
                    oCode.SendCommand(txsocket, txtarget, Tuning_Knob_Controls.CMD_SET_TRANSCEIVER_DISPLAY, transceiver_display);
                    buf[1] = (byte)index;
                    oCode.SendCommand_MultiByte(txsocket, txtarget,
                                               Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
                    //Update_MFC_Init_File();
                    Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.KNOB_SWITCH] = (sbyte)index;
                    MFC_Knob_label38.Text = "K: " + Knob_comboBox1.Text;
                }
                else
                {
                    Knob_comboBox1.SelectedIndex = 
                        Tuning_Knob_Controls.previous_switch_table[Tuning_Knob_Controls.KNOB_SWITCH];
                }
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        /*private void txholdlabel8_Click(object sender, EventArgs e)
        {

        }
        */
        private void semicheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (oCode.oldSemiBreakin == 0)
            {
                oCode.oldSemiBreakin = 1;
            }
            else
            {
                oCode.oldSemiBreakin = 0;
            }
            oCode.SendCommand(txsocket, txtarget, oCode.SET_SEMI_BREAKIN, (short)oCode.oldSemiBreakin);
        }

        private void CW_Pitch_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading == true) return;
            if (!Master_Controls.code_triggered)
            {
                Filter_control.CW_Pitch_Index = CW_Pitch_listBox1.SelectedIndex;
                switch (Filter_control.CW_Pitch_Index)
                {
                    case 0:
                        Filter_control.CW_Pitch = 400;
                        break;

                    case 1:
                        Filter_control.CW_Pitch = 500;
                        break;

                    case 2:
                        Filter_control.CW_Pitch = 600;
                        break;

                    case 3:
                        Filter_control.CW_Pitch = 700;
                        break;

                    case 4:
                        Filter_control.CW_Pitch = 800;
                        break;
                }
                oCode.SendCommand(txsocket, txtarget, Filter_control.CMD_SET_CW_BW, (short)Filter_control.CW_Bw);
                oCode.SendCommand(txsocket, txtarget, Filter_control.CMD_SET_CW_PITCH,
                    (short)Filter_control.CW_Pitch_Index);
            }
        }

        private void label35_Click(object sender, EventArgs e)
        {

        }

        /*private void txholdhScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (oCode.isLoading == true) return;
            int txholdValue = txholdhScrollBar1.Value;
            if (txholdValue == oCode.oldTxHold) return;           // nothing happens if value doesn't change from previous

            oCode.oldTxHold = txholdValue;
            //txholdlabel8.Text = Convert.ToString(txholdValue) + " Tx Hold";
            oCode.SendCommand(txsocket, txtarget, oCode.SET_TX_HOLD, (short)txholdhScrollBar1.Value);
        }*/

        private void AMP_Band_label5_Click(object sender, EventArgs e)
        {

        }

        private void CW_Hold_numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            int txholdValue = (int)CW_Hold_numericUpDown2.Value;
            if (txholdValue == oCode.oldTxHold) return;           // nothing happens if value doesn't change from previous

            oCode.oldTxHold = txholdValue;
            oCode.SendCommand(txsocket, txtarget, oCode.SET_TX_HOLD, (short)txholdValue);
        }

        private void CW_Mode_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;
            if (oCode.isLoading == true) return;
            index = CW_Mode_listBox1.SelectedIndex;
            oCode.SendCommand(txsocket, txtarget, oCode.SET_IAMBIC_MODE, (short)index);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int index = 0;
            if (oCode.isLoading == true) return;
            index = (int) numericUpDown1.Value;
            oCode.SendCommand(txsocket, txtarget, oCode.SET_WPM, (short)index);
        }

        private void CW_Paddle_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;
            if (oCode.isLoading == true) return;
            index = CW_Paddle_listBox1.SelectedIndex;
            oCode.SendCommand(txsocket, txtarget, oCode.SET_CW_PADDLE, (short)index);
        }

        private void CW_Lag_numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            int index = 0;
            index = (int)CW_Lag_numericUpDown2.Value;
            oCode.SendCommand(txsocket, txtarget, oCode.SET_LAG, (short)index);
        }

        private void CW_Weight_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;
            short weight = 0;
            if (oCode.isLoading == true) return;
            index = CW_Weight_listBox1.SelectedIndex;
            switch (index)
            {
                case 0:
                    weight = 25;
                    break;
                case 1:
                    weight = 50;
                    break;
                case 2:
                    weight = 75;
                    break;

            }
            oCode.SendCommand(txsocket, txtarget, oCode.SET_WEIGHT, (short)weight);
        }

        private void CW_Space_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;
            if (oCode.isLoading == true) return;
            index = CW_Space_listBox1.SelectedIndex;
            index++;
            oCode.SendCommand(txsocket, txtarget, oCode.SET_SPACING, (short)index);
        }

        private void CW_Type_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;
            if (oCode.isLoading == true) return;
            index = CW_Type_listBox1.SelectedIndex;
            oCode.SendCommand(txsocket, txtarget, oCode.SET_IAMBIC_TYPE, (short)index);
        }

        private void Main_vuMeter1_Load_1(object sender, EventArgs e)
        {

        }

        private void Freq_Cal_checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (Frequency_Calibration_controls.Freq_Cal_Loose == false)
            {
                Frequency_Calibration_controls.Freq_Cal_Loose = true;
                oCode.SendCommand32(txsocket, txtarget, Frequency_Calibration_controls.CMD_SET_CAL_LOOSE, 1);
            }
            else
            {
                Frequency_Calibration_controls.Freq_Cal_Loose = false;
                oCode.SendCommand32(txsocket, txtarget, Frequency_Calibration_controls.CMD_SET_CAL_LOOSE, 0);
            }
        }

        private void Freq_Cal_Reset_button4_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            DialogResult ret = MessageBox.Show("Selecting RESET will reset the calibration to default values\r\n" +
                "Do you wish to proceed? ", "MSCC", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            if (ret == DialogResult.Yes)
            {
                if (!Frequency_Calibration_controls.Calibration_In_Progress)
                {
                    if (!Frequency_Calibration_controls.standard_carrier_selected)
                    {
                        MessageBox.Show("No Standard Carrier Selected. Select a Standard Carrier", "MSCC",
                                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }
                    if (Filter_control.CW_Pitch != 600)
                    {
                        MessageBox.Show("CW Pitch must be set to 600", "MSCC",
                                       MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }
                    oCode.SendCommand32(txsocket, txtarget, Frequency_Calibration_controls.CMD_SET_CAL_RESET, 1);
                    Frequency_Calibration_controls.Display_Wait_count = 9;
                    Frequency_Calibration_controls.Display_Wait = true;
                    Frequency_Calibration_controls.Reset = true;
                    Freq_Cal_timer4.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Frequency Calibration Check In Progress.", "MSCC",
                                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            }
        }

        /*private void Calibration_Progress_Bar_Click(object sender, EventArgs e)
        {

        }*/

        private void Freq_CAl_Progress_Lable_Click(object sender, EventArgs e)
        {

        }

        private void Compression_Level_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value;
            if (oCode.isLoading) return;
            value = Compression_Level_hScrollBar1.Value;
            if (Volume_Controls.Previous_Compression_Value == value) return;
            Volume_Controls.Previous_Compression_Value = (short)value;
            Compression_label44.Text = Convert.ToString(Volume_Controls.Previous_Compression_Value) + " db";
            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_COMPRESSION_LEVEL, Volume_Controls.Previous_Compression_Value);
        }

        private void Compression_button2_Click(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (Volume_Controls.Compression_State == 0)
            {
                Volume_Controls.Compression_State = 1;
                Set_Button_Color(true, Compression_button2);
                Set_Button_Color(true, Compression_button4);
                Compression_button2.Text = "Compression ON";
            }
            else
            {
                Volume_Controls.Compression_State = 0;
                Set_Button_Color(false, Compression_button2);
                Set_Button_Color(false, Compression_button4);
                Compression_button2.Text = "Compression OFF";
            }
            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_COMPRESSION_STATE, Volume_Controls.Compression_State);
        }

        private void Mic_Gain_label2_Click(object sender, EventArgs e)
        {

        }

        private void Mic_Gain_Step_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            Settings.Default.Mic_Gain_Step = (short)Mic_Gain_Step_listBox1.SelectedIndex;
            RPi_Settings.Volume_Settings.Mic_Pre_Gain = Settings.Default.Mic_Gain_Step;
            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_GAIN, Settings.Default.Mic_Gain_Step);
            Thread.Sleep(100);
            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_VOLUME, Volume_Controls.Mic_Volume);
        }

        private void Power_Meter_Hold_ValueChanged_1(object sender, EventArgs e)
        {
            byte[] buf = new byte[2];
            int index;
            if (oCode.isLoading) return;
            buf[0] = Master_Controls.Extended_Commands.CMD_SET_METER_HOLD;
            index = (int)Power_Meter_Hold.Value;
            Settings.Default.SWR_Meter_Hold = index;
            buf[1] = (byte)index;
            oCode.SendCommand_MultiByte(txsocket, txtarget,Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
        }

        private void RPi_Temperature_label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            //this.Size = new Size(800, 480);
        }

        private void fontDialog1_Apply(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult ret = MessageBox.Show("Close MSCC", "MSCC", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ret == DialogResult.Yes)
            {
                Master_Controls.Shutdown = true;
                if (Master_Controls.Initialize_network_status == true)
                {
                    oCode.SendCommand(Panadapter_Controls.txsocket, Panadapter_Controls.txtarget, oCode.CMD_SET_STOP, 1);
                }
                else
                {
                    StartUP_label44.Text = "MSCC is Shutting Down\r\nPlease Wait";
                    StartUP_label44.Enabled = true;
                    StartUP_label44.Visible = true;
                }
                Thread.Sleep(1000);
                Application.Exit();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private bool _dragging = false;
        private Point _start_point = new Point(0, 0);

        private void Move_Window(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;  // _dragging is your variable flag
                _start_point = new Point(e.X, e.Y);
            }
        }

        private void Move_Window_MouseUP(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void Move_Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this._start_point.X, p.Y - this._start_point.Y);
            }
        }

        private void Audio_Digital_button3_Click_1(object sender, EventArgs e)
        {
            if (oCode.isLoading) return;
            if (!Settings.Default.Mic_Is_Digital)
            {
                Audio_Digital_button3.Text = "D";
                Settings.Default.Mic_Is_Digital = true;
                Compression_button4.Enabled = false;
                Compression_button2.Enabled = false;
                MonitorTextBoxText(" Audio_Digital_button3: " + Convert.ToString(Settings.Default.Mic_Is_Digital));
                MicVolume_hScrollBar1.Value = Settings.Default.Digital_Volume;
                Microphone_textBox2.Text = Convert.ToString(Settings.Default.Digital_Volume);
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_VOLUME, Settings.Default.Digital_Volume);
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_COMPRESSION_STATE, 0);
            }
            else
            {
                Audio_Digital_button3.Text = "V";
                Compression_button4.Enabled = true;
                Compression_button2.Enabled = true;
                Settings.Default.Mic_Is_Digital = false;
                MonitorTextBoxText(" Audio_Digital_button3: " + Convert.ToString(Settings.Default.Mic_Is_Digital));
                MicVolume_hScrollBar1.Value = Settings.Default.Voice_Volume;
                Microphone_textBox2.Text = Convert.ToString(Settings.Default.Voice_Volume);
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_MIC_VOLUME, Settings.Default.Voice_Volume);
                oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_COMPRESSION_STATE, Volume_Controls.Compression_State);
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Window_Refresh_timer_Tick(object sender, EventArgs e)
        {
            if (Master_Controls.Main_Tab_Active)
            {

            }
        }

        private void Time_checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (Time_checkBox2.Checked == true)
            {
                Time_display_label33.Visible = true;
                Local_Date_label46.Visible = true;
                Time_display_UTC_label34.Visible = true;
                UTC_Date_label46.Visible = true;
                Settings.Default.Time_Check_Box = true;
                RPi_Settings.Time_Display = 1;
            }
            else
            {
                Time_display_label33.Visible = false;
                Local_Date_label46.Visible = false;
                Time_display_UTC_label34.Visible = false;
                UTC_Date_label46.Visible = false;
                Settings.Default.Time_Check_Box = false;
                RPi_Settings.Time_Display = 0;
            }
            //RPi_Settings.RPi_Needs_Updated = true;
        }

        private void ritfreqtextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }


        //******************** Delay providing data to graphics displays until one complete cycle

        public static short Graphics_Displays_Startup = 0; // This updated in OnUdpData

        //******************* S Meter Routines ***************************************//

        public void Initialize_Smeter()
        {
            Peak_hold_on_offlistBox1.SelectedIndex = 1;
            Peak_hold_on_offlistBox1_SelectedIndexChanged(null, null);
            Peak_Hold_listBox1.SelectedIndex = 2;
            Peak_Hold_listBox1_SelectedIndexChanged(null, null);
            Smeter_controls.Smeter.smeter_dial[0] = "[S]";
            Smeter_controls.Smeter.smeter_dial[1] = "1";
            Smeter_controls.Smeter.smeter_dial[2] = "3";
            Smeter_controls.Smeter.smeter_dial[3] = "5";
            Smeter_controls.Smeter.smeter_dial[4] = "7";
            Smeter_controls.Smeter.smeter_dial[5] = "9";
            Smeter_controls.Smeter.smeter_dial[6] = "+20";
            Smeter_controls.Smeter.smeter_dial[7] = "+40";
            Smeter_controls.Smeter.smeter_dial[8] = "+60";

            Smeter_controls.Smeter.vu_dial[0] = "-20";
            Smeter_controls.Smeter.vu_dial[1] = "-10";
            Smeter_controls.Smeter.vu_dial[2] = "-7";
            Smeter_controls.Smeter.vu_dial[3] = "-5";
            Smeter_controls.Smeter.vu_dial[4] = "-3";
            Smeter_controls.Smeter.vu_dial[5] = "0";
            Smeter_controls.Smeter.vu_dial[6] = "+1";
            Smeter_controls.Smeter.vu_dial[7] = "+2";
            Smeter_controls.Smeter.vu_dial[8] = "+3";
            Smeter_controls.Smeter.vu_dial[9] = " ";

            Smeter_controls.Smeter.power_dial_QRP[0] = "0";
            Smeter_controls.Smeter.power_dial_QRP[1] = "1";
            Smeter_controls.Smeter.power_dial_QRP[2] = "2";
            Smeter_controls.Smeter.power_dial_QRP[3] = "3";
            Smeter_controls.Smeter.power_dial_QRP[4] = "4";
            Smeter_controls.Smeter.power_dial_QRP[5] = "5";
            Smeter_controls.Smeter.power_dial_QRP[6] = "6";


            Smeter_controls.Smeter.power_dial_QRO[0] = "0";
            Smeter_controls.Smeter.power_dial_QRO[1] = "20";
            Smeter_controls.Smeter.power_dial_QRO[2] = "40";
            Smeter_controls.Smeter.power_dial_QRO[3] = "60";
            Smeter_controls.Smeter.power_dial_QRO[4] = "80";
            Smeter_controls.Smeter.power_dial_QRO[5] = "100";
            Smeter_controls.Smeter.power_dial_QRO[6] = "110";

            Smeter_controls.Smeter.swr_dial[0] = "1.0";
            Smeter_controls.Smeter.swr_dial[1] = "1.2";
            Smeter_controls.Smeter.swr_dial[2] = "1.4";
            Smeter_controls.Smeter.swr_dial[3] = "1.6";
            Smeter_controls.Smeter.swr_dial[4] = "1.8";
            Smeter_controls.Smeter.swr_dial[5] = "2.0";
            Smeter_controls.Smeter.swr_dial[6] = "2.5";
            Smeter_controls.Smeter.swr_dial[7] = "3.0";
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

        public void Update_Smeter(Int32 value)
        {
            Main_Smeter_controls.SMeter_value = value;
            Smeter_controls.smeter_value = value;
        }

        private void vuMeter1_Load(object sender, EventArgs e)
        {

        }

        public void Set_ALC_Meter(Int32 alcValue)
        {

        }

        private void Peak_Hold_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

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

        private void ALC_Meter_Load(object sender, EventArgs e)
        {

        }

        private void Meter_Mode_button8_Click(object sender, EventArgs e)
        {
            Settings.Default.Meter_Mode++;
            if (Settings.Default.Meter_Mode > 6)
            {
                Settings.Default.Meter_Mode = Smeter_controls.ALC_MODE;
            }
            switch (Settings.Default.Meter_Mode)
            {
                case Smeter_controls.POWER_MODE:
                    Meter_Mode_button8.Text = "PWR";
                    break;
                case Smeter_controls.SWR_MODE:
                    Meter_Mode_button8.Text = "SWR";
                    break;
                case Smeter_controls.ALC_MODE:
                    Meter_Mode_button8.Text = "ALC";
                    break;
            }
            RPi_Settings.Meter_Mode = Settings.Default.Meter_Mode;
            //RPi_Settings.RPi_Needs_Updated = true;
        }

        private void Configure_Meter()
        {
            if (Smeter_controls.Current_Mode != Settings.Default.Meter_Mode)
            {
                MonitorTextBoxText(" Configure_Meter: Current_mode: " + Convert.ToString(Smeter_controls.Current_Mode) +
                                                      " Default.Meter: " + Convert.ToString(Settings.Default.Meter_Mode));
                Smeter_controls.Current_Mode = Settings.Default.Meter_Mode;
                vuMeter1.Level = 0;
                Power_Value_label2.Visible = false;
                label9.Visible = false;
                switch (Settings.Default.Meter_Mode)
                {
                    case Smeter_controls.POWER_MODE:
                        switch (Master_Controls.QRP_Mode)
                        {
                            case true:
                                Meter_Mode_button8.Text = "PWR";
                                vuMeter1.SuspendLayout();
                                vuMeter1.MeterScale = VU_MeterLibrary.MeterScale.Analog;
                                vuMeter1.VuText = "Watts";
                                vuMeter1.TextInDial = Smeter_controls.Smeter.power_dial_QRP;
                                vuMeter1.Level = 0;
                                vuMeter1.Led1Count = 4;
                                vuMeter1.Led2Count = 0;
                                vuMeter1.Led3Count = 2;
                                vuMeter1.LevelMax = 110;
                                vuMeter1.ResumeLayout();
                                MonitorTextBoxText(" Meter Set to QRP mode");
                                break;
                            default:
                                Meter_Mode_button8.Text = "PWR";
                                vuMeter1.SuspendLayout();
                                vuMeter1.MeterScale = VU_MeterLibrary.MeterScale.Analog;
                                vuMeter1.VuText = "Watts";
                                vuMeter1.TextInDial = Smeter_controls.Smeter.power_dial_QRO;
                                vuMeter1.Level = 0;
                                vuMeter1.Led1Count = 6;
                                vuMeter1.Led2Count = 2;
                                vuMeter1.Led3Count = 2;
                                vuMeter1.LevelMax = 110;
                                vuMeter1.ResumeLayout();
                                break;

                        }
                        break;
                    case Smeter_controls.SWR_MODE:
                        vuMeter1.SuspendLayout();
                        vuMeter1.MeterScale = VU_MeterLibrary.MeterScale.Analog;
                        vuMeter1.VuText = "SWR";
                        vuMeter1.TextInDial = Smeter_controls.Smeter.swr_dial;
                        vuMeter1.Level = 0;
                        vuMeter1.Led1Count = 4;
                        vuMeter1.Led2Count = 3;
                        vuMeter1.Led3Count = 3;
                        vuMeter1.LevelMax = 3;
                        vuMeter1.ResumeLayout();
                        break;
                    case Smeter_controls.ALC_MODE:
                        vuMeter1.SuspendLayout();
                        vuMeter1.MeterScale = VU_MeterLibrary.MeterScale.Analog;
                        vuMeter1.VuText = "ALC";
                        vuMeter1.TextInDial = Smeter_controls.Smeter.vu_dial;
                        vuMeter1.Level = 0;
                        vuMeter1.Led1Count = 6;
                        vuMeter1.Led2Count = 0;
                        vuMeter1.Led3Count = 4;
                        vuMeter1.LevelMax = 700;
                        vuMeter1.ResumeLayout();
                        break;
                }
            }
        }

        private void Display_ALC_Value()
        {
            int vu_display_value = 0;
            int power_display_value = 0;

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
                if (power_display_value > 105)
                {
                    Power_Value_label2.ForeColor = System.Drawing.Color.Red;
                    Power_Value_label2.BackColor = System.Drawing.Color.White;
                }
                else
                {
                    Power_Value_label2.BackColor = System.Drawing.Color.Black;
                    Power_Value_label2.ForeColor = Color.White;
                }
                Power_Value_label2.Text = Convert.ToString(power_display_value) + " %";
            }
            //Write_Debug_Message(" vuMeter Value: " + Convert.ToString(vu_display_value));
        }

        private void Display_Smeter_Value()
        {
            int db_value = 0;

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
            vuMeter1.Level = Smeter_controls.smeter_display_value;
            db_value = Smeter_controls.smeter_value;
            Power_Value_label2.Text = Convert.ToString(db_value);
        }

        private void Display_Power_Value()
        {
            vuMeter1.Level = Smeter_controls.Power_Value;
            //MonitorTextBoxText(" Display_Power_Value: " + Convert.ToString(Smeter_controls.Power_Value));
        }

        private void Display_SWR_Value()
        {
            vuMeter1.Level = (int)Smeter_controls.SWR_Value;
        }

        private void Smeter_Timer_Tick(object sender, EventArgs e)
        {

            if (!Master_Controls.Transmit_Mode)
            {
                if (Smeter_controls.Current_Mode != Smeter_controls.S_METER_MODE)
                {
                    Meter_Mode_button8.Visible = false;
                    vuMeter1.MeterScale = VU_MeterLibrary.MeterScale.Analog;
                    vuMeter1.SuspendLayout();
                    vuMeter1.VuText = "S Meter";
                    vuMeter1.TextInDial = Smeter_controls.Smeter.smeter_dial;
                    vuMeter1.Led1Count = 9;
                    vuMeter1.Led2Count = 0;
                    vuMeter1.Led3Count = 5;
                    Power_Value_label2.Visible = true;
                    Power_Value_label2.BackColor = Color.Black;
                    Power_Value_label2.ForeColor = Color.White;
                    label9.Visible = true;
                    vuMeter1.LevelMax = 145;
                    vuMeter1.ResumeLayout();
                    Smeter_controls.Current_Mode = Smeter_controls.S_METER_MODE;
                }
            }
            else
            {
                Power_Value_label2.Visible = false;
                label9.Visible = false;
                Configure_Meter();
                Meter_Mode_button8.Visible = true;
            }

            switch (Smeter_controls.Current_Mode)
            {
                case Smeter_controls.ALC_MODE:
                    Display_ALC_Value();
                    break;
                case Smeter_controls.POWER_MODE:
                    Display_Power_Value();
                    break;
                case Smeter_controls.SWR_MODE:
                    Display_SWR_Value();
                    break;
                case Smeter_controls.S_METER_MODE:
                    Display_Smeter_Value();
                    break;
            }
        }

        private void Peak_Hold_listBox1_SelectedIndexChanged_2(object sender, EventArgs e)
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

        private void Peak_hold_on_offlistBox1_SelectedIndexChanged_1(object sender, EventArgs e)
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

        private void Power_Value_label2_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            switch (VFO_Controls.VFO_A)
            {
                case true:
                    groupBox3.Text = "VFO B";
                    VFO_Controls.VFO_A = false;
                    break;
                default:
                    groupBox3.Text = "VFO A";
                    VFO_Controls.VFO_A = true;
                    break;
            }
        }

        private void Peak_Needle_checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (Peak_Needle_checkBox2.Checked)
            {
                vuMeter1.PeakHold = true;
                Settings.Default.Meter_Peak_Needle = true;
                RPi_Settings.Peak_Needle = 1;
            }
            else
            {
                vuMeter1.PeakHold = false;
                Settings.Default.Meter_Peak_Needle = false;
                RPi_Settings.Peak_Needle = 0;
            }
            //RPi_Settings.RPi_Needs_Updated = true;
        }

        private void Peak_Needle_Delay_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int value;

            value = Peak_Needle_Delay_listBox1.SelectedIndex;
            Settings.Default.Meter_Peak_Hold = (byte)value;
            switch (value)
            {
                case 0:
                    vuMeter1.Peakms = 1000;
                    break;
                case 1:
                    vuMeter1.Peakms = 2000;
                    break;
                case 2:
                    vuMeter1.Peakms = 3000;
                    break;
            }
            RPi_Settings.Peak_Needle_Delay_Index = value;
            //RPi_Settings.RPi_Needs_Updated = true;
        }

        private void Meter_Peak_Needle_Color_SelectedIndexChanged(object sender, EventArgs e)
        {
            int value;

            value = Meter_Peak_Needle_Color.SelectedIndex;
            Settings.Default.Meter_Peak_Needle_Color = (byte)value;
            switch (value)
            {
                case 0:
                    vuMeter1.PeakNeedleColor = Color.Red;
                    break;
                case 1:
                    vuMeter1.PeakNeedleColor = Color.Blue;
                    break;
                case 2:
                    vuMeter1.PeakNeedleColor = Color.Green;
                    break;
                case 3:
                    vuMeter1.PeakNeedleColor = Color.Yellow;
                    break;
                case 4:
                    vuMeter1.PeakNeedleColor = Color.White;
                    break;
                case 5:
                    vuMeter1.PeakNeedleColor = Color.Black;
                    break;
            }
            RPi_Settings.Peak_Needle_Color_Index = value;
            //RPi_Settings.RPi_Needs_Updated = true;
        }

        private void Main_Power_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value = 0;
            value = Main_Power_hScrollBar1.Value;
            if (Power_Controls.Main_Tab_Power == value) return;
            Power_Controls.Main_Tab_Power = (short)value;
            Master_Controls.code_triggered = true;
            switch (oCode.modeswitch)
            {
                case 0:
                    Power_hScrollBar1_Scroll(null, null);
                    break;
                case 1:
                    Power_hScrollBar1_Scroll(null, null);
                    break;
                case 2:
                    Power_hScrollBar1_Scroll(null, null);
                    break;
                case 3:
                    CW_Power_hScrollBar1_Scroll(null, null);
                    break;
            }
        }

        private void Minimize_checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            Minimize_checkBox2.CheckedChanged -= Minimize_checkBox2_CheckedChanged;
            Minimize_checkBox2.Checked = false;
            Minimize_checkBox2.CheckedChanged += Minimize_checkBox2_CheckedChanged;
        }

        private void Volume_Attn_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int value = 0;
            value = Volume_Attn_listBox1.SelectedIndex;
            Settings.Default.Volume_Attn_Index = value;
            RPi_Settings.Volume_Settings.Volume_ATTN_Index = value;
            //RPi_Settings.RPi_Needs_Updated = true;
            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_VOLUME_ATTN, (short)Settings.Default.Volume_Attn_Index);
        }


        //******************************************Waterfall***********************************************
        public static Thread Waterfall_thread = new Thread(new ThreadStart(Display_GDI.Waterfall_Callback));

        //public static int zero_gain = 410;
        public const int low_thread_hold = 500;
        public static Size pic_window_size;
        public static Size previous_form_size;

        public static Size form_window_size;
        public static Size max_size;
        public static Size panel_size;
        public static int H = 194;
        public static int W = (Panadapter_Controls.Max_X * 2);
        //private const int CP_NOCLOSE_BUTTON = 0x200;
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

        public void Restart_Waterfall()
        {
            bool close_status = false;

            waterfall_run = false;
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
                waterfall_run = true;
            }
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
                    if (picture_width <= 0)
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
                    //    Convert.ToString(ten_thousand) + Convert.ToString(thousand);

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
                    //   Convert.ToString(ten_thousand) + Convert.ToString(thousand);

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
                    //Convert.ToString(ten_thousand) + Convert.ToString(thousand);

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

        private void Initialize_Waterfall()
        {
            String Message = " Initialize_Waterfall -> Called";
            Size pic_size = new Size(0, 0);

            Write_Debug_Message(Message);
            form_window_size = this.Size;
            previous_form_size = this.Size;
            //pic_size = this.Size;
            //pic_size.Width = pic_size.Width - 8;
            //pic_size.Height = picWaterfall.Size.Height;
            //picWaterfall.Size = pic_size;
            //this.picWaterfall.ClientSize = pic_size;
            pic_window_size = this.picWaterfall.ClientSize;
            this.picWaterfall.BackColor = Color.Transparent;
            Window_controls.Waterfall_Controls.Pic_Waterfall_graphics = this.picWaterfall.CreateGraphics();
            this.picWaterfall.Show();

            Display_GDI.Window_size = pic_window_size;
            Message = " Initialize_Waterfall -> Form Size -> W: " + Convert.ToString(picWaterfall.Width) + " H: " +
                     Convert.ToString(picWaterfall.Height);
            Write_Debug_Message(Message);
            Message = " Initialize_Waterfall -> Pic Size -> W: " + Convert.ToString(picWaterfall.ClientSize.Width) + " H: "
                + Convert.ToString(picWaterfall.ClientSize.Height);
            Write_Debug_Message(Message);
            Window_controls.Waterfall_Controls.Display_Operation_Complete = true;
            Display_GDI.WaterfallDataReady = false;
            Display_GDI.Init();
            Display_GDI.MOX = false;
            Display_GDI.CurrentDisplayMode = DisplayMode.WATERFALL;
            Display_GDI.WaterfallLowThreshold = 1.0f;
            Display_GDI.WaterfallHighThreshold = 3090.0f;
            Waterfall_thread.Name = "Waterfall";
            Waterfall_thread.Start();
            Message = " Initialize_Waterfall Finished";
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
            //int viewport = 72000;
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
            //Cursor_textBox1.Text = "Cursor: " + Display_freq + " MHz"
            //   + "\r\n" + "VIEWPORT: " + Convert.ToString(viewport);

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
                    //if (Mouse_location.X >= pic_window_size.Width - Cursor_textBox1.Size.Width)
                    //{
                    //    Cursor_Location.X = Mouse_location.X - Cursor_textBox1.Size.Width;
                    //}
                    //else
                    //{
                    Cursor_Location.X = Mouse_location.X + 20;
                    //}
                    Display_GDI.CursorPosition = Cursor_Location.X;
                    //Cursor_textBox1.Location = Cursor_Location;
                    Previous_Mouse_location = Mouse_location;
                    Cursor_timer2.Enabled = true;
                    //Cursor_textBox1.Show();
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
            //Cursor_textBox1.Hide();
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
                //Cursor_textBox1.Show();
            }
        }

        private void Waterfall_timer_Tick(object sender, EventArgs e)
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
            if (previous_filter_size != Window_controls.Waterfall_Controls.Markers.band_marker_high ||
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

        private void Cursor_textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Cursor_timer2_Tick(object sender, EventArgs e)
        {
            //Cursor_textBox1.Hide();
            Write_Debug_Message(" Cursor_timer. Timeout");
            Cursor_timer2.Enabled = false;
        }


        //*******************************************************Spectrum***********************************************
        static PointPairList list = new PointPairList();
        private const string V = " Send log files to Multus SDR, LLC.";
        public static GraphPane myPane;
        public static LineItem myCurve;
        public static Size zed_size = new Size();
        public static Point zed_location = new Point();
        public static LineObj cursor_low = new LineObj();
        public static LineObj cursor_center = new LineObj();
        public static LineObj cursor_high = new LineObj();
        public static LineObj cursor_user = new LineObj();
        public static bool spectrum_first_pass = true;
        public static int gain_value = 0;
        public static int cw_offset = 0;
        public static String Cursor_message = "";
        public static bool spectrum_initialized = false;
        public static bool previous_spectrum_zoom = false;
        public static bool spectrum_operation_complete = true;
        public static String spectrum_mode = "AM";

        private void Zedgraph_Control_Load(object sender, EventArgs e)
        {

        }

        public void Update_Spectrum()
        {
            if (spectrum_initialized == true)
            {
                Run_Spectrum();
            }
        }

        public static string getBetween_spectrum(string strSource, string strStart, string strEnd)
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
            Panadapter_Controls.Filter_Markers.Window_Size = Zedgraph_Control.Width;
            Panadapter_Controls.Filter_Markers.Display_Center = 399;
            cursor_marker = ((int)high_cut / (Panadapter_Controls.Filter_Markers.DISPLAY_BANDWIDTH / (int)zoom));
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
                                                                                            Panadapter_Controls.Filter_Markers.CW_Offset;
                    Panadapter_Controls.Filter_Markers.G_band_marker_low = 0;
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
            }
        }

        public bool Init_Spectrum_values()
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
            String Message = "Main_Form -> Init_Spectrum_values started";
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
                temp_string = getBetween_spectrum(line, "PANADAPTER_GAIN=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    gain_value = temp_int;
                    Message = " PANADAPTER_GAIN: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween_spectrum(line, "PANADAPTER_BASE=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    base_value = temp_int;
                    Message = " PANADAPTER_BASE: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }

                temp_string = getBetween_spectrum(line, "PANADAPTER_FILL=", ";");
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
                temp_string = getBetween_spectrum(line, "PANADAPTER_LINE=", ";");
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

                temp_string = getBetween_spectrum(line, "PANADAPTER_MARKER=", ";");
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
                temp_string = getBetween_spectrum(line, "PANADAPTER_BACKGROUND=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    line_value = temp_int;
                    Message = " PANADAPTER_BACKGROUND: " + Convert.ToString(temp_int);
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
                temp_string = getBetween_spectrum(line, "PANADAPTER_CURSOR=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    line_value = temp_int;
                    Message = " PANADAPTER_CURSOR: " + Convert.ToString(temp_int);
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
                temp_string = getBetween_spectrum(line, "AUTO_SNAP_STATUS=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    auto_snap_status = temp_int;
                    Message = " AUTO_SNAP_STATUS: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }
                temp_string = getBetween_spectrum(line, "AUTO_SNAP_INDEX=", ";");
                if (temp_string != "")
                {
                    Int16.TryParse(temp_string, out temp_int);
                    auto_snap_index = temp_int;
                    Message = " AUTO_SNAP_INDEX: " + Convert.ToString(temp_int);
                    Write_Debug_Message(Message);
                }
            }
            file.Close();
            Panadapter_Controls.Spectrum_Base_Line = (base_value);
            Panadapter_Controls.Spectrum_Gain = (6000 - gain_value);
            Display_GDI.WaterfallHighThreshold = 7000.0f - (float)Window_controls.Waterfall_Controls.gain;
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

            //code_triggered = false;
            Message = "Update_User_values Finished";
            Write_Debug_Message(Message);
            return true;
        }

        public void Initialize_Spectrum()
        {
            String Message = " Initialize_Spectrum Starting";
            Write_Debug_Message(Message);
            zed_size = Zedgraph_Control.Size;
            Message = "Zedgraph size: " + Convert.ToString(zed_size);
            Write_Debug_Message(Message);
            CreateGraph(Zedgraph_Control);
            Create_Marker_lines();
            Zedgraph_Control.CursorValueEvent += new ZedGraphControl.CursorValueHandler(myGraph_CursorValueEvent);
            Zedgraph_Control.MouseEnter += new EventHandler(Display_User_Cursor);
            Zedgraph_Control.MouseLeave += new EventHandler(Hide_User_Cursor);
            Panadapter_Controls.Filter_Markers.Window_Size = Zedgraph_Control.Width;
            Zedgraph_Control.MouseClick += new MouseEventHandler(Mouse_Set_freq);
            Zedgraph_Control.IsEnableWheelZoom = true;
            Zedgraph_Control.Visible = true;
            Init_Spectrum_values();
            spectrum_initialized = true;
            Message = " Initialize_Spectrum Finished ";
            Write_Debug_Message(Message);
        }

        void Display_User_Cursor(object sender, EventArgs e)
        {

            cursor_user.IsVisible = true;
        }

        void Hide_User_Cursor(object send, EventArgs e)
        {
            cursor_user.IsVisible = false;
            Display_GDI.CursorPosition = 0;
        }

        public string myGraph_CursorValueEvent(ZedGraphControl send, GraphPane pane, Point mousePt)
        {
            double x, y;
            double start_freq = 0.0;
            int freq = 0;
            double cw_offset = 0.0;


            pane.ReverseTransform(mousePt, out x, out y);
            if (Panadapter_Controls.Mouse_event.x == x && Panadapter_Controls.Mouse_event.y == y)
            {
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
            Panadapter_Controls.Mouse_event.Display_Freq = Convert.ToString(MHz) + "." +
                string.Format("{0:000}", KHz) + "." + string.Format("{0:000}", Hz);
            //Panadapter_Mouse_label1.Text = Panadapter_Controls.Mouse_event.Display_Freq;
            cursor_user.Location.X = Panadapter_Controls.Mouse_event.User_Cursor_x;
            Display_GDI.CursorPosition = (int)Panadapter_Controls.Mouse_event.User_Cursor_x;
            //zedGraphControl2.AxisChange();
            //zedGraphControl2.Refresh();
            Cursor_message = "Cursor: " + Panadapter_Controls.Mouse_event.Display_Freq
                + "\r\n" + "VIEWPORT: " +
                Convert.ToString((Panadapter_Controls.Freq_Bounds.High_Freq - Panadapter_Controls.Freq_Bounds.Low_Freq));
            //return Panadapter_Controls.Mouse_event.Display_Freq;
            return Cursor_message;
        }

        public void Display_Main_Freq_by_Panadapter()
        {
            var thisForm2 = Application.OpenForms.OfType<Main_form>().Single();
            thisForm2.Update_Main_Display();
        }

        public void Set_Spectrum_Frequency(int freq)
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
            //Fine_Tune_hScrollBar1.Value = 0;
            Panadapter_Controls.Fine_Tune_Delta = 0;
            Set_Spectrum_Frequency((int)Panadapter_Controls.Graph_Freq);
            //MessageBox.Show("Mouse Event. X Value: " + Convert.ToString(Panadapter_Controls.Graph_Freq),"MSCC", 
            //  MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void zg1_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            foreach (GraphObj graphObj in sender.GraphPane.GraphObjList)
            {
                if (graphObj.Tag.ToString().Contains("cursorY"))
                {
                    graphObj.Location.Height = 0; // just to be sure
                    graphObj.Location.Width = (Zedgraph_Control.GraphPane.XAxis.Scale.Max -
                        Zedgraph_Control.GraphPane.XAxis.Scale.Min) * 3;
                    graphObj.Location.X = Zedgraph_Control.GraphPane.XAxis.Scale.Min - (graphObj.Location.Width / 3);
                }
                else if (graphObj.Tag.ToString().Contains("cursorX"))
                {
                    graphObj.Location.Width = 0; // just to be sure
                    graphObj.Location.Height = (Zedgraph_Control.GraphPane.YAxis.Scale.Max -
                        Zedgraph_Control.GraphPane.YAxis.Scale.Min) * 3;
                    graphObj.Location.Y = Zedgraph_Control.GraphPane.YAxis.Scale.Min - (graphObj.Location.Height / 3);
                }
            }
        }

        public void Display_Spectrum(object sender, DoWorkEventArgs e)
        {
            int i = 0;
            //int i_shifted = 0;
            int panadapter_y_value;
            int spectrum_base_line_temp = 0;
            double x = 0;
            double y = 0;
            int max_limit = 0;
            float filter_size = 0f;
            int high_temp = 0;
            int MHz = 0;
            int KHz = 0;
            int Hz = 0;
            String Message;
            long time_now;
            long previous_time = 0;
            long receive_time = 0;

            time_now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            receive_time = time_now - previous_time;
            previous_time = time_now;
            Message = " Interation Time (milliseconds): " + Convert.ToString(receive_time);
            if (Master_Controls.Debug_Display)
            {
                Write_Debug_Message(Message);
            }
            bool seq_0 = Panadapter_Controls.Sequence_0_Complete;
            bool seq_1 = Panadapter_Controls.Sequence_1_Complete;
            if (Panadapter_Controls.Sequence_1_Complete && Panadapter_Controls.Sequence_0_Complete)
            {
                if (Panadapter_Controls.Freq_Set_By_Master)
                {
                    Panadapter_Controls.Display_freq = oCode.DisplayFreq;
                    Panadapter_Controls.Fine_Tune_Delta = 0;
                    Panadapter_Controls.Freq_Set_By_Master = false;
                    Panadapter_Controls.Fine_Tuning = false;
                }
                if (Panadapter_Controls.CW_Snap.CW_snap_status == false && Panadapter_Controls.CW_Snap.CW_button == true)
                {
                    //panadapter_control Pan_form = new panadapter_control();
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
                if (Panadapter_Controls.Anti_Alias)
                {
                    Zedgraph_Control.IsAntiAlias = true;
                }
                else
                {
                    Zedgraph_Control.IsAntiAlias = false;
                }
                filter_size = Filter_size(Filter_control.Filter_High_Index);
                Calculate_Band_Marker(Last_used.Current_mode, 0.0f, filter_size);
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
                        spectrum_mode = "CW";
                        break;
                    case 'A':
                        cursor_high.IsVisible = true;
                        cursor_low.IsVisible = true;
                        spectrum_mode = "AM";
                        break;
                    case 'L':
                        cursor_high.IsVisible = false;
                        cursor_low.IsVisible = true;
                        spectrum_mode = "LSB";
                        break;
                    case 'U':
                        cursor_high.IsVisible = true;
                        cursor_low.IsVisible = false;
                        spectrum_mode = "USB";
                        break;
                }

                if (Panadapter_Controls.Previous_mode != Last_used.Current_mode)
                {
                    Graph_name(spectrum_mode);
                    Panadapter_Controls.Previous_mode = Last_used.Current_mode;
                }
                if (Panadapter_Controls.Previous_Filter_Index != Panadapter_Controls.Filter_Index)
                {
                    Graph_name(spectrum_mode);
                    Panadapter_Controls.Previous_Filter_Index = Panadapter_Controls.Filter_Index;
                }

                if (Panadapter_Controls.Display_Operation_Complete == false) return;
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
                //if (Spectrum_Master_Controls.Debug_Display)
                //{
                //   Message = Convert.ToString("Display Panadapter -> Current X min " +
                //       Panadapter_Controls.Freq_Bounds.Current_X_Min) +
                //       " Current X Max " + Convert.ToString(Panadapter_Controls.Freq_Bounds.Current_X_Max);
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
                        MHz = (Panadapter_Controls.Freq_Bounds.Low_Freq) / 1000000;
                        KHz = ((Panadapter_Controls.Freq_Bounds.Low_Freq) - (MHz * 1000000)) / 1000;
                        Hz = (Panadapter_Controls.Freq_Bounds.Low_Freq) - (MHz * 1000000) - (KHz * 1000);

                        MHz = (Panadapter_Controls.Freq_Bounds.High_Freq) / 1000000;
                        KHz = ((Panadapter_Controls.Freq_Bounds.High_Freq) - (MHz * 1000000)) / 1000;
                        Hz = (Panadapter_Controls.Freq_Bounds.High_Freq) - (MHz * 1000000) - (KHz * 1000);


                    }
                }
                myPane.YAxis.Scale.Max = Panadapter_Controls.Spectrum_Gain;
                list.Clear();
                spectrum_base_line_temp = Panadapter_Controls.Spectrum_Base_Line;
                max_limit = Panadapter_Controls.Max_X * 2;
                //i_shifted = 0;
                for (i = 0; i < max_limit; i++)
                {
                    x = (double)i;
                    /*if (i > 300)
                    {
                        i_shifted = i + 4;
                        if (i_shifted < max_limit)
                        {
                            x = (double)(i_shifted);
                        }
                    }*/

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
                    y = (double)panadapter_y_value;
                    list.Add(x, y);
                }

                MHz = (Panadapter_Controls.Display_freq + Panadapter_Controls.Fine_Tune_Delta) / 1000000;
                KHz = ((Panadapter_Controls.Display_freq + Panadapter_Controls.Fine_Tune_Delta) - (MHz * 1000000)) / 1000;
                Hz = (Panadapter_Controls.Display_freq + Panadapter_Controls.Fine_Tune_Delta) - (MHz * 1000000) - (KHz * 1000);
                //Panadapter_Freq_Label1.Text = Convert.ToString(MHz) + "." + string.Format("{0:000}", KHz) + "." + (string.Format("{0:000}", Hz));

                if (spectrum_first_pass)
                {
                    ZoomState oldState = null;
                    ZoomState newState = null;
                    zg1_ZoomEvent(Zedgraph_Control, oldState, newState);
                    //Pan_form.Panadapter_Gain_hScrollBar1_Scroll(null, null);
                    spectrum_first_pass = false;
                }
                Zedgraph_Control.AxisChange();
                Zedgraph_Control.Refresh();
                Panadapter_Controls.Display_Operation_Complete = true;
            }
            else
            {
                if (Master_Controls.Transmit_Mode == false)
                {
                    Message = " Display_Panadapter -> Data Not Ready. Seq0: " +
                        Convert.ToString(seq_0) + " Seq1: " + Convert.ToString(seq_1);
                    Write_Debug_Message(Message);
                }
            }
            spectrum_operation_complete = true;
        }

        public void Run_Spectrum()
        {
            if (spectrum_operation_complete == true)
            {
                try
                {
                    BackgroundWorker display_spectrum = new BackgroundWorker();
                    display_spectrum.DoWork += Display_Spectrum;
                    spectrum_operation_complete = false;
                    display_spectrum.RunWorkerAsync();
                }
                catch
                {
                    Write_Debug_Message(" Run_Spectrum Failed");
                }
            }
        }

        private void CreateGraph(ZedGraphControl zgc)
        {
            String Message = " Create_Graph Starting";
            Write_Debug_Message(Message);
            Panadapter_Controls.Panadapter_Colors.Line_Color = Color.Blue;
            myPane = zgc.GraphPane;
            myCurve = myPane.AddCurve("", list, Panadapter_Controls.Panadapter_Colors.Line_Color,
                                            SymbolType.None);
            // Set the titles and axis labels
            myPane.Title.Text = "";
            myPane.XAxis.Title.Text = "";
            myPane.YAxis.Title.Text = "";
            myPane.Border.IsVisible = false;
            myPane.Legend.IsVisible = false;
            myPane.Title.IsVisible = false;
            myPane.Margin.All = 0;

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
            zgc.AxisChange();
            zgc.Refresh();
            Message = " Create_Graph Finished";
            Write_Debug_Message(Message);
        }

        public void Create_Marker_lines()
        {
            String Message = " Create_Marker_lines Starting";
            Write_Debug_Message(Message);
            Panadapter_Controls.Filter_Markers.Window_Size = Zedgraph_Control.Width;
            //Create the center marker
            cursor_center.Location.Width = 0; // this time width must be zero
            cursor_center.Location.Height = (Zedgraph_Control.GraphPane.YAxis.Scale.Max - Zedgraph_Control.GraphPane.YAxis.Scale.Min) * 3;
            cursor_center.Location.X = Panadapter_Controls.Filter_Markers.Display_Center;
            cursor_center.Location.Y = Zedgraph_Control.GraphPane.YAxis.Scale.Min - (cursor_center.Location.Width / 3);
            cursor_center.Line.Color = Color.Red;
            cursor_center.Line.Width = 2;
            cursor_center.Line.Style = System.Drawing.Drawing2D.DashStyle.DashDot;
            //cursor_center.Line.Style = System.Drawing.Drawing2D.HatchStyle.Cross;
            cursor_center.IsClippedToChartRect = true; // when true, line isn't drawn outside the boundaries of the chart rectangle
            cursor_center.Tag = "cursorX1";
            cursor_center.ZOrder = ZOrder.D_BehindAxis; // sets the order of the cursor in front of the curves, filling and gridlines but behind axis, border and legend. You can choose whatever you like of course.
            Zedgraph_Control.GraphPane.GraphObjList.Add(cursor_center); // just add the cursor to the grapphane and that's it!

            //Create the high marker
            double yAxisLength = Zedgraph_Control.GraphPane.YAxis.Scale.Max - Zedgraph_Control.GraphPane.YAxis.Scale.Min;
            cursor_high.Line.Style = System.Drawing.Drawing2D.DashStyle.Solid;
            cursor_high.IsClippedToChartRect = true;
            cursor_high.Tag = "cursorX2";
            cursor_high.ZOrder = ZOrder.D_BehindAxis;
            cursor_high.Line.Width = 1;
            cursor_high.Line.Color = Color.Black;
            Zedgraph_Control.GraphPane.GraphObjList.Add(cursor_high);

            //Now create the low marker
            double yAxisLength_1 = Zedgraph_Control.GraphPane.YAxis.Scale.Max - Zedgraph_Control.GraphPane.YAxis.Scale.Min;
            cursor_low.Line.Style = System.Drawing.Drawing2D.DashStyle.Solid;
            cursor_low.IsClippedToChartRect = true;
            cursor_low.Tag = "cursorX3";
            cursor_low.ZOrder = ZOrder.D_BehindAxis;
            cursor_low.Line.Width = 1;
            cursor_low.Line.Color = Color.Black;
            Zedgraph_Control.GraphPane.GraphObjList.Add(cursor_low);

            //Now create the user cursor
            double yAxisLength_3 = Zedgraph_Control.GraphPane.YAxis.Scale.Max - Zedgraph_Control.GraphPane.YAxis.Scale.Min;
            cursor_user.Line.Style = System.Drawing.Drawing2D.DashStyle.Solid;
            cursor_user.IsClippedToChartRect = true;
            cursor_user.Tag = "cursorX4";
            cursor_user.ZOrder = ZOrder.D_BehindAxis;
            cursor_user.Line.Width = 1;
            cursor_user.Line.Color = Color.OrangeRed;
            Zedgraph_Control.GraphPane.GraphObjList.Add(cursor_user);

            Zedgraph_Control.AxisChange();
            Zedgraph_Control.Refresh();
            Message = " Create_Marker_lines Finished";
            Write_Debug_Message(Message);
        }
             
        private void picWaterfall_Click_1(object sender, EventArgs e)
        {

        }

        private void picWaterfall_paint(object sender, PaintEventArgs e)
        {
            int cursor_bottom = Main_form.pic_window_size.Height;
            Pen cursor = new Pen(new SolidBrush(Color.White), 1.0f);
            Window_controls.Waterfall_Controls.Pic_Waterfall_graphics.DrawLine(cursor, Display_GDI.CursorPosition, 0,
                Display_GDI.CursorPosition, cursor_bottom);
        }
              
        //************************ Solidus IQBD *****************************************************

        private void IQBD_Select_Band()
        {
            switch (Amplifier_Power_Controls.Band)
            {
                case 160:
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B160);
                    break;
                case 80:
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B80);
                    break;
                case 60:
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B60);
                    break;
                case 40:
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B40);
                    break;
                case 30:
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B30);
                    break;
                case 20:
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B20);
                    break;
                case 17:
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B17);
                    break;
                case 15:
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B15);
                    break;
                case 12:
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B12);
                    break;
                case 10:
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_BAND, IQ_Controls.B10);
                    break;
            }
            MonitorTextBoxText(" IQBD_Select_Band -> current band: " + Convert.ToString(oCode.current_band));
        }

        private void IQBD_ONOFF_Click(object sender, EventArgs e)
        {
            byte[] buf = new byte[2];

            if (oCode.isLoading) return;
            buf[0] = Master_Controls.Extended_Commands.CMD_SET_IQBD_MONITOR;
            if (Amplifier_Power_Controls.Solidus_Band_Selected)
            {
                if (!IQ_Controls.IQBD_MONITOR)
                {
                    oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 0);
                    IQ_Controls.Previous_QRP_Mode = Master_Controls.QRP_Mode;
                    MonitorTextBoxText(" Previous_QRP_Mode: " + Convert.ToString(IQ_Controls.Previous_QRP_Mode));
                    if (Master_Controls.Tuning_Mode || Master_Controls.Transmit_Mode)
                    {
                        oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_TX_ON, 0);
                        if (!Settings.Default.Speaker_MutED)
                        {
                            oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                        }
                    }
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_RIG_TUNE, 0);
                    oCode.SendCommand(txsocket, txtarget, oCode.CMD_SET_MAIN_MODE, Mode.TUNE_mode);
                    IQBD_ONOFF.ForeColor = Color.Red;
                    IQ_Controls.IQ_TX_MODE_ACTIVE = true;
                    IQBD_hScrollBar2.Value = Power_Controls.TUNE_Power;
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_RX_TX, 1);
                    IQBD_Select_Band();
                    buf[1] = 1;
                    oCode.SendCommand_MultiByte(txsocket, txtarget,
                                            Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
                    IQ_Controls.IQ_Calibrating = true;
                    IQ_Controls.IQBD_MONITOR = true;
                    AMP_groupBox3.Enabled = false;
                    AMP_groupBox3.ForeColor = Color.Red;
                    IQBD_Tune_button8.Enabled = true;
                }
                else
                {
                    IQBD_ONOFF.ForeColor = Color.Black;
                    buf[1] = 0;
                    oCode.SendCommand_MultiByte(txsocket, txtarget,
                                            Master_Controls.Extended_Commands.CMD_SET_EXTENDED_COMMAND, buf, buf.Length);
                    IQ_Controls.IQ_Calibrating = false;
                    IQ_Controls.IQBD_MONITOR = false;
                    if (Solidus_Controls.Mia_Status)
                    {
                        AMP_groupBox3.Enabled = true;
                    }
                    AMP_groupBox3.ForeColor = Color.White;
                    IQBD_Tune_button8.Enabled = false;
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 0);
                    IQBD_Tune_button8.ForeColor = Color.Black;
                    switch (IQ_Controls.Previous_QRP_Mode)
                    {
                        case true:
                            oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 0);
                            break;
                        default:
                            oCode.SendCommand(txsocket, txtarget, Master_Controls.CMD_SET_PA_BYPASS, 1);
                            break;
                    }
                }
            }
            else
            {
                DialogResult ret = MessageBox.Show("Select a Band", "MSCC",MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            
        }

        private void IQBD_hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            int value;
            if (oCode.isLoading == true) return;
            value = IQBD_hScrollBar2.Value;
            if (IQ_Controls.Tune_power == (short)value) return;
            IQ_Controls.Tune_power = (short)value;
            oCode.SendCommand(txsocket, txtarget, Power_Controls.CMD_SET_TUNE_POWER, (short)value);
        }

        private void IQBD_Tune_button8_Click(object sender, EventArgs e)
        {
            if (IQ_Controls.IQBD_MONITOR)
            {
                if (!IQ_Controls.Tuning_Mode)
                {
                    oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 1);
                    IQ_Controls.Tuning_Mode = true;
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 1);
                    IQBD_Tune_button8.ForeColor = Color.Red;
                    Master_Controls.Transmit_Mode = true;
                }
                else
                {
                    IQ_Controls.Tuning_Mode = false;
                    oCode.SendCommand(txsocket, txtarget, IQ_Controls.IQ_CALIBRATION_TUNE, 0);
                    IQBD_Tune_button8.ForeColor = Color.Black;
                    Master_Controls.Transmit_Mode = false;
                    if (!Settings.Default.Speaker_MutED)
                    {
                        oCode.SendCommand(txsocket, txtarget, Volume_Controls.CMD_SET_SPEAKER_MUTE, 0);
                    }
                }
            }
            else
            {
                MessageBox.Show("Set IQBD Mode ON", "MSCC", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void IQBD_Monitor_Click(object sender, EventArgs e)
        {

        }

        private void IQBD_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (oCode.isLoading == true) return;
            int value = IQBD_hScrollBar1.Value;
            if (value == IQ_Controls.IQ_Offset) return;

            if (IQ_Controls.IQ_TX_MODE_ACTIVE)
            {
                IQ_Controls.IQ_Offset = value;
                MonitorTextBoxText(" IQBD_hScrollBar1_Scroll TX IQ_Controls.Volume:  " + (short)IQ_Controls.IQ_Offset);
                oCode.SendCommand32(txsocket, txtarget, IQ_Controls.CMD_SET_IQ_OFFSET, (IQ_Controls.IQ_Offset));
            }
            else
            {
                DialogResult ret = MessageBox.Show("Select a Band", "MSCC",
                              MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void IQBD_Apply_button8_Click(object sender, EventArgs e)
        {
            if (IQ_Controls.IQ_TX_MODE_ACTIVE)
            {
                if (IQ_Controls.band_selected)
                {
                    if (IQ_Controls.IQ_TX_MODE_ACTIVE)
                    {
                        {
                            IQBD_Apply_button8.ForeColor = Color.Red;
                            IQBD_Apply_button8.Text = "APPLYING";
                            oCode.SendCommand(txsocket, txtarget, IQ_Controls.CMD_SET_COMMIT_IQ, 0);
                            IQ_Controls.IQ_Calibrating = true;
                            //IQ_Controls.IQ_TX_MODE_ACTIVE = false;
                        }
                    }
                    else
                    {
                        DialogResult ret = MessageBox.Show("TX MODE NOT SET ON \r\nSET TX MODE to ON", "MSCC",
                              MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                else
                {
                    DialogResult ret = MessageBox.Show("Select a Band", "MSCC",
                              MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }

        private void IQBD_groupBox4_Enter(object sender, EventArgs e)
        {

        }

        public void Manage_Solidus_Status()
        {
            MonitorTextBoxText(" Manage_Solidus_Status -> Called ");
            if (Solidus_Controls.Solidus_Status_Set && Solidus_Controls.Mia_Status_Set)
            {
                if (Solidus_Controls.Solidus_Status)
                {
                    IQBD_groupBox4.Enabled = true;
                    IQ_groupBox3.Enabled = false;
                    AMP_groupBox3.Enabled = true;
                }
                else
                {
                    IQBD_groupBox4.Enabled = false;
                    IQ_groupBox3.Enabled = true;
                    AMP_groupBox3.Enabled = false;
                }
                MonitorTextBoxText(" Manage_Solidus_Status -> Mia_Status: " + Convert.ToString(Solidus_Controls.Mia_Status));
                if (!Solidus_Controls.Mia_Status)
                {
                    PA_vButton1.Enabled = false;
                    PA_vButton1.Text = "QRP";
                    PA_vButton1.BackColor = Color.Gainsboro;
                    PA_vButton1.ForeColor = Color.Black;
                    Master_Controls.QRP_Mode = true;
                    AMP_groupBox3.Enabled = false;
                }
                else
                {
                    PA_vButton1.Enabled = true;
                }
            }
            else
            {
                MonitorTextBoxText(" Manage_Solidus_Status -> Status NOT Set -> Solidus_status_Set: " + 
                    Convert.ToString(Solidus_Controls.Solidus_Status_Set) + 
                    " Mia_Status_Set: " + Convert.ToString(Solidus_Controls.Mia_Status_Set));
            }
        }

        private void Solidus_Band_label4_Click(object sender, EventArgs e)
        {

        }

        private void MFC_Click(object sender, EventArgs e)
        {

        }

        private void Solidus_Function_Buttons_Enter(object sender, EventArgs e)
        {

        }

        private void Tuning_Knob_groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void Solidus_Tab_Power_Display_label33_Click(object sender, EventArgs e)
        {

        }

        private void XCRV_Power_Display_label33_Click(object sender, EventArgs e)
        {

        }

        private void AMP_comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int value;
            if (oCode.isLoading) return;
            value = AMP_comboBox1.SelectedIndex;
            switch (value)
            {
                case 0:
                    Amplifier_Power_Controls.Band = 160;
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B160.Freq);
                    break;
                case 1:
                    Amplifier_Power_Controls.Band = 80;
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B80.Freq);
                    break;
                case 2:
                    Amplifier_Power_Controls.Band = 60;
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B60.Freq);
                    break;
                case 3:
                    Amplifier_Power_Controls.Band = 40;
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B40.Freq);
                    break;
                case 4:
                    Amplifier_Power_Controls.Band = 30;
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B30.Freq);
                    break;
                case 5:
                    Amplifier_Power_Controls.Band = 20;
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B20.Freq);
                    break;
                case 6:
                    Amplifier_Power_Controls.Band = 17;
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B17.Freq);
                    break;
                case 7:
                    Amplifier_Power_Controls.Band = 15;
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B15.Freq);
                    break;
                case 8:
                    Amplifier_Power_Controls.Band = 12;
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B12.Freq);
                    break;
                case 9:
                    Amplifier_Power_Controls.Band = 10;
                    oCode.SendCommand32(txsocket, txtarget, oCode.CMD_SET_MAIN_FREQ, Last_used.B10.Freq);
                    break;
            }
            oCode.SendCommand(txsocket, txtarget, Amplifier_Power_Controls.CMD_SET_AMPLIFIER_INITIALIZE, 
                                                                                      (short)Amplifier_Power_Controls.Band);
            Amplifier_Power_Controls.Solidus_Band_Selected = true;
        }

        private void SSB_Power_label36_Click(object sender, EventArgs e)
        {

        }

        private void MFC_A_label38_Click(object sender, EventArgs e)
        {

        }

        private void MFC_B_label38_Click(object sender, EventArgs e)
        {

        }

        private void MFC_C_label38_Click(object sender, EventArgs e)
        {

        }

        private void Volume_textBox2_Click(object sender, EventArgs e)
        {

        }

        private void Microphone_textBox2_Click(object sender, EventArgs e)
        {

        }

        private void Freq_Comp_label32_Click(object sender, EventArgs e)
        {

        }

        private void label60_Click(object sender, EventArgs e)
        {

        }

        private void Side_Tone_Volume_hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int index = 0;
            index = Side_Tone_Volume_hScrollBar1.Value;
            oCode.SendCommand(txsocket, txtarget, oCode.SET_SIDE_TONE_VOLUME, (short)index);
        }

        private void Set_Freq_Digit_Pointer()
        {
            Freq_Pointer_0.Text = "";
            Freq_Pointer_1.Text = "";
            Freq_Pointer_2.Text = "";
            Freq_Pointer_3.Text = "";
            Freq_Pointer_4.Text = "";
            Freq_Pointer_5.Text = "";
            Freq_Pointer_6.Text = "";
            Freq_Pointer_7.Text = "";

            MonitorTextBoxText(" Select_Freq_Digit_Pointer: " + Convert.ToString(oCode.FreqDigit));
            switch (oCode.FreqDigit)
            {
                case 0:
                    Freq_Pointer_0.Text = "^";
                    break;
                case 1:
                    Freq_Pointer_1.Text = "^";
                    break;
                case 2:
                    Freq_Pointer_2.Text = "^";
                    break;
                case 3:
                    Freq_Pointer_3.Text = "^";
                    break;
                case 4:
                    Freq_Pointer_4.Text = "^";
                    break;
                case 5:
                    Freq_Pointer_5.Text = "^";
                    break;
                case 6:
                    Freq_Pointer_6.Text = "^";
                    break;
                case 7:
                    Freq_Pointer_7.Text = "^";
                    break;
            }
        }



        private void MFC_Knob_label38_Click(object sender, EventArgs e)
        {

        }

        private void Freq_Pointer_5_Click(object sender, EventArgs e)
        {

        }

        private void Freq_Pointer_4_Click(object sender, EventArgs e)
        {

        }

        private void Freq_Pointer_3_Click(object sender, EventArgs e)
        {

        }

        private void Freq_Pointer_2_Click(object sender, EventArgs e)
        {

        }

        private void Freq_Pointer_1_Click(object sender, EventArgs e)
        {

        }

        private void Freq_Pointer_0_Click(object sender, EventArgs e)
        {

        }

        private void Freq_Pointer_7_Click(object sender, EventArgs e)
        {

        }

        private void Freq_Pointer_6_Click(object sender, EventArgs e)
        {

        }

        private void label32_Click(object sender, EventArgs e)
        {

        }

        private void band_stack_label29_Click_1(object sender, EventArgs e)
        {

        }
    }
}


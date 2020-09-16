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
    public partial class Main_form: Form
    {
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
                MonitorTextBoxText(  " B160_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode );
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B160.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B160.Mode = mode;
                oCode.previous_main_band = 0;
                main160radioButton10.Checked = true;
                main160radioButton10_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
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
                MonitorTextBoxText(  " B80_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode );
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B80.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B80.Mode = mode;
                oCode.previous_main_band = 0;
                main80radioButton9.Checked = true;
                main80radioButton9_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
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
                MonitorTextBoxText(  " B60_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode );
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B60.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B60.Mode = mode;
                oCode.previous_main_band = 0;
                main60radioButton8.Checked = true;
                main60radioButton8_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
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
                MonitorTextBoxText(  " B40_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode );
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B40.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B40.Mode = mode;
                oCode.previous_main_band = 0;
                main40radioButton7.Checked = true;
                main40radioButton7_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
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
                MonitorTextBoxText(  " B30_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode );
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B30.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B30.Mode = mode;
                oCode.previous_main_band = 0;
                main30radioButton6.Checked = true;
                main30radioButton6_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
                B30_Favs_listView1.SelectedIndices.Clear();
            }
        }

        private void B20_Favs_listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fav_freq;
            string fav_mode;
            string fav_name;
            Int32 frequency;
            char mode;

            if (B20_Favs_listView1.SelectedItems.Count != 0)
            {
                fav_name = B20_Favs_listView1.SelectedItems[0].SubItems[0].Text;
                fav_freq = B20_Favs_listView1.SelectedItems[0].SubItems[1].Text;
                fav_mode = B20_Favs_listView1.SelectedItems[0].SubItems[2].Text;
                MonitorTextBoxText(  " B20_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode );
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B20.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B20.Mode = mode;
                oCode.previous_main_band = 0;
                main20radioButton5.Checked = true;
                main20radioButton5_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
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
                MonitorTextBoxText(  " B17_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode );
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B17.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B17.Mode = mode;
                oCode.previous_main_band = 0;
                main17radioButton4.Checked = true;
                main17radioButton4_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
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
                MonitorTextBoxText(  " B15_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode );
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B15.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B15.Mode = mode;
                oCode.previous_main_band = 0;
                main15radiobutton.Checked = true;
                main15radiobutton_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
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
                MonitorTextBoxText(  " B12_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode );
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B12.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B12.Mode = mode;
                oCode.previous_main_band = 0;
                main12radioButton2.Checked = true;
                main12radioButton2_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
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
                MonitorTextBoxText(  " B10_Favs_listView1 Name: " + fav_name + " Freq: " + fav_freq + " mode: " + fav_mode );
                frequency = Convert.ToInt32(fav_freq);
                Last_used.B10.Freq = frequency;
                mode = Convert_Favs_Mode(fav_mode);
                Last_used.B10.Mode = mode;
                oCode.previous_main_band = 0;
                main10radioButton1.Checked = true;
                main10radioButton1_CheckedChanged(null, null);
                band_stack_textBox1.Text = fav_freq;
                textBox1.Text = fav_mode;
                B10_Favs_listView1.SelectedIndices.Clear();
            }
        }

        public bool Load_Favorites(String Band,int band_number)
        {
            String path, line;
            System.IO.StreamReader file;
            String[] arr = new string[4];
            ListViewItem itm;
            String temp_string;
            int param_pos;
            int end_pos;
         
            oCode.Platform = (int)Environment.OSVersion.Platform;
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if ((oCode.Platform == 4) || (oCode.Platform == 6) || (oCode.Platform == 128))
            {// A kludge to check for non Windows OS.  
             //These values may change in the future.
                //path += "//multus-sdr-mfc//b160_favs.ini";
                path += "//multus-sdr-mfc//" + Band;
            }
            else
            {
                path += "\\multus-sdr-mfc\\" + Band;
            }
            try
            {
                file = new System.IO.StreamReader(File.OpenRead(path));
            }
            catch (IOException e)
            {
                string er = e.Message;
                DialogResult ret =  MessageBox.Show("IO Exception opening: " + Band + "\r\n Error: " + er + "\r\nMake note of the error and contact Multus SDR,LLC.",
                    "MSCC-Core", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            while ((line = file.ReadLine()) != null)
            {
                /*arr[0] = "product_1";
                arr[1] = "100";
                arr[2] = "10";*/
                param_pos = line.IndexOf("NAME");                       // get the position of the DEVICE_NAME parameter
                temp_string = line.Substring((param_pos + 5), (line.Length - param_pos - 5));   // parse everything between the end of NAME= and the end of the line.
                end_pos = temp_string.IndexOf(","); //temp string will start with the DEVICE_NAME parameter value. Find the trailing comma
                arr[0] = temp_string.Substring(0, end_pos);   // temp string will start with the DEVICE_NAME parameter value. 
                param_pos = line.IndexOf("FREQ");                       // get the position of the DEVICE_NAME parameter
                temp_string = line.Substring((param_pos + 5), (line.Length - param_pos - 5));   // parse everything between the end of FREQ= and the end of the line.
                end_pos = temp_string.IndexOf(","); //temp string will start with the DEVICE_NAME parameter value. Find the trailing comma
                arr[1] = temp_string.Substring(0, end_pos);   // temp string will start with the DEVICE_NAME parameter value. 
                param_pos = line.IndexOf("MODE");                       // get the position of the DEVICE_NAME parameter
                temp_string = line.Substring((param_pos + 5), (line.Length - param_pos - 5));   // parse everything between the end of MODE= and the end of the line.
                end_pos = temp_string.IndexOf(";"); //temp string will start with the DEVICE_NAME parameter value. Find the trailing comma
                arr[2] = temp_string.Substring(0, end_pos);   // temp string will start with the DEVICE_NAME parameter value. 
                switch (band_number)
                {
                    case 0:
                        itm = new ListViewItem(arr);
                        B10_Favs_listView1.Items.Add(itm);
                        break;
                    case 1:
                        itm = new ListViewItem(arr);
                        B12_Favs_listView1.Items.Add(itm);
                        break;
                    case 2:
                        itm = new ListViewItem(arr);
                        B15_Favs_listView1.Items.Add(itm);
                        break;
                    case 3:
                        itm = new ListViewItem(arr);
                        B17_Favs_listView1.Items.Add(itm);
                        break;
                    case 4:
                        itm = new ListViewItem(arr);
                        B20_Favs_listView1.Items.Add(itm);
                        break;
                    case 5:
                        itm = new ListViewItem(arr);
                        B30_Favs_listView1.Items.Add(itm);
                        break;
                    case 6:
                        itm = new ListViewItem(arr);
                        B40_Favs_listView1.Items.Add(itm);
                        break;
                    case 7:
                        itm = new ListViewItem(arr);
                        B60_Favs_listView1.Items.Add(itm);
                        break;
                    case 8:
                        itm = new ListViewItem(arr);
                        B80_Favs_listView1.Items.Add(itm);
                        break;
                    case 9:
                        itm = new ListViewItem(arr);
                        B160_Favs_listView1.Items.Add(itm);
                        break;
                }
               
            }
           file.Close();
           return true;
        }
         
        public void Initialize_Favorites()
        {
            String band = "b160_favs.ini";
            int band_number = 0;

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
            Load_Favorites(band, band_number++);
            band = "b12_favs.ini";
            Load_Favorites(band, band_number++);
            band = "b15_favs.ini";
            Load_Favorites(band, band_number++);
            band = "b17_favs.ini";
            Load_Favorites(band, band_number++);
            band = "b20_favs.ini";
            Load_Favorites(band, band_number++);
            band = "b30_favs.ini";
            Load_Favorites(band, band_number++);
            band = "b40_favs.ini";
            Load_Favorites(band, band_number++);
            band = "b60_favs.ini";
            Load_Favorites(band, band_number++);
            band = "b80_favs.ini";
            Load_Favorites(band, band_number++);
            band = "b160_favs.ini";
            Load_Favorites(band, band_number++);
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
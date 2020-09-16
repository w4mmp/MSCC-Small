using System;
using System.Collections.Generic;
using System.Linq;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using OmniaGUI.Properties;
using System.Threading;

namespace OmniaGUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (Mutex mutex = new Mutex(false, @"Global\" + appGuid))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show("MSCC is already running","MSCC",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
                    return;
                }
                Process[] processes = Process.GetProcessesByName("MSCC");
                //foreach (Process proc in processes)
                //{
                 //   proc.PriorityClass = ProcessPriorityClass.AboveNormal;
                //}
                GC.Collect();
                Application.Run(new Main_form());
            }
        }
        private static string appGuid = "c0a76b5a-12ab-45c5-b9d9-d693faa6e7b9";
    }
}

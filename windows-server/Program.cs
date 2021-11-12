using System;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace NcmlAtwServer {

    class Program : WindowsFormsApplicationBase {

        [STAThread]
        static void Main(string[] args) {
            // Application.EnableVisualStyles();
            // Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(MainForm.Instance);
            var app = new Program();
            app.Run(args);
        }

        [System.Diagnostics.DebuggerStepThrough]
        public Program() {
            IsSingleInstance = true;
            EnableVisualStyles = true;
            SaveMySettingsOnExit = true;
            ShutdownStyle = ShutdownMode.AfterMainFormCloses;
        }

        [System.Diagnostics.DebuggerStepThrough]
        protected override void OnCreateMainForm() {
            MainForm = NcmlAtwServer.MainForm.Instance;
        }

        [System.Diagnostics.DebuggerStepThrough]
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs) {
            if (MainForm.WindowState == FormWindowState.Minimized) {
                MainForm.WindowState = FormWindowState.Normal;
            }
            MainForm.Activate();
        }
    }
}

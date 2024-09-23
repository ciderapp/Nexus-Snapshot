using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace C2Windows
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            webView21.DefaultBackgroundColor = Color.Transparent;
            this.FormBorderStyle = FormBorderStyle.None; // no borders
            this.DoubleBuffered = true;
            Load += Form1_Load;
            Closed += Form1_Closed;
        }
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private void Form1_Load(object sender, EventArgs e)
        {
            // Remove the title bar
            int style = GetWindowLong(this.Handle, -16);
            SetWindowLong(this.Handle, -16, (style & ~0x00400000) | 0x00040000);
        }

        private void Form1_Closed(object sender, EventArgs e)
        {
            // quit
            Application.Exit();
        }

    }
}

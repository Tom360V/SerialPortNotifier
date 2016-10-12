using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialPortNotifier
{
    public partial class Form1 : Form
    {
        private const int VisibleTime_mSec = 500;
        List<string> listSerialPorts;
        DateTime dt;
        public Form1()
        {
            InitializeComponent();
            listSerialPorts = new List<string>();

            NI.BalloonTipText = "No updates";
            NI.BalloonTipTitle = "Serial monitor";
            NI.Visible = true;

            dt = DateTime.Now;
            var ports = SerialPort.GetPortNames().OrderBy(name => name);
            foreach (var portName in ports)
            {
                listSerialPorts.Add(portName);
            }
            Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Hide();
        }


        void showLastMessage()
        {
            if (NI.BalloonTipText != "")
            {
                NI.Visible = true;
                NI.ShowBalloonTip(VisibleTime_mSec);
            }
        }

        void RefreshMessage()
        {
            if (dt.AddMilliseconds(VisibleTime_mSec) < DateTime.Now)
            {
                NI.BalloonTipText = "";
            }

            dt = DateTime.Now;
            List<string> newList = new List<string>();

            var ports = SerialPort.GetPortNames().OrderBy(name => name);
            foreach (var portName in ports)
            {
                newList.Add(portName);
                if (listSerialPorts.Contains(portName) == false)
                {
                    NI.BalloonTipText += "\n+" + portName;
                }
                else
                {
                    listSerialPorts.Remove(portName);
                }
            }
            foreach (string s in listSerialPorts)
            {
                NI.BalloonTipText += "\n-" + s;
            }

            listSerialPorts = newList;

/*            if (NI.BalloonTipText == "")
            {
                NI.BalloonTipText = "No Updates";
            }*/

            showLastMessage();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 537: //WM_DEVICECHANGE
                    RefreshMessage();
                    break;
            }
            base.WndProc(ref m);
        }

        private void NI_Click(object sender, EventArgs e)
        {
        }

        private void NI_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    showLastMessage();
                    break;

                case MouseButtons.Right:
                    contextMenuStrip2.Show();
                    break;
            }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int delay = 1000;
            NI.BalloonTipText = "Bye!";
            NI.ShowBalloonTip(delay);
            Thread.Sleep(delay);
            Close();
        }

        private void showActivePortsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NI.BalloonTipText = "Active ports:";
            var ports = SerialPort.GetPortNames().OrderBy(name => name);
            foreach (var portName in ports)
            {
                NI.BalloonTipText += "\n " + portName;
            }
            showLastMessage();
            NI.BalloonTipText = "";

        }
    }
}

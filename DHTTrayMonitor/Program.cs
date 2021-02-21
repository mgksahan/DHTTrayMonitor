using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Timers;
using System.IO.Ports;

namespace DHTTrayMonitor
{
    public class Program : ApplicationContext
    {
        private static System.Timers.Timer aTimer;
        static SerialPort _serialPort;
        static NotifyIcon notifyIcon;
        static Graphics g;
        static Font fontToUse;
        static Brush brushToUse;
        static Bitmap bitmapText;
        static IntPtr hIcon;
        public Program()
        {
            fontToUse = new Font("Arial", 16, FontStyle.Regular, GraphicsUnit.Pixel);
            brushToUse = new SolidBrush(Color.Orange);
            bitmapText = new Bitmap(16, 16);
            g = System.Drawing.Graphics.FromImage(bitmapText);
            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.DrawString("52", fontToUse, brushToUse, -4, -2);
            hIcon = (bitmapText.GetHicon());
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = DHTTrayMonitor.Properties.Resources.DisconnectedIcon;


            MenuItem comPorts = new MenuItem("COM Ports");
            foreach (string s in SerialPort.GetPortNames())
            {
                comPorts.MenuItems.Add(s, new EventHandler(ComPortSelect));
            }

            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[]
                { comPorts, exitMenuItem });
            notifyIcon.Visible = true;
        }


        [STAThread]
        static void Main()
        {

            Application.Run(new Program());

        }
        static void ComPortSelect(object sender, EventArgs e)
        {
            MenuItem foo = sender as MenuItem;
            _serialPort = new SerialPort();
            _serialPort.PortName = foo.Text;
            _serialPort.BaudRate = 9600;
            _serialPort.Open();
            SetTimer();
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        static void Exit(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private static void SetTimer()
        {
            aTimer = new System.Timers.Timer(2000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            _serialPort.Write("Temp?");
            //Console.WriteLine("Temp?");

        }

        private static void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            //Console.WriteLine("Data Received:");
            //Console.Write(indata);
            UpdateNumber(indata, g, fontToUse, brushToUse, hIcon, bitmapText, notifyIcon);
        }

        private static void UpdateNumber(string indata, Graphics g, Font fontToUse, Brush brushToUse, IntPtr hIcon, Bitmap bitmapText, NotifyIcon notifyIcon)
        {

            float temperature = float.Parse(indata);
            int temperature2 = (int)temperature;
            g.Clear(Color.Transparent);
            g.DrawString(temperature2.ToString(), fontToUse, brushToUse, -4, -2);
            hIcon = (bitmapText.GetHicon());
            notifyIcon.Icon = System.Drawing.Icon.FromHandle(hIcon);
        }

    }
}

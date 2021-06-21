using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Hardware;
using System.Globalization;
using MySql.Data.MySqlClient;
using System.Xml;
using System.Net;
using System.IO;
using System.Windows.Media;
using Newtonsoft.Json;

namespace DarkDemo
{
    public partial class Form1 : Form
    {
        

        Computer thisComputer;

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
                         int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        
        /* performance monitor cpu load from task manager*/
        //PerformanceCounter theCPUCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
        
        /* performance monitor system up time from seconds*/
        PerformanceCounter uptime = new PerformanceCounter("System", "System Up Time");

        /* performance monitor cpu load from performance monitor*/
        //PerformanceCounter theCPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        /* performance monitor available memory from performance monitor*/
        //PerformanceCounter theRAMCounter = new PerformanceCounter("Memory", "Available MBytes");

        //String temp = "";
        String CPUTemper = "";
        float theCPUTemper;
       
        //String ram = "";
        String RAMCounter = "";
        float dram;
        int theRAMCounter;

        //String cpu = "";
        String cpuc = "";
        float dcpuc;
        int thecpuc;

        //public String cname, cip, cport, cdb, cusr, cpass, cevery;

        String hostName, myIP;

        Stopwatch stopWatch = new Stopwatch();
        string xmlFile = "config.xml";
        XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object

        int msb = 0;
        int msx = 1;
        //int zxx = 0;

        //main form first load program

        DialogResult d;
        public Form1()
        {
            //inInitialize component
            InitializeComponent();
            //load xml file
            xmlDoc.Load(xmlFile);
            //invisible setting menu
            panelformularios.Visible = false;
            panelformularios.BringToFront();
            //this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
            //inInitialize computersensor *you can enable any sensors from this
            //**** default is enabled only cpu and ram
            thisComputer = new Computer() { CPUEnabled = true, 
                                            RAMEnabled = true,
                                            HDDEnabled = false,
                                            GPUEnabled = false,
                                            MainboardEnabled = false,
                                            FanControllerEnabled = false
            };
            thisComputer.Open();

            //settingup solisolidGauge
            solidGauge1.Uses360Mode = false;// 360 mode is not beautiful dashboard
            solidGauge1.From = 0;//set value min = 0
            solidGauge1.To = 100;//set value max = 100
            //set font color **sensorvalue = white
            solidGauge1.ForeGround = System.Windows.Media.Brushes.White; 

            solidGauge2.Uses360Mode = false;
            solidGauge2.From = 0;
            solidGauge2.To = 100;
            solidGauge2.ForeGround = System.Windows.Media.Brushes.White;

            solidGauge3.Uses360Mode = false;
            solidGauge3.From = 0;
            solidGauge3.To = 100;
            solidGauge3.ForeGround = System.Windows.Media.Brushes.White;
            //load xml to settime interval for any thread
            xmlDoc.Load("config.xml");
            XmlNodeList gevery = xmlDoc.GetElementsByTagName("send_data_every_millisec");
            String ts = gevery[0].InnerText;
            int time = Int32.Parse(ts);
            //start timer
            timer2.Interval = time;
            timer2.Enabled = true;
            timer3.Interval = time;
            timer3.Enabled = true;
            timer2.Start();
            timer3.Start();
            
        }

        //minimize windows button
        private void button7_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        //do this first when program load
        private void Form1_Load(object sender, EventArgs e)
        {
            //get icon and title program name
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.Text = "Over Send";
            stopWatch.Start();
            
            
        }

        //switch panel menu
        private void button4_Click(object sender, EventArgs e)
        {
            //this.Close();
            panelformularios.Visible = true;
            Thread3();

        }

        ////refresh setting get value from xml and reload setting
        private void Thread3()
        {
            
            XmlNodeList xname = xmlDoc.GetElementsByTagName("name");
            XmlNodeList xip = xmlDoc.GetElementsByTagName("ip");
            XmlNodeList xport = xmlDoc.GetElementsByTagName("port");
            XmlNodeList xdb = xmlDoc.GetElementsByTagName("database");
            XmlNodeList xusr = xmlDoc.GetElementsByTagName("usr");
            XmlNodeList npump = xmlDoc.GetElementsByTagName("nopumpdata");
            XmlNodeList nbak = xmlDoc.GetElementsByTagName("nobackupdata");
            XmlNodeList gevery = xmlDoc.GetElementsByTagName("send_data_every_millisec");
            bool myBool1 = Convert.ToBoolean(npump[0].InnerText);
            bool myBool2 = Convert.ToBoolean(nbak[0].InnerText);
            checkBox1.Checked = myBool1;
            checkBox2.Checked = myBool2;
            textpcname.Text = xname[0].InnerText;
            textserverip.Text = xip[0].InnerText;
            textserverport.Text = xport[0].InnerText;
            textdbname.Text = xdb[0].InnerText;
            textdbuser.Text = xusr[0].InnerText;
            textdbpass.Text = "*********";
            int sub = Int16.Parse(gevery[0].InnerText);
            sub = sub / 1000;
            sendlabel.Text = string.Format("{0} sec",sub);
            tracksendata.Value = sub;
        }

        //call Thread 3 to refresh setting
        private void clearbt_Click(object sender, EventArgs e)
        {
            Thread3();
        }

        //save setting when click submit button/write setting to config xml
        private void Submitbt_Click(object sender, EventArgs e)
        {
            String pname = textpcname.Text;
            String pip = textserverip.Text;
            String pport = textserverport.Text;
            String pdb = textdbname.Text;
            String pusr = textdbuser.Text;
            String ppass = textdbpass.Text;
            int timeinterval = tracksendata.Value;
            bool pump = checkBox1.Checked;
            bool bak = checkBox2.Checked;
            string ppump = pump.ToString();
            string bbak = bak.ToString();
            int subtime = timeinterval*1000;
            //get data from textbox
            xmlDoc.SelectSingleNode("data/display/name").InnerText = pname;
            xmlDoc.SelectSingleNode("data/display/ip").InnerText = pip;
            xmlDoc.SelectSingleNode("data/display/port").InnerText = pport;
            xmlDoc.SelectSingleNode("data/display/database").InnerText = pdb;
            xmlDoc.SelectSingleNode("data/display/usr").InnerText = pusr;
            xmlDoc.SelectSingleNode("data/display/passwd").InnerText = ppass;
            xmlDoc.SelectSingleNode("data/display/send_data_every_millisec").InnerText = subtime.ToString();
            xmlDoc.SelectSingleNode("data/display/nopumpdata").InnerText = ppump;
            xmlDoc.SelectSingleNode("data/display/nobackupdata").InnerText = bbak;
            //write to xml
            xmlDoc.Save(xmlFile);
            //call Thread 3 to refresh setting
            Thread3();
        }

        //tracking send_data_every_millisec value from xml to displai ms to sec
        private void tracksendata_Scroll(object sender, EventArgs e)
        {
            int timeinterval = tracksendata.Value;
            sendlabel.Text = string.Format("{0} sec",timeinterval.ToString());
        }

        //call back color into button
        private void CloseForms(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms["Form1"] == null)
                button1.BackColor = System.Drawing.Color.FromArgb(4, 41, 68);
            if (Application.OpenForms["Form2"] == null)
                button4.BackColor = System.Drawing.Color.FromArgb(4, 41, 68);
        }

        //dragable panel function
        private void Form1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        //clear ram usage from this program
        private void timer2_Tick(object sender, EventArgs e)
        {

            //start Thread1 function
            Thread t1 = new Thread(new ThreadStart(Thread1));
            t1.Start();
            
            //Force garbage collection.
            GC.Collect();

            // Wait for all finalizers to complete before continuing.
            GC.WaitForPendingFinalizers();
        }

        //switch menu
        private void button1_Click(object sender, EventArgs e)
        {
            panelformularios.Visible = false;
        }


        //system uptime function
        private String systime()
        {
            uptime.NextValue();
            TimeSpan result = TimeSpan.FromSeconds(uptime.NextValue());
            string elapsedTimeString = string.Format("{0:D2} D : {1:D2} H : {2:D2} m : {3:D2} s",
                                          result.Days,
                                          result.Hours,
                                          result.Minutes,
                                          result.Seconds);

            return elapsedTimeString;
        }

        //program uptime function
        private String protime()
        {
            //stopWatch.Stop();
            TimeSpan result = stopWatch.Elapsed;
            string elapsedTimeString = string.Format("{0:D2} D : {1:D2} H : {2:D2} m : {3:D2} s",
                                          result.Days,
                                          result.Hours,
                                          result.Minutes,
                                          result.Seconds);

            return elapsedTimeString;

        }
        
        //main function do not touch this
        private void timer1_Tick(object sender, EventArgs e)
        {

            foreach (var hardwareItem in thisComputer.Hardware)
            {
                //check hardware
                hardwareItem.Update();
                foreach (IHardware subHardware in hardwareItem.SubHardware)
                    subHardware.Update();
                //search sensor
                foreach (var sensor in hardwareItem.Sensors)
                {   //temp sensors
                    if (sensor.SensorType == SensorType.Temperature)
                    {   //get cpu sensors value
                        if (sensor.Name.Contains("CPU"))
                        {
                            //temp = String.Format("{0} Temperature = {1}\r\n", sensor.Name, sensor.Value.HasValue ? sensor.Value.Value.ToString() : "no value");
                            CPUTemper = sensor.Value.Value.ToString();
                            theCPUTemper = float.Parse(CPUTemper);
                        }
                    }
                    //workload sensors 
                    if (sensor.SensorType == SensorType.Load)
                    {   //get cpu sensors value
                        if (sensor.Name.Contains("CPU"))
                        {
                            //cpu = String.Format("{0} CPU Load = {1}\r\n", sensor.Name, sensor.Value.HasValue ? sensor.Value.Value.ToString() : "no value");
                            cpuc = sensor.Value.Value.ToString();
                            dcpuc = float.Parse(cpuc);
                            thecpuc = (int)dcpuc;
                        }
                        //get ram sensors value
                        if (sensor.Name.Contains("Memory"))
                        {
                            //ram = String.Format("{0} RamUsage = {1}\r\n", sensor.Name, sensor.Value.HasValue ? sensor.Value.Value.ToString() : "no value");
                            RAMCounter = sensor.Value.Value.ToString();
                            dram = float.Parse(RAMCounter);
                            theRAMCounter = (int)dram;
                        }
                    }

                }
            }

            //format label solidGauge1-3 display % and C *basic solidGauge candisplay only value
            Func<double, string> func0 = (thecpuc) => string.Format("{0} %", thecpuc);
            Func<double, string> func1 = (theCPUTemper) => string.Format("{0} \u00B0C", theCPUTemper);
            Func<double, string> func2 = (theRAMCounter) => string.Format("{0} %", theRAMCounter);
            solidGauge1.LabelFormatter = func0;
            solidGauge2.LabelFormatter = func1;
            solidGauge3.LabelFormatter = func2;

            //change soliGauge value 
            solidGauge1.Value = thecpuc;//get cpu workload value
            solidGauge2.Value = theCPUTemper;//get cpu temperatures value
            solidGauge3.Value = theRAMCounter;//get ram usage value

            //get system uptime and program uptime / display in the program
            label16.Text = systime();//call func system up time and display
            label3.Text = protime();//call func program up time and display


        }

        // send data cpuload cputemp ram use to server function
        private void Thread1()
        {
            // Get elements
            XmlNodeList gname = xmlDoc.GetElementsByTagName("name");
            XmlNodeList gip = xmlDoc.GetElementsByTagName("ip");
            XmlNodeList gport = xmlDoc.GetElementsByTagName("port");
            XmlNodeList gdb = xmlDoc.GetElementsByTagName("database");
            XmlNodeList gusr = xmlDoc.GetElementsByTagName("usr");
            XmlNodeList gpass = xmlDoc.GetElementsByTagName("passwd");
            
            hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            // Get the IPv4
            myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            // Get the IPv6 ??
            //myIP = Dns.GetHostEntry(hostName).AddressList[0].ToString();
            
            //parse text from elements in xml
            String server = gip[0].InnerText;
            String user = gusr[0].InnerText;
            String datab = gdb[0].InnerText;
            String portsql = gport[0].InnerText;
            String passwd = gpass[0].InnerText;
            String PCname = gname[0].InnerText;
            
            //mysql connection
            string MyConnection2 = string.Format("server={0};port={1};database={2};uid={3};pwd={4};", server, portsql, datab, user, passwd);
            string Query = string.Format("update menu set cpuload='{0}',cputemp='{1:0.0}',ramload='{2}',systime='{3}' where pcname='{4}'", thecpuc, theCPUTemper, theRAMCounter, systime(), PCname);
            MySqlConnection MyConn2 = new MySqlConnection();
            MySqlCommand MyCommand2 = new MySqlCommand();
            MySqlDataReader MyReader2;

            try
            {
                //query and update data to server
                if (MyConn2.State != ConnectionState.Open)
                {
                    MyConn2 = new MySqlConnection(MyConnection2);
                    MyCommand2 = new MySqlCommand(Query, MyConn2);
                    MyConn2.Open();
                }
                MyReader2 = MyCommand2.ExecuteReader();     // Here our query will be executed and data saved into the database.  
                while (MyReader2.Read())
                {
                    //Read data
                }
                    //change label in the app if connect to server
                MethodInvoker inv = delegate
                {
                        this.label15.Text = "Connected";
                        this.label11.Text = PCname;
                        this.label12.Text = myIP;
                        this.label13.Text = server;
                };
                this.Invoke(inv);
                MyConn2.Close();
            }
            catch (MySqlException ex)
            {
                //change label in the app if not connect to server
                MethodInvoker inv = delegate
                {
                    this.label15.Text = "Server Not Connected";
                };
                this.Invoke(inv);
                /*
                    // error message box when not connect to database
                    if (msx == 1 && msb == 0) {
                        msx = msb;
                        d = MessageBox.Show(MyConnection2, "Can't Connect to Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (d == DialogResult.OK)
                        {
                         msx = 1;
                        }
                        else{msx = 0;}
                    }
                */
            }
            

        }

        // Thread2 function for backup Logfile.txt
        private void Thread2()
        {
            // Get elements
            XmlNodeList gname = xmlDoc.GetElementsByTagName("name");
            XmlNodeList gip = xmlDoc.GetElementsByTagName("ip");

            hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            // Get the IP  
            myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            // get time
            DateTime localDate = DateTime.Now;
            // get time format from country
            var culture = new CultureInfo("th-TH");
            //backup file format
            string jsn = string.Format("PC:\n"+
              "\tName : {0}\n"+
              "\tCPU Load {1} %\n"+
              "\tCPU Temp {2:0.0} C\n"+
              "\tRAM Useage {3} %\n"+
              "Status :\n"+ 
                "\tSystem Up Time {4}\n"+
                "\tProgram Up Time {5}\n"+
              "Network Infomation :\n"+
                "\tIP : {6}\n"+
                "\tServer IP : {7}\n"+
                "Last Time :\n"+
                "\t{8}: {9}\n"
            , gname[0].InnerText, thecpuc,theCPUTemper,theRAMCounter,systime(),protime(),myIP, gip[0].InnerText, culture, localDate.ToString(culture));
            //get directory and backup Logfile.txt
            string logpath = Directory.GetCurrentDirectory();
            string FilePath = Path.Combine(logpath, "LogFile.txt");
            File.WriteAllText(FilePath, jsn);
        }

        //timer3
        private void timer3_Tick(object sender, EventArgs e)
        {
            //start Thread2 function
            Thread t3 = new Thread(new ThreadStart(Thread2));
            t3.Start();
        }

        //pump data to server checkbox
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true) { 
                timer2.Stop();
                myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                XmlNodeList iname = xmlDoc.GetElementsByTagName("name");
                XmlNodeList iserver = xmlDoc.GetElementsByTagName("ip");
                this.label11.Text = iname[0].InnerText;
                this.label12.Text = myIP;
                this.label13.Text = iserver[0].InnerText;
            }
            else { timer2.Start(); }
        }

        //exit button
        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
            Environment.Exit(1);
        }

        private void sendlabel_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void Version_Click(object sender, EventArgs e)
        {

        }

        //check box for backup logfile
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true) { timer3.Stop(); }
            else { timer3.Start(); }
        }

    }
}

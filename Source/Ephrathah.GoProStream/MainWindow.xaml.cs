using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Net;
using System.Diagnostics;

namespace Ephrathah.GoProStream
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        bool MediaServerStarted = false;

        bool MediaPlayStarted= false;

        MediaPlayerForm mediaPlayerForm;

        public MainWindow()
        {
            InitializeComponent();

            DisplayScreens();


            lbl_StreamPath.Content = "Stream URL : " + GetRTMPAddress() + "   ";

           

        }

        

        private void StartMediaServer()
        {
            CopyIniFileWithIPAddress();

            string MediaServeExeName = ConfigurationManager.AppSettings["MediaServer"];

            CheckandShutdowntheExe(MediaServeExeName);

            Process process = new Process();
            // Configure the process using the StartInfo properties.
            process.StartInfo.FileName = MediaServeExeName;
            //process.StartInfo.Arguments = "-n";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process.Start();

        }

        private void CheckandShutdowntheExe(string exename)
        {
            Process[] processlist = Process.GetProcessesByName(exename);

            if (processlist.Length > 0)
            {
                for (int i = 0; i < processlist.Length; i++)
                {
                    processlist[i].Kill();
                }
            }
        }


        private void CopyIniFileWithIPAddress()
        {
            string inputFile = ConfigurationManager.AppSettings["inputiniFile"];

            string outputFile = ConfigurationManager.AppSettings["outputiniFile"];

            if (inputFile != null && outputFile != null)
            {
                StreamReader streamReader = new StreamReader(inputFile);

                string currentLine = streamReader.ReadLine(); ;

                using (StreamWriter streamWriter = new StreamWriter(outputFile))
                {
                   
                    while(currentLine!=null)
                    {

                        if(currentLine.Contains("port="))
                        {
                            streamWriter.WriteLine("port=" + ConfigurationManager.AppSettings["Port"]);
                        }
                        else if(currentLine.Contains("host="))
                        {
                            streamWriter.WriteLine("host=" + GetIPAddress());
                        }

                        else if (currentLine.Contains("publicPort="))
                        {
                            streamWriter.WriteLine("publicPort=" + ConfigurationManager.AppSettings["Port"]);
                        }
                        else if (currentLine.Contains("publicHost="))
                        {
                            streamWriter.WriteLine("publicHost=" + GetIPAddress());
                        }
                        else
                        {
                            streamWriter.WriteLine(currentLine);
                        }

                        currentLine = streamReader.ReadLine();
                    }

                }

                streamReader.Close();
            }
                

        }

        private string GetIPAddress()
        {
            string IP = "127.0.0.1";

            IPHostEntry IPHost = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

            foreach (var ipAddress in IPHost.AddressList)
            {
                IP = ipAddress.ToString();

            }

            return IP;
        }

        private string GetRTMPAddress()
        {
           
                    

            int Port = int.Parse(ConfigurationManager.AppSettings["Port"]);

            string StreamKey = ConfigurationManager.AppSettings["StreamKey"];



            return "rtmp://" + GetIPAddress().ToString() + ":" + Port.ToString()+"/"+ StreamKey;
        }


        private void btn_Play(object sender, RoutedEventArgs e)
        {

            if (MediaServerStarted)
            {

                mediaPlayerForm = new MediaPlayerForm();

                mediaPlayerForm.Show();

                mediaPlayerForm.Play(GetRTMPAddress());

                MediaPlayStarted = true;
            }
            else
            {
                System.Windows.MessageBox.Show("Please Click Start Button Before clicking Play Button");
            }

        }
        int angle = 0;

        private void btn_Rotate(object sender, RoutedEventArgs e)
        {
            if (MediaPlayStarted)
            {
                angle += 90;
                mediaPlayerForm.Rotate(angle);
            }
            else
            {
                System.Windows.MessageBox.Show("Please Click Play Button Before clicking Rotate Button");
            }
        }

        private int ScreenIndexNumber = 0;

        private int PrimaryScreenIndexNumber = 0;

        private void AddButtonForScreen(int ScreenNumber)
        {
            System.Windows.Controls.Button button = new System.Windows.Controls.Button();
            button.Name = "ScreenNumber" + ScreenNumber.ToString();
            button.FontSize = 14;

            button.Content = " " + ScreenNumber.ToString() + " ";
            button.Click += Btn_CLick;
            
            this.ScreenPanel.Children.Add(button);

        }

        private void Btn_CLick(object sender, RoutedEventArgs e)
        {
           

            if (MediaPlayStarted)
            {

                mediaPlayerForm.WindowState = WindowState.Normal;
                mediaPlayerForm.WindowStyle = WindowStyle.ThreeDBorderWindow;

                string? screennumber = (sender as System.Windows.Controls.Button).Content as string;

                Screen FullScreenWindow = Screen.AllScreens[int.Parse(screennumber)-1];

                mediaPlayerForm.Left = FullScreenWindow.WorkingArea.Left;
                mediaPlayerForm.Top = FullScreenWindow.WorkingArea.Top;

                mediaPlayerForm.WindowStyle = WindowStyle.None;
                mediaPlayerForm.WindowState = WindowState.Maximized;

            }
            else
            {
                System.Windows.MessageBox.Show("Please Click Play Button Before clicking Screen Button");
            }

        }

        private void DisplayScreens()
        {
            int TotalScreens = Screen.AllScreens.Length;

            Screen FullScreenWindow = Screen.PrimaryScreen;

                for (int i = 1; i <= TotalScreens; i++)
                {
                AddButtonForScreen(i);
                }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

            int TotalScreens = Screen.AllScreens.Length;

            Screen FullScreenWindow = Screen.PrimaryScreen;


            if (TotalScreens > 1)
            {

                for (int i = 0; i < TotalScreens; i++)
                {
                    if (Screen.AllScreens[i] != Screen.PrimaryScreen)
                    {
                        FullScreenWindow = Screen.AllScreens[i];
                        break;
                    }
                }
            }

            mediaPlayerForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //FullScreenWindow.Bounds.;
            mediaPlayerForm.Left = FullScreenWindow.WorkingArea.Left;
            mediaPlayerForm.Top = FullScreenWindow.WorkingArea.Top;

            mediaPlayerForm.WindowStyle = WindowStyle.None;
            mediaPlayerForm.WindowState= WindowState.Maximized;
            
        }

        private void main_Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void btn_Start(object sender, RoutedEventArgs e)
        {

            //this.IsEnabled = false;

            StartMediaServer();
            MediaServerStarted = true;

            //btn_Play(sender, e);

            //this.IsEnabled = true;
        }



    }
}

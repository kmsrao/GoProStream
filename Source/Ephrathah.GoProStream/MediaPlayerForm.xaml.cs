using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ephrathah.GoProStream
{
    /// <summary>
    /// Interaction logic for MediaPlayerForm.xaml
    /// </summary>
    public partial class MediaPlayerForm : Window
    {
        public MediaPlayerForm()
        {
            InitializeComponent();
            var currentAssembly = Assembly.GetEntryAssembly();


            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            
            // Default installation path of VideoLAN.LibVLC.Windows
            var libDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

            this.rtmpPlayer.SourceProvider.CreatePlayer(libDirectory/* pass your player parameters here */);
            
            this.rtmpPlayer.SourceProvider.MediaPlayer.Video.FullScreen = true;

            this.rtmpPlayer.SourceProvider.MediaPlayer.Stopped += MediaPlayer_Stopped;


        }

        private void MediaPlayer_Stopped(object? sender, Vlc.DotNet.Core.VlcMediaPlayerStoppedEventArgs e)
        {
            this.rtmpPlayer.SourceProvider.Dispose();
            this.Hide();
        }

        public void Play(string filePath)
        {
            this.rtmpPlayer.SourceProvider.MediaPlayer.Audio.IsMute = true;
            this.rtmpPlayer.SourceProvider.MediaPlayer.Play(new Uri(filePath));
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           if(this.rtmpPlayer.SourceProvider.MediaPlayer.IsPlaying())
            {
                this.rtmpPlayer.SourceProvider.MediaPlayer.Stop();
            }
           else
            {
                this.rtmpPlayer.SourceProvider.Dispose();
                
            }
          
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.mediaplayerCanvas.Width = e.NewSize.Width;
            this.mediaplayerCanvas.Height = e.NewSize.Height;
            this.rtmpPlayer.Width   = e.NewSize.Width;
            this.rtmpPlayer.Height = e.NewSize.Height;
        }

        public void Rotate(int degress)
        {
           VideoRotationTransform.Angle = degress; 
           
        }
    }
}

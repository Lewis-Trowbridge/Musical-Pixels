using NAudio.Wave;
using System;
using Gtk;

namespace MusicArduino
{
    class Program
    {
        static float[] amplitudeBuffer = new float[8192];
        static int currentSentPosition = 0;
        
        static void Main(string[] args)
        {
            Application.Init();
            WaveOutEvent outputDevice = new WaveOutEvent();
            string musicPath = GetFilepath();
            AudioFileReader musicDataReader = new AudioFileReader(musicPath);
            AudioFileReader musicPlaybackReader = new AudioFileReader(musicPath);
            SerialConnection.Start();
            Console.WriteLine("Connection open!");
            outputDevice.Init(musicPlaybackReader);
            outputDevice.Play();
            while (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                if (musicDataReader.HasData(currentSentPosition))
                {
                    musicDataReader.Position = musicPlaybackReader.Position;
                    musicDataReader.Read(amplitudeBuffer, 0, amplitudeBuffer.Length);
                    AudioData lightCounts = AudioMethods.GetFrequencyBands(amplitudeBuffer);
                    byte[] help = SerialConnection.SendFrequencyBandData(lightCounts);
                    foreach(byte helpme in help)
                    {
                        Console.Write(helpme + ", ");
                    }
                    Console.WriteLine("");
                    currentSentPosition += amplitudeBuffer.Length;
                }
            }
            SerialConnection.SendData(new byte[48]);
        }

        private static string GetFilepath()
        {
            Gtk.Window window = new Gtk.Window(Gtk.WindowType.Toplevel);
            FileChooserNative fileChooser = new FileChooserNative("Choose a file...", window, FileChooserAction.Open, "Open", "Cancel");
            window.DeleteEvent += Window_DeleteEvent;
            window.Show();
            fileChooser.Show();
            Application.Run();
            string filepath = fileChooser.Filename;
            return filepath;
        }

        private static void Window_DeleteEvent(object o, DeleteEventArgs args)
        {
            Application.Quit();
        }
    }
}

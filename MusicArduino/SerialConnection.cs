using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using CSharpColorSpaceConverter;
using System.IO.Ports;
using System.Linq;

namespace MusicArduino
{
    public class SerialConnection
    {

        private static SerialPort port;
        private static double currentMaxCount = 0;
        private static double currentMinCount = 1;

        public static void Start()
        {
            port = new SerialPort("COM3", 9600);
            port.Open();
        }

        public static void SendData(byte[] dataToSend)
        {
            port.Write(dataToSend, 0, dataToSend.Length);
        }

        public static void CloseConnection()
        {
            port.Close();
        }

        public static byte[] SendFrequencyBandData(AudioData audioData)
        {
            int[] lightCounts = audioData.LightFrequencyCounts;
            byte[] dataToSend = new byte[48];
            double[] lightDoubles = new double[lightCounts.Length];
            for (int logPosition = 0; logPosition < lightCounts.Length; logPosition++)
            {
                if (lightCounts[logPosition] != 0)
                {
                    lightDoubles[logPosition] = Math.Log10(lightCounts[logPosition]) /( Math.Log(lightCounts[logPosition]) + 1);
                }
            }
            double maxFrequencyCount = lightDoubles.Max();
            if (maxFrequencyCount > currentMaxCount)
            {
                currentMaxCount = maxFrequencyCount;
            }
            double minFrequencyCount = lightDoubles.Where(lightNum => lightNum != 0).Min();
            if (minFrequencyCount < currentMinCount)
            {
                currentMinCount = minFrequencyCount;
            }
            for (int lightPosition = 0; lightPosition < lightCounts.Length; lightPosition++)
            {
                double hueColour;
                int[] rgbColour = { 0, 0, 0 };
                if (lightDoubles[lightPosition] != 0)
                {
                    hueColour = (lightDoubles[lightPosition] - currentMinCount) / (currentMaxCount - currentMinCount);
                    
                }
                else
                {
                    hueColour = 0;
                }
                if (hueColour != 0)
                {
                    double lightness = (audioData.AverageAmplitudes[lightPosition] * 0.75) + 0.25;
                    rgbColour = ColorSpaceConverter.HSLToRGB(hueColour, 1, lightness);
                }
                

                dataToSend[(lightPosition + 1) * 3 - 3] = Convert.ToByte(rgbColour[0]);
                dataToSend[(lightPosition + 1) * 3 - 2] = Convert.ToByte(rgbColour[1]);
                dataToSend[(lightPosition + 1) * 3 - 1] = Convert.ToByte(rgbColour[2]);
            }
            
            port.Write(dataToSend, 0, dataToSend.Length);
            return dataToSend;
        }

    }
}

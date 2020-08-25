using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;
using NAudio.Dsp;

namespace MusicArduino
{
    class AudioMethods
    {

        public static AudioData GetFrequencyBands(float[] sampleArray)
        {
            NAudio.Dsp.Complex[] complices = new NAudio.Dsp.Complex[sampleArray.Length];
            List<float>[] amplitudeLists = GetEmptyFloatListArray();
            // Collates the amplitudes into complexes wtih the Hann window applied
            for (int fftPosition = 0; fftPosition < sampleArray.Length; fftPosition++)
            {
                complices[fftPosition].X = Convert.ToSingle(sampleArray[fftPosition] * FastFourierTransform.HannWindow(fftPosition, sampleArray.Length));
                complices[fftPosition].Y = 0;
            }
            // Performs the FFT to get the frequencies out
            FastFourierTransform.FFT(true, (int)Math.Log(sampleArray.Length, 2.0), complices);
            float[] realFrequencies = new float[complices.Length];
            // Gets the real numbers out of the complex numbers
            for (int realPosition = 0; realPosition < complices.Length; realPosition++)
            {
                realFrequencies[realPosition] = Convert.ToSingle(Math.Pow(complices[realPosition].X, 2) + Math.Pow(complices[realPosition].Y, 2));
            }
            // Creates an array of empty numbers to collate the counts of categorisation
            int[] counts = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            float maxFrequency = realFrequencies.Max();
            for (int lightPosition = 0; lightPosition < realFrequencies.Length; lightPosition++)
            {
                // Gets the frequency on a scale from 0 to 1 in this window
                float realFrequencyNormal = realFrequencies[lightPosition] / maxFrequency;
                int lightToIncrement;
                // If the max frequency in this time slice is 0, that means that all of the values are 0 and we should avoid doing any more maths
                if (maxFrequency == 0)
                {
                    lightToIncrement = 0;
                }
                else
                {
                    lightToIncrement = Convert.ToInt32(Math.Round(16 * (Math.Sqrt(realFrequencyNormal))));
                }
                // Prevents the index from going over the maximum range
                if (lightToIncrement == 16)
                {
                    lightToIncrement = 15;
                }
                counts[lightToIncrement]++;
                amplitudeLists[lightToIncrement].Add(sampleArray[lightPosition]);
            }
            float[] averageAmplitudes = new float[16];
            for (int averageAmplitudePosition = 0; averageAmplitudePosition < averageAmplitudes.Length; averageAmplitudePosition++)
            {
                if (amplitudeLists[averageAmplitudePosition].Count != 0)
                {
                    averageAmplitudes[averageAmplitudePosition] = Math.Abs(amplitudeLists[averageAmplitudePosition].Average());
                }
                else
                {
                    averageAmplitudes[averageAmplitudePosition] = 0;
                }
            }
            AudioData output = new AudioData(counts, averageAmplitudes);
            // These mask functions were intended to distribute values, solving the issue of the frequencies dogpiling the first band, but I found these did more harm than good, and commented them out
            //Vector<short> countVector = GetLightVector(counts);
            //for (int vectorPosition = 0; vectorPosition < counts.Length; vectorPosition++)
            //{
            //    Vector<short> currentMask = GetMaskVector(vectorPosition);
            //    countVector = Vector.Divide(countVector, currentMask);
            //}
            //for (int vectorToArrayPosition = 0; vectorToArrayPosition < 16; vectorToArrayPosition++)
            //{
            //    counts[vectorToArrayPosition] = Convert.ToInt32(countVector[vectorToArrayPosition]);
            //}
            return output;
        }


        private static Vector<float> SquareFloatVector(Vector<float> vectorToSquare)
        {
            Vector<float> squaredVector = Vector.Multiply(vectorToSquare, vectorToSquare);
            return squaredVector;
        }


        // Gets the vector of the integer light count array passed in to allow for vector operations
        private static Vector<short> GetLightVector(int[] lightCount)
        {
            short[] lightShorts = new short[lightCount.Length];
            for (int lightPosition = 0; lightPosition < lightCount.Length; lightPosition++)
            {
                lightShorts[lightPosition] = Convert.ToInt16(lightCount[lightPosition]);
            }
            Vector<short> vectorToOutput = new Vector<short>(lightShorts);
            return vectorToOutput;
        }

        // Gets the mask needed at a certain position in the lights array given that position
        private static Vector<short> GetMaskVector(int position)
        {
            short[] mask = { 3, 2, 1, 2, 3 };
            int maskOffset = 2 - position;
            if (maskOffset < 0)
            {
                maskOffset = 0;
            }
            int maskCount = Convert.ToInt32(10.5 - Math.Abs(position - 7.5));
            if (maskCount > 5)
            {
                maskCount = 5;
            }
            ArraySegment<short> maskSegment = new ArraySegment<short>(mask, maskOffset, maskCount);
            short[] maskSegmentArray = maskSegment.ToArray();
            int newPosition;
            if (position <= 2)
            {
                newPosition = position;
            }
            else
            {
                newPosition = 2;
            }
            newPosition = position - newPosition;
            short[] fullMask = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            for (int vectorPosition = 0; vectorPosition < maskSegmentArray.Length; vectorPosition++)
            {
                fullMask[newPosition] = maskSegmentArray[vectorPosition];
                newPosition++;
            }
            Vector<short> maskVector = new Vector<short>(fullMask);
            return maskVector;

        }

        private static List<float>[] GetEmptyFloatListArray()
        {
            // Creates an array of literally 16 empty lists
            List<float>[] listArray = { new List<float>(), new List<float>(), new List<float>(), 
                new List<float>(), new List<float>(), new List<float>(), new List<float>(), new List<float>(), new List<float>(),
                new List<float>(), new List<float>(), new List<float>(), new List<float>(),new List<float>(), new List<float>(), new List<float>() };
            return listArray;
        }

    }
    public struct AudioData
    {
        public int[] LightFrequencyCounts { get; }

        public float[] AverageAmplitudes { get; }

        public AudioData(int[] lightFrequencyCounts, float[] averageAmplitudes)
        {
            this.LightFrequencyCounts = lightFrequencyCounts;
            this.AverageAmplitudes = averageAmplitudes;
        }

    }
}

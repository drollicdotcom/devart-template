using System;
using System.Collections.Generic;
using System.Text;

namespace com.drollic.graphics.wavelets
{
    public sealed class WaveletFactory
    {
        private static Random rand = new Random();
        private static readonly int TotalWavelets = 10;

        public static GenericWavelet RandomWavelet()
        {
            int value = rand.Next(TotalWavelets);
            GenericWavelet wavelet = null;

            if (value == 0)
            {
                wavelet = new BattleLemarieWavelet(); 
            }
            else if (value == 1)
            {
                wavelet = new BurtAdelsonWavelet();
            }
            else if (value == 2)
            {
                wavelet = new Coiflet4Wavelet();
            }
            else if (value == 3)
            {
                wavelet = new Daub10Wavelet();
            }
            else if (value == 4)
            {
                wavelet = new Daub12Wavelet();
            }
            else if (value == 5)
            {
                wavelet = new Daub20Wavelet();
            }
            else if (value == 6)
            {
                wavelet = new Daub4Wavelet();
            }
            else if (value == 7)
            {
                wavelet = new Daub6Wavelet();
            }
            else if (value == 8)
            {
                wavelet = new Daub8Wavelet();
            }
            else
            {
                wavelet = new PseudoCoiflet4Wavelet();
            }

            return wavelet;
        }
    }
}

using System;
using System.Security.Cryptography;

//
// Borrowed from:
// http://eimagine.com/how-to-generate-better-random-numbers-in-c-net-2/
// How to Generate Better Random Numbers in C# .NET
// by Ben Klopfer | Dec 4, 2012
//

namespace RNG
{
    public class RandomNumberGenerator
    {
        // strong but slow RNG
        private static readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public RandomNumberGenerator()
        {
        }

        // 
        // generate a RNG which is a positive number between 0 and 1, [0.0, 1.0)
        //
        private double NextDouble()
        {
            byte[] bytes = new byte[4];
            rng.GetBytes(bytes);
            UInt32 random = BitConverter.ToUInt32(bytes, 0); // [0, UInt32.MaxValue], UInt32.MaxValue + 1 possible values
            return ( (double) random / ( (double) UInt32.MaxValue + 1.0 ) );
        }

        //
        // get the next random number between [0,N)
        //
        public int Next(int maxValue)
        {
            double percentage = NextDouble();
            int randomNumber = (int) ( (double) maxValue * percentage ); // note final int conversion truncates, not rounds.
            return randomNumber;
        }
    }

    // System.Security.Cryptography.RandomNumberGenerator is another possibility???

    //
    // Lehmer random number generator aka the Park–Miller random number generator
    // is quicker (300x) but not as strong.  This RNG guarantees to sequence through
    // all of the numbers in the int32 space before repeating itself.
    //
    // See
    // https://en.wikipedia.org/wiki/Lehmer_random_number_generator
    // and
    // https://msdn.microsoft.com/en-us/magazine/mt767700.aspx
    // Test Run - Lightweight Random Number Generation
    // and finally
    // https://pdfs.semanticscholar.org/9af1/2693368d7c76ea5b0ee4bf7e729de0b6f089.pdf
    // Random Number Geuerators: Good Ones Are Hard to Find by Park and Miller, CACM
    //
    // This is really a pseduo RNG (since generating every number in the set is not random?).
    //

    public class ParkAndMillerPseudoRandomNumberGenerator
    {
        private const int a = 16807;       // or 48271
        private const int m = 2147483647;  // aka int.MaxValue
        private const int q = 127773;      // or 44488
        private const int r = 2836;        // or 3399

        private int seed;

        public ParkAndMillerPseudoRandomNumberGenerator()
        {
            SetSeed();
        }

        public ParkAndMillerPseudoRandomNumberGenerator(int seed)
        {
            SetSeed(seed);
        }

        private void SetSeed()
        {
            this.seed = (int) ( (ulong) DateTime.Now.Ticks % int.MaxValue ); // 0 <= seed < int.MaxValue
        }

        private void SetSeed(int seed)
        {
            if ( seed <= 0 || seed == int.MaxValue )
            {
                SetSeed();
            }
            else
            {
                this.seed = seed;
            }
        }

        public double NextDouble()
        {
            int hi = seed / q;
            int lo = seed % q;
            seed = ( a * lo ) - ( r * hi );
            if ( seed <= 0 )
                seed = seed + m;
            return ( seed * 1.0 ) / m;
        }
    }
}

using Isopoh.Cryptography.Argon2;
using System.Text;

namespace NBitcoin.Crypto
{
    //
    // Summary:
    //     Implements the Argon2d hash algorithm for Dynamic.
    public static class Argon2d_Dynamic
    {
        const int OUTPUT_BYTES = 32;
        const int MEMORY_COST = 500;
        const int NUMBER_LANES = 8;
        const int NUMBER_THREADS = 1;
        const int TIME_COST = 2;

        public static byte[] Hash(byte[] input)
        {
            //
            // Summary:
            //     Hashes the input using Dynamic's Argon2d parameters
            //          Memory Cost = 500
            //          Lanes (degree of parallelism) = 8
            //          Time Cost = 2
            //          Threads = 1 (changing this parameter does not change the resulting hash but increasing slows it down)
            //          Salt = input
            //          Password = input
            //          Hash length = 32 bytes
            //          See https://github.com/duality-solutions/Dynamic/blob/master/src/hash.h
            //                  inline int Argon2d_Phase1_Hash()
            // Parameters:
            //   input:
            //     input bytes to hash.
            //
            // Returns:
            //     The resulting hash byte array.
            //

            Argon2Config config = new Argon2Config();
            config.MemoryCost = MEMORY_COST;
            config.Lanes = NUMBER_LANES;
            config.TimeCost = TIME_COST;
            config.Threads = NUMBER_THREADS;
            config.Salt = input;
            config.Password = input;
            config.HashLength = OUTPUT_BYTES;
            config.Type = Argon2Type.DataDependentAddressing; // DataDependentAddressing = Argon2d.  DataIndependentAddressing = Argon2i.
            config.Version = Argon2Version.Nineteen; // not sure about this parameter.
            config.Secret = null;
            config.AssociatedData = null;
            string strHash = Argon2.Hash(config);
            return Encoding.ASCII.GetBytes(strHash);
        }
    }
}
 
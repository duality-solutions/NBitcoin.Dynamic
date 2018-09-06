
using System.Collections.Generic;

namespace NBitcoin.Dynamic.RPC
{
    public class ScriptSig
    {
        public string asm { get; set; }
        public string hex { get; set; }
    }

    public class Vin
    {
        public string txid { get; set; }
        public int vout { get; set; }
        public ScriptSig scriptSig { get; set; }
        public double value { get; set; }
        public object valueSat { get; set; }
        public string address { get; set; }
        public object sequence { get; set; }
    }

    public class ScriptPubKey
    {
        public string asm { get; set; }
        public string hex { get; set; }
        public int reqSigs { get; set; }
        public string type { get; set; }
        public List<string> addresses { get; set; }
    }

    public class Vout
    {
        public double value { get; set; }
        public long valueSat { get; set; }
        public int n { get; set; }
        public ScriptPubKey scriptPubKey { get; set; }
    }

    public class Transaction
    {
        public string hex { get; set; }
        public string txid { get; set; }
        public int size { get; set; }
        public int version { get; set; }
        public int locktime { get; set; }
        public List<Vin> vin { get; set; }
        public List<Vout> vout { get; set; }
        public string blockhash { get; set; }
        public int height { get; set; }
        public int confirmations { get; set; }
        public int time { get; set; }
        public int blocktime { get; set; }
    }

    public class JsonTransaction
    {
        public Transaction result { get; set; }
        public object error { get; set; }
        public string id { get; set; }
    }
}

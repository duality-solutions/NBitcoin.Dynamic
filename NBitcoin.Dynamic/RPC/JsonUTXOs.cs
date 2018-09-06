using System;
using System.Collections.Generic;
using System.Text;

namespace NBitcoin.Dynamic.RPC
{
    public class UTXO
    {
        public string address { get; set; }
        public string txid { get; set; }
        public uint outputIndex { get; set; }
        public string script { get; set; }
        public ulong satoshis { get; set; }
        public int height { get; set; }
    }

    public class JsonUTXOs
    {
        public List<UTXO> result { get; set; }
        public object error { get; set; }
        public string id { get; set; }
    }
}

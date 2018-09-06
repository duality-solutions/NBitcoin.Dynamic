using System;
using System.Collections.Generic;
using System.Text;

namespace NBitcoin.Dynamic.RPC
{
    public class JsonAddressTxIDs
    {
        public List<string> result { get; set; }
        public object error { get; set; }
        public string id { get; set; }
    }
}

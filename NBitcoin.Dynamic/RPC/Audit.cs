
using System.Collections.Generic;

namespace NBitcoin.Dynamic.RPC
{
    public class Audit
    {
        public string owner { get; set; }
        public string description { get; set; }
        public string algorithm_type { get; set; }
        public string txid { get; set; }
        public string error_message { get; set; }
    }

}


using System.Collections.Generic;

namespace NBitcoin.Dynamic.RPC
{
    public class Certificate
    {
        public string version { get; set; }
        public string months_valid { get; set; }
        public string subject { get; set; }
        public string subject_signature { get; set; }
        public string subject_public_key { get; set; }
        public string issuer_public_key { get; set; }
        public string issuer_signature { get; set; }
        public string approved { get; set; }
        public string root_certificate { get; set; }
        public string serial_number { get; set; }
        public string pem { get; set; }
        public string txid_request { get; set; }
        public string txid_signed { get; set; }
        public int request_time { get; set; }
        public string request_height { get; set; }
        public int valid_from { get; set; }
        public int valid_until { get; set; }
        public string approve_height { get; set; }
        public string error_message { get; set; }
    }

    public class CertificateVerify
    {
        public string valid { get; set; }
        public string certificate_subject_pubkey { get; set; }
        public string error_message { get; set; }

    }

}

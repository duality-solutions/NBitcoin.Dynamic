using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NBitcoin.RPC;
using NBitcoin.DataEncoders;
using Newtonsoft.Json;
using System.IO;

namespace NBitcoin.Dynamic.RPC
{
    public class DynamicRPCClient : RPCClient
    {
        //
        // Summary:
        //     Use default Dynamic parameters to configure a RPCDynamicClient.
        //
        // Parameters:
        //   network:
        //     The network used by the node. Must not be null.
        public DynamicRPCClient(Network network)
            : base(network) { }

        public DynamicRPCClient(RPCCredentialString credentials, Network network)
            : base(credentials, network) { }

        public DynamicRPCClient(RPCCredentialString credentials, string host, Network network)
            : base(credentials, host, network) { }

        public DynamicRPCClient(RPCCredentialString credentials, Uri address, Network network)
                : base(credentials, address, network) { }

        //
        // Summary:
        //     Create a new Dynamic RPCDynamicClient instance
        //
        // Parameters:
        //   authenticationString:
        //     username:password, the content of the .cookie file, or cookiefile=pathToCookieFile
        //
        //   hostOrUri:
        //
        //   network:
        public DynamicRPCClient(string authenticationString, string hostOrUri, Network network)
            : base(authenticationString, hostOrUri, network) { }

        public DynamicRPCClient(NetworkCredential credentials, Uri address, Network network = null)
            : base(credentials, address, network) { }

        //
        // Summary:
        //     Create a new Dynamic RPCDynamicClient instance
        //
        // Parameters:
        //   authenticationString:
        //     username:password or the content of the .cookie file or null to auto configure
        //
        //   address:
        //
        //   network:
        public DynamicRPCClient(string authenticationString, Uri address, Network network = null)
            : base(authenticationString, address, network) { }

        public async Task<string> SendDynamicCommandAsync(string jsonPayload, bool throwIfRPCError = true)
        {
            try
            {
                return await SendDynamicCommandAsyncCore(jsonPayload, throwIfRPCError).ConfigureAwait(false);
            }
            catch (WebException ex)
            {
                if (!IsUnauthorized(ex))
                    throw;
                if (GetCookiePath() == null)
                    throw;

                return await SendDynamicCommandAsyncCore(jsonPayload, throwIfRPCError).ConfigureAwait(false);
            }
        }

        public async Task<JsonTransaction> GetTransactionAsync(string txid, bool throwIfNotFound = true)
        {
            JsonTransaction tx;
            try
            {
                string strReponse = "";
                //{"jsonrpc": "1.0", "id": "GetTransactionAsync", "method": "getrawtransaction", "params": ["mytxid", true] }
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                JsonWriter configRPC = new JsonTextWriter(sw);
                configRPC.Formatting = Formatting.None;
                configRPC.WriteStartObject();
                configRPC.WritePropertyName("jsonrpc");
                configRPC.WriteValue("1.0");
                configRPC.WritePropertyName("id");
                configRPC.WriteValue("GetTransactionAsync");
                configRPC.WritePropertyName("method");
                configRPC.WriteValue("getrawtransaction");
                configRPC.WritePropertyName("params");
                configRPC.WriteStartArray();
                configRPC.WriteValue(txid);
                configRPC.WriteValue(true);
                configRPC.WriteEndArray();
                configRPC.WriteEndObject();
                strReponse = await SendDynamicCommandAsync(sb.ToString()).ConfigureAwait(true);
                tx = JsonConvert.DeserializeObject<JsonTransaction>(strReponse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get UTXOs for address='{txid}'", ex);
            }
            return tx;
        }

        public async Task<JsonUTXOs> GetAddressUTXOsAsync(string address)
        {
            JsonUTXOs jsonUTXOs;
            try
            {
                string strReponse = "";
                //{"jsonrpc": "1.0", "id": "GetAddressUTXOsAsync", "method": "getaddressutxos", "params": [{"addresses": [address]}] }
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                JsonWriter configRPC = new JsonTextWriter(sw);
                configRPC.Formatting = Formatting.None;
                configRPC.WriteStartObject();
                configRPC.WritePropertyName("jsonrpc");
                configRPC.WriteValue("1.0");
                configRPC.WritePropertyName("id");
                configRPC.WriteValue("GetAddressUTXOsAsync");
                configRPC.WritePropertyName("method");
                configRPC.WriteValue("getaddressutxos");
                configRPC.WritePropertyName("params");
                configRPC.WriteStartArray();
                configRPC.WriteStartObject();
                configRPC.WritePropertyName("addresses");
                configRPC.WriteStartArray();
                configRPC.WriteValue(address);
                configRPC.WriteEndArray();
                configRPC.WriteEndObject();
                configRPC.WriteEndArray();
                configRPC.WriteEndObject();
                strReponse = await SendDynamicCommandAsync(sb.ToString()).ConfigureAwait(true);
                jsonUTXOs = JsonConvert.DeserializeObject<JsonUTXOs>(strReponse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get UTXOs for address='{address}'", ex);
            }
            return jsonUTXOs;
        }

        public async Task<JsonAddressTxIDs> GetAddressTxIDsAsync(string address)
        {
            JsonAddressTxIDs jsonAddressTxIDs;
            try
            {
                string strReponse = "";
                //{"jsonrpc": "1.0", "id": "GetAddressTxIDsAsync", "method": "getaddresstxids", "params": [{"addresses": [address]}] }
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                JsonWriter configRPC = new JsonTextWriter(sw);
                configRPC.Formatting = Formatting.None;
                configRPC.WriteStartObject();
                configRPC.WritePropertyName("jsonrpc");
                configRPC.WriteValue("1.0");
                configRPC.WritePropertyName("id");
                configRPC.WriteValue("GetAddressTxIDsAsync");
                configRPC.WritePropertyName("method");
                configRPC.WriteValue("getaddresstxids");
                configRPC.WritePropertyName("params");
                configRPC.WriteStartArray();
                configRPC.WriteStartObject();
                configRPC.WritePropertyName("addresses");
                configRPC.WriteStartArray();
                configRPC.WriteValue(address);
                configRPC.WriteEndArray();
                configRPC.WriteEndObject();
                configRPC.WriteEndArray();
                configRPC.WriteEndObject();
                strReponse = await SendDynamicCommandAsync(sb.ToString()).ConfigureAwait(true);
                jsonAddressTxIDs = JsonConvert.DeserializeObject<JsonAddressTxIDs>(strReponse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get TxIDs for address='{address}'", ex);
            }
            return jsonAddressTxIDs;
        }

        public Certificate CertificateView(string serialnumber)
        {
            Certificate certificate;
            try
            {
                string strResponse = "";
                RPCRequest rpcRequest = new RPCRequest();

                rpcRequest.Method = "certificate";
                rpcRequest.Params = new object[] { "view", serialnumber };
                strResponse = SendCommand(rpcRequest).ResultString;
                certificate = JsonConvert.DeserializeObject<Certificate>(strResponse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to view certificate", ex);
            }
            return certificate;
        }

        public CertificateVerify CertificateVerify(string serialnumber, string subject, string signature, string data)
        {
            CertificateVerify certificateVerify;
            try
            {
                string strResponse = "";
                RPCRequest rpcRequest = new RPCRequest();

                rpcRequest.Method = "certificate";
                rpcRequest.Params = new object[] { "verify", serialnumber, subject, signature, data };
                strResponse = SendCommand(rpcRequest).ResultString;
                certificateVerify = JsonConvert.DeserializeObject<CertificateVerify>(strResponse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to verify certificate", ex);
            }
            return certificateVerify;
        }

        async Task<string> SendDynamicCommandAsyncCore(string json, bool throwIfRPCError)
        {
            string response = "";
            HttpWebRequest webRequest = CreateWebRequest();

            var bytes = Encoding.UTF8.GetBytes(json);
#if !(PORTABLE || NETCORE)
            webRequest.ContentLength = bytes.Length;
#endif
            var dataStream = await webRequest.GetRequestStreamAsync().ConfigureAwait(false);
            await dataStream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            await dataStream.FlushAsync().ConfigureAwait(false);
            dataStream.Dispose();

            WebResponse webResponse = null;
            WebResponse errorResponse = null;
            try
            {
                webResponse = await webRequest.GetResponseAsync().ConfigureAwait(false);
                Stream stream = await ToMemoryStreamAsync(webResponse.GetResponseStream()).ConfigureAwait(false);
                StreamReader reader = new StreamReader(stream);
                response = reader.ReadToEnd();
            }
            catch (WebException ex)
            {
                if (ex.Response == null || ex.Response.ContentLength == 0 ||
                    !ex.Response.ContentType.Equals("application/json", StringComparison.Ordinal))
                    throw;
            }
            finally
            {
                if (errorResponse != null)
                {
                    errorResponse.Dispose();
                    errorResponse = null;
                }
                if (webResponse != null)
                {
                    webResponse.Dispose();
                    webResponse = null;
                }
            }
            return response;
        }

        private static bool IsUnauthorized(WebException ex)
        {
            var httpResp = ex.Response as HttpWebResponse;
            var isUnauthorized = httpResp != null && httpResp.StatusCode == HttpStatusCode.Unauthorized;
            return isUnauthorized;
        }

        private async Task<Stream> ToMemoryStreamAsync(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            await stream.CopyToAsync(ms).ConfigureAwait(false);
            ms.Position = 0;
            return ms;
        }

        private HttpWebRequest CreateWebRequest()
        {
            var address = Address.AbsoluteUri;
            if (!string.IsNullOrEmpty(CredentialString.WalletName))
            {
                if (!address.EndsWith("/"))
                    address = address + "/";
                address += "wallet/" + CredentialString.WalletName;
            }
            var webRequest = (HttpWebRequest)WebRequest.Create(address);
            webRequest.Headers[HttpRequestHeader.Authorization] = "Basic " + Encoders.Base64.EncodeData(Encoders.ASCII.DecodeData(Authentication));
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";
            return webRequest;
        }

        private string GetCookiePath()
        {
            if (CredentialString.UseDefault && Network == null)
                throw new InvalidOperationException("NBitcoin bug, report to the developers");
            if (CredentialString.UseDefault)
                return GetDefaultCookieFilePath(Network);
            if (CredentialString.CookieFile != null)
                return CredentialString.CookieFile;
            return null;
        }
    }
}
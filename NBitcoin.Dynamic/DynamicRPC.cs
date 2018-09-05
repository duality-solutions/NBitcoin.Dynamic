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
    class DynamicRPCDynamicClient : RPCClient
    {
        //
        // Summary:
        //     Use default Dynamic parameters to configure a RPCDynamicClient.
        //
        // Parameters:
        //   network:
        //     The network used by the node. Must not be null.
        public DynamicRPCDynamicClient(Network network)
            : base(network) { }

        public DynamicRPCDynamicClient(RPCCredentialString credentials, Network network)
            : base(credentials, network) { }

        public DynamicRPCDynamicClient(RPCCredentialString credentials, string host, Network network)
            : base(credentials, host, network) { }

        public DynamicRPCDynamicClient(RPCCredentialString credentials, Uri address, Network network)
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
        public DynamicRPCDynamicClient(string authenticationString, string hostOrUri, Network network)
            : base(authenticationString, hostOrUri, network) { }

        public DynamicRPCDynamicClient(NetworkCredential credentials, Uri address, Network network = null)
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
        public DynamicRPCDynamicClient(string authenticationString, Uri address, Network network = null)
            : base(authenticationString, address, network) { }

        public async Task<string> SendDynamicCommandAsync(string jsonPayload)
        {
            try
            {
                return await SendDynamicCommandAsyncCore(jsonPayload).ConfigureAwait(false);
            }
            catch (WebException ex)
            {
                if (!IsUnauthorized(ex))
                    throw;
                if (GetCookiePath() == null)
                    throw;

                return await SendDynamicCommandAsyncCore(jsonPayload).ConfigureAwait(false);
            }
        }

        public async Task<string> GetAddressUTXOsAsync(string address)
        {
            string reponse = "";
            try
            {
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                JsonWriter configRPC = new JsonTextWriter(sw);
                configRPC.Formatting = Formatting.None;
                configRPC.WriteStartObject();
                configRPC.WritePropertyName("jsonrpc");
                configRPC.WriteValue("1.0");
                configRPC.WritePropertyName("id");
                configRPC.WriteValue("NBitcoin.Dynamic");
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

                reponse = await SendDynamicCommandAsync(sb.ToString()).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get UTXOs for address='{address}'", ex);
            }
            return reponse;
        }

        async Task<string> SendDynamicCommandAsyncCore(string json)
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
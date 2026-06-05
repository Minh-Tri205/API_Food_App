using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Net;

namespace API_Food_App.Services
{
    public class VnpayService
    {
        private readonly IConfiguration config;

        public VnpayService(IConfiguration config)
        {
            this.config = config;
        }

        // Build URL thanh toan VNPay sandbox.
        // amount: VND (vd: 150000), VNPay yeu cau * 100.
        public string CreatePaymentUrl(
            long amount,
            string orderInfo,
            string txnRef,
            string ipAddress)
        {
            // .Trim() de tranh khoang trang an khi copy tu email
            var tmnCode = config["VnpayConfig:TmnCode"]!.Trim();
            var hashSecret = config["VnpayConfig:HashSecret"]!.Trim();
            var baseUrl = config["VnpayConfig:BaseUrl"]!;
            var returnUrl = config["VnpayConfig:ReturnUrl"]!;
            var version = config["VnpayConfig:Version"]!;
            var command = config["VnpayConfig:Command"]!;
            var currCode = config["VnpayConfig:CurrCode"]!;
            var locale = config["VnpayConfig:Locale"]!;

            if (ipAddress == "::1") ipAddress = "127.0.0.1";

            // SortedDictionary -> tu sort theo alphabet (yeu cau VNPay)
            var data = new SortedDictionary<string, string>(
                StringComparer.Ordinal)
            {
                { "vnp_Version",    version },
                { "vnp_Command",    command },
                { "vnp_TmnCode",    tmnCode },
                { "vnp_Amount",     (amount * 100).ToString() },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode",   currCode },
                { "vnp_IpAddr",     ipAddress },
                { "vnp_Locale",     locale },
                { "vnp_OrderInfo",  orderInfo },
                { "vnp_OrderType",  "other" },
                { "vnp_ReturnUrl",  returnUrl },
                { "vnp_TxnRef",     txnRef },
            };

            // QUAN TRONG: dung Uri.EscapeDataString (encode space = %20)
            // KHONG dung WebUtility.UrlEncode (encode space = +) -> VNPay tu choi
            var signData = new StringBuilder();
            var query = new StringBuilder();
            foreach (var kv in data)
            {
                if (string.IsNullOrEmpty(kv.Value)) continue;
                var encKey = WebUtility.UrlEncode(kv.Key); 
                var encVal = WebUtility.UrlEncode(kv.Value); 

                if (signData.Length > 0) signData.Append('&');
                signData.Append(encKey).Append('=').Append(encVal);

                if (query.Length > 0) query.Append('&');
                query.Append(encKey).Append('=').Append(encVal);
            }

            var secureHash = HmacSha512(hashSecret, signData.ToString());

            // DEBUG - in ca Console (dotnet run) va Debug (Visual Studio)
            var log = $"\n===== VNPAY DEBUG =====\n" +
                      $"SignData: {signData}\n" +
                      $"Hash:     {secureHash}\n" +
                      $"=======================\n";
            Console.WriteLine(log);
            Debug.WriteLine(log);

            return $"{baseUrl}?{query}&vnp_SecureHash={secureHash}";
        }

        // Verify chu ky tra ve khi VNPay redirect ve ReturnUrl
        public bool ValidateSignature(
            IQueryCollection query,
            out string responseCode,
            out string txnRef)
        {
            responseCode = query["vnp_ResponseCode"].ToString();
            txnRef = query["vnp_TxnRef"].ToString();
            var receivedHash = query["vnp_SecureHash"].ToString();

            var data = new SortedDictionary<string, string>(
                StringComparer.Ordinal);
            foreach (var kv in query)
            {
                if (kv.Key.StartsWith("vnp_") &&
                    kv.Key != "vnp_SecureHash" &&
                    kv.Key != "vnp_SecureHashType")
                {
                    data[kv.Key] = kv.Value.ToString();
                }
            }

            var signData = new StringBuilder();
            foreach (var kv in data)
            {
                if (string.IsNullOrEmpty(kv.Value)) continue;
                if (signData.Length > 0) signData.Append('&');
                signData.Append(WebUtility.UrlEncode(kv.Key))      
                        .Append('=')
                        .Append(WebUtility.UrlEncode(kv.Value));  
            }

            var hashSecret = config["VnpayConfig:HashSecret"]!.Trim();
            var expected = HmacSha512(hashSecret, signData.ToString());

            return string.Equals(
                expected,
                receivedHash,
                StringComparison.OrdinalIgnoreCase);
        }

        private static string HmacSha512(string key, string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
                sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            return sb.ToString();
        }
    }
}

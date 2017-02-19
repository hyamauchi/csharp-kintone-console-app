using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace csharp_kintone_console_app
{
    class Program
    {
        private readonly static string KINTONE_HOST = "{0}.cybozu.com";
        private readonly static string KINTONE_PORT = "443";
        private readonly static string KINTONE_API_FORMAT = "https://{0}/k/v1/{1}.json";

        static void Main(string[] args)
        {
            string subdomain = System.Environment.GetEnvironmentVariable("KINTONE_APP_SUBDOMAIN");
            string apiToken = System.Environment.GetEnvironmentVariable("KINTONE_APP_API_TOKEN");
            string appId = System.Environment.GetEnvironmentVariable("KINTONE_APP_APPID");

            var request = makeHeader(subdomain, apiToken, "records", "app="+appId);
            string responseStr = getResponse(request);
            System.Diagnostics.Debug.WriteLine(responseStr);
        }

        private static HttpWebRequest makeHeader(string subdomain, string apiToken, string command, string query)
        {
            string host = string.Format(KINTONE_HOST, subdomain);
            string uri = string.Format(KINTONE_API_FORMAT, host, command);
            if (!string.IsNullOrEmpty(query)) uri += "?" + query;
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Headers.Add("X-Cybozu-API-Token", apiToken);
            request.Host = host + ":" + KINTONE_PORT;
            request.Method = "GET";
            return request;
        }

        private static string getResponse(HttpWebRequest request)
        {
            string responseStr = "";
            HttpWebResponse response = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (var res = ex.Response)
                    {
                        using (var reader = new StreamReader(res.GetResponseStream()))
                        {
                            responseStr = reader.ReadToEnd();
                        }
                    }
                }
            }
            finally
            {
                request.ServicePoint.CloseConnectionGroup(request.ConnectionGroupName);
            }

            if (response != null)
            {
                var reader = new StreamReader(response.GetResponseStream());
                responseStr = reader.ReadToEnd();
            }
            return responseStr;
        }

    }
}

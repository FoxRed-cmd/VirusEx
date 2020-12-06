using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VirusT
{
	public class VirusTotal
	{
		public string APIkey;
		public string results = "https://www.virustotal.com/api/get_file_report.json";

		public VirusTotal(string API)
		{
			APIkey = API;
		}

		public string ScanFile(string path_to_file)
		{
			string uploadResult, resourceId;
			var nvc = new NameValueCollection();
			nvc.Add("apikey", APIkey);
			using (WebClient webClient = new WebClient() { QueryString = nvc })
			{
				FileInfo fileInfo = new FileInfo(path_to_file);
				webClient.Headers.Add("Content-type", "binary/octet-stream");
				uploadResult = Encoding.Default.GetString(webClient.UploadFile("https://www.virustotal.com/vtapi/v2/file/scan", "post", path_to_file));
				resourceId = getJsonValue(uploadResult, "resource");
				var data = string.Format("resource={0}&key={1}", resourceId, APIkey);
				string s = webClient.UploadString(results, "POST", data);
			}
			return resourceId;
		}

		public string GetResults(string resourceID)
		{
			var data = string.Format("resource={0}&key={1}", resourceID, APIkey);
			var c = new WebClient();
			string s = c.UploadString(results, "POST", data);
			return s;
		}

		Func<string, string, string> getJsonValue = delegate (string source, string key)
		{
			int from = source.IndexOf(key + "\":"), to = from > 0 ? source.IndexOf(",", from) : -1;
			if (from > 0 && to > 0)
				return source.Substring(from + (key + "\":").Length, to - from - (key + "\":").Length).Replace("\"", "").Trim();
			return null;
		};

	}

}

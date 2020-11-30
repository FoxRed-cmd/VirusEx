using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VirusTotalNet;
using VirusTotalNet.Results;

namespace VirusEx
{
	public partial class Form1 : Form
	{
		string apiKey = "433ffd858b4b51bea110b50c569e9c08bd4cdc1fe63028756b36593062251c31";
		string[] files;


		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		Func<string, string, string> getJsonValue = delegate (string source, string key)
		{
			int from = source.IndexOf(key + "\":"), to = from > 0 ? source.IndexOf(",", from) : -1;
			if (from > 0 && to > 0)
				return source.Substring(from + (key + "\":").Length, to - from - (key + "\":").Length).Replace("\"", "").Trim();
			return null;
		};


		async void panel1_DragDrop(object sender, DragEventArgs e)
		{
			await Task.Run(() =>
			{
				Action action = async () =>
				{
					files = (string[])e.Data.GetData(DataFormats.FileDrop);
					VirusTotal virusTotal = new VirusTotal(apiKey);
					virusTotal.UseTLS = true;
					var nvc = new NameValueCollection();
					nvc.Add("apikey", apiKey);
					using (WebClient webClient = new WebClient() { QueryString = nvc })
					{
						FileInfo fileInfo = new FileInfo(files[0]);
						webClient.Headers.Add("Content-type", "binary/octet-stream");
						string uploadResult = Encoding.Default.GetString(webClient.UploadFile("https://www.virustotal.com/vtapi/v2/file/scan", "post", files[0]));
						string resourceId = getJsonValue(uploadResult, "resource");
						FileReport fileReport = await virusTotal.GetFileReportAsync(resourceId);
						foreach (var item in fileReport.Scans)
						{
							label1.Text += item.Value.Result == null ? "Undetected" + "\r\n" : item.Value.Result.ToString() + "\r\n";
							label2.Text += item.Value.Result == null ? item.Key.ToString() + "\r\n" : item.Key.ToString() + "\r\n";

						}
					}
				};
				if (InvokeRequired)
				{
					Invoke(action);
				}
				else
				{
					action();
				}
			});

		}

		private void panel1_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;

			}
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{

		}
	}
}

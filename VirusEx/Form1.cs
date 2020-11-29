using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Windows.Forms;

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

		private void panel1_DragDrop(object sender, DragEventArgs e)
		{
			files = (string[])e.Data.GetData(DataFormats.FileDrop);
			var nvc = new NameValueCollection();
			nvc.Add("apikey", apiKey);
			using (var webClient = new WebClient() { QueryString = nvc })
			{
				webClient.Headers.Add("Content-type", "binary/octet-stream");
				string uploadResult = Encoding.Default.GetString(webClient.UploadFile("https://www.virustotal.com/vtapi/v2/file/scan", "post", files[0]));
				string resourceId = getJsonValue(uploadResult, "resource");
			}
		}

		private void panel1_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;

			}
		}
	}
}

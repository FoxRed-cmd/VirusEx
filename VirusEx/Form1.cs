using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
			using (WebClient wc = new WebClient())
			{
				string uploadResult = Encoding.Default.GetString(wc.UploadFile("https://www.virustotal.com/vtapi/v2/file/scan","post", files[0]));
				label1.Text = uploadResult;
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

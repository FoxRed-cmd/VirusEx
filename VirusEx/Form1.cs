using System;
using System.Collections.Specialized;
using System.Drawing;
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
		string apiKey;
		string[] files;
		string pathAPI = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\API.txt";


		public Form1()
		{
			InitializeComponent();
			label1.Text = "";
			label2.Text = "";
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			if (File.Exists(pathAPI))
			{
				using (FileStream file = File.OpenRead(pathAPI))
				{
					using (StreamReader sw = new StreamReader(file))
					{
						apiKey = sw.ReadToEnd();
					}
				}
				textBox1.Visible = false;
				label5.Visible = false;
				button1.Visible = false;
			}
			
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
			try
			{
				label1.Text = "";
				label2.Text = "";
				await Task.Run(() =>
				{
					Action action = async () =>
					{
						string uploadResult, resourceId;
						files = (string[])e.Data.GetData(DataFormats.FileDrop);
						VirusTotal virusTotal = new VirusTotal(apiKey);
						virusTotal.UseTLS = true;
						var nvc = new NameValueCollection();
						nvc.Add("apikey", apiKey);
						using (WebClient webClient = new WebClient() { QueryString = nvc })
						{
							FileInfo fileInfo = new FileInfo(files[0]);
							webClient.Headers.Add("Content-type", "binary/octet-stream");
							do
							{
								uploadResult = Encoding.Default.GetString(webClient.UploadFile("https://www.virustotal.com/vtapi/v2/file/scan", "post", files[0]));
								resourceId = getJsonValue(uploadResult, "resource");

							} while (resourceId == null);
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
			catch (Exception)
			{
				MessageBox.Show("Не удалось получить отчёт о файле. Ваш API ключ VirusTotal не безлимитный и имеет ограничение по умолчанию, - 4 запроса в минуту\nПовторите попытку позже", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
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
			float[] arr = { 5, 2 };
			Pen pen = new Pen(Color.WhiteSmoke, 2);
			pen.DashPattern = arr;
			e.Graphics.DrawRectangle(pen, 1, 1, panel1.Width - 2, panel1.Height - 2);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (!File.Exists(pathAPI) && textBox1.Text != "")
			{
				using (FileStream fs = new FileStream(pathAPI, FileMode.OpenOrCreate))
				{
					byte[] array = Encoding.UTF8.GetBytes(textBox1.Text);
					fs.Write(array, 0, array.Length);
				}
				using (FileStream file = File.OpenRead(pathAPI))
				{
					using (StreamReader sw = new StreamReader(file))
					{
						apiKey = sw.ReadToEnd();
					}
				}
				textBox1.Visible = false;
				label5.Visible = false;
				button1.Visible = false;
			}
			else
			{
				textBox1.Visible = false;
				label5.Visible = false;
				button1.Visible = false;
				using (FileStream file = File.OpenRead(pathAPI))
				{
					using (StreamReader sw = new StreamReader(file))
					{
						apiKey = sw.ReadToEnd();
					}
				}
			}
		}
	}
}

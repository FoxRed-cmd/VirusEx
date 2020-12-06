using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VirusT;

namespace VirusEx
{
	public partial class Form1 : Form
	{
		string apiKey, resultID, result;
		string[] files;
		string pathAPI = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\API.txt";
		List<string> results = new List<string>();


		public Form1()
		{
			InitializeComponent();
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

		void panel1_DragDrop(object sender, DragEventArgs e)
		{
			textBox2.Clear();
			try
			{
				files = (string[])e.Data.GetData(DataFormats.FileDrop);
				VirusTotal virusTotal = new VirusTotal(apiKey);
				resultID = virusTotal.ScanFile(files[0]);
				result = virusTotal.GetResults(resultID);
				var r = ParseJSON(result);

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

		Dictionary<string, string> ParseJSON(string json)
		{
			var d = new Dictionary<string, string>();
			json = json.Replace("\"", null).Replace("[", null).Replace("]", null);
			var r = json.Substring(1, json.Length - 2).Split(',');
			foreach (string s in r)
			{
				d.Add(s.Split(':')[0], s.Split(':')[1]);
			}
			foreach (var item in d)
			{
				textBox2.Text += item.Key + ": ";
				textBox2.Text += item.Value == " " ? "Undetected" + "\r\n" : item.Value + "\r\n";
			}
			return d;
		}
	}
}

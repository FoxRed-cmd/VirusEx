using System;
using System.Windows.Forms;
using VirusTotalNet;

namespace VirusEx
{
	public partial class Form1 : Form
	{
		string apiKey = "433ffd858b4b51bea110b50c569e9c08bd4cdc1fe63028756b36593062251c31";
		string files;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}


		private void panel1_DragDrop(object sender, DragEventArgs e)
		{
			files = (string)e.Data.GetData(DataFormats.FileDrop);
			VirusTotal virusTotal = new VirusTotal(apiKey);
			virusTotal.UseTLS = true;
			virusTotal.ScanFileAsync(files);
			virusTotal.GetFileReportAsync(files);


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

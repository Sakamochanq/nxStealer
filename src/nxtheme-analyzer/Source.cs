using nxtheme_analyzer.utils;
using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using System.Xml.Linq;

namespace nxtheme_analyzer
{
    public partial class Source : Form
    {
        public Source()
        {
            InitializeComponent();
        }

        string filePath;

        private void OpenButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog() {Filter = "nxtheme File (*.nxtheme) | *.nxtheme;" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    filePath = ofd.FileName;
                    SatusLabel.Text = "NxTheme: " + filePath;

                    NxTheme nxTheme = new NxTheme(filePath);

                    // 情報を表示
                    AuthorLabel.Text = $"{nxTheme.Author() ?? "---"}";
                    NameLabel.Text = $"{nxTheme.Name() ?? "---"}";
                    VersionLabel.Text = $"{nxTheme.Version()}";
                    TargetLabel.Text = $"{nxTheme.Target()}";

                    Analyzing();

                    //ImageBox.Image = nxTheme.GetImage();
                }
            }
        }

        private void Analyzing()
        {
            byte[] fileData = File.ReadAllBytes(filePath);

            if (fileData[0] == 0x59 && fileData[1] == 0x61 &&
                fileData[2] == 0x7A && fileData[3] == 0x30)
            {
                LogTextBox.AppendText("\r\nThis is Yaz0 !");
            }
            else
            {
                LogTextBox.AppendText("\r\nThis is Not Yaz0");
            }

            LogTextBox.AppendText("\r\nExpanding Yaz0...\r\n\r\n");

            byte[] decompressed = Yaz0.Decompress(fileData);

            LogTextBox.AppendText($"Decompressed size : {decompressed.Length:N0} byte\r\n");
            LogTextBox.AppendText($"First 4 byte : {decompressed[0]:X2} {decompressed[1]:X2} {decompressed[2]:X2} {decompressed[3]:X2}\r\n");

        }
    }
}
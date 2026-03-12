using nxtheme_analyzer.utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace nxtheme_analyzer
{
    public partial class Source : Form
    {
        public Source()
        {
            InitializeComponent();
        }

        private void WaitTimer_Tick(object sender, EventArgs e)
        {
            StatusLabel.ForeColor = Color.Black;
            StatusLabel.Text = "Ready";
            WaitTimer.Stop();
        }

        string filePath;

        private void OpenButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog() { Filter = "nxtheme File (*.nxtheme) | *.nxtheme; | All Files (*.) | *.*;" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        filePath = ofd.FileName;
                        PathBox.Text = filePath;

                        NxTheme nxTheme = new NxTheme(filePath);

                        // 情報を表示
                        AuthorLabel.Text = $"{nxTheme.Author() ?? "---"}";
                        NameLabel.Text = $"{nxTheme.Name() ?? "---"}";
                        VersionLabel.Text = $"{nxTheme.Version()}";
                        TargetLabel.Text = $"{nxTheme.Target()}";

                        // 画像を表示
                        ImageBox.Image = nxTheme.nxImage();

                        // ログ出力
                        NxThemeInfo();

                        OutputLog("NxTheme Analysis", true);
                    }
                    catch (Exception ex)
                    {
                        OutputLog(ex.Message, false);
                    }
                }
            }
        }

        private void NewButton_Click_1(object sender, EventArgs e)
        {
            PathBox.Text = "";
            AuthorLabel.Text = "---";
            NameLabel.Text = "---";
            VersionLabel.Text = "---";
            TargetLabel.Text = "---";
            ImageBox.Image = null;
            LogTextBox.Clear();
            OutputLog("Cleared", true);
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void NxThemeInfo()
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

        private void SaveImageButton_Click(object sender, EventArgs e)
        {
            if (ImageBox.Image == null)
            {
                return;
            }
            else
            {
                using (SaveFileDialog dialog = new SaveFileDialog() { Filter = "JPEG (*.jpg;) | *.jpg", FileName = TargetLabel.Text })
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            ImageBox.Image.Save(dialog.FileName, ImageFormat.Jpeg);
                            LogTextBox.AppendText($"\r\nSuccess : Save Image {dialog.FileName}");
                            OutputLog($"Save Image {dialog.FileName}", true);
                        }
                        catch (Exception ex)
                        {
                            LogTextBox.AppendText($"\r\nError : {ex.Message}");
                        }
                    }
                }
            }
        }

        private void ReloadButton_Click(object sender, EventArgs e)
        {
            ImageBox.Refresh();
        }

        private void SaveImageButton2_Click(object sender, EventArgs e)
        {
            SaveImageButton_Click(sender, e);
        }

        /// <param name="isStats">true : Success   |   false : Error</param>
        private void OutputLog(string Message, bool isStats)
        {
            if (isStats == true)
            {
                StatusLabel.ForeColor = Color.Green;
                StatusLabel.Text = $"Success : {Message}";
            }
            else
            {
                StatusLabel.ForeColor = Color.Red;
                StatusLabel.Text = $"Error : {Message}";
            }
            WaitTimer.Start();
        }
    }
}
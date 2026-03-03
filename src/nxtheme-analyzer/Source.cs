using nxtheme_analyzer.utils;
using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace nxtheme_analyzer
{
    public partial class Source : Form
    {
        public Source()
        {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog() {Filter = "nxtheme File (*.nxtheme) | *.nxtheme;" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    NxTheme nxTheme = new NxTheme(ofd.FileName);
                }
            }
        }
    }
}
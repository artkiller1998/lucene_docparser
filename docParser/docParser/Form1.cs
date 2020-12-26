using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace docParser
{
    public partial class Form1 : Form
    {
        string inFolderName = String.Empty;
        string outFolderName = String.Empty;
        string[] inFiles;
        TikaOnDotNet.TextExtraction.TextExtractionResult _result;

        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    inFolderName = dlg.SelectedPath;
            }
            textBox1.Text = inFolderName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    outFolderName = dlg.SelectedPath;
            }
            textBox2.Text = outFolderName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (inFolderName == String.Empty)
                MessageBox.Show("Error! Enter input path.", "No path to parse", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                inFiles = System.IO.Directory.GetFiles(inFolderName,"*.*", SearchOption.AllDirectories);

                progressBar1.Visible = true;
                progressBar1.Maximum = inFiles.Length;
                progressBar1.Minimum = 1;
                progressBar1.Step = 1;
                progressBar1.Value = 1;

                if (outFolderName == String.Empty)
                    outFolderName = Path.Combine(inFolderName, "docParser");
                if (!Directory.Exists(outFolderName))
                    Directory.CreateDirectory(outFolderName);
                Directory.SetCurrentDirectory(outFolderName);

                foreach (string file in inFiles) {

                    var _te = new TikaOnDotNet.TextExtraction.TextExtractor();
                    
                    try
                    {
                        _result = _te.Extract(file);
                    }
                    catch
                    {
                        continue;
                    }

                    progressBar1.PerformStep();

                    var shortFilename = Path.GetFileNameWithoutExtension(file) + ".txt";
                    using (System.IO.FileStream fs = File.Create(shortFilename))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(_result.ToString());
                        // Add some information to the file.
                        fs.Write(info, 0, info.Length);
                    }
                }
            }
            MessageBox.Show("The work is done! Check the path:\n" + outFolderName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

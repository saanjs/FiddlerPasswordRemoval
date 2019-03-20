using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Compression;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace RemoveFiddlerPassword
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        string text = "";

        private void Form1_Load(object sender, EventArgs e)
        {                       
            richTextBox1.Visible = false;            
            openFileDialog1.DefaultExt = ".saz,.har"; // Default file extension            
            openFileDialog1.Filter = "Fiddler Trace Files |*.saz|HTTP Archives|*.har";
        }

        public void ReadFile(string zipPath, string pathExtension)
        {            
            int successCount = 0;
            string document = "";

            try
            {
                if (pathExtension == ".saz")
                {
                    using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
                    {
                        progBar.Minimum = 0;
                        progBar.Maximum = archive.Entries.Count;
                        progBar.Visible = true;

                        int progressCount = 0;

                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            progressCount++;
                            progBar.Value = progressCount;
                            progBar.Refresh();
                            progBar.CreateGraphics().DrawString("Processing... "
                                + Math.Round((((double)progressCount / (double)progBar.Maximum) * 100)).ToString() + "%",
                                new Font("Arial",
                                (float)8.25, FontStyle.Regular),
                                Brushes.Red,
                                new PointF(progBar.Width / 2 - 10, progBar.Height / 2 - 7));

                            if (entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                            {
                                using (var stream = entry.Open())
                                using (var reader = new StreamReader(stream))
                                {
                                    document = reader.ReadToEnd();
                                    if (document.ToLower().Contains("passwd")
                                        || document.Contains("&Password"))
                                    {
                                        if (!document.ToLower().Contains("deleted"))
                                        {
                                            if (document.ToLower().Contains("passwd"))
                                            {
                                                document = document.Replace(document.Substring((document.IndexOf("passwd") + 7),
                                                    (document.IndexOf("&ps") - (document.IndexOf("passwd") + 7))),
                                                    "DELETED");
                                            }
                                            else
                                            {
                                                document = document.Replace(document.Substring((document.IndexOf("Password") + 9),
                                                (document.IndexOf("&AuthMethod") - (document.IndexOf("Password") + 9))),
                                                "DELETED");
                                            }

                                            stream.SetLength(text.Length);

                                            using (StreamWriter writer = new StreamWriter(stream))
                                            {
                                                writer.Write(document);
                                                successCount++;
                                            }
                                        }
                                        
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    string harFile = File.ReadAllText(zipPath);
                    int passwdLocation = 0;
                    string extractedText = string.Empty;
                    string oldPassword = string.Empty;

                    if (harFile.Contains("passwd"))
                    {
                        passwdLocation = harFile.IndexOf("passwd");
                        extractedText = harFile.Substring(passwdLocation + 52);
                        oldPassword = extractedText.Substring(0, extractedText.IndexOf('"'));
                        successCount = ProcessFile(oldPassword, harFile, zipPath);
                    }                    
                    else if (harFile.Contains("&Password"))
                    {
                        harFile = File.ReadAllText(zipPath);
                        passwdLocation = harFile.IndexOf("&Password");
                        extractedText = harFile.Substring(passwdLocation + 10);
                        oldPassword = extractedText.Substring(0, extractedText.IndexOf('&'));
                        successCount = ProcessFile(oldPassword, harFile, zipPath);
                    }
                    else if (harFile.Contains("Password"))
                    {
                        harFile = File.ReadAllText(zipPath);
                        passwdLocation = harFile.IndexOf("Password");
                        extractedText = harFile.Substring(passwdLocation + 54);
                        oldPassword = extractedText.Substring(0, extractedText.IndexOf('"'));
                        successCount = ProcessFile(oldPassword, harFile, zipPath);
                    }
                }                            
                           
                if (successCount > 0)
                {
                    MessageBox.Show("Found clear text password and deleted successfuly.", "Successfully Removed Password", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    progBar.Visible = false;
                }
                else
                {
                    MessageBox.Show("There is no clear text password found in the network trace.", "No password found", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    progBar.Visible = false;
                }
            }
            catch (Exception ex)
            {
                richTextBox1.Text = ex.Message.ToString() + ex.StackTrace.ToString();
                richTextBox1.Visible = true;
            }            
        }

        private int ProcessFile(string oldPassword, string harFile, string zipPath)
        {
            int successCount = 0;
            if (oldPassword != null)
            {
                if (oldPassword.Length != 0 &&
                    oldPassword != "DELETED")
                {

                    string newharFile = harFile.Replace(oldPassword, "DELETED");

                    using (StreamWriter writer = new StreamWriter(zipPath))
                    {
                        writer.Write(newharFile);
                        successCount++;
                    }
                }
            }
            return successCount;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {           
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (Path.GetExtension(openFileDialog1.FileName) == ".saz"
                    || Path.GetExtension(openFileDialog1.FileName) == ".har")
                {                    
                    ReadFile(openFileDialog1.FileName, Path.GetExtension(openFileDialog1.FileName));
                }
                else
                {
                    MessageBox.Show("This is not a valid network trace file. Please select a .saz or .har file.", "Invalid File Type...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }        
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace 代码整理
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string foldPath;
//        string[] filesPath;
        private void button1_Click(object sender, EventArgs e)
        {
            string defaultfilePath = "";
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "请选择文件路径";
            folder.SelectedPath = defaultfilePath;
            if (folder.ShowDialog() == DialogResult.OK)
            {
                foldPath = folder.SelectedPath;
                //MessageBox.Show("已选择文件夹:" + foldPath, "选择文件夹提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox1.Text = foldPath;
                defaultfilePath = foldPath;
                dirCl(foldPath);
                MessageBox.Show("完成:", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public int dirCl(string foldPath)
        {
            try
            {
                DirectoryInfo mydir = new DirectoryInfo(foldPath);
                FileInfo[] infos = mydir.GetFiles();
                DirectoryInfo[] zdirs = mydir.GetDirectories();
                FilePcl(infos);
                foreach (DirectoryInfo zdir in zdirs)
                {
                    string name = foldPath + "\\" + zdir.Name;
                    dirCl(name);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("错误", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return 1;
            }
            return 0;
        }
        public int FilePcl(FileInfo[] infos)
        {
            foreach (FileInfo file in infos)
            {
                if (file.Name.EndsWith(".v"))
                {
                    string strBody = "";
                    try
                    {
                        StreamReader sr = new StreamReader(file.DirectoryName + "\\" + file.Name, GetEncoding(file));
                        strBody = sr.ReadToEnd();
                        sr.Close();
                        StringCl(ref strBody);
                        UTF8Encoding utf8 = new UTF8Encoding(false);
                        StreamWriter sw = new StreamWriter(file.DirectoryName + "\\" + file.Name, false, utf8);
                        sw.Write(strBody);
                        sw.Close();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("错误" + file.Name, "警告", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
/*                    using (FileStream fs = file.Open(FileMode.Open))
                    {
                        fs.Seek(0, SeekOrigin.Begin);
                        fs.Close();
                    }*/
                }
            }
            return 0;
        }
        public static Encoding GetEncoding(FileInfo file)
        {
            Encoding encoding1 = Encoding.Default;
            try
            {
                using (FileStream stream1 = file.Open(FileMode.Open, FileAccess.Read))
                {
                    if (stream1.Length > 0)
                    {
                        using (StreamReader reader1 = new StreamReader(stream1, true))
                        {
                            char[] chArray1 = new char[1];
                            reader1.Read(chArray1, 0, 1);
                            encoding1 = reader1.CurrentEncoding;
                            reader1.BaseStream.Position = 0;
                            if (encoding1 == Encoding.UTF8)
                            {
                                byte[] buffer1 = encoding1.GetPreamble();
                                if (stream1.Length >= buffer1.Length)
                                {
                                    byte[] buffer2 = new byte[buffer1.Length];
                                    stream1.Read(buffer2, 0, buffer2.Length);
                                    for (int num1 = 0; num1 < buffer2.Length; num1++)
                                    {
                                        if (buffer2[num1] != buffer1[num1])
                                        {
                                            encoding1 = Encoding.Default;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    encoding1 = Encoding.Default;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            if (encoding1 == null)
            {
                encoding1 = Encoding.UTF8;
            }
            return encoding1;
        }
        public void StringCl(ref string strBody)
        {
            string strBody_done = "";
            int index = 0;
            Deal.Last last = new Deal.Last();
            while (Deal.delete_space(ref strBody, ref strBody_done, ref index) == 1) ;
            strBody = strBody_done;
            strBody_done = "";
            index = 0;
            while (Deal.deal1(ref strBody, ref strBody_done, ref last, ref index) == 1) ;
            strBody = strBody_done;
        }
    }
}

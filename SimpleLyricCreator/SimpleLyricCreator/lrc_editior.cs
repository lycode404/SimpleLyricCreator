using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shell32;

namespace SimpleLyricCreator
{
    public partial class lrc_editior : Form
    {
        public lrc_editior()
        {
            InitializeComponent();
        }
        bool record = false;
        private void lrc_editior_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                path.Text = openFileDialog1.FileName;
                axWindowsMediaPlayer1.URL = path.Text;
                FileInfo info = new FileInfo(path.Text);

                ShellClass sh = new ShellClass();
                Folder dir = sh.NameSpace(Path.GetDirectoryName(path.Text));
                FolderItem item = dir.ParseName(Path.GetFileName(path.Text));

                artist.Text = dir.GetDetailsOf(item,13);
                title.Text = dir.GetDetailsOf(item,21);
                album.Text = dir.GetDetailsOf(item,14);
                au.Text = dir.GetDetailsOf(item, 236);
                leng.Text = dir.GetDetailsOf(item, 27);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView1.Items[0].Text = "[ar:" + artist.Text + "]";
            listView1.Items[1].Text = "[ti:" + title.Text + "]";
            listView1.Items[2].Text = "[al:" + album.Text + "]";
            listView1.Items[3].Text = "[by:" + by.Text + "]";
            listView1.Items[4].Text = "[offset:" + offset.Text + "]";
            listView1.Items[5].Text = "[au:" + au.Text + "]";
            listView1.Items[6].Text = "[length:" + leng.Text + "]";
            listView1.Items[7].Text = "[re:" + editior.Text + "]";
            listView1.Items[8].Text = "[ve:" + ver.Text + "]";
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double time = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            toolStripStatusLabel1.Text = time.ToString();
            string min= ((int)time / 60).ToString();
            string sec= (time - ((int)time / 60) * 60).ToString("#0.00");

            if (min.Length<=1)
            {
                min = "0" + min;
            }
            if (sec.Length<=4)
            {
                sec = "0" + sec;
            }
            toolStripStatusLabel2.Text = "[" + min + ":" + sec+"]";

        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (string lyric in fullLyric.Lines)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.SubItems.Add(lyric);
                listView1.Items.Add(lvi);  
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (record==true)
            {
                try
                {
                    richTextBox1.Text = listView1.SelectedItems[0].SubItems[1].Text;
                    listView1.SelectedItems[0].Text = toolStripStatusLabel2.Text;
                }
                catch
                {
                    //logerrors(err);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            switch (record)
            {
                case true:
                    button4.Text = "开始录制";
                    axWindowsMediaPlayer1.Ctlcontrols.pause();
                    record = false;
                    break;
                case false:
                    button4.Text = "停止录制";
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                    listView1.Select();
                    listView1.Items[8].Selected = true;
                    listView1.Items[8].Focused = true;
                    record = true;
                    break;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.FileName = openFileDialog1.SafeFileName.Remove(openFileDialog1.SafeFileName.Length - 4, 4);
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string finallyric = "";
                    foreach (ListViewItem lvi in listView1.Items)
                    {
                        finallyric += lvi.Text + lvi.SubItems[1].Text + "\r\n";
                    }
                    StreamWriter sw = File.AppendText(saveFileDialog1.FileName);
                    sw.Write(finallyric);
                    sw.Flush();
                    sw.Close();

                }

            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
                logerrors(err);
            }
        }

        private void 关于本软件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 abbox = new AboutBox1();
            abbox.Show();
        }
        private void logerrors(Exception err)
        {
            StreamWriter sw = File.AppendText(System.AppDomain.CurrentDomain.BaseDirectory + "errlog.txt");
            sw.Write(err.ToString() + "\r\n");
            sw.Flush();
            sw.Close();
        }
    }
}

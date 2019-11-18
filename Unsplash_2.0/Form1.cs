using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.IO;
using Microsoft.Win32;

namespace Unsplash_2._0
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]

        public static extern int SystemParametersInfo(UAction uAction, int uParam, StringBuilder lpvParam, int fuWinIni);

        string workingpath = System.IO.Directory.GetCurrentDirectory();
        public Form1()
        {
            InitializeComponent();
            notifyIcon1.Visible = true;    //显示托盘图标
            System.Timers.Timer aTimer = new System.Timers.Timer
            {
                Interval = 120000,
                Enabled = true
            };
            aTimer.Start();
            aTimer.Elapsed += new ElapsedEventHandler(AutoSetWallPaper);
        }

        public enum UAction
        {
            /// <summary>
            /// set the desktop background image
            /// </summary>
            SPI_SETDESKWALLPAPER = 0x0014,
            /// <summary>
            /// set the desktop background image
            /// </summary>
            SPI_GETDESKWALLPAPER = 0x0073,
        }

        private void Form1_Load(object sender, EventArgs e)  //默认最小化
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
        }
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)  //双击托盘显示界面，不显示界面
        {
            //this.Visible = true;
            //this.ShowInTaskbar = true;
            //this.Show();
            //this.Focus();
        }

        private void Form1_AutoSizeChanged(object sender, EventArgs e)  //最小化的时候隐藏到托盘
        {
            if (this.WindowState == FormWindowState.Minimized)    //最小化到系统托盘
            {
                this.Hide();    //隐藏窗口
            }
        }
        //退出软件
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Application.Exit();
        }

        //手动更新壁纸
        private void 更新壁纸ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread fetch_set = new Thread(Downpng_set);
            fetch_set.Start();
        }

        //保存当前壁纸
        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            var wallpapername = dt.GetHashCode().ToString();
            wallpapername = wallpapername.Substring(1);
            string filepath = workingpath + "\\Picture\\" + wallpapername + ".png";
            try
            {
                File.Move(workingpath + "\\Picture\\temp.png", filepath);
            }
            catch
            {
                MessageBox.Show("壁纸已保存");
            }
        }

        //任务栏图标右键操作
        private void NotifyIcon1_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show();
            }
        }

        public static string GetBackgroud()
        {
            StringBuilder s = new StringBuilder(300);
            SystemParametersInfo(UAction.SPI_GETDESKWALLPAPER, 300, s, 0);
            return s.ToString();
        }

        public static int SetBackgroud(string fileName)
        {
            int result = 0;
            if (File.Exists(fileName))
            {
                StringBuilder s = new StringBuilder(fileName);
                result = SystemParametersInfo(UAction.SPI_SETDESKWALLPAPER, 0, s, 0x2);
            }
            return result;
        }

        //public static bool SetOptions(string optionsName, string optionsData, ref string msg)
        //{
        //    bool returnBool = true;
        //    RegistryKey classesRoot = Registry.CurrentUser;
        //    RegistryKey registryKey = classesRoot.OpenSubKey(@"Control Panel\Desktop", true);
        //    try
        //    {
        //        if (registryKey != null)
        //        {
        //            registryKey.SetValue(optionsName.ToUpper(), optionsData);
        //        }
        //        else
        //        {
        //            returnBool = false;
        //        }
        //    }
        //    catch
        //    {
        //        returnBool = false;
        //        msg = "Error when read the registry";
        //    }
        //    finally
        //    {
        //        classesRoot.Close();
        //        registryKey.Close();
        //    }
        //    return returnBool;
        //}

        //下载壁纸
        private void Downpng_set()
        {
            notifyIcon1.Text= "正在更新";
            try
            {
                if (!Directory.Exists(workingpath + "\\Picture"))
                    Directory.CreateDirectory(workingpath + "\\Picture");
                if (File.Exists(workingpath + "\\Picture\\temp.png"))
                {
                    File.Delete(workingpath + "\\Picture\\temp.png");
                }
                //这一行是用来下载Unsplash的壁纸的
                System.Net.WebRequest webreq = System.Net.WebRequest.Create("https://source.unsplash.com/featured/1920x1080/?+");
                System.Net.WebResponse webres = webreq.GetResponse();
                Stream stream = webres.GetResponseStream();

                Image drawingImage = Image.FromStream(stream);
                stream.Dispose();

                drawingImage.Save(workingpath + "\\Picture\\temp.png");
            }
            catch (Exception)
            {
            }
            GetBackgroud();
            SetBackgroud(workingpath + "\\Picture\\temp.png");
            notifyIcon1.Text = "";
        }
        private void AutoSetWallPaper(object source, ElapsedEventArgs e)
        {
            Thread fetch_set = new Thread(Downpng_set);
            fetch_set.Start();
        }

    }
}

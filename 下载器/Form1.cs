using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
//using WebSocketSharp;
namespace 下载器
{
    public partial class Form1 : Form
    {
        string text = "更新";
        public Form1()
        {
            InitializeComponent();
            CheckVersion();
        }
        static List<string> config => File.ReadAllLines("config.ini").ToList();

        public void CheckVersion()
        {
            string ip = config[0];
            var checkVersionClient = new WebSocket($"ws://{ip}/CheckVersion");
            FileInfo fileInfo = new FileInfo(comboBox1.Text + "//GameVersion.txt");
            string currentVersion = File.ReadAllText(comboBox1.Text + "//GameVersion.txt");

            checkVersionClient.OnMessage += (send, ev) =>
            {
                Console.WriteLine("服务器存在版本为" + ev.Data);
                if (fileInfo.Exists)
                {
                    if (currentVersion == ev.Data)
                    {
                        Console.WriteLine("当前为最新版本");
                    }
                    else
                    {
                        Console.WriteLine("请点击更新");
                    }
                }
            };
            checkVersionClient.OnClose += (send, ev) => { Console.WriteLine(ev.Reason); };
            checkVersionClient.OnError += (send, ev) => { Console.WriteLine(ev.Message); };
            checkVersionClient.Connect();
            checkVersionClient.Send(comboBox1.Text);
        }
        private void btn_Update_Click(object sender, EventArgs e)
        {

            string ip = config[0];
            //ip = "127.0.0.1:495";
            var downloadClient = new WebSocket($"ws://{ip}/Download");
            TaskManager.Init();
            downloadClient.OnMessage += (send, ev) =>
            {
                FileData fileData = ev.Data.ToObject<FileData>();
                TaskManager.Add(fileData);
                Console.WriteLine("接收文件中" + fileData.fileName + ":" + fileData.currentRank + "/" + fileData.totalRank);
                downloadClient.Send("Success");
            };
            downloadClient.OnClose += (send, ev) => { Console.WriteLine(ev.Reason); };
            downloadClient.OnError += (send, ev) => { Console.WriteLine(ev.Message); };
            //Console.ReadLine();
            downloadClient.Connect();
            Console.WriteLine("连接完成");
            FlieList flieList = new FlieList(comboBox1.Text);
            new DirectoryInfo(comboBox1.Text).GetFiles("*", SearchOption.AllDirectories).ToList().ForEach(file => flieList.Add(file));
            downloadClient.Send(flieList.ToJson());
            Console.WriteLine("发送完毕");

            //Console.ReadLine();
            //downloadClient.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            btn_Update.Text=text;
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            CheckVersion();
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            Process.Start(new DirectoryInfo(comboBox1.Text).GetFiles("*.exe").First().FullName);
        }
    }
}

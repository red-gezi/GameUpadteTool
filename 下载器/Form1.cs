using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public Form1()
        {
            InitializeComponent();
        }
        static List<string> config => File.ReadAllLines("config.ini").ToList();

        private void btn_Update_Click(object sender, EventArgs e)
        {

            string ip = config[0];
            //ip = "127.0.0.1:495";
            var downloadClient = new WebSocket($"ws://{ip}/Download");
            downloadClient.OnMessage += (send, ev) =>
            {
                FileData fileData = ev.Data.ToObject<FileData>();
                TaskManager.Add(fileData);
                Console.WriteLine("接收文件中" + fileData.fileName + ":" + fileData.currentRank + "/" + fileData.totalRank);
                downloadClient.Send("Success");
            };
            downloadClient.OnClose += (send, ev) => { Console.WriteLine(ev.Reason); };
            downloadClient.OnError += (send, ev) => { Console.WriteLine(ev.Message); };
            downloadClient.Connect();
            Console.WriteLine("连接完成");
            downloadClient.Send(comboBox1.Text);
            Console.WriteLine("发送完毕");
        }
    }
}

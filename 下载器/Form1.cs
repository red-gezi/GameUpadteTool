using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
//using WebSocketSharp;
namespace 下载器
{
    public partial class Form1 : Form
    {
        string ip => config[0];
        string tag => config[1];
        bool isUpdateOver = false;
        List<ServerFileInfo> serverFiles = new List<ServerFileInfo>();
        static List<string> config => File.ReadAllLines("config.ini").ToList();
        private void comboBox1_TextChanged(object sender, EventArgs e) => GetServerFileList();

        public Form1()
        {
            InitializeComponent();
            label1.Parent = pictureBox1;
            //ip = "127.0.0.1:495";
        }
        public void GetServerFileList()
        {
            isUpdateOver = false;
            var websocket = new WebSocket($"ws://{ip}/GetServerFileList");
            websocket.OnMessage += (send, ev) =>
            {
                //Console.WriteLine(ev.Data.ToObject<List<ServerFileInfo>>().ToJson(Newtonsoft.Json.Formatting.Indented));
                serverFiles = ev.Data.ToObject<List<ServerFileInfo>>();
                websocket.Close();
                Console.WriteLine("检查本地文件列表");
                CheckClinetFileList();
            };
            websocket.OnClose += (send, ev) => { Console.WriteLine(ev.Reason); };
            websocket.OnError += (send, ev) => { Console.WriteLine(ev.Message); };
            websocket.Connect();
            websocket.Send(tag);
        }
        public void CheckClinetFileList()
        {
            List<ServerFileInfo> clientFileInfos = new List<ServerFileInfo>();
            new DirectoryInfo(tag).Create();
            new DirectoryInfo(tag).GetFiles("*.*", SearchOption.AllDirectories).ToList().ForEach(file =>
            {
                string fileName = file.FullName.Replace(Directory.GetCurrentDirectory(), "");
                clientFileInfos.Add(new ServerFileInfo(fileName, file.Length));
            });
            //对比服务器和客户端文件记录
            clientFileInfos.ForEach(clientFile =>
            {
                var targetFile = serverFiles.FirstOrDefault(serverFile => serverFile.name == clientFile.name && serverFile.length == clientFile.length);
                if (targetFile != null)
                {
                    serverFiles.Remove(targetFile);
                }
            });
            isUpdateOver = true;
            SetProgressBar(serverFiles.Count);

            if (serverFiles.Any())
            {
                serverFiles.ToList().ForEach(serverFile =>
                {
                    var websocket = new WebSocket($"ws://{ip}/Download");
                    websocket.OnMessage += (send, ev) =>
                    {
                        string targetPath = Directory.GetCurrentDirectory() + serverFile.name;
                        new FileInfo(targetPath).Directory.Create();
                        File.WriteAllBytes(targetPath, ev.RawData);
                        AddProgressBar();
                        if (progressBar1.Value == progressBar1.Maximum)
                        {
                            GetServerFileList();
                        }
                    };
                    websocket.OnClose += (send, ev) => { Console.WriteLine(ev.Reason); };
                    websocket.OnError += (send, ev) => { Console.WriteLine(ev.Message); };
                    websocket.Connect();
                    Console.WriteLine("连接完成:下载" + serverFile.name);
                    websocket.Send(serverFile.ToJson());
                });
                Console.WriteLine("发送完毕");
            }
            else
            {
                MessageBox.Show("这时启动游戏");
                //Process.Start(new DirectoryInfo(comboBox1.Text).GetFiles("*.exe").First().FullName);
            }
        }
        public void SetProgressBar(int num)
        {
            Action a = () =>
            {

                progressBar1.Visible = !(num == 0);
                label1.Visible = !(num == 0);
                progressBar1.Maximum = num;
                progressBar1.Value = num == 0 ? 0 : 0;
            };
            Invoke(a);
        }
        public void AddProgressBar()
        {
            Action a = () =>
            {
                progressBar1.Value++;
            };
            Invoke(a);
        }
        private void btn_start_Click(object sender, EventArgs e) => GetServerFileList();
    }
}

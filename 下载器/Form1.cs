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
        string ip = configs[0];
        string tag => configs[1];
        List<ServerFileInfo> serverFiles = new List<ServerFileInfo>();
        static List<string> configs => File.ReadAllLines("config.ini").ToList();
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            var tempConfigs = configs;
            tempConfigs[1] = comboBox1.Text;
            File.WriteAllLines("config.ini", tempConfigs);
        }
        public Form1()
        {
            InitializeComponent();
            label1.Parent = pictureBox1;
            //ip = "127.0.0.1:495";
        }
        public void GetServerFileList()
        {
            var websocket = new WebSocket($"ws://{ip}/GetServerFileList");
            websocket.OnMessage += (send, ev) =>
            {
                serverFiles = ev.Data.ToObject<List<ServerFileInfo>>();
                Console.WriteLine("服务端文件列表数量" + serverFiles.Count);
                websocket.Close();
                _ = CheckClinetFileListAsync();
            };
            websocket.OnClose += (send, ev) => { Console.WriteLine(ev.Reason); };
            websocket.OnError += (send, ev) => { Console.WriteLine(ev.Message); };
            websocket.Connect();
            websocket.Send(tag);
        }
        public async Task CheckClinetFileListAsync()
        {
            List<ServerFileInfo> clientFileInfos = new List<ServerFileInfo>();
            new DirectoryInfo(tag).Create();
            new DirectoryInfo(tag).GetFiles("*.*", SearchOption.AllDirectories).ToList().ForEach(file =>
            {
                string fileName = file.FullName.Replace(Directory.GetCurrentDirectory(), "");
                clientFileInfos.Add(new ServerFileInfo(fileName.Replace('\\', '/'), file.Length));
            });
            Console.WriteLine("客户端文件列表数量" + serverFiles.Count);
            //对比服务器和客户端文件记录
            clientFileInfos.ForEach(clientFile =>
            {
                var targetFile = serverFiles.FirstOrDefault(serverFile =>
                {
                    string serverName = serverFile.name.Replace('\\', '/');//解决windows和linux平台上路径分隔符不一致问题
                    return serverName == clientFile.name && serverFile.length == clientFile.length;
                    });
                ;
                if (targetFile != null)
                {
                    serverFiles.Remove(targetFile);
                }
            });
            Console.WriteLine("对比后文件列表数量" + serverFiles.Count);
            SetProgressBar(serverFiles.Count);

            if (serverFiles.Any())
            {
                foreach (var serverFile in serverFiles.ToList())
                {
                    Console.WriteLine("下载一个文件");
                    var websocket = new WebSocket($"ws://{ip}/Download");
                    //bool isFIleDownOver = false;
                    List<byte> fileData = new List<byte>();
                    websocket.OnMessage += (send, ev) =>
                    {
                        string targetPath = Directory.GetCurrentDirectory() + serverFile.name;
                        new FileInfo(targetPath).Directory.Create();
                        if (ev.Data == "over")
                        {
                            File.WriteAllBytes(targetPath, fileData.ToArray());
                            //isFIleDownOver = true;
                            AddProgressBar();
                            if (progressBar1.Value == progressBar1.Maximum)
                            {
                                GetServerFileList();
                            }
                            websocket.Close();
                        }
                        else
                        {
                            fileData.AddRange(ev.RawData);
                            Console.Write("*");
                        }
                    };
                    websocket.OnClose += (send, ev) => { Console.WriteLine(ev.Reason); };
                    websocket.OnError += (send, ev) => { Console.WriteLine(ev.Message); };
                    websocket.Connect();
                    Console.WriteLine("连接完成:下载" + serverFile.name);
                    websocket.Send(serverFile.ToJson());
                    //await Task.Run(async () =>
                    //{
                    //    while (!isFIleDownOver)
                    //    {
                    //        await Task.Delay(100);
                    //    }
                    //});
                };
                Console.WriteLine("发送完毕");
            }
            else
            {
                //MessageBox.Show("这时启动游戏");
                Process.Start(new DirectoryInfo(comboBox1.Text).GetFiles("*.exe").First().FullName);
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

namespace 下载器
{
    class ServerFileInfo
    {
        public string name;
        public long length;

        public ServerFileInfo(string name, long length)
        {
            this.name = name;
            this.length = length;
        }
    }
}

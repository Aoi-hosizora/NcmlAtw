using System.Text;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;

namespace NeteaseM2DServer
{
    class Program {

        static void Main(string[] args) {
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 1212));
            listener.Start();
            while (true) {
                TcpClient remoteClient = listener.AcceptTcpClient();
                NetworkStream streamToClient = remoteClient.GetStream();
                byte[] srcBuffer = new byte[1024];
                int bytesRead;
                try {
                    lock (streamToClient) {
                        bytesRead = streamToClient.Read(srcBuffer, 0, 1024);
                        byte[] retBuffer = new byte[bytesRead];
                        Buffer.BlockCopy(srcBuffer, 0, retBuffer, 0, bytesRead);
                        string ret = System.Text.Encoding.UTF8.GetString(retBuffer);
                        Console.WriteLine(ret);
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}

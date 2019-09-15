using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;

namespace NeteaseM2DServer.Src.Service
{
    class SocketService
    {

        /// <summary>
        /// 新线程，委托调用 `RunService`
        /// </summary>
        /// <param name="obj">int： 1212</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void RunThread(object obj) {
            if (obj == null)
                return;
            else {
                try {
                    RunService(Convert.ToInt32(obj));
                }
                catch (Exception) {
                    return;
                }
            }
        }

        /// <summary>
        /// 监听 Socket 服务
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void RunService(int port) {
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Parse("0.0.0.0"), port));
            listener.Start();
            while (true) {
                TcpClient remoteClient = listener.AcceptTcpClient();
                NetworkStream streamToClient = remoteClient.GetStream();
                byte[] srcBuffer = new byte[2048];
                int bytesRead;
                try {
                    string ret;
                    lock (streamToClient) {
                        bytesRead = streamToClient.Read(srcBuffer, 0, 1024);
                        if (bytesRead == 0) {
                            ret = "ping";
                        }
                        else {
                            byte[] retBuffer = new byte[bytesRead - 1];
                            Buffer.BlockCopy(srcBuffer, 0, retBuffer, 0, bytesRead - 1);
                            ret = System.Text.Encoding.UTF8.GetString(retBuffer);
                        }
                       
                        Console.WriteLine(ret);
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using NeteaseM2DServer.Src.Model;
using System.Threading;

namespace NeteaseM2DServer.Src.Service
{
    class SocketService {

        public int port { get; set; }
        public SuccessListenCackDelegate listenCb;
        public PingCallBackDelegate pingCb;
        public PlaybackStateCallBackDelegate playbackStateCb;
        public MetadataCallBackDelegate metaDataCb;

        public delegate void SuccessListenCackDelegate(bool ok);
        public delegate void PingCallBackDelegate(bool ok);
        public delegate void PlaybackStateCallBackDelegate(PlaybackState obj);
        public delegate void MetadataCallBackDelegate(Metadata obj);

        /// <summary>
        /// 新线程，委托调用 `RunService`
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RunThread() {
            if (port > 0 && port < 65536)
                RunService(port);
        }

        /// <summary>
        /// TCP Socket 服务器
        /// </summary>
        public TcpListener serverListener;

        /// <summary>
        /// 监听 Socket 服务
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RunService(int port) {
            serverListener = new TcpListener(new IPEndPoint(IPAddress.Parse("0.0.0.0"), port));
            try {
                serverListener.Start();
                listenCb(true);
            }
            catch (Exception ex) {
                // 监听失败
                Console.WriteLine(ex.Message);
                listenCb(false);
                return;
            }

            // 监听成功
            while (true) {
                TcpClient remoteClient;
                try {
                    // ブロック操作は WSACancelBlockingCall の呼び出しに割り込まれました。
                    remoteClient = serverListener.AcceptTcpClient();
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    break;
                }

                // 收到数据
                try {
                    using (NetworkStream remoteStream = remoteClient.GetStream()) {
                        byte[] srcBuffer = new byte[2048];
                        string ret;

                        int bytesRead;
                        lock (remoteStream) {
                            bytesRead = remoteStream.Read(srcBuffer, 0, 2048);
                            if (bytesRead == 0)
                                pingCb(true); // ping
                            else {
                                byte[] retBuffer = new byte[bytesRead - 1];
                                Buffer.BlockCopy(srcBuffer, 0, retBuffer, 0, bytesRead - 1);
                                ret = System.Text.Encoding.UTF8.GetString(retBuffer);

                                if (ret.StartsWith("{\"isPlay\":"))
                                    playbackStateCb(PlaybackState.parseJson(ret)); // PlaybackState
                                else
                                    metaDataCb(Metadata.parseJson(ret)); // Metadata
                            }
                        }
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
                finally {
                    remoteClient.Close();
                }
                Thread.Sleep(100);
            }
        }
    }
}

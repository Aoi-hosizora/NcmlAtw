using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace NcmlAtwServer {

    class SocketService {

        public delegate void ListenCallbackDelegate(bool ok);
        public delegate void PingCallbackDelegate();
        public delegate void MetadataCallbackDelegate(Metadata o);
        public delegate void PlaybackStateCallbackDelegate(PlaybackState o);
        public delegate void SessionDestroyedDelegate();

        public ListenCallbackDelegate listenCallback;
        public PingCallbackDelegate pingCallback;
        public MetadataCallbackDelegate metadataCallback;
        public PlaybackStateCallbackDelegate playbackStateCallback;
        public SessionDestroyedDelegate sessionDestroyedCallback;

        private TcpListener listener;
        private const int BUFFER_MAX_SIZE = 4096;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void StartService(int port) {
            listener = new TcpListener(new IPEndPoint(IPAddress.Parse("0.0.0.0"), port));
            try {
                listener.Start();
                listenCallback?.Invoke(true);
            } catch (Exception ex) {
                Console.WriteLine(ex);
                listenCallback?.Invoke(false);
                return;
            }

            while (true) {
                TcpClient client;
                try {
                    client = listener.AcceptTcpClient();
                } catch (ThreadAbortException) {
                    Console.WriteLine("abort");
                    return;
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                    continue;
                }

                try {
                    using (var stream = client.GetStream()) {
                        var buf = new byte[BUFFER_MAX_SIZE];
                        int bytes = stream.Read(buf, 0, buf.Length);
                        if (bytes != 0) {
                            var messageBuf = new byte[bytes - 1];
                            Buffer.BlockCopy(buf, 0, messageBuf, 0, bytes - 1);
                            ProcessReceivedMessage(Encoding.UTF8.GetString(messageBuf), stream);
                        }
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                } finally {
                    client.Close();
                }

                Thread.Sleep(100);
            }
        }

        public void StopService() {
            listener?.Stop();
        }

        private void ProcessReceivedMessage(string message, NetworkStream stream) {
            Console.WriteLine(message);
            if (message == "ping") {
                var buf = Encoding.UTF8.GetBytes("pong");
                stream.Write(buf, 0, buf.Length);
                pingCallback?.Invoke();
            } else if (message.StartsWith("{\"title\"")) {
                try {
                    metadataCallback?.Invoke(Metadata.FromJson(message));
                } catch { }
            } else if (message.StartsWith("{\"isPlaying\"")) {
                try {
                    playbackStateCallback?.Invoke(PlaybackState.FromJson(message));
                } catch { }
            } else if (message.StartsWith("{\"isDestroyed\"")) {
                sessionDestroyedCallback?.Invoke();
            }
        }
    }
}

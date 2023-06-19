using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading;

namespace Network
{
    public class NetworkManager
    {
        private TcpListener tcpListener;
        private TcpClient tcpClient;

        private bool isAsyncHost = false;
        private int bufferSize = 8192;


        /// <summary>
        /// Returns the TcpClient or null if no connection has been established.
        /// </summary>
        public TcpClient TcpClient { get => tcpClient; }

        /// <summary>
        /// Returns your current public IP address.
        /// Returns null if none of the APIs could be reached.
        /// </summary>
        public IPAddress PublicIPAddress
        {
            get
            {
                var publicIpApis = new List<String>();
                publicIpApis.Add("http://api.ipify.org");
                publicIpApis.Add("http://ipinfo.io/ip");
                var http = new HttpClient();

                foreach (var s in publicIpApis)
                {
                    try
                    {
                        var ip = http.GetStringAsync(s).Result.Trim();
                        if (ip.Length > 0)
                            return IPAddress.Parse(ip);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Returns your local IP address.
        /// </summary>
        public IPAddress LocalIPAddress
        {
            get =>
                (
                from host
                in Dns.GetHostEntry(Dns.GetHostName()).AddressList
                where host.AddressFamily == AddressFamily.InterNetwork
                select host
                ).FirstOrDefault();
        }

        /// <summary>
        /// Gets whether the <see cref="HostAsync(int, Action{TcpClient})"/> method is running.
        /// </summary>
        public bool IsAsyncHost { get => isAsyncHost; }

        /// <summary>
        /// Returns or sets the buffer size for all read and write operations.
        /// Default buffer size is 8192 bytes.
        /// </summary>
        public int BufferSize { get => bufferSize; set => bufferSize = value; }

        /// <summary>
        /// Initialises a new instance of the <see cref="NetworkManager"/> class.
        /// </summary>
        public NetworkManager()
        { }

        /// <summary>
        /// Initialises a new instance of the <see cref="NetworkManager"/> class and calls <see cref="Connect(string, int)".
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public NetworkManager(String ip, int port)
        {
            Connect(ip, port);
        }


        /// <summary>
        /// Works only if <see cref="Connect(string, int)"/> has been called successfully.
        /// Otherwise use <seealso cref="WriteAsync(NetworkStream, Stream)"/>.
        /// 
        /// Reads all bytes blockwise (<see cref="BufferSize"/>) asynchronously from the <see cref="Stream"/> and writes them into the <see cref="NetworkStream"/>.
        /// </summary>
        /// <param name="stream"></param>
        public async Task WriteAsync(Stream stream)
        {
            await WriteAsync(tcpClient.GetStream(), stream);
        }

        /// <summary>
        /// Works only if <see cref="Connect(string, int)"/> has been called successfully.
        /// Otherwise use <seealso cref="ReadAsync(NetworkStream, Stream)"/>.
        /// 
        /// Reads all bytes blockwise (<see cref="BufferSize"/>) asynchronously from the <see cref="NetworkStream"/> and writes them into the <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream"></param>
        public async Task ReadAsync(Stream stream)
        {
            await ReadAsync(tcpClient.GetStream(), stream);
        }

        /// <summary>
        /// Works only if <see cref="Connect(string, int)"/> has been called successfully.
        /// Otherwise use <seealso cref="Write(NetworkStream, Stream)"/>.
        /// 
        /// Reads all bytes blockwise (<see cref="BufferSize"/>) from the <see cref="Stream"/> and writes them into the <see cref="NetworkStream"/>.
        /// </summary>
        /// <param name="stream"></param>
        public void Write(Stream stream)
        {
            Write(tcpClient.GetStream(), stream);
        }

        /// <summary>
        /// Works only if <see cref="Connect(string, int)"/> has been called successfully.
        /// Otherwise use <seealso cref="Read(NetworkStream, Stream)"/>.
        /// 
        /// Reads all bytes blockwise (<see cref="BufferSize"/>) from the <see cref="Stream"/> and writes them into the <see cref="NetworkStream"/>.
        /// </summary>
        /// <param name="stream"></param>
        public void Read(Stream stream)
        {
            Read(tcpClient.GetStream(), stream);
        }

        /// <summary>
        /// Reads all bytes blockwise (<see cref="BufferSize"/>) asynchronously from the <see cref="Stream"/> and writes them into the <see cref="NetworkStream"/>.
        /// </summary>
        /// <param name="networkStream"></param>
        /// <param name="stream"></param>
        public async Task WriteAsync(NetworkStream networkStream, Stream stream)
        {
            var buffer = new byte[bufferSize];
            var lengthInBytes = BitConverter.GetBytes(stream.Length);

            networkStream.Write(lengthInBytes, 0, lengthInBytes.Length);
            int count;

            while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                await networkStream.WriteAsync(buffer, 0, count);
        }

        /// <summary>
        /// Reads all bytes blockwise (<see cref="BufferSize"/>) asynchronously from the <see cref="NetworkStream"/> and writes them into the <see cref="Stream"/>.
        /// </summary>
        /// <param name="networkStream"></param>
        /// <param name="stream"></param>
        public async Task ReadAsync(NetworkStream networkStream, Stream stream)
        {
            var buffer = new byte[bufferSize];
            var lengthInBytes = new byte[8];

            networkStream.Read(lengthInBytes, 0, lengthInBytes.Length);
            long length = BitConverter.ToInt64(lengthInBytes, 0);

            int count;
            long bytesToRead = length;
            while (bytesToRead > 0)
            {
                count = await networkStream.ReadAsync(buffer, 0, bytesToRead >= bufferSize ? bufferSize : (int)bytesToRead);
                await stream.WriteAsync(buffer, 0, count);
                bytesToRead -= count;
            }
        }

        /// <summary>
        /// Reads all bytes blockwise (<see cref="BufferSize"/>) from the <see cref="Stream"/> and writes them into the <see cref="NetworkStream"/>.
        /// </summary>
        /// <param name="networkStream"></param>
        /// <param name="stream"></param>
        public void Write(NetworkStream networkStream, Stream stream)
        {
            var buffer = new byte[bufferSize];
            var lengthInBytes = BitConverter.GetBytes(stream.Length);

            networkStream.Write(lengthInBytes, 0, lengthInBytes.Length);
            int count;

            while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                networkStream.Write(buffer, 0, count);
        }

        /// <summary>
        /// Reads all bytes blockwise (<see cref="BufferSize"/>) from the <see cref="NetworkStream"/> and writes them into the <see cref="Stream"/>.
        /// </summary>
        /// <param name="networkStream"></param>
        /// <param name="stream"></param>
        public void Read(NetworkStream networkStream, Stream stream)
        {
            var buffer = new byte[bufferSize];
            var lengthInBytes = new byte[8];

            networkStream.Read(lengthInBytes, 0, lengthInBytes.Length);
            long length = BitConverter.ToInt64(lengthInBytes, 0);

            int count;
            long bytesToRead = length;
            Console.WriteLine(length);
            while (bytesToRead > 0)
            {
                count = networkStream.Read(buffer, 0, bytesToRead >= bufferSize ? bufferSize : (int)bytesToRead);
                stream.Write(buffer, 0, count);
                bytesToRead -= count;
            }
        }

        /// <summary>
        /// Attempts to connect to specified ip and port
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns>true if connection was successfully established and false if not</returns>
        public bool Connect(String ip, int port)
        {
            try
            {
                tcpClient = new TcpClient(ip, port);
                return tcpClient.Connected;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Accepts all incoming connection requests and executes the passed method (<see cref="Action"/>) for each connected client.
        /// </summary>
        /// <param name="port">Port which should be listened to</param>
        /// <param name="action">Method with TcpClient as parameter which will be executed for each connection individually</param>
        public async void MultiHostAsync(int port, Action<TcpClient> action)
        {
            isAsyncHost = true;
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();

            await Task.Run(() =>
            {
                while (isAsyncHost)
                {
                    try
                    {
                        var tcpClient = tcpListener.AcceptTcpClient();
                        action(tcpClient);
                    }
                    catch (Exception ex)
                    {
                        Logging.LogError(ex);
                    }
                }
            });
        }

        /// <summary>
        /// Accept the first incoming connection request.
        /// </summary>
        /// <param name="port">Port which should be listened to</param>
        public void Host(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();

            tcpClient = tcpListener.AcceptTcpClient();

            tcpListener.Stop();
        }

        /// <summary>
        /// Stops the <see cref="HostAsync(int, Action{TcpClient})"/> method and the <see cref="TcpListener"/>.
        /// </summary>
        public void StopAsyncHost()
        {
            isAsyncHost = false;
            tcpListener.Stop();
        }

    }
}
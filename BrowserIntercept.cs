﻿using System.Net;
using System.Net.Sockets;
using System.Text;
using static DownloadManager.Logging;

namespace DownloadManager
{
    internal class BrowserIntercept
    {
        public BrowserIntercept _instance;
        public Socket httpServer;
        private int serverPort = 65535;
        public Thread thread;

        public void StartServer()
        {
            _instance = this;
            Log("Starting internal server...", Color.Black);
            try
            {
                httpServer = new Socket(SocketType.Stream, ProtocolType.Tcp);
                thread = new Thread(new ThreadStart(ConnectionThreadMethod));
                thread.Start();
            }
            catch (Exception ex)
            {
                Log("Error while starting internal server:" + Environment.NewLine + ex.Message + ex.StackTrace, Color.Red);
            }
        }

        private void ConnectionThreadMethod()
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, serverPort);
                httpServer.Bind(endPoint);
                httpServer.Listen(1);
                ListenForConnections();
            }
            catch (Exception ex)
            {
                Log("Error while starting internal server:" + Environment.NewLine + ex.Message + ex.StackTrace, Color.Red);
            }
        }

        private void ListenForConnections()
        {
            Log("Internal server started. Listening for connections on port " + serverPort + ".", Color.Green);
            while (true)
            {
                String data = "";
                byte[] bytes = new byte[2048];

                Socket client = httpServer.Accept();

                // Read inbound connection data
                while (true)
                {
                    int numBytes = client.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, numBytes);

                    if (data.IndexOf("\r\n") > -1)
                    {
                        break;
                    }
                }

                string[] splittedData = data.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                String url = splittedData[0].ToString().Replace("%22 HTTP/1.1", "");
                url = url.Replace("GET /?url=%22", "");

                if (!url.Contains("favicon.ico"))
                {
                    Logging._instance.Invoke((MethodInvoker)delegate
                    {
                        Log("Request received for URL: " + url, Color.Black);
                        DownloadProgress downloadProgress = new DownloadProgress(url, Settings1.Default.defaultDownload, "", "", 0);
                        downloadProgress.Show();
                        //Log("--- Start Request ---", Color.Black);
                        //Log(data, Color.Black);
                        //Log("--- End Request ---", Color.Black);
                    });
                }

                String resHeader = "HTTP/1.1 200 OK\nServer: DownloadManager_Internal\nContent-Type: text/html; charset: UTF-8\n\n";
                String resBody = "<!DOCTYPE html> <head><title>Download Manager</title></head><body><p>Please do not manually send requests to the internal server.<br>If a request is sent incorrectly the internal server may crash. If this occurs please restart the Download Manager application.</p></body>";
                String resStr = resHeader + resBody;
                byte[] resData = Encoding.ASCII.GetBytes(resStr);
                client.SendTo(resData, client.RemoteEndPoint);
                client.Close();
            }
            Log("Internal server stopped unexpectedly. Restarting server...", Color.Red);
            ConnectionThreadMethod();
        }
    }
}

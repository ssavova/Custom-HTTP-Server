using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Custom_HTTP_Server
{
    class Program
    {
        public static async Task Main(string[] args)
        {

            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 8888);
            tcpListener.Start();

            while (true)
            {
                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                Task.Run(() => ProcessClientAsync(tcpClient));
            }
        }

        private static async Task ProcessClientAsync(TcpClient client)
        {
            const string newLine = "\r\n";

            using (NetworkStream networkStream = client.GetStream())
            {
                //TODO: use buffer
                byte[] requestBytes = new byte[10000000];
                int readedBytes = await networkStream.ReadAsync(requestBytes, 0, requestBytes.Length);
                string request =  Encoding.UTF8.GetString(requestBytes, 0, readedBytes);

                string responseText = @"<form action='/Account/Login' method='post'> 
                                                    <input type=text name='firstname' />
                                                    <input type=text name='lastname'/>
                                                    <input type=date name='date'/>
                                                    <input type=submit value='Login'/>
                                           </form > ";

                string response = "HTTP/1.0 200 OK" + newLine
                    + "Server: StefiServer/1.0" + newLine
                    + "Content-Type: text/html" + newLine
                    + "Content-Length: " + responseText.Length + newLine + newLine
                    + responseText;
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length);

                Console.WriteLine(request);
                Console.WriteLine(new string('=', 60));
            }
        }
        public static async Task MockBrowser()
        {
            // this is demo how to make browser

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://softuni.bg/");
            string result = await response.Content.ReadAsStringAsync();
            File.WriteAllText("index.html", result);
        }

    }
}
    

    

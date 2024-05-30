using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsynkServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void buttonStartServer_Click(object sender, EventArgs e)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            textBoxLog.AppendText("Server started...\r\n");

            while (true)
            {
                try
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    textBoxLog.AppendText("Client connected...\r\n");

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            using (NetworkStream stream = client.GetStream())
                            {
                                byte[] buffer = new byte[1024];
                                int bytesRead;
                                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                                {
                                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                                    Invoke((Action)(() => textBoxLog.AppendText($"Received: {message}\r\n")));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Invoke((Action)(() => textBoxLog.AppendText($"Error: {ex.Message}\r\n")));
                        }
                        textBoxLog.AppendText("Client disconnected...\r\n");
                    });
                }
                catch (Exception ex)
                {
                    textBoxLog.AppendText($"Error: {ex.Message}\r\n");
                }
            }
        }
    }
}

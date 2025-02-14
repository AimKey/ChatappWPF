using ChatApp.Constants;
using ChatApp.Net.IO;
using System.Net.Sockets;

namespace ChatApp.Net
{
    public class Server
    {
        TcpClient _client;
        public PacketBuilder packetBuilder;
        public PacketReader packetReader;
        public event Action ConnectedEvent;
        public event Action MessageReceivedEvent;
        public event Action UserDisconnectedEvent;


        public Server()
        {
            _client = new TcpClient();
        }

        // Connect to the server in ChatAppServer program.cs
        public void ConnectToServer(String username)
        {
            if (!_client.Connected)
            {
                // Connect to server with the ip address and port
                _client.Connect("192.168.1.19", 3000);

                packetReader = new PacketReader(_client.GetStream());
                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    // Introduce yourself to the server by sending your username
                    connectPacket.WriteOpCode(AppConstants.OPCODE_CLIENT_SEND_USERNAME);
                    connectPacket.WriteMessage(username);
                    _client.Client.Send(connectPacket.GetPacketBytes()); // send to da server
                }
                // Start reading packets non stop until disconnected
                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            // Use thread to not block the main thread
            Task.Run(() =>
            {
                while (true)
                {
                    // Handle every packet that is sent by the server
                    var opcode = packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            ConnectedEvent?.Invoke(); // Invoke the event that added into this ConnectedEvent, see MainViewModel.cs
                            break;
                        case 5:
                            MessageReceivedEvent?.Invoke();
                            break;
                        case 10:
                            UserDisconnectedEvent?.Invoke();
                            break;
                        default:
                            Console.Write("Unsupported opcode");
                            break;
                    }
                }
            });
        }

        public void sendMessageToServer(String msg)
        {
            var packet = new PacketBuilder();
            packet.WriteOpCode(AppConstants.OPCODE_CLIENT_SEND_MESSAGE);
            packet.WriteMessage(msg);
            _client.Client.Send(packet.GetPacketBytes());
        }
    }
}

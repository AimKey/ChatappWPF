using ChatAppServer.Net.IO;
using System.Net.Sockets;

namespace ChatAppServer.Model
{
    public class Client
    {
        public string Username { get; set; }
        public Guid Id { get; set; }
        // The socket to communicates with the client
        public TcpClient ClientSocket { get; set; }
        PacketReader _packetReader;
        public Client(TcpClient tcpClient)
        {
            ClientSocket = tcpClient;
            //this.Username = username;
            Id = Guid.NewGuid();

            _packetReader = new PacketReader(ClientSocket.GetStream());
            var opcode = _packetReader.ReadByte();

            Username = _packetReader.ReadMessage();
            Console.WriteLine($"[{DateTime.Now}]: A wild {Username} has appeared! ID: {Id}");
            Task.Run(() => Program.BroadcastMessage($"[{DateTime.Now}]: A wild {Username} has entered the chat"));
            // Put the process in another thread so it doesn't block the main thread
            Task.Run(() => Process());
        }

        // Constantly read the packets from the client and broadcast it to all users
        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = _packetReader.ReadMessage();
                            Program.BroadcastMessage($"[{DateTime.Now}]: [{Username}]: {msg}");
                            break;
                        default:
                            Console.WriteLine("Not supported OPCODE: " + opcode);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[{Username} - {Id}]: Disconnected");
                    Console.WriteLine(e.Message);
                    Program.BroadcastDisconnect(Id.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}

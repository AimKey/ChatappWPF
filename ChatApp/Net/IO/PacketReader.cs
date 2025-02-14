using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChatApp.Net.IO
{
    /// <summary>
    /// Read the stuff client sent to the server
    /// </summary>
    public class PacketReader : BinaryReader
    {
        public NetworkStream _ns { get; }
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }

        /// <summary>
        /// This one read the length and message and return the message
        /// </summary>
        /// <returns></returns>
        public string ReadMessage()
        {
            try
            {
                byte[] msgBuffer;
                var length = ReadInt32(); // Remember the 4 bytes determine the length ?
                msgBuffer = new byte[length];
                _ns.Read(msgBuffer, 0, length);

                var msg = Encoding.ASCII.GetString(msgBuffer);
                return msg;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Something went wrong: " + e.Message;
            }
        }

    }
}

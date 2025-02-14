using System.Text;

namespace ChatAppServer.Net.IO
{
    class PacketBuilder
    {
        private MemoryStream _memoryStream; // Write da bytes into da memory

        public PacketBuilder()
        {
            _memoryStream = new MemoryStream();
        }


        // Write the opcode to the memory stream, 0 for sending message, 1 for sending file <br/>
        // You can use int as well but byte is enough for this small app, we are not going to have 2 billion opcode anyway <br/>
        // Opcode is the code to determine what action is about (send msg or file)
        /// <summary>
        /// Write the opcode to the memory stream
        /// </summary>
        /// <param name="opcode"></param>
        public void WriteOpCode(byte opcode)
        {
            _memoryStream.WriteByte(opcode);
        }

        /// <summary>
        /// Send the message to the server, this one already write the bytes for length of the message
        /// </summary>
        /// <param name="msg"></param>
        public void WriteMessage(String msg)
        {
            var len = msg.Length;
            _memoryStream.Write(BitConverter.GetBytes(len)); // Convert the length to 4 bytes 
            //_memoryStream.Write(Encoding.UTF8.GetBytes(msg)); // Uncomment this if you want Vietnamese support
            _memoryStream.Write(Encoding.ASCII.GetBytes(msg));
            // The payload should look like this: [1 byte OPCODE][4 bytes determine the length of MSG][msg]
        }

        /// <summary>
        /// Get the bytes of the packet that we are storing in the memory stream
        /// </summary>
        /// <returns></returns>
        public Byte[] GetPacketBytes()
        {
            return _memoryStream.ToArray();
        }

        //TODO: Send files !
        public void WriteFile(String path)
        {
            var bytes = File.ReadAllBytes(path);
            _memoryStream.Write(bytes, 0, bytes.Length);
        }
    }
}

using System.Net.Sockets;

namespace im_bored.net{
    public class PacketReceivedEvent(Packet packet, IClientData? sender) : EventArgs{
        public Packet Packet{get;} = packet;
        /// <summary>
        /// Null if the source is a server.
        /// </summary>
        public IClientData? Sender{get;} = sender; // TODO: Change to custom NetClient based system
    }
}
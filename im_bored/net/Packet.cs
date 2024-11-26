using System.Text;

namespace im_bored.net{
    public enum PacketHeader{
        Userdata // Example
    }
    public interface IPacket{
        PacketHeader Header{get;}
        byte[] Raw();
        void Construct(byte[] raw);
    }
    public abstract class Packet : IPacket{
        public abstract PacketHeader Header{get;}
        protected byte[] Data{get;set;} = [];
        protected string StringData{get{return Encoding.UTF8.GetString(Data);}}
        public void Construct(byte[] raw){
            if (raw.Length < 1) throw new Exception("Invalid RAW.");
            if (Header != (PacketHeader)raw[0]) throw new Exception($"Invalid header:\n * Expected: {Header}\n * Got: {(PacketHeader)raw[0]}");
            Data = new byte[raw.Length-1];
            Array.Copy(raw, 1, Data, 0, Data.Length);
        }
        public byte[] Raw(){
            if (Data.Length == 0) throw new Exception("Packet data hasn't been set yet.");
            byte[] raw = new byte[1 + Data.Length];
            raw[0] = (byte)Header;
            Array.Copy(Data, 0, raw, 1, Data.Length);
            return raw;
        }
        public static implicit operator ReadOnlySpan<byte>(Packet packet){
            return packet.Raw();
        }
        public override string ToString(){
            return $"{Header}|{StringData}";
        }
    }
    public class UserdataPacket : Packet{
        public override PacketHeader Header => PacketHeader.Userdata;
        public string Username { get; }
        public string GPU { get; }
        public string CPU { get; }
        public string Platform { get; }
        public UserdataPacket(byte[] raw){
            Construct(raw);
            string[] parts = StringData.Split("|$|");
            Username = parts[0];
            GPU = parts[1];
            CPU = parts[2];
            Platform = parts[3];
        }
        public UserdataPacket(string username, string gpu, string cpu, string platform){
            Username = username;
            GPU = gpu;
            CPU = cpu;
            Platform = platform;
            Data = Encoding.UTF8.GetBytes(string.Join("|$|",username,gpu,cpu,platform));
        }
    }
    public class PacketFactory{
        public static IPacket Create(byte[] raw){
            if (raw.Length < 1) throw new Exception("Invalid RAW.");
            PacketHeader header = (PacketHeader)raw[0];
            IPacket packet = header switch
            {
                PacketHeader.Userdata => new UserdataPacket(raw),
                _ => throw new NotSupportedException($"Unknown header: {header}")
            };
            return packet;
        }   
    }
}

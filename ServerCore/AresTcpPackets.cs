using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class AresTcpPackets
    {
        public static byte[] UserListBotItem()
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteUInt16(0);
            packet.WriteUInt32(0);
            packet.WriteIP(System.Net.IPAddress.Parse("0.0.0.0"));
            packet.WriteUInt16(0);
            packet.WriteIP(System.Net.IPAddress.Parse("0.0.0.0"));
            packet.WriteUInt16(0);
            packet.WriteByte(0);
            packet.WriteString(Settings.Get<String>("bot"));
            packet.WriteIP(System.Net.IPAddress.Parse("0.0.0.0"));
            packet.WriteByte(0);
            packet.WriteByte(3);
            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteString(String.Empty);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_CHANNEL_USER_LIST);
        }
        public static byte[] UpdateUserStatus(UserObject client, UserObject target)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteString(client, target.Name);
            packet.WriteUInt16(target.FileCount);
            packet.WriteByte((byte)(0));
            packet.WriteIP(target.NodeIP);
            packet.WriteUInt16(target.NodePort);
            packet.WriteIP("0.0.0.0");
            packet.WriteByte((byte)target.Level);
            packet.WriteByte(target.Age);
            packet.WriteByte(target.Sex);
            packet.WriteByte(target.Country);
            packet.WriteString(client, target.Location);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_UPDATE_USER_STATUS);
        }
        public static byte[] FastPing()
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_FASTPING);
        }
        public static byte[] BotAvatar(byte[] data)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteString(Settings.Get<String>("bot"));
            packet.WriteBytes(data);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_AVATAR);
        }
        public static byte[] NoSuch(String text)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();

            if (text.Length > 1024)
                text = text.Substring(0, 1024);

            packet.WriteString(text, false);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_NOSUCH);
        }
        public static byte[] Avatar(UserObject userobj)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteString(userobj.Name);
            packet.WriteBytes(userobj.Avatar);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_AVATAR);
        }
        public static byte[] LoginAck(UserObject userobj)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteString(userobj.Name);
            packet.WriteString(Settings.Get<String>("name"));
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_LOGIN_ACK);
        }

        public static byte[] TopicFirst()
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteString(Settings.Get<String>("topic"), false);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_TOPIC_FIRST);
        }
        public static byte[] MyFeatures(UserObject userobj)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteString(Settings.VERSION);
            packet.WriteByte(7);
            packet.WriteByte(63);
            packet.WriteByte((byte)Settings.Get<ushort>("lenguage"));
            packet.WriteUInt32(userobj.Cookie);
            packet.WriteByte(1);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_MYFEATURES);
        }
        public static byte[] Join(UserObject userobj)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteUInt16(userobj.FileCount);
            packet.WriteUInt32(0);
            packet.WriteIP(userobj.ExternalIP);
            packet.WriteUInt16(userobj.Port);
            packet.WriteIP(userobj.NodeIP);
            packet.WriteUInt16(userobj.NodePort);
            packet.WriteByte(0);
            packet.WriteString(userobj.Name);
            packet.WriteIP(userobj.LocalIP);
            packet.WriteByte(userobj.CanBrowse ? (byte)1 : (byte)0);
            packet.WriteByte(userobj.Level);
            packet.WriteByte(userobj.Age);
            packet.WriteByte(userobj.Sex);
            packet.WriteByte(userobj.Country);
            packet.WriteString(userobj.Location);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_JOIN);
        }
        public static byte[] PersonalMessage(UserObject userobj)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteString(userobj.Name);
            packet.WriteString(userobj.PersonalMessage, false);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_PERSONAL_MESSAGE);
        }
        public static byte[] Part(UserObject userobj)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteString(userobj.Name);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_PART);
        }
        public static byte[] OpChange(UserObject userobj)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteByte(userobj.Level > 0 ? (byte)1 : (byte)0);
            packet.WriteByte(0);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_OPCHANGE);
        }
        public static byte[] UserListItem(UserObject userobj)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteUInt16(userobj.FileCount);
            packet.WriteUInt32(0);
            packet.WriteIP(userobj.ExternalIP);
            packet.WriteUInt16(userobj.Port);
            packet.WriteIP(userobj.NodeIP);
            packet.WriteUInt16(userobj.NodePort);
            packet.WriteByte(0);
            packet.WriteString(userobj.Name);
            packet.WriteIP(userobj.LocalIP);
            packet.WriteByte(userobj.CanBrowse ? (byte)1 : (byte)0);
            packet.WriteByte(userobj.Level);
            packet.WriteByte(userobj.Age);
            packet.WriteByte(userobj.Sex);
            packet.WriteByte(userobj.Country);
            packet.WriteString(userobj.Location);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_CHANNEL_USER_LIST);
        }
        public static byte[] Public(String username, String text)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteString(username);
            packet.WriteString(text, false);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_PUBLIC);
        }
        public static byte[] UserListEnd()
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteByte(0);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_CHANNEL_USER_LIST_END);
        }
        public static byte[] UpdateUserStatus(UserObject userobj)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteString(userobj.Name);
            packet.WriteUInt16(userobj.FileCount);
            packet.WriteByte(userobj.CanBrowse ? (byte)1 : (byte)0);
            packet.WriteIP(userobj.NodeIP);
            packet.WriteUInt16(userobj.NodePort);
            packet.WriteIP(userobj.ExternalIP);
            packet.WriteByte(userobj.Level);
            packet.WriteByte(userobj.Age);
            packet.WriteByte(userobj.Sex);
            packet.WriteByte(userobj.Country);
            packet.WriteString(userobj.Location);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_UPDATE_USER_STATUS);
        }
        public static byte[] AvatarCleared(UserObject client, UserObject target)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteString(client, target.Name);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_AVATAR);
        }

        public static byte[] Avatar(UserObject client, UserObject target)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteString(client, target.Name);
            packet.WriteBytes(target.Avatar);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_AVATAR);
        }

        public static byte[] BotAvatarCleared(UserObject client)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteString(client, Settings.Get<String>("bot"));
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_AVATAR);
        }

        public static byte[] BotAvatar(UserObject client, byte[] data)
        {
            AresTCPPacketWriter packet = new AresTCPPacketWriter();
            packet.WriteString(Settings.Get<String>("bot"));
            packet.WriteBytes(data);
            return packet.ToAresPacket(ProtoMessage.MSG_CHAT_SERVER_AVATAR);
        }
    }
}

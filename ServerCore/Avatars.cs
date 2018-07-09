using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Avatars
    {
        
        public static byte[] server_avatar { get; set; }
        public static byte[] default_avatar { get; set; }

        public static void UpdateServerAvatar(byte[] data)
        {
            server_avatar = Scale(data);
            if (UserPool.Users != null)
                UserPool.Users.ForEach(x => { x.SendPacket(AresTcpPackets.BotAvatar(server_avatar)); });
        }
        public static void UpdateDefaultAvatar(byte[] data)
        {
            default_avatar = Scale(data);
            if (UserPool.Users != null)
                UserPool.Users.ForEach(x => { if(x.Avatar.Length < 10) x.Avatar = default_avatar; });
        }
        
        public static void CheckAvatars()
        {
            if (default_avatar == null)
                return;
            UserPool.Users.ForEach(x => {
                if(x.Avatar.Length < 10)
                {
                    x.Avatar = default_avatar;
                    x.avatar = default_avatar;
                }
            });
        }
        internal static byte[] Server(UserObject client)
        {
            if (server_avatar == null)
                return AresTcpPackets.BotAvatarCleared(client);
            else
                return AresTcpPackets.BotAvatar(client, server_avatar);
        }
        private static byte[] Scale(byte[] raw)
        {
            byte[] result;

            using (MemoryStream raw_ms = new MemoryStream(raw))
            using (Bitmap raw_bmp = new Bitmap(raw_ms))
            using (Bitmap sized = new Bitmap(48, 48))
            using (Graphics g = Graphics.FromImage(sized))
            {
                using (SolidBrush sb = new SolidBrush(Color.White))
                    g.FillRectangle(sb, new Rectangle(0, 0, 48, 48));

                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawImage(raw_bmp, new Rectangle(0, 0, 48, 48));
                ImageCodecInfo info = new List<ImageCodecInfo>(ImageCodecInfo.GetImageEncoders()).Find(x => x.MimeType == "image/jpeg");
                EncoderParameters encoding = new EncoderParameters();
                encoding.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 69L);

                using (MemoryStream ms = new MemoryStream())
                {
                    sized.Save(ms, info, encoding);
                    result = ms.ToArray();
                }
            }

            return result;
        }

    }
}

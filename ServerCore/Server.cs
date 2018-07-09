using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Server
    {
        private Thread main_thread { get; set; }
        private TcpListener tcp_listener { get; set; }
        public bool Running = false;
        public Server()
        {
            this.main_thread = new Thread(new ThreadStart(this.SocketService));
        }
        public void Start()
        {
            Misc.Reset();
            this.main_thread.Start();
            this.Running = true;
        }
        public void Stop()
        {
            if(this.Running)
            {
                this.tcp_listener.Stop();
                UserPool.Users.ForEach(x => x.TerminateSocket());
                this.Running = false;
            }
        }
        private void SocketService()
        {
            UserPool.Init();
            
            this.tcp_listener = new TcpListener(new IPEndPoint(IPAddress.Any, (int)Settings.Get<ushort>("port")));
            this.tcp_listener.Start();

            uint last_time = Misc.UnixTime;
            ulong fast_ping_timer = Misc.Now;

            while (true)
            {
                ulong time = Misc.Now;
                if (!this.Running)
                    return;

                if (time > (fast_ping_timer + 2000))
                {
                    fast_ping_timer = time;
                    UserPool.Users.ForEach(x => { x.SendPacket(AresTcpPackets.FastPing()); } );
                    Avatars.CheckAvatars();
                }

                uint time_now = Misc.UnixTime;
                this.CheckDisponse(time_now);
                this.ServiceUsers(time_now);
                ServerEvents.CycleTick();
                Thread.Sleep(25);
            }
        }
        private void CheckDisponse(uint time)
        {
            while (this.tcp_listener.Pending())
                UserPool.Users.Add(new UserObject(this.tcp_listener.AcceptSocket(), time));
        }

        private void ServiceUsers(uint time)
        {
            UserPool.Users.ForEach(x => x.SocketTasks(time));
            UserPool.Users.FindAll(x => x.Expired).ForEach(x => x.Disconnect());
            UserPool.Users.RemoveAll(x => x.Expired);

        }
    }
}

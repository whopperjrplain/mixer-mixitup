﻿using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Threading.Tasks;

namespace MixItUp.Desktop.Util
{
    public abstract class SocketIOService
    {
        protected Socket socket;

        private string connectionURL;

        public SocketIOService(string connectionURL) { this.connectionURL = connectionURL; }

        public virtual Task Connect()
        {
            this.socket = IO.Socket(this.connectionURL);
            return Task.FromResult(0);
        }

        public virtual Task Disconnect()
        {
            if (this.socket != null)
            {
                this.socket.Close();
            }
            this.socket = null;
            return Task.FromResult(0);
        }

        protected void SocketReceiveWrapper(string eventString, Action<object> processEvent)
        {
            this.socket.On(eventString, (eventData) =>
            {
                try
                {
                    processEvent(eventData);
                }
                catch (Exception ex) { MixItUp.Base.Util.Logger.Log(ex); }
            });
        }

        protected void SocketEventReceiverWrapper<T>(string eventString, Action<T> processEvent)
        {
            this.SocketReceiveWrapper(eventString, (eventData) =>
            {
                JObject jobj = JObject.Parse(eventData.ToString());
                if (jobj != null)
                {
                    processEvent(jobj.ToObject<T>());
                }
            });
        }

        protected void SocketSendWrapper(string eventString, object data)
        {
            try
            {
                this.socket.Emit(eventString, data);
            }
            catch (Exception ex) { MixItUp.Base.Util.Logger.Log(ex); }
        }
    }
}

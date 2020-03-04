using SuperSocket.ClientEngine;
using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;   // DllImport
using Tardis.Socket;
using UnityEngine;
using WebSocket4Net;

namespace Tardis
{
    public class TardisWebglSocket : MonoBehaviour, ITardisSocket
    {
#if UNITY_WEBGL
        private int webGLSocket = -1;
        private bool isConnected = false;
        private Connector Connector = null;

        [DllImport("__Internal")]
        private static extern int TardisWebSocketConnect(IntPtr url, IntPtr callee, IntPtr open, IntPtr message, IntPtr close, IntPtr error);

        [DllImport("__Internal")]
        private static extern int TardisWebSocketState(int socket);

        [DllImport("__Internal")]
        private static extern void TardisWebSocketSend(int socket, IntPtr data, int length);

        [DllImport("__Internal")]
        private static extern void TardisWebSocketClose(int socket);

        [DllImport("__Internal")]
        private static extern void TardisWebSocketRecv(int socket, IntPtr data, int length);

        [DllImport("__Internal")]
        private static extern int TardisWebSocketRecvLength(int socket);

        [DllImport("__Internal")]
        private static extern int TardisWebSocketError(int socket, IntPtr data, int length);

        public void InitTardisSocket(Connector connector)
        {
            Connector = connector;
        }

        public bool IsConnected()
        {
            return isConnected;
        }

        private void OnOpen()
        {
            isConnected = true;
            Connector.TardisSocket.OnConnect(this, new EventArgs());

        }
        private void OnReceive()
        {
            if (this.webGLSocket == -1)
                return;

            int length = TardisWebSocketRecvLength(this.webGLSocket);

            if (length == 0)
                return;

            byte[] buffer = new byte[length];
            IntPtr unmanagedPointer = Marshal.AllocHGlobal(length);
            TardisWebSocketRecv(this.webGLSocket, unmanagedPointer, length);
            Marshal.Copy(unmanagedPointer, buffer, 0, length);
            Marshal.FreeHGlobal(unmanagedPointer);

            Connector.TardisSocket.Received(buffer, 0, buffer.Length);
        }

        private void OnClose()
        {
            isConnected = false;
            
            ClosedEventArgs arg = new ClosedEventArgs(-1, "Close Socket");
            Connector.TardisSocket.OnDisconnect(this, arg);
        }

        private void OnError()
        {
            isConnected = false;            
            ErrorEventArgs arg = new ErrorEventArgs(new SocketException((int)System.Net.Sockets.SocketError.Disconnecting));
            Connector.TardisSocket.OnErrors(this, arg);
        }

        private string GetError()
        {
            if (this.webGLSocket != -1)
            {
                byte[] buffer = new byte[512];
                IntPtr unmanagedPointer = Marshal.AllocHGlobal(512);
                TardisWebSocketError(this.webGLSocket, unmanagedPointer, 512);
                Marshal.Copy(unmanagedPointer, buffer, 0, 512);
                Marshal.FreeHGlobal(unmanagedPointer);
                return System.Text.Encoding.UTF8.GetString(buffer);
            }
            else
            {
                return "";
            }
        }


        public void Connect(string ip,int port)
        {
            string url = string.Format("{0}:{1}/tardis", ip, port);
            IntPtr target = Marshal.StringToHGlobalAnsi(url);
            IntPtr callee = Marshal.StringToHGlobalAnsi("TardisWebglSocket");
            IntPtr open = Marshal.StringToHGlobalAnsi("OnOpen");
            IntPtr message = Marshal.StringToHGlobalAnsi("OnReceive");
            IntPtr close = Marshal.StringToHGlobalAnsi("OnClose");
            IntPtr error = Marshal.StringToHGlobalAnsi("OnClose");

            webGLSocket = TardisWebSocketConnect(target, callee, open, message, close, error);

            Marshal.FreeHGlobal(target);
            Marshal.FreeHGlobal(callee);
            Marshal.FreeHGlobal(open);
            Marshal.FreeHGlobal(message);
            Marshal.FreeHGlobal(close);
            
            Marshal.FreeHGlobal(error);            
        }

        public void Close()
        {
            if (this.webGLSocket == -1) return;

            TardisWebSocketClose(this.webGLSocket);
            this.webGLSocket = -1;
        }

        public void Send(byte[] data, int offset, int length)
        {
            if (this.webGLSocket == -1)
            {
                return;
            }

            IntPtr ptr = Marshal.AllocHGlobal(data.Length - offset);
            Marshal.Copy(data, offset, ptr, data.Length);
            TardisWebSocketSend(this.webGLSocket, ptr, data.Length);
            Marshal.FreeHGlobal(ptr);
        }

        public void Init()
        {
        }
#else
        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Connect(string ip, int port)
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public bool IsConnected()
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] data, int offset, int length)
        {
            throw new NotImplementedException();
        }
#endif
    }
}
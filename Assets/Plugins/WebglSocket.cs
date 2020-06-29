using SuperSocket.ClientEngine;
using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;   // DllImport
using Gameflex.Socket;
using UnityEngine;
using WebSocket4Net;

namespace Gameflex
{
    public class WebglSocket : MonoBehaviour, ISocket
    {
#if UNITY_WEBGL
        private int webGLSocket = -1;
        private bool isConnected = false;
        private Connector Connector = null;

        [DllImport("__Internal")]
        private static extern int WebSocketConnect(IntPtr url, IntPtr callee, IntPtr open, IntPtr message, IntPtr close, IntPtr error);

        [DllImport("__Internal")]
        private static extern int WebSocketState(int socket);

        [DllImport("__Internal")]
        private static extern void WebSocketSend(int socket, IntPtr data, int length);

        [DllImport("__Internal")]
        private static extern void WebSocketClose(int socket);

        [DllImport("__Internal")]
        private static extern void WebSocketRecv(int socket, IntPtr data, int length);

        [DllImport("__Internal")]
        private static extern int WebSocketRecvLength(int socket);

        [DllImport("__Internal")]
        private static extern int WebSocketError(int socket, IntPtr data, int length);

        public void InitGameflexSocket(Connector connector)
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
            Connector.GameflexSocket.OnConnect(this, new EventArgs());

        }
        private void OnReceive()
        {
            if (this.webGLSocket == -1)
                return;

            int length = WebSocketRecvLength(this.webGLSocket);

            if (length == 0)
                return;

            byte[] buffer = new byte[length];
            IntPtr unmanagedPointer = Marshal.AllocHGlobal(length);
            WebSocketRecv(this.webGLSocket, unmanagedPointer, length);
            Marshal.Copy(unmanagedPointer, buffer, 0, length);
            Marshal.FreeHGlobal(unmanagedPointer);

            Connector.GameflexSocket.Received(buffer, 0, buffer.Length);
        }

        private void OnClose()
        {
            isConnected = false;
            
            ClosedEventArgs arg = new ClosedEventArgs(-1, "Close Socket");
            Connector.GameflexSocket.OnDisconnect(this, arg);
        }

        private void OnError()
        {
            isConnected = false;            
            ErrorEventArgs arg = new ErrorEventArgs(new SocketException((int)System.Net.Sockets.SocketError.Disconnecting));
            Connector.GameflexSocket.OnErrors(this, arg);
        }

        private string GetError()
        {
            if (this.webGLSocket != -1)
            {
                byte[] buffer = new byte[512];
                IntPtr unmanagedPointer = Marshal.AllocHGlobal(512);
                WebSocketError(this.webGLSocket, unmanagedPointer, 512);
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
            string url = string.Format("{0}:{1}/Gameflex", ip, port);
            IntPtr target = Marshal.StringToHGlobalAnsi(url);
            IntPtr callee = Marshal.StringToHGlobalAnsi("WebglSocket");
            IntPtr open = Marshal.StringToHGlobalAnsi("OnOpen");
            IntPtr message = Marshal.StringToHGlobalAnsi("OnReceive");
            IntPtr close = Marshal.StringToHGlobalAnsi("OnClose");
            IntPtr error = Marshal.StringToHGlobalAnsi("OnClose");

            webGLSocket = WebSocketConnect(target, callee, open, message, close, error);

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

            WebSocketClose(this.webGLSocket);
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
            WebSocketSend(this.webGLSocket, ptr, data.Length);
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
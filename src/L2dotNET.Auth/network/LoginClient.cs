﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using L2dotNET.Auth.basetemplate;
using L2dotNET.Auth.rcv_l2;
using L2dotNET.Auth.serverpackets;
using L2Crypt;

namespace L2dotNET.Auth
{
    public class LoginClient
    {
        public int SessionId;
        public EndPoint _address;
        public TcpClient _client;
        public NetworkStream _stream;
        private byte[] _buffer;
        private LoginCrypt _loginCrypt;
        public byte[] BlowfishKey;
        public ScrambledKeyPair RsaPair;

        public LoginClient(TcpClient tcpClient)
        {
            //  CLogger.extra_info("connection from " + tcpClient.Client.RemoteEndPoint);
            _client = tcpClient;
            _stream = tcpClient.GetStream();
            _address = tcpClient.Client.RemoteEndPoint;
            SessionId = new Random().Next(int.MaxValue);

            initializeNetwork();
        }

        public void initializeNetwork()
        {
            RsaPair = ClientManager.getInstance().getScrambledKeyPair();
            BlowfishKey = ClientManager.getInstance().getBlowfishKey();
            _loginCrypt = new LoginCrypt();
            _loginCrypt.updateKey(BlowfishKey);

            new System.Threading.Thread(read).Start();
            new System.Threading.Thread(sendInit).Start();
        }

        public void sendInit()
        {
            sendPacket(new SM_INIT(this));
        }

        public void sendPacket(SendBasePacket sbp)
        {
            sbp.write();
            byte[] data = sbp.ToByteArray();
            data = _loginCrypt.encrypt(data, 0, data.Length);
            List<byte> array = new List<byte>();
            array.AddRange(BitConverter.GetBytes((short)(data.Length + 2)));
            array.AddRange(data);
            _stream.Write(array.ToArray(), 0, array.Count);
            _stream.Flush();
        }

        public void read()
        {
            try
            {
                _buffer = new byte[2];
                _stream.BeginRead(_buffer, 0, 2, new AsyncCallback(OnReceiveCallbackStatic), null);
            }
            catch (Exception ex)
            {
                close();
                throw ex;
            }
        }

        private void OnReceiveCallbackStatic(IAsyncResult result)
        {
            int rs = 0;
            try
            {
                rs = _stream.EndRead(result);

                if (rs > 0)
                {
                    short Length = BitConverter.ToInt16(_buffer, 0);
                    _buffer = new byte[Length - 2];
                    _stream.BeginRead(_buffer, 0, Length - 2, new AsyncCallback(OnReceiveCallback), result.AsyncState);
                }
            }
            catch(Exception s)
            {
                CLogger.warning(_address + " was closed by force."+ s);
                close();
            }
        }

        public void close()
        {
            ClientManager.getInstance().removeClient(this);
        }

        private void OnReceiveCallback(IAsyncResult result)
        {
            _stream.EndRead(result);

            byte[] buff = new byte[_buffer.Length];
            _buffer.CopyTo(buff, 0);

            if (!_loginCrypt.decrypt(ref buff, 0, buff.Length))
            {
                CLogger.error("blowfish failed on " + _address + ". please restart auth server.");
            }
            else
            {
                handlePacket(buff);
                new System.Threading.Thread(read).Start();
            }
        }

        private void handlePacket(byte[] buff)
        {
            byte id = buff[0];

            //string str = "header: "+buff[0]+"\n";
            //foreach (byte b in buff)
            //    str += b.ToString("x2")+" ";

            //Console.WriteLine(str);
            //File.WriteAllText("header" + buff[0], str);

            ReceiveBasePacket msg = null;
            switch (id)
            {
                case 0x00:
                    msg = new RequestAuthLogin(this, buff);
                    break;
                case 0x02:
                    msg = new RequestServerLogin(this, buff);
                    break;
                case 0x05:
                    msg = new RequestServerList(this, buff);
                    break;
                case 0x07:
                    msg = new AuthGameGuard(this, buff);
                    break;

                default:
                    CLogger.warning("LoginClient: received unk request " + id);
                    break;
            }

            if (msg != null)
                new Thread(new ThreadStart(msg.run)).Start();
        }

        public int login1, login2;
        public void setLoginPair(int key1, int key2)
        {
            login1 = key1;
            login2 = key2;
        }

        public int play1, play2;
        public void setPlayPair(int key1, int key2)
        {
            play1 = key1;
            play2 = key2;
        }

        public L2Account activeAccount;
    }
}

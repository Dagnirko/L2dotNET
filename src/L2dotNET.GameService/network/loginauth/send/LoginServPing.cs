﻿using L2dotNET.Network;

namespace L2dotNET.GameService.Network.LoginAuth.Send
{
    class LoginServPing : GameserverPacket
    {
        public string Version;
        private readonly int _build;

        public LoginServPing(AuthThread th)
        {
            Version = th.Version;
            _build = th.Build;
        }

        public override void Write()
        {
            WriteByte(0xA0);
            WriteString(Version);
            WriteInt(_build);
        }
    }
}
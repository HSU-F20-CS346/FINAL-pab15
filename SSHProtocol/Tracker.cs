
using System;
using System.Collections.Generic;
using System.Text;

namespace SSHProtocol
{
    public class Tracker
    {
        public KeyManager KeyManager { get; set; }
        public int Transmissions { get; set; }
        public Tracker()
        {

        }
        public Tracker (KeyManager _keyManager, int _transmissions = 1)
        {
            KeyManager = _keyManager;
            Transmissions = _transmissions;
        }
        public void AddTransmission()
        {
            Transmissions += 1;
            KeyManager.RecalculateIV(Transmissions);
        }
    }
}

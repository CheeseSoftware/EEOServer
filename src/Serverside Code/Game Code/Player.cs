using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;

namespace MushroomsUnity3DExample
{
    public class Player : Movement.PhysicsPlayer
    {
        string name;

        public Player()
            : base(0, "", 0, 16, 16, false, false, false, 0, false, false, 0)
        {
        }

        //public Player(Message m)
        //     : base(m.GetInt(0), m.GetString(1).ToLower(), m.GetInt(2), m.GetFloat(3), m.GetFloat(4), m.GetBoolean(5), m.GetBoolean(6), m.GetBoolean(7), m.GetInt(8), false, false, 0)

        public string Name
        {
            get { return this.name; }
        }

        public object X { get; set; }

        public object Y { get; set; }

        public bool HasAccess { get; set; }

        public bool IsMod { get; set; }
    }
}

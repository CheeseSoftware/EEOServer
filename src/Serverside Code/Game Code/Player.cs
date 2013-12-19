using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;

namespace MushroomsUnity3DExample
{
    class Player : BasePlayer
    {
        string name;

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

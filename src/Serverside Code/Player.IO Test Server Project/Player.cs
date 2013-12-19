using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;

namespace DevelopmentTestServer
{
    class Player : BasePlayer
    {
        string name;

        public string Name
        {
            get { return this.name; }
        }
    }
}

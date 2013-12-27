using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MushroomsUnity3DExample.utlity
{
    public class Pair<A,B>
    {
        public A first;
        public B second;

        public Pair(A first, B second)
        {
            this.first = first;
            this.second = second;
        }
    }
}

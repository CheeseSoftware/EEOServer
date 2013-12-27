using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MushroomsUnity3DExample.utlity
{
    public class QueueReader<T>
    {
        private T[] t;
        private int index = 0;

        public QueueReader(T[] array)
        {
            this.t = array;
        }

        public T Dequeue()
        {
            return t[index++];
        }

        public int Length
        {
            get { return t.Length - index; }
        }

        public bool Empty
        {
            get { return t.Length == index; }
        }

    }
}

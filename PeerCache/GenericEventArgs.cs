using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeerCache
{
    public class GenericEventArgs<T> : EventArgs
    {
        public GenericEventArgs() : this(default(T)) { }
        public GenericEventArgs(T value) { Value = value; }
        public T Value { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Aoite.Net.Sockets
{
    class AdvSocketAsyncEventArgs : SocketAsyncEventArgs
    {
        public ArraySegment<byte> ArraySegmentBuffer { get; set; }
    }
}

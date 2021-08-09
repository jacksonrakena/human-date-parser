using System;
using System.Linq;

namespace HumanDateParser.Buffers
{
    internal class CharacterBufferStream : BufferStream<int>
    {
        public CharacterBufferStream(string script)
        {
            foreach (var ch in script)
            {
                _list.Add(ch);
            }
        }
    }
}
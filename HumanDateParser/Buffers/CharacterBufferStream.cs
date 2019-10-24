using System;

namespace HumanDateParser
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

        public override void Dispose()
        {
        }
    }
}
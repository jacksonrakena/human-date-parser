using System;

namespace HumanDateParser
{
    internal class CharacterBuffer : Buffer<int>
    {
        public CharacterBuffer(string script)
        {
            foreach (var ch in script)
            {
                _list.Add(ch);
            }
        }
    }
}
using System;

namespace HumanDateParser
{
    internal class CharacterEnumerator : Enumerator<int>
    {
        public CharacterEnumerator(string script)
        {
            foreach (var ch in script)
            {
                _list.Add(ch);
            }
        }
    }
}
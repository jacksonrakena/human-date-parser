using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HumanDateParser
{
    internal class Buffer<T> : IEnumerator<T>
    {
        private int _position = -1;
        protected List<T> _list = new List<T>();

        public T Current => _list[_position];

        object IEnumerator.Current => Current;

        public bool Any(Func<T, bool> predicate) => _list.Any(predicate);

        public T PeekNext() => Peek(1);

        public T Peek(int position)
        {
            var np = _position + position;
            return _list.ElementAtOrDefault(np);
        }

        public bool MoveNext()
        {
            _position++;
            return _position < _list.Count;
        }

        public void MoveBack()
        {
            _position--;
        }

        public void Reset()
        {
            _position = -1;
        }

        public void Dispose()
        {
        }
    }
}

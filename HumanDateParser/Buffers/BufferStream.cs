using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HumanDateParser
{
    internal abstract class BufferStream<T> : IEnumerator<T>, IEnumerable<T>
    {
        private int _position = -1;
        protected List<T> _list = new List<T>();

        public T Current => _list[_position];

        object IEnumerator.Current => Current;

        internal List<T> All() => _list;

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

        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public abstract void Dispose();
    }
}

using System;

namespace HumanDateParser
{
    internal class CharacterBuffer : ICharacterBuffer
    {
        private int _currentPosition;
        private readonly int[] _bufferArray;
        private readonly string _code;
        private readonly int _size;

        public CharacterBuffer(string script, int bufferSize)
        {
            _code = script;
            _size = bufferSize;
            _bufferArray = new int[_size];
            SetPosition(0);
        }

        public void SetPosition(int position)
        {
            _currentPosition = position;
        }

        public void Next(int length)
        {
            if(length > _size) length = _size;
            for (var i = 1; i <= length; i++)
                Next();
        }

        public void Next()
        {
            for (var i = 0; i < _size - 1; i++){
                _bufferArray[i] = _bufferArray[i + 1];
            }
            try{
                if (_currentPosition == _code.Length){
                    _bufferArray[_size - 1] = -1;
                }
                else{
                    _bufferArray[_size - 1] = _code[_currentPosition++];
                }
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public int Peek(int pos)
        {
            if (pos >= 1 && pos <= _size)
                return _bufferArray[pos - 1];
            return 0;
        }
    }
}
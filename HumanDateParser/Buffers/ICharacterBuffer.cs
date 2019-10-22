namespace HumanDateParser
{
    internal interface ICharacterBuffer
    {
        void SetPosition(int position);
        void Next(int length);
        void Next();
        // peaks at pos character
        int Peek(int pos);
    }
}
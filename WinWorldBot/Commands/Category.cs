namespace WinWorldBot.Commands
{
    // This is used instead of an enum because I don't want to have to cast to an int for every command
    internal class Category
    {
        public const int Main = 0;
        public const int Fun = 1;
        public const int Owner = 2;
    }
}
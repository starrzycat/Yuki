namespace Yuki.ColoredConsole
{
    public struct ColoredConsoleColor
    {
        public int R;
        public int G;
        public int B;

        public override string ToString()
        {
            return $"\x1b[38;2;{R};{G};{B}m";
        }
    }
}

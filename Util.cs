namespace LenovoController
{
    public static class Util
    {
        public static bool GetNthBit(uint num, int n)
        {
            return (num & (1 << n)) != 0;
        }
    }
}
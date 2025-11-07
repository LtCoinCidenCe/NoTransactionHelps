using System.Text;

namespace NTH.Utilities;

public static class RandomASCIIGenerator
{
    private static Random random = new();
    public static string GetString(int length)
    {
        StringBuilder builder = new(length);
        int minPrintable = 0x21;
        // int chars = 95;
        for (int i = 0; i < length; i++)
        {
            int letter = minPrintable + random.Next() % 95;
            builder.Append((char)letter);
        }
        return builder.ToString();
    }
}

using System.Text;

namespace NTH.Utilities;

public static class Random5ASCII
{
    private static Random random = new();
    public static string GetString()
    {
        StringBuilder builder = new(5);
        int minPrintable = 0x21;
        // int chars = 95;
        for (int i = 0; i < 5; i++)
        {
            int letter = minPrintable + random.Next() % 95;
            builder.Append((char)letter);
        }
        return builder.ToString();
    }
}

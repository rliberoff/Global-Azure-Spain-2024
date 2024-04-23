namespace GlobalAzureSpain.Agents.Demo;

internal static class Utils
{
    internal static void ReadKey()
    {
        Console.ReadKey(true);
    }

    internal static void Write(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }

    internal static void WriteLine(string message, ConsoleColor color)
    {
        Write(message, color);

        Console.WriteLine();
    }
}

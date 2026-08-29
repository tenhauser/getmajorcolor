using System.Threading.Tasks;
using GetMajorColors.Commands;

namespace GetMajorColors;

internal static class Program
{
    internal static async Task<int> Main(string[] args)
    {
        return await AnalyzeCommand.RunAsync(args, Console.Out, Console.Error).ConfigureAwait(false);
    }
}

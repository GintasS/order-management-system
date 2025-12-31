using System.Collections.Concurrent;
using System.Text.Json;
using CSS.Challenge.Domain.Models;

namespace CSS.Challenge.Domain.Utilities
{
    public static class FileOps
    {
        public static void WriteActionsToFile(string file, BlockingCollection<OrderStateChangeLog> allActions)
        {
            var text = JsonSerializer.Serialize(allActions.ToList());
            File.WriteAllText(file, text);
        }
    }
}

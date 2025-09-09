using System.Globalization;

record UserRecord(string Name, string Age, string Country);

static class CsvLoader
{
    public static List<UserRecord> Load(string path)
    {
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"Could not read CSV at {Path.GetFullPath(path)}");
            return new();
        }
        var lines = File.ReadAllLines(path);
        if (lines.Length == 0) return new();
        var header = lines[0].Split(',').Select(h => h.Trim()).ToArray();
        int idxName = Array.FindIndex(header, h => string.Equals(h, "name", StringComparison.OrdinalIgnoreCase));
        int idxAge = Array.FindIndex(header, h => string.Equals(h, "age", StringComparison.OrdinalIgnoreCase));
        int idxCountry = Array.FindIndex(header, h => string.Equals(h, "country", StringComparison.OrdinalIgnoreCase));
        var list = new List<UserRecord>();
        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var cols = line.Split(',').Select(c => c.Trim()).ToArray();
            string Col(int i) => i >= 0 && i < cols.Length ? cols[i] : string.Empty;
            list.Add(new UserRecord(Col(idxName), Col(idxAge), Col(idxCountry)));
        }
        return list;
    }
}

static class Logic
{
    public static List<UserRecord> FilterByMinAge(IEnumerable<UserRecord> users, int threshold = 30) =>
        users.Where(u => int.TryParse(u.Age, out var a) && a >= threshold).ToList();

    public static Dictionary<string, int> CountByCountry(IEnumerable<UserRecord> users) =>
        users.GroupBy(u => u.Country)
             .OrderBy(g => g.Key)
             .ToDictionary(g => g.Key, g => g.Count());

    public static double AverageAge(IEnumerable<UserRecord> users)
    {
        var ages = users.Select(u => int.TryParse(u.Age, out var a) ? a : (int?)null)
                        .Where(a => a.HasValue)
                        .Select(a => a!.Value)
                        .ToList();
        if (ages.Count == 0) return 0.0;
        var avg = ages.Average();
        return Math.Round(avg * 10) / 10.0;
    }

    public static List<UserRecord> TopNOldest(IEnumerable<UserRecord> users, int n = 3) =>
        users.OrderByDescending(u => int.TryParse(u.Age, out var a) ? a : -1)
             .Take(n)
             .ToList();

    public static string RegionFor(string country) => country switch
    {
        "Finland" or "Germany" or "France" or "UK" => "Europe",
        "USA" or "Canada" => "North America",
        "Brazil" => "South America",
        "India" or "Japan" => "Asia",
        "Australia" => "Oceania",
        _ => "Other"
    };

    public static Dictionary<string,int> UsersByRegion(IEnumerable<UserRecord> users) =>
        users.GroupBy(u => RegionFor(u.Country))
             .OrderBy(g => g.Key)
             .ToDictionary(g => g.Key, g => g.Count());
}

class Program
{
    private static readonly string CsvPath = Path.Combine(AppContext.BaseDirectory, "users.csv");

    static void LogKeyValueLines<TKey>(IDictionary<TKey, int> dict)
    {
        foreach (var kv in dict)
            Console.WriteLine($"  {kv.Key}: {kv.Value}");
    }

    static void DoSummary(List<UserRecord> users)
    {
        Console.WriteLine($"Total users: {users.Count}");
        var filtered = Logic.FilterByMinAge(users);
        Console.WriteLine($"Filtered count: {filtered.Count}");
        Console.WriteLine("Users per country:");
        LogKeyValueLines(Logic.CountByCountry(users));
        Console.WriteLine($"Average age: {Logic.AverageAge(users)}");
        Console.WriteLine("Top 3 oldest users:");
        foreach (var u in Logic.TopNOldest(users))
            Console.WriteLine($"  {u.Name} ({u.Age})");
        Console.WriteLine("Users per region:");
        LogKeyValueLines(Logic.UsersByRegion(users));
    }

    static int Main(string[] args)
    {
        var users = CsvLoader.Load(CsvPath);
        if (users.Count == 0) return 1;
        var op = args.Length > 0 ? args[0] : "summary";
        switch (op)
        {
            case "summary":
                DoSummary(users); break;
            case "filter":
                Console.WriteLine($"Filtered count: {Logic.FilterByMinAge(users).Count}"); break;
            case "group":
                Console.WriteLine("Users per country:");
                LogKeyValueLines(Logic.CountByCountry(users)); break;
            case "avg":
                Console.WriteLine($"Average age: {Logic.AverageAge(users)}"); break;
            case "top":
                foreach (var u in Logic.TopNOldest(users)) Console.WriteLine($"{u.Name} ({u.Age})"); break;
            case "region":
                Console.WriteLine("Users per region:");
                LogKeyValueLines(Logic.UsersByRegion(users)); break;
            default:
                Console.WriteLine($"Unknown operation '{op}'. Use summary|filter|group|avg|top|region.");
                return 2;
        }
        return 0;
    }
}

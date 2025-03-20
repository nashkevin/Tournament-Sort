using System.Runtime.CompilerServices;

internal static class Program
{
    const bool isDebug = true;

    static ConsoleKey leftKeybind = ConsoleKey.None;
    static ConsoleKey rightKeybind = ConsoleKey.None;

    static Random random = new();

    static int comparisonCount = 0;
    static int tournamentCount = 0;

    /// <summary>
    /// Prompts for console input to select a winning competitor.
    /// </summary>
    /// <returns>One winner from two competitors.</returns>
    static Competitor PromptForWinner(Competitor a, Competitor b)
    {
        if (leftKeybind == ConsoleKey.None)
        {
            Console.WriteLine("Press the key you would like to use for the LEFT selection.");
            leftKeybind = GetPressedKey();
        }

        if (rightKeybind == ConsoleKey.None)
        {
            Console.WriteLine("Press the key you would like to use for the RIGHT selection.");
            rightKeybind = GetPressedKey();
        }

        ConsoleKey pressedKey;
        do
        {
            Console.WriteLine($"\t{a,32}\t<-  or  ->\t{b}");
            pressedKey = GetPressedKey();
        }
        while (pressedKey != leftKeybind && pressedKey != rightKeybind);

        if (pressedKey == leftKeybind)
        {
            a.WinCount++;
            b.LossCount++;
            return a;
        }
        b.WinCount++;
        a.LossCount++;
        return b;
    }

    /// <summary>
    /// Gets the first ordered element and updates each competitor's win or loss count.
    /// </summary>
    /// <returns>One winner from a variable number of competitors.</returns>
    static Competitor? GetWinner(params Competitor[] competitors)
    {
        comparisonCount++;
        
        Competitor winner = isDebug
            ? competitors[competitors[0].Name.CompareTo(competitors[1].Name) < 0 ? 0 : 1] // test alphabetic sort
            : competitors.Min();

        foreach (Competitor competitor in competitors)
        {
            if (competitor.Equals(winner))
            {
                competitor.WinCount++;
            }
            else
            {
                competitor.LossCount++;
            }
        }
        return winner;
    }

    /// <summary>
    /// Gets a filtered list of undecided competitors.
    /// </summary>
    /// <returns>
    /// A <see cref="List{Competitor}"/> whose competitors are untested or have
    /// win ratios that are tied with another competitor.
    /// </returns>
    static List<Competitor> GetUndecided(this IList<Competitor> list)
    {
        return list
            .Where(x => x.AllCount == 0)
            .Union(list
                .GroupBy(x => new { x.WinRatio })
                .Where(x => x.Skip(1).Any())
                .SelectMany(x => x)
                .Shuffle(random)) // TODO random yields fewer comparisons than ordered by rank,
            .ToList();            // but could something else be optimal...?
    }

    /// <summary>
    /// <para>
    /// Creates match pairs of competitors, then gets a winner for each
    /// match, then makes a recursive call with the round winners until
    /// a single winner is determined.
    /// </para><para>
    /// If any round begins with an unmatched competitor (the count is odd),
    /// the unmatched competitor is advanced to the next round with neither a
    /// loss nor a win.
    /// </para>
    /// </summary>
    /// <param name="competitors"></param>
    static void ConductRound(List<Competitor> competitors)
    {
        const int padLength = 16;

        if (competitors.Count == 1)
        {
            Console.WriteLine($"Single Winner: {competitors[0]}");
            Console.WriteLine();
            return;
        }

        Console.WriteLine("---------------------------------- New Round -----------------------------------");
        List<Competitor> winners = [];
        for (int i = 0; i < competitors.Count / 2; i++)
        {
            Competitor? winner = isDebug
                ? GetWinner(competitors[i], competitors[^(i + 1)])
                : PromptForWinner(competitors[i], competitors[^(i + 1)]);

            Console.WriteLine($"{(competitors[i].Name + ' ').PadRight(padLength, '─')}┐");
            Console.WriteLine($"{string.Empty, -padLength}├── {winner}");
            Console.WriteLine($"{(competitors[^(i + 1)].Name + ' ').PadRight(padLength, '─')}┘");
            Console.WriteLine();
            winners.Add(winner);
        }
        if (0 < competitors.Count % 2) // advance the odd competitor out, if present
        {
            Console.WriteLine($"{(competitors[competitors.Count / 2].Name + ' ').PadRight(padLength + 2, '─')} {competitors[competitors.Count / 2]}");
            Console.WriteLine();
            winners.Add(competitors[competitors.Count / 2]);
        }

        ConductRound(winners);
    }

    public static void InsertionSort<T>(T[] array, int left, int right)
    {
        throw new NotImplementedException(); // TODO
    }

    public static void Merge<T>(T[] array, int l, int m, int r)
    {
        throw new NotImplementedException(); // TODO
    }

    public static void TimSort<T>(T[] array)
    {
        throw new NotImplementedException(); // TODO
    }

    static ConsoleKey GetPressedKey()
    {
        while (!Console.KeyAvailable)
        {
            Thread.Sleep(100);
        }
        return Console.ReadKey(intercept: true).Key;
    }

    public static void Shuffle<T>(this IList<T> list, Random random)
    {
        for (int i = list.Count - 1; 1 < i; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable, Random random)
    {
        var buffer = enumerable.ToList();
        for (int i = 0; i < buffer.Count; i++)
        {
            int j = random.Next(i, buffer.Count);
            yield return buffer[j];

            buffer[j] = buffer[i];
        }
    }

    private static string? GetSourceFilePath([CallerFilePath] string? path = null)
    {
        return path;
    }


    private static void Main(string[] args)
    {
        Random random = 1 < args.Length && int.TryParse(args[1], out int seed)
            ? new(seed)
            : new();

        string path = 0 < args.Length
            ? args[0]
            : Path.Combine(Path.GetDirectoryName(GetSourceFilePath()), @"test_kanto.txt");

        List<Competitor> competitors = File.ReadAllLines(path)
            .Select(x => new Competitor(x))
            .ToList();

        List<Competitor> undecided = competitors.GetUndecided();
        while (0 < undecided.Count)
        {
            Console.WriteLine("================================ New Tournament ================================");
            Console.WriteLine($"{undecided.Count} undecided");
            foreach (Competitor competitor in undecided)
            {
                Console.WriteLine($"{competitor.Name, -16}W:L = {competitor.WinRatio}");
            }
            Console.WriteLine();

            tournamentCount++;
            ConductRound(undecided);
            undecided = competitors.GetUndecided();
        }

        competitors.Sort();

        foreach (Competitor competitor in competitors)
        {
            Console.WriteLine($"{competitor.Name,-16}W:L = {competitor.WinRatio}");
        }
        Console.WriteLine();
        Console.WriteLine($"Tournaments: {tournamentCount}");
        Console.WriteLine($"Comparisons: {comparisonCount}");
    }
}

class Competitor(string name) : IComparable<Competitor>
{
    public static int comparisonCount = 0;
    public string Name { get; set; } = name;
    public int WinCount { get; set; }
    public int LossCount { get; set; }
    public int AllCount { get => WinCount + LossCount; }
    public decimal WinRatio
    {
        get => LossCount == 0
            ? WinCount * 1000 // arbitrarily large number to distinguish W/0 from W/1
            : Math.Round((decimal)WinCount / LossCount, 7); // rounding here obviates the need for equality deltas elsewhere
    }

    /// <summary>
    /// The properties used for ordering, listed by descending priority
    /// <list type="number">
    /// <item>if unequal, <c>WinRatio</c> (greater comes first)</item>
    /// <item>if neither Competitor has wins, <c>LossCount</c> (lesser comes first)</item>
    /// <item>if neither Competitor has losses, <c>WinCount</c> (greater comes first)</item>
    /// <item>if unequal, <c>AllCount</c> (greater comes first)</item>
    /// </list>
    /// </summary>
    public int CompareTo(Competitor? other)
    {
        if (other == null)
        {
            return 1;
        }

        if (other.WinRatio != WinRatio)
        {
            return other.WinRatio.CompareTo(WinRatio);
        }

        if (WinCount == 0)
        {
            return LossCount.CompareTo(other.LossCount);
        }

        if (LossCount == 0)
        {
            return other.WinCount.CompareTo(WinCount);
        }

        if (other.AllCount != AllCount)
        {
            return other.AllCount.CompareTo(AllCount);
        }

        return 0;
    }

    public override string ToString() => $"{Name} {WinRatio}";
}

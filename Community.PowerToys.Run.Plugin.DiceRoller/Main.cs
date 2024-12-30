using System.Text;
using System.Text.RegularExpressions;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.DiceRoller
{
    public class Main : IPlugin
    {
        public static string PluginID => "A6C12BABD1F4488882031CD16CCAE1BF";
        public string Name => "Dice Roller";
        public string Description => "Roll Dice using common dice notation (1d6 for example)";

        private const string ICON_PATH_LIGHT = "Images\\light-dice_1.png";

        private PluginInitContext? _context;
        private readonly Random _random = new Random();

        public void Init(PluginInitContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        private bool TryParseDiceNotation(string diceNotation, out (int diceCount, int diceSize, int? modifier) result)
        {
            result = (0, 0, null);
            var match = RegexExpression.DiceNotation().Match(diceNotation);
            if (!match.Success)
            {
                return false;
            }
            result.diceCount = int.Parse(match.Groups[1].Value);
            result.diceSize = int.Parse(match.Groups[2].Value);
            if (match.Groups[3].Success)
            {
                result.modifier = int.Parse(match.Groups[3].Value);
            }

            return true;
        }

        private Result GetDiceNotationResult(params (int diceCount, int diceSize, int? modifier)[] rollProperties)
        {
            if (rollProperties.Length == 0)
            {
                return new Result();
            }
            var result = new Result();
            result.SubTitle = "Press To Roll";
            result.IcoPath = ICON_PATH_LIGHT;
            var title = new StringBuilder();
            for (int i = 0; i < rollProperties.Length; i++)
            {
                var roll = rollProperties[i];
                title.Append($"{roll.diceCount}d{roll.diceSize}");
                title.Append(roll.modifier.HasValue ? $"{roll.modifier.Value:+#;-#;0}" : string.Empty);
                if (i < rollProperties.Length - 1)
                {
                    title.Append(" ");
                }
            }
            result.Title = title.ToString();

            int totalSum = 0;
            result.Action = e =>
            {
                var builder = new StringBuilder();

                for (int i = 0; i < rollProperties.Length; i++)
                {
                    var roll = rollProperties[i];
                    builder.Append($"{roll.diceCount}d{roll.diceSize}: ");
                    var rolls = new List<int>();
                    for (var j = 0; j < roll.diceCount; j++)
                    {
                        var rollValue = _random.Next(1, roll.diceSize + 1);
                        rolls.Add(rollValue);
                    }

                    builder.Append($"{string.Join(", ", rolls)}");
                    if (roll.modifier.HasValue)
                    {
                        builder.Append($" ({roll.modifier.Value:+#;-#;0})");
                    }

                    var total = rolls.Sum() + (roll.modifier ?? 0);

                    if (rolls.Count > 1 || roll.modifier.HasValue)
                    {
                        builder.Append($" = {total}");
                    }
                    if (i < rollProperties.Length - 1)
                    {
                        builder.Append(Environment.NewLine);
                    }
                    totalSum += total;
                }

                if (rollProperties.Length > 1)
                {
                    builder.AppendLine();
                    builder.Append($"Total: {totalSum}");
                }

                _context?.API.ShowMsg(
                    $"Rolling...",
                    builder.ToString(),
                    ICON_PATH_LIGHT,
                    false);

                return true;
            };

            return result;
        }

        public List<Result> Query(Query query)
        {
            var results = new List<Result>();

            if (query.Terms.Count > 0)
            {
                List<(int diceCount, int diceSize, int? modifier)> rolls = new();
                for (int i = 0; i < query.Terms.Count; i++)
                {
                    var search = query.Terms[i];
                    if (TryParseDiceNotation(search, out (int diceCount, int diceSize, int? modifier) rollProperties))
                    {
                        rolls.Add(rollProperties);
                    }
                }

                if (rolls.Count > 0)
                {
                    var result = GetDiceNotationResult(rolls.ToArray());
                    result.SubTitle = " (Add more rolls with Space)";
                    result.Score = 1000;
                    results.Add(result);
                }
            }

            if (results.Count == 0)
            {
                results.Add(new()
                {
                    QueryTextDisplay = query.Search,
                    Title = "Dice Roller",
                    SubTitle = "Write a common dice notation to create a roll. (1d6, 2d20, 5d8, etc..)" +
                    "\r\nMulti rolls can be added with Space." +
                    "\r\nAdd a modifier to the roll with +/-# (1d6+2, 2d20-5, etc...)",
                    Score = 1000,
                });
            }

            var d4 = GetDiceNotationResult((diceCount: 1, diceSize: 4, modifier: null));
            d4.Score = 60;
            results.Add(d4);

            var d6 = GetDiceNotationResult((diceCount: 1, diceSize: 6, modifier: null));
            d6.Score = 50;
            results.Add(d6);

            var d8 = GetDiceNotationResult((diceCount: 1, diceSize: 8, modifier: null));
            d8.Score = 40;
            results.Add(d8);

            var d10 = GetDiceNotationResult((diceCount: 1, diceSize: 10, modifier: null));
            d10.Score = 30;
            results.Add(d10);

            var d12 = GetDiceNotationResult((diceCount: 1, diceSize: 12, modifier: null));
            d12.Score = 20;
            results.Add(d12);

            var d20 = GetDiceNotationResult((diceCount: 1, diceSize: 20, modifier: null));
            d20.Score = 10;
            results.Add(d20);

            var d100 = GetDiceNotationResult((diceCount: 1, diceSize: 100, modifier: null));
            d100.Score = 0;
            results.Add(d100);

            return results;
        }
    }

    public static partial class RegexExpression
    {
        [GeneratedRegex(@"^(\d+)d(\d+)([\+\-]\d+)?$", RegexOptions.IgnoreCase)]
        public static partial Regex DiceNotation();
    }
}

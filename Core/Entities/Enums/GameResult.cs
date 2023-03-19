using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

namespace Core.Entities.Enums;

[JsonConverter(typeof(SmartEnumValueConverter<GameResult, string>))]
public sealed class GameResult : SmartEnum<GameResult, string>
{
    public static readonly GameResult NotOver = new(nameof(NotOver));
    public static readonly GameResult CrossesWon = new(nameof(CrossesWon));
    public static readonly GameResult NoughtsWon = new(nameof(NoughtsWon));
    public static readonly GameResult Draw = new(nameof(Draw));

    public GameResult(string name) : base(name, name.ToLower())
    {
    }
}
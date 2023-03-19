using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

namespace Core.Entities.Enums;

[JsonConverter(typeof(SmartEnumValueConverter<GameStatus, string>))]
public sealed class GameStatus : SmartEnum<GameStatus, string>
{
    public static readonly GameStatus Invited = new(nameof(Invited), true);
    public static readonly GameStatus Accepted = new(nameof(Accepted), true);
    public static readonly GameStatus Declined = new(nameof(Declined), false);
    public static readonly GameStatus Ended = new(nameof(Ended), false);
    public static readonly GameStatus Cancelled = new(nameof(Ended), false);

    public GameStatus(string name, bool inProcess) : base(name, name.ToLower())
    {
        InProcess = inProcess;
    }


    public bool InProcess { get; }
}
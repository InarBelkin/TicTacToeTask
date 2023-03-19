using System.ComponentModel.DataAnnotations;

namespace Core.Model;

public class InviteModel
{
    public int InvitedUserId { get; set; }
    [Range(1, 10)] public int CountOfRounds { get; set; }
}
namespace Core.Model;

public class InviteAnswerModel
{
    public required bool Accept { get; set; }
    public required int InvitingUserId { get; set; }
}
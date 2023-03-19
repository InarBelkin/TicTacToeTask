using System.Text;
using Core.Entities.Enums;

namespace Core.Services;

public class GameHelperService
{
    public const char Nought = 'o';
    public const char Cross = 'x';
    public const char Empty = ' ';

    public char[,] ConvertFieldToArray(string field)
    {
        if (field.Length != 9) throw new ArgumentException("Field's lenght must me equal to 9");
        var result = new char[3, 3];
        for (var i = 0; i < 9; i++) result[i / 3, i % 3] = field[i];

        return result;
    }

    public string ConvertArrayToField(char[,] array)
    {
        var result = new StringBuilder();
        for (var i = 0; i < 3; i++)
        for (var j = 0; j < 3; j++)
            result.Append(array[i, j]);

        return result.ToString();
    }

    public GameResult GetResult(char[,] field)
    {
        //rows,columns, diagonals
        var resLines = new List<char>[8];
        for (var i = 0; i < 8; i++) resLines[i] = new List<char>();

        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                resLines[i].Add(field[i, j]);
                resLines[3 + i].Add(field[j, i]);
            }

            resLines[6].Add(field[i, i]);
            resLines[7].Add(field[i, 2 - i]);
        }

        if (resLines.Any(r => r.All(c => c == Cross))) return GameResult.CrossesWon;
        if (resLines.Any(r => r.All(c => c == Nought))) return GameResult.NoughtsWon;
        return resLines.SelectMany(r => r).Any(c => c == Empty) ? GameResult.NotOver : GameResult.Draw;
    }
}
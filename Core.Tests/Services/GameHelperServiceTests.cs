using Core.Services;

namespace Core.Tests.Services;

public class GameHelperServiceTests
{
    private readonly GameHelperService service = new();

    [Fact]
    public void ConvertFieldToArray_Correct()
    {
        var array = service.ConvertFieldToArray("xo ox  xo");
        Assert.Equal(new[,] { { 'x', 'o', ' ' }, { 'o', 'x', ' ' }, { ' ', 'x', 'o' } }, array);
    }

    [Fact]
    public void ConvertArrayToField_Correct()
    {
        var field = service.ConvertArrayToField(new[,]
            { { 'x', 'o', ' ' }, { 'o', 'x', ' ' }, { ' ', 'x', 'o' } });
        Assert.Equal("xo ox  xo", field);
    }

    [Theory]
    [InlineData("x  xo xxo", "CrossesWon")]
    [InlineData("oo xo xxo", "NoughtsWon")]
    [InlineData("0x0xxxo o", "CrossesWon")]
    [InlineData("xoo x oox", "CrossesWon")]
    [InlineData("xoo o oox", "NoughtsWon")]
    [InlineData("oox x xox", "CrossesWon")]
    [InlineData("oxooxoxox", "Draw")]
    public void GetResult_IsCorrect(string field, string expected)
    {
        var fieldArr = service.ConvertFieldToArray(field);
        var result = service.GetResult(fieldArr);
        Assert.Equal(expected, result.Name);
    }
}
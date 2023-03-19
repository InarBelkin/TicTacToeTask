namespace Core.Dtos;

public record ErrorsDto(List<BlErrorDto> Errors)
{
    public ErrorsDto(params BlErrorDto[] errors) : this(errors.ToList())
    {
    }
}
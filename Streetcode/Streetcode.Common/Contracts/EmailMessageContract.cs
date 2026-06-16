namespace Streetcode.Common.Contracts;

public class EmailMessageContract
{
    public List<string> To { get; set; } = [];

    public string From { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
}
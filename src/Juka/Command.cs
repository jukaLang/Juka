namespace Juka
{
    public class Command
    {
        int id;
        string? shortName;
        string? longName;
        string? description;
        delegate double MyFunction(double x);
    }
}

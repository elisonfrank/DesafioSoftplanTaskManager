namespace Softplan.TaskManager.Shared;

public static class StringExtensions
{
    public static string MaskEmail(this string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2) return email;

        var name = parts[0];
        var masked = name[0] + new string('*', name.Length - 1);
        return $"{masked}@{parts[1]}";
    }
}
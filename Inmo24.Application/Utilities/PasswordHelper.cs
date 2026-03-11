namespace Inmo24.Application.Utilities;

public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool VerifyPassword(string passwordIngresada, string passwordHashGuardada)
    {
        if (string.IsNullOrEmpty(passwordIngresada) || string.IsNullOrEmpty(passwordHashGuardada))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(passwordIngresada, passwordHashGuardada);
        }
        catch
        {
            return false;
        }
    }
}
namespace Praxisarbeit_M295.Services
{
    public interface IJwtService
    {
        string GenerateToken(string username, string role);
    }
}

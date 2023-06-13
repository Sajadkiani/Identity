namespace Identity.Domain.IServices;

public interface IPasswordService
{
    string HashPassword(string password, string userName);
}
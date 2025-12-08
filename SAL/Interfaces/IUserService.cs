using InfraBuzzar.Models;

namespace InfraBuzzar.SAL.Interfaces
{
    public interface IUserService
    {
        bool addNewUser(User u);
        List<User> getAllUser();

        string ConvertToEncrypt(string password);
        string convertToDecrypt(string base64EncodeData);
    }
}

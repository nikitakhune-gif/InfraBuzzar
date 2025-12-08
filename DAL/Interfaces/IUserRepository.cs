using InfraBuzzar.Models;

namespace InfraBuzzar.DAL.Interfaces
{
    public interface IUserRepository
    {
        bool addUser(User u);
        List<User> getuser();
    }
}

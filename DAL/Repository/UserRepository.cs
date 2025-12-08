using InfraBuzzar.DAL.Interfaces;
using InfraBuzzar.Data;
using InfraBuzzar.Models;

namespace InfraBuzzar.DAL.Repository
{
    public class UserRepository : IUserRepository
    {

        public readonly ApplicationDbContext _ctx;
        public UserRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public bool addUser(User u)
        {
            _ctx.User.Add(u);

            _ctx.SaveChanges();
            return true;
        }

        public List<User> getuser()
        {
            // CollectionContex ctx = new CollectionContex();
            var Users = from user in _ctx.User select user;
            return Users.ToList<User>();
        }
    }
}

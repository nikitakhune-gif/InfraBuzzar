using InfraBuzzar.DAL.Interfaces;
using InfraBuzzar.Models;
using System.Text;

namespace InfraBuzzar.SAL.Services
{
    public class UserService : Interfaces.IUserService
    {
        private readonly IUserRepository userio;

        public UserService(IUserRepository userio)
        {
            this.userio = userio;
        }
        public string ConvertToEncrypt(String password)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(passwordBytes);

        }
        public string convertToDecrypt(string base64EncodeData)
        {
            var base64EncodeBytes = Convert.FromBase64String(base64EncodeData);
            var result = Encoding.UTF8.GetString(base64EncodeBytes);
            return result;
        }
        public bool addNewUser(User user)
        {
            DateTime date = DateTime.Now.Date;
            User newuser = user;
            newuser.RegistrationDate = date;
            newuser.Password = ConvertToEncrypt(user.Password);
            Boolean status = userio.addUser(newuser);
            return status;
        }

        public List<User> getAllUser()
        {
            List<User> alluser = userio.getuser();
            return alluser;
        }

    }
}


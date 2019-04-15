using App.Core.Domain;
using App.Core.Repository;

namespace App.Data.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }
    }
}

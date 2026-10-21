using BookingSystem.Domain.Entities;

namespace BookingSystem.Infrastructure.Persistence.Repositories;

public interface IUsersRepository
{
    Task<User?> GetByIdAndPwdAsync(long id, string password);
}

public class UsersRepository : IUsersRepository
{
    private static readonly List<User> _users =
    [
        new User
        {
            Id = 1,
            Name = "jd@gmail.com",
            Password = "1111",
            Email = "John Doe",
        },
        new User
        {
            Id = 2,
            Name = "js@gmail.com",
            Password = "1111",
            Email = "Jane Smith",
        }
    ];

    public Task<User?> GetByIdAndPwdAsync(long id, string password)
        => Task.FromResult(_users.FirstOrDefault(u => u.Id == id && u.Password == password));

}

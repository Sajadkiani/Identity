﻿using Identity.Domain.SeedWork;

namespace Identity.Domain.Aggregates.Users;

public interface IUserStore : IRepository<User, int>
{
    Task<User> GetTokenByRefreshAsync(string refreshToken);
    Task<User> GetByUserNameAsync(string userName);
}
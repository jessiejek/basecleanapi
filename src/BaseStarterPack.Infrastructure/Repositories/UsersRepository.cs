using BaseStarterPack.Application.Interfaces.Repositories;
using BaseStarterPack.Domain.Entities;
using BaseStarterPack.Infrastructure.Context;
using BaseStarterPack.Infrastructure.Repositories.Common;

namespace BaseStarterPack.Infrastructure.Repositories;

public sealed class UsersRepository(AppDbContext ctx) : Repository<User>(ctx), IUsersRepository { }

using ReadOrNot.Application.DTOs.Account;

namespace ReadOrNot.Application.Interfaces;

public interface IAccessTokenService
{
    AccessTokenResult Create(Guid userId, string email, string displayName);
}

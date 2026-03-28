using YepPet.Domain.Users;

namespace YepPet.Application.Auth;

public interface IAccessTokenIssuer
{
    AccessTokenResult Issue(User user);
}

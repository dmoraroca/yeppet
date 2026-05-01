using Zuppeto.Domain.Users;

namespace Zuppeto.Application.Auth;

public interface IAccessTokenIssuer
{
    AccessTokenResult Issue(User user);
}

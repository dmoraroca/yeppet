using System;
using System.Threading.Tasks;

namespace Zuppeto.Application.Common.Interfaces;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(Guid userId, string permissionKey);
}
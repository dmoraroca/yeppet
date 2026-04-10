using System;
using System.Threading.Tasks;

namespace YepPet.Application.Common.Interfaces;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(Guid userId, string permissionKey);
}
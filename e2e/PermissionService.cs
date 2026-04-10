using System;
using System.Threading.Tasks;
using YepPet.Application.Common.Interfaces;

namespace YepPet.Infrastructure.Services;

/// <summary>
/// Implementació base del servei de permisos seguint SOLID.
/// En aquesta fase, la lògica es basarà en els rols definits al domini.
/// </summary>
public class PermissionService : IPermissionService
{
    public Task<bool> HasPermissionAsync(Guid userId, string permissionKey)
    {
        // TODO: Implementar consulta real a la taula de permisos per rol
        return Task.FromResult(true); 
    }
}
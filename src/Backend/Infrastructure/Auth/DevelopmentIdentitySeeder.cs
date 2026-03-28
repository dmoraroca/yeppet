using Microsoft.EntityFrameworkCore;
using YepPet.Application.Auth;
using YepPet.Infrastructure.Persistence;
using YepPet.Infrastructure.Persistence.Entities;

namespace YepPet.Infrastructure.Auth;

public sealed class DevelopmentIdentitySeeder(
    YepPetDbContext dbContext,
    IPasswordHasher passwordHasher)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await EnsureUserAsync(
            email: "admin@admin.adm",
            password: "Admin123",
            role: "Admin",
            displayName: "Administrador YepPet",
            city: "Barcelona",
            country: "Espanya",
            bio: "Accés intern per revisar arquitectura, permisos i evolució del producte.",
            privacyAccepted: true,
            cancellationToken);

        await EnsureUserAsync(
            email: "user@user.com",
            password: "Admin123",
            role: "User",
            displayName: "Usuari de prova",
            city: "Madrid",
            country: "Espanya",
            bio: "Usuari local de desenvolupament per validar autenticació, sessió i perfil real.",
            privacyAccepted: true,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUserAsync(
        string email,
        string password,
        string role,
        string displayName,
        string city,
        string country,
        string bio,
        bool privacyAccepted,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var existing = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);

        if (existing is not null)
        {
            if (!existing.PasswordHash.StartsWith("pbkdf2$", StringComparison.OrdinalIgnoreCase))
            {
                existing.PasswordHash = passwordHasher.Hash(password);
            }

            return;
        }

        dbContext.Users.Add(new UserRecord
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            PasswordHash = passwordHasher.Hash(password),
            Role = role,
            DisplayName = displayName,
            City = city,
            Country = country,
            Bio = bio,
            AvatarUrl = null,
            PrivacyAccepted = privacyAccepted,
            PrivacyAcceptedAtUtc = privacyAccepted ? DateTimeOffset.UtcNow : null
        });
    }
}

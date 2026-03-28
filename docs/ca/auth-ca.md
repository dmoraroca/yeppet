# Autenticació (CA)

## Objectiu

Aquest document recull la primera base real d'autenticació de la Fase IV.

## Estat

- punt de fase: `autenticació pròpia i federada`
- estat: `(**EN CURS**)`

## Base actual implementada

- login propi real contra backend
- emissió de `JWT`
- persistència de sessió al navegador
- endpoint de sessió actual via `GET /api/auth/me`
- catàleg inicial de proveïdors via `GET /api/auth/providers`
- base preparada per federació futura amb `Google`, `LinkedIn`, `Facebook` i altres proveïdors `OAuth/OIDC`

## Usuaris de desenvolupament

- `admin@admin.adm / Admin123`
- `user@user.com / Admin123`

## Endpoints

- `POST /api/auth/login`
- `GET /api/auth/providers`
- `GET /api/auth/me`

## UML

<pre style="background:#020617; color:#e5eef7; border:1px solid #1e293b; border-radius:16px; padding:20px; margin:16px 0; overflow:auto; line-height:1.65;"><code><span style="color:#5eead4; font-weight:700;">flowchart LR</span>
  <span style="color:#93c5fd;">USER[Usuari]</span> --&gt; <span style="color:#c4b5fd;">WEB[LoginPage Angular]</span>
  <span style="color:#c4b5fd;">WEB</span> --&gt; <span style="color:#86efac;">AUTH[/api/auth/login]</span>
  <span style="color:#86efac;">AUTH</span> --&gt; <span style="color:#fcd34d;">APP[AuthApplicationService]</span>
  <span style="color:#fcd34d;">APP</span> --&gt; <span style="color:#f9a8d4;">HASH[Pbkdf2PasswordHasher]</span>
  <span style="color:#fcd34d;">APP</span> --&gt; <span style="color:#a7f3d0;">JWT[JwtAccessTokenIssuer]</span>
  <span style="color:#86efac;">AUTH</span> --&gt; <span style="color:#67e8f9;">ME[/api/auth/me]</span>
  <span style="color:#c4b5fd;">WEB</span> -.-> <span style="color:#fde68a;">FED[Google / LinkedIn / Facebook / OIDC]</span></code></pre>

## Validació feta

- `dotnet build YepPet.sln` correcte
- `npm run build` correcte
- `GET /api/auth/providers` correcte
- `POST /api/auth/login` correcte
- `GET /api/auth/me` correcte amb `Bearer token`

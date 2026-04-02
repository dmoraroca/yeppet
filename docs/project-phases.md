# YepPet · Fases del projecte

Aquest document defineix com s'organitza el desenvolupament de YepPet per fases, què s'espera de cada etapa i l'estat real de cadascuna.

## Objectiu general

YepPet ha de créixer com una plataforma pet-friendly per descobrir llocs, estades i serveis que accepten mascotes. El desenvolupament es farà de manera incremental:

- primer validant experiència i estructura amb frontend i dades fake
- després consolidant models i navegació funcional
- i finalment passant a backend real, permisos i internacionalització

## Principis de treball

- Frontend primer, backend després
- Dades fake mentre validem UX i estructura
- Arquitectura per `features`
- Components separats per responsabilitat
- Cada component dins la seva pròpia carpeta
- Reutilització real abans que duplicació
- Internacionalització al final, no al principi

## Criteris actius de treball

- Quan es consulti l'`estat`, la referència principal és aquest document
- Cada fase i cada punt rellevant s'han de marcar explícitament com a (**PENDENT**), (**EN CURS**) o (**FET**)
- Els punts marcats en negreta compten com a fets o consolidats mentre no estiguin normalitzats amb etiqueta explícita
- Els punts sense negreta compten com a pendents o oberts mentre no estiguin normalitzats amb etiqueta explícita
- Si no queden punts objectiu pendents dins d'una fase, la fase es considera acabada
- La Fase III s'ha de construir amb `DDD` com a base arquitectònica
- El disseny i la implementació han de seguir `SOLID` de manera estricta
- L'ordre de treball de la Fase III és: tancar el model de domini, després contractes i necessitats de persistència, després model relacional a `PostgreSQL`, `Entity Framework`, mapatges, migracions i connexió amb l'API
- Es prioritzaran patrons de disseny quan aportin mantenibilitat, claredat i facilitat d'evolució
- Si apareix una solució més moderna, més simple o tecnològicament millor, s'ha de plantejar abans d'implementar-la

## Fase I · Frontend base funcional amb dades simulades (**FET**)

### Objectiu

Construir una web Angular funcional, visualment coherent i preparada per créixer, però encara sense backend real.

### Què entra dins la fase I

- estructura base del projecte (**FET**)
- web Angular actual (**FET**)
- disseny i UX de la `home` (**FET**)
- navegació inicial (**FET**)
- dades fake (**FET**)
- components reutilitzables (**FET**)
- base de `features`, `shared` i `core` (**FET**)

### Estat actual de la fase I

La fase I queda completada amb aquesta base:

- projecte base a `yeppet`
- Angular 21 a `src/Web`
- `header` i `footer`
- `home` separada en seccions
- dades simulades per a la `home`
- feature `places`
- llistat de llocs amb filtres
- mapa funcional a `places`
- detall de lloc
- feature `favorites` amb estat fake
- navegació funcional des de la `home`
- `Trending cities` connectat a `places` per ciutat
- components separats en carpetes pròpies
- components compartits reutilitzables
- pàgina `permissions`
- primera base visual del producte

### Components compartits reutilitzables acordats

Per a la fase I consolidem aquests components com a reutilitzables reals:

- `app-section-heading`
- `app-generic-info-card`
- `app-favorite-toggle-button`
- `app-place-card`
- `app-place-map`

La resta de components es mantenen específics de cada `feature` mentre no tinguem una necessitat real de reutilització.

### Què queda validat en tancar la fase I

Queda validat:

- model TypeScript de `Place` (**FET**)
- `PlaceService` mock (**FET**)
- navegació entre `home`, `places`, `place detail`, `favorites` i `permissions` (**FET**)
- filtres i cerca sobre dades simulades (**FET**)
- component de mapa centralitzat i reutilitzat a `places` i `place detail` (**FET**)
- estat fake de favorits (**FET**)
- base responsive per `home`, `places`, `detail` i `favorites` (**FET**)
- coherència general de copies en català (**FET**)

### Criteri de finalització de la fase I

La fase I es considerarà acabada quan:

- es pugui navegar per `home`, `places`, `place detail` i `favorites` (**FET**)
- la navegació funcioni sense backend (**FET**)
- totes les dades surtin de mocks estructurats (**FET**)
- la UI sigui responsive (**FET**)
- la base de components sigui prou neta per créixer (**FET**)

## Fase II · Consolidació funcional i refinament (**FET**)

### Objectiu

Convertir la base funcional de la fase I en una aplicació frontend més completa i més refinada a nivell de producte.

### Què entra dins la fase II

- consolidar la feature `places` amb una UX de cerca més rica (**FET**)
- polir la vista mapa ja existent a `places` (**FET**)
- millorar la sincronització entre mapa, filtres i resultats (**FET**)
- decidir si `places` treballarà amb `llista`, `mapa` o mode mixt (**FET**)
- refinar la UX de marcadors, popups i selecció al mapa (**FET**)
- decidir com escalar la vista mapa quan hi hagi més densitat de dades (**FET**)
- refinar `favorites` perquè el flux de guardar i revisar llocs sigui més natural (**FET**)
- millorar el `place detail` amb millor jerarquia i més context (**FET**)
- revisar empty states, filtres actius i textos de suport (**FET**)
- polir les seccions de la `home` que ara són correctes però encara provisionals (**FET**)
- consolidar quins components compartits val la pena fixar definitivament (**FET**)
- enriquir les dades simulades perquè siguin més realistes (**FET**)
- preparar els serveis mock per substituir-los per API sense reescriure UI (**FET**)
- introduir una capa base de gestió d'errors (**FET**)
- afegir interceptor global per errors HTTP (**FET**)
- afegir servei central d'errors o notificacions (**FET**)
- definir una UI comuna per mostrar errors i missatges globals (**FET**)
- reduir `try/catch` repetits als punts on el problema sigui transversal (**FET**)
- revisar responsive fi de totes les pantalles (**FET**)
- definir millor `Ajuda`, `Contacta'ns` i les pàgines informatives (**FET**)
- afinar la navegació general perquè cada CTA tingui una funció clara (**FET**)

### Entregables de la fase II

La fase II s'hauria de poder resumir en aquests blocs:

1. `Places` amb mapa sota filtres (**FET**)
2. `Place detail` més complet i més clar (**FET**)
3. Base d'autenticació i perfil (**FET**)
4. `Home` més madura visualment (**FET**)
5. Components compartits consolidats (**FET**)
6. Mocks més rics i preparats per API (**FET**)
7. Capa base de gestió d'errors (**FET**)
8. Responsive i UX refinats (**FET**)

### Estat parcial de la fase II

Ara mateix, dins de la fase II, ja tenim avançat:

- mapa funcional integrat a `places` sota els filtres (**FET**)
- component `app-place-map` centralitzat i parametritzable (**FET**)
- coordenades simulades precises als `Place` (**FET**)
- reutilització del mateix mapa a `place detail` (**FET**)
- millora del context de filtres actius i títols dinàmics a `places` (**FET**)
- decisió de producte fixada: `places` treballa en mode mixt amb mapa sota filtres i llistat sincronitzat com a base principal de comparació (**FET**)
- poliment real de la UX del mapa a `places` (**FET**)
- decisió d'escalat del mapa fixada per aquesta fase: mostrar tots els marcadors sense clustering mentre el volum segueixi sent assumible (**FET**)
- selecció de marcador amb resum contextual del lloc (**FET**)
- accions de mapa per veure tots els resultats o treure la selecció (**FET**)
- highlight visual del `place-card` associat al marcador seleccionat (**FET**)
- header corregit perquè el mapa no el sobreposi en scroll (**FET**)
- preview del `hero` més orientada a contingut destacat i no només a ciutats (**FET**)
- `home` polida amb millor direcció de producte: hero menys provisional, bloc de recorregut funcional, ciutats amb CTA més clar i tancament orientat a `places` i `favorites` (**FET**)
- catàleg de compartits fixat definitivament: `app-section-heading`, `app-generic-info-card`, `app-favorite-toggle-button`, `app-place-card`, `app-place-map` i `app-error-notifications` (**FET**)
- mocks de `places` enriquits amb barri, volum de ressenyes, preu orientatiu i política pet per fer més creïbles el llistat i el detall (**FET**)
- serveis mock desacoblats de la font de dades amb ports i tokens injectables perquè més endavant es puguin substituir per API sense tocar la UI (**FET**)
- login fake amb email i password (**FET**)
- rols `USER` i `ADMIN` ja operatius (**FET**)
- sessió fake mantinguda amb redirecció automàtica a `login` (**FET**)
- guards de `auth`, `guest` i `admin` ja aplicats a les rutes (**FET**)
- pàgina de `Perfil` fake amb manteniment bàsic de dades (**FET**)
- consentiment LGPD/GDPR obligatori per `USER` al manteniment de perfil (**FET**)
- visibilitat i accés de `Del desenvolupador` només per `ADMIN` (**FET**)
- revisió de `favorites` més natural amb resum, guardat recent, filtres locals i ordenació per reprendre la cerca sense tornar a començar (**FET**)
- interceptor HTTP global per capturar errors de backend (**FET**)
- servei centralitzat de notificacions d'error (**FET**)
- UI global de notificacions per mostrar errors sense repetir lògica a les pàgines (**FET**)
- ajust responsive fi aplicat a `header`, `footer`, `login`, `profile`, `contact`, `places`, `place detail`, `favorites`, `place-card` i `place-filters` per millorar pantalles estretes (**FET**)
- `Ajuda` consolidada com a pàgina pròpia per explicar el flux real de producte i `Contacta'ns` definida amb canals més clars per suport i col·laboracions (**FET**)
- CTA afinats perquè cada acció principal tingui una intenció clara: descobrir a `places`, entendre el flux a `ajuda`, reprendre a `favorites` i mantenir navegació base també des del `footer` (**FET**)

La Fase II queda donada per acabada segons aquest criteri d'etiquetes explícites.

### Resultat esperat

Una aplicació frontend sòlida que simula millor el comportament real del producte i està llesta per començar a parlar amb backend.

## Fase III · Backend real i persistència (**FET**)

### Objectiu

Passar de frontend mock-first a un sistema real amb backend i base de dades.

### Què entra dins la fase III

- disseny del model de domini real (**FET**)
- contractes de repositori i necessitats de persistència (**FET**)
- model relacional a `PostgreSQL` (**FET**)
- persistència amb `Entity Framework` última versió (**FET**)
- configuració de mapatge, migracions i repositoris (**FET**)
- backend `.NET` (**FET**)
- API per `places`, `favorites`, `users`, `reviews` (**FET**)
- substitució progressiva de serveis mock per serveis reals (**FET**)

### Resultat esperat

YepPet deixa de ser una simulació i passa a tenir dades persistides i fluxos reals (**FET**).

### Estat actual del punt tancat

El punt `model relacional a PostgreSQL` queda tancat amb:

- model relacional consolidat a `database-model-ca.md`
- diagrama relacional ampliat per `users`, `places`, `favorite_lists`, `favorite_entries`, `place_reviews`, `tags`, `features`, `place_tags`, `place_features` i `privacy_consent_events`
- decisio presa: `tags` i `features` es normalitzen en taules propies i taules d'unio
- decisio presa: `rating_average` i `review_count` es mantenen a `places` com a snapshot optimitzat, derivat de `place_reviews`
- decisio presa: el consentiment es manté en estat actual a `users` i amb historial a `privacy_consent_events`
- base de dades de desenvolupament operativa amb `Docker` i `PostgreSQL`
- coexistencia resolta amb altres projectes locals fent servir el port extern `5433` per `YepPet`
- `docker-compose.yml` al repo per poder aixecar la BBDD local sense dependències manuals

### Estat actual del punt tancat

El punt `persistència amb Entity Framework` queda tancat amb:

- base de dades local `yeppet` aixecada en `Docker` a `localhost:5433`
- `dotnet-ef` 10 configurat localment al repo via `dotnet-tools.json`
- projecte `Infrastructure` connectat a `Entity Framework Core`
- `YepPetDbContext` creat com a peça central de persistència
- models de persistència separats del domini creats a `Infrastructure/Persistence/Entities`
- configuracions EF creades a `Infrastructure/Persistence/Configurations`
- `Api` preparada per registrar el `DbContext` i apuntar a PostgreSQL local
- migració inicial `InitialCreate` generada a `Infrastructure/Persistence/Migrations`
- base recreada i esquema aplicat des d'`Entity Framework` amb `database update`
- historial de migracions validat amb la taula `__EFMigrationsHistory`
- `sql/init` reduit a bootstrap mínim perquè l'esquema el governi EF i no SQL manual

### Estat actual del punt tancat

El punt `configuració de mapatge, migracions i repositoris` queda tancat amb:

- mappers manuals creats per `Place`, `User`, `FavoriteList` i `PlaceReview`
- conversions explícites entre agregats de domini i records de persistència
- repositoris EF creats per `IPlaceRepository`, `IUserRepository`, `IFavoriteListRepository` i `IPlaceReviewRepository`
- registre dels repositoris a `DependencyInjection`
- compilació del backend validada amb `dotnet build YepPet.sln`

### Estat actual del punt tancat

El punt `backend .NET` queda tancat amb:

- projecte `Application` deixat operatiu
- capa d'aplicacio amb DTOs i serveis per `places`, `favorites`, `users` i `reviews`
- registre d'`Application` a la DI del backend
- integracio de `Application` + `Infrastructure` + `Api` validada en compilacio
- `dotnet build YepPet.sln` correcte amb les quatre capes del backend

### Estat actual del punt tancat

El punt `API per places, favorites, users i reviews` queda tancat amb:

- `Api` exposant rutes HTTP reals via `minimal APIs`
- documentacio navegable de l'API disponible via `Swagger`
- grups de rutes separats per `places`, `favorites`, `users` i `reviews`
- ús exclusiu de serveis d'`Application` des de la capa `Api`
- sense dependència directa de `DbContext` ni de `Infrastructure` dins dels endpoints
- validació end-to-end real contra `PostgreSQL` local a `localhost:5433`
- flux provat de punta a punta: alta de `user`, alta de `place`, consulta de `place`, alta de favorit i alta de review
- lectura real dels recursos guardats via HTTP sobre dades persistides

### Estat actual del punt tancat

El punt `substitució progressiva de serveis mock per serveis reals` queda tancat amb:

- `Web` consumint HTTP real contra `Api` per `places`
- `favorites` persistits i sincronitzats contra backend real
- `perfil` guardant contra l'endpoint de `users`
- login mantenint porta d'entrada fake però sincronitzant usuaris contra backend per obtenir identitat real
- `ng build` correcte un cop connectat el frontend a la nova API
- convivència controlada entre UX existent i dades reals sense reescriure la navegació
- stack local complet de desenvolupament disponible amb `Docker Compose` per `db`, `api` i `web`
- perfils de `Run and Debug` de `VS Code` disponibles per aixecar la stack completa o cada servei per separat sobre `Docker`

## Fase IV · Permisos, administració i operativa (**EN CURS**)

### Objectiu

Separar clarament les zones públiques de les zones internes o controlades per permisos.

### Què entra dins la fase IV

- autenticació pròpia i federada (`Google`, `LinkedIn`, `Facebook` i altres proveïdors OAuth/OIDC) (**EN CURS**)
- rols i permisos (**PENDENT**)
- pàgines internes (**PENDENT**)
- gestió de contingut o dades (**PENDENT**)
- accessos restringits a determinades funcionalitats (**PENDENT**)

### Resultat esperat

La plataforma ja diferencia entre usuaris públics, usuaris autenticats i àrees internes (**PENDENT**).

### Estat actual del punt en curs

El punt `autenticació pròpia i federada` queda ara mateix en curs amb aquesta base ja operativa:

- `Api` exposa `POST /api/auth/login`, `GET /api/auth/providers` i `GET /api/auth/me`
- `Api` exposa també `POST /api/auth/google` per tancar el primer proveïdor federat real
- el login propi ja valida credencials reals contra backend
- la sessió es representa amb `JWT`
- el frontend ja desa i reutilitza el token per a les crides HTTP
- `Google` ja queda configurat en entorn de desenvolupament amb `ClientId` local i botó federat visible a `login`
- si un usuari entra per Google i no existeix a BBDD, el backend el crea automàticament amb email, nom visible i avatar
- si un usuari ja existeix i el perfil es pot sincronitzar, el backend actualitza nom i avatar des de Google
- la credencial descarregada de Google queda guardada com a suport local a `config/google/yeppet-dev.json`, fora de versionat
- `yeppetcontact@gmail.com` queda definit com a compte administrador quan entra per Google en desenvolupament
- la stack Docker de l'`Api` ja arrenca des de `src/Backend/Api`, carregant correctament `appsettings.Development.json`
- existeixen usuaris de desenvolupament bootstrap:
  - `admin@admin.adm / Admin123`
  - `user@user.com / Admin123`
- el focus immediat del punt en curs passa a ser `LinkedIn`
- `LinkedIn` continua pendent dins del mateix punt d'autenticació federada
- `Facebook` queda aparcat expressament fins després de publicar la web
- el patró federat ja queda validat amb `Google`

## Fase V · Internacionalització (**PENDENT**)

### Objectiu

Fer el producte multiidioma de manera seriosa, un cop el contingut i l'estructura siguin estables.

### Què entra dins la fase V

- estratègia d'i18n (**PENDENT**)
- idiomes d'Europa (**PENDENT**)
- àrab (**PENDENT**)
- xinès (**PENDENT**)
- suport RTL (**PENDENT**)
- revisió de longituds de text (**PENDENT**)
- SEO per idioma (**PENDENT**)

### Resultat esperat

YepPet pot operar en diversos idiomes sense haver d'improvisar textos dispersos dins components (**PENDENT**).

## Fase VI · Poliment i desplegament (**PENDENT**)

### Objectiu

Preparar el producte per sortir a un entorn real.

### Què entra dins la fase VI

- optimització visual final (**PENDENT**)
- revisió de responsive complet (**PENDENT**)
- revisió de rendiment (**PENDENT**)
- QA (**PENDENT**)
- desplegament (**PENDENT**)
- observabilitat mínima (**PENDENT**)

## Decisió actual

Ara mateix el focus correcte és aquest:

1. donar per tancada la fase I
2. entrar a fase II començant per `places`
3. consolidar `auth/profile`, polir el mapa ja integrat i afegir capa base d'errors
4. no entrar encara ni en backend real ni en multiidioma complet

## Estat actual

La fase I ja queda tancada perquè la web permet:

- navegar per `home`, `places`, `place detail`, `favorites` i `permissions`
- treballar amb dades simulades però estructurades
- reutilitzar components reals i no només markup repetit
- validar fluxos de filtrat, detall i favorits sense backend
- tenir una primera base de mapa funcional amb component centralitzat

El següent tram de treball ja correspon a la fase II.

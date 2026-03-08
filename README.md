![](./shop-net-logo.png)

## For developper

### Une fois les container Up et running

1. Installation des packages nécessaires :

- Entrez dans votre container API : `just shell api`.
- Installez les outils EF Core :

```shell
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL (pour Postgres)
dotnet add package Pomelo.EntityFrameworkCore.MySql (pour MySQL)
```
```shell
# Ajoute le support de base pour Entity Framework
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.3
dotnet add package Microsoft.EntityFrameworkCore.Relational --version 10.0.3
```
```shell
# Ajoute le support pour PostgreSQL (votre base de données)
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 10.0.0
```
```shell
# Ajoute le support spécifique pour Identity avec EF Core
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 10.0.3
```

2. Créer les migrations

```shell
dotnet ef migrations add InitialIdentity --context MainDbContext --output-dir Data/Migrations
```
```shell
dotnet ef database update --context MainDbContext
```
```shell
Build started...
Build succeeded.
fail: Microsoft.EntityFrameworkCore.Database.Command[20102]
      Failed executing DbCommand (17ms) [Parameters=[], CommandType='Text', CommandTimeout='30']    <--- Ceci est normal >
      SELECT "MigrationId", "ProductVersion"
      FROM "__EFMigrationsHistory"
      ORDER BY "MigrationId";
Failed executing DbCommand (17ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
SELECT "MigrationId", "ProductVersion"
FROM "__EFMigrationsHistory"
ORDER BY "MigrationId";
info: Microsoft.EntityFrameworkCore.Migrations[20411]
      Acquiring an exclusive lock for migration application. See https://aka.ms/efcore-docs-migrations-lock for more information if this takes too long.
Acquiring an exclusive lock for migration application. See https://aka.ms/efcore-docs-migrations-lock for more information if this takes too long.
info: Microsoft.EntityFrameworkCore.Migrations[20402]
      Applying migration '20260308142857_InitialIdentity'.
Applying migration '20260308142857_InitialIdentity'.
Done.
```

L'erreur que vous voyez au début (Failed executing DbCommand... "__EFMigrationsHistory") est tout à fait normale : c'était la toute première fois que votre API tentait d'interroger la table d'historique des migrations. Comme elle n'existait pas encore, le "Failed" n'est pas une erreur critique, mais simplement le comportement par défaut d'EF Core qui constate que la table est absente et procède donc à sa création.

| Ce qui vient de se passer :

- Verrouillage (Locking) : EF Core a sécurisé la base pour éviter que deux instances ne tentent d'écrire des changements en même temps.
- Application du schéma : Il a exécuté les commandes SQL générées dans votre fichier de migration (20260308142857_InitialIdentity).
- Synchronisation : Votre base PostgreSQL est maintenant prête, avec toutes les tables users, roles, user_roles, etc., prêtes à recevoir vos données.

3. Création de roles et users

``shell
# Commande pour initialiser les rôles
dotnet run -- --ensure-roles roles=Administrator,Sale,Customer,IT,Logistic,Communication,TechnicalAdvice,Billing,SAV,UsedAndRefurbish,Inventory
```
```shell
# Commande pour créer le super-admin
dotnet run -- --create-user username=admin email=admin@shop.net password=@dm1nSh0pN3t# roles=Administrator,IT
```

4. Renommage de tables 

Si vous modifiez les noms des tables maintenant, vous devrez :

- Modifier MainDbContext.cs.
- Lancer : `dotnet ef migrations add RenameIdentityTables --context MainDbContext --output-dir Data/Migrations`.
- Lancer : `dotnet ef database update --context MainDbContext`.

### Les petits pièges à éviter

1. Pour que dotnet ef fonctionne, il lui faut le package Microsoft.EntityFrameworkCore.Design dans ton projet .csproj. Vérifie si tu l'as :

```shell
just exec api dotnet add package Microsoft.EntityFrameworkCore.Design
just exec api dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
just exec api dotnet add package Pomelo.EntityFrameworkCore.MySql
```
```shell
docker exec -it shopnet_api curl -i -X POST http://localhost:5000/api/auth/login \
    -H "Content-Type: application/json" \
    -H "Accept: application/json" \
    -d '{"email":"admin@mkdevs.eu","password":"Admin123!"}'
```
```shell
# Create user
docker exec -it shopnet_api dotnet ~/bin/Debug/net10.0/ShopNetApi.dll --create-user email=admin@mkdevs.eu password=Admin123! username=admin firstname=Master lastname=Admin role=Admin

# Delete user
docker exec -it shopnet_api dotnet ~/bin/Debug/net10.0/ShopNetApi.dll --delete-user username=admin --urls=http://localhost:5001
```

```shell
docker exec -it shopnet_postgres psql -U shopnet -d shopnet -c "SELECT \"Email\", \"PasswordHash\", \"EmailConfirmed\" FROM \"Users\";"
docker exec -it shopnet_postgres psql -U shopnet -d shopnet -c "SELECT \"PasswordHash\" FROM \"Users\" WHERE \"Email\" = 'admin@mkdevs.eu';"

docker exec -it shopnet_postgres psql -U shopnet -d shopnet -c "\dt"
docker exec -it shopnet_postgres psql -U shopnet -d shopnet -c "TRUNCATE TABLE \"Roles\" CASCADE;"
docker exec -it shopnet_postgres psql -U shopnet -d shopnet -c "TRUNCATE TABLE \"UserRoles\" CASCADE;"

# Crée la structure initiale (Users, Roles, Brands)
docker exec -it shopnet_api ~/.dotnet/tools/dotnet-ef migrations add InitialCreate

# Applique la structure à la base Postgres dans Docker
docker exec -it shopnet_api ~/.dotnet/tools/dotnet-ef database update
```
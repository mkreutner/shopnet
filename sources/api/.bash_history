dotnet tool install --global dotnet-ef
dotnet ef
.dotnet/tools/dotnet-ef 
touch .bashrc
vi .bashrc 
vi .bashrc 
dotnet ef
dotnet-ef
vi .bashrc 
dotnet-ef
dotnet ef
vi .bashrc 
cat .bashrc 
dotnet ef --help
cat .bashrc 
vi .bashrc 
dotnet ef --help
source .bashrc 
dotnet ef --help
export PATH="$PATH:$USER/.dotnet/tools/"
dotnet ef --help
vi .bashrc 
curl http://localhost:5000/api/system/roles
dotnet ef migrations add AddBrandsTable
dotnet ef database update
dotnet ef migrations add AddCategoriesTable
dotnet ef database update
dotnet ef migrations add AddSupplierAndWarehouse
dotnet ef migrations remove
dotnet ef migrations add AddSupplierAndWarehouse
dotnet ef database update
dotnet ef migrations add InitPartnersAndInventory
dotnet ef database update
dotnet ef migrations add UpdateSuppliers
dotnet ef database update
dotnet ef migrations add RefactorConfiguration
dotnet ef database update
dotnet ef database update
dotnet ef migrations add InitSchemaRefactor
dotnet ef database update
dotnet ef database update
dotnet ef migrations remove
dotnet ef database update
dotnet ef migrations add InitSchemaRefactor
dotnet ef database update
dotnet ef database update
dotnet ef migrations add InitSchemaRefactor
dotnet ef migrations add InitSchemaRefactor
rm -rf Migrations/
dotnet ef migrations add InitSchemaRefactor
dotnet ef migrations remove
rm -rf Migrations/
dotnet ef migrations add InitSchemaRefactor
dotnet ef migrations remove
dotnet ef migrations add InitialSchema --output-dir Data/Migrations --context MainDbContext
dotnet ef database update
dotnet run -- --ensure-roles roles=Administrator,Sale,Customer,IT,Logistic,Communication,TechnicalAdvice,Billing,SAV,UsedAndRefurbish,Inventory,User
```shell
# Commande pour créer le super-admin
dotnet run -- --create-user username=admin email=admin@shop.net password=@dm1nSh0pN3t# roles=Administrator,IT

```shell
# Commande pour créer le super-admin
dotnet run -- --create-user username=admin email=admin@shop.net password=@dm1nSh0pN3t# roles=Administrator,IT
dotnet run -- --create-user username=admin email=admin@shop.net password=@dm1nSh0pN3t# roles=Administrator,IT
dotnet ef migrations add AddProductsAndStocks -o Data/Migrations
dotnet ef database update
dotnet ef migrations add InitialSnakeCaseSchema -o Data/Migrations
dotnet ef database update
dotnet ef migrations add InitialSchemaSnakeCase -o Data/Migrations
dotnet ef database update
dotnet ef migrations add InitialSchemaSnakeCase -o Data/Migrations
dotnet ef database update
dotnet run -- --create-user username=admin email=admin@shop.net password=@dm1nSh0pN3t# roles=Administrator,IT
dotnet ef migrations add AddInventorySystem -o Data/Migrations
dotnet ef database update
dotnet ef migrations add InitialSchema -o Data/Migrations
dotnet ef database update
dotnet run -- --create-user username=admin email=admin@shop.net password=@dm1nSh0pN3t# roles=Administrator,IT
dotnet ef migrations add InitialSchema -o Data/Migrations
dotnet ef database update
dotnet ef migrations add InitialSchema -o Data/Migrations
dotnet ef migrations add InitialSchema -o Data/Migrations
dotnet ef database update
dotnet run -- --create-user username=admin email=admin@shop.net password=@dm1nSh0pN3t# roles=Administrator,IT
dotnet ef migrations add FixWarehouseAddressOptional
dotnet ef database
dotnet ef database update
dotnet ef migrations add FinalSync
dotnet ef database update
dotnet ef migrations add UseEnumForMovement
dotnet ef migrations remove
dotnet ef migrations add FixMovementTypeToEnum
dotnet ef database update
dotnet ef migrations add AddUserProfilesAndEmployeeEntities
dotnet ef database update
dotnet ef migrations add AddSoftDeleteSupport
dotnet ef database update
dotnet ef migrations remove
dotnet ef migrations add AddSoftDeleteSupport
dotnet ef database update

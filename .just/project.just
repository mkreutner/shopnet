# +---------------------------------------------------------------------------
# : Project specific recipes
# +---------------------------------------------------------------------------
# Build or rebuild services
EF_COMMAND := "PATH=\"$PATH:$HOME/.dotnet/tools\" dotnet-ef"

build *OPTIONS:
    @docker compose build {{OPTIONS}}

# Create and start containers (detached)
up *OPTIONS:
    @docker compose up -d {{OPTIONS}}

# Stop and remove containers, networks
down *CONTAINER:
    @docker compose down {{CONTAINER}}

# Stop and remove containers, networks, remove orphans and volumes
down-pp *CONTAINER:
    @docker compose down --remove-orphans --volumes {{CONTAINER}}

# Stop and remove containers, networks, remove orphans and volumes
restart *CONTAINER:
    @docker compose restart {{CONTAINER}} 

# List containers
ps *OPTIONS:
    @docker compose ps {{OPTIONS}}

# Restart service containers
#restart:
#    @docker compose restart

# Watch build context for service and rebuild/refresh containers when files are updated
watch:
    @docker compose watch

# Execute a COMMAND in CONTAINER container
exec CONTAINER *COMMAND:
    @docker compose exec {{CONTAINER}} {{COMMAND}}

# Launch SHELL in CONTAINER
shell CONTAINER SHELL="bash": 
    @docker compose exec {{CONTAINER}} {{SHELL}}

# Display tdout logs in CONTAINER 
logs CONTAINER:
    @docker compose logs -f -n 40 {{CONTAINER}}

# Clean Bash History file
history-clean CONTAINER:
    @echo -n > ./src/{{CONTAINER}}/.bash_history

# Créer une migration (ex: just api-migrate Initial)
api-migrate MIGRATION_NAME:
    just exec api bash -c "{{EF_COMMAND}} migrations add {{MIGRATION_NAME}} --project ShopNet.Api"

# Exemple de recette mise à jour
api-db-update:
    just exec api bash -c "{{EF_COMMAND}} database update"

# Commande pour voir si tout est au vert (ton dashboard perso)
status:
    @docker compose ps --format 'table {{{{.Names}}\t{{{{.Status}}\t{{{{.Ports}}'
    @echo "---"
    @curl -s http://localhost:4040/api/tunnels | jq -r '"Ngrok URL: " + .tunnels[0].public_url'

[private]
@help:
    echo "🛟  Project Help 🛟"
    just --list --list-heading '' --list-prefix "    ↪ " --justfile {{justfile()}}

# Setting up the project
[private]
@setup:
    echo ">>> Project Setup <<<"
    ./bin/00_create_envfile.sh
    ./bin/01_create_tmp_folders.sh
    ./bin/02_set_identities.sh


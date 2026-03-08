#!/bin/bash
set -e

# 1. Détection du projet
# On cherche s'il y a un fichier .csproj dans le dossier courant
if [ -z "$(ls *.csproj 2>/dev/null)" ]; then
    echo "🚀 Aucun projet .NET détecté. Initialisation de l'api..."
    
    # On crée le projet directement dans le dossier courant (.)
    # --name définit le nom du namespace et du fichier csproj
    # --no-https car on est derrière un reverse proxy ou en dev local simple
    dotnet new webapi --name ShopNetApi --use-controllers --use-minimal-apis --no-https --output .
    
    echo "✅ Projet initialisé avec succès."
else
    echo "Context: Projet .NET existant trouvé."
fi

# 2. Exécution de la commande passée au container (dotnet watch, etc.)
exec "$@"
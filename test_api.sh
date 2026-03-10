#!/bin/bash

# Configuration
API_URL="https://mkdevs.eu.ngrok.io/api"
USERNAME="admin"
PASSWORD="@dm1nSh0pN3t#"

echo "--- 0. Nettoyage des tables métier ---"
docker compose exec -t postgres bash -c \
     'PGPASSWORD="sh0pn€7!" psql -U shopnet -d shopnet -c "TRUNCATE TABLE \"Categories\", \"Brands\" RESTART IDENTITY CASCADE;"'

echo "--- 1. Connexion en cours... ---"
# On récupère le JSON de login, on extrait la valeur après "token":" et on nettoie les guillemets
TOKEN=$(curl -s -X POST "$API_URL/auth/login" \
     -H "Content-Type: application/json" \
     -d "{\"username\":\"$USERNAME\", \"password\":\"$PASSWORD\"}" \
     | grep -o '"token":"[^"]*' | grep -o '[^"]*$')

if [ -z "$TOKEN" ]; then
    echo "❌ Erreur : Impossible de récupérer le token. Vérifie tes identifiants."
    exit 1
fi

echo "✅ Token récupéré avec succès."
echo ""

echo "--- 2. Création d'une marque (POST /Brands) ---"
curl -i -X POST "$API_URL/Brands" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d '{
       "name": "Framework Lab",
       "description": "Premium development tools and hardware."
     }'

echo ""
echo "--- 3. Liste des marques (GET /Brands) ---"
curl -s "$API_URL/Brands" | python3 -m json.tool || curl -s "$API_URL/Brands"

# Avant l'étape 4, assure-toi d'avoir installé jq : sudo apt install jq

echo "--- 4. Création de la catégorie Électronique ---"
RESPONSE=$(curl -s -X POST "$API_URL/Categories" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d '{"name": "Electronics", "description": "High tech stuff"}')

# Utilise jq pour extraire l'ID proprement
PARENT_ID=$(echo "$RESPONSE" | jq -r '.id // empty')

if [ -z "$PARENT_ID" ]; then
    echo "❌ Erreur critique : Impossible d'extraire l'ID du parent."
    echo "Réponse de l'API : $RESPONSE"
    exit 1
fi

echo "Parent ID : $PARENT_ID"

echo "--- 5. Création de l'enfant Smartphones ---"
# Utilise un format plus propre avec une variable JSON pour éviter les erreurs d'échappement
JSON_PAYLOAD=$(printf '{"name": "Smartphones", "description": "Mobile devices", "parentCategoryId": "%s"}' "$PARENT_ID")

curl -i -X POST "$API_URL/Categories" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d "$JSON_PAYLOAD"
#!/bin/bash

# Configuration
API_URL="https://mkdevs.eu.ngrok.io/api"
USERNAME="admin"
PASSWORD="@dm1nSh0pN3t#"

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 0. Nettoyage complet des tables métier"
echo "+----------------------------------------------------------------------------------------"
# Ajout des tables suppliers, contacts, etc. au nettoyage
docker compose exec -t postgres bash -c \
     'PGPASSWORD="sh0pn€7!" psql -U shopnet -d shopnet -c \
     "TRUNCATE TABLE \"Categories\", \"Brands\", \"Addresses\", \"Contacts\", \"suppliers\" RESTART IDENTITY CASCADE;"'

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 1. Connexion en cours..."
echo "+----------------------------------------------------------------------------------------"
TOKEN=$(curl -s -X POST "$API_URL/auth/login" \
     -H "Content-Type: application/json" \
     -d "{\"username\":\"$USERNAME\", \"password\":\"$PASSWORD\"}" \
     | grep -o '"token":"[^"]*' | grep -o '[^"]*$')

if [ -z "$TOKEN" ]; then
    echo "❌ Erreur : Impossible de récupérer le token."
    exit 1
fi
echo "✅ Token récupéré."

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 2. Création d'une marque (POST /Brands)"
echo "+----------------------------------------------------------------------------------------"
curl -i -X POST "$API_URL/Brands" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d '{
       "name": "Framework Lab",
       "description": "Premium development tools and hardware."
     }'

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 3. Liste des marques (GET /Brands)"
echo "+----------------------------------------------------------------------------------------"
curl -s "$API_URL/Brands" | python3 -m json.tool || curl -s "$API_URL/Brands"

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 4. Création de la catégorie Électronique"
echo "+----------------------------------------------------------------------------------------"
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

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 5. Création de l'enfant Smartphones"
echo "+----------------------------------------------------------------------------------------"
# Utilise un format plus propre avec une variable JSON pour éviter les erreurs d'échappement
JSON_PAYLOAD=$(printf '{"name": "Smartphones", "description": "Mobile devices", "parentCategoryId": "%s"}' "$PARENT_ID")

curl -i -X POST "$API_URL/Categories" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d "$JSON_PAYLOAD"

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 6. Création d'un Fournisseur avec relations (POST /Suppliers)"
echo "+----------------------------------------------------------------------------------------"
# On envoie tout en une fois : le Supplier, ses Contacts et ses Adresses
# EF Core va gérer l'insertion en cascade grâce à nos configurations
curl -i -X POST "$API_URL/Suppliers" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d '{
       "name": "Global Tech Logistics",
       "vatNumber": "FR9988776655",
       "contacts": [
         {
           "firstName": "Alice",
           "lastName": "Durand",
           "email": "alice.d@globaltech.com",
           "phoneNumber": "+33123456789",
           "role": "Account Manager"
         }
       ],
       "addresses": [
         {
           "street": "50 Boulevard de la Technologie",
           "city": "Lyon",
           "zipCode": "69000",
           "country": "France"
         }
       ]
     }'

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 7. Vérification : Liste des fournisseurs (GET /Suppliers)"
echo "+----------------------------------------------------------------------------------------"
SUPPLIER_ID=$(curl -s -H "Authorization: Bearer $TOKEN" "$API_URL/Suppliers" | jq -r '.[0].id')

if [ -z "$SUPPLIER_ID" ]; then
    echo "❌ Erreur : Impossible de récupérer un ID de fournisseur."
    exit 1
fi
echo "✅ ID Fournisseur récupéré : $SUPPLIER_ID"

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 8. Test Update Fournisseur (PUT /Suppliers/{id})"
echo "+----------------------------------------------------------------------------------------"
# Remplace $SUPPLIER_ID par l'ID que tu auras récupéré
curl -i -X PUT "$API_URL/Suppliers/$SUPPLIER_ID" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d '{"id": "'$SUPPLIER_ID'", "name": "Global Tech Updated", "vatNumber": "FR9988776655"}'

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 9. Test Delete Fournisseur (DELETE /Suppliers/{id})"
echo "+----------------------------------------------------------------------------------------"
curl -i -X DELETE "$API_URL/Suppliers/$SUPPLIER_ID" \
     -H "Authorization: Bearer $TOKEN"



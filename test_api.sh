#!/bin/bash

# Configuration
API_URL="https://mkdevs.eu.ngrok.io/api"
USERNAME="admin"
PASSWORD="@dm1nSh0pN3t#"

echo "+----------------------------------------------------------------------------------------"
echo ":--- 0. Nettoyage complet des tables métier (en snake_case)"
echo "+----------------------------------------------------------------------------------------"
docker compose exec -t postgres bash -c \
     'PGPASSWORD="sh0pn€7!" psql -U shopnet -d shopnet -c \
     "TRUNCATE TABLE 
        categories, 
        brands, 
        addresses, 
        contacts, 
        suppliers, 
        products, 
        warehouses, 
        stock_movements, 
        product_stocks 
      RESTART IDENTITY CASCADE;"'

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
# curl -i -X DELETE "$API_URL/Suppliers/$SUPPLIER_ID" \
#      -H "Authorization: Bearer $TOKEN"

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 10. Création Produit (POST /Products)"
echo "+----------------------------------------------------------------------------------------"
BRAND_ID=$(curl -s -H "Authorization: Bearer $TOKEN" "$API_URL/Brands" | jq -r '.[0].id')
CAT_ID=$(curl -s -H "Authorization: Bearer $TOKEN" "$API_URL/Categories" | jq -r '.[0].id')
SUPP_ID=$(curl -s -H "Authorization: Bearer $TOKEN" "$API_URL/Suppliers" | jq -r '.[0].id')

PRODUCT_RESPONSE=$(curl -s -X POST "$API_URL/Products" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d "{
       \"name\": \"Test Product\",
       \"sku\": \"TEST-001\",
       \"purchasePrice\": 100.0,
       \"brandId\": \"$BRAND_ID\",
       \"categoryId\": \"$CAT_ID\",
       \"supplierId\": \"$SUPP_ID\"
     }")

PRODUCT_ID=$(echo "$PRODUCT_RESPONSE" | jq -r '.id // empty')
echo "✅ Produit créé : $PRODUCT_ID"

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 11. Création Entrepôt (POST /Warehouses)"
echo "+----------------------------------------------------------------------------------------"
WAREHOUSE_RESPONSE=$(curl -s -X POST "$API_URL/Warehouses" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d '{"name": "Dépôt Metz", "type": 0}')

WAREHOUSE_ID=$(echo "$WAREHOUSE_RESPONSE" | jq -r '.id // empty')
echo "✅ Entrepôt créé : $WAREHOUSE_ID"

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 12. Mouvement de Stock (POST /StockMovements)"
echo "+----------------------------------------------------------------------------------------"
curl -i -X POST "$API_URL/StockMovements" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d "{
       \"productId\": \"$PRODUCT_ID\",
       \"warehouseId\": \"$WAREHOUSE_ID\",
       \"quantityChange\": 50,
       \"movementType\": \"IN\",
       \"reason\": \"Arrivage initial\"
     }"

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 13. Vérification Stock Final (GET /InventoryStatus/{id})"
echo "+----------------------------------------------------------------------------------------"
curl -s -H "Authorization: Bearer $TOKEN" "$API_URL/InventoryStatus/$PRODUCT_ID"
echo ""

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 14. Sortie de Stock (POST /StockMovements - OUT)"
echo "+----------------------------------------------------------------------------------------"
# On sort 20 unités
curl -i -X POST "$API_URL/StockMovements" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d "{
       \"productId\": \"$PRODUCT_ID\",
       \"warehouseId\": \"$WAREHOUSE_ID\",
       \"quantityChange\": -20,
       \"movement\": \"OUT\",
       \"reason\": \"Vente client #001\"
     }"

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 15. Vérification Stock Final après Sortie (GET /InventoryStatus/{id})"
echo "+----------------------------------------------------------------------------------------"
# Le résultat devrait être 50 - 20 = 30
curl -s -H "Authorization: Bearer $TOKEN" "$API_URL/InventoryStatus/$PRODUCT_ID"
echo ""
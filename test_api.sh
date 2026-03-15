#!/bin/bash

# Configuration
API_URL="https://mkdevs.eu.ngrok.io/api"
USERNAME="admin"
PASSWORD="@dm1nSh0pN3t#"

echo "+----------------------------------------------------------------------------------------"
echo ":--- 0. Reset complet, rôles et Admin"
echo "+----------------------------------------------------------------------------------------"
# Nettoyage total et préparation du système
docker compose exec -t api sh -c "dotnet run -- --reset-db" 
docker compose exec -t api sh -c "dotnet run -- --ensure-roles roles=Administrator,Sale,Customer,IT,Logistic,Communication,TechnicalAdvice,Billing,SAV,UsedAndRefurbish,Inventory,User"
docker compose exec -t api sh -c "dotnet run -- --create-user username=$USERNAME email=$USERNAME@shop.net password=$PASSWORD roles=Administrator,IT"

echo "✅ Environnement prêt, base vierge et Admin configuré."

echo ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>"
echo ">> LANCEMENT DES TESTS D'APPELS A L'API ..."
echo ""
echo "+----------------------------------------------------------------------------------------"
echo ""


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
echo "On retire 20 articles ($PRODUCT_ID) du stock du dépôt ($WAREHOUSE_ID)"
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

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 16. Sortie de Stock avec stock insuffisant (POST /StockMovements - OUT)"
echo "+----------------------------------------------------------------------------------------"
echo "On retire 200 articles ($PRODUCT_ID) du stock du dépôt ($WAREHOUSE_ID)"
curl -i -X POST "$API_URL/StockMovements" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d "{
       \"productId\": \"$PRODUCT_ID\",
       \"warehouseId\": \"$WAREHOUSE_ID\",
       \"quantityChange\": -200,
       \"movement\": \"OUT\",
       \"reason\": \"Vente massive\"
     }"

echo ""
echo "+----------------------------------------------------------------------------------------"
echo ":--- 17. Gestion des employés (POST /users/employees)"
echo "+----------------------------------------------------------------------------------------"
echo ": 17.1 - Création du manager (Marc LeManager)"
OLD_MANAGER_RESPONSE=$(curl -s -X POST "$API_URL/users/employees" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d "{
           \"email\": \"marc@entreprise.com\",
           \"password\": \"Password123!\",
           \"firstName\": \"Marc\",
           \"lastName\": \"LeManager\",
           \"service\": \"Logistique\",
           \"managerId\": null
         }")
OLD_MANAGER_ID=$(echo "$OLD_MANAGER_RESPONSE" | jq -r '.userId // empty')
if [ -z "$OLD_MANAGER_ID" ]; then
    echo "❌ Erreur : Impossible de créer le manager."
    echo "Réponse : $OLD_MANAGER_RESPONSE"
    exit 1
fi
echo "✅ Manager créé avec succès. ID : $OLD_MANAGER_ID"
sleep 1
echo ""
echo ": 17.2 - Création d'un employée (Jonathan LePlouk)"
USER_RESPONSE_01=$(curl -s -X POST "$API_URL/users/employees" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d "{
           \"email\": \"jonathan@entreprise.com\",
           \"password\": \"Password123!\",
           \"firstName\": \"Jonathan\",
           \"lastName\": \"LePlouk\",
           \"service\": \"Logistique\",
           \"managerId\": \"$OLD_MANAGER_ID\"
         }")
USER_ID_01=$(echo "$USER_RESPONSE_01" | jq -r '.userId // empty')
if [ -z "$USER_ID_01" ]; then
    echo "❌ Erreur : Impossible de créer l'utilisateur."
    echo "Réponse : $USER_RESPONSE_01"
    exit 1
fi
echo "✅ Utilisateur créé avec succès. ID : $USER_ID_01"
sleep 1
echo ""
echo ": 17.3 - Création d'un second employée (Paul Hille)"
USER_RESPONSE_02=$(curl -s -X POST "$API_URL/users/employees" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d "{
           \"email\": \"paul@entreprise.com\",
           \"password\": \"Password123!\",
           \"firstName\": \"Paul\",
           \"lastName\": \"Hille\",
           \"service\": \"Logistique\",
           \"managerId\": \"$OLD_MANAGER_ID\"
         }")
USER_ID_02=$(echo "$USER_RESPONSE_02" | jq -r '.userId // empty')
if [ -z "$USER_ID_02" ]; then
    echo "❌ Erreur : Impossible de créer l'utilisateur."
    echo "Réponse : $USER_RESPONSE_02"
    exit 1
fi
echo "✅ Utilisateur créé avec succès. ID : $USER_ID_02"
sleep 1
echo ""
echo ": 17.4 - Promotion de Paul Hille (PATCH /users/employees)"
echo ":        Paul devient manager et n'a plus de supérieur."
PROMOTION_RESPONSE=$(curl -s -X PATCH "$API_URL/users/employees/$USER_ID_02" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d "{
           \"managerId\": null
         }")
if echo "$PROMOTION_RESPONSE" | grep -q "error"; then
    echo "❌ Erreur lors de la promotion de Paul."
    echo "Réponse : $PROMOTION_RESPONSE"
else
    echo "✅ Paul Hille est maintenant promu (ManagerId: null)."
fi
sleep 1
echo ""
echo ": 17.5 - Départ de Marc LeManager (DELETE /users/employees)"
DELETE_RESPONSE=$(curl -s -I -X DELETE "$API_URL/users/employees/$OLD_MANAGER_ID" \
     -H "Authorization: Bearer $TOKEN")
echo "✅ Marc LeManager a été supprimé (Soft Delete)."
echo ""
sleep 1
echo ": 17.6 - Réassignement de Jonathan (PATCH /users/employees)"
echo ":        Jonathan LePlouk est réassigné sous les ordres de Paul Hille."
REASSIGN_RESPONSE=$(curl -s -X PATCH "$API_URL/users/employees/$USER_ID_01" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d "{
           \"managerId\": \"$USER_ID_02\"
         }")
NEW_MANAGER_CHECK=$(echo "$REASSIGN_RESPONSE" | jq -r '.newManagerId // empty')
if [ "$NEW_MANAGER_CHECK" == "$USER_ID_02" ]; then
    echo "✅ Jonathan est maintenant supervisé par Paul Hille ($USER_ID_02)."
else
    echo "❌ Erreur : Le réassignement a échoué."
    echo "Réponse API : $REASSIGN_RESPONSE"
    exit 1
fi
echo ""
sleep 1
echo ": 17.7 - Vérification finale de l'état de Jonathan"
FINAL_CHECK_RESPONSE=$(curl -s -X GET "$API_URL/users/employees/$USER_ID_01" \
     -H "Authorization: Bearer $TOKEN")
FINAL_CHECK_MANAGER_ID=$(echo "$FINAL_CHECK_RESPONSE" | jq -r '.managerId // empty')
if [ "$FINAL_CHECK_MANAGER_ID" == "$USER_ID_02" ]; then
    echo "✅ Validation confirmée en base : Jonathan a bien Paul pour manager."
else
    echo "❌ Oups, l'affichage ne correspond pas à l'enregistrement."
    echo "Réponse API : $FINAL_CHECK_RESPONSE"
fi
echo ""
sleep 1


echo ""

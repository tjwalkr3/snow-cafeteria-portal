Should probably look at adding another row in the customer db for admin, or create a new table for admin users

# Endpoints for auth add
## Drink
CreateDrink
UpdateDrinkByID
SetStockStatusByID
DeleteDrinkByID

## Entree
CreateEntree
UpdateEntreeByID
SetStockStatusByID
DeleteEntreeByID

## Location
CreateLocation
UpdateLocation
DeleteLocation
AddLocationHours
UpdateLocationHours
DeleteLocationHours

## ManagerControls
CreateFoodOption
UpdateFoodOption
DeleteFoodOption
CreateFoodType
UpdateFoodType
DeleteFoodType
CreateOptionOptionType
UpdateOptionOptionType
DeleteOptionOptionType

## Order
CreateOrder
GetOrderById # There is a change in naming convention. Others have ID not Id
GetAllOrders

## Side
CreateSide
UpdateSideByID
SetStockStatusById #Here too
DeleteSideByID

## Station
CreateStationForLocation
UpdateStation
DeleteStation
AddStationHours
UpdateStationHours
DeleteStationHours



# Things that need to change with the introduction of customers:
## DTOs
OrderDTO
CreateOrderDTO

# Pages that need updating
ALL OF THEM
We need to add auth to each page Minus the sign in page for the customer app and the admin app
We need to put auth on all of the pages for the customer app
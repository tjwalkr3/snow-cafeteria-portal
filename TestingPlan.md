# Integration Testing Plan

## Overview
This document tracks API endpoints that are missing integration tests. The analysis compares all methods in Cafeteria.Api controllers against existing integration tests in Cafeteria.IntegrationTests.

**Last Updated:** March 16, 2026  
**Total Methods Without Tests:** ~13 methods across 4 controllers

---

## Missing Integration Tests by Controller

### CustomerController ✅ COMPLETE
- All methods have integration tests

### DrinkController ✅ COMPLETE
- ✅ GetDrinkById
- ✅ GetAllDrinks
- ✅ GetDrinksByLocationId
- ✅ CreateDrink
- ✅ UpdateDrinkById
- ✅ `SetStockStatusById` (PUT `/api/drink/{id}/stock`)
- ✅ `DeleteDrinkById` (DELETE `/api/drink/{id}`)

### EntreeController ✅ COMPLETE
- ✅ GetEntreeById
- ✅ GetAllEntrees
- ✅ GetEntreesByStationId
- ✅ CreateEntree
- ✅ UpdateEntreeById
- ✅ `SetStockStatusById` (PUT `/api/entree/{id}/stock`)
- ✅ `DeleteEntreeById` (DELETE `/api/entree/{id}`)

### FoodOptionController ✅ COMPLETE
- ✅ GetFoodOptionById
- ✅ GetAllFoodOptions
- ✅ `GetFoodOptionsByEntreeId` (GET `/api/FoodOption/entree/{entreeId}`)
- ✅ `GetFoodOptionsBySideId` (GET `/api/FoodOption/side/{sideId}`)
- ✅ CreateFoodOption
- ✅ UpdateFoodOptionById
- ✅ `DeleteFoodOptionById` (DELETE `/api/FoodOption/{id}`)

### FoodOptionTypeController ✅ COMPLETE
- ✅ GetFoodOptionTypeByID
- ✅ GetAllFoodOptionTypes
- ✅ `GetFoodOptionTypesByEntreeId` (GET `/api/FoodOptionType/entree/{entreeId}`)
- ✅ `GetFoodOptionTypesWithOptionsByEntreeId` (GET `/api/FoodOptionType/with-options/entree/{entreeId}`)
- ✅ CreateFoodOptionType
- ✅ UpdateFoodOptionTypeById
- ✅ `DeleteFoodOptionTypeById` (DELETE `/api/FoodOptionType/{id}`)

### IconController ✅ COMPLETE
- ✅ `GetAllIcons` (GET `/api/icon`)

### LocationController ✅ COMPLETE
- ✅ GetAllLocations
- ✅ GetLocationById
- ✅ `GetLocationBusinessHoursByLocationId` (GET `/api/location/{locationId}/hours`)
- ✅ `GetLocationBusinessHoursById` (GET `/api/location/hours/{locationHrsId}`)
- ✅ `GetAuthenticatedLocation` (GET `/api/location/authenticated`)
- ✅ CreateLocation
- ✅ UpdateLocationById
- ✅ `DeleteLocationById` (DELETE `/api/location/{locationId}`)
- ✅ `AddLocationHoursByLocationId` (POST `/api/location/{locationId}/hours`)
- ✅ `UpdateLocationHoursById` (PUT `/api/location/hours/{locationHrsId}`)

### OptionOptionTypeController ✅ COMPLETE
- ✅ GetOptionOptionTypeById
- ✅ GetAllOptionOptionTypes
- ✅ CreateOptionOptionType
- ✅ UpdateOptionOptionTypeById
- ✅ `DeleteOptionOptionTypeById` (DELETE `/api/OptionOptionType/{id}`)

### OrderController ✅ COMPLETE
- ✅ GetOrderById
- ✅ GetAllOrders
- ✅ `GetAllOrdersWithCustomer` (GET `/api/order/with-customer`)
- ✅ `GetOrderWithCustomerById` (GET `/api/order/with-customer/{id}`)
- ✅ `GetOrdersByCustomer` (GET `/api/order/customer/{badgerId}`)
- ✅ GetOrdersByCustomerEmail
- ✅ CreateOrder

### SchedulingExceptionsController 🔴 10+ Missing (100% Untested)
**No integration test file exists for this controller**

#### Location Exception Endpoints
- ❌ `GetLocationExceptionsByLocationId` (GET `/api/exception/location/{locationId}`)
- ❌ `GetLocationExceptionById` (GET `/api/exception/location/{exceptionId}/detail`)
- ❌ `AddLocationException` (POST `/api/exception/location/{locationId}`)
- ❌ `UpdateLocationException` (PUT `/api/exception/location/{exceptionId}`)
- ❌ `DeleteLocationException` (DELETE `/api/exception/location/{exceptionId}`)

#### Station Exception Endpoints
- ❌ `GetStationExceptionsByStationId` (GET `/api/exception/station/{stationId}`)
- ❌ `GetStationExceptionById` (GET `/api/exception/station/{exceptionId}/detail`)
- ❌ `AddStationException` (POST `/api/exception/station/{stationId}`)
- ❌ `UpdateStationException` (PUT `/api/exception/station/{exceptionId}`)
- ❌ `DeleteStationException` (DELETE `/api/exception/station/{exceptionId}`)

### SideController ⚠️ 3 Missing
- ✅ GetSideById
- ✅ GetAllSides
- ✅ GetSidesByStationId
- ❌ `GetSidesByStationIdWithOptions` (GET `/api/side/station/{stationId}/with-options`)
- ✅ CreateSide
- ✅ UpdateSideById
- ❌ `SetStockStatusById` (PUT `/api/side/{id}/stock`)
- ❌ `DeleteSideById` (DELETE `/api/side/{id}`)

### StationController ⚠️ 4 Missing
- ✅ GetAllStations
- ✅ GetStationsByLocationId
- ✅ GetStationById
- ❌ `GetStationBusinessHoursByStationId` (GET `/api/station/{stationId}/hours`)
- ❌ `GetStationBusinessHoursById` (GET `/api/station/hours/{stationHrsId}`)
- ✅ CreateStationByLocationId
- ✅ UpdateStationById
- ❌ `DeleteStationById` (DELETE `/api/station/{stationId}`)
- ❌ `AddStationHoursByStationId` (POST `/api/station/{stationId}/hours`)

### SwipeController ⚠️ 1 Partial
- ✅ GetSwipesByUserID
- ✅ GetSwipesByEmail
- ⚠️ `GetAllCustomers` (GET `/api/swipe/all-customers`) - Basic test exists, may need enhancement

---

## Testing Priority

### Priority 1: Critical Gaps (100% Untested Controllers)
- [ ] **SchedulingExceptionsController** - All 10+ endpoints
- [ ] **IconController** - GetAllIcons

### Priority 2: High Coverage Gaps (40%+ Missing)
- [ ] **LocationController** - Business hours endpoints (6 missing)
- [ ] **OrderController** - Customer-specific queries (4 missing)

### Priority 3: Medium Coverage Gaps (20-40% Missing)
- [ ] **DrinkController** - Stock & Delete (2 missing)
- [ ] **EntreeController** - Stock & Delete (2 missing)
- [ ] **SideController** - Stock, Delete, WithOptions (3 missing)
- [ ] **StationController** - Business hours & Delete (4 missing)
- [ ] **FoodOptionController** - Filtered queries & Delete (3 missing)
- [ ] **FoodOptionTypeController** - Filtered queries & Delete (3 missing)

### Priority 4: Low Coverage Gaps (Single Methods)
- [ ] **OptionOptionTypeController** - DeleteOptionOptionTypeById

---

## Standards for Integration Tests

When creating integration tests, follow these patterns from existing tests:

1. **Class Structure**
   - Inherit from `IDisposable`
   - Use `[Collection("Database")]` attribute
   - Setup DatabaseFixture and HttpClient in constructor

2. **CRUD Pattern**
   ```csharp
   [Fact]
   public async Task GetById_ReturnsCorrect[Entity]()
   {
       var response = await _client.GetAsync("/api/[endpoint]/[id]");
       response.EnsureSuccessStatusCode();
       var result = await response.Content.ReadFromJsonAsync<[DtoType]>();
       Assert.NotNull(result);
       // Assert properties
   }
   ```

3. **Database Setup**
   - Use pre-loaded sample data when possible
   - Use `SqlInsertQueries` for test-specific data
   - Clean up in `Dispose()` or within test methods

4. **Test Naming**
   - `[MethodName]_[Scenario]_[ExpectedOutcome]()`
   - Examples: `CreateOrder_AddsNewOrder()`, `GetById_ReturnsNotFound_WhenNotExists()`

---

## Tracking Progress

### Completed
- ✅ Customer integration tests
- ✅ Drink integration tests (all methods)
- ✅ Entree integration tests (all methods)
- ✅ FoodOption integration tests (all methods)
- ✅ FoodOptionType integration tests (all methods)
- ✅ Icon integration tests (all methods)
- ✅ Location integration tests (all methods)
- ✅ OptionOptionType integration tests (all methods)
- ✅ Order integration tests (all methods)

### In Progress
- None

### Not Started
- All others

---

## Notes
- When implementing tests for DELETE endpoints, verify the resource is actually removed from the database
- Business hours endpoints may require additional setup with location/station relationships
- SchedulingExceptions controller requires creating dedicated test file
- Ordered endpoints (with-customer, by-customer) may need authentication mocking

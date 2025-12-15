# FakeStore API Test Suite

This project is an **automated test suite** for the [FakeStore API](https://fakestoreapi.com/), built using **C#, NUnit, and RestSharp**. It covers user stories for online shopping, product management, and admin actions.

---

## **User Stories Covered**

### **User Story 1: Online Shopper**
- View all available products.
- Filter electronics in stock.
- Add the **cheapest electronics product** to the cart.
- **Acceptance Criteria Validated:**
  - Products belong to the electronics category.
  - Only in-stock products considered.
  - Cart reflects correct product and quantity.

### **User Story 2: Store Manager**
- Add **three unique clothing items** to the product catalog.
- **Acceptance Criteria Validated:**
  - Each product has a unique title/ID.
  - Products with duplicates or invalid data are rejected.
  - Newly added items appear in the product listing.

### **User Story 3: Store Admin**
- Delete the product with the **lowest customer rating**.
- **Acceptance Criteria Validated:**
  - Product selection based on lowest rating.
  - Product is removed from listings after deletion.
  - Retrieving deleted product returns `404 Not Found`.

---

## **Project Structure**

- `Clients/` - API client wrapper using RestSharp.
- `Models/` - Classes representing API entities (`Product`, `Cart`, `Login`).
- `Tests/` - NUnit test classes for each user story and CRUD/negative tests.
- `azure-pipelines.yml` - CI pipeline to run tests on Azure DevOps.

---

## **Running Tests Locally**

1. Clone the repo:

```bash
git clone https://github.com/go2khurram/FakeStoreAPITests.git
cd FakeStoreAPITests

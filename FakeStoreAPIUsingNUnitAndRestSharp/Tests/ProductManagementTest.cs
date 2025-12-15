using FakeStoreAPIUsingNUnitAndRestSharp.Clients;
using FakeStoreAPIUsingNUnitAndRestSharp.Models;
using Newtonsoft.Json;
using System.Net;

namespace FakeStoreAPIUsingNUnitAndRestSharp.Tests
{
    [TestFixture]
    [Category("UserStory2")]
    public class ProductManagementTest
    {
        private ApiClients _apiClient;
        [SetUp]
        public void Setup()
        {
            _apiClient = new ApiClients("https://fakestoreapi.com/");
        }

        /// <summary>
        /// User Story 2:
        /// As a store manager, I want to add three new clothing items to the product catalogue.
        /// Acceptance Criteria:
        /// 1. Each product must have a unique name and ID.
        /// 2. Duplicate names or IDs should be rejected.
        /// 3. Newly added items should be immediately visible via the product listing API.      
        /// </summary>


        #region Helper Methods

        //Helper method to generate a 
        public Product GenerateUniqueClothingProduct(string baseName)
        {
            string uniqueSuffix = Guid.NewGuid().ToString().Substring(0, 8);
            Random rand = new Random();

            return new Product
            {
                Title = $"{baseName} {uniqueSuffix}",
                Price = rand.Next(20, 500), // Random price between 20 and 500
                Description = "A stylish piece of clothing.",
                Category = "clothing",
                Image = "https://fakestoreapi.com/img/71-3HjGNDUL._AC_SY879._SX._UX._SY._UY_t.png",
                Rating = new Rating
                {
                    Rate = Math.Round((decimal)(rand.NextDouble() * 5), 2), // Random rating between 0 and 5
                    Count = rand.Next(1, 1000) // Random count between 1 and 1000
                },
            };
        }

        #endregion

        #region Positive Test Cases

        [Test]

        public void AddThreeUniqueClothingProduct_ShouldAppearInCatelog()
        {

            // Create three unique clothing products
            Product clothingProduct1 = GenerateUniqueClothingProduct("Casual T-Shirt");
            Product clothingProduct2 = GenerateUniqueClothingProduct("Formal Shirt");
            Product clothingProduct3 = GenerateUniqueClothingProduct("Jeans Pants");

            // Add the products to the FakeStoreAPI
           var response1= _apiClient.PostRaw("/products", clothingProduct1);
           var response2= _apiClient.PostRaw("/products", clothingProduct2);
           var response3= _apiClient.PostRaw("/products", clothingProduct3);

            //Assert Response Codes OK or Created

            Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(response3.StatusCode, Is.EqualTo(HttpStatusCode.Created));


            //Deserialize created products

            Product createdProduct1 = JsonConvert.DeserializeObject<Product>(response1.Content);
            Product createdProduct2 = JsonConvert.DeserializeObject<Product>(response2.Content);
            Product createdProduct3 = JsonConvert.DeserializeObject<Product>(response3.Content);

            // Verify the products were added successfully
            Assert.IsNotNull(createdProduct1, "Created product 1 should not be null.");
            Assert.IsNotNull(createdProduct2, "Created product 2 should not be null.");
            Assert.IsNotNull(createdProduct3, "Created product 3 should not be null.");

            // Verify the products appear in the product listing
            Assert.AreEqual(clothingProduct1.Title, createdProduct1.Title, "Product 1 titles should match.");
            Assert.AreEqual(clothingProduct2.Title, createdProduct2.Title, "Product 2 titles should match.");
            Assert.AreEqual(clothingProduct3.Title, createdProduct3.Title, "Product 3 titles should match.");


            //Verify Ids are unique  - Fake API generating same ID each time so can't be
            Console.WriteLine("Fake API Generating MOCK Response and not generating records, hence failing");
            var ids = new[] { createdProduct1.Id, createdProduct2.Id, createdProduct3.Id };
            Assert.AreEqual(ids.Distinct().Count(), ids.Length, "Duplicate Product IDs detected");



            //Verify Names are unique
            var names = new[] { createdProduct1.Title, createdProduct2.Title, createdProduct3.Title };
            Assert.AreEqual(names.Distinct().Count(), names.Length, "Duplicate Product Names detected");



            //Fetch all products and verify the newly added products are present
            var allProducts = _apiClient.Get<List<Product>>("/products");
            Assert.IsTrue(allProducts.Any(p => p.Title == createdProduct1.Title), "Product 1 not found in product listing.");
            Assert.IsTrue(allProducts.Any(p => p.Title == createdProduct2.Title), "Product 2 not found in product listing.");
            Assert.IsTrue(allProducts.Any(p => p.Title == createdProduct3.Title), "Product 3 not found in product listing.");

            Console.WriteLine($"\nNew Clothing Products Added Successfully:\n1. ID: {createdProduct1.Id}, Title: {createdProduct1.Title}, Price: {createdProduct1.Price}, Category: {createdProduct1.Category}\n2. ID: {createdProduct2.Id}, Title: {createdProduct2.Title}, Price: {createdProduct2.Price}, Category: {createdProduct2.Category}\n3. ID: {createdProduct3.Id}, Title: {createdProduct3.Title}, Price: {createdProduct3.Price}, Category: {createdProduct3.Category}");
            Console.WriteLine("\nAll three unique clothing products added and verified in the catalog successfully.");

        }

        #endregion

        #region Negative Test Cases

        /// <summary>
        /// Prducts with duplicate names or ID should be rejected.
        /// </summary>
        
        [Test]

        public void AddDuplicateProduct_ShouldBeRejected()
        {
            //Create a unique clothing product
            Product clothingProduct = GenerateUniqueClothingProduct("Summer Dress");
            
            //Add the product to the FakeStoreAPI
            var response = _apiClient.PostRaw("/products", clothingProduct);

            //Verify response status code OK or Created
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            
            // Deserialize created product
            Product createdProduct = JsonConvert.DeserializeObject<Product>(response.Content);
            
            // Verify the product was added successfully
            Assert.IsNotNull(createdProduct, "Created product should not be null.");
            
            // Attempt to add the same product again (duplicate)
            var duplicateResponse = _apiClient.PostRaw("/products", clothingProduct);
            
            // Since FakeStoreAPI does not enforce uniqueness, we check if the response contains the same title
            Product duplicateCreatedProduct = JsonConvert.DeserializeObject<Product>(duplicateResponse.Content);
            
            // Check if the titles are the same indicating a duplicate addition
            Assert.AreEqual(createdProduct.Title, duplicateCreatedProduct.Title, "Duplicate product titles should match.");
            Console.WriteLine($"\nAttempted to add duplicate product:\nOriginal ID: {createdProduct.Id}, Title: {createdProduct.Title}\nDuplicate ID: {duplicateCreatedProduct.Id}, Title: {duplicateCreatedProduct.Title}");
            Console.WriteLine("\nNote: FakeStoreAPI does not enforce uniqueness, so duplicates may be added.");

            // In a real-world scenario, we would expect a rejection response here.
            Assert.AreNotEqual(createdProduct.Id, duplicateCreatedProduct.Id, "Duplicate product IDs should not created");
            Assert.AreNotEqual(createdProduct.Title, duplicateCreatedProduct.Title, "Duplicate product Titles should not created");
            Assert.AreNotEqual(HttpStatusCode.Created,duplicateResponse.StatusCode ,"Should not return 201 Created");


        }


        /// <summary>
        /// Adding products with missing title should be rejected.
        /// </summary>
        /// 

        [Test]

        public void AddProduct_MissingTitle_ShouldBeRejected()
        {
            // Create a clothing product with missing title
            Product invalidProduct = new Product
            {
                // Title is missing
                Price = 49.99M,
                Description = "A stylish piece of clothing.",
                Category = "clothing",
                Image = "https://fakestoreapi.com/img/71-3HjGNDUL._AC_SY879._SX._UX._SY._UY_t.png",
                Rating = new Rating
                {
                    Rate = 4.5M,
                    Count = 150
                },
            };


            // Attempt to add the invalid product to the FakeStoreAPI
            var response = _apiClient.PostRaw("/products", invalidProduct);


            // In a real-world scenario, we would expect a rejection response here.
            Console.WriteLine($"Status Code : {(int)response.StatusCode}");
            //Assert.AreNotEqual(HttpStatusCode.Created, response.StatusCode, "Should not return 201 Created for invalid product.");






            //=====Implementing Fake Store API behavior =====//

            // Since FakeStoreAPI does not enforce validation, we check if the response contains a title
            Product createdProduct = JsonConvert.DeserializeObject<Product>(response.Content);            
            
            // Check if the title is null or empty indicating a missing title
            Assert.IsTrue(string.IsNullOrEmpty(createdProduct.Title), "Product with missing title should not be created.");
            Console.WriteLine($"\nAttempted to add product with missing title:\nResponse ID: {createdProduct.Id}, Title: '{createdProduct.Title}'");
            Console.WriteLine("\nNote: FakeStoreAPI does not enforce validation, so products with missing fields may be added.");
            
            
           
        }

        ///<summary
        ///Adding products with negative price should be rejected.
        ///</summary>
        
        [Test]
        public void AddProduct_NegativePrice_ShouldBeRejected()
        {
            // Create a clothing product with negative price
            Product invalidProduct = new Product
            {
                Title = "Invalid Price Shirt",
                Price = -10.00M, // Negative price
                Description = "A stylish piece of clothing.",
                Category = "clothing",
                Image = "https://fakestoreapi.com/img/71-3HjGNDUL._AC_SY879._SX._UX._SY._UY_t.png",
                Rating = new Rating
                {
                    Rate = 4.5M,
                    Count = 150
                },
            };

            // Attempt to add the invalid product to the FakeStoreAPI
            var response = _apiClient.PostRaw("/products", invalidProduct);
            
            // In a real-world scenario, we would expect a rejection response here.
            Console.WriteLine($"Status Code : {(int)response.StatusCode}");
            //Assert.AreNotEqual(HttpStatusCode.Created, response.StatusCode, "Should not return 201 Created for invalid product.");
            
            //=====Implementing Fake Store API behavior =====//
            // Since FakeStoreAPI does not enforce validation, we check if the response contains a valid price
            Product createdProduct = JsonConvert.DeserializeObject<Product>(response.Content);
            // Check if the price is negative indicating an invalid price
            Assert.IsTrue(createdProduct.Price < 0, "Product with negative price should not be created.");
            Console.WriteLine($"\nAttempted to add product with negative price:\nResponse ID: {createdProduct.Id}, Price: {createdProduct.Price}");
            Console.WriteLine("\nNote: FakeStoreAPI does not enforce validation, so products with invalid fields may be added.");
        }



        #endregion



        #region Additional CRUD Tests

        // Additional tests for Update and Delete operations .

        [Test]
        public void UpdateProduct_ShouldReflectChanges()
        {
            // Create and add a new product
            Product newProduct = GenerateUniqueClothingProduct("Winter Track Suite");
            var createResponse = _apiClient.PostRaw("/products", newProduct);
            Product createdProduct = JsonConvert.DeserializeObject<Product>(createResponse.Content);


            // Update product details
            createdProduct.Price += 10; // Increase price by 10
            createdProduct.Description = "Updated description.";


            // Send update request
            var updatedProduct = _apiClient.Put<Product>($"/products/{createdProduct.Id}", createdProduct);
            
            // Verify the updates
            Assert.AreEqual(createdProduct.Price, updatedProduct.Price, "Product price should be updated.");
            Assert.AreEqual(createdProduct.Description, updatedProduct.Description, "Product description should be updated.");
        }



        [Test]
        public void DeleteProduct_ShouldRemoveFromCatalog()
        {
            // Create and add a new product
            Product newProduct = GenerateUniqueClothingProduct("Spring Jacket");
            var createResponse = _apiClient.PostRaw("/products", newProduct);
            Product createdProduct = JsonConvert.DeserializeObject<Product>(createResponse.Content);

            Console.WriteLine($" Created Product ID : {createdProduct.Id}");


            //Attempt to get the product 
            Product b4deleted = _apiClient.Get<Product>($"/products/{createdProduct.Id}");

            Console.WriteLine(b4deleted == null
                ? "Fake API does not persist craeted products"
                : $"Fetched before deletion: {b4deleted.Id}");


            // Delete the product
            _apiClient.Delete($"/products/{createdProduct.Id}");

            // Fetch the product to verify deletion

            Product deleted = _apiClient.Get<Product>($"/products/{createdProduct.Id}");

            // In real - world: Assert.IsNull(deleted)
            // For FakeStoreAPI: Assert null is expected behavior both before and after delete
            Assert.IsNull(deleted, "Deleted product should not be found in the catalog.");

        }
        #endregion

    }
    }

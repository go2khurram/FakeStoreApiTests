using FakeStoreAPIUsingNUnitAndRestSharp.Clients;
using FakeStoreAPIUsingNUnitAndRestSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreAPIUsingNUnitAndRestSharp.Tests
{
    [TestFixture]
    [Category("UserStory3")]
    public class ProductDeletionTest
    {

        private ApiClients _apiClient;

        [SetUp]
        public void Setup()
        {
            _apiClient = new ApiClients("https://fakestoreapi.com/");
        }

        /// <summary>
        /// User Story 3:
        /// As a store admin, I want to delete the product with the lowest rating from the store.
        /// Acceptance Criteria:
        /// 1. The product must be selected based on the lowest customer rating.
        /// 2. After deletion, the product should no longer appear in any product listing.
        /// 3. Attempts to retrieve the deleted product should return a 404 Not Found.
        /// </summary>


        [Test]
        public void DeleteLowerRatigProduct()
        {
            // Step 1: Retrieve all products
            var allProducts = _apiClient.Get<List<Product>>("/products");

            Assert.IsNotNull(allProducts , "Failed to fetch products from API.");

            
            // Step 2: Identify the product with the lowest rating
            var lowestRatedProduct = allProducts.OrderBy(p => p.Rating.Rate).First();

            Assert.IsNotNull(lowestRatedProduct, "No products available to delete.");
            Console.WriteLine($"\nLowest Rated Product Identified:\nID: {lowestRatedProduct.Id},Title: {lowestRatedProduct.Title}, Rating: {lowestRatedProduct.Rating?.Rate}");

            // Step 3: Delete the identified product
            var deleteRespose = _apiClient.Delete($"products/{lowestRatedProduct.Id}");
            Console.WriteLine($"\nDelete Response Status Code: {(int)deleteRespose.StatusCode} {deleteRespose.StatusCode}");

            // In a real API, we’d expect 200 OK or 204 NoContent
            Assert.That(deleteRespose.StatusCode == System.Net.HttpStatusCode.OK || deleteRespose.StatusCode == System.Net.HttpStatusCode.NoContent,
                "Product deletion failed.");

            //Step 4: Try fecting the deleted product
            var getDeletedProductResponse = _apiClient.GetRaw($"products/{lowestRatedProduct.Id}");
            Console.WriteLine($"\nGet After Delete Status: {(int)getDeletedProductResponse.StatusCode}   {getDeletedProductResponse.StatusCode}");


            // Step 5: Verify product no longer exists
            // In a real API, we’d expect 404 Not Found — FakeStoreAPI returns 200 OK but same static list
            if (getDeletedProductResponse.StatusCode == HttpStatusCode.NotFound)
            {
                Assert.Pass("Deleted product is not retrievable (404 Not Found).");
            }
            else
            {
                Console.WriteLine("FakeStoreAPI does not persist deletions; verifying via product listing...");

                var refreshedProducts = _apiClient.Get<List<Product>>("/products");
                bool stillExists = refreshedProducts.Any(p => p.Id == lowestRatedProduct.Id);

                Assert.IsFalse(stillExists, "Deleted product still appears in product listing.");
            }
            
        }


    }
}

using FakeStoreAPIUsingNUnitAndRestSharp.Clients;
using FakeStoreAPIUsingNUnitAndRestSharp.Models;
using RestSharp;
using System.Diagnostics;

namespace FakeStoreAPIUsingNUnitAndRestSharp.Tests
{

    /// <summary>
    /// User Story 1:
    /// As an online shopper, I want to view all available products and add the cheapest electronics item to my cart.
    /// Acceptance Criteria:
    /// 1. The product must belong to a specific category.
    /// 2. Only products that are in stock (have quantity available) should be considered.
    /// 3. Once added to the cart, the product should appear with correct price and quantity.
    /// 
    /// Additional CRUD Tests:
    /// Covers GET, POST, PUT, DELETE for Products and Carts
    /// </summary>
    /// 



    [TestFixture]
    [Category("UserStory1")]
    
    public class OnlineShopperWorkflowTests
    {
        public ApiClients _apiClient;

        [SetUp]
        public void Setup()
        {
            _apiClient = new ApiClients("https://fakestoreapi.com/");
        }


        #region HelperMethod

        private void MockProductStock(List<Product> products)
        {
            Random rand = new Random();
            foreach (var product in products)
            {
                product.Quantity = rand.Next(0, 10); // Random quantity between 0 and 10
            }
        }

        #endregion


        #region Main Acceptance Test - User Story 1


        [Test]

        public void OnlineShopper_ShouldViewAllElectronicsProductAndAddCheapestItemToCart()
        {
            // Fetch all products from the API

            List<Product> allProducts = _apiClient.Get<List<Product>>("/products");

            // Mock the quantity since FakeStoreAPI does not provide it

            MockProductStock(allProducts);

            Console.WriteLine("\n ====== All Products with Mock Stock ===\n");

            foreach (var product in allProducts)
            {
                Console.WriteLine($"ID: {product.Id}, Title: {product.Title}, Price: {product.Price}, Category: {product.Category} ,Quantity: {product.Quantity}");
            }


            //Filter products by 'electronics' category and in stock

            List<Product> electronics = allProducts.Where(p => p.Category.ToLower() == "electronics" && p.Quantity >0 ).ToList();

            Assert.IsNotEmpty(electronics , "No in-stock electronics products found.");

            // Validating Acceptance Criteria 1:  "The product must belong to a specific category." 

            Assert.IsTrue(electronics.All(p => p.Category.ToLower() == "electronics"));

            // Verify Acceptance Criteria 2:  "Only products that are in stock (have quantity available) should be considered" 
           
            Assert.IsTrue(electronics.All(p=>p.Quantity >0));

            

            //Find the cheapest product in electronics

            Product cheapestProduct = electronics.OrderBy(p => p.Price).First();

            Console.WriteLine("\n======Cheapest Electronics Product=====\n");
            Console.WriteLine($"ID: {cheapestProduct.Id}, Title: {cheapestProduct.Title}, Price: {cheapestProduct.Price}, Category: {cheapestProduct.Category}");

            //Add the cheapest product to a new cart
            Console.WriteLine($"Adding cheapest product '{cheapestProduct.Title}' to cart...");

            Cart newCart = new Cart
            {
                UserId = 1,
                Products = new List<CartItems>
                {
                    new CartItems
                    {
                        ProductId = cheapestProduct.Id,
                        Quantity = 1
                    }
                }
            };


            //Post the cart to the API

            Cart addCart = _apiClient.Post<Cart>("/carts", newCart);

            Assert.IsNotNull(addCart, "Failed to create a new cart.");
            Assert.AreEqual(1, addCart.Products.Count, "The cart does not contain the expected number of products.");

            //Verify the product in the cart is the cheapest electronics product

            CartItems cartItem = addCart.Products.FirstOrDefault();
            Assert.AreEqual(cheapestProduct.Id, cartItem.ProductId, "Incorrect product added to cart.");
            Assert.AreEqual(1, cartItem.Quantity, "The quantity of the product in the cart is incorrect.");

        }

        #endregion


        #region Additional CRUD Tests for Products

        [Test]
        public void GetProductById_ShouldReturnCorrectProduct()
        {
            int productId = 15; // Example product ID

            Product product = _apiClient.Get<Product>($"/products/{productId}");

            Assert.IsNotNull(product, $"Product with ID {productId} should not be null.");

            Assert.AreEqual(productId, product.Id, $"Product ID should be {productId}.");

            Console.WriteLine($"\nRetrieved Product:\nID: {product.Id}, Title: {product.Title}, Price: {product.Price}, Category: {product.Category}");
        }


        [Test]

        public void UpdateProduct_ShouldReturnUpdatedProduct()
        {
            // Product ID to update
            int productId = 10;

            //Fetching the existing product
            Product existingProduct = _apiClient.Get<Product>($"/products/{productId}");
            
            Assert.IsNotNull(existingProduct, $"Product with ID {productId} should not be null.");

            // Update product details
            existingProduct.Price += 5.00m; // Increase price by 5
            existingProduct.Title += " - Updated";

            // Send PUT request to update the product
            Product updatedProduct = _apiClient.Put<Product>($"/products/{productId}", existingProduct);

            //Assertions to verify the update
            Assert.IsNotNull(updatedProduct, "Failed to update the product.");
            Assert.AreEqual(existingProduct.Price, updatedProduct.Price, "Product price was not updated correctly.");
            Assert.AreEqual(existingProduct.Title, updatedProduct.Title, "Product title was not updated correctly.");
            Console.WriteLine($"\nUpdated Product:\nID: {updatedProduct.Id}, Title: {updatedProduct.Title}, Price: {updatedProduct.Price}, Category: {updatedProduct.Category}");
        }
        

        [Test]
        public void DeleteProduct_ShouldReturnNullOnGet()
        {
            //Create a Dummy Product to delete

            Product dummyProduct = new Product
            {
                Title = "Dummy Product for Delete",
                Price = 99.99M,
                Description = "This is a dummy product created for deletion test.",
                Category = "electronics",
                Image = "https://fakestoreapi.com/img/71-3HjGNDUL._AC_SY879._SX._UX._SY._UY_t.pn",
                Rating = new Rating { Rate = 0, Count = 0 }

            };


            Product createdProduct = _apiClient.Post<Product>("/products", dummyProduct); 
            Assert.IsNotNull(createdProduct, "Failed to create a dummy product for deletion test.");

            // Send DELETE request to remove the product
            _apiClient.Delete($"/products/{createdProduct.Id}");

            // Attempt to fetch the deleted product
            Product deletedProduct = _apiClient.Get<Product>($"/products/{createdProduct.Id}");

            // Assertion to verify the product is deleted
            Assert.IsNull(deletedProduct, $"Product with ID {createdProduct.Id} should be null after deletion.");

        }


        #endregion


        #region Additional CRUD Tests for Carts

        [Test]

        public void CreateCartWithMultipleProducts_ShouldReturnCartWithProducts()
        {
            // Create a new cart with multiple products
            Cart newCart = new Cart
            {
                UserId = 1,
                Products = new List<CartItems>
                {
                    new CartItems { ProductId = 1, Quantity = 3 },
                    new CartItems { ProductId = 2, Quantity = 1 },
                    new CartItems { ProductId = 3, Quantity = 5 }
                }
            };
            // Post the cart to the API
            Cart createdCart = _apiClient.Post<Cart>("/carts", newCart);
            Assert.IsNotNull(createdCart, "Failed to create a new cart.");
            Assert.AreEqual(3, createdCart.Products.Count, "The cart does not contain the expected number of products.");
            Console.WriteLine($"\nCreated Cart ID: {createdCart.Id} with {createdCart.Products.Count} products.");
        }


        [Test]

        public void UpdateCart_ShouldChangeProductQuantity()
        {
            Cart newCart = new Cart
            {
                UserId = 1,
                Products = new List<CartItems>
                {
                    new CartItems { ProductId = 4, Quantity = 2 }
                }
            };

            var createdCart = _apiClient.Post<Cart>("/carts", newCart);

            createdCart.Products[0].Quantity = 3; // Change quantity to 3

            var updatedCart = _apiClient.Put<Cart>($"/carts/{createdCart.Id}", createdCart);

            Assert.IsNotNull(updatedCart, "Failed to update the cart.");
            Assert.AreEqual(3, updatedCart.Products[0].Quantity, "Cart quantity update failed.");

            Console.WriteLine($"\nUpdated Cart ID: {updatedCart.Id} with Product ID: {updatedCart.Products[0].ProductId}, Quantity: {updatedCart.Products[0].Quantity}");

        }



        [Test]

        public void DeleteCart_ShouldReturnNullOnGet()
        {
            // Create a new cart to delete
            Cart newCart = new Cart
            {
                UserId = 1,
                Products = new List<CartItems>
                {
                    new CartItems { ProductId = 5, Quantity = 1 }
                }
            };


            var createdCart = _apiClient.Post<Cart>("/carts", newCart);

            Assert.IsNotNull(createdCart, "Failed to create a new cart for deletion test.");

            // Send DELETE request to remove the cart
            _apiClient.Delete($"/carts/{createdCart.Id}");

            // Attempt to fetch the deleted cart
            
            Cart deletedCart = _apiClient.Get<Cart>($"/carts/{createdCart.Id}");
            
            // Assertion to verify the cart is deleted
            Assert.IsNull(deletedCart, $"Cart with ID {createdCart.Id} should be null after deletion.");
        }



        [Test]
        public void GetCartById_ShouldReturnCorrectCart()
        {
            int cartId = 4; // Example cart ID
            Cart cart = _apiClient.Get<Cart>($"/carts/{cartId}");
            Assert.IsNotNull(cart, $"Cart with ID {cartId} should not be null.");
            Assert.AreEqual(cartId, cart.Id, $"Cart ID should be {cartId}.");
            Console.WriteLine($"\nRetrieved Cart:\nID: {cart.Id}, UserID: {cart.UserId}, Products Count: {cart.Products.Count}");
        }


        #endregion


        #region NegativeTests

        [Test]
        public void GetProductByInvalidId_ShouldReturnNull()
        {
            int invalidProductId = 9999; // Assuming this ID does not exist
            Product product = _apiClient.Get<Product>($"/products/{invalidProductId}");
            Assert.IsNull(product, $"Product with ID {invalidProductId} should be null.");
        }

        [Test]

        public void CreateCartWithoutProducts_ShouldReturnCartWithNoProducts()
        {
            Cart newCart = new Cart
            {
                UserId = 1,
                Products = new List<CartItems>()
            };

            Cart createdCart = _apiClient.Post<Cart>("/carts", newCart);

            Assert.IsNotNull(createdCart, "Failed to create a new cart.");

            Assert.AreEqual(0, createdCart.Products.Count, "The cart should not contain any products.");

            Console.WriteLine($"\nCreated Cart ID: {createdCart.Id} with {createdCart.Products.Count} products.");
        }

        #endregion

    }
}
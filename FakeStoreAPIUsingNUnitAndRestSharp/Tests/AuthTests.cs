using FakeStoreAPIUsingNUnitAndRestSharp.Clients;
using FakeStoreAPIUsingNUnitAndRestSharp.Models;
using Newtonsoft.Json;
using System.Net;

namespace FakeStoreAPIUsingNUnitAndRestSharp.Tests
{
    [TestFixture]
    [Category("Authentication")]
    public class AuthTests
    {
        ApiClients _apiClient;

        [SetUp]
        public void Setup()
        {
            _apiClient = new ApiClients("https://fakestoreapi.com/");
        }

        [Test]
        public void TestLoginWithValidCredentials_ShouldReturnToken()
        {
            var loginPayload = new
            {
                username = "mor_2314",
                password = "83r5^_"
            };

            var response = _apiClient.PostRaw("/auth/login", loginPayload);
            Assert.IsNotNull(response);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.Created), "Login request did not return OK status.");

            var loginResponse  = JsonConvert.DeserializeObject<LoginResponse>(response.Content);

            Assert.IsNotNull(loginResponse, "Login response deserialization failed.");

            Assert.IsFalse(string.IsNullOrEmpty(loginResponse.Token), "Token is null or empty in the login response.");

            Console.WriteLine($"\nAuthentication successful. Token: {loginResponse.Token}");

        }


        [Test]

        public void TestLoginWithInvalidCredentials_ShouldReturnUnauthorized()
        {
            var loginPayload = new
            {
                username = "invalid_user",
                password = "wrong_password"
            };

            var response = _apiClient.PostRaw("/auth/login", loginPayload);

            Assert.IsNotNull(response);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Expected Unauthorized status for invalid credentials.");

            Console.WriteLine($"\nAuthentication failed as expected. Status Code: {(int)response.StatusCode} {response.StatusCode}");
        }



    }
}

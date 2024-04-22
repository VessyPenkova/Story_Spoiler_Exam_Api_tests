using System;
using System.Net;
using System.Text.Json;
using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using Story_Spoiler_Exam_Api_tests.Models;

namespace Story_Spoiler_Exam_Api_tests
{
    [TestFixture]
    public class StorySpoilerApiTests
    {
        private RestClient client;
        private static string storyId;

        private static string email = "veselina";
        private static string password = "veselina76";
        private static string baseUrl = "https://d5wfqm7y6yb3q.cloudfront.net";

        [OneTimeSetUp]
        public void Setup()
        {
            string jwtToken = GetJwtToken(email, password);

            var options = new RestClientOptions(baseUrl)
            {
                Authenticator = new JwtAuthenticator(jwtToken)
            };

            this.client = new RestClient(options);
        }

        private string GetJwtToken(string email, string password)
        {
            var authenticationClient = new RestClient(baseUrl);
            var authenticationRequest = new RestRequest("/api/User/Authentication", Method.Post);
            authenticationRequest.AddJsonBody(new
            {
                UserName = email,
                Password = password
            });

            var response = authenticationClient.Execute(authenticationRequest);

            if (response.IsSuccessStatusCode)
            {
                var content = JsonSerializer.Deserialize<AuthenticationResponse>(response.Content);

                var accessToken = content.AccessToken;


                return accessToken;
            }
            else
            {
                throw new InvalidOperationException("Authentication failed");
            }
        }

        [Order(1)]
        [Test]
        public void CreateNewStorySpoiler_WithRequiredFields_ShouldSucceed()
        {
            //Arrange
            var newStorySpoilerRequest = new StoryDTO
            {
                Title = "New Story",
                Description = "Description of the new story.",
                Url = "",
            };
            var request = new RestRequest("/api/Story/Create", Method.Post);
            request.AddJsonBody(newStorySpoilerRequest);

            //Act
            var response = this.client.Execute(request);

            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var responseData = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);
            Assert.That(responseData.Msg, Is.EqualTo("Successfully created!"));
            storyId = responseData.StoryId;
        }

        [Order(2)]
        [Test]
        public void GetAllStories_ShouldReturnNonEmptyArray()
        {
            //Arrange
            var allRequest = new RestRequest("/api/Story/All", Method.Get);

            //Act
            var allResponse = this.client.Execute(allRequest);

           //Assert
            Assert.That(allResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var responseData = JsonSerializer.Deserialize<List<ApiResponseDTO>>(allResponse.Content);
            Assert.That(allResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.IsNotNull(responseData);
            Assert.IsNotEmpty(responseData);
            storyId = responseData.LastOrDefault()?.StoryId;
        }

        [Order(3)]
        [Test]
        public void EditStory_WithNewTitleAndDescription_ShouldSucceed()
        {
            //Arrange
            var editRequest = new StoryDTO
            {
                Title = "Edited Title of the Story",
                Url = "",
                Description = "Updated description."

            };

            var request = new RestRequest($"/api/Story/Edit/{storyId}", Method.Put);
            request.AddQueryParameter("storyId", storyId);
            request.AddJsonBody(editRequest);

            //Act
            var response = this.client.Execute(request);
            var editResponse = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(editResponse.Msg, Is.EqualTo("Successfully edited"));
        }

        [Order(4)]
        [Test]
        public void DeleteExistingStory_ShouldSucceed()
        {
            var deleteRequest = new RestRequest($"/api/Story/Delete", Method.Delete);
            deleteRequest.AddQueryParameter("storyId", storyId);
            var deleteResponse = this.client.Execute(deleteRequest);

            //Assert
            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));           
        }

        [Order(5)]
        [Test]
        public void CreateNewStorySpoiler_WithouRequiredFields_ShouldFeil()
        {
            //Arrange
            var newStorySpoilerRequest = new StoryDTO
            {
                //Title = "New Story",
                Description = "Description of the new story.",
                Url = "",
            };
            var request = new RestRequest("/api/Story/Create", Method.Post);
            request.AddJsonBody(newStorySpoilerRequest);

            //Act
            var response = this.client.Execute(request);

            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            var responseData = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);
            //Assert.That(responseData.Msg, Is.EqualTo("Successfully created!"));
            //storyId = responseData.StoryId;
        }

        [Order(6)]
        [Test]
        public void EditNotExistingStory_ShouldFeil()
        {
            //Arrange
            string nonExistingStoryId = "123";
            var editRequest = new StoryDTO
            {
                Title = "Edited Title of the Story",
                Description = "Updated description."

            };

            var request = new RestRequest($"/api/Story/Edit/{nonExistingStoryId}", Method.Put);
            request.AddQueryParameter("storyId", nonExistingStoryId);
            request.AddJsonBody(editRequest);

            //Act
            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        }

    }
}
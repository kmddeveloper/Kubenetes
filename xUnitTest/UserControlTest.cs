using Kubernetes.TransferObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Web;
using Xunit;
using Xunit.Extensions;

namespace xUnitTest
{
    public class UserControlTest: IClassFixture<WebApplicationFactory<Startup>>
    {

        readonly WebApplicationFactory<Startup> _factory;

        public UserControlTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }



        public static IEnumerable<object[]> UserData =>                   
            new List<object[]>
            {
                new object[] { 1, "Kevin" }
            };
                 

        [Theory]
        [MemberData(nameof(UserData))]   
        public async Task GetUserByIdTest(int Id, string firstName)
        {

            var input = new User { Id = Id };
            var payload = JsonConvert.SerializeObject(input);

            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
            // Act
            var client = _factory.CreateClient();
            var response = await client.PostAsync("/api/user", content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var user = JsonConvert.DeserializeObject<User>(responseString);

            Assert.NotNull(user);   
            Assert.Equal(firstName, user.FirstName);
        }




    }
}

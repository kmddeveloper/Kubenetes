using Kubernetes.TransferObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Web;
using Xunit;
using Xunit.Extensions;

namespace xUnitTest
{
    public class CustomData
    {
        public long CreationTime;
        public int Name;
        public int ThreadNum;
    }

    public class UserControlTest: IClassFixture<WebApplicationFactory<Startup>>
    {

        public Func<string, string, string> concat = (x, y) => { return $"{x} {y}"; };

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
              
        
        [Fact]
        public void TestDelegate()
        {
            Debug.WriteLine("this is a test");
            Action<string, string> concatOutput = (x, y) => { Debug.WriteLine($"{x} {y}"); };

            var s1 = test(concat);
               

            var s = concat("test1", "test2");

            concatOutput("test1", "test2");
        }

      


        void CPUKill(object cpuUsage)
        {
            Parallel.For(0, 1, new Action<int>((int i) =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (true)
                {
                    if (watch.ElapsedMilliseconds > (int)cpuUsage)
                    {
                        Thread.Sleep(100 - (int)cpuUsage);
                        watch.Reset();
                        watch.Start();
                    }
                }
            }));

        }

        public string test(Func<string,string, string> combine)
        {
            return combine("test3", "test4");

        }



        [Theory]
        [MemberData(nameof(UserData))]   
        public async Task GetUserByIdTest(int Id, string firstName)
        {

            var input = new User { Id = Id };
            var payload = JsonConvert.SerializeObject(input);

            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

            // Act
            var client = _factory.CreateClient();
            //client.BaseAddress = new Uri("http://localhost:60667");
            var response = await client.PostAsync("/api/user/GetUserPost", content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var user = JsonConvert.DeserializeObject<User>(responseString);

            Assert.NotNull(user);   
            Assert.Equal(firstName, user.FirstName);
        }




    }
}

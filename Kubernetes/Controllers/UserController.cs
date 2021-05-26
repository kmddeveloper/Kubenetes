using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Kubernetes.TransferObjects;
using Kubernetes.Business;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly IUserManager _userManager;
        readonly ITokenManager _tokenManager;

        readonly ILogger<UserController> _logger; 
        public UserController(IUserManager userManager, ITokenManager tokenManager,  ILogger<UserController> logger)
        {
            _userManager = userManager;
            _tokenManager = tokenManager;
            _logger = logger;
        }

        // GET: api/User
        [HttpGet]
        public IEnumerable<string> Get()
        {

            var token = _tokenManager.CreateJWT();

            return new string[] { token.Access_Token };
        }
        // http://localhost/netcore/api/user/5
        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult> Get(int id)
        {
            try
            {


                var context = ControllerContext.HttpContext;
                var isSocketRequest = context.WebSockets.IsWebSocketRequest;

                if (isSocketRequest)
                {
                    using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                    {
                        var user1 = await _userManager.PassFakeUserAysnc(() => { return new User { Id = 1, FirstName = "Pearl", LastName = "Diep" }; });
                        await GetMessages(context, webSocket);
                        return new EmptyResult();
                       
                    }
                   
                }
                else
                {

                    var user1 = await _userManager.PassFakeUserAysnc(() => { return new User { Id = 1, FirstName = "Pearl", LastName = "Diep" }; });
                    var user = await _userManager.GetFakeUserByIdAsync(id);
                    return Ok(user);
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Registration registration)
        {
            _logger.LogWarning($"{DateTime.Now} --  Executing user controller");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            var result =  await _userManager.GetFakeUserByIdAsync(5);
            return Ok(result);

        }



        [HttpGet("spikecpu/{x}", Name = "spikecpu")]
        public ActionResult SpikeCPUPercentage(int x)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            while (true)
            {
                // Make the loop go on for "percentage" milliseconds then sleep the
                // remaining percentage milliseconds. So 40% utilization means work 40ms and sleep 60ms
                if (watch.ElapsedMilliseconds > x)
                {
                    System.Threading.Thread.Sleep(100 - x);
                    watch.Reset();
                    watch.Start();
                }
            }
            return new OkResult();
        }

        [HttpPost("GetUser")]        
        public async Task<ActionResult> GetUser([FromBody] int Id)
        {
            _logger.LogWarning($"{DateTime.Now} --  Executing user controller");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var context = ControllerContext.HttpContext;
            var isSocketRequest = context.WebSockets.IsWebSocketRequest;

            if (isSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await GetMessages(context, webSocket);
                return new OkResult();
            }
            else
            {

                var result = await _userManager.GetFakeUserByIdAsync(5);
                return Ok(result);
            }

        }


        [HttpPost("GetUserPost")]
        public async Task<ActionResult> GetUser([FromBody] User user)
        {
            _logger.LogWarning($"{DateTime.Now} --  Executing user controller");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           
            
           var result = await _userManager.GetFakeUserByIdAsync(5);
           return Ok(result);
           

        }

        private async Task GetMessages(HttpContext context, WebSocket webSocket)
        {
            var messages = new[]
            {
            "Message1",
            "Message2",
            "Message3",
            "Message4",
            "Message5",
            "Message6",
            "Message7"f
        };

            foreach (var message in messages)
            {
                var bytes = Encoding.ASCII.GetBytes(message);
                var arraySegment = new ArraySegment<byte>(bytes);
                await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                Thread.Sleep(2000); //sleeping so that we can see several messages are sent
            }

            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes("DONE")), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

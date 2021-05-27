using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Kubernetes.TransferObjects;
using Kubernetes.Business;
using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Text;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly IUserManager _userManager;
        readonly ILogger _logger;
        public UserController(IUserManager userManager, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // GET: api/User
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result =  await _userManager.GetUserByIdAsync(user.Id);
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
            "Message7",
            "Message8",
            "Message9"
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Transaction.Api.Infrastructure.Data;
using Transaction.Api.Infrastructure.Services;
using Transaction.Api.Models;

namespace Transaction.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly PWTranscationContext _context;
        private readonly IIdentityService _identityService;
        private readonly ILogger<TransactionsController> _logger;

        public UserInfoController(PWTranscationContext context,
            IIdentityService identityService,
            ILogger<TransactionsController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET api/v1/userinfo/find[?username="dem"]
        [Route("find")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserInfoViewModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<UserInfoViewModel>>> FindByUserName([FromQuery]string userName)
        {
            if (userName == null || userName.Length < 2)
                return Ok(new List<UserInfoViewModel>());

            var _userName = userName.ToLower();
            var userid = _identityService.GetUserIdentity();

            var userList = await _context.Users
                .Where(c => c.Id != userid && (c.UserName.ToLower().Contains(_userName) || c.FullName.ToLower().Contains(_userName)))
                .OrderBy(c => c.UserName)
                .Take(30).Select(c => new UserInfoViewModel(c))
                .ToListAsync();

            return Ok(userList);
        }

        // GET api/v1/userinfo/userId
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserInfoViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<UserInfoViewModel>> GetUserInfo(string userId)
        {
            if (userId == null || userId.Length < 2)
                return Ok(new List<UserInfoViewModel>());

            var appUser = await _context.Users.FirstOrDefaultAsync(c => c.Id == userId);
            if (appUser == null)
                return NotFound();
                
            return Ok(new UserInfoViewModel(appUser));
        }

    }
}
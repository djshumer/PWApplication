using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PWApplication.TransactionApi.Extensions;
using PWApplication.TransactionApi.Infrastructure.Data;
using PWApplication.TransactionApi.Infrastructure.Services;
using PWApplication.TransactionApi.Models;

namespace PWApplication.TransactionApi.Controllers
{
    [Route("api/v1/userinfo")]
    [Authorize]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;

        public UserInfoController(IUnitOfWork unitOfWork,
            IIdentityService identityService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
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
            var userId = _identityService.GetUserIdentity();

            var userList = await _unitOfWork.UserInfoRepository.Find(userId, _userName, 30);

            return Ok(userList.ToUserInfoViewModels());
        }

        // GET api/v1/userinfo/userId
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserInfoViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<UserInfoViewModel>> GetUserInfo(string userId)
        {
            if (userId == null || userId.Length < 2)
                return Ok(new List<UserInfoViewModel>());

            var appUser = await _unitOfWork.UserInfoRepository.GetAsync(userId);
            if (appUser == null)
                return NotFound();
                
            return Ok(new UserInfoViewModel(appUser));
        }

    }
}
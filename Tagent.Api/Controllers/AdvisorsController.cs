using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Tagent.Api.DTOs.Advisor;
using Tagent.Bussiness.Define;
using Tagent.Domain.Entities;

namespace Tagent.Api.Controllers
{
    [Authorize(Roles = "advisor")]
    [Route("api/advisors")]
    [ApiController]
    public class AdvisorsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IAccountRoleService _accountRoleService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly IAdvisorService _advisorSerivce;

        public AdvisorsController(IAccountRoleService accountRoleService, IAccountService accountService, IRoleService roleService, IMapper mapper, IAdvisorService advisorService)
        {
            _accountRoleService = accountRoleService;
            _accountService = accountService;
            _roleService = roleService;
            _mapper = mapper;
            _advisorSerivce = advisorService;
        }
        #region Get All User Account
        [HttpGet("users")]
        public IActionResult GetAllUserAccount(int numpage = 1, int perpage = 10)
        {
            int userid = _roleService.Get(a => a.Name.Equals("user"), null).Id;
            var listuser = _accountRoleService.GetAll(a => a.RoleId == userid, numpage, perpage, null).ToList();
            List<UserAccountResponse> listuseraccounts = new List<UserAccountResponse>();
            foreach (var user in listuser)
            {
                var useraccount = _mapper.Map<UserAccountResponse>(_accountService.Get(a => a.Id == user.AccountId, null));
                listuseraccounts.Add(useraccount);
            }
            if (listuseraccounts != null)
            {
                return StatusCode(200, new { listuser = listuseraccounts });
            }

            return StatusCode(404, new { message = "Not found" });
        }
        #endregion

        #region Get User Account by Id
        [HttpGet("user/{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _accountService.Get(a => a.Id == id, null);
            if (user != null)
            {
                var resource = _mapper.Map<AdvisorResponse>(user);
                return StatusCode(200, new { userdetail = resource });
            }
            return StatusCode(404, new { message = "Not found" });
        }
        #endregion

        #region Update User to Customer
        [HttpPut("{id}")]
        public IActionResult UpdateAccount(int id)
        {
            var userrole = _accountRoleService.Get(a => a.AccountId == id, null);
            userrole.RoleId = _roleService.Get(a => a.Name.Equals("customer"), null).Id;

            _accountRoleService.Update(userrole);
            return StatusCode(200, new { message = "Update Success" });
        }
        #endregion

        #region View profile
        [HttpGet("profile/{id}")]
        public IActionResult ViewProfile(int id)
        {
            var user = _accountService.Get(a => a.Id == id, null);
            var advisor = _advisorSerivce.Get(a => a.AccountId == id, null);
            if (user != null && advisor != null)
            {
                var resource = _mapper.Map<AdvisorProfileView>(user);
                return StatusCode(200, new { userdetail = resource });
            }
            return StatusCode(404, new { message = "Not found" });
        }
        #endregion

        #region Edit Profile
        [HttpPut("profile")]
        public IActionResult EditProfile([FromBody] AdvisorRequest advisorRequest)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400, "Invaild Adivor");
            }
            var account = _mapper.Map<Account>(advisorRequest);
            _accountService.Update(account);

            return StatusCode(200, new { message = "Update Success" });
        }
        #endregion

        #region Change Password
        [HttpPut("password")]
        public IActionResult UpdatePassword([FromBody] UpdatePasswordRequest updatePasswordRequest)
        {
            var account = _accountService.Get(a => a.Id == updatePasswordRequest.Id, null);
            if (account != null)
            {
                account.Password = Encode(updatePasswordRequest.Password);
                _accountService.Update(account);
                return StatusCode(200, new { message = "Update Success" });
            }
            return StatusCode(404, new { message = "Not found your id" });
        }
        #endregion

        #region Update Image
        [HttpPut("image")]
        public IActionResult UpdateImage([FromBody] UpdateImageRequest updateImageRequest)
        {
            var account = _accountService.Get(a => a.Id == updateImageRequest.Id, null);
            if (account != null)
            {
                account.Password = updateImageRequest.Image;
                _accountService.Update(account);
                return StatusCode(200, new { message = "Update Success" });
            }
            return StatusCode(404, new { message = "Not found your id" });
        }
        #endregion

        #region EnCode Base64
        private string Encode(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
        #endregion
    }
}

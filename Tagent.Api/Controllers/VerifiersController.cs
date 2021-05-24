using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tagent.Api.DTOs.Advisor;
using Tagent.Api.DTOs.Verifier;
using Tagent.Bussiness.Define;
using Tagent.Domain.Entities;

namespace Tagent.Api.Controllers
{
    [Authorize(Roles = "verifier")]
    [Route("api/verifiers")]
    [ApiController]
    public class VerifiersController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IAccountRoleService _accountRoleService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly IVerifierService _verifierSerivce;

        public VerifiersController(IAccountRoleService accountRoleService, IAccountService accountService, IRoleService roleService, IMapper mapper, IVerifierService verifierSerivce)
        {
            _accountRoleService = accountRoleService;
            _accountService = accountService;
            _roleService = roleService;
            _mapper = mapper;
            _verifierSerivce = verifierSerivce;
        }
        #region View profile
        [HttpGet("profile/{id}")]
        public IActionResult ViewProfile(int id)
        {
            var user = _accountService.Get(a => a.Id == id ,null);
            var advisor = _verifierSerivce.Get(a => a.AccountId == id, null);
            if (user != null && advisor != null)
            {
                var resource = _mapper.Map<VerifierProfileResponse>(user);
                return StatusCode(200, new { userdetail = resource });
            }
            return StatusCode(404, new { message = "Not found" });
        }
        #endregion

        #region Edit Profile
        [HttpPut("profile")]
        public IActionResult EditProfile([FromBody] VerifierProfileRequest advisorRequest)
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

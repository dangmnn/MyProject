using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Tagent.Api.DTOs.Account;
using Tagent.Api.DTOs.Account.Advisor;
using Tagent.Api.DTOs.Account.Agency;
using Tagent.Api.DTOs.Account.Create;
using Tagent.Api.DTOs.Account.Customer;
using Tagent.Api.DTOs.Account.Verifier;
using Tagent.Bussiness.Define;
using Tagent.Domain.Entities;
using Tagent.EmailService;
using Tagent.EmailService.Define;
using Tagent.LoggerService;

namespace Tagent.Api.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        #region Initial
        private readonly IAccountService _accountservice;
        private readonly IRoleService _roleservice;
        private readonly IAccountRoleService _accountroleservice;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IAdvisorService _advisorService;
        private readonly IAgencyService _agencyService;
        private readonly IVerifierService _verifierService;
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly ICompanyService _companyService;
        private readonly ICustomerService _customerService;
        #endregion

        #region Contructor
        public AccountsController(IAccountService accountservice, IRoleService roleservice, IAccountRoleService acountroleservice, IConfiguration config, IMapper mapper, IAdvisorService advisorService, IAgencyService agencyService, IVerifierService verifierService, IUserService userService, IEmailSender emailSender, ICompanyService companyService, ICustomerService customerService)
        {
            this._accountservice = accountservice;
            this._roleservice = roleservice;
            this._accountroleservice = acountroleservice;
            this._config = config;
            this._mapper = mapper;
            this._advisorService = advisorService;
            this._agencyService = agencyService;
            this._verifierService = verifierService;
            this._userService = userService;
            this._emailSender = emailSender;
            this._companyService = companyService;
            this._customerService = customerService;
        }
        #endregion

        #region Login
        #region Login with email

        [AllowAnonymous]
        [HttpPost("authenticate/email")]
        public IActionResult LoginWithEmail([FromBody] AccountRequest accountRequest)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticaUser(accountRequest);
            if (user != null)
            {
                var tokenStr = GenerateJSONWebToken(user);
                response = Ok(new { token =tokenStr, id= user.Id, role = user.Role });
            }
            else
            {
                return StatusCode(400, new { message = "Wrong email or password" });
            }
            return response;
        }
        #endregion

        #region Login with gg
        [AllowAnonymous]
        [HttpPost("authenticate/google")]
        public IActionResult LoginWithGG(string idToken)
        {
            string[] include = { "AccountRoles" };
            if (VerifyToken(idToken))
            {
                var user = EnCodeJwt(idToken);
                if (Exited(user.Email))
                {
                    var account = _accountservice.Get(a => a.Email.Equals(user.Email), include);
                    List<string> rolename = new List<string>();
                    foreach(var role in account.AccountRoles)
                    {
                        var name = _roleservice.Get(a => a.Id == role.RoleId, null).Name;
                        rolename.Add(name);
                    }
                    var accountresponse = _mapper.Map<AccountResponse>(user);
                    string token = GenerateJSONWebToken(accountresponse);
                    return StatusCode(200, new { token = token, firstname = GenerateFirstnameAndLastname(user.Name).Fistname.Trim(), lastname = GenerateFirstnameAndLastname(user.Name).Lastname.Trim(), picture = user.Picture, id = account.Id, role = rolename.ToList() });
                }
                else
                {
                    Account account = new Account();
                    account.Email = user.Email;
                    account.Password = "";
                    account.Firstname = GenerateFirstnameAndLastname(user.Name).Fistname.Trim();
                    account.Lastname = GenerateFirstnameAndLastname(user.Name).Lastname.Trim();
                    account.Image = user.Picture;
                    _accountservice.Create(account);

                    CreateRoleForAccount(user.Email, "user");

                    var accountresponse = _mapper.Map<AccountResponse>(user);
                    accountresponse.Role.Add(user.Role);
                    var accountget = _accountservice.Get(a => a.Email.Equals(user.Email), include);
                    List<string> rolename = new List<string>();
                    foreach (var role in accountget.AccountRoles)
                    {
                        var name = _roleservice.Get(a => a.Id == role.RoleId, null).Name;
                        rolename.Add(name);
                    }
                    string token = GenerateJSONWebToken(accountresponse);
                    return StatusCode(200, new { token = token, firstname = GenerateFirstnameAndLastname(user.Name).Fistname.Trim(), lastname = GenerateFirstnameAndLastname(user.Name).Lastname.Trim(), picture = user.Picture, id = accountget.Id, role = rolename.ToList() });
                }
            }
            else
            {
                return BadRequest("Verify Id Failed");
            }
        }
        #endregion
        #endregion

        #region Create
        #region Create User Account
        [AllowAnonymous]
        [HttpPost("registration/user")]
        public IActionResult CreaeteUserAccount([FromBody] CreateAccountRequest accUserRequest)
        {
            if (Exited(accUserRequest.Email))
            {
                return StatusCode(400, new { message = "Account already existed" });
            }

            var account = _mapper.Map<Account>(accUserRequest);
            account.Password = Encode(account.Password);
            _accountservice.Create(account);

            CreateRoleForAccount(account.Email, "user");

            var accountget = _accountservice.Get(a => a.Email.Equals(account.Email), null);
            User user = new User();
            user.AccountId = accountget.Id;
            _userService.Create(user);

            var roleid = _accountroleservice.Get(a => a.AccountId == accountget.Id, null).RoleId;
            var role = _roleservice.Get(a => a.Id == roleid, null).Name;
            var accountreponse = _mapper.Map<AccountResponse>(accountget);
            accountreponse.Role.Add(role);
            string tokenStr = GenerateJSONWebToken(accountreponse);

            return StatusCode(200, new { token = tokenStr, id = accountget.Id, role = accountreponse.Role });
        }
        #endregion

        #region Create Advisor Account
        [Authorize(Roles = "admin")]
        [HttpPost("registration/advisor")]
        public IActionResult CreateAdvisorAccount([FromBody] AdvisorAccountRequest accAdvisorRequest)
        {
            if (Exited(accAdvisorRequest.Email))
            {
                return StatusCode(400, new { message = "This email already existed" });
            }

            var account = _mapper.Map<Account>(accAdvisorRequest);
            account.Password = Encode(account.Password);
            _accountservice.Create(account);

            var advisor = _mapper.Map<Advisor>(accAdvisorRequest);
            advisor.AccountId = _accountservice.Get(a => a.Email.Equals(account.Email), null).Id;
            _advisorService.Create(advisor);

            CreateRoleForAccount(account.Email, "advisor");

            var accountget = _accountservice.Get(a => a.Email.Equals(account.Email), null);


            var roleid = _accountroleservice.Get(a => a.AccountId == accountget.Id, null).RoleId;
            var role = _roleservice.Get(a => a.Id == roleid, null).Name;
            var accountreponse = _mapper.Map<AccountResponse>(accountget);
            accountreponse.Role.Add(role);
            string tokenStr = GenerateJSONWebToken(accountreponse);

            return StatusCode(200, new { token = tokenStr, id = accountget.Id, role = accountreponse.Role });
        }
        #endregion

        #region Create Verifier Account
        [Authorize(Roles = "admin")]
        [HttpPost("registration/verifier")]
        public IActionResult CreateVerifierAccount([FromBody] VerifierAccountRequest accVerifierRequest)
        {
            if (Exited(accVerifierRequest.Email))
            {
                return StatusCode(400, new { message = "Account already existed" });
            }

            var account = _mapper.Map<Account>(accVerifierRequest);
            account.Password = Encode(account.Password);
            _accountservice.Create(account);

            var verifier = _mapper.Map<Verifier>(accVerifierRequest);
            verifier.AccountId = _accountservice.Get(a => a.Email.Equals(account.Email),null).Id;
            _verifierService.Create(verifier);

            CreateRoleForAccount(account.Email, "verifier");

            var accountget = _accountservice.Get(a => a.Email.Equals(account.Email), null);
            var roleid = _accountroleservice.Get(a => a.AccountId == accountget.Id, null).RoleId;
            var role = _roleservice.Get(a => a.Id == roleid, null).Name;
            var accountreponse = _mapper.Map<AccountResponse>(accountget);
            accountreponse.Role.Add(role);
            string tokenStr = GenerateJSONWebToken(accountreponse);

            return StatusCode(200, new { token = tokenStr, id = accountget.Id, role = accountreponse.Role });
        }
        #endregion

        #region Create Agency Account
        [Authorize(Roles ="admin")]
        [HttpPost("registration/agency")]
        public IActionResult CreateAgencyAccount([FromBody] AgencyAccountRequest accAgencyRequest)
        {
            if (Exited(accAgencyRequest.Email))
            {
                return StatusCode(400, new { message = "Account already existed" });
            }
            Company company = new Company();
            company.Name = accAgencyRequest.CompanyName;
            _companyService.Create(company);

            var companyid = _companyService.Get(a => a.Name.Equals(accAgencyRequest.CompanyName), null).Id;

            var account = _mapper.Map<Account>(accAgencyRequest);
            account.Password = Encode(account.Password);
            _accountservice.Create(account);

            var agency = _mapper.Map<Agency>(accAgencyRequest);
            agency.AccountId = _accountservice.Get(a => a.Email.Equals(account.Email), null).Id;
            agency.CompanyId = companyid;
            _agencyService.Create(agency);

            CreateRoleForAccount(account.Email, "agency");

            var accountget = _accountservice.Get(a => a.Email.Equals(account.Email), null);
            var roleid = _accountroleservice.Get(a => a.AccountId == accountget.Id, null).RoleId;
            var role = _roleservice.Get(a => a.Id == roleid, null).Name;
            var accountreponse = _mapper.Map<AccountResponse>(accountget);
            accountreponse.Role.Add(role);
            string tokenStr = GenerateJSONWebToken(accountreponse);

            return StatusCode(200, new { token = tokenStr, id = accountget.Id, role = accountreponse.Role });
        }
        #endregion
        #endregion

        #region Get All
        #region Get All Advisor
        [Authorize(Roles ="admin")]
        [HttpGet("advisors")]
        public IActionResult GetAllAdvisor(int numpage = 1, int perpage = 10)
        {
            string[] inclue = { "AccountRoles" };
            var listadvisor = _advisorService.GetAll(null, numpage, perpage, null).ToList();
            if (listadvisor.Count > 0)
            {
                List<AdvisorAccountResponse> listadvisorinfor = new List<AdvisorAccountResponse>();
                foreach (var advisor in listadvisor)
                {
                    AdvisorAccountResponse advisorAccountResponse = new AdvisorAccountResponse();
                    var advisoracc = _accountservice.Get(a => a.Id == advisor.AccountId , inclue);
                    advisorAccountResponse.Id = advisor.Id;
                    advisorAccountResponse.CreateDate = advisoracc.AccountRoles.FirstOrDefault().CreateDate;
                    advisorAccountResponse.Email = advisoracc.Email;
                    advisorAccountResponse.Name = advisoracc.Firstname + " " + advisoracc.Lastname;
                    listadvisorinfor.Add(advisorAccountResponse);
                }

                var totaladvisor = _advisorService.GetAll(null, 0, 0, null).ToList().Count();
                var totalpage = Totalpagenumber(perpage, totaladvisor);
                return StatusCode(200, new { listadvisor = listadvisorinfor, total = totalpage });
            }
            else
            {
                return StatusCode(404, new { message = "Advisors is empty" });
            }
        }
        #endregion

        #region Get All Verifier
        [Authorize(Roles = "admin")]
        [HttpGet("verifiers")]
        public IActionResult GetAllVerifier(int numpage = 1, int perpage = 10)
        {
            var listverifier = _verifierService.GetAll(null, numpage, perpage, null).ToList();
            if (listverifier.Count > 0)
            {
                string[] inclue = { "AccountRoles" };
                List<VerifierAccountResponse> listverifierinfor = new List<VerifierAccountResponse>();
                foreach (var verifier in listverifier)
                {
                    VerifierAccountResponse verifierAccountResponse = new VerifierAccountResponse();
                    var verifieracc = _accountservice.Get(a => a.Id == verifier.AccountId, inclue);
                    verifierAccountResponse.Id = verifier.Id;
                    verifierAccountResponse.Name = verifieracc.Firstname + " " + verifieracc.Lastname;
                    verifierAccountResponse.Email = verifieracc.Email;
                    verifierAccountResponse.CreateDate = verifieracc.AccountRoles.FirstOrDefault().CreateDate;

                    listverifierinfor.Add(verifierAccountResponse);
                }

                var totalverifier = _verifierService.GetAll(null, 0, 0, null).ToList().Count();
                var totalpage = Totalpagenumber(perpage, totalverifier);
                return StatusCode(200, new { listverifier = listverifierinfor, total = totalpage });
            }
            else
            {
                return StatusCode(404, new { message = "Verifiers are empty" });
            }
        }
        #endregion

        #region Get All Company
        [Authorize(Roles = "admin")]
        [HttpGet("companies")]
        public IActionResult GetAllCompany(int numpage = 1, int perpage = 0)
        {
            var listcompany = _companyService.GetAll(null, numpage, perpage, null).ToList();
            if (listcompany.Count > 0)
            {
                var totalverifier = _verifierService.GetAll(null, 0, 0, null).ToList().Count();
                var totalpage = Totalpagenumber(perpage, totalverifier);
                return StatusCode(200, new { listcompanies = listcompany, total = totalpage });
            }
            else
            {
                return StatusCode(404, new { message = "Companies are empty" });
            }
        }
        #endregion

        #region Get All Agency
        [Authorize(Roles = "admin")]
        [HttpGet("agencies")]
        public IActionResult GetAllAgency(int numpage = 1, int perpage = 10)
        {
            string[] inclueaccount = { "AccountRoles" };
            string[] incluecompany = { "Company" };
            var listagency = _agencyService.GetAll(null, numpage, perpage, incluecompany).ToList();
            if (listagency.Count > 0)
            {
                List<AgencyAccountResponse> listagencyinfor = new List<AgencyAccountResponse>();
                foreach (var agency in listagency)
                {
                    AgencyAccountResponse agencyAccountResponse = new AgencyAccountResponse();
                    var agencyacc = _accountservice.Get(a => a.Id == agency.AccountId, inclueaccount);
                    agencyAccountResponse.Id = agency.Id;
                    agencyAccountResponse.Name = agencyacc.Firstname + " " + agencyacc.Lastname;
                    agencyAccountResponse.Email = agencyacc.Email;
                    agencyAccountResponse.CompanyName = agency.Company.Name;
                    agencyAccountResponse.CreateTime = agencyacc.AccountRoles.FirstOrDefault().CreateDate;

                    listagencyinfor.Add(agencyAccountResponse);
                }

                var totaladvisor = _advisorService.GetAll(null, 0, 0, null).ToList().Count();
                var totalpage = Totalpagenumber(perpage, totaladvisor);
                return StatusCode(200, new { listadvisor = listagencyinfor, total = totalpage });
            }
            else
            {
                return StatusCode(404, new { message = "Agencies is empty" });
            }
        }
        #endregion

        #region Get All Customers
        [Authorize(Roles = "admin")]
        [HttpGet("customers")]
        public IActionResult GetAllCustomers(int numpage = 1, int perpage = 10)
        {
            string[] inclueaccount = { "AccountRoles" };
            string[] incluecompany = { "Company" };
            var listcustomer = _customerService.GetAll(null, numpage, perpage, incluecompany).ToList();
            if (listcustomer.Count > 0)
            {
                List<CustomerAccountResponse> listcustomerinfor = new List<CustomerAccountResponse>();
                foreach (var customer in listcustomer)
                {
                    CustomerAccountResponse customerAccountResponse = new CustomerAccountResponse();
                    var customeracc = _accountservice.Get(a => a.Id == customer.AccountId, inclueaccount);
                    customerAccountResponse.Id = customer.Id;
                    customerAccountResponse.Name = customeracc.Firstname + " " + customeracc.Lastname;
                    customerAccountResponse.Email = customeracc.Email;
                    customerAccountResponse.CompanyName = customer.Company.Name;
                    customerAccountResponse.CreateTime = customeracc.AccountRoles.FirstOrDefault().CreateDate;

                    listcustomerinfor.Add(customerAccountResponse);
                }

                var totaladvisor = _advisorService.GetAll(null, 0, 0, null).ToList().Count();
                var totalpage = Totalpagenumber(perpage, totaladvisor);
                return StatusCode(200, new { listadvisor = listcustomerinfor, total = totalpage });
            }
            else
            {
                return StatusCode(404, new { message = "Compaies is empty" });
            }
        }
        #endregion

        #endregion

        #region Get By Id

        #region Advisor
        [HttpGet("advisor/{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult GetAdvisorById(int id)
        {
            string[] includeaccount = { "Account" };
            string[] includerole = { "AccountRoles" };
            var advisor = _advisorService.Get(a => a.Id == id, includeaccount);
            if(advisor != null)
            {
                var advisoraccount = _accountservice.Get(a => a.Id == advisor.AccountId, includerole);
                var advisorInforReponse = _mapper.Map<AdvisorInforReponse>(advisoraccount);
                advisorInforReponse.Age = advisor.Age;
                advisorInforReponse.Gender = advisor.Gender;
                advisorInforReponse.Image = advisoraccount.Image;
                advisorInforReponse.CreateDate = advisoraccount.AccountRoles.FirstOrDefault().CreateDate;
                advisorInforReponse.UpdateDate = advisoraccount.AccountRoles.FirstOrDefault().UpdateDate;
                return StatusCode(200,advisorInforReponse);
            }
            else
            {
                return StatusCode(404, "Not found your advisor");
            }
        }
        #endregion

        #region Verifier
        [Authorize(Roles = "admin")]
        [HttpGet("verifier/{id}")]
        public IActionResult GetVerifierById(int id)
        {
            string[] includeaccount = { "Account" };
            string[] includerole = { "AccountRoles" };
            var verifier = _verifierService.Get(a => a.Id == id, includeaccount);
            if(verifier != null)
            {
                var verifieraccount = _accountservice.Get(a => a.Id == verifier.AccountId, includerole);
                var verifierInforReponse = _mapper.Map<VerifierInforResponse>(verifieraccount);
                verifierInforReponse.Age = verifier.Age;
                verifierInforReponse.Gender = verifier.Gender;
                verifierInforReponse.Image = verifieraccount.Image;
                verifierInforReponse.CreateDate = verifieraccount.AccountRoles.FirstOrDefault().CreateDate;
                verifierInforReponse.UpdateDate = verifieraccount.AccountRoles.FirstOrDefault().UpdateDate;
                return StatusCode(200,verifierInforReponse);
            }
            else
            {
                return StatusCode(404, "Not found your verifier");
            }
        }
        #endregion

        #region Agency
        [Authorize(Roles = "admin")]
        [HttpGet("agency/{id}")]
        public IActionResult GetAgencyById(int id)
        {
            var agency = _agencyService.Get(a => a.Id == id, null);
            if(agency != null)
            {
                var agencyaccount = _accountservice.Get(a => a.Id == agency.AccountId, null);
                var agencycompany = _companyService.Get(a => a.Id == agency.CompanyId, null);
                AgencyInforResponse agencyInforResponse = new AgencyInforResponse();
                agencyInforResponse = _mapper.Map<AgencyInforResponse>(agencycompany);
                agencyInforResponse.CompanyEmail = agencycompany.Email;
                agencyInforResponse.CompanyPhone = agencycompany.Phone;
                agencyInforResponse.CompanyName = agencycompany.Name;
                agencyInforResponse.Email = agencyaccount.Email;
                agencyInforResponse.Firstname = agencyaccount.Firstname;
                agencyInforResponse.Lastname = agencyaccount.Lastname;
                agencyInforResponse.Phone = agencyaccount.Phone;
                agencyInforResponse.Image = agencyaccount.Image;
                return StatusCode(200,agencyInforResponse);
            }
            else
            {
                return StatusCode(404, "Not found your agency");
            }
        }
        #endregion

        #region Customer
        [Authorize(Roles = "admin")]
        [HttpGet("customer/{id}")]
        public IActionResult GetCustomerById(int id)
        {
            var customer = _customerService.Get(a => a.Id == id, null);
            if(customer != null)
            {
                var customeraccount = _accountservice.Get(a => a.Id == customer.AccountId, null);
                var customercompany = _companyService.Get(a => a.Id == customer.CompanyId, null);
                CustomerInforResponse customerInforResponse = new CustomerInforResponse();
                customerInforResponse = _mapper.Map<CustomerInforResponse>(customercompany);
                customerInforResponse.CompanyEmail = customercompany.Email;
                customerInforResponse.CompanyPhone = customercompany.Phone;
                customerInforResponse.CompanyName = customercompany.Name;
                customerInforResponse.Email = customeraccount.Email;
                customerInforResponse.Firstname = customeraccount.Firstname;
                customerInforResponse.Lastname = customeraccount.Lastname;
                customerInforResponse.Phone = customeraccount.Phone;
                customerInforResponse.Image = customeraccount.Image;
                return StatusCode(200,customerInforResponse);
            }
           else
            {
                return StatusCode(404, "Not found your customer");
            }
        }
        #endregion

        #region Company
        #endregion

        #endregion

        #region Admin
        #region View profile
        [Authorize(Roles = "admin")]
        [HttpGet("admin/profile/{id}")]
        public IActionResult ViewProfile(int id)
        {
            var user = _accountservice.Get(a => a.Id == id, null);
            var advisor = _accountservice.Get(a => a.Id == id, null);
            if (user != null && advisor != null)
            {
                var resource = _mapper.Map<Account>(user);
                return StatusCode(200, new { userdetail = resource });
            }
            return StatusCode(404, new { message = "Not found" });
        }
        #endregion

        #region Edit Profile
        [Authorize(Roles = "admin")]
        [HttpPut("admin/profile")]
        public IActionResult EditProfile([FromBody] CreateAccountRequest advisorRequest)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400, "Invaild Adivor");
            }
            var account = _mapper.Map<Account>(advisorRequest);
            _accountservice.Update(account);

            return StatusCode(200, new { message = "Update Success" });
        }
        #endregion

        #region Change Password
        [Authorize(Roles = "admin")]
        [HttpPut("admin/password")]
        public IActionResult UpdatePassword([FromBody] CreateAccountRequest updatePasswordRequest)
        {
            var account = _accountservice.Get(a => a.Id == updatePasswordRequest.Id, null);
            if (account != null)
            {
                account.Password = updatePasswordRequest.Password;
                _accountservice.Update(account);
                return StatusCode(200, new { message = "Update Success" });
            }
            return StatusCode(404, new { message = "Not found your id" });
        }
        #endregion

        #region Update Image
        [Authorize(Roles = "admin")]
        [HttpPut("admin/image")]
        public IActionResult UpdateImage([FromBody] CreateAccountRequest updateImageRequest)
        {
            var account = _accountservice.Get(a => a.Id == updateImageRequest.Id, null);
            if (account != null)
            {
                account.Password = updateImageRequest.Image;
                _accountservice.Update(account);
                return StatusCode(200, new { message = "Update Success" });
            }
            return StatusCode(404, new { message = "Not found your id" });
        }
        #endregion
        #endregion

        #region ForgetPassword
        [HttpPost("email")]
        public IActionResult Getemail(string email)
        {
            if(Exited(email))
            {
                var account = _accountservice.Get(a => a.Email.Equals(email), null);
                if(account.Password.Equals(""))
                {
                    return StatusCode(400, new { message = "Invalid Email" });
                }
                var accountresponse = _mapper.Map<AccountResponse>(account);
                var tokenstr = GenerateJSONWebToken(accountresponse);
                var resource = SendMail(tokenstr);
                if (resource.Equals("Success"))
                {
                    return StatusCode(200, new { message = "Please check your email for password reset instructions" });
                }
                else
                {
                    return StatusCode(500, resource);
                }
            }
            else
            {
                return StatusCode(404, "Not found your email");
            }
        }
        [HttpPut("password")]
        public IActionResult UpdatePassword(string token, string password)
        {
            var handler = new JwtSecurityTokenHandler();
            var json = handler.ReadJwtToken(token).Claims.ToList();
            var email = json.Where(x => x.Type.Contains("email")).FirstOrDefault().Value;
            var account = _accountservice.Get(a => a.Email.Equals(email), null);
            if(account == null)
            {
                return StatusCode(400, "Invalid Token");
            }
            account.Password = Encode(password);

            _accountservice.Update(account);

            return StatusCode(200, "Update Success");
        }

        private string SendMail(string token)
        {
            try
            {

                var handler = new JwtSecurityTokenHandler();
                var json = handler.ReadJwtToken(token).Claims.ToList();
                var email = json.Where(x => x.Type.Contains("email")).FirstOrDefault().Value;
                List<string> listemail = new List<string>();
                listemail.Add(email);
                string link = "http://localhost:3000/reset-password/"+token;
                var message = new Message(listemail, "Tagent: Reset Your Password", "<!DOCTYPE html><html><body>Dear " + email + " <br/> This is your link to Update your password: <br/>" + "<a href=\""+link+"\">Update Password</a></body></html> ");
                _emailSender.SendEmail(message);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region function helper
        #region Check Account Exited
        private bool Exited(string email)
        {
            var account = _accountservice.Get(a => a.Email.Equals(email), null);
            if (account != null)
            {
                return true;//exited email
            }

            return false;//not exited email
        }
        #endregion

        #region AuthenticaUser
        private AccountResponse AuthenticaUser(AccountRequest accountResquest)
        {
            string[] include = { "AccountRoles" };
            AccountResponse user = new AccountResponse();
            var accountchecked = _accountservice.Get(a => a.Email.Equals(accountResquest.Email), include);
            if (accountchecked == null || !Decode(accountchecked.Password).Equals(accountResquest.Password) || accountchecked.AccountRoles.FirstOrDefault().Status.Equals("disable"))
            {
                return null;
            }
            user = _mapper.Map<AccountResponse>(_accountservice.Get(a => a.Email.Equals(accountResquest.Email),null));
            var listidrole = _accountroleservice.GetAll(a => a.AccountId == user.Id,0,0,null).ToList();
            foreach(var idrole in listidrole)
            {
                string rolename = _roleservice.Get(a => a.Id == idrole.RoleId, null).Name;
                user.Role.Add(rolename);
            }
            return user;
        }
        #endregion

        #region Generate token
        private string GenerateJSONWebToken(AccountResponse userinfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim("email", userinfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach(var role in userinfo.Role)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            

            var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Issuer"],
                    claims,
                    expires: DateTime.Now.AddHours(15),
                    signingCredentials: credentials
                );

            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }
        #endregion

        #region Encode token to object
        private AccountGGReponse EnCodeJwt(string _token)
        {
            var handler = new JwtSecurityTokenHandler();
            var json = handler.ReadJwtToken(_token).Claims.ToList();
            AccountGGReponse accountGGReponse = new AccountGGReponse();
            accountGGReponse.Name = json.Where(x => x.Type.Contains("name")).FirstOrDefault().Value;
            accountGGReponse.Email = json.Where(x => x.Type.Contains("email")).FirstOrDefault().Value;
            accountGGReponse.Role = "superguest";
            accountGGReponse.Picture = json.Where(x => x.Type.Contains("picture")).FirstOrDefault().Value;
            return accountGGReponse;
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

        #region Decode Base64
        private string Decode(string hashcode)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(hashcode);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
        #endregion

        #region VerifyToken
        private bool VerifyToken(string token)
        {
            var auth = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance;

            var response = auth.VerifyIdTokenAsync(token);
            if (response != null)
                return true;

            return false;
        }
        #endregion

        #region Split string
        private Name GenerateFirstnameAndLastname(string name)
        {
            string[] arr = name.Split(' ');
            Name resouce = new Name();
            resouce.Lastname = arr[arr.Length - 1];
            for (int i = 0; i < arr.Length - 1; i++)
            {
                resouce.Fistname += arr[i] + " ";
            }

            return resouce;
        }

        #endregion

        #region Create Role for Account
        private void CreateRoleForAccount(string email, string role)
        {
            AccountRole accountRole = new AccountRole();
            accountRole.AccountId = _accountservice.Get(a => a.Email.Equals(email), null).Id;
            accountRole.RoleId = _roleservice.Get(a => a.Name.Equals(role),null).Id;
            accountRole.Status = "enable";
            accountRole.CreateDate = DateTime.Now;
            _accountroleservice.Create(accountRole);
        }
        #endregion

        #region Total page number
        private int Totalpagenumber(int perpage, int total)
        {
            int resource = total / perpage;
            if(total % perpage != 0)
                resource += 1;
            return resource;
        }
        #endregion

        #endregion
    }

    class Name
    {
        public string Fistname { get; set; }
        public string Lastname { get; set; }
    }
}

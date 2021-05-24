using AutoMapper;
using Tagent.Api.DTOs.Account;
using Tagent.Api.DTOs.Account.Advisor;
using Tagent.Api.DTOs.Account.Agency;
using Tagent.Api.DTOs.Account.Create;
using Tagent.Api.DTOs.Account.Customer;
using Tagent.Api.DTOs.Account.Verifier;
using Tagent.Api.DTOs.Advisor;
using Tagent.Api.DTOs.Customer;
using Tagent.Api.DTOs.User;
using Tagent.Domain.Entities;

namespace Tagent.Api.Helpers
{
    public class TagentProfile : Profile
    {
        public TagentProfile()
        {
            CreateMap<AccountRequest, Account>();
            CreateMap<Account, AccountRequest>();

            CreateMap<AccountResponse, Account>();
            CreateMap<Account, AccountResponse>();

            CreateMap<Account, CreateAccountRequest>();
            CreateMap<CreateAccountRequest, Account>();

            CreateMap<AccountResponse, AccountGGReponse>();
            CreateMap<AccountGGReponse, AccountResponse>();

            CreateMap<Account, AccountGGReponse>();
            CreateMap<AccountGGReponse, Account>();

            CreateMap<Advisor, AdvisorAccountRequest>();
            CreateMap<AdvisorAccountRequest, Advisor>();

            CreateMap<Verifier, VerifierAccountRequest>();
            CreateMap<VerifierAccountRequest, Verifier>();

            CreateMap<Agency, AgencyAccountRequest>();
            CreateMap<AgencyAccountRequest, Agency>();

            CreateMap<Account, UserAccountResponse>();
            CreateMap<UserAccountResponse, Account>();

            CreateMap<Account, AdvisorResponse>();
            CreateMap<AdvisorResponse, Account>();

            CreateMap<Account, AdvisorRequest>();
            CreateMap<AdvisorRequest, Account>();

            CreateMap<CustomerProfileRequest, Account>();
            CreateMap<Account, CustomerProfileRequest>();

            CreateMap<CustomerProfileResponse, Account>();
            CreateMap<Account, CustomerProfileResponse>();

            CreateMap<UserProfileRequest, Account>();
            CreateMap<Account, UserProfileRequest>();

            CreateMap<UserProfileResponse, Account>();
            CreateMap<Account, UserProfileResponse>();

            CreateMap<AdvisorAccountRequest, Account>();
            CreateMap<Account, AdvisorAccountRequest>();

            CreateMap<Account, VerifierAccountRequest>();
            CreateMap<VerifierAccountRequest, Account>();

            CreateMap<AdvisorAccountResponse, Advisor>();
            CreateMap<Advisor, AdvisorAccountResponse>();

            CreateMap<AdvisorAccountResponse, Account>();
            CreateMap<Account, AdvisorAccountResponse>();

            CreateMap<VerifierAccountResponse, Account>();
            CreateMap<Account, VerifierAccountResponse>();

            CreateMap<AgencyAccountRequest, Account>();
            CreateMap<Account, AgencyAccountRequest>();

            CreateMap<Advisor, AdvisorInforReponse>();
            CreateMap<AdvisorInforReponse, Advisor>();

            CreateMap<Account, AdvisorInforReponse>();
            CreateMap<AdvisorInforReponse, Account>();

            CreateMap<Verifier, VerifierInforResponse>();
            CreateMap<VerifierInforResponse, Verifier>();

            CreateMap<Account, VerifierInforResponse>();
            CreateMap<VerifierInforResponse, Account>();

            CreateMap<Account, AgencyInforResponse>();
            CreateMap<AgencyInforResponse, Account>();

            CreateMap<Company, AgencyInforResponse>();
            CreateMap<AgencyInforResponse, Company>();

            CreateMap<Account, CustomerInforResponse>();
            CreateMap<CustomerInforResponse, Account>();

            CreateMap<Company, CustomerInforResponse>();
            CreateMap<CustomerInforResponse, Company>();
        }
    }
}

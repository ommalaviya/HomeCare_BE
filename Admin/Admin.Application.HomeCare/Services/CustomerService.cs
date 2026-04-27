using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.Customer;
using Admin.Domain.HomeCare.DataModels.Response.Customer;
using Admin.Domain.HomeCare.Interface;
using AutoMapper;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;
using System.Security.Claims;

namespace Admin.Application.HomeCare.Services
{
    public class CustomerService : GenericService<User>, ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(
            ICustomerRepository customerRepository,
            ClaimsPrincipal principal,
            IUnitOfWork unitOfWork,
            IMapper mapper)
            : base(customerRepository, unitOfWork, mapper, principal)
        {
            _customerRepository = customerRepository;
        }

        public async Task<FilteredDataQueryResponseModel<GetCustomerResponseModel>> GetAllCustomersAsync(
            FilterCustomerRequestModel filter)
        {
            var result = await _customerRepository.GetCustomersWithBookingCountAsync(filter);

            return new FilteredDataQueryResponseModel<GetCustomerResponseModel>
            {
                TotalRecords = result.TotalRecords,
                Records = result.Records,
                FilterMeta = result.FilterMeta
            };
        }

        public async Task<GetCustomerResponseModel> CreateCustomerAsync(
            CreateCustomerRequestModel request)
        {
            await ThrowIfDuplicateAsync(
                x => x.Email == request.Email && !x.IsDeleted,
                string.Format(Messages.DuplicateRecord, Messages.User));

            var entity = ToEntity(request);
            await AddAsync(entity);

            return ToResponseModel<GetCustomerResponseModel>(entity);
        }

        public async Task<bool> UpdateStatusAsync(int id)
        {
            var entity = await FindOrThrowAsync(
                x => x.Id == id && !x.IsDeleted,
                string.Format(Messages.NotFound, Messages.User));

            var isBlocking = entity.Status == "Active";
            entity.Status = isBlocking ? "Block" : "Active";
            await UpdateAsync(entity);

            if (isBlocking)
            {
                await _customerRepository.CancelActiveBookingsByUserIdAsync(
                    id,
                    Messages.BookingsCancelledOnBlock);
            }

            return true;
        }

        public async Task<bool> SoftDeleteCustomerAsync(int id)
        {
            var entity = await FindOrThrowAsync(
                x => x.Id == id && !x.IsDeleted,
                string.Format(Messages.NotFound, Messages.User));

            await _customerRepository.CancelActiveBookingsByUserIdAsync(
                id,
                Messages.BookingsCancelledOnDelete);

            await SoftDeleteAsync(entity);
            return true;
        }

        public async Task<CustomerDetailResponse> GetCustomerDetailAsync(int customerId)
        {
            var result = await _customerRepository.GetCustomerDetailAsync(customerId);

            if (result is null)
                throw new KeyNotFoundException(string.Format(Messages.NotFound, Messages.User));

            return result;
        }
    }
}
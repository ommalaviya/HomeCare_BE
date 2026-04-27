using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.Transaction;
using Admin.Domain.HomeCare.DataModels.Response.Transaction;
using Admin.Domain.HomeCare.Interface;
using AutoMapper;
using Shared.HomeCare.DataModel.Request;
using Shared.Extensions;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Admin.Application.HomeCare.Services
{
    public class TransactionService(
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal)
        : GenericService<Transaction>(transactionRepository, unitOfWork, mapper, principal),
          ITransactionService
    {
        public async Task<FilteredDataQueryResponseModel<GetTransactionResponseModel>> GetAllTransactionsAsync(
            FilterTransactionRequestModel? filter = null)
        {
            filter ??= new FilterTransactionRequestModel();

            decimal realMaxAmount = await transactionRepository.MaxAsync(
                x => x.TransactionAmount);

            Expression<Func<Transaction, bool>> expression = x => !x.IsDeleted;

            if (filter.MinAmount.HasValue)
                expression = expression.Add(x => x.TransactionAmount >= filter.MinAmount.Value);

            if (filter.MaxAmount.HasValue)
                expression = expression.Add(x => x.TransactionAmount <= filter.MaxAmount.Value);

            if (!string.IsNullOrWhiteSpace(filter.PaymentMethod) &&
                Enum.TryParse<PaymentMethod>(filter.PaymentMethod, ignoreCase: true, out var paymentMethod))
            {
                expression = expression.Add(x => x.PaymentMethod == paymentMethod);
            }

            var includer = new ExpressionIncluder<Transaction>
            {
                Includes = [x => x.User, x => x.Service]
            };

            bool sortByUserName = string.Equals(
                filter.SortField, "UserName", StringComparison.OrdinalIgnoreCase);

            if (sortByUserName)
            {
                var fetchFilter = new FilterTransactionRequestModel
                {
                    MinAmount     = filter.MinAmount,
                    MaxAmount     = filter.MaxAmount,
                    PaymentMethod = filter.PaymentMethod,
                    SortField     = null,
                    SortDirection = null,
                    PageNumber    = 0,
                    PageSize      = 0
                };

                var all    = await GetAllAsync(predicate: expression, model: fetchFilter, includer: includer);
                var mapped = ToResponseModel<IEnumerable<GetTransactionResponseModel>>(all.Records).ToList();

                var sorted = string.Equals(filter.SortDirection, "desc", StringComparison.OrdinalIgnoreCase)
                    ? mapped.OrderByDescending(x => x.UserName).ToList()
                    : mapped.OrderBy(x => x.UserName).ToList();

                var pageSize   = filter.PageSize   > 0 ? filter.PageSize   : 10;
                var pageNumber = filter.PageNumber  > 0 ? filter.PageNumber : 1;

                return new FilteredDataQueryResponseModel<GetTransactionResponseModel>
                {
                    TotalRecords = all.TotalRecords,
                    Records      = sorted.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
                    FilterMeta   = new FilterRangeMeta { MaxAmount = realMaxAmount }
                };
            }

            var response = await GetAllAsync(predicate: expression, model: filter, includer: includer);

            return new FilteredDataQueryResponseModel<GetTransactionResponseModel>
            {
                TotalRecords = response.TotalRecords,
                Records = ToResponseModel<IEnumerable<GetTransactionResponseModel>>(response.Records),
                FilterMeta = new FilterRangeMeta
                {
                    MaxAmount = realMaxAmount
                }
            };
        }

        public async Task<TransactionDetailResponseModel?> GetTransactionDetailAsync(int id)
        {
            var includer = new ExpressionIncluder<Transaction>
            {
                Includes = [x => x.User, x => x.Service]
            };

            var transaction = await transactionRepository.FindDataAsync(
                predicate: x => x.Id == id && !x.IsDeleted,
                includer: includer);

            if (transaction is null)
                return null;

            var result = mapper.Map<TransactionDetailResponseModel>(transaction);

            return result;
        }

        public async Task<FilteredDataQueryResponseModel<GetUserTransactionResponseModel>> GetTransactionsByUserIdAsync(int userId, PageRequest pageRequest)
        {
            var includer = new ExpressionIncluder<Transaction>
            {
                Includes = [x => x.User, x => x.Service]
            };

            var response = await GetAllAsync(
                predicate: x => x.UserId == userId && !x.IsDeleted,
                model: pageRequest,
                includer: includer);

            return new FilteredDataQueryResponseModel<GetUserTransactionResponseModel>
            {
                TotalRecords = response.TotalRecords,
                Records = ToResponseModel<IEnumerable<GetUserTransactionResponseModel>>(response.Records)
            };
        }

        public async Task<bool> softDelteTransactionAsync(int id)
        {
            var entity = await FindOrThrowAsync(
                x => x.Id == id && !x.IsDeleted,
                string.Format(Messages.NotFound, Messages.Transaction));

            await SoftDeleteAsync(entity);
            return true;
        }
    }
}
using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using Shared.HomeCare.DataModel.Request;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Exceptions;
using Shared.HomeCare.Extensions;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.Interfaces.Services;

namespace Shared.HomeCare.Services;

public class GenericService<TEntity>(
    IGenericRepository<TEntity> genericRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ClaimsPrincipal principal) : IGenericService<TEntity> where TEntity : class
{
    public int CurrentUserId => principal.GetUserId();
    protected IUnitOfWork UnitOfWork => unitOfWork;

    public Task<DataQueryResponse<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate,
        PageRequest? model = null,
        ExpressionIncluder<TEntity>? includer = null)
        => genericRepository.GetAllAsync(predicate, model, includer);

    public async Task<TEntity?> GetByIdAsync(int id)
        => await genericRepository.GetByIdAsync(id);

    public async Task<TEntity> GetOrThrowAsync(int id, string notFoundMessage)
    {
        var entity = await genericRepository.GetByIdAsync(id);

        if (entity is null || (entity is BaseEntity b && b.IsDeleted))
            throw new KeyNotFoundException(notFoundMessage);

        return entity;
    }

    public TResponse ToResponseModel<TResponse>(dynamic obj) where TResponse : class
        => mapper.Map<TResponse>(obj);

    public TDest Map<TDest, TSource>(TSource source) where TDest : class
        => mapper.Map<TDest>(source);

    public TDest Map<TDest, TSource>(TSource source, TDest destination) where TDest : class
        => mapper.Map(source, destination);

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        if (entity is BaseEntity baseEntity)
        {
            baseEntity.CreatedAt = DateTime.UtcNow;
            baseEntity.CreatedBy = CurrentUserId;
        }

        await genericRepository.AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
        return entity;
    }

    public virtual async Task AddRangeForAsync(IEnumerable<TEntity> entities)
    {
        var list = entities.ToList();

        // Auto-set CreatedBy for all entities
        foreach (var entity in list)
            if (entity is BaseEntity baseEntity)
                baseEntity.CreatedBy = CurrentUserId;

        await genericRepository.AddRangeAsync(list);
        await unitOfWork.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        if (entity is BaseEntity baseEntity)
        {
            baseEntity.ModifiedAt = DateTime.UtcNow;
            baseEntity.ModifiedBy = CurrentUserId;
        }

        await genericRepository.UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public virtual async Task SoftDeleteAsync(TEntity entity)
    {
        if (entity is BaseEntity baseEntity)
        {
            baseEntity.IsDeleted = true;
            baseEntity.ModifiedAt = DateTime.UtcNow;
            baseEntity.ModifiedBy = CurrentUserId;
        }

        await genericRepository.UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public Task<bool> CheckDuplicateAsync(Expression<Func<TEntity, bool>> predicate)
        => genericRepository.CheckDuplicateAsync(predicate);

    public async Task ThrowIfDuplicateAsync(
        Expression<Func<TEntity, bool>> predicate,
        string duplicateMessage)
    {
        if (await genericRepository.CheckDuplicateAsync(predicate))
            throw new DuplicateRecordException(duplicateMessage);
    }

    public async Task<TEntity> FindOrThrowAsync(
         Expression<Func<TEntity, bool>> predicate,
    string notFoundMessage,
         ExpressionIncluder<TEntity>? includer = null)
    {
        var entity = await genericRepository.FindDataAsync(predicate, includer);

        if (entity is null)
            throw new KeyNotFoundException(notFoundMessage);

        return entity;
    }

    public virtual async Task AddRangeAsync<TChildEntity>(
          IGenericRepository<TChildEntity> repository,
          IEnumerable<TChildEntity> entities) where TChildEntity : BaseEntity
    {
        var list = entities.ToList();

        foreach (var entity in list)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = CurrentUserId;
        }

        await repository.AddRangeAsync(list);
        await unitOfWork.SaveChangesAsync();
    }

    public virtual async Task RemoveRangeAsync<TChildEntity>(
        IGenericRepository<TChildEntity> repository,
        IEnumerable<TChildEntity> entities) where TChildEntity : class
    {
        await repository.RemoveRangeAsync(entities);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null){
        return await genericRepository.CountAsync(predicate);
    }


    public TEntity ToEntity(dynamic obj)
        => mapper.Map<TEntity>(obj);
    protected List<TResponse> MapToList<TResponse>(IEnumerable<object> source)
        where TResponse : class
        => mapper.Map<List<TResponse>>(source);
}

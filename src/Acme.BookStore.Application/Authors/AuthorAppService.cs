using Acme.BookStore.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.OpenIddict;
using System.Linq;
using static Acme.BookStore.Permissions.BookStorePermissions;
using AutoMapper;
using Acme.BookStore.Books;
                
using System.Linq.Dynamic.Core; 
using Microsoft.EntityFrameworkCore;        
 

namespace Acme.BookStore.Authors;

[Authorize(BookStorePermissions.Authors.Default)]
public class AuthorAppService : BookStoreAppService, IAuthorAppService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly AuthorManager _authorManager;

    public AuthorAppService(
        IAuthorRepository authorRepository,
        AuthorManager authorManager)
    {
        _authorRepository = authorRepository;
        _authorManager = authorManager;
    }

    public async Task<AuthorDto> GetAsync(Guid id)
    {
        var author = await _authorRepository.GetAsync(id);
        return ObjectMapper.Map<Author, AuthorDto>(author);
    }

    public async Task<PagedResultDto<AuthorDto>> GetListAsync(GetAuthorListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(Author.Name);
        }
        var queryable = await _authorRepository.GetQueryableAsync();
        var query = from author in queryable select new { author };
        query = query.Skip(input.SkipCount).Take(input.MaxResultCount);
        var queryResult = await AsyncExecuter.ToListAsync(query);

        var authorDtos = queryResult.Select(x =>
            {
                var authorDto = ObjectMapper.Map<Author, AuthorDto>(x.author);
                authorDto.Name = x.author.Name;
                return authorDto;
            }).ToList();
        var totalCount = await _authorRepository.GetCountAsync();
        return new PagedResultDto<AuthorDto>(
          totalCount,
           authorDtos
      );
    }

    public async Task<PagedResultDto<AuthorDto>> GetPagedListAsync(GetAuthorListDto input)
    {


        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(Author.Name);
        }

        var AuthorQueryable = await _authorRepository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.Filter))
            AuthorQueryable = AuthorQueryable.Where(i => i.Name.ToLower().Contains(input.Filter.ToLower()) || i.Name.ToString() == input.Filter || i.ShortBio.ToLower().Contains(input.Filter.ToLower()));

        if (input.AuthorSearch != null)
        {
            if (!string.IsNullOrEmpty(input.AuthorSearch.Name))
            {
                AuthorQueryable = AuthorQueryable.Where(i => i.Name.ToLower().Contains(input.AuthorSearch.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(input.AuthorSearch.ShortBio))
            {
                AuthorQueryable = AuthorQueryable.Where(i => i.ShortBio.ToLower().Contains(input.AuthorSearch.ShortBio.ToLower()));
            }
            if (!string.IsNullOrEmpty(input.AuthorSearch.Sex))
            {
                AuthorQueryable = AuthorQueryable.Where(i => i.Sex.ToLower()==input.AuthorSearch.Sex.ToLower());
            }
            if (!string.IsNullOrEmpty(input.AuthorSearch.BirthDate.ToString()))
            {
                AuthorQueryable = AuthorQueryable.Where(i => i.BirthDate == input.AuthorSearch.BirthDate);
            }
        }


        var result = await AuthorQueryable
            .OrderBy(input.Sorting)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToListAsync();

        var totalCount = await AuthorQueryable.CountAsync();

        return new PagedResultDto<AuthorDto>(
            totalCount,
            ObjectMapper.Map<List<Author>, List<AuthorDto>>(result)
        );
    }

    [Authorize(BookStorePermissions.Authors.Create)]
    public async Task<AuthorDto> CreateAsync(CreateAuthorDto input)
    {
        var author = await _authorManager.CreateAsync(
            input.Name,
            input.BirthDate,
            input.Sex   ,
            input.ShortBio 
        );

        await _authorRepository.InsertAsync(author);

        return ObjectMapper.Map<Author, AuthorDto>(author);
    }

    [Authorize(BookStorePermissions.Authors.Edit)]
    public async Task UpdateAsync(Guid id, UpdateAuthorDto input)
    {
        var author = await _authorRepository.GetAsync(id);

        if (author.Name != input.Name)
        {
            await _authorManager.ChangeNameAsync(author, input.Name);
        }

        author.BirthDate = input.BirthDate;
        author.ShortBio = input.ShortBio;

        await _authorRepository.UpdateAsync(author);
    }

    [Authorize(BookStorePermissions.Authors.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _authorRepository.DeleteAsync(id);
    }
}
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
        var query = from author in queryable    
                    select new { author };
        switch (input.Sorting)
        {
            case "Id": 
                query = query.OrderBy(x=>x.author.Id).Skip(input.SkipCount).Take(input.MaxResultCount);
                break;
            case "Name":
                query = query.OrderBy(x => x.author.Name).Skip(input.SkipCount).Take(input.MaxResultCount);
                break;
            case "BirthDate":
                query = query.OrderBy(x => x.author.BirthDate).Skip(input.SkipCount).Take(input.MaxResultCount);
                break;
            case "ShortBio":
                query = query.OrderBy(x => x.author.ShortBio).Skip(input.SkipCount).Take(input.MaxResultCount);
                break;   
        }
       
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

    [Authorize(BookStorePermissions.Authors.Create)]
    public async Task<AuthorDto> CreateAsync(CreateAuthorDto input)
    {
        var author = await _authorManager.CreateAsync(
            input.Name,
            input.BirthDate,
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
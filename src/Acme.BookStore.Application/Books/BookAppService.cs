using Acme.BookStore.Authors;
using Acme.BookStore.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.Books;

[Authorize(BookStorePermissions.Books.Default)]
public class BookAppService :
    CrudAppService<
        Book, //The Book entity
        BookDto, //Used to show books
        Guid, //Primary key of the book entity
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CreateUpdateBookDto>, //Used to create/update a book
    IBookAppService //implement the IBookAppService
{
    private readonly IAuthorRepository _authorRepository;

    public BookAppService(
        IRepository<Book, Guid> repository,
        IAuthorRepository authorRepository)
        : base(repository)
    {
        _authorRepository = authorRepository;
        GetPolicyName = BookStorePermissions.Books.Default;
        GetListPolicyName = BookStorePermissions.Books.Default;
        CreatePolicyName = BookStorePermissions.Books.Create;
        UpdatePolicyName = BookStorePermissions.Books.Edit;
        DeletePolicyName = BookStorePermissions.Books.Create;
    }

    public override async Task<BookDto> GetAsync(Guid id)
    {
        //Get the IQueryable<Book> from the repository
        var queryable = await Repository.GetQueryableAsync();

        //Prepare a query to join books and authors
        var query = from book in queryable
                    join author in await _authorRepository.GetQueryableAsync() on book.AuthorId equals author.Id
                    where book.Id == id
                    select new { book, author };

        //Execute the query and get the book with author
        var queryResult = await AsyncExecuter.FirstOrDefaultAsync(query);
        if (queryResult == null)
        {
            throw new EntityNotFoundException(typeof(Book), id);
        }

        var bookDto = ObjectMapper.Map<Book, BookDto>(queryResult.book);
        bookDto.AuthorName = queryResult.author.Name;
        return bookDto;
    }

    public override async Task<PagedResultDto<BookDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        //Get the IQueryable<Book> from the repository
        var queryable = await Repository.GetQueryableAsync();

        //Prepare a query to join books and authors
        var query = from book in queryable
                    join author in await _authorRepository.GetQueryableAsync()
                    on book.AuthorId equals author.Id
                    select new { book, author };

        //Paging
        query = query
            .OrderBy(NormalizeSorting(input.Sorting))
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        //Execute the query and get a list
        var queryResult = await AsyncExecuter.ToListAsync(query);

        //Convert the query result to a list of BookDto objects
        var bookDtos = queryResult.Select(x =>
        {
            var bookDto = ObjectMapper.Map<Book, BookDto>(x.book);
            bookDto.AuthorName = x.author.Name;
            bookDto.Birthdate = x.author.BirthDate;
            bookDto.Sex = x.author.Sex;
            return bookDto;
        }).ToList();

        //Get the total count with another query
        var totalCount = await Repository.GetCountAsync();

        return new PagedResultDto<BookDto>(
            totalCount,
            bookDtos
        );
    }
    public async Task<PagedResultDto<BookDto>> GetAuthorsbookListAsync(GetAuthorListDto input)
    {
        //Get the IQueryable<Book> from the repository
        var queryable = await Repository.GetQueryableAsync();

        //Prepare a query to join books and authors
        var query = from book in queryable
                    join author in await _authorRepository.GetQueryableAsync()
                    on book.AuthorId equals author.Id
                    select new { book, author };

        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(Author.Name);
        }

        if (!string.IsNullOrEmpty(input.Filter))
            query = query.Where(i => i.author.Name.ToLower().Contains(input.Filter.ToLower()) || i.author.Sex.ToString() == input.Filter || i.book.Price.ToString() == input.Filter || i.book.Name.ToString() == input.Filter || i.author.BirthDate.ToString() == input.Filter);

        if (input.AuthorSearch != null)
        {
            if (!string.IsNullOrEmpty(input.AuthorSearch.AuthorName))
            {
                query = query.Where(i => i.author.Name.ToLower().Contains(input.AuthorSearch.AuthorName.ToLower()));
            }

            if (!string.IsNullOrEmpty(input.AuthorSearch.Sex))
            {
                query = query.Where(i => i.author.Sex.ToLower() == input.AuthorSearch.Sex.ToLower());
            }
            if (input.AuthorSearch != null && input.AuthorSearch.Birthdate != null && input.AuthorSearch.Birthdate != DateTime.MinValue)
            {
                query = query.Where(i => i.author.BirthDate == input.AuthorSearch.Birthdate);
            }


            if (!string.IsNullOrEmpty(input.AuthorSearch.BookName))
            {
                query = query.Where(i => i.book.Name.ToLower().Contains(input.AuthorSearch.BookName.ToLower()));
            }
            if (input.AuthorSearch?.Price != 0)
            {
                query = query.Where(i => i.book.Price == input.AuthorSearch.Price);
            }
        }
        var total = query;

        //Paging
        query = query
            .OrderBy(NormalizeSorting(input.Sorting))
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        //Execute the query and get a list
        var queryResult = await AsyncExecuter.ToListAsync(query);

        //Convert the query result to a list of BookDto objects
        var bookDtos = queryResult.Select(x =>
        {
            var bookDto = ObjectMapper.Map<Book, BookDto>(x.book);
            bookDto.AuthorName = x.author.Name;
            bookDto.Sex = x.author.Sex;
            bookDto.Birthdate = x.author.BirthDate;
            return bookDto;
        }).ToList();

        var totalCount = total.Count();
        //Get the total count with another query

        return new PagedResultDto<BookDto>(
            totalCount,
            bookDtos
        );
    }
    public async Task<List<string>> GetBookList()
    {
        var queryable = await Repository.GetQueryableAsync();

        //Prepare a query to join books and authors
        var query = from book in queryable
                    select book.Name;
        var queryResult = await AsyncExecuter.ToListAsync(query);
        return queryResult;

    }
    public async Task<ListResultDto<AuthorLookupDto>> GetAuthorLookupAsync()
    {
        var authors = await _authorRepository.GetListAsync();

        return new ListResultDto<AuthorLookupDto>(
            ObjectMapper.Map<List<Author>, List<AuthorLookupDto>>(authors)
        );
    }

    private static string NormalizeSorting(string sorting)
    {
        if (sorting.IsNullOrEmpty())
        {
            return $"book.{nameof(Book.Name)}";
        }

        if (sorting.Contains("authorName", StringComparison.OrdinalIgnoreCase))
        {
            return sorting.Replace(
                "authorName",
                "author.Name",
                StringComparison.OrdinalIgnoreCase
            );
        }

        return $"book.{sorting}";
    }
}

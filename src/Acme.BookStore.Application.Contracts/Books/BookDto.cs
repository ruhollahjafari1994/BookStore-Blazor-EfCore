using System;
using Volo.Abp.Application.Dtos;
using System.Linq;
using System.Collections.Generic;

namespace Acme.BookStore.Books;

[Serializable]
public class BookDto : AuditedEntityDto<Guid>
{
    public Guid AuthorId { get; set; }

    public string AuthorName { get; set; }
    public DateTime Birthdate { get; set; }
    public string Sex { get; set; }
    public string Name { get; set; }

    public BookType Type { get; set; }

    public DateTime PublishDate { get; set; }

    public float Price { get; set; }
}
public class AuthorBooksDto
{
    public Guid AuthorId { get; set; }

    public string AuthorName { get; set; }
    public List<BookDto> Books { get; set; }
}
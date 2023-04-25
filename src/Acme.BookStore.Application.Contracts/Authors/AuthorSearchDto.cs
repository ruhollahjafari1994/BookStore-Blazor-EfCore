using Acme.BookStore.Books;
using System;

namespace Acme.BookStore.Authors;

public class AuthorSearchDto
{
    public string BookName { get; set; }
                 
    public string AuthorName { get; set; }
    public DateTime Birthdate { get; set; }
    public string Sex { get; set; }
 
    public BookType Type { get; set; }

    public DateTime PublishDate { get; set; }

    public float Price { get; set; }
}

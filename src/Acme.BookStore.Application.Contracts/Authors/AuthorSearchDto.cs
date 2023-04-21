using System;

namespace Acme.BookStore.Authors;

public class AuthorSearchDto
{
    public string Name { get; set; }

    public DateTime BirthDate { get; set; }

    public string ShortBio { get; set; }
}

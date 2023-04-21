using System;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Authors;

[Serializable]
public class GetAuthorListDto : PagedAndSortedResultRequestDto
{
    public AuthorSearchDto AuthorSearch { get; set; }

    public string Filter { get; set; }
}

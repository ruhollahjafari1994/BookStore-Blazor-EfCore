using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Authors;

[Serializable]
public class AuthorDto : EntityDto<Guid>
{
    public string Name { get; set; }

    public DateTime BirthDate { get; set; }

    public string ShortBio { get; set; }
    public string Sex { get; set; }
}
public enum SexStatusEnum
{
    [Display(Name = "All")]
    All = -1,

    [Display(Name = "Select An Option")]
    SelectAnOption = 0,

    [Display(Name = "Female")]
    Female = 1,

    [Display(Name = "Male")]
    Male = 2,
}
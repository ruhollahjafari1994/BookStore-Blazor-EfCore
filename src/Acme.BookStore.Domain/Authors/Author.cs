using JetBrains.Annotations;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.BookStore.Authors;

public class Author : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public DateTime BirthDate { get; set; }
    public string ShortBio { get; set; }
    public string Sex { get; set; }

    private Author()
    {
        /* This constructor is for deserialization / ORM purpose */
    }

    internal Author(
        Guid id,
        [NotNull] string name,
        DateTime birthDate,
        string sex,
        [CanBeNull] string shortBio = null
        )
        : base(id)
    {
        SetName(name);
        BirthDate = birthDate;
        ShortBio = shortBio;
        Sex = sex;
    }

    internal Author ChangeName([NotNull] string name)
    {
        SetName(name);
        return this;
    }

    private void SetName([NotNull] string name)
    {
        Name = Check.NotNullOrWhiteSpace(
            name,
            nameof(name),
            maxLength: AuthorConsts.MaxNameLength
        );
    }
}

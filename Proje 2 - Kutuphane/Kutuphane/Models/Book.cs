using System;
using System.Collections.Generic;

namespace Kutuphane.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public int PublishYear { get; set; }

    public int AuthorId { get; set; }

    public virtual Author? Author { get; set; } = null!;

    public virtual ICollection<Borrower>? Borrowers { get; set; } = new List<Borrower>();
}

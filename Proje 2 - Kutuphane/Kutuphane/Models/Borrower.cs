using System;
using System.Collections.Generic;

namespace Kutuphane.Models;

public partial class Borrower
{
    public int BorrowerId { get; set; }

    public string FullName { get; set; } = null!;

    public DateTime BorrowDate { get; set; }

    public int BookId { get; set; }

    public virtual Book? Book { get; set; } = null!;
}

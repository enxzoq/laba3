using System;
using System.Collections.Generic;

namespace Lab3.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string? Position { get; set; }

    public string? Education { get; set; }

    public virtual ICollection<ServiceContract> ServiceContracts { get; set; } = new List<ServiceContract>();
}

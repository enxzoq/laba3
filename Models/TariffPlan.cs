using System;
using System.Collections.Generic;

namespace Lab3.Models;

public partial class TariffPlan
{
    public string Name { get; set; } = null!;

    public decimal? SubscriptionFee { get; set; }

    public decimal? LocalCallCost { get; set; }

    public decimal? LongDistanceCallCost { get; set; }

    public decimal? InternationalCallCost { get; set; }

    public string? BillingType { get; set; }

    public decimal? Smscost { get; set; }

    public decimal? Mmscost { get; set; }

    public decimal? DataTransferCost { get; set; }

    public virtual ICollection<ServiceContract> ServiceContracts { get; set; } = new List<ServiceContract>();
}

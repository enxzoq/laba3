using System;
using System.Collections.Generic;

namespace Lab3.Models;

public partial class ServiceContract
{
    public int ContractId { get; set; }

    public int? SubscriberId { get; set; }

    public DateOnly? ContractDate { get; set; }

    public string? TariffPlanName { get; set; }

    public string? PhoneNumber { get; set; }

    public int? EmployeeId { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<ServiceStatistic> ServiceStatistics { get; set; } = new List<ServiceStatistic>();

    public virtual Subscriber? Subscriber { get; set; }

    public virtual TariffPlan? TariffPlanNameNavigation { get; set; }
}

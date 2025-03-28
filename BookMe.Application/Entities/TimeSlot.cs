using System;

namespace BookMe.Application.Entities;

public class TimeSlot : BaseEntity
{
  public DateTimeOffset Start { get; set; }
  public DateTimeOffset End { get; set; }
  public Booking? Booking { get; set; }
}

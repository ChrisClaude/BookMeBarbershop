using System;

namespace BookMe.Application.Entities;

public class TimeSlot
{
  public Guid Id { get; set; }
  public DateTimeOffset Start { get; set; }
  public DateTimeOffset End { get; set; }
  public Booking? Booking { get; set; }
}

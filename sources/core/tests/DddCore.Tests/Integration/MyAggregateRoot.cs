using System.ComponentModel.DataAnnotations;
using Ggio.DddCore;

namespace DddCore.Tests.Integration;

public class MyAggregateRoot : EntityBase<Guid>, IAggregateRoot
{
     [Key]
     [MaxLength(250)]
     public required string Name { get; set; }

     public void MakeWonderful()
     {
          RegisterDomainEvent(new WonderfulEvent());
          Name = "Wonderful";
     }
}

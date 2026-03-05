using Facet;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;

namespace Ggio.BikeSherpa.Backend.Model;

[Facet(typeof(Address), GenerateToSource = true)]
public partial record AddressCrud;

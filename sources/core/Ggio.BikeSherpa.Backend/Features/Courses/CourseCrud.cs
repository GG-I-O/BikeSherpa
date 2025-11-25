using Facet;
using Ggio.BikeSherpa.Backend.Domain.CourseAggregate;

namespace Ggio.BikeSherpa.BackendSaaS.Features.Courses;

[Facet(typeof(Course))]
public partial record CourseCrud;

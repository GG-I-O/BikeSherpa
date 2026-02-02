using BackendTests.Services;

namespace BackendTests.Features.Customers.Delete;

public abstract class DeleteCustomerWebApplicationFactory() : TestWebApplicationFactory("write:customers", "write:customers");
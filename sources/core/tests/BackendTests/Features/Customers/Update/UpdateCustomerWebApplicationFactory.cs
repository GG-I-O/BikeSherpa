using BackendTests.Services;

namespace BackendTests.Features.Customers.Update;

public abstract class UpdateCustomerWebApplicationFactory() : TestWebApplicationFactory("write:customers", "write:customers");
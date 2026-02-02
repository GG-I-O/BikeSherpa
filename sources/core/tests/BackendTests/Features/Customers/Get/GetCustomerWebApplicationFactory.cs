using BackendTests.Services;

namespace BackendTests.Features.Customers.Get;

public abstract class GetCustomerWebApplicationFactory() : TestWebApplicationFactory("read:customers", "read:customers");
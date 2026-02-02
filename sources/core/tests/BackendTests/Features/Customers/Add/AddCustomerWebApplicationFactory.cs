using BackendTests.Services;

namespace BackendTests.Features.Customers.Add;

public class AddCustomerWebApplicationFactory() : TestWebApplicationFactory("write:customers", "write:customers");
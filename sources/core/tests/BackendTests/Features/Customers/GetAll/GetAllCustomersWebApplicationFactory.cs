using BackendTests.Services;

namespace BackendTests.Features.Customers.GetAll;

public abstract class GetAllCustomersWebApplicationFactory() : TestWebApplicationFactory("read:customers", "read:customers");
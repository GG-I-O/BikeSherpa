using Microsoft.AspNetCore.Authentication;

namespace BackendTests.Services;

public class TestAuthSchemeOptions : AuthenticationSchemeOptions
{
     public string? Scope { get; set; }
}
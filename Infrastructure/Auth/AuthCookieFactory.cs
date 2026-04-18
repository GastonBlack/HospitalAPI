namespace HospitalAPI.Infrastructure.Auth;

public class AuthCookieFactory
{
    // //////////////////////////////////////////
    // Inyections
    // //////////////////////////////////////////
    private readonly IWebHostEnvironment _environment;
    public AuthCookieFactory(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    // //////////////////////////////////////////
    // Cookie
    // //////////////////////////////////////////
    public CookieOptions Create(DateTime? expires = null)
    {
        bool isDevelopment = _environment.IsDevelopment();

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDevelopment,
            SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.None,
            Expires = expires
        };
    }
}

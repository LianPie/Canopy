using Microsoft.AspNetCore.Http;
using System;

public static class CookieHelper
{
    public static void Set(HttpResponse response,
                           string key,
                           string value,
                           int expiresDays = 30,
                           bool httpOnly = true,
                           bool secure = true)
    {
        var options = new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(expiresDays),
            HttpOnly = httpOnly,
            Secure = secure,
            SameSite = SameSiteMode.Strict
        };
        response.Cookies.Append(key, value, options);
    }

    public static string? Get(HttpRequest request, string key)
    {
        return request.Cookies.TryGetValue(key, out var val) ? val : null;
    }

    public static void Delete(HttpResponse response, string key)
    {
        response.Cookies.Delete(key);
    }
}
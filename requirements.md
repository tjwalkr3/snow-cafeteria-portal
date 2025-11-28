## Backend Authentication

**Overview**
I have already implemented authentication in the Management App. There is a Keycloak server that the frontend interacts with to sign in and the signin info is stored in a cookie. You can explore the configuration of the Keycloak server by looking inside of AppRealm.json. 

**Steps**
1. Create a folder called Services inside of the Management project. 
3. Create a service inside of this folder called HttpClientAuth. It should be a wrapper around a normal HTTP client that adds an authentication header to normal HTTP requests. This should be a skin and wrap of the HttpClient, but the wrapper should add an authentication header. There should be methods for Get, Post, Put, Delete and Patch. Each of these methods should be generic, so that it can receive the exact same arguments as the normal HttpClient methods. Add tests for these methods to the UnitTests project (in /Cafeteria.UnitTests/Management/Services), and test whether the Authentication Header is really being added in the case of each of the methods. 
4. Add an authentication middleware to the API similar to the code in the code block following this steps section. The Authority and the Audience should be pulled from configurations (instead of being hard-coded like they are in the example). These configurations should be added as environment variables in the API's Aspire deployment. If any other information is needed to validate the JWT, modify the jwtOptions to read more info from configs, and add the info in the AppHost. 
5. Create a new controller in the API called LocationController. This should contain a single endpoint called /location/authenticated. This endpoint should just return a JSON object of the form { username: "Test User" } if the user is logged in. If the user is not logged in, this endpoint should return a 401 error. This controller (and by extension, its endpoint) should be the only ones in the app using the authentication middleware. The MenuController should not use authentication. 

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(jwtOptions =>
{
  jwtOptions.Authority = "https://{--your-authority--}";
  jwtOptions.Audience = "https://{--your-audience--}";
});
```

**Rules:**
* Do not write unnecessary markdown documentation or unnecessary comments. 
* Don't do more than I tell you to. 
* Make sure tests that you write are actually testing behavior of the code, and not aimlessly testing a mock. 
* Write your tests before you write your implementations, as this is good practice (TDD). 
* If you need any more information about implementing backend-auth, you may get it [here](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-jwt-bearer-authentication?view=aspnetcore-9.0)

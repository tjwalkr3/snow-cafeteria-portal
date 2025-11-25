## Authentication in the management app

**Contextual information:**
* There is an Aspire deployment that spins up all of the necessary containers to get the app to work. A Keycloak container is spun up when this Aspire deployment turns on. 
* When the Keycloak container turns on, it creates a default realm and client using the configuration info in AppRealm.json. This file also contains redirectUris, webOrigins, and postLogoutRedirectUris that refer to the containers in the Aspire deployment. 
* The System.IdentityModel.Tokens.Jwt and Microsoft.AspNetCore.Authentication.OpenIdConnect packages have been added to the Cafeteria.Management project to handle authentication. 
* An instance of the authentication was injected into the program.cs of Cafeteria.Management, and the AddOpenIdConnect method was called. It is being configured with a sections from the configurations (this is being passed in as environment variables in the Aspire AppHost). 

**Goals:**
* I want to create a simple component that contains nothing but a button. 
* It should be created in its own folder inside of the Components/Layout folder. It's folder should have a file called SigninButton.razor, a codebehind file called SigninButton.razor.cs (inheriting from ComponentBase), and a scoped css file called SigninButton.razor.cs. 
* I want the text inside of the signin button to be set based on the authentication state. 
  * When logged out, the text will be "Sign In".
  * When logged in, the text will be "Sign Out [UserName]" where the username is the username retrieved from the appropriate claim. 
* When I am logged out, and I click the signin button, I want it to redirect me to the Keycloak signin page, so that I can sign into the app. After I sign in with the correct credentials, Keycloak will automatically redirect me back into the app with the JWT, and the signin button should be updated to the logged in state. 
  * When redirecting back from the Keycloak instance after sign in, the "state", "session_state", "iss", and "code" url parameters should be stripped from the URL to avoid issues if I return to the signin page. 
* When I am logged in, and I click the signout button, it will redirect me to the keycloak logout page, and then instantly back into the app, and the signin button should be updated to the logged out state. 

**Rules:**
* Do not create unnecessary documentation files or comments. 
* Don't add excessive try/catch blocks. 
* Code should be easy to understand and short. This is meant to be a minimum viable product. 
* Try to keep as much of the functionality (of not all of it) inside of this signin button component. 
# CraftExercise_CoreMVC_Sample
Craft Exercise integrating Square and QBO using QBO DotNetCore MVC5 Sample App

# PreRequisites
Visual Studio 2019 or above
Microsoft.Net.Compilers 2.10.0
.Net Core 3.1

# Setup
Clone this repository/Download the sample app.

# Configuring your app
All configuration for this app is located in appsettings.json. Locate and open this file.

Update the following:

OAuth2Keys (QBO)
- ClientId
- ClientSecret
- RedirectURL
- Environment
- DBConnectionString (Optional)
- QBOBaseURL

AppSettings (Square)
- AccessToken

# Run your app
After setting up both QBO and Square Developer Portal and updating the above appsettings.json, run the sample app in Visual Studio

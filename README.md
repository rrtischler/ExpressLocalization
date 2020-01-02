# ExpressLocalization

## Quick navigation
- [What is ExpressLocalization](#what-is-expresslocalization)
- [Version history](#version-history)
- [Installation and how to use](#installation)
- [Culture Fallback Behaviour](#culture-fallback-behaviour)
- [DataAnnotations Localization](#dataannotations-localization)
- [Views Localization](#views-localization)
- [Client Side Validation Libraries](#client-side-validation-libraries)
- [Identity RedirectTo Paths ( >= v3.1.1)](#identity-redirectto-paths)
- [Language Dropdown Menu](#language-dropdown-menu)
- [Dependencies](#dependencies)
- [Step by step tutorial](#step-by-step-tutorial)
- [Sample projects](#sample-projects)
- [Project website](#project-website)
- [License](#license)

## What is ExpressLocalization? 
A nuget package to simplify the localization of any Asp.Net Core web app to one step only. All below localization settings in one clean step:

- Global route template: Add {culture} paramter to all routes, so urls will be like http://www.example.com/en-US/
- RouteValueRequestCultureProvider : Register route value request culture provider, so culture selection will be based on route value
- ViewLocalization : Use [LocalizeTagHelper](https://github.com/lazziya/TagHelpers.Localization) for localizing all razor pages depending on a shared resource. In order to use LocalizetagHelper [LazZiya.TagHelpers.Localization](https://github.com/lazziya/TagHelpers.Localization) must be installed separately.
- DataAnnotations Localization : All data annotations validation messages and display names attributes localization
- ModelBinding Localization : localize model binding error messages
- IdentityErrors Localization : localize identity describer error messages
- Client Side Validation : include all client side libraries for validating localized input fields like decimal numbers. This option requires [LazZiya.TagHelpers](http://github.com/lazziya/TagHelpers) package that will be installed automatically.

[🔝](#quick-navigation)

## Version history
v3.1.3
- .Net Core 3.1 support

v3.1.2 :
- New optional boolean parameter : UseAllCultureProviders
- Route culture provider is not blocking other culture providers any more when UseAllCultureProviders value is false.
 

v3.1.1 :
 - Identity redirect Paths : Auto configure idenetity cookie to include culture value in the redirect to path on below events:
   - OnRedirectToLogin
   - OnRedirectToLogOut
   - OnRedirectToAccessDenied

[🔝](#quick-navigation)

## Installation
Install from nuget :

````
Install-Package LazZiya.ExpressLocalization
````

### How to use
- Install from nuget as mention above
- Relevant localization resource files are available in [LazZiya.ExpressLocalizationSample](https://github.com/LazZiya/ExpressLocalizationSample) repo.
Download the resources and add them to your main web project, or just create you own resource files with the relevant key names as in [ExpressLocalizationResource.tr.resx](https://github.com/LazZiya/ExpressLocalizationSample/blob/master/ExpressLocalizationSampleProject/LocalizationResources/ExpressLocalizationResource.tr.resx) file.
- In your main project' startup.cs file, define supported cultures list then add express localization setup in one step or customized steps as mentioned below

### One step setup:
This step will add all localization settings :
````cs
//add reference to :
using LazZiya.ExpressLocalization;

//setup express localization under ConfigureServices method:
public void ConfigureServices(IServiceCollection services)
{
    //other configuration settings....
    
    var cultures = new CultureInfo[]
    {
        new CultureInfo("en"),
        new CultureInfo("tr"),
        new CultureInfo("ar")
    };

    services.AddRazorPages()
        //ExpressLocalizationResource and ViewLocalizationResource are available in :
        // https://github.com/LazZiya/ExpressLocalizationSample
        .AddExpressLocalization<ExpressLocalizationResource, ViewLocalizationResource>(
            exOps =>
            {
                exOps.ResourcesPath = "LocalizationResources";
                exOps.RequestLocalizationOptions = ops =>
                {
                    ops.SupportedCultures = cultures;
                    ops.SupportedUICultures = cultures;
                    ops.DefaultRequestCulture = new RequestCulture("en");
                };
            });
}
````

Then configure the app to use RequestLocalizationMiddleware :
````cs
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    //other codes...
    
    //add localization middleware to the app
    app.UseRequestLocalization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapRazorPages();
    });
}
````

if you are using Mvc just add the culture parameter to the route as below:
````cs
app.UseMvc(routes =>
{
    routes.MapRoute(
    name: "default",
    template: "{culture=en}/{controller=Home}/{action=Index}/{id?}",
    );
});
````

Also it is possible to add culture parameter to the route attributes as well:
````cs
[Route("{culture}/Home")]
public class HomeController : Controller {
     // ...
 }
````

[🔝](#quick-navigation)

### Customized steps (optional)
If you don't need all settings in one step, you can use below methods for manually configuring localizaton steps.
For example if you need to provide separate localization resouce files for each of DataAnnotations, Identity and ModelBinding:
````cs
//configure request localizaton options
services.Configure<RequestLocalizationOptions>(
    ops =>
    {
        ops.SupportedCultures = cultures;
        ops.SupportedUICultures = cultures;
        ops.DefaultRequestCulture = new RequestCulture("en");
    });
    
services.AddRazorPages()
    //add view localization
    .AddViewLocalization(ops => { ops.ResourcesPath = "LocalizationResources"; })
    
    //register route value request culture provider, 
    //and add route parameter {culture} at the beginning of every url
    .ExAddRouteValueRequestCultureProvider(cultures, "en")

    //add shared view localization, 
    //use by injecting SharedCultureLocalizer to the views as below:
    //@inject SharedCultureLocalizer _loc
    //_loc.GetLocalizedString("Hello world")
    .ExAddSharedCultureLocalizer<ViewLocalizationResource>()

    //add DataAnnotations localization
    .ExAddDataAnnotationsLocalization<DataAnnotationsResource>()

    //add ModelBinding localization
    .ExAddModelBindingLocalization<ModelBindingResource>()

    //add IdentityErrors localization
    .ExAddIdentityErrorMessagesLocalization<IdentityErrorsResource>()
    
    //add client side validation libraries for localized inputs
    .ExAddClientSideLocalizationValidationScripts();
    
    // configure identity redirect to paths (without culture value)
    .ExConfigureApplicationCookie(string loginPath, string logoutPath, string accessDeniedPath, string defCulture);
````

### _Notice_
- if you are creating your own resource files, the relevant key names must be defined as in [ExpressLocalizationResource](https://github.com/LazZiya/ExpressLocalizationSample/blob/master/ExpressLocalizationSampleProject/LocalizationResources/ExpressLocalizationResource.tr.resx) file.
- All localization resources can be combined in one single resource or separate resources.

[🔝](#quick-navigation)

## Culture Fallback Behaviour
When using all the [localization culture providers][5], the localization process will check all available culture providers in order to detect the request culture. If the request culture is found it will stop checking and do localization accordingly. If the request culture is not found it will check the next provider by order. Finally if no culture is detected the default culture will be used.

Checking order for request culture:
1) [RouteSegmentCultureProvider][6]
2) [QueryStringRequestCultureProvider][7]
3) [CookieRequestCultureProvider][3]
4) [AcceptedLanguageHeaderRequestCultureProvider][4]
5) Use default request culture from startup settings

To restrict culture fallback to route culture provider only use below implementation in startup:
````cs
services.AddRazorPages()
        .AddExpressLocalization<ExpressLocalizationResource, ViewLocalizationResource>(ops =>
        {
            // Use only route segment culture provider
             ops.UseAllCultureProviders = false;
               
            // the rest of the code...
        });
````
_reference to issue [#13][8]_

[🔝](#quick-navigation)

## DataAnnotations Localization
All system data annotations error messages are defined in ExpressLocalizationResource. You can add your own localized texts to the same file.

For easy access all system validation messages can be accessed from [DataAnnotationsErrorMessages](https://github.com/LazZiya/ExpressLocalization/blob/master/LazZiya.ExpressLocalization/Messages/DataAnnotationsErrorMessages.cs) as below:

````cs
using LazZiya.ExpressLocalization.Messages

public class MyModel
{
    [Required(ErrorMessage = DataAnnotationsErrorMessages.RequiredAttribute_ValidationError)]
    [StringLength(maximumLength: 25, 
                ErrorMessage = DataAnnotationsErrorMessages.StringLengthAttribute_ValidationErrorIncludingMinimum, 
                MinimumLength = 3)]
    [Display(Name = "Name")]
    public string Name { get; set; }
}
````

[🔝](#quick-navigation)

## Views localization

### Option 1 (recommended)
Localize views using Localize tag helper, require installation of [LocalizeTagHelper](https://github.com/lazziya/TagHelpers.Localization):
````razor
<localize>Hello world!</localize>
````
or 
````razor
<div localize-content>
    <h1>Title</h1>
    <p>More text for localization.....</p>
</div>
````
for more details see [Live demo](http://demo.ziyad.info/en/localize) and [TagHelpers.Localization](http://github.com/lazziya/TagHelpers.Localization)


### Option 2
- inject shared culture localizer directly to the view or to _ViewImports.cshtml :
````razor
@using LazZiya.ExpressLocalization
@inject SharedCultureLocalizer _loc
````
- call localization function to get localized strings in views:
````razor
<h1 class="display-4">@_loc.GetLocalizedString("Welcome")</h1>
````
Views are using shared resource files like: [ViewLocalizationResource](https://github.com/LazZiya/ExpressLocalizationSample/blob/master/ExpressLocalizationSampleProject/LocalizationResources/ViewLocalizationResource.tr.resx)

[🔝](#quick-navigation)

## Client Side Validation Libraries
All required libraries to valdiate localized inputs like decimal numbers
- register TagHelpers in _ViewImports.cshtml :
````cshtml
@addTagHelper *, LazZiya.TagHelpers
````
- add tag helper to the view to validate localized input:
````cshtml
<localization-validation-scripts></localization-validation-scripts>
````

For more details see [LazZiya.TagHelpers](https://github.com/LazZiya/TagHelpers/) 

[🔝](#quick-navigation)

## Identity RedirectTo Paths 
_for versions ( >= v3.1.1)_
ExpressLocalization will automatically configure app cookie to add culture value to the redirect path when redirect events are invoked.
The default events and paths after configurations are: 
- OnRedirectToLogin : "{culture}/Identity/Account/Login/"
- OnRedirectToLogout : "{culture}/Identity/Account/Logout/"
- OnRedirectToAccessDenied : "{culture}/Identity/Account/AccessDenied/"

You can define custom paths for login, logout and access denied using ExpressLocalization as below:

````cs
services.AddRazorPages()
    .AddExpressLocalization<ExpressLocalizationResource, ViewLocalizationResource>(
        exOps =>
        {
            exOps.LoginPath = "/CustomLoginPath/";
            exOps.LogoutPath = "/CustomLogoutPath/";
            exOps.AcceddDeniedPath = "/CustomAccessDeniedPath/";
            
            // culture name to use when no culture value is defined in the routed url
            // default value is "en"
            exOps.DefaultCultureName = "tr-TR"; 
            
            exOps.RequestLocalizationOptions = ops =>
            {
                // ...
            };
        });
````

Or if you need to completely use custom cookie configurations using the identity extensions method, you need to set the value of `ConfigureRedirectPaths` to false as below:

````cs
services.AddRazorPages()
    .AddExpressLocalization<ExpressLocalizationResource, ViewLocalizationResource>(
        exOps =>
        {            
            // don't configure redirect to paths on redirect events
            exOps.ConfigureRedirectPaths = false;
            
            exOps.RequestLocalizationOptions = ops =>
            {
                // ...
            };
        });
````

in this case you need to manually configure the app cookie to handle the culture value on redirect events as described in this [issue][2]. 

[🔝](#quick-navigation)

## Language Dropdown Menu
To easily create a language navigation dropdown for changing the culture use [LanguageNavTagHelper](http://demo.ziyad.info/en/LanguageNav) from [LazZiya.TagHelpers](https://github.com/lazziya/taghelpers)

## Dependencies
[LazZiya.TagHelpers](https://github.com/LazZiya/TagHelpers/) package will be installed automatically, it is necessary for adding client side validation libraries for localized input fields like decimal numbers.

## Step by step tutorial 
http://ziyad.info/en/articles/36-Develop_Multi_Cultural_Web_Application_Using_ExpressLocalization

## Sample projects
 * Asp.Net Core 2.2 : https://github.com/LazZiya/ExpressLocalizationSample
 * Asp.Net Core 3.0 : https://github.com/LazZiya/ExpressLocalizationSampleCore3

## Project website
For discussion please visit: http://ziyad.info/en/articles/33-Express_Localization

[🔝](#quick-navigation)

## License
MIT

[1]: https://github.com/LazZiya/ExpressLocalization/tree/ExpressLocalizationCore3
[2]: https://github.com/LazZiya/ExpressLocalization/issues/6
[6]: https://github.com/LazZiya/ExpressLocalization/blob/master/LazZiya.ExpressLocalization/RouteSegmentCultureProvider.cs
[7]: https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.localization.querystringrequestcultureprovider
[3]: https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.localization.cookierequestcultureprovider
[4]: https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.localization.acceptlanguageheaderrequestcultureprovider
[5]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization-extensibility?view=aspnetcore-3.1#localization-culture-providers
[8]: https://github.com/LazZiya/ExpressLocalization/issues/13

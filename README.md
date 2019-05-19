# ExpressLocalization
Express localization settings for Asp.NetCore 2.x.
All below localization settings in one clean step:

- Global route template: Add {culture} paramter to all routes
- RouteValueRequestCultureProvider : register route value request culture provider
- ViewLocalization : shared resource for localizing all razor pages
- DataAnnotations Localization : All data annotations validation messages and display names attributes localization
- ModelBinding Localization : localize model binding error messages
- IdentityErrors Localization : localize identity describer error messages

## Installation
````
Install-Package LazZiya.ExpressLocalization -Version 1.0.0
````

## How to use
- Install from nuget as mention above
- Relevant localization resource files are available in [LazZiya.ExpressLocalization.Resources](https://github.com/LazZiya/ExpressLocalization.Resources) repo.
Download the resources project and reference it to your main web project, or just create you own resource files with the relevant key names as in [ExpressLocalizationResource.tr.resx](https://github.com/LazZiya/ExpressLocalization.Resources/blob/master/LazZiya.ExpressLocalization.Resources/ExpressLocalizationResource.tr.resx) file.
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

    services.AddMvc()
        //ExpressLocalizationResource and ViewLocalizationResource are available in :
        // https://github.com/LazZiya/ExpressLocalization.Resources
        .AddExpressLocalization<ExpressLocalizationResource, ViewLocalizationResource>(
            exOps =>
            {
                exOps.RequestLocalizationOptions = ops =>
                {
                    ops.SupportedCultures = cultures;
                    ops.SupportedUICultures = cultures;
                    ops.DefaultRequestCulture = new RequestCulture("en");
                };
            })
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
}
````

Then configure the app to use RequestLocalizationMiddleware :
````cs
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    //other codes...
    
    //add localization middleware to the app
    app.UseRequestLocalization();

    app.UseMvc();
}
````

### Customized steps (optional)
If you don't need all settings in one step, you can use below methods for manually configuring localizaton steps.
For example if you need to provide separate localization resouce files for each of DataAnnotations, Identity and ModelBinding:
````cs
services.AddMvc()
    //register route value request culture provider, 
    //and add route parameter {culture} at the beginning of every url
    .ExAddRouteValueRequestCultureProvider(cultures, "en")

    //add shared view localization, 
    //use by injecting SharedCultureLocalizer to the views as below:
    //@inject SharedCultureLocalizer _loc
    //_loc.Text("Hello world")
    .ExAddSharedCultureLocalizer<ViewLocalizationResource>()

    //add DataAnnotations localization
    .ExAddDataAnnotationsLocalization<DataAnnotationsResource>()

    //add ModelBinding localization
    .ExAddModelBindingLocalization<ModelBindingResource>()

    //add IdentityErrors localization
    .ExAddIdentityErrorMessagesLocalization<IdentityErrorsResource>()

    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
````

Notic: if you are creating your own resource files, the relevant key names must be defined as in [ExpressLocalizationResource](https://github.com/LazZiya/ExpressLocalization.Resources/blob/master/LazZiya.ExpressLocalization.Resources/ExpressLocalizationResource.tr.resx) file.

## DataAnnotations
All system data annotations error messages are defined in ExpressLocalizationResource. You can add your own localized texts to the same file.

For easy access there is a struct with all pre-defined validation messages can be accessed as below:

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

## View localization
- inject shared culture localizer directly to the view or to _ViewImports.cshtml :
````razor
@using LazZiya.ExpressLocalization
@inject SharedCultureLocalizer _loc
````
- call localization function to get localized strings in views:
````razor
<h1 class="display-4">@_loc.Text("Welcome")</h1>
````

## Sample project
See this sample project : https://github.com/LazZiya/ExpressLocalizationSample

## More
To easily create a language navigation dropdown for changing the culture use [LazZiya.TagHelpers](http://ziyad.info/en/articles/27-LazZiya_TagHelpers)

## License
MIT
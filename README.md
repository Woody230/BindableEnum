![](https://img.shields.io/github/v/release/Woody230/BindableEnum)
[![](https://img.shields.io/nuget/v/BindableEnum)](https://www.nuget.org/packages?q=BindableEnum)
![](https://img.shields.io/github/license/Woody230/BindableEnum)

# BindableEnum
Provides an enumeration wrapper that holds whether binding from the original string is successful.

This is useful when automatic model state validation needs to be disabled so that validation can be postponed to a later point in time. 

Normally a bad enum will cause the binding to fail, even though it would be preferrable to check for this in the postponed validation. 

With the bindable enum, this is avoided while also providing the convenience of automatically handling the conversion to an enum that would otherwise be missing when reverting back to using a string within a model.

# Setup

## Program.cs
Configure the `BindableEnumSwaggerGenOptions` on the service collection.

```c#
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Woody230.BindableEnum.Library.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureOptions<BindableEnumSwaggerGenOptions>();
```

This will map the `BindableEnum` generic type definition with an OpenApiSchema type of string.

Also, the `BindableEnumSchemaFilter` is applied in order to add the enumerations to the OpenApiSchema.

## Model
When declaring your bindable enumeration, you can use the `BindedEnumAttribute` to validate that the enumeration has been successfully binded. 

Note that a null bindable enumeration is NOT validated. The `RequiredAttribute` must be added to support this.

```c#
using System.ComponentModel.DataAnnotations;
using Woody230.BindableEnum.Library.Attributes;
using Woody230.BindableEnum.Library.Models;

public record WeatherForecast
{
    [Required]
    [BindedEnum]
    public BindableEnum<DayOfWeek> DayOfWeek { get; set; }
}    
```
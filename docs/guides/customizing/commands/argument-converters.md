---
uid: Guides.Customizing.Commands.ArgumentConverters
title: Customizing Wolfringo - Argument Converters
---

# Customizing Argument Conversion
When your command method executes, Commands System needs to convert string argument into whatever type you set in your method for that argument. It does it by using a set of [IArgumentConverters](xref:TehGM.Wolfringo.Commands.Parsing.IArgumentConverter).

## Creating a new Argument Converter
Creating an Argument Converter is very easy - simply create a new class and implement @TehGM.Wolfringo.Commands.Parsing.IArgumentConverter interface. @TehGM.Wolfringo.Commands.Parsing.IArgumentConverter requires 2 methods:
- [CanConvert](xref:TehGM.Wolfringo.Commands.Parsing.IArgumentConverter.CanConvert(System.Reflection.ParameterInfo)) - determines if this converter can convert to given method parameter at all.
- [Convert](xref:TehGM.Wolfringo.Commands.Parsing.IArgumentConverter.Convert(System.Reflection.ParameterInfo,System.String)) - converts the string argument into fitting method parameter.

Here is an @TehGM.Wolfringo.Commands.Parsing.ArgumentConverters.Int32Converter implementation for example:
```csharp
public class Int32Converter : IArgumentConverter
{
    public bool CanConvert(ParameterInfo parameter)
        => typeof(Int32) == parameter.ParameterType;

    public object Convert(ParameterInfo parameter, string arg)
        => System.Convert.ToInt32(arg, CultureInfo.InvariantCulture);
}
```

## Registering Custom Argument Converter
In order for Wolfringo Commands System to know about your converter, you need to register it with @TehGM.Wolfringo.Commands.Parsing.IArgumentConverterProvider. Default @TehGM.Wolfringo.Commands.Parsing.ArgumentConverterProvider implementation in Wolfringo uses @TehGM.Wolfringo.Commands.Parsing.ArgumentConverterProviderOptions with a dictionary map and explicit enum converter, so for most use cases, you don't even need to create a custom one.

### [Without Wolfringo.Hosting (Normal Bot)](#tab/configuring-normal-bot)
Note: instructions skip logger setup. See [Logging guide](xref:Guides.Features.Logging) for details.
1. Manually create an instance of @TehGM.Wolfringo.Commands.Parsing.ArgumentConverterProviderOptions.
2. Add your converter to @TehGM.Wolfringo.Commands.Parsing.ArgumentConverterProviderOptions.Converters dictionary, optionally change @TehGM.Wolfringo.Commands.Parsing.ArgumentConverterProviderOptions.EnumConverter.
3. Use `WithDefaultArgumentsConverterProvider` method of @TehGM.Wolfringo.Commands.CommandsServiceBuilder and pass your options as argument.

```csharp
ArgumentConverterProviderOptions options = new ArgumentConverterProviderOptions();
options.Converters[typeof(MySpecialType)] = new MySpecialTypeConverter();
_client = new WolfClientBuilder()
    .WithCommands(commands => 
    {
        commands.WithDefaultArgumentsConverterProvider(options);
    })
    .Build();
```

### [With Wolfringo.Hosting (.NET Generic Host/ASP.NET Core)](#tab/configuring-hosted-bot)
1. Configure @TehGM.Wolfringo.Commands.Parsing.ArgumentConverterProviderOptions.
2. Add your converter to @TehGM.Wolfringo.Commands.Parsing.ArgumentConverterProviderOptions.Converters dictionary, optionally change @TehGM.Wolfringo.Commands.Parsing.ArgumentConverterProviderOptions.EnumConverter.
```csharp
services.Configure<ArgumentConverterProviderOptions>(options =>
{
    options.Converters[typeof(MySpecialType)] = new MySpecialTypeConverter();
});
```

***

## Custom ConverterProvider
The default @TehGM.Wolfringo.Commands.Parsing.ArgumentConverterProvider implementation will first check @TehGM.Wolfringo.Commands.Parsing.ArgumentConverterProviderOptions.Converters for specific parameter type. If it doesn't exist, it'll check if the type is an `enum`, and if it is, it'll return @TehGM.Wolfringo.Commands.Parsing.ArgumentConverterProviderOptions.EnumConverter.

If your requirements are more complex, you can write your own class that implements @TehGM.Wolfringo.Commands.Parsing.IArgumentConverterProvider and register it with @TehGM.Wolfringo.Commands.CommandsServiceBuilder as explained in [Introduction](xref:Guides.Customizing.Intro).

You can view default implementation [on GitHub](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Commands/Parsing/ArgumentConverterProvider.cs) for reference.
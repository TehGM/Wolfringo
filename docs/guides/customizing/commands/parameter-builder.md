---
uid: Guides.Customizing.Commands.ParameterBuilder
title: Customizing Wolfringo - Parameter Builder
---

# Custom Parameter Builder
@TehGM.Wolfringo.Commands.Parsing.IParameterBuilder is responsible for actually injecting values of Commands' method arguments.  
The default @TehGM.Wolfringo.Commands.Parsing.ParameterBuilder implementation works as follows:
1. Check if it's a special argument provided by Command Instance. This is mainly used by @TehGM.Wolfringo.Commands.Initialization.RegexCommandInstance to provide [regex Match](xref:System.Text.RegularExpressions.Match) to the command method.
2. Provide @TehGM.Wolfringo.Commands.ICommandContext, or any type contained in the context.
3. Provide @TehGM.Wolfringo.Commands.Initialization.ICommandInstance that executes the method.
4. Provide Command Service's @System.Threading.CancellationToken.
5. Resolve services from @System.IServiceProvider. This includes generic loggers.
6. Attempt to use any of registered [IArgumentConverters](xref:TehGM.Wolfringo.Commands.Parsing.IArgumentConverter).

If no match is found through all these steps, the Builder will report a failure and command execution will fail.

## Custom Parameter Builder
If the flow above doesn't meet your needs, you can create a custom class implementing @TehGM.Wolfringo.Commands.Parsing.IParameterBuilder interface. This interface has only one method - [BuildParamsAsync](xref:TehGM.Wolfringo.Commands.Parsing.IParameterBuilder.BuildParamsAsync(System.Collections.Generic.IEnumerable{System.Reflection.ParameterInfo},TehGM.Wolfringo.Commands.Parsing.ParameterBuilderValues,System.Threading.CancellationToken)), which takes following parameters:
- [IEnumerable\<ParameterInfo\> parameters](xref:System.Collections.Generic.IEnumerable`1) - enumerable of each parameter in the command method being executed, in order.
- [ParameterBuilderValues values](xref:TehGM.Wolfringo.Commands.Parsing.ParameterBuilderValues) - set of values provided by @TehGM.Wolfringo.Commands.Initialization.ICommandInstance executing the method. This includes @System.IServiceProvider, @TehGM.Wolfringo.Commands.ICommandContext, the parsed arguments of the command (as string array), and some more required things.
- [CancellationToken cancellationToken](xref:System.Threading.CancellationToken) - cancellation token that can cancel the building. Note: this is only for builder, the CancellationToken to be used as a method parameter exists in @TehGM.Wolfringo.Commands.Parsing.ParameterBuilderValues.

This method returns a @TehGM.Wolfringo.Commands.Results.ParameterBuildingResult. The most important property is @TehGM.Wolfringo.Commands.Results.ParameterBuildingResult.Values - array of actual values to inject into the command method, in the same order as [IEnumerable\<ParameterInfo\> parameters](xref:System.Collections.Generic.IEnumerable`1) provided.

If you wish to see default @TehGM.Wolfringo.Commands.Parsing.ParameterBuilder implementation for reference, it is available [on GitHub](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Commands/Parsing/ParameterBuilder.cs).

Once your custom Parameter Builder is ready, you need to register it with @TehGM.Wolfringo.Commands.CommandsService as explained in [Introduction](xref:Guides.Customizing.Intro).
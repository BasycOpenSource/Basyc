//using Microsoft.JSInterop;
//using System.Reflection;

//namespace Microsoft.AspNetCore.Components;
//public static class ElementReferenceElementJsInteropExtensions
//{
//    private static readonly PropertyInfo jsRuntimeProperty = typeof(WebElementReferenceContext)!.GetProperty("JSRuntime", BindingFlags.Instance | BindingFlags.NonPublic).Value();

//    public static ValueTask<string> GetCssVariable(this ElementReference elementReference, string name) =>
//        elementReference.GetJSRuntime()?.InvokeAsync<string>("getCssVariable", elementReference, name) ?? ValueTask.FromResult(string.Empty);

//    public static ValueTask<T> GetCssVariable<T>(this ElementReference elementReference, string name) =>
//    elementReference.GetJSRuntime()?.InvokeAsync<T>("getCssVariable", elementReference, name) ?? ValueTask.FromResult(default(T))!;

//    public static ValueTask SetCssVariable(this ElementReference elementReference, string name, string value) =>
//        elementReference.GetJSRuntime()?.InvokeVoidAsync("setCssVariable", elementReference, name, value) ?? ValueTask.CompletedTask;

//    internal static IJSRuntime? GetJSRuntime(this ElementReference elementReference)
//    {
//        if (elementReference.Context is not WebElementReferenceContext webElementReferenceContext)
//            return null;

//        return (IJSRuntime?)jsRuntimeProperty.GetValue(webElementReferenceContext);
//    }
//}

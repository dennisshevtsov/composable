using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Patchable;

/// <summary>
/// The <see cref="ComposableModelBinder"/> provides a mechanism to create an instance
/// of a model that implements the <see cref="IComposable"/> from HTTP request. The binder
/// allows to bind parameters from the body, the route and the query string of the HTTP.
/// </summary>
public sealed class ComposableModelBinder : IModelBinder
{
  /// <summary>
  /// Attempts to bind a model.
  /// </summary>
  /// <param name="bindingContext">A context that contains operating information for model binding and validation.</param>
  /// <returns>An instance of the <see cref="Task"/> that represents an asynchronous operation.</returns>
  public async Task BindModelAsync(ModelBindingContext bindingContext)
  {
    object model = Activator.CreateInstance(bindingContext.ModelType)!;

    bindingContext.Result = ModelBindingResult.Success(model);
  }
}

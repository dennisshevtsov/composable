using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IPatchable.Binding;

/// <summary>
/// The <see cref="PatchableModelBinder"/> provides a mechanism to create an instance
/// of a model that implements the <see cref="IPatchable"/> from HTTP request. The binder
/// allows to bind parameters from the body, the route and the query string of the HTTP.
/// The binder also sets a list of properties that have been populated from the HTTP request.
/// </summary>
public sealed class PatchableModelBinder : IModelBinder
{
  public Task BindModelAsync(ModelBindingContext bindingContext)
  {
    throw new NotImplementedException();
  }
}

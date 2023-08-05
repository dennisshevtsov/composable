# Patchable

The project contains implementations of two model binders.
- `ComposableModelBinder` allows to combine data in one model from different source: the body, the route and the query string to the HTTP request.
- `PatchableModelBinder` allows to access the list of properties which were passed in the HTTP request. `PatchableModelBinder` inherits `ComposableModelBinder`.

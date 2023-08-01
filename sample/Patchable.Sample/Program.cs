// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

using Patchable.Sample.Book;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(options => options.AddPatchable());
builder.Services.AddScoped<BookService>();

WebApplication app = builder.Build();
app.Run();

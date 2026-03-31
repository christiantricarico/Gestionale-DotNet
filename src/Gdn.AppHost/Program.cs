var builder = DistributedApplication.CreateBuilder(args);

//builder.AddProject<Projects.Gdn_Web_Fluentblazor>("gdn-web-fluentblazor");
builder.AddProject<Projects.Gdn_Web_MudBlazor>("gdn-web-mudblazor");
builder.AddProject<Projects.Gdn_Web_Api_Vs>("gdn-web-api-vs");

builder.Build().Run();

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LifeDeck;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<LifeDeck.Services.Storage.BrowserStorageService>();
builder.Services.AddScoped<LifeDeck.Services.Api.WeatherService>();
builder.Services.AddScoped<LifeDeck.Services.Api.CurrencyService>();
builder.Services.AddScoped<LifeDeck.Services.Backup.BackupService>();


await builder.Build().RunAsync();

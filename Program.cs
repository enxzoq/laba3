using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Lab3.Models;
using Lab3.Service;
using Lab3.Data;

namespace Lab3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Подключение к базе данных
            var connectionString = builder.Configuration.GetConnectionString("DBConnection");

            // Регистрация сервисов
            builder.Services.AddDbContext<TelecomContext>(options =>
                options.UseSqlServer(connectionString));

            // Регистрация кэша и сервиса
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<CachedDataService>();

            // Регистрация сессий
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            app.UseSession();

            // Главная страница
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    string strResponse = "<HTML><HEAD><TITLE>Главная страница</TITLE></HEAD>" +
                                         "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                                         "<BODY>";
                    strResponse += "<BR><A href='/table'>Таблицы</A>";
                    strResponse += "<BR><A href='/info'>Информация</A>";
                    strResponse += "<BR><A href='/searchform1'>SearchForm1</A>";
                    strResponse += "<BR><A href='/searchform2'>SearchForm2</A>";
                    strResponse += "</BODY></HTML>";
                    await context.Response.WriteAsync(strResponse);
                    return;
                }
                await next.Invoke();
            });

            // SearchForm1 с использованием Cookie
            app.Map("/searchform1", appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    var dbContext = context.RequestServices.GetService<TelecomContext>();

                    if (context.Request.Method == "GET")
                    {
                        // Получение сохраненных данных из Cookie
                        var fullName = context.Request.Cookies["FullName"] ?? "";
                        var selectedTariffId = context.Request.Cookies["TariffId"] ?? "";

                        var tariffs = await dbContext.TariffPlans.ToListAsync();

                        var html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Search Form 1</title></head><body>";
                        html += "<h1>Search Subscribers (Cookie)</h1>";
                        html += "<form method='post'>";
                        html += "<label for='FullName'>Full Name:</label><br/>";
                        html += $"<input type='text' id='FullName' name='FullName' value='{fullName}' /><br/><br/>";

                        html += "<label for='TariffId'>Tariff:</label><br/>";
                        html += "<select id='TariffId' name='TariffId'>";
                        foreach (var tariff in tariffs)
                        {
                            var selected = tariff.Name == selectedTariffId ? "selected" : "";
                            html += $"<option value='{tariff.Name}' {selected}>{tariff.Name}</option>";
                        }
                        html += "</select><br/><br/>";

                        html += "<button type='submit'>Search</button>";
                        html += "</form>";
                        html += "</body></html>";

                        await context.Response.WriteAsync(html);
                    }
                    else if (context.Request.Method == "POST")
                    {
                        var formData = await context.Request.ReadFormAsync();

                        var fullName = formData["FullName"].ToString();
                        var tariffId = formData["TariffId"].ToString();

                        context.Response.Cookies.Append("FullName", fullName);
                        context.Response.Cookies.Append("TariffId", tariffId);

                        var query = dbContext.Subscribers.AsQueryable();

                        if (!string.IsNullOrEmpty(fullName))
                        {
                            query = query.Where(s => s.FullName.Contains(fullName));
                        }

                        if (!string.IsNullOrEmpty(tariffId))
                        {
                            query = query.Where(s => s.ServiceContracts.Any(c => c.TariffPlanName == tariffId));
                        }

                        var results = await query.Take(20).ToListAsync();

                        var html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Search Results</title></head><body>";
                        html += "<h1>Search Results</h1>";

                        if (results.Count > 0)
                        {
                            html += "<table border='1' style='border-collapse:collapse'>";
                            html += "<tr><th>ID</th><th>Full Name</th><th>Address</th></tr>";
                            foreach (var subscriber in results)
                            {
                                html += "<tr>";
                                html += $"<td>{subscriber.Id}</td>";
                                html += $"<td>{subscriber.FullName}</td>";
                                html += $"<td>{subscriber.HomeAddress}</td>";
                                html += "</tr>";
                            }
                            html += "</table>";
                        }
                        else
                        {
                            html += "<p>No results found.</p>";
                        }

                        html += "<br/><a href='/searchform1'>Back to Search</a>";
                        html += "</body></html>";

                        await context.Response.WriteAsync(html);
                    }
                });
            });

            // SearchForm2 с использованием Session
            app.Map("/searchform2", appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    var dbContext = context.RequestServices.GetService<TelecomContext>();

                    if (context.Request.Method == "GET")
                    {
                        // Получение сохраненных данных из Session
                        var fullName = context.Session.GetString("FullName") ?? "";
                        var selectedTariffId = context.Session.GetString("TariffId") ?? "";

                        var tariffs = await dbContext.TariffPlans.ToListAsync();

                        var html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Search Form 2</title></head><body>";
                        html += "<h1>Search Subscribers (Session)</h1>";
                        html += "<form method='post'>";
                        html += "<label for='FullName'>Full Name:</label><br/>";
                        html += $"<input type='text' id='FullName' name='FullName' value='{fullName}' /><br/><br/>";

                        html += "<label for='TariffId'>Tariff:</label><br/>";
                        html += "<select id='TariffId' name='TariffId'>";
                        foreach (var tariff in tariffs)
                        {
                            var selected = tariff.Name == selectedTariffId ? "selected" : "";
                            html += $"<option value='{tariff.Name}' {selected}>{tariff.Name}</option>";
                        }
                        html += "</select><br/><br/>";

                        html += "<button type='submit'>Search</button>";
                        html += "</form>";
                        html += "</body></html>";

                        await context.Response.WriteAsync(html);
                    }
                    else if (context.Request.Method == "POST")
                    {
                        var formData = await context.Request.ReadFormAsync();

                        var fullName = formData["FullName"].ToString();
                        var tariffId = formData["TariffId"].ToString();

                        context.Session.SetString("FullName", fullName);
                        context.Session.SetString("TariffId", tariffId);

                        var query = dbContext.Subscribers.AsQueryable();

                        if (!string.IsNullOrEmpty(fullName))
                        {
                            query = query.Where(s => s.FullName.Contains(fullName));
                        }

                        if (!string.IsNullOrEmpty(tariffId))
                        {
                            query = query.Where(s => s.ServiceContracts.Any(c => c.TariffPlanName == tariffId));
                        }

                        var results = await query.Take(20).ToListAsync();

                        var html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Search Results</title></head><body>";
                        html += "<h1>Search Results</h1>";

                        if (results.Count > 0)
                        {
                            html += "<table border='1' style='border-collapse:collapse'>";
                            html += "<tr><th>ID</th><th>Full Name</th><th>Address</th></tr>";
                            foreach (var subscriber in results)
                            {
                                html += "<tr>";
                                html += $"<td>{subscriber.Id}</td>";
                                html += $"<td>{subscriber.FullName}</td>";
                                html += $"<td>{subscriber.HomeAddress}</td>";
                                html += "</tr>";
                            }
                            html += "</table>";
                        }
                        else
                        {
                            html += "<p>No results found.</p>";
                        }

                        html += "<br/><a href='/searchform2'>Back to Search</a>";
                        html += "</body></html>";

                        await context.Response.WriteAsync(html);
                    }
                });
            });


            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/table")
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    string strResponse = "<HTML><HEAD><TITLE>table</TITLE></HEAD>" +
                     "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                     "<BODY>";
                    strResponse += "<BR><A href='/table/Employee'>Employee</A>";
                    strResponse += "<BR><A href='/table/ServiceContract'>ServiceContract</A>";
                    strResponse += "<BR><A href='/table/ServiceStatistic'>ServiceStatistic</A>";
                    strResponse += "<BR><A href='/table/Subscriber'>Subscriber</A>";
                    strResponse += "<BR><A href='/table/TariffPlan'>TariffPlan</A>";
                    strResponse += "</BODY></HTML>";
                    await context.Response.WriteAsync(strResponse);
                    return;
                }
                await next.Invoke();
            });

            // Кэширование данных
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/table", out var remainingPath) && remainingPath.HasValue && remainingPath.Value.StartsWith("/"))
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    var tableName = remainingPath.Value.Substring(1);

                    var cachedService = context.RequestServices.GetService<CachedDataService>();

                    if (tableName == "Employee")
                    {
                        var list = cachedService.GetEmployees();
                        await RenderTable(context, list);
                    }
                    else if (tableName == "ServiceContract")
                    {
                        var list = cachedService.GetServiceContracts();
                        await RenderTable(context, list);
                    }
                    else if (tableName == "ServiceStatistic")
                    {
                        var list = cachedService.GetServiceStatistics();
                        await RenderTable(context, list);
                    }
                    else if (tableName == "Subscriber")
                    {
                        var list = cachedService.GetSubscribers();
                        await RenderTable(context, list);
                    }
                    else if (tableName == "TariffPlan")
                    {
                        var list = cachedService.GetTariffPlans();
                        await RenderTable(context, list);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("Error");
                    }

                    return;
                }
                await next.Invoke();
            });


            // Информация о запросе
            app.Map("/info", appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    string strResponse = "<HTML><HEAD><TITLE>Информация</TITLE></HEAD>" +
                                         "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                                         "<BODY><H1>Информация о запросе:</H1>";
                    strResponse += "<BR> Сервер: " + context.Request.Host;
                    strResponse += "<BR> Путь: " + context.Request.Path;
                    strResponse += "<BR> Протокол: " + context.Request.Protocol;
                    strResponse += "<BR><A href='/'>Главная</A></BODY></HTML>";
                    await context.Response.WriteAsync(strResponse);
                });
            });

            app.Run();
        }

        static async Task RenderTable<T>(HttpContext context, IEnumerable<T> data)
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            var html = "<table border='1' style='border-collapse:collapse'>";

            var type = typeof(T);

            html += "<tr>";
            foreach (var prop in type.GetProperties())
            {
                if (!IsSimpleType(prop.PropertyType))
                {
                    continue;
                }

                html += $"<th>{prop.Name}</th>";
            }
            html += "</tr>";

            foreach (var item in data)
            {
                html += "<tr>";
                foreach (var prop in type.GetProperties())
                {
                    if (!IsSimpleType(prop.PropertyType))
                    {
                        continue;
                    }

                    var value = prop.GetValue(item);

                    if (value is DateTime dateValue)
                    {
                        html += $"<td>{dateValue.ToString("dd.MM.yyyy")}</td>";
                    }
                    else
                    {
                        html += $"<td>{value}</td>";
                    }
                }
                html += "</tr>";
            }

            html += "</table>";
            await context.Response.WriteAsync(html);
        }

        static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive ||
                   type.IsValueType ||
                   type == typeof(string) ||
                   type == typeof(DateTime) ||
                   type == typeof(decimal);
        }
    }

}
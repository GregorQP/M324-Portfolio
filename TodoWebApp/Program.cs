using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string filePath = "todo_list.json";
List<string> tasks = LoadTasks();

static List<string> LoadTasks()
{
    if (File.Exists("todo_list.json"))
        return JsonSerializer.Deserialize<List<string>>(File.ReadAllText("todo_list.json")) ?? new List<string>();
    return new List<string>();
}

static void SaveTasks(List<string> tasks) => File.WriteAllText("todo_list.json", JsonSerializer.Serialize(tasks));

// **Endpoints for To-Do API**
app.MapGet("/tasks", () => Results.Json(tasks));

app.MapPost("/tasks", async (HttpContext context) =>
{
    var newTask = await context.Request.ReadFromJsonAsync<string>();
    if (!string.IsNullOrWhiteSpace(newTask))
    {
        tasks.Add(newTask);
        SaveTasks(tasks);
        return Results.Ok(newTask);
    }
    return Results.BadRequest();
});

app.MapDelete("/tasks/{index:int}", (int index) =>
{
    if (index >= 0 && index < tasks.Count)
    {
        tasks.RemoveAt(index);
        SaveTasks(tasks);
        return Results.Ok();
    }
    return Results.BadRequest();
});

// **Serve a basic HTML frontend**
app.MapGet("/", async (HttpContext context) =>
{
    await context.Response.WriteAsync(File.ReadAllText("wwwroot/index.html"));
});

app.UseStaticFiles();

app.Run();

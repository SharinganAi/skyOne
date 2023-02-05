using skyOne.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using skyOne.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Group the APIs
var todoItems = app.MapGroup("/todoitems");
todoItems.MapGet("/", GetAllTodos);
todoItems.MapGet("/complete", GetCompletedTodos);
todoItems.MapGet("/incomplete", GetIncompletedTodos);
todoItems.MapGet("/{id}", GetTodoByID);
todoItems.MapPost("/create", CreateNewTask);


// Handlers
///Get all the tasks present in DB
static async Task<IResult> GetAllTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.ToArrayAsync());
}

static async Task<IResult> GetCompletedTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.Where(x => x.IsCompleted).ToListAsync());
}

static async Task<IResult> GetIncompletedTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.Where(x => !x.IsCompleted).ToListAsync());
}

static async Task<IResult> GetTodoByID(int id, TodoDb db)
{
    return await db.Todos.FindAsync(id) is Todo todo ? TypedResults.Ok(todo) : TypedResults.NotFound();
}

static async Task<IResult> CreateNewTask(Todo todo, TodoDb db)
{
    todo.Id = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return TypedResults.Created($"/todoitems/{todo.Id}", todo);
}

app.Run();



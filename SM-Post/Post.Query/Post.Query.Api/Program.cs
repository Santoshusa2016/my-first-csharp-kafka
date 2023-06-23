using Confluent.Kafka;
using CQRS.Core.Consumer;
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Repository;
using Post.Query.Infrastructure.Consumer;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// sect10:46 - Add services to the container.
Action<DbContextOptionsBuilder> configureDbContext = o => o.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
builder.Services.AddDbContext<DatabaseContext>(configureDbContext); //here we pass optionsbuilder to OnConfiguring method of DBContext
builder.Services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDbContext));

//sect10:47 - create DB and tables from code
var dataContext = builder.Services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
dataContext.Database.EnsureCreated();




//sect11:52 - register repository <<check order of services added>>
builder.Services.AddScoped<IPostRepository, PostRespository>();
builder.Services.AddScoped<ICommentRepository, CommentRespository>();
builder.Services.AddScoped<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();



//sect12:56 - Event consumer
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
builder.Services.AddScoped<IEventConsumer, EventConsumer>();



//sect12:57 ConsumerHosted background service
builder.Services.AddHostedService<ConsumerHostedService>(); //this will call StartAsync method on startup of api


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.Run();

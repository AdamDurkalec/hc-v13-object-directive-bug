var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddDirectiveType<MyCustomDirectiveObjectDirectiveType>()
    .AddDirectiveType<MyCustomDirectiveFieldDefinitionDirectiveType>()
    .AddQueryType<Query>()
    .AddType<BookType>()
    .AddType<AuthorType>(); 

var app = builder.Build();

app.MapGraphQL();

app.Run();



public class Book
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public Author? Author { get; set; }
}

public class Author
{
    public string? Name { get; set; }
}

public class Query
{
    public Book GetBook() =>
        new Book
        {
            Id = Guid.NewGuid(),
            Title = "C# in depth.",
            Author = new Author
            {
                Name = "Jon Skeet"
            }
        };
}

public class BookType : ObjectType<Book>
{
    protected override void Configure(IObjectTypeDescriptor<Book> descriptor)
    {
        descriptor.Field(b => b.Id).ID();
        // this directive on an object does not work
        descriptor.Directive(new MyCustomDirectiveObjectDirective());
    }
}

public class AuthorType : ObjectType<Author>
{
    protected override void Configure(IObjectTypeDescriptor<Author> descriptor)
    {
        // this directive on field works well
        descriptor.Field(a=>a.Name).Directive(new MyCustomDirectiveFieldDefinitionDirective()); 
    }
}

public class MyCustomDirectiveObjectDirective { }

public class MyCustomDirectiveObjectDirectiveType : DirectiveType<MyCustomDirectiveObjectDirective>
{
    protected override void Configure(
        IDirectiveTypeDescriptor<MyCustomDirectiveObjectDirective> descriptor)
    {
        descriptor.Name("MyCustomDirectiveObjectDirective");
        descriptor.Location(DirectiveLocation.Object);

        descriptor.Use((next, Directive) => context =>
        {
            context.Result = "Bar-Object";
            return next.Invoke(context);
        });
    }
}

public class MyCustomDirectiveFieldDefinitionDirective { }

public class MyCustomDirectiveFieldDefinitionDirectiveType : DirectiveType<MyCustomDirectiveFieldDefinitionDirective>
{
    protected override void Configure(
        IDirectiveTypeDescriptor<MyCustomDirectiveFieldDefinitionDirective> descriptor)
    {
        descriptor.Name("MyCustomDirectiveFieldDefinitionDirective");
        descriptor.Location(DirectiveLocation.FieldDefinition);

        descriptor.Use((next, Directive) => context =>
        {
            context.Result = "Bar-FieldDefinition";
            return next.Invoke(context);
        });
    }
}
# CommandBus

Provides a simple reflection based service setup for commands within a CQRS architecture.

## How to use

Simply define your command and a corresponding `CommandHandler`...

```csharp
public class CreateFooCommand
{
    public string Name { get; }

    public CreateFooCommand(string name)
    {
        Name = name;
    }
}

internal class CreateFooCommandHandler : CommandHandler<CreateFooCommand>
{
    public override Task Handle(CreateFooCommand command)
    {
        //Implement Logic
    }
}
```

Optionally you can also define a custom command result...

```csharp
public class DeleteFooCommand
{
    public int Id { get; }

    public DeleteFooCommand(int id)
    {
        Id = id;
    }
}

public class DeleteFooCommandResult
{
    public bool Success { get; set; }
}

internal class DeleteFooCommandHandler : CommandHandler<DeleteFooCommand, DeleteFooCommandResult>
{
    public override Task<DeleteFooCommandResult> HandleAndGetResult(DeleteFooCommand command)
    {
        //Implement Logic
    }
}
```

You can also define a validator that will execute before the handler...

```csharp
public class UpdateFooCommand
{
    public int Id { get; }
    public string Name { get; }

    public UpdateFooCommand(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

internal class UpdateFooCommandValidator : Validator<UpdateFooCommand>
{
    public override Task Validate(UpdateFooCommand command)
    {
        if (command.Id == 0)
            AddErrorMessage("Invalid id provided");
    }
}

internal class UpdateFooCommandHandler : CommandHandler<UpdateFooCommand>
{
    public override Task Handle(UpdateFooCommand command)
    {
        //Implement Logic
    }
}
```

Then use the provided `IServiceCollection` extension method, passing in the assemblies containing your commands...

```csharp
var sc = new ServiceCollection();
sc.AddCommandBus(typeof(CreateFooCommand).Assembly, typeof(DeleteFooCommand).Assembly, typeof(UpdateFooCommand).Assembly);
```

You can then use the service provider to get your `ICommandBus` that can be used to execute your commands...

```csharp
var sp = sc.BuildServiceProvider();
var commandBus = sp.GetRequiredService<ICommandBus>();

var createFooResult = commandBus.Execute(new CreateFooCommand("Foo"));
var updateFooResult = commandBus.Execute(new UpdateFooCommand(1, "Foo"));
var deleteFooResult = commandBus.Execute<DeleteFooCommand, DeleteFooCommandResult>(new DeleteFooCommand(1));
```
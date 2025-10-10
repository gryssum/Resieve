# Introduction

Resieve is a simple, clean and extensible to add pagination, filtering and sorting to your project.
It is designed to integrate with into your .NET applications with minimal setup.
By abstracting the complexity of query building, Resieve allows you to focus on your business logic while ensuring your APIs remain efficient and maintainable.

## Table of Contents

- [Supported operators](#supported-operators)
  - [Sorting](#sorting)
  - [Filtering](#filtering)
    - [Logical operators](#logical-operators)
    - [Comparison operators](#comparison-operators)
    - [Case-insensitive comparison operators](#case-insensitive-comparison-operators)
- [Usage](#usage)
  - [Installation](#installation)
  - [Getting started](#getting-started)
- [Advanced usage](#advanced-usage)
  - [Custom filtering](#custom-filtering)
  - [Custom sorting](#custom-sorting)

# Supported operators

## Sorting

Sorting enables you to order results by one or more properties. Prefixing a property with `-` sorts it in descending order, while multiple properties can be chained using commas for multi-level sorting.

| Operator | Description                                               | Example           |
|----------|-----------------------------------------------------------|-------------------|
| `,`      | Comma seperator  for multiple sort statements             | `sort1,sort2`     |
| `-`      | A dash in front of a property will sort it descendingly   | `-sort1 ,sort2`   |

## Filtering
Filtering allows you to narrow down results based on property values. Combine multiple filters using `,` for AND logic or `|` for OR logic to create complex queries.

### Logical operators

| Operator | Description                       | Example                        |
|----------|-----------------------------------|--------------------------------|
| `,`      | Comma seperator wil act as an AND | `property1==a,propeerty2==b`   |
| `\|`     | A pipe symbol will act as an OR   | `property1==a\|propeerty2==b`  |


### Comparison operators
These operators provide granular control over how data is filtered, supporting both numeric and string comparisons. Use them to match, exclude, or partially match property values.

| Operator | Description                            | Example                   |
|----------|----------------------------------------|---------------------------|
| `>=`     | Greater than or equal                  | `NumberProperty  >= 4`    |
| `<=`     | Less than or equal                     | `NumberProperty  <= 5`    |
| `==`     | Equals                                 | `NumberProperty  == 5`    |
| `!=`     | Not equals                             | `NumberProperty  != 5`    |
| `>`      | Greater than                           | `NumberProperty  > 4`     |
| `<`      | Less than                              | `NumberProperty < 5`      |
| `@=`     | Contains                               | `FruitProperty @= app`    |
| `!@=`    | Does not contain                       | `FruitProperty !@= apple` |
| `_=`     | Starts with                            | `WordProperty _= hel`     |
| `_-=`    | Ends with                              | `WordProperty _-= ld`     |
| `!_=`    | Does not start with                    | `FruitProperty !_= ap`    |
| `!_-=`   | Does not end with                      | `FruitProperty !_-= le`   |

### Case-insensitive comparison operators
Disclaimer: The case-insensitive will call a tolowercase on both sides of the comparison, so be aware of potential performance implications when working with large datasets.
To properly support case-insensitive filtering consider using a case-insensitive collation on your database columns.

| Operator | Description                            | Example                   |
|----------|----------------------------------------|---------------------------|
| `==*`    | Equals (case-insensitive))             | `WordProperty ==* HELLO`  |
| `!=*`    | Not equals (case-insensitive))         | `WordProperty !=* HELLO`  |
| `@=*`    | Contains (case-insensitive)            | `WordProperty @=* LlO`    |
| `!@=*`   | Does not contain (case-insensitive)    | `WordProperty !@=* LlO`   |
| `_=*`    | Starts with (case-insensitive)         | `WordProperty _=* HeL`    |
| `_-=*`   | Ends with (case-insensitive)           | `WordProperty _-=* OrLd`  |
| `!_=*`   | Does not start with (case-insensitive) | `WordProperty !_=* HeL`   |
| `!_-=*`  | Does not end with (case-insensitive)}  | `WordProperty !_-=* OrlD` |

Case-insensitive operators are useful when you want to ignore letter casing in your queries. For optimal performance, especially on large datasets, consider configuring your database columns with case-insensitive collation.

# Usage

## Installation
Install Resieve via NuGet to make its features available in your project. This command will add the package reference to your solution.

> dotnet nuget add package Resieve

## Getting started

Add the following line to you `Program.cs` or `Startup.cs` file to register the Resieve services:
```csharp
using Resieve;

builder.Services.AddResieve();
```
---

Add entity mapping to configure on which properties the consumer is allowed to sort or/and filter.
This lets you decide exactly which properties users can sort and filter by, so you keep your API flexible and safe.
```csharp
public class ProductMapper : IResieveMapper
{
    public void Map(ResieveMapper mapper)
    {        
        public void Configure(ResieveMapper mapper)
        {
            // Only sorting
            mapper.ForProperty<Product>(x => x.Id).CanSort();
            
            // Only filtering
            mapper.ForProperty<Product>(x => x.Name).CanFilter();
            
            // Both filtering and sorting
            mapper.ForProperty<Product>(x => x.Price).CanFilter().CanSort();
        }
    }
}
```

By accepting a `ResieveModel` from query parameters, your controller can automatically handle incoming filter, sort, and pagination requests, streamlining endpoint logic.

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductController(ProductRepository repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get([FromQuery] ResieveModel model)
    {
        var products = repository.GetFilteredProducts(model);
        return Ok(products);
    }
}
```
---

For a basic repository implemention where we simply return the results of the filtering, pagination and sorting.

```csharp
public class ProductRepository
{
    private readonly AppDbContext _context;
    private readonly IResieveProcessor _processor;

    public ProductRepository(AppDbContext context, IResieveProcessor processor)
    {
        _context = context;
        _processor = processor;
    }

    public IEnumerable<Product> GetFilteredProducts(ResieveModel model)
    {
        var source = _context
            .Products
            .Include(p => p.Tags)
            .AsNoTracking();
        
        return _processor.Process(model, source);
    }
}
```

More advanced repository implementation also returning pagination details.
```csharp

public class ProductAdvancedRepository(AppDbContext context, IResieveProcessor processor)
{
    public PaginatedResponse<IEnumerable<Product>> GetFilteredProducts(ResieveModel model)
    {
        var source = context
            .Products
            .Include(p => p.Tags)
            .AsNoTracking();

        var count = source.Count();
        var result = processor.Process(model, context.Products);

        return new PaginatedResponse<IEnumerable<Product>>(result, model.Page, model.PageSize, count);
    }
}

public record PaginatedResponse<T>(T Items, int PageNumber, int PageSize, int TotalCount);
```
---

## Advanced usage
For trickier cases like relationships or complex queries, you can plug in your own custom filtering and sorting logic.
If you need to filter or sort on one-to-many or many-to-many relationships, just add a custom key and hook up your own filter or sort.
You’ll get the operator and value, so you can handle it however you want.

### Custom filtering
Custom filters let you define exactly how filtering should behave for a property or key. 
Implement the interface and register your filter to override or extend default behavior.


Example implementation of a custom filter that performs a case-insensitive equality check on the product name.
```csharp
public class CustomNameFilter : IResieveCustomFilter<Product>
{    
    public Expression<Func<Product, bool>> BuildWhereExpression(string @operator, string value)
    {
        return x => x.Name.ToLower() == value.ToLower();
    }
}
```

Example registration of custom filter in the entity mapping.
```csharp
public class ProductMapper : IResieveMapper
{
    public void Map(ResieveMapper mapper)
    {        
        public void Configure(ResieveMapper mapper)
        {            
            // Overwrite the default property filter
            mapper.ForProperty<Product>(x => x.Name).CanFilter<CustomNameFilter>();
            
            // Or add custom name
            mapper.ForKey<Product>("Tags.Name").CanFilter<CustomNameFilter>();
        }
    }
}
```

### Custom sorting
Custom sorts allow you to control the ordering logic for properties or keys, supporting advanced scenarios such as sorting on related entities or computed fields.

Example implementation of a custom sort that sorts products by their name.
```csharp
public class CustomNameSort : IResieveCustomSort<Product>
{
    public IOrderedQueryable<Product> Apply(IQueryable<Product> source, bool isDescending)
    {
        return isDescending ? source.OrderByDescending(x => x.Name) : source.OrderBy(x => x.Name);
    }
    
    public IOrderedQueryable<Product> ApplyThenBy(IOrderedQueryable<Product> source, bool isDescending)
    {
        return isDescending ? source.ThenByDescending(x => x.Name) : source.ThenBy(x => x.Name);
    }
}
```

Example registration of custom sort in the entity mapping.
```csharp
public class ProductMapper : IResieveMapper
{
    public void Map(ResieveMapper mapper)
    {        
        public void Configure(ResieveMapper mapper)
        {            
            // Overwrite the default property filter
            mapper.ForProperty<Product>(x => x.Name).CanSort<CustomNameFilter>();
            
            // Or add custom name
            mapper.ForKey<Product>("Tags.Name").CanSort<CustomNameFilter>();
        }
    }
}
```

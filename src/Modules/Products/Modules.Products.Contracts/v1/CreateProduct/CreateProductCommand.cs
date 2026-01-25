using Mediator;

namespace FSH.Modules.Products.Contracts.v1.CreateProduct;

public sealed record CreateProductCommand(
    string Title,
    string Revision) : ICommand<CreateProductResponse>;

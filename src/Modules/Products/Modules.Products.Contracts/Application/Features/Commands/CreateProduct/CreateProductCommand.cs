using FSH.Modules.Products.Contracts.v1.CreateProduct;
using Mediator;

namespace FSH.Modules.Products.Contracts.Application.Features.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Title,
    int Revision) : ICommand<CreateProductResponse>;
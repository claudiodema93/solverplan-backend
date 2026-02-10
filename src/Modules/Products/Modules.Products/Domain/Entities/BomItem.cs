using FSH.Framework.Core.Domain;
using FSH.Modules.Products.Contracts.Domain.Enums;
using System;

namespace FSH.Modules.Products.Domain.Entities;

public class BomItem : BaseEntity<int>, IHasTenant, IAuditableEntity
{
    /// <summary>
    /// Foreign key to the associated product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Navigation property to the associated product.
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// Foreign key to the Child Product entity
    /// </summary>
    public int ChildProductId { get; set; }

    /// <summary>
    /// Navigation property to the child Product entity
    /// </summary>
    public Product? ChildProduct { get; set; }

    /// <summary>
    /// Quantity necessary of child product
    /// </summary>
    public double Quantity { get; set; } = 1.0;

    /// <summary>
    /// Unit of measurement
    /// </summary>
    public UnitOfMeasurement Unit { get; set; } = UnitOfMeasurement.Pcs;

    /// <summary>
    /// The user entered manually
    /// </summary>
    public bool IsManual { get; set; } = false;

    /// <summary>
    /// Optional notes of the bom item
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Information about the tenant.
    /// </summary>
    public string TenantId { get; set; } = default!;

    /// <summary>
    /// The date and time when the product was created in UTC.
    /// </summary>
    public DateTimeOffset CreatedOnUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The identifier of the user who created the product.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// The date and time when the product was last modified in UTC.
    /// </summary>
    public DateTimeOffset? LastModifiedOnUtc { get; set; }

    /// <summary>
    /// The identifier of the user who last modified the product.
    ///  </summary>
    public string? LastModifiedBy { get; set; }
}

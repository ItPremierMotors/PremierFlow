using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Domain.Common
{
    /// <summary>
    /// Clase base para entidades que implementan soft delete.
    /// </summary>
    public abstract class SoftDeletableEntity : AuditableEntity
    {
        public bool Activo { get; set; } = true;
    }
}

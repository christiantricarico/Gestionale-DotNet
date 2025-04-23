using System.ComponentModel.DataAnnotations.Schema;

namespace Gdn.Domain.Models.Bases;

public abstract class BaseEntity<T> where T : struct
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public T Id { get; set; }
}

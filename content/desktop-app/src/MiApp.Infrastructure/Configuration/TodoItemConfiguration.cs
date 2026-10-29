using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiApp.Infrastructure.Configuration;

public sealed class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("TodoItems");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever(); // el ID lo genera el dominio (Guid v7)

        builder.Property(t => t.Title).HasMaxLength(TodoItem.TitleMaxLength).UseCollation("NOCASE");
    }
}

using Microsoft.EntityFrameworkCore;
using Thesaurus.core.Entities;

namespace Thesaurus.Core.Data;

public class ThesaurusDbContext : DbContext
{
    public DbSet<Word> Words => Set<Word>();
    public DbSet<WordSynonym> WordSynonyms => Set<WordSynonym>();

    public ThesaurusDbContext(DbContextOptions<ThesaurusDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Word>()
            .HasIndex(w => w.Text)
            .IsUnique();

        modelBuilder.Entity<WordSynonym>()
            .HasOne(ws => ws.Word)
            .WithMany(w => w.Synonyms)
            .HasForeignKey(ws => ws.WordId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WordSynonym>()
            .HasOne(ws => ws.Synonym)
            .WithMany()
            .HasForeignKey(ws => ws.SynonymId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Microsoft.EntityFrameworkCore;
using Thesaurus.core.Entities;

namespace Thesaurus.Core.Data;

public class ThesaurusDbContext : DbContext
{
    // DbSet for Words table
    public DbSet<Word> Words => Set<Word>();

    // DbSet for WordSynonyms table
    public DbSet<WordSynonym> WordSynonyms => Set<WordSynonym>();

    // Constructor to initialize DbContext with options
    public ThesaurusDbContext(DbContextOptions<ThesaurusDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ensure the 'Text' column in the Words table is unique
        modelBuilder.Entity<Word>()
            .HasIndex(w => w.Text)
            .IsUnique();

        // Configure the relationship between WordSynonym and Word
        modelBuilder.Entity<WordSynonym>()
            .HasOne(ws => ws.Word) // WordSynonym references Word
            .WithMany(w => w.Synonyms) // Word has many synonyms
            .HasForeignKey(ws => ws.WordId) // Foreign key in WordSynonym
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

        // Configure the relationship between WordSynonym and Synonym (another Word)
        modelBuilder.Entity<WordSynonym>()
            .HasOne(ws => ws.Synonym) // WordSynonym references Synonym
            .WithMany() // Synonym does not have a navigation property back
            .HasForeignKey(ws => ws.SynonymId) // Foreign key in WordSynonym
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
    }
}

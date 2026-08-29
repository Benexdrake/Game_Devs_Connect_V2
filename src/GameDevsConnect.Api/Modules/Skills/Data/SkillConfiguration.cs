using GameDevsConnect.Api.Modules.Skills.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDevsConnect.Api.Modules.Skills.Data;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("skills");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired();
        builder.HasIndex(s => s.Name).IsUnique();
        builder.Property(s => s.Category).HasConversion<string>().IsRequired();

        builder.HasData(
            Seed("b14f822f-009e-4bdd-8ee8-11f717c507e8", "Unity", SkillCategory.Programming),
            Seed("30e3205f-3abd-4b5d-89e9-9d4b32f282b6", "Unreal Engine", SkillCategory.Programming),
            Seed("2838e506-cb9c-408a-ac3e-2a525c3d982b", "Godot", SkillCategory.Programming),
            Seed("8e63ea09-22dc-4398-a5d5-c1bf8790867d", "C#", SkillCategory.Programming),
            Seed("333877c3-9486-45bd-8b37-7f7ca9f7ee0a", "C++", SkillCategory.Programming),
            Seed("ccba96cf-0795-461b-8c92-99fcf4dd8bbe", "Blender", SkillCategory.Art3D),
            Seed("240d07c6-d557-4dc3-befc-77ed08584c17", "Maya", SkillCategory.Art3D),
            Seed("deccc28e-e8be-4cca-9f2b-7415f47e468b", "Photoshop", SkillCategory.Art2D),
            Seed("67aabe52-5d35-4e68-96da-ab51702eece9", "2D Art", SkillCategory.Art2D),
            Seed("439780f2-c1da-4f58-9207-f6efe2badfd7", "3D Art", SkillCategory.Art3D),
            Seed("a72cb020-7595-4f01-805e-56c1887492c2", "Animation", SkillCategory.Animation),
            Seed("6f77e0f9-75c3-48f3-aec3-fc8cc052b5c4", "Rigging", SkillCategory.Animation),
            Seed("f43e2b0d-5cbd-48de-bcf7-39b679195498", "Music", SkillCategory.Audio),
            Seed("81b3199c-400f-4c3c-a74a-49ffd46e521c", "Sound Design", SkillCategory.Audio),
            Seed("1e8cf9d9-bb76-4c08-9870-1df2f22bb6ca", "Game Design", SkillCategory.Design),
            Seed("3b563484-5b50-4943-814d-5b4fa647e5f5", "Level Design", SkillCategory.Design),
            Seed("145d0eaf-6309-448a-bb15-50a1523ecb24", "Writing", SkillCategory.Writing)
        );
    }

    private static Skill Seed(string id, string name, SkillCategory category) =>
        new() { Id = Guid.Parse(id), Name = name, Category = category };
}

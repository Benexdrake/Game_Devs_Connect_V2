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
            // Engines
            Seed("b14f822f-009e-4bdd-8ee8-11f717c507e8", "Unity", SkillCategory.Engines),
            Seed("30e3205f-3abd-4b5d-89e9-9d4b32f282b6", "Unreal Engine", SkillCategory.Engines),
            Seed("2838e506-cb9c-408a-ac3e-2a525c3d982b", "Godot", SkillCategory.Engines),
            Seed("76e0dfb8-b0d1-4151-b9ff-4e200ec03914", "GameMaker", SkillCategory.Engines),
            // Programming
            Seed("8e63ea09-22dc-4398-a5d5-c1bf8790867d", "C#", SkillCategory.Programming),
            Seed("333877c3-9486-45bd-8b37-7f7ca9f7ee0a", "C++", SkillCategory.Programming),
            Seed("c72ce11d-8e03-4f87-9ee9-ddf2d7e80d3b", "Python", SkillCategory.Programming),
            Seed("11128a9b-c4de-45b1-96de-f8c59c5abe33", "Lua", SkillCategory.Programming),
            Seed("4f44b26c-63c8-4bf4-a6a5-69c070b7bc29", "GDScript", SkillCategory.Programming),
            // Art2D
            Seed("deccc28e-e8be-4cca-9f2b-7415f47e468b", "Photoshop", SkillCategory.Art2D),
            Seed("4688962e-b2f6-4958-9859-6c00d6783259", "Illustrator", SkillCategory.Art2D),
            Seed("aa33b59d-e508-4576-b586-b7810ab89084", "Aseprite", SkillCategory.Art2D),
            // Art3D
            Seed("ccba96cf-0795-461b-8c92-99fcf4dd8bbe", "Blender", SkillCategory.Art3D),
            Seed("240d07c6-d557-4dc3-befc-77ed08584c17", "Maya", SkillCategory.Art3D),
            Seed("78b25059-9ae8-408f-925c-75d98676263d", "ZBrush", SkillCategory.Art3D),
            Seed("17174a66-afe0-4a05-904b-657273b638b2", "Substance Painter", SkillCategory.Art3D),
            // Animation
            Seed("a72cb020-7595-4f01-805e-56c1887492c2", "Animation", SkillCategory.Animation),
            Seed("6f77e0f9-75c3-48f3-aec3-fc8cc052b5c4", "Rigging", SkillCategory.Animation),
            Seed("993fe2fc-8afc-40fe-9a83-e686ead6ccf3", "VFX", SkillCategory.Animation),
            // Audio
            Seed("f43e2b0d-5cbd-48de-bcf7-39b679195498", "Music", SkillCategory.Audio),
            Seed("81b3199c-400f-4c3c-a74a-49ffd46e521c", "Sound Design", SkillCategory.Audio),
            Seed("1cf30b71-1812-4221-be32-0f526a027bb3", "Voice Acting", SkillCategory.Audio),
            Seed("edbbeb85-0210-4a75-a354-8873ebee6f79", "Wwise / FMOD", SkillCategory.Audio),
            // Design
            Seed("1e8cf9d9-bb76-4c08-9870-1df2f22bb6ca", "Game Design", SkillCategory.Design),
            Seed("3b563484-5b50-4943-814d-5b4fa647e5f5", "Level Design", SkillCategory.Design),
            Seed("9b5c497b-1d79-4850-8123-8ed4525640e9", "UI/UX Design", SkillCategory.Design),
            Seed("bf950d08-5ca0-431a-aaea-df42d829fcf1", "Narrative Design", SkillCategory.Design),
            // Writing
            Seed("145d0eaf-6309-448a-bb15-50a1523ecb24", "Writing", SkillCategory.Writing),
            Seed("5eb0e9c4-dbad-426b-a429-c74aac945a24", "Dialogue Writing", SkillCategory.Writing),
            Seed("275ac4a5-1897-4959-98be-a8a53c7e8372", "Localization", SkillCategory.Writing),
            // Production
            Seed("bb2e2d42-8ce1-4d6b-9457-9809aec7d707", "Project Management", SkillCategory.Production),
            Seed("381cf829-8c1c-4808-876c-231bbdb13ba5", "QA / Testing", SkillCategory.Production),
            Seed("447e1a2b-9b9c-44cf-afcc-7c2cfe5ae031", "Community Management", SkillCategory.Production),
            Seed("4509c131-7068-4984-b592-78a6ac8a1163", "Marketing", SkillCategory.Production)
        );
    }

    private static Skill Seed(string id, string name, SkillCategory category) =>
        new() { Id = Guid.Parse(id), Name = name, Category = category };
}

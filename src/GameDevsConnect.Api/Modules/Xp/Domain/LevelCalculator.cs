namespace GameDevsConnect.Api.Modules.Xp.Domain;

// Level is a pure derivation from total XP - never a persisted field. The
// exact curve is an implementation detail (see README §12); this one just
// needs to be deterministic and strictly increasing.
public static class LevelCalculator
{
    private const int MaxLevel = 50;

    public static int ThresholdForLevel(int level) => 50 * (level - 1) * level;

    public static int LevelForXp(int totalXp)
    {
        var level = 1;
        while (level < MaxLevel && totalXp >= ThresholdForLevel(level + 1))
        {
            level++;
        }

        return level;
    }

    public static int XpForNextLevel(int totalXp)
    {
        var level = LevelForXp(totalXp);
        return level >= MaxLevel ? ThresholdForLevel(level) : ThresholdForLevel(level + 1);
    }
}

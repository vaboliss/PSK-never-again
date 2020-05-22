
namespace EducationSystem.Static
{
    public static class GlobalRestrictions
    {
        public static int MaxConsecutiveDays { get; set; }
        public static int MaxPerMonth { get; set; }
        public static int MaxPerYear { get; set; }
        public static int MaxPerQuarter { get; set; } = 3;
    }
}

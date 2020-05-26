
namespace EducationSystem.Interfaces
{
    interface IGlobalRestrictions
    {
        public void SetMaxConsecutiveDays(int value);
        public void SetMaxPerYear(int value);
        public void SetMaxPerMonth(int value);
        public void SetMaxPerQuarter(int value);
    }
}

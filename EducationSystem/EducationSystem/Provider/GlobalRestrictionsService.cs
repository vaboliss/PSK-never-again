
using EducationSystem.Interfaces;
using EducationSystem.Static;

namespace EducationSystem.Provider
{
    public class GlobalRestrictionsService : IGlobalRestrictions
    {
        public void SetMaxConsecutiveDays(int value)
        {
            if (value >= 0 && value <= GlobalRestrictions.MaxPerQuarter)
                GlobalRestrictions.MaxConsecutiveDays = value;
        }
        public void SetMaxPerYear(int value)
        {
            if (value >= 0 && value <= GlobalRestrictions.MaxPerQuarter * 4)
                GlobalRestrictions.MaxPerYear = value;
        }
        public void SetMaxPerMonth(int value)
        {
            if (value >= 0 && value <= GlobalRestrictions.MaxPerQuarter)
                GlobalRestrictions.MaxPerMonth = value;
        }
        public void SetMaxPerQuarter(int value)
        {
            GlobalRestrictions.MaxPerQuarter = value;
        }
    }
}

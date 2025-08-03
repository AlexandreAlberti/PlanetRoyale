using UnityEngine;

namespace Application.Utils
{
    public static class TimeUtils
    {
        public const int SecondsInHour = 3600;
        public static float TimeToGetNextInHours(float current, float regenRate)
        {
            // it having 1.3 whatever, you lack 0.7 to get next whatever point.
            // In hours you get 6 per hour, so 1/6 * 0.7 gives you the remaining time.
            int currentInt = Mathf.FloorToInt(current);
            return (1 - (current - currentInt)) / regenRate;
        }
        
        public static float TimeToGetNextInSeconds(float current, float regenRate)
        {
            return SecondsInHour * TimeToGetNextInHours(current, regenRate);
        }
    }
}
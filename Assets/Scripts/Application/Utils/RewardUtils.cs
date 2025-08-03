using Application.Data.Rewards;

namespace Application.Utils
{
    public static class RewardUtils
    {
        public static int RewardQuantityByType(LeagueReachedRewardsData leagueReachedRewardsData)
        {
            if (leagueReachedRewardsData.Equipments.Length > 0)
            {
                return leagueReachedRewardsData.Equipments.Length;
            }

            if (leagueReachedRewardsData.Materials.Length == 1)
            {
                return leagueReachedRewardsData.Materials[0].Amount;
            }

            if (leagueReachedRewardsData.Gold > 0)
            {
                return leagueReachedRewardsData.Gold;
            }
            
            if (leagueReachedRewardsData.Gems > 0)
            {
                return leagueReachedRewardsData.Gems;
            }
            
            if (leagueReachedRewardsData.SilverKeys > 0)
            {
                return leagueReachedRewardsData.SilverKeys;
            }
            
            if (leagueReachedRewardsData.GoldenKeys > 0)
            {
                return leagueReachedRewardsData.GoldenKeys;
            }
            
            return 0;
        }
        
    }
}
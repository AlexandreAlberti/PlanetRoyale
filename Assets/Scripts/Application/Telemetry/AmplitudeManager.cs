using System;
using System.Collections.Generic;
using Application.Data.Progression;
using Game.Reward;
using UnityEngine;

namespace Application.Telemetry
{
    public class AmplitudeManager : MonoBehaviour
    {
        // Dev Key = 353c803902da562fd7746f1e5cba1823
        // Prod Key = 8f6011ffd7913e76fd72cf4f5ff0c041 
        private const string ApiKey = "8f6011ffd7913e76fd72cf4f5ff0c041";
        private const string ServerUrl = "https://api2.amplitude.com";

        // Events
        private const string AppOpenEvent = "app_open";
        private const string InstallEvent = "install";
        private const string GameStartEvent = "game_start";
        private const string GameEndEvent = "game_end";
        private const string GameTransactionItemEvent = "game_transaction_item";
        private const string TutorialEvent = "ftue_flow";

        // Properties Common
        private const string AppIdProperty = "app_id";
        private const string TimestampProperty = "timestamp";
        // private const string DeviceIdProperty = "device_id";
        // private const string UserIdProperty = "user_id";

        // Properties Game 
        private const string GameLevelProperty = "game_id";
        private const string GameTypeProperty = "game_type";
        private const string LeagueProperty = "league";
        private const string OutcomeProperty = "outcome";
        private const string RankingProperty = "ranking";
        private const string GamesPlayedProperty = "games_played";
        private const string GameTimePlayedProperty = "game_time_played";
        private const string KillsProperty = "kills_quantity";
        private const string PlayerHealthProperty = "player_health";
        private const string PlayerScoreProperty = "points";
        private const string BossKillsProperty = "bosses_defeated";
        private const string IsFirstTimeProperty = "is_first_time";
        private const string ItemsCollectedProperty = "items_collected";

        // Properties Tutorial
        private const string TutorialCompletedProperty = "is_ftue_completed";
        private const string TutorialStepSkippedProperty = "is_step_skipped";
        private const string TutorialStepIdProperty = "tutorial_step_id";
        private const string TutorialStepNameProperty = "tutorial_step_name";
        private const string TutorialVersionProperty = "tutorial_version";
        private const string TutorialStepDurationProperty = "tutorial_step_duration";
        
        // GameTransactionItemProperties
        private const string GameTransactionItemReferenceTypeProperty = "reference_type";
        private const string GameTransactionItemReferenceSubtypeProperty = "reference_subtype";
        private const string GameTransactionItemReferenceContextProperty = "reference_context";
        private const string GameTransactionItemTransactionIdProperty = "transaction_id";
        private const string GameTransactionItemTransactionTypeProperty = "transaction_type";
        private const string GameTransactionItemTransactionSubtypeProperty = "transaction_subtype";
        private const string GameTransactionItemItemIdProperty = "item_id";
        private const string GameTransactionItemItemNameProperty = "item_name";
        private const string GameTransactionItemItemRarityProperty = "item_rarity";
        private const string GameTransactionItemItemCategoryProperty = "item_category";
        private const string GameTransactionItemItemQuantityProperty = "item_quantity";
        private const string GameTransactionItemItemBalanceProperty = "item_balance";

        private const string GameTransactionItemEquipmentScreenNameValue = "equipment";
        private const string GameTransactionItemTalentScreenNameValue = "talents";
        private const string GameTransactionItemGameEndValue = "game_end";
        private const string GameTransactionItemRewardValue = "reward";
        private const string GameTransactionItemSpendValue = "spend";
        private const string GameTransactionItemUpgradeValue = "upgrade";

        private const string GameTypeSingleplayer = "Singleplayer";
        private const string GameTypeMultiplayer = "Multiplayer";

        // Values
        private const string AppIdValue = "PR";

        // Other
        private const bool LoggingEnabled = true;
        private const bool TrackSessionEventsEnabled = true;

        public static AmplitudeManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            // Amplitude amplitude = Amplitude.getInstance();
            // amplitude.setServerUrl(ServerUrl);
            // amplitude.logging = LoggingEnabled;
            // amplitude.trackSessionEvents(TrackSessionEventsEnabled);
            // amplitude.init(ApiKey);
        }

        private void Start()
        {
            string userId = ProgressionDataManager.Instance.GetUserId();
            // Amplitude.getInstance().setUserId(userId);
            // Amplitude.getInstance().setDeviceId(userId);
            AppOpen();
        }

        private void AppOpen()
        {
            if (!ProgressionDataManager.Instance.IsRegisterDoneAmplitude())
            {
                // Amplitude.getInstance().logEvent(InstallEvent, GetEventProperties());
                ProgressionDataManager.Instance.SetRegisterDoneAmplitude();
            }

            // Amplitude.getInstance().logEvent(AppOpenEvent, GetEventProperties());
        }

        public void GameStart(int gamesPlayed, string planetName)
        {
            Dictionary<string,object> eventProperties = GetEventProperties();
            eventProperties.Add(GameLevelProperty, planetName);
            eventProperties.Add(GamesPlayedProperty, gamesPlayed);
            eventProperties.Add(IsFirstTimeProperty, gamesPlayed == 0);
            // Amplitude.getInstance().logEvent(GameStartEvent, eventProperties);
        }

        public void TalentUpgrade(string talentName, int talentLevel, int numberOfTalentUpgrades)
        {
            SendGameTransactionItemEvent(
                GameTransactionItemTalentScreenNameValue,
                null,
                null,
                DateTime.UtcNow.Ticks.ToString(),
                GameTransactionItemUpgradeValue,
                null,
                talentName,
                null,
                null,
                numberOfTalentUpgrades.ToString(),
                1,
                talentLevel
            );
        }

        public void EquipmentSlotUpgrade(string slotName, int slotLevel, int numberOfEquipmentUpgrades)
        {
            SendGameTransactionItemEvent(
                GameTransactionItemEquipmentScreenNameValue,
                null,
                null,
                DateTime.UtcNow.Ticks.ToString(),
                GameTransactionItemUpgradeValue,
                null,
                slotName,
                null,
                null,
                numberOfEquipmentUpgrades.ToString(),
                1,
                slotLevel
            );
        }

        public void GameEnd(string gameLevel, int levelIndex, string outcome, int ranking, int gamesPlayed, int secondsPassed, int kills, int playerHealth, int playerScore, int bossKills)
        {
            Dictionary<string, object> eventProperties = GetEventProperties();
            eventProperties.Add(GameLevelProperty, gameLevel);
            eventProperties.Add(GameTypeProperty, GameTypeSingleplayer);
            eventProperties.Add(LeagueProperty, levelIndex);
            eventProperties.Add(OutcomeProperty, outcome);
            eventProperties.Add(RankingProperty, ranking);
            eventProperties.Add(GamesPlayedProperty, gamesPlayed);
            eventProperties.Add(IsFirstTimeProperty, gamesPlayed == 1);
            eventProperties.Add(GameTimePlayedProperty, secondsPassed);
            eventProperties.Add(KillsProperty, kills);
            eventProperties.Add(PlayerHealthProperty, playerHealth);
            eventProperties.Add(PlayerScoreProperty, playerScore);
            eventProperties.Add(BossKillsProperty, bossKills);
            eventProperties.Add(ItemsCollectedProperty, RewardManager.Instance.ObtainRewardsJson());
            // Amplitude.getInstance().logEvent(GameEndEvent, eventProperties);
        }

        public void TutorialStep(bool isTutorialCompleted, bool isStepSkipped, int tutorialStepId, string tutorialStepName, string tutorialVersion, float tutorialStepDuration)
        {
            Dictionary<string, object> eventProperties = GetEventProperties();
            eventProperties.Add(TutorialCompletedProperty, isTutorialCompleted);
            eventProperties.Add(TutorialStepSkippedProperty, isStepSkipped);
            eventProperties.Add(TutorialStepIdProperty, tutorialStepId);
            eventProperties.Add(TutorialStepNameProperty, tutorialStepName);
            eventProperties.Add(TutorialVersionProperty, tutorialVersion);
            eventProperties.Add(TutorialStepDurationProperty, tutorialStepDuration);
            // Amplitude.getInstance().logEvent(TutorialEvent, eventProperties);
        }
        
        public void SoftCurrencyBalanceChange(SoftCurrencyType softType, int amountChanged, int totalBalance, SoftCurrencyModificationType reason)
        {
            string referenceType = reason == SoftCurrencyModificationType.LevelCompletedReward ? GameTransactionItemGameEndValue :
                reason == SoftCurrencyModificationType.EquipmentLevelUp ? GameTransactionItemEquipmentScreenNameValue :
                reason == SoftCurrencyModificationType.TalentUpgrade ? GameTransactionItemTalentScreenNameValue : null;
            string transactionType = reason == SoftCurrencyModificationType.LevelCompletedReward ? GameTransactionItemRewardValue : GameTransactionItemSpendValue;
            SendGameTransactionItemEvent(
                referenceType,
                null,
                null,
                DateTime.UtcNow.Ticks.ToString(),
                transactionType,
                null,
                softType.ToString(),
                null,
                null,
                softType.ToString(),
                amountChanged,
                totalBalance
            );
        }

        private void SendGameTransactionItemEvent(string referenceType, string referenceSubtype, string referenceContext,
            string transactionId, string transactionType, string transactionSubtype, string itemId,
            string itemName, string itemRarity, string itemCategory, int itemQuantity, int itemBalance)
        {
            Dictionary<string, object> eventProperties = GetEventProperties();
            eventProperties.Add(GameTransactionItemReferenceTypeProperty, referenceType);
            eventProperties.Add(GameTransactionItemReferenceSubtypeProperty, referenceSubtype);
            eventProperties.Add(GameTransactionItemReferenceContextProperty, referenceContext);
            eventProperties.Add(GameTransactionItemTransactionIdProperty, transactionId);
            eventProperties.Add(GameTransactionItemTransactionTypeProperty, transactionType);
            eventProperties.Add(GameTransactionItemTransactionSubtypeProperty, transactionSubtype);
            eventProperties.Add(GameTransactionItemItemIdProperty, itemId);
            eventProperties.Add(GameTransactionItemItemNameProperty, itemName);
            eventProperties.Add(GameTransactionItemItemRarityProperty, itemRarity);
            eventProperties.Add(GameTransactionItemItemCategoryProperty, itemCategory);
            eventProperties.Add(GameTransactionItemItemQuantityProperty, itemQuantity);
            eventProperties.Add(GameTransactionItemItemBalanceProperty, itemBalance);
            // Amplitude.getInstance().logEvent(GameTransactionItemEvent, eventProperties);
        }

        private Dictionary<string, object> GetEventProperties()
        {
            return new Dictionary<string, object>
            {
                { AppIdProperty, AppIdValue },
                { TimestampProperty, DateTime.UtcNow.Ticks }
            };
        }
    }
}
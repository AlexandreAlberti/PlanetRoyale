using Application.Data.Equipment;
using Application.Data.Progression;
using Game.Equipment;
using UnityEngine;
using MaterialReward = Application.Data.Rewards.MaterialReward;

namespace UI.MainMenu
{
    public class LeagueRewards : MonoBehaviour
    {
        [SerializeField] private EquipmentPieceButton _rewardEquipment;
        [SerializeField] private RewardCoinsHolder _rewardCoins;
        [SerializeField] private EquipmentMaterial _rewardMaterials;
        [SerializeField] private PlanetReward _planetReward;
        
        private Animator _animator;
        
        private static readonly int Normal = Animator.StringToHash("Normal");
        private static readonly int AppearTrigger = Animator.StringToHash("Appear");

        public void Initialize(LeagueData leagueData)
        {
            if (leagueData.LeagueReachedReward.Gold > 0)
            {
                _rewardEquipment.gameObject.SetActive(false);
                _rewardMaterials.gameObject.SetActive(false);
                _rewardCoins.gameObject.SetActive(true);
                _planetReward.gameObject.SetActive(false);
                
                _rewardCoins.Initialize(leagueData.LeagueReachedReward.Gold);
                _animator = _rewardCoins.GetComponent<Animator>();
                _animator.Play(Normal);
            }
            else if (leagueData.LeagueReachedReward.Materials.Length > 0)
            {
                _rewardEquipment.gameObject.SetActive(false);
                _rewardCoins.gameObject.SetActive(false);
                _rewardMaterials.gameObject.SetActive(true);
                _planetReward.gameObject.SetActive(false);
                
                MaterialReward materialReward = leagueData.LeagueReachedReward.Materials[0];
                _rewardMaterials.Initialize(materialReward.Type);
                _rewardMaterials.UpdateAmount(materialReward.Amount);
                _animator = _rewardMaterials.GetComponent<Animator>();
                _animator.Play(Normal);
            }
            else if (leagueData.LeagueReachedReward.Equipments.Length > 0)
            {
                _rewardCoins.gameObject.SetActive(false);
                _rewardMaterials.gameObject.SetActive(false);
                _rewardEquipment.gameObject.SetActive(true);
                _planetReward.gameObject.SetActive(false);
                
                EquipmentReward equipmentReward = leagueData.LeagueReachedReward.Equipments[0];
                EquipmentPieceInstance equipmentPieceInstance = EquipmentPieceFactory.GenerateEquipmentPieceById(equipmentReward.Type.Id);
                _rewardEquipment.Initialize(equipmentPieceInstance);
                _animator = _rewardEquipment.GetComponent<Animator>();
                _animator.Play(Normal);
            }
            else if (leagueData.LeagueReachedReward.Planets.Length > 0)
            {
                _rewardCoins.gameObject.SetActive(false);
                _rewardMaterials.gameObject.SetActive(false);
                _rewardEquipment.gameObject.SetActive(false);
                _planetReward.gameObject.SetActive(true);
                
                _planetReward.Initialize(leagueData.LeagueReachedReward.Planets[0]);
                _animator = _planetReward.GetComponent<Animator>();
                _animator.Play(Normal);
            }
        }

        public void Appear()
        {
            _animator.SetTrigger(AppearTrigger);
        }
    }
}
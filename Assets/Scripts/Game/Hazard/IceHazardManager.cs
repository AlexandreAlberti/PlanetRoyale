using Application.Data.Hazard;
using Game.Camera;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.Hazard
{
    public class IceHazardManager : MonoBehaviour
    {
        [SerializeField] private GameObject _iceHazardFogPrefab;
        public static IceHazardManager Instance { get; private set; }

        private float _fogFieldOfView;
        
        private void Awake()
        {
            Instance = this;
        }

        public void ApplyPlayerRangeHazard(IceHazardData data, float sphereRadius)
        {
            _fogFieldOfView = data.PlayerAreaOfViewDistance;
            Player player = PlayerManager.Instance.GetHumanPlayer();
            ApplyHazardToHumanPlayer(data, player, sphereRadius);

            foreach (Player npcPlayer in PlayerManager.Instance.GetNpcPlayersList())
            {
                ApplyHazardToNpcPlayer(data, npcPlayer);
            }
        }

        private void ApplyHazardToHumanPlayer(IceHazardData data, Player player, float sphereRadius)
        {
            GameObject fogInstance = Instantiate(_iceHazardFogPrefab, player.transform);
            fogInstance.transform.localPosition -= new Vector3(0, sphereRadius, 0);
            player.InitializeIceHazardFogFieldOfView(Mathf.Sin(_fogFieldOfView / sphereRadius) * sphereRadius);

            if (player is not PlayerRanged)
            {
                return;
            }

            float dataPlayerRangeMultiplier = data.PlayerRangeMultiplier;
            ApplyHazardToPlayer(dataPlayerRangeMultiplier, player);
            float factorToSee = player.GetFactorOfPlayerRadiusToSee();
            player.SetFactorOfPlayerRadiusToSee(factorToSee / dataPlayerRangeMultiplier);
            CameraManager.Instance.RecalculateMagnitudeConstantMultiplier();
        }
        
        private void ApplyHazardToNpcPlayer(IceHazardData data, Player player)
        {
            if (player is not PlayerRanged)
            {
                return;
            }
            
            ApplyHazardToPlayer(data.PlayerRangeMultiplier, player);
        }

        private void ApplyHazardToPlayer(float rangeMultiplier, Player player)
        {
            player.SetAttackRange(player.GetAttackRange() * rangeMultiplier);
        }

        public float GetFogFieldOfView()
        {
            return _fogFieldOfView;
        }
    }
}
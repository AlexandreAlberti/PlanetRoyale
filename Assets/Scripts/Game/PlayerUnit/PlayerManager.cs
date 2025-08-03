using System;
using System.Collections.Generic;
using Application.Data;
using Application.Data.Equipment;
using Game.Drop;
using Game.Enabler;
using Game.Equipment;
using Game.Map;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.PlayerUnit
{
    public class PlayerManager : EnablerMonoBehaviour
    {
        [Range(0.0f, 1.0f)] 
        [SerializeField] private float _xpPercentageToDropWhenDead;
        [SerializeField] private PlayerNpc _npc1Prefab;
        [SerializeField] private PlayerNpc _npc2Prefab;
        [SerializeField] private PlayerNpc _npc3Prefab;
        [Range(0, 3)]
        [SerializeField] private int _npcAmount;
        [SerializeField] private PlayerNpcConfigData[] _playerNpcConfigs;
        [SerializeField] private HeroPrefabManager _humanHeroPrefabManager;

        public static PlayerManager Instance;

        private List<Player> _players;
        private int _amountOfAlivePlayers;
        private Player _humanPlayer;
        private List<Player> _npcPlayers;

        public Action<int> OnPlayerDead;

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize(Vector3 sphereCenter, float sphereRadius)
        {
            _players = new List<Player>();
            _npcPlayers = new List<Player>();

            PlayerStartingPoint[] playerStartingPoints = FindObjectsOfType<PlayerStartingPoint>();
            int startingPointIndex = Random.Range(0, playerStartingPoints.Length);
            int playerIndex = Player.GetHumanPlayerIndex();

            WeaponType weaponType = EquipmentDataManager.Instance.GetEquippedWeaponType();
            PlayerHuman playerPrefab = _humanHeroPrefabManager.GetHeroPrefabByWeaponType(weaponType).GetComponent<PlayerHuman>();
            
            InstantiateHumanPlayer(playerPrefab, sphereCenter, sphereRadius, startingPointIndex, playerIndex, playerStartingPoints);
            
            List<string> npcNames = NameDataManager.Instance.GetGeneratedPlayerNpcNames(_npcAmount);
            
            if (_npcAmount >= 1)
            {
                InstantiateNpcPlayer(_npc1Prefab, sphereCenter, sphereRadius, (startingPointIndex + 1) % playerStartingPoints.Length, playerIndex + 1, playerStartingPoints, npcNames[0]);
            }

            if (_npcAmount >= 2)
            {
                InstantiateNpcPlayer(_npc2Prefab, sphereCenter, sphereRadius, (startingPointIndex + 2) % playerStartingPoints.Length, playerIndex + 2, playerStartingPoints, npcNames[1]);
            }

            if (_npcAmount >= 3)
            {
                InstantiateNpcPlayer(_npc3Prefab, sphereCenter, sphereRadius, (startingPointIndex + 3) % playerStartingPoints.Length, playerIndex + 3, playerStartingPoints, npcNames[2]);
            }

            foreach (Player player in _players)
            {
                player.OnDead += Player_OnDead;
            }

            foreach (PlayerStartingPoint playerStartingPoint in playerStartingPoints)
            {
                playerStartingPoint.gameObject.SetActive(false);
            }
            
            _amountOfAlivePlayers = _players.Count;
        }

        private void InstantiateHumanPlayer(PlayerHuman prefab, Vector3 sphereCenter, float sphereRadius, int startingPointIndex, int playerIndex, PlayerStartingPoint[] playerStartingPoints)
        {
            _humanPlayer = Instantiate(prefab).GetComponent<Player>();
            InitializePlayerPosition(startingPointIndex, playerStartingPoints, _humanPlayer);
            _humanPlayer.Initialize(playerIndex, sphereCenter, sphereRadius, NameDataManager.Instance.GetPlayerName());
            _players.Add(_humanPlayer);
        }

        private void InstantiateNpcPlayer(PlayerNpc prefab, Vector3 sphereCenter, float sphereRadius, int startingPointIndex, int playerIndex, PlayerStartingPoint[] playerStartingPoints, string playerName)
        {
            Player player = Instantiate(prefab).GetComponent<Player>();
            PlayerNpc npc = player.GetComponent<PlayerNpc>();
            npc.InitializeColor(_playerNpcConfigs[playerIndex - 1]);
            player.Initialize(playerIndex, sphereCenter, sphereRadius, playerName);
            InitializePlayerPosition(startingPointIndex, playerStartingPoints, player);
            _npcPlayers.Add(player);
            _players.Add(player);
        }

        private void InitializePlayerPosition(int startingPointIndex, PlayerStartingPoint[] playerStartingPoints, Player player)
        {
            player.transform.position = playerStartingPoints[startingPointIndex % playerStartingPoints.Length].transform.position;
            player.transform.rotation = playerStartingPoints[startingPointIndex % playerStartingPoints.Length].transform.rotation;
        }

        private void Player_OnDead(Player player)
        {
            _amountOfAlivePlayers--;
            OnPlayerDead?.Invoke(player.GetPlayerIndex());

            if (DropManager.Instance)
            {
                StartCoroutine(DropManager.Instance.GiveXpDropPrefabs(Mathf.FloorToInt(player.GetTotalXp() * _xpPercentageToDropWhenDead), player.transform.position, player.transform.rotation, player.transform.up));
            }
        }

        public Player GetHumanPlayer()
        {
            return _humanPlayer;
        }

        public List<Player> GetNpcPlayersList()
        {
            return _npcPlayers;
        }

        public Player GetPlayer(int index)
        {
            return _players[index];
        }

        public List<Player> GetPlayersList()
        {
            return _players;
        }

        public override void Enable()
        {
            base.Enable();
            EnablePlayers();
        }

        private void EnablePlayers()
        {
            _humanPlayer.Enable();

            foreach (Player npcPlayer in _npcPlayers)
            {
                npcPlayer.Enable();
            }
        }

        public override void Disable()
        {
            base.Disable();
            DisablePlayers();
        }

        private void DisablePlayers()
        {
            _humanPlayer.Disable();

            foreach (Player npcPlayer in _npcPlayers)
            {
                npcPlayer.Disable();
            }
        }

        public int GetAmountOfAlivePlayers()
        {
            return _amountOfAlivePlayers;
        }
    }
}
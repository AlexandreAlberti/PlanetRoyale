/// ---------------------------------------------
/// Shared Add-On for Behavior Designer Pro
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.AddOns.Shared.Runtime
{
    using Opsive.BehaviorDesigner.Runtime;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Manages the scenario selections for the Behavior Designer Pro Add-Ons.
    /// </summary>
    public class ScenarioSelector : MonoBehaviour
    {
        /// <summary>
        /// Details about the scenario.
        /// </summary>
        [System.Serializable]
        protected struct Scenario
        {
            [Tooltip("The start location of the agent.")]
            public Transform StartLocation;
            [Tooltip("The start location of the destination marker.")]
            public Transform DestinationLocation;
            [Tooltip("The parent transform of the camera. If empty the camera's starting location will be used.")]
            public Transform CameraParent;
            [Tooltip("The desription of the scenario.")]
            [Multiline] public string Description;
            [Tooltip("The index of the behavior tree that should be enabled on the destination marker. Set to 0 to disable.")]
            public int DestinationTreeIndex;
            [Tooltip("Specifies any GameObjects that should be activated.")]
            public GameObject[] ActiveTargets;
            [Tooltip("Specifies a component that should be enabled.")]
            public MonoBehaviour[] EnableTargets;
            [Tooltip("Should the player be enabled?")]
            public bool ShowPlayer;
            [Tooltip("The start location of the player. If empty the player's starting location will be used.")]
            public Transform PlayerStartLocation;
        }

        [Tooltip("A reference to the behavior tree agent.")]
        [SerializeField] protected GameObject m_Agent;
        [Tooltip("A reference to the destination marker.")]
        [SerializeField] protected GameObject m_Destination;
        [Tooltip("A reference to the player.")]
        [SerializeField] protected GameObject m_Player;
        [Tooltip("A reference to the UI title text.")]
        [SerializeField] protected Text m_TitleText;
        [Tooltip("A reference to the UI description text.")]
        [SerializeField] protected Text m_DescriptionText;
        [Tooltip("The scenario index.")]
        [SerializeField] protected int m_ActiveIndex;
        [Tooltip("Specifies details about each scenario.")]
        [SerializeField] protected Scenario[] m_Scenarios;

        private Transform m_AgentTransform;
        private Transform m_DestinationTransform;
        private IPathfindingAgent m_PathfindingAgent;

        private Transform m_PlayerTransform;
        private Vector3 m_PlayerStartPosition;
        private Quaternion m_PlayerStartRotation;
        private Transform m_CameraTransform;
        private Vector3 m_CameraStartPosition;
        private Quaternion m_CameraStartRotation;

        private BehaviorTree[] m_AgentBehaviorTrees;
        private BehaviorTree[] m_DestinationBehaviorTrees;

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        private void Awake()
        {
            m_AgentTransform = m_Agent.transform;
            m_PathfindingAgent = m_Agent.GetComponent<IPathfindingAgent>();
            if (m_Destination != null) {
                m_DestinationTransform = m_Destination.transform;
                m_DestinationBehaviorTrees = m_Destination.GetComponents<BehaviorTree>();
            }
            if (m_Player != null) {
                m_PlayerTransform = m_Player.transform;
                m_PlayerStartPosition = m_PlayerTransform.position;
                m_PlayerStartRotation = m_PlayerTransform.rotation;

                m_CameraTransform = Camera.main.transform;
                m_CameraStartPosition = m_CameraTransform.position;
                m_CameraStartRotation = m_CameraTransform.rotation;
            }
            for (int i = 0; i < m_Scenarios.Length; ++i) {
                if (m_Scenarios[i].ActiveTargets != null) {
                    for (int j = 0; j < m_Scenarios[i].ActiveTargets.Length; ++j) {
                        if (m_Scenarios[i].ActiveTargets[j] == null) {
                            continue;
                        }
                        m_Scenarios[i].ActiveTargets[j].SetActive(false);
                    }
                }
            }

            m_AgentBehaviorTrees = m_Agent.GetComponents<BehaviorTree>();
            Array.Sort(m_AgentBehaviorTrees, (a, b) =>
            {
                return a.Index.CompareTo(b.Index);
            });
            for (int i = 0; i < m_AgentBehaviorTrees.Length; ++i) {
                m_AgentBehaviorTrees[i].enabled = false;
            }

            EnableScenario(m_ActiveIndex);
        }

        /// <summary>
        /// Enables the sceario at the specified index.
        /// </summary>
        /// <param name="index"></param>
        private void EnableScenario(int index)
        {
            m_AgentBehaviorTrees[m_ActiveIndex].enabled = false;
            if (m_Scenarios[m_ActiveIndex].ActiveTargets != null) {
                for (int i = 0; i < m_Scenarios[m_ActiveIndex].ActiveTargets.Length; ++i) {
                    if (m_Scenarios[m_ActiveIndex].ActiveTargets[i] == null) {
                        continue;
                    }

                    m_Scenarios[m_ActiveIndex].ActiveTargets[i].SetActive(false);
                }
            }
            if (m_Scenarios[m_ActiveIndex].EnableTargets != null) {
                for (int i = 0; i < m_Scenarios[m_ActiveIndex].EnableTargets.Length; ++i) {
                    if (m_Scenarios[m_ActiveIndex].EnableTargets[i] == null) {
                        continue;
                    }

                    m_Scenarios[m_ActiveIndex].EnableTargets[i].enabled = false;
                }
            }
            if (m_Scenarios[m_ActiveIndex].ShowPlayer) {
                m_Player.SetActive(false);
            }
            EnableDisableDestinationTree(m_Scenarios[m_ActiveIndex].DestinationTreeIndex, false);

            m_ActiveIndex = index;
            // Reset the objects for the new scenario.
            var scenario = m_Scenarios[m_ActiveIndex];
            m_PathfindingAgent.Warp(scenario.StartLocation.position);
            m_AgentTransform.SetPositionAndRotation(scenario.StartLocation.position, scenario.StartLocation.rotation);
            if (m_Destination != null) {
                m_Destination.SetActive(scenario.DestinationLocation != null);
                if (scenario.DestinationLocation != null) {
                    m_Destination.transform.SetPositionAndRotation(scenario.DestinationLocation.position, scenario.DestinationLocation.rotation);
                }
            }
            if (scenario.ActiveTargets != null) {
                for (int i = 0; i < scenario.ActiveTargets.Length; ++i) {
                    if (scenario.ActiveTargets[i] == null) {
                        continue;
                    }

                    m_Scenarios[m_ActiveIndex].ActiveTargets[i].SetActive(true);
                }
            }
            if (m_Player != null && scenario.ShowPlayer) {
                if (scenario.PlayerStartLocation == null) {
                    m_PlayerTransform.SetPositionAndRotation(m_PlayerStartPosition, m_PlayerStartRotation);
                } else {
                    m_PlayerTransform.SetPositionAndRotation(scenario.PlayerStartLocation.position, scenario.PlayerStartLocation.rotation);
                }
                m_Player.SetActive(true);
            }
            if (scenario.CameraParent != null) {
                m_CameraTransform.parent = scenario.CameraParent.transform;
                m_CameraTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            } else if (m_CameraTransform != null) {
                m_CameraTransform.parent = null;
                m_CameraTransform.SetPositionAndRotation(m_CameraStartPosition, m_CameraStartRotation);
            }
            if (scenario.EnableTargets != null) {
                for (int i = 0; i < scenario.EnableTargets.Length; ++i) {
                    if (scenario.EnableTargets[i] == null) {
                        continue;
                    }

                    scenario.EnableTargets[i].enabled = true;
                }
            }
            m_TitleText.text = m_AgentBehaviorTrees[m_ActiveIndex].Name;
            m_DescriptionText.text = scenario.Description;
            EnableDisableDestinationTree(scenario.DestinationTreeIndex, true);
            m_AgentBehaviorTrees[m_ActiveIndex].enabled = true;
        }

        /// <summary>
        /// Resets the current scenario.
        /// </summary>
        public void ResetScenario()
        {
            EnableScenario(m_ActiveIndex);
        }

        /// <summary>
        /// Changes to the next or previous scenario.
        /// </summary>
        /// <param name="next">Should the next scenario be selected? If false the previous scenario will be selected.</param>
        public void ChangeScenario(bool next)
        {
            var nextIndex = (m_ActiveIndex + (next ? 1 : -1)) % m_AgentBehaviorTrees.Length;
            if (nextIndex < 0) nextIndex = m_AgentBehaviorTrees.Length - 1;
            EnableScenario(nextIndex);
        }

        /// <summary>
        /// Enables or disables the destination tree at the specified index.
        /// </summary>
        /// <param name="index">The index of the destination tree.</param>
        /// <param name="enable">Should the destination tree be enabled?</param>
        private void EnableDisableDestinationTree(int index, bool enable)
        {
            // All destination trees will have a positive value.
            if (index == 0) {
                return;
            }

            BehaviorTree destinationTree = null;
            for (int i = 0; i < m_DestinationBehaviorTrees.Length; ++i) {
                if (m_DestinationBehaviorTrees[i].Index == index) {
                    destinationTree = m_DestinationBehaviorTrees[i];
                    break;
                }
            }
            if (destinationTree == null) {
                return;
            }
            destinationTree.enabled = enable;
        }
    }
}
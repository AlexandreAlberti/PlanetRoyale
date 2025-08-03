/// ---------------------------------------------
/// Movement Pack for Behavior Designer Pro
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.AddOns.MovementPack.Editor
{
    using Opsive.BehaviorDesigner.AddOns.MovementPack.Runtime.Tasks;
    using Opsive.Shared.Editor.UIElements;
    using Opsive.Shared.Editor.UIElements.Controls;
    using Opsive.Shared.Editor.UIElements.Controls.Types;
    using Opsive.Shared.Utility;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// Implements TypeControlBase for the MovementBase type.
    /// </summary>
    [ControlType(typeof(MovementBase))]
    public class MovementBaseTypeControl : TypeControlBase
    {
        /// <summary>
        /// The MovementBaseView displays the contents of the MovementBase.
        /// </summary>
        public class MovementBaseView : VisualElement
        {
            private const string c_PathfinderIndexKey = "Opsive.BehaviorDesigner.MovementPack.PathfinderIndex";

            protected UnityEngine.Object m_UnityObject;
            protected MovementBase m_MovementBase;
            protected Func<object, bool> m_OnChangeEvent;

            private List<Type> m_PathfinderTypes = new List<Type>();
            private List<string> m_PathfinderTypeNames = new List<string>();

            private int m_PathfinderIndex = 0;
            private VisualElement m_PathfinderContainer;

            /// <summary>
            /// MovementBaseView constructor.
            /// </summary>
            /// <param name="unityObject">A reference to the owning Unity Object.</param>
            /// <param name="movementBase">The MovementBase task being drawn.</param>
            /// <param name="onChangeEvent">An event that is sent when the value changes. Returns false if the control cannot be changed.</param>
            public MovementBaseView(UnityEngine.Object unityObject, MovementBase movementBase, Func<object, bool> onChangeEvent)
            {
                m_UnityObject = unityObject;
                m_MovementBase = movementBase;
                m_OnChangeEvent = onChangeEvent;

                PopulatePathfinderTypes();

                // Set the NavMeshAgent implementation if the task doesn't already have one set.
                if (movementBase.Pathfinder == null) {
                    m_MovementBase.Pathfinder = Activator.CreateInstance(m_PathfinderTypes[m_PathfinderIndex]) as Pathfinder;
                    onChangeEvent?.Invoke(m_MovementBase);
                }

                // Add a popup with the different pathfinding choices.
                AddTitleLabel("Pathfinder", string.Empty);
                var pathfinderTypePopup = new PopupField<string>();
                pathfinderTypePopup.label = "Pathfinder";
                pathfinderTypePopup.tooltip = "Specifies the base pathfinding implementation that should be used.";
                pathfinderTypePopup.choices = m_PathfinderTypeNames;
                pathfinderTypePopup.value = m_PathfinderTypeNames[m_PathfinderIndex];
                pathfinderTypePopup.RegisterValueChangedCallback(c =>
                {
                    m_PathfinderIndex = pathfinderTypePopup.index;
                    EditorPrefs.SetInt(c_PathfinderIndexKey, m_PathfinderIndex);
                    m_MovementBase.Pathfinder = Activator.CreateInstance(m_PathfinderTypes[m_PathfinderIndex]) as Pathfinder;
                    
                    m_PathfinderContainer.Clear();
                    FieldInspectorView.AddFields(m_UnityObject, movementBase.Pathfinder, MemberVisibility.Public, m_PathfinderContainer, (object obj) => { onChangeEvent?.Invoke(obj); });

                    onChangeEvent?.Invoke(m_MovementBase);
                });
                Add(pathfinderTypePopup);

                // Show any pathfinder fields.
                m_PathfinderContainer = new VisualElement();
                m_PathfinderContainer.style.marginBottom = 4;
                FieldInspectorView.AddFields(m_UnityObject, movementBase.Pathfinder, MemberVisibility.Public, m_PathfinderContainer, (object obj) => { onChangeEvent?.Invoke(obj); });
                Add(m_PathfinderContainer);

                AddTitleLabel(m_MovementBase.GetType().Name, m_MovementBase.GetType().FullName);
                AddTaskFields();
            }

            /// <summary>
            /// Gets all of the pathfinding implementations available.
            /// </summary>
            private void PopulatePathfinderTypes()
            {
                m_PathfinderTypes.Clear();
                m_PathfinderTypeNames.Clear();
                var types = UnitOptions.GetAllTypes();
                for (int i = 0; i < types.Length; ++i) {
                    if (typeof(Pathfinder).IsAssignableFrom(types[i])) {
                        m_PathfinderTypes.Add(types[i]);
                        m_PathfinderTypeNames.Add(types[i].Name.Replace("Pathfinder", ""));
                    }
                }

                m_PathfinderIndex = Mathf.Clamp(EditorPrefs.GetInt(c_PathfinderIndexKey, 0), 0, m_PathfinderTypeNames.Count - 1);
            }

            /// <summary>
            /// Adds a title label with the specified text.
            /// </summary>
            /// <param name="text">The label text.</param>
            /// <param name="tooltip">The label tooltip.</param>
            private void AddTitleLabel(string text, string tooltip)
            {
                var label = new Label();
                label.enableRichText = true;
                label.style.marginLeft = 3;
                label.style.unityTextAlign = TextAnchor.MiddleCenter;
                label.text = $"<font-weight=700><size=+2>{text}</size></font-weight>";
                if (!string.IsNullOrEmpty(tooltip)) {
                    label.tooltip = tooltip;
                }
                Add(label);
            }

            /// <summary>
            /// Adds the remaining task fields.
            /// </summary>
            protected virtual void AddTaskFields()
            {
                FieldInspectorView.AddFields(m_UnityObject, m_MovementBase, MemberVisibility.Public, this, (object obj) => { m_OnChangeEvent?.Invoke(obj); });
            }
        }

        /// <summary>
        /// Does the control use a label?
        /// </summary>
        public override bool UseLabel { get { return false; } }

        /// <summary>
        /// Returns the control that should be used for the specified ControlType.
        /// </summary>
        /// <param name="input">The input to the control.</param>
        /// <returns>The created control.</returns>
        protected override VisualElement GetControl(TypeControlInput input)
        {
            return new MovementBaseView(input.UnityObject, input.Value as MovementBase, input.OnChangeEvent);
        }
    }
}
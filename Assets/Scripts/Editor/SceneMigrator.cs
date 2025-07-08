#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

using XRGraphicRaycaster = UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster;
using DefaultGraphicRaycaster = UnityEngine.UI.GraphicRaycaster;
using System.Collections.Generic;
using System.Linq;

public class SceneMigrator : EditorWindow
{
    abstract class MigrateType
    {
        public abstract Type From { get; }
        public abstract Type To { get; }

        public Component[] GetMigrateObjects() =>
            UnityEngine.Object
                .FindObjectsByType(From, FindObjectsSortMode.None)
                .Cast<Component>()
                .ToArray();

        public void Migrate(Component[] targetComponents)
        {
            foreach (var target in targetComponents)
            {
                if (target is Component oldComponent)
                {
                    var newComponent = Undo.AddComponent(oldComponent.gameObject, To);
                    MigrateComponent(oldComponent, newComponent);
                }
                else
                {
                    Debug.LogWarning($"Target {target.name} is not a Component, skipping migration.");
                }
            }
        }

        protected virtual void MigrateComponent(Component oldComponent, Component newComponent) { }
    }

    private class GenericMigrateType<TFrom, TTo> : MigrateType
        where TFrom : Component
        where TTo : Component
    {
        public override Type From => typeof(TFrom);
        public override Type To => typeof(TTo);

        Action<TFrom, TTo> _migrateAction;

        public GenericMigrateType(Action<TFrom, TTo> migrateAction)
        {
            _migrateAction = migrateAction;
        }

        protected override void MigrateComponent(Component oldComponent, Component newComponent)
        {
            if (oldComponent is TFrom oldTyped && newComponent is TTo newTyped)
            {
                Undo.RecordObject(oldTyped, "Migrating from " + From.Name + " to " + To.Name);
                Undo.RecordObject(newTyped, "Migrating to " + To.Name);
                _migrateAction?.Invoke(oldTyped, newTyped);
            }
        }
    }
    private class SwapType<TFrom, TTo> : GenericMigrateType<TFrom, TTo>
        where TFrom : Component
        where TTo : Component
    {
        public SwapType() : base(_MigrateAction) { }

        static void _MigrateAction(TFrom oldComponent, TTo newComponent)
        {
            Undo.DestroyObjectImmediate(oldComponent);
        }
    }
    private class AppendType<TFrom, TTo> : GenericMigrateType<TFrom, TTo>
        where TFrom : Component
        where TTo : Component
    {
        public AppendType() : base(_MigrateAction) { }

        static void _MigrateAction(TFrom oldComponent, TTo newComponent) { }
    }

    static MigrateType[] _MigrationTypes = new MigrateType[]
    {
        new SwapType<DefaultGraphicRaycaster, XRGraphicRaycaster>(),
    };
    static Dictionary<Type, Component[]> _MigrateObjects = new();
    static MigrateType _CurrentMigrateType = null;
    static int _UndoGroup;
    Vector2 _scrollPos;

    [MenuItem("Tools/Show Scene Migration Helper Window")]
    static void ShowWindow()
    {
        var window = GetWindow<SceneMigrator>();
        window.titleContent = new GUIContent("Scene Migration Helper");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (_CurrentMigrateType is null)
        {
            if (GUILayout.Button("Update All Migrate Objects"))
                UpdateMigrateObjects(_MigrationTypes);
        }
        else
        {
            if (GUILayout.Button("<<"))
                _CurrentMigrateType = null;
            if (GUILayout.Button("Update Migrate Objects"))
                UpdateMigrateObjects(_CurrentMigrateType);

            if (GUILayout.Button("Migrate All"))
            {
                var type = _CurrentMigrateType.GetType();
                _CurrentMigrateType.Migrate(_MigrateObjects[type]);
                _MigrateObjects.Remove(type);
                _CurrentMigrateType = null;
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        if (_CurrentMigrateType is null)
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            foreach (var migrateType in _MigrationTypes)
            {
                if (GUILayout.Button($"{migrateType.From.Name} -> {migrateType.To.Name}"))
                    _CurrentMigrateType = migrateType;
            }
            GUILayout.EndScrollView();
        }
        else
        {
            if (!_MigrateObjects.TryGetValue(_CurrentMigrateType.GetType(), out var components))
                GUILayout.Label("No objects to migrate.");
            else
            {
                _scrollPos = GUILayout.BeginScrollView(_scrollPos);
                foreach (var component in components)
                {
                    if (GUILayout.Button($"{component.name}"))
                        Selection.SetActiveObjectWithContext(component, null);
                }
                GUILayout.EndScrollView();
            }

        }
        GUILayout.EndVertical();
    }

    static void UpdateMigrateObjects(params MigrateType[] migrateTypes)
    {
        foreach (var migrateType in migrateTypes)
        {
            var type = migrateType.GetType();

            var targets = UnityEngine.Object.FindObjectsByType(migrateType.From, FindObjectsSortMode.None);
            _MigrateObjects[type] = migrateType.GetMigrateObjects();
        }
    }
}

#endif
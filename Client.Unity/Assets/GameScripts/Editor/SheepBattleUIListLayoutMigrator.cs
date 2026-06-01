using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class SheepBattleUIListLayoutMigrator
{
    private enum LayoutMode
    {
        Vertical,
        Horizontal,
        Grid
    }

    private sealed class ListConfig
    {
        public string PrefabPath;
        public string ListPath;
        public string TemplateName;
        public LayoutMode Mode;
        public Vector2 CellSize;
        public Vector2 Spacing;
        public int ConstraintCount;
        public bool HorizontalScroll;
        public bool VerticalScroll;
    }

    private static readonly ListConfig[] ListConfigs =
    {
        new()
        {
            PrefabPath = "Assets/AssetRaw/UI/BagUI.prefab",
            ListPath = "m_imgPanel/m_listItems",
            TemplateName = "m_btnItemTemplate",
            Mode = LayoutMode.Grid,
            CellSize = new Vector2(138f, 76f),
            Spacing = new Vector2(16f, 14f),
            ConstraintCount = 2,
            VerticalScroll = true
        },
        new()
        {
            PrefabPath = "Assets/AssetRaw/UI/MailUI.prefab",
            ListPath = "m_imgPanel/m_listMails",
            TemplateName = "m_btnMailTemplate",
            Mode = LayoutMode.Vertical,
            CellSize = new Vector2(0f, 64f),
            Spacing = new Vector2(0f, 10f),
            VerticalScroll = true
        },
        new()
        {
            PrefabPath = "Assets/AssetRaw/UI/MailUI.prefab",
            ListPath = "m_imgPanel/m_detailPanel/m_listAttachments",
            TemplateName = "m_btnAttachmentTemplate",
            Mode = LayoutMode.Horizontal,
            CellSize = new Vector2(74f, 74f),
            Spacing = new Vector2(8f, 0f),
            HorizontalScroll = true
        },
        new()
        {
            PrefabPath = "Assets/AssetRaw/UI/CharacterUI.prefab",
            ListPath = "m_imgPanel/m_listCharacters",
            TemplateName = "m_btnCharacterTemplate",
            Mode = LayoutMode.Vertical,
            CellSize = new Vector2(0f, 64f),
            Spacing = new Vector2(0f, 10f),
            VerticalScroll = true
        },
        new()
        {
            PrefabPath = "Assets/AssetRaw/UI/RewardPopupUI.prefab",
            ListPath = "m_imgPanel/m_listRewards",
            TemplateName = "m_btnRewardTemplate",
            Mode = LayoutMode.Grid,
            CellSize = new Vector2(104f, 124f),
            Spacing = new Vector2(18f, 18f),
            ConstraintCount = 5,
            VerticalScroll = true
        },
        new()
        {
            PrefabPath = "Assets/AssetRaw/UI/RoomListUI.prefab",
            ListPath = "m_imgPanel/m_listRooms",
            TemplateName = "m_btnRoomTemplate",
            Mode = LayoutMode.Vertical,
            CellSize = new Vector2(0f, 68f),
            Spacing = new Vector2(0f, 10f),
            VerticalScroll = true
        },
        new()
        {
            PrefabPath = "Assets/AssetRaw/UI/SocialUI.prefab",
            ListPath = "m_imgPanel/m_listPlayers",
            TemplateName = "m_btnPlayerTemplate",
            Mode = LayoutMode.Vertical,
            CellSize = new Vector2(0f, 64f),
            Spacing = new Vector2(0f, 10f),
            VerticalScroll = true
        }
    };

    [MenuItem("SheepBattle/Migrate Runtime UI List Layouts")]
    public static void MigrateAll()
    {
        foreach (var config in ListConfigs)
        {
            MigrateList(config);
        }

        ConfigureBattleHudLists("Assets/AssetRaw/UI/BattleMainUI.prefab");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("SheepBattle UI list layouts migrated.");
    }

    public static void MigrateAllAndExit()
    {
        try
        {
            MigrateAll();
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void MigrateList(ListConfig config)
    {
        var root = PrefabUtility.LoadPrefabContents(config.PrefabPath);
        try
        {
            var list = root.transform.Find(config.ListPath) as RectTransform;
            if (list == null)
            {
                Debug.LogWarning($"List path not found: {config.PrefabPath}/{config.ListPath}");
                return;
            }

            ConfigureScrollList(list, config);
            PrefabUtility.SaveAsPrefabAsset(root, config.PrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ConfigureScrollList(RectTransform list, ListConfig config)
    {
        var viewport = EnsureChild(list, "Viewport");
        Stretch(viewport);
        EnsureComponent<RectMask2D>(viewport.gameObject);

        var content = EnsureChild(viewport, "Content");
        ConfigureContentRect(content, config.Mode);

        for (var i = list.childCount - 1; i >= 0; i--)
        {
            var child = list.GetChild(i);
            if (child == viewport || child.name == "m_txtEmpty")
            {
                continue;
            }

            child.SetParent(content, false);
        }

        var template = FindDescendant(content, config.TemplateName);
        if (template != null)
        {
            template.SetParent(content, false);
            template.gameObject.SetActive(false);
            ConfigureTemplate(template as RectTransform, config);
        }

        ConfigureLayout(content.gameObject, config);

        var scroll = EnsureComponent<ScrollRect>(list.gameObject);
        scroll.viewport = viewport;
        scroll.content = content;
        scroll.horizontal = config.HorizontalScroll;
        scroll.vertical = config.VerticalScroll;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.inertia = true;
        scroll.scrollSensitivity = 24f;

        var empty = list.Find("m_txtEmpty") as RectTransform;
        if (empty != null)
        {
            Stretch(empty);
            empty.SetAsLastSibling();
        }
    }

    private static void ConfigureBattleHudLists(string prefabPath)
    {
        var root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            ConfigureHorizontalList(root.transform, "m_avatarList", "m_avatarTemplate", new Vector2(48f, 48f), 8f);
            ConfigureHorizontalList(root.transform, "m_cardList", "m_buildCardTemplate", new Vector2(132f, 82f), 10f);
            ConfigureHorizontalList(root.transform, "m_buttonList", "m_operationButtonTemplate", new Vector2(96f, 42f), 8f);
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ConfigureHorizontalList(Transform root, string listName, string templateName, Vector2 itemSize, float spacing)
    {
        var list = FindDescendant(root, listName) as RectTransform;
        if (list == null)
        {
            return;
        }

        var layout = EnsureSingleLayout<HorizontalLayoutGroup>(list.gameObject);
        layout.spacing = spacing;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        var fitter = EnsureComponent<ContentSizeFitter>(list.gameObject);
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

        var template = FindDescendant(list, templateName) as RectTransform;
        ConfigureTemplate(template, new ListConfig { CellSize = itemSize });
    }

    private static void ConfigureContentRect(RectTransform content, LayoutMode mode)
    {
        if (mode == LayoutMode.Horizontal)
        {
            content.anchorMin = new Vector2(0f, 0f);
            content.anchorMax = new Vector2(0f, 1f);
            content.pivot = new Vector2(0f, 0.5f);
            content.offsetMin = Vector2.zero;
            content.offsetMax = Vector2.zero;
            content.sizeDelta = Vector2.zero;
            return;
        }

        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.offsetMin = new Vector2(0f, 0f);
        content.offsetMax = new Vector2(0f, 0f);
        content.sizeDelta = Vector2.zero;
    }

    private static void ConfigureLayout(GameObject content, ListConfig config)
    {
        switch (config.Mode)
        {
            case LayoutMode.Horizontal:
            {
                var layout = EnsureSingleLayout<HorizontalLayoutGroup>(content);
                layout.spacing = config.Spacing.x;
                layout.childAlignment = TextAnchor.MiddleLeft;
                layout.childControlWidth = false;
                layout.childControlHeight = false;
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;
                var fitter = EnsureComponent<ContentSizeFitter>(content);
                fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                break;
            }
            case LayoutMode.Grid:
            {
                var layout = EnsureSingleLayout<GridLayoutGroup>(content);
                layout.cellSize = config.CellSize;
                layout.spacing = config.Spacing;
                layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
                layout.startAxis = GridLayoutGroup.Axis.Horizontal;
                layout.childAlignment = TextAnchor.UpperLeft;
                layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                layout.constraintCount = Mathf.Max(1, config.ConstraintCount);
                var fitter = EnsureComponent<ContentSizeFitter>(content);
                fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                break;
            }
            default:
            {
                var layout = EnsureSingleLayout<VerticalLayoutGroup>(content);
                layout.spacing = config.Spacing.y;
                layout.childAlignment = TextAnchor.UpperCenter;
                layout.childControlWidth = true;
                layout.childControlHeight = false;
                layout.childForceExpandWidth = true;
                layout.childForceExpandHeight = false;
                var fitter = EnsureComponent<ContentSizeFitter>(content);
                fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                break;
            }
        }
    }

    private static void ConfigureTemplate(RectTransform template, ListConfig config)
    {
        if (template == null)
        {
            return;
        }

        var layout = EnsureComponent<LayoutElement>(template.gameObject);
        if (config.CellSize.x > 0f)
        {
            layout.preferredWidth = config.CellSize.x;
        }

        if (config.CellSize.y > 0f)
        {
            layout.preferredHeight = config.CellSize.y;
        }
    }

    private static RectTransform EnsureChild(RectTransform parent, string name)
    {
        var child = parent.Find(name) as RectTransform;
        if (child != null)
        {
            return child;
        }

        var go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    private static T EnsureComponent<T>(GameObject go) where T : Component
    {
        return go.GetComponent<T>() ?? go.AddComponent<T>();
    }

    private static T EnsureSingleLayout<T>(GameObject go) where T : LayoutGroup
    {
        RemoveOtherLayoutGroups<T>(go);
        return EnsureComponent<T>(go);
    }

    private static void RemoveOtherLayoutGroups<T>(GameObject go) where T : Component
    {
        foreach (var component in go.GetComponents<LayoutGroup>())
        {
            if (component is not T)
            {
                UnityEngine.Object.DestroyImmediate(component);
            }
        }
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static Transform FindDescendant(Transform root, string name)
    {
        if (root == null)
        {
            return null;
        }

        for (var i = 0; i < root.childCount; i++)
        {
            var child = root.GetChild(i);
            if (child.name == name)
            {
                return child;
            }

            var match = FindDescendant(child, name);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }
}

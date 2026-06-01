using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class SheepBattleBattleMainUIMigrator
{
    private const string BattleMainPrefabPath = "Assets/AssetRaw/UI/BattleMainUI.prefab";

    [MenuItem("SheepBattle/Migrate Battle Main Troll UI")]
    public static void Migrate()
    {
        var root = PrefabUtility.LoadPrefabContents(BattleMainPrefabPath);
        try
        {
            var hud = FindDescendant(root.transform, "m_battleHud") as RectTransform;
            if (hud == null)
            {
                Debug.LogError("BattleMainUI.prefab missing m_battleHud.");
                return;
            }

            var elfPanel = EnsurePanel(hud, "m_elfPanel", new Color(0f, 0f, 0f, 0f));
            Stretch(elfPanel);
            MoveUnderIfFound(elfPanel, hud, "m_buildPanel");
            MoveUnderIfFound(elfPanel, hud, "m_buttonList");
            MoveUnderIfFound(elfPanel, hud, "m_txtBuildingInfo");
            var buildPanel = FindDescendant(hud, "m_buildPanel") as RectTransform;
            if (buildPanel != null)
            {
                buildPanel.gameObject.SetActive(false);
                buildPanel.SetSiblingIndex(hud.childCount - 1);
            }

            var resourcePanel = EnsurePanel(hud, "m_resourcePanel", new Color(0.05f, 0.06f, 0.07f, 0.78f));
            MoveUnderIfFound(hud, resourcePanel, "m_goldRow");
            MoveUnderIfFound(hud, resourcePanel, "m_woodRow");
            MoveUnderIfFound(elfPanel, resourcePanel, "m_goldRow");
            MoveUnderIfFound(elfPanel, resourcePanel, "m_woodRow");

            var skillPanel = EnsureRect(hud, "m_skillPanel");
            Anchor(skillPanel, 1f, 0f, 1f, 0f, -88f, 196f, 96f, 220f);
            skillPanel.SetSiblingIndex(hud.childCount - 1);
            EnsureSkillButtons(skillPanel);

            var playerInfoPanel = EnsurePanel(hud, "m_playerInfoPanel", new Color(0.09f, 0.11f, 0.12f, 0.82f));
            Anchor(playerInfoPanel, 0.5f, 0f, 0.5f, 0f, 0f, 86f, 560f, 132f);
            playerInfoPanel.SetSiblingIndex(hud.childCount - 1);
            MoveUnderIfFound(hud, playerInfoPanel, "m_equipmentSlots");
            MoveUnderIfFound(hud, playerInfoPanel, "m_txtTrollStats");
            RenameIfFound(playerInfoPanel, "m_txtTrollStats", "m_txtPlayerStats");
            EnsureText(playerInfoPanel, "m_txtPlayerStats", "玩家信息", 18, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(280f, -24f), new Vector2(-20f, 30f));
            DestroyIfFound(playerInfoPanel, "m_btnBattleShop");
            DestroyIfFound(hud, "m_trollPanel");
            var equipmentSlots = EnsureRect(playerInfoPanel, "m_equipmentSlots");
            SetRect(equipmentSlots, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(204f, 44f), new Vector2(-18f, 58f));
            EnsureEquipmentSlots(equipmentSlots);

            var shopPanel = EnsurePanel(hud, "m_trollShopPanel", new Color(0.94f, 0.96f, 0.94f, 0.98f));
            Anchor(shopPanel, 1f, 0f, 1f, 0f, -268f, 272f, 448f, 330f);
            shopPanel.gameObject.SetActive(false);
            shopPanel.SetSiblingIndex(hud.childCount - 1);
            EnsureText(shopPanel, "m_txtBattleShopTitle", "局内商店", 24, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(110f, -28f), new Vector2(-132f, 44f));
            EnsureButton(shopPanel, "m_btnBattleShopClose", "关闭", new Color(0.35f, 0.37f, 0.36f, 1f),
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-54f, -28f), new Vector2(84f, 38f));
            EnsureText(shopPanel, "m_txtBattleShopHint", string.Empty, 18, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.16f, 0.16f, 0.16f, 1f),
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(206f, -72f), new Vector2(-32f, 34f));
            var goodsRoot = EnsureRect(shopPanel, "m_battleShopGoods");
            SetRect(goodsRoot, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, -42f), new Vector2(-24f, -118f));
            EnsureVerticalGoodsList(goodsRoot);
            EnsureGoodsTemplate(goodsRoot);

            if (buildPanel != null)
            {
                buildPanel.SetAsLastSibling();
            }

            PrefabUtility.SaveAsPrefabAsset(root, BattleMainPrefabPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("BattleMainUI prefab nodes migrated.");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    public static void MigrateAndExit()
    {
        try
        {
            Migrate();
            EditorApplication.Exit(0);
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static RectTransform EnsurePanel(RectTransform parent, string name, Color color)
    {
        var rect = EnsureRect(parent, name);
        var image = rect.GetComponent<Image>() ?? rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = color.a > 0f;
        return rect;
    }

    private static RectTransform EnsureRect(Transform parent, string name)
    {
        var existing = FindDescendant(parent, name) as RectTransform;
        if (existing != null)
        {
            return existing;
        }

        var go = new GameObject(name, typeof(RectTransform));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    private static Text EnsureText(Transform parent, string name, string value, int size, FontStyle style, TextAnchor alignment, Color color,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        var rect = EnsureRect(parent, name);
        var text = rect.GetComponent<Text>() ?? rect.gameObject.AddComponent<Text>();
        EnsureComponent<CanvasRenderer>(rect.gameObject);
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.text = value;
        SetRect(rect, anchorMin, anchorMax, anchoredPosition, sizeDelta);
        return text;
    }

    private static Button EnsureButton(Transform parent, string name, string label, Color color,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        var rect = EnsurePanel(parent as RectTransform, name, color);
        SetRect(rect, anchorMin, anchorMax, anchoredPosition, sizeDelta);
        var button = rect.GetComponent<Button>() ?? rect.gameObject.AddComponent<Button>();
        button.targetGraphic = rect.GetComponent<Image>();
        var labelText = EnsureText(rect, "m_txtLabel", label, 18, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Stretch(labelText.rectTransform);
        return button;
    }

    private static void EnsureEquipmentSlots(RectTransform root)
    {
        RemoveUnexpectedEquipmentSlots(root);

        var layout = EnsureSingleLayout<HorizontalLayoutGroup>(root.gameObject);
        layout.spacing = 7f;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        for (var i = 0; i < 6; i++)
        {
            var slot = EnsurePanel(root, $"m_equipmentSlot_{i}", new Color(0.16f, 0.18f, 0.2f, 0.95f));
            var layoutElement = EnsureComponent<LayoutElement>(slot.gameObject);
            layoutElement.preferredWidth = 58f;
            layoutElement.preferredHeight = 58f;
            EnsureText(slot, "m_txtLabel", "空", 15, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Stretch((FindDescendant(slot, "m_txtLabel") as RectTransform)!);
        }
    }

    private static void RemoveUnexpectedEquipmentSlots(RectTransform root)
    {
        for (var i = root.childCount - 1; i >= 0; i--)
        {
            var child = root.GetChild(i);
            if (!child.name.StartsWith("m_equipmentSlot_", System.StringComparison.Ordinal))
            {
                continue;
            }

            var keep = false;
            for (var slotIndex = 0; slotIndex < 6; slotIndex++)
            {
                if (child.name == $"m_equipmentSlot_{slotIndex}")
                {
                    keep = true;
                    break;
                }
            }

            if (!keep)
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }
    }

    private static void EnsureVerticalGoodsList(RectTransform root)
    {
        var layout = EnsureSingleLayout<VerticalLayoutGroup>(root.gameObject);
        layout.spacing = 8f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        var fitter = EnsureComponent<ContentSizeFitter>(root.gameObject);
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private static void EnsureGoodsTemplate(RectTransform parent)
    {
        var template = EnsureButton(parent, "m_btnBattleGoodsTemplate", string.Empty, new Color(0.84f, 0.87f, 0.82f, 1f),
            new Vector2(0f, 1f), new Vector2(1f, 1f), Vector2.zero, new Vector2(0f, 58f));
        var rect = template.GetComponent<RectTransform>();
        var layoutElement = EnsureComponent<LayoutElement>(template.gameObject);
        layoutElement.preferredHeight = 58f;
        var label = FindDescendant(rect, "m_txtLabel")?.GetComponent<Text>();
        if (label != null)
        {
            label.fontSize = 16;
            label.fontStyle = FontStyle.Normal;
            label.alignment = TextAnchor.MiddleLeft;
            label.color = Color.black;
            label.rectTransform.offsetMin = new Vector2(12f, 4f);
            label.rectTransform.offsetMax = new Vector2(-12f, -4f);
        }

        template.gameObject.SetActive(false);
    }

    private static void EnsureSkillButtons(RectTransform root)
    {
        var layout = EnsureSingleLayout<VerticalLayoutGroup>(root.gameObject);
        layout.spacing = 12f;
        layout.childAlignment = TextAnchor.LowerCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        EnsureSkillButton(root, "m_btnSecondarySkill", "取消", new Color(0.28f, 0.31f, 0.36f, 0.96f), 70f);
        EnsureSkillButton(root, "m_btnPrimarySkill", "建造", new Color(0.20f, 0.48f, 0.32f, 0.98f), 86f);
    }

    private static void EnsureSkillButton(RectTransform parent, string name, string label, Color color, float size)
    {
        var button = EnsureButton(parent, name, label, color, Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(size, size));
        var rect = button.GetComponent<RectTransform>();
        var layoutElement = EnsureComponent<LayoutElement>(button.gameObject);
        layoutElement.preferredWidth = size;
        layoutElement.preferredHeight = size;
        rect.localScale = Vector3.one;
        var image = button.GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = true;
        }

        var labelText = FindDescendant(rect, "m_txtLabel")?.GetComponent<Text>();
        if (labelText != null)
        {
            labelText.fontSize = 18;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.rectTransform.offsetMin = new Vector2(4f, 4f);
            labelText.rectTransform.offsetMax = new Vector2(-4f, -4f);
        }
    }

    private static void DestroyIfFound(Transform root, string name)
    {
        var child = FindDescendant(root, name);
        if (child != null)
        {
            Object.DestroyImmediate(child.gameObject);
        }
    }

    private static void RenameIfFound(Transform root, string currentName, string newName)
    {
        var child = FindDescendant(root, currentName);
        if (child != null)
        {
            child.name = newName;
        }
    }

    private static void MoveUnderIfFound(RectTransform searchRoot, RectTransform targetParent, string childName)
    {
        var child = FindDescendant(searchRoot, childName);
        if (child == null || child == targetParent)
        {
            return;
        }

        child.SetParent(targetParent, true);
    }

    private static T EnsureComponent<T>(GameObject go) where T : Component
    {
        return go.GetComponent<T>() ?? go.AddComponent<T>();
    }

    private static T EnsureSingleLayout<T>(GameObject go) where T : LayoutGroup
    {
        foreach (var layout in go.GetComponents<LayoutGroup>())
        {
            if (layout is not T)
            {
                Object.DestroyImmediate(layout);
            }
        }

        return EnsureComponent<T>(go);
    }

    private static void Anchor(RectTransform rect, float minX, float minY, float maxX, float maxY, float x, float y, float width, float height)
    {
        rect.anchorMin = new Vector2(minX, minY);
        rect.anchorMax = new Vector2(maxX, maxY);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
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

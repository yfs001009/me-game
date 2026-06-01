using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class SheepBattleChatUIPrefabMigrator
{
    [MenuItem("SheepBattle/Migrate Chat UI Prefabs")]
    public static void Migrate()
    {
        AddLobbyChatButton();
        SheepBattleRuntimeUIPrefabBuilder.BuildChatUI();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("SheepBattle chat UI prefabs migrated.");
    }

    public static void MigrateAndExit()
    {
        Migrate();
        EditorApplication.Exit(0);
    }

    private static void AddLobbyChatButton()
    {
        const string path = "Assets/AssetRaw/UI/LobbyUI.prefab";
        var root = PrefabUtility.LoadPrefabContents(path);
        try
        {
            var bottomBar = root.transform.Find("m_bottomBar") as RectTransform;
            if (bottomBar == null)
            {
                return;
            }

            var chatButton = bottomBar.Find("m_btnChat") as RectTransform;
            if (chatButton == null)
            {
                var button = CreateButton("m_btnChat", bottomBar, "聊天", new Color(0.11f, 0.20f, 0.25f, 0.86f));
                chatButton = button.GetComponent<RectTransform>();
            }

            ConfigureChatPreviewButton(chatButton);
            MoveButton(bottomBar, "m_btnBag", -95f, 89f);
            MoveButton(bottomBar, "m_btnCard", -228.5f, 87f);
            MoveButton(bottomBar, "m_btnHero", -406f, 87f);
            PrefabUtility.SaveAsPrefabAsset(root, path);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void MoveButton(Transform parent, string name, float x, float y)
    {
        var rect = parent.Find(name) as RectTransform;
        if (rect != null)
        {
            rect.anchoredPosition = new Vector2(x, y);
        }
    }

    private static void ConfigureChatPreviewButton(RectTransform chatButton)
    {
        chatButton.anchorMin = new Vector2(0f, 0f);
        chatButton.anchorMax = new Vector2(0f, 0f);
        chatButton.pivot = new Vector2(0.5f, 0.5f);
        chatButton.sizeDelta = new Vector2(428f, 58f);
        chatButton.anchoredPosition = new Vector2(246f, 82f);

        if (chatButton.TryGetComponent<Image>(out var image))
        {
            image.color = new Color(0.08f, 0.12f, 0.15f, 0.78f);
        }

        var icon = chatButton.Find("m_txtLabel")?.GetComponent<Text>();
        if (icon == null)
        {
            var iconGo = new GameObject("m_txtLabel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            iconGo.layer = LayerMask.NameToLayer("UI");
            iconGo.transform.SetParent(chatButton, false);
            icon = iconGo.GetComponent<Text>();
        }

        icon.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        icon.fontSize = 24;
        icon.fontStyle = FontStyle.Bold;
        icon.alignment = TextAnchor.MiddleCenter;
        icon.color = Color.white;
        icon.text = "聊";
        icon.raycastTarget = false;
        var iconRect = icon.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0f, 0.5f);
        iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.sizeDelta = new Vector2(44f, 44f);
        iconRect.anchoredPosition = new Vector2(30f, 0f);

        var latest = chatButton.Find("m_txtLatestChat")?.GetComponent<Text>();
        if (latest == null)
        {
            var existing = chatButton.parent.Find("m_txtLatestChat");
            if (existing != null)
            {
                existing.SetParent(chatButton, false);
                latest = existing.GetComponent<Text>();
            }
        }

        if (latest == null)
        {
            var latestGo = new GameObject("m_txtLatestChat", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            latestGo.layer = LayerMask.NameToLayer("UI");
            latestGo.transform.SetParent(chatButton, false);
            latest = latestGo.GetComponent<Text>();
        }

        var rect = latest.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.sizeDelta = new Vector2(330f, 42f);
        rect.anchoredPosition = new Vector2(68f, 0f);

        latest.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        latest.fontSize = 20;
        latest.fontStyle = FontStyle.Normal;
        latest.alignment = TextAnchor.MiddleLeft;
        latest.color = new Color(0.94f, 0.96f, 0.92f, 0.95f);
        latest.horizontalOverflow = HorizontalWrapMode.Wrap;
        latest.verticalOverflow = VerticalWrapMode.Truncate;
        latest.text = string.Empty;
        latest.raycastTarget = false;
    }

    private static Button CreateButton(string name, Transform parent, string label, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.color = color;
        var button = go.GetComponent<Button>();
        button.targetGraphic = image;

        var textGo = new GameObject("m_txtLabel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textGo.layer = LayerMask.NameToLayer("UI");
        textGo.transform.SetParent(go.transform, false);
        var text = textGo.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 24;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.text = label;
        var textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        return button;
    }
}

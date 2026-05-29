using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class SheepBattleUIPrefabBuilder
{
    private const string UiDir = "Assets/AssetRaw/UI";
    private const string LobbyPath = UiDir + "/LobbyUI.prefab";
    private const string CreateRoomPath = UiDir + "/CreateRoomUI.prefab";
    private const string RoomPlayerSlotPath = UiDir + "/RoomPlayerSlot.prefab";

    [InitializeOnLoadMethod]
    private static void BuildMissingPrefabsOnEditorLoad()
    {
        EditorApplication.delayCall += () =>
        {
            if (Application.isBatchMode)
            {
                return;
            }

            var hasLobby = IsLobbyPrefabComplete();
            var hasRoomPrefabs = File.Exists(CreateRoomPath) && File.Exists(RoomPlayerSlotPath);
            if (hasLobby && hasRoomPrefabs)
            {
                return;
            }

            if (!hasLobby)
            {
                Debug.Log("SheepBattle LobbyUI prefab is missing or outdated. Building LobbyUI prefab.");
                BuildLobbyUI();
            }

            if (!hasRoomPrefabs)
            {
                Debug.Log("SheepBattle room UI prefabs are missing. Building room prefabs.");
                BuildCreateRoomUI();
                BuildRoomUI();
                BuildRoomPlayerSlot();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        };
    }

    [MenuItem("SheepBattle/Build UI Prefabs")]
    public static void BuildAll()
    {
        Directory.CreateDirectory(UiDir);
        BuildLobbyUI();
        BuildCreateRoomUI();
        BuildRoomUI();
        BuildRoomPlayerSlot();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("SheepBattle/Build Lobby UI Prefab")]
    public static void BuildLobbyUI()
    {
        var root = CreateRoot("LobbyUI");
        var bg = CreateImage("m_imgBackground", root.transform, Color.white);
        Stretch(bg.rectTransform);
        ApplySprite(bg, "Assets/AssetRaw/UI/Art/lobby_bg_fortified_camp.png", Image.Type.Simple);

        var topBar = CreateRect("m_topBar", root.transform);
        SetStretchTop(topBar, 28f, -98f, -28f, 82f);

        var playerPanel = CreateImage("m_playerPanel", topBar, new Color(0f, 0f, 0f, 0.40f));
        SetAnchor(playerPanel.rectTransform, 0f, 0.5f, 0f, 0.5f, 170f, 0f, 340f, 72f);

        var avatar = CreateImage("m_imgAvatar", playerPanel.transform, new Color(0.95f, 0.73f, 0.38f, 1f));
        SetAnchor(avatar.rectTransform, 0f, 0.5f, 0f, 0.5f, 42f, 0f, 58f, 58f);
        var avatarText = CreateText("m_txtAvatar", avatar.transform, 26, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(avatarText.rectTransform);
        avatarText.text = "羊";

        var playerName = CreateText("m_txtPlayerName", playerPanel.transform, 21, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
        SetAnchor(playerName.rectTransform, 0f, 0.5f, 1f, 0.5f, 120f, 12f, -146f, 30f);
        playerName.text = "玩家";

        var level = CreateText("m_txtLevel", playerPanel.transform, 17, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(1f, 0.89f, 0.52f, 1f));
        SetAnchor(level.rectTransform, 0f, 0.5f, 1f, 0.5f, 120f, -15f, -146f, 26f);
        level.text = "Lv.1";

        CreateCurrency("m_currencyGold", topBar, "Assets/AssetRaw/UI/Art/icon/ic_coin.png", "12,800", -438f);
        CreateCurrency("m_currencyGem", topBar, "Assets/AssetRaw/UI/Art/icon/ic_gem.png", "680", -248f);
        CreateCurrency("m_currencyCrystal", topBar, "Assets/AssetRaw/UI/Art/icon/ic_crystal.png", "96", -58f);

        CreateSmallButton("m_btnMail", topBar, "邮件", -190f);
        CreateSmallButton("m_btnSettings", topBar, "设置", -64f);

        var mainPanel = CreateImage("m_mainPanel", root.transform, new Color(1f, 0.96f, 0.82f, 0.88f));
        SetCenter(mainPanel.rectTransform, 0f, 18f, 680f, 300f);
        ApplySprite(mainPanel, "Assets/AssetRaw/UI/Art/lobby_panel.png", Image.Type.Sliced);

        var title = CreateText("m_txtTitle", mainPanel.transform, 34, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.12f, 0.10f, 0.08f, 1f));
        SetAnchor(title.rectTransform, 0.5f, 1f, 0.5f, 1f, 0f, -58f, 400f, 52f);
        title.text = "羊群战场";

        var summary = CreateText("m_txtLobbySummary", mainPanel.transform, 22, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.12f, 0.10f, 0.08f, 1f));
        SetCenter(summary.rectTransform, 0f, 36f, 560f, 72f);
        summary.text = "大厅数据加载中";

        var status = CreateText("m_txtStatus", mainPanel.transform, 19, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.37f, 0.31f, 0.22f, 1f));
        SetCenter(status.rectTransform, 0f, -30f, 560f, 44f);
        status.text = "状态：等待进入大厅";

        CreateStat("m_statRooms", mainPanel.transform, "房间", "- 个", -190f);
        CreateStat("m_statMatch", mainPanel.transform, "匹配", "空闲", 0f);
        CreateStat("m_statLoadout", mainPanel.transform, "卡组", "默认 6 张", 190f);

        var bottomBar = CreateRect("m_bottomBar", root.transform);
        SetStretchBottom(bottomBar, 30f, 28f, -30f, 150f);
        CreateLargeButton("m_btnStartMatch", bottomBar, "匹配", -312f, "Assets/AssetRaw/UI/Art/button_primary_green.png");
        CreateLargeButton("m_btnCreateRoom", bottomBar, "自定义", -156f, "Assets/AssetRaw/UI/Art/button_secondary_blue.png");
        CreateLargeButton("m_btnRoomList", bottomBar, "房间", 0f, "Assets/AssetRaw/UI/Art/button_secondary_blue.png");
        CreateLargeButton("m_btnCards", bottomBar, "卡片", 156f, "Assets/AssetRaw/UI/Art/button_secondary_blue.png");
        CreateLargeButton("m_btnRefresh", bottomBar, "刷新", 312f, "Assets/AssetRaw/UI/Art/button_secondary_blue.png");

        var sideMenu = CreateRect("m_sideMenu", root.transform);
        SetAnchor(sideMenu, 1f, 0.5f, 1f, 0.5f, -78f, -34f, 108f, 300f);
        CreateSideButton("m_btnShop", sideMenu, "商店", 90f);
        CreateSideButton("m_btnBag", sideMenu, "背包", 0f);
        CreateSideButton("m_btnCardsSide", sideMenu, "卡片", -90f);

        SavePrefab(root, LobbyPath);
    }

    public static void BuildCreateRoomUI()
    {
        var root = CreateRoot("CreateRoomUI");
        var mask = CreateImage("m_imgMask", root.transform, new Color(0f, 0f, 0f, 0.55f));
        Stretch(mask.rectTransform);

        var panel = CreateImage("m_imgPanel", root.transform, new Color(0.94f, 0.96f, 0.94f, 1f));
        SetCenter(panel.rectTransform, 0f, 0f, 680f, 520f);

        var title = CreateText("m_txtTitle", panel.transform, 30, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black);
        SetCenter(title.rectTransform, 0f, 202f, 600f, 58f);
        title.text = "创建自定义房间";

        var roomName = CreateInput("m_inputRoomName", panel.transform, "房间名称", "默认房间");
        SetCenter(roomName.GetComponent<RectTransform>(), 0f, 124f, 420f, 54f);

        var maxPlayers = CreateInput("m_inputMaxPlayers", panel.transform, "最大人数", "4");
        SetCenter(maxPlayers.GetComponent<RectTransform>(), 0f, 54f, 420f, 54f);

        var toggle = CreateToggle("m_togglePrivate", panel.transform, "私密房间");
        SetCenter(toggle.GetComponent<RectTransform>(), -118f, -16f, 184f, 46f);

        var password = CreateInput("m_inputPassword", panel.transform, "房间密码", string.Empty);
        SetCenter(password.GetComponent<RectTransform>(), 0f, -82f, 420f, 54f);

        var hint = CreateText("m_txtHint", panel.transform, 20, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.15f, 0.15f, 0.15f, 1f));
        SetCenter(hint.rectTransform, 0f, -148f, 560f, 44f);
        hint.text = "人数范围：2-8";

        var createButton = CreateButton("m_btnCreate", panel.transform, "创建", new Color(0.22f, 0.44f, 0.78f, 1f));
        SetCenter(createButton.GetComponent<RectTransform>(), -96f, -218f, 160f, 54f);

        var closeButton = CreateButton("m_btnClose", panel.transform, "取消", new Color(0.35f, 0.37f, 0.36f, 1f));
        SetCenter(closeButton.GetComponent<RectTransform>(), 96f, -218f, 160f, 54f);

        SavePrefab(root, CreateRoomPath);
    }

    public static void BuildRoomUI()
    {
        var root = CreateRoot("RoomUI");
        var bg = CreateImage("m_imgBackground", root.transform, new Color(0.9f, 0.94f, 0.92f, 1f));
        Stretch(bg.rectTransform);

        var title = CreateText("m_txtRoomTitle", root.transform, 30, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black);
        SetCenter(title.rectTransform, 0f, 126f, 680f, 56f);
        title.text = "房间";

        var info = CreateText("m_txtRoomInfo", root.transform, 22, FontStyle.Normal, TextAnchor.MiddleCenter, Color.black);
        SetCenter(info.rectTransform, 0f, 64f, 680f, 52f);
        info.text = "房间信息待加载";

        var state = CreateText("m_txtRoomState", root.transform, 20, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.16f, 0.16f, 0.16f, 1f));
        SetCenter(state.rectTransform, 0f, 8f, 680f, 52f);
        state.text = "状态：等待进入房间";

        var center = CreatePanel("m_panelCenter", root.transform, new Color(0.84f, 0.9f, 0.88f, 1f));
        SetCenter(center, 0f, -126f, 720f, 300f);

        var scrollGo = new GameObject("m_scrollPlayers", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
        scrollGo.transform.SetParent(center, false);
        var scrollRect = scrollGo.GetComponent<RectTransform>();
        Stretch(scrollRect);
        scrollGo.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.45f);

        var viewportGo = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
        viewportGo.transform.SetParent(scrollGo.transform, false);
        var viewportRect = viewportGo.GetComponent<RectTransform>();
        Stretch(viewportRect);
        viewportGo.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.04f);
        viewportGo.GetComponent<Mask>().showMaskGraphic = false;

        var contentGo = new GameObject("Content", typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter));
        contentGo.transform.SetParent(viewportGo.transform, false);
        var contentRect = contentGo.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0f, 0f);
        var layout = contentGo.GetComponent<GridLayoutGroup>();
        layout.padding = new RectOffset(16, 16, 16, 16);
        layout.spacing = new Vector2(10f, 10f);
        layout.cellSize = new Vector2(329f, 72f);
        layout.startAxis = GridLayoutGroup.Axis.Horizontal;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = 2;
        var fitter = contentGo.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var scroll = scrollGo.GetComponent<ScrollRect>();
        scroll.viewport = viewportRect;
        scroll.content = contentRect;
        scroll.horizontal = false;
        scroll.vertical = true;

        var startButton = CreateButton("m_btnStartBattle", root.transform, "开始游戏", new Color(0.22f, 0.44f, 0.78f, 1f));
        SetCenter(startButton.GetComponent<RectTransform>(), -100f, -275f, 180f, 60f);

        var leaveButton = CreateButton("m_btnLeaveRoom", root.transform, "离开房间", new Color(0.35f, 0.37f, 0.36f, 1f));
        SetCenter(leaveButton.GetComponent<RectTransform>(), 100f, -275f, 180f, 60f);

        SavePrefab(root, $"{UiDir}/RoomUI.prefab");
    }

    public static void BuildRoomPlayerSlot()
    {
        var root = new GameObject("RoomPlayerSlot", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
        root.layer = LayerMask.NameToLayer("UI");
        var rect = root.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(640f, 72f);
        root.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.92f);
        var layout = root.GetComponent<LayoutElement>();
        layout.preferredHeight = 72f;
        layout.minHeight = 72f;

        var owner = CreateText("m_txtOwner", root.transform, 18, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.95f, 0.56f, 0.12f, 1f));
        SetAnchor(owner.rectTransform, 0f, 0.5f, 0f, 0.5f, 58f, 0f, 72f, 36f);
        owner.text = "房主";

        var nickname = CreateText("m_txtNickname", root.transform, 24, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black);
        SetAnchor(nickname.rectTransform, 0f, 0.5f, 0f, 0.5f, 156f, 0f, 180f, 44f);
        nickname.text = "空位";

        var level = CreateText("m_txtLevel", root.transform, 20, FontStyle.Normal, TextAnchor.MiddleCenter, Color.black);
        SetAnchor(level.rectTransform, 0.5f, 0.5f, 0.5f, 0.5f, 20f, 0f, 120f, 40f);
        level.text = "Lv.1";

        var ready = CreateText("m_txtReadyState", root.transform, 20, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.45f, 0.45f, 0.45f, 1f));
        SetAnchor(ready.rectTransform, 1f, 0.5f, 1f, 0.5f, -92f, 0f, 160f, 40f);
        ready.text = "等待加入";

        SavePrefab(root, RoomPlayerSlotPath);
    }

    private static GameObject CreateRoot(string name)
    {
        var root = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));
        root.layer = LayerMask.NameToLayer("UI");
        var rect = root.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        return root;
    }

    private static RectTransform CreateRect(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    private static RectTransform CreatePanel(string name, Transform parent, Color color)
    {
        var image = CreateImage(name, parent, color);
        return image.rectTransform;
    }

    private static Image CreateImage(string name, Transform parent, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private static Text CreateCurrency(string name, Transform parent, string iconPath, string value, float x)
    {
        var chip = CreateImage(name, parent, new Color(0.12f, 0.10f, 0.08f, 0.62f));
        SetAnchor(chip.rectTransform, 1f, 0.5f, 1f, 0.5f, x, 0f, 168f, 48f);

        var icon = CreateImage("m_imgIcon", chip.transform, Color.white);
        SetAnchor(icon.rectTransform, 0f, 0.5f, 0f, 0.5f, 26f, 0f, 32f, 32f);
        ApplySprite(icon, iconPath, Image.Type.Simple);

        var text = CreateText("m_txtValue", chip.transform, 19, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
        SetAnchor(text.rectTransform, 0f, 0.5f, 1f, 0.5f, 88f, 0f, -64f, 34f);
        text.text = value;
        return text;
    }

    private static Text CreateStat(string name, Transform parent, string title, string value, float x)
    {
        var card = CreateImage(name, parent, new Color(0.99f, 0.92f, 0.68f, 0.74f));
        SetAnchor(card.rectTransform, 0.5f, 0f, 0.5f, 0f, x, 54f, 150f, 64f);

        var titleText = CreateText("m_txtTitle", card.transform, 16, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.37f, 0.31f, 0.22f, 1f));
        SetAnchor(titleText.rectTransform, 0.5f, 1f, 0.5f, 1f, 0f, -17f, 126f, 24f);
        titleText.text = title;

        var valueText = CreateText("m_txtValue", card.transform, 20, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.12f, 0.10f, 0.08f, 1f));
        SetAnchor(valueText.rectTransform, 0.5f, 0f, 0.5f, 0f, 0f, 18f, 126f, 30f);
        valueText.text = value;
        return valueText;
    }

    private static Button CreateSmallButton(string name, Transform parent, string label, float x)
    {
        var button = CreateButton(name, parent, label, new Color(0.14f, 0.20f, 0.24f, 0.86f));
        SetAnchor(button.GetComponent<RectTransform>(), 1f, 0.5f, 1f, 0.5f, x, 0f, 108f, 44f);
        return button;
    }

    private static Button CreateLargeButton(string name, Transform parent, string label, float x, string spritePath)
    {
        var button = CreateButton(name, parent, label, Color.white);
        SetAnchor(button.GetComponent<RectTransform>(), 0.5f, 0.5f, 0.5f, 0.5f, x, 0f, 132f, 76f);
        ApplySprite(button.targetGraphic as Image, spritePath, Image.Type.Sliced);
        return button;
    }

    private static Button CreateSideButton(string name, Transform parent, string label, float y)
    {
        var button = CreateButton(name, parent, label, new Color(0.11f, 0.20f, 0.25f, 0.86f));
        SetAnchor(button.GetComponent<RectTransform>(), 0.5f, 0.5f, 0.5f, 0.5f, 0f, y, 92f, 64f);
        return button;
    }

    private static void ApplySprite(Image image, string assetPath, Image.Type type)
    {
        if (image == null)
        {
            return;
        }

        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        image.type = type;
        image.color = Color.white;
    }

    private static Text CreateText(string name, Transform parent, int size, FontStyle style, TextAnchor anchor, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);
        var text = go.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = anchor;
        text.color = color;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    private static InputField CreateInput(string name, Transform parent, string placeholder, string value)
    {
        var image = CreateImage(name, parent, Color.white);
        var input = image.gameObject.AddComponent<InputField>();
        input.targetGraphic = image;

        var holder = CreateText("m_txtPlaceholder", image.transform, 22, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.45f, 0.45f, 0.45f, 1f));
        StretchInset(holder.rectTransform, 16f, 0f);
        holder.text = placeholder;

        var text = CreateText("m_txtText", image.transform, 22, FontStyle.Normal, TextAnchor.MiddleLeft, Color.black);
        StretchInset(text.rectTransform, 16f, 0f);
        text.text = value;

        input.placeholder = holder;
        input.textComponent = text;
        input.text = value;
        return input;
    }

    private static Toggle CreateToggle(string name, Transform parent, string label)
    {
        var root = new GameObject(name, typeof(RectTransform), typeof(Toggle));
        root.layer = LayerMask.NameToLayer("UI");
        root.transform.SetParent(parent, false);
        var toggle = root.GetComponent<Toggle>();

        var box = CreateImage("m_imgCheckBox", root.transform, Color.white);
        SetCenter(box.rectTransform, -74f, 0f, 30f, 30f);

        var check = CreateImage("m_imgCheck", box.transform, new Color(0.22f, 0.44f, 0.78f, 1f));
        check.rectTransform.anchorMin = new Vector2(0.2f, 0.2f);
        check.rectTransform.anchorMax = new Vector2(0.8f, 0.8f);
        check.rectTransform.offsetMin = Vector2.zero;
        check.rectTransform.offsetMax = Vector2.zero;

        var text = CreateText("m_txtLabel", root.transform, 22, FontStyle.Normal, TextAnchor.MiddleLeft, Color.black);
        SetCenter(text.rectTransform, 24f, 0f, 130f, 40f);
        text.text = label;

        toggle.targetGraphic = box;
        toggle.graphic = check;
        return toggle;
    }

    private static Button CreateButton(string name, Transform parent, string label, Color color)
    {
        var image = CreateImage(name, parent, color);
        var button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        var text = CreateText("m_txtLabel", image.transform, 22, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(text.rectTransform);
        text.text = label;
        return button;
    }

    private static void SetCenter(RectTransform rect, float x, float y, float width, float height)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);
    }

    private static void SetAnchor(RectTransform rect, float minX, float minY, float maxX, float maxY, float x, float y, float width, float height)
    {
        rect.anchorMin = new Vector2(minX, minY);
        rect.anchorMax = new Vector2(maxX, maxY);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);
    }

    private static void SetStretchTop(RectTransform rect, float left, float top, float right, float height)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.offsetMin = new Vector2(left, top - height);
        rect.offsetMax = new Vector2(right, top);
    }

    private static void SetStretchBottom(RectTransform rect, float left, float bottom, float right, float height)
    {
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(right, bottom + height);
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void StretchInset(RectTransform rect, float horizontal, float vertical)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(horizontal, vertical);
        rect.offsetMax = new Vector2(-horizontal, -vertical);
    }

    private static void SavePrefab(GameObject root, string path)
    {
        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }

    private static bool IsLobbyPrefabComplete()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(LobbyPath);
        if (prefab == null)
        {
            return false;
        }

        var root = prefab.transform;
        return root.Find("m_topBar/m_currencyGold/m_txtValue") != null
               && root.Find("m_mainPanel/m_statLoadout/m_txtValue") != null
               && root.Find("m_bottomBar/m_btnStartMatch") != null
               && root.Find("m_bottomBar/m_btnCreateRoom") != null
               && root.Find("m_bottomBar/m_btnRoomList") != null
               && root.Find("m_bottomBar/m_btnRefresh") != null
               && root.Find("m_sideMenu/m_btnShop") != null;
    }
}

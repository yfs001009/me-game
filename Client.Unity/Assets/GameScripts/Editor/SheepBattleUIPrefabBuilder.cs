using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class SheepBattleUIPrefabBuilder
{
    private const string UiDir = "Assets/AssetRaw/UI";
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

            if (File.Exists(CreateRoomPath) && File.Exists(RoomPlayerSlotPath))
            {
                return;
            }

            Debug.Log("SheepBattle UI prefabs are missing. Building CreateRoomUI and RoomPlayerSlot prefabs.");
            BuildAll();
        };
    }

    [MenuItem("SheepBattle/Build UI Prefabs")]
    public static void BuildAll()
    {
        Directory.CreateDirectory(UiDir);
        BuildCreateRoomUI();
        BuildRoomUI();
        BuildRoomPlayerSlot();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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
}

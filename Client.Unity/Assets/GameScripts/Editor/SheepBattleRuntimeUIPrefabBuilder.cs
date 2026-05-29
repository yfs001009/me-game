using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class SheepBattleRuntimeUIPrefabBuilder
{
    private const string UiDir = "Assets/AssetRaw/UI";
    private static readonly string[] RequiredPrefabNames =
    {
        "CommonNoticeUI",
        "NicknameUI",
        "MatchQueueUI",
        "RoomListUI",
        "RoomPasswordUI",
        "RegisterUI",
        "SplashUI",
        "VersionCheckUI",
        "LoadingUI"
    };

    [InitializeOnLoadMethod]
    private static void BuildMissingPrefabsOnEditorLoad()
    {
        EditorApplication.delayCall += () =>
        {
            if (Application.isBatchMode || HasAllRuntimePrefabs())
            {
                return;
            }

            Debug.Log("SheepBattle runtime UI prefabs are missing. Building fixed UI prefabs.");
            BuildAll();
        };
    }

    [MenuItem("SheepBattle/Build Runtime UI Prefabs")]
    public static void BuildAll()
    {
        Directory.CreateDirectory(UiDir);
        BuildCommonNoticeUI();
        BuildNicknameUI();
        BuildMatchQueueUI();
        BuildRoomListUI();
        BuildRoomPasswordUI();
        BuildRegisterUI();
        BuildSplashUI();
        BuildVersionCheckUI();
        BuildLoadingUI();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void BuildCommonNoticeUI()
    {
        var root = CreateRoot("CommonNoticeUI");
        AddMask(root.transform, 0.55f);
        var panel = CreatePanel(root.transform, 560f, 300f);
        Center(CreateText("m_txtTitle", panel, "提示", 30, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black).rectTransform, 0f, 106f, 480f, 58f);
        Center(CreateText("m_txtMessage", panel, string.Empty, 24, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.12f, 0.12f, 0.12f, 1f)).rectTransform, 0f, 16f, 464f, 118f);
        Center(CreateButton("m_btnConfirm", panel, "确定", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 0f, -104f, 180f, 54f);
        Save(root, "CommonNoticeUI");
    }

    public static void BuildNicknameUI()
    {
        var root = CreateRoot("NicknameUI");
        AddMask(root.transform, 0.62f);
        var panel = CreatePanel(root.transform, 560f, 330f);
        Center(CreateText("m_txtTitle", panel, "取一个名字", 32, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black).rectTransform, 0f, 116f, 480f, 64f);
        Center(CreateText("m_txtTip", panel, "首次进入大厅前需要设置昵称", 22, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.2f, 0.2f, 0.2f, 1f)).rectTransform, 0f, 74f, 480f, 42f);
        Center(CreateInput("m_inputNickname", panel, "请输入昵称", string.Empty, 12, false).GetComponent<RectTransform>(), 0f, 12f, 420f, 58f);
        Center(CreateButton("m_btnConfirm", panel, "进入大厅", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 0f, -104f, 200f, 58f);
        Save(root, "NicknameUI");
    }

    public static void BuildMatchQueueUI()
    {
        var root = CreateRoot("MatchQueueUI");
        AddMask(root.transform, 0.55f);
        var panel = CreatePanel(root.transform, 600f, 360f);
        Center(CreateText("m_txtTitle", panel, "匹配队列", 30, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black).rectTransform, 0f, 112f, 520f, 58f);
        Center(CreateText("m_txtStatus", panel, string.Empty, 24, FontStyle.Normal, TextAnchor.MiddleCenter, Color.black).rectTransform, 0f, 20f, 520f, 120f);
        Center(CreateButton("m_btnRefresh", panel, "刷新状态", new Color(0.20f, 0.46f, 0.70f, 1f)).GetComponent<RectTransform>(), -96f, -116f, 160f, 54f);
        Center(CreateButton("m_btnClose", panel, "返回大厅", new Color(0.66f, 0.22f, 0.22f, 1f)).GetComponent<RectTransform>(), 96f, -116f, 160f, 54f);
        Save(root, "MatchQueueUI");
    }

    public static void BuildRoomListUI()
    {
        var root = CreateRoot("RoomListUI");
        AddMask(root.transform, 0.55f);
        var panel = CreatePanel(root.transform, 760f, 520f);
        Anchor(CreateText("m_txtTitle", panel, "房间列表", 30, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black).rectTransform, 0f, 1f, 1f, 1f, 32f, -48f, -180f, 48f);
        Anchor(CreateButton("m_btnClose", panel, "关闭", new Color(0.66f, 0.22f, 0.22f, 1f)).GetComponent<RectTransform>(), 1f, 1f, 1f, 1f, -88f, -48f, 120f, 48f);
        Anchor(CreateButton("m_btnRefresh", panel, "刷新", new Color(0.20f, 0.46f, 0.70f, 1f)).GetComponent<RectTransform>(), 1f, 1f, 1f, 1f, -220f, -48f, 120f, 48f);

        var list = new GameObject("m_listRooms", typeof(RectTransform));
        list.layer = LayerMask.NameToLayer("UI");
        list.transform.SetParent(panel, false);
        var listRect = list.GetComponent<RectTransform>();
        listRect.anchorMin = Vector2.zero;
        listRect.anchorMax = Vector2.one;
        listRect.offsetMin = new Vector2(32f, 34f);
        listRect.offsetMax = new Vector2(-32f, -96f);

        var empty = CreateText("m_txtEmpty", list.transform, "暂无可加入房间", 24, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.18f, 0.18f, 0.18f, 1f));
        Stretch(empty.rectTransform);

        var template = CreateButton("m_btnRoomTemplate", list.transform, string.Empty, new Color(0.20f, 0.46f, 0.70f, 1f));
        var templateRect = template.GetComponent<RectTransform>();
        templateRect.anchorMin = new Vector2(0f, 1f);
        templateRect.anchorMax = new Vector2(1f, 1f);
        templateRect.pivot = new Vector2(0.5f, 1f);
        templateRect.sizeDelta = new Vector2(0f, 68f);
        templateRect.anchoredPosition = Vector2.zero;
        template.gameObject.SetActive(false);
        var label = template.GetComponentInChildren<Text>();
        label.alignment = TextAnchor.MiddleLeft;
        label.color = Color.black;
        label.fontStyle = FontStyle.Normal;
        label.rectTransform.offsetMin = new Vector2(18f, 0f);
        label.rectTransform.offsetMax = new Vector2(-18f, 0f);
        Save(root, "RoomListUI");
    }

    public static void BuildRoomPasswordUI()
    {
        var root = CreateRoot("RoomPasswordUI");
        AddMask(root.transform, 0.55f);
        var panel = CreatePanel(root.transform, 520f, 300f);
        Center(CreateText("m_txtTitle", panel, "请输入房间密码", 28, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black).rectTransform, 0f, 96f, 460f, 50f);
        Center(CreateInput("m_inputPassword", panel, "房间密码", string.Empty, 32, false).GetComponent<RectTransform>(), 0f, 24f, 380f, 54f);
        Center(CreateButton("m_btnJoin", panel, "加入", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), -86f, -84f, 150f, 52f);
        Center(CreateButton("m_btnClose", panel, "取消", new Color(0.66f, 0.22f, 0.22f, 1f)).GetComponent<RectTransform>(), 86f, -84f, 150f, 52f);
        Save(root, "RoomPasswordUI");
    }

    public static void BuildRegisterUI()
    {
        var root = CreateRoot("RegisterUI");
        AddMask(root.transform, 0.56f);
        var panel = CreatePanel(root.transform, 560f, 430f);
        Center(CreateText("m_txtTitle", panel, "注册新账号", 32, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black).rectTransform, 0f, 150f, 420f, 56f);
        Center(CreateInput("m_inputAccount", panel, "用户名（3-12位）", string.Empty, 12, false).GetComponent<RectTransform>(), 0f, 74f, 420f, 56f);
        Center(CreateInput("m_inputPassword", panel, "密码（6-20位）", string.Empty, 20, true).GetComponent<RectTransform>(), 0f, 8f, 420f, 56f);
        Center(CreateInput("m_inputConfirm", panel, "确认密码", string.Empty, 20, true).GetComponent<RectTransform>(), 0f, -58f, 420f, 56f);
        Center(CreateButton("m_btnRegister", panel, "注册", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), -96f, -146f, 160f, 54f);
        Center(CreateButton("m_btnBack", panel, "返回", new Color(0.66f, 0.22f, 0.22f, 1f)).GetComponent<RectTransform>(), 96f, -146f, 160f, 54f);
        Save(root, "RegisterUI");
    }

    public static void BuildSplashUI()
    {
        var root = CreateRoot("SplashUI");
        Stretch(CreateImage("m_imgBg", root.transform, new Color(0.08f, 0.12f, 0.13f, 1f)).rectTransform);
        Center(CreateText("m_txtLogo", root.transform, "SheepBattle", 56, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white).rectTransform, 0f, 48f, 560f, 90f);
        Center(CreateText("m_txtVersion", root.transform, "v0.1.0", 22, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.74f, 0.82f, 0.78f, 1f)).rectTransform, 0f, -48f, 260f, 40f);
        Save(root, "SplashUI");
    }

    public static void BuildVersionCheckUI()
    {
        var root = CreateRoot("VersionCheckUI");
        Stretch(CreateImage("m_imgBg", root.transform, new Color(0.09f, 0.12f, 0.13f, 1f)).rectTransform);
        Center(CreateText("m_txtTitle", root.transform, "版本检查", 36, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white).rectTransform, 0f, 84f, 360f, 64f);
        Center(CreateText("m_txtStatus", root.transform, "正在检查资源版本...", 24, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.78f, 0.86f, 0.80f, 1f)).rectTransform, 0f, 20f, 620f, 56f);
        Center(CreateText("m_txtVersion", root.transform, "当前版本：v0.1.0", 20, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.60f, 0.68f, 0.64f, 1f)).rectTransform, 0f, -34f, 360f, 40f);
        var update = CreateButton("m_btnUpdate", root.transform, "前往更新", new Color(0.20f, 0.48f, 0.36f, 1f));
        Center(update.GetComponent<RectTransform>(), 0f, -108f, 260f, 56f);
        update.gameObject.SetActive(false);
        Save(root, "VersionCheckUI");
    }

    public static void BuildLoadingUI()
    {
        var root = CreateRoot("LoadingUI");
        Stretch(CreateImage("m_imgBg", root.transform, new Color(0.10f, 0.13f, 0.12f, 1f)).rectTransform);
        Center(CreateText("m_txtTitle", root.transform, "加载中", 34, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white).rectTransform, 0f, 72f, 360f, 60f);
        Center(CreateText("m_txtTip", root.transform, string.Empty, 22, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.77f, 0.84f, 0.78f, 1f)).rectTransform, 0f, 14f, 620f, 48f);
        var track = CreateImage("m_imgTrack", root.transform, new Color(0.24f, 0.28f, 0.27f, 1f));
        Center(track.rectTransform, 0f, -48f, 520f, 22f);
        var bar = CreateImage("m_imgBar", track.transform, new Color(0.18f, 0.62f, 0.42f, 1f));
        bar.rectTransform.anchorMin = new Vector2(0f, 0f);
        bar.rectTransform.anchorMax = new Vector2(0f, 1f);
        bar.rectTransform.pivot = new Vector2(0f, 0.5f);
        bar.rectTransform.sizeDelta = new Vector2(0f, 0f);
        bar.rectTransform.anchoredPosition = Vector2.zero;
        Center(CreateText("m_txtProgress", root.transform, "0%", 20, FontStyle.Normal, TextAnchor.MiddleCenter, Color.white).rectTransform, 0f, -84f, 160f, 32f);
        Save(root, "LoadingUI");
    }

    private static void AddMask(Transform parent, float alpha)
    {
        Stretch(CreateImage("m_imgMask", parent, new Color(0f, 0f, 0f, alpha)).rectTransform);
    }

    private static Transform CreatePanel(Transform parent, float width, float height)
    {
        var image = CreateImage("m_imgPanel", parent, new Color(0.94f, 0.96f, 0.94f, 1f));
        Center(image.rectTransform, 0f, 0f, width, height);
        return image.transform;
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

    private static Image CreateImage(string name, Transform parent, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private static Text CreateText(string name, Transform parent, string value, int size, FontStyle style, TextAnchor anchor, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
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
        text.text = value;
        return text;
    }

    private static InputField CreateInput(string name, Transform parent, string placeholder, string value, int limit, bool password)
    {
        var image = CreateImage(name, parent, Color.white);
        var input = image.gameObject.AddComponent<InputField>();
        input.targetGraphic = image;
        input.characterLimit = limit;
        input.contentType = password ? InputField.ContentType.Password : InputField.ContentType.Standard;
        var holder = CreateText("m_txtPlaceholder", image.transform, placeholder, 22, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.45f, 0.45f, 0.45f, 1f));
        StretchInset(holder.rectTransform, 16f, 0f);
        var text = CreateText("m_txtText", image.transform, value, 22, FontStyle.Normal, TextAnchor.MiddleLeft, Color.black);
        StretchInset(text.rectTransform, 16f, 0f);
        input.placeholder = holder;
        input.textComponent = text;
        input.text = value;
        return input;
    }

    private static Button CreateButton(string name, Transform parent, string label, Color color)
    {
        var image = CreateImage(name, parent, color);
        var button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        var text = CreateText("m_txtLabel", image.transform, label, 22, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(text.rectTransform);
        return button;
    }

    private static void Center(RectTransform rect, float x, float y, float width, float height)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);
    }

    private static void Anchor(RectTransform rect, float minX, float minY, float maxX, float maxY, float x, float y, float width, float height)
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

    private static void Save(GameObject root, string name)
    {
        PrefabUtility.SaveAsPrefabAsset(root, $"{UiDir}/{name}.prefab");
        Object.DestroyImmediate(root);
    }

    private static bool HasAllRuntimePrefabs()
    {
        for (var i = 0; i < RequiredPrefabNames.Length; i++)
        {
            if (!File.Exists($"{UiDir}/{RequiredPrefabNames[i]}.prefab"))
            {
                return false;
            }
        }

        return true;
    }
}

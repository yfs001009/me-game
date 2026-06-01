using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class SheepBattleBagMailWidgetMigrator
{
    private const string BagPath = "Assets/AssetRaw/UI/BagUI.prefab";
    private const string MailPath = "Assets/AssetRaw/UI/MailUI.prefab";
    private const string BagItemWidgetPath = "Assets/AssetRaw/UI/BagItemWidget.prefab";
    private const string MailItemWidgetPath = "Assets/AssetRaw/UI/MailItemWidget.prefab";

    [InitializeOnLoadMethod]
    private static void AutoMigrateWhenNeeded()
    {
        EditorApplication.delayCall += () =>
        {
            if (Application.isBatchMode)
            {
                return;
            }

            if (!NeedsMigration())
            {
                return;
            }

            Migrate();
        };
    }

    [MenuItem("SheepBattle/Migrate Bag Mail Widget Templates")]
    public static void Migrate()
    {
        BuildBagItemWidgetPrefab();
        BuildMailItemWidgetPrefab();
        MigrateBag();
        MigrateMail();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Migrated BagUI and MailUI list templates to UIWidget-compatible item nodes.");
    }

    private static bool NeedsMigration()
    {
        return AssetDatabase.LoadAssetAtPath<GameObject>(BagItemWidgetPath) == null
               || AssetDatabase.LoadAssetAtPath<GameObject>(MailItemWidgetPath) == null
               || !HasChild(BagPath, "m_imgPanel/m_listItems/m_item_BagItemTemplate/m_imgIcon")
               || !HasChild(BagPath, "m_imgPanel/m_listItems/m_item_BagItemTemplate/m_txtName")
               || !HasChild(BagPath, "m_imgPanel/m_listItems/m_item_BagItemTemplate/m_txtCount")
               || !HasChild(MailPath, "m_imgPanel/m_listMails/m_item_MailTemplate/m_txtTitle")
               || !HasChild(MailPath, "m_imgPanel/m_listMails/m_item_MailTemplate/m_txtState")
               || !HasChild(MailPath, "m_imgPanel/m_listMails/m_item_MailTemplate/m_imgAttachment");
    }

    private static bool HasChild(string prefabPath, string childPath)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        return prefab != null && prefab.transform.Find(childPath) != null;
    }

    private static void BuildBagItemWidgetPrefab()
    {
        var root = CreateButton("BagItemWidget", null, string.Empty, new Color(0.78f, 0.82f, 0.76f, 1f));
        SetupListItemRect(root.GetComponent<RectTransform>(), 72f);
        RemoveChild(root.transform, "m_txtLabel");

        var icon = CreateImage("m_imgIcon", root.transform, Color.white);
        Anchor(icon.rectTransform, 0f, 0.5f, 0f, 0.5f, 42f, 0f, 46f, 46f);

        var name = CreateText("m_txtName", root.transform, string.Empty, 19, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
        Anchor(name.rectTransform, 0f, 0.5f, 1f, 0.5f, 128f, 8f, -126f, 30f);

        var count = CreateText("m_txtCount", root.transform, string.Empty, 17, FontStyle.Bold, TextAnchor.MiddleRight, Color.white);
        Anchor(count.rectTransform, 1f, 0.5f, 1f, 0.5f, -54f, -18f, 92f, 26f);

        PrefabUtility.SaveAsPrefabAsset(root.gameObject, BagItemWidgetPath);
        Object.DestroyImmediate(root.gameObject);
    }

    private static void BuildMailItemWidgetPrefab()
    {
        var root = CreateButton("MailItemWidget", null, string.Empty, new Color(0.84f, 0.87f, 0.82f, 1f));
        SetupListItemRect(root.GetComponent<RectTransform>(), 72f);
        RemoveChild(root.transform, "m_txtLabel");

        var title = CreateText("m_txtTitle", root.transform, string.Empty, 20, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black);
        Anchor(title.rectTransform, 0f, 0.5f, 1f, 0.5f, 18f, 10f, -122f, 32f);

        var state = CreateText("m_txtState", root.transform, string.Empty, 16, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.2f, 0.6f, 0.3f, 1f));
        Anchor(state.rectTransform, 0f, 0.5f, 0f, 0.5f, 42f, -18f, 76f, 24f);

        var attachment = CreateImage("m_imgAttachment", root.transform, new Color(0.95f, 0.67f, 0.18f, 1f));
        Anchor(attachment.rectTransform, 1f, 0.5f, 1f, 0.5f, -36f, 0f, 30f, 30f);

        PrefabUtility.SaveAsPrefabAsset(root.gameObject, MailItemWidgetPath);
        Object.DestroyImmediate(root.gameObject);
    }

    private static void MigrateBag()
    {
        var root = PrefabUtility.LoadPrefabContents(BagPath);
        var list = root.transform.Find("m_imgPanel/m_listItems/Viewport/Content")
                   ?? root.transform.Find("m_imgPanel/m_listItems");
        if (list == null)
        {
            Debug.LogError("BagUI.prefab missing m_listItems.");
            PrefabUtility.UnloadPrefabContents(root);
            return;
        }

        RemoveChild(list, "m_btnItemTemplate");
        RemoveChild(list, "m_item_BagItemTemplate");
        CreateBagTemplate(list);
        PrefabUtility.SaveAsPrefabAsset(root, BagPath);
        PrefabUtility.UnloadPrefabContents(root);
    }

    private static void MigrateMail()
    {
        var root = PrefabUtility.LoadPrefabContents(MailPath);
        var list = root.transform.Find("m_imgPanel/m_listMails/Viewport/Content")
                   ?? root.transform.Find("m_imgPanel/m_listMails");
        if (list == null)
        {
            Debug.LogError("MailUI.prefab missing m_listMails.");
            PrefabUtility.UnloadPrefabContents(root);
            return;
        }

        RemoveChild(list, "m_btnMailTemplate");
        RemoveChild(list, "m_item_MailTemplate");
        CreateMailTemplate(list);
        PrefabUtility.SaveAsPrefabAsset(root, MailPath);
        PrefabUtility.UnloadPrefabContents(root);
    }

    private static void CreateBagTemplate(Transform parent)
    {
        var template = CreateButton("m_item_BagItemTemplate", parent, string.Empty, new Color(0.78f, 0.82f, 0.76f, 1f));
        SetupListItemRect(template.GetComponent<RectTransform>(), 72f);
        RemoveChild(template.transform, "m_txtLabel");

        var icon = CreateImage("m_imgIcon", template.transform, Color.white);
        Anchor(icon.rectTransform, 0f, 0.5f, 0f, 0.5f, 42f, 0f, 46f, 46f);

        var name = CreateText("m_txtName", template.transform, string.Empty, 19, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
        Anchor(name.rectTransform, 0f, 0.5f, 1f, 0.5f, 128f, 8f, -126f, 30f);

        var count = CreateText("m_txtCount", template.transform, string.Empty, 17, FontStyle.Bold, TextAnchor.MiddleRight, Color.white);
        Anchor(count.rectTransform, 1f, 0.5f, 1f, 0.5f, -54f, -18f, 92f, 26f);
        template.gameObject.SetActive(false);
    }

    private static void CreateMailTemplate(Transform parent)
    {
        var template = CreateButton("m_item_MailTemplate", parent, string.Empty, new Color(0.84f, 0.87f, 0.82f, 1f));
        SetupListItemRect(template.GetComponent<RectTransform>(), 72f);
        RemoveChild(template.transform, "m_txtLabel");

        var title = CreateText("m_txtTitle", template.transform, string.Empty, 20, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black);
        Anchor(title.rectTransform, 0f, 0.5f, 1f, 0.5f, 18f, 10f, -122f, 32f);

        var state = CreateText("m_txtState", template.transform, string.Empty, 16, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.2f, 0.6f, 0.3f, 1f));
        Anchor(state.rectTransform, 0f, 0.5f, 0f, 0.5f, 42f, -18f, 76f, 24f);

        var attachment = CreateImage("m_imgAttachment", template.transform, new Color(0.95f, 0.67f, 0.18f, 1f));
        Anchor(attachment.rectTransform, 1f, 0.5f, 1f, 0.5f, -36f, 0f, 30f, 30f);
        template.gameObject.SetActive(false);
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

    private static Image CreateImage(string name, Transform parent, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.layer = LayerMask.NameToLayer("UI");
        if (parent != null)
        {
            go.transform.SetParent(parent, false);
        }
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

    private static void SetupListItemRect(RectTransform rect, float height)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(0f, height);
        rect.anchoredPosition = Vector2.zero;
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

    private static void RemoveChild(Transform parent, string name)
    {
        var child = parent.Find(name);
        if (child != null)
        {
            Object.DestroyImmediate(child.gameObject);
        }
    }
}

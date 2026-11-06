using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class PrototypeSetup
{
    private const string DataPath = "Assets/Data";
    private const string PrefabPath = "Assets/Prefabs";
    private const string ScenePath = "Assets/Scenes/Prototype.unity";
    private const string ArtPath = "Assets/Art";
    private const string PlayerSpritePath = "Assets/Art/PlayerSprites";

    [MenuItem("Balancing Prototype/Generate Demo Scene")]
    public static void GenerateDemoScene()
    {
        EnsureFolder(DataPath);
        EnsureFolder(PrefabPath);
        EnsureFolder("Assets/Scenes");
        EnsureFolder(ArtPath);
        EnsureFolder(PlayerSpritePath);
        EnsureEnemyLayer();
        ConfigurePlayerSprites();

        Sprite playerSprite = CreateSprite("PlayerBlue", new Color32(53, 166, 255, 255));
        Sprite groundSprite = CreateSprite("Stone", new Color32(54, 68, 87, 255));
        Sprite[] enemySprites =
        {
            CreateSprite("EnemyGreen", new Color32(70, 200, 125, 255)),
            CreateSprite("EnemyOrange", new Color32(244, 155, 70, 255)),
            CreateSprite("EnemyRed", new Color32(232, 78, 91, 255))
        };

        EnemyStats[] stats =
        {
            CreateStats("Enemy_Grunt", "Grunt", 50f, 10f, 2f, 1.5f, 0.2f, 10, 5),
            CreateStats("Enemy_Brute", "Brute", 100f, 20f, 1.5f, 1.3f, 0.3f, 20, 10),
            CreateStats("Enemy_Stalker", "Stalker", 180f, 35f, 1f, 1f, 0.4f, 35, 20)
        };

        int enemyLayer = LayerMask.NameToLayer("Enemy");

        GameObject[] enemyPrefabs = new GameObject[stats.Length];
        for (int i = 0; i < stats.Length; i++)
            enemyPrefabs[i] = CreateEnemyPrefab(stats[i], enemySprites[i], enemyLayer);

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateEnvironment(groundSprite);

        GameObject player = CreatePlayer(playerSprite);
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.backgroundColor = new Color32(20, 27, 43, 255);
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        cameraObject.AddComponent<CameraFollow>().SetTarget(player.transform);

        CreateHUD(player);
        CreateCoinManager();

        Vector3[] spawnPositions =
        {
            new Vector3(-1f, -2.35f, 0f),
            new Vector3(3f, -2.35f, 0f),
            new Vector3(7f, -2.35f, 0f)
        };
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            GameObject enemy = (GameObject)PrefabUtility.InstantiatePrefab(enemyPrefabs[i], scene);
            enemy.transform.position = spawnPositions[i];
        }

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(ScenePath, true)
        };

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Prototype ready. Open Assets/Scenes/Prototype.unity and press Play.");
    }

    private static void CreateEnvironment(Sprite groundSprite)
    {
        CreateBlock("Ground", groundSprite, new Vector2(0f, -3.25f), new Vector2(30f, 0.8f), true);
        CreateBlock("Platform_Left", groundSprite, new Vector2(-4f, -0.8f), new Vector2(4f, 0.35f), true);
        CreateBlock("Platform_Right", groundSprite, new Vector2(5f, -0.25f), new Vector2(4f, 0.35f), true);
    }

    private static GameObject CreatePlayer(Sprite sprite)
    {
        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(-6f, -2.2f, 0f);
        player.transform.localScale = new Vector3(0.55f, 0.55f, 1f);
        SpriteRenderer spriteRenderer = player.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1f, 2.7f);
        Rigidbody2D body = player.AddComponent<Rigidbody2D>();
        body.freezeRotation = true;
        PlayerController controller = player.AddComponent<PlayerController>();
        SerializedObject serialized = new SerializedObject(controller);
        serialized.FindProperty("enemyLayer").intValue = LayerMask.GetMask("Enemy");
        serialized.ApplyModifiedPropertiesWithoutUndo();

        GameObject attackPoint = new GameObject("AttackPoint");
        attackPoint.transform.SetParent(player.transform, false);
        attackPoint.transform.localPosition = new Vector3(0.9f, 0.05f, 0f);
        serialized = new SerializedObject(controller);
        serialized.FindProperty("attackPoint").objectReferenceValue = attackPoint.transform;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        PlayerSpriteAnimator animator = player.AddComponent<PlayerSpriteAnimator>();
        Sprite[] idle = LoadSpriteFrames("idle", 3);
        Sprite[] walk = LoadSpriteFrames("walk", 4);
        Sprite[] run = LoadSpriteFrames("run", 4);
        Sprite[] jump = LoadSpriteFrames("jump", 2);
        Sprite[] fall = LoadSpriteFrames("fall", 4);
        SerializedObject animatorSerialized = new SerializedObject(animator);
        animatorSerialized.FindProperty("spriteRenderer").objectReferenceValue = spriteRenderer;
        animatorSerialized.FindProperty("player").objectReferenceValue = controller;
        SetSpriteArray(animatorSerialized.FindProperty("idleFrames"), idle);
        SetSpriteArray(animatorSerialized.FindProperty("walkFrames"), walk);
        SetSpriteArray(animatorSerialized.FindProperty("runFrames"), run);
        SetSpriteArray(animatorSerialized.FindProperty("jumpFrames"), jump);
        SetSpriteArray(animatorSerialized.FindProperty("fallFrames"), fall);
        animatorSerialized.ApplyModifiedPropertiesWithoutUndo();
        return player;
    }

    private static void ConfigurePlayerSprites()
    {
        string[] names =
        {
            "fall_01", "fall_02", "fall_03", "fall_04",
            "idle_01", "idle_02", "idle_03",
            "jump_02", "jump_03",
            "run_01", "run_02", "run_03", "run_04",
            "walk_01", "walk_02", "walk_03", "walk_04"
        };

        foreach (string spriteName in names)
        {
            string path = $"{PlayerSpritePath}/{spriteName}.png";
            if (!File.Exists(path))
            {
                Debug.LogError($"Player animation frame is missing: {path}");
                continue;
            }

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                continue;

            bool changed = importer.textureType != TextureImporterType.Sprite ||
                           importer.spriteImportMode != SpriteImportMode.Single ||
                           importer.spritePixelsPerUnit != 64f ||
                           importer.filterMode != FilterMode.Point ||
                           importer.textureCompression != TextureImporterCompression.Uncompressed;
            if (!changed)
                continue;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 64f;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }

    private static Sprite[] LoadSpriteFrames(string prefix, int count)
    {
        Sprite[] frames = new Sprite[count];
        for (int i = 0; i < count; i++)
        {
            int frameNumber = prefix == "jump" ? i + 2 : i + 1;
            string path = $"{PlayerSpritePath}/{prefix}_{frameNumber:00}.png";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
            {
                Debug.LogError($"Could not load player animation frame: {path}");
                return new Sprite[0];
            }

            frames[i] = sprite;
        }

        return frames;
    }

    private static void SetSpriteArray(SerializedProperty property, Sprite[] sprites)
    {
        property.arraySize = sprites.Length;
        for (int i = 0; i < sprites.Length; i++)
            property.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
    }

    private static GameObject CreateEnemyPrefab(EnemyStats stats, Sprite sprite, int enemyLayer)
    {
        GameObject enemy = new GameObject(stats.enemyName);
        enemy.layer = enemyLayer;
        enemy.transform.localScale = new Vector3(0.85f, 0.95f, 1f);
        enemy.AddComponent<SpriteRenderer>().sprite = sprite;
        enemy.AddComponent<BoxCollider2D>();
        Rigidbody2D body = enemy.AddComponent<Rigidbody2D>();
        body.freezeRotation = true;
        EnemyController controller = enemy.AddComponent<EnemyController>();
        SerializedObject serialized = new SerializedObject(controller);
        serialized.FindProperty("stats").objectReferenceValue = stats;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        string path = $"{PrefabPath}/{stats.enemyName}.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(enemy, path);
        Object.DestroyImmediate(enemy);
        return prefab;
    }

    private static void CreateBlock(string name, Sprite sprite, Vector2 position, Vector2 size, bool collider)
    {
        GameObject block = new GameObject(name);
        block.transform.position = position;
        block.transform.localScale = new Vector3(size.x, size.y, 1f);
        block.AddComponent<SpriteRenderer>().sprite = sprite;
        if (collider)
            block.AddComponent<BoxCollider2D>();
    }

    private static void CreateCoinManager()
    {
        GameObject manager = new GameObject("CoinManager");
        CoinManager coinManager = manager.AddComponent<CoinManager>();
        Text coinText = GameObject.Find("CoinText").GetComponent<Text>();
        SerializedObject serialized = new SerializedObject(coinManager);
        serialized.FindProperty("coinText").objectReferenceValue = coinText;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateHUD(GameObject player)
    {
        GameObject canvasObject = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);

        Text coinText = CreateText(canvasObject.transform, "CoinText", "COIN: 0", 28,
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(24f, -72f), new Vector2(-24f, -24f));
        coinText.alignment = TextAnchor.UpperLeft;

        Text statusText = CreateText(canvasObject.transform, "StatusText", "", 20,
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(24f, -136f), new Vector2(-24f, -84f));
        statusText.alignment = TextAnchor.UpperLeft;

        PrototypeHUD hud = canvasObject.AddComponent<PrototypeHUD>();
        SerializedObject serialized = new SerializedObject(hud);
        serialized.FindProperty("statusText").objectReferenceValue = statusText;
        serialized.FindProperty("player").objectReferenceValue = player.GetComponent<PlayerController>();
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static Text CreateText(Transform parent, string name, string content, int fontSize,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(parent, false);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = Color.white;
        text.text = content;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    private static EnemyStats CreateStats(string assetName, string label, float hp, float damage,
        float speed, float cooldown, float armor, int cost, int reward)
    {
        string path = $"{DataPath}/{assetName}.asset";
        EnemyStats stats = AssetDatabase.LoadAssetAtPath<EnemyStats>(path);
        if (stats == null)
        {
            stats = ScriptableObject.CreateInstance<EnemyStats>();
            AssetDatabase.CreateAsset(stats, path);
        }

        stats.enemyName = label;
        stats.maxHealth = hp;
        stats.damage = damage;
        stats.moveSpeed = speed;
        stats.attackCooldown = cooldown;
        stats.armorPercent = armor;
        stats.cost = cost;
        stats.coinReward = reward;
        EditorUtility.SetDirty(stats);
        return stats;
    }

    private static Sprite CreateSprite(string name, Color color)
    {
        string texturePath = $"{ArtPath}/{name}_Texture.asset";
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        if (texture == null)
        {
            texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[16 * 16];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;
            texture.SetPixels(pixels);
            texture.Apply();
            AssetDatabase.CreateAsset(texture, texturePath);
        }

        string spritePath = $"{ArtPath}/{name}_Sprite.asset";
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (sprite == null)
        {
            sprite = Sprite.Create(texture, new Rect(0f, 0f, 16f, 16f), new Vector2(0.5f, 0.5f), 16f);
            AssetDatabase.CreateAsset(sprite, spritePath);
        }
        return sprite;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
        string folderName = Path.GetFileName(path);
        if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, folderName);
    }

    private static void EnsureEnemyLayer()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");
        for (int i = 8; i < layers.arraySize; i++)
        {
            SerializedProperty layer = layers.GetArrayElementAtIndex(i);
            if (layer.stringValue == "Enemy")
                return;
            if (string.IsNullOrEmpty(layer.stringValue))
            {
                layer.stringValue = "Enemy";
                tagManager.ApplyModifiedProperties();
                return;
            }
        }

        Debug.LogError("No free user layer is available for Enemy. Clear a custom layer in Project Settings > Tags and Layers.");
    }
}

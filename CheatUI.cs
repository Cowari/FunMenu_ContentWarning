using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// Class for creating and managing the user interface (UI) of a game mod using the Unity engine.
/// Utilizes BepInEx for dynamic injection of UI elements at runtime.
/// </summary>
/// <remarks>
/// Класс для создания и управления пользовательским интерфейсом (UI) модификации в игре на движке Unity.
/// Использует BepInEx для динамической инъекции UI элементов в runtime.
/// </remarks>
public static class CheatUI
{
    // UI Colors
    /// <summary>Color of the main UI panel</summary>
    /// <remarks>Цвет основной панели UI</remarks>
    private static readonly Color PanelColor = new Color(0.21f, 0.16f, 0.18f, 0.99f);
    
    /// <summary>Background color for buttons</summary>
    /// <remarks>Цвет кнопок</remarks>
    private static readonly Color ButtonBackgroundColor = new Color(0.45f, 0.39f, 0.57f);
    
    /// <summary>Text color for buttons</summary>
    /// <remarks>Цвет текста на кнопках</remarks>
    private static readonly Color ButtonTextColor = new Color(0.27f, 0.27f, 0.28f);

    // UI Size Constants
    /// <summary>Button width in pixels</summary>
    /// <remarks>Ширина кнопок</remarks>
    private const float ButtonWidth = 100f;
    
    /// <summary>Button height in pixels</summary>
    /// <remarks>Высота кнопок</remarks>
    private const float ButtonHeight = 30f;
    
    /// <summary>Button text font size</summary>
    /// <remarks>Размер шрифта на кнопках</remarks>
    private const float ButtonFontSize = 16f;

    /// <summary>Reference to the main UI panel object</summary>
    /// <remarks>Ссылка на основную панель UI</remarks>
    private static GameObject myPanel;

    /// <summary>
    /// Initializes UI elements.
    /// Creates necessary Canvas and EventSystem components, then sets up the UI structure.
    /// </summary>
    /// <remarks>
    /// Инициализация UI элементов.
    /// Создает необходимые компоненты Canvas и EventSystem,
    /// затем инициализирует структуру UI.
    /// </remarks>
    public static void Initialize()
    {
        EnsureEventSystem();
        var canvas = EnsureCanvas();
        CreateUIElements(canvas);
    }

    /// <summary>
    /// Creates EventSystem if missing in the scene.
    /// Required for keyboard/mouse input handling.
    /// </summary>
    /// <remarks>
    /// Создает EventSystem, если его нет в сцене.
    /// Требуется для обработки ввода с клавиатуры/мыши.
    /// </remarks>
    private static void EnsureEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }

    /// <summary>
    /// Finds or creates Canvas object with necessary components.
    /// Sets display mode to ScreenSpaceOverlay.
    /// </summary>
    /// <returns>Canvas game object</returns>
    /// <remarks>
    /// Находит или создает объект Canvas с необходимыми компонентами.
    /// Устанавливает режим отображения ScreenSpaceOverlay.
    /// </remarks>
    private static GameObject EnsureCanvas()
    {
        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            canvas = new GameObject("Canvas");
            var canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
        }

        // Проверка и добавление GraphicRaycaster для обработки кликов
        if(canvas.GetComponent<GraphicRaycaster>() == null)
        {
            Debug.LogWarning("GraphicRaycaster NOT FOUND! Creating GraphicRaycaster...");
            canvas.AddComponent<GraphicRaycaster>();
        }
        return canvas;
    }

    /// <summary>
    /// Creates UI structure: main panel and buttons.
    /// Uses RectTransform for element positioning.
    /// </summary>
    /// <param name="canvas">Parent canvas object</param>
    /// <remarks>
    /// Создает структуру UI: основную панель и кнопки.
    /// Использует RectTransform для позиционирования элементов.
    /// </remarks>
    private static void CreateUIElements(GameObject canvas)
    {
        // Создание основной панели
        myPanel = new GameObject("MyPanel");
        myPanel.AddComponent<RectTransform>();
        myPanel.transform.SetParent(canvas.transform, false);
        ConfigureRectTransform(myPanel.GetComponent<RectTransform>(),
            0.5f, 0.5f, 0.5f, 0.5f, 410, 500); // Центрирует панель
        myPanel.AddComponent<Image>().color = PanelColor;

        // Создание контейнера для кнопок
        var buttonsBorder = new GameObject("ButtonsBorder");
        buttonsBorder.AddComponent<RectTransform>();
        buttonsBorder.transform.SetParent(myPanel.transform, false);
        ConfigureRectTransform(buttonsBorder.GetComponent<RectTransform>(),
            0.5f, 1f, 0.5f, 0.5f, 410, 32); // Позиционирует внизу панели

        buttonsBorder.AddComponent<Image>().color = new Color(0.35f, 0.3f, 0.4f);
        var layoutGroup = buttonsBorder.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = 4f;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;

        // Создание кнопок
        CreateButton(buttonsBorder, "Player", () => { });
        CreateButton(buttonsBorder, "Other", () => { });
        CreateButton(buttonsBorder, "...", () => { });
        CreateButton(buttonsBorder, "???", () => { });
    }

    /// <summary>
    /// Creates button with text inside parent container.
    /// Configures visual style and click handler.
    /// </summary>
    /// <param name="parent">Parent UI element</param>
    /// <param name="name">Button text</param>
    /// <param name="action">Click event handler</param>
    /// <remarks>
    /// Создание кнопки с текстом внутри родительского объекта.
    /// Настривает визуальные параметры и обработчик кликов.
    /// </remarks>
    private static void CreateButton(GameObject parent, string name, System.Action action)
    {
        // Создание кнопки
        var buttonObj = new GameObject(name);
        buttonObj.AddComponent<RectTransform>();
        buttonObj.transform.SetParent(parent.transform, false);
        var button = buttonObj.AddComponent<Button>();
        buttonObj.AddComponent<Image>().color = ButtonBackgroundColor;

        // Создание текстового элемента внутри кнопки
        var textObj = new GameObject("ButtonText");
        textObj.transform.SetParent(buttonObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(ButtonWidth, ButtonHeight);
        textRect.pivot = Vector2.one * 0.5f;
        textRect.anchorMin = Vector2.one * 0.5f;
        textRect.anchorMax = Vector2.one * 0.5f;

        var buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = name;
        buttonText.color = ButtonTextColor;
        buttonText.fontSize = (int)ButtonFontSize;
        buttonText.alignment = TextAlignmentOptions.Center;

        // Добавление обработчика клика
        button.onClick.AddListener(() =>
        {
            action?.Invoke();
            Debug.Log($"Clicked {name} button");
        });
    }

    /// <summary>
    /// Configures RectTransform properties for UI element positioning.
    /// </summary>
    /// <param name="rect">RectTransform component to configure</param>
    /// <param name="anchorX">X-axis anchor value (0-1)</param>
    /// <param name="anchorY">Y-axis anchor value (0-1)</param>
    /// <param name="pivotX">X-axis pivot value (0-1)</param>
    /// <param name="pivotY">Y-axis pivot value (0-1)</param>
    /// <param name="width">Width in pixels</param>
    /// <param name="height">Height in pixels</param>
    /// <remarks>
    /// Настройка RectTransform для позиционирования UI элементов.
    /// </remarks>
    private static void ConfigureRectTransform(RectTransform rect, float anchorX, float anchorY, float pivotX, float pivotY, float width, float height)
    {
        rect.anchorMin = new Vector2(anchorX, anchorY);
        rect.anchorMax = new Vector2(anchorX, anchorY);
        rect.pivot = new Vector2(pivotX, pivotY);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// Toggles UI visibility using F2 keypress.
    /// </summary>
    /// <remarks>
    /// Переключает видимость UI по нажатию F2.
    /// </remarks>
    public static void ToggleUI()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (myPanel == null) Initialize();
            myPanel.SetActive(!myPanel.activeSelf);
        }
    }
}
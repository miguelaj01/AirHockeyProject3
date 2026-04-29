using UnityEngine;
using UnityEngine.UIElements;

public class AirHockeyUIController : MonoBehaviour
{
    private UIDocument uiDocument;

    private VisualElement mainMenu;
    private VisualElement hud;

    private Label scoreLabel;
    private Label roleLabel;
    private Label winnerLabel;

    private Button restartButton;

    public static string selectedRole = "Striker";
    public static string boardOrientation = "Horizontal";

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        mainMenu = root.Q<VisualElement>("MainMenu");
        hud = root.Q<VisualElement>("HUD");

        scoreLabel = root.Q<Label>("ScoreLabel");
        roleLabel = root.Q<Label>("RoleLabel");
        winnerLabel = root.Q<Label>("WinnerLabel");

        Button strikerButton = root.Q<Button>("PlayStrikerButton");
        Button defenderButton = root.Q<Button>("PlayDefenderButton");
        Button verticalButton = root.Q<Button>("VerticalButton");
        Button horizontalButton = root.Q<Button>("HorizontalButton");
        restartButton = root.Q<Button>("RestartButton");

        strikerButton.clicked += () =>
        {
            selectedRole = "Striker";
            StartGame();
        };

        defenderButton.clicked += () =>
        {
            selectedRole = "Defender";
            StartGame();
        };

        verticalButton.clicked += () =>
        {
            boardOrientation = "Vertical";
            SetCameraVertical();
        };

        horizontalButton.clicked += () =>
        {
            boardOrientation = "Horizontal";
            SetCameraHorizontal();
        };

        restartButton.clicked += () =>
        {
            if (AirHockeyGameManager.Instance != null)
            {
                AirHockeyGameManager.Instance.ResetFullMatch();
                restartButton.style.display = DisplayStyle.None;
                winnerLabel.text = "";
            }
        };

        hud.style.display = DisplayStyle.None;
        restartButton.style.display = DisplayStyle.None;
    }

    void Update()
    {
        if (AirHockeyGameManager.Instance == null) return;

        scoreLabel.text = "Left Team: " + AirHockeyGameManager.Instance.leftScore +
                          " | Right Team: " + AirHockeyGameManager.Instance.rightScore;

        roleLabel.text = "Current Role: " + selectedRole +
                         " | Board: " + boardOrientation;

        if (AirHockeyGameManager.Instance.matchOver)
        {
            winnerLabel.text = AirHockeyGameManager.Instance.winnerMessage;
            restartButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            winnerLabel.text = "";
            restartButton.style.display = DisplayStyle.None;
        }
    }

    void StartGame()
    {
        mainMenu.style.display = DisplayStyle.None;
        hud.style.display = DisplayStyle.Flex;
    }

    void SetCameraHorizontal()
    {
        if (Camera.main == null) return;

        Camera.main.transform.position = new Vector3(0f, 8.5f, -7.5f);
        Camera.main.transform.rotation = Quaternion.Euler(55f, 0f, 0f);
    }

    void SetCameraVertical()
    {
        if (Camera.main == null) return;

        Camera.main.transform.position = new Vector3(-7.5f, 8.5f, 0f);
        Camera.main.transform.rotation = Quaternion.Euler(55f, 90f, 0f);
    }
}
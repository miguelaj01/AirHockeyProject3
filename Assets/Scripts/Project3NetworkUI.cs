using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Project3NetworkUI : MonoBehaviour
{
    private Button leftSlot1Button;
    private Button leftSlot2Button;
    private Button rightSlot1Button;
    private Button rightSlot2Button;

    private Text statusText;

    private void Start()
    {
        CreateEventSystemIfMissing();
        CreateUI();
        UpdateSlotButtons();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ClaimSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ClaimSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ClaimSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ClaimSlot(3);
    }

    private void CreateEventSystemIfMissing()
    {
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }
    }

    private void CreateUI()
    {
        Canvas canvas = new GameObject("Project3 Network UI").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvas.gameObject.AddComponent<GraphicRaycaster>();

        CreateButton(canvas.transform, "Host", new Vector2(-350, 430), StartHost);
        CreateButton(canvas.transform, "Client", new Vector2(350, 430), StartClient);

        CreateButton(canvas.transform, "1v1", new Vector2(-450, 330), () => SetMatchConfig(2));
        CreateButton(canvas.transform, "2v1", new Vector2(0, 330), () => SetMatchConfig(3));
        CreateButton(canvas.transform, "2v2", new Vector2(450, 330), () => SetMatchConfig(4));

        leftSlot1Button = CreateButton(canvas.transform, "Claim Left Slot 1", new Vector2(-550, 180), () => ClaimSlot(0));
        leftSlot2Button = CreateButton(canvas.transform, "Claim Left Slot 2", new Vector2(-180, 180), () => ClaimSlot(1));
        rightSlot1Button = CreateButton(canvas.transform, "Claim Right Slot 1", new Vector2(180, 180), () => ClaimSlot(2));
        rightSlot2Button = CreateButton(canvas.transform, "Claim Right Slot 2", new Vector2(550, 180), () => ClaimSlot(3));

        statusText = CreateText(canvas.transform, "Status: Choose Host or Client", new Vector2(0, 70), 30);
        CreateButton(canvas.transform, "Restart Match", new Vector2(0, -20), RestartMatch);
    }

    private void StartHost()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager is missing.");
            return;
        }

        NetworkManager.Singleton.StartHost();
        SetStatus("Host started. Select match type and claim a slot.");
        Debug.Log("HOST STARTED");
    }

    private void StartClient()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager is missing.");
            return;
        }

        NetworkManager.Singleton.StartClient();
        SetStatus("Client started. Claim an available slot.");
        Debug.Log("CLIENT STARTED");
    }
    private void RestartMatch()
    {
        if (AirHockeyGameManager.Instance != null)
        {
            AirHockeyGameManager.Instance.ResetFullMatch();
            SetStatus("Match restarted.");
            Debug.Log("MATCH RESTARTED");
        }
    }
    private void SetMatchConfig(int playerCount)
    {
        Project3GameSettings.selectedPlayerCount = playerCount;

        if (Project3SlotManager.Instance != null)
        {
            Project3SlotManager.Instance.SetMatchConfig(playerCount);
        }

        string label = playerCount == 2 ? "1v1" : playerCount == 3 ? "2v1" : "2v2";
        SetStatus("Match config selected: " + label);
        Debug.Log("MATCH CONFIG SELECTED: " + playerCount);

        UpdateSlotButtons();
    }

    private void ClaimSlot(int slotIndex)
    {
        if (!IsSlotAllowedByCurrentConfig(slotIndex))
        {
            SetStatus("That slot is not available for this match type.");
            Debug.LogWarning("Slot not available for this config: " + slotIndex);
            return;
        }

        Project3SlotManager.localClaimedSlot = slotIndex;

        if (Project3SlotManager.Instance != null)
        {
            Project3SlotManager.Instance.ClaimSlot(slotIndex);
        }

        string slotName = GetSlotName(slotIndex);
        SetStatus("You claimed: " + slotName);
        Debug.Log("LOCAL SLOT CLAIMED FROM UI: " + slotIndex);
    }

    private void UpdateSlotButtons()
    {
        if (leftSlot1Button == null) return;

        SetButtonAvailable(leftSlot1Button, true);
        SetButtonAvailable(leftSlot2Button, Project3GameSettings.selectedPlayerCount >= 3);
        SetButtonAvailable(rightSlot1Button, true);
        SetButtonAvailable(rightSlot2Button, Project3GameSettings.selectedPlayerCount == 4);
    }

    private bool IsSlotAllowedByCurrentConfig(int slotIndex)
    {
        int config = Project3GameSettings.selectedPlayerCount;

        if (config == 2)
        {
            return slotIndex == 0 || slotIndex == 2;
        }

        if (config == 3)
        {
            return slotIndex == 0 || slotIndex == 1 || slotIndex == 2;
        }

        if (config == 4)
        {
            return true;
        }

        return false;
    }

    private string GetSlotName(int slotIndex)
    {
        if (slotIndex == 0) return "Left Slot 1";
        if (slotIndex == 1) return "Left Slot 2";
        if (slotIndex == 2) return "Right Slot 1";
        if (slotIndex == 3) return "Right Slot 2";
        return "Unknown Slot";
    }

    private void SetButtonAvailable(Button button, bool available)
    {
        button.interactable = available;

        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = available
                ? new Color(0.1f, 0.25f, 0.55f, 0.95f)
                : new Color(0.25f, 0.25f, 0.25f, 0.55f);
        }
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = "Status: " + message;
        }
    }

    private Button CreateButton(Transform parent, string text, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObj = new GameObject(text + " Button");
        buttonObj.transform.SetParent(parent, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 70);
        rect.anchoredPosition = position;

        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.1f, 0.25f, 0.55f, 0.95f);

        Button button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(action);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text label = textObj.AddComponent<Text>();
        label.text = text;
        label.alignment = TextAnchor.MiddleCenter;
        label.fontSize = 24;
        label.color = Color.white;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        return button;
    }

    private Text CreateText(Transform parent, string text, Vector2 position, int fontSize)
    {
        GameObject textObj = new GameObject("Status Text");
        textObj.transform.SetParent(parent, false);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(1000, 60);
        rect.anchoredPosition = position;

        Text label = textObj.AddComponent<Text>();
        label.text = text;
        label.alignment = TextAnchor.MiddleCenter;
        label.fontSize = fontSize;
        label.color = Color.white;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        return label;
    }
}
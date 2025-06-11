using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RegisterPanel : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_InputField playerIdInput;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button goToLoginButton;

    private LoginSceneManager sceneManager;

    private void Awake()
    {
        sceneManager = FindObjectOfType<LoginSceneManager>();
        if (sceneManager == null)
        {
            Debug.LogError("LoginSceneManager를 찾을 수 없습니다!");
        }
    }

    private void Start()
    {
        SetupButtons();
        SetupInputField();
    }

    private void SetupButtons()
    {
        if (registerButton != null)
        {
            registerButton.onClick.AddListener(OnRegisterClicked);
        }

        if (goToLoginButton != null)
        {
            goToLoginButton.onClick.AddListener(OnGoToLoginClicked);
        }
    }

    private void SetupInputField()
    {
        if (playerIdInput != null)
        {
            // 엔터키로 회원가입
            playerIdInput.onSubmit.AddListener((value) => OnRegisterClicked());
        }
    }

    private void OnRegisterClicked()
    {
        if (sceneManager == null)
        {
            Debug.LogError("LoginSceneManager가 없습니다!");
            return;
        }

        string playerId = playerIdInput.text.Trim();

        if (string.IsNullOrEmpty(playerId))
        {
            Debug.Log("플레이어 ID를 입력해주세요.");
            return;
        }

        if (playerId.Length < 2)
        {
            Debug.Log("플레이어 ID는 2글자 이상이어야 합니다.");
            return;
        }

        if (playerId.Length > 20)
        {
            Debug.Log("플레이어 ID는 20글자 이하여야 합니다.");
            return;
        }

        // PlayerDataManager를 사용하여 데이터 존재 여부 확인 후 등록
        if (PlayerDataManager.HasPlayerData(playerId))
        {
            Debug.Log("이미 존재하는 플레이어입니다. 다른 ID를 사용해주세요.");
        }
        else
        {
            sceneManager.Register(playerId);
        }
    }

    private void OnGoToLoginClicked()
    {
        if (sceneManager != null)
        {
            sceneManager.ShowLoginPanel();
        }
    }

    private void OnEnable()
    {
        // 패널이 활성화될 때 입력 필드 포커스
        if (playerIdInput != null)
        {
            playerIdInput.text = "";
            playerIdInput.Select();
            playerIdInput.ActivateInputField();
        }
    }
}
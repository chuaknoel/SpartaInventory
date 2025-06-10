using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginPanel : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_InputField playerIdInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button goToRegisterButton;

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
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginClicked);
        }

        if (goToRegisterButton != null)
        {
            goToRegisterButton.onClick.AddListener(OnGoToRegisterClicked);
        }
    }

    private void SetupInputField()
    {
        if (playerIdInput != null)
        {
            // 엔터키로 로그인
            playerIdInput.onSubmit.AddListener((value) => OnLoginClicked());
        }
    }

    private void OnLoginClicked()
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

        // 데이터 존재 여부 확인 후 로그인
        if (DataManager.HasPlayerData(playerId))
        {
            sceneManager.Login(playerId);
        }
        else
        {
            Debug.Log("존재하지 않는 플레이어입니다. 회원가입을 해주세요.");
        }
    }

    private void OnGoToRegisterClicked()
    {
        if (sceneManager != null)
        {
            sceneManager.ShowRegisterPanel();
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
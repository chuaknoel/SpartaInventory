using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginSceneManager : MonoBehaviour
{
    [Header("=== LOGIN SCENE MANAGER ===")]

    [Header("Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;

    [Header("Login Panel UI")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button goToRegisterButton;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Test Buttons")]
    [SerializeField] private Button testButton1;
    [SerializeField] private Button testButton2;
    [SerializeField] private Button testButton3;

    private void Start()
    {
        SetupUI();
        SetupButtons();
        ShowLoginPanel(); // 시작시 로그인 패널 표시
    }

    private void SetupUI()
    {
        // 기본 상태 텍스트
        if (statusText != null)
        {
            statusText.text = "플레이어 이름을 입력하고 로그인하세요.";
            statusText.color = Color.white;
        }

        // 입력 필드 포커스
        if (playerNameInput != null)
        {
            playerNameInput.text = "";
            playerNameInput.Select();
            playerNameInput.ActivateInputField();
        }
    }

    private void SetupButtons()
    {
        // 로그인 버튼
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }

        // 회원가입으로 이동 버튼
        if (goToRegisterButton != null)
        {
            goToRegisterButton.onClick.AddListener(ShowRegisterPanel);
        }

        // 테스트 버튼들
        if (testButton1 != null)
        {
            testButton1.onClick.AddListener(() => CreateTestPlayer("테스트플레이어1"));
        }

        if (testButton2 != null)
        {
            testButton2.onClick.AddListener(() => CreateTestPlayer("테스트플레이어2"));
        }

        if (testButton3 != null)
        {
            testButton3.onClick.AddListener(() => CreateTestPlayer("테스트플레이어3"));
        }

        // 엔터키로 로그인
        if (playerNameInput != null)
        {
            playerNameInput.onSubmit.AddListener((value) => OnLoginButtonClicked());
        }
    }

    #region Panel Management

    /// <summary>
    /// 로그인 패널 표시
    /// </summary>
    public void ShowLoginPanel()
    {
        if (loginPanel != null) loginPanel.SetActive(true);
        if (registerPanel != null) registerPanel.SetActive(false);

        Debug.Log("로그인 패널 표시");
    }

    /// <summary>
    /// 회원가입 패널 표시
    /// </summary>
    public void ShowRegisterPanel()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (registerPanel != null) registerPanel.SetActive(true);

        Debug.Log("회원가입 패널 표시");
    }

    #endregion

    #region Login System (새로운 방식)

    /// <summary>
    /// 기존 플레이어로 로그인
    /// </summary>
    public void Login(string playerId)
    {
        ShowStatus("로그인 중...", Color.yellow);

        if (PlayerDataManager.HasPlayerData(playerId))
        {
            PlayerData playerData = PlayerDataManager.LoadPlayerData(playerId);
            if (playerData != null)
            {
                // GameManager에 플레이어 설정
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetCurrentPlayer(playerData);
                    ShowStatus($"{playerData.playerName}으로 로그인 성공!", Color.green);

                    // 잠시 기다린 후 인벤토리 씬으로 이동
                    Invoke(nameof(LoadInventoryScene), 1.0f);
                }
                else
                {
                    ShowStatus("GameManager를 찾을 수 없습니다!", Color.red);
                }
            }
            else
            {
                ShowStatus("플레이어 데이터 로드에 실패했습니다!", Color.red);
            }
        }
        else
        {
            ShowStatus("존재하지 않는 플레이어입니다!", Color.red);
        }
    }

    /// <summary>
    /// 새 플레이어 등록
    /// </summary>
    public void Register(string playerId)
    {
        ShowStatus("등록 중...", Color.yellow);

        if (PlayerDataManager.HasPlayerData(playerId))
        {
            ShowStatus("이미 존재하는 플레이어 ID입니다!", Color.red);
            return;
        }

        if (string.IsNullOrEmpty(playerId) || playerId.Length < 2)
        {
            ShowStatus("플레이어 ID는 2글자 이상이어야 합니다!", Color.red);
            return;
        }

        try
        {
            // 새로운 플레이어 데이터 생성
            PlayerData newPlayer = new PlayerData(playerId, playerId);

            // 시작 아이템 추가 (나중에 InventoryManager를 통해 처리 가능)
            newPlayer.inventoryItemIds.Add(1); // 기본 무기
            newPlayer.inventoryItemIds.Add(5); // 기본 물약

            // 데이터 저장
            if (PlayerDataManager.SavePlayerData(newPlayer))
            {
                // GameManager에 플레이어 설정
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetCurrentPlayer(newPlayer);
                    ShowStatus($"{playerId} 등록 및 로그인 성공!", Color.green);

                    // 잠시 기다린 후 인벤토리 씬으로 이동
                    Invoke(nameof(LoadInventoryScene), 1.0f);
                }
                else
                {
                    ShowStatus("GameManager를 찾을 수 없습니다!", Color.red);
                }
            }
            else
            {
                ShowStatus("플레이어 데이터 저장에 실패했습니다!", Color.red);
            }
        }
        catch (System.Exception e)
        {
            ShowStatus($"등록 실패: {e.Message}", Color.red);
            Debug.LogError($"플레이어 등록 오류: {e}");
        }
    }

    #endregion

    #region Legacy Methods (기존 호환성)

    private void OnLoginButtonClicked()
    {
        if (playerNameInput == null) return;

        string playerName = playerNameInput.text.Trim();

        // 입력 검증
        if (string.IsNullOrEmpty(playerName))
        {
            ShowStatus("플레이어 이름을 입력해주세요!", Color.red);
            return;
        }

        if (playerName.Length < 2)
        {
            ShowStatus("플레이어 이름은 2글자 이상이어야 합니다!", Color.red);
            return;
        }

        if (playerName.Length > 20)
        {
            ShowStatus("플레이어 이름은 20글자 이하여야 합니다!", Color.red);
            return;
        }

        // 기존 플레이어 체크 후 로그인 또는 생성
        if (PlayerDataManager.HasPlayerData(playerName))
        {
            Login(playerName);
        }
        else
        {
            Register(playerName);
        }
    }

    private void CreateTestPlayer(string testName)
    {
        if (PlayerDataManager.HasPlayerData(testName))
        {
            // 이미 존재하면 로그인
            Login(testName);
        }
        else
        {
            // 없으면 새로 생성
            Register(testName);
        }
    }

    #endregion

    #region Utility Methods

    private void LoadInventoryScene()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadInventoryScene();
        }
    }

    private void ShowStatus(string message, Color color)
    {
        //if (statusText != null)
        //{
        //    statusText.text = message;
        //    statusText.color = color;
        //}

        Debug.Log($"LoginScene: {message}");
    }

    #endregion

    #region Input Validation

    private bool IsValidPlayerName(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        if (name.Length < 2 || name.Length > 20) return false;

        // 특수문자 체크 (선택사항)
        foreach (char c in name)
        {
            if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c))
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region Context Menu Methods

    [ContextMenu("테스트 로그인")]
    private void TestLogin()
    {
        CreateTestPlayer("TestPlayer");
    }

    [ContextMenu("GameManager 확인")]
    private void CheckGameManager()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log("GameManager 정상 작동");
            Debug.Log($"현재 플레이어: {(GameManager.Instance.HasCurrentPlayer ? GameManager.Instance.CurrentPlayer.playerName : "없음")}");
        }
        else
        {
            Debug.LogWarning("GameManager가 없습니다!");
        }
    }

    [ContextMenu("저장된 플레이어 목록")]
    private void ShowSavedPlayers()
    {
        var playerIds = PlayerDataManager.GetAllPlayerIds();
        Debug.Log($"저장된 플레이어 ({playerIds.Count}명):");
        foreach (string id in playerIds)
        {
            Debug.Log($"  - {id}");
        }
    }

    [ContextMenu("저장 폴더 열기")]
    private void OpenSaveFolder()
    {
        PlayerDataManager.OpenSaveFolder();
    }

    #endregion
}
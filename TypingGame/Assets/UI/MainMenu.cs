using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private DifficultySO NormalDifficultySO;
    [SerializeField] private DifficultySO ChallengeDifficultySO;
    [SerializeField] private DifficultySO ExtremeDifficultySO;
    [SerializeField] private Camera _mainCamera;

    private MenuLevelButton[] _menuLevelButtons;

    private void Awake()
    {
        _menuLevelButtons = GetComponentsInChildren<MenuLevelButton>(includeInactive: true);
    }

    private void Start()
    {
        // TODO: Do this in a manager with states

        if (SceneHelper.IsSceneLoadedAdditively(SceneNames.MainMenu))
        {
            LoadSceneManager.Instance.OnLoadComplete += OnLoadComplete;
        }
        else
        {
            OnLoadComplete();
        }

        foreach (var button in _menuLevelButtons)
        {
            button.OnLevelSubmit += HandleLevelButtonSubmit;
        }
    }

    private void OnDestroy()
    {
        if (LoadSceneManager.Instance != null)
        {
            LoadSceneManager.Instance.OnLoadComplete -= OnLoadComplete;
        }

        foreach (var button in _menuLevelButtons)
        {
            button.OnLevelSubmit -= HandleLevelButtonSubmit;
        }
    }

    public void SetNormalDifficulty() => SetDifficulty(NormalDifficultySO);
    public void SetChallengeDifficulty() => SetDifficulty(ChallengeDifficultySO);
    public void SetExtremeDifficulty() => SetDifficulty(ExtremeDifficultySO);

    public void OnQuitButton() => Application.Quit();


    private void SetDifficulty(DifficultySO difficultySO)
    {
        GameSettingsManager.Instance.Difficulty = difficultySO;
    }

    private void Load(string sceneName)
    {
        SoundManager.Instance.PlayGameStart();
        LoadSceneManager.Instance.StartLoad(sceneName);
    }

    private void HandleLevelButtonSubmit(LevelSettingsSO levelSettings)
    {
        Load(levelSettings.SceneName);
    }

    private void OnLoadComplete()
    {
        _mainCamera.gameObject.SetActive(true);
        SceneHider.Instance.StartOfSceneFadeIn();
    }
}

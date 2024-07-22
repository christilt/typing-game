using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private DifficultySO NormalDifficultySO;
    [SerializeField] private DifficultySO ChallengeDifficultySO;
    [SerializeField] private DifficultySO ExtremeDifficultySO;

    private void Start()
    {
        // TODO: Do this in a manager with states
        SceneHider.Instance.StartOfSceneFadeIn();
    }

    public void SetNormalDifficulty() => SetDifficultyAndStartGame(NormalDifficultySO);
    public void SetChallengeDifficulty() => SetDifficultyAndStartGame(ChallengeDifficultySO);
    public void SetExtremeDifficulty() => SetDifficultyAndStartGame(ExtremeDifficultySO);

    public void OnQuitButton()
    {
        Application.Quit();
    }

    private void SetDifficultyAndStartGame(DifficultySO difficultySO)
    {
        GameSettingsManager.Instance.Difficulty = difficultySO;
        LoadingManager.Instance.StartLoad(LoadingManager.SceneNames.NewGameLevel);
    }
}

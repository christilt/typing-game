﻿using UnityEngine;
using UnityEngine.SceneManagement;
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

    public void SetNormalDifficulty() => SetDifficulty(NormalDifficultySO);
    public void SetChallengeDifficulty() => SetDifficulty(ChallengeDifficultySO);
    public void SetExtremeDifficulty() => SetDifficulty(ExtremeDifficultySO);

    public void Load(Scene scene) => LoadSceneManager.Instance.StartLoad(scene.name);
    public void World1Level1() => Load(SceneNames.Early1);
    public void World2Level1() => Load(SceneNames.Mid1);
    public void World3Level1() => Load(SceneNames.Later1);

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
}

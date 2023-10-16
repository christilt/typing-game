using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private CinemachineVirtualCamera _playerPlayingCamera;
    [SerializeField] private LevelHider _hider;

    public void CompleteLevel()
    {
        _playerPlayingCamera.gameObject.SetActive(false);
        _hider.Hide(2);
    }

    public void ReloadLevel()
    {
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
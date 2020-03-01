using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public int mainSceneIndex = 0;
    public int inventorySceneIndex = 1;
    public int fightSceneIndex = 2;
    public int gachaSceneIndex = 3;

    public AudioSource buttonSfx;
    
    public void ButtonAudio()
    {
        buttonSfx.Play();
    }
    
    public void LoadMainScene()
    {
        SceneManager.LoadScene(mainSceneIndex);
    }

    public void LoadInventoryScene()
    {
        SceneManager.LoadScene(inventorySceneIndex);
    }

    public void LoadFightScene()
    {
        SceneManager.LoadScene(fightSceneIndex);
    }

    public void LoadGachaScene()
    {
        SceneManager.LoadScene(gachaSceneIndex);
    }
}

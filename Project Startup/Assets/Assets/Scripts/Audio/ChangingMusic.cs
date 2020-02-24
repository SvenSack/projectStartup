using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangingMusic : MonoBehaviour
{
    [SerializeField] private GameObject mainMusic;
    [SerializeField] private GameObject fightMusic;
    [SerializeField] private GameObject winMusic;

    [SerializeField] private float crossFade;
    
    private bool audioBlendInprogress = false;

    public void StartFightMusic()
    {
        StartCoroutine(CrossFadeAudio(
            mainMusic.GetComponent<MusicManager>(), 
            fightMusic.GetComponent<MusicManager>(),
            crossFade, 
            1.0f,
            mainMusic,
            fightMusic));
    }

    public void StartWinMusic()
    {
        StartCoroutine(CrossFadeAudio(
            fightMusic.GetComponent<MusicManager>(), 
            winMusic.GetComponent<MusicManager>(), 
            crossFade, 
            1.0f,
            fightMusic,
            winMusic));
    }

    public void StartMainMusicAgain()
    {
        StartCoroutine(CrossFadeAudio(
            winMusic.GetComponent<MusicManager>(), 
            mainMusic.GetComponent<MusicManager>(), 
            crossFade, 
            1.0f, 
            winMusic, 
            mainMusic));
    }
    
//----------------------------------
// AUDIO CROSS-FADE
//----------------------------------
    private IEnumerator CrossFadeAudio(MusicManager oldAudioSource, MusicManager newAudioSource, float crossFadeTime, float newAudioSourceVolumeTarget, GameObject oldAudio, GameObject newAudio)
    {
        string debugStart = "<b><color=red>ERROR:</color></b> ";
        int maxLoopCount = 575;
        int loopCount = 0;
        float startAudioSource1Volume = oldAudioSource.loopMusic.volume;
        float startAudioSource1VolumeIntro = oldAudioSource.introMusic.volume;

        if(oldAudioSource == null || newAudioSource == null)
        {
            Debug.Log
                (debugStart + 
                 transform.name + 
                 ".EngineController.CrossFadeAudio received NULL value.\n*audioSource1=" + 
                 oldAudioSource + 
                 "\n*audioSource2=" + 
                 newAudioSource, 
                gameObject);
            
            yield return null;
        }
        else
        {
            audioBlendInprogress = true;
 
            newAudioSource.introMusic.volume = 0.0f;
            // newAudioSource.introMusic.Play();
            // newAudioSource.enabled = true;
            newAudio.SetActive(true);
            if (newAudioSource.introMusic.isPlaying == false &&
                newAudioSource.loopMusic.isPlaying == false)
            {
                newAudioSource.startedLoop = false;
                newAudioSource.introMusic.Play();
            }
 
            while ((oldAudioSource.loopMusic.volume > 0.0f && newAudioSource.introMusic.volume < newAudioSourceVolumeTarget) && loopCount <= maxLoopCount) //  && loopCount <= maxLoopCount
            {
                oldAudioSource.loopMusic.volume -= startAudioSource1Volume * Time.deltaTime / crossFadeTime;
                oldAudioSource.introMusic.volume -= startAudioSource1VolumeIntro * Time.deltaTime / crossFadeTime;
                newAudioSource.introMusic.volume += newAudioSourceVolumeTarget * Time.deltaTime / crossFadeTime;
                loopCount++;
                yield return null;
            }
 
            if (loopCount <= maxLoopCount)
            {
                // oldAudioSource.enabled = false;
                oldAudio.SetActive(false);
                oldAudioSource.loopMusic.Stop();
                oldAudioSource.loopMusic.volume = startAudioSource1Volume;
                oldAudioSource.introMusic.volume = startAudioSource1VolumeIntro;
                audioBlendInprogress = false;
            }
            
            else
            {
                Debug.Log
                (debugStart + 
                 transform.name + 
                 ".EngineController.CrossFadeAudio.loopCount reached max value.\nloopCount=" + 
                 loopCount + 
                 "\nmaxLoopCount=" + 
                 maxLoopCount, 
                 gameObject);
            }
        }
    }
    
    
    
    [SerializeField] private int debugScene;
    public void ChangeScenes()
    {
        SceneManager.LoadScene(debugScene);
    }
}

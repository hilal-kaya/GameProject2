using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.Audio; 
using System.IO; 

public class MainMenuController : MonoBehaviour
{
    public AudioMixer mainMixer; 

    public void LoadAIAndStart()
    {
        Debug.Log("Butona tıklandı, dosya aranıyor..."); 
        string path = Path.Combine(Application.streamingAssetsPath, "RLBrain.json");
        
        if (File.Exists(path))
        {
            GameManagerStatic.LoadedJsonData = File.ReadAllText(path);
            GameManagerStatic.IsAiLoaded = true;
            Debug.Log("DOSYA BULUNDU: " + GameManagerStatic.LoadedJsonData);
        }
        else
        {
            GameManagerStatic.IsAiLoaded = false;
            Debug.LogError("HATA: " + path + " adresinde dosya yok!");
        }
        SceneManager.LoadScene(1);
    }

    public void PlayGame()
    {
        GameManagerStatic.IsAiLoaded = false;
        SceneManager.LoadScene(1);
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        mainMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
    }

}
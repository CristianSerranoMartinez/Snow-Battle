using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour {

    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private Slider musicSlider;
    [SerializeField]
    private Slider effectsSlider;
    [SerializeField]
    GameObject selfCanvas;
    [SerializeField]
    GameObject menuCanvas;

   /*private void Start()
    {
        ConfigurationData cd =SaveSystemFile.LoadConfiguration();

        if (cd != null)
        {
            Debug.Log("From File");

            audioMixer.SetFloat("Music", cd.musicVolume);

            audioMixer.SetFloat("Effects", cd.effectsVolume);

            Debug.Log("cdm: " + cd.musicVolume);

            Debug.Log("cde: " + cd.effectsVolume);

            musicSlider.value = cd.musicVolume;

            effectsSlider.value = cd.effectsVolume;
        }
        else
        {
            float volume;

            if (audioMixer.GetFloat("Music", out volume))
            {
                musicSlider.value = volume;
            }

            if (audioMixer.GetFloat("Effects", out volume))
            {
                effectsSlider.value = volume;
            }
        }

    }*/

    private void OnEnable()
    {
        ConfigurationData cd = SaveSystemFile.LoadConfiguration();

        if (cd != null)
        {
            Debug.Log("From File");

            audioMixer.SetFloat("Music", cd.musicVolume);

            audioMixer.SetFloat("Effects", cd.effectsVolume);

            Debug.Log("cdm: " + cd.musicVolume);

            Debug.Log("cde: " + cd.effectsVolume);

            musicSlider.value = cd.musicVolume;

            effectsSlider.value = cd.effectsVolume;
        }
        else
        {
            float volume;

            if (audioMixer.GetFloat("Music", out volume))
            {
                musicSlider.value = volume;
            }

            if (audioMixer.GetFloat("Effects", out volume))
            {
                effectsSlider.value = volume;
            }
        }
    }

    public void ButtonOpenWeb()
    {
        Application.OpenURL("http://www.google.com");
    }

    public void ButtonBack()
    {

        menuCanvas.SetActive(true);
        selfCanvas.SetActive(false);
        //SceneManager.LoadScene("Menu");
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("Music", volume);
    }

    public void SetEffectsVolume(float volume)
    {
        audioMixer.SetFloat("Effects", volume);
    }

    public void ButtonSave()
    {
        SaveSystemFile.SaveConfiguration(musicSlider.value, effectsSlider.value);

        Debug.Log("Save...");
    }
}

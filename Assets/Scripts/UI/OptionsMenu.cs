using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// 
/// </summary>
public class OptionsMenu : MonoBehaviour
{

    #region Variable Declarations
    [SerializeField] TMPro.TMP_Dropdown resolutionDropdown = null;
    [SerializeField] Toggle fullscreenToggle = null;
    [SerializeField] Button backButton = null;
    [SerializeField] AudioMixer masterMixer = null;
    Resolution[] resolutions;

    float maxSFX;
    float maxMusic;
    #endregion



    #region Unity Event Functions
    private void Start()
    {
        masterMixer.GetFloat(Constants.MIXER_SFX_VOLUME, out maxSFX);
        masterMixer.GetFloat(Constants.MIXER_MUSIC_VOLUME, out maxMusic);

        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height) 
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreen;
	}
	
	private void Update()
    {
        if (InputHelper.GetButtonDown(RewiredConsts.Action.UICANCEL))
        {
            backButton.onClick.Invoke();
        }
	}
    #endregion



    #region Public Functions
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetSFXVolume(float volume)
    {
        masterMixer.SetFloat(Constants.MIXER_SFX_VOLUME, ExtensionMethods.Remap(Mathf.Log10(volume) * 20, -80f, 0f, -80f, maxSFX));
    }

    public void SetMusicVolume(float volume)
    {
        masterMixer.SetFloat(Constants.MIXER_MUSIC_VOLUME, ExtensionMethods.Remap(Mathf.Log10(volume) * 20, -80f, 0f, -80f, maxMusic));
    }
    #endregion



    #region Private Functions
    #endregion
}

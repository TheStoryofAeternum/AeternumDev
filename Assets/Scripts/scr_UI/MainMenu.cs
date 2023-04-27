using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// The master script for the main menu that controls all the core features and opens/closes UI.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject par_MMContent;
    [SerializeField] private GameObject par_LoadMenuContent;
    [SerializeField] private GameObject par_SettingsContent;
    [SerializeField] private GameObject par_CreditsContent;
    [SerializeField] private Button btn_Start;
    [SerializeField] private Button btn_ShowLoadMenuContent;
    [SerializeField] private Button btn_ShowSettings;
    [SerializeField] private Button btn_ShowCredits;
    [SerializeField] private Button btn_ReturnToMM;
    [SerializeField] private Button btn_Quit;

    private void Awake()
    {
        btn_Start.onClick.AddListener(StartGame);
        btn_ShowLoadMenuContent.onClick.AddListener(ShowLoadMenuContent);
        btn_ShowSettings.onClick.AddListener(ShowSettings);
        btn_ShowCredits.onClick.AddListener(ShowCredits);
        btn_ReturnToMM.onClick.AddListener(ShowMMContent);
        btn_Quit.onClick.AddListener(QuitGame);
    }

    /// <summary>
    /// Switches from the main menu scene to the game scene.
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    /// <summary>
    /// Closes settings and credits, opens main menu.
    /// </summary>
    public void ShowMMContent()
    {
        par_MMContent.SetActive(true);
        par_SettingsContent.SetActive(false);
        par_CreditsContent.SetActive(false);

        btn_ReturnToMM.gameObject.SetActive(false);
    }
    /// <summary>
    /// Closes main menu and opens load menu.
    /// </summary>
    public void ShowLoadMenuContent()
    {
        par_MMContent.SetActive(false);
        par_LoadMenuContent.SetActive(true);

        btn_ReturnToMM.gameObject.SetActive(true);
    }
    /// <summary>
    /// Closes main menu and opens settings.
    /// </summary>
    public void ShowSettings()
    {
        par_MMContent.SetActive(false);
        par_SettingsContent.SetActive(true);

        btn_ReturnToMM.gameObject.SetActive(true);
    }
    /// <summary>
    /// Closes main menu and opens credits.
    /// </summary>
    public void ShowCredits()
    {
        par_MMContent.SetActive(false);
        par_SettingsContent.SetActive(true);

        btn_ReturnToMM.gameObject.SetActive(true);
    }
    /// <summary>
    /// Force-quits the game.
    /// <para>
    /// Todo (greenlaser): Needs a confirmation check to ask the player if they really want to quit the game or not.
    /// </para>
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
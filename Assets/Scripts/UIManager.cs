using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Button = UnityEngine.UI.Button;

public class UIManager : MonoBehaviour
{
    #region Paramètres

    /// <summary>
    /// Instance du script
    /// </summary>
    public static UIManager instance;

    [SerializeField]
    private Text textCoupRestant;

    [SerializeField]
    private GameObject niveauBienvenuePrefab;
    [SerializeField]
    private GameObject mapBienvenuePrefab;

    #region Menus et Panel

    [SerializeField]
    private GameObject menuBienvenue;
    [SerializeField]
    private GameObject menuNiveaux;
    [SerializeField]
    private GameObject menuJeux;
    [SerializeField]
    private GameObject panneauOption;
    [SerializeField]
    private GameObject panneauFinDeJeux;

    #endregion

    #region Bouttons

    [SerializeField]
    private GameObject boutonRetry;
    [SerializeField]
    private GameObject boutonNext;
    [SerializeField]
    private GameObject prefabBoutonNiveau;
    [SerializeField]
    private GameObject container;

    #endregion

    #region Toggle et Sliders

    [SerializeField]
    private Slider volumeMusique;
    [SerializeField]
    private Slider volumeSfx;


    #endregion

    #region Getter

    public Text TextCoupRestant { get { return textCoupRestant; } }

    #endregion

    /// <summary>
    /// Initialise le singleton ou détruit l'instance en cours
    /// </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Crée les boutons de niveau ou lance le niveau selon l'état du jeu
    /// </summary>
    private void Start()
    {
        // Si le jeux n'a pas commencé, on créer les boutons
        if (GameManager.singleton.gameStatus == GameStatus.WELCOME)
        {
            //Instancie niveau pour fond
            Instantiate(niveauBienvenuePrefab, new Vector3(-8, 0, -6), Quaternion.identity);
            Instantiate(mapBienvenuePrefab, mapBienvenuePrefab.transform.position, Quaternion.identity);

            //affiche le bon menue
            menuBienvenue.SetActive(true);
        }
        else if(GameManager.singleton.gameStatus == GameStatus.LEVELSELECTION)
        {
            menuBienvenue.SetActive(false);
            panneauOption.SetActive(false);
            menuNiveaux.SetActive(true);
            CreerBoutonsNiveaux();
        }
        //Sinon si le jeux est en cours, on génère le terrain
        else if(GameManager.singleton.gameStatus == GameStatus.FAILED || GameManager.singleton.gameStatus == GameStatus.COMPLETE)
        {

            // Affiche le bon menu
            panneauOption.SetActive(false);
            menuNiveaux.SetActive(false);
            menuJeux.SetActive(true);

            // Génère le niveau
            LevelManager.instance.InstancieNiveau(GameManager.singleton.currentLevelIndex);
        }
    }

    #region Boutons en jeux

    /// <summary>
    /// Réinitialise le jeu et recharge la scène principale
    /// </summary>
    public void LevelSelectionButton()
    {
        // Sons du bouton
        SoundManager.singleton.PlaySfx("button");

        // Change l'état du jeu
        GameManager.singleton.gameStatus = GameStatus.LEVELSELECTION;

        // Regénère la scène
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenOptionButton()
    {
        // Sons du bouton
        SoundManager.singleton.PlaySfx("button");

        // Affiche les options
        panneauOption.SetActive(true);
    }

    public void CloseOptionButton()
    {
        // Sons du bouton
        SoundManager.singleton.PlaySfx("button");

        // Enlève les options
        panneauOption.SetActive(false);
    }

    /// <summary>
    /// Recharge la scène actuelle pour relancer ou passer au niveau suivant
    /// </summary>
    public void RetryNextButton()
    {
        // Sons du bouton
        SoundManager.singleton.PlaySfx("button");

        // Génère le niveau
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CloseGame()
    {
        // Sons du bouton
        SoundManager.singleton.PlaySfx("button");
        //Ferme le jeux
        Application.Quit();
    }

    #endregion

    #region Sliders


    public void MusicVolume()
    {
        SoundManager.singleton.VolumeMusic(volumeMusique.value);
    }

    public void SfxVolume()
    {
        SoundManager.singleton.VolumeSfx(volumeSfx.value);
    }

    #endregion

    #endregion

    #region Helpers

    /// <summary>
    /// Affiche le panneau de fin de partie avec les options "Retry" ou "Next"
    /// </summary>
    public void ResultatPartie()
    {
        // Affiche le bon niveau
        panneauFinDeJeux.SetActive(true);

        // Suivant l'état du jeu, affiche le bon bouton
        switch (GameManager.singleton.gameStatus)
        {
            case GameStatus.COMPLETE:
                // Joue un sons de victoire
                SoundManager.singleton.PlaySfx("success");

                boutonNext.SetActive(true);
                break;
            case GameStatus.FAILED:
                // Joue un sons de défaite
                SoundManager.singleton.PlaySfx("failed");

                boutonRetry.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Crée les boutons de niveau avec des actions de clic associées
    /// </summary>
    private void CreerBoutonsNiveaux()
    {
        // Pour chaque niveau, créer un bouton correspondant
        for (int i = 0; i < LevelManager.instance.LevelDatas.Length; i++)
        {
            // Créer le bouton
            GameObject levelButton = Instantiate(prefabBoutonNiveau, container.transform);
            // Lui attribut un numéro
            levelButton.transform.GetChild(0).GetComponent<Text>().text = "" + (i + 1);
            // Ecoute les évènement du bouton
            Button button = levelButton.GetComponent<Button>();
            button.onClick.AddListener(() => OnClick(button));
        }
    }

    /// <summary>
    /// Gère le clic sur un bouton de niveau en lançant le niveau correspondant
    /// </summary>
    private void OnClick(Button button)
    {
        // Sons du bouton
        SoundManager.singleton.PlaySfx("button");

        // Affiche le bon menu
        menuJeux.SetActive(true);
        menuNiveaux.SetActive(false);

        // Génère le bon niveau
        GameManager.singleton.currentLevelIndex = button.transform.GetSiblingIndex();
        LevelManager.instance.InstancieNiveau(GameManager.singleton.currentLevelIndex);
    }

    #endregion
}

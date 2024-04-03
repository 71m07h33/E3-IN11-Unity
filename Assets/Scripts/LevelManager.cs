using UnityEngine;

/// <summary>
/// Permet de gérer les niveaux
/// </summary>
public class LevelManager : MonoBehaviour
{
    #region Paramètres

    /// <summary>
    /// Instance du script
    /// </summary>
    public static LevelManager instance;

    #region Objets

    [SerializeField]
    private GameObject prefabBalle;
    [SerializeField]
    private LevelData[] listeNiveaux;

    #endregion

    #region Coups

    private int nombreDeCoup = 0;
    private int nombreDeCoupMax = 0;

    #endregion

    #region Accesseur

    public LevelData[] LevelDatas { get {  return listeNiveaux; } }

    #endregion

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

    #region Helpers

    /// <summary>
    /// Instancie le niveau spécifié, place le joueur (représenté par la balle) et initialise le statut de jeu en "PLAYING".
    /// </summary>
    public void InstancieNiveau(int indexNiveau)
    {
        // Met l'état du jeu à "PLAYING"
        GameManager.singleton.gameStatus = GameStatus.PLAYING;

        // Instantie la balle et le niveau
        Instantiate(prefabBalle, Vector3.up*3, Quaternion.identity);
        Instantiate(LevelDatas[indexNiveau].prefabNiveau, Vector3.zero, Quaternion.identity);
        Instantiate(LevelDatas[indexNiveau].prefabMap, LevelDatas[indexNiveau].prefabMap.transform.position, Quaternion.identity);

        // Récupère le nombre de coups maximum par niveau
        nombreDeCoupMax = listeNiveaux[indexNiveau].limiteDeCoup;
    }

    /// <summary>
    /// Incrémente le compteur de tirs et met à jour l'affichage du nombre de tirs. Si le nombre maximal de tirs est dépassé, déclenche l'échec du niveau
    /// </summary>
    public void CoupTire()
    {

        // S'il reste des coups au joueur
        if(nombreDeCoupMax > 0)
        {
            // Incrémente le nombre de coup
            nombreDeCoup++;

            // Change le texte en haut de l'écran
            UIManager.instance.TextCoupRestant.text = nombreDeCoup+"/"+nombreDeCoupMax ;

            // Si lejoueur n'a plus de coup, il a perdu
            if(nombreDeCoup >= nombreDeCoupMax)
                NiveauPerdue();
        }
    }

    #region Etat des niveaux

    /// <summary>
    /// Marque le niveau comme terminé si le jeu est en cours, passe au niveau suivant s'il existe, sinon retourne au premier niveau. Affiche ensuite le résultat du jeu
    /// </summary>
    public void NiveauTermine()
    {
        // Si la partie est en cours
        if(GameManager.singleton.gameStatus == GameStatus.PLAYING)
        {
            // On a terminé le niveau
            GameManager.singleton.gameStatus = GameStatus.COMPLETE;

            // Si ce n'est pas le dernier niveau
            if(GameManager.singleton.currentLevelIndex < listeNiveaux.Length-1)
                // On incrémente l'index du niveau actuel
                GameManager.singleton.currentLevelIndex++;
            // Sinon on recommmence à zéro
            else
                GameManager.singleton.currentLevelIndex = 0;

            // Affiche l'écran de fin de jeu
            UIManager.instance.ResultatPartie();
        }
    }

    /// <summary>
    /// Marque le niveau comme échoué si le jeu est en cours. Affiche ensuite le résultat
    /// </summary>
    public void NiveauPerdue()
    {
        // Si la partie est en cours
        if (GameManager.singleton.gameStatus == GameStatus.PLAYING)
        {
            // On a perdu le niveau
            GameManager.singleton.gameStatus = GameStatus.FAILED;

            // Affiche l'écran de fin de jeu
            UIManager.instance.ResultatPartie();
        }
    }

    #endregion

    #endregion
}

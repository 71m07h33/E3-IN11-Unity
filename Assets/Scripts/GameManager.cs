using UnityEngine;

#region Game Status

[System.Serializable]
/// <summary>
/// Etat de l'application
/// </summary>
public enum GameStatus
{
    WELCOME,
    LEVELSELECTION, 
    PLAYING,
    FAILED,
    COMPLETE
}

#endregion

/// <summary>
/// Gère le jeux dans son ensemble
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Paramètres

    /// <summary>
    /// Singleton of the script
    /// </summary>
    public static GameManager singleton;

    #region Paramètre du jeu

    [HideInInspector]
    public int currentLevelIndex = 0;
    [HideInInspector]
    public GameStatus gameStatus = GameStatus.LEVELSELECTION;

    #endregion

    #endregion

    /// <summary>
    /// Initialise le singleton ou détruit l'instance en cours, assurant qu'une seule instance de ce script existe dans la scène
    /// </summary>
    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            // Ne supprime pas l'objet à la génération de nouveau niveau
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
}

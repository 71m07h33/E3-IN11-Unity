using UnityEngine;

#region Vecteurs

[System.Serializable]
/// <summary>
/// Vecteurs actif représenté par des booléens
/// </summary>
public class ActiveVectors
{
    public bool x, y, z;
}

#endregion

/// <summary>
/// Permet à la caméra de suivre la balle
/// </summary>
public class CameraFollow : MonoBehaviour
{
    #region Paramètres

    /// <summary>
    /// Instance du script
    /// </summary>
    public static CameraFollow instance;

    #region Paramètre suivi

    [SerializeField]
    private ActiveVectors vecteursActifs;
    private GameObject cible;

    #endregion

    #region Vecteurs positions

    private Vector3 decalage;
    private Vector3 nouvellePosition;

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

    /// <summary>
    /// Met à jour la position de la caméra pour suivre la cible selon les axes actifs
    /// </summary>
    private void LateUpdate()
    {
        // Si la cible existe
        if (cible)
        {
            if (vecteursActifs.x) nouvellePosition.x = cible.transform.position.x - decalage.x;
            if (vecteursActifs.y) nouvellePosition.y = cible.transform.position.y - decalage.y;
            if (vecteursActifs.z) nouvellePosition.z = cible.transform.position.z - decalage.z;

            // Met à jours la nouvelle position
            transform.position = nouvellePosition;
        }
    }

    #region Helper

    /// <summary>
    /// Définit la cible à suivre et initialise les variables nécessaires
    /// </summary>
    public void InstancieCible(GameObject target)
    {
        // Initialisation des variables
        cible = target;
        nouvellePosition = transform.position;
        decalage = cible.transform.position - transform.position;
    }

    #endregion
}

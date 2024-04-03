using UnityEngine;
using static UnityEngine.ParticleSystem;

[RequireComponent(typeof(Rigidbody))]

/// <summary>
/// Gère les actions de la balle
/// </summary>
public class BallController : MonoBehaviour
{
    #region Paramètres

    /// <summary>
    /// Instance du script
    /// </summary>
    public static BallController instance;

    #region Contrôle du jeu

    /// <summary>
    /// Permet de dessiner la ligne de tire du joueur
    /// </summary>
    [SerializeField]
    private LineRenderer ligneDeTire;

    /// <summary>
    /// Zone autour de la balle, pour une utilisation plus claire
    /// </summary>
    [SerializeField]
    private GameObject zoneDeTire;

    /// <summary>
    /// Permet de spécifier sur quelle couche on calcule la position du clic de la souris
    /// </summary>
    [SerializeField]
    private LayerMask CoucheDeTire;

    private Rigidbody rigidBody;

    #endregion

    #region Force

    [SerializeField]
    private float forceMaximal;
    [SerializeField]
    private float coefficientModificationForce;

    private float force;

    #endregion

    #region Direction

    private Vector3 positionDepart;
    private Vector3 positionArrivee;
    private Vector3 direction;

    #endregion

    #region Etat de la balle

    private ParticleSystem streak;
    private bool peutTirer = false;
    private bool estStatique = true;

    #endregion

    #endregion

    /// <summary>
    /// Initialise le singleton instance et récupère le composant Rigidbody attaché au GameObject.
    /// </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        rigidBody = GetComponent<Rigidbody>();
        streak = FindAnyObjectByType<ParticleSystem>();
    }

    /// <summary>
    /// Configure la caméra pour suivre la balle
    /// </summary>
    private void Start()
    {
        CameraFollow.instance.InstancieCible(gameObject);
    }

    /// <summary>
    /// Vérifie si la balle est immobile et déclenche une action si c'est le cas
    /// </summary>
    private void Update()
    {
        streak.startSpeed = rigidBody.velocity.x;

        if (rigidBody.velocity == Vector3.zero && !estStatique)
        {
            // Met les variables de la balle dans un état statique
            estStatique = true;
            rigidBody.velocity = Vector3.zero;
            // Permet de tirer
            zoneDeTire.SetActive(true);
            // Permet de décompter un coup
            LevelManager.instance.CoupTire();

        }
            
    }

    /// <summary>
    /// Applique la force au Rigidbody de la balle lorsque le tir est effectué, déclenche des actions et des sons associés
    /// </summary>
    private void FixedUpdate()
    {
        if (peutTirer)
        {
            // Modifie les bouléens d'états de la balle
            peutTirer = false;
            estStatique = false;
            // Désactive la zone de tire
            zoneDeTire.SetActive(false);

            // Met un coup dans la balle
            direction = positionDepart - positionArrivee;
            rigidBody.AddForce(direction * force, ForceMode.Impulse);
            force = 0;

            // Initialise les positions de départ et d'arrivée à zero
            positionDepart = Vector3.zero;
            positionArrivee = Vector3.zero;

            // Joue un son de tire
            SoundManager.singleton.PlaySfx("throw");
        }
    }

    #region Collisions

    /// <summary>
    /// Gère les collisions avec d'autres objets, déclenchant des actions spécifiques lorsque la balle entre en collision avec un objet "Hole" ou "Destroyer".
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Si la balle est dans le troue
        if(other.name == "Hole")
        {
            // Termine le niveau
            LevelManager.instance.NiveauTermine();
        }

        // Si la balle atteint les limites de la carte
        if (other.name == "Terrain")
            // Echoue le niveau
            LevelManager.instance.NiveauPerdue();
    }

    #endregion

    #region Méthode de souris

    /// <summary>
    /// Déclenché lors du clic de souris, enregistre la position de départ pour le tir
    /// </summary>
    public void SourisCliquee()
    {
        // On ne tire pas si la balle roule
        if (!estStatique)
            return;
        
        // Dessine une ligne
        ligneDeTire.gameObject.SetActive(true);

        // Enregistre la position de départ et dessine la ligne jusqu'à la souris
        positionDepart = PositionClique();
        ligneDeTire.SetPosition(0, ligneDeTire.transform.localPosition);
    }

    /// <summary>
    /// Calcul de la force du tir en fonction de la distance entre la position de départ et la position actuelle de la souris
    /// </summary>
    public void SourisEnfoncee()
    {
        // Enregistre la position d'arrivee
        positionArrivee = PositionClique();
        positionArrivee.y = ligneDeTire.transform.position.y;

        // Enregistre la force du joueur
        force = Mathf.Clamp(Vector3.Distance(positionArrivee, positionDepart) * coefficientModificationForce, 0, forceMaximal);

        // Dessine la ligne jusqu'à la position d'arrivée
        ligneDeTire.SetPosition(1, transform.InverseTransformPoint(positionArrivee));
    }

    /// <summary>
    /// Déclenché lorsque le bouton de la souris est relâché, indiquant que le tir est prêt à être effectué
    /// </summary>
    public void SourisRelachee()
    {
        // Modifie la variable d'état de la balle
        peutTirer = true;

        // Supprime la ligne
        ligneDeTire.gameObject.SetActive(false);
    }

    #endregion

    #region Helper

    /// <summary>
    /// Renvoie la position dans le monde où le rayon de la souris interagit avec les objets de la scène
    /// </summary>
    Vector3 PositionClique()
    {
        // Créer une position initialisé à la position à zéro
        Vector3 position = Vector3.zero;
        // Créer un rayon partant de la souris
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Détecte une intersection entre le rayon partant de la souris et la couhe de tire.
        if (Physics.Raycast(ray, out RaycastHit intersection, Mathf.Infinity, CoucheDeTire))
        {
            position = intersection.point;
        }

        // Retourne la position du clic
        return position;
    }

    #endregion
}

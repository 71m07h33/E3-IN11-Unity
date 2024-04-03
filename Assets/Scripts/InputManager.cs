using UnityEngine;

/// <summary>
/// Gère la sourie en entrée.
/// </summary>
public class InputManager : MonoBehaviour
{
    #region Paramètres

    [SerializeField]
    private float limiteDeDistance = 1.5f;

    private float distanceDeTrainee;

    private bool peutTournerLaCamera = false;

    #endregion

    /// <summary>
    /// Gère les entrées utilisateur pour contrôler la caméra et la balle.
    /// </summary>
    void Update()
    {
        // Si on ne joue pas, on ne tiens pas compte des entrées, on sort de la fonction.
        if (GameManager.singleton.gameStatus != GameStatus.PLAYING)
            return;

        //Si le bouton gauche de la sourie est enfoncé et la caméra ne peut pas tourner
        if(Input.GetMouseButtonDown(0) && !peutTournerLaCamera)
        {
            // On peut tourner la caméra :)
            peutTournerLaCamera = true;

            // On stock la distance
            GetDistance();

            // Si la distance est dans le périmètre de la balle (1.5f)
            if (distanceDeTrainee <= limiteDeDistance)
                // On tire
                BallController.instance.SourisCliquee();
        }

        // Si on peut tourner
        if(peutTournerLaCamera)
        {
            // Si le clic gauche est enfoncé actuellement
            if(Input.GetMouseButton(0))
            {
                // Si la distance est dans le périmètre de la balle 
                if (distanceDeTrainee <= limiteDeDistance)
                    BallController.instance.SourisEnfoncee();
                else
                    CameraRotation.instance.TourneLaCamera(Input.GetAxis("Mouse X"));
            }

            if (Input.GetMouseButtonUp(0))
            {
                peutTournerLaCamera = false;

                if (distanceDeTrainee <= limiteDeDistance)
                    BallController.instance.SourisRelachee();
            }
        }
    }

    #region Helpers

    /// <summary>
    /// Retourne la distance entre la souris et la balle.
    /// </summary>
    void GetDistance()
    {
        // Créer un rayon qui va de la caméra à la souris
        var rayon = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Créer un plan qui va de la sourie à la balle
        var planSourisBalle = new Plane(Camera.main.transform.forward, BallController.instance.transform.position);

        // Si le rayon intersecte le plan, rayon vaudra true et distance vaudra la distance
        if(planSourisBalle.Raycast(rayon, out float distance))
        {
            // Point d'intersection entre le rayon et le plan
            var pointIntersection = rayon.GetPoint(distance);

            // L'attribut sera la distance entre la sourie et la balle
            distanceDeTrainee = Vector3.Distance(pointIntersection, BallController.instance.transform.position);
        }
    }

    #endregion
}

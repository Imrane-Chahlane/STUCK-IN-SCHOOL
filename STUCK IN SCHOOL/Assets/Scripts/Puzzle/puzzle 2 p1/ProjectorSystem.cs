using UnityEngine;
using UnityEngine.Video;

public class ProjectorSystem : MonoBehaviour
{
    [Header("Video Player")]
    public VideoPlayer videoPlayer;

    [Header("Disc Position")]
    public Transform discPlacePoint;

    [Header("Animation")]
    public string discAnimationStateName = "DiscSpin";

    private GameObject currentDisc;
    private Animator currentDiscAnimator;

    private void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    public void InsertDisc(GameObject discObject)
    {
        if (discObject == null)
        {
            Debug.LogWarning("No disc object provided.");
            return;
        }

        FilmDisc filmDisc = discObject.GetComponent<FilmDisc>();

        if (filmDisc == null)
        {
            Debug.LogWarning("This object is not a FilmDisc.");
            return;
        }

        if (filmDisc.filmClip == null)
        {
            Debug.LogWarning("This disc has no video clip assigned.");
            return;
        }

        if (videoPlayer == null)
        {
            Debug.LogError("Video Player is not assigned in ProjectorSystem.");
            return;
        }

        if (discPlacePoint == null)
        {
            Debug.LogError("Disc Place Point is not assigned in ProjectorSystem.");
            return;
        }

        currentDisc = discObject;

        currentDisc.SetActive(true);
        currentDisc.transform.SetParent(null);

        currentDisc.transform.position = discPlacePoint.position;
        currentDisc.transform.rotation = discPlacePoint.rotation;
        currentDisc.transform.SetParent(discPlacePoint);

        Rigidbody rb = currentDisc.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = currentDisc.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Start disc animation
        currentDiscAnimator = currentDisc.GetComponent<Animator>();

        if (currentDiscAnimator != null)
        {
            currentDiscAnimator.enabled = true;
            currentDiscAnimator.Play(discAnimationStateName, 0, 0f);
            Debug.Log("Disc animation started.");
        }
        else
        {
            Debug.LogWarning("No Animator found on inserted disc.");
        }

        // Start video
        videoPlayer.isLooping = false;
        videoPlayer.clip = filmDisc.filmClip;
        videoPlayer.Play();

        Debug.Log("Disc inserted and video started: " + filmDisc.discName);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video finished.");

        if (currentDiscAnimator != null)
        {
            currentDiscAnimator.enabled = false;
            Debug.Log("Disc animation stopped.");
        }
    }
}
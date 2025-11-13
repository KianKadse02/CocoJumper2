using UnityEngine;

public class JumpCollectible : MonoBehaviour
{
    [Header("Effects")]
    [Tooltip("Optional: A particle effect to play on collection")]
    public GameObject collectionEffect;

    [Tooltip("Optional: A sound to play on collection")]
    public AudioClip collectionSound;


    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is the Player
        if (other.CompareTag("Player"))
        {
            // Try to get the PlayerMovement component from the player object
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

            // Make sure the player has the script before we try to use it
            if (playerMovement != null)
            {
                // Play effects before destroying the object
                PlayEffects();

                // Call the public method on the player's script
                playerMovement.AddAirJump();

                // The collectible has been used, so destroy it
                Destroy(gameObject);
            }
        }
    }

    private void PlayEffects()
    {
        // If a particle effect has been assigned, create it
        if (collectionEffect != null)
        {
            Instantiate(collectionEffect, transform.position, Quaternion.identity);
        }

        // If a sound has been assigned, play it at the collectible's position
        // This is a great way to play a sound without needing an AudioSource component
        if (collectionSound != null)
        {
            AudioSource.PlayClipAtPoint(collectionSound, transform.position);
        }
    }
}
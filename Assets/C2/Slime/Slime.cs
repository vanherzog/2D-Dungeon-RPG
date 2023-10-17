using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    [SerializeField] Sprite[] idleSprites;
    [SerializeField] float timeBetweenFrames = 0.1f;

    SpriteRenderer sr;
    float animationTimer = 0f;
    int animationIndex = 0;

    void Awake() => sr = GetComponent<SpriteRenderer>();

    public Canvas canvas; // Reference to the Canvas you want to activate
    public GameObject all;
    public GameObject sprite;

    private bool playerInRange = false;

    void Update()
    {
        // Animate the slime sprite
        if (idleSprites.Length > 0)
        {
            if (animationTimer >= timeBetweenFrames)
            {
                sr.sprite = idleSprites[++animationIndex % idleSprites.Length];
                animationTimer = 0f;
            }
          
            animationTimer += Time.deltaTime;
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            canvas.gameObject.SetActive(true);
            all.SetActive(false);
            sprite.SetActive(false);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Trigger");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    public GameObject respawnPoint;

    [SerializeField] private AudioSource deathSoundEffect;

    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }


    // Detect if player collide trap
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            StartCoroutine(Die(2f));
        }
    }

    // Cooldown function before respawn
    private IEnumerator Die(float coolDownBeforeRespawn)
    {
        //deathSoundEffect.Play();
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetInteger("state", 5);

        yield return new WaitForSeconds(coolDownBeforeRespawn);

        rb.bodyType = RigidbodyType2D.Dynamic;
        anim.SetInteger("state", 0);
        rb.gameObject.transform.position = respawnPoint.transform.position;
    }
}

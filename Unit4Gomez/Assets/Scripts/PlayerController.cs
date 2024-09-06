using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameObject focalPoint;
    private GameObject cam;
    public GameObject powerupIndicator;

    private Rigidbody playerRb;
    private Rigidbody enemyRb;

    public bool hasPowerup;

    public float speed = 80;
    private float powerupStrength = 25.0f;

    public float smashForce;
    public float explosionForce;
    public float jumpForce;
    public float explosionRadius;
    public bool jumped = false;

    public PowerUpType currentPowerUp = PowerUpType.None;

    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerupCountdown;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
        cam = GameObject.Find("Camera");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float forwardInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed);
        playerRb.AddForce(focalPoint.transform.right * horizontalInput * speed);

        
    }

    private void Update()
    {
        //this doesnt work well in fixed update so its going here
        if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Pew");
            LaunchRockets();
        }

        if (currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) && !jumped)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumped = true;
            StartCoroutine(SmashDown());
        }

        powerupIndicator.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            hasPowerup = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            powerupIndicator.SetActive(true);
            Destroy(other.gameObject);

            if (powerupCountdown != null)
            {
                Debug.Log("Powerup Timer Reset");
                StopCoroutine(powerupCountdown);
            }
            powerupCountdown = StartCoroutine(PowerupCountdownRoutine());
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        powerupIndicator.SetActive(false);
        hasPowerup = false;
        currentPowerUp = PowerUpType.None;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp == PowerUpType.Pushback)
        {
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);

            Debug.Log("Player collided with " + collision.gameObject.name + " with powerup set to " + currentPowerUp.ToString());
            enemyRb.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
        }

        if (collision.gameObject.CompareTag("Ground") && jumped)
        {

            var enemies = FindObjectsOfType<Enemy>();
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null)
                {
                    enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
                }
            }
            jumped = false;
        }
    }

    void LaunchRockets()
    {
        foreach(var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            tmpRocket.GetComponent<ProjBehavior>().Fire(enemy.transform);
        }
    }

    IEnumerator SmashDown()
    {
        yield return new WaitForSeconds(0.35f);
        playerRb.AddForce(Vector3.down * smashForce * 5, ForceMode.Impulse);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int SHRINKING_DELAY = 10;
    private const int BLUE = 0, RED = 1, YELLOW = 2;
    public float forwardSpeed = 20f;
    public float sideSpeed = 15f;
    public float boostingSpeed = 20f;
    public Vector3 velocity;


    public Material playerMaterial;
    public ParticleSystem[] explosions;
    public bool sockPower = false;
    public bool onCheckpoint = true;
    [HideInInspector]
    public bool isRed = false;
    [HideInInspector]
    public bool isYellow = false;
    public AudioSource pop;
    public AudioSource roll;

    public Rigidbody playerRb;
    private GameManager gameManager;
    private SpawnManager spawnManager;
    private float sockPowerCoolDown = 0f;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        pop = GetComponent<AudioSource>();
        StartCoroutine(ShrinkFasterOverTime());
    }

    void Update()
    {
        MoveSideways();
        velocity = playerRb.GetPointVelocity(transform.position);
        roll.pitch = playerRb.velocity.magnitude / forwardSpeed;
        sockPowerCoolDown -= Time.deltaTime;

        //Check Player position relative to Checkpoints to offset the slow Collider;
        if(transform.position.z >= spawnManager.checkpoint_Position-240 - 2.5f && transform.position.z <= spawnManager.checkpoint_Position-240 + 2.5f)
        {
            onCheckpoint = true;
            roll.Stop();
        }
        else
        {
            onCheckpoint = false;
        }
        
        if (onCheckpoint && GameManager.isRunning)
        {
            transform.Translate(new Vector3(0,0,1) * Time.deltaTime,Space.World);
            transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime / 15;
            transform.position = new Vector3(transform.position.x, (transform.localScale.y / 2), transform.position.z);
            transform.Rotate(Vector3.forward);
            
            //Shoot away from Checkpoint on Space down
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopCoroutine("ChangeColor");
                playerRb.AddForce(Vector3.forward * boostingSpeed, ForceMode.Impulse);
            }  
        }
        else if(!onCheckpoint && GameManager.isRunning)
        {
            playerRb.AddForce(Vector3.forward * forwardSpeed * Time.deltaTime, ForceMode.Impulse);
            transform.localScale -= new Vector3(1,1,1) * Time.deltaTime / SHRINKING_DELAY;
            transform.position = new Vector3(transform.position.x,((transform.localScale.y / 2)) , transform.position.z);

            //if shrunk too small end game

            if (transform.localScale.x < 0 )
            {
                playerRb.velocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;
                gameManager.EndGame();
            }
        }
        if (GameManager.isRunning && sockPowerCoolDown < 0)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if(PlayerPrefs.GetInt("SockCount") > 0)
                {
                    gameManager.ReduceSockCount();
                    StartCoroutine(ActivateSockPower());
                    sockPowerCoolDown = 10;
                }
            }
        }
         
    }

    private void MoveSideways()
    {
        if (GameManager.isRunning)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            playerRb.AddForce(Vector3.right * Time.deltaTime * sideSpeed * horizontalInput, ForceMode.Impulse);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            onCheckpoint = true;
            StartCoroutine("ChangeColor");
            transform.position = new Vector3(transform.position.x, transform.localScale.y / 2, spawnManager.checkpoint_Position - 240 - 2.5f);
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;

        }
    }   
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            roll.Play();
            StopCoroutine("ChangeColor");
            spawnManager.SpawnCrateLine();
            spawnManager.SpawnBoostingPadsAndRandomCrates();
            spawnManager.SpawnEnvironment();
            spawnManager.SpawnCheckpoint();
            onCheckpoint = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isRed && !isYellow && collision.gameObject.CompareTag("Blue Crate"))
        {
            pop.Play();
            Destroy(collision.gameObject.gameObject);
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            transform.position = new Vector3(transform.position.x, (transform.localScale.y / 2), transform.position.z);
            playerRb.velocity = velocity;
            Instantiate(explosions[BLUE], collision.gameObject.transform.position, collision.gameObject.transform.rotation);
            gameManager.UpdateScore(20);

        }
        if (isRed && collision.gameObject.CompareTag("Red Crate"))
        {
            pop.Play();
            Destroy(collision.gameObject.gameObject);
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            transform.position = new Vector3(transform.position.x, (transform.localScale.y / 2), transform.position.z);
            playerRb.velocity = velocity;
            Instantiate(explosions[RED], collision.gameObject.transform.position, collision.gameObject.transform.rotation);
            gameManager.UpdateScore(20);


        }
        if(isYellow && collision.gameObject.CompareTag("Yellow Crate"))
        {
            pop.Play();
            Destroy(collision.gameObject.gameObject);
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            transform.position = new Vector3(transform.position.x, (transform.localScale.y / 2), transform.position.z);
            playerRb.velocity = velocity;
            Instantiate(explosions[YELLOW], collision.gameObject.transform.position, collision.gameObject.transform.rotation);
            gameManager.UpdateScore(20);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!isRed && !isYellow && collision.gameObject.CompareTag("Blue Crate"))
        {
            Destroy(collision.gameObject.gameObject);
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            transform.position = new Vector3(transform.position.x, (transform.localScale.y / 2), transform.position.z);
            playerRb.velocity = velocity;
            Instantiate(explosions[BLUE], collision.gameObject.transform.position, collision.gameObject.transform.rotation);
            gameManager.UpdateScore(20);

        }
        if (isRed && collision.gameObject.CompareTag("Red Crate"))
        {
            Destroy(collision.gameObject.gameObject);
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            transform.position = new Vector3(transform.position.x, (transform.localScale.y / 2), transform.position.z);
            playerRb.velocity = velocity;
            Instantiate(explosions[RED], collision.gameObject.transform.position, collision.gameObject.transform.rotation);
            gameManager.UpdateScore(20);


        }
        if (isYellow && collision.gameObject.CompareTag("Yellow Crate"))
        {
            Destroy(collision.gameObject.gameObject);
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            transform.position = new Vector3(transform.position.x, (transform.localScale.y / 2), transform.position.z);
            playerRb.velocity = velocity;
            Instantiate(explosions[YELLOW], collision.gameObject.transform.position, collision.gameObject.transform.rotation);
            gameManager.UpdateScore(20);
        }
    }
    IEnumerator ChangeColor()
    {
        while (onCheckpoint)
        {
            playerMaterial.color = Color.blue;
            isRed = false;
            isYellow = false;
            yield return new WaitForSeconds(0.5f);
            playerMaterial.color = Color.red;
            isRed = true;
            yield return new WaitForSeconds(0.5f);
            playerMaterial.color = Color.yellow;
            isRed = false;
            isYellow = true;
            yield return new WaitForSeconds(0.5f);
        }

    }
    IEnumerator ActivateSockPower()
    {
        for (int i = 0; i < 5; i++) {
            playerMaterial.color = Color.blue;
            isRed = false;
            isYellow = false;
            yield return new WaitForSeconds(0.5f);
            playerMaterial.color = Color.red;
            isRed = true;
            yield return new WaitForSeconds(0.5f);
            playerMaterial.color = Color.yellow;
            isRed = false;
            isYellow = true;
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator ShrinkFasterOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(60);
            SHRINKING_DELAY--;

        }
    }



}

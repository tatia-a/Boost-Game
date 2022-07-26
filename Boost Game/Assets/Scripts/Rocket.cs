using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float thrustSpeed = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip successSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool collisionsDisabled = false;

    enum State { Alive, Dying, Transceding }
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = rigidBody.GetComponent<AudioSource>(); // ?
    }

    // Update is called once per frame
    void Update()
    {
        if (Debug.isDebugBuild)
        { 
            RespondToDebugKeys(); 
        }
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    private void RespondToDebugKeys()
    {
        // load new level
        if (Input.GetKeyDown(KeyCode.L)) LoadNewLevel();
        // turns off collisions
        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionsDisabled) { return; } 

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    {
        state = State.Transceding;

        mainEngineParticles.Stop();
        audioSource.Stop();

        successParticles.Play();
        audioSource.PlayOneShot(successSound);
        Invoke("LoadNewLevel", levelLoadDelay);
    }
    private void StartDeathSequence()
    {
        state = State.Dying;

        mainEngineParticles.Stop();
        audioSource.Stop();

        deathParticles.Play();
        audioSource.PlayOneShot(deathSound);
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadNewLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if(nextSceneIndex >= SceneManager.sceneCountInBuildSettings) nextSceneIndex = 0;
        SceneManager.LoadScene(nextSceneIndex); 
    }
    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void RespondToRotateInput()
    {
        // ��������
        rigidBody.freezeRotation = true;

        float rotationThisFrame = rotationSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            mainEngineParticles.Stop();
            audioSource.Stop(); 
        }
    }

    private void ApplyThrust()
    {
        // ������ �� ������ (�� ������)
        float thrustThisFrame = thrustSpeed * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);

        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(mainEngine);
        if (!mainEngineParticles.isPlaying)
            mainEngineParticles.Play();
    }
}

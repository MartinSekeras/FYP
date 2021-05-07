using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed;             // Speed for player.
    public float jumpForce;             // Force of player jump.
    public float jumpBoost;             // Jump force after boost pickup.
    public float jumpMaxBeforeBoost;    // Allows for declaration of maximum jump height before jump boost.

    public int currentHealth;           // Health variable.
    private int minHealth;              // Minimum Health.
    public int takeDamage;              // Damage you take from hazards variable.

    public bool paused = false;         // Paused bool variable.
    public bool jumping = false;        // Jumping variable so we can jump only while touching ground platform

    GameObject[] pausedScreen;          // Array to hold all objects for pause screen.

    public Rigidbody2D rb;              // Rigidbody for collision.

    void Start()
    {
        /* Gets rigidbody component of our object */
        rb = GetComponent<Rigidbody2D>();

        /* Sets the minimum health for survival */
        minHealth = 0;

        /* Default time scale used for pausing game and unpausing game */
        Time.timeScale = 1;

        /* Add all game objects with tag OnPause to pausedScreen array and sets them inactive. */
        pausedScreen = GameObject.FindGameObjectsWithTag("OnPause");
        foreach(GameObject obj in pausedScreen)
        {
            obj.SetActive(false);
        }
    }

    void Update()
    {
        /* Horizonal input to right. */
        if (Input.GetKey("d") && Time.timeScale == 1)
        {
            transform.position += transform.right * (Time.deltaTime * moveSpeed);
        }

        /* Horizonal input to left. */
        if (Input.GetKey("a") && Time.timeScale == 1)
        {
            transform.position -= transform.right * (Time.deltaTime * moveSpeed);
        }

        /* Vertical jump. */
        if (Input.GetButtonDown("Jump") && Time.timeScale == 1 && jumping == false)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        /* Pauses and Unpauses the game. 
         * Sets all items in pausedScreen array to active and sets time to 0 to pause the game.
         */
        if (Input.GetKeyDown("escape") && paused == false)
        {
            Time.timeScale = 0;
            paused = true;
            foreach (GameObject obj in pausedScreen)
            {
                obj.SetActive(true);
            }
        }
        else if (Input.GetKeyDown("escape") && paused == true)
        {
            resumeGame();
        }

    }

    /* On Collision Trigger function that handles collision with objects labeled as trigger
     * This function handles collision with objects in a way so that the object that we collided with pass right through without interaction.
     */
    private void OnTriggerEnter2D(Collider2D other)
    {
        /* Deals damage and handles death if we collide with any hazard object. */
        if (other.gameObject.tag == "Hazard")
        {
            currentHealth -= takeDamage;

            if (currentHealth == minHealth) {
                restartLevel();
                print("Player died, resetting level.");
            }
        }

        /* Activates jump boost if we collide with jump boost object. */
        if (other.gameObject.tag == "JumpBoostActi")
        {
            if (jumpForce < jumpMaxBeforeBoost)
            {
                print("Jump Boost Active.");
                jumpForce += jumpBoost;
            }
            else
            {
                print("Jump Boost Already Active.");
            }
        }

        /* Deactives jump boost if we collide with object to deactivate jump boost and use regular jump.*/
        if (other.gameObject.tag == "JumpBoostDeac")
        {
            if (jumpForce >= jumpMaxBeforeBoost)
            {
                print("Jump Boost Deactivated.");
                jumpForce -= jumpBoost;
            }
            else
            {
                print("Jump Boost Already Deactivated.");
            }
        }

        /* Level selector using simple if statements. */
        if (other.gameObject.tag == "Exit")
        {
            if (SceneManager.GetActiveScene().name == "Level_1")
            {
                SceneManager.LoadScene(4);
            }

            if (SceneManager.GetActiveScene().name == "Level_2")
            {
                SceneManager.LoadScene(5);
            }

            if (SceneManager.GetActiveScene().name == "Level_3")
            {
                SceneManager.LoadScene(6);
            }

            if (SceneManager.GetActiveScene().name == "Level_4")
            {
                SceneManager.LoadScene(7);
            }

            if (SceneManager.GetActiveScene().name == "Level_5")
            {
                SceneManager.LoadScene(8);
            }
        }
    }

    /* On Collision function that handles collision with objects on Enter
     * This function handles collision with objects in a way so that the object that we collided with can be moved.
     */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        /* Jump platform collision check to see if player is touching platform or is mid jump.
         * IF we are touching platform we are not jumping and we can jump.
         */
        if (collision.gameObject.tag == "Platform")
        {
            jumping = false;
        }
    }

    /* On Collision function that handles collision with objects on Exit 
     * This function handles collision with objects after we depart from them and stop touching.
     */
    private void OnCollisionExit2D(Collision2D collision)
    {
        /* Jump platform collision check to see if player is touching platform or is mid jump.
         * If the player leaves the platform meaning he falls from the platform or jumps we no longer touching ground and therefore cannot jump
         */
        if (collision.gameObject.tag == "Platform")
        {
            jumping = true;
        }
    }

    /* This function handles level restart due to user selection or death
     */
    public void restartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /* This function handles level resume from paused state due to user selection such as button in paused menu.
     */
    public void resumeGame()
    {
        Time.timeScale = 1;
        paused = false;
        foreach (GameObject obj in pausedScreen)
        {
            obj.SetActive(false);
        }
    }
}

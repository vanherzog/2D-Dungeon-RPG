using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public Button attackButton;
    public Button defendButton;
    public Button healButton;
    public Button runButton;

    public Image hpBarImage;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI startText;
    public GameObject panel;

    private int playerHP;
    public int maxHP;
    private int enemyHP;
    public int enemyMaxHP;
    private bool playerTurn;
    private bool defend = false;

    private bool special = false;
    public Canvas canvas;
    public Animator enemyAnimator;
    public GameObject all;
    public GameObject sprite;
    public AudioClip boostSound;
    private AudioSource audioSource;
    private bool load; 

    private void OnEnable()
    {
        playerTurn = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = boostSound;
        audioSource.Play();

        playerHP = maxHP;
        enemyHP = enemyMaxHP;

        StartCoroutine(UpdateHP(true, 0));

        runButton.onClick.AddListener(runButtonClicked);        
        playerTurn = Random.value > 0.5f; // 50% chance for player to start
        StartCoroutine(Text());
    }

    IEnumerator Text()
    {
        startText.text = "50/50 who starts";
        yield return new WaitForSeconds(2);
        startText.text = playerTurn ? "Player goes first!" : "Enemy goes first!";
        yield return new WaitForSeconds(2);
        panel.SetActive(false);
        attackButton.onClick.AddListener(AttackButtonClicked);
        defendButton.onClick.AddListener(DefendButtonClicked);
        healButton.onClick.AddListener(HealButtonClicked);
        StartCoroutine(NextTurn());
    }

    void Update()
    {
        //StartCoroutine(UpdateHP(true, 0);
    }

    void AttackButtonClicked()
    {
        if (playerTurn)
        {
            playerTurn = !playerTurn;
            StartCoroutine(attack());
        }
    }

    IEnumerator attack()
    {
        if (Random.value <= 0.7f) // 50% chance to hit with Firethrow
        {
            StartCoroutine(UpdateHP(false, -5));
            enemyAnimator.SetTrigger("got hit");
            yield return new WaitForSeconds(enemyAnimator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            panel.SetActive(true);
            startText.text = "Haha you missed!";
            yield return new WaitForSeconds(2);
            panel.SetActive(false);
        }
        Debug.Log("Enemy turn");
        StartCoroutine(NextTurn());
    }

    void DefendButtonClicked()
    {
        if (playerTurn)
        {
            defend = true;
            Debug.Log("Player used Defend.");
            playerTurn = !playerTurn;
            Debug.Log("Enemy turn");
            StartCoroutine(NextTurn());
        }
    }

    void runButtonClicked()
    {
        canvas.gameObject.SetActive(false);
        all.SetActive(true);
        sprite.SetActive(true);
    }

    void HealButtonClicked()
    {
        if (playerTurn)
        {
            StartCoroutine(UpdateHP(true, 3));
            Debug.Log("Player used Heal.");
            playerTurn = !playerTurn;
            Debug.Log("Enemy turn");
            StartCoroutine(NextTurn());
        }
    }

    private IEnumerator NextTurn()
    {

        StartCoroutine(UpdateHP(true, 0));

        //playerTurn = !playerTurn; // Switch turn to the other player

        if (!playerTurn)
        {
            load = Random.value >= 0.75f;
            // Enemy's turn
            if (special == false  && load == false)
            {
                if (Random.value <= 0.85f && defend == false) // 85% chance to hit 
                {
                    enemyAnimator.SetTrigger("attack");
                    yield return new WaitForSeconds(enemyAnimator.GetCurrentAnimatorStateInfo(0).length - 1);
                    StartCoroutine(UpdateHP(true,-5));
                    Debug.Log("Enemy dealt 5 damage to the player.");
                }
                else if(defend == false)
                {
                    enemyAnimator.SetTrigger("attack");
                    panel.SetActive(true);
                    startText.text = "Enemy attacked but missed!";
                    yield return new WaitForSeconds(2);
                    panel.SetActive(false);
                }
                else
                {
                    enemyAnimator.SetTrigger("attack");
                    panel.SetActive(true);
                    startText.text = "Blocked!";
                    yield return new WaitForSeconds(2);
                    panel.SetActive(false);
                }
            } 
            else if (special == true && defend == false && load == false)
            {
                special = false;
                enemyAnimator.SetTrigger("special");
                yield return new WaitForSeconds(enemyAnimator.GetCurrentAnimatorStateInfo(0).length -1);
                StartCoroutine(UpdateHP(true, -10));
            }
            else if (special == true && defend == true && load == false)
            {
                enemyAnimator.SetTrigger("special");
                yield return new WaitForSeconds(enemyAnimator.GetCurrentAnimatorStateInfo(0).length);
                special = false;
                panel.SetActive(true);
                startText.text = "attack blocked";
                yield return new WaitForSeconds(2);
                panel.SetActive(false);
            }
            else 
            {
                // Load special attack
                special = true;
                panel.SetActive(true);
                startText.text = "Loading special attack";
                yield return new WaitForSeconds(2);
                panel.SetActive(false);
            }
            Debug.Log("Playerturn");
            defend = false;
            playerTurn = !playerTurn; // Switch turn to the other player
        }
    }

    IEnumerator UpdateHP(bool our,int amount)
    {
        Debug.Log(amount);
        
        if (our)
        {
            playerHP += amount;
        }else
        {
            enemyHP += amount;
        }

        playerHP = Mathf.Min(playerHP, maxHP);
        UpdateHPUI();
        if (playerHP <= 0)
        {
            Debug.Log("You died");
            panel.SetActive(true);
            startText.text = "I won. Come back next time";
            yield return new WaitForSeconds(3);
            panel.SetActive(false);

            canvas.gameObject.SetActive(false);
            all.SetActive(true);
            sprite.SetActive(true);
        }

        enemyHP = Mathf.Min(enemyHP, enemyMaxHP);
        if (enemyHP <= 0)
        {
            Debug.Log("Bat died");
            panel.SetActive(true);
            startText.text = "You won the fight";
            yield return new WaitForSeconds(3);
            panel.SetActive(false);

            canvas.gameObject.SetActive(false);
            all.SetActive(true);
            sprite.SetActive(true);
        }
        
    }

    private void UpdateHPUI()
    {
        Debug.Log(playerHP);

        // Update the fuel text based on remaining fuel percentage
        if (playerHP > 0f)
        {
            hpText.text = playerHP.ToString() + "HP";
            float fillAmount = (float)playerHP / maxHP;
            Vector3 newScale = new Vector3(fillAmount, 1f, 0f);
            hpBarImage.rectTransform.localScale = newScale;
        }
        else
        {
            Vector3 newScale = new Vector3(0f, 1f, 1f);
            hpBarImage.rectTransform.localScale = newScale;
            hpText.text = "DEAD";
        }
    }
}

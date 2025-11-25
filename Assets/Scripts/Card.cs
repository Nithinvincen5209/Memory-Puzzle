using UnityEngine;

public class Card : MonoBehaviour
{
   
    private int cardID;
    private GameManager gameManager;

 
    private bool isFlipped = false;
    private bool isMatched = false;

   
    [SerializeField] private GameObject cardBack;
    [SerializeField] private GameObject cardFront;
    private Animator animator;
    private AudioSource AS;

    public AudioClip flipSfx;
    public AudioClip mismatchSfx;

    private void Awake()
    {
        animator =  GetComponent<Animator>();
        AS = GetComponent<AudioSource>();


    }
    public void SetUp(int id, Sprite frontSprite, GameManager manager)
    {
        cardID = id;
        
        cardFront.GetComponent<SpriteRenderer>().sprite = frontSprite;
        gameManager = manager;
        
    }

    private void OnMouseDown()
    {
        if (!isFlipped && !isMatched && gameManager.CanFlip())
        {
            Flip();
            gameManager.CardFlipped(this);
        }
    }

    public void Flip()
    {
        isFlipped = true;
        animator.SetTrigger("Flip");
        AS.PlayOneShot(flipSfx);


    }
    public void Shake()
    {
        isFlipped = false;
        animator.SetTrigger("Shake");
        AS.PlayOneShot(mismatchSfx);
    }

    public void SetMatched()
    {
        isMatched = true;
    }

    public int GetCardID()
    {
        return cardID;
    }
}
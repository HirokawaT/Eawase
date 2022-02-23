using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<GameObject> openedCards = new List<GameObject>();  // 表向きになったカードのリスト
    public List<GameObject> reverseCards = new List<GameObject>();  // 今めくっているカード
    public List<int> cardTag;  // カードのタグ

    [SerializeField] Text playerScoreText, enemyScoreText;  // 自分と相手のスコアの表示
    [SerializeField] Text playerTurnText, enemyTurnText;  // あなたのターン、相手のターン
    public Text gameText;  // ゲーム終了時のテキスト

    // スクリプト
    private CardController cardController;
    [SerializeField] ButtonController buttonController;
    [SerializeField] EnemyController enemyController;

    // 音
    [SerializeField] List<AudioClip> clip = new List<AudioClip>();
    [SerializeField] AudioSource audioSource1;  // 効果音
    [SerializeField] AudioSource audioSource2;  // BGM

    public bool isIndexTranse;  // ターン遷移の管理
    public bool isActionPlayer;  // 自分の行動管理

    private int playerScore, enemyScore;  // 自分と相手のスコア
    public int index = -1;  // ターン管理


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        buttonController.playButton.gameObject.SetActive(true);
        buttonController.quitButton.gameObject.SetActive(true);
    }

    void Update() 
    {
        switch(index) 
        {
            // ゲーム準備
            case 0:
                if (isIndexTranse)
                {
                    // 値の初期化
                    cardTag = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };  // カードのタグ

                    // カードをシャッフル
                    for (int i = 0; i < cardTag.Count; i++)
                    {
                        int tmp = cardTag[i];
                        int rand = Random.Range(0, cardTag.Count);
                        cardTag[i] = cardTag[rand];
                        cardTag[rand] = tmp;
                    }

                    // カードを並べる
                    int num = 0;
                    for (int z = 0; z < 4; z++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            GameObject card = (GameObject)Resources.Load(cardTag[num].ToString());
                            Instantiate(card, new Vector3(x * 10 + 2, 0f, z * 10 + 2), Quaternion.identity);

                            num++;
                        }
                    }

                    playerScore = 0;
                    enemyScore = 0;
                    isActionPlayer = false;

                    // 自分と相手のスコアの表示
                    playerScoreText.text = "Player:" + playerScore;
                    playerScoreText.gameObject.SetActive(true);
                    enemyScoreText.text = "Computer:" + enemyScore;
                    enemyScoreText.gameObject.SetActive(true);

                    gameText.gameObject.SetActive(false);

                    isIndexTranse = false;
                }

                IndexTranse();
            break;

            // 自分ターン開始時
            case 1:
                if(isIndexTranse)
                {
                    StartCoroutine(SetTurnText(playerTurnText));

                    isIndexTranse = false;
                }
            break;

            // 自分のターン
            case 2:
                if(isIndexTranse)
                {
                    isActionPlayer = true;  // プレイヤーがカードをめくれるようにする

                    isIndexTranse = false;
                }
            break;

            // 相手ターン開始時
            case 3:
                if(isIndexTranse)
                {
                    StartCoroutine(SetTurnText(enemyTurnText));

                    isIndexTranse = false;
                }
            break;

            // 相手ターン開始時
            case 4:
                if(isIndexTranse)
                {
                    enemyController.EnemyAction();

                    isIndexTranse = false;
                }
            break;

            // 1に戻る
            case 5:
                index = 1;
            break;

            // ゲーム終了
            case 6:
                if(isIndexTranse)
                {
                    Result();

                    isIndexTranse = false;
                }
            break;
        }
    }

    // indexを遷移する
    public void IndexTranse()
    {
        index++;
        isIndexTranse = true;
    }

    // カードを表向きにする
    public void ReverseCard(GameObject card)
    {
        reverseCards.Add(card);  

        cardController = card.GetComponent<CardController>();
        cardController.cardAnim.SetBool("Reverse", true);  // カードを表向きにする

        audioSource1.PlayOneShot(clip[0]);  // 効果音

        // すでに一度表向きになっていたカードならopenedCardListに追加しないようにする
        for (int i = 0; i < openedCards.Count; i++)
        {
            if (card.name == openedCards[i].name)
            {
                return;
            }
        }

        openedCards.Add(card);  
    }

    // カードを裏向きに戻す
    public void ResetCard(GameObject card)
    {
        cardController = card.GetComponent<CardController>();
        cardController.cardAnim.SetBool("Reset", true);  // カードを裏向きにする

        audioSource1.PlayOneShot(clip[1]);  // 効果音
    }

    // 表向きにした2枚のカードを確認する
    public void CheckCard()
    {
        // めくったカードが同じ数字のカードなら
        if (reverseCards[0].tag == reverseCards[1].tag)
        {
            // openedCardsから揃ったカードを消す
            openedCards.Remove(reverseCards[0]);
            openedCards.Remove(reverseCards[1]);

            // 揃ったカードのcardTagを消す
            string str1 = reverseCards[0].name.Substring(0, reverseCards[0].name.Length - 7);
            string str2 = reverseCards[1].name.Substring(0, reverseCards[1].name.Length - 7);
            cardTag.Remove(int.Parse(str1));
            cardTag.Remove(int.Parse(str2));

            // カードを消す
            Destroy(reverseCards[0]);
            Destroy(reverseCards[1]);

            if (index == 2)  // 自分のターンなら
            {
                // スコア加算
                playerScore++;
                playerScoreText.text = "Player:" + playerScore;

                if (cardTag.Count != 0)   // 最後のペアではなければ
                {
                    audioSource1.PlayOneShot(clip[2]);  // 効果音
                }
            }
            else if (index == 4)  // 相手のターンなら
            {
                // スコア加算
                enemyScore++;
                enemyScoreText.text = "Computer:" + enemyScore;

                if (cardTag.Count != 0)  // 最後のペアではなければ
                {
                    audioSource1.PlayOneShot(clip[3]);  // 効果音

                    enemyController.EnemyAction();  // もう一度行動
                }
            }
            // カードがなくなったら
            if (cardTag.Count == 0)
            {
                isIndexTranse = true;
                index = 6;
            }
        }
        // めくったカードが違う数字のカードなら
        else
        {
            isActionPlayer = false;

            // カードを裏向きに戻す
            ResetCard(reverseCards[0]);
            ResetCard(reverseCards[1]);

            Invoke("IndexTranse", 0.8f);  // index遷移
        }

        reverseCards.Clear();
    }

    // あなたのターン、相手のターンを表示させる
    private IEnumerator SetTurnText(Text text)
    {
        text.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        text.gameObject.SetActive(false);

        IndexTranse();
    }

    private void Result()
    {
        if (playerScore > enemyScore)
        {
            audioSource1.PlayOneShot(clip[4]);  // 効果音

            gameText.text = "You Win!";
            gameText.color = Color.blue;
        }
        else if (playerScore == enemyScore)
        {
            audioSource1.PlayOneShot(clip[5]);  // 効果音

            gameText.text = "Draw";
            gameText.color = Color.green;
        }
        else if (playerScore < enemyScore)
        {
            audioSource1.PlayOneShot(clip[6]);  // 効果音

            gameText.text = "You Lose";
            gameText.color = Color.red;
        }

        buttonController.playButton.GetComponentInChildren<Text>().text = "◆ Replay";

        // ゲーム終了画面を表示
        gameText.gameObject.SetActive(true);
        buttonController.playButton.gameObject.SetActive(true);
        buttonController.quitButton.gameObject.SetActive(true);
    }
}
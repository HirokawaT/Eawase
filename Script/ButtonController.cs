using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public Button playButton, quitButton;  // タイトルのボタン
    [SerializeField] Button easy, normal, hard;  // 難易度のボタン

    [SerializeField] AudioClip playClip;  // 効果音
    [SerializeField] AudioSource audioSource;

    [SerializeField] EnemyController enemy;


    // Playが押されたときの処理 
    public void ClickPlayButton()
    {
        // ボタンを隠す
        playButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

        // 難易度設定画面
        easy.gameObject.SetActive(true);
        normal.gameObject.SetActive(true);
        hard.gameObject.SetActive(true);

        audioSource.PlayOneShot(playClip);
    }

    // Quitが押されたときの処理
    public void ClickQuitButton()
    {
        Application.Quit();
    }

    // Easy
    public void ClickEasyButton()
    {
        enemy.enemyLevel = 1;
        GameStart();
    }

    // Normal
    public void ClickNormalButton()
    {
        enemy.enemyLevel = 2;
        GameStart();
    }

    // Hard
    public void ClickHardButton()
    {
        enemy.enemyLevel = 3;
        GameStart();
    }

    public void GameStart()
    {
        // 難易度設定画面
        easy.gameObject.SetActive(false);
        normal.gameObject.SetActive(false);
        hard.gameObject.SetActive(false);

        // index遷移
        GameManager.instance.index = 0;
        GameManager.instance.isIndexTranse = true;

        audioSource.PlayOneShot(playClip);
    }
}

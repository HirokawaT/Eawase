using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int enemyLevel;


    public void EnemyAction()
    {
        // すでに表向きになったカードの中でペアのカードがあるかどうかを調べる
        for(int i = 0; i < GameManager.instance.openedCards.Count - 1; i++)
        {
            for(int k = i + 1; k < GameManager.instance.openedCards.Count; k++)
            {
                // すでに表向きになったカードの中でペアのカードがあるなら
                if (GameManager.instance.openedCards[i].tag == GameManager.instance.openedCards[k].tag)
                {
                    if(EnemyLevel(enemyLevel))
                    {
                        StartCoroutine(EnemyReverseCard(GameManager.instance.openedCards[i], GameManager.instance.openedCards[k]));  // カードをめくる

                        return;
                    }
                }
            }
        }

        // 上記以外
        int num1 = GameManager.instance.cardTag[Random.Range(0, GameManager.instance.cardTag.Count)];
        GameObject card1 = GameObject.Find(num1 + "(Clone)");

        // 1枚目をめくったとき、ペアのカードがすでに表向きになっているかどうかを調べる
        for(int i = 0; i < GameManager.instance.openedCards.Count; i++)
        {
            if(card1.tag == GameManager.instance.openedCards[i].tag && card1.name != GameManager.instance.openedCards[i].name)
            {
                if(EnemyLevel(enemyLevel))
                {
                    StartCoroutine(EnemyReverseCard(card1, GameManager.instance.openedCards[i]));  // カードをめくる

                    return;
                }
            }
        }

        // 上記以外
        int num2 = num1;
        while(num1 == num2)
        {
            num2 = GameManager.instance.cardTag[Random.Range(0, GameManager.instance.cardTag.Count)];
        }
        GameObject card2 = GameObject.Find(num2 + "(Clone)");

        StartCoroutine(EnemyReverseCard(card1, card2));  // カードをめくる
    }

    // カードをめくる
    private IEnumerator EnemyReverseCard(GameObject card1, GameObject card2)
    {
        yield return new WaitForSeconds(0.7f);

        GameManager.instance.ReverseCard(card1);

        yield return new WaitForSeconds(1.5f);

        GameManager.instance.ReverseCard(card2);
    }

    private bool EnemyLevel(int level)
    {
        if(level == 1)
        {
            return Probability(10);
        }
        else if(level == 2)
        {
            return Probability(40);
        }
        else 
        {
            return Probability(70);
        }
    }

    // 確率
    private bool Probability(float fPercent)
    {
        float fProbabilityRate = Random.value * 100.0f;

        if (fPercent == 100.0f && fProbabilityRate == fPercent)
        {
            return true;
        }
        else if (fProbabilityRate < fPercent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

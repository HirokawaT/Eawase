using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CardController : MonoBehaviour, IPointerClickHandler
{
    public Animator cardAnim;  // カードのアニメーション

    bool isOpen;  // 表を向いているかどうか


    // カードを表向きにするアニメーションの終わりに呼び出される
    public void StopReverseAnim()
    {
        cardAnim.SetBool("Reverse", false);

        // 2枚カードが表向きになっているなら
        if (GameManager.instance.reverseCards.Count == 2)
        {
            StartCoroutine(CheckCoroutine());
        }
    }

    private IEnumerator CheckCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        GameManager.instance.CheckCard();
    }

    // カードを裏向きにするアニメーションの終わりに呼び出される
    public void StopResetAnim()
    {
        cardAnim.SetBool("Reset", false);
        isOpen = false;
    }

    // カードをクリックしたときの処理
    public void OnPointerClick(PointerEventData eventData)
    {
        if(GameManager.instance.index == 2 && GameManager.instance.isActionPlayer && !isOpen)  // 自分のターン中かつカードが裏向きなら
        {
            if(GameManager.instance.reverseCards.Count < 2)  // 2枚以上表向きにはできないように
            {
                GameManager.instance.ReverseCard(gameObject);
                isOpen = true;
            }
        }
    }
}

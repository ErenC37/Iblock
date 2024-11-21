using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CountDown : MonoBehaviour
{
   [SerializeField] private Image _timerImg;

   [SerializeField] private TMP_Text _timerText;

   [SerializeField] static public float _currentTime;

   [SerializeField]  private float _duration;

   IEnumerator co;


   
   void Start()
   {
    _currentTime = _duration;
    _timerText.text = _currentTime.ToString();
        co = UpdateTime();
    StartCoroutine(co);
   }

   public void coUpdate()
    {
        StopCoroutine(co);   
    }

    void Update()
    {
        if(_currentTime == 0)
        {
           GameEvents.GameOver(false);
        }
        if (_currentTime == 5)
        {
            AudioManager.instance.Play("Saat");
        }

    }

   
   private IEnumerator UpdateTime()
   {
    while(_currentTime >= 0)
    {
        _timerImg.fillAmount = Mathf.InverseLerp(0, _duration, _currentTime);
        _timerText.text = _currentTime.ToString();
        yield return new WaitForSeconds(1f);
        _currentTime--;
    }
    yield return null;
   }
}

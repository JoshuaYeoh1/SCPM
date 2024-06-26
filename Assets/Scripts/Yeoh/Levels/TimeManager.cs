using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Current;

    void Awake()
    {
        Current=this;
    }
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public int hour=10;
    public string meridiem="pm";
    public Vector2 realHourInterval = new Vector2(45, 60);

    void OnEnable()
    {
        EventManager.Current.ClockInEvent += OnClockIn;
    }
    void OnDisable()
    {
        EventManager.Current.ClockInEvent -= OnClockIn;
    }
    
    void OnClockIn()
    {
        StartCoroutine(TimeRunning());
    }

    IEnumerator TimeRunning()
    {
        while(true)
        {
            EventManager.Current.OnHourTick(hour);

            yield return new WaitForSeconds(Random.Range(realHourInterval.x, realHourInterval.y));

            hour++;

            if(hour>=13)
            {
                hour=1;

                meridiem = meridiem=="pm" ? "am" : "pm";
            }
        }
    }
}

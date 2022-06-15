using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Day_Night_Cycle : MonoBehaviour
{
    [SerializeField]
    int multiplier;
    
    int hour = 0;
    int frameCounter = 0;
    int secondsPerGameHour = 45;
    int fixedUpdatePerSecond = 50;
    int minuteCounter = 37;
    int hourCounter = 45 * 50;
    int secondCounter = 50;

    [SerializeField]
    int Hour;
    [SerializeField]
    bool PM;

    [SerializeField]
    bool freezeTime;
    

    int clockInSeconds;

    int _12am = 0;
    int _1am = 45;
    int _2am = 90;
    int _3am = 135;
    int _4am = 180;
    int _5am = 225;
    int _6am = 270;
    int _7am= 315;
    int _8am = 360;
    int _9am = 405;
    int _10am = 450;
    int _11am = 495;
    int _12pm = 540;
    int _1pm = 585;
    int _2pm = 630;
    int _3pm = 675;
    int _4pm = 720;
    int _5pm = 765;
    int _6pm = 810;
    int _7pm = 855;
    int _8pm = 900;
    int _9pm = 945;
    int _10pm = 990;
    int _11pm = 1035;


    float R;
    float G;
    float B;

    [SerializeField]
    Light2D daylight;
    Color color;
    float intensity;

    private void OnEnable()
    {
        

    }

    // Start is called before the first frame update
    void Start()
    {
        color = daylight.color;
        intensity = daylight.intensity;

        clockInSeconds = setTime();


        updateLight();
    }

    private void FixedUpdate()
    {
        if (freezeTime) return;
        
        frameCounter++;
        if (frameCounter >= 50)
        {
            clockInSeconds+= multiplier;
            frameCounter = 0;
            updateLight();

            if (clockInSeconds >= 1080)
                clockInSeconds = 0;
        }

            
    }

    void updateLight()
    {
        //5am to 7pm Day
        if (clockInSeconds > _5am && clockInSeconds < _7pm)
        {
            R = G = B = 0.84f;
        }

        //7pm to 10pm Sunset
        else if(clockInSeconds >= _7pm && clockInSeconds < _10pm)
        {
            R = 0.84f - ((clockInSeconds % _7pm / (float)(_10pm - _7pm)) * 0.64f);
            G = 0.84f - ((clockInSeconds % _7pm / (float)(_10pm - _7pm)) * 0.64f);
            B = 0.84f - ((clockInSeconds % _7pm / (float)(_10pm - _7pm)) * 0.47f);
        }

        else if(clockInSeconds >= _10pm || clockInSeconds < _4am)
        {
            R = G = 0.2f;
            B = 0.37f;
        }

        else
        {
            R = 0.20f + ((clockInSeconds % _4am / (float)(_7am - _4am)) * 0.64f);
            G = 0.20f + ((clockInSeconds % _4am / (float)(_7am - _4am)) * 0.64f);
            B = 0.37f + ((clockInSeconds % _4am / (float)(_7am - _4am)) * 0.64f);
        }



        //R -= (1.5f/255f);
        //G -= (1.5f/255f);
        //B -= (0.39f/255f);

        Color newColor = new Color(R, G, B);

        daylight.color = newColor;


    }


    public string getTimeString()
    {
        int hour = clockInSeconds / 45;
        int minute = (int)(((clockInSeconds % 45f) / 45f) * 60f);

        bool pm = hour >= 12;

        string minuteString = "";
        if (minute < 10)
            minuteString += "0";
        minuteString += minute.ToString();

        if(pm)
        {
            if(hour == 12)
            {
                return hour.ToString() + ":" + minuteString + "PM";
            }
            return (hour - 12).ToString() + ":" + minuteString + "PM";
        }
        if (hour == 0)
            return "12" + ":" + minuteString + "AM";
        return hour.ToString() + ":" + minuteString + "AM";

    }

    /// <summary>
    /// //TODO this function not working yet
    /// </summary>
    /// <returns></returns>
    int setTime()
    {
        if (Hour < 0 || Hour > 12) return 0;

        if (PM)
        {
            if (Hour != 12)
                Hour += 12;
            else
                Hour = 0;
        }

        return Hour * secondsPerGameHour;
    }

    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavimodiere_Lab_6
{
    /*
    // Notes:
        * timeIsUp is used to execute when timer is up once
        * timeIsUpCounter is used to execute every frame after the timer has completed
        * if the if statement checks for a specific number, it will be executed until the timer is reset
        * if the if statement checks for any number above a range, it will be executed always after the specified number of resets
        * Create var as: static Timer VarName = new Timer(delayInSec);
        * Call update function in update
        * TimeLeft is negative when no time is left
    */

    public class Timer
    {
        

        //vars
        private bool enabled;
        public bool timeIsUp = false;
        private float delay;
        private float currentTimeInSec = 0.0f;
        public float timeLeft;

        public int timerResetCounter;//display

        //constructors
        public Timer(float delayInSec)//set timer at beginning
        {
            SetTime(delayInSec);
        }

        public Timer()//make var
        {

        }

        //sets+gets
            //timeIsUp
            //timeLeft
            //timeIsUpCounter

        //methods
        public void SetTime(float delayInSec)//set delay in seconds
        {
            if (enabled == false && delayInSec > 0)//if old timer is complete and input is above 0
            {
                delay = delayInSec + currentTimeInSec;
                timeIsUp = false;
                enabled = true;
                Console.WriteLine("Timer Set to " + delayInSec + " and is now running!");
            }
            else if (enabled == true)
            {
                throw new InvalidOperationException("Unable to set new delay, run this function only after the timer is disabled or reset");
            }
        }

        public void AddTime(float delayInSecToAdd)//add to current timer (when enabled)
        {
            if (enabled == true)//if timer is running
            {
                delay += delayInSecToAdd;
                timeIsUp = false;
                enabled = true;
                timeLeft = delay - currentTimeInSec;//recalculate time left to display accurately
                Console.WriteLine("Time Added:" + delayInSecToAdd);
            }
            else if (enabled == false)
            {
                throw new InvalidOperationException("Unable to add to delay, run this function after the timer is enabled");
            }
        }

        public void Reset()
        {
            Console.WriteLine("Timer Reset!");
            enabled = false;
            timeIsUp = false;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTimer)//call in update
        {
            if (enabled == true)
            {
                currentTimeInSec += (float)gameTimer.ElapsedGameTime.TotalSeconds;

                timeLeft = delay - currentTimeInSec;

                if (timeLeft <= 0)
                {
                    timeIsUp = true;
                    timerResetCounter++;
                    enabled = false;
                }
            }
        }
    }
}

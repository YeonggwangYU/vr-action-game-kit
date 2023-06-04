using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _3DGamekitLite.Scripts.Runtime.Game.Player
{
    public class XROriginController : MonoBehaviour
    {
        /// <summary>
        /// play footstep sound.
        /// </summary>
        [SerializeField] private AudioSource audioSource;
        
        /// <summary>
        /// sound played when XROrigin move.
        /// </summary>
        [SerializeField] private AudioClip walkSound;
        
        /// <summary>
        /// Interval of footstep sound.
        /// </summary>
        [SerializeField] private float intervalPlaySound;
        
        /// <summary>
        /// threshold of play walk sound.
        /// </summary>
        [SerializeField] private float thresholdSpeedWalkSound;
        
        /// <summary>
        /// interval to prevents continuous footsteps.
        /// </summary>
        private DateTime _lastStepTime;

        /// <Summary>
        /// posititon before 1 frame.
        /// </Summary>
        private Vector3 _prevPosition;

        private void Start()
        {
            _lastStepTime = DateTime.Now;
        }

        private void Update()
        {
            if ((DateTime.Now - _lastStepTime).TotalSeconds < intervalPlaySound) {
                // do nothing to prevents continuous footsteps.
            }
            else
            {
                //do nothing when deltatime is 0.
                if (Mathf.Approximately(Time.deltaTime, 0))
                {
                    //do nothing
                }
                else
                {
                    // get current position.
                    Vector3 position = transform.position;

                    // calcurate current speed.
                    Vector3 velocity = (position - _prevPosition) / Time.deltaTime;

                    // dont play sound while move speed is slow.
                    if (velocity.magnitude > thresholdSpeedWalkSound)
                    {
                        // play wolk sound.
                        audioSource.PlayOneShot(walkSound);
                    }
                    
                    //update previous frame position.
                    _prevPosition = position;
                    
                    //set present time.
                    _lastStepTime = DateTime.Now;
                }

            }
            
        }
    }
}
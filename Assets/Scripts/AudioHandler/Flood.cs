using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JacDev.Audio
{
    public class Flood : AudioHandler
    {
        [Header("Audio")]
        public AudioClip specWaterMeter200;
        public AudioClip specWaterMeter350;
        public AudioClip specWaterMeter500;
        public AudioClip plantDown;
        public AudioClip instalGateSide;
        public AudioClip instalGateMid;
        public AudioClip getRescueKit;
        public AudioClip turnOffSwitch;
        public AudioClip turnOffGas;
        public AudioClip waterIn;
        public AudioClip escape;
        public AudioClip stageClear;

        [Header("Additional Sound")]
        public AudioClip broadcast1;
        public AudioClip broadcast2;
        public AudioClip boilWater;
        public AudioClip switchSound;
    }
}
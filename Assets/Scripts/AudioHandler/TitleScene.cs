using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JacDev.Audio
{
    public class TitleScene : AudioHandler
    {
        [Header("Audios")]
        public AudioClip welcome;
        public AudioClip controllerFrontIntro1;
        public AudioClip controllerFrontIntro2;
        public AudioClip contorllerRearIntro1;
        public AudioClip contorllerRearIntro2;

        public AudioClip numberKeyin;
        public AudioClip tableIntro;
        public AudioClip hammerIntro;
        public AudioClip hammerComplete;

        public AudioClip takeBomb;
        public AudioClip bombIntro;
        public AudioClip chairIntro;

        public AudioClip great;
        public AudioClip goSelectMission;

        public AudioClip earthquakeIntro;
        public AudioClip fireTruckIntro;
        public AudioClip floodIntro;

        [Header("Additional Sounds")]
        public AudioClip rexGetHit;
        public AudioClip rexSprayFire;
        public AudioClip explosive;
        public AudioClip fall;
        public AudioClip fuse;

        public AudioClip bgm;
    }
}


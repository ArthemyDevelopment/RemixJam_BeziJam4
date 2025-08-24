using System.Collections;
using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using UnityEngine;

public class SFXManager : SingletonManager<SFXManager>
{

    [BoxGroup("References"), SerializeField] private AudioSource SfxSource;
    [BoxGroup("References"), SerializeField] private AudioClip LockSound;
    [BoxGroup("References"), SerializeField] private AudioClip RotateSound;
    [BoxGroup("References"), SerializeField] private AudioClip HoldSound;
    [BoxGroup("References"), SerializeField] private AudioClip LineSound;
    [BoxGroup("References"), SerializeField] private AudioClip MoveSound;

    [BoxGroup("Sfx Settings"), SerializeField] private float RepeatClearedLinesDelay;

    [FoldoutGroup("DEBUG"), SerializeField, ReadOnly]private bool canPlaySFX = true;


    public void TriggerLockSound()
    {
        if (!canPlaySFX) return;
        SfxSource.clip = LockSound;
        SfxSource.Play();
    }

    public void TriggerRotateSound()
    {
        if (!canPlaySFX) return;
        SfxSource.clip = RotateSound;
        SfxSource.Play();
    }

    public void TriggerHoldSound()
    {
        if (!canPlaySFX) return;
        SfxSource.clip = HoldSound;
        SfxSource.Play();
    }
    public void TriggerMoveSound()
    {
        if (!canPlaySFX) return;
        SfxSource.clip = MoveSound;
        SfxSource.Play();
    }

    public void TriggerLineSound(int clearedLines)
    {
        SfxSource.clip = LineSound;
        StartCoroutine(TriggerLineCoroutine(clearedLines));
    }


    IEnumerator TriggerLineCoroutine(int clearedLines)
    {
        canPlaySFX = false;
        for (int i = 0; i < clearedLines; i++)
        {
            SfxSource.Play();
            yield return ScriptsTools.GetWait(RepeatClearedLinesDelay);
        }

        yield return ScriptsTools.GetWait(SfxSource.clip.length);
        canPlaySFX = true;
    }
}

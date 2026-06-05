using UnityEngine;
using FMODUnity;

public class AnimationSoundTrigger : MonoBehaviour
{
    [SerializeField] EventReference reference;
    public void PlayAnimationSound(string path)
    {
        if (path != null)
        {
            AudioManager.Instance.PlayOneShot(reference, this.transform.position);
        }
    }
}

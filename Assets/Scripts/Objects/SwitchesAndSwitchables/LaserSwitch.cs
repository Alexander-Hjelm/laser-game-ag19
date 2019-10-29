using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This Laser Switch is only activated as long as a laser is hitting it.
 */
public class LaserSwitch : Switch
{
    private int shutDownCount;
    private int startUpCount;

    [SerializeField]
    private int startUpTime;
    [SerializeField]
    private int shutDownTime;

    private Image _loaderImage;
    private AudioSource _audioSource;
    private bool _playAudio;
    
    private void Awake()
    {
        _loaderImage = GetComponentInChildren<Image>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        shutDownCount = 0;
        startUpCount = 0;
        _playAudio = false;
    }

    // Update is called once per frame
    void Update()
    {
        shutDownCount++;
        if (shutDownCount >= shutDownTime) {// Turn switchable off if laser isn't hitting the switch after 2 updates.
            switchable.SwitchTo(false);
            startUpCount = 0;
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
            _playAudio = false;
        }
        if (startUpCount >= startUpTime) {
            switchable.SwitchTo(true);
        }

        // Only uses startup time for now. Maybe add shutdown time later?
        _loaderImage.fillAmount = (float)startUpCount / (float)startUpTime;
    }

    public override void ActivateSwitch() {
        if (!_playAudio)
        {
            _audioSource.PlayOneShot(_audioSource.clip);
            _playAudio = true;
        }
        startUpCount++;
        shutDownCount = 0;
    }
}

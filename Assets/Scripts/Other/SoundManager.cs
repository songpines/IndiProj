using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }


    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioSource SFX;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        BGM.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayResourceLackSound()
    {
        SFX.Play();
    }
}

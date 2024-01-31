using UnityEngine;

public class DrawerHelper : MonoBehaviour
{
	[SerializeField] Animator _anim;
	[SerializeField] CameraState _state;
	[Header("Opcional")]
	[SerializeField] AudioSource _audio;
	[SerializeField] AudioClip[] clips;
	bool opened = false;

	public string[] triggerNames = {"TriggerA", "TriggerB"};

	public int drawerState = 1;

	public void TWO()
	{
        int state = _state.GetState();
		if ((state != drawerState) && opened) 
		{
            _anim.SetTrigger(triggerNames[1]);
			opened = false;
		}
	}

	public void ONE(int nextState)
	{
		if(nextState == drawerState && !opened)
		{
            _anim.SetTrigger(triggerNames[0]);
			opened = true;
		}
	}

	public void Play(int clip)
    {
		if (_audio == null) return;
		if (clip >= clips.Length || clip < 0) return;
		_audio.PlayOneShot(clips[clip]);
    }

	private void OnEnable()
	{
		_state.onStarting.AddListener(ONE);
		_state.onFinished.AddListener(TWO);
	}
	private void OnDisable()
	{
		_state.onStarting.RemoveListener(ONE);
		_state.onFinished.RemoveListener(TWO);
	}
}

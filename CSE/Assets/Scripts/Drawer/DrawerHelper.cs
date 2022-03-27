using UnityEngine;

public class DrawerHelper : MonoBehaviour
{
	[SerializeField] Animator _anim;
	[SerializeField] CameraState _state;
	bool opened = false;

	public string[] triggerNames = {"TriggerA", "TriggerB"};

	public int drawerState = 1;

	public void TWO()
	{
		int state = _state.GetState();
		if ((state == drawerState + 1 || state == drawerState - 1) && opened) 
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

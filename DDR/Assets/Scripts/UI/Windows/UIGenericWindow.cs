using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIGenericWindow : MonoBehaviour {
	#region Serialized Fields
	[Header("Base Content")]
	[SerializeField] private Button _btnClose = null;
	[SerializeField] private AudioClip _acClose = null;

	[SerializeField] private CustomPlayableDirector _cpdFadeIn = null;
	[SerializeField] private CustomPlayableDirector _cpdFadeOut = null;
	#endregion

	#region Exposed Fields
	public abstract string WindowName {
		get;
	}
	#endregion

	#region Properties
	public bool IsPlaying {
		get {
			return _cpdFadeIn.IsPlaying || _cpdFadeOut.IsPlaying;
		}
	}
	#endregion

	#region Mono Behaviour Hooks
	private void Awake() {
		if (_btnClose != null) {
			_btnClose.onClick.AddListener(ButtonCloseOnClick);
		}

		OnWindowAwake();
	}

	private void OnEnable() {
		OnWindowEnable();
	}

	private void OnDisable() {
		OnWindowDisable();
	}

	private void OnDestroy() {
		if (_btnClose != null) {
			_btnClose.onClick.RemoveAllListeners();
		}

		OnWindowDestroy();
	}
	#endregion

	#region Virtual Methods
	protected virtual void OnWindowAwake() {

	}

	protected virtual void OnWindowEnable() {

	}

	protected virtual void OnWindowDisable() {

	}

	protected virtual void OnWindowDestroy() {

	}
	#endregion

	#region UI Button Handlings
	private void ButtonCloseOnClick() {
		PlayFadeOut().DoNotAwait();

		if (_acClose != null && AudioManager.Instance != null) {
			AudioManager.Instance.PlaySE(_acClose);
		}
	}
	#endregion

	#region APIs
	public async Task PlayFadeIn() {
		_cpdFadeOut.Stop();

		await _cpdFadeIn.Play();
	}

	public async Task PlayFadeOut() {
		_cpdFadeIn.Stop();

		await _cpdFadeOut.Play();
	}
	#endregion
}
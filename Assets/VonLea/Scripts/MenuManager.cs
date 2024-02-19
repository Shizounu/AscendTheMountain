using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField, Header("References")] private GameObject[] _views;

    private void Awake()
    {
        //if(_views != null)
        //{
        //    _views[0].SetActive(true);
        //}
    }

    public void LoadScene(int idx)
    {
        SceneManager.LoadScene(idx);
    }

    /// <summary>
    /// method to switch between views
    /// </summary>
    /// <param name="idx">0-Menu;1-Options;2-Credits</param>
    public void SwitchView(int idx)
    {
        if (idx >= _views.Length) return;

        for (int i = 0; i < _views.Length; i++)
        {
            _views[i].SetActive(i == idx);
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

}

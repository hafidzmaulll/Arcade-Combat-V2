using UnityEngine;

public class ExitGame : MonoBehaviour
{
    // Fungsi ini akan dipanggil saat pemain ingin keluar dari game
    public void QuitGame()
    {
        // Jika sedang di editor, berhenti dari mode play
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Jika sedang dalam build (game yang sudah di-compile), keluar dari game
        Application.Quit();
#endif
    }
}

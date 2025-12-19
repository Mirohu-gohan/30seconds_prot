using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    void Start()
    {
        // 1. ƒQ[ƒ€ŠJnA©•ª‚ğ GameManager ‚É“o˜^‚·‚é
        if (GameManager_M.Instance != null)
        {
            GameManager_M.Instance.RegisterPlayer(gameObject);
            Debug.Log(gameObject.name + " ‚ğ“o˜^‚µ‚Ü‚µ‚½");
        }
    }

    // —‰º‚µ‚½‚É GameManager ‚©‚çŒÄ‚Î‚ê‚é
    public void OnFallOut()
    {
        Debug.Log(gameObject.name + " ‚ª—‰º‚µ‚Ü‚µ‚½I");

        // 2. GameManager ‚Éu©•ª‚ª’E—‚µ‚½v‚±‚Æ‚ğ“`‚¦‚é
        if (GameManager_M.Instance != null)
        {
            GameManager_M.Instance.OnPlayerEliminated();
        }

        // 3. ©•ª‚ğÁ‹‚·‚é
        Destroy(gameObject);
    }
}
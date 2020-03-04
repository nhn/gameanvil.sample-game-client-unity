using UnityEngine;

// r공통 적으로 사용 하는 스크립트
public class ClickEvent : MonoBehaviour
{
    // 클릭 이벤트 발생시 해당 오브젝트 숨김
    public void OnClickObjectHide()
    {
        gameObject.SetActive(false);
    }
}

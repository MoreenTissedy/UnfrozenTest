using UnityEngine;

namespace UnfrozenTest
{
    public class Parallax : MonoBehaviour
    {
        [Range(0.5f, 1)]
        [SerializeField] private float moveCoef = 0.7f;
        [Range(0.8f, 1)]
        [SerializeField] private float scaleCoef = 0.95f;
        private Vector3 cameraPos;
        private float cameraScale;

        private void Start()
        {
            cameraPos = Camera.main.transform.position;
            cameraScale = Camera.main.orthographicSize;
        }

        private void Update()
        {
            Vector3 cameraNewPos = Camera.main.transform.position;
            if (cameraNewPos != cameraPos)
            {
                Vector3 cameraMove = cameraNewPos - cameraPos;
                transform.Translate(-cameraMove * (1 - moveCoef));
                cameraPos = cameraNewPos;
            }
            
            float orthographicSize = Camera.main.orthographicSize;
            if (orthographicSize != cameraScale)
            {
                float change = -(orthographicSize - cameraScale)*(1 - scaleCoef);
                var localScale = transform.localScale;
                localScale = localScale + localScale * change;
                transform.localScale = localScale;
                cameraScale = orthographicSize;
            }
        }
    }
}
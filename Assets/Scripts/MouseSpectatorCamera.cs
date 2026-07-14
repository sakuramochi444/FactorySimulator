using UnityEngine;

public class MouseSpectatorCamera : MonoBehaviour
{
    [Header("マウス感度")]
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("移動速度")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float fastMoveMultiplier = 2.5f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // 開発中に操作しやすいよう、ゲーム画面をクリックしたらマウスカーソルを中央に固定＆非表示にする
        // （Escキーを押すとカーソルが戻ります）
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 現在のカメラの回転初期値を同期
        Vector3 currentRotation = transform.localRotation.eulerAngles;
        rotationY = currentRotation.y;
        rotationX = currentRotation.x;
    }

    void Update()
    {
        LookAround();
        Move();
    }

    // 1. マウス移動による視線変更（上下左右）
    void LookAround()
    {
        // マウスの移動量を取得
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationY += mouseX;
        rotationX -= mouseY; // 反転防止のためマイナス

        // 真上や真下を向いたときに画面がひっくり返らないよう、上下の回転角を制限（-85度〜85度）
        rotationX = Mathf.Clamp(rotationX, -85f, 85f);

        // カメラの回転を適用
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    // 2. 見ている方向を基準にした移動（WASD）
    void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D
        float moveZ = Input.GetAxisRaw("Vertical");   // W/S

        // Shiftキーで加速
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= fastMoveMultiplier;
        }

        // 【ここがポイント】カメラが現在「実際に向いている方向」を基準に移動する
        Vector3 moveDirection = (transform.forward * moveZ + transform.right * moveX).normalized;

        // 空間を自由に徘徊（上昇・下降も含めて進む）
        transform.position += moveDirection * currentSpeed * Time.deltaTime;
    }
}
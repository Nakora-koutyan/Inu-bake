using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    //PlayerControl制御
    private Vector2 _move_input;        //MoveInput受け取り
    private bool _jump_pressed;         //JumpInput受け取り
    private bool _shake_pressed;        //ShakeInput受け取り

    //MainGameUIControl制御
    private bool _click_pressed;        //ClickInput受け取り

    private bool _input_lock;           //Player制御のLockを行う変数

    //移動入力処理受け付け関数(Player用)
    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log($"Jump Button 受け付け完了！ phase: {context.phase}");
        _move_input = context.ReadValue<Vector2>();
    }
    //ジャンプ入力処理受け付け関数(Player用)
    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            _jump_pressed = true;
        }
        else if(context.canceled)
        {
            _jump_pressed = false;
        }
    }
    //揺らす入力処理受け付け関数(Player用)
    public void OnShake(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _shake_pressed = true;
        }
        else if (context.canceled)
        {
            _shake_pressed = false;
        }
    }
    //クリック入力処理受け付け処理
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _click_pressed = true;
        }
        else if (context.canceled)
        {
            _click_pressed = false;
        }
    }

    //ジャンプボタンが入力された場合、ワンショット式で値を返す
    public bool JumpLicence()
    {
        if(_jump_pressed)
        {
            _jump_pressed = false;
            return true;
        }
        return false;
    }
    //揺らすボタンが揺らされた場合、ワンショット式で値を返す
    public bool ShakeLicence()
    {
        if (_shake_pressed)
        {
            _shake_pressed = false;
            return true;
        }
        return false;
    }
    //移動の値を取得
    public Vector2 MoveValue()
    {
        return _move_input;
    }

    //クリックボタンが入力された場合、ワンショット式で値を返す
    public bool ClickLicence()
    {
        if (_click_pressed)
        {
            _click_pressed = false;
            return true;
        }
        return false;
    }
    //プレイヤー操作をLockする？
    public void PlayerInputLock(bool ret)
    {
        _input_lock = ret;
    }
}
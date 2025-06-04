using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Animator playerAnimator;
    


    private PlayerMovement playerCotroller;
    private StateController playerStateController;

    private void Awake()
    {
        playerCotroller = GetComponent<PlayerMovement>();
        playerStateController = GetComponent<StateController>();
    }

    private void Update()
    {
        SetPlayerAnimation();
    }

    private void SetPlayerAnimation()
    {
        var currentState = playerStateController.GetCurrentState();

        switch (currentState)
        {
            case PlayerState.Idle:
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_IDLE, true);
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_WALKING, false);
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_RUNNING, false);
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_JUMPING, false);
                break;

            case PlayerState.Walking:
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_IDLE, false);
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_WALKING, true);
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_RUNNING, false);
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_JUMPING, false);
                break;

            case PlayerState.Running:
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_IDLE, false);
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_WALKING, false);
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_RUNNING, true);
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_JUMPING, false);
                break;

            case PlayerState.Jumping:
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_IDLE, false);
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_WALKING, false);
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_RUNNING, false);
                playerAnimator.SetBool(Consts.PlayerAnimations.IS_JUMPING, true);
                break;

            default:
                break;
        }
    }
}

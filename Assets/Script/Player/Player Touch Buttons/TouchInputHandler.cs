using UnityEngine;

public class TouchInputHandler : MonoBehaviour
{
    private PlayerMovements player;
    private PlayerCombatController combat;

    private void OnEnable()
    {
        PlayerMovements.OnPlayerSpawned += HandlePlayerSpawned;
    }

    private void OnDisable()
    {
        PlayerMovements.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    private void HandlePlayerSpawned(PlayerMovements newPlayer)
    {
        player = newPlayer;
        combat = newPlayer.GetComponent<PlayerCombatController>();
    }

    public void OnMoveLeftDown() => player?.SetUIMoveLeft(true);
    public void OnMoveLeftUp() => player?.SetUIMoveLeft(false);
    public void OnMoveRightDown() => player?.SetUIMoveRight(true);
    public void OnMoveRightUp() => player?.SetUIMoveRight(false);
    public void OnJumpButton() => player?.SetUIJump(true);
    public void OnDashButton() => player?.SetUIDash(true);
    public void OnVanishButton() => player?.TriggerVanish();
    public void OnAttackButton() => combat?.OnAttackButtonPressed();

}

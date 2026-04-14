// IState.cs
public interface IState
{
    void Enter();
    void Tick();
    void FixedTick();
    void LateTick();
    void Exit();
}
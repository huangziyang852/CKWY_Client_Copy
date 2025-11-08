namespace Game.Ingame.Controller
{
    public interface IUnitState<T> where T : UnitController
    {
        void OnEnter(T unitController);
        void OnUpdate();
        void OnExit();
    }
}

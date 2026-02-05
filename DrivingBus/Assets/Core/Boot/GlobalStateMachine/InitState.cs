using Core.Services;
using Core.Utils.StateSystem;

namespace Core.Boot.GlobalStateMachine
{
    public class InitState : BaseState
    {
        IFadeService _fadeService;

        public InitState(IFadeService fadeService)
        {
            _fadeService = fadeService;
        }
		
        public override void Enter()
        {
            _fadeService.FadeIn();
            StateMachine.Enter<MainMenuState>();
        }

        public override void Exit()
        {
			
        }
    }
}
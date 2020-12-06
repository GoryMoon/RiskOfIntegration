using R2API.Utils;
using RoR2;

namespace RiskOfIntegration.Actions
{
    public class MessageAction: BaseAction
    {
        private readonly string _message;

        public MessageAction(string message)
        {
            _message = message;
        }

        public override ActionResponse Handle()
        {
            ChatMessage.SendColored(_message, "#0984e3");
            return ActionResponse.Done;
        }
    }
}
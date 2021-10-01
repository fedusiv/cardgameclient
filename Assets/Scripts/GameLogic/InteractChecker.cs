using Cards;
namespace GameLogic
{
    public class InteractChecker
    {
        public static bool IsInteractable(CardType caller, CardType target)
        {
            var result = false;
            // Verify, that cards can interact with each other
            switch (caller)
            {
                case CardType.Scroll:
                    switch (target)
                    {
                        case CardType.Creature:
                            result = true;
                            break;
                    }
                    break;
                case CardType.Weapon:
                    switch (target)
                    {
                        case CardType.Creature:
                            result = true;
                            break;
                    }
                    break;

            }

            return result;
        }
    }
}
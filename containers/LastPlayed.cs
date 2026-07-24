using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class LastPlayed : Node {
    [Export] CardSlot[] slots;
    [Export] CardSlot discard;

    public async Task take_and_animate_card(Card card) {
        foreach(CardSlot slot in slots) {
            if(!slot.is_occupied) {
                await slot.take_and_animate_card(card);
                return;
            }
        }
        // Shift all slots by one
        Card last = slots[0].eject();
        int nslots = slots.Count();
        Task[] animates = new Task[nslots + 1];
        for(int i = 1; i < nslots; i++) {
            animates[i - 1] = slots[i - 1].take_and_animate_card(slots[i].eject());
        }
        animates[nslots - 1] = slots[nslots - 1].take_and_animate_card(card);
        animates[nslots] = discard.take_and_animate_card(last);
        await Task.WhenAll(animates);
        discard.eject().QueueFree();
    }

    public List<Card> peek() => slots.Map(slot => slot.is_occupied ? slot.peek() : null);
}

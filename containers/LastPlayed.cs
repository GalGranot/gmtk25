using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;

public partial class LastPlayed : Node {
    [Export] CardSlot[] slots;
    [Export] CardSlot discard;
    Queue<Card> q = new();
    SemaphoreSlim lck = new(1, 1);

    public async Task take_and_animate_card(Card card) {
        await lck.WaitAsync();
        q.Enqueue(card);
        lck.Release();
    }

    public void spawn_worker() {
        take_and_animate_card_inner().Forget();
    }

    public async Task take_and_animate_card_inner() {
        Card card;
        while(true) {
            await lck.WaitAsync();
            if(q.Count == 0) {
                lck.Release();
                await Time.WaitForSeconds(this, 0.001f);
                continue;
            } else {
                card = q.Dequeue();
                lck.Release();
            }
            bool finish = false;
            foreach(CardSlot slot in slots) {
                if(!slot.is_occupied) {
                    await slot.take_and_animate_card(card);
                    finish = true;
                    break;
                }
            }
            if(finish) {
                continue;
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

    }

    public List<Card> peek() => slots.Map(slot => slot.is_occupied ? slot.peek() : null);
}

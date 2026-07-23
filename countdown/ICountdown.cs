public interface IOnCardPlayed {
    CountdownState on_card_selected(Card card);
}

public abstract class CountdownBase {
    int seconds;
    public CountdownBase(int seconds) {
        this.seconds = seconds;
    }

    public CountdownState tick() {
        seconds -= 1;
        if(seconds == 0) {
            return CountdownState.Finished;
        }
        return CountdownState.Running;
    }

    public abstract string text_display();
}

public enum CountdownState {
    Running,
    Finished,
}
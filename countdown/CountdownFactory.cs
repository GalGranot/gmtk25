using System;
using System.Collections.Generic;

public static class CountdownFactory {
    static int seconds;
    static int required_cards;
    static List<Func<CountdownBase>> factories = new() {
        () => new CdPlayCardsOfSameSuit(seconds, required_cards),
    };

    public static CountdownBase random(int seconds) {
        CountdownFactory.seconds = seconds;
        return random();
    }

    public static CountdownBase random() {
        int randi = Random.in_range(factories.Count);
        return factories[randi]();
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class CountdownSpawner : Node2D {
	[Export] PackedScene countdown_scene;
	GameConfig config;
	Main main;
	List<Countdown> countdowns;
    
	public void _Initialize(Main main) {
		this.main = main;
		main.on_card_selected += on_card_selected;
	}
	
	public override void _Ready() {
		config = GD.Load<GameConfig>("res://config/GameConfig.tres");
		spawn_in_loop();
		tick();
    }

	async Task tick() {
		while(true) {
			await Time.WaitForSeconds(this, 1f);
			for(int i = 0; i < countdowns.Count; i++) {
				Countdown cd = countdowns[i];
				if (cd.tick() is CountdownState.Finished) {
					cd.QueueFree();
					countdowns.RemoveAt(i);
				}
			}
		}
	}

	async Task spawn_in_loop() {
		while(true) {
			spawn_countdown();
			await Time.WaitForSeconds(this, config.countdown_spawn_delay);
		}
	}

	void on_card_selected(Card card) {
		foreach(Countdown cd in countdowns) {
			if (cd is IOnCardPlayed react_to_card_played) {
				react_to_card_played.on_card_selected(card);
			}
		}
	}

	async Task spawn_countdown() {
		CountdownBase countdown_base = CountdownFactory.random();
		Countdown countdown = countdown_scene.Instantiate<Countdown>();
		AddChild(countdown);
		countdown._Initialize(countdown_base);
		await Time.WaitForSeconds(this, 3f);
		countdown.QueueFree();
	}
}

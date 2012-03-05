using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DActionGame
{
	/// <summary>
	/// 各Effectの管理クラス
	/// </summary>
	public class EffectManeger
	{
		public static Game1 game;
		public List<Effect> effects { get; set; }
		private Stage stage;

		public EffectManeger(Stage stage)
		{
			this.stage = stage;
			effects = new List<Effect>();
		}

		private void NeedEffect(Object targetObj, int i)
		{
			if (effects.Count > 0 && effects.Any((x) => x.targetObject == (targetObj))) { }
					else {
				if (stage.activeObjects[i] is Enemy && (targetObj as Enemy).deathEffected) { }
				effects.Add(new Effect(stage, targetObj, i));
			}
		}

		public void Update()
		{
			for (int i = 0; i < stage.activeObjects.Count; i++) {
				if (stage.activeObjects[i] is JumpingEnemy && stage.activeObjects[i].HP <= 0) { }
				if (stage.activeObjects[i] is JumpingEnemy && stage.activeObjects[i].HP <= 0 && stage.activeObjects[i].deathEffected) { }

				if (stage.activeObjects[i].isEffected && !stage.activeObjects[i].hasDeathEffected) {
					if (stage.activeObjects[i].deathEffected) { }
					// NeedEffect(stage.activeObjects[i], i);
					if (effects.Count > 0 && effects.Any((x) => x.targetObject.Equals(stage.activeObjects[i]))) { }
					else {
						if (stage.activeObjects[i] is Enemy && (stage.activeObjects[i] as Enemy).deathEffected) { }
						if (stage.activeObjects[i] is Fuujin) { }
						effects.Add(new Effect(stage, stage.activeObjects[i], i));
					}
				}
			}
			if (effects.Count > 0) {
				for (int j = 0; j < effects.Count; j++) {
					effects[j].DrawObjectEffects(game.spriteBatch);
					if (effects[j].hasEffected) {
						effects[j].targetObject.isEffected = false;
						effects[j].targetObject.damageEffected = false;

						effects.RemoveAt(j);
					}
				}
			}

		}
	}
}

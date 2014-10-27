using System;
using System.Threading.Tasks;
using System.Threading;
using Lina.Customs.Core;

namespace Lina.AnCo.Core
{
	/// <summary>
	/// I game service.
	/// </summary>
	public interface IGameService
	{
		/// <summary>
		/// Starts the game.
		/// </summary>
		/// <param name="actionDone">Action done.</param>
		void StartGame (Action<bool,GameModel> actionDone);

		/// <summary>
		/// Trường hợp 1: Nếu user trả lời đúng thì kiểm tra NotificationType, nếu NotificationType == NotificationType.LevelUp thì thông báo Level Up, sau đó chuyển sang câu tiếp theo, nếu không thì chuyển sang câu hỏi tiếp theo.
		/// Trường hợp 2: Nếu user trả lời sai thì thông báo game over sau đó kiểm tra NotificationType, nếu NotificationType == NotificationType.QuestionUp thì thông báo có thành tích mới.
		/// </summary>
		/// <param name="game">Game.</param>
		/// <param name="actionDone">Action done.</param>
		void Return (GameModel game, Action<bool, NotificationType, GameModel> actionDone);

		/// <summary>
		/// Platform chủ gọi hàm này nếu muốn Stop Game. Dùng khi hết thời gian
		/// </summary>
		/// <param name="game">Game.</param>
		/// <param name="actionDone">Action done.</param>
		void StopGame (GameModel game, Action<NotificationType> actionDone);

		/// <summary>
		/// Pauses the game.
		/// </summary>
		/// <param name="game">Game.</param>
		/// <param name="actionDone">Action done.</param>
		void PauseGame (GameModel game, Action<bool> actionDone);

		/// <summary>
		/// Resumes the game.
		/// </summary>
		/// <param name="actionDone">Action done.</param>
		void ResumeGame (Action<bool, GameModel> actionDone);

		/// <summary>
		/// Plaies the audio.
		/// </summary>
		/// <param name="file">File.</param>
		void PlayAudio (string file);

		/// <summary>
		/// Shake this instance.
		/// </summary>
		void Vibrate ();

		/// <summary>
		/// Changes the state of the sound. ActionDone return current sound state
		/// </summary>
		/// <param name="actionDone">Action done.</param>
		void ChangeSoundState (Action<bool> actionDone);

		/// <summary>
		/// Changes the state of the shake. ActionDone return current shake state
		/// </summary>
		/// <param name="actionDone">Action done.</param>
		void ChangeVibrateState (Action<bool> actionDone);

		/// <summary>
		/// Gets the state of the sound.
		/// </summary>
		/// <returns><c>true</c>, if sound state was gotten, <c>false</c> otherwise.</returns>
		bool GetSoundState ();

		/// <summary>
		/// Gets the state of the shake.
		/// </summary>
		/// <returns><c>true</c>, if shake state was gotten, <c>false</c> otherwise.</returns>
		bool GetVibrateState ();

		/// <summary>
		/// Gets a value indicating whether this <see cref="Lina.AnCo.Core.IGameService"/> vibrate state.
		/// </summary>
		/// <value><c>true</c> if vibrate state; otherwise, <c>false</c>.</value>
		bool VibrateState { get; }
	}

	/// <summary>
	/// Game service.
	/// </summary>
	public class GameService : IGameService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Lina.AnCo.Core.GameService"/> class.
		/// </summary>
		public GameService ()
		{
		}

		/// <summary>
		/// Starts the game.
		/// </summary>
		/// <param name="actionDone">Action done.</param>
		public void StartGame (Action<bool,GameModel> actionDone)
		{
			GameModel game = new GameModel ();
			game.Start ();
			actionDone (true, game);
		}

		/// <summary>
		/// Trường hợp 1: Nếu user trả lời đúng thì kiểm tra NotificationType, nếu NotificationType ==
		/// NotificationType.LevelUp thì thông báo Level Up, sau đó chuyển sang câu tiếp theo, nếu không thì chuyển sang câu
		/// hỏi tiếp theo.
		/// Trường hợp 2: Nếu user trả lời sai thì thông báo game over sau đó kiểm tra NotificationType, nếu NotificationType
		/// == NotificationType.QuestionUp thì thông báo có thành tích mới.
		/// </summary>
		/// <param name="game">Game.</param>
		/// <param name="actionDone">Action done.</param>
		public void Return (GameModel game, Action<bool, NotificationType, GameModel> actionDone)
		{
			NotificationType noti;
			bool isOK = game.CheckQuestion (out noti);
			if (isOK) {
				actionDone (true, noti, game);
			} else {
				actionDone (false, noti, null);
			}
		}

		/// <summary>
		/// Platform chủ gọi hàm này nếu muốn Stop Game. Dùng khi hết thời gian
		/// </summary>
		/// <param name="game">Game.</param>
		/// <param name="actionDone">Action done.</param>
		public void StopGame (GameModel game, Action<NotificationType> actionDone)
		{
			NotificationType noti;
			game.CheckQuestion (out noti);
			actionDone (noti);
		}

		private static GameModel CurrentGame;

		/// <summary>
		/// Pauses the game.
		/// </summary>
		/// <param name="game">Game.</param>
		/// <param name="actionDone">Action done.</param>
		public void PauseGame (GameModel game, Action<bool> actionDone)
		{
			CurrentGame = game;
			actionDone (true);
		}

		/// <summary>
		/// Resumes the game.
		/// </summary>
		/// <param name="actionDone">Action done.</param>
		public void ResumeGame (Action<bool, GameModel> actionDone)
		{
			CurrentGame.Question = new QuestionModel ();
			actionDone (true, CurrentGame);
		}

		/// <summary>
		/// Plaies the audio.
		/// </summary>
		/// <param name="file">File.</param>
		public void PlayAudio (string file)
		{
			if (!CTGlobalService.Setting.GetValueOrDefault<bool> (SettingKey.Sound.ToString (), false)) {
				CTGlobalService.Platform.StreamAudio (file, delegate(bool isComplete) {

				});
			}
		}

		/// <summary>
		/// Gets the state of the shake.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public void Vibrate ()
		{
			if (!CTGlobalService.Setting.GetValueOrDefault<bool> (SettingKey.Vibrate.ToString (), false)) {
				CTGlobalService.Platform.Vibrate ();
			}
		}

		/// <summary>
		/// Changes the state of the sound. ActionDone return current sound state
		/// </summary>
		/// <param name="actionDone">Action done.</param>
		public void ChangeSoundState (Action<bool> actionDone)
		{
			bool state = CTGlobalService.Setting.GetValueOrDefault<bool> (SettingKey.Sound.ToString (), false);
			CTGlobalService.Setting.AddOrUpdateValue (SettingKey.Sound.ToString (), !state);
			CTGlobalService.Setting.Save ();
			actionDone (!state);
		}

		/// <summary>
		/// Changes the state of the shake. ActionDone return current shake state
		/// </summary>
		/// <param name="actionDone">Action done.</param>
		public void ChangeVibrateState (Action<bool> actionDone)
		{
			bool state = CTGlobalService.Setting.GetValueOrDefault<bool> (SettingKey.Vibrate.ToString (), false);
			CTGlobalService.Setting.AddOrUpdateValue (SettingKey.Vibrate.ToString (), !state);
			CTGlobalService.Setting.Save ();
			if (state) {
				CTGlobalService.Platform.Vibrate ();
			}
			actionDone (!state);
		}

		/// <summary>
		/// Gets the state of the sound.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool GetSoundState ()
		{
			return CTGlobalService.Setting.GetValueOrDefault<bool> (SettingKey.Sound.ToString (), false);
		}

		/// <summary>
		/// Gets the state of the shake.
		/// </summary>
		/// <returns><c>true</c>, if shake state was gotten, <c>false</c> otherwise.</returns>
		public bool GetVibrateState ()
		{
			return CTGlobalService.Setting.GetValueOrDefault<bool> (SettingKey.Vibrate.ToString (), false);
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Lina.AnCo.Core.GameService"/> vibrate state.
		/// </summary>
		/// <value><c>true</c> if vibrate state; otherwise, <c>false</c>.</value>
		public bool VibrateState { get { return GetVibrateState (); } }
	}
}


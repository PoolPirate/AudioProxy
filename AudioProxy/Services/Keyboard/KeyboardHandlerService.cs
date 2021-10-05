using AudioProxy.Models;
using Common.Services;
using System.Threading.Tasks;

namespace AudioProxy.Services
{
    public sealed class KeyboardHandlerService : Service
    {
        [Inject] private readonly KeyboardHookService KeyboardHookService;
        [Inject] private readonly ProfileService ProfileService;
        [Inject] private readonly AudioOutputService AudioOutputService;
        [Inject] private readonly HotkeyService HotkeyService;

        protected override ValueTask InitializeAsync()
        {
            KeyboardHookService.OnKeyPressed += HandleKeyDownAsync;
            return default;
        }

        private async Task HandleKeyDownAsync(Keys key)
        {
            var pressedKeys = KeyboardHookService.GetPressedKeys();

            if (HotkeyService.TryGetPressedHotkey(pressedKeys, out var hotkey))
            {
                switch (hotkey)
                {
                    case Hotkey.Next:
                        ProfileService.SelectNextProfile();
                        break;
                    case Hotkey.Last:
                        ProfileService.SelectLastProfile();
                        break;
                    case Hotkey.Stop:
                        AudioOutputService.StopAllSounds();
                        break;
                }
            }

            if (ProfileService.TryGetPressedProfileSounds(pressedKeys, out var profileSounds))
            {
                foreach (var profileSound in profileSounds)
                {
                    await AudioOutputService.PlaySoundAsync(profileSound.SoundId);
                }
            }
        }
    }
}
